using UnityEngine;
using UnityEngine.VR;
using Valve.VR;

namespace FistVR
{
	public class GM : ManagerSingleton<GM>
	{
		private GameOptions m_options = new GameOptions();

		private OmniMaster m_omni = new OmniMaster();

		private RewardSystem m_rewardSystem = new RewardSystem();

		private WurstWorldSaveGame wwSaveGame = new WurstWorldSaveGame();

		private TAHFlags tahSettings = new TAHFlags();

		private TNHFlags tnhOptions = new TNHFlags();

		private MeatmasFlags mmFlags = new MeatmasFlags();

		private MF2Flags mfFlags = new MF2Flags();

		private ReturnRotwienersSaveGames rrSaveGames = new ReturnRotwienersSaveGames();

		private FVRSceneSettings m_currentSceneSettings;

		private GamePlannerManager m_currentGamePlannerManager;

		private FVRMovementManager m_currentMovementManager;

		private FVRPlayerBody m_currentPlayerBody;

		private Transform m_playerRoot;

		private AIManager m_aIManager;

		private GameObject m_currentOptionsPanel;

		private GameObject m_currentSpectatorPanel;

		private MeatGrinderMaster mgmaster;

		private TAH_Manager tahmaster;

		private ZosigGameManager zMaster;

		private TNH_Manager tnhmaster;

		public GameObject[] QuickbeltConfigurations;

		public GameObject[] ViveControllerStyles;

		public FVRControllerDefinition[] ControllerDefinitions;

		public GameObject LivPrefab;

		public GameObject SpectatorCamPrefab;

		public GameObject SpectatorCamPreviewPrefab;

		private ControlMode hmdMode;

		private AnvilPrewarmCallback m_loadingCallback;

		public ShaderVariantCollection ShaderVariants;

		private bool m_isDead;

		public static GameOptions Options
		{
			get
			{
				return ManagerSingleton<GM>.Instance.m_options;
			}
			set
			{
				ManagerSingleton<GM>.Instance.m_options = value;
			}
		}

		public static OmniMaster Omni
		{
			get
			{
				return ManagerSingleton<GM>.Instance.m_omni;
			}
			set
			{
				ManagerSingleton<GM>.Instance.m_omni = value;
			}
		}

		public static RewardSystem Rewards
		{
			get
			{
				return ManagerSingleton<GM>.Instance.m_rewardSystem;
			}
			set
			{
				ManagerSingleton<GM>.Instance.m_rewardSystem = value;
			}
		}

		public static WurstWorldSaveGame WWSaveGame
		{
			get
			{
				return ManagerSingleton<GM>.Instance.wwSaveGame;
			}
			set
			{
				ManagerSingleton<GM>.Instance.wwSaveGame = value;
			}
		}

		public static TAHFlags TAHSettings
		{
			get
			{
				return ManagerSingleton<GM>.Instance.tahSettings;
			}
			set
			{
				ManagerSingleton<GM>.Instance.tahSettings = value;
			}
		}

		public static TNHFlags TNHOptions
		{
			get
			{
				return ManagerSingleton<GM>.Instance.tnhOptions;
			}
			set
			{
				ManagerSingleton<GM>.Instance.tnhOptions = value;
			}
		}

		public static MeatmasFlags MMFlags
		{
			get
			{
				return ManagerSingleton<GM>.Instance.mmFlags;
			}
			set
			{
				ManagerSingleton<GM>.Instance.mmFlags = value;
			}
		}

		public static MF2Flags MFFlags
		{
			get
			{
				return ManagerSingleton<GM>.Instance.mfFlags;
			}
			set
			{
				ManagerSingleton<GM>.Instance.mfFlags = value;
			}
		}

		public static ReturnRotwienersSaveGames ROTRWSaves
		{
			get
			{
				return ManagerSingleton<GM>.Instance.rrSaveGames;
			}
			set
			{
				ManagerSingleton<GM>.Instance.rrSaveGames = value;
			}
		}

		public static FVRSceneSettings CurrentSceneSettings => ManagerSingleton<GM>.Instance.m_currentSceneSettings;

