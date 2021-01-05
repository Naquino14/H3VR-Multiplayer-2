// Decompiled with JetBrains decompiler
// Type: LIV.SDK.Unity.MixedRealityRender
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;

namespace LIV.SDK.Unity
{
  [AddComponentMenu("LIV/MixedRealityRender")]
  public class MixedRealityRender : MonoBehaviour
  {
    private bool _isSetup;
    private LIV.SDK.Unity.LIV _liv;
    private Camera _mrCamera;
    private Transform _hmd;
    private GameObject _clipQuad;
    private Material _clipMaterial;
    private Material _blitMaterial;
    private RenderTexture _compositeTexture;
    private RenderTexture _foregroundTexture;
    private RenderTexture _backgroundTexture;

    public void Setup(LIV.SDK.Unity.LIV liv)
    {
      this._liv = liv;
      this._mrCamera = this.GetComponent<Camera>();
      this._mrCamera.rect = new Rect(0.0f, 0.0f, 1f, 1f);
      this._mrCamera.depth = float.MaxValue;
      this._mrCamera.stereoTargetEye = StereoTargetEyeMask.None;
      this._mrCamera.useOcclusionCulling = false;
      this._mrCamera.enabled = false;
      this._clipMaterial = new Material(Shader.Find("Custom/LIV_ClearBackground"));
      this._blitMaterial = new Material(Shader.Find("Custom/LIV_Blit"));
      this.CreateClipQuad();
      this._isSetup = true;
    }

    private void CreateClipQuad()
    {
      this._clipQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
      this._clipQuad.name = "ClipQuad";
      Object.Destroy((Object) this._clipQuad.GetComponent<MeshCollider>());
      this._clipQuad.transform.parent = this.transform;
      MeshRenderer component = this._clipQuad.GetComponent<MeshRenderer>();
      component.material = this._clipMaterial;
      component.shadowCastingMode = ShadowCastingMode.Off;
      component.receiveShadows = false;
      component.lightProbeUsage = LightProbeUsage.Off;
      component.reflectionProbeUsage = ReflectionProbeUsage.Off;
      Transform transform = this._clipQuad.transform;
      transform.localScale = new Vector3(1000f, 1000f, 1f);
      transform.localRotation = Quaternion.identity;
      this._clipQuad.SetActive(false);
    }

    private void UpdateCamera()
    {
      this._mrCamera.fieldOfView = Calibration.FieldOfVision;
      this._mrCamera.nearClipPlane = Calibration.NearClip;
      this._mrCamera.farClipPlane = Calibration.FarClip;
      this._mrCamera.cullingMask = (int) this._liv.SpectatorLayerMask;
      this._hmd = this._liv.HMDCamera.transform;
      this.transform.localPosition = Calibration.PositionOffset;
      this.transform.localRotation = Calibration.RotationOffset;
      this.transform.localScale = Vector3.one;
    }

    private void UpdateTextures()
    {
      int textureWidth = SharedTextureProtocol.TextureWidth;
      int textureHeight = SharedTextureProtocol.TextureHeight;
      int height = SharedTextureProtocol.TextureHeight / 2;
      if ((Object) this._compositeTexture == (Object) null || this._compositeTexture.width != textureWidth || this._compositeTexture.height != textureHeight)
      {
        this._compositeTexture = new RenderTexture(textureWidth, textureHeight, 24, RenderTextureFormat.ARGB32);
        this._compositeTexture.antiAliasing = 1;
      }
      if ((Object) this._foregroundTexture == (Object) null || this._foregroundTexture.width != textureWidth || this._foregroundTexture.height != height)
      {
        RenderTexture renderTexture = new RenderTexture(textureWidth, height, 24, RenderTextureFormat.ARGB32);
        renderTexture.antiAliasing = 1;
        renderTexture.wrapMode = TextureWrapMode.Clamp;
        renderTexture.useMipMap = false;
        renderTexture.anisoLevel = 0;
        this._foregroundTexture = renderTexture;
      }
      if (!((Object) this._backgroundTexture == (Object) null) && this._backgroundTexture.width == textureWidth && this._backgroundTexture.height == height)
        return;
      RenderTexture renderTexture1 = new RenderTexture(textureWidth, height, 24, RenderTextureFormat.ARGB32);
      renderTexture1.antiAliasing = 1;
      renderTexture1.wrapMode = TextureWrapMode.Clamp;
      renderTexture1.useMipMap = false;
      renderTexture1.anisoLevel = 0;
      this._backgroundTexture = renderTexture1;
    }

