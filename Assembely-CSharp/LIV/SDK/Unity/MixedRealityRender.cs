using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace LIV.SDK.Unity
{
	[AddComponentMenu("LIV/MixedRealityRender")]
	public class MixedRealityRender : MonoBehaviour
	{
		private bool _isSetup;

		private LIV _liv;

		private Camera _mrCamera;

		private Transform _hmd;

		private GameObject _clipQuad;

		private Material _clipMaterial;

		private Material _blitMaterial;

		private RenderTexture _compositeTexture;

		private RenderTexture _foregroundTexture;

		private RenderTexture _backgroundTexture;

		public void Setup(LIV liv)
		{
			_liv = liv;
			_mrCamera = GetComponent<Camera>();
			_mrCamera.rect = new Rect(0f, 0f, 1f, 1f);
			_mrCamera.depth = float.MaxValue;
			_mrCamera.stereoTargetEye = StereoTargetEyeMask.None;
			_mrCamera.useOcclusionCulling = false;
			_mrCamera.enabled = false;
			_clipMaterial = new Material(Shader.Find("Custom/LIV_ClearBackground"));
			_blitMaterial = new Material(Shader.Find("Custom/LIV_Blit"));
			CreateClipQuad();
			_isSetup = true;
		}

		private void CreateClipQuad()
		{
			_clipQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
			_clipQuad.name = "ClipQuad";
			Object.Destroy(_clipQuad.GetComponent<MeshCollider>());
			_clipQuad.transform.parent = base.transform;
			MeshRenderer component = _clipQuad.GetComponent<MeshRenderer>();
			component.material = _clipMaterial;
			component.shadowCastingMode = ShadowCastingMode.Off;
			component.receiveShadows = false;
			component.lightProbeUsage = LightProbeUsage.Off;
			component.reflectionProbeUsage = ReflectionProbeUsage.Off;
			Transform transform = _clipQuad.transform;
			transform.localScale = new Vector3(1000f, 1000f, 1f);
			transform.localRotation = Quaternion.identity;
			_clipQuad.SetActive(value: false);
		}

		private void UpdateCamera()
		{
			_mrCamera.fieldOfView = Calibration.FieldOfVision;
			_mrCamera.nearClipPlane = Calibration.NearClip;
			_mrCamera.farClipPlane = Calibration.FarClip;
			_mrCamera.cullingMask = _liv.SpectatorLayerMask;
			_hmd = _liv.HMDCamera.transform;
			base.transform.localPosition = Calibration.PositionOffset;
			base.transform.localRotation = Calibration.RotationOffset;
			base.transform.localScale = Vector3.one;
		}

		private void UpdateTextures()
		{
			int textureWidth = SharedTextureProtocol.TextureWidth;
			int textureHeight = SharedTextureProtocol.TextureHeight;
			int num = SharedTextureProtocol.TextureHeight / 2;
			if (_compositeTexture == null || _compositeTexture.width != textureWidth || _compositeTexture.height != textureHeight)
			{
				_compositeTexture = new RenderTexture(textureWidth, textureHeight, 24, RenderTextureFormat.ARGB32);
				_compositeTexture.antiAliasing = 1;
			}
			if (_foregroundTexture == null || _foregroundTexture.width != textureWidth || _foregroundTexture.height != num)
			{
				_foregroundTexture = new RenderTexture(textureWidth, num, 24, RenderTextureFormat.ARGB32)
				{
					antiAliasing = 1,
					wrapMode = TextureWrapMode.Clamp,
					useMipMap = false,
					anisoLevel = 0
				};
			}
			if (_backgroundTexture == null || _backgroundTexture.width != textureWidth || _backgroundTexture.height != num)
			{
				_backgroundTexture = new RenderTexture(textureWidth, num, 24, RenderTextureFormat.ARGB32)
				{
					antiAliasing = 1,
					wrapMode = TextureWrapMode.Clamp,
					useMipMap = false,
					anisoLevel = 0
				};
			}
		}

		public float GetDistanceToHMD()
		{
			if (_hmd == null)
			{
				return Calibration.NearClip;
			}
			Transform transform = _mrCamera.transform;
			Vector3 vector = new Vector3(transform.forward.x, 0f, transform.forward.z);
			Vector3 normalized = vector.normalized;
			Vector3 position = _hmd.position;
			Vector3 vector2 = new Vector3(_hmd.forward.x, 0f, _hmd.forward.z);
			Vector3 inPoint = position + vector2.normalized * Calibration.HMDOffset;
			return 0f - new Plane(normalized, inPoint).GetDistanceToPoint(transform.position);
		}

		private void OrientClipQuad()
		{
			float num = Mathf.Clamp(GetDistanceToHMD() + Calibration.NearOffset, Calibration.NearClip, Calibration.FarClip);
			Transform parent = _clipQuad.transform.parent;
			_clipQuad.transform.position = parent.position + parent.forward * num;
			_clipQuad.transform.LookAt(new Vector3(_clipQuad.transform.parent.position.x, _clipQuad.transform.position.y, _clipQuad.transform.parent.position.z));
		}

		private void RenderNear()
		{
			MonoBehaviour[] array = null;
			bool[] array2 = null;
			if (_liv.DisableStandardAssets)
			{
				array = _mrCamera.gameObject.GetComponents<MonoBehaviour>();
				array2 = new bool[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					MonoBehaviour monoBehaviour = array[i];
					if (monoBehaviour.enabled && monoBehaviour.GetType().ToString().StartsWith("UnityStandardAssets."))
					{
						monoBehaviour.enabled = false;
						array2[i] = true;
					}
				}
			}
			CameraClearFlags clearFlags = _mrCamera.clearFlags;
			Color backgroundColor = _mrCamera.backgroundColor;
			_mrCamera.clearFlags = CameraClearFlags.Color;
			_mrCamera.backgroundColor = Color.clear;
			_clipQuad.SetActive(value: true);
			_mrCamera.targetTexture = _foregroundTexture;
			_foregroundTexture.DiscardContents(discardColor: true, discardDepth: true);
			_mrCamera.Render();
			_clipQuad.SetActive(value: false);
			_mrCamera.clearFlags = clearFlags;
			_mrCamera.backgroundColor = backgroundColor;
			if (array == null)
			{
				return;
			}
			for (int j = 0; j < array.Length; j++)
			{
				if (array2[j])
				{
					array[j].enabled = true;
				}
			}
		}

		private void RenderFar()
		{
			_mrCamera.targetTexture = _backgroundTexture;
			_backgroundTexture.DiscardContents(discardColor: true, discardDepth: true);
			_mrCamera.Render();
		}

		private void Composite()
		{
			_compositeTexture.DiscardContents(discardColor: true, discardDepth: true);
			_blitMaterial.SetTexture("_NearTex", _foregroundTexture);
			_blitMaterial.SetTexture("_FarTex", _backgroundTexture);
			Graphics.Blit(null, _compositeTexture, _blitMaterial);
			SharedTextureProtocol.SetOutputTexture(_compositeTexture);
		}

		private void OnEnable()
		{
			StartCoroutine(RenderLoop());
		}

		private IEnumerator RenderLoop()
		{
			while (Application.isPlaying && base.enabled)
			{
				yield return new WaitForEndOfFrame();
				if (_isSetup && SharedTextureProtocol.IsActive)
				{
					UpdateCamera();
					UpdateTextures();
					OrientClipQuad();
					RenderNear();
					RenderFar();
					Composite();
				}
			}
		}
	}
}
