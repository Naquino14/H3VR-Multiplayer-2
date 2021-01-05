// Decompiled with JetBrains decompiler
// Type: SteamVR_LoadLevel
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Valve.VR;

public class SteamVR_LoadLevel : MonoBehaviour
{
  private static SteamVR_LoadLevel _active;
  public string levelName;
  public string internalProcessPath;
  public string internalProcessArgs;
  public bool loadAdditive;
  public bool loadAsync = true;
  public Texture loadingScreen;
  public Texture progressBarEmpty;
  public Texture progressBarFull;
  public float loadingScreenWidthInMeters = 6f;
  public float progressBarWidthInMeters = 3f;
  public float loadingScreenDistance;
  public Transform loadingScreenTransform;
  public Transform progressBarTransform;
  public Texture front;
  public Texture back;
  public Texture left;
  public Texture right;
  public Texture top;
  public Texture bottom;
  public Color backgroundColor = Color.black;
  public bool showGrid;
  public float fadeOutTime = 0.5f;
  public float fadeInTime = 0.5f;
  public float postLoadSettleTime;
  public float loadingScreenFadeInTime = 1f;
  public float loadingScreenFadeOutTime = 0.25f;
  private float fadeRate = 1f;
  private float alpha;
  private AsyncOperation async;
  private RenderTexture renderTexture;
  private ulong loadingScreenOverlayHandle;
  private ulong progressBarOverlayHandle;
  public bool autoTriggerOnEnable;

  public static bool loading => (Object) SteamVR_LoadLevel._active != (Object) null;

  public static float progress => (Object) SteamVR_LoadLevel._active != (Object) null && SteamVR_LoadLevel._active.async != null ? SteamVR_LoadLevel._active.async.progress : 0.0f;

  public static Texture progressTexture => (Object) SteamVR_LoadLevel._active != (Object) null ? (Texture) SteamVR_LoadLevel._active.renderTexture : (Texture) null;

  private void OnEnable()
  {
    if (!this.autoTriggerOnEnable)
      return;
    this.Trigger();
  }

  public void Trigger()
  {
    if (SteamVR_LoadLevel.loading || string.IsNullOrEmpty(this.levelName))
      return;
    this.StartCoroutine(this.LoadLevel());
  }

  public static void Begin(
    string levelName,
    bool showGrid = false,
    float fadeOutTime = 0.5f,
    float r = 0.0f,
    float g = 0.0f,
    float b = 0.0f,
    float a = 1f)
  {
    SteamVR_LoadLevel steamVrLoadLevel = new GameObject("loader").AddComponent<SteamVR_LoadLevel>();
    steamVrLoadLevel.levelName = levelName;
    steamVrLoadLevel.showGrid = showGrid;
    steamVrLoadLevel.fadeOutTime = fadeOutTime;
    steamVrLoadLevel.backgroundColor = new Color(r, g, b, a);
    steamVrLoadLevel.Trigger();
  }

  private void OnGUI()
  {
    if ((Object) SteamVR_LoadLevel._active != (Object) this || !((Object) this.progressBarEmpty != (Object) null) || !((Object) this.progressBarFull != (Object) null))
      return;
    if (this.progressBarOverlayHandle == 0UL)
      this.progressBarOverlayHandle = this.GetOverlayHandle("progressBar", !((Object) this.progressBarTransform != (Object) null) ? this.transform : this.progressBarTransform, this.progressBarWidthInMeters);
    if (this.progressBarOverlayHandle == 0UL)
      return;
    float width1 = this.async == null ? 0.0f : this.async.progress;
    int width2 = this.progressBarFull.width;
    int height = this.progressBarFull.height;
    if ((Object) this.renderTexture == (Object) null)
    {
      this.renderTexture = new RenderTexture(width2, height, 0);
      this.renderTexture.Create();
    }
    RenderTexture active = RenderTexture.active;
    RenderTexture.active = this.renderTexture;
    if (UnityEngine.Event.current.type == EventType.Repaint)
      GL.Clear(false, true, Color.clear);
    GUILayout.BeginArea(new Rect(0.0f, 0.0f, (float) width2, (float) height));
    GUI.DrawTexture(new Rect(0.0f, 0.0f, (float) width2, (float) height), this.progressBarEmpty);
    GUI.DrawTextureWithTexCoords(new Rect(0.0f, 0.0f, width1 * (float) width2, (float) height), this.progressBarFull, new Rect(0.0f, 0.0f, width1, 1f));
    GUILayout.EndArea();
    RenderTexture.active = active;
    CVROverlay overlay = OpenVR.Overlay;
    if (overlay == null)
      return;
    int num = (int) overlay.SetOverlayTexture(this.progressBarOverlayHandle, ref new Texture_t()
    {
      handle = this.renderTexture.GetNativeTexturePtr(),
      eType = SteamVR.instance.textureType,
      eColorSpace = EColorSpace.Auto
    });
  }

