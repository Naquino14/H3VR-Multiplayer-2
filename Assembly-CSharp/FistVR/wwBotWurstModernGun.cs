// Decompiled with JetBrains decompiler
// Type: FistVR.wwBotWurstModernGun
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class wwBotWurstModernGun : MonoBehaviour
  {
    public wwBotWurst Bot;
    public wwBotWurstModernGun.FiringState FireState;
    [Header("Pose Vars")]
    public List<Transform> FiringPoses = new List<Transform>();
    private int m_curFiringPose;
    private int m_prevFiringPose;
    public Transform ReloadingPose;
    public Transform Rig_Gun;
    public Transform Rig_ReciprocatingPiece;
    private Vector3 gunPos;
    private Quaternion gunRot = Quaternion.identity;
    public bool UsesReciprocatingPiece = true;
    public float EjectionReciprocatingZ;
    [Header("Timing Vars")]
    public Vector2 Timer_RateLimiter;
    public Vector2 Timer_EjectionDelay;
    public float Timer_EjectionBack;
    public float Timer_EjectionForward;
    public float Timer_GoingToReload;
    public Vector2 Timer_ReloadTime;
    public float Timer_RecoveringFromReload;
    public bool UsesBurst;
    public int BurstCountMin = 3;
    public int BurstCountMax = 3;
    private int m_burstLimit;
    public Vector2 BurstAddDelay;
    private int m_burstCount;
    private float m_tick;
    private bool m_fireAtWill;
    [Header("Gun Vars")]
    public int AmmoCapacity;
    public bool CyclesOnLastRound = true;
    private int m_shotsLeft;
    [Header("AudioEvents")]
    public wwBotWurstGunSoundConfig GunShotProfile;
    private bool m_hasProfile;
    private Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet> m_shotDic = new Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet>();
    public float MinHandlingDistance = 15f;
    [Header("Firing Vars")]
    public Transform Muzzle;
    public float AccuracyRange = 1f;
    public GameObject Projectile;
    public int NumProjectiles = 1;
    private bool m_usesFastProjectile;
    public bool UsesMuzzleFire;
    public ParticleSystem[] PSystemsMuzzle;
    public int MuzzlePAmount;
    public bool DoesFlashOnFire;
    [Header("PoseTest Vars")]
    private float m_timeSincePoseChange;
    private int m_nextPoseToTestToTarget;
    public LayerMask LM_VisCheck;
    private RaycastHit m_hit;
    private List<bool> m_validSightPoses = new List<bool>();
    private float angToTarget;

    public void SetUseFastProjectile(bool b) => this.m_usesFastProjectile = b;

    private void Awake()
    {
      this.m_tick = Random.Range(this.Timer_RateLimiter.x, this.Timer_RateLimiter.y);
      this.m_shotsLeft = this.AmmoCapacity;
      this.gunPos = this.FiringPoses[0].localPosition;
      this.m_burstLimit = Random.Range(this.BurstCountMin, this.BurstCountMax);
      for (int index = 0; index < this.FiringPoses.Count; ++index)
        this.m_validSightPoses.Add(false);
      if ((Object) this.GunShotProfile != (Object) null)
        this.m_hasProfile = true;
      if (!this.m_hasProfile)
        return;
      this.PrimeDics();
    }

    private void Start()
    {
      this.Rig_Gun.position = this.FiringPoses[this.m_curFiringPose].position;
      this.Rig_Gun.rotation = this.FiringPoses[this.m_curFiringPose].rotation;
    }

    private void Update()
    {
      this.UpdateGunHandlingPose();
      this.UpdateGun();
    }

    public void SetFireAtWill(bool b) => this.m_fireAtWill = b;

    public void UpdateGunHandlingPose()
    {
      if (this.Bot.State != wwBotWurst.BotState.Fighting && this.Bot.State != wwBotWurst.BotState.Searching)
        return;
      if ((double) this.m_timeSincePoseChange <= 1.0)
      {
        this.gunPos = Vector3.Lerp(this.FiringPoses[this.m_prevFiringPose].localPosition, this.FiringPoses[this.m_curFiringPose].localPosition, this.m_timeSincePoseChange);
        if (this.FireState != wwBotWurstModernGun.FiringState.GoingToReload && this.FireState != wwBotWurstModernGun.FiringState.Reloading && this.FireState != wwBotWurstModernGun.FiringState.RecoveringFromReload)
          this.Rig_Gun.localPosition = this.gunPos;
      }
      Vector3 position = this.FiringPoses[this.m_nextPoseToTestToTarget].position;
      Vector3 vector3 = this.Bot.LastPlaceTargetSeen + Random.onUnitSphere * 0.1f - position;
      this.m_validSightPoses[this.m_nextPoseToTestToTarget] = !Physics.Raycast(position, vector3.normalized, out this.m_hit, Mathf.Min(vector3.magnitude, this.Bot.MaxViewDistance), (int) this.LM_VisCheck, QueryTriggerInteraction.Ignore);
      ++this.m_nextPoseToTestToTarget;
      if (this.m_nextPoseToTestToTarget >= this.m_validSightPoses.Count)
        this.m_nextPoseToTestToTarget = 0;
      if ((double) this.m_timeSincePoseChange < 1.0)
      {
        this.m_timeSincePoseChange += Time.deltaTime * 3f;
      }
      else
      {
        if (this.m_validSightPoses[this.m_curFiringPose])
          return;
        int num = -1;
        for (int index = 0; index < this.m_validSightPoses.Count; ++index)
        {
          if (this.m_validSightPoses[index])
          {
            num = index;
            break;
          }
        }
        if (num <= -1)
          return;
        this.m_prevFiringPose = this.m_curFiringPose;
        this.m_curFiringPose = num;
        this.m_timeSincePoseChange = 0.0f;
      }
    }

    public void UpdateGun()
    {
      if ((Object) this.Bot == (Object) null || (Object) this.Bot.RB_Bottom == (Object) null || (Object) this.Bot.RB_Torso == (Object) null)
        return;
      if (this.Bot.State == wwBotWurst.BotState.Fighting)
      {
        Vector3 vector3_1 = this.Bot.LastPlaceTargetSeen - this.Rig_Gun.position;
        Vector3 target = Vector3.ProjectOnPlane(vector3_1, this.Bot.RB_Bottom.transform.right);
        Vector3 normalized1 = Vector3.RotateTowards(this.Bot.RB_Torso.transform.forward, target, 1.396264f, 0.0f).normalized;
        Vector3 normalized2 = Vector3.RotateTowards(normalized1, vector3_1, this.Bot.MaxFiringAngle * 0.0174533f, 0.0f).normalized;
        Vector3 vector3_2 = Vector3.ProjectOnPlane(vector3_1, this.Bot.RB_Bottom.transform.up);
        Vector3 normalized3 = Vector3.RotateTowards(this.Bot.RB_Torso.transform.forward, vector3_2, this.Bot.MaxFiringAngle * 0.0174533f, 0.0f).normalized;
        Debug.DrawLine(this.transform.position, this.transform.position + vector3_1.normalized * 20f, Color.red);
        Debug.DrawLine(this.transform.position, this.transform.position + target * 20f, Color.blue);
        Debug.DrawLine(this.transform.position, this.transform.position + normalized1 * 20f, Color.green);
        Vector3.Cross(normalized3, normalized1);
        Debug.DrawLine(this.transform.position, this.transform.position + normalized2 * 20f, Color.yellow);
        this.gunRot = Quaternion.Slerp(this.gunRot, Quaternion.LookRotation(normalized2, this.Bot.RB_Torso.transform.up), Time.deltaTime * 2f);
        this.angToTarget = Vector3.Angle(vector3_2, Vector3.ProjectOnPlane(this.transform.forward, Vector3.up));
      }
      else
        this.gunRot = Quaternion.Slerp(this.gunRot, this.transform.rotation, Time.deltaTime * 2f);
      if (this.FireState != wwBotWurstModernGun.FiringState.GoingToReload && this.FireState != wwBotWurstModernGun.FiringState.RecoveringFromReload && this.FireState != wwBotWurstModernGun.FiringState.Reloading)
        this.Rig_Gun.rotation = this.gunRot;
      switch (this.FireState)
      {
        case wwBotWurstModernGun.FiringState.ReadyToFire:
          if ((double) this.m_tick > 0.0)
          {
            this.m_tick -= Time.deltaTime;
            break;
          }
          if (!this.m_fireAtWill || (double) this.angToTarget >= (double) this.Bot.Config.AngularFiringRange || !this.Bot.GetFireClear(this.Muzzle.position, this.Muzzle.position + this.transform.forward * 30f))
            break;
          this.Fire();
          if (this.UsesBurst)
            ++this.m_burstCount;
          --this.m_shotsLeft;
          this.FireState = wwBotWurstModernGun.FiringState.Firing;
          this.m_tick = Random.Range(this.Timer_EjectionDelay.x, this.Timer_EjectionDelay.y);
          break;
        case wwBotWurstModernGun.FiringState.Firing:
          if ((double) this.m_tick > 0.0)
          {
            this.m_tick -= Time.deltaTime;
            break;
          }
          if (this.m_shotsLeft <= 0 && !this.CyclesOnLastRound)
          {
            this.FireState = wwBotWurstModernGun.FiringState.GoingToReload;
            this.m_tick = this.Timer_GoingToReload;
            if (!this.m_hasProfile || this.GunShotProfile.GoingToReload.Clips.Count <= 0 || (double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) >= (double) this.MinHandlingDistance)
              break;
            SM.PlayCoreSound(FVRPooledAudioType.NPCHandling, this.GunShotProfile.GoingToReload, this.transform.position);
            break;
          }
          this.FireState = wwBotWurstModernGun.FiringState.EjectionBack;
          this.m_tick = this.Timer_EjectionBack;
          if (!this.m_hasProfile || this.GunShotProfile.EjectionBack.Clips.Count <= 0 || (double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) >= (double) this.MinHandlingDistance)
            break;
          SM.PlayCoreSound(FVRPooledAudioType.NPCHandling, this.GunShotProfile.EjectionBack, this.transform.position);
          break;
        case wwBotWurstModernGun.FiringState.EjectionBack:
          if ((double) this.m_tick > 0.0)
            this.m_tick -= Time.deltaTime;
          if ((double) this.m_tick < 0.0)
            this.m_tick = 0.0f;
          if (this.UsesReciprocatingPiece)
            this.Rig_ReciprocatingPiece.localPosition = new Vector3(0.0f, 0.0f, Mathf.Lerp(0.0f, this.EjectionReciprocatingZ, (float) (1.0 - (double) this.m_tick / (double) this.Timer_EjectionBack)));
          if ((double) this.m_tick > 0.0)
            break;
          this.m_tick = this.Timer_EjectionForward;
          this.FireState = wwBotWurstModernGun.FiringState.EjectionForward;
          if (!this.m_hasProfile || this.GunShotProfile.EjectionForward.Clips.Count <= 0 || (double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) >= (double) this.MinHandlingDistance)
            break;
          SM.PlayCoreSound(FVRPooledAudioType.NPCHandling, this.GunShotProfile.EjectionForward, this.transform.position);
          break;
        case wwBotWurstModernGun.FiringState.EjectionForward:
          if ((double) this.m_tick > 0.0)
            this.m_tick -= Time.deltaTime;
          if ((double) this.m_tick < 0.0)
            this.m_tick = 0.0f;
          if (this.UsesReciprocatingPiece)
            this.Rig_ReciprocatingPiece.localPosition = new Vector3(0.0f, 0.0f, Mathf.Lerp(this.EjectionReciprocatingZ, 0.0f, (float) (1.0 - (double) this.m_tick / (double) this.Timer_EjectionBack)));
          if ((double) this.m_tick > 0.0)
            break;
          if (this.m_shotsLeft <= 0)
          {
            this.m_tick = this.Timer_GoingToReload;
            this.FireState = wwBotWurstModernGun.FiringState.GoingToReload;
            if (!this.m_hasProfile || this.GunShotProfile.GoingToReload.Clips.Count <= 0 || (double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) >= (double) this.MinHandlingDistance)
              break;
            SM.PlayCoreSound(FVRPooledAudioType.NPCHandling, this.GunShotProfile.GoingToReload, this.transform.position);
            break;
          }
          float num = 0.0f;
          if (this.UsesBurst && this.m_burstCount > this.m_burstLimit)
          {
            num = Random.Range(this.BurstAddDelay.x, this.BurstAddDelay.y);
            this.m_burstLimit = Random.Range(this.BurstCountMin, this.BurstCountMax);
            this.m_burstCount = 0;
          }
          this.m_tick = Random.Range(this.Timer_RateLimiter.x, this.Timer_RateLimiter.y) + num;
          this.FireState = wwBotWurstModernGun.FiringState.ReadyToFire;
          break;
        case wwBotWurstModernGun.FiringState.GoingToReload:
          if ((double) this.m_tick > 0.0)
            this.m_tick -= Time.deltaTime;
          if ((double) this.m_tick < 0.0)
            this.m_tick = 0.0f;
          float t1 = (float) (1.0 - (double) this.m_tick / (double) this.Timer_GoingToReload);
          this.Rig_Gun.localPosition = Vector3.Lerp(this.gunPos, this.ReloadingPose.localPosition, t1);
          this.Rig_Gun.rotation = Quaternion.Slerp(this.gunRot, this.ReloadingPose.rotation, t1);
          if ((double) this.m_tick > 0.0)
            break;
          this.m_tick = Random.Range(this.Timer_ReloadTime.x, this.Timer_ReloadTime.y);
          this.FireState = wwBotWurstModernGun.FiringState.Reloading;
          if (!this.m_hasProfile || this.GunShotProfile.Reloading.Clips.Count <= 0 || (double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) >= (double) this.MinHandlingDistance)
            break;
          SM.PlayCoreSound(FVRPooledAudioType.NPCHandling, this.GunShotProfile.Reloading, this.transform.position);
          break;
        case wwBotWurstModernGun.FiringState.Reloading:
          if ((double) this.m_tick > 0.0)
          {
            this.m_tick -= Time.deltaTime;
            break;
          }
          this.m_tick = this.Timer_RecoveringFromReload;
          this.FireState = wwBotWurstModernGun.FiringState.RecoveringFromReload;
          this.m_shotsLeft = this.AmmoCapacity;
          if (!this.m_hasProfile || this.GunShotProfile.RecoveringFromReload.Clips.Count <= 0 || (double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) >= (double) this.MinHandlingDistance)
            break;
          SM.PlayCoreSound(FVRPooledAudioType.NPCHandling, this.GunShotProfile.RecoveringFromReload, this.transform.position);
          break;
        case wwBotWurstModernGun.FiringState.RecoveringFromReload:
          if ((double) this.m_tick > 0.0)
            this.m_tick -= Time.deltaTime;
          if ((double) this.m_tick < 0.0)
            this.m_tick = 0.0f;
          float t2 = (float) (1.0 - (double) this.m_tick / (double) this.Timer_RecoveringFromReload);
          this.Rig_Gun.localPosition = Vector3.Lerp(this.ReloadingPose.localPosition, this.gunPos, t2);
          this.Rig_Gun.rotation = Quaternion.Slerp(this.ReloadingPose.rotation, this.gunRot, t2);
          if ((double) this.m_tick > 0.0)
            break;
          if (this.UsesBurst)
            this.m_burstCount = 0;
          this.m_tick = Random.Range(this.Timer_RateLimiter.x, this.Timer_RateLimiter.y);
          this.FireState = wwBotWurstModernGun.FiringState.ReadyToFire;
          break;
      }
    }

    private void Fire()
    {
      if (this.m_hasProfile)
        this.PlayShotEvent(this.Muzzle.position);
      GM.CurrentSceneSettings.OnBotShotFired(this);
      GameObject projectile = this.Projectile;
      for (int index = 0; index < this.NumProjectiles; ++index)
      {
        this.Muzzle.localEulerAngles = new Vector3(Random.Range(-this.AccuracyRange, this.AccuracyRange), Random.Range(-this.AccuracyRange, this.AccuracyRange), 0.0f);
        GameObject gameObject = Object.Instantiate<GameObject>(projectile, this.Muzzle.position, this.Muzzle.rotation);
        if (!this.m_usesFastProjectile)
          gameObject.GetComponent<BallisticProjectile>().FlightVelocityMultiplier = 0.05f;
        gameObject.GetComponent<BallisticProjectile>().Fire(this.Muzzle.forward, (FVRFireArm) null);
      }
      if (!this.UsesMuzzleFire)
        return;
      for (int index = 0; index < this.PSystemsMuzzle.Length; ++index)
        this.PSystemsMuzzle[index].Emit(this.MuzzlePAmount);
      if (!this.DoesFlashOnFire)
        return;
      FXM.InitiateMuzzleFlash(this.Muzzle.position, this.Muzzle.forward, 1f, new Color(1f, 0.9f, 0.77f), 1f);
    }

    private void PlayShotEvent(Vector3 source)
    {
      float num = Vector3.Distance(source, GM.CurrentPlayerBody.Head.position);
      float delay = num / 343f;
      wwBotWurstGunSoundConfig.BotGunShotSet shotSet = this.GetShotSet(SM.GetSoundEnvironment(this.transform.position));
      if ((double) num < 20.0)
        SM.PlayCoreSoundDelayedOverrides(FVRPooledAudioType.NPCShotNear, shotSet.ShotSet_Near, source, shotSet.ShotSet_Distant.VolumeRange, shotSet.ShotSet_Distant.PitchRange, delay);
      else if ((double) num < 100.0)
      {
        float y = Mathf.Lerp(0.4f, 0.2f, (float) (((double) num - 20.0) / 80.0));
        Vector2 vol = new Vector2(y * 0.95f, y);
        SM.PlayCoreSoundDelayedOverrides(FVRPooledAudioType.NPCShotFarDistant, shotSet.ShotSet_Far, source, vol, shotSet.ShotSet_Distant.PitchRange, delay);
      }
      else
        SM.PlayCoreSoundDelayedOverrides(FVRPooledAudioType.NPCShotFarDistant, shotSet.ShotSet_Distant, source, shotSet.ShotSet_Distant.VolumeRange, shotSet.ShotSet_Distant.PitchRange, delay);
    }

    private wwBotWurstGunSoundConfig.BotGunShotSet GetShotSet(
      FVRSoundEnvironment e)
    {
      return this.m_shotDic[e];
    }

    private void PrimeDics()
    {
      for (int index1 = 0; index1 < this.GunShotProfile.ShotSets.Count; ++index1)
      {
        for (int index2 = 0; index2 < this.GunShotProfile.ShotSets[index1].EnvironmentsUsed.Count; ++index2)
          this.m_shotDic.Add(this.GunShotProfile.ShotSets[index1].EnvironmentsUsed[index2], this.GunShotProfile.ShotSets[index1]);
      }
    }

    public enum FiringState
    {
      ReadyToFire,
      Firing,
      EjectionBack,
      EjectionForward,
      GoingToReload,
      Reloading,
      RecoveringFromReload,
    }
  }
}
