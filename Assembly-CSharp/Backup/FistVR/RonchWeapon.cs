// Decompiled with JetBrains decompiler
// Type: FistVR.RonchWeapon
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class RonchWeapon : MonoBehaviour
  {
    public RonchMaster Master;
    [Header("State")]
    public RonchWeapon.FiringState FireState;
    public float Time_CycleUp = 1f;
    public float Time_Firing = 1f;
    public bool SendsFireCompleteMessage;
    public AudioEvent AudEvent_CycleUp;
    [Header("Mecha")]
    public Transform Muzzle;
    public float Spread;
    public int NumShotsPerSalvo = 1;
    public float RefireRate = 0.2f;
    [Header("Projectile")]
    public GameObject Prefab_Projectile;
    public float FlightVelocityMultiplier = 0.2f;
    public RonchWeapon.ProjectileType Type;
    [Header("Audio Params")]
    public wwBotWurstGunSoundConfig GunShotProfile;
    private Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet> m_shotDic = new Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet>();
    [Header("MuzzleFX")]
    public bool UsesMuzzleFire;
    public bool DoesFlashOnFire;
    public List<ParticleSystem> PSystemsMuzzle;
    public int MuzzlePAmount;
    private float m_stateTickDown = 1f;
    private float m_refireTickDown = 1f;
    private int m_shotsLeft;
    private Transform targetPos;
    private bool m_canFire = true;

    public void SetTargetPos(Transform t) => this.targetPos = t;

    protected void Start()
    {
      if (!((Object) this.GunShotProfile != (Object) null))
        return;
      this.PrimeDics();
    }

    public void BeginFiringSequence()
    {
      if (!this.m_canFire || this.FireState != RonchWeapon.FiringState.Idle)
        return;
      this.FireState = RonchWeapon.FiringState.CyclingUp;
      this.m_stateTickDown = this.Time_CycleUp;
    }

    private void InitiateFiringState()
    {
      this.FireState = RonchWeapon.FiringState.Firing;
      this.m_stateTickDown = this.Time_Firing;
      this.m_refireTickDown = this.RefireRate;
      this.m_shotsLeft = this.NumShotsPerSalvo;
    }

    private void InitiateDoneFiringState() => this.FireState = RonchWeapon.FiringState.DoneFiring;

    public void AbortFiringState() => this.FireState = RonchWeapon.FiringState.Idle;

    public void DestroyGun()
    {
      this.AbortFiringState();
      this.m_canFire = false;
    }

    public void Update() => this.StateUpdate();

    private void StateUpdate()
    {
      if (this.FireState == RonchWeapon.FiringState.CyclingUp)
      {
        this.m_stateTickDown -= Time.deltaTime;
        if ((double) this.m_stateTickDown > 0.0)
          return;
        this.InitiateFiringState();
      }
      else if (this.FireState == RonchWeapon.FiringState.Firing)
      {
        if ((double) this.m_refireTickDown > 0.0)
          this.m_refireTickDown -= Time.deltaTime;
        else
          this.Fire();
        this.m_stateTickDown -= Time.deltaTime;
        if ((double) this.m_stateTickDown > 0.0)
          return;
        this.InitiateDoneFiringState();
      }
      else
      {
        if (this.FireState != RonchWeapon.FiringState.DoneFiring)
          return;
        if (this.SendsFireCompleteMessage)
          this.Master.FiringComplete();
        this.FireState = RonchWeapon.FiringState.Idle;
      }
    }

    private void Fire()
    {
      --this.m_shotsLeft;
      this.m_refireTickDown = this.RefireRate;
      switch (this.Type)
      {
        case RonchWeapon.ProjectileType.Projectile:
          GameObject gameObject1 = Object.Instantiate<GameObject>(this.Prefab_Projectile, this.Muzzle.position, this.Muzzle.rotation);
          gameObject1.transform.Rotate(new Vector3(Random.Range(-this.Spread, this.Spread), Random.Range(-this.Spread, this.Spread), 0.0f));
          gameObject1.GetComponent<BallisticProjectile>().FlightVelocityMultiplier = this.FlightVelocityMultiplier;
          gameObject1.GetComponent<BallisticProjectile>().Fire(gameObject1.transform.forward, (FVRFireArm) null);
          this.PlayShotEvent(this.Muzzle.position);
          break;
        case RonchWeapon.ProjectileType.Stinger:
          GameObject gameObject2 = Object.Instantiate<GameObject>(this.Prefab_Projectile, this.Muzzle.position, this.Muzzle.rotation);
          gameObject2.transform.Rotate(new Vector3(Random.Range(-this.Spread, this.Spread), Random.Range(-this.Spread, this.Spread), 0.0f));
          StingerMissile component = gameObject2.GetComponent<StingerMissile>();
          component.SetMotorPower(20f);
          component.SetMaxSpeed(25f);
          component.SetTurnSpeed(Random.Range(2.4f, 3f));
          component.Fire(this.targetPos.position, 40f);
          break;
      }
      if (this.UsesMuzzleFire)
      {
        for (int index = 0; index < this.PSystemsMuzzle.Count; ++index)
          this.PSystemsMuzzle[index].Emit(this.MuzzlePAmount);
      }
      if (this.DoesFlashOnFire)
        FXM.InitiateMuzzleFlash(this.Muzzle.position, this.Muzzle.forward, 1f, new Color(1f, 0.9f, 0.77f), 1f);
      if ((double) this.m_shotsLeft > 0.0)
        return;
      this.InitiateDoneFiringState();
    }

    private FVRSoundEnvironment PlayShotEvent(Vector3 source)
    {
      float num = Vector3.Distance(source, GM.CurrentPlayerBody.Head.position);
      float delay = num / 343f;
      FVRSoundEnvironment environment = SM.GetReverbEnvironment(this.transform.position).Environment;
      wwBotWurstGunSoundConfig.BotGunShotSet shotSet = this.GetShotSet(environment);
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
      return environment;
    }

    private wwBotWurstGunSoundConfig.BotGunShotSet GetShotSet(
      FVRSoundEnvironment e)
    {
      return this.m_shotDic[e];
    }

    private void PrimeDics()
    {
      if (!((Object) this.GunShotProfile != (Object) null))
        return;
      for (int index1 = 0; index1 < this.GunShotProfile.ShotSets.Count; ++index1)
      {
        for (int index2 = 0; index2 < this.GunShotProfile.ShotSets[index1].EnvironmentsUsed.Count; ++index2)
          this.m_shotDic.Add(this.GunShotProfile.ShotSets[index1].EnvironmentsUsed[index2], this.GunShotProfile.ShotSets[index1]);
      }
    }

    public enum FiringState
    {
      Idle,
      CyclingUp,
      Firing,
      DoneFiring,
    }

    public enum ProjectileType
    {
      Projectile,
      Stinger,
      Beam,
    }
  }
}
