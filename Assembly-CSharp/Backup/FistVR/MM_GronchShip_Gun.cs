// Decompiled with JetBrains decompiler
// Type: FistVR.MM_GronchShip_Gun
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class MM_GronchShip_Gun : MonoBehaviour
  {
    [Header("Main Info")]
    public MM_GronchShip Ship;
    [Header("AudioEvents")]
    public wwBotWurstGunSoundConfig GunShotProfile;
    private Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet> m_shotDic = new Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet>();
    public MM_GronchShip_DamagePiece[] DamageZones;
    public MM_GronchShip_Gun.MMGronchGunType Type;
    private bool m_isFireAtWill;
    private bool m_isDestroyed;
    public Transform YRotPiece;
    public Transform XRotPiece;
    public float YRotThreshold = 45f;
    public GameObject VFXPrefab;
    public Transform VFXSpawnPoint;
    [Header("Gun Details")]
    public GameObject ProjectilePrefab;
    public ParticleSystem MuzzleFlash;
    public Transform[] Muzzles;
    public Transform Aimer;
    public Vector2 FireTickRange = new Vector2(0.2f, 0.4f);
    public Vector2 BurstTickRange = new Vector2(0.2f, 0.4f);
    public float InaccuracyRange = 1f;
    public int MinShotsInBurst = 1;
    public int MaxShotsInBurst = 1;
    public int MinBurstsInSequence = 1;
    public int MaxBurstsInSequence = 1;
    public int MuzzleFlashParticleAmount = 2;
    public Vector2 VelocityRange = new Vector2(10f, 20f);
    private int m_currentMuzzle;
    private float m_fireTick;
    private float m_burstTick;
    private int m_shotsLeftInBurst;
    private int m_burstsLeftInSequence;
    private bool m_isFiringSequenceCompleted = true;
    private float angleToTarget = 20f;

    private void Start() => this.PrimeDics();

    public bool IsFiringSequenceCompleted() => this.m_isFiringSequenceCompleted;

    public bool IsDestroyed() => this.m_isDestroyed;

    public void InitiateFiringSequence()
    {
      this.m_isFiringSequenceCompleted = false;
      this.m_currentMuzzle = 0;
      this.m_fireTick = Random.Range(this.FireTickRange.x, this.FireTickRange.y);
      this.m_burstTick = Random.Range(this.BurstTickRange.x, this.BurstTickRange.y);
      this.m_shotsLeftInBurst = Random.Range(this.MinShotsInBurst, this.MaxShotsInBurst + 1);
      this.m_burstsLeftInSequence = Random.Range(this.MinBurstsInSequence, this.MaxBurstsInSequence + 1);
      this.SetFireAtWill(true);
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

    public void SetFireAtWill(bool f) => this.m_isFireAtWill = f;

    private void Update()
    {
      this.DamageCheck();
      this.RotationSystem();
      this.FireControl();
      if (!this.m_isDestroyed)
        return;
      this.m_isFiringSequenceCompleted = true;
    }

    private void RotationSystem()
    {
      if (!this.m_isFireAtWill)
        return;
      Vector3 vector3 = Vector3.ProjectOnPlane(GM.CurrentPlayerBody.transform.position - this.YRotPiece.transform.position, this.transform.up);
      this.YRotPiece.rotation = Quaternion.Slerp(this.YRotPiece.rotation, Quaternion.LookRotation(vector3, this.transform.up), Time.deltaTime * 4f);
      this.YRotPiece.localEulerAngles = new Vector3(0.0f, this.YRotPiece.localEulerAngles.y, 0.0f);
      this.angleToTarget = Vector3.Angle(vector3, this.YRotPiece.forward);
      if ((double) this.angleToTarget >= (double) this.YRotThreshold)
        return;
      this.XRotPiece.rotation = Quaternion.Slerp(this.XRotPiece.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(GM.CurrentPlayerBody.Torso.position + Vector3.up * 0.25f - this.Aimer.position, this.YRotPiece.right), this.transform.up), Time.deltaTime * 4f);
      this.XRotPiece.localEulerAngles = new Vector3(this.XRotPiece.localEulerAngles.x, 0.0f, 0.0f);
    }

    private void FireControl()
    {
      if (this.m_isDestroyed || !this.m_isFireAtWill)
        return;
      if (this.m_burstsLeftInSequence <= 0)
      {
        this.m_isFiringSequenceCompleted = true;
        this.SetFireAtWill(false);
      }
      else if (this.m_shotsLeftInBurst <= 0)
      {
        --this.m_burstsLeftInSequence;
        this.m_burstTick = Random.Range(this.BurstTickRange.x, this.BurstTickRange.y);
        this.m_shotsLeftInBurst = Random.Range(this.MinShotsInBurst, this.MaxShotsInBurst + 1);
      }
      else if ((double) this.m_burstTick > 0.0)
        this.m_burstTick -= Time.deltaTime;
      else if ((double) this.m_fireTick > 0.0)
      {
        this.m_fireTick -= Time.deltaTime;
      }
      else
      {
        if ((double) this.angleToTarget >= 30.0)
          return;
        this.m_fireTick = Random.Range(this.FireTickRange.x, this.FireTickRange.y);
        this.Fire();
        --this.m_shotsLeftInBurst;
      }
    }

    private void Fire()
    {
      this.PlayShotEvent(this.Muzzles[this.m_currentMuzzle].position);
      this.Muzzles[this.m_currentMuzzle].localEulerAngles = new Vector3(Random.Range(-this.InaccuracyRange, this.InaccuracyRange), 0.0f, Random.Range(-this.InaccuracyRange, this.InaccuracyRange));
      GameObject gameObject = Object.Instantiate<GameObject>(this.ProjectilePrefab, this.Muzzles[this.m_currentMuzzle].position, this.Muzzles[this.m_currentMuzzle].rotation);
      BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
      if ((Object) component != (Object) null)
        component.Fire(Random.Range(this.VelocityRange.x, this.VelocityRange.y), this.Muzzles[this.m_currentMuzzle].forward, (FVRFireArm) null);
      else
        gameObject.GetComponent<Rigidbody>().velocity = Random.Range(this.VelocityRange.x, this.VelocityRange.y) * this.Muzzles[this.m_currentMuzzle].forward;
      if ((Object) this.MuzzleFlash != (Object) null)
      {
        this.MuzzleFlash.transform.position = this.Muzzles[this.m_currentMuzzle].position;
        this.MuzzleFlash.Emit(this.MuzzleFlashParticleAmount);
      }
      ++this.m_currentMuzzle;
      if (this.m_currentMuzzle < this.Muzzles.Length)
        return;
      this.m_currentMuzzle = 0;
    }

    private void DamageCheck()
    {
      if (this.m_isDestroyed)
        return;
      bool flag = true;
      for (int index = 0; index < this.DamageZones.Length; ++index)
      {
        if (!this.DamageZones[index].IsDestroyed())
          flag = false;
      }
      if (!flag)
        return;
      this.SetFireAtWill(false);
      this.Explode();
    }

    private void Explode()
    {
      if (this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      Object.Instantiate<GameObject>(this.VFXPrefab, this.VFXSpawnPoint.position, this.VFXSpawnPoint.rotation);
      this.Ship.GunDestroyed();
      this.gameObject.SetActive(false);
    }

    public enum MMGronchGunType
    {
      Gatling,
      Plasma,
      Mortar,
      Rockets,
    }
  }
}
