// Decompiled with JetBrains decompiler
// Type: FistVR.Derringer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class Derringer : FVRFireArm
  {
    [Header("Derringer Params")]
    public List<Derringer.DBarrel> Barrels;
    private int m_curBarrel;
    public Transform Hinge;
    public FVRPhysicalObject.Axis Hinge_Axis;
    public FVRPhysicalObject.InterpStyle Hinge_InterpStyle;
    public Vector2 HingeValues;
    private bool m_isHingeLatched = true;
    public bool DoesAutoEjectRounds;
    public bool HasLatchPiece;
    public Transform LatchPiece;
    public FVRPhysicalObject.Axis Latch_Axis;
    public FVRPhysicalObject.InterpStyle Latch_InterpStyle;
    public Vector2 LatchValues;
    public bool HasExternalHammer;
    private bool m_isExternalHammerCocked;
    public Transform ExternalHammer;
    public FVRPhysicalObject.Axis ExternalHammer_Axis;
    public FVRPhysicalObject.InterpStyle ExternalHammer_InterpStyle;
    public Vector2 ExternalHammer_Values;
    public Transform Trigger;
    public FVRPhysicalObject.Axis Trigger_Axis;
    public FVRPhysicalObject.InterpStyle Trigger_InterpStyle;
    public Vector2 Trigger_Values;
    public bool IsTriggerDoubleAction;
    private bool m_hasTriggerReset;
    public bool DeletesCartridgeOnFire;
    private float triggerFloat;

    public override Transform GetMuzzle() => this.Barrels[this.m_curBarrel].MuzzlePoint;

    private void CockHammer()
    {
      if (!this.HasExternalHammer || this.m_isExternalHammerCocked)
        return;
      this.m_isExternalHammerCocked = true;
      this.SetAnimatedComponent(this.ExternalHammer, this.ExternalHammer_Values.y, this.ExternalHammer_InterpStyle, this.ExternalHammer_Axis);
      this.PlayAudioEvent(FirearmAudioEventType.Prefire);
      ++this.m_curBarrel;
      if (this.m_curBarrel < this.Barrels.Count)
        return;
      this.m_curBarrel = 0;
    }

    private void DropHammer()
    {
      bool flag = false;
      if (this.HasExternalHammer)
      {
        if (this.m_isExternalHammerCocked)
        {
          this.m_isExternalHammerCocked = false;
          this.SetAnimatedComponent(this.ExternalHammer, this.ExternalHammer_Values.x, this.ExternalHammer_InterpStyle, this.ExternalHammer_Axis);
          this.PlayAudioEvent(FirearmAudioEventType.HammerHit);
          flag = true;
        }
      }
      else
      {
        this.PlayAudioEvent(FirearmAudioEventType.HammerHit);
        flag = true;
      }
      if (!flag)
        return;
      this.FireBarrel(this.m_curBarrel);
    }

    private void Unlatch()
    {
      if (!this.m_isHingeLatched)
        return;
      this.m_isHingeLatched = false;
      if (this.HasLatchPiece)
        this.SetAnimatedComponent(this.LatchPiece, this.LatchValues.y, this.Latch_InterpStyle, this.Latch_Axis);
      this.SetAnimatedComponent(this.Hinge, this.HingeValues.y, this.Hinge_InterpStyle, this.Hinge_Axis);
      this.PlayAudioEvent(FirearmAudioEventType.BreachOpen);
      for (int index = 0; index < this.Barrels.Count; ++index)
        this.Barrels[index].Chamber.IsAccessible = true;
      if (!this.DoesAutoEjectRounds)
        return;
      for (int index = 0; index < this.Barrels.Count; ++index)
      {
        FVRFireArmChamber chamber = this.Barrels[index].Chamber;
        chamber.EjectRound(chamber.transform.position + chamber.transform.forward * -0.06f, chamber.transform.forward * -0.3f, Vector3.right);
      }
    }

    private void Latch()
    {
      if (this.m_isHingeLatched)
        return;
      this.m_isHingeLatched = true;
      if (this.HasLatchPiece)
        this.SetAnimatedComponent(this.LatchPiece, this.LatchValues.x, this.Latch_InterpStyle, this.Latch_Axis);
      this.SetAnimatedComponent(this.Hinge, this.HingeValues.x, this.Hinge_InterpStyle, this.Hinge_Axis);
      this.PlayAudioEvent(FirearmAudioEventType.BreachClose);
      for (int index = 0; index < this.Barrels.Count; ++index)
        this.Barrels[index].Chamber.IsAccessible = false;
    }

    private void ToggleLatchState()
    {
      if (this.m_isHingeLatched)
        this.Unlatch();
      else
        this.Latch();
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      float num = 0.0f;
      switch (this.Hinge_Axis)
      {
        case FVRPhysicalObject.Axis.X:
          num = this.transform.InverseTransformDirection(hand.Input.VelAngularWorld).x;
          break;
        case FVRPhysicalObject.Axis.Y:
          num = this.transform.InverseTransformDirection(hand.Input.VelAngularWorld).y;
          break;
        case FVRPhysicalObject.Axis.Z:
          num = this.transform.InverseTransformDirection(hand.Input.VelAngularWorld).z;
          break;
      }
      if ((double) num < -15.0)
        this.Latch();
      if (!this.IsAltHeld)
      {
        if (hand.IsInStreamlinedMode)
        {
          if (hand.Input.BYButtonDown)
            this.ToggleLatchState();
          if (hand.Input.AXButtonDown)
            this.CockHammer();
        }
        else if (hand.Input.TouchpadDown)
        {
          if (hand.Input.TouchpadSouthPressed)
            this.CockHammer();
          if (hand.Input.TouchpadWestPressed)
            this.ToggleLatchState();
        }
      }
      this.triggerFloat = !this.m_hasTriggeredUpSinceBegin || this.IsAltHeld ? 0.0f : hand.Input.TriggerFloat;
      this.SetAnimatedComponent(this.Trigger, Mathf.Lerp(this.Trigger_Values.x, this.Trigger_Values.y, this.triggerFloat), this.Trigger_InterpStyle, this.Trigger_Axis);
      if ((double) this.triggerFloat > 0.699999988079071 && this.m_hasTriggerReset)
      {
        this.m_hasTriggerReset = false;
        this.DropHammer();
      }
      else
      {
        if ((double) this.triggerFloat >= 0.200000002980232 || !this.m_hasTriggeredUpSinceBegin || this.m_hasTriggerReset)
          return;
        this.m_hasTriggerReset = true;
        if (this.IsTriggerDoubleAction)
        {
          ++this.m_curBarrel;
          if (this.m_curBarrel >= this.Barrels.Count)
            this.m_curBarrel = 0;
        }
        this.PlayAudioEvent(FirearmAudioEventType.TriggerReset);
      }
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      base.EndInteraction(hand);
      this.triggerFloat = 0.0f;
      this.SetAnimatedComponent(this.Trigger, 0.0f, this.Trigger_InterpStyle, this.Trigger_Axis);
    }

    private void FireBarrel(int i)
    {
      if (!this.m_isHingeLatched)
        return;
      FVRFireArmChamber chamber = this.Barrels[this.m_curBarrel].Chamber;
      if (!chamber.Fire())
        return;
      this.Fire(chamber, this.GetMuzzle(), true);
      this.FireMuzzleSmoke();
      this.Recoil(this.IsTwoHandStabilized(), (UnityEngine.Object) this.AltGrip != (UnityEngine.Object) null, this.IsShoulderStabilized());
      this.PlayAudioGunShot(chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
      if (GM.CurrentSceneSettings.IsAmmoInfinite || GM.CurrentPlayerBody.IsInfiniteAmmo)
      {
        chamber.IsSpent = false;
        chamber.UpdateProxyDisplay();
      }
      else if (chamber.GetRound().IsCaseless)
        chamber.SetRound((FVRFireArmRound) null);
      if (!this.DeletesCartridgeOnFire)
        return;
      chamber.SetRound((FVRFireArmRound) null);
    }

    [Serializable]
    public class DBarrel
    {
      public Transform MuzzlePoint;
      public FVRFireArmChamber Chamber;
    }
  }
}