  private void Update()
  {
    if ((Object) SteamVR_LoadLevel._active != (Object) this)
      return;
    this.alpha = Mathf.Clamp01(this.alpha + this.fadeRate * Time.deltaTime);
    CVROverlay overlay = OpenVR.Overlay;
    if (overlay == null)
      return;
    if (this.loadingScreenOverlayHandle != 0UL)
    {
      int num1 = (int) overlay.SetOverlayAlpha(this.loadingScreenOverlayHandle, this.alpha);
    }
    if (this.progressBarOverlayHandle == 0UL)
      return;
    int num2 = (int) overlay.SetOverlayAlpha(this.progressBarOverlayHandle, this.alpha);
  }

  [DebuggerHidden]
  private IEnumerator LoadLevel() => (IEnumerator) new SteamVR_LoadLevel.\u003CLoadLevel\u003Ec__Iterator0()
  {
    \u0024this = this
  };

  private ulong GetOverlayHandle(string overlayName, Transform transform, float widthInMeters = 1f)
  {
    ulong pOverlayHandle = 0;
    CVROverlay overlay1 = OpenVR.Overlay;
    if (overlay1 == null)
      return pOverlayHandle;
    string pchOverlayKey = SteamVR_Overlay.key + "." + overlayName;
    EVROverlayError overlay2 = overlay1.FindOverlay(pchOverlayKey, ref pOverlayHandle);
    if (overlay2 != EVROverlayError.None)
      overlay2 = overlay1.CreateOverlay(pchOverlayKey, overlayName, ref pOverlayHandle);
    if (overlay2 == EVROverlayError.None)
    {
      int num1 = (int) overlay1.ShowOverlay(pOverlayHandle);
      int num2 = (int) overlay1.SetOverlayAlpha(pOverlayHandle, this.alpha);
      int num3 = (int) overlay1.SetOverlayWidthInMeters(pOverlayHandle, widthInMeters);
      if (SteamVR.instance.textureType == ETextureType.DirectX)
      {
        int num4 = (int) overlay1.SetOverlayTextureBounds(pOverlayHandle, ref new VRTextureBounds_t()
        {
          uMin = 0.0f,
          vMin = 1f,
          uMax = 1f,
          vMax = 0.0f
        });
      }
      SteamVR_Camera steamVrCamera = (double) this.loadingScreenDistance != 0.0 ? (SteamVR_Camera) null : SteamVR_Render.Top();
      if ((Object) steamVrCamera != (Object) null && (Object) steamVrCamera.origin != (Object) null)
      {
        SteamVR_Utils.RigidTransform rigidTransform = new SteamVR_Utils.RigidTransform(steamVrCamera.origin, transform);
        rigidTransform.pos.x /= steamVrCamera.origin.localScale.x;
        rigidTransform.pos.y /= steamVrCamera.origin.localScale.y;
        rigidTransform.pos.z /= steamVrCamera.origin.localScale.z;
        HmdMatrix34_t hmdMatrix34 = rigidTransform.ToHmdMatrix34();
        int num5 = (int) overlay1.SetOverlayTransformAbsolute(pOverlayHandle, SteamVR_Settings.instance.trackingSpace, ref hmdMatrix34);
      }
      else
      {
        HmdMatrix34_t hmdMatrix34 = new SteamVR_Utils.RigidTransform(transform).ToHmdMatrix34();
        int num5 = (int) overlay1.SetOverlayTransformAbsolute(pOverlayHandle, SteamVR_Settings.instance.trackingSpace, ref hmdMatrix34);
      }
    }
    return pOverlayHandle;
  }
}
