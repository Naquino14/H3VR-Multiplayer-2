// Decompiled with JetBrains decompiler
// Type: FistVR.LeverActionFirearm
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class LeverActionFirearm : FVRFireArm
  {
    [Header("LeverAction Config")]
    public FVRFireArmChamber Chamber;
    public bool UsesSecondChamber;
    public FVRFireArmChamber Chamber2;
    public Transform SecondEjectionSpot;
    public Transform SecondMuzzle;
    private bool m_isHammerCocked2;
    private bool m_isSecondaryMuzzlePos;
    public Transform Lever;
    public Transform LeverRoot;
    public Transform Hammer;
    public Transform LoadingGate;
    public Transform Trigger;
    public Vector2 TriggerRotRange;
    public FVRAlternateGrip ForeGrip;
    public Vector2 LeverAngleRange = new Vector2(-68f, 0.0f);
    public Vector2 HammerAngleRange = new Vector2(-36f, 0.0f);
    public Vector2 LoadingGateAngleRange = new Vector2(-24f, 0.0f);
    public Vector3 EjectionDir = new Vector3(0.0f, 2f, 0.0f);
    public Vector3 EjectionSpin = new Vector3(80f, 0.0f, 0.0f);
    private bool m_isLeverReleasePressed;
    private float m_curLeverRot;
    private float m_tarLeverRot;
    private float m_leverRotSpeed = 700f;
    private bool m_isActionMovingForward;
    private LeverActionFirearm.ZPos m_curLeverPos = LeverActionFirearm.ZPos.Rear;
    private LeverActionFirearm.ZPos m_lastLeverPos = LeverActionFirearm.ZPos.Rear;
    private FVRFirearmMovingProxyRound m_proxy;
    private FVRFirearmMovingProxyRound m_proxy2;
    [Header("Round Positions Config")]
    public Transform ReceiverLowerPathForward;
    public Transform ReceiverLowerPathRearward;
    public Transform ReceiverUpperPathForward;
    public Transform ReceiverUpperPathRearward;
    public Transform ReceiverEjectionPathForward;
    public Transform ReceiverEjectionPathRearward;
    public Transform ReceiverEjectionPoint;
    private bool m_isHammerCocked;
    [Header("Spinning Config")]
    public Transform PoseSpinHolder;
    public bool CanSpin;
    private bool m_isSpinning;
    public LeverActionFirearm.LeverActuatedPiece[] ActuatedPieces;
    private bool useLinearRacking = true;
    private float baseDistance = 1f;
    private float BaseAngleOffset;
    private bool m_wasLeverLocked;
    private Vector3 m_baseSpinPosition = Vector3.zero;
    private float curDistanceBetweenGrips = 1f;
    private float lastDistanceBetweenGrips = -1f;
    private float m_rackingDisplacement;
    private float xSpinRot;
    private float xSpinVel;

    public bool IsHammerCocked => this.m_isHammerCocked;

    protected override void Awake()
    {
      base.Awake();
      this.m_proxy = new GameObject("m_proxyRound").AddComponent<FVRFirearmMovingProxyRound>();
      this.m_proxy.Init(this.transform);
      if (this.UsesSecondChamber)
      {
        this.m_proxy2 = new GameObject("m_proxyRound2").AddComponent<FVRFirearmMovingProxyRound>();
        this.m_proxy2.Init(this.transform);
      }
      this.m_baseSpinPosition = this.PoseSpinHolder.localPosition;
    }

    public override Transform GetMuzzle()
    {
      if (!this.UsesSecondChamber)
        return base.GetMuzzle();
      return this.m_isSecondaryMuzzlePos ? this.SecondMuzzle : this.MuzzlePos;
    }

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      this.SetBaseHandAngle(hand);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      this.UpdateLever();
      this.Trigger.localEulerAngles = new Vector3(Mathf.Lerp(this.TriggerRotRange.x, this.TriggerRotRange.y, hand.Input.TriggerFloat), 0.0f, 0.0f);
      if (hand.Input.TriggerDown && !this.IsAltHeld && (this.m_curLeverPos == LeverActionFirearm.ZPos.Rear && this.m_hasTriggeredUpSinceBegin) && (this.m_isHammerCocked || this.m_isHammerCocked2))
        this.Fire();
      float t = Mathf.InverseLerp(this.LeverAngleRange.y, this.LeverAngleRange.x, this.m_curLeverRot);
      if (!((UnityEngine.Object) this.Hammer != (UnityEngine.Object) null))
        return;
      if (this.m_isHammerCocked)
        this.Hammer.localEulerAngles = new Vector3(this.HammerAngleRange.x, 0.0f, 0.0f);
      else
        this.Hammer.localEulerAngles = new Vector3(Mathf.Lerp(this.HammerAngleRange.y, this.HammerAngleRange.x, t), 0.0f, 0.0f);
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      this.Trigger.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
      base.EndInteraction(hand);
    }

    private void SetBaseHandAngle(FVRViveHand hand)
    {
      Vector3 normalized = Vector3.ProjectOnPlane(this.m_hand.PoseOverride.forward, this.LeverRoot.right).normalized;
      Vector3 forward = this.LeverRoot.forward;
      this.BaseAngleOffset = Mathf.Atan2(Vector3.Dot(this.LeverRoot.right, Vector3.Cross(forward, normalized)), Vector3.Dot(forward, normalized)) * 57.29578f;
    }

    private void UpdateLever()
    {
      bool flag1 = false;
      bool flag2 = false;
      if (this.IsHeld)
      {
        if (this.m_hand.IsInStreamlinedMode)
        {
          flag1 = this.m_hand.Input.BYButtonPressed;
          flag2 = this.m_hand.Input.BYButtonUp;
        }
        else
        {
          flag1 = this.m_hand.Input.TouchpadPressed;
          flag2 = this.m_hand.Input.TouchpadUp;
        }
      }
      this.m_isLeverReleasePressed = false;
      bool flag3 = false;
      if (!this.IsAltHeld && (UnityEngine.Object) this.ForeGrip.m_hand != (UnityEngine.Object) null)
      {
        if (this.ForeGrip.m_hand.Input.TriggerPressed && this.ForeGrip.m_hasTriggeredUpSinceBegin)
          flag3 = true;
        this.m_isLeverReleasePressed = true;
        this.curDistanceBetweenGrips = Vector3.Distance(this.m_hand.PalmTransform.position, this.AltGrip.m_hand.PalmTransform.position);
        if ((double) this.lastDistanceBetweenGrips < 0.0)
          this.lastDistanceBetweenGrips = this.curDistanceBetweenGrips;
      }
      else
        this.lastDistanceBetweenGrips = -1f;
      this.m_isSpinning = false;
      if (!this.IsAltHeld && this.CanSpin && flag1)
        this.m_isSpinning = true;
      bool flag4 = false;
      if ((this.m_isHammerCocked || this.m_isHammerCocked2) && (!this.m_isSpinning && this.m_curLeverPos == LeverActionFirearm.ZPos.Rear))
        flag4 = true;
      if (flag3)
        flag4 = false;
      if ((UnityEngine.Object) this.AltGrip == (UnityEngine.Object) null && !this.IsAltHeld)
        flag4 = true;
      if (flag4 && this.useLinearRacking)
        this.SetBaseHandAngle(this.m_hand);
      this.m_wasLeverLocked = flag4;
      if (flag2)
      {
        this.m_tarLeverRot = 0.0f;
        this.PoseSpinHolder.localPosition = this.m_baseSpinPosition;
        this.lastDistanceBetweenGrips = this.curDistanceBetweenGrips;
        this.m_rackingDisplacement = 0.0f;
      }
      else if (this.m_isLeverReleasePressed && !flag4)
      {
        if (this.useLinearRacking)
        {
          this.curDistanceBetweenGrips = Vector3.Distance(this.m_hand.PalmTransform.position, this.AltGrip.m_hand.PalmTransform.position);
          if ((double) this.curDistanceBetweenGrips < (double) this.lastDistanceBetweenGrips)
            this.m_rackingDisplacement += this.lastDistanceBetweenGrips - this.curDistanceBetweenGrips;
          else
            this.m_rackingDisplacement -= this.curDistanceBetweenGrips - this.lastDistanceBetweenGrips;
          this.m_rackingDisplacement = Mathf.Clamp(this.m_rackingDisplacement, 0.0f, 0.04f);
          if ((double) this.m_rackingDisplacement < 0.00499999988824129)
            this.m_rackingDisplacement = 0.0f;
          if ((double) this.m_rackingDisplacement > 0.0350000001490116)
            this.m_rackingDisplacement = 0.04f;
          this.PoseSpinHolder.localPosition = this.m_baseSpinPosition + Vector3.forward * this.m_rackingDisplacement * 2f;
          this.m_tarLeverRot = Mathf.Lerp(this.LeverAngleRange.y, this.LeverAngleRange.x, this.m_rackingDisplacement * 25f);
          this.lastDistanceBetweenGrips = this.curDistanceBetweenGrips;
        }
        else
        {
          Vector3 normalized = Vector3.ProjectOnPlane(this.m_hand.PoseOverride.forward, this.LeverRoot.right).normalized;
          Vector3 forward = this.LeverRoot.forward;
          this.m_tarLeverRot = Mathf.Clamp((Mathf.Atan2(Vector3.Dot(this.LeverRoot.right, Vector3.Cross(forward, normalized)), Vector3.Dot(forward, normalized)) * 57.29578f - this.BaseAngleOffset) * 3f, this.LeverAngleRange.x, this.LeverAngleRange.y);
        }
      }
      else if (this.m_isSpinning)
      {
        this.m_tarLeverRot = Mathf.Clamp(-Mathf.Clamp(Mathf.Clamp(this.m_hand.Input.VelLinearWorld.magnitude - 1f, 0.0f, 3f) * 120f, 0.0f, Mathf.Repeat(Mathf.Abs(this.xSpinRot), 360f) * 0.5f), this.LeverAngleRange.x, this.LeverAngleRange.y);
        this.PoseSpinHolder.localPosition = this.m_baseSpinPosition;
      }
      if ((double) Mathf.Abs(this.m_curLeverRot - this.LeverAngleRange.y) < 1.0)
      {
        if (this.m_lastLeverPos == LeverActionFirearm.ZPos.Forward)
        {
          this.m_curLeverPos = LeverActionFirearm.ZPos.Middle;
        }
        else
        {
          this.m_curLeverPos = LeverActionFirearm.ZPos.Rear;
          this.IsBreachOpenForGasOut = false;
        }
      }
      else if ((double) Mathf.Abs(this.m_curLeverRot - this.LeverAngleRange.x) < 1.0)
      {
        if (this.m_lastLeverPos == LeverActionFirearm.ZPos.Rear)
        {
          this.m_curLeverPos = LeverActionFirearm.ZPos.Middle;
        }
        else
        {
          this.m_curLeverPos = LeverActionFirearm.ZPos.Forward;
          this.IsBreachOpenForGasOut = true;
        }
      }
      else
      {
        this.m_curLeverPos = LeverActionFirearm.ZPos.Middle;
        this.IsBreachOpenForGasOut = true;
      }
      if (this.m_curLeverPos == LeverActionFirearm.ZPos.Rear && this.m_lastLeverPos != LeverActionFirearm.ZPos.Rear)
      {
        this.m_tarLeverRot = this.LeverAngleRange.y;
        this.m_curLeverRot = this.LeverAngleRange.y;
        if (this.m_isActionMovingForward && this.m_proxy.IsFull && !this.Chamber.IsFull)
        {
          this.m_hand.Buzz(this.m_hand.Buzzer.Buzz_OnHoverInteractive);
          this.Chamber.SetRound(this.m_proxy.Round);
          this.m_proxy.ClearProxy();
          this.PlayAudioEvent(FirearmAudioEventType.HandleBack);
        }
        else
          this.PlayAudioEvent(FirearmAudioEventType.HandleBackEmpty);
        if (this.UsesSecondChamber && this.m_isActionMovingForward && (this.m_proxy2.IsFull && !this.Chamber2.IsFull))
        {
          this.Chamber2.SetRound(this.m_proxy2.Round);
          this.m_proxy2.ClearProxy();
        }
        this.m_isActionMovingForward = false;
      }
      else if (this.m_curLeverPos == LeverActionFirearm.ZPos.Forward && this.m_lastLeverPos != LeverActionFirearm.ZPos.Forward)
      {
        this.m_tarLeverRot = this.LeverAngleRange.x;
        this.m_curLeverRot = this.LeverAngleRange.x;
        this.m_isHammerCocked = true;
        if (this.UsesSecondChamber)
          this.m_isHammerCocked2 = true;
        this.PlayAudioEvent(FirearmAudioEventType.Prefire);
        if (!this.m_isActionMovingForward && this.Chamber.IsFull)
        {
          this.m_hand.Buzz(this.m_hand.Buzzer.Buzz_OnHoverInteractive);
          this.Chamber.EjectRound(this.ReceiverEjectionPoint.position, this.transform.right * this.EjectionDir.x + this.transform.up * this.EjectionDir.y + this.transform.forward * this.EjectionDir.z, this.transform.right * this.EjectionSpin.x + this.transform.up * this.EjectionSpin.y + this.transform.forward * this.EjectionSpin.z);
          this.PlayAudioEvent(FirearmAudioEventType.HandleForward);
        }
        else
          this.PlayAudioEvent(FirearmAudioEventType.HandleForwardEmpty);
        if (this.UsesSecondChamber && !this.m_isActionMovingForward && this.Chamber2.IsFull)
          this.Chamber2.EjectRound(this.SecondEjectionSpot.position, this.transform.right * this.EjectionDir.x + this.transform.up * this.EjectionDir.y + this.transform.forward * this.EjectionDir.z, this.transform.right * this.EjectionSpin.x + this.transform.up * this.EjectionSpin.y + this.transform.forward * this.EjectionSpin.z);
        this.m_isActionMovingForward = true;
      }
      else if (this.m_curLeverPos == LeverActionFirearm.ZPos.Middle && this.m_lastLeverPos == LeverActionFirearm.ZPos.Rear)
      {
        if ((UnityEngine.Object) this.Magazine != (UnityEngine.Object) null && !this.m_proxy.IsFull && this.Magazine.HasARound())
          this.m_proxy.SetFromPrefabReference(this.Magazine.RemoveRound(false));
        if (this.UsesSecondChamber && (UnityEngine.Object) this.Magazine != (UnityEngine.Object) null && (!this.m_proxy2.IsFull && this.Magazine.HasARound()))
          this.m_proxy2.SetFromPrefabReference(this.Magazine.RemoveRound(false));
      }
      float t = Mathf.InverseLerp(this.LeverAngleRange.y, this.LeverAngleRange.x, this.m_curLeverRot);
      if (this.m_proxy.IsFull)
      {
        if (this.m_isActionMovingForward)
        {
          this.m_proxy.ProxyRound.position = Vector3.Lerp(this.ReceiverUpperPathForward.position, this.ReceiverUpperPathRearward.position, t);
          this.m_proxy.ProxyRound.rotation = Quaternion.Slerp(this.ReceiverUpperPathForward.rotation, this.ReceiverUpperPathRearward.rotation, t);
          if ((UnityEngine.Object) this.LoadingGate != (UnityEngine.Object) null)
            this.LoadingGate.localEulerAngles = new Vector3(this.LoadingGateAngleRange.x, 0.0f, 0.0f);
        }
        else
        {
          this.m_proxy.ProxyRound.position = Vector3.Lerp(this.ReceiverLowerPathForward.position, this.ReceiverLowerPathRearward.position, t);
          this.m_proxy.ProxyRound.rotation = Quaternion.Slerp(this.ReceiverLowerPathForward.rotation, this.ReceiverLowerPathRearward.rotation, t);
          if ((UnityEngine.Object) this.LoadingGate != (UnityEngine.Object) null)
            this.LoadingGate.localEulerAngles = new Vector3(this.LoadingGateAngleRange.y, 0.0f, 0.0f);
        }
      }
      else if ((UnityEngine.Object) this.LoadingGate != (UnityEngine.Object) null)
        this.LoadingGate.localEulerAngles = new Vector3(this.LoadingGateAngleRange.y, 0.0f, 0.0f);
      if (this.Chamber.IsFull)
      {
        this.Chamber.ProxyRound.position = Vector3.Lerp(this.ReceiverEjectionPathForward.position, this.ReceiverEjectionPathRearward.position, t);
        this.Chamber.ProxyRound.rotation = Quaternion.Slerp(this.ReceiverEjectionPathForward.rotation, this.ReceiverEjectionPathRearward.rotation, t);
      }
      if (this.UsesSecondChamber && this.Chamber2.IsFull)
      {
        this.Chamber2.ProxyRound.position = Vector3.Lerp(this.ReceiverEjectionPathForward.position, this.ReceiverEjectionPathRearward.position, t);
        this.Chamber2.ProxyRound.rotation = Quaternion.Slerp(this.ReceiverEjectionPathForward.rotation, this.ReceiverEjectionPathRearward.rotation, t);
      }
      this.Chamber.IsAccessible = this.m_curLeverPos != LeverActionFirearm.ZPos.Rear && !this.m_proxy.IsFull;
      if (this.UsesSecondChamber)
        this.Chamber2.IsAccessible = this.m_curLeverPos != LeverActionFirearm.ZPos.Rear && !this.m_proxy2.IsFull;
      for (int index = 0; index < this.ActuatedPieces.Length; ++index)
      {
        if (this.ActuatedPieces[index].InterpStyle == FVRPhysicalObject.InterpStyle.Translate)
          this.ActuatedPieces[index].Piece.localPosition = Vector3.Lerp(this.ActuatedPieces[index].PosBack, this.ActuatedPieces[index].PosForward, t);
        else
          this.ActuatedPieces[index].Piece.localEulerAngles = Vector3.Lerp(this.ActuatedPieces[index].PosBack, this.ActuatedPieces[index].PosForward, t);
      }
      this.m_lastLeverPos = this.m_curLeverPos;
    }

    private void Fire()
    {
      if (this.m_isHammerCocked)
        this.m_isHammerCocked = false;
      else if (this.m_isHammerCocked2)
        this.m_isHammerCocked2 = false;
      this.PlayAudioEvent(FirearmAudioEventType.HammerHit);
      bool flag1 = false;
      bool flag2 = true;
      if (this.Chamber.Fire())
      {
        flag1 = true;
        flag2 = true;
        this.m_isSecondaryMuzzlePos = false;
      }
      else if (this.UsesSecondChamber && this.Chamber2.Fire())
      {
        flag1 = true;
        flag2 = false;
        this.m_isSecondaryMuzzlePos = true;
      }
      if (!flag1)
        return;
      if (flag2)
        this.Fire(this.Chamber, this.GetMuzzle(), true);
      else
        this.Fire(this.Chamber2, this.SecondMuzzle, true);
      this.FireMuzzleSmoke();
      this.Recoil(this.IsTwoHandStabilized(), (UnityEngine.Object) this.AltGrip != (UnityEngine.Object) null, this.IsShoulderStabilized());
      if (flag2)
        this.PlayAudioGunShot(this.Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
      else
        this.PlayAudioGunShot(this.Chamber2.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
    }

    protected override void FVRFixedUpdate()
    {
      base.FVRFixedUpdate();
      this.m_curLeverRot = Mathf.MoveTowards(this.m_curLeverRot, this.m_tarLeverRot, this.m_leverRotSpeed * Time.deltaTime);
      this.Lever.localEulerAngles = new Vector3(this.m_curLeverRot, 0.0f, 0.0f);
      this.UpdateSpinning();
    }

    private void UpdateSpinning()
    {
      if (!this.IsHeld || this.IsAltHeld || (UnityEngine.Object) this.AltGrip != (UnityEngine.Object) null)
        this.m_isSpinning = false;
      if (this.m_isSpinning)
      {
        Vector3 vector3 = Vector3.zero;
        if ((UnityEngine.Object) this.m_hand != (UnityEngine.Object) null)
          vector3 = this.m_hand.Input.VelLinearWorld;
        float f = Mathf.Clamp(Vector3.Dot(vector3.normalized, this.transform.up), -vector3.magnitude, vector3.magnitude);
        if ((double) Mathf.Abs(this.xSpinVel) < 90.0)
          this.xSpinVel += (float) ((double) f * (double) Time.deltaTime * 600.0);
        else if ((double) Mathf.Sign(f) == (double) Mathf.Sign(this.xSpinVel))
          this.xSpinVel += (float) ((double) f * (double) Time.deltaTime * 600.0);
        if ((double) Mathf.Abs(this.xSpinVel) < 90.0)
        {
          if ((double) Vector3.Dot(this.transform.up, Vector3.down) >= 0.0 && (double) Mathf.Sign(this.xSpinVel) == 1.0)
            this.xSpinVel += Time.deltaTime * 50f;
          if ((double) Vector3.Dot(this.transform.up, Vector3.down) < 0.0 && (double) Mathf.Sign(this.xSpinVel) == -1.0)
            this.xSpinVel -= Time.deltaTime * 50f;
        }
        this.xSpinVel = Mathf.Clamp(this.xSpinVel, -200f, 200f);
        this.xSpinRot += (float) ((double) this.xSpinVel * (double) Time.deltaTime * 5.0);
        this.PoseSpinHolder.localEulerAngles = new Vector3(this.xSpinRot, 0.0f, 0.0f);
        this.xSpinVel = Mathf.Lerp(this.xSpinVel, 0.0f, Time.deltaTime * 0.6f);
      }
      else
      {
        this.xSpinRot = 0.0f;
        this.xSpinVel = 0.0f;
        this.PoseSpinHolder.localEulerAngles = new Vector3(this.xSpinRot, 0.0f, 0.0f);
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

    public enum ZPos
    {
      Forward,
      Middle,
      Rear,
    }

    [Serializable]
    public class LeverActuatedPiece
    {
      public Transform Piece;
      public Vector3 PosBack;
      public Vector3 PosForward;
      public FVRPhysicalObject.InterpStyle InterpStyle;
      public LeverActionFirearm.ActuationType ActuationType;
    }

    public enum ActuationType
    {
      Lever,
      Hammer,
    }
  }
}
