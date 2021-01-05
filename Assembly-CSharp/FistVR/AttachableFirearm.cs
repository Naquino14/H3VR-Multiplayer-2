// Decompiled with JetBrains decompiler
// Type: FistVR.AttachableFirearm
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class AttachableFirearm : MonoBehaviour
  {
    public AttachableFirearmPhysicalObject Attachment;
    public AttachableFirearmInterface Interface;
    public Transform MuzzlePos;
    public FireArmRoundType RoundType;
    public FVRFireArmMechanicalAccuracyClass AccuracyClass;
    public FVRFireArm OverrideFA;
    [Header("Audio")]
    public FVRFirearmAudioSet AudioClipSet;
    protected SM.AudioSourcePool m_pool_shot;
    protected SM.AudioSourcePool m_pool_tail;
    protected SM.AudioSourcePool m_pool_mechanics;
    protected SM.AudioSourcePool m_pool_handling;
    private bool m_fireWeaponHandlingPerceivableSounds;
    [Header("Recoil")]
    public Transform RecoilingPoseHolder;
    public FVRFireArmRecoilProfile RecoilProfile;
    public bool UsesStockedRecoilProfile;
    public FVRFireArmRecoilProfile RecoilProfileStocked;
    private Vector3 m_recoilPoseHolderLocalPosStart;
    private float m_recoilX;
    private float m_recoilY;
    private float m_recoilLinearZ;
    private Vector2 m_recoilLinearXY = new Vector2(0.0f, 0.0f);
    private Vector2 m_recoilLinearXYBase = new Vector2(0.0f, 0.0f);
    public bool HasActiveShoulderStock;
    public Transform StockPos;
    private float m_massDriftInstability;
    private Vector2 m_curRecoilMassDrift = Vector2.zero;
    private Vector2 m_tarRecoilMassDrift = Vector2.zero;
    [Header("Muzzle Effects")]
    public MuzzleEffectSize DefaultMuzzleEffectSize = MuzzleEffectSize.Standard;
    public MuzzleEffect[] MuzzleEffects;
    private List<MuzzlePSystem> m_muzzleSystems = new List<MuzzlePSystem>();
    private Quaternion m_storedLocalPoseOverrideRot;
    private FVRSceneSettings m_sceneSettings;

    public float GetRecoilZ() => this.m_recoilLinearZ;

    public FVRSceneSettings SceneSettings => this.m_sceneSettings;

    public virtual void Awake()
    {
      this.m_sceneSettings = GM.CurrentSceneSettings;
      this.m_recoilPoseHolderLocalPosStart = this.RecoilingPoseHolder.localPosition;
      this.m_recoilLinearXYBase = new Vector2(this.m_recoilPoseHolderLocalPosStart.x, this.m_recoilPoseHolderLocalPosStart.y);
      this.m_recoilLinearZ = this.m_recoilPoseHolderLocalPosStart.z;
      if ((Object) this.Attachment != (Object) null)
        this.m_storedLocalPoseOverrideRot = this.Attachment.PoseOverride.transform.localRotation;
      this.m_pool_shot = SM.CreatePool(3, 3, FVRPooledAudioType.GunShot);
      if ((Object) this.AudioClipSet == (Object) null)
        Debug.Log((object) ("Missing audio" + this.gameObject.name));
      this.m_pool_tail = SM.CreatePool(this.AudioClipSet.TailConcurrentLimit, this.AudioClipSet.TailConcurrentLimit, FVRPooledAudioType.GunTail);
      this.m_pool_handling = SM.CreatePool(3, 3, FVRPooledAudioType.GunHand);
      this.m_pool_mechanics = SM.CreatePool(3, 3, FVRPooledAudioType.GunMech);
      this.RegenerateMuzzleEffects();
      if ((Object) this.OverrideFA != (Object) null)
        this.OverrideFA.SetIntegratedAttachableFirearm(this);
      if (!this.m_sceneSettings.UsesWeaponHandlingAISound)
        return;
      this.m_fireWeaponHandlingPerceivableSounds = true;
    }

    public FVRFireArmRecoilProfile GetRecoilProfile() => this.UsesStockedRecoilProfile && this.HasActiveShoulderStock ? this.RecoilProfileStocked : this.RecoilProfile;

    public virtual void ProcessInput(FVRViveHand hand, bool fromInterface, FVRInteractiveObject o)
    {
    }

    public virtual void Fire(
      FVRFireArmChamber chamber,
      Transform muzzle,
      bool doBuzz,
      FVRFireArm fa,
      float velMult = 1f)
    {
      if (!doBuzz)
        ;
      if ((Object) fa != (Object) null)
        GM.CurrentSceneSettings.OnShotFired(fa);
      GM.CurrentPlayerBody.VisibleEvent(2f);
      for (int index = 0; index < chamber.GetRound().NumProjectiles; ++index)
      {
        float num = chamber.GetRound().ProjectileSpread + AM.GetFireArmMechanicalSpread(this.AccuracyClass);
        if ((Object) chamber.GetRound().BallisticProjectilePrefab != (Object) null)
        {
          Vector3 vector3 = muzzle.forward * 0.005f;
          GameObject gameObject = Object.Instantiate<GameObject>(chamber.GetRound().BallisticProjectilePrefab, muzzle.position - vector3, muzzle.rotation);
          Vector2 vector2 = Random.insideUnitCircle * num;
          gameObject.transform.Rotate(new Vector3(vector2.x, vector2.y, 0.0f));
          BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
          component.SetSource_IFF(GM.CurrentPlayerBody.GetPlayerIFF());
          component.Fire(component.MuzzleVelocityBase * chamber.ChamberVelocityMultiplier * velMult, gameObject.transform.forward, (FVRFireArm) null);
        }
      }
    }

    public virtual void FireMuzzleSmoke()
    {
      for (int index = 0; index < this.m_muzzleSystems.Count; ++index)
      {
        if ((Object) this.m_muzzleSystems[index].OverridePoint == (Object) null)
          this.m_muzzleSystems[index].PSystem.transform.position = this.MuzzlePos.position;
        this.m_muzzleSystems[index].PSystem.Emit(this.m_muzzleSystems[index].NumParticlesPerShot);
      }
    }

    private void RegenerateMuzzleEffects()
    {
      for (int index = 0; index < this.m_muzzleSystems.Count; ++index)
        Object.Destroy((Object) this.m_muzzleSystems[index].PSystem);
      this.m_muzzleSystems.Clear();
      bool flag = false;
      MuzzleEffect[] muzzleEffects = this.MuzzleEffects;
      for (int index1 = 0; index1 < muzzleEffects.Length; ++index1)
      {
        if (muzzleEffects[index1].Entry != MuzzleEffectEntry.None)
        {
          MuzzleEffectConfig muzzleConfig = FXM.GetMuzzleConfig(muzzleEffects[index1].Entry);
          MuzzleEffectSize muzzleEffectSize = muzzleEffects[index1].Size;
          if (flag)
            muzzleEffectSize = this.DefaultMuzzleEffectSize;
          GameObject gameObject = !GM.CurrentSceneSettings.IsSceneLowLight ? Object.Instantiate<GameObject>(muzzleConfig.Prefabs_Highlight[(int) muzzleEffectSize], this.MuzzlePos.position, this.MuzzlePos.rotation) : Object.Instantiate<GameObject>(muzzleConfig.Prefabs_Lowlight[(int) muzzleEffectSize], this.MuzzlePos.position, this.MuzzlePos.rotation);
          if ((Object) muzzleEffects[index1].OverridePoint == (Object) null)
          {
            gameObject.transform.SetParent(this.MuzzlePos.transform);
          }
          else
          {
            gameObject.transform.SetParent(muzzleEffects[index1].OverridePoint);
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localEulerAngles = Vector3.zero;
          }
          MuzzlePSystem muzzlePsystem = new MuzzlePSystem();
          muzzlePsystem.PSystem = gameObject.GetComponent<ParticleSystem>();
          muzzlePsystem.OverridePoint = muzzleEffects[index1].OverridePoint;
          int index2 = (int) muzzleEffectSize;
          muzzlePsystem.NumParticlesPerShot = !GM.CurrentSceneSettings.IsSceneLowLight ? muzzleConfig.NumParticles_Highlight[index2] : muzzleConfig.NumParticles_Lowlight[index2];
          this.m_muzzleSystems.Add(muzzlePsystem);
        }
      }
    }

    public virtual bool IsTwoHandStabilized(FVRViveHand m_hand)
    {
      bool flag = false;
      if ((Object) m_hand != (Object) null && (Object) m_hand.OtherHand != (Object) null && ((Object) m_hand.OtherHand.CurrentInteractable == (Object) null || m_hand.OtherHand.CurrentInteractable is Flashlight || m_hand.OtherHand.CurrentInteractable is FVRFireArmMagazine) && (double) Vector3.Distance(m_hand.PalmTransform.position, m_hand.OtherHand.PalmTransform.position) < 0.150000005960464)
        flag = true;
      return flag;
    }

    protected virtual void Recoil(
      bool twoHandStabilized,
      bool foregripStabilized,
      bool shoulderStabilized)
    {
      float num1 = 0.0f;
      FVRFireArmRecoilProfile recoilProfile = this.GetRecoilProfile();
      float maxVerticalRot = recoilProfile.MaxVerticalRot;
      float maxHorizontalRot = recoilProfile.MaxHorizontalRot;
      float num2;
      float f1;
      float f2;
      float num3;
      if (foregripStabilized)
      {
        num2 = num1 = recoilProfile.VerticalRotPerShot * Random.Range(0.4f, 0.45f);
        f1 = Random.Range(-recoilProfile.HorizontalRotPerShot, recoilProfile.HorizontalRotPerShot) * Random.Range(0.4f, 0.45f);
        f2 = recoilProfile.ZLinearPerShot * Random.Range(0.8f, 0.9f);
        num3 = recoilProfile.MassDriftFactors.x;
        maxVerticalRot *= 0.6f;
        maxHorizontalRot *= 0.9f;
      }
      else if (twoHandStabilized)
      {
        num2 = recoilProfile.VerticalRotPerShot * Random.Range(0.5f, 0.6f);
        f1 = Random.Range(-recoilProfile.HorizontalRotPerShot, recoilProfile.HorizontalRotPerShot) * Random.Range(0.6f, 0.8f);
        f2 = recoilProfile.ZLinearPerShot * Random.Range(0.8f, 0.9f);
        num3 = recoilProfile.MassDriftFactors.y;
        maxVerticalRot *= 0.8f;
        maxHorizontalRot *= 0.95f;
      }
      else
      {
        num2 = recoilProfile.VerticalRotPerShot * Random.Range(0.9f, 1f);
        f1 = Random.Range(-recoilProfile.HorizontalRotPerShot, recoilProfile.HorizontalRotPerShot) * Random.Range(0.9f, 1f);
        f2 = recoilProfile.ZLinearPerShot * Random.Range(0.9f, 1f);
        num3 = recoilProfile.MassDriftFactors.z;
      }
      if (shoulderStabilized)
      {
        if (foregripStabilized || twoHandStabilized)
        {
          num2 *= 0.3f;
          f1 = Random.Range(-recoilProfile.HorizontalRotPerShot, recoilProfile.HorizontalRotPerShot) * Random.Range(0.2f, 0.3f);
          f2 *= 0.5f;
        }
        else
        {
          num2 *= 0.75f;
          f2 *= 0.7f;
        }
        maxVerticalRot *= 0.5f;
        maxHorizontalRot *= 0.6f;
        num3 *= recoilProfile.MassDriftFactors.w;
      }
      this.m_massDriftInstability += num3 * recoilProfile.MassDriftIntensity;
      float num4 = this.m_recoilX + num2;
      float num5 = 0.0f;
      if ((double) num4 > (double) maxVerticalRot)
      {
        num5 = (float) (((double) num4 - (double) maxVerticalRot) * 0.25);
        num4 = maxVerticalRot - Random.Range(num2 * 0.2f, num2 * 0.8f);
      }
      this.m_recoilX = Mathf.Clamp(num4, -maxVerticalRot, maxVerticalRot);
      this.m_recoilY = Mathf.Clamp((float) ((double) this.m_recoilY + (double) f1 + (double) num5 * (double) Mathf.Sign(f1)), -maxHorizontalRot, maxHorizontalRot);
      this.m_recoilLinearZ += f2;
      this.m_recoilLinearZ = Mathf.Clamp(this.m_recoilLinearZ, this.m_recoilPoseHolderLocalPosStart.z, this.m_recoilPoseHolderLocalPosStart.z + recoilProfile.ZLinearMax);
      Vector2 vector2 = new Vector2((float) ((double) Mathf.Abs(this.m_recoilY) / (double) recoilProfile.MaxHorizontalRot * ((double) Mathf.Abs(f2) / (double) recoilProfile.ZLinearMax)) * Mathf.Sign(this.m_recoilY) * recoilProfile.XYLinearPerShot, (float) (-((double) Mathf.Abs(this.m_recoilX) / (double) recoilProfile.MaxVerticalRot) * ((double) Mathf.Abs(f2) / (double) recoilProfile.ZLinearMax) * (double) Mathf.Sign(this.m_recoilX) * (double) recoilProfile.XYLinearPerShot * 0.100000001490116));
      float xyLinearMax = recoilProfile.XYLinearMax;
      if (shoulderStabilized)
        xyLinearMax *= 0.35f;
      this.m_recoilLinearXY += vector2;
      this.m_recoilLinearXY = Vector2.ClampMagnitude(this.m_recoilLinearXY, xyLinearMax);
    }

    public virtual void Tick(float t, FVRViveHand m_hand)
    {
      if (!((Object) this.RecoilingPoseHolder != (Object) null) || !((Object) this.RecoilProfile != (Object) null))
        return;
      FVRFireArmRecoilProfile recoilProfile = this.GetRecoilProfile();
      bool flag1 = this.IsTwoHandStabilized(m_hand);
      bool flag2 = false;
      bool flag3 = false;
      this.RecoilingPoseHolder.localEulerAngles = new Vector3(this.m_recoilX, this.m_recoilY, 0.0f);
      this.RecoilingPoseHolder.localPosition = new Vector3(this.m_recoilLinearXYBase.x + this.m_recoilLinearXY.x, this.m_recoilLinearXYBase.y + this.m_recoilLinearXY.y, this.m_recoilLinearZ);
      this.m_massDriftInstability = Mathf.Clamp(this.m_massDriftInstability, 0.0f, recoilProfile.MaxMassDriftMagnitude);
      float num1 = 0.0f;
      float num2 = !flag2 ? (!flag1 ? recoilProfile.MassDriftFactors.z * recoilProfile.MaxMassMaxRotation : recoilProfile.MassDriftFactors.y * recoilProfile.MaxMassMaxRotation) : recoilProfile.MassDriftFactors.x * recoilProfile.MaxMassMaxRotation;
      if (flag3)
        num1 = num2 * recoilProfile.MassDriftFactors.w;
      if ((Object) m_hand != (Object) null)
      {
        this.m_tarRecoilMassDrift += new Vector2(-m_hand.Input.VelAngularLocal.y, -m_hand.Input.VelAngularLocal.x) * Time.deltaTime * 10f;
        this.m_tarRecoilMassDrift = Vector2.ClampMagnitude(this.m_tarRecoilMassDrift, recoilProfile.MaxMassMaxRotation);
      }
      else
        this.m_tarRecoilMassDrift = Vector2.zero;
      this.m_massDriftInstability = Mathf.Lerp(this.m_massDriftInstability, 0.0f, Time.deltaTime * recoilProfile.MassDriftRecoveryFactor);
      this.m_curRecoilMassDrift = Vector2.Lerp(this.m_curRecoilMassDrift, this.m_tarRecoilMassDrift, Time.deltaTime * recoilProfile.MassDriftRecoveryFactor);
      float num3 = 1f;
      float x;
      float y;
      float num4;
      float num5;
      if (flag2)
      {
        x = recoilProfile.RecoveryStabilizationFactors_Foregrip.x;
        y = recoilProfile.RecoveryStabilizationFactors_Foregrip.y;
        num4 = recoilProfile.RecoveryStabilizationFactors_Foregrip.z;
        num5 = recoilProfile.RecoveryStabilizationFactors_Foregrip.w;
        num3 = 1.5f;
      }
      else if (flag1)
      {
        x = recoilProfile.RecoveryStabilizationFactors_Twohand.x;
        y = recoilProfile.RecoveryStabilizationFactors_Twohand.y;
        num4 = recoilProfile.RecoveryStabilizationFactors_Twohand.z;
        num5 = recoilProfile.RecoveryStabilizationFactors_Twohand.w;
        num3 = 1.2f;
      }
      else
      {
        x = recoilProfile.RecoveryStabilizationFactors_None.x;
        y = recoilProfile.RecoveryStabilizationFactors_None.y;
        num4 = recoilProfile.RecoveryStabilizationFactors_None.z;
        num5 = recoilProfile.RecoveryStabilizationFactors_None.w;
      }
      if (flag3)
      {
        num4 = 1f;
        num5 = 1f;
        num3 = 4f;
      }
      this.m_tarRecoilMassDrift = Vector2.Lerp(this.m_tarRecoilMassDrift, Vector2.zero, Time.deltaTime * recoilProfile.MassDriftRecoveryFactor * num3);
      Vector2 vector2 = Vector2.ClampMagnitude(this.m_curRecoilMassDrift * this.m_massDriftInstability, recoilProfile.MaxMassMaxRotation);
      this.m_recoilX = Mathf.Lerp(this.m_recoilX, vector2.y, Time.deltaTime * y * recoilProfile.VerticalRotRecovery * Random.Range(0.9f, 1.1f));
      this.m_recoilY = Mathf.Lerp(this.m_recoilY, vector2.x, Time.deltaTime * x * recoilProfile.HorizontalRotRecovery * Random.Range(0.9f, 1.1f));
      this.m_recoilLinearZ = Mathf.Lerp(this.m_recoilLinearZ, this.m_recoilPoseHolderLocalPosStart.z, Time.deltaTime * recoilProfile.ZLinearRecovery * num5 * Random.Range(0.9f, 1.1f));
      this.m_recoilLinearXY = Vector2.Lerp(this.m_recoilLinearXY, Vector2.zero, Time.deltaTime * recoilProfile.XYLinearRecovery * num4 * Random.Range(0.9f, 1.1f));
    }

    public void PlayAudioGunShot(FVRFireArmRound round, FVRSoundEnvironment TailEnvironment)
    {
      Vector3 position = this.transform.position;
      if (this.AudioClipSet.UsesLowPressureSet)
      {
        if (round.IsHighPressure)
        {
          this.PlayAudioEvent(FirearmAudioEventType.Shots_Main);
          if (this.AudioClipSet.UsesTail_Main)
          {
            AudioEvent tailSet = SM.GetTailSet(round.TailClass, TailEnvironment);
            this.m_pool_tail.PlayClipPitchOverride(tailSet, position, this.AudioClipSet.TailPitchMod_Main * tailSet.PitchRange.x);
          }
        }
        else
        {
          this.PlayAudioEvent(FirearmAudioEventType.Shots_LowPressure);
          if (this.AudioClipSet.UsesTail_Main)
          {
            AudioEvent tailSet = SM.GetTailSet(round.TailClass, TailEnvironment);
            this.m_pool_tail.PlayClipPitchOverride(tailSet, position, this.AudioClipSet.TailPitchMod_LowPressure * tailSet.PitchRange.x);
          }
        }
      }
      else
      {
        this.PlayAudioEvent(FirearmAudioEventType.Shots_Main);
        if (this.AudioClipSet.UsesTail_Main)
        {
          AudioEvent tailSet = SM.GetTailSet(round.TailClass, TailEnvironment);
          this.m_pool_tail.PlayClipPitchOverride(tailSet, position, this.AudioClipSet.TailPitchMod_Main * tailSet.PitchRange.x);
        }
      }
      float multByEnvironment = SM.GetSoundTravelDistanceMultByEnvironment(TailEnvironment);
      int playerIff = GM.CurrentPlayerBody.GetPlayerIFF();
      if (this.AudioClipSet.UsesLowPressureSet && !round.IsHighPressure)
        GM.CurrentSceneSettings.OnPerceiveableSound(this.AudioClipSet.Loudness_Primary * 0.6f, this.AudioClipSet.Loudness_Primary * 0.6f * multByEnvironment, this.transform.position, playerIff);
      else
        GM.CurrentSceneSettings.OnPerceiveableSound(this.AudioClipSet.Loudness_Primary, this.AudioClipSet.Loudness_Primary * multByEnvironment, this.transform.position, playerIff);
    }

    public void PlayAudioGunShot(
      bool IsHighPressure,
      FVRTailSoundClass TailClass,
      FVRTailSoundClass TailClassSuppressed,
      FVRSoundEnvironment TailEnvironment)
    {
      Vector3 position = this.transform.position;
      float pitchmod = 1f;
      if (IsHighPressure)
      {
        this.PlayAudioEvent(FirearmAudioEventType.Shots_Main, pitchmod);
        if (this.AudioClipSet.UsesTail_Main)
        {
          AudioEvent tailSet = SM.GetTailSet(TailClass, TailEnvironment);
          this.m_pool_tail.PlayClipPitchOverride(tailSet, position, this.AudioClipSet.TailPitchMod_Main * tailSet.PitchRange.x * pitchmod);
        }
      }
      else
      {
        this.PlayAudioEvent(FirearmAudioEventType.Shots_LowPressure, pitchmod);
        if (this.AudioClipSet.UsesTail_Main)
        {
          AudioEvent tailSet = SM.GetTailSet(TailClass, TailEnvironment);
          this.m_pool_tail.PlayClipPitchOverride(tailSet, position, this.AudioClipSet.TailPitchMod_LowPressure * tailSet.PitchRange.x * pitchmod);
        }
      }
      float multByEnvironment = SM.GetSoundTravelDistanceMultByEnvironment(TailEnvironment);
      int playerIff = GM.CurrentPlayerBody.GetPlayerIFF();
      if (this.AudioClipSet.UsesLowPressureSet && !IsHighPressure)
        GM.CurrentSceneSettings.OnPerceiveableSound(this.AudioClipSet.Loudness_Primary * 0.6f, this.AudioClipSet.Loudness_Primary * 0.6f * multByEnvironment, this.transform.position, playerIff);
      else
        GM.CurrentSceneSettings.OnPerceiveableSound(this.AudioClipSet.Loudness_Primary, this.AudioClipSet.Loudness_Primary * multByEnvironment, this.transform.position, playerIff);
    }

    public void PlayAudioEventHandling(AudioEvent e)
    {
      Vector3 position = this.transform.position;
      this.m_pool_handling.PlayClip(e, position);
    }

    public void PlayAudioEvent(FirearmAudioEventType eType, float pitchmod = 1f)
    {
      Vector3 position = this.transform.position;
      float num = 1f;
      if (this.m_fireWeaponHandlingPerceivableSounds)
        num = SM.GetSoundTravelDistanceMultByEnvironment(GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
      int playerIff = GM.CurrentPlayerBody.GetPlayerIFF();
      switch (eType)
      {
        case FirearmAudioEventType.BoltSlideForward:
          this.m_pool_mechanics.PlayClip(this.AudioClipSet.BoltSlideForward, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(20f * this.AudioClipSet.Loudness_OperationMult, (float) (20.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.BoltSlideBack:
          this.m_pool_mechanics.PlayClip(this.AudioClipSet.BoltSlideBack, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(40f * this.AudioClipSet.Loudness_OperationMult, (float) (20.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.BoltSlideForwardHeld:
          this.m_pool_mechanics.PlayClip(this.AudioClipSet.BoltSlideForwardHeld, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(20f * this.AudioClipSet.Loudness_OperationMult, (float) (20.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.BoltSlideBackHeld:
          this.m_pool_mechanics.PlayClip(this.AudioClipSet.BoltSlideBackHeld, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(20f * this.AudioClipSet.Loudness_OperationMult, (float) (20.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.BoltSlideBackLocked:
          this.m_pool_mechanics.PlayClip(this.AudioClipSet.BoltSlideBackLocked, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(20f * this.AudioClipSet.Loudness_OperationMult, (float) (20.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.CatchOnSear:
          this.m_pool_mechanics.PlayClip(this.AudioClipSet.CatchOnSear, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(20f * this.AudioClipSet.Loudness_OperationMult, (float) (20.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.HammerHit:
          this.m_pool_mechanics.PlayClip(this.AudioClipSet.HammerHit, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(40f * this.AudioClipSet.Loudness_OperationMult, (float) (20.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.Prefire:
          this.m_pool_mechanics.PlayClip(this.AudioClipSet.Prefire, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(15f * this.AudioClipSet.Loudness_OperationMult, (float) (15.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.BoltRelease:
          this.m_pool_mechanics.PlayClip(this.AudioClipSet.BoltRelease, position);
          break;
        case FirearmAudioEventType.HandleGrab:
          this.m_pool_handling.PlayClip(this.AudioClipSet.HandleGrab, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(10f * this.AudioClipSet.Loudness_OperationMult, (float) (10.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.HandleBack:
          this.m_pool_handling.PlayClip(this.AudioClipSet.HandleBack, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(20f * this.AudioClipSet.Loudness_OperationMult, (float) (20.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.HandleForward:
          this.m_pool_handling.PlayClip(this.AudioClipSet.HandleForward, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(20f * this.AudioClipSet.Loudness_OperationMult, (float) (20.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.HandleUp:
          this.m_pool_handling.PlayClip(this.AudioClipSet.HandleUp, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(10f * this.AudioClipSet.Loudness_OperationMult, (float) (10.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.HandleDown:
          this.m_pool_handling.PlayClip(this.AudioClipSet.HandleDown, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(20f * this.AudioClipSet.Loudness_OperationMult, (float) (20.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.Safety:
          this.m_pool_handling.PlayClip(this.AudioClipSet.Safety, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(8f * this.AudioClipSet.Loudness_OperationMult, (float) (8.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.FireSelector:
          this.m_pool_handling.PlayClip(this.AudioClipSet.FireSelector, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(8f * this.AudioClipSet.Loudness_OperationMult, (float) (8.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.TriggerReset:
          this.m_pool_handling.PlayClip(this.AudioClipSet.TriggerReset, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(3f * this.AudioClipSet.Loudness_OperationMult, (float) (3.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.BreachOpen:
          this.m_pool_handling.PlayClip(this.AudioClipSet.BreachOpen, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(20f * this.AudioClipSet.Loudness_OperationMult, (float) (20.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.BreachClose:
          this.m_pool_handling.PlayClip(this.AudioClipSet.BreachClose, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(20f * this.AudioClipSet.Loudness_OperationMult, (float) (20.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.MagazineIn:
          this.m_pool_handling.PlayClip(this.AudioClipSet.MagazineIn, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(20f * this.AudioClipSet.Loudness_OperationMult, (float) (20.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.MagazineOut:
          this.m_pool_handling.PlayClip(this.AudioClipSet.MagazineOut, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(10f * this.AudioClipSet.Loudness_OperationMult, (float) (10.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.TopCoverRelease:
          this.m_pool_handling.PlayClip(this.AudioClipSet.TopCoverRelease, position);
          break;
        case FirearmAudioEventType.TopCoverUp:
          this.m_pool_handling.PlayClip(this.AudioClipSet.TopCoverUp, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(10f * this.AudioClipSet.Loudness_OperationMult, (float) (10.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.TopCoverDown:
          this.m_pool_handling.PlayClip(this.AudioClipSet.TopCoverDown, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(10f * this.AudioClipSet.Loudness_OperationMult, (float) (10.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.MagazineInsertRound:
          this.m_pool_handling.PlayClip(this.AudioClipSet.MagazineInsertRound, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(10f * this.AudioClipSet.Loudness_OperationMult, (float) (10.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.MagazineEjectRound:
          this.m_pool_handling.PlayClip(this.AudioClipSet.MagazineEjectRound, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(10f * this.AudioClipSet.Loudness_OperationMult, (float) (10.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.StockOpen:
          this.m_pool_handling.PlayClip(this.AudioClipSet.StockOpen, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(10f * this.AudioClipSet.Loudness_OperationMult, (float) (10.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.StockClosed:
          this.m_pool_handling.PlayClip(this.AudioClipSet.StockClosed, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(10f * this.AudioClipSet.Loudness_OperationMult, (float) (10.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.BipodOpen:
          this.m_pool_handling.PlayClip(this.AudioClipSet.BipodOpen, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(10f * this.AudioClipSet.Loudness_OperationMult, (float) (10.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.BipodClosed:
          this.m_pool_handling.PlayClip(this.AudioClipSet.BipodClosed, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(10f * this.AudioClipSet.Loudness_OperationMult, (float) (10.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.HandleForwardEmpty:
          this.m_pool_handling.PlayClip(this.AudioClipSet.HandleForwardEmpty, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(20f * this.AudioClipSet.Loudness_OperationMult, (float) (20.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.HandleBackEmpty:
          this.m_pool_handling.PlayClip(this.AudioClipSet.HandleBackEmpty, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(20f * this.AudioClipSet.Loudness_OperationMult, (float) (20.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.ChamberManual:
          this.m_pool_mechanics.PlayClip(this.AudioClipSet.ChamberManual, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(20f * this.AudioClipSet.Loudness_OperationMult, (float) (20.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.BeltGrab:
          this.m_pool_handling.PlayClip(this.AudioClipSet.BeltGrab, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(10f * this.AudioClipSet.Loudness_OperationMult, (float) (10.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.BeltRelease:
          this.m_pool_handling.PlayClip(this.AudioClipSet.BeltRelease, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(10f * this.AudioClipSet.Loudness_OperationMult, (float) (10.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.BeltSeat:
          this.m_pool_handling.PlayClip(this.AudioClipSet.BeltSeat, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(10f * this.AudioClipSet.Loudness_OperationMult, (float) (10.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        default:
          switch (eType - 100)
          {
            case FirearmAudioEventType.BoltSlideForward:
              this.m_pool_shot.PlayClipPitchOverride(this.AudioClipSet.Shots_Main, this.MuzzlePos.position, new Vector2(this.AudioClipSet.Shots_Main.PitchRange.x * pitchmod, this.AudioClipSet.Shots_Main.PitchRange.y * pitchmod));
              return;
            case FirearmAudioEventType.BoltSlideBack:
              this.m_pool_shot.PlayClip(this.AudioClipSet.Shots_Suppressed, this.MuzzlePos.position);
              return;
            case FirearmAudioEventType.BoltSlideForwardHeld:
              this.m_pool_shot.PlayClip(this.AudioClipSet.Shots_LowPressure, this.MuzzlePos.position);
              return;
            default:
              return;
          }
      }
    }
  }
}
