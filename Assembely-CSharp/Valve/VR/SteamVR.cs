using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VR;
using Valve.Newtonsoft.Json;

namespace Valve.VR
{
	public class SteamVR : IDisposable
	{
		public enum InitializedStates
		{
			None,
			Initializing,
			InitializeSuccess,
			InitializeFailure
		}

		private static bool _enabled = true;

		private static SteamVR _instance;

		public static InitializedStates initializedState = InitializedStates.None;

		public static bool[] connected = new bool[64];

		public ETextureType textureType;

		private static bool runningTemporarySession = false;

		public const string defaultUnityAppKeyTemplate = "application.generated.unity.{0}.exe";

		public const string defaultAppKeyTemplate = "application.generated.{0}";

		public static bool active => _instance != null;

		public static bool enabled
		{
			get
			{
				if (!VRSettings.enabled)
				{
					enabled = false;
				}
				return _enabled;
			}
			set
			{
				_enabled = value;
				if (_enabled)
				{
					Initialize();
				}
				else
				{
					SafeDispose();
				}
			}
		}

		public static SteamVR instance
		{
			get
			{
				if (!enabled)
				{
					return null;
				}
				if (_instance == null)
				{
					_instance = CreateInstance();
					if (_instance == null)
					{
						_enabled = false;
					}
				}
				return _instance;
			}
		}

		public static bool usingNativeSupport => VRDevice.GetNativePtr() != IntPtr.Zero;

		public static SteamVR_Settings settings
		{
			get;
			private set;
		}

		public CVRSystem hmd
		{
			get;
			private set;
		}

		public CVRCompositor compositor
		{
			get;
			private set;
		}

		public CVROverlay overlay
		{
			get;
			private set;
		}

		public static bool initializing
		{
			get;
			private set;
		}

		public static bool calibrating
		{
			get;
			private set;
		}

		public static bool outOfRange
		{
			get;
			private set;
		}

		public float sceneWidth
		{
			get;
			private set;
		}

		public float sceneHeight
		{
			get;
			private set;
		}

		public float aspect
		{
			get;
			private set;
		}

		public float fieldOfView
		{
			get;
			private set;
		}

		public Vector2 tanHalfFov
		{
			get;
			private set;
		}

		public VRTextureBounds_t[] textureBounds
		{
			get;
			private set;
		}

		public SteamVR_Utils.RigidTransform[] eyes
		{
			get;
			private set;
		}

		public string hmd_TrackingSystemName => GetStringProperty(ETrackedDeviceProperty.Prop_TrackingSystemName_String);

		public string hmd_ModelNumber => GetStringProperty(ETrackedDeviceProperty.Prop_ModelNumber_String);

		public string hmd_SerialNumber => GetStringProperty(ETrackedDeviceProperty.Prop_SerialNumber_String);

		public float hmd_SecondsFromVsyncToPhotons => GetFloatProperty(ETrackedDeviceProperty.Prop_SecondsFromVsyncToPhotons_Float);

		public float hmd_DisplayFrequency => GetFloatProperty(ETrackedDeviceProperty.Prop_DisplayFrequency_Float);