    public float GetDistanceToHMD()
    {
      if ((Object) this._hmd == (Object) null)
        return Calibration.NearClip;
      Transform transform = this._mrCamera.transform;
      return -new Plane(new Vector3(transform.forward.x, 0.0f, transform.forward.z).normalized, this._hmd.position + new Vector3(this._hmd.forward.x, 0.0f, this._hmd.forward.z).normalized * Calibration.HMDOffset).GetDistanceToPoint(transform.position);
    }

    private void OrientClipQuad()
    {
      float num = Mathf.Clamp(this.GetDistanceToHMD() + Calibration.NearOffset, Calibration.NearClip, Calibration.FarClip);
      Transform parent = this._clipQuad.transform.parent;
      this._clipQuad.transform.position = parent.position + parent.forward * num;
      this._clipQuad.transform.LookAt(new Vector3(this._clipQuad.transform.parent.position.x, this._clipQuad.transform.position.y, this._clipQuad.transform.parent.position.z));
    }

    private void RenderNear()
    {
      MonoBehaviour[] monoBehaviourArray = (MonoBehaviour[]) null;
      bool[] flagArray = (bool[]) null;
      if (this._liv.DisableStandardAssets)
      {
        monoBehaviourArray = this._mrCamera.gameObject.GetComponents<MonoBehaviour>();
        flagArray = new bool[monoBehaviourArray.Length];
        for (int index = 0; index < monoBehaviourArray.Length; ++index)
        {
          MonoBehaviour monoBehaviour = monoBehaviourArray[index];
          if (monoBehaviour.enabled && monoBehaviour.GetType().ToString().StartsWith("UnityStandardAssets."))
          {
            monoBehaviour.enabled = false;
            flagArray[index] = true;
          }
        }
      }
      CameraClearFlags clearFlags = this._mrCamera.clearFlags;
      Color backgroundColor = this._mrCamera.backgroundColor;
      this._mrCamera.clearFlags = CameraClearFlags.Color;
      this._mrCamera.backgroundColor = Color.clear;
      this._clipQuad.SetActive(true);
      this._mrCamera.targetTexture = this._foregroundTexture;
      this._foregroundTexture.DiscardContents(true, true);
      this._mrCamera.Render();
      this._clipQuad.SetActive(false);
      this._mrCamera.clearFlags = clearFlags;
      this._mrCamera.backgroundColor = backgroundColor;
      if (monoBehaviourArray == null)
        return;
      for (int index = 0; index < monoBehaviourArray.Length; ++index)
      {
        if (flagArray[index])
          monoBehaviourArray[index].enabled = true;
      }
    }

    private void RenderFar()
    {
      this._mrCamera.targetTexture = this._backgroundTexture;
      this._backgroundTexture.DiscardContents(true, true);
      this._mrCamera.Render();
    }

    private void Composite()
    {
      this._compositeTexture.DiscardContents(true, true);
      this._blitMaterial.SetTexture("_NearTex", (Texture) this._foregroundTexture);
      this._blitMaterial.SetTexture("_FarTex", (Texture) this._backgroundTexture);
      Graphics.Blit((Texture) null, this._compositeTexture, this._blitMaterial);
      SharedTextureProtocol.SetOutputTexture((Texture) this._compositeTexture);
    }

    private void OnEnable() => this.StartCoroutine(this.RenderLoop());

    [DebuggerHidden]
    private IEnumerator RenderLoop() => (IEnumerator) new MixedRealityRender.\u003CRenderLoop\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }
}
