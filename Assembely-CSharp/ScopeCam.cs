using System;
using FistVR;
using UnityEngine;
using UnityEngine.VR;

public class ScopeCam : MonoBehaviour
{
	public Material PostMaterial;

	public GameObject Reticule;

	public Camera ScopeCamera;

	public float Magnification = 5f;

	public int Resolution = 512;

	public float AngleBlurStrength = 0.5f;

	public float CutoffSoftness = 0.05f;

	public float AngularOccludeSensitivity = 0.5f;

	public float ReticuleScale = 1f;

	public bool MagnificationEnabledAtStart;

	[Range(0f, 1f)]
	public float LensSpaceDistortion = 0.075f;

	[Range(0f, 5f)]
	public float LensChromaticDistortion = 0.075f;

	private Renderer m_renderer;

	private MaterialPropertyBlock m_block;

	private Vector3 m_reticuleSize = new Vector3(0.1f, 0.1f, 0.1f);

	private RenderTexture m_mainTex;

	private RenderTexture m_blurTex;

	private bool m_magnifcationEnabled;

	public bool MagnificationEnabled
	{
		get
		{
			return m_magnifcationEnabled;
		}
		set
		{
			if (m_magnifcationEnabled == value)
			{
				return;
			}
			m_magnifcationEnabled = value;
			if (m_magnifcationEnabled)
			{
				if (Reticule != null)
				{
					Reticule.SetActive(value: true);
				}
			}
			else if (Reticule != null)
			{
				Reticule.SetActive(value: false);
			}
			if (!m_magnifcationEnabled && m_mainTex != null)
			{
				ClearToBlack(m_mainTex);
				ClearToBlack(m_blurTex);
			}
		}
	}

	private void ClearToBlack(RenderTexture tex)
	{
		if (tex != null)
		{
			RenderTexture.active = tex;
			GL.Clear(clearDepth: false, clearColor: true, Color.black);
			RenderTexture.active = null;
		}
	}

	private void OnEnable()
	{
		m_mainTex = new RenderTexture(Resolution, Resolution, 24, RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.sRGB)
		{
			wrapMode = TextureWrapMode.Clamp
		};
		m_blurTex = new RenderTexture(Resolution, Resolution, 0, m_mainTex.format, RenderTextureReadWrite.sRGB);
		m_mainTex.Create();
		m_blurTex.Create();
		ScopeCamera.enabled = false;
		ScopeCamera.allowHDR = true;
		ScopeCamera.allowMSAA = false;
		m_renderer = GetComponent<Renderer>();
		m_block = new MaterialPropertyBlock();
		if (Reticule != null)
		{
			m_reticuleSize = Reticule.transform.localScale;
		}
		MagnificationEnabled = MagnificationEnabledAtStart;
	}