		private SteamVR()
		{
			hmd = OpenVR.System;
			UnityEngine.Debug.Log("<b>[SteamVR]</b> Initialized. Connected to " + hmd_TrackingSystemName + ":" + hmd_SerialNumber);
			compositor = OpenVR.Compositor;
			overlay = OpenVR.Overlay;
			uint pnWidth = 0u;
			uint pnHeight = 0u;
			hmd.GetRecommendedRenderTargetSize(ref pnWidth, ref pnHeight);
			sceneWidth = pnWidth;
			sceneHeight = pnHeight;
			float pfLeft = 0f;
			float pfRight = 0f;
			float pfTop = 0f;
			float pfBottom = 0f;
			hmd.GetProjectionRaw(EVREye.Eye_Left, ref pfLeft, ref pfRight, ref pfTop, ref pfBottom);
			float pfLeft2 = 0f;
			float pfRight2 = 0f;
			float pfTop2 = 0f;
			float pfBottom2 = 0f;
			hmd.GetProjectionRaw(EVREye.Eye_Right, ref pfLeft2, ref pfRight2, ref pfTop2, ref pfBottom2);
			tanHalfFov = new Vector2(Mathf.Max(0f - pfLeft, pfRight, 0f - pfLeft2, pfRight2), Mathf.Max(0f - pfTop, pfBottom, 0f - pfTop2, pfBottom2));
			textureBounds = new VRTextureBounds_t[2];
			textureBounds[0].uMin = 0.5f + 0.5f * pfLeft / tanHalfFov.x;
			textureBounds[0].uMax = 0.5f + 0.5f * pfRight / tanHalfFov.x;
			textureBounds[0].vMin = 0.5f - 0.5f * pfBottom / tanHalfFov.y;
			textureBounds[0].vMax = 0.5f - 0.5f * pfTop / tanHalfFov.y;
			textureBounds[1].uMin = 0.5f + 0.5f * pfLeft2 / tanHalfFov.x;
			textureBounds[1].uMax = 0.5f + 0.5f * pfRight2 / tanHalfFov.x;
			textureBounds[1].vMin = 0.5f - 0.5f * pfBottom2 / tanHalfFov.y;
			textureBounds[1].vMax = 0.5f - 0.5f * pfTop2 / tanHalfFov.y;
			sceneWidth /= Mathf.Max(textureBounds[0].uMax - textureBounds[0].uMin, textureBounds[1].uMax - textureBounds[1].uMin);
			sceneHeight /= Mathf.Max(textureBounds[0].vMax - textureBounds[0].vMin, textureBounds[1].vMax - textureBounds[1].vMin);
			aspect = tanHalfFov.x / tanHalfFov.y;
			fieldOfView = 2f * Mathf.Atan(tanHalfFov.y) * 57.29578f;
			eyes = new SteamVR_Utils.RigidTransform[2]
			{
				new SteamVR_Utils.RigidTransform(hmd.GetEyeToHeadTransform(EVREye.Eye_Left)),
				new SteamVR_Utils.RigidTransform(hmd.GetEyeToHeadTransform(EVREye.Eye_Right))
			};
			switch (SystemInfo.graphicsDeviceType)
			{
			case GraphicsDeviceType.OpenGLES2:
			case GraphicsDeviceType.OpenGLES3:
			case GraphicsDeviceType.OpenGLCore:
				textureType = ETextureType.OpenGL;
				break;
			case GraphicsDeviceType.Vulkan:
				textureType = ETextureType.Vulkan;
				break;
			default:
				textureType = ETextureType.DirectX;
				break;
			}
			SteamVR_Events.Initializing.Listen(OnInitializing);
			SteamVR_Events.Calibrating.Listen(OnCalibrating);
			SteamVR_Events.OutOfRange.Listen(OnOutOfRange);
			SteamVR_Events.DeviceConnected.Listen(OnDeviceConnected);
			SteamVR_Events.NewPoses.Listen(OnNewPoses);
		}

		public static void Initialize(bool forceUnityVRMode = false)
		{
			if (forceUnityVRMode)
			{
				SteamVR_Behaviour.instance.InitializeSteamVR(forceUnityVRToOpenVR: true);
				return;
			}
			if (_instance == null)
			{
				_instance = CreateInstance();
				if (_instance == null)
				{
					_enabled = false;
				}
			}
			if (_enabled)
			{
				SteamVR_Behaviour.Initialize(forceUnityVRMode);
			}
		}

		private static void ReportGeneralErrors()
		{
			string text = "<b>[SteamVR]</b> Initialization failed. ";
			if (!VRSettings.enabled)
			{
				text += "VR may be disabled in player settings. Go to player settings in the editor and check the 'Virtual Reality Supported' checkbox'. ";
			}
			if (VRSettings.supportedDevices != null && VRSettings.supportedDevices.Length > 0)
			{
				if (!VRSettings.supportedDevices.Contains("OpenVR"))
				{
					text += "OpenVR is not in your list of supported virtual reality SDKs. Add it to the list in player settings. ";
				}
				else if (!VRSettings.supportedDevices.First().Contains("OpenVR"))
				{
					text += "OpenVR is not first in your list of supported virtual reality SDKs. <b>This is okay, but if you have an Oculus device plugged in, and Oculus above OpenVR in this list, it will try and use the Oculus SDK instead of OpenVR.</b> ";
				}
			}
			else
			{
				text += "You have no SDKs in your Player Settings list of supported virtual reality SDKs. Add OpenVR to it. ";
			}
			text += "To force OpenVR initialization call SteamVR.Initialize(true). ";
			UnityEngine.Debug.LogWarning(text);
		}

