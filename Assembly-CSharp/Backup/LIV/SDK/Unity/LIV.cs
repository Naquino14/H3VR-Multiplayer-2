// Decompiled with JetBrains decompiler
// Type: LIV.SDK.Unity.LIV
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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
    public LayerMask SpectatorLayerMask = (LayerMask) -1;
    protected bool WasActive;
    private SharedTextureProtocol _sharedTextureProtocol;
    private ExternalCamera _externalCamera;
    private MixedRealityRender _mixedRealityRender;
    private bool _wasSteamVRExternalCameraActive;

    public Transform Origin => (Object) this.TrackedSpaceOrigin == (Object) null ? this.transform.parent : this.TrackedSpaceOrigin;

    private void OnEnable()
    {
      if (!SteamVRCompatibility.IsAvailable)
      {
        Debug.LogError((object) "LIV: SteamVR asset not found. In Unity versions earlier than 2017.2, SteamVR is required.");
        this.enabled = false;
      }
      else if ((Object) this.HMDCamera == (Object) null)
      {
        Debug.LogError((object) "LIV: HMD Camera is a required parameter!");
        this.enabled = false;
      }
      else
      {
        if ((int) this.SpectatorLayerMask == 0)
          Debug.LogWarning((object) "LIV: The spectator layer mask is set to not show anything. Is this right?");
        Debug.Log((object) "LIV: Ready! Waiting for compositor.");
      }
    }

    private void OnDisable()
    {
      Debug.Log((object) "LIV: Disabled, cleaning up.");
      if (!this.WasActive)
        return;
      this.OnCompositorDeactivated();
    }

    private void Update()
    {
      if (SharedTextureProtocol.IsActive && !this.WasActive)
        this.OnCompositorActivated();
      if (SharedTextureProtocol.IsActive || !this.WasActive)
        return;
      this.OnCompositorDeactivated();
    }

    private void OnCompositorActivated()
    {
      this.WasActive = true;
      Debug.Log((object) "LIV: Compositor connected, setting up MR!");
      this.CreateSharedTextureProtocol();
      this.CreateExternalCamera();
      this.CreateMixedRealityRender();
      if (!SteamVRCompatibility.IsAvailable)
        return;
      Component component = this.GetComponent(SteamVRCompatibility.SteamVRExternalCamera);
      if (!((Object) component != (Object) null))
        return;
      this._wasSteamVRExternalCameraActive = component.gameObject.activeInHierarchy;
      component.gameObject.SetActive(false);
    }

    private void OnCompositorDeactivated()
    {
      this.WasActive = false;
      Debug.Log((object) "LIV: Compositor disconnected, cleaning up.");
      this.DestroySharedTextureProtocol();
      this.DestroyMixedRealityRender();
      this.DestroyExternalCamera();
      if (!SteamVRCompatibility.IsAvailable)
        return;
      Component component = this.GetComponent(SteamVRCompatibility.SteamVRExternalCamera);
      if (!((Object) component != (Object) null))
        return;
      component.gameObject.SetActive(this._wasSteamVRExternalCameraActive);
    }

    private void CreateSharedTextureProtocol() => this._sharedTextureProtocol = this.gameObject.AddComponent<SharedTextureProtocol>();

    private void DestroySharedTextureProtocol()
    {
      if (!((Object) this._sharedTextureProtocol != (Object) null))
        return;
      Object.Destroy((Object) this._sharedTextureProtocol);
      this._sharedTextureProtocol = (SharedTextureProtocol) null;
    }

    private void CreateExternalCamera() => this._externalCamera = new GameObject("LIV Camera Reference")
    {
      transform = {
        parent = this.Origin
      }
    }.AddComponent<ExternalCamera>();

    private void DestroyExternalCamera()
    {
      if (!((Object) this._externalCamera != (Object) null))
        return;
      Object.Destroy((Object) this._externalCamera.gameObject);
      this._externalCamera = (ExternalCamera) null;
    }

    private void CreateMixedRealityRender()
    {
      this.HMDCamera.enabled = false;
      this.HMDCamera.gameObject.SetActive(false);
      GameObject gameObject = Object.Instantiate<GameObject>(this.HMDCamera.gameObject);
      this.HMDCamera.gameObject.SetActive(true);
      this.HMDCamera.enabled = true;
      gameObject.name = "LIV Camera";
      while (gameObject.transform.childCount > 0)
        Object.DestroyImmediate((Object) gameObject.transform.GetChild(0).gameObject);
      Object.DestroyImmediate((Object) gameObject.GetComponent("AudioListener"));
      Object.DestroyImmediate((Object) gameObject.GetComponent("MeshCollider"));
      if (SteamVRCompatibility.IsAvailable)
      {
        Object.DestroyImmediate((Object) gameObject.GetComponent(SteamVRCompatibility.SteamVRCamera));
        Object.DestroyImmediate((Object) gameObject.GetComponent(SteamVRCompatibility.SteamVRFade));
      }
      this._mixedRealityRender = gameObject.AddComponent<MixedRealityRender>();
      gameObject.transform.parent = this._externalCamera.transform;
      gameObject.SetActive(true);
      this._mixedRealityRender.Setup(this);
    }

    private void DestroyMixedRealityRender()
    {
      if (!((Object) this._mixedRealityRender != (Object) null))
        return;
      Object.Destroy((Object) this._mixedRealityRender.gameObject);
      this._mixedRealityRender = (MixedRealityRender) null;
    }
  }
}
