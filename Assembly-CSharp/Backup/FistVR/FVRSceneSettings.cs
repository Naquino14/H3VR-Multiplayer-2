// Decompiled with JetBrains decompiler
// Type: FistVR.FVRSceneSettings
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace FistVR
{
  public class FVRSceneSettings : MonoBehaviour
  {
    [Header("PlayerAffordance")]
    public bool IsSpawnLockingEnabled = true;
    public bool AreHitboxesEnabled;
    public bool DoesDamageGetRegistered;
    public float MaxPointingDistance = 1f;
    public float MaxProjectileRange = 500f;
    public bool ForcesCasingDespawn;
    [Header("Locomotion Options")]
    public bool IsLocoTeleportAllowed = true;
    public bool IsLocoSlideAllowed = true;
    public bool IsLocoDashAllowed = true;
    public bool IsLocoTouchpadAllowed = true;
    public bool IsLocoArmSwingerAllowed = true;
    public bool DoesTeleportUseCooldown;
    public bool DoesAllowAirControl;
    public bool UsesMaxSpeedClamp;
    public float MaxSpeedClamp = 3f;
    [Header("Player Catching Options")]
    public bool UsesPlayerCatcher = true;
    public float CatchHeight = -50f;
    [Header("Player Respawn Options")]
    public int DefaultPlayerIFF;
    public bool DoesPlayerRespawnOnDeath = true;
    public float PlayerDeathFade = 3f;
    public float PlayerRespawnLoadDelay = 3.5f;
    public string SceneToLoadOnDeath = string.Empty;
    public bool DoesUseHealthBar;
    public bool IsQuickbeltSwappingAllowed = true;
    public bool AreQuickbeltSlotsEnabled;
    public bool ConfigQuickbeltOnLoad;
    public int QuickbeltToConfig;
    public bool IsSceneLowLight;
    public bool IsAmmoInfinite;
    public bool AllowsInfiniteAmmoMags = true;
    public bool UsesUnlockSystem;
    public Transform DeathResetPoint;
    public GameObject OnDeathSendMessageTarget;
    public string OneDeathSendMessage;
    public List<GameObject> ShotEventReceivers;
    public int HowManyToShotReceivePerFrame = 10;
    private HashSet<GameObject> m_shotEventReceiversInQueue = new HashSet<GameObject>();
    private Queue<GameObject> m_shotEventReceiversQueue = new Queue<GameObject>();
    private Vector3 m_curNotifyPos = Vector3.zero;
    [Header("QuitReceivers")]
    public List<GameObject> QuitReceivers;
    [Header("Audio Stuff")]
    public FVRSoundEnvironment DefaultSoundEnvironment;
    public float BaseLoudness = 5f;
    public bool UsesWeaponHandlingAISound;
    public float MaxImpactSoundEventDistance = 15f;
    [Header("Post FX Volumes")]
    public PostProcessVolume Vol_AO;
    public PostProcessVolume Vol_Bloom;
    public PostProcessVolume Vol_CC;
    public List<GameObject> Post_SnakeEyes;
    public List<GameObject> Post_MoleEye;
    public List<GameObject> Post_Blort;
    public List<GameObject> Post_Dlort;
    public List<GameObject> Post_FarOutMan;
    public List<GameObject> Post_BadTrip;
    [Header("PowerUpStuff")]
    public Transform PowerupPoint_HomeTown;
    public Transform PowerupPoint_InverseHomeTown;
    public List<Transform> PowerPoints_WheredIGo_TP;
    public List<Transform> PowerPoints_WheredIGo_Grav;
    private Camera m_specCam;
    private Camera m_previewCam;
    private PostProcessLayer m_postLayer;
    private Vector3 smoothVel = Vector3.zero;
    private Vector3 m_camForward = Vector3.zero;
    private Vector3 m_camForwardVel = Vector3.zero;
    private Vector3 m_camUp = Vector3.zero;
    private Vector3 m_camUpVel = Vector3.zero;
    private Vector3 localPosHead = Vector3.one;
    private SosigLink m_attachedLink;
    private Transform m_camObjectPoint;
    private float[] TPCDistances = new float[5]
    {
      0.5f,
      0.75f,
      1f,
      1.25f,
      1.5f
    };
    private float[] TPCSideOffsets = new float[5]
    {
      0.18f,
      0.25f,
      0.35f,
      0.45f,
      0.55f
    };
    private RaycastHit m_hit;
    private int numSuppressingEventsThisFrame;

    private void Awake() => ManagerBootStrap.BootStrap();

    private void Start()
    {
      GM.RefreshQuality();
      SM.WarmupGenericPools();
      SM.SetReverbEnvironment(this.DefaultSoundEnvironment);
      this.Invoke("ConfigLiv", 0.1f);
      if (this.ConfigQuickbeltOnLoad)
        this.Invoke("DelayedConfig", 0.2f);
      if ((Object) this.PowerupPoint_HomeTown == (Object) null)
        this.PowerupPoint_HomeTown = this.DeathResetPoint;
      if ((Object) this.PowerupPoint_InverseHomeTown == (Object) null)
        this.PowerupPoint_HomeTown = this.PowerupPoint_InverseHomeTown;
      if (this.PowerPoints_WheredIGo_TP.Count < 1)
        this.PowerPoints_WheredIGo_TP.Add(this.PowerupPoint_InverseHomeTown);
      if (this.PowerPoints_WheredIGo_Grav.Count < 1)
        this.PowerPoints_WheredIGo_Grav.Add(this.PowerupPoint_InverseHomeTown);
      Debug.Log((object) ("Scene delta is:" + (object) Time.fixedDeltaTime));
    }

    private void DelayedConfig() => GM.CurrentPlayerBody.ConfigureQuickbelt(this.QuickbeltToConfig);

    public void SetAttachedLink(SosigLink l) => this.m_attachedLink = l;

    public void SetCamObjectPoint(Transform t) => this.m_camObjectPoint = t;

    public Transform GetCamObjectPoint() => this.m_camObjectPoint;

    private void UpdateSpectatorCamera()
    {
      if ((Object) this.m_specCam != (Object) null)
      {
        switch (GM.Options.ControlOptions.CamQual)
        {
          case ControlOptions.DesktopRenderQuality.Low:
            this.m_postLayer.enabled = false;
            this.m_specCam.allowMSAA = false;
            break;
          case ControlOptions.DesktopRenderQuality.Med:
            this.m_postLayer.enabled = false;
            this.m_specCam.allowMSAA = true;
            break;
          case ControlOptions.DesktopRenderQuality.High:
            this.m_postLayer.enabled = true;
            this.m_specCam.allowMSAA = true;
            break;
        }
      }
      ControlOptions.DesktopCameraMode camMode = GM.Options.ControlOptions.CamMode;
      float num = 0.032f;
      if (GM.Options.ControlOptions.CamEye == ControlOptions.DesktopCameraEye.Left)
        num = -num;
      float camFov = GM.Options.ControlOptions.CamFOV;
      float camSmoothingLinear = GM.Options.ControlOptions.CamSmoothingLinear;
      float smoothingRotational = GM.Options.ControlOptions.CamSmoothingRotational;
      float camLeveling = GM.Options.ControlOptions.CamLeveling;
      Vector3 up = Vector3.up;
      Vector3 b1 = Vector3.up;
      switch (camMode)
      {
        case ControlOptions.DesktopCameraMode.Default:
          if ((Object) this.m_specCam != (Object) null && this.m_specCam.gameObject.activeSelf)
          {
            this.m_specCam.gameObject.SetActive(false);
            break;
          }
          break;
        case ControlOptions.DesktopCameraMode.HDSpectator:
          if ((Object) this.m_specCam == (Object) null)
          {
            GameObject gameObject = Object.Instantiate<GameObject>(ManagerSingleton<GM>.Instance.SpectatorCamPrefab, GM.CurrentPlayerBody.Head.position, GM.CurrentPlayerBody.Head.rotation);
            this.m_specCam = gameObject.GetComponent<Camera>();
            this.m_camForward = GM.CurrentPlayerBody.Head.forward;
            this.m_camUp = GM.CurrentPlayerBody.Head.up;
            this.localPosHead = GM.CurrentPlayerBody.Head.localPosition;
            this.m_postLayer = gameObject.GetComponent<PostProcessLayer>();
          }
          if (!this.m_specCam.gameObject.activeSelf)
            this.m_specCam.gameObject.SetActive(true);
          if ((double) camSmoothingLinear < 0.100000001490116)
          {
            this.m_specCam.transform.position = GM.CurrentPlayerBody.Head.position + GM.CurrentPlayerBody.Head.right * num;
          }
          else
          {
            this.localPosHead = Vector3.SmoothDamp(this.localPosHead, GM.CurrentPlayerBody.Head.localPosition + GM.CurrentPlayerBody.transform.InverseTransformDirection(GM.CurrentPlayerBody.Head.right) * num, ref this.smoothVel, camSmoothingLinear * 0.1f);
            this.m_specCam.transform.position = GM.CurrentPlayerBody.transform.TransformPoint(this.localPosHead);
          }
          if ((double) smoothingRotational < 0.100000001490116)
          {
            this.m_camUp = GM.CurrentPlayerBody.Head.up;
            this.m_camForward = GM.CurrentPlayerBody.Head.forward;
            up = GM.CurrentPlayerBody.Head.up;
          }
          else
          {
            up = GM.CurrentPlayerBody.Head.up;
            this.m_camForward = Vector3.SmoothDamp(this.m_camForward, GM.CurrentPlayerBody.Head.forward, ref this.m_camForwardVel, smoothingRotational * 0.1f);
            this.m_camUp = Vector3.SmoothDamp(this.m_camUp, GM.CurrentPlayerBody.Head.up, ref this.m_camUpVel, smoothingRotational * 0.1f);
          }
          Vector3 b2 = Vector3.up;
          if ((double) Vector3.Dot(this.m_camUp, -Vector3.up) > 0.0)
            b2 = -Vector3.up;
          this.m_specCam.transform.rotation = Quaternion.LookRotation(this.m_camForward, Vector3.Slerp(this.m_camUp, b2, camLeveling * 0.25f));
          this.m_specCam.fieldOfView = Mathf.MoveTowards(this.m_specCam.fieldOfView, camFov, Time.deltaTime * 15f);
          break;
        case ControlOptions.DesktopCameraMode.ThirdPerson:
          if ((Object) this.m_specCam == (Object) null)
          {
            GameObject gameObject = Object.Instantiate<GameObject>(ManagerSingleton<GM>.Instance.SpectatorCamPrefab, GM.CurrentPlayerBody.Head.position, GM.CurrentPlayerBody.Head.rotation);
            this.m_specCam = gameObject.GetComponent<Camera>();
            this.m_camForward = GM.CurrentPlayerBody.Head.forward;
            this.m_camUp = GM.CurrentPlayerBody.Head.up;
            this.localPosHead = GM.CurrentPlayerBody.Head.localPosition;
            this.m_postLayer = gameObject.GetComponent<PostProcessLayer>();
          }
          if (!this.m_specCam.gameObject.activeSelf)
            this.m_specCam.gameObject.SetActive(true);
          Vector3 zero = Vector3.zero;
          Vector3 origin1;
          if ((double) camSmoothingLinear < 0.100000001490116)
          {
            origin1 = GM.CurrentPlayerBody.Head.position + GM.CurrentPlayerBody.Head.right * num;
          }
          else
          {
            this.localPosHead = Vector3.SmoothDamp(this.localPosHead, GM.CurrentPlayerBody.Head.localPosition + GM.CurrentPlayerBody.transform.InverseTransformDirection(GM.CurrentPlayerBody.Head.right) * num, ref this.smoothVel, camSmoothingLinear * 0.1f);
            origin1 = GM.CurrentPlayerBody.transform.TransformPoint(this.localPosHead);
          }
          if ((double) smoothingRotational < 0.100000001490116)
          {
            this.m_camUp = GM.CurrentPlayerBody.Head.up;
            this.m_camForward = GM.CurrentPlayerBody.Head.forward;
            up = GM.CurrentPlayerBody.Head.up;
          }
          else
          {
            up = GM.CurrentPlayerBody.Head.up;
            this.m_camForward = Vector3.SmoothDamp(this.m_camForward, GM.CurrentPlayerBody.Head.forward, ref this.m_camForwardVel, smoothingRotational * 0.1f);
            this.m_camUp = Vector3.SmoothDamp(this.m_camUp, GM.CurrentPlayerBody.Head.up, ref this.m_camUpVel, smoothingRotational * 0.1f);
          }
          Vector3 b3 = Vector3.up;
          if ((double) Vector3.Dot(this.m_camUp, -Vector3.up) > 0.0)
            b3 = -Vector3.up;
          this.m_specCam.transform.rotation = Quaternion.LookRotation(this.m_camForward, Vector3.Slerp(this.m_camUp, b3, camLeveling * 0.25f));
          this.m_specCam.fieldOfView = Mathf.MoveTowards(this.m_specCam.fieldOfView, camFov, Time.deltaTime * 25f);
          int tpcDistanceIndex = GM.Options.ControlOptions.TPCDistanceIndex;
          int tpcLateralIndex = GM.Options.ControlOptions.TPCLateralIndex;
          float maxDistance1 = this.TPCDistances[tpcDistanceIndex];
          if (Physics.Raycast(origin1, -this.m_camForward, out this.m_hit, maxDistance1, (int) GM.CurrentMovementManager.LM_PointSearch, QueryTriggerInteraction.Ignore))
            maxDistance1 = this.m_hit.distance;
          Vector3 origin2 = origin1 + -this.m_camForward * maxDistance1;
          float maxDistance2 = this.TPCSideOffsets[tpcLateralIndex];
          Vector3 direction = this.m_specCam.transform.right;
          if (GM.Options.ControlOptions.CamEye == ControlOptions.DesktopCameraEye.Left)
            direction = -direction;
          if (Physics.Raycast(origin2, direction, out this.m_hit, maxDistance2, (int) GM.CurrentMovementManager.LM_PointSearch, QueryTriggerInteraction.Ignore))
            maxDistance2 = this.m_hit.distance;
          this.m_specCam.transform.position = origin2 + direction * maxDistance2;
          break;
        case ControlOptions.DesktopCameraMode.SpawnedObject:
          if ((Object) this.m_specCam == (Object) null)
          {
            GameObject gameObject = Object.Instantiate<GameObject>(ManagerSingleton<GM>.Instance.SpectatorCamPrefab, GM.CurrentPlayerBody.Head.position, GM.CurrentPlayerBody.Head.rotation);
            this.m_specCam = gameObject.GetComponent<Camera>();
            this.m_camForward = GM.CurrentPlayerBody.Head.forward;
            this.m_camUp = GM.CurrentPlayerBody.Head.up;
            this.localPosHead = GM.CurrentPlayerBody.Head.localPosition;
            this.m_postLayer = gameObject.GetComponent<PostProcessLayer>();
          }
          if (!this.m_specCam.gameObject.activeSelf)
            this.m_specCam.gameObject.SetActive(true);
          if ((Object) this.m_camObjectPoint != (Object) null)
          {
            if ((double) camSmoothingLinear < 0.100000001490116)
            {
              this.m_specCam.transform.position = this.m_camObjectPoint.position;
            }
            else
            {
              this.localPosHead = Vector3.SmoothDamp(this.localPosHead, this.m_camObjectPoint.position, ref this.smoothVel, camSmoothingLinear * 0.1f);
              this.m_specCam.transform.position = this.localPosHead;
            }
            if ((double) smoothingRotational < 0.100000001490116)
            {
              this.m_camUp = this.m_camObjectPoint.up;
              this.m_camForward = this.m_camObjectPoint.forward;
              up = this.m_camObjectPoint.up;
            }
            else
            {
              up = this.m_camObjectPoint.up;
              this.m_camForward = Vector3.SmoothDamp(this.m_camForward, this.m_camObjectPoint.forward, ref this.m_camForwardVel, smoothingRotational * 0.1f);
              this.m_camUp = Vector3.SmoothDamp(this.m_camUp, this.m_camObjectPoint.up, ref this.m_camUpVel, smoothingRotational * 0.1f);
            }
            if ((double) Vector3.Dot(this.m_camUp, -Vector3.up) > 0.0)
              b1 = -Vector3.up;
            this.m_specCam.transform.rotation = Quaternion.LookRotation(this.m_camForward, Vector3.Slerp(this.m_camUp, b1, camLeveling * 0.25f));
            this.m_specCam.fieldOfView = Mathf.MoveTowards(this.m_specCam.fieldOfView, camFov, Time.deltaTime * 25f);
            break;
          }
          if ((Object) this.m_specCam != (Object) null && this.m_specCam.gameObject.activeSelf)
          {
            this.m_specCam.gameObject.SetActive(false);
            break;
          }
          break;
        case ControlOptions.DesktopCameraMode.SosigView:
          if ((Object) this.m_specCam == (Object) null)
          {
            GameObject gameObject = Object.Instantiate<GameObject>(ManagerSingleton<GM>.Instance.SpectatorCamPrefab, GM.CurrentPlayerBody.Head.position, GM.CurrentPlayerBody.Head.rotation);
            this.m_specCam = gameObject.GetComponent<Camera>();
            this.m_camForward = GM.CurrentPlayerBody.Head.forward;
            this.m_camUp = GM.CurrentPlayerBody.Head.up;
            this.localPosHead = GM.CurrentPlayerBody.Head.localPosition;
            this.m_postLayer = gameObject.GetComponent<PostProcessLayer>();
          }
          if (!this.m_specCam.gameObject.activeSelf)
            this.m_specCam.gameObject.SetActive(true);
          if ((Object) this.m_attachedLink != (Object) null)
          {
            if ((double) camSmoothingLinear < 0.100000001490116)
            {
              this.m_specCam.transform.position = this.m_attachedLink.transform.position;
            }
            else
            {
              this.localPosHead = Vector3.SmoothDamp(this.localPosHead, this.m_attachedLink.transform.position, ref this.smoothVel, camSmoothingLinear * 0.1f);
              this.m_specCam.transform.position = this.localPosHead;
            }
            if ((double) smoothingRotational < 0.100000001490116)
            {
              this.m_camUp = this.m_attachedLink.transform.up;
              this.m_camForward = this.m_attachedLink.transform.forward;
              up = this.m_attachedLink.transform.up;
            }
            else
            {
              up = this.m_attachedLink.transform.up;
              this.m_camForward = Vector3.SmoothDamp(this.m_camForward, this.m_attachedLink.transform.forward, ref this.m_camForwardVel, smoothingRotational * 0.1f);
              this.m_camUp = Vector3.SmoothDamp(this.m_camUp, this.m_attachedLink.transform.up, ref this.m_camUpVel, smoothingRotational * 0.1f);
            }
            if ((double) Vector3.Dot(this.m_camUp, -Vector3.up) > 0.0)
              b1 = -Vector3.up;
            this.m_specCam.transform.rotation = Quaternion.LookRotation(this.m_camForward, Vector3.Slerp(this.m_camUp, b1, camLeveling * 0.25f));
            this.m_specCam.fieldOfView = Mathf.MoveTowards(this.m_specCam.fieldOfView, camFov, Time.deltaTime * 25f);
            break;
          }
          if ((Object) this.m_specCam != (Object) null && this.m_specCam.gameObject.activeSelf)
          {
            this.m_specCam.gameObject.SetActive(false);
            break;
          }
          break;
      }
      if ((Object) this.m_specCam != (Object) null && this.m_specCam.gameObject.activeSelf && GM.Options.ControlOptions.PCamMode == ControlOptions.PreviewCamMode.Enabled)
      {
        if ((Object) this.m_previewCam == (Object) null)
          this.m_previewCam = Object.Instantiate<GameObject>(ManagerSingleton<GM>.Instance.SpectatorCamPreviewPrefab, GM.CurrentPlayerBody.Head.position, GM.CurrentPlayerBody.Head.rotation).GetComponent<Camera>();
        this.m_previewCam.fieldOfView = this.m_specCam.fieldOfView;
        this.m_previewCam.transform.position = this.m_specCam.transform.position;
        this.m_previewCam.transform.rotation = this.m_specCam.transform.rotation;
        if (this.m_previewCam.gameObject.activeSelf)
          return;
        this.m_previewCam.gameObject.SetActive(true);
      }
      else
      {
        if (!((Object) this.m_previewCam != (Object) null) || !this.m_previewCam.gameObject.activeSelf)
          return;
        this.m_previewCam.gameObject.SetActive(false);
      }
    }

    private void ConfigLiv()
    {
      Debug.Log((object) "Prepping LIV Prefab");
      LIV.SDK.Unity.LIV component = Object.Instantiate<GameObject>(ManagerSingleton<GM>.Instance.LivPrefab, Vector3.zero, Quaternion.identity).GetComponent<LIV.SDK.Unity.LIV>();
      component.TrackedSpaceOrigin = GM.CurrentPlayerRoot.transform;
      component.HMDCamera = GM.CurrentPlayerBody.EyeCam;
      component.enabled = true;
      component.gameObject.SetActive(true);
      Debug.Log((object) "LIV Prefab Deployed");
    }

    private void OnDestroy()
    {
      FXM.ClearDecalPools();
      SM.ClearGenericPools();
    }

    public void SetSnakeEyes(bool b)
    {
      for (int index = 0; index < this.Post_SnakeEyes.Count; ++index)
        this.Post_SnakeEyes[index].SetActive(b);
    }

    public void SetMoleEye(bool b)
    {
      for (int index = 0; index < this.Post_MoleEye.Count; ++index)
        this.Post_MoleEye[index].SetActive(b);
    }

    public void SetBlort(bool b)
    {
      for (int index = 0; index < this.Post_Blort.Count; ++index)
        this.Post_Blort[index].SetActive(b);
    }

    public void SetDlort(bool b)
    {
      for (int index = 0; index < this.Post_Dlort.Count; ++index)
        this.Post_Dlort[index].SetActive(b);
    }

    public void SetFarOutMan(bool b)
    {
      for (int index = 0; index < this.Post_FarOutMan.Count; ++index)
        this.Post_FarOutMan[index].SetActive(b);
    }

    public void SetBadTrip(bool b)
    {
      for (int index = 0; index < this.Post_BadTrip.Count; ++index)
        this.Post_BadTrip[index].SetActive(b);
    }

    public void Init()
    {
    }

    public void PingReceivers(Vector3 pos)
    {
      if (this.ShotEventReceivers.Count < 1)
        return;
      this.m_curNotifyPos = pos;
      for (int index = 0; index < this.ShotEventReceivers.Count; ++index)
      {
        if ((Object) this.ShotEventReceivers[index] != (Object) null && !this.m_shotEventReceiversInQueue.Contains(this.ShotEventReceivers[index]))
        {
          this.m_shotEventReceiversInQueue.Add(this.ShotEventReceivers[index]);
          this.m_shotEventReceiversQueue.Enqueue(this.ShotEventReceivers[index]);
        }
      }
    }

    public void UpdateGlobalPostVolumes()
    {
      Debug.Log((object) "Updating Global Post Volumes:");
      Debug.Log((object) ("AO:" + (object) GM.Options.PerformanceOptions.IsPostEnabled_AO));
      Debug.Log((object) ("CC:" + (object) GM.Options.PerformanceOptions.IsPostEnabled_CC));
      Debug.Log((object) ("BL:" + (object) GM.Options.PerformanceOptions.IsPostEnabled_Bloom));
      if ((Object) this.Vol_AO != (Object) null)
      {
        if (GM.Options.PerformanceOptions.IsPostEnabled_AO)
          this.Vol_AO.enabled = true;
        else
          this.Vol_AO.enabled = false;
      }
      if ((Object) this.Vol_CC != (Object) null)
      {
        if (GM.Options.PerformanceOptions.IsPostEnabled_CC)
          this.Vol_CC.enabled = true;
        else
          this.Vol_CC.enabled = false;
      }
      if (!((Object) this.Vol_Bloom != (Object) null))
        return;
      if (GM.Options.PerformanceOptions.IsPostEnabled_Bloom)
        this.Vol_Bloom.enabled = true;
      else
        this.Vol_Bloom.enabled = false;
    }

    public void Update()
    {
      this.numSuppressingEventsThisFrame = 0;
      for (int index = 0; index < this.HowManyToShotReceivePerFrame; ++index)
      {
        if (this.m_shotEventReceiversQueue.Count > 0)
        {
          GameObject gameObject = this.m_shotEventReceiversQueue.Dequeue();
          this.m_shotEventReceiversInQueue.Remove(gameObject);
          if ((Object) gameObject != (Object) null)
            gameObject.SendMessage("ShotEvent", (object) this.m_curNotifyPos, SendMessageOptions.DontRequireReceiver);
        }
      }
      if (this.UsesPlayerCatcher && (double) GM.CurrentPlayerBody.transform.position.y < (double) this.CatchHeight)
      {
        double point = (double) GM.CurrentMovementManager.TeleportToPoint(this.DeathResetPoint.position, true, this.DeathResetPoint.forward);
        GM.CurrentMovementManager.CleanupFlagsForModeSwitch();
      }
      this.UpdateSpectatorCamera();
    }

    public event FVRSceneSettings.PlayerDeath PlayerDeathEvent;

    public void OnPlayerDeath(bool killedSelf)
    {
      if (this.PlayerDeathEvent == null)
        return;
      this.PlayerDeathEvent(killedSelf);
    }

    public event FVRSceneSettings.PlayerTookDamage PlayerTookDamageEvent;

    public void OnPlayerTookDamage(float percentOfLife)
    {
      if (this.PlayerTookDamageEvent == null)
        return;
      this.PlayerTookDamageEvent(percentOfLife);
    }

    public event FVRSceneSettings.BotKill KillEvent;

    public void OnBotKill(Damage dam)
    {
      if (this.KillEvent == null)
        return;
      this.KillEvent(dam);
    }

    public event FVRSceneSettings.SosigKill SosigKillEvent;

    public void OnSosigKill(Sosig s)
    {
      if (this.SosigKillEvent == null)
        return;
      this.SosigKillEvent(s);
    }

    public event FVRSceneSettings.ShotFired ShotFiredEvent;

    public void OnShotFired(FVRFireArm firearm)
    {
      if (this.ShotFiredEvent == null)
        return;
      this.ShotFiredEvent(firearm);
    }

    public event FVRSceneSettings.BotShotFired BotShotFiredEvent;

    public void OnBotShotFired(wwBotWurstModernGun gun)
    {
      if (this.BotShotFiredEvent == null)
        return;
      this.BotShotFiredEvent(gun);
    }

    public event FVRSceneSettings.PowerupUse PowerupUseEvent;

    public void OnPowerupUse(PowerupType type)
    {
      if (this.PowerupUseEvent == null)
        return;
      this.PowerupUseEvent(type);
    }

    public event FVRSceneSettings.FVRObjectPickedUp ObjectPickedUpEvent;

    public void OnFVRObjectPickedUp(FVRPhysicalObject obj)
    {
      if (this.ObjectPickedUpEvent == null)
        return;
      this.ObjectPickedUpEvent(obj);
    }

    public event FVRSceneSettings.FireArmReloaded FireArmReloadedEvent;

    public void OnFireArmReloaded(FVRObject obj)
    {
      if (this.FireArmReloadedEvent == null)
        return;
      this.FireArmReloadedEvent(obj);
    }

    public event FVRSceneSettings.PerceiveableSound PerceiveableSoundEvent;

    public void OnPerceiveableSound(
      float loudness,
      float maxDistanceHeard,
      Vector3 pos,
      int iffcode)
    {
      if (this.PerceiveableSoundEvent == null)
        return;
      this.PerceiveableSoundEvent(loudness, maxDistanceHeard, pos, iffcode);
    }

    public event FVRSceneSettings.SuppressingEvent SuppressingEventEvent;

    public void OnSuppressingEvent(
      Vector3 pos,
      Vector3 dir,
      int iffcode,
      float intensity,
      float range)
    {
      if (this.SuppressingEventEvent == null || this.numSuppressingEventsThisFrame >= 3)
        return;
      ++this.numSuppressingEventsThisFrame;
      this.SuppressingEventEvent(pos, dir, iffcode, intensity, range);
    }

    public delegate void PlayerDeath(bool killedSelf);

    public delegate void PlayerTookDamage(float percentOfLife);

    public delegate void BotKill(Damage dam);

    public delegate void SosigKill(Sosig s);

    public delegate void ShotFired(FVRFireArm firearm);

    public delegate void BotShotFired(wwBotWurstModernGun gun);

    public delegate void PowerupUse(PowerupType type);

    public delegate void FVRObjectPickedUp(FVRPhysicalObject obj);

    public delegate void FireArmReloaded(FVRObject obj);

    public delegate void PerceiveableSound(
      float loudness,
      float maxDistanceHeard,
      Vector3 pos,
      int iffcode);

    public delegate void SuppressingEvent(
      Vector3 pos,
      Vector3 dir,
      int iffcode,
      float intensity,
      float range);
  }
}
