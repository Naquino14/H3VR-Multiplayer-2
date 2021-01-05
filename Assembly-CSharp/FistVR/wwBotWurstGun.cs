// Decompiled with JetBrains decompiler
// Type: FistVR.wwBotWurstGun
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class wwBotWurstGun : MonoBehaviour
  {
    public wwBotWurst Bot;
    public Transform Gun;
    public Transform Pose_Firing;
    public Transform Pose_Reloading;
    public Transform GunCyclePiece;
    public Transform CyclePose_Firing;
    public Transform CyclePose_Reloading;
    private bool m_isReloading;
    public float PoseChangeSpeed;
    private float m_poseChangeLerp;
    public bool DoesCycle;
    public float CycleSpeed;
    public float CycleStall;
    private float m_cycleStallTick = 1f;
    private float m_cycleLerp;
    private bool m_isCycleStalling;
    public float ReloadingSpeed;
    private float m_reloadingLerp;
    public float HangOnMidReloadingTime = 1f;
    private float m_hangTick = 1f;
    public wwBotWurstGun.FiringState FireState;
    public Vector2 Firing_RefireRange = new Vector2(0.25f, 0.5f);
    public int ShotsPerLoad = 1;
    private float m_firingTick = 0.5f;
    private int m_shotsLeft = 1;
    public wwBotWurstGunSoundConfig GunShotProfile;
    private Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet> m_shotDic = new Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet>();
    public float MinHandlingDistance = 15f;
    public Transform Muzzle;
    public float AccuracyRange = 1f;
    public GameObject Projectile;
    public int NumProjectiles = 1;
    private bool m_fireAtWill;
    public float RecoilAngle = -30f;
    private float angToTarget;
    public bool UsesMuzzleFire;
    public ParticleSystem[] PSystemsMuzzle;
    public int MuzzlePAmount;
    public bool DoesFlashOnFire;

    private void Awake()
    {
      this.m_shotsLeft = this.ShotsPerLoad;
      this.PrimeDics();
    }

    private void Update() => this.UpdateGun();

    public void SetFireAtWill(bool b) => this.m_fireAtWill = b;

    private void Fire()
    {
      this.PlayShotEvent(this.Muzzle.position);
      for (int index = 0; index < this.NumProjectiles; ++index)
      {
        this.Muzzle.localEulerAngles = new Vector3(Random.Range(-this.AccuracyRange, this.AccuracyRange), Random.Range(-this.AccuracyRange, this.AccuracyRange), 0.0f);
        GameObject gameObject = Object.Instantiate<GameObject>(this.Projectile, this.Muzzle.position, this.Muzzle.rotation);
        gameObject.GetComponent<BallisticProjectile>().FlightVelocityMultiplier = 0.1f;
        gameObject.GetComponent<BallisticProjectile>().Fire(this.Muzzle.forward, (FVRFireArm) null);
      }
      if (this.UsesMuzzleFire)
      {
        for (int index = 0; index < this.PSystemsMuzzle.Length; ++index)
          this.PSystemsMuzzle[index].Emit(this.MuzzlePAmount);
        if (this.DoesFlashOnFire)
          FXM.InitiateMuzzleFlash(this.Muzzle.position, this.Muzzle.forward, 1f, new Color(1f, 0.9f, 0.77f), 1f);
      }
      this.Gun.localEulerAngles = new Vector3(this.RecoilAngle, Random.Range((float) (-(double) this.RecoilAngle * 0.200000002980232), this.RecoilAngle * 0.2f), 0.0f);
    }

    public void UpdateGun()
    {
      if ((Object) this.Bot == (Object) null || (Object) this.Bot.RB_Bottom == (Object) null || (Object) this.Bot.RB_Torso == (Object) null)
        return;
      if (this.Bot.State == wwBotWurst.BotState.Fighting)
      {
        Vector3 vector3 = this.Bot.LastPlaceTargetSeen - this.transform.position;
        Vector3 target = Vector3.ProjectOnPlane(vector3, this.Bot.RB_Bottom.transform.right);
        Vector3 normalized1 = Vector3.RotateTowards(this.Bot.RB_Torso.transform.forward, target, 1.396264f, 0.0f).normalized;
        Vector3 normalized2 = Vector3.RotateTowards(normalized1, vector3, this.Bot.MaxFiringAngle * 0.0174533f, 0.0f).normalized;
        Vector3 normalized3 = Vector3.RotateTowards(this.Bot.RB_Torso.transform.forward, Vector3.ProjectOnPlane(vector3, this.Bot.RB_Bottom.transform.up), this.Bot.MaxFiringAngle * 0.0174533f, 0.0f).normalized;
        Debug.DrawLine(this.transform.position, this.transform.position + vector3.normalized * 20f, Color.red);
        Debug.DrawLine(this.transform.position, this.transform.position + target * 20f, Color.blue);
        Debug.DrawLine(this.transform.position, this.transform.position + normalized1 * 20f, Color.green);
        Vector3.Cross(normalized3, normalized1);
        Debug.DrawLine(this.transform.position, this.transform.position + normalized2 * 20f, Color.yellow);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(normalized2, this.Bot.RB_Torso.transform.up), Time.deltaTime * 2f);
        this.angToTarget = Vector3.Angle(vector3, this.transform.forward);
      }
      else
        this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, Quaternion.identity, Time.deltaTime * 2f);
      switch (this.FireState)
      {
        case wwBotWurstGun.FiringState.firing:
          if ((double) this.m_firingTick > 0.0)
            this.m_firingTick -= Time.deltaTime;
          else if (this.m_fireAtWill && (double) this.angToTarget < (double) this.Bot.MaxFiringAngle)
          {
            this.Fire();
            --this.m_shotsLeft;
            if (this.m_shotsLeft > 0)
            {
              if (this.DoesCycle)
              {
                this.FireState = wwBotWurstGun.FiringState.cycledown;
                this.m_cycleStallTick = this.CycleStall;
                this.m_isCycleStalling = true;
              }
              this.m_firingTick = Random.Range(this.Firing_RefireRange.x, this.Firing_RefireRange.y);
            }
            else
              this.FireState = wwBotWurstGun.FiringState.movingToLoad;
          }
          this.Gun.localRotation = Quaternion.Slerp(this.Gun.localRotation, this.Pose_Firing.localRotation, Time.deltaTime * 15f);
          break;
        case wwBotWurstGun.FiringState.cycledown:
          if (this.m_isCycleStalling)
          {
            if ((double) this.m_cycleStallTick > 0.0)
            {
              this.m_cycleStallTick -= Time.deltaTime;
              break;
            }
            this.m_cycleStallTick = 0.0f;
            SM.PlayCoreSound(FVRPooledAudioType.NPCHandling, this.GunShotProfile.EjectionBack, this.transform.position);
            this.m_isCycleStalling = false;
            break;
          }
          if ((double) this.m_cycleLerp < 1.0)
          {
            this.m_cycleLerp += Time.deltaTime * this.CycleSpeed;
          }
          else
          {
            this.m_cycleLerp = 1f;
            this.FireState = wwBotWurstGun.FiringState.cycleup;
            SM.PlayCoreSound(FVRPooledAudioType.NPCHandling, this.GunShotProfile.EjectionForward, this.transform.position);
          }
          this.GunCyclePiece.localRotation = Quaternion.Slerp(this.CyclePose_Firing.localRotation, this.CyclePose_Reloading.localRotation, Mathf.Sqrt(this.m_cycleLerp));
          break;
        case wwBotWurstGun.FiringState.cycleup:
          if ((double) this.m_cycleLerp > 0.0)
          {
            this.m_cycleLerp -= Time.deltaTime * this.CycleSpeed;
          }
          else
          {
            this.m_cycleLerp = 0.0f;
            this.FireState = wwBotWurstGun.FiringState.firing;
          }
          this.GunCyclePiece.localRotation = Quaternion.Slerp(this.CyclePose_Firing.localRotation, this.CyclePose_Reloading.localRotation, Mathf.Pow(this.m_cycleLerp, 2f));
          break;
        case wwBotWurstGun.FiringState.movingToLoad:
          if ((double) this.m_poseChangeLerp < 1.0)
          {
            this.m_poseChangeLerp += Time.deltaTime * this.PoseChangeSpeed;
          }
          else
          {
            this.m_poseChangeLerp = 1f;
            this.FireState = wwBotWurstGun.FiringState.reloading;
            SM.PlayCoreSound(FVRPooledAudioType.NPCHandling, this.GunShotProfile.Reloading, this.transform.position);
          }
          this.Gun.localPosition = Vector3.Lerp(this.Pose_Firing.localPosition, this.Pose_Reloading.localPosition, Mathf.Pow(this.m_poseChangeLerp, 4f));
          this.Gun.localRotation = Quaternion.Slerp(this.Pose_Firing.localRotation, this.Pose_Reloading.localRotation, Mathf.Pow(this.m_poseChangeLerp, 4f));
          break;
        case wwBotWurstGun.FiringState.reloading:
          if ((double) this.m_reloadingLerp < 1.0)
          {
            this.m_reloadingLerp += Time.deltaTime * this.ReloadingSpeed;
          }
          else
          {
            this.m_reloadingLerp = 1f;
            this.m_shotsLeft = this.ShotsPerLoad;
            this.FireState = wwBotWurstGun.FiringState.hanging;
            this.m_hangTick = this.HangOnMidReloadingTime;
          }
          this.GunCyclePiece.localRotation = Quaternion.Slerp(this.CyclePose_Firing.localRotation, this.CyclePose_Reloading.localRotation, Mathf.Pow(this.m_reloadingLerp, 4f));
          break;
        case wwBotWurstGun.FiringState.hanging:
          if ((double) this.m_hangTick > 0.0)
          {
            this.m_hangTick -= Time.deltaTime;
            break;
          }
          this.FireState = wwBotWurstGun.FiringState.recovery;
          break;
        case wwBotWurstGun.FiringState.recovery:
          if ((double) this.m_reloadingLerp > 0.0)
          {
            this.m_reloadingLerp -= Time.deltaTime * this.ReloadingSpeed;
          }
          else
          {
            this.m_reloadingLerp = 0.0f;
            this.FireState = wwBotWurstGun.FiringState.movingToFire;
          }
          this.GunCyclePiece.localRotation = Quaternion.Slerp(this.CyclePose_Firing.localRotation, this.CyclePose_Reloading.localRotation, Mathf.Pow(this.m_reloadingLerp, 4f));
          break;
        case wwBotWurstGun.FiringState.movingToFire:
          if ((double) this.m_poseChangeLerp > 0.0)
          {
            this.m_poseChangeLerp -= Time.deltaTime * this.PoseChangeSpeed;
          }
          else
          {
            this.m_poseChangeLerp = 0.0f;
            this.m_firingTick = Random.Range(this.Firing_RefireRange.x, this.Firing_RefireRange.y);
            this.FireState = wwBotWurstGun.FiringState.firing;
          }
          this.Gun.localPosition = Vector3.Lerp(this.Pose_Firing.localPosition, this.Pose_Reloading.localPosition, Mathf.Pow(this.m_poseChangeLerp, 4f));
          this.Gun.localRotation = Quaternion.Slerp(this.Pose_Firing.localRotation, this.Pose_Reloading.localRotation, Mathf.Pow(this.m_poseChangeLerp, 4f));
          break;
      }
    }

    private void PlayShotEvent(Vector3 source)
    {
      float num = Vector3.Distance(source, GM.CurrentPlayerBody.Head.position);
      float delay = num / 343f;
      wwBotWurstGunSoundConfig.BotGunShotSet shotSet = this.GetShotSet(SM.GetReverbEnvironment(this.transform.position).Environment);
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
      firing,
      cycledown,
      cycleup,
      movingToLoad,
      reloading,
      hanging,
      recovery,
      movingToFire,
    }
  }
}
