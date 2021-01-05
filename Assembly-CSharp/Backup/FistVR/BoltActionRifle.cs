// Decompiled with JetBrains decompiler
// Type: FistVR.BoltActionRifle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class BoltActionRifle : FVRFireArm
  {
    [Header("BoltActionRifle Config")]
    public FVRFireArmChamber Chamber;
    public bool HasMagEjectionButton = true;
    public bool HasFireSelectorButton = true;
    public BoltActionRifle_Handle BoltHandle;
    public float BoltLerp;
    public bool BoltMovingForward;
    public BoltActionRifle_Handle.BoltActionHandleState CurBoltHandleState;
    public BoltActionRifle_Handle.BoltActionHandleState LastBoltHandleState;
    [Header("Hammer Config")]
    public bool HasVisualHammer;
    public Transform Hammer;
    public float HammerUncocked;
    public float HammerCocked;
    private bool m_isHammerCocked;
    public BoltActionRifle.HammerCockType CockType;
    private FVRFirearmMovingProxyRound m_proxy;
    [Header("Round Positions Config")]
    public Transform Extraction_MagazinePos;
    public Transform Extraction_ChamberPos;
    public Transform Extraction_Ejecting;
    public Transform EjectionPos;
    public float UpwardEjectionForce;
    public float RightwardEjectionForce = 2f;
    public float YSpinEjectionTorque = 80f;
    public Transform Muzzle;
    public GameObject ReloadTriggerWell;
    [Header("Control Config")]
    public float TriggerResetThreshold = 0.1f;
    public float TriggerFiringThreshold = 0.8f;
    private float m_triggerFloat;
    private bool m_hasTriggerCycled;
    private bool m_isMagReleasePressed;
    public Transform Trigger_Display;
    public float Trigger_ForwardValue;
    public float Trigger_RearwardValue;
    public FVRPhysicalObject.InterpStyle TriggerInterpStyle = FVRPhysicalObject.InterpStyle.Rotation;
    public Transform Trigger_Display2;
    public float Trigger_ForwardValue2;
    public float Trigger_RearwardValue2;
    public FVRPhysicalObject.InterpStyle TriggerInterpStyle2 = FVRPhysicalObject.InterpStyle.Rotation;
    public Transform MagReleaseButton_Display;
    public FVRPhysicalObject.Axis MagReleaseAxis;
    public FVRPhysicalObject.InterpStyle MagReleaseInterpStyle = FVRPhysicalObject.InterpStyle.Rotation;
    public float MagReleasePressedValue;
    public float MagReleaseUnpressedValue;
    private float m_magReleaseCurValue;
    private float m_magReleaseTarValue;
    private Vector2 TouchPadAxes = Vector2.zero;
    public Transform FireSelector_Display;
    public FVRPhysicalObject.Axis FireSelector_Axis;
    public FVRPhysicalObject.InterpStyle FireSelector_InterpStyle = FVRPhysicalObject.InterpStyle.Rotation;
    public BoltActionRifle.FireSelectorMode[] FireSelector_Modes;
    private int m_fireSelectorMode;
    public bool RequiresHammerUncockedToToggleFireSelector;
    public bool UsesSecondFireSelectorChange;
    public Transform FireSelector_Display_Secondary;
    public FVRPhysicalObject.Axis FireSelector_Axis_Secondary;
    public FVRPhysicalObject.InterpStyle FireSelector_InterpStyle_Secondary = FVRPhysicalObject.InterpStyle.Rotation;
    public BoltActionRifle.FireSelectorMode[] FireSelector_Modes_Secondary;
    [Header("Special Features")]
    public bool EjectsMagazineOnEmpty;
    public bool PlaysExtraTailOnShot;
    public FVRTailSoundClass ExtraTail = FVRTailSoundClass.Explosion;
    [Header("Reciprocating Barrel")]
    public bool HasReciprocatingBarrel;
    public G11RecoilingSystem RecoilSystem;
    private bool m_isQuickboltTouching;
    private Vector2 lastTPTouchPoint = Vector2.zero;

    public bool IsHammerCocked => this.m_isHammerCocked;

    public bool HasExtractedRound() => this.m_proxy.IsFull;

    protected override void Awake()
    {
      base.Awake();
      if (this.UsesClips && (UnityEngine.Object) this.ClipTrigger != (UnityEngine.Object) null)
      {
        if (this.CurBoltHandleState == BoltActionRifle_Handle.BoltActionHandleState.Rear)
        {
          if (!this.ClipTrigger.activeSelf)
            this.ClipTrigger.SetActive(true);
        }
        else if (this.ClipTrigger.activeSelf)
          this.ClipTrigger.SetActive(false);
      }
      this.m_proxy = new GameObject("m_proxyRound").AddComponent<FVRFirearmMovingProxyRound>();
      this.m_proxy.Init(this.transform);
    }

    public bool CanBoltMove() => this.FireSelector_Modes.Length < 1 || !this.FireSelector_Modes[this.m_fireSelectorMode].IsBoltLocked;

    public override int GetTutorialState()
    {
      if (this.FireSelector_Modes[this.m_fireSelectorMode].ModeType == BoltActionRifle.FireSelectorModeType.Safe)
        return 4;
      if (this.Chamber.IsFull)
      {
        if (this.Chamber.IsSpent)
          return 0;
        return (UnityEngine.Object) this.AltGrip != (UnityEngine.Object) null ? 6 : 5;
      }
      if (!((UnityEngine.Object) this.Magazine != (UnityEngine.Object) null) || this.Magazine.HasARound())
        return 3;
      if (this.CurBoltHandleState == BoltActionRifle_Handle.BoltActionHandleState.Forward || this.CurBoltHandleState == BoltActionRifle_Handle.BoltActionHandleState.Mid)
        return 0;
      if ((UnityEngine.Object) this.Clip != (UnityEngine.Object) null)
        return 2;
      return !this.Magazine.IsFull() ? 1 : 3;
    }

    public override void BeginInteraction(FVRViveHand hand) => base.BeginInteraction(hand);

    public override void UpdateInteraction(FVRViveHand hand)
    {
      this.TouchPadAxes = hand.Input.TouchpadAxes;
      if (this.m_hasTriggeredUpSinceBegin)
        this.m_triggerFloat = hand.Input.TriggerFloat;
      if (!this.m_hasTriggerCycled)
      {
        if ((double) this.m_triggerFloat >= (double) this.TriggerFiringThreshold)
          this.m_hasTriggerCycled = true;
      }
      else if ((double) this.m_triggerFloat <= (double) this.TriggerResetThreshold)
        this.m_hasTriggerCycled = false;
      this.m_isMagReleasePressed = false;
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      if (hand.IsInStreamlinedMode)
      {
        if (this.HasFireSelectorButton && hand.Input.BYButtonDown)
          this.ToggleFireSelector();
        if (this.HasMagEjectionButton && hand.Input.AXButtonPressed)
          this.m_isMagReleasePressed = true;
      }
      else
      {
        if (this.HasMagEjectionButton && hand.Input.TouchpadPressed && (this.m_hasTriggeredUpSinceBegin && (double) this.TouchPadAxes.magnitude > 0.300000011920929) && (double) Vector2.Angle(this.TouchPadAxes, Vector2.down) <= 45.0)
          this.m_isMagReleasePressed = true;
        if (GM.Options.QuickbeltOptions.BoltActionModeSetting == QuickbeltOptions.BoltActionMode.Quickbolting)
          flag3 = true;
        if (GM.Options.QuickbeltOptions.BoltActionModeSetting == QuickbeltOptions.BoltActionMode.Slidebolting)
          flag2 = true;
        if (GM.Options.ControlOptions.UseGunRigMode2)
        {
          flag2 = true;
          flag3 = false;
        }
        if ((UnityEngine.Object) this.Bipod != (UnityEngine.Object) null && this.Bipod.IsBipodActive)
        {
          flag2 = true;
          flag3 = false;
        }
        if (!this.CanBoltMove())
        {
          flag2 = false;
          flag3 = false;
        }
        if (this.IsHammerCocked && this.BoltHandle.HandleState == BoltActionRifle_Handle.BoltActionHandleState.Forward && this.BoltHandle.HandleRot == BoltActionRifle_Handle.BoltActionHandleRot.Down)
          flag2 = false;
        if (hand.Input.TouchpadDown && (double) this.TouchPadAxes.magnitude > 0.100000001490116)
        {
          if (flag3 && (double) Vector2.Angle(this.TouchPadAxes, Vector2.right) <= 45.0 && (this.BoltHandle.UsesQuickRelease && this.BoltHandle.HandleState == BoltActionRifle_Handle.BoltActionHandleState.Forward))
            flag1 = true;
          else if ((double) Vector2.Angle(this.TouchPadAxes, Vector2.left) <= 45.0 && this.HasFireSelectorButton)
            this.ToggleFireSelector();
        }
      }
      if (this.m_isMagReleasePressed)
      {
        this.ReleaseMag();
        if ((UnityEngine.Object) this.ReloadTriggerWell != (UnityEngine.Object) null)
          this.ReloadTriggerWell.SetActive(false);
      }
      else if ((UnityEngine.Object) this.ReloadTriggerWell != (UnityEngine.Object) null)
        this.ReloadTriggerWell.SetActive(true);
      if (this.m_hasTriggeredUpSinceBegin && !flag1 && flag2)
      {
        if ((UnityEngine.Object) this.AltGrip != (UnityEngine.Object) null && !this.IsAltHeld || GM.Options.ControlOptions.UseGunRigMode2 || (UnityEngine.Object) this.Bipod != (UnityEngine.Object) null && this.Bipod.IsBipodActive)
        {
          if (hand.Input.TouchpadTouched)
          {
            Vector2 touchpadAxes = hand.Input.TouchpadAxes;
            if ((double) touchpadAxes.magnitude > 0.100000001490116)
            {
              bool quickboltTouching = this.m_isQuickboltTouching;
              if ((double) Vector2.Angle(touchpadAxes, Vector2.right + Vector2.up) < 90.0 && !this.m_isQuickboltTouching)
                this.m_isQuickboltTouching = true;
              if (this.m_isQuickboltTouching && quickboltTouching)
                this.BoltHandle.DriveBolt((float) (-(double) this.GetSAngle(touchpadAxes, this.lastTPTouchPoint, hand.CMode) / 90.0));
              this.lastTPTouchPoint = touchpadAxes;
            }
            else
              this.lastTPTouchPoint = Vector2.zero;
          }
          else
            this.lastTPTouchPoint = Vector2.zero;
        }
        if (this.m_isQuickboltTouching)
          Debug.DrawLine(this.BoltHandle.BoltActionHandleRoot.transform.position, this.BoltHandle.BoltActionHandleRoot.transform.position + 0.1f * new Vector3(this.lastTPTouchPoint.x, this.lastTPTouchPoint.y, 0.0f), Color.blue);
      }
      if (hand.Input.TouchpadTouchUp)
      {
        this.m_isQuickboltTouching = false;
        this.lastTPTouchPoint = Vector2.zero;
      }
      this.FiringSystem();
      base.UpdateInteraction(hand);
      if (!flag1 || this.IsAltHeld || !((UnityEngine.Object) this.AltGrip != (UnityEngine.Object) null))
        return;
      this.m_isQuickboltTouching = false;
      this.lastTPTouchPoint = Vector2.zero;
      hand.Buzz(hand.Buzzer.Buzz_BeginInteraction);
      hand.HandMadeGrabReleaseSound();
      hand.EndInteractionIfHeld((FVRInteractiveObject) this);
      this.EndInteraction(hand);
      this.BoltHandle.BeginInteraction(hand);
      hand.ForceSetInteractable((FVRInteractiveObject) this.BoltHandle);
      this.BoltHandle.TPInitiate();
    }

    public float GetSignedAngle(Vector2 from, Vector2 to)
    {
      Vector2 normalized = new Vector2(from.y, -from.x).normalized;
      float num = Mathf.Sign(Vector2.Dot(from, normalized));
      return Vector2.Angle(from, to) * num;
    }

    private float GetSAngle(Vector2 v1, Vector2 v2, ControlMode m)
    {
      if (m == ControlMode.Index)
        return (float) (((double) v1.y - (double) v2.y) * 130.0);
      float num = Mathf.Sign((float) ((double) v1.x * (double) v2.y - (double) v1.y * (double) v2.x));
      return Vector2.Angle(v1, v2) * num;
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      this.m_triggerFloat = 0.0f;
      this.m_hasTriggerCycled = false;
      this.m_isMagReleasePressed = false;
      this.m_isQuickboltTouching = false;
      this.lastTPTouchPoint = Vector2.zero;
      base.EndInteraction(hand);
    }

    public void SetHasTriggeredUp() => this.m_hasTriggeredUpSinceBegin = true;

    public void CockHammer()
    {
      if (this.m_isHammerCocked)
        return;
      this.m_isHammerCocked = true;
      if (!this.HasVisualHammer)
        return;
      this.SetAnimatedComponent(this.Hammer, this.HammerCocked, FVRPhysicalObject.InterpStyle.Translate, FVRPhysicalObject.Axis.Z);
    }

    public void DropHammer()
    {
      if (!this.IsHammerCocked)
        return;
      this.m_isHammerCocked = false;
      this.PlayAudioEvent(FirearmAudioEventType.HammerHit);
      this.Fire();
      if (!this.HasVisualHammer)
        return;
      this.SetAnimatedComponent(this.Hammer, this.HammerUncocked, FVRPhysicalObject.InterpStyle.Translate, FVRPhysicalObject.Axis.Z);
    }

    protected virtual void ToggleFireSelector()
    {
      if (this.RequiresHammerUncockedToToggleFireSelector && this.IsHammerCocked || this.FireSelector_Modes.Length <= 1)
        return;
      ++this.m_fireSelectorMode;
      if (this.m_fireSelectorMode >= this.FireSelector_Modes.Length)
        this.m_fireSelectorMode -= this.FireSelector_Modes.Length;
      this.PlayAudioEvent(FirearmAudioEventType.FireSelector);
      if (!((UnityEngine.Object) this.FireSelector_Display != (UnityEngine.Object) null))
        return;
      this.SetAnimatedComponent(this.FireSelector_Display, this.FireSelector_Modes[this.m_fireSelectorMode].SelectorPosition, this.FireSelector_InterpStyle, this.FireSelector_Axis);
      if (!this.UsesSecondFireSelectorChange)
        return;
      this.SetAnimatedComponent(this.FireSelector_Display_Secondary, this.FireSelector_Modes_Secondary[this.m_fireSelectorMode].SelectorPosition, this.FireSelector_InterpStyle_Secondary, this.FireSelector_Axis_Secondary);
    }

    public void ReleaseMag()
    {
      if (!((UnityEngine.Object) this.Magazine != (UnityEngine.Object) null))
        return;
      this.m_magReleaseCurValue = this.MagReleasePressedValue;
      this.EjectMag();
    }

    public BoltActionRifle.FireSelectorMode GetFiringMode() => this.FireSelector_Modes[this.m_fireSelectorMode];

    protected virtual void FiringSystem()
    {
      if (this.FireSelector_Modes[this.m_fireSelectorMode].ModeType == BoltActionRifle.FireSelectorModeType.Safe || this.IsAltHeld || (this.BoltHandle.HandleState != BoltActionRifle_Handle.BoltActionHandleState.Forward || this.BoltHandle.HandleRot == BoltActionRifle_Handle.BoltActionHandleRot.Up) || !this.m_hasTriggerCycled)
        return;
      this.DropHammer();
    }

    public bool Fire()
    {
      BoltActionRifle.FireSelectorMode fireSelectorMode = this.FireSelector_Modes[this.m_fireSelectorMode];
      if (!this.Chamber.Fire())
        return false;
      this.Fire(this.Chamber, this.GetMuzzle(), true);
      this.FireMuzzleSmoke();
      this.Recoil(this.IsTwoHandStabilized(), this.IsForegripStabilized(), this.IsShoulderStabilized());
      FVRSoundEnvironment soundEnvironment = GM.CurrentPlayerBody.GetCurrentSoundEnvironment();
      this.PlayAudioGunShot(this.Chamber.GetRound(), soundEnvironment);
      if (this.PlaysExtraTailOnShot)
      {
        AudioEvent tailSet = SM.GetTailSet(this.ExtraTail, soundEnvironment);
        this.m_pool_tail.PlayClipVolumePitchOverride(tailSet, this.transform.position, tailSet.VolumeRange * 1f, this.AudioClipSet.TailPitchMod_Main * tailSet.PitchRange.x);
      }
      if (this.HasReciprocatingBarrel)
        this.RecoilSystem.Recoil(false);
      return true;
    }

    protected override void FVRFixedUpdate()
    {
      base.FVRFixedUpdate();
      this.UpdateComponentDisplay();
    }

    private void UpdateComponentDisplay()
    {
      if ((UnityEngine.Object) this.Trigger_Display != (UnityEngine.Object) null)
      {
        if (this.TriggerInterpStyle == FVRPhysicalObject.InterpStyle.Translate)
          this.Trigger_Display.localPosition = new Vector3(0.0f, 0.0f, Mathf.Lerp(this.Trigger_ForwardValue, this.Trigger_RearwardValue, this.m_triggerFloat));
        else if (this.TriggerInterpStyle == FVRPhysicalObject.InterpStyle.Rotation)
          this.Trigger_Display.localEulerAngles = new Vector3(Mathf.Lerp(this.Trigger_ForwardValue, this.Trigger_RearwardValue, this.m_triggerFloat), 0.0f, 0.0f);
      }
      if ((UnityEngine.Object) this.Trigger_Display2 != (UnityEngine.Object) null)
      {
        if (this.TriggerInterpStyle == FVRPhysicalObject.InterpStyle.Translate)
          this.Trigger_Display2.localPosition = new Vector3(0.0f, 0.0f, Mathf.Lerp(this.Trigger_ForwardValue2, this.Trigger_RearwardValue2, this.m_triggerFloat));
        else if (this.TriggerInterpStyle == FVRPhysicalObject.InterpStyle.Rotation)
          this.Trigger_Display2.localEulerAngles = new Vector3(Mathf.Lerp(this.Trigger_ForwardValue2, this.Trigger_RearwardValue2, this.m_triggerFloat), 0.0f, 0.0f);
      }
      if ((UnityEngine.Object) this.MagReleaseButton_Display != (UnityEngine.Object) null)
      {
        Vector3 zero = Vector3.zero;
        this.m_magReleaseTarValue = !this.m_isMagReleasePressed ? this.MagReleaseUnpressedValue : this.MagReleasePressedValue;
        this.m_magReleaseCurValue = Mathf.Lerp(this.m_magReleaseCurValue, this.m_magReleaseTarValue, Time.deltaTime * 4f);
        float magReleaseCurValue = this.m_magReleaseCurValue;
        switch (this.MagReleaseAxis)
        {
          case FVRPhysicalObject.Axis.X:
            zero.x = magReleaseCurValue;
            break;
          case FVRPhysicalObject.Axis.Y:
            zero.y = magReleaseCurValue;
            break;
          case FVRPhysicalObject.Axis.Z:
            zero.z = magReleaseCurValue;
            break;
        }
        switch (this.MagReleaseInterpStyle)
        {
          case FVRPhysicalObject.InterpStyle.Translate:
            this.MagReleaseButton_Display.localPosition = zero;
            break;
          case FVRPhysicalObject.InterpStyle.Rotation:
            this.MagReleaseButton_Display.localEulerAngles = zero;
            break;
        }
      }
      if (this.CurBoltHandleState == BoltActionRifle_Handle.BoltActionHandleState.Forward)
        this.IsBreachOpenForGasOut = false;
      else
        this.IsBreachOpenForGasOut = true;
    }

    public void UpdateBolt(BoltActionRifle_Handle.BoltActionHandleState State, float lerp)
    {
      this.CurBoltHandleState = State;
      this.BoltLerp = lerp;
      this.Chamber.IsAccessible = this.CurBoltHandleState != BoltActionRifle_Handle.BoltActionHandleState.Forward && !this.m_proxy.IsFull && !this.Chamber.IsFull;
      if (this.UsesClips && (UnityEngine.Object) this.ClipTrigger != (UnityEngine.Object) null)
      {
        if (this.CurBoltHandleState == BoltActionRifle_Handle.BoltActionHandleState.Rear)
        {
          if (!this.ClipTrigger.activeSelf)
            this.ClipTrigger.SetActive(true);
        }
        else if (this.ClipTrigger.activeSelf)
          this.ClipTrigger.SetActive(false);
      }
      if (this.CurBoltHandleState == BoltActionRifle_Handle.BoltActionHandleState.Rear && this.LastBoltHandleState != BoltActionRifle_Handle.BoltActionHandleState.Rear)
      {
        if (this.CockType == BoltActionRifle.HammerCockType.OnBack)
          this.CockHammer();
        if (this.Chamber.IsFull)
        {
          this.Chamber.EjectRound(this.EjectionPos.position, this.transform.right * this.RightwardEjectionForce + this.transform.up * this.UpwardEjectionForce, this.transform.up * this.YSpinEjectionTorque);
          this.PlayAudioEvent(FirearmAudioEventType.HandleBack);
        }
        else
          this.PlayAudioEvent(FirearmAudioEventType.HandleBackEmpty);
        this.BoltMovingForward = true;
      }
      else if (this.CurBoltHandleState == BoltActionRifle_Handle.BoltActionHandleState.Forward && this.LastBoltHandleState != BoltActionRifle_Handle.BoltActionHandleState.Forward)
      {
        if (this.CockType == BoltActionRifle.HammerCockType.OnForward)
          this.CockHammer();
        if (this.m_proxy.IsFull && !this.Chamber.IsFull)
        {
          this.Chamber.SetRound(this.m_proxy.Round);
          this.m_proxy.ClearProxy();
          this.PlayAudioEvent(FirearmAudioEventType.HandleForward);
        }
        else
          this.PlayAudioEvent(FirearmAudioEventType.HandleForwardEmpty);
        this.BoltMovingForward = false;
      }
      else if (this.CurBoltHandleState == BoltActionRifle_Handle.BoltActionHandleState.Mid && this.LastBoltHandleState == BoltActionRifle_Handle.BoltActionHandleState.Rear && (UnityEngine.Object) this.Magazine != (UnityEngine.Object) null)
      {
        if (!this.m_proxy.IsFull && this.Magazine.HasARound() && !this.Chamber.IsFull)
          this.m_proxy.SetFromPrefabReference(this.Magazine.RemoveRound(false));
        if (this.EjectsMagazineOnEmpty && !this.Magazine.HasARound())
          this.EjectMag();
      }
      if (this.m_proxy.IsFull)
      {
        this.m_proxy.ProxyRound.position = Vector3.Lerp(this.Extraction_ChamberPos.position, this.Extraction_MagazinePos.position, this.BoltLerp);
        this.m_proxy.ProxyRound.rotation = Quaternion.Slerp(this.Extraction_ChamberPos.rotation, this.Extraction_MagazinePos.rotation, this.BoltLerp);
      }
      if (this.Chamber.IsFull)
      {
        this.Chamber.ProxyRound.position = Vector3.Lerp(this.Extraction_ChamberPos.position, this.Extraction_Ejecting.position, this.BoltLerp);
        this.Chamber.ProxyRound.rotation = Quaternion.Slerp(this.Extraction_ChamberPos.rotation, this.Extraction_Ejecting.rotation, this.BoltLerp);
      }
      this.LastBoltHandleState = this.CurBoltHandleState;
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
      if (f.ContainsKey(key1))
      {
        if (f[key1] == "Cocked")
          this.m_isHammerCocked = true;
        if (this.HasVisualHammer)
          this.SetAnimatedComponent(this.Hammer, this.HammerCocked, FVRPhysicalObject.InterpStyle.Translate, FVRPhysicalObject.Axis.Z);
      }
      if (this.FireSelector_Modes.Length <= 1)
        return;
      string key2 = "FireSelectorState";
      if (f.ContainsKey(key2))
        int.TryParse(f[key2], out this.m_fireSelectorMode);
      if (!((UnityEngine.Object) this.FireSelector_Display != (UnityEngine.Object) null))
        return;
      this.SetAnimatedComponent(this.FireSelector_Display, this.FireSelector_Modes[this.m_fireSelectorMode].SelectorPosition, this.FireSelector_InterpStyle, this.FireSelector_Axis);
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

    public enum ZPos
    {
      Forward,
      Middle,
      Rear,
    }

    public enum HammerCockType
    {
      OnBack,
      OnUp,
      OnClose,
      OnForward,
    }

    public enum FireSelectorModeType
    {
      Safe,
      Single,
    }

    [Serializable]
    public class FireSelectorMode
    {
      public float SelectorPosition;
      public BoltActionRifle.FireSelectorModeType ModeType;
      public bool IsBoltLocked;
    }
  }
}