		public static GamePlannerManager CurrentGamePlannerManager => ManagerSingleton<GM>.Instance.m_currentGamePlannerManager;

		public static FVRMovementManager CurrentMovementManager => ManagerSingleton<GM>.Instance.m_currentMovementManager;

		public static FVRPlayerBody CurrentPlayerBody => ManagerSingleton<GM>.Instance.m_currentPlayerBody;

		public static Transform CurrentPlayerRoot => ManagerSingleton<GM>.Instance.m_playerRoot;

		public static AIManager CurrentAIManager
		{
			get
			{
				return ManagerSingleton<GM>.Instance.m_aIManager;
			}
			set
			{
				ManagerSingleton<GM>.Instance.m_aIManager = value;
			}
		}

		public static GameObject CurrentOptionsPanel
		{
			get
			{
				return ManagerSingleton<GM>.Instance.m_currentOptionsPanel;
			}
			set
			{
				ManagerSingleton<GM>.Instance.m_currentOptionsPanel = value;
			}
		}

		public static GameObject CurrentSpectatorPanel
		{
			get
			{
				return ManagerSingleton<GM>.Instance.m_currentSpectatorPanel;
			}
			set
			{
				ManagerSingleton<GM>.Instance.m_currentSpectatorPanel = value;
			}
		}

		public static MeatGrinderMaster MGMaster
		{
			get
			{
				return ManagerSingleton<GM>.Instance.mgmaster;
			}
			set
			{
				ManagerSingleton<GM>.Instance.mgmaster = value;
			}
		}

		public static TAH_Manager TAHMaster
		{
			get
			{
				return ManagerSingleton<GM>.Instance.tahmaster;
			}
			set
			{
				ManagerSingleton<GM>.Instance.tahmaster = value;
			}
		}

		public static ZosigGameManager ZMaster
		{
			get
			{
				return ManagerSingleton<GM>.Instance.zMaster;
			}
			set
			{
				ManagerSingleton<GM>.Instance.zMaster = value;
			}
		}

		public static TNH_Manager TNH_Manager
		{
			get
			{
				return ManagerSingleton<GM>.Instance.tnhmaster;
			}
			set
			{
				ManagerSingleton<GM>.Instance.tnhmaster = value;
			}
		}

		public static ControlMode HMDMode
		{
			get
			{
				return ManagerSingleton<GM>.Instance.hmdMode;
			}
			set
			{
				ManagerSingleton<GM>.Instance.hmdMode = value;
			}
		}

