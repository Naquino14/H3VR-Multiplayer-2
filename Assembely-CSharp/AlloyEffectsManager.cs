using System;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Alloy/Alloy Effects Manager")]
public class AlloyEffectsManager : MonoBehaviour
{
	[Serializable]
	public struct SkinSettingsData
	{
		public bool Enabled;

		public Texture2D Lut;

		[Range(0f, 1f)]
		public float Weight;

		[Range(0.01f, 1f)]
		public float MaskCutoff;

		[Range(0f, 1f)]
		public float Bias;

		[Range(0f, 1f)]
		public float Scale;

		[Range(0f, 1f)]
		public float BumpBlur;

		public Vector3 Absorption;

		public Vector3 AoColorBleed;
	}

	[Serializable]
	public struct TransmissionSettingsData
	{
		public bool Enabled;

		[Range(0f, 1f)]
		public float Weight;

		[Range(0f, 1f)]
		public float ShadowWeight;

		[Range(0f, 1f)]
		[Tooltip("Amount that the transmission is distorted by surface normals.")]
		public float BumpDistortion;

		[MinValue(1f)]
		public float Falloff;
	}

	private const float c_blurWdith = 0.15f;

	private const float c_blurDepthDifferenceMultiplier = 100f;

	private const string c_copyTransmissionBufferName = "AlloyCopyTransmission";

	private const string c_blurNormalsBufferName = "AlloyBlurNormals";

	private const CameraEvent c_copyTransmissionEvent = CameraEvent.AfterGBuffer;

	private const CameraEvent c_blurNormalsEvent = CameraEvent.BeforeLighting;

	public SkinSettingsData SkinSettings = new SkinSettingsData
	{
		Enabled = true,
		Weight = 1f,
		MaskCutoff = 0.1f,
		Bias = 0f,
		Scale = 1f,
		BumpBlur = 0.7f,
		Absorption = new Vector3(-8f, -40f, -64f),
		AoColorBleed = new Vector3(0.4f, 0.15f, 0.13f)
	};

	public TransmissionSettingsData TransmissionSettings = new TransmissionSettingsData
	{
		Enabled = true,
		Weight = 1f,
		ShadowWeight = 0.5f,
		BumpDistortion = 0.05f,
		Falloff = 1f
	};

	[HideInInspector]
	public Texture2D SkinLut;

	[HideInInspector]
	public Shader TransmissionBlitShader;

	[HideInInspector]
	public Shader BlurNormalsShader;

	private Material m_deferredTransmissionBlitMaterial;

	private Material m_deferredBlurredNormalsMaterial;

	private Camera m_camera;

	private bool m_isTransmissionEnabled;

	private bool m_isScatteringEnabled;

	private CommandBuffer m_copyTransmission;

	private CommandBuffer m_renderBlurredNormals;

	private void Awake()
	{
		m_camera = GetComponent<Camera>();
	}

	private void Reset()
	{
		ResetCommandBuffers();
	}

	private void OnEnable()
	{
		ResetCommandBuffers();
	}

	private void OnDisable()
	{
		DestroyCommandBuffers();
	}

	private void OnDestroy()
	{
		DestroyCommandBuffers();
	}

	public void Refresh()
	{
		bool enabled = SkinSettings.Enabled;
		bool flag = TransmissionSettings.Enabled || enabled;
		if (m_isTransmissionEnabled == flag && m_isScatteringEnabled == enabled)
		{
			RefreshProperties();
		}
		else
		{
			ResetCommandBuffers();
		}
	}

	private void RefreshProperties()
	{
		if (m_isTransmissionEnabled || m_isScatteringEnabled)
		{
			float x = ((!m_isTransmissionEnabled) ? 0f : Mathf.GammaToLinearSpace(TransmissionSettings.Weight));
			Shader.SetGlobalVector("_DeferredTransmissionParams", new Vector4(x, TransmissionSettings.Falloff, TransmissionSettings.BumpDistortion, TransmissionSettings.ShadowWeight));
			if (m_isScatteringEnabled)
			{
				float num = 1f / Mathf.Tan((float)Math.PI / 360f * m_camera.fieldOfView);
				float x2 = 0.15f * num;
				float y = 100f * num;
				Shader.SetGlobalVector("_DeferredBlurredNormalsParams", new Vector2(x2, y));
				Vector3 absorption = SkinSettings.Absorption;
				Vector3 aoColorBleed = SkinSettings.AoColorBleed;
				Shader.SetGlobalTexture("_DeferredSkinLut", SkinSettings.Lut);
				Shader.SetGlobalVector("_DeferredSkinParams", new Vector3(SkinSettings.Weight, 1f / SkinSettings.MaskCutoff, SkinSettings.BumpBlur));
				Shader.SetGlobalVector("_DeferredSkinTransmissionAbsorption", new Vector4(absorption.x, absorption.y, absorption.z, SkinSettings.Bias));
				Shader.SetGlobalVector("_DeferredSkinColorBleedAoWeights", new Vector4(aoColorBleed.x, aoColorBleed.y, aoColorBleed.z, SkinSettings.Scale));
			}
		}
	}

