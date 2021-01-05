// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.VR;
using Valve.Newtonsoft.Json;

namespace Valve.VR
{
  public class SteamVR : IDisposable
  {
    private static bool _enabled = true;
    private static SteamVR _instance;
    public static SteamVR.InitializedStates initializedState = SteamVR.InitializedStates.None;
    public static bool[] connected = new bool[new IntPtr(64)];
    public ETextureType textureType;
    private static bool runningTemporarySession = false;
    public const string defaultUnityAppKeyTemplate = "application.generated.unity.{0}.exe";
    public const string defaultAppKeyTemplate = "application.generated.{0}";

    private SteamVR()
    {
      this.hmd = OpenVR.System;
      UnityEngine.Debug.Log((object) ("<b>[SteamVR]</b> Initialized. Connected to " + this.hmd_TrackingSystemName + ":" + this.hmd_SerialNumber));
      this.compositor = OpenVR.Compositor;
      this.overlay = OpenVR.Overlay;
      uint pnWidth = 0;
      uint pnHeight = 0;
      this.hmd.GetRecommendedRenderTargetSize(ref pnWidth, ref pnHeight);
      this.sceneWidth = (float) pnWidth;
      this.sceneHeight = (float) pnHeight;
      float pfLeft1 = 0.0f;
      float pfRight1 = 0.0f;
      float pfTop1 = 0.0f;
      float pfBottom1 = 0.0f;
      this.hmd.GetProjectionRaw(EVREye.Eye_Left, ref pfLeft1, ref pfRight1, ref pfTop1, ref pfBottom1);
      float pfLeft2 = 0.0f;
      float pfRight2 = 0.0f;
      float pfTop2 = 0.0f;
      float pfBottom2 = 0.0f;
      this.hmd.GetProjectionRaw(EVREye.Eye_Right, ref pfLeft2, ref pfRight2, ref pfTop2, ref pfBottom2);
      this.tanHalfFov = new Vector2(Mathf.Max(-pfLeft1, pfRight1, -pfLeft2, pfRight2), Mathf.Max(-pfTop1, pfBottom1, -pfTop2, pfBottom2));
      this.textureBounds = new VRTextureBounds_t[2];
      this.textureBounds[0].uMin = (float) (0.5 + 0.5 * (double) pfLeft1 / (double) this.tanHalfFov.x);
      this.textureBounds[0].uMax = (float) (0.5 + 0.5 * (double) pfRight1 / (double) this.tanHalfFov.x);
      this.textureBounds[0].vMin = (float) (0.5 - 0.5 * (double) pfBottom1 / (double) this.tanHalfFov.y);
      this.textureBounds[0].vMax = (float) (0.5 - 0.5 * (double) pfTop1 / (double) this.tanHalfFov.y);
      this.textureBounds[1].uMin = (float) (0.5 + 0.5 * (double) pfLeft2 / (double) this.tanHalfFov.x);
      this.textureBounds[1].uMax = (float) (0.5 + 0.5 * (double) pfRight2 / (double) this.tanHalfFov.x);
      this.textureBounds[1].vMin = (float) (0.5 - 0.5 * (double) pfBottom2 / (double) this.tanHalfFov.y);
      this.textureBounds[1].vMax = (float) (0.5 - 0.5 * (double) pfTop2 / (double) this.tanHalfFov.y);
      this.sceneWidth /= Mathf.Max(this.textureBounds[0].uMax - this.textureBounds[0].uMin, this.textureBounds[1].uMax - this.textureBounds[1].uMin);
      this.sceneHeight /= Mathf.Max(this.textureBounds[0].vMax - this.textureBounds[0].vMin, this.textureBounds[1].vMax - this.textureBounds[1].vMin);
      this.aspect = this.tanHalfFov.x / this.tanHalfFov.y;
      this.fieldOfView = (float) (2.0 * (double) Mathf.Atan(this.tanHalfFov.y) * 57.2957801818848);
      this.eyes = new SteamVR_Utils.RigidTransform[2]
      {
        new SteamVR_Utils.RigidTransform(this.hmd.GetEyeToHeadTransform(EVREye.Eye_Left)),
        new SteamVR_Utils.RigidTransform(this.hmd.GetEyeToHeadTransform(EVREye.Eye_Right))
      };
      GraphicsDeviceType graphicsDeviceType = SystemInfo.graphicsDeviceType;
      switch (graphicsDeviceType)
      {
        case GraphicsDeviceType.OpenGLES2:
        case GraphicsDeviceType.OpenGLES3:
          this.textureType = ETextureType.OpenGL;
          break;
        default:
          if (graphicsDeviceType != GraphicsDeviceType.OpenGLCore)
          {
            this.textureType = graphicsDeviceType == GraphicsDeviceType.Vulkan ? ETextureType.Vulkan : ETextureType.DirectX;
            break;
          }
          goto case GraphicsDeviceType.OpenGLES2;
      }
      SteamVR_Events.Initializing.Listen(new UnityAction<bool>(this.OnInitializing));
      SteamVR_Events.Calibrating.Listen(new UnityAction<bool>(this.OnCalibrating));
      SteamVR_Events.OutOfRange.Listen(new UnityAction<bool>(this.OnOutOfRange));
      SteamVR_Events.DeviceConnected.Listen(new UnityAction<int, bool>(this.OnDeviceConnected));
      SteamVR_Events.NewPoses.Listen(new UnityAction<TrackedDevicePose_t[]>(this.OnNewPoses));
    }

