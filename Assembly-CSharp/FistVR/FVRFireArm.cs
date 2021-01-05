// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFireArm
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FVRFireArm : FVRPhysicalObject
  {
    [Header("FireArm Config")]
    [SearchableEnum]
    public FireArmMagazineType MagazineType;
    public FireArmClipType ClipType;
    public FireArmRoundType RoundType;
    public FVRFireArmMechanicalAccuracyClass AccuracyClass;
    [Header("Muzzle Params")]
    public Transform MuzzlePos;
    public GameObject Foregrip;
    [Header("NewAudioImplementation")]
    public FVRFirearmAudioSet AudioClipSet;
    protected SM.AudioSourcePool m_pool_shot;
    protected SM.AudioSourcePool m_pool_tail;
    protected SM.AudioSourcePool m_pool_mechanics;
    protected SM.AudioSourcePool m_pool_handling;
    protected SM.AudioSourcePool m_pool_belt;
    private bool m_fireWeaponHandlingPerceivableSounds;
    [Header("Firing/Suppression Params")]
    public FVRFireArm.MuzzleState DefaultMuzzleState;
    public FVRFireArmMechanicalAccuracyClass DefaultMuzzleDamping;
    protected bool m_isSuppressed;
    protected bool m_isBraked;
    public Suppressor CurrentSuppressor;
    public MuzzleBrake CurrentMuzzleBrake;
    public List<MuzzleDevice> MuzzleDevices = new List<MuzzleDevice>();
    public Transform CurrentMuzzle;
    public AttachableMeleeWeapon CurrentAttachableMeleeWeapon;
    [Header("Magazine Params")]
    public bool UsesMagazines;
    public bool UsesBeltBoxes;
    public Transform MagazineMountPos;
    public Transform MagazineEjectPos;
    public FVRFireArmMagazine Magazine;
    public Transform BeltBoxMountPos;
    public Transform BeltBoxEjectPos;
    public bool UsesBelts;
    public FVRFirearmBeltDisplayData BeltDD;
    public bool ConnectedToBox;
    public bool HasBelt;
    public bool UsesTopCover;
    public bool IsTopCoverUp;
    public bool RequiresBoltBackToSeatBelt;
    [Header("Clip Params")]
    public bool UsesClips;
    public Transform ClipMountPos;
    public Transform ClipEjectPos;
    public FVRFireArmClip Clip;
    public GameObject ClipTrigger;
    private float m_clipEjectDelay;
    [Header("Recoil Params")]
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
    protected float m_ejectDelay;
    private FVRFireArmMagazine m_lastEjectedMag;
    [Header("Muzzle Smoke Params")]
    public MuzzleEffectSize DefaultMuzzleEffectSize = MuzzleEffectSize.Standard;
    public MuzzleEffect[] MuzzleEffects;
    private List<MuzzlePSystem> m_muzzleSystems = new List<MuzzlePSystem>();
    public FVRFireArm.GasOutEffect[] GasOutEffects;
    public bool ControlsOwnGasOut = true;
    [HideInInspector]
    public bool IsBreachOpenForGasOut = true;
    [Header("Firing/Suppression Params")]
    private AttachableFirearm m_integratedAttachedFirearm;
    private Quaternion m_storedLocalPoseOverrideRot;
    public FVRSceneSettings SceneSettings;
    private float m_internalMechanicalMOA;
    public float StockDist;

    public bool IsSuppressed() => this.m_isSuppressed || this.DefaultMuzzleState == FVRFireArm.MuzzleState.Suppressor && this.MuzzleDevices.Count == 0;

    public bool IsBraked() => this.m_isBraked || this.DefaultMuzzleState == FVRFireArm.MuzzleState.MuzzleBreak && this.MuzzleDevices.Count == 0;

    public Transform GetMagMountPos(bool isBeltBox) => isBeltBox ? this.BeltBoxMountPos : this.MagazineMountPos;

    public Transform GetMagEjectPos(bool isBeltBox) => isBeltBox ? this.BeltBoxEjectPos : this.MagazineEjectPos;

    public float ClipEjectDelay => this.m_clipEjectDelay;

    public float GetRecoilZ() => this.m_recoilLinearZ;

    public float EjectDelay
    {
      get => this.m_ejectDelay;
      set => this.m_ejectDelay = value;
    }

    public FVRFireArmMagazine LastEjectedMag => this.m_lastEjectedMag;

    public AttachableFirearm GetIntegratedAttachableFirearm() => this.m_integratedAttachedFirearm;

    public void SetIntegratedAttachableFirearm(AttachableFirearm a) => this.m_integratedAttachedFirearm = a;

    protected override void Awake()
    {
      base.Awake();
      this.CurrentMuzzle = this.MuzzlePos;
      this.m_internalMechanicalMOA = AM.GetFireArmMechanicalSpread(this.AccuracyClass);
      this.SceneSettings = GM.CurrentSceneSettings;
      this.m_recoilPoseHolderLocalPosStart = this.RecoilingPoseHolder.localPosition;
      this.m_recoilLinearXYBase = new Vector2(this.m_recoilPoseHolderLocalPosStart.x, this.m_recoilPoseHolderLocalPosStart.y);
      this.m_recoilLinearZ = this.m_recoilPoseHolderLocalPosStart.z;
      this.RegenerateMuzzleEffects((MuzzleDevice) null);
      int num = 0;
      while (num < this.MuzzleEffects.Length)
        ++num;
      for (int index = 0; index < this.GasOutEffects.Length; ++index)
      {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.GasOutEffects[index].EffectPrefab, this.GasOutEffects[index].Parent.position, this.GasOutEffects[index].Parent.rotation);
        gameObject.transform.SetParent(this.GasOutEffects[index].Parent.transform);
        this.GasOutEffects[index].PSystem = gameObject.GetComponent<ParticleSystem>();
      }
      this.m_storedLocalPoseOverrideRot = this.PoseOverride.transform.localRotation;
      this.m_pool_shot = SM.CreatePool(3, 3, FVRPooledAudioType.GunShot);
      if ((UnityEngine.Object) this.AudioClipSet == (UnityEngine.Object) null)
        Debug.Log((object) ("Missing audio" + this.gameObject.name));
      this.m_pool_tail = SM.CreatePool(this.AudioClipSet.TailConcurrentLimit, this.AudioClipSet.TailConcurrentLimit, FVRPooledAudioType.GunTail);
      this.m_pool_handling = SM.CreatePool(3, 3, FVRPooledAudioType.GunHand);
      this.m_pool_mechanics = SM.CreatePool(3, 3, FVRPooledAudioType.GunMech);
      if (this.AudioClipSet.BeltSettlingLimit > 0)
        this.m_pool_belt = SM.CreatePool(this.AudioClipSet.BeltSettlingLimit, this.AudioClipSet.BeltSettlingLimit, FVRPooledAudioType.GunMech);
      if (!GM.CurrentSceneSettings.UsesWeaponHandlingAISound)
        return;
      this.m_fireWeaponHandlingPerceivableSounds = true;
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (this.ControlsOwnGasOut)
      {
        for (int index = 0; index < this.GasOutEffects.Length; ++index)
          this.GasOutEffects[index].GasUpdate(this.IsBreachOpenForGasOut);
      }
      if (!this.UsesBelts)
        return;
      this.BeltDD.UpdateBelt();
    }

    public FVRFireArmRecoilProfile GetRecoilProfile() => this.UsesStockedRecoilProfile && this.HasActiveShoulderStock ? this.RecoilProfileStocked : this.RecoilProfile;

    public virtual void FireMuzzleSmoke()
    {
      if (GM.CurrentSceneSettings.IsSceneLowLight)
      {
        if (this.IsSuppressed())
          FXM.InitiateMuzzleFlash(this.GetMuzzle().position, this.GetMuzzle().forward, 0.25f, new Color(1f, 0.9f, 0.77f), 0.5f);
        else
          FXM.InitiateMuzzleFlash(this.GetMuzzle().position, this.GetMuzzle().forward, 1f, new Color(1f, 0.9f, 0.77f), 1f);
      }
      for (int index = 0; index < this.m_muzzleSystems.Count; ++index)
      {
        if ((UnityEngine.Object) this.m_muzzleSystems[index].OverridePoint == (UnityEngine.Object) null)
          this.m_muzzleSystems[index].PSystem.transform.position = this.GetMuzzle().position;
        this.m_muzzleSystems[index].PSystem.Emit(this.m_muzzleSystems[index].NumParticlesPerShot);
      }
      for (int index = 0; index < this.GasOutEffects.Length; ++index)
        this.GasOutEffects[index].AddGas(this.IsSuppressed());
    }

    public virtual void FireMuzzleSmoke(int i)
    {
      if (GM.CurrentSceneSettings.IsSceneLowLight)
      {
        if (this.IsSuppressed())
          FXM.InitiateMuzzleFlash(this.GetMuzzle().position, this.GetMuzzle().forward, 0.25f, new Color(1f, 0.9f, 0.77f), 0.5f);
        else
          FXM.InitiateMuzzleFlash(this.GetMuzzle().position, this.GetMuzzle().forward, 1f, new Color(1f, 0.9f, 0.77f), 1f);
      }
      if ((UnityEngine.Object) this.m_muzzleSystems[i].OverridePoint == (UnityEngine.Object) null)
        this.m_muzzleSystems[i].PSystem.transform.position = this.GetMuzzle().position;
      this.m_muzzleSystems[i].PSystem.Emit(this.m_muzzleSystems[i].NumParticlesPerShot);
    }

    public virtual void AddGas(int i) => this.GasOutEffects[i].AddGas(this.IsSuppressed());

    public void RattleSuppresor()
    {
      if (!((UnityEngine.Object) this.CurrentSuppressor != (UnityEngine.Object) null))
        return;
      this.CurrentSuppressor.ShotEffect();
      if ((double) this.CurrentSuppressor.CatchRot >= 90.0 || this.CurrentSuppressor.IsIntegrate)
        return;
      this.CurrentSuppressor.CatchRot -= UnityEngine.Random.Range(1f, 9f);
      this.CurrentSuppressor.CatchRot = Mathf.Clamp(this.CurrentSuppressor.CatchRot, 0.0f, 360f);
      this.CurrentSuppressor.transform.localEulerAngles = new Vector3(0.0f, 0.0f, this.CurrentSuppressor.CatchRot);
      if ((double) this.CurrentSuppressor.CatchRot >= 1.0)
        return;
      Suppressor currentSuppressor = this.CurrentSuppressor;
      this.CurrentSuppressor.DetachFromMount();
      currentSuppressor.RootRigidbody.AddForceAtPosition((this.MuzzlePos.forward + UnityEngine.Random.onUnitSphere * 0.02f) * 0.3f, this.MuzzlePos.position + UnityEngine.Random.onUnitSphere * (1f / 500f), ForceMode.Impulse);
    }

    public virtual Transform GetMuzzle() => this.CurrentMuzzle;

    public void RegisterMuzzleDevice(MuzzleDevice m)
    {
      if (!((UnityEngine.Object) m != (UnityEngine.Object) null) || this.MuzzleDevices.Contains(m))
        return;
      this.MuzzleDevices.Add(m);
      this.UpdateCurrentMuzzle();
    }

    public void DeRegisterMuzzleDevice(MuzzleDevice m)
    {
      if (!((UnityEngine.Object) m != (UnityEngine.Object) null) || !this.MuzzleDevices.Contains(m))
        return;
      this.MuzzleDevices.Remove(m);
      this.UpdateCurrentMuzzle();
    }

    public float GetCombinedMuzzleDeviceAccuracy()
    {
      float num = 0.0f;
      for (int index = 0; index < this.MuzzleDevices.Count; ++index)
        num += this.MuzzleDevices[index].GetMechanicalAccuracy();
      return num;
    }

    public float GetCombinedFixedDrop(FVRFireArmMechanicalAccuracyClass c)
    {
      if (this.MuzzleDevices.Count < 1)
        return 0.0f;
      float max = 1f;
      if (this.IsBraked() || this.IsSuppressed())
        max = AM.GetDropMult(c);
      for (int index = 0; index < this.MuzzleDevices.Count; ++index)
        max *= this.MuzzleDevices[index].GetDropMult(this);
      return Mathf.Clamp(max - 1f, 0.0f, max);
    }

    public Vector2 GetCombinedFixedDrift(FVRFireArmMechanicalAccuracyClass c)
    {
      if (this.MuzzleDevices.Count < 1)
        return Vector2.zero;
      Vector2 vector2 = new Vector2(1f, 1f);
      if (this.IsBraked() || this.IsSuppressed())
        vector2 = new Vector2(AM.GetDriftMult(c), AM.GetDriftMult(c));
      Vector2 driftMult = this.MuzzleDevices[this.MuzzleDevices.Count - 1].GetDriftMult(this);
      vector2 = new Vector2(vector2.x * driftMult.x, vector2.y * driftMult.y);
      return vector2;
    }

    public float GetReductionFactor() => this.MuzzleDevices.Count > 0 ? AM.GetRecoilMult(this.MuzzleDevices[this.MuzzleDevices.Count - 1].MechanicalAccuracy) : AM.GetRecoilMult(this.DefaultMuzzleDamping);

    public void RegisterAttachedMeleeWeapon(AttachableMeleeWeapon a)
    {
      this.CurrentAttachableMeleeWeapon = a;
      if ((UnityEngine.Object) a == (UnityEngine.Object) null)
      {
        this.MP.CanNewStab = false;
        this.MP.HighDamageBCP = Vector3.zero;
        this.MP.StabDamageBCP = Vector3.zero;
        this.MP.TearDamageBCP = Vector3.zero;
        this.MP.HighDamageColliders.Clear();
        this.MP.HighDamageVectors.Clear();
        this.MP.StabColliders.Clear();
        this.MP.StabDirection = (Transform) null;
      }
      else
      {
        this.MP.HighDamageBCP = a.MP.HighDamageBCP;
        this.MP.StabDamageBCP = a.MP.StabDamageBCP;
        this.MP.TearDamageBCP = a.MP.TearDamageBCP;
        this.MP.HighDamageColliders.Clear();
        this.MP.HighDamageVectors.Clear();
        this.MP.StabColliders.Clear();
        this.MP.StabDirection = a.MP.StabDirection;
        this.MP.CanNewStab = a.MP.CanNewStab;
        this.MP.BladeLength = a.MP.BladeLength;
        this.MP.MassWhileStabbed = a.MP.MassWhileStabbed;
        this.MP.StabAngularThreshold = a.MP.StabAngularThreshold;
        this.MP.StabVelocityRequirement = a.MP.StabVelocityRequirement;
        this.MP.CanTearOut = a.MP.CanTearOut;
        this.MP.TearOutVelThreshold = a.MP.TearOutVelThreshold;
        for (int index = 0; index < a.MP.HighDamageColliders.Count; ++index)
          this.MP.HighDamageColliders.Add(a.MP.HighDamageColliders[index]);
        for (int index = 0; index < a.MP.HighDamageVectors.Count; ++index)
          this.MP.HighDamageVectors.Add(a.MP.HighDamageVectors[index]);
        for (int index = 0; index < a.MP.StabColliders.Count; ++index)
          this.MP.StabColliders.Add(a.MP.StabColliders[index]);
      }
    }

    public void RegisterSuppressor(Suppressor s)
    {
      this.CurrentSuppressor = s;
      if ((UnityEngine.Object) s == (UnityEngine.Object) null)
        this.m_isSuppressed = false;
      else
        this.m_isSuppressed = true;
    }

    public void RegisterMuzzleBrake(MuzzleBrake m)
    {
      this.CurrentMuzzleBrake = m;
      if ((UnityEngine.Object) m == (UnityEngine.Object) null)
        this.m_isBraked = false;
      else
        this.m_isBraked = true;
    }

    private void UpdateCurrentMuzzle()
    {
      if (this.MuzzleDevices.Count == 0)
      {
        this.CurrentMuzzle = this.MuzzlePos;
        this.RegenerateMuzzleEffects((MuzzleDevice) null);
      }
      else
      {
        this.CurrentMuzzle = this.MuzzleDevices[this.MuzzleDevices.Count - 1].Muzzle;
        this.RegenerateMuzzleEffects(this.MuzzleDevices[this.MuzzleDevices.Count - 1]);
      }
      this.UpdateGasOutLocation();
    }

    private void RegenerateMuzzleEffects(MuzzleDevice m)
    {
      for (int index = 0; index < this.m_muzzleSystems.Count; ++index)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_muzzleSystems[index].PSystem);
      this.m_muzzleSystems.Clear();
      bool flag = false;
      MuzzleEffect[] muzzleEffects = this.MuzzleEffects;
      if ((UnityEngine.Object) m != (UnityEngine.Object) null)
      {
        muzzleEffects = m.MuzzleEffects;
        if (!m.ForcesEffectSize)
          flag = true;
      }
      for (int index1 = 0; index1 < muzzleEffects.Length; ++index1)
      {
        if (muzzleEffects[index1].Entry != MuzzleEffectEntry.None)
        {
          MuzzleEffectConfig muzzleConfig = FXM.GetMuzzleConfig(muzzleEffects[index1].Entry);
          MuzzleEffectSize muzzleEffectSize = muzzleEffects[index1].Size;
          if (flag)
            muzzleEffectSize = this.DefaultMuzzleEffectSize;
          GameObject gameObject = !GM.CurrentSceneSettings.IsSceneLowLight ? UnityEngine.Object.Instantiate<GameObject>(muzzleConfig.Prefabs_Highlight[(int) muzzleEffectSize], this.MuzzlePos.position, this.MuzzlePos.rotation) : UnityEngine.Object.Instantiate<GameObject>(muzzleConfig.Prefabs_Lowlight[(int) muzzleEffectSize], this.MuzzlePos.position, this.MuzzlePos.rotation);
          if ((UnityEngine.Object) muzzleEffects[index1].OverridePoint == (UnityEngine.Object) null)
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

    private void UpdateGasOutLocation()
    {
      for (int index = 0; index < this.GasOutEffects.Length; ++index)
      {
        if (this.GasOutEffects[index].FollowsMuzzle)
          this.GasOutEffects[index].PSystem.transform.position = this.GetMuzzle().position;
      }
    }

    public override void SetQuickBeltSlot(FVRQuickBeltSlot slot)
    {
      if ((UnityEngine.Object) slot != (UnityEngine.Object) null && !this.IsHeld)
      {
        if ((UnityEngine.Object) this.Magazine != (UnityEngine.Object) null)
          this.Magazine.SetAllCollidersToLayer(false, "NoCol");
      }
      else if ((UnityEngine.Object) this.Magazine != (UnityEngine.Object) null)
        this.Magazine.SetAllCollidersToLayer(false, "Default");
      base.SetQuickBeltSlot(slot);
    }

    public virtual bool IsTwoHandStabilized()
    {
      bool flag = false;
      if ((UnityEngine.Object) this.m_hand != (UnityEngine.Object) null && (UnityEngine.Object) this.m_hand.OtherHand != (UnityEngine.Object) null && ((UnityEngine.Object) this.m_hand.OtherHand.CurrentInteractable == (UnityEngine.Object) null || this.m_hand.OtherHand.CurrentInteractable is Flashlight || this.m_hand.OtherHand.CurrentInteractable is FVRFireArmMagazine) && (double) Vector3.Distance(this.m_hand.PalmTransform.position, this.m_hand.OtherHand.PalmTransform.position) < 0.150000005960464)
        flag = true;
      return flag;
    }

    public virtual bool IsShoulderStabilized()
    {
      if (!this.HasActiveShoulderStock || (UnityEngine.Object) this.StockPos == (UnityEngine.Object) null)
        return false;
      float num = Vector3.Distance(this.GetClosestValidPoint(this.GetClosestValidPoint(GM.CurrentPlayerBody.Torso.position + GM.CurrentPlayerBody.Torso.right * 0.15f + GM.CurrentPlayerBody.Torso.forward * 0.1f - GM.CurrentPlayerBody.Torso.up * 0.1f, GM.CurrentPlayerBody.Torso.position + GM.CurrentPlayerBody.Torso.right * -0.15f + GM.CurrentPlayerBody.Torso.forward * 0.1f - GM.CurrentPlayerBody.Torso.up * 0.1f, this.StockPos.position), this.GetClosestValidPoint(GM.CurrentPlayerBody.Torso.position + GM.CurrentPlayerBody.Torso.right * 0.15f + GM.CurrentPlayerBody.Torso.forward * 0.1f + GM.CurrentPlayerBody.Torso.up * 0.1f, GM.CurrentPlayerBody.Torso.position + GM.CurrentPlayerBody.Torso.right * -0.15f + GM.CurrentPlayerBody.Torso.forward * 0.1f + GM.CurrentPlayerBody.Torso.up * 0.1f, this.StockPos.position), this.StockPos.position), this.StockPos.position);
      this.StockDist = num;
      return (double) num <= 0.200000002980232;
    }

    public virtual bool IsForegripStabilized() => !((UnityEngine.Object) this.AltGrip == (UnityEngine.Object) null);

    public virtual void Recoil(
      bool twoHandStabilized,
      bool foregripStabilized,
      bool shoulderStabilized,
      FVRFireArmRecoilProfile overrideprofile = null,
      float VerticalRecoilMult = 1f)
    {
      float num1 = 0.0f;
      FVRFireArmRecoilProfile armRecoilProfile = !((UnityEngine.Object) overrideprofile != (UnityEngine.Object) null) ? this.GetRecoilProfile() : overrideprofile;
      float max1 = armRecoilProfile.MaxVerticalRot;
      float max2 = armRecoilProfile.MaxHorizontalRot;
      float num2;
      float num3;
      float num4;
      float num5;
      if ((UnityEngine.Object) this.Bipod != (UnityEngine.Object) null && this.Bipod.IsBipodActive)
      {
        num2 = armRecoilProfile.VerticalRotPerShot_Bipodded * UnityEngine.Random.Range(0.8f, 1f);
        num3 = armRecoilProfile.HorizontalRotPerShot_Bipodded * UnityEngine.Random.Range(0.8f, 1f);
        num4 = armRecoilProfile.ZLinearPerShot * UnityEngine.Random.Range(0.8f, 1f);
        num5 = 0.0f;
        max1 = armRecoilProfile.MaxVerticalRot_Bipodded;
        max2 = armRecoilProfile.MaxHorizontalRot_Bipodded;
      }
      else if (foregripStabilized)
      {
        num2 = num1 = armRecoilProfile.VerticalRotPerShot * UnityEngine.Random.Range(0.4f, 0.45f);
        num3 = UnityEngine.Random.Range(-armRecoilProfile.HorizontalRotPerShot, armRecoilProfile.HorizontalRotPerShot) * UnityEngine.Random.Range(0.4f, 0.45f);
        num4 = armRecoilProfile.ZLinearPerShot * UnityEngine.Random.Range(0.8f, 0.9f);
        num5 = armRecoilProfile.MassDriftFactors.x;
        max1 *= 0.6f;
        max2 *= 0.9f;
      }
      else if (twoHandStabilized)
      {
        num2 = armRecoilProfile.VerticalRotPerShot * UnityEngine.Random.Range(0.5f, 0.6f);
        num3 = UnityEngine.Random.Range(-armRecoilProfile.HorizontalRotPerShot, armRecoilProfile.HorizontalRotPerShot) * UnityEngine.Random.Range(0.6f, 0.8f);
        num4 = armRecoilProfile.ZLinearPerShot * UnityEngine.Random.Range(0.8f, 0.9f);
        num5 = armRecoilProfile.MassDriftFactors.y;
        max1 *= 0.8f;
        max2 *= 0.95f;
      }
      else
      {
        num2 = armRecoilProfile.VerticalRotPerShot * UnityEngine.Random.Range(0.9f, 1f);
        num3 = UnityEngine.Random.Range(-armRecoilProfile.HorizontalRotPerShot, armRecoilProfile.HorizontalRotPerShot) * UnityEngine.Random.Range(0.9f, 1f);
        num4 = armRecoilProfile.ZLinearPerShot * UnityEngine.Random.Range(0.9f, 1f);
        num5 = armRecoilProfile.MassDriftFactors.z;
      }
      if (shoulderStabilized)
      {
        if (foregripStabilized || twoHandStabilized)
        {
          num2 *= 0.3f;
          num3 = UnityEngine.Random.Range(-armRecoilProfile.HorizontalRotPerShot, armRecoilProfile.HorizontalRotPerShot) * UnityEngine.Random.Range(0.2f, 0.3f);
          num4 *= 0.5f;
        }
        else
        {
          num2 *= 0.75f;
          num4 *= 0.7f;
        }
        max1 *= 0.5f;
        max2 *= 0.6f;
        num5 *= armRecoilProfile.MassDriftFactors.w;
      }
      float reductionFactor = this.GetReductionFactor();
      float num6 = num2 * reductionFactor;
      float f1 = num3 * reductionFactor;
      float f2 = num4 * reductionFactor;
      float num7 = num6 * VerticalRecoilMult;
      this.m_massDriftInstability += num5 * armRecoilProfile.MassDriftIntensity;
      float num8 = this.m_recoilX + num7;
      float num9 = 0.0f;
      if ((double) num8 > (double) max1)
      {
        num9 = (float) (((double) num8 - (double) max1) * 0.25);
        num8 = max1 - UnityEngine.Random.Range(num7 * 0.2f, num7 * 0.8f);
      }
      this.m_recoilX = Mathf.Clamp(num8, -max1, max1);
      this.m_recoilY = Mathf.Clamp((float) ((double) this.m_recoilY + (double) f1 + (double) num9 * (double) Mathf.Sign(f1)), -max2, max2);
      this.m_recoilLinearZ += f2;
      this.m_recoilLinearZ = Mathf.Clamp(this.m_recoilLinearZ, this.m_recoilPoseHolderLocalPosStart.z, this.m_recoilPoseHolderLocalPosStart.z + armRecoilProfile.ZLinearMax);
      Vector2 vector2 = new Vector2((float) ((double) Mathf.Abs(this.m_recoilY) / (double) armRecoilProfile.MaxHorizontalRot * ((double) Mathf.Abs(f2) / (double) armRecoilProfile.ZLinearMax)) * Mathf.Sign(this.m_recoilY) * armRecoilProfile.XYLinearPerShot, (float) (-((double) Mathf.Abs(this.m_recoilX) / (double) armRecoilProfile.MaxVerticalRot) * ((double) Mathf.Abs(f2) / (double) armRecoilProfile.ZLinearMax) * (double) Mathf.Sign(this.m_recoilX) * (double) armRecoilProfile.XYLinearPerShot * 0.100000001490116));
      float xyLinearMax = armRecoilProfile.XYLinearMax;
      if (shoulderStabilized)
        xyLinearMax *= 0.35f;
      this.m_recoilLinearXY += vector2;
      this.m_recoilLinearXY = Vector2.ClampMagnitude(this.m_recoilLinearXY, xyLinearMax);
    }

    public virtual void Fire(
      FVRFireArmChamber chamber,
      Transform muzzle,
      bool doBuzz,
      float velMult = 1f)
    {
      if (doBuzz && (UnityEngine.Object) this.m_hand != (UnityEngine.Object) null)
      {
        this.m_hand.Buzz(this.m_hand.Buzzer.Buzz_GunShot);
        if ((UnityEngine.Object) this.AltGrip != (UnityEngine.Object) null && (UnityEngine.Object) this.AltGrip.m_hand != (UnityEngine.Object) null)
          this.AltGrip.m_hand.Buzz(this.m_hand.Buzzer.Buzz_GunShot);
      }
      GM.CurrentSceneSettings.OnShotFired(this);
      if (this.IsSuppressed())
        GM.CurrentPlayerBody.VisibleEvent(0.1f);
      else
        GM.CurrentPlayerBody.VisibleEvent(2f);
      float chamberVelMult = AM.GetChamberVelMult(chamber.RoundType, Vector3.Distance(chamber.transform.position, muzzle.position));
      float num1 = this.GetCombinedFixedDrop(this.AccuracyClass) * 0.0166667f;
      Vector2 vector2_1 = this.GetCombinedFixedDrift(this.AccuracyClass) * 0.0166667f;
      for (int index = 0; index < chamber.GetRound().NumProjectiles; ++index)
      {
        float num2 = chamber.GetRound().ProjectileSpread + this.m_internalMechanicalMOA + this.GetCombinedMuzzleDeviceAccuracy();
        if ((UnityEngine.Object) chamber.GetRound().BallisticProjectilePrefab != (UnityEngine.Object) null)
        {
          Vector3 vector3 = muzzle.forward * 0.005f;
          GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(chamber.GetRound().BallisticProjectilePrefab, muzzle.position - vector3, muzzle.rotation);
          Vector2 vector2_2 = (UnityEngine.Random.insideUnitCircle + UnityEngine.Random.insideUnitCircle + UnityEngine.Random.insideUnitCircle) * 0.3333333f * num2;
          gameObject.transform.Rotate(new Vector3(vector2_2.x + vector2_1.y + num1, vector2_2.y + vector2_1.x, 0.0f));
          BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
          component.Fire(component.MuzzleVelocityBase * chamber.ChamberVelocityMultiplier * velMult * chamberVelMult, gameObject.transform.forward, this);
        }
      }
    }

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      if (GM.Options.ControlOptions.UseGunRigMode2)
        this.PoseOverride.localRotation = Quaternion.Inverse(hand.m_storedInitialPointingTransformDir);
      else if ((hand.CMode == ControlMode.Oculus || hand.CMode == ControlMode.Index) && (UnityEngine.Object) this.PoseOverride_Touch != (UnityEngine.Object) null)
        this.PoseOverride.localRotation = this.PoseOverride_Touch.localRotation;
      else
        this.PoseOverride.localRotation = this.m_storedLocalPoseOverrideRot;
    }

    public override void EndInteraction(FVRViveHand hand) => base.EndInteraction(hand);

    public override void EndInteractionIntoInventorySlot(FVRViveHand hand, FVRQuickBeltSlot slot) => base.EndInteractionIntoInventorySlot(hand, slot);

    protected override void FVRFixedUpdate()
    {
      base.FVRFixedUpdate();
      if ((UnityEngine.Object) this.RecoilingPoseHolder != (UnityEngine.Object) null && (UnityEngine.Object) this.RecoilProfile != (UnityEngine.Object) null)
      {
        FVRFireArmRecoilProfile recoilProfile = this.GetRecoilProfile();
        bool flag1 = this.IsTwoHandStabilized();
        bool flag2 = this.IsForegripStabilized();
        bool flag3 = this.IsShoulderStabilized();
        this.RecoilingPoseHolder.localEulerAngles = new Vector3(this.m_recoilX, this.m_recoilY, 0.0f);
        this.RecoilingPoseHolder.localPosition = new Vector3(this.m_recoilLinearXYBase.x + this.m_recoilLinearXY.x, this.m_recoilLinearXYBase.y + this.m_recoilLinearXY.y, this.m_recoilLinearZ);
        this.m_massDriftInstability = Mathf.Clamp(this.m_massDriftInstability, 0.0f, recoilProfile.MaxMassDriftMagnitude);
        float num1 = 0.0f;
        float num2 = !flag2 ? (!flag1 ? recoilProfile.MassDriftFactors.z * recoilProfile.MaxMassMaxRotation : recoilProfile.MassDriftFactors.y * recoilProfile.MaxMassMaxRotation) : recoilProfile.MassDriftFactors.x * recoilProfile.MaxMassMaxRotation;
        if (flag3)
          num1 = num2 * recoilProfile.MassDriftFactors.w;
        if ((UnityEngine.Object) this.m_hand != (UnityEngine.Object) null)
        {
          this.m_tarRecoilMassDrift += new Vector2(-this.m_hand.Input.VelAngularLocal.y, -this.m_hand.Input.VelAngularLocal.x) * Time.deltaTime * 10f;
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
        this.m_recoilX = Mathf.Lerp(this.m_recoilX, vector2.y, Time.deltaTime * y * recoilProfile.VerticalRotRecovery * UnityEngine.Random.Range(0.9f, 1.1f));
        this.m_recoilY = Mathf.Lerp(this.m_recoilY, vector2.x, Time.deltaTime * x * recoilProfile.HorizontalRotRecovery * UnityEngine.Random.Range(0.9f, 1.1f));
        this.m_recoilLinearZ = Mathf.Lerp(this.m_recoilLinearZ, this.m_recoilPoseHolderLocalPosStart.z, Time.deltaTime * recoilProfile.ZLinearRecovery * num5 * UnityEngine.Random.Range(0.9f, 1.1f));
        this.m_recoilLinearXY = Vector2.Lerp(this.m_recoilLinearXY, Vector2.zero, Time.deltaTime * recoilProfile.XYLinearRecovery * num4 * UnityEngine.Random.Range(0.9f, 1.1f));
        if ((UnityEngine.Object) this.Bipod != (UnityEngine.Object) null)
          this.Bipod.RecoilFactor = this.m_recoilLinearZ;
      }
      if ((double) this.m_ejectDelay > 0.0)
        this.m_ejectDelay -= Time.deltaTime;
      else
        this.m_ejectDelay = 0.0f;
      if ((double) this.m_clipEjectDelay > 0.0)
        this.m_clipEjectDelay -= Time.deltaTime;
      else
        this.m_clipEjectDelay = 0.0f;
    }

    public virtual void LoadMag(FVRFireArmMagazine mag)
    {
      if (!((UnityEngine.Object) this.Magazine == (UnityEngine.Object) null) || !((UnityEngine.Object) mag != (UnityEngine.Object) null))
        return;
      this.m_lastEjectedMag = (FVRFireArmMagazine) null;
      this.Magazine = mag;
      if ((UnityEngine.Object) this.m_hand != (UnityEngine.Object) null)
      {
        this.m_hand.Buzz(this.m_hand.Buzzer.Buzz_BeginInteraction);
        if ((UnityEngine.Object) this.Magazine.m_hand != (UnityEngine.Object) null)
          this.Magazine.m_hand.Buzz(this.m_hand.Buzzer.Buzz_BeginInteraction);
      }
      if (mag.UsesOverrideInOut)
        this.PlayAudioEventHandling(mag.ProfileOverride.MagazineIn);
      else
        this.PlayAudioEvent(FirearmAudioEventType.MagazineIn);
    }

    public virtual Transform GetMagMountingTransform() => this.transform;

    public virtual void EjectMag()
    {
      if (!((UnityEngine.Object) this.Magazine != (UnityEngine.Object) null))
        return;
      if (this.Magazine.UsesOverrideInOut)
        this.PlayAudioEventHandling(this.Magazine.ProfileOverride.MagazineOut);
      else
        this.PlayAudioEvent(FirearmAudioEventType.MagazineOut);
      this.m_lastEjectedMag = this.Magazine;
      this.m_ejectDelay = 0.4f;
      if ((UnityEngine.Object) this.m_hand != (UnityEngine.Object) null)
        this.m_hand.Buzz(this.m_hand.Buzzer.Buzz_BeginInteraction);
      this.Magazine.Release();
      if ((UnityEngine.Object) this.Magazine.m_hand != (UnityEngine.Object) null)
        this.Magazine.m_hand.Buzz(this.m_hand.Buzzer.Buzz_BeginInteraction);
      this.Magazine = (FVRFireArmMagazine) null;
    }

    public virtual void LoadClip(FVRFireArmClip clip)
    {
      if (!((UnityEngine.Object) this.Clip == (UnityEngine.Object) null) || !((UnityEngine.Object) clip != (UnityEngine.Object) null))
        return;
      this.Clip = clip;
      if (!((UnityEngine.Object) this.m_hand != (UnityEngine.Object) null))
        return;
      this.m_hand.Buzz(this.m_hand.Buzzer.Buzz_BeginInteraction);
      if (!((UnityEngine.Object) this.Clip.m_hand != (UnityEngine.Object) null))
        return;
      this.Clip.m_hand.Buzz(this.m_hand.Buzzer.Buzz_BeginInteraction);
    }

    public virtual void EjectClip()
    {
      if (!((UnityEngine.Object) this.Clip != (UnityEngine.Object) null))
        return;
      this.m_clipEjectDelay = 0.4f;
      if ((UnityEngine.Object) this.m_hand != (UnityEngine.Object) null)
        this.m_hand.Buzz(this.m_hand.Buzzer.Buzz_BeginInteraction);
      this.Clip.Release();
      if ((UnityEngine.Object) this.Clip.m_hand != (UnityEngine.Object) null)
        this.Clip.m_hand.Buzz(this.m_hand.Buzzer.Buzz_BeginInteraction);
      this.Clip = (FVRFireArmClip) null;
    }

    public void PlayShotTail(
      FVRTailSoundClass tailClass,
      FVRSoundEnvironment TailEnvironment,
      float globalLoudnessMultiplier = 1f)
    {
      AudioEvent tailSet = SM.GetTailSet(tailClass, TailEnvironment);
      Vector3 position = this.transform.position;
      this.m_pool_tail.PlayClipVolumePitchOverride(tailSet, position, tailSet.VolumeRange * globalLoudnessMultiplier, this.AudioClipSet.TailPitchMod_Suppressed * tailSet.PitchRange.x);
    }

    public void PlayAudioGunShot(
      FVRFireArmRound round,
      FVRSoundEnvironment TailEnvironment,
      float globalLoudnessMultiplier = 1f)
    {
      Vector3 position = this.transform.position;
      FVRTailSoundClass tailClass = FVRTailSoundClass.Tiny;
      if (this.IsSuppressed())
      {
        this.PlayAudioEvent(FirearmAudioEventType.Shots_Suppressed);
        if (this.AudioClipSet.UsesTail_Suppressed)
        {
          tailClass = round.TailClassSuppressed;
          AudioEvent tailSet = SM.GetTailSet(round.TailClassSuppressed, TailEnvironment);
          this.m_pool_tail.PlayClipVolumePitchOverride(tailSet, position, tailSet.VolumeRange * globalLoudnessMultiplier, this.AudioClipSet.TailPitchMod_Suppressed * tailSet.PitchRange.x);
        }
      }
      else if (this.AudioClipSet.UsesLowPressureSet)
      {
        if (round.IsHighPressure)
        {
          this.PlayAudioEvent(FirearmAudioEventType.Shots_Main);
          if (this.AudioClipSet.UsesTail_Main)
          {
            tailClass = round.TailClass;
            AudioEvent tailSet = SM.GetTailSet(round.TailClass, TailEnvironment);
            this.m_pool_tail.PlayClipVolumePitchOverride(tailSet, position, tailSet.VolumeRange * globalLoudnessMultiplier, this.AudioClipSet.TailPitchMod_Main * tailSet.PitchRange.x);
          }
        }
        else
        {
          this.PlayAudioEvent(FirearmAudioEventType.Shots_LowPressure);
          if (this.AudioClipSet.UsesTail_Main)
          {
            tailClass = round.TailClass;
            AudioEvent tailSet = SM.GetTailSet(round.TailClass, TailEnvironment);
            this.m_pool_tail.PlayClipVolumePitchOverride(tailSet, position, tailSet.VolumeRange * globalLoudnessMultiplier, this.AudioClipSet.TailPitchMod_LowPressure * tailSet.PitchRange.x);
          }
        }
      }
      else
      {
        this.PlayAudioEvent(FirearmAudioEventType.Shots_Main);
        if (this.AudioClipSet.UsesTail_Main)
        {
          tailClass = round.TailClass;
          AudioEvent tailSet = SM.GetTailSet(round.TailClass, TailEnvironment);
          this.m_pool_tail.PlayClipVolumePitchOverride(tailSet, position, tailSet.VolumeRange * globalLoudnessMultiplier, this.AudioClipSet.TailPitchMod_Main * tailSet.PitchRange.x);
        }
      }
      float multByEnvironment = SM.GetSoundTravelDistanceMultByEnvironment(TailEnvironment);
      int playerIff = GM.CurrentPlayerBody.GetPlayerIFF();
      if (this.IsSuppressed())
        GM.CurrentSceneSettings.OnPerceiveableSound(this.AudioClipSet.Loudness_Suppressed, (float) ((double) this.AudioClipSet.Loudness_Suppressed * (double) multByEnvironment * 0.5) * globalLoudnessMultiplier, this.transform.position, playerIff);
      else if (this.AudioClipSet.UsesLowPressureSet && !round.IsHighPressure)
        GM.CurrentSceneSettings.OnPerceiveableSound(this.AudioClipSet.Loudness_Primary * 0.6f, this.AudioClipSet.Loudness_Primary * 0.6f * multByEnvironment * globalLoudnessMultiplier, this.transform.position, playerIff);
      else
        GM.CurrentSceneSettings.OnPerceiveableSound(this.AudioClipSet.Loudness_Primary, this.AudioClipSet.Loudness_Primary * multByEnvironment * globalLoudnessMultiplier, this.transform.position, playerIff);
      if (!this.IsSuppressed())
        this.SceneSettings.PingReceivers(this.MuzzlePos.position);
      this.RattleSuppresor();
      for (int index = 0; index < this.MuzzleDevices.Count; ++index)
        this.MuzzleDevices[index].OnShot(this, tailClass);
    }

    public void PlayAudioGunShot(
      bool IsHighPressure,
      FVRTailSoundClass TailClass,
      FVRTailSoundClass TailClassSuppressed,
      FVRSoundEnvironment TailEnvironment)
    {
      Vector3 position = this.transform.position;
      FVRTailSoundClass tailClass = FVRTailSoundClass.Tiny;
      if (this.IsSuppressed())
      {
        this.PlayAudioEvent(FirearmAudioEventType.Shots_Suppressed);
        if (this.AudioClipSet.UsesTail_Suppressed)
        {
          tailClass = TailClassSuppressed;
          AudioEvent tailSet = SM.GetTailSet(TailClassSuppressed, TailEnvironment);
          this.m_pool_tail.PlayClipPitchOverride(tailSet, position, this.AudioClipSet.TailPitchMod_Suppressed * tailSet.PitchRange.x);
        }
      }
      else
      {
        float pitchmod = 1f;
        if (this.IsBraked())
          pitchmod = 0.92f;
        if (IsHighPressure)
        {
          this.PlayAudioEvent(FirearmAudioEventType.Shots_Main, pitchmod);
          if (this.AudioClipSet.UsesTail_Main)
          {
            tailClass = TailClass;
            AudioEvent tailSet = SM.GetTailSet(TailClass, TailEnvironment);
            this.m_pool_tail.PlayClipPitchOverride(tailSet, position, this.AudioClipSet.TailPitchMod_Main * tailSet.PitchRange.x * pitchmod);
          }
        }
        else
        {
          this.PlayAudioEvent(FirearmAudioEventType.Shots_LowPressure, pitchmod);
          if (this.AudioClipSet.UsesTail_Main)
          {
            tailClass = TailClass;
            AudioEvent tailSet = SM.GetTailSet(TailClass, TailEnvironment);
            this.m_pool_tail.PlayClipPitchOverride(tailSet, position, this.AudioClipSet.TailPitchMod_LowPressure * tailSet.PitchRange.x * pitchmod);
          }
        }
      }
      float multByEnvironment = SM.GetSoundTravelDistanceMultByEnvironment(TailEnvironment);
      int playerIff = GM.CurrentPlayerBody.GetPlayerIFF();
      if (this.IsSuppressed())
        GM.CurrentSceneSettings.OnPerceiveableSound(this.AudioClipSet.Loudness_Suppressed, (float) ((double) this.AudioClipSet.Loudness_Suppressed * (double) multByEnvironment * 0.400000005960464), this.transform.position, playerIff);
      else if (this.AudioClipSet.UsesLowPressureSet && !IsHighPressure)
        GM.CurrentSceneSettings.OnPerceiveableSound(this.AudioClipSet.Loudness_Primary * 0.6f, this.AudioClipSet.Loudness_Primary * 0.6f * multByEnvironment, this.transform.position, playerIff);
      else
        GM.CurrentSceneSettings.OnPerceiveableSound(this.AudioClipSet.Loudness_Primary, this.AudioClipSet.Loudness_Primary * multByEnvironment, this.transform.position, playerIff);
      if (!this.IsSuppressed())
        this.SceneSettings.PingReceivers(this.MuzzlePos.position);
      this.RattleSuppresor();
      for (int index = 0; index < this.MuzzleDevices.Count; ++index)
        this.MuzzleDevices[index].OnShot(this, tailClass);
    }

    public void PlayAudioEventHandling(AudioEvent e)
    {
      Vector3 position = this.transform.position;
      this.m_pool_handling.PlayClip(e, position);
    }

    public FVRPooledAudioSource PlayAudioAsHandling(AudioEvent ev, Vector3 pos)
    {
      float num = 1f;
      if (this.m_fireWeaponHandlingPerceivableSounds)
        num = SM.GetSoundTravelDistanceMultByEnvironment(GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
      int playerIff = GM.CurrentPlayerBody.GetPlayerIFF();
      if (this.m_fireWeaponHandlingPerceivableSounds)
        GM.CurrentSceneSettings.OnPerceiveableSound(10f * this.AudioClipSet.Loudness_OperationMult, (float) (10.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
      return this.m_pool_handling.PlayClip(ev, pos);
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
          if (!this.IsHeld)
            break;
          this.m_hand.ForceTubeRumble(this.AudioClipSet.FTP.Rumble_Handling_Intensity, this.AudioClipSet.FTP.Rumble_Handling_Duration);
          break;
        case FirearmAudioEventType.HandleGrab:
          this.m_pool_handling.PlayClip(this.AudioClipSet.HandleGrab, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(10f * this.AudioClipSet.Loudness_OperationMult, (float) (10.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.HandleBack:
          this.m_pool_handling.PlayClip(this.AudioClipSet.HandleBack, position);
          if (this.m_fireWeaponHandlingPerceivableSounds)
            GM.CurrentSceneSettings.OnPerceiveableSound(20f * this.AudioClipSet.Loudness_OperationMult, (float) (20.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          if (!this.IsHeld)
            break;
          this.m_hand.ForceTubeRumble(this.AudioClipSet.FTP.Rumble_Handling_Intensity, this.AudioClipSet.FTP.Rumble_Handling_Duration);
          break;
        case FirearmAudioEventType.HandleForward:
          this.m_pool_handling.PlayClip(this.AudioClipSet.HandleForward, position);
          if (this.m_fireWeaponHandlingPerceivableSounds)
            GM.CurrentSceneSettings.OnPerceiveableSound(20f * this.AudioClipSet.Loudness_OperationMult, (float) (20.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          if (!this.IsHeld)
            break;
          this.m_hand.ForceTubeRumble(this.AudioClipSet.FTP.Rumble_Handling_Intensity, this.AudioClipSet.FTP.Rumble_Handling_Duration);
          break;
        case FirearmAudioEventType.HandleUp:
          this.m_pool_handling.PlayClip(this.AudioClipSet.HandleUp, position);
          if (this.m_fireWeaponHandlingPerceivableSounds)
            GM.CurrentSceneSettings.OnPerceiveableSound(10f * this.AudioClipSet.Loudness_OperationMult, (float) (10.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          if (!this.IsHeld)
            break;
          this.m_hand.ForceTubeRumble(this.AudioClipSet.FTP.Rumble_Handling_Intensity, this.AudioClipSet.FTP.Rumble_Handling_Duration);
          break;
        case FirearmAudioEventType.HandleDown:
          this.m_pool_handling.PlayClip(this.AudioClipSet.HandleDown, position);
          if (this.m_fireWeaponHandlingPerceivableSounds)
            GM.CurrentSceneSettings.OnPerceiveableSound(20f * this.AudioClipSet.Loudness_OperationMult, (float) (20.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          if (!this.IsHeld)
            break;
          this.m_hand.ForceTubeRumble(this.AudioClipSet.FTP.Rumble_Handling_Intensity, this.AudioClipSet.FTP.Rumble_Handling_Duration);
          break;
        case FirearmAudioEventType.Safety:
          this.m_pool_handling.PlayClip(this.AudioClipSet.Safety, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(8f * this.AudioClipSet.Loudness_OperationMult, (float) (8.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.FireSelector:
          this.m_pool_handling.PlayClip(this.AudioClipSet.FireSelector, position);
          if (this.m_fireWeaponHandlingPerceivableSounds)
            GM.CurrentSceneSettings.OnPerceiveableSound(8f * this.AudioClipSet.Loudness_OperationMult, (float) (8.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          if (!this.IsHeld)
            break;
          this.m_hand.ForceTubeRumble(this.AudioClipSet.FTP.Rumble_Handling_Intensity, this.AudioClipSet.FTP.Rumble_Handling_Duration);
          break;
        case FirearmAudioEventType.TriggerReset:
          this.m_pool_handling.PlayClip(this.AudioClipSet.TriggerReset, position);
          if (!this.m_fireWeaponHandlingPerceivableSounds)
            break;
          GM.CurrentSceneSettings.OnPerceiveableSound(3f * this.AudioClipSet.Loudness_OperationMult, (float) (3.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          break;
        case FirearmAudioEventType.BreachOpen:
          this.m_pool_handling.PlayClip(this.AudioClipSet.BreachOpen, position);
          if (this.m_fireWeaponHandlingPerceivableSounds)
            GM.CurrentSceneSettings.OnPerceiveableSound(20f * this.AudioClipSet.Loudness_OperationMult, (float) (20.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          if (!this.IsHeld)
            break;
          this.m_hand.ForceTubeRumble(this.AudioClipSet.FTP.Rumble_Handling_Intensity, this.AudioClipSet.FTP.Rumble_Handling_Duration);
          break;
        case FirearmAudioEventType.BreachClose:
          this.m_pool_handling.PlayClip(this.AudioClipSet.BreachClose, position);
          if (this.m_fireWeaponHandlingPerceivableSounds)
            GM.CurrentSceneSettings.OnPerceiveableSound(20f * this.AudioClipSet.Loudness_OperationMult, (float) (20.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          if (!this.IsHeld)
            break;
          this.m_hand.ForceTubeRumble(this.AudioClipSet.FTP.Rumble_Handling_Intensity, this.AudioClipSet.FTP.Rumble_Handling_Duration);
          break;
        case FirearmAudioEventType.MagazineIn:
          this.m_pool_handling.PlayClip(this.AudioClipSet.MagazineIn, position);
          if (this.m_fireWeaponHandlingPerceivableSounds)
            GM.CurrentSceneSettings.OnPerceiveableSound(20f * this.AudioClipSet.Loudness_OperationMult, (float) (20.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          if (!this.IsHeld)
            break;
          this.m_hand.ForceTubeRumble(this.AudioClipSet.FTP.Rumble_Handling_Intensity, this.AudioClipSet.FTP.Rumble_Handling_Duration);
          break;
        case FirearmAudioEventType.MagazineOut:
          this.m_pool_handling.PlayClip(this.AudioClipSet.MagazineOut, position);
          if (this.m_fireWeaponHandlingPerceivableSounds)
            GM.CurrentSceneSettings.OnPerceiveableSound(10f * this.AudioClipSet.Loudness_OperationMult, (float) (10.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          if (!this.IsHeld)
            break;
          this.m_hand.ForceTubeRumble(this.AudioClipSet.FTP.Rumble_Handling_Intensity, this.AudioClipSet.FTP.Rumble_Handling_Duration);
          break;
        case FirearmAudioEventType.TopCoverRelease:
          this.m_pool_handling.PlayClip(this.AudioClipSet.TopCoverRelease, position);
          break;
        case FirearmAudioEventType.TopCoverUp:
          this.m_pool_handling.PlayClip(this.AudioClipSet.TopCoverUp, position);
          if (this.m_fireWeaponHandlingPerceivableSounds)
            GM.CurrentSceneSettings.OnPerceiveableSound(10f * this.AudioClipSet.Loudness_OperationMult, (float) (10.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          if (!this.IsHeld)
            break;
          this.m_hand.ForceTubeRumble(this.AudioClipSet.FTP.Rumble_Handling_Intensity, this.AudioClipSet.FTP.Rumble_Handling_Duration);
          break;
        case FirearmAudioEventType.TopCoverDown:
          this.m_pool_handling.PlayClip(this.AudioClipSet.TopCoverDown, position);
          if (this.m_fireWeaponHandlingPerceivableSounds)
            GM.CurrentSceneSettings.OnPerceiveableSound(10f * this.AudioClipSet.Loudness_OperationMult, (float) (10.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          if (!this.IsHeld)
            break;
          this.m_hand.ForceTubeRumble(this.AudioClipSet.FTP.Rumble_Handling_Intensity, this.AudioClipSet.FTP.Rumble_Handling_Duration);
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
          if (this.m_fireWeaponHandlingPerceivableSounds)
            GM.CurrentSceneSettings.OnPerceiveableSound(10f * this.AudioClipSet.Loudness_OperationMult, (float) (10.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          if (!this.IsHeld)
            break;
          this.m_hand.ForceTubeRumble(this.AudioClipSet.FTP.Rumble_Handling_Intensity, this.AudioClipSet.FTP.Rumble_Handling_Duration);
          break;
        case FirearmAudioEventType.StockClosed:
          this.m_pool_handling.PlayClip(this.AudioClipSet.StockClosed, position);
          if (this.m_fireWeaponHandlingPerceivableSounds)
            GM.CurrentSceneSettings.OnPerceiveableSound(10f * this.AudioClipSet.Loudness_OperationMult, (float) (10.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          if (!this.IsHeld)
            break;
          this.m_hand.ForceTubeRumble(this.AudioClipSet.FTP.Rumble_Handling_Intensity, this.AudioClipSet.FTP.Rumble_Handling_Duration);
          break;
        case FirearmAudioEventType.BipodOpen:
          this.m_pool_handling.PlayClip(this.AudioClipSet.BipodOpen, position);
          if (this.m_fireWeaponHandlingPerceivableSounds)
            GM.CurrentSceneSettings.OnPerceiveableSound(10f * this.AudioClipSet.Loudness_OperationMult, (float) (10.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          if (!this.IsHeld)
            break;
          this.m_hand.ForceTubeRumble(this.AudioClipSet.FTP.Rumble_Handling_Intensity, this.AudioClipSet.FTP.Rumble_Handling_Duration);
          break;
        case FirearmAudioEventType.BipodClosed:
          this.m_pool_handling.PlayClip(this.AudioClipSet.BipodClosed, position);
          if (this.m_fireWeaponHandlingPerceivableSounds)
            GM.CurrentSceneSettings.OnPerceiveableSound(10f * this.AudioClipSet.Loudness_OperationMult, (float) (10.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          if (!this.IsHeld)
            break;
          this.m_hand.ForceTubeRumble(this.AudioClipSet.FTP.Rumble_Handling_Intensity, this.AudioClipSet.FTP.Rumble_Handling_Duration);
          break;
        case FirearmAudioEventType.HandleForwardEmpty:
          this.m_pool_handling.PlayClip(this.AudioClipSet.HandleForwardEmpty, position);
          if (this.m_fireWeaponHandlingPerceivableSounds)
            GM.CurrentSceneSettings.OnPerceiveableSound(20f * this.AudioClipSet.Loudness_OperationMult, (float) (20.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          if (!this.IsHeld)
            break;
          this.m_hand.ForceTubeRumble(this.AudioClipSet.FTP.Rumble_Handling_Intensity, this.AudioClipSet.FTP.Rumble_Handling_Duration);
          break;
        case FirearmAudioEventType.HandleBackEmpty:
          this.m_pool_handling.PlayClip(this.AudioClipSet.HandleBackEmpty, position);
          if (this.m_fireWeaponHandlingPerceivableSounds)
            GM.CurrentSceneSettings.OnPerceiveableSound(20f * this.AudioClipSet.Loudness_OperationMult, (float) (20.0 * (double) this.AudioClipSet.Loudness_OperationMult * (double) num * 0.5), this.transform.position, playerIff);
          if (!this.IsHeld)
            break;
          this.m_hand.ForceTubeRumble(this.AudioClipSet.FTP.Rumble_Handling_Intensity, this.AudioClipSet.FTP.Rumble_Handling_Duration);
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
        case FirearmAudioEventType.BeltSettle:
          this.m_pool_belt.PlayClip(this.AudioClipSet.BeltSettle, position);
          break;
        default:
          switch (eType - 100)
          {
            case FirearmAudioEventType.BoltSlideForward:
              this.m_pool_shot.PlayClipPitchOverride(this.AudioClipSet.Shots_Main, this.GetMuzzle().position, new Vector2(this.AudioClipSet.Shots_Main.PitchRange.x * pitchmod, this.AudioClipSet.Shots_Main.PitchRange.y * pitchmod));
              if (!this.IsHeld)
                return;
              this.m_hand.ForceTubeKick(this.AudioClipSet.FTP.Kick_Shot);
              this.m_hand.ForceTubeRumble(this.AudioClipSet.FTP.Rumble_Shot_Intensity, this.AudioClipSet.FTP.Rumble_Shot_Duration);
              return;
            case FirearmAudioEventType.BoltSlideBack:
              this.m_pool_shot.PlayClipPitchOverride(this.AudioClipSet.Shots_Suppressed, this.GetMuzzle().position, new Vector2(this.AudioClipSet.Shots_Suppressed.PitchRange.x * pitchmod, this.AudioClipSet.Shots_Suppressed.PitchRange.y * pitchmod));
              if (!this.IsHeld)
                return;
              this.m_hand.ForceTubeKick(this.AudioClipSet.FTP.Kick_Shot);
              this.m_hand.ForceTubeRumble(this.AudioClipSet.FTP.Rumble_Shot_Intensity, this.AudioClipSet.FTP.Rumble_Shot_Duration);
              return;
            case FirearmAudioEventType.BoltSlideForwardHeld:
              this.m_pool_shot.PlayClipPitchOverride(this.AudioClipSet.Shots_LowPressure, this.GetMuzzle().position, new Vector2(this.AudioClipSet.Shots_LowPressure.PitchRange.x * pitchmod, this.AudioClipSet.Shots_LowPressure.PitchRange.y * pitchmod));
              if (!this.IsHeld)
                return;
              this.m_hand.ForceTubeKick(this.AudioClipSet.FTP.Kick_Shot);
              this.m_hand.ForceTubeRumble(this.AudioClipSet.FTP.Rumble_Shot_Intensity, this.AudioClipSet.FTP.Rumble_Shot_Duration);
              return;
            default:
              return;
          }
      }
    }

    public virtual List<FireArmRoundClass> GetChamberRoundList() => (List<FireArmRoundClass>) null;

    public virtual List<string> GetFlagList() => (List<string>) null;

    public virtual void SetLoadedChambers(List<FireArmRoundClass> rounds)
    {
    }

    public virtual void SetFromFlagList(List<string> flags)
    {
    }

    public override Dictionary<string, string> GetFlagDic() => new Dictionary<string, string>();

    [ContextMenu("ConfigureForMelee")]
    public void ConfigureForMelee()
    {
      this.MP.IsMeleeWeapon = true;
      if ((UnityEngine.Object) this.StockPos != (UnityEngine.Object) null)
        this.MP.HandPoint = this.StockPos;
      else
        this.MP.HandPoint = this.transform;
      this.MP.EndPoint = this.MuzzlePos;
      this.MP.BaseDamageBCP = new Vector3(100f, 0.0f, 0.0f);
    }

    public enum MuzzleState
    {
      None,
      Suppressor,
      MuzzleBreak,
    }

    [Serializable]
    public class GasOutEffect
    {
      public GameObject EffectPrefab;
      public ParticleSystem PSystem;
      public Transform Parent;
      public bool FollowsMuzzle;
      public float MaxGasRate;
      [Tooltip("x = unsuppressed, y = suppresed")]
      public Vector2 GasPerEvent = Vector2.zero;
      public float GasDownRate = 0.5f;
      private float m_currentGasRate;
      private float m_timeSinceLastEvent;

      public void GasUpdate(bool BreachOpen)
      {
        if ((double) this.m_timeSinceLastEvent < 1.0)
          this.m_timeSinceLastEvent += Time.deltaTime;
        if (this.FollowsMuzzle)
          this.m_currentGasRate -= this.GasDownRate * Time.deltaTime;
        else if (BreachOpen)
          this.m_currentGasRate -= this.GasDownRate * Time.deltaTime;
        else
          this.m_currentGasRate -= (float) ((double) this.GasDownRate * (double) Time.deltaTime * 0.0199999995529652);
        this.m_currentGasRate = Mathf.Clamp(this.m_currentGasRate, 0.0f, this.MaxGasRate);
        ParticleSystem.EmissionModule emission = this.PSystem.emission;
        ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
        rateOverTime.mode = ParticleSystemCurveMode.Constant;
        rateOverTime.constantMax = this.m_currentGasRate;
        rateOverTime.constantMin = this.m_currentGasRate;
        if (this.FollowsMuzzle)
        {
          if ((double) this.m_timeSinceLastEvent < 0.25)
          {
            rateOverTime.constantMax = 0.0f;
            rateOverTime.constantMin = 0.0f;
          }
          else
          {
            rateOverTime.constantMax = this.m_currentGasRate;
            rateOverTime.constantMin = this.m_currentGasRate;
          }
        }
        else if (BreachOpen)
        {
          rateOverTime.constantMax = this.m_currentGasRate;
          rateOverTime.constantMin = this.m_currentGasRate;
          emission.enabled = true;
        }
        else
        {
          rateOverTime.constantMax = 0.0f;
          rateOverTime.constantMin = 0.0f;
          emission.enabled = false;
        }
        emission.rateOverTime = rateOverTime;
      }

      public void AddGas(bool isSuppressed)
      {
        this.m_timeSinceLastEvent = 0.0f;
        if (isSuppressed)
          this.m_currentGasRate += this.GasPerEvent.y;
        else
          this.m_currentGasRate += this.GasPerEvent.x;
        this.m_currentGasRate = Mathf.Clamp(this.m_currentGasRate, 0.0f, this.MaxGasRate);
      }
    }
  }
}