	private void OnWillRenderObject()
	{
		if (Camera.current == ScopeCamera || !MagnificationEnabled)
		{
			return;
		}
		Vector3 min = m_renderer.bounds.min;
		Vector3 max = m_renderer.bounds.max;
		min = m_renderer.transform.position - Camera.current.transform.right * m_renderer.transform.localScale.x - Camera.current.transform.up * m_renderer.transform.localScale.x;
		max = m_renderer.transform.position + Camera.current.transform.right * m_renderer.transform.localScale.x + Camera.current.transform.up * m_renderer.transform.localScale.x;
		Vector3 vector = Camera.current.WorldToViewportPoint(min);
		Vector3 vector2 = Camera.current.WorldToViewportPoint(max);
		float num = Mathf.Abs(Mathf.Clamp01(vector2.x) - Mathf.Clamp01(vector.x));
		float num2 = Mathf.Abs(Mathf.Clamp01(vector2.y) - Mathf.Clamp01(vector.y));
		float num3 = Mathf.Sqrt(num * num + num2 * num2);
		ScopeCamera.fieldOfView = Camera.current.fieldOfView * num3 / (Magnification * (float)Math.PI * 0.5f);
		Vector3 vector3;
		if (!Camera.current.stereoEnabled)
		{
			vector3 = Camera.current.transform.position;
			m_block.SetFloat("_EyeIndex", 0f);
		}
		else
		{
			Vector3 position = GM.CurrentPlayerBody.Head.position;
			Vector3 vector4 = position + GM.CurrentPlayerBody.Head.right * -0.022f;
			Vector3 vector5 = position + GM.CurrentPlayerBody.Head.right * 0.022f;
			Vector3 to = vector4 - base.transform.position;
			Vector3 to2 = vector5 - base.transform.position;
			float num4 = Vector3.Angle(base.transform.forward, to);
			float num5 = Vector3.Angle(base.transform.forward, to2);
			VRNode vRNode = ((!(num5 < num4)) ? VRNode.RightEye : VRNode.LeftEye);
			vector3 = Quaternion.Inverse(InputTracking.GetLocalRotation(vRNode)) * InputTracking.GetLocalPosition(vRNode);
			Matrix4x4 cameraToWorldMatrix = Camera.current.cameraToWorldMatrix;
			Vector3 v = Quaternion.Inverse(InputTracking.GetLocalRotation(VRNode.Head)) * InputTracking.GetLocalPosition(VRNode.Head);
			vector3 = cameraToWorldMatrix.MultiplyPoint(vector3) + (Camera.current.transform.position - cameraToWorldMatrix.MultiplyPoint(v));
			m_block.SetFloat("_EyeIndex", (vRNode != 0) ? 1 : 0);
		}
		if ((ScopeCamera.transform.position - vector3).magnitude >= Mathf.Epsilon)
		{
			ScopeCamera.targetTexture = m_mainTex;
			if (Reticule != null)
			{
				Transform transform = Reticule.transform;
				transform.position = ScopeCamera.transform.position + base.transform.forward * 0.1f;
				transform.rotation = base.transform.rotation;
			}
			ScopeCamera.Render();
			PostMaterial.SetVector("_CamPos", vector3);
			PostMaterial.SetMatrix("_ScopeVisualToWorld", base.transform.localToWorldMatrix);
			PostMaterial.SetVector("_Forward", ScopeCamera.transform.forward);
			PostMaterial.SetVector("_Offset", Vector2.right * AngleBlurStrength * 0.01f);
			Graphics.Blit(m_mainTex, m_blurTex, PostMaterial);
			PostMaterial.SetVector("_Offset", Vector2.up * AngleBlurStrength * 0.01f);
			Graphics.Blit(m_blurTex, m_mainTex, PostMaterial);
		}
		m_block.SetVector("_TubeCenter", base.transform.position);
		m_block.SetVector("_TubeForward", base.transform.forward);
		m_block.SetFloat("_TubeRadius", base.transform.localScale.x);
		float magnitude = (ScopeCamera.transform.position - base.transform.position).magnitude;
		magnitude *= Mathf.Lerp(1f, Magnification, AngularOccludeSensitivity);
		m_block.SetFloat("_TubeLength", magnitude);
		m_block.SetFloat("_CutoffSoftness", CutoffSoftness);
		m_block.SetFloat("_LensDistortion", 1f - LensSpaceDistortion);
		m_block.SetFloat("_Chroma", LensChromaticDistortion);
		m_block.SetTexture("_MainTex0", m_mainTex);
		m_renderer.SetPropertyBlock(m_block);
	}

	public void PointTowards(Vector3 p)
	{
		Vector3 vector = p - ScopeCamera.transform.position;
		vector = Vector3.ProjectOnPlane(vector, base.transform.right);
		ScopeCamera.transform.rotation = Quaternion.LookRotation(vector, base.transform.up);
	}

	private void RenderScopeTex(VRNode node, RenderTexture tex)
	{
		Vector3 v = Quaternion.Inverse(InputTracking.GetLocalRotation(node)) * InputTracking.GetLocalPosition(node);
		Matrix4x4 cameraToWorldMatrix = Camera.current.cameraToWorldMatrix;
		Vector3 v2 = Quaternion.Inverse(InputTracking.GetLocalRotation(VRNode.Head)) * InputTracking.GetLocalPosition(VRNode.Head);
		v = cameraToWorldMatrix.MultiplyPoint(v) + (Camera.current.transform.position - cameraToWorldMatrix.MultiplyPoint(v2));
		Vector3 vector = ScopeCamera.transform.position - v;
		if (!(vector.magnitude < Mathf.Epsilon))
		{
			Quaternion quaternion = Quaternion.LookRotation(vector.normalized, base.transform.up);
			ScopeCamera.targetTexture = tex;
			if (Reticule != null)
			{
				Transform transform = Reticule.transform;
				transform.position = ScopeCamera.transform.position + base.transform.forward * 0.1f;
				transform.rotation = base.transform.rotation;
			}
			ScopeCamera.Render();
			PostMaterial.SetVector("_CamPos", v);
			PostMaterial.SetMatrix("_ScopeVisualToWorld", base.transform.localToWorldMatrix);
			PostMaterial.SetVector("_Forward", ScopeCamera.transform.forward);
			PostMaterial.SetVector("_Offset", Vector2.right * AngleBlurStrength * 0.01f);
			Graphics.Blit(tex, m_blurTex, PostMaterial);
			PostMaterial.SetVector("_Offset", Vector2.up * AngleBlurStrength * 0.01f);
			Graphics.Blit(m_blurTex, tex, PostMaterial);
		}
	}

	private void OnDisable()
	{
		UnityEngine.Object.DestroyImmediate(m_mainTex);
		UnityEngine.Object.DestroyImmediate(m_blurTex);
	}
}