    public static bool active => SteamVR._instance != null;

    public static bool enabled
    {
      get
      {
        if (!VRSettings.enabled)
          SteamVR.enabled = false;
        return SteamVR._enabled;
      }
      set
      {
        SteamVR._enabled = value;
        if (SteamVR._enabled)
          SteamVR.Initialize();
        else
          SteamVR.SafeDispose();
      }
    }

    public static SteamVR instance
    {
      get
      {
        if (!SteamVR.enabled)
          return (SteamVR) null;
        if (SteamVR._instance == null)
        {
          SteamVR._instance = SteamVR.CreateInstance();
          if (SteamVR._instance == null)
            SteamVR._enabled = false;
        }
        return SteamVR._instance;
      }
    }

    public static void Initialize(bool forceUnityVRMode = false)
    {
      if (forceUnityVRMode)
      {
        SteamVR_Behaviour.instance.InitializeSteamVR(true);
      }
      else
      {
        if (SteamVR._instance == null)
        {
          SteamVR._instance = SteamVR.CreateInstance();
          if (SteamVR._instance == null)
            SteamVR._enabled = false;
        }
        if (!SteamVR._enabled)
          return;
        SteamVR_Behaviour.Initialize(forceUnityVRMode);
      }
    }

    public static bool usingNativeSupport => VRDevice.GetNativePtr() != IntPtr.Zero;

    public static SteamVR_Settings settings { get; private set; }

    private static void ReportGeneralErrors()
    {
      string str = "<b>[SteamVR]</b> Initialization failed. ";
      if (!VRSettings.enabled)
        str += "VR may be disabled in player settings. Go to player settings in the editor and check the 'Virtual Reality Supported' checkbox'. ";
      if (VRSettings.supportedDevices != null && VRSettings.supportedDevices.Length > 0)
      {
        if (!((IEnumerable<string>) VRSettings.supportedDevices).Contains<string>("OpenVR"))
          str += "OpenVR is not in your list of supported virtual reality SDKs. Add it to the list in player settings. ";
        else if (!((IEnumerable<string>) VRSettings.supportedDevices).First<string>().Contains("OpenVR"))
          str += "OpenVR is not first in your list of supported virtual reality SDKs. <b>This is okay, but if you have an Oculus device plugged in, and Oculus above OpenVR in this list, it will try and use the Oculus SDK instead of OpenVR.</b> ";
      }
      else
        str += "You have no SDKs in your Player Settings list of supported virtual reality SDKs. Add OpenVR to it. ";
      UnityEngine.Debug.LogWarning((object) (str + "To force OpenVR initialization call SteamVR.Initialize(true). "));
    }