		private static SteamVR CreateInstance()
		{
			initializedState = InitializedStates.Initializing;
			try
			{
				EVRInitError peError = EVRInitError.None;
				if (!usingNativeSupport)
				{
					ReportGeneralErrors();
					initializedState = InitializedStates.InitializeFailure;
					SteamVR_Events.Initialized.Send(arg0: false);
					return null;
				}
				OpenVR.GetGenericInterface("IVRCompositor_022", ref peError);
				if (peError != 0)
				{
					initializedState = InitializedStates.InitializeFailure;
					ReportError(peError);
					ReportGeneralErrors();
					SteamVR_Events.Initialized.Send(arg0: false);
					return null;
				}
				OpenVR.GetGenericInterface("IVROverlay_019", ref peError);
				if (peError != 0)
				{
					initializedState = InitializedStates.InitializeFailure;
					ReportError(peError);
					SteamVR_Events.Initialized.Send(arg0: false);
					return null;
				}
				OpenVR.GetGenericInterface("IVRInput_006", ref peError);
				if (peError != 0)
				{
					initializedState = InitializedStates.InitializeFailure;
					ReportError(peError);
					SteamVR_Events.Initialized.Send(arg0: false);
					return null;
				}
				settings = SteamVR_Settings.instance;
				if (Application.isEditor)
				{
					IdentifyEditorApplication();
				}
				SteamVR_Input.IdentifyActionsFile();
				if (SteamVR_Settings.instance.inputUpdateMode != SteamVR_UpdateModes.Nothing || SteamVR_Settings.instance.poseUpdateMode != SteamVR_UpdateModes.Nothing)
				{
					SteamVR_Input.Initialize();
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError("<b>[SteamVR]</b> " + ex);
				SteamVR_Events.Initialized.Send(arg0: false);
				return null;
			}
			_enabled = true;
			initializedState = InitializedStates.InitializeSuccess;
			SteamVR_Events.Initialized.Send(arg0: true);
			return new SteamVR();
		}

		private static void ReportError(EVRInitError error)
		{
			switch (error)
			{
			case EVRInitError.None:
				break;
			case EVRInitError.VendorSpecific_UnableToConnectToOculusRuntime:
				UnityEngine.Debug.LogWarning("<b>[SteamVR]</b> Initialization Failed!  Make sure device is on, Oculus runtime is installed, and OVRService_*.exe is running.");
				break;
			case EVRInitError.Init_VRClientDLLNotFound:
				UnityEngine.Debug.LogWarning("<b>[SteamVR]</b> Drivers not found!  They can be installed via Steam under Library > Tools.  Visit http://steampowered.com to install Steam.");
				break;
			case EVRInitError.Driver_RuntimeOutOfDate:
				UnityEngine.Debug.LogWarning("<b>[SteamVR]</b> Initialization Failed!  Make sure device's runtime is up to date.");
				break;
			default:
				UnityEngine.Debug.LogWarning("<b>[SteamVR]</b> " + OpenVR.GetStringForHmdError(error));
				break;
			}
		}

		public string GetTrackedDeviceString(uint deviceId)
		{
			ETrackedPropertyError pError = ETrackedPropertyError.TrackedProp_Success;
			uint stringTrackedDeviceProperty = hmd.GetStringTrackedDeviceProperty(deviceId, ETrackedDeviceProperty.Prop_AttachedDeviceId_String, null, 0u, ref pError);
			if (stringTrackedDeviceProperty > 1)
			{
				StringBuilder stringBuilder = new StringBuilder((int)stringTrackedDeviceProperty);
				hmd.GetStringTrackedDeviceProperty(deviceId, ETrackedDeviceProperty.Prop_AttachedDeviceId_String, stringBuilder, stringTrackedDeviceProperty, ref pError);
				return stringBuilder.ToString();
			}
			return null;
		}

		public string GetStringProperty(ETrackedDeviceProperty prop, uint deviceId = 0u)
		{
			ETrackedPropertyError pError = ETrackedPropertyError.TrackedProp_Success;
			uint stringTrackedDeviceProperty = hmd.GetStringTrackedDeviceProperty(deviceId, prop, null, 0u, ref pError);
			if (stringTrackedDeviceProperty > 1)
			{
				StringBuilder stringBuilder = new StringBuilder((int)stringTrackedDeviceProperty);
				hmd.GetStringTrackedDeviceProperty(deviceId, prop, stringBuilder, stringTrackedDeviceProperty, ref pError);
				return stringBuilder.ToString();
			}
			return (pError == ETrackedPropertyError.TrackedProp_Success) ? "<unknown>" : pError.ToString();
		}

		public float GetFloatProperty(ETrackedDeviceProperty prop, uint deviceId = 0u)
		{
			ETrackedPropertyError pError = ETrackedPropertyError.TrackedProp_Success;
			return hmd.GetFloatTrackedDeviceProperty(deviceId, prop, ref pError);
		}

		public static bool InitializeTemporarySession(bool initInput = false)
		{
			if (Application.isEditor)
			{
				EVRInitError peError = EVRInitError.None;
				OpenVR.GetGenericInterface("IVRCompositor_022", ref peError);
				bool flag = peError != EVRInitError.None;
				if (flag)
				{
					EVRInitError peError2 = EVRInitError.None;
					OpenVR.Init(ref peError2, EVRApplicationType.VRApplication_Overlay, string.Empty);
					if (peError2 != 0)
					{
						UnityEngine.Debug.LogError("<b>[SteamVR]</b> Error during OpenVR Init: " + peError2);
						return false;
					}
					IdentifyEditorApplication(showLogs: false);
					SteamVR_Input.IdentifyActionsFile(showLogs: false);
					runningTemporarySession = true;
				}
				if (initInput)
				{
					SteamVR_Input.Initialize(force: true);
				}
				return flag;
			}
			return false;
		}

		public static void ExitTemporarySession()
		{
			if (runningTemporarySession)
			{
				OpenVR.Shutdown();
				runningTemporarySession = false;
			}
		}

		public static string GenerateAppKey()
		{
			string arg = GenerateCleanProductName();
			return $"application.generated.unity.{arg}.exe";
		}

		public static string GenerateCleanProductName()
		{
			string productName = Application.productName;
			if (string.IsNullOrEmpty(productName))
			{
				return "unnamed_product";
			}
			productName = Regex.Replace(Application.productName, "[^\\w\\._]", string.Empty);
			return productName.ToLower();
		}

		private static string GetManifestFile()
		{
			string dataPath = Application.dataPath;
			int num = dataPath.LastIndexOf('/');
			dataPath = dataPath.Remove(num, dataPath.Length - num);
			string text = Path.Combine(dataPath, "unityProject.vrmanifest");
			FileInfo fileInfo = new FileInfo(SteamVR_Settings.instance.actionsFilePath);
			if (File.Exists(text))
			{
				string value = File.ReadAllText(text);
				SteamVR_Input_ManifestFile steamVR_Input_ManifestFile = JsonConvert.DeserializeObject<SteamVR_Input_ManifestFile>(value);
				if (steamVR_Input_ManifestFile != null && steamVR_Input_ManifestFile.applications != null && steamVR_Input_ManifestFile.applications.Count > 0 && steamVR_Input_ManifestFile.applications[0].app_key != SteamVR_Settings.instance.editorAppKey)
				{
					UnityEngine.Debug.Log("<b>[SteamVR]</b> Deleting existing VRManifest because it has a different app key.");
					FileInfo fileInfo2 = new FileInfo(text);
					if (fileInfo2.IsReadOnly)
					{
						fileInfo2.IsReadOnly = false;
					}
					fileInfo2.Delete();
				}
				if (steamVR_Input_ManifestFile != null && steamVR_Input_ManifestFile.applications != null && steamVR_Input_ManifestFile.applications.Count > 0 && steamVR_Input_ManifestFile.applications[0].action_manifest_path != fileInfo.FullName)
				{
					UnityEngine.Debug.Log("<b>[SteamVR]</b> Deleting existing VRManifest because it has a different action manifest path:\nExisting:" + steamVR_Input_ManifestFile.applications[0].action_manifest_path + "\nNew: " + fileInfo.FullName);
					FileInfo fileInfo3 = new FileInfo(text);
					if (fileInfo3.IsReadOnly)
					{
						fileInfo3.IsReadOnly = false;
					}
					fileInfo3.Delete();
				}
			}
			if (!File.Exists(text))
			{
				SteamVR_Input_ManifestFile steamVR_Input_ManifestFile2 = new SteamVR_Input_ManifestFile();
				steamVR_Input_ManifestFile2.source = "Unity";
				SteamVR_Input_ManifestFile_Application steamVR_Input_ManifestFile_Application = new SteamVR_Input_ManifestFile_Application();
				steamVR_Input_ManifestFile_Application.app_key = SteamVR_Settings.instance.editorAppKey;
				steamVR_Input_ManifestFile_Application.action_manifest_path = fileInfo.FullName;
				steamVR_Input_ManifestFile_Application.launch_type = "url";
				steamVR_Input_ManifestFile_Application.url = "steam://launch/";
				steamVR_Input_ManifestFile_Application.strings.Add("en_us", new SteamVR_Input_ManifestFile_ApplicationString
				{
					name = $"{Application.productName} [Testing]"
				});
				steamVR_Input_ManifestFile2.applications = new List<SteamVR_Input_ManifestFile_Application>();
				steamVR_Input_ManifestFile2.applications.Add(steamVR_Input_ManifestFile_Application);
				string contents = JsonConvert.SerializeObject(steamVR_Input_ManifestFile2, Formatting.Indented, new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore
				});
				File.WriteAllText(text, contents);
			}
			return text;
		}

