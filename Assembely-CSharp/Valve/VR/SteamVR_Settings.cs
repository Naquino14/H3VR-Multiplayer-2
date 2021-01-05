using UnityEngine;
using UnityEngine.Serialization;

namespace Valve.VR
{
	public class SteamVR_Settings : ScriptableObject
	{
		private static SteamVR_Settings _instance;

		public bool pauseGameWhenDashboardVisible = true;

		public bool lockPhysicsUpdateRateToRenderFrequency = true;

		[SerializeField]
		[FormerlySerializedAs("trackingSpace")]
		private ETrackingUniverseOrigin trackingSpaceOrigin = ETrackingUniverseOrigin.TrackingUniverseStanding;

		[Tooltip("Filename local to the project root (or executable, in a build)")]
		public string actionsFilePath = "actions.json";

		[Tooltip("Path local to the Assets folder")]
		public string steamVRInputPath = "SteamVR_Input";

		public SteamVR_UpdateModes inputUpdateMode = SteamVR_UpdateModes.OnUpdate;

		public SteamVR_UpdateModes poseUpdateMode = SteamVR_UpdateModes.OnPreCull;

		public bool activateFirstActionSetOnStart = true;

		[Tooltip("This is the app key the unity editor will use to identify your application. (can be \"steam.app.[appid]\" to persist bindings between editor steam)")]
		public string editorAppKey;

		[Tooltip("The SteamVR Plugin can automatically make sure VR is enabled in your player settings and if not, enable it.")]
		public bool autoEnableVR = true;

		public static SteamVR_Settings instance
		{
			get
			{
				LoadInstance();
				return _instance;
			}
		}

		public ETrackingUniverseOrigin trackingSpace
		{
			get
			{
				return trackingSpaceOrigin;
			}
			set
			{
				trackingSpaceOrigin = value;
				if (SteamVR_Behaviour.isPlaying)
				{
					SteamVR_Action_Pose.SetTrackingUniverseOrigin(trackingSpaceOrigin);
				}
			}
		}

		public bool IsInputUpdateMode(SteamVR_UpdateModes tocheck)
		{
			return (inputUpdateMode & tocheck) == tocheck;
		}

		public bool IsPoseUpdateMode(SteamVR_UpdateModes tocheck)
		{
			return (poseUpdateMode & tocheck) == tocheck;
		}

		public static void VerifyScriptableObject()
		{
			LoadInstance();
		}

		private static void LoadInstance()
		{
			if (_instance == null)
			{
				_instance = Resources.Load<SteamVR_Settings>("SteamVR_Settings");
				if (_instance == null)
				{
					_instance = ScriptableObject.CreateInstance<SteamVR_Settings>();
				}
				if (string.IsNullOrEmpty(_instance.editorAppKey))
				{
					_instance.editorAppKey = SteamVR.GenerateAppKey();
					Debug.Log("<b>[SteamVR Setup]</b> Generated you an editor app key of: " + _instance.editorAppKey + ". This lets the editor tell SteamVR what project this is. Has no effect on builds. This can be changed in Assets/SteamVR/Resources/SteamVR_Settings");
				}
			}
		}
	}
}