    private static SteamVR CreateInstance()
    {
      SteamVR.initializedState = SteamVR.InitializedStates.Initializing;
      try
      {
        EVRInitError peError = EVRInitError.None;
        if (!SteamVR.usingNativeSupport)
        {
          SteamVR.ReportGeneralErrors();
          SteamVR.initializedState = SteamVR.InitializedStates.InitializeFailure;
          SteamVR_Events.Initialized.Send(false);
          return (SteamVR) null;
        }
        OpenVR.GetGenericInterface("IVRCompositor_022", ref peError);
        if (peError != EVRInitError.None)
        {
          SteamVR.initializedState = SteamVR.InitializedStates.InitializeFailure;
          SteamVR.ReportError(peError);
          SteamVR.ReportGeneralErrors();
          SteamVR_Events.Initialized.Send(false);
          return (SteamVR) null;
        }
        OpenVR.GetGenericInterface("IVROverlay_019", ref peError);
        if (peError != EVRInitError.None)
        {
          SteamVR.initializedState = SteamVR.InitializedStates.InitializeFailure;
          SteamVR.ReportError(peError);
          SteamVR_Events.Initialized.Send(false);
          return (SteamVR) null;
        }
        OpenVR.GetGenericInterface("IVRInput_006", ref peError);
        if (peError != EVRInitError.None)
        {
          SteamVR.initializedState = SteamVR.InitializedStates.InitializeFailure;
          SteamVR.ReportError(peError);
          SteamVR_Events.Initialized.Send(false);
          return (SteamVR) null;
        }
        SteamVR.settings = SteamVR_Settings.instance;
        if (Application.isEditor)
          SteamVR.IdentifyEditorApplication();
        SteamVR_Input.IdentifyActionsFile();
        if (SteamVR_Settings.instance.inputUpdateMode == SteamVR_UpdateModes.Nothing)
        {
          if (SteamVR_Settings.instance.poseUpdateMode == SteamVR_UpdateModes.Nothing)
            goto label_15;
        }
        SteamVR_Input.Initialize();
      }
      catch (Exception ex)
      {
        UnityEngine.Debug.LogError((object) ("<b>[SteamVR]</b> " + (object) ex));
        SteamVR_Events.Initialized.Send(false);
        return (SteamVR) null;
      }
label_15:
      SteamVR._enabled = true;
      SteamVR.initializedState = SteamVR.InitializedStates.InitializeSuccess;
      SteamVR_Events.Initialized.Send(true);
      return new SteamVR();
    }

    private static void ReportError(EVRInitError error)
    {
      switch (error)
      {
        case EVRInitError.None:
          break;
        case EVRInitError.Init_VRClientDLLNotFound:
          UnityEngine.Debug.LogWarning((object) "<b>[SteamVR]</b> Drivers not found!  They can be installed via Steam under Library > Tools.  Visit http://steampowered.com to install Steam.");
          break;
        case EVRInitError.Driver_RuntimeOutOfDate:
          UnityEngine.Debug.LogWarning((object) "<b>[SteamVR]</b> Initialization Failed!  Make sure device's runtime is up to date.");
          break;
        case EVRInitError.VendorSpecific_UnableToConnectToOculusRuntime:
          UnityEngine.Debug.LogWarning((object) "<b>[SteamVR]</b> Initialization Failed!  Make sure device is on, Oculus runtime is installed, and OVRService_*.exe is running.");
          break;
        default:
          UnityEngine.Debug.LogWarning((object) ("<b>[SteamVR]</b> " + OpenVR.GetStringForHmdError(error)));
          break;
      }
    }

    public CVRSystem hmd { get; private set; }

    public CVRCompositor compositor { get; private set; }