		private static void IdentifyEditorApplication(bool showLogs = true)
		{
			string manifestFile = GetManifestFile();
			EVRApplicationError eVRApplicationError = OpenVR.Applications.AddApplicationManifest(manifestFile, bTemporary: true);
			if (eVRApplicationError != 0)
			{
				UnityEngine.Debug.LogError("<b>[SteamVR]</b> Error adding vr manifest file: " + eVRApplicationError);
			}
			else if (showLogs)
			{
				UnityEngine.Debug.Log("<b>[SteamVR]</b> Successfully added VR manifest to SteamVR");
			}
			int id = Process.GetCurrentProcess().Id;
			EVRApplicationError eVRApplicationError2 = OpenVR.Applications.IdentifyApplication((uint)id, SteamVR_Settings.instance.editorAppKey);
			if (eVRApplicationError2 != 0)
			{
				UnityEngine.Debug.LogError("<b>[SteamVR]</b> Error identifying application: " + eVRApplicationError2);
			}
			else if (showLogs)
			{
				UnityEngine.Debug.Log($"<b>[SteamVR]</b> Successfully identified process as editor project to SteamVR ({SteamVR_Settings.instance.editorAppKey})");
			}
		}

		private void OnInitializing(bool initializing)
		{
			SteamVR.initializing = initializing;
		}