		public static AnvilPrewarmCallback LoadingCallback
		{
			get
			{
				return ManagerSingleton<GM>.Instance.m_loadingCallback;
			}
			set
			{
				ManagerSingleton<GM>.Instance.m_loadingCallback = value;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			Options.InitializeFromSaveFile();
			Omni.InitializeFromSaveFile();
			Omni.InitializeUnlocks();
			Rewards.InitializeFromSaveFile();
			WWSaveGame.InitializeFromSaveFile();
			TAHSettings.InitializeFromSaveFile();
			TNHOptions.InitializeFromSaveFile();
			MMFlags.InitializeFromSaveFile();
			MFFlags.InitializeFromSaveFile();
			ROTRWSaves.InitializeFromSaveFile();
			InitScene();
			ShaderVariants.WarmUp();
			LoadingCallback = AnvilManager.PreloadBundleAsync("assets_resources_objectids_weaponry_ammunition");
			LoadingCallback.AddCallback(delegate
			{
				Debug.Log("Prewarm Completed Lambda");
			});
			string model = VRDevice.model;
			model = model.ToLower();
			if (model.Contains("index") || model.Contains("utah"))
			{
				ManagerSingleton<GM>.Instance.hmdMode = ControlMode.Index;
			}
			else if (model.Contains("vive"))
			{
				ManagerSingleton<GM>.Instance.hmdMode = ControlMode.Vive;
			}
			else if (model.Contains("oculus") || model.Contains("rift"))
			{
				ManagerSingleton<GM>.Instance.hmdMode = ControlMode.Oculus;
			}
			else
			{
				ManagerSingleton<GM>.Instance.hmdMode = ControlMode.WMR;
			}
		}

		private void OnLevelWasLoaded(int level)
		{
			InitScene();
			ResetPlayer();
		}

		public void RefreshGravity()
		{
			switch (Options.SimulationOptions.ObjectGravityMode)
			{
			case SimulationOptions.GravityMode.Realistic:
				Physics.gravity = Vector3.down * 9.81f;
				break;
			case SimulationOptions.GravityMode.Playful:
				Physics.gravity = Vector3.down * 5f;
				break;
			case SimulationOptions.GravityMode.OnTheMoon:
				Physics.gravity = Vector3.down * 1.62f;
				break;
			case SimulationOptions.GravityMode.None:
				Physics.gravity = Vector3.zero;
				break;
			}
		}

		private void InitScene()
		{
			Debug.Log("Initializing Scene");
			m_currentSceneSettings = Object.FindObjectOfType<FVRSceneSettings>();
			if (m_currentSceneSettings != null)
			{
				m_currentSceneSettings.Init();
			}
			m_currentMovementManager = Object.FindObjectOfType<FVRMovementManager>();
			if (m_currentMovementManager != null)
			{
				m_currentMovementManager.Init(m_currentSceneSettings);
			}
			m_currentPlayerBody = Object.FindObjectOfType<FVRPlayerBody>();
			if (m_currentPlayerBody != null)
			{
				m_currentPlayerBody.Init(m_currentSceneSettings);
				m_playerRoot = m_currentPlayerBody.transform;
			}
			m_currentGamePlannerManager = Object.FindObjectOfType<GamePlannerManager>();
			if (m_currentGamePlannerManager != null)
			{
				m_currentGamePlannerManager.Init();
			}
		}

		public static void RefreshQuality()
		{
			QualitySettings.SetQualityLevel((int)Options.PerformanceOptions.CurrentQualitySetting);
			if (CurrentSceneSettings != null)
			{
				CurrentSceneSettings.UpdateGlobalPostVolumes();
			}
			if (CurrentPlayerBody != null)
			{
				CurrentPlayerBody.UpdateCameraPost();
			}
		}

		public static float GetPlayerHealth()
		{
			if (CurrentPlayerBody == null)
			{
				return 1f;
			}
			return CurrentPlayerBody.GetPlayerHealth();
		}

		public static bool IsDead()
		{
			return ManagerSingleton<GM>.Instance.m_isDead;
		}

		public void KillPlayer(bool KilledSelf)
		{
			m_isDead = true;
			CurrentPlayerBody.DisableHands();
			CurrentPlayerBody.DisableHitBoxes();
			SteamVR_Fade.Start(Color.black, CurrentSceneSettings.PlayerDeathFade);
			CurrentSceneSettings.OnPlayerDeath(KilledSelf);
			if (CurrentSceneSettings.DoesPlayerRespawnOnDeath)
			{
				if (CurrentSceneSettings.OnDeathSendMessageTarget != null)
				{
					CurrentSceneSettings.OnDeathSendMessageTarget.SendMessage(CurrentSceneSettings.OneDeathSendMessage);
				}
				Invoke("BringBackPlayer", CurrentSceneSettings.PlayerRespawnLoadDelay);
			}
			else
			{
				Invoke("LoadDeathLevel", CurrentSceneSettings.PlayerRespawnLoadDelay);
			}
		}

		public void BringBackPlayer()
		{
			CurrentMovementManager.TeleportToPoint(CurrentSceneSettings.DeathResetPoint.position, isAbsolute: true);
			SteamVR_Fade.Start(Color.clear, 0.5f);
			ResetPlayer();
			CurrentPlayerBody.EnableHands();
			CurrentPlayerBody.EnableHitBoxes();
		}

		public void LoadDeathLevel()
		{
			ResetPlayer();
			SteamVR_LoadLevel.Begin(CurrentSceneSettings.SceneToLoadOnDeath);
		}

		public void ResetPlayer()
		{
			if (CurrentPlayerBody != null)
			{
				CurrentPlayerBody.ResetHealth();
			}
			m_isDead = false;
		}
	}
}