    public CVROverlay overlay { get; private set; }

    public static bool initializing { get; private set; }

    public static bool calibrating { get; private set; }

    public static bool outOfRange { get; private set; }

    public float sceneWidth { get; private set; }

    public float sceneHeight { get; private set; }

    public float aspect { get; private set; }

    public float fieldOfView { get; private set; }

    public Vector2 tanHalfFov { get; private set; }

    public VRTextureBounds_t[] textureBounds { get; private set; }

    public SteamVR_Utils.RigidTransform[] eyes { get; private set; }

    public string hmd_TrackingSystemName => this.GetStringProperty(ETrackedDeviceProperty.Prop_TrackingSystemName_String);

    public string hmd_ModelNumber => this.GetStringProperty(ETrackedDeviceProperty.Prop_ModelNumber_String);

    public string hmd_SerialNumber => this.GetStringProperty(ETrackedDeviceProperty.Prop_SerialNumber_String);

    public float hmd_SecondsFromVsyncToPhotons => this.GetFloatProperty(ETrackedDeviceProperty.Prop_SecondsFromVsyncToPhotons_Float);

    public float hmd_DisplayFrequency => this.GetFloatProperty(ETrackedDeviceProperty.Prop_DisplayFrequency_Float);

    public string GetTrackedDeviceString(uint deviceId)
    {
      ETrackedPropertyError pError = ETrackedPropertyError.TrackedProp_Success;
      uint trackedDeviceProperty1 = this.hmd.GetStringTrackedDeviceProperty(deviceId, ETrackedDeviceProperty.Prop_AttachedDeviceId_String, (StringBuilder) null, 0U, ref pError);
      if (trackedDeviceProperty1 <= 1U)
        return (string) null;
      StringBuilder pchValue = new StringBuilder((int) trackedDeviceProperty1);
      int trackedDeviceProperty2 = (int) this.hmd.GetStringTrackedDeviceProperty(deviceId, ETrackedDeviceProperty.Prop_AttachedDeviceId_String, pchValue, trackedDeviceProperty1, ref pError);
      return pchValue.ToString();
    }

    public string GetStringProperty(ETrackedDeviceProperty prop, uint deviceId = 0)
    {
      ETrackedPropertyError pError = ETrackedPropertyError.TrackedProp_Success;
      uint trackedDeviceProperty1 = this.hmd.GetStringTrackedDeviceProperty(deviceId, prop, (StringBuilder) null, 0U, ref pError);
      if (trackedDeviceProperty1 > 1U)
      {
        StringBuilder pchValue = new StringBuilder((int) trackedDeviceProperty1);
        int trackedDeviceProperty2 = (int) this.hmd.GetStringTrackedDeviceProperty(deviceId, prop, pchValue, trackedDeviceProperty1, ref pError);
        return pchValue.ToString();
      }
      return pError != ETrackedPropertyError.TrackedProp_Success ? pError.ToString() : "<unknown>";
    }

    public float GetFloatProperty(ETrackedDeviceProperty prop, uint deviceId = 0)
    {
      ETrackedPropertyError pError = ETrackedPropertyError.TrackedProp_Success;
      return this.hmd.GetFloatTrackedDeviceProperty(deviceId, prop, ref pError);
    }

    public static bool InitializeTemporarySession(bool initInput = false)
    {
      if (!Application.isEditor)
        return false;
      EVRInitError peError1 = EVRInitError.None;
      OpenVR.GetGenericInterface("IVRCompositor_022", ref peError1);
      bool flag = peError1 != EVRInitError.None;
      if (flag)
      {
        EVRInitError peError2 = EVRInitError.None;
        OpenVR.Init(ref peError2, EVRApplicationType.VRApplication_Overlay, string.Empty);
        if (peError2 != EVRInitError.None)
        {
          UnityEngine.Debug.LogError((object) ("<b>[SteamVR]</b> Error during OpenVR Init: " + peError2.ToString()));
          return false;
        }
        SteamVR.IdentifyEditorApplication(false);
        SteamVR_Input.IdentifyActionsFile(false);
        SteamVR.runningTemporarySession = true;
      }
      if (initInput)
        SteamVR_Input.Initialize(true);
      return flag;
    }

