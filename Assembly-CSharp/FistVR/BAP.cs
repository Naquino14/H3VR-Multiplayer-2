// Decompiled with JetBrains decompiler
// Type: FistVR.BAP
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class BAP : FVRFireArm
  {
    [Header("BoltActionRifle Config")]
    public FVRFireArmChamber Chamber;
    public BAPHandle Handle;
    public Transform Muzzle;
    public GameObject ReloadTriggerWell;
    public bool HasMagEjectionButton = true;
    private FVRFirearmMovingProxyRound m_proxy;
    public Transform Extraction_MagazinePos;
    public Transform Extraction_ChamberPos;
    public Transform Extraction_Ejecting;
    public Transform EjectionPos;
    public float UpwardEjectionForce;
    public float RightwardEjectionForce = 2f;
    public float YSpinEjectionTorque = 80f;
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
    public Transform MagReleaseButton_Display;
    public FVRPhysicalObject.Axis MagReleaseAxis;
    public FVRPhysicalObject.InterpStyle MagReleaseInterpStyle = FVRPhysicalObject.InterpStyle.Rotation;
    public float MagReleasePressedValue;
    public float MagReleaseUnpressedValue;
    private float m_magReleaseCurValue;
    private float m_magReleaseTarValue;
    private Vector2 TouchPadAxes = Vector2.zero;
    public bool HasFireSelectorButton;
    public Transform FireSelector_Display;
    public FVRPhysicalObject.Axis FireSelector_Axis;
    public FVRPhysicalObject.InterpStyle FireSelector_InterpStyle = FVRPhysicalObject.InterpStyle.Rotation;
    public BAP.FireSelectorMode[] FireSelector_Modes;
    private int m_fireSelectorMode;
    public bool RequiresHammerUncockedToToggleFireSelector;
    [Header("Baffle")]
    public GameObject BaffleRoot;
    public List<GameObject> BaffleStates;
    public GameObject Cap;
    public bool HasCap = true;
    public bool HasBaffle = true;
    public int BaffleState;
    public GameObject Prefab_Baffle;
    public GameObject Prefab_Cap;
    public Transform PPoint_Baffle;
    public Transform PPoint_Cap;
    private float m_timeTilCanDetectPiece = 1f;
    public AudioEvent AudClip_InsertCap;
    public AudioEvent AudClip_RemoveCap;
    public AudioEvent AudClip_InsertBaffle;
    public AudioEvent AudClip_RemoveBaffle;
    private bool m_isHammerCocked;
    private bool BoltMovingForward;

    public bool HasExtractedRound() => this.m_proxy.IsFull;

    public void RemoveThing() => this.m_timeTilCanDetectPiece = 1f;

    public bool CanDetectPiece() => (double) this.m_timeTilCanDetectPiece <= 0.0;

    public void SetCapState(bool hasC)
    {
      if (hasC && !this.HasCap)
        this.PlayAudioAsHandling(this.AudClip_InsertCap, this.Cap.transform.position);
      else if (!hasC && this.HasCap)
        this.PlayAudioAsHandling(this.AudClip_RemoveCap, this.Cap.transform.position);
      this.HasCap = hasC;
      this.Cap.SetActive(this.HasCap);
    }

    public void SetBaffleState(bool hasB, int s)
    {
      this.BaffleState = s;
      if (hasB && !this.HasBaffle)
        this.PlayAudioAsHandling(this.AudClip_InsertBaffle, this.Cap.transform.position);
      else if (!hasB && this.HasBaffle)
        this.PlayAudioAsHandling(this.AudClip_RemoveBaffle, this.Cap.transform.position);
      this.HasBaffle = hasB;
      this.BaffleRoot.SetActive(this.HasBaffle);
      for (int index = 0; index < this.BaffleStates.Count; ++index)
      {
        if (index == this.BaffleState)
          this.BaffleStates[index].SetActive(true);
        else
          this.BaffleStates[index].SetActive(false);
      }
      if (!this.HasBaffle)
        this.DefaultMuzzleState = FVRFireArm.MuzzleState.None;
      else
        this.DefaultMuzzleState = FVRFireArm.MuzzleState.Suppressor;
    }

    protected override void Awake()
    {
      base.Awake();
      this.m_proxy = new GameObject("m_proxyRound").AddComponent<FVRFirearmMovingProxyRound>();
      this.m_proxy.Init(this.transform);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      this.TouchPadAxes = hand.Input.TouchpadAxes;
      this.m_triggerFloat = !this.m_hasTriggeredUpSinceBegin || this.IsAltHeld ? 0.0f : hand.Input.TriggerFloat;
      if (!this.m_hasTriggerCycled)
      {
        if ((double) this.m_triggerFloat >= (double) this.TriggerFiringThreshold)
          this.m_hasTriggerCycled = true;
      }
      else if ((double) this.m_triggerFloat <= (double) this.TriggerResetThreshold)
        this.m_hasTriggerCycled = false;
      this.m_isMagReleasePressed = false;
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
        if (hand.Input.TouchpadDown && (double) this.TouchPadAxes.magnitude > 0.25 && ((double) Vector2.Angle(this.TouchPadAxes, Vector2.left) <= 45.0 && this.HasFireSelectorButton))
          this.ToggleFireSelector();
      }
      if (this.m_isMagReleasePressed)
      {
        this.ReleaseMag();
        if ((UnityEngine.Object) this.ReloadTriggerWell != (UnityEngine.Object) null)
          this.ReloadTriggerWell.SetActive(false);
      }
      else if ((UnityEngine.Object) this.ReloadTriggerWell != (UnityEngine.Object) null)
        this.ReloadTriggerWell.SetActive(true);
      this.FiringSystem();
      base.UpdateInteraction(hand);
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      this.m_triggerFloat = 0.0f;
      this.m_hasTriggerCycled = false;
      this.m_isMagReleasePressed = false;
      base.EndInteraction(hand);
    }

    protected override void FVRFixedUpdate()
    {
      base.FVRFixedUpdate();
      this.UpdateComponentDisplay();
      if ((double) this.m_timeTilCanDetectPiece > 0.0)
        this.m_timeTilCanDetectPiece -= Time.deltaTime;
      if (this.HasCap || !this.HasBaffle || (double) Vector3.Angle(this.Muzzle.forward, Vector3.down) >= 45.0)
        return;
      this.RemoveThing();
      BAPBaffle component = UnityEngine.Object.Instantiate<GameObject>(this.Prefab_Baffle, this.PPoint_Baffle.position, this.PPoint_Baffle.rotation).GetComponent<BAPBaffle>();
      component.SetState(this.BaffleState);
      component.RootRigidbody.velocity = this.Muzzle.forward * 2.5f;
      this.SetBaffleState(false, 0);
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
      if (this.Handle.CurHandleSlideState == BAPHandle.HandleSlideState.Forward)
        this.IsBreachOpenForGasOut = false;
      else
        this.IsBreachOpenForGasOut = true;
    }

    public void CockHammer()
    {
      if (this.m_isHammerCocked)
        return;
      this.m_isHammerCocked = true;
    }

    public void DropHammer()
    {
      if (!this.m_isHammerCocked)
        return;
      this.m_isHammerCocked = false;
      this.PlayAudioEvent(FirearmAudioEventType.HammerHit);
      this.Fire();
    }

    private void ToggleFireSelector()
    {
      if (this.FireSelector_Modes.Length <= 1)
        return;
      ++this.m_fireSelectorMode;
      if (this.m_fireSelectorMode >= this.FireSelector_Modes.Length)
        this.m_fireSelectorMode -= this.FireSelector_Modes.Length;
      this.PlayAudioEvent(FirearmAudioEventType.FireSelector);
      if (!((UnityEngine.Object) this.FireSelector_Display != (UnityEngine.Object) null))
        return;
      this.SetAnimatedComponent(this.FireSelector_Display, this.FireSelector_Modes[this.m_fireSelectorMode].SelectorPosition, this.FireSelector_InterpStyle, this.FireSelector_Axis);
    }

    public void ReleaseMag()
    {
      if (!((UnityEngine.Object) this.Magazine != (UnityEngine.Object) null))
        return;
      this.m_magReleaseCurValue = this.MagReleasePressedValue;
      this.EjectMag();
    }

    protected virtual void FiringSystem()
    {
      if (this.FireSelector_Modes[this.m_fireSelectorMode].ModeType == BAP.FireSelectorModeType.Safe || this.IsAltHeld || (this.Handle.CurHandleRotState == BAPHandle.HandleRotState.Open || this.Handle.CurHandleSlideState != BAPHandle.HandleSlideState.Forward) || !this.m_hasTriggerCycled)
        return;
      this.DropHammer();
    }

    public bool Fire()
    {
      BAP.FireSelectorMode fireSelectorMode = this.FireSelector_Modes[this.m_fireSelectorMode];
      if (!this.Chamber.Fire())
        return false;
      this.Fire(this.Chamber, this.GetMuzzle(), true);
      this.FireMuzzleSmoke();
      this.Recoil(this.IsTwoHandStabilized(), this.IsForegripStabilized(), this.IsShoulderStabilized());
      if (!this.HasCap && this.HasBaffle)
      {
        this.RemoveThing();
        BAPBaffle component = UnityEngine.Object.Instantiate<GameObject>(this.Prefab_Baffle, this.PPoint_Baffle.position, this.PPoint_Baffle.rotation).GetComponent<BAPBaffle>();
        component.SetState(this.BaffleState);
        component.RootRigidbody.velocity = this.Muzzle.forward * 2.5f;
        this.SetBaffleState(false, 0);
      }
      this.PlayWellrodGunShot(this.Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
      ++this.BaffleState;
      if (this.BaffleState > 10)
        this.BaffleState = 10;
      this.SetBaffleState(this.HasBaffle, this.BaffleState);
      return true;
    }

    public void PlayWellrodGunShot(
      FVRFireArmRound round,
      FVRSoundEnvironment TailEnvironment,
      float globalLoudnessMultiplier = 1f)
    {
      FVRTailSoundClass tailClass = FVRTailSoundClass.Tiny;
      Vector3 position = this.transform.position;
      float num1 = (float) this.BaffleState * 0.1f;
      float num2 = 1f;
      if (this.HasBaffle)
      {
        this.m_pool_shot.PlayClipVolumePitchOverride(this.AudioClipSet.Shots_Suppressed, this.GetMuzzle().position, new Vector2(this.AudioClipSet.Shots_Suppressed.VolumeRange.x * num1, this.AudioClipSet.Shots_Suppressed.VolumeRange.y * num1), new Vector2(this.AudioClipSet.Shots_Suppressed.PitchRange.x * num2, this.AudioClipSet.Shots_Suppressed.PitchRange.y * num2));
        if (this.IsHeld)
        {
          this.m_hand.ForceTubeKick(this.AudioClipSet.FTP.Kick_Shot);
          this.m_hand.ForceTubeRumble(this.AudioClipSet.FTP.Rumble_Shot_Intensity, this.AudioClipSet.FTP.Rumble_Shot_Duration);
        }
        if (this.AudioClipSet.UsesTail_Suppressed)
        {
          tailClass = round.TailClassSuppressed;
          AudioEvent tailSet = SM.GetTailSet(round.TailClassSuppressed, TailEnvironment);
          this.m_pool_tail.PlayClipVolumePitchOverride(tailSet, position, tailSet.VolumeRange * globalLoudnessMultiplier * num1, this.AudioClipSet.TailPitchMod_Suppressed * tailSet.PitchRange.x);
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
      if (this.HasBaffle)
        GM.CurrentSceneSettings.OnPerceiveableSound(this.AudioClipSet.Loudness_Suppressed * num1, (float) ((double) this.AudioClipSet.Loudness_Suppressed * (double) multByEnvironment * 0.5) * globalLoudnessMultiplier * num1, this.transform.position, playerIff);
      else
        GM.CurrentSceneSettings.OnPerceiveableSound(this.AudioClipSet.Loudness_Primary, this.AudioClipSet.Loudness_Primary * multByEnvironment * globalLoudnessMultiplier, this.transform.position, playerIff);
      if (!this.HasBaffle)
        this.SceneSettings.PingReceivers(this.MuzzlePos.position);
      for (int index = 0; index < this.MuzzleDevices.Count; ++index)
        this.MuzzleDevices[index].OnShot((FVRFireArm) this, tailClass);
    }

    public void UpdateBolt(float BoltLerp)
    {
      this.Chamber.IsAccessible = this.Handle.CurHandleSlideState != BAPHandle.HandleSlideState.Forward && !this.m_proxy.IsFull && !this.Chamber.IsFull;
      if (this.Handle.CurHandleSlideState == BAPHandle.HandleSlideState.Forward && this.Handle.LastHandleSlideState != BAPHandle.HandleSlideState.Forward)
      {
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
      else if (this.Handle.CurHandleSlideState == BAPHandle.HandleSlideState.Rear && this.Handle.LastHandleSlideState != BAPHandle.HandleSlideState.Rear)
      {
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
      else if (this.Handle.CurHandleSlideState == BAPHandle.HandleSlideState.Mid && this.Handle.LastHandleSlideState == BAPHandle.HandleSlideState.Rear && ((UnityEngine.Object) this.Magazine != (UnityEngine.Object) null && !this.m_proxy.IsFull) && (this.Magazine.HasARound() && !this.Chamber.IsFull))
        this.m_proxy.SetFromPrefabReference(this.Magazine.RemoveRound(false));
      if (this.m_proxy.IsFull)
      {
        this.m_proxy.ProxyRound.position = Vector3.Lerp(this.Extraction_ChamberPos.position, this.Extraction_MagazinePos.position, BoltLerp);
        this.m_proxy.ProxyRound.rotation = Quaternion.Slerp(this.Extraction_ChamberPos.rotation, this.Extraction_MagazinePos.rotation, BoltLerp);
      }
      if (!this.Chamber.IsFull)
        return;
      this.Chamber.ProxyRound.position = Vector3.Lerp(this.Extraction_ChamberPos.position, this.Extraction_Ejecting.position, BoltLerp);
      this.Chamber.ProxyRound.rotation = Quaternion.Slerp(this.Extraction_ChamberPos.rotation, this.Extraction_Ejecting.rotation, BoltLerp);
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

    public enum FireSelectorModeType
    {
      Safe,
      Single,
    }

    [Serializable]
    public class FireSelectorMode
    {
      public float SelectorPosition;
      public BAP.FireSelectorModeType ModeType;
      public bool IsBoltLocked;
    }
  }
}
