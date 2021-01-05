// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Render
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR
{
  public class SteamVR_Render : MonoBehaviour
  {
    public SteamVR_ExternalCamera externalCamera;
    public const string externalCameraConfigPath = "externalcamera-legacy.cfg";
    private static bool isQuitting;
    private SteamVR_Camera[] cameras = new SteamVR_Camera[0];
    public TrackedDevicePose_t[] poses = new TrackedDevicePose_t[new IntPtr(64)];
    public TrackedDevicePose_t[] gamePoses = new TrackedDevicePose_t[0];
    private static bool _pauseRendering;
    private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    private float sceneResolutionScale = 1f;
    private float timeScale = 1f;
    private EVRScreenshotType[] screenshotTypes = new EVRScreenshotType[1]
    {
      EVRScreenshotType.StereoPanorama
    };
    private static int lastFrameCount = -1;

    public static EVREye eye { get; private set; }

    public static SteamVR_Render instance => SteamVR_Behaviour.instance.steamvr_render;

    private void OnApplicationQuit()
    {
      SteamVR_Render.isQuitting = true;
      SteamVR.SafeDispose();
    }

    public static void Add(SteamVR_Camera vrcam)
    {
      if (SteamVR_Render.isQuitting)
        return;
      SteamVR_Render.instance.AddInternal(vrcam);
    }

    public static void Remove(SteamVR_Camera vrcam)
    {
      if (SteamVR_Render.isQuitting || !((UnityEngine.Object) SteamVR_Render.instance != (UnityEngine.Object) null))
        return;
      SteamVR_Render.instance.RemoveInternal(vrcam);
    }

    public static SteamVR_Camera Top() => !SteamVR_Render.isQuitting ? SteamVR_Render.instance.TopInternal() : (SteamVR_Camera) null;

    private void AddInternal(SteamVR_Camera vrcam)
    {
      Camera component1 = vrcam.GetComponent<Camera>();
      int length = this.cameras.Length;
      SteamVR_Camera[] steamVrCameraArray = new SteamVR_Camera[length + 1];
      int index1 = 0;
      for (int index2 = 0; index2 < length; ++index2)
      {
        Camera component2 = this.cameras[index2].GetComponent<Camera>();
        if (index2 == index1 && (double) component2.depth > (double) component1.depth)
          steamVrCameraArray[index1++] = vrcam;
        steamVrCameraArray[index1++] = this.cameras[index2];
      }
      if (index1 == length)
        steamVrCameraArray[index1] = vrcam;
      this.cameras = steamVrCameraArray;
    }

    private void RemoveInternal(SteamVR_Camera vrcam)
    {
      int length = this.cameras.Length;
      int num1 = 0;
      for (int index = 0; index < length; ++index)
      {
        if ((UnityEngine.Object) this.cameras[index] == (UnityEngine.Object) vrcam)
          ++num1;
      }
      if (num1 == 0)
        return;
      SteamVR_Camera[] steamVrCameraArray = new SteamVR_Camera[length - num1];
      int num2 = 0;
      for (int index = 0; index < length; ++index)
      {
        SteamVR_Camera camera = this.cameras[index];
        if ((UnityEngine.Object) camera != (UnityEngine.Object) vrcam)
          steamVrCameraArray[num2++] = camera;
      }
      this.cameras = steamVrCameraArray;
    }

    private SteamVR_Camera TopInternal() => this.cameras.Length > 0 ? this.cameras[this.cameras.Length - 1] : (SteamVR_Camera) null;

    public static bool pauseRendering
    {
      get => SteamVR_Render._pauseRendering;
      set
      {
        SteamVR_Render._pauseRendering = value;
        OpenVR.Compositor?.SuspendRendering(value);
      }
    }

    [DebuggerHidden]
    private IEnumerator RenderLoop() => (IEnumerator) new SteamVR_Render.\u003CRenderLoop\u003Ec__Iterator0()
    {
      \u0024this = this
    };

    private void RenderExternalCamera()
    {
      if ((UnityEngine.Object) this.externalCamera == (UnityEngine.Object) null || !this.externalCamera.gameObject.activeInHierarchy || Time.frameCount % ((int) Mathf.Max(this.externalCamera.config.frameSkip, 0.0f) + 1) != 0)
        return;
      this.externalCamera.AttachToCamera(this.TopInternal());
      this.externalCamera.RenderNear();
      this.externalCamera.RenderFar();
    }

    private void OnInputFocus(bool hasFocus)
    {
      if (SteamVR.active)
        ;
    }

    private string GetScreenshotFilename(
      uint screenshotHandle,
      EVRScreenshotPropertyFilenames screenshotPropertyFilename)
    {
      EVRScreenshotError pError = EVRScreenshotError.None;
      uint propertyFilename1 = OpenVR.Screenshots.GetScreenshotPropertyFilename(screenshotHandle, screenshotPropertyFilename, (StringBuilder) null, 0U, ref pError);
      if (pError != EVRScreenshotError.None && pError != EVRScreenshotError.BufferTooSmall)
        return (string) null;
      if (propertyFilename1 <= 1U)
        return (string) null;
      StringBuilder pchFilename = new StringBuilder((int) propertyFilename1);
      int propertyFilename2 = (int) OpenVR.Screenshots.GetScreenshotPropertyFilename(screenshotHandle, screenshotPropertyFilename, pchFilename, propertyFilename1, ref pError);
      return pError != EVRScreenshotError.None ? (string) null : pchFilename.ToString();
    }

    private void OnRequestScreenshot(VREvent_t vrEvent)
    {
      uint handle = vrEvent.data.screenshot.handle;
      EVRScreenshotType type = (EVRScreenshotType) vrEvent.data.screenshot.type;
      if (type != EVRScreenshotType.StereoPanorama)
        return;
      string screenshotFilename1 = this.GetScreenshotFilename(handle, EVRScreenshotPropertyFilenames.Preview);
      string screenshotFilename2 = this.GetScreenshotFilename(handle, EVRScreenshotPropertyFilenames.VR);
      if (screenshotFilename1 == null || screenshotFilename2 == null)
        return;
      SteamVR_Utils.TakeStereoScreenshot(handle, new GameObject("screenshotPosition")
      {
        transform = {
          position = SteamVR_Render.Top().transform.position,
          rotation = SteamVR_Render.Top().transform.rotation,
          localScale = SteamVR_Render.Top().transform.lossyScale
        }
      }, 32, 0.064f, ref screenshotFilename1, ref screenshotFilename2);
      int num = (int) OpenVR.Screenshots.SubmitScreenshot(handle, type, screenshotFilename1, screenshotFilename2);
    }

    private void OnEnable()
    {
      this.StartCoroutine(this.RenderLoop());
      SteamVR_Events.InputFocus.Listen(new UnityAction<bool>(this.OnInputFocus));
      SteamVR_Events.System(EVREventType.VREvent_RequestScreenshot).Listen(new UnityAction<VREvent_t>(this.OnRequestScreenshot));
      Camera.onPreCull += new Camera.CameraCallback(this.OnCameraPreCull);
      if (SteamVR.initializedState == SteamVR.InitializedStates.InitializeSuccess)
      {
        int num = (int) OpenVR.Screenshots.HookScreenshot(this.screenshotTypes);
      }
      else
        SteamVR_Events.Initialized.AddListener(new UnityAction<bool>(this.OnSteamVRInitialized));
    }

    private void OnSteamVRInitialized(bool success)
    {
      if (!success)
        return;
      int num = (int) OpenVR.Screenshots.HookScreenshot(this.screenshotTypes);
    }

    private void OnDisable()
    {
      this.StopAllCoroutines();
      SteamVR_Events.InputFocus.Remove(new UnityAction<bool>(this.OnInputFocus));
      SteamVR_Events.System(EVREventType.VREvent_RequestScreenshot).Remove(new UnityAction<VREvent_t>(this.OnRequestScreenshot));
      Camera.onPreCull -= new Camera.CameraCallback(this.OnCameraPreCull);
      if (SteamVR.initializedState == SteamVR.InitializedStates.InitializeSuccess)
        return;
      SteamVR_Events.Initialized.RemoveListener(new UnityAction<bool>(this.OnSteamVRInitialized));
    }

    private void Awake()
    {
      if (!((UnityEngine.Object) this.externalCamera == (UnityEngine.Object) null) || !File.Exists("externalcamera-legacy.cfg"))
        return;
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("SteamVR_ExternalCamera"));
      gameObject.gameObject.name = "External Camera";
      this.externalCamera = gameObject.transform.GetChild(0).GetComponent<SteamVR_ExternalCamera>();
      this.externalCamera.configPath = "externalcamera-legacy.cfg";
      this.externalCamera.ReadConfig();
    }

    public void UpdatePoses()
    {
      CVRCompositor compositor = OpenVR.Compositor;
      if (compositor == null)
        return;
      int lastPoses = (int) compositor.GetLastPoses(this.poses, this.gamePoses);
      SteamVR_Events.NewPoses.Send(this.poses);
      SteamVR_Events.NewPosesApplied.Send();
    }

    private void OnCameraPreCull(Camera cam)
    {
      if (!SteamVR.active || !cam.stereoEnabled || Time.frameCount == SteamVR_Render.lastFrameCount)
        return;
      SteamVR_Render.lastFrameCount = Time.frameCount;
      if (!SteamVR.settings.IsPoseUpdateMode(SteamVR_UpdateModes.OnPreCull))
        return;
      this.UpdatePoses();
    }

    private void Update()
    {
      if (!SteamVR.active)
        return;
      this.UpdatePoses();
      CVRSystem system = OpenVR.System;
      if (system != null)
      {
        VREvent_t pEvent = new VREvent_t();
        uint uncbVREvent = (uint) Marshal.SizeOf(typeof (VREvent_t));
        for (int index = 0; index < 64 && system.PollNextEvent(ref pEvent, uncbVREvent); ++index)
        {
          switch ((EVREventType) pEvent.eventType)
          {
            case EVREventType.VREvent_InputFocusCaptured:
              if (pEvent.data.process.oldPid == 0U)
              {
                SteamVR_Events.InputFocus.Send(false);
                break;
              }
              break;
            case EVREventType.VREvent_InputFocusReleased:
              if (pEvent.data.process.pid == 0U)
              {
                SteamVR_Events.InputFocus.Send(true);
                break;
              }
              break;
            case EVREventType.VREvent_HideRenderModels:
              SteamVR_Events.HideRenderModels.Send(true);
              break;
            case EVREventType.VREvent_ShowRenderModels:
              SteamVR_Events.HideRenderModels.Send(false);
              break;
            default:
              SteamVR_Events.System((EVREventType) pEvent.eventType).Send(pEvent);
              break;
          }
        }
      }
      Application.targetFrameRate = -1;
      Application.runInBackground = true;
      QualitySettings.maxQueuedFrames = -1;
      QualitySettings.vSyncCount = 0;
    }
  }
}