    public static void ExitTemporarySession()
    {
      if (!SteamVR.runningTemporarySession)
        return;
      OpenVR.Shutdown();
      SteamVR.runningTemporarySession = false;
    }

    public static string GenerateAppKey() => string.Format("application.generated.unity.{0}.exe", (object) SteamVR.GenerateCleanProductName());

    public static string GenerateCleanProductName() => !string.IsNullOrEmpty(Application.productName) ? Regex.Replace(Application.productName, "[^\\w\\._]", string.Empty).ToLower() : "unnamed_product";

    private static string GetManifestFile()
    {
      string dataPath = Application.dataPath;
      int startIndex = dataPath.LastIndexOf('/');
      string str = Path.Combine(dataPath.Remove(startIndex, dataPath.Length - startIndex), "unityProject.vrmanifest");
      FileInfo fileInfo1 = new FileInfo(SteamVR_Settings.instance.actionsFilePath);
      if (File.Exists(str))
      {
        SteamVR_Input_ManifestFile inputManifestFile = JsonConvert.DeserializeObject<SteamVR_Input_ManifestFile>(File.ReadAllText(str));
        if (inputManifestFile != null && inputManifestFile.applications != null && (inputManifestFile.applications.Count > 0 && inputManifestFile.applications[0].app_key != SteamVR_Settings.instance.editorAppKey))
        {
          UnityEngine.Debug.Log((object) "<b>[SteamVR]</b> Deleting existing VRManifest because it has a different app key.");
          FileInfo fileInfo2 = new FileInfo(str);
          if (fileInfo2.IsReadOnly)
            fileInfo2.IsReadOnly = false;
          fileInfo2.Delete();
        }
        if (inputManifestFile != null && inputManifestFile.applications != null && (inputManifestFile.applications.Count > 0 && inputManifestFile.applications[0].action_manifest_path != fileInfo1.FullName))
        {
          UnityEngine.Debug.Log((object) ("<b>[SteamVR]</b> Deleting existing VRManifest because it has a different action manifest path:\nExisting:" + inputManifestFile.applications[0].action_manifest_path + "\nNew: " + fileInfo1.FullName));
          FileInfo fileInfo2 = new FileInfo(str);
          if (fileInfo2.IsReadOnly)
            fileInfo2.IsReadOnly = false;
          fileInfo2.Delete();
        }
      }
      if (!File.Exists(str))
      {
        SteamVR_Input_ManifestFile inputManifestFile = new SteamVR_Input_ManifestFile();
        inputManifestFile.source = "Unity";
        SteamVR_Input_ManifestFile_Application manifestFileApplication = new SteamVR_Input_ManifestFile_Application();
        manifestFileApplication.app_key = SteamVR_Settings.instance.editorAppKey;
        manifestFileApplication.action_manifest_path = fileInfo1.FullName;
        manifestFileApplication.launch_type = "url";
        manifestFileApplication.url = "steam://launch/";
        manifestFileApplication.strings.Add("en_us", new SteamVR_Input_ManifestFile_ApplicationString()
        {
          name = string.Format("{0} [Testing]", (object) Application.productName)
        });
        inputManifestFile.applications = new List<SteamVR_Input_ManifestFile_Application>();
        inputManifestFile.applications.Add(manifestFileApplication);
        string contents = JsonConvert.SerializeObject((object) inputManifestFile, Formatting.Indented, new JsonSerializerSettings()
        {
          NullValueHandling = NullValueHandling.Ignore
        });
        File.WriteAllText(str, contents);
      }
      return str;
    }

