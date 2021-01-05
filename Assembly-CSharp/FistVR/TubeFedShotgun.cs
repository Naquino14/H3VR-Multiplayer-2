// Decompiled with JetBrains decompiler
// Type: FistVR.TubeFedShotgun
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class TubeFedShotgun : FVRFireArm
  {
    [Header("Shotgun Params")]
    public TubeFedShotgunBolt Bolt;
    public TubeFedShotgunHandle Handle;
    public FVRFireArmChamber Chamber;
    public bool HasHandle;
    [Header("Component Connections")]
    public Transform Trigger;
    public Transform Safety;
    public GameObject ReloadTriggerWell;
    [Header("Carrier System")]
    public bool UsesAnimatedCarrier;
    public Transform Carrier;
    public Vector2 CarrierRots;
    public Transform CarrierComparePoint1;
    public Transform CarrierComparePoint2;
    public float CarrierDetectDistance;
    private float m_curCarrierRot;
    private float m_tarCarrierRot;
    [Header("Round Positions")]
    public Transform RoundPos_LowerPath_Forward;
    public Transform RoundPos_LowerPath_Rearward;
    public Transform RoundPos_UpperPath_Forward;
    public Transform RoundPos_UpperPath_Rearward;
    public Transform RoundPos_Ejecting;
    public Transform RoundPos_Ejection;
    public Vector3 RoundEjectionSpeed;
    public Vector3 RoundEjectionSpin;
    private FVRFirearmMovingProxyRound m_proxy;
    private bool m_isExtractedRoundOnLowerPath = true;
    [Header("Trigger Params")]
    public bool HasTrigger;
    public FVRPhysicalObject.InterpStyle TriggerInterp;
    public FVRPhysicalObject.Axis TriggerAxis;
    public float TriggerUnheld;
    public float TriggerHeld;
    public float TriggerResetThreshold = 0.45f;
    public float TriggerBreakThreshold = 0.85f;
    private float m_triggerFloat;
    private bool m_hasTriggerReset = true;
    public bool UsesSlamFireTrigger;
    [Header("Safety Params")]
    public bool HasSafety;
    public FVRPhysicalObject.InterpStyle Safety_Interp;
    public FVRPhysicalObject.Axis SafetyAxis;
    public float SafetyOff;
    public float SafetyOn;
    private bool m_isSafetyEngaged = true;
    private bool m_isHammerCocked;
    [Header("Control Params")]
    public bool HasSlideReleaseButton;
    [HideInInspector]
    public bool IsSlideReleaseButtonHeld;
    [Header("Mode Params")]
    public bool CanModeSwitch;
    public TubeFedShotgun.ShotgunMode Mode;
    private bool m_isChamberRoundOnExtractor;

    public bool HasExtractedRound() => this.m_proxy.IsFull;

    public bool IsSafetyEngaged => this.m_isSafetyEngaged;

    public bool IsHammerCocked => this.m_isHammerCocked;

    protected override void Awake()
    {
      base.Awake();
      if (this.Mode == TubeFedShotgun.ShotgunMode.Automatic)
      {
        if (this.HasHandle)
          this.Handle.LockHandle();
      }
      else if (this.HasHandle)
        this.Handle.UnlockHandle();
      if (!this.HasSafety)
        this.m_isSafetyEngaged = false;
      this.m_proxy = new GameObject("m_proxyRound").AddComponent<FVRFirearmMovingProxyRound>();
      this.m_proxy.Init(this.transform);
    }

    public override int GetTutorialState()
    {
      if (this.m_isSafetyEngaged)
        return 2;
      if (this.Chamber.IsFull && !this.Chamber.IsSpent)
        return 3;
      return (Object) this.Magazine == (Object) null || !this.Magazine.HasARound() ? 0 : 1;
    }

    public void BoltReleasePressed()
    {
      if (this.Mode != TubeFedShotgun.ShotgunMode.Automatic)
        return;
      this.Bolt.ReleaseBolt();
    }

    public bool CanCycleMagState() => this.Handle.CurPos == TubeFedShotgunHandle.BoltPos.Forward && !this.HasExtractedRound();

    public void ToggleSafety()
    {
      if (!this.HasSafety)
        return;
      this.m_isSafetyEngaged = !this.m_isSafetyEngaged;
      this.PlayAudioEvent(FirearmAudioEventType.Safety);
      this.UpdateSafetyGeo();
    }

    private void UpdateSafetyGeo() => this.SetAnimatedComponent(this.Safety, !this.m_isSafetyEngaged ? this.SafetyOff : this.SafetyOn, this.Safety_Interp, this.SafetyAxis);

    public void EjectExtractedRound()
    {
      if (!this.m_isChamberRoundOnExtractor)
        return;
      this.m_isChamberRoundOnExtractor = false;
      if (!this.Chamber.IsFull)
        return;
      this.Chamber.EjectRound(this.RoundPos_Ejection.position, this.transform.right * this.RoundEjectionSpeed.x + this.transform.up * this.RoundEjectionSpeed.y + this.transform.forward * this.RoundEjectionSpeed.z, this.transform.right * this.RoundEjectionSpin.x + this.transform.up * this.RoundEjectionSpin.y + this.transform.forward * this.RoundEjectionSpin.z);
    }

    public void ExtractRound()
    {
      if ((Object) this.Magazine == (Object) null || this.m_proxy.IsFull || (this.m_proxy.IsFull || !this.Magazine.HasARound()))
        return;
      this.m_proxy.SetFromPrefabReference(this.Magazine.RemoveRound(false));
      this.m_isExtractedRoundOnLowerPath = true;
    }

    public bool ChamberRound()
    {
      if (this.Chamber.IsFull)
        this.m_isChamberRoundOnExtractor = true;
      if (!this.m_proxy.IsFull || this.Chamber.IsFull || this.m_isExtractedRoundOnLowerPath)
        return false;
      this.m_isChamberRoundOnExtractor = true;
      this.Chamber.SetRound(this.m_proxy.Round);
      this.m_proxy.ClearProxy();
      return true;
    }

    public bool ReturnCarrierRoundToMagazineIfRelevant()
    {
      if (!this.m_proxy.IsFull || !this.m_isExtractedRoundOnLowerPath)
        return false;
      this.Magazine.AddRound(this.m_proxy.Round.RoundClass, false, true);
      this.m_proxy.ClearProxy();
      return true;
    }

    public void TransferShellToUpperTrack()
    {
      if (!this.m_proxy.IsFull || !this.m_isExtractedRoundOnLowerPath || this.Chamber.IsFull)
        return;
      this.m_isExtractedRoundOnLowerPath = false;
    }

    public void ToggleMode()
    {
      if (this.Bolt.CurPos != TubeFedShotgunBolt.BoltPos.Forward)
        Debug.Log((object) "not forward");
      else if (this.HasHandle && this.Handle.CurPos != TubeFedShotgunHandle.BoltPos.Forward)
        Debug.Log((object) "not forward");
      else if (this.m_isHammerCocked)
      {
        Debug.Log((object) "hammer cocked");
      }
      else
      {
        this.PlayAudioEvent(FirearmAudioEventType.FireSelector);
        if (this.Mode == TubeFedShotgun.ShotgunMode.PumpMode)
        {
          this.Mode = TubeFedShotgun.ShotgunMode.Automatic;
          if (!this.HasHandle)
            return;
          this.Handle.LockHandle();
        }
        else
        {
          this.Mode = TubeFedShotgun.ShotgunMode.PumpMode;
          if (!this.HasHandle)
            return;
          this.Handle.UnlockHandle();
        }
      }
    }

    public void CockHammer()
    {
      if (this.m_isHammerCocked)
        return;
      this.m_isHammerCocked = true;
      this.PlayAudioEvent(FirearmAudioEventType.Prefire);
    }

    public void ReleaseHammer()
    {
      if (!this.m_isHammerCocked || this.Bolt.CurPos != TubeFedShotgunBolt.BoltPos.Forward)
        return;
      this.PlayAudioEvent(FirearmAudioEventType.HammerHit);
      this.Fire();
      this.m_isHammerCocked = false;
      if (!this.HasHandle || this.Mode != TubeFedShotgun.ShotgunMode.PumpMode)
        return;
      this.Handle.UnlockHandle();
    }

    public bool Fire()
    {
      if (!this.Chamber.Fire())
        return false;
      this.Fire(this.Chamber, this.GetMuzzle(), true);
      this.FireMuzzleSmoke();
      this.Recoil(this.IsTwoHandStabilized(), this.IsForegripStabilized(), this.IsShoulderStabilized());
      this.PlayAudioGunShot(this.Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
      if (this.Mode == TubeFedShotgun.ShotgunMode.Automatic && this.Chamber.GetRound().IsHighPressure)
        this.Bolt.ImpartFiringImpulse();
      return true;
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      this.UpdateComponents();
      this.UpdateCarrier();
      if (this.HasHandle)
      {
        this.Handle.UpdateHandle();
        bool state = false;
        if ((this.Handle.IsHeld || this.IsAltHeld) && this.Mode == TubeFedShotgun.ShotgunMode.PumpMode)
          state = true;
        this.Bolt.UpdateHandleHeldState(state, 1f - this.Handle.GetBoltLerpBetweenRearAndFore());
      }
      this.Bolt.UpdateBolt();
      this.UpdateDisplayRoundPositions();
      if (this.HasExtractedRound() && this.m_isExtractedRoundOnLowerPath)
      {
        if (!((Object) this.Magazine != (Object) null))
          return;
        this.Magazine.IsDropInLoadable = false;
      }
      else
      {
        if (!((Object) this.Magazine != (Object) null))
          return;
        this.Magazine.IsDropInLoadable = true;
      }
    }

    private void UpdateCarrier()
    {
      if (!this.UsesAnimatedCarrier)
        return;
      this.m_tarCarrierRot = !this.IsHeld ? this.CarrierRots.x : (!((Object) this.m_hand.OtherHand.CurrentInteractable != (Object) null) ? this.CarrierRots.x : (!(this.m_hand.OtherHand.CurrentInteractable is FVRFireArmRound) ? this.CarrierRots.x : ((double) Vector3.Distance(this.m_hand.OtherHand.CurrentInteractable.transform.position, this.GetClosestValidPoint(this.CarrierComparePoint1.position, this.CarrierComparePoint2.position, this.m_hand.OtherHand.CurrentInteractable.transform.position)) >= (double) this.CarrierDetectDistance ? this.CarrierRots.x : this.CarrierRots.y)));
      if (this.HasExtractedRound() && !this.m_isExtractedRoundOnLowerPath)
        this.m_tarCarrierRot = this.CarrierRots.y;
      if ((double) Mathf.Abs(this.m_curCarrierRot - this.m_tarCarrierRot) <= 1.0 / 1000.0)
        return;
      this.m_curCarrierRot = Mathf.MoveTowards(this.m_curCarrierRot, this.m_tarCarrierRot, 270f * Time.deltaTime);
      this.Carrier.localEulerAngles = new Vector3(this.m_curCarrierRot, 0.0f, 0.0f);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      this.UpdateInputAndAnimate(hand);
    }

    private void UpdateInputAndAnimate(FVRViveHand hand)
    {
      this.IsSlideReleaseButtonHeld = false;
      if (this.IsAltHeld)
        return;
      this.m_triggerFloat = !this.m_hasTriggeredUpSinceBegin ? 0.0f : hand.Input.TriggerFloat;
      if (!this.m_hasTriggerReset && (double) this.m_triggerFloat <= (double) this.TriggerResetThreshold)
      {
        this.m_hasTriggerReset = true;
        this.PlayAudioEvent(FirearmAudioEventType.TriggerReset);
      }
      Vector2 touchpadAxes = hand.Input.TouchpadAxes;
      if (hand.IsInStreamlinedMode)
      {
        if (hand.Input.BYButtonDown)
          this.ToggleSafety();
        if (hand.Input.AXButtonPressed)
        {
          this.IsSlideReleaseButtonHeld = true;
          if (this.HasHandle && this.Mode == TubeFedShotgun.ShotgunMode.PumpMode)
            this.Handle.UnlockHandle();
        }
      }
      else
      {
        if (hand.Input.TouchpadDown && (double) touchpadAxes.magnitude > 0.200000002980232)
        {
          if ((double) Vector2.Angle(touchpadAxes, Vector2.left) <= 45.0)
            this.ToggleSafety();
          else if ((double) Vector2.Angle(touchpadAxes, Vector2.up) <= 45.0 && this.Mode == TubeFedShotgun.ShotgunMode.Automatic)
            this.Bolt.ReleaseBolt();
        }
        if (hand.Input.TouchpadPressed && (double) touchpadAxes.magnitude > 0.200000002980232 && (double) Vector2.Angle(touchpadAxes, Vector2.up) <= 45.0)
        {
          this.IsSlideReleaseButtonHeld = true;
          if (this.HasHandle && this.Mode == TubeFedShotgun.ShotgunMode.PumpMode)
            this.Handle.UnlockHandle();
        }
      }
      if ((double) this.m_triggerFloat < (double) this.TriggerBreakThreshold || !this.m_isHammerCocked || this.m_isSafetyEngaged)
        return;
      if (this.m_hasTriggerReset || this.UsesSlamFireTrigger)
        this.ReleaseHammer();
      this.m_hasTriggerReset = false;
    }

    private void UpdateComponents()
    {
      if (!this.HasTrigger)
        return;
      this.SetAnimatedComponent(this.Trigger, Mathf.Lerp(this.TriggerUnheld, this.TriggerHeld, this.m_triggerFloat), this.TriggerInterp, this.TriggerAxis);
    }

    private void UpdateDisplayRoundPositions()
    {
      float betweenLockAndFore = this.Bolt.GetBoltLerpBetweenLockAndFore();
      if (this.Chamber.IsFull)
      {
        if (this.m_isChamberRoundOnExtractor)
        {
          this.Chamber.ProxyRound.position = Vector3.Lerp(this.RoundPos_Ejecting.position, this.Chamber.transform.position, betweenLockAndFore);
          this.Chamber.ProxyRound.rotation = Quaternion.Slerp(this.RoundPos_Ejecting.rotation, this.Chamber.transform.rotation, betweenLockAndFore);
        }
        else
        {
          this.Chamber.ProxyRound.position = this.Chamber.transform.position;
          this.Chamber.ProxyRound.rotation = this.Chamber.transform.rotation;
        }
      }
      if (!this.m_proxy.IsFull)
        return;
      if (this.m_isExtractedRoundOnLowerPath || this.Chamber.IsFull)
      {
        this.m_proxy.ProxyRound.position = Vector3.Lerp(this.RoundPos_LowerPath_Rearward.position, this.RoundPos_LowerPath_Forward.position, betweenLockAndFore);
        this.m_proxy.ProxyRound.rotation = Quaternion.Slerp(this.RoundPos_LowerPath_Rearward.rotation, this.RoundPos_LowerPath_Forward.rotation, betweenLockAndFore);
      }
      else
      {
        this.m_proxy.ProxyRound.position = Vector3.Lerp(this.RoundPos_UpperPath_Rearward.position, this.RoundPos_UpperPath_Forward.position, betweenLockAndFore);
        this.m_proxy.ProxyRound.rotation = Quaternion.Slerp(this.RoundPos_UpperPath_Rearward.rotation, this.RoundPos_UpperPath_Forward.rotation, betweenLockAndFore);
      }
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
      if (!this.HasSafety)
        return;
      string key2 = "SafetyState";
      if (!f.ContainsKey(key2))
        return;
      if (f[key2] == "On")
        this.m_isSafetyEngaged = true;
      this.UpdateSafetyGeo();
    }

    public override Dictionary<string, string> GetFlagDic()
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      string key1 = "HammerState";
      string str1 = "Uncocked";
      if (this.m_isHammerCocked)
        str1 = "Cocked";
      dictionary.Add(key1, str1);
      if (this.HasSafety)
      {
        string key2 = "SafetyState";
        string str2 = "Off";
        if (this.m_isSafetyEngaged)
          str2 = "On";
        dictionary.Add(key2, str2);
      }
      return dictionary;
    }

    public enum ShotgunMode
    {
      PumpMode,
      Automatic,
    }
  }
}
