// Decompiled with JetBrains decompiler
// Type: FistVR.Revolver
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class Revolver : FVRFireArm
  {
    [Header("Revolver Config")]
    public bool AllowsSuppressor;
    public bool isChiappa;
    public bool isChiappaHammer;
    public Transform Hammer;
    public bool CanManuallyCockHammer = true;
    public bool IsDoubleActionTrigger = true;
    private float m_hammerForwardRot;
    public float m_hammerBackwardRot = -49f;
    private float m_hammerCurrentRot;
    public Transform Trigger;
    private float m_triggerForwardRot;
    public float m_triggerBackwardRot = 30f;
    private float m_triggerCurrentRot;
    private bool m_isHammerLocked;
    private bool m_hasTriggerCycled;
    public bool DoesFiringRecock;
    public Transform CylinderReleaseButton;
    public bool isCyclinderReleaseARot;
    public Vector3 CylinderReleaseButtonForwardPos;
    public Vector3 CylinderReleaseButtonRearPos;
    private bool m_isCylinderReleasePressed;
    private float m_curCyclinderReleaseRot;
    [Header("Cylinder Config")]
    public bool UsesCylinderArm = true;
    public Transform CylinderArm;
    private bool m_isCylinderArmLocked = true;
    private bool m_wasCylinderArmLocked = true;
    private float CylinderArmRot;
    public bool IsCylinderRotClockwise = true;
    public Vector2 CylinderRotRange = new Vector2(0.0f, 105f);
    public bool IsCylinderArmZ;
    public bool AngInvert;
    public bool GravityRotsCylinderPositive = true;
    public RevolverCylinder Cylinder;
    private int m_curChamber;
    private float m_tarChamberLerp;
    private float m_curChamberLerp;
    [Header("Chambers Config")]
    public FVRFireArmChamber[] Chambers;
    [Header("Spinning Config")]
    public Transform PoseSpinHolder;
    public bool CanSpin = true;
    private bool m_isSpinning;
    public Transform Muzzle;
    public bool UsesAltPoseSwitch = true;
    public Transform Pose_Main;
    public Transform Pose_Reloading;
    private bool m_isInMainpose = true;
    private Vector2 TouchPadAxes = Vector2.zero;
    private bool m_hasEjectedSinceOpening;
    public List<Revolver.TriggerPiece> TPieces;
    private float xSpinRot;
    private float xSpinVel;
    private float lastTriggerRot;
    private bool m_shouldRecock;
    private float m_CylCloseVel;

    public bool isCylinderArmLocked => this.m_isCylinderArmLocked;

    public int CurChamber
    {
      get => this.m_curChamber;
      set
      {
        if (value < 0)
          this.m_curChamber = this.Cylinder.numChambers - 1;
        else
          this.m_curChamber = value % this.Cylinder.numChambers;
      }
    }

    protected override void Awake() => base.Awake();

    public override int GetTutorialState()
    {
      int num1 = 0;
      int num2 = 0;
      for (int index = 0; index < this.Chambers.Length; ++index)
      {
        if (this.Chambers[index].IsFull)
        {
          ++num1;
          if (!this.Chambers[index].IsSpent)
            ++num2;
        }
      }
      return num1 <= 0 ? (this.m_isCylinderArmLocked ? 0 : 1) : (num2 > 0 ? (this.m_isCylinderArmLocked ? 3 : 2) : (this.m_isCylinderArmLocked ? 0 : 4));
    }

    protected override void FVRUpdate() => base.FVRUpdate();

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      if (!this.IsAltHeld)
      {
        if (this.m_isInMainpose)
        {
          this.PoseOverride.localPosition = this.Pose_Main.localPosition;
          this.PoseOverride.localRotation = this.Pose_Main.localRotation;
          this.m_grabPointTransform.localPosition = this.Pose_Main.localPosition;
          this.m_grabPointTransform.localRotation = this.Pose_Main.localRotation;
        }
        else
        {
          this.PoseOverride.localPosition = this.Pose_Reloading.localPosition;
          this.PoseOverride.localRotation = this.Pose_Reloading.localRotation;
          this.m_grabPointTransform.localPosition = this.Pose_Reloading.localPosition;
          this.m_grabPointTransform.localRotation = this.Pose_Reloading.localRotation;
        }
      }
      this.RootRigidbody.maxAngularVelocity = 40f;
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      base.EndInteraction(hand);
      this.RootRigidbody.AddRelativeTorque(new Vector3(this.xSpinVel, 0.0f, 0.0f), ForceMode.Impulse);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (this.IsAltHeld && !this.m_isInMainpose)
      {
        this.m_isInMainpose = true;
        this.PoseOverride.localPosition = this.Pose_Main.localPosition;
        this.PoseOverride.localRotation = this.Pose_Main.localRotation;
        this.m_grabPointTransform.localPosition = this.Pose_Main.localPosition;
        this.m_grabPointTransform.localRotation = this.Pose_Main.localRotation;
      }
      this.TouchPadAxes = hand.Input.TouchpadAxes;
      this.m_isSpinning = false;
      if (!this.IsAltHeld && !hand.IsInStreamlinedMode)
      {
        if (hand.Input.TouchpadPressed && (double) Vector2.Angle(this.TouchPadAxes, Vector2.up) < 45.0)
          this.m_isSpinning = true;
        if (hand.Input.TouchpadDown && (double) Vector2.Angle(this.TouchPadAxes, Vector2.right) < 45.0 && this.UsesAltPoseSwitch)
        {
          this.m_isInMainpose = !this.m_isInMainpose;
          if (this.m_isInMainpose)
          {
            this.PoseOverride.localPosition = this.Pose_Main.localPosition;
            this.PoseOverride.localRotation = this.Pose_Main.localRotation;
            this.m_grabPointTransform.localPosition = this.Pose_Main.localPosition;
            this.m_grabPointTransform.localRotation = this.Pose_Main.localRotation;
          }
          else
          {
            this.PoseOverride.localPosition = this.Pose_Reloading.localPosition;
            this.PoseOverride.localRotation = this.Pose_Reloading.localRotation;
            this.m_grabPointTransform.localPosition = this.Pose_Reloading.localPosition;
            this.m_grabPointTransform.localRotation = this.Pose_Reloading.localRotation;
          }
        }
      }
      this.UpdateTriggerHammer();
      this.UpdateCylinderRelease();
      if (this.IsHeld && !this.IsAltHeld && !((UnityEngine.Object) this.AltGrip != (UnityEngine.Object) null))
        return;
      this.m_isSpinning = false;
    }

    protected override void FVRFixedUpdate()
    {
      this.UpdateSpinning();
      base.FVRFixedUpdate();
    }

    public void EjectChambers()
    {
      bool flag = false;
      for (int index = 0; index < this.Chambers.Length; ++index)
      {
        if (this.Chambers[index].IsFull)
        {
          flag = true;
          if (this.AngInvert)
            this.Chambers[index].EjectRound(this.Chambers[index].transform.position + this.Chambers[index].transform.forward * this.Cylinder.CartridgeLength, this.Chambers[index].transform.forward, UnityEngine.Random.onUnitSphere, true);
          else
            this.Chambers[index].EjectRound(this.Chambers[index].transform.position + -this.Chambers[index].transform.forward * this.Cylinder.CartridgeLength, -this.Chambers[index].transform.forward, UnityEngine.Random.onUnitSphere, true);
        }
      }
      if (!flag)
        return;
      this.PlayAudioEvent(FirearmAudioEventType.MagazineOut);
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
        this.xSpinVel = Mathf.Clamp(this.xSpinVel, -500f, 500f);
        this.xSpinRot += (float) ((double) this.xSpinVel * (double) Time.deltaTime * 5.0);
        this.PoseSpinHolder.localEulerAngles = new Vector3(this.xSpinRot, 0.0f, 0.0f);
        this.xSpinVel = Mathf.Lerp(this.xSpinVel, 0.0f, Time.deltaTime * 0.6f);
      }
      else
      {
        this.xSpinRot = 0.0f;
        this.xSpinVel = 0.0f;
      }
    }

    private void UpdateTriggerHammer()
    {
      float t = 0.0f;
      if (this.m_hasTriggeredUpSinceBegin && !this.m_isSpinning && (!this.IsAltHeld && this.isCylinderArmLocked))
        t = this.m_hand.Input.TriggerFloat;
      if (this.m_isHammerLocked)
      {
        t += 0.8f;
        this.m_triggerCurrentRot = Mathf.Lerp(this.m_triggerForwardRot, this.m_triggerBackwardRot, t);
      }
      else
        this.m_triggerCurrentRot = Mathf.Lerp(this.m_triggerForwardRot, this.m_triggerBackwardRot, t);
      if ((double) Mathf.Abs(this.m_triggerCurrentRot - this.lastTriggerRot) > 0.00999999977648258)
      {
        if ((UnityEngine.Object) this.Trigger != (UnityEngine.Object) null)
          this.Trigger.localEulerAngles = new Vector3(this.m_triggerCurrentRot, 0.0f, 0.0f);
        for (int index = 0; index < this.TPieces.Count; ++index)
          this.SetAnimatedComponent(this.TPieces[index].TPiece, Mathf.Lerp(this.TPieces[index].TRange.x, this.TPieces[index].TRange.y, t), this.TPieces[index].TInterp, this.TPieces[index].TAxis);
      }
      this.lastTriggerRot = this.m_triggerCurrentRot;
      if (this.m_shouldRecock)
      {
        this.m_shouldRecock = false;
        this.m_isHammerLocked = true;
        this.PlayAudioEvent(FirearmAudioEventType.Prefire);
      }
      if (!this.m_hasTriggerCycled || !this.IsDoubleActionTrigger)
      {
        if ((double) t >= 0.980000019073486 && (this.m_isHammerLocked || this.IsDoubleActionTrigger) && !this.m_hand.Input.TouchpadPressed)
        {
          if (this.m_isCylinderArmLocked)
          {
            this.m_hasTriggerCycled = true;
            this.m_isHammerLocked = false;
            if (this.IsCylinderRotClockwise)
              ++this.CurChamber;
            else
              --this.CurChamber;
            this.m_curChamberLerp = 0.0f;
            this.m_tarChamberLerp = 0.0f;
            this.PlayAudioEvent(FirearmAudioEventType.HammerHit);
            if (this.Chambers[this.CurChamber].IsFull && !this.Chambers[this.CurChamber].IsSpent)
            {
              this.Chambers[this.CurChamber].Fire();
              this.Fire();
              if (GM.CurrentSceneSettings.IsAmmoInfinite || GM.CurrentPlayerBody.IsInfiniteAmmo)
              {
                this.Chambers[this.CurChamber].IsSpent = false;
                this.Chambers[this.CurChamber].UpdateProxyDisplay();
              }
              if (this.DoesFiringRecock)
                this.m_shouldRecock = true;
            }
          }
          else
          {
            this.m_hasTriggerCycled = true;
            this.m_isHammerLocked = false;
          }
        }
        else if (((double) t <= 0.0799999982118607 || !this.IsDoubleActionTrigger) && (!this.m_isHammerLocked && this.CanManuallyCockHammer) && !this.IsAltHeld)
        {
          if (this.m_hand.IsInStreamlinedMode)
          {
            if (this.m_hand.Input.AXButtonDown)
            {
              this.m_isHammerLocked = true;
              this.PlayAudioEvent(FirearmAudioEventType.Prefire);
            }
          }
          else if (this.m_hand.Input.TouchpadDown && (double) Vector2.Angle(this.TouchPadAxes, Vector2.down) < 45.0)
          {
            this.m_isHammerLocked = true;
            this.PlayAudioEvent(FirearmAudioEventType.Prefire);
          }
        }
      }
      else if (this.m_hasTriggerCycled && (double) this.m_hand.Input.TriggerFloat <= 0.0799999982118607)
      {
        this.m_hasTriggerCycled = false;
        this.PlayAudioEvent(FirearmAudioEventType.TriggerReset);
      }
      if (!this.isChiappaHammer)
        this.m_hammerCurrentRot = this.m_hasTriggerCycled || !this.IsDoubleActionTrigger ? (!this.m_isHammerLocked ? Mathf.Lerp(this.m_hammerCurrentRot, this.m_hammerForwardRot, Time.deltaTime * 30f) : Mathf.Lerp(this.m_hammerCurrentRot, this.m_hammerBackwardRot, Time.deltaTime * 10f)) : (!this.m_isHammerLocked ? Mathf.Lerp(this.m_hammerForwardRot, this.m_hammerBackwardRot, t) : Mathf.Lerp(this.m_hammerCurrentRot, this.m_hammerBackwardRot, Time.deltaTime * 10f));
      if (this.isChiappaHammer)
      {
        bool flag = false;
        if (this.m_hand.IsInStreamlinedMode && this.m_hand.Input.AXButtonPressed)
          flag = true;
        else if ((double) Vector2.Angle(this.m_hand.Input.TouchpadAxes, Vector2.down) < 45.0 && this.m_hand.Input.TouchpadPressed)
          flag = true;
        this.m_hammerCurrentRot = (double) t > 0.0199999995529652 || this.IsAltHeld || !flag ? Mathf.Lerp(this.m_hammerCurrentRot, this.m_hammerForwardRot, Time.deltaTime * 6f) : Mathf.Lerp(this.m_hammerCurrentRot, this.m_hammerBackwardRot, Time.deltaTime * 15f);
      }
      if (!((UnityEngine.Object) this.Hammer != (UnityEngine.Object) null))
        return;
      this.Hammer.localEulerAngles = new Vector3(this.m_hammerCurrentRot, 0.0f, 0.0f);
    }

    private void Fire()
    {
      FVRFireArmChamber chamber = this.Chambers[this.CurChamber];
      this.Fire(chamber, this.GetMuzzle(), true);
      this.FireMuzzleSmoke();
      if (chamber.GetRound().IsHighPressure)
        this.Recoil(this.IsTwoHandStabilized(), (UnityEngine.Object) this.AltGrip != (UnityEngine.Object) null, this.IsShoulderStabilized());
      this.PlayAudioGunShot(chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
      if (!chamber.GetRound().IsCaseless)
        return;
      chamber.SetRound((FVRFireArmRound) null);
    }

    public void AddCylinderCloseVel(float f) => this.m_CylCloseVel = f;

    private void UpdateCylinderRelease()
    {
      float num1 = 0.0f;
      if (this.m_hasTriggeredUpSinceBegin && !this.m_isSpinning && (!this.IsAltHeld && this.isCylinderArmLocked))
        num1 = this.m_hand.Input.TriggerFloat;
      this.m_isCylinderReleasePressed = false;
      if (!this.IsAltHeld && (!this.m_isHammerLocked || this.DoesFiringRecock))
      {
        if (this.m_hand.IsInStreamlinedMode)
        {
          if (this.m_hand.Input.BYButtonPressed)
            this.m_isCylinderReleasePressed = true;
        }
        else if (this.m_hand.Input.TouchpadPressed && (double) Vector2.Angle(this.TouchPadAxes, Vector2.left) < 45.0)
          this.m_isCylinderReleasePressed = true;
      }
      if ((UnityEngine.Object) this.CylinderReleaseButton != (UnityEngine.Object) null)
      {
        if (this.isCyclinderReleaseARot)
        {
          this.m_curCyclinderReleaseRot = this.m_isCylinderReleasePressed ? Mathf.Lerp(this.m_curCyclinderReleaseRot, this.CylinderReleaseButtonRearPos.x, Time.deltaTime * 3f) : Mathf.Lerp(this.m_curCyclinderReleaseRot, this.CylinderReleaseButtonForwardPos.x, Time.deltaTime * 3f);
          this.CylinderReleaseButton.localEulerAngles = new Vector3(this.m_curCyclinderReleaseRot, 0.0f, 0.0f);
        }
        else
          this.CylinderReleaseButton.localPosition = !this.m_isCylinderReleasePressed ? Vector3.Lerp(this.CylinderReleaseButton.localPosition, this.CylinderReleaseButtonRearPos, Time.deltaTime * 3f) : Vector3.Lerp(this.CylinderReleaseButton.localPosition, this.CylinderReleaseButtonForwardPos, Time.deltaTime * 3f);
      }
      if (this.m_isCylinderReleasePressed)
      {
        this.m_isCylinderArmLocked = false;
      }
      else
      {
        float f = this.CylinderArm.localEulerAngles.z;
        if (this.IsCylinderArmZ)
          f = this.CylinderArm.localEulerAngles.x;
        if ((double) Mathf.Abs(f) <= 1.0 && !this.m_isCylinderArmLocked)
        {
          this.m_isCylinderArmLocked = true;
          this.CylinderArm.localEulerAngles = Vector3.zero;
        }
      }
      float num2 = 160f;
      if (!this.GravityRotsCylinderPositive)
        num2 *= -1f;
      if (!this.m_isCylinderArmLocked)
      {
        float num3 = this.transform.InverseTransformDirection(this.m_hand.Input.VelAngularWorld).z;
        float num4 = this.transform.InverseTransformDirection(this.m_hand.Input.VelLinearWorld).x;
        if (this.IsCylinderArmZ)
        {
          num3 = this.transform.InverseTransformDirection(this.m_hand.Input.VelAngularWorld).x;
          num4 = this.transform.InverseTransformDirection(this.m_hand.Input.VelLinearWorld).y;
        }
        if (this.AngInvert)
        {
          num3 = -num3;
          num4 = -num4;
        }
        float num5 = num2 + num3 * 70f + num4 * -350f + this.m_CylCloseVel;
        this.m_CylCloseVel = 0.0f;
        float num6 = Mathf.Clamp(this.CylinderArmRot + num5 * Time.deltaTime, this.CylinderRotRange.x, this.CylinderRotRange.y);
        if ((double) num6 != (double) this.CylinderArmRot)
        {
          this.CylinderArmRot = num6;
          this.CylinderArm.localEulerAngles = !this.IsCylinderArmZ ? new Vector3(0.0f, 0.0f, num6) : new Vector3(num6, 0.0f, 0.0f);
        }
      }
      float f1 = this.CylinderArm.localEulerAngles.z;
      if (this.IsCylinderArmZ)
        f1 = this.CylinderArm.localEulerAngles.x;
      if ((double) Mathf.Abs(f1) > 30.0)
      {
        for (int index = 0; index < this.Chambers.Length; ++index)
          this.Chambers[index].IsAccessible = true;
      }
      else
      {
        for (int index = 0; index < this.Chambers.Length; ++index)
          this.Chambers[index].IsAccessible = false;
      }
      if ((double) Mathf.Abs(f1) < 1.0 && this.IsCylinderArmZ)
        this.m_hasEjectedSinceOpening = false;
      if ((double) Mathf.Abs(f1) > 45.0 && this.IsCylinderArmZ && !this.m_hasEjectedSinceOpening)
      {
        this.m_hasEjectedSinceOpening = true;
        this.EjectChambers();
      }
      if (!this.IsCylinderArmZ && ((double) Mathf.Abs(this.CylinderArm.localEulerAngles.z) > 75.0 && (double) Vector3.Angle(this.transform.forward, Vector3.up) <= 120.0))
      {
        float num3 = this.transform.InverseTransformDirection(this.m_hand.Input.VelLinearWorld).z;
        if (this.AngInvert)
          num3 = -num3;
        if ((double) num3 < -2.0)
          this.EjectChambers();
      }
      if (this.m_isCylinderArmLocked && !this.m_wasCylinderArmLocked)
      {
        this.m_curChamber = this.Cylinder.GetClosestChamberIndex();
        this.Cylinder.transform.localRotation = this.Cylinder.GetLocalRotationFromCylinder(this.m_curChamber);
        this.m_curChamberLerp = 0.0f;
        this.m_tarChamberLerp = 0.0f;
        this.PlayAudioEvent(FirearmAudioEventType.BreachClose);
      }
      if (!this.m_isCylinderArmLocked && this.m_wasCylinderArmLocked)
        this.PlayAudioEvent(FirearmAudioEventType.BreachOpen);
      if (this.m_isHammerLocked)
        this.m_tarChamberLerp = 1f;
      else if (!this.m_hasTriggerCycled && this.IsDoubleActionTrigger)
        this.m_tarChamberLerp = num1 * 1.4f;
      this.m_curChamberLerp = Mathf.Lerp(this.m_curChamberLerp, this.m_tarChamberLerp, Time.deltaTime * 16f);
      int cylinder = !this.IsCylinderRotClockwise ? (this.CurChamber - 1) % this.Cylinder.numChambers : (this.CurChamber + 1) % this.Cylinder.numChambers;
      if (this.isCylinderArmLocked)
        this.Cylinder.transform.localRotation = Quaternion.Slerp(this.Cylinder.GetLocalRotationFromCylinder(this.CurChamber), this.Cylinder.GetLocalRotationFromCylinder(cylinder), this.m_curChamberLerp);
      this.m_wasCylinderArmLocked = this.m_isCylinderArmLocked;
    }

    public override List<FireArmRoundClass> GetChamberRoundList()
    {
      bool flag = false;
      List<FireArmRoundClass> fireArmRoundClassList = new List<FireArmRoundClass>();
      for (int index = 0; index < this.Chambers.Length; ++index)
      {
        if (this.Chambers[index].IsFull)
        {
          fireArmRoundClassList.Add(this.Chambers[index].GetRound().RoundClass);
          flag = true;
        }
      }
      return flag ? fireArmRoundClassList : (List<FireArmRoundClass>) null;
    }

    public override void SetLoadedChambers(List<FireArmRoundClass> rounds)
    {
      if (rounds.Count <= 0)
        return;
      for (int index = 0; index < this.Chambers.Length; ++index)
      {
        if (index < rounds.Count)
          this.Chambers[index].Autochamber(rounds[index]);
      }
    }

    public override List<string> GetFlagList() => (List<string>) null;

    public override void SetFromFlagList(List<string> flags)
    {
    }

    [Serializable]
    public class TriggerPiece
    {
      public Transform TPiece;
      public FVRPhysicalObject.Axis TAxis;
      public Vector2 TRange;
      public FVRPhysicalObject.InterpStyle TInterp;
    }
  }
}
