// Decompiled with JetBrains decompiler
// Type: FistVR.OpenBoltReceiver
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class OpenBoltReceiver : FVRFireArm
  {
    [Header("OpenBoltWeapon Config")]
    public bool HasTriggerButton = true;
    public bool HasFireSelectorButton = true;
    public bool HasMagReleaseButton = true;
    public bool DoesForwardBoltDisableReloadWell;
    [Header("Component Connections")]
    public OpenBoltReceiverBolt Bolt;
    public FVRFireArmChamber Chamber;
    public Transform Trigger;
    public Transform MagReleaseButton;
    public Transform FireSelectorSwitch;
    public Transform FireSelectorSwitch2;
    public GameObject ReloadTriggerWell;
    [Header("Round Positions")]
    public Transform RoundPos_Ejecting;
    public Transform RoundPos_Ejection;
    public Transform RoundPos_MagazinePos;
    private FVRFirearmMovingProxyRound m_proxy;
    public Vector3 EjectionSpeed;
    public Vector3 EjectionSpin;
    public bool UsesDelinker;
    public ParticleSystem DelinkerSystem;
    [Header("Trigger Config")]
    public float TriggerFiringThreshold = 0.8f;
    public float TriggerResetThreshold = 0.4f;
    public float Trigger_ForwardValue;
    public float Trigger_RearwardValue;
    public OpenBoltReceiver.InterpStyle TriggerInterpStyle = OpenBoltReceiver.InterpStyle.Rotation;
    private float m_triggerFloat;
    private bool m_hasTriggerCycled;
    private bool m_isSeerEngaged = true;
    private bool m_isHammerCocked;
    private bool m_isCamSet = true;
    private int m_CamBurst;
    public int SuperBurstAmount = 3;
    private int m_fireSelectorMode;
    [Header("Fire Selector Config")]
    public OpenBoltReceiver.InterpStyle FireSelector_InterpStyle = OpenBoltReceiver.InterpStyle.Rotation;
    public OpenBoltReceiver.Axis FireSelector_Axis;
    public OpenBoltReceiver.FireSelectorMode[] FireSelector_Modes;
    [Header("Secondary Fire Selector Config")]
    public OpenBoltReceiver.InterpStyle FireSelector_InterpStyle2 = OpenBoltReceiver.InterpStyle.Rotation;
    public OpenBoltReceiver.Axis FireSelector_Axis2;
    public OpenBoltReceiver.FireSelectorMode[] FireSelector_Modes2;
    private float m_timeSinceFiredShot = 1f;
    [Header("SpecialFeatures")]
    public bool UsesRecoilingSystem;
    public G11RecoilingSystem RecoilingSystem;
    public bool UsesMagMountTransformOverride;
    public Transform MagMountTransformOverride;

    public bool HasExtractedRound() => this.m_proxy.IsFull;

    public bool IsSeerEngaged => this.m_isSeerEngaged;

    public bool IsHammerCocked => this.m_isHammerCocked;

    public int FireSelectorModeIndex => this.m_fireSelectorMode;

    protected override void Awake()
    {
      base.Awake();
      this.m_proxy = new GameObject("m_proxyRound").AddComponent<FVRFirearmMovingProxyRound>();
      this.m_proxy.Init(this.transform);
    }

    public override int GetTutorialState()
    {
      if ((UnityEngine.Object) this.Magazine == (UnityEngine.Object) null)
        return 0;
      if ((UnityEngine.Object) this.Magazine != (UnityEngine.Object) null && !this.Magazine.HasARound())
        return 5;
      if (this.FireSelector_Modes[this.m_fireSelectorMode].ModeType == OpenBoltReceiver.FireSelectorModeType.Safe)
        return 1;
      if (this.Bolt.CurPos == OpenBoltReceiverBolt.BoltPos.Forward & (double) this.m_timeSinceFiredShot > 0.400000005960464)
        return 2;
      return (UnityEngine.Object) this.AltGrip == (UnityEngine.Object) null ? 3 : 4;
    }

    public void SecondaryFireSelectorClicked() => this.PlayAudioEvent(FirearmAudioEventType.FireSelector);

    public bool IsBoltCatchEngaged() => this.m_isSeerEngaged;

    public void ReleaseSeer()
    {
      if (this.m_isSeerEngaged && this.Bolt.CurPos == OpenBoltReceiverBolt.BoltPos.Locked)
        this.PlayAudioEvent(FirearmAudioEventType.Prefire);
      this.m_isSeerEngaged = false;
    }

    public void EngageSeer() => this.m_isSeerEngaged = true;

    protected virtual void ToggleFireSelector()
    {
      if (this.FireSelector_Modes.Length <= 1)
        return;
      ++this.m_fireSelectorMode;
      if (this.m_fireSelectorMode >= this.FireSelector_Modes.Length)
        this.m_fireSelectorMode -= this.FireSelector_Modes.Length;
      this.PlayAudioEvent(FirearmAudioEventType.FireSelector);
      if ((UnityEngine.Object) this.FireSelectorSwitch != (UnityEngine.Object) null)
      {
        switch (this.FireSelector_InterpStyle)
        {
          case OpenBoltReceiver.InterpStyle.Translate:
            Vector3 zero1 = Vector3.zero;
            switch (this.FireSelector_Axis)
            {
              case OpenBoltReceiver.Axis.X:
                zero1.x = this.FireSelector_Modes[this.m_fireSelectorMode].SelectorPosition;
                break;
              case OpenBoltReceiver.Axis.Y:
                zero1.y = this.FireSelector_Modes[this.m_fireSelectorMode].SelectorPosition;
                break;
              case OpenBoltReceiver.Axis.Z:
                zero1.z = this.FireSelector_Modes[this.m_fireSelectorMode].SelectorPosition;
                break;
            }
            this.FireSelectorSwitch.localPosition = zero1;
            break;
          case OpenBoltReceiver.InterpStyle.Rotation:
            Vector3 zero2 = Vector3.zero;
            switch (this.FireSelector_Axis)
            {
              case OpenBoltReceiver.Axis.X:
                zero2.x = this.FireSelector_Modes[this.m_fireSelectorMode].SelectorPosition;
                break;
              case OpenBoltReceiver.Axis.Y:
                zero2.y = this.FireSelector_Modes[this.m_fireSelectorMode].SelectorPosition;
                break;
              case OpenBoltReceiver.Axis.Z:
                zero2.z = this.FireSelector_Modes[this.m_fireSelectorMode].SelectorPosition;
                break;
            }
            this.FireSelectorSwitch.localEulerAngles = zero2;
            break;
        }
      }
      if (!((UnityEngine.Object) this.FireSelectorSwitch2 != (UnityEngine.Object) null))
        return;
      switch (this.FireSelector_InterpStyle2)
      {
        case OpenBoltReceiver.InterpStyle.Translate:
          Vector3 zero3 = Vector3.zero;
          switch (this.FireSelector_Axis2)
          {
            case OpenBoltReceiver.Axis.X:
              zero3.x = this.FireSelector_Modes2[this.m_fireSelectorMode].SelectorPosition;
              break;
            case OpenBoltReceiver.Axis.Y:
              zero3.y = this.FireSelector_Modes2[this.m_fireSelectorMode].SelectorPosition;
              break;
            case OpenBoltReceiver.Axis.Z:
              zero3.z = this.FireSelector_Modes2[this.m_fireSelectorMode].SelectorPosition;
              break;
          }
          this.FireSelectorSwitch2.localPosition = zero3;
          break;
        case OpenBoltReceiver.InterpStyle.Rotation:
          Vector3 zero4 = Vector3.zero;
          switch (this.FireSelector_Axis2)
          {
            case OpenBoltReceiver.Axis.X:
              zero4.x = this.FireSelector_Modes2[this.m_fireSelectorMode].SelectorPosition;
              break;
            case OpenBoltReceiver.Axis.Y:
              zero4.y = this.FireSelector_Modes2[this.m_fireSelectorMode].SelectorPosition;
              break;
            case OpenBoltReceiver.Axis.Z:
              zero4.z = this.FireSelector_Modes2[this.m_fireSelectorMode].SelectorPosition;
              break;
          }
          this.FireSelectorSwitch2.localEulerAngles = zero4;
          break;
      }
    }

    public void EjectExtractedRound()
    {
      if (!this.Chamber.IsFull)
        return;
      this.Chamber.EjectRound(this.RoundPos_Ejection.position, this.transform.right * this.EjectionSpeed.x + this.transform.up * this.EjectionSpeed.y + this.transform.forward * this.EjectionSpeed.z, this.transform.right * this.EjectionSpin.x + this.transform.up * this.EjectionSpin.y + this.transform.forward * this.EjectionSpin.z);
    }

    public void BeginChamberingRound()
    {
      OpenBoltReceiver.FireSelectorModeType modeType = this.FireSelector_Modes[this.m_fireSelectorMode].ModeType;
      OpenBoltReceiver.FireSelectorMode fireSelectorMode = this.FireSelector_Modes[this.m_fireSelectorMode];
      if (modeType == OpenBoltReceiver.FireSelectorModeType.Single || modeType == OpenBoltReceiver.FireSelectorModeType.SuperFastBurst)
        this.EngageSeer();
      bool flag = false;
      GameObject go = (GameObject) null;
      if (this.HasBelt)
      {
        if (!this.m_proxy.IsFull && this.BeltDD.HasARound())
        {
          if (this.AudioClipSet.BeltSettlingLimit > 0)
            this.PlayAudioEvent(FirearmAudioEventType.BeltSettle);
          flag = true;
          go = this.BeltDD.RemoveRound(false);
        }
      }
      else if (!this.m_proxy.IsFull && (UnityEngine.Object) this.Magazine != (UnityEngine.Object) null && (!this.Magazine.IsBeltBox && this.Magazine.HasARound()))
      {
        flag = true;
        go = this.Magazine.RemoveRound(false);
      }
      if (!flag)
        return;
      if (flag)
        this.m_proxy.SetFromPrefabReference(go);
      if (!this.Bolt.HasLastRoundBoltHoldOpen || !((UnityEngine.Object) this.Magazine != (UnityEngine.Object) null) || (this.Magazine.HasARound() || !this.Magazine.DoesFollowerStopBolt) || this.Magazine.IsBeltBox)
        return;
      this.EngageSeer();
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

    public bool Fire()
    {
      if (!this.Chamber.Fire())
        return false;
      this.m_timeSinceFiredShot = 0.0f;
      this.Fire(this.Chamber, this.GetMuzzle(), true);
      this.FireMuzzleSmoke();
      if (this.UsesDelinker && this.HasBelt)
        this.DelinkerSystem.Emit(1);
      if (this.HasBelt)
        this.BeltDD.AddJitter();
      bool twoHandStabilized = this.IsTwoHandStabilized();
      bool foregripStabilized = (UnityEngine.Object) this.AltGrip != (UnityEngine.Object) null;
      bool shoulderStabilized = this.IsShoulderStabilized();
      this.Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
      bool flag = false;
      if (this.FireSelector_Modes[this.m_fireSelectorMode].ModeType == OpenBoltReceiver.FireSelectorModeType.SuperFastBurst)
      {
        for (int index = 1; index < this.SuperBurstAmount; ++index)
        {
          if (this.Magazine.HasARound())
          {
            this.Magazine.RemoveRound();
            this.Fire(this.Chamber, this.GetMuzzle(), false);
            flag = true;
            this.FireMuzzleSmoke();
            this.Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
          }
        }
      }
      if (this.UsesRecoilingSystem)
      {
        if (flag)
          this.RecoilingSystem.Recoil(true);
        else
          this.RecoilingSystem.Recoil(false);
      }
      if (flag)
        this.PlayAudioGunShot(false, this.Chamber.GetRound().TailClass, this.Chamber.GetRound().TailClassSuppressed, GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
      else
        this.PlayAudioGunShot(this.Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
      return true;
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      this.UpdateControls();
      this.Bolt.UpdateBolt();
      this.UpdateDisplayRoundPositions();
      if ((double) this.m_timeSinceFiredShot >= 1.0)
        return;
      this.m_timeSinceFiredShot += Time.deltaTime;
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (!((UnityEngine.Object) this.Trigger != (UnityEngine.Object) null))
        return;
      if (this.TriggerInterpStyle == OpenBoltReceiver.InterpStyle.Translate)
      {
        this.Trigger.localPosition = new Vector3(0.0f, 0.0f, Mathf.Lerp(this.Trigger_ForwardValue, this.Trigger_RearwardValue, this.m_triggerFloat));
      }
      else
      {
        if (this.TriggerInterpStyle != OpenBoltReceiver.InterpStyle.Rotation)
          return;
        this.Trigger.localEulerAngles = new Vector3(Mathf.Lerp(this.Trigger_ForwardValue, this.Trigger_RearwardValue, this.m_triggerFloat), 0.0f, 0.0f);
      }
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      this.EngageSeer();
      this.m_hasTriggerCycled = false;
      this.m_triggerFloat = 0.0f;
      if ((UnityEngine.Object) this.Trigger != (UnityEngine.Object) null)
      {
        if (this.TriggerInterpStyle == OpenBoltReceiver.InterpStyle.Translate)
          this.Trigger.localPosition = new Vector3(0.0f, 0.0f, Mathf.Lerp(this.Trigger_ForwardValue, this.Trigger_RearwardValue, this.m_triggerFloat));
        else if (this.TriggerInterpStyle == OpenBoltReceiver.InterpStyle.Rotation)
          this.Trigger.localEulerAngles = new Vector3(Mathf.Lerp(this.Trigger_ForwardValue, this.Trigger_RearwardValue, this.m_triggerFloat), 0.0f, 0.0f);
      }
      base.EndInteraction(hand);
    }

    public override void EndInteractionIntoInventorySlot(FVRViveHand hand, FVRQuickBeltSlot slot)
    {
      this.EngageSeer();
      this.m_hasTriggerCycled = false;
      base.EndInteractionIntoInventorySlot(hand, slot);
    }

    private void UpdateControls()
    {
      if (this.IsHeld)
      {
        this.m_triggerFloat = !this.HasTriggerButton || !this.m_hasTriggeredUpSinceBegin || (this.IsAltHeld || this.FireSelector_Modes[this.m_fireSelectorMode].ModeType == OpenBoltReceiver.FireSelectorModeType.Safe) ? 0.0f : this.m_hand.Input.TriggerFloat;
        bool flag = false;
        if (this.Bolt.HasLastRoundBoltHoldOpen && (UnityEngine.Object) this.Magazine != (UnityEngine.Object) null && (!this.Magazine.HasARound() && !this.Magazine.IsBeltBox))
          flag = true;
        if (!this.m_hasTriggerCycled)
        {
          if ((double) this.m_triggerFloat >= (double) this.TriggerFiringThreshold)
          {
            this.m_hasTriggerCycled = true;
            if (!flag)
              this.ReleaseSeer();
          }
        }
        else if ((double) this.m_triggerFloat <= (double) this.TriggerResetThreshold && this.m_hasTriggerCycled)
        {
          this.EngageSeer();
          this.m_hasTriggerCycled = false;
          this.PlayAudioEvent(FirearmAudioEventType.TriggerReset);
        }
        if (this.IsAltHeld)
          return;
        if (this.m_hand.IsInStreamlinedMode)
        {
          if (this.m_hand.Input.BYButtonDown && this.HasFireSelectorButton)
            this.ToggleFireSelector();
          if (!this.m_hand.Input.AXButtonDown || !this.HasMagReleaseButton)
            return;
          this.EjectMag();
        }
        else
        {
          if (!this.m_hand.Input.TouchpadDown || (double) this.m_hand.Input.TouchpadAxes.magnitude <= 0.100000001490116)
            return;
          if (this.HasFireSelectorButton && (double) Vector2.Angle(this.m_hand.Input.TouchpadAxes, Vector2.left) <= 45.0)
          {
            this.ToggleFireSelector();
          }
          else
          {
            if (!this.HasMagReleaseButton || (double) Vector2.Angle(this.m_hand.Input.TouchpadAxes, Vector2.down) > 45.0)
              return;
            this.EjectMag();
          }
        }
      }
      else
        this.m_triggerFloat = 0.0f;
    }

    private void UpdateDisplayRoundPositions()
    {
      float betweenLockAndFore = this.Bolt.GetBoltLerpBetweenLockAndFore();
      if (this.m_proxy.IsFull)
      {
        this.m_proxy.ProxyRound.position = Vector3.Lerp(this.RoundPos_MagazinePos.position, this.Chamber.transform.position, betweenLockAndFore);
        this.m_proxy.ProxyRound.rotation = Quaternion.Slerp(this.RoundPos_MagazinePos.rotation, this.Chamber.transform.rotation, betweenLockAndFore);
      }
      else if (this.Chamber.IsFull)
      {
        this.Chamber.ProxyRound.position = Vector3.Lerp(this.RoundPos_Ejecting.position, this.Chamber.transform.position, betweenLockAndFore);
        this.Chamber.ProxyRound.rotation = Quaternion.Slerp(this.RoundPos_Ejecting.rotation, this.Chamber.transform.rotation, betweenLockAndFore);
      }
      if (!this.DoesForwardBoltDisableReloadWell)
        return;
      if (this.Bolt.CurPos >= OpenBoltReceiverBolt.BoltPos.Locked)
      {
        if (this.ReloadTriggerWell.activeSelf)
          return;
        this.ReloadTriggerWell.SetActive(true);
      }
      else
      {
        if (!this.ReloadTriggerWell.activeSelf)
          return;
        this.ReloadTriggerWell.SetActive(false);
      }
    }

    public void ReleaseMag()
    {
      if (!((UnityEngine.Object) this.Magazine != (UnityEngine.Object) null))
        return;
      this.EjectMag();
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

    public new enum InterpStyle
    {
      Translate,
      Rotation,
    }

    public new enum Axis
    {
      X,
      Y,
      Z,
    }

    public enum FireSelectorModeType
    {
      Safe,
      Single,
      FullAuto,
      SuperFastBurst,
    }

    [Serializable]
    public class FireSelectorMode
    {
      public float SelectorPosition;
      public OpenBoltReceiver.FireSelectorModeType ModeType;
    }
  }
}
