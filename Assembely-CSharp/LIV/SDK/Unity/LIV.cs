using UnityEngine;

namespace LIV.SDK.Unity
{
	[AddComponentMenu("LIV/LIV")]
	public class LIV : MonoBehaviour
	{
		[Tooltip("If unpopulated, we'll use the parent transform.")]
		public Transform TrackedSpaceOrigin;

		[Space]
		public Camera HMDCamera;

		[Space]
		public bool DisableStandardAssets;

		[Space]
		public LayerMask SpectatorLayerMask = -1;

		protected bool WasActive;

		private SharedTextureProtocol _sharedTextureProtocol;

		private ExternalCamera _externalCamera;

		private MixedRealityRender _mixedRealityRender;

		private bool _wasSteamVRExternalCameraActive;

		public Transform Origin => (!(TrackedSpaceOrigin == null)) ? TrackedSpaceOrigin : base.transform.parent;

		private void OnEnable()
		{
			if (!SteamVRCompatibility.IsAvailable)
			{
				Debug.LogError("LIV: SteamVR asset not found. In Unity versions earlier than 2017.2, SteamVR is required.");
				base.enabled = false;
				return;
			}
			if (HMDCamera == null)
			{
				Debug.LogError("LIV: HMD Camera is a required parameter!");
				base.enabled = false;
				return;
			}
			if ((int)SpectatorLayerMask == 0)
			{
				Debug.LogWarning("LIV: The spectator layer mask is set to not show anything. Is this right?");
			}
			Debug.Log("LIV: Ready! Waiting for compositor.");
		}

		private void OnDisable()
		{
			Debug.Log("LIV: Disabled, cleaning up.");
			if (WasActive)
			{
				OnCompositorDeactivated();
			}
		}

		private void Update()
		{
			if (SharedTextureProtocol.IsActive && !WasActive)
			{
				OnCompositorActivated();
			}
			if (!SharedTextureProtocol.IsActive && WasActive)
			{
				OnCompositorDeactivated();
			}
		}

		private void OnCompositorActivated()
		{
			WasActive = true;
			Debug.Log("LIV: Compositor connected, setting up MR!");
			CreateSharedTextureProtocol();
			CreateExternalCamera();
			CreateMixedRealityRender();
			if (SteamVRCompatibility.IsAvailable)
			{
				Component component = GetComponent(SteamVRCompatibility.SteamVRExternalCamera);
				if (component != null)
				{
					_wasSteamVRExternalCameraActive = component.gameObject.activeInHierarchy;
					component.gameObject.SetActive(value: false);
				}
			}
		}

		private void OnCompositorDeactivated()
		{
			WasActive = false;
			Debug.Log("LIV: Compositor disconnected, cleaning up.");
			DestroySharedTextureProtocol();
			DestroyMixedRealityRender();
			DestroyExternalCamera();
			if (SteamVRCompatibility.IsAvailable)
			{
				Component component = GetComponent(SteamVRCompatibility.SteamVRExternalCamera);
				if (component != null)
				{
					component.gameObject.SetActive(_wasSteamVRExternalCameraActive);
				}
			}
		}

		private void CreateSharedTextureProtocol()
		{
			_sharedTextureProtocol = base.gameObject.AddComponent<SharedTextureProtocol>();
		}

		private void DestroySharedTextureProtocol()
		{
			if (_sharedTextureProtocol != null)
			{
				Object.Destroy(_sharedTextureProtocol);
				_sharedTextureProtocol = null;
			}
		}

		private void CreateExternalCamera()
		{
			GameObject gameObject = new GameObject("LIV Camera Reference");
			gameObject.transform.parent = Origin;
			_externalCamera = gameObject.AddComponent<ExternalCamera>();
		}

		private void DestroyExternalCamera()
		{
			if (_externalCamera != null)
			{
				Object.Destroy(_externalCamera.gameObject);
				_externalCamera = null;
			}
		}

		private void CreateMixedRealityRender()
		{
			HMDCamera.enabled = false;
			HMDCamera.gameObject.SetActive(value: false);
			GameObject gameObject = Object.Instantiate(HMDCamera.gameObject);
			HMDCamera.gameObject.SetActive(value: true);
			HMDCamera.enabled = true;
			gameObject.name = "LIV Camera";
			while (gameObject.transform.childCount > 0)
			{
				Object.DestroyImmediate(gameObject.transform.GetChild(0).gameObject);
			}
			Object.DestroyImmediate(gameObject.GetComponent("AudioListener"));
			Object.DestroyImmediate(gameObject.GetComponent("MeshCollider"));
			if (SteamVRCompatibility.IsAvailable)
			{
				Object.DestroyImmediate(gameObject.GetComponent(SteamVRCompatibility.SteamVRCamera));
				Object.DestroyImmediate(gameObject.GetComponent(SteamVRCompatibility.SteamVRFade));
			}
			_mixedRealityRender = gameObject.AddComponent<MixedRealityRender>();
			gameObject.transform.parent = _externalCamera.transform;
			gameObject.SetActive(value: true);
			_mixedRealityRender.Setup(this);
		}

		private void DestroyMixedRealityRender()
		{
			if (_mixedRealityRender != null)
			{
				Object.Destroy(_mixedRealityRender.gameObject);
				_mixedRealityRender = null;
			}
		}
	}
}