		private void OnCalibrating(bool calibrating)
		{
			SteamVR.calibrating = calibrating;
		}

		private void OnOutOfRange(bool outOfRange)
		{
			SteamVR.outOfRange = outOfRange;
		}

		private void OnDeviceConnected(int i, bool connected)
		{
			SteamVR.connected[i] = connected;
		}

		private void OnNewPoses(TrackedDevicePose_t[] poses)
		{
			ref SteamVR_Utils.RigidTransform reference = ref eyes[0];
			reference = new SteamVR_Utils.RigidTransform(hmd.GetEyeToHeadTransform(EVREye.Eye_Left));
			ref SteamVR_Utils.RigidTransform reference2 = ref eyes[1];
			reference2 = new SteamVR_Utils.RigidTransform(hmd.GetEyeToHeadTransform(EVREye.Eye_Right));
			for (int i = 0; i < poses.Length; i++)
			{
				bool bDeviceIsConnected = poses[i].bDeviceIsConnected;
				if (bDeviceIsConnected != connected[i])
				{
					SteamVR_Events.DeviceConnected.Send(i, bDeviceIsConnected);
				}
			}
			if ((long)poses.Length > 0L)
			{
				ETrackingResult eTrackingResult = poses[0].eTrackingResult;
				bool flag = eTrackingResult == ETrackingResult.Uninitialized;
				if (flag != initializing)
				{
					SteamVR_Events.Initializing.Send(flag);
				}
				bool flag2 = eTrackingResult == ETrackingResult.Calibrating_InProgress || eTrackingResult == ETrackingResult.Calibrating_OutOfRange;
				if (flag2 != calibrating)
				{
					SteamVR_Events.Calibrating.Send(flag2);
				}
				bool flag3 = eTrackingResult == ETrackingResult.Running_OutOfRange || eTrackingResult == ETrackingResult.Calibrating_OutOfRange;
				if (flag3 != outOfRange)
				{
					SteamVR_Events.OutOfRange.Send(flag3);
				}
			}
		}

		~SteamVR()
		{
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			SteamVR_Events.Initializing.Remove(OnInitializing);
			SteamVR_Events.Calibrating.Remove(OnCalibrating);
			SteamVR_Events.OutOfRange.Remove(OnOutOfRange);
			SteamVR_Events.DeviceConnected.Remove(OnDeviceConnected);
			SteamVR_Events.NewPoses.Remove(OnNewPoses);
			_instance = null;
		}

		public static void SafeDispose()
		{
			if (_instance != null)
			{
				_instance.Dispose();
			}
		}
	}
}
