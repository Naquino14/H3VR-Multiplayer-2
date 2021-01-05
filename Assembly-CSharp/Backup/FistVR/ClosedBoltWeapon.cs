// Decompiled with JetBrains decompiler
// Type: FistVR.ClosedBoltWeapon
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class ClosedBoltWeapon : FVRFireArm
  {
    [Header("ClosedBoltWeapon Config")]
    public bool HasFireSelectorButton = true;
    public bool HasMagReleaseButton = true;
    public bool HasBoltReleaseButton = true;
    public bool HasBoltCatchButton = true;
    public bool HasHandle = true;
    [Header("Component Connections")]
    public ClosedBolt Bolt;
    public ClosedBoltHandle Handle;
    public FVRFireArmChamber Chamber;
    public Transform Trigger;
    public Transform FireSelectorSwitch;
    public Transform FireSelectorSwitch2;
    [Header("Round Positions")]
    public Transform RoundPos_Ejecting;
    public Transform RoundPos_Ejection;
    public Transform RoundPos_MagazinePos;
    private FVRFirearmMovingProxyRound m_proxy;
    public Vector3 EjectionSpeed = new Vector3(4f, 2.5f, -1.2f);
    public Vector3 EjectionSpin = new Vector3(20f, 180f, 30f);
    public bool UsesDelinker;
    public ParticleSystem DelinkerSystem;
    [Header("Trigger Config")]
    public bool HasTrigger;
    public float TriggerFiringThreshold = 0.8f;
    public float TriggerResetThreshold = 0.4f;
    public float TriggerDualStageThreshold = 0.95f;
    public float Trigger_ForwardValue;
    public float Trigger_RearwardValue;
    public FVRPhysicalObject.Axis TriggerAxis;
    public FVRPhysicalObject.InterpStyle TriggerInterpStyle = FVRPhysicalObject.InterpStyle.Rotation;
    public bool UsesDualStageFullAuto;
    private float m_triggerFloat;
    private bool m_hasTriggerReset;
    private int m_CamBurst;
    private bool m_isHammerCocked;
    private int m_fireSelectorMode;
    [Header("Fire Selector Config")]
    public FVRPhysicalObject.InterpStyle FireSelector_InterpStyle = FVRPhysicalObject.InterpStyle.Rotation;
    public FVRPhysicalObject.Axis FireSelector_Axis;
    public ClosedBoltWeapon.FireSelectorMode[] FireSelector_Modes;
    [Header("Secondary Fire Selector Config")]
    public FVRPhysicalObject.InterpStyle FireSelector_InterpStyle2 = FVRPhysicalObject.InterpStyle.Rotation;
    public FVRPhysicalObject.Axis FireSelector_Axis2;
    public ClosedBoltWeapon.FireSelectorMode[] FireSelector_Modes2;
    [Header("SpecialFeatures")]
    public bool EjectsMagazineOnEmpty;
    public bool BoltLocksWhenNoMagazineFound;
    public bool DoesClipEntryRequireBoltBack = true;
    public bool UsesMagMountTransformOverride;
    public Transform MagMountTransformOverride;
    public bool ReciprocatesOnShot = true;
    [Header("StickySystem")]
    public bool UsesStickyDetonation;
    public AudioEvent StickyDetonateSound;
    public Transform StickyTrigger;
    public Vector2 StickyRotRange = new Vector2(0.0f, 20f);
    private float m_stickyChargeUp;
    public float StickyChargeUpSpeed = 1f;
    public AudioSource m_chargeSound;
    public Renderer StickyScreen;
    public float StickyMaxMultBonus = 1.3f;
    public float StickyForceMult = 1f;
    [HideInInspector]
    public bool IsMagReleaseButtonHeld;
    [HideInInspector]
    public bool IsBoltReleaseButtonHeld;
    [HideInInspector]
    public bool IsBoltCatchButtonHeld;
    private float m_timeSinceFiredShot = 1f;
    private bool m_hasStickTriggerDown;
    private List<MF2_StickyBomb> m_primedBombs = new List<MF2_StickyBomb>();

    public bool HasExtractedRound() => this.m_proxy.IsFull;

    public bool IsHammerCocked => this.m_isHammerCocked;

    public int FireSelectorModeIndex => this.m_fireSelectorMode;

    protected override void Awake()
    {
      base.Awake();
      this.m_CamBurst = 1;
      this.m_proxy = new GameObject("m_proxyRound").AddComponent<FVRFirearmMovingProxyRound>();
      this.m_proxy.Init(this.transform);
    }

    public override int GetTutorialState()
    {
      if ((UnityEngine.Object) this.Magazine == (UnityEngine.Object) null)
        return 0;
      if ((UnityEngine.Object) this.Magazine != (UnityEngine.Object) null && !this.Magazine.HasARound())
        return 5;
      if (this.FireSelector_Modes[this.m_fireSelectorMode].ModeType == ClosedBoltWeapon.FireSelectorModeType.Safe)
        return 1;
      if (!this.Chamber.IsFull & (double) this.m_timeSinceFiredShot > 0.400000005960464)
        return 2;
      return (UnityEngine.Object) this.AltGrip == (UnityEngine.Object) null ? 3 : 4;
    }

    public void SecondaryFireSelectorClicked() => this.PlayAudioEvent(FirearmAudioEventType.FireSelector);

    public void CockHammer()
    {
      if (this.m_isHammerCocked)
        return;
      this.m_isHammerCocked = true;
      this.PlayAudioEvent(FirearmAudioEventType.Prefire);
    }

    public void DropHammer()
    {
      if (!this.m_isHammerCocked)
        return;
      this.m_isHammerCocked = false;
      this.PlayAudioEvent(FirearmAudioEventType.HammerHit);
      this.Fire();
    }

    public bool IsWeaponOnSafe() => this.FireSelector_Modes.Length != 0 && this.FireSelector_Modes[this.m_fireSelectorMode].ModeType == ClosedBoltWeapon.FireSelectorModeType.Safe;

    public void ResetCamBurst() => this.m_CamBurst = this.FireSelector_Modes[this.m_fireSelectorMode].BurstAmount;

    protected virtual void ToggleFireSelector()
    {
      if (this.FireSelector_Modes.Length <= 1)
        return;
      if (this.Bolt.UsesAKSafetyLock && !this.Bolt.IsBoltForwardOfSafetyLock())
      {
        int index = this.m_fireSelectorMode + 1;
        if (index >= this.FireSelector_Modes.Length)
          index -= this.FireSelector_Modes.Length;
        if (this.FireSelector_Modes[index].ModeType == ClosedBoltWeapon.FireSelectorModeType.Safe)
          return;
      }
      ++this.m_fireSelectorMode;
      if (this.m_fireSelectorMode >= this.FireSelector_Modes.Length)
        this.m_fireSelectorMode -= this.FireSelector_Modes.Length;
      ClosedBoltWeapon.FireSelectorMode fireSelectorMode = this.FireSelector_Modes[this.m_fireSelectorMode];
      if ((double) this.m_triggerFloat < 0.100000001490116)
        this.m_CamBurst = fireSelectorMode.BurstAmount;
      this.PlayAudioEvent(FirearmAudioEventType.FireSelector);
      if ((UnityEngine.Object) this.FireSelectorSwitch != (UnityEngine.Object) null)
        this.SetAnimatedComponent(this.FireSelectorSwitch, fireSelectorMode.SelectorPosition, this.FireSelector_InterpStyle, this.FireSelector_Axis);
      if (!((UnityEngine.Object) this.FireSelectorSwitch2 != (UnityEngine.Object) null))
        return;
      this.SetAnimatedComponent(this.FireSelectorSwitch2, this.FireSelector_Modes2[this.m_fireSelectorMode].SelectorPosition, this.FireSelector_InterpStyle2, this.FireSelector_Axis2);
    }

    public void EjectExtractedRound()
    {
      if (!this.Chamber.IsFull)
        return;
      this.Chamber.EjectRound(this.RoundPos_Ejection.position, this.transform.right * this.EjectionSpeed.x + this.transform.up * this.EjectionSpeed.y + this.transform.forward * this.EjectionSpeed.z, this.transform.right * this.EjectionSpin.x + this.transform.up * this.EjectionSpin.y + this.transform.forward * this.EjectionSpin.z);
    }

    public void BeginChamberingRound()
    {
      bool flag = false;
      GameObject go = (GameObject) null;
      if (this.HasBelt)
      {
        if (!this.m_proxy.IsFull && this.BeltDD.HasARound())
        {
          flag = true;
          go = this.BeltDD.RemoveRound(false);
        }
      }
      else if (!this.m_proxy.IsFull && (UnityEngine.Object) this.Magazine != (UnityEngine.Object) null && (!this.Magazine.IsBeltBox && this.Magazine.HasARound()))
      {
        flag = true;
        go = this.Magazine.RemoveRound(false);
      }
      if (!flag || !flag)
        return;
      this.m_proxy.SetFromPrefabReference(go);
    }

    public bool ChamberRound()
    {
      if (!this.m_proxy.IsFull || this.Chamber.IsFull)
        return false;
      this.Chamber.SetRound(this.m_proxy.Round);
      this.m_proxy.ClearProxy();
      return true;
    }

    public override Transform GetMagMountingTransform() => this.UsesMagMountTransformOverride ? this.MagMountTransformOverride : base.GetMagMountingTransform();

    protected override void FVRFixedUpdate()
    {
      base.FVRFixedUpdate();
      if (!this.UsesStickyDetonation || (double) this.m_stickyChargeUp <= 0.0)
        return;
      this.RootRigidbody.velocity += UnityEngine.Random.onUnitSphere * 0.2f * this.m_stickyChargeUp * this.StickyForceMult;
      this.RootRigidbody.angularVelocity += UnityEngine.Random.onUnitSphere * 1f * this.m_stickyChargeUp * this.StickyForceMult;
    }

    public bool Fire()
    {
      if (!this.Chamber.Fire())
        return false;
      this.m_timeSinceFiredShot = 0.0f;
      float velMult = 1f;
      if (this.UsesStickyDetonation)
        velMult = 1f + Mathf.Lerp(0.0f, this.StickyMaxMultBonus, this.m_stickyChargeUp);
      this.Fire(this.Chamber, this.GetMuzzle(), true, velMult);
      bool twoHandStabilized = this.IsTwoHandStabilized();
      bool foregripStabilized = (UnityEngine.Object) this.AltGrip != (UnityEngine.Object) null;
      bool shoulderStabilized = this.IsShoulderStabilized();
      this.Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
      bool flag = false;
      ClosedBoltWeapon.FireSelectorMode fireSelectorMode = this.FireSelector_Modes[this.m_fireSelectorMode];
      if (fireSelectorMode.ModeType == ClosedBoltWeapon.FireSelectorModeType.SuperFastBurst)
      {
        for (int index = 0; index < fireSelectorMode.BurstAmount - 1; ++index)
        {
          if (this.Magazine.HasARound())
          {
            this.Magazine.RemoveRound();
            this.Fire(this.Chamber, this.GetMuzzle(), false);
            flag = true;
            this.Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
          }
        }
      }
      this.FireMuzzleSmoke();
      if (this.UsesDelinker && this.HasBelt)
        this.DelinkerSystem.Emit(1);
      if (this.HasBelt)
        this.BeltDD.AddJitter();
      if (flag)
        this.PlayAudioGunShot(false, this.Chamber.GetRound().TailClass, this.Chamber.GetRound().TailClassSuppressed, GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
      else
        this.PlayAudioGunShot(this.Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
      if (this.ReciprocatesOnShot)
        this.Bolt.ImpartFiringImpulse();
      return true;
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      this.UpdateComponents();
      if (this.HasHandle)
      {
        this.Handle.UpdateHandle();
        this.Bolt.UpdateHandleHeldState(this.Handle.ShouldControlBolt(), 1f - this.Handle.GetBoltLerpBetweenLockAndFore());
      }
      this.Bolt.UpdateBolt();
      if (this.UsesClips && this.DoesClipEntryRequireBoltBack)
      {
        if (this.Bolt.CurPos >= ClosedBolt.BoltPos.Locked)
        {
          if (!this.ClipTrigger.activeSelf)
            this.ClipTrigger.SetActive(true);
        }
        else if (this.ClipTrigger.activeSelf)
          this.ClipTrigger.SetActive(false);
      }
      this.UpdateDisplayRoundPositions();
      if ((double) this.m_timeSinceFiredShot >= 1.0)
        return;
      this.m_timeSinceFiredShot += Time.deltaTime;
    }

    public override void LoadMag(FVRFireArmMagazine mag)
    {
      base.LoadMag(mag);
      if (!this.BoltLocksWhenNoMagazineFound || !((UnityEngine.Object) mag != (UnityEngine.Object) null) || !this.Bolt.IsBoltLocked())
        return;
      this.Bolt.ReleaseBolt();
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      this.UpdateInputAndAnimate(hand);
    }

    private void UpdateInputAndAnimate(FVRViveHand hand)
    {
      this.IsBoltReleaseButtonHeld = false;
      this.IsBoltCatchButtonHeld = false;
      if (this.IsAltHeld)
        return;
      this.m_triggerFloat = !this.m_hasTriggeredUpSinceBegin ? 0.0f : hand.Input.TriggerFloat;
      if (!this.m_hasTriggerReset && (double) this.m_triggerFloat <= (double) this.TriggerResetThreshold)
      {
        this.m_hasTriggerReset = true;
        if (this.FireSelector_Modes.Length > 0)
          this.m_CamBurst = this.FireSelector_Modes[this.m_fireSelectorMode].BurstAmount;
        this.PlayAudioEvent(FirearmAudioEventType.TriggerReset);
      }
      if (hand.IsInStreamlinedMode)
      {
        if (hand.Input.BYButtonDown)
        {
          this.ToggleFireSelector();
          if (this.UsesStickyDetonation)
            this.Detonate();
        }
        if (hand.Input.AXButtonDown && this.HasMagReleaseButton && (!this.EjectsMagazineOnEmpty || this.Bolt.CurPos >= ClosedBolt.BoltPos.Locked && this.Bolt.IsHeld && !this.m_proxy.IsFull))
          this.ReleaseMag();
        if (this.UsesStickyDetonation)
        {
          if (hand.Input.BYButtonDown)
            this.SetAnimatedComponent(this.StickyTrigger, this.StickyRotRange.y, FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
          else if (hand.Input.BYButtonUp)
            this.SetAnimatedComponent(this.StickyTrigger, this.StickyRotRange.x, FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
        }
      }
      else
      {
        Vector2 touchpadAxes = hand.Input.TouchpadAxes;
        if (hand.Input.TouchpadDown)
        {
          if (this.UsesStickyDetonation)
            this.Detonate();
          if ((double) touchpadAxes.magnitude > 0.200000002980232)
          {
            if ((double) Vector2.Angle(touchpadAxes, Vector2.left) <= 45.0)
              this.ToggleFireSelector();
            else if ((double) Vector2.Angle(touchpadAxes, Vector2.up) <= 45.0)
            {
              if (this.HasBoltReleaseButton)
                this.Bolt.ReleaseBolt();
            }
            else if ((double) Vector2.Angle(touchpadAxes, Vector2.down) <= 45.0 && this.HasMagReleaseButton && (!this.EjectsMagazineOnEmpty || this.Bolt.CurPos >= ClosedBolt.BoltPos.Locked && this.Bolt.IsHeld && !this.m_proxy.IsFull))
              this.ReleaseMag();
          }
        }
        if (this.UsesStickyDetonation)
        {
          if (hand.Input.TouchpadDown)
            this.SetAnimatedComponent(this.StickyTrigger, this.StickyRotRange.y, FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
          else if (hand.Input.TouchpadUp)
            this.SetAnimatedComponent(this.StickyTrigger, this.StickyRotRange.x, FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
        }
        if (hand.Input.TouchpadPressed && (double) touchpadAxes.magnitude > 0.200000002980232)
        {
          if ((double) Vector2.Angle(touchpadAxes, Vector2.up) <= 45.0)
          {
            if (this.HasBoltReleaseButton)
              this.IsBoltReleaseButtonHeld = true;
          }
          else if ((double) Vector2.Angle(touchpadAxes, Vector2.right) <= 45.0 && this.HasBoltCatchButton)
            this.IsBoltCatchButtonHeld = true;
        }
      }
      ClosedBoltWeapon.FireSelectorModeType modeType = this.FireSelector_Modes[this.m_fireSelectorMode].ModeType;
      if (modeType == ClosedBoltWeapon.FireSelectorModeType.Safe || !this.m_hasTriggeredUpSinceBegin)
        return;
      if (this.UsesStickyDetonation)
      {
        if (this.Bolt.CurPos != ClosedBolt.BoltPos.Forward || !this.Chamber.IsFull || this.Chamber.IsSpent)
          return;
        if (hand.Input.TriggerPressed && this.m_hasTriggerReset)
        {
          this.m_hasStickTriggerDown = true;
          this.m_stickyChargeUp += Time.deltaTime * 0.25f * this.StickyChargeUpSpeed;
          this.m_stickyChargeUp = Mathf.Clamp(this.m_stickyChargeUp, 0.0f, 1f);
          if ((double) this.m_stickyChargeUp > 0.0500000007450581 && !this.m_chargeSound.isPlaying)
            this.m_chargeSound.Play();
        }
        else
        {
          if (this.m_chargeSound.isPlaying)
            this.m_chargeSound.Stop();
          this.m_stickyChargeUp = 0.0f;
        }
        if (!this.m_hasStickTriggerDown || !hand.Input.TriggerUp && (double) this.m_stickyChargeUp < 1.0)
          return;
        this.m_hasStickTriggerDown = false;
        this.DropHammer();
        this.EndStickyCharge();
      }
      else
      {
        if ((double) this.m_triggerFloat < (double) this.TriggerFiringThreshold || this.Bolt.CurPos != ClosedBolt.BoltPos.Forward || !this.m_hasTriggerReset && (modeType != ClosedBoltWeapon.FireSelectorModeType.FullAuto || this.UsesDualStageFullAuto) && (modeType != ClosedBoltWeapon.FireSelectorModeType.FullAuto || !this.UsesDualStageFullAuto || (double) this.m_triggerFloat <= (double) this.TriggerDualStageThreshold) && (modeType != ClosedBoltWeapon.FireSelectorModeType.Burst || this.m_CamBurst <= 0))
          return;
        this.DropHammer();
        this.m_hasTriggerReset = false;
        if (this.m_CamBurst <= 0)
          return;
        --this.m_CamBurst;
      }
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      if (this.UsesStickyDetonation)
        this.EndStickyCharge();
      base.EndInteraction(hand);
    }

    private void EndStickyCharge()
    {
      this.m_chargeSound.Stop();
      this.m_stickyChargeUp = 0.0f;
    }

    private void UpdateComponents()
    {
      if (this.HasTrigger)
        this.SetAnimatedComponent(this.Trigger, Mathf.Lerp(this.Trigger_ForwardValue, this.Trigger_RearwardValue, this.m_triggerFloat), this.TriggerInterpStyle, this.TriggerAxis);
      if (!this.UsesStickyDetonation)
        return;
      float t = Mathf.Clamp((float) this.m_primedBombs.Count / 8f, 0.0f, 1f);
      float y = Mathf.Lerp(0.56f, 0.23f, t);
      Mathf.Lerp(5f, 15f, t);
      this.StickyScreen.material.SetTextureOffset("_IncandescenceMap", new Vector2(0.0f, y));
    }

    private void UpdateDisplayRoundPositions()
    {
      float betweenLockAndFore = this.Bolt.GetBoltLerpBetweenLockAndFore();
      if (this.Chamber.IsFull)
      {
        this.Chamber.ProxyRound.position = Vector3.Lerp(this.RoundPos_Ejecting.position, this.Chamber.transform.position, betweenLockAndFore);
        this.Chamber.ProxyRound.rotation = Quaternion.Slerp(this.RoundPos_Ejecting.rotation, this.Chamber.transform.rotation, betweenLockAndFore);
      }
      if (!this.m_proxy.IsFull)
        return;
      this.m_proxy.ProxyRound.position = Vector3.Lerp(this.RoundPos_MagazinePos.position, this.Chamber.transform.position, betweenLockAndFore);
      this.m_proxy.ProxyRound.rotation = Quaternion.Slerp(this.RoundPos_MagazinePos.rotation, this.Chamber.transform.rotation, betweenLockAndFore);
    }

    public void ReleaseMag()
    {
      if (!((UnityEngine.Object) this.Magazine != (UnityEngine.Object) null))
        return;
      this.EjectMag();
    }

    public void RegisterStickyBomb(MF2_StickyBomb sb)
    {
      if (!((UnityEngine.Object) sb != (UnityEngine.Object) null))
        return;
      this.m_primedBombs.Add(sb);
    }

    public void Detonate()
    {
      bool flag = false;
      if (this.m_primedBombs.Count > 0)
      {
        for (int index = this.m_primedBombs.Count - 1; index >= 0; --index)
        {
          if ((UnityEngine.Object) this.m_primedBombs[index] != (UnityEngine.Object) null && this.m_primedBombs[index].m_hasStuck && !this.m_primedBombs[index].m_hasExploded)
          {
            flag = true;
            this.m_primedBombs[index].Detonate();
            this.m_primedBombs.RemoveAt(index);
          }
        }
      }
      if (!flag)
        return;
      SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.StickyDetonateSound, this.transform.position);
    }

    public override List<FireArmRoundClass> GetChamberRoundList()
    {
      if (!this.Chamber.IsFull || this.Chamber.IsSpent)
        return (List<FireArmRoundClass>) null;
      return new List<FireArmRoundClass>()
      {
        this.Chamber.GetRound().RoundClass
      };
    }

    public override void SetLoadedChambers(List<FireArmRoundClass> rounds)
    {
      if (rounds.Count <= 0)
        return;
      this.Chamber.Autochamber(rounds[0]);
    }

    public override List<string> GetFlagList() => (List<string>) null;

    public override void SetFromFlagList(List<string> flags)
    {
    }

    public override void ConfigureFromFlagDic(Dictionary<string, string> f)
    {
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      string key1 = "HammerState";
      if (f.ContainsKey(key1) && f[key1] == "Cocked")
        this.m_isHammerCocked = true;
      if (this.FireSelector_Modes.Length <= 1)
        return;
      string key2 = "FireSelectorState";
      if (f.ContainsKey(key2))
        int.TryParse(f[key2], out this.m_fireSelectorMode);
      if ((UnityEngine.Object) this.FireSelectorSwitch != (UnityEngine.Object) null)
        this.SetAnimatedComponent(this.FireSelectorSwitch, this.FireSelector_Modes[this.m_fireSelectorMode].SelectorPosition, this.FireSelector_InterpStyle, this.FireSelector_Axis);
      if (!((UnityEngine.Object) this.FireSelectorSwitch2 != (UnityEngine.Object) null))
        return;
      this.SetAnimatedComponent(this.FireSelectorSwitch2, this.FireSelector_Modes2[this.m_fireSelectorMode].SelectorPosition, this.FireSelector_InterpStyle2, this.FireSelector_Axis2);
    }

    public override Dictionary<string, string> GetFlagDic()
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      string key1 = "HammerState";
      string str1 = "Uncocked";
      if (this.m_isHammerCocked)
        str1 = "Cocked";
      dictionary.Add(key1, str1);
      if (this.FireSelector_Modes.Length > 1)
      {
        string key2 = "FireSelectorState";
        string str2 = this.m_fireSelectorMode.ToString();
        dictionary.Add(key2, str2);
      }
      return dictionary;
    }

    public enum FireSelectorModeType
    {
      Safe,
      Single,
      Burst,
      FullAuto,
      SuperFastBurst,
    }

    [Serializable]
    public class FireSelectorMode
    {
      public float SelectorPosition;
      public ClosedBoltWeapon.FireSelectorModeType ModeType;
      public int BurstAmount = 3;
    }
  }
}