	private void ResetCommandBuffers()
	{
		m_isScatteringEnabled = SkinSettings.Enabled;
		m_isTransmissionEnabled = TransmissionSettings.Enabled || m_isScatteringEnabled;
		if (SkinSettings.Lut == null)
		{
			SkinSettings.Lut = SkinLut;
		}
		DestroyCommandBuffers();
		if ((m_isTransmissionEnabled || m_isScatteringEnabled) && m_camera != null && TransmissionBlitShader != null)
		{
			int num = Shader.PropertyToID("_DeferredPlusBuffer");
			m_deferredTransmissionBlitMaterial = new Material(TransmissionBlitShader);
			m_deferredTransmissionBlitMaterial.hideFlags = HideFlags.HideAndDontSave;
			m_copyTransmission = new CommandBuffer();
			m_copyTransmission.name = "AlloyCopyTransmission";
			if (!m_isScatteringEnabled)
			{
				m_copyTransmission.GetTemporaryRT(num, -1, -1, 0, FilterMode.Point, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
				m_copyTransmission.Blit(BuiltinRenderTextureType.CameraTarget, num, m_deferredTransmissionBlitMaterial);
				m_copyTransmission.ReleaseTemporaryRT(num);
			}
			else if (BlurNormalsShader != null)
			{
				int width = m_camera.pixelWidth / 2;
				int height = m_camera.pixelHeight / 2;
				int num2 = Shader.PropertyToID("_DeferredBlurredNormalPingBuffer");
				int num3 = Shader.PropertyToID("_DeferredBlurredNormalPongBuffer");
				m_copyTransmission.SetGlobalTexture("_DeferredTransmissionBuffer", BuiltinRenderTextureType.CameraTarget);
				m_deferredBlurredNormalsMaterial = new Material(BlurNormalsShader);
				m_deferredBlurredNormalsMaterial.hideFlags = HideFlags.HideAndDontSave;
				m_renderBlurredNormals = new CommandBuffer();
				m_renderBlurredNormals.name = "AlloyBlurNormals";
				m_renderBlurredNormals.GetTemporaryRT(num, -1, -1, 0, FilterMode.Point, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
				m_renderBlurredNormals.GetTemporaryRT(num2, width, height, 0, FilterMode.Point, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
				m_renderBlurredNormals.GetTemporaryRT(num3, width, height, 0, FilterMode.Point, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
				m_renderBlurredNormals.Blit(BuiltinRenderTextureType.GBuffer2, num2, m_deferredBlurredNormalsMaterial, 0);
				m_renderBlurredNormals.Blit(num2, num3, m_deferredBlurredNormalsMaterial, 1);
				m_renderBlurredNormals.Blit(num3, num2, m_deferredBlurredNormalsMaterial, 2);
				m_renderBlurredNormals.Blit(num2, num, m_deferredBlurredNormalsMaterial, 3);
				m_renderBlurredNormals.ReleaseTemporaryRT(num);
				m_renderBlurredNormals.ReleaseTemporaryRT(num2);
				m_renderBlurredNormals.ReleaseTemporaryRT(num3);
				m_camera.depthTextureMode |= DepthTextureMode.Depth;
				m_camera.AddCommandBuffer(CameraEvent.BeforeLighting, m_renderBlurredNormals);
			}
			m_camera.AddCommandBuffer(CameraEvent.AfterGBuffer, m_copyTransmission);
		}
		RefreshProperties();
	}

	private void DestroyCommandBuffers()
	{
		if (m_copyTransmission != null)
		{
			m_camera.RemoveCommandBuffer(CameraEvent.AfterGBuffer, m_copyTransmission);
		}
		if (m_renderBlurredNormals != null)
		{
			m_camera.RemoveCommandBuffer(CameraEvent.BeforeLighting, m_renderBlurredNormals);
		}
		if (m_deferredTransmissionBlitMaterial != null)
		{
			UnityEngine.Object.DestroyImmediate(m_deferredTransmissionBlitMaterial);
		}
		if (m_deferredBlurredNormalsMaterial != null)
		{
			UnityEngine.Object.DestroyImmediate(m_deferredBlurredNormalsMaterial);
		}
		m_copyTransmission = null;
		m_renderBlurredNormals = null;
		m_deferredTransmissionBlitMaterial = null;
		m_deferredBlurredNormalsMaterial = null;
	}
}
