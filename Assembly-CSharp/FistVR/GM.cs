// Decompiled with JetBrains decompiler
// Type: FistVR.GM
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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
      get => ManagerSingleton<GM>.Instance.m_options;
      set => ManagerSingleton<GM>.Instance.m_options = value;
    }

    public static OmniMaster Omni
    {
      get => ManagerSingleton<GM>.Instance.m_omni;
      set => ManagerSingleton<GM>.Instance.m_omni = value;
    }

    public static RewardSystem Rewards
    {
      get => ManagerSingleton<GM>.Instance.m_rewardSystem;
      set => ManagerSingleton<GM>.Instance.m_rewardSystem = value;
    }

    public static WurstWorldSaveGame WWSaveGame
    {
      get => ManagerSingleton<GM>.Instance.wwSaveGame;
      set => ManagerSingleton<GM>.Instance.wwSaveGame = value;
    }

    public static TAHFlags TAHSettings
    {
      get => ManagerSingleton<GM>.Instance.tahSettings;
      set => ManagerSingleton<GM>.Instance.tahSettings = value;
    }

    public static TNHFlags TNHOptions
    {
      get => ManagerSingleton<GM>.Instance.tnhOptions;
      set => ManagerSingleton<GM>.Instance.tnhOptions = value;
    }

    public static MeatmasFlags MMFlags
    {
      get => ManagerSingleton<GM>.Instance.mmFlags;
      set => ManagerSingleton<GM>.Instance.mmFlags = value;
    }

    public static MF2Flags MFFlags
    {
      get => ManagerSingleton<GM>.Instance.mfFlags;
      set => ManagerSingleton<GM>.Instance.mfFlags = value;
    }

    public static ReturnRotwienersSaveGames ROTRWSaves
    {
      get => ManagerSingleton<GM>.Instance.rrSaveGames;
      set => ManagerSingleton<GM>.Instance.rrSaveGames = value;
    }

    public static FVRSceneSettings CurrentSceneSettings => ManagerSingleton<GM>.Instance.m_currentSceneSettings;

    public static GamePlannerManager CurrentGamePlannerManager => ManagerSingleton<GM>.Instance.m_currentGamePlannerManager;

    public static FVRMovementManager CurrentMovementManager => ManagerSingleton<GM>.Instance.m_currentMovementManager;

    public static FVRPlayerBody CurrentPlayerBody => ManagerSingleton<GM>.Instance.m_currentPlayerBody;

    public static Transform CurrentPlayerRoot => ManagerSingleton<GM>.Instance.m_playerRoot;

    public static AIManager CurrentAIManager
    {
      get => ManagerSingleton<GM>.Instance.m_aIManager;
      set => ManagerSingleton<GM>.Instance.m_aIManager = value;
    }

    public static GameObject CurrentOptionsPanel
    {
      get => ManagerSingleton<GM>.Instance.m_currentOptionsPanel;
      set => ManagerSingleton<GM>.Instance.m_currentOptionsPanel = value;
    }

    public static GameObject CurrentSpectatorPanel
    {
      get => ManagerSingleton<GM>.Instance.m_currentSpectatorPanel;
      set => ManagerSingleton<GM>.Instance.m_currentSpectatorPanel = value;
    }

    public static MeatGrinderMaster MGMaster
    {
      get => ManagerSingleton<GM>.Instance.mgmaster;
      set => ManagerSingleton<GM>.Instance.mgmaster = value;
    }

    public static TAH_Manager TAHMaster
    {
      get => ManagerSingleton<GM>.Instance.tahmaster;
      set => ManagerSingleton<GM>.Instance.tahmaster = value;
    }

    public static ZosigGameManager ZMaster
    {
      get => ManagerSingleton<GM>.Instance.zMaster;
      set => ManagerSingleton<GM>.Instance.zMaster = value;
    }

    public static TNH_Manager TNH_Manager
    {
      get => ManagerSingleton<GM>.Instance.tnhmaster;
      set => ManagerSingleton<GM>.Instance.tnhmaster = value;
    }

    public static ControlMode HMDMode
    {
      get => ManagerSingleton<GM>.Instance.hmdMode;
      set => ManagerSingleton<GM>.Instance.hmdMode = value;
    }

    public static AnvilPrewarmCallback LoadingCallback
    {
      get => ManagerSingleton<GM>.Instance.m_loadingCallback;
      set => ManagerSingleton<GM>.Instance.m_loadingCallback = value;
    }

    protected override void Awake()
    {
      base.Awake();
      GM.Options.InitializeFromSaveFile();
      GM.Omni.InitializeFromSaveFile();
      GM.Omni.InitializeUnlocks();
      GM.Rewards.InitializeFromSaveFile();
      GM.WWSaveGame.InitializeFromSaveFile();
      GM.TAHSettings.InitializeFromSaveFile();
      GM.TNHOptions.InitializeFromSaveFile();
      GM.MMFlags.InitializeFromSaveFile();
      GM.MFFlags.InitializeFromSaveFile();
      GM.ROTRWSaves.InitializeFromSaveFile();
      this.InitScene();
      this.ShaderVariants.WarmUp();
      GM.LoadingCallback = AnvilManager.PreloadBundleAsync("assets_resources_objectids_weaponry_ammunition");
      GM.LoadingCallback.AddCallback((System.Action) (() => Debug.Log((object) "Prewarm Completed Lambda")));
      string lower = VRDevice.model.ToLower();
      if (lower.Contains("index") || lower.Contains("utah"))
        ManagerSingleton<GM>.Instance.hmdMode = ControlMode.Index;
      else if (lower.Contains("vive"))
        ManagerSingleton<GM>.Instance.hmdMode = ControlMode.Vive;
      else if (lower.Contains("oculus") || lower.Contains("rift"))
        ManagerSingleton<GM>.Instance.hmdMode = ControlMode.Oculus;
      else
        ManagerSingleton<GM>.Instance.hmdMode = ControlMode.WMR;
    }

    private void OnLevelWasLoaded(int level)
    {
      this.InitScene();
      this.ResetPlayer();
    }

    public void RefreshGravity()
    {
      switch (GM.Options.SimulationOptions.ObjectGravityMode)
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
      Debug.Log((object) "Initializing Scene");
      this.m_currentSceneSettings = UnityEngine.Object.FindObjectOfType<FVRSceneSettings>();
      if ((UnityEngine.Object) this.m_currentSceneSettings != (UnityEngine.Object) null)
        this.m_currentSceneSettings.Init();
      this.m_currentMovementManager = UnityEngine.Object.FindObjectOfType<FVRMovementManager>();
      if ((UnityEngine.Object) this.m_currentMovementManager != (UnityEngine.Object) null)
        this.m_currentMovementManager.Init(this.m_currentSceneSettings);
      this.m_currentPlayerBody = UnityEngine.Object.FindObjectOfType<FVRPlayerBody>();
      if ((UnityEngine.Object) this.m_currentPlayerBody != (UnityEngine.Object) null)
      {
        this.m_currentPlayerBody.Init(this.m_currentSceneSettings);
        this.m_playerRoot = this.m_currentPlayerBody.transform;
      }
      this.m_currentGamePlannerManager = UnityEngine.Object.FindObjectOfType<GamePlannerManager>();
      if (!((UnityEngine.Object) this.m_currentGamePlannerManager != (UnityEngine.Object) null))
        return;
      this.m_currentGamePlannerManager.Init();
    }

    public static void RefreshQuality()
    {
      QualitySettings.SetQualityLevel((int) GM.Options.PerformanceOptions.CurrentQualitySetting);
      if ((UnityEngine.Object) GM.CurrentSceneSettings != (UnityEngine.Object) null)
        GM.CurrentSceneSettings.UpdateGlobalPostVolumes();
      if (!((UnityEngine.Object) GM.CurrentPlayerBody != (UnityEngine.Object) null))
        return;
      GM.CurrentPlayerBody.UpdateCameraPost();
    }

    public static float GetPlayerHealth() => (UnityEngine.Object) GM.CurrentPlayerBody == (UnityEngine.Object) null ? 1f : GM.CurrentPlayerBody.GetPlayerHealth();

    public static bool IsDead() => ManagerSingleton<GM>.Instance.m_isDead;

    public void KillPlayer(bool KilledSelf)
    {
      this.m_isDead = true;
      GM.CurrentPlayerBody.DisableHands();
      GM.CurrentPlayerBody.DisableHitBoxes();
      SteamVR_Fade.Start(Color.black, GM.CurrentSceneSettings.PlayerDeathFade);
      GM.CurrentSceneSettings.OnPlayerDeath(KilledSelf);
      if (GM.CurrentSceneSettings.DoesPlayerRespawnOnDeath)
      {
        if ((UnityEngine.Object) GM.CurrentSceneSettings.OnDeathSendMessageTarget != (UnityEngine.Object) null)
          GM.CurrentSceneSettings.OnDeathSendMessageTarget.SendMessage(GM.CurrentSceneSettings.OneDeathSendMessage);
        this.Invoke("BringBackPlayer", GM.CurrentSceneSettings.PlayerRespawnLoadDelay);
      }
      else
        this.Invoke("LoadDeathLevel", GM.CurrentSceneSettings.PlayerRespawnLoadDelay);
    }

    public void BringBackPlayer()
    {
      double point = (double) GM.CurrentMovementManager.TeleportToPoint(GM.CurrentSceneSettings.DeathResetPoint.position, true);
      SteamVR_Fade.Start(Color.clear, 0.5f);
      this.ResetPlayer();
      GM.CurrentPlayerBody.EnableHands();
      GM.CurrentPlayerBody.EnableHitBoxes();
    }

    public void LoadDeathLevel()
    {
      this.ResetPlayer();
      SteamVR_LoadLevel.Begin(GM.CurrentSceneSettings.SceneToLoadOnDeath);
    }

    public void ResetPlayer()
    {
      if ((UnityEngine.Object) GM.CurrentPlayerBody != (UnityEngine.Object) null)
        GM.CurrentPlayerBody.ResetHealth();
      this.m_isDead = false;
    }
  }
}