    private static void IdentifyEditorApplication(bool showLogs = true)
    {
      EVRApplicationError applicationError1 = OpenVR.Applications.AddApplicationManifest(SteamVR.GetManifestFile(), true);
      if (applicationError1 != EVRApplicationError.None)
        UnityEngine.Debug.LogError((object) ("<b>[SteamVR]</b> Error adding vr manifest file: " + applicationError1.ToString()));
      else if (showLogs)
        UnityEngine.Debug.Log((object) "<b>[SteamVR]</b> Successfully added VR manifest to SteamVR");
      EVRApplicationError applicationError2 = OpenVR.Applications.IdentifyApplication((uint) Process.GetCurrentProcess().Id, SteamVR_Settings.instance.editorAppKey);
      if (applicationError2 != EVRApplicationError.None)
      {
        UnityEngine.Debug.LogError((object) ("<b>[SteamVR]</b> Error identifying application: " + applicationError2.ToString()));
      }
      else
      {
        if (!showLogs)
          return;
        UnityEngine.Debug.Log((object) string.Format("<b>[SteamVR]</b> Successfully identified process as editor project to SteamVR ({0})", (object) SteamVR_Settings.instance.editorAppKey));
      }
    }

    private void OnInitializing(bool initializing) => SteamVR.initializing = initializing;

    private void OnCalibrating(bool calibrating) => SteamVR.calibrating = calibrating;

    private void OnOutOfRange(bool outOfRange) => SteamVR.outOfRange = outOfRange;

    private void OnDeviceConnected(int i, bool connected) => SteamVR.connected[i] = connected;

    private void OnNewPoses(TrackedDevicePose_t[] poses)
    {
      this.eyes[0] = new SteamVR_Utils.RigidTransform(this.hmd.GetEyeToHeadTransform(EVREye.Eye_Left));
      this.eyes[1] = new SteamVR_Utils.RigidTransform(this.hmd.GetEyeToHeadTransform(EVREye.Eye_Right));
      for (int index = 0; index < poses.Length; ++index)
      {
        bool deviceIsConnected = poses[index].bDeviceIsConnected;
        if (deviceIsConnected != SteamVR.connected[index])
          SteamVR_Events.DeviceConnected.Send(index, deviceIsConnected);
      }
      if (poses.Length <= 0)
        return;
      ETrackingResult eTrackingResult = poses[IntPtr.Zero].eTrackingResult;
      bool flag1 = eTrackingResult == ETrackingResult.Uninitialized;
      if (flag1 != SteamVR.initializing)
        SteamVR_Events.Initializing.Send(flag1);
      bool flag2 = eTrackingResult == ETrackingResult.Calibrating_InProgress || eTrackingResult == ETrackingResult.Calibrating_OutOfRange;
      if (flag2 != SteamVR.calibrating)
        SteamVR_Events.Calibrating.Send(flag2);
      bool flag3 = eTrackingResult == ETrackingResult.Running_OutOfRange || eTrackingResult == ETrackingResult.Calibrating_OutOfRange;
      if (flag3 == SteamVR.outOfRange)
        return;
      SteamVR_Events.OutOfRange.Send(flag3);
    }

    ~SteamVR() => this.Dispose(false);

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      SteamVR_Events.Initializing.Remove(new UnityAction<bool>(this.OnInitializing));
      SteamVR_Events.Calibrating.Remove(new UnityAction<bool>(this.OnCalibrating));
      SteamVR_Events.OutOfRange.Remove(new UnityAction<bool>(this.OnOutOfRange));
      SteamVR_Events.DeviceConnected.Remove(new UnityAction<int, bool>(this.OnDeviceConnected));
      SteamVR_Events.NewPoses.Remove(new UnityAction<TrackedDevicePose_t[]>(this.OnNewPoses));
      SteamVR._instance = (SteamVR) null;
    }

    public static void SafeDispose()
    {
      if (SteamVR._instance == null)
        return;
      SteamVR._instance.Dispose();
    }

    public enum InitializedStates
    {
      None,
      Initializing,
      InitializeSuccess,
      InitializeFailure,
    }
  }
}
