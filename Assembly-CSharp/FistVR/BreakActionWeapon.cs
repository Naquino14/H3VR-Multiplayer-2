// Decompiled with JetBrains decompiler
// Type: FistVR.BreakActionWeapon
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class BreakActionWeapon : FVRFireArm
  {
    [Header("Component Connections")]
    public BreakActionWeapon.BreakActionBarrel[] Barrels;
    public Transform[] Triggers;
    public FVRPhysicalObject.InterpStyle TriggerInterpStyle = FVRPhysicalObject.InterpStyle.Rotation;
    public FVRPhysicalObject.Axis TriggerAxis;
    public float TriggerUnpulled;
    public float TriggerPulled;
    public bool FireAllBarrels;
    [Header("Hinge Params")]
    public HingeJoint Hinge;
    private Vector3 m_foreStartPos;
    [Header("Latch Params")]
    public bool m_hasLatch;
    public Transform Latch;
    public float MaxRotExtent;
    private float m_latchRot;
    private bool m_isLatched = true;
    [Header("Control Params")]
    public bool UsesManuallyCockedHammers;
    private int m_curBarrel;
    [HideInInspector]
    public bool IsLatchHeldOpen;
    [HideInInspector]
    public bool HasTriggerReset = true;
    private float m_triggerFloat;

    public bool IsLatched => this.m_isLatched;

    protected override void Awake()
    {
      base.Awake();
      this.m_foreStartPos = this.Hinge.transform.localPosition;
    }

    public override Transform GetMuzzle() => this.Barrels[this.m_curBarrel].Muzzle;

    public void Unlatch()
    {
    }

    public void PopOutEmpties()
    {
      for (int index = 0; index < this.Barrels.Length; ++index)
      {
        if (this.Barrels[index].Chamber.IsFull && this.Barrels[index].Chamber.IsSpent)
          this.PopOutRound(this.Barrels[index].Chamber);
      }
    }

    public void PopOutRound(FVRFireArmChamber chamber)
    {
      this.PlayAudioEvent(FirearmAudioEventType.MagazineEjectRound);
      chamber.EjectRound(chamber.transform.position + chamber.transform.forward * -0.06f, chamber.transform.forward * -2.5f, Vector3.right);
    }

    public void CockHammer()
    {
      for (int index = 0; index < this.Barrels.Length; ++index)
      {
        if (!this.Barrels[index].m_isHammerCocked)
        {
          this.Barrels[index].m_isHammerCocked = true;
          this.PlayAudioEvent(FirearmAudioEventType.Prefire);
          break;
        }
      }
      this.UpdateVisualHammers();
    }

    public void CockAllHammers()
    {
      bool flag = false;
      for (int index = 0; index < this.Barrels.Length; ++index)
      {
        if (!this.Barrels[index].m_isHammerCocked)
        {
          this.Barrels[index].m_isHammerCocked = true;
          flag = true;
        }
      }
      if (!flag)
        return;
      this.PlayAudioEvent(FirearmAudioEventType.Prefire);
      this.UpdateVisualHammers();
    }

    public void DropHammer()
    {
      if (!this.m_isLatched)
        return;
      for (int b = 0; b < this.Barrels.Length; ++b)
      {
        if (this.Barrels[b].m_isHammerCocked)
        {
          this.PlayAudioEvent(FirearmAudioEventType.HammerHit);
          this.Barrels[b].m_isHammerCocked = false;
          this.UpdateVisualHammers();
          this.Fire(b);
          if (!this.FireAllBarrels)
            break;
        }
      }
    }

    public bool Fire(int b)
    {
      this.m_curBarrel = b;
      if (!this.Barrels[b].Chamber.Fire())
        return false;
      this.Fire(this.Barrels[b].Chamber, this.GetMuzzle(), true);
      this.FireMuzzleSmoke(this.Barrels[b].MuzzleIndexBarrelFire);
      this.FireMuzzleSmoke(this.Barrels[b].MuzzleIndexBarrelSmoke);
      this.AddGas(this.Barrels[b].GasOutIndexBarrel);
      this.AddGas(this.Barrels[b].GasOutIndexBreach);
      this.Recoil(this.IsTwoHandStabilized(), this.IsForegripStabilized(), this.IsShoulderStabilized());
      this.PlayAudioGunShot(this.Barrels[b].Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
      if (GM.CurrentSceneSettings.IsAmmoInfinite || GM.CurrentPlayerBody.IsInfiniteAmmo)
      {
        this.Barrels[b].Chamber.IsSpent = false;
        this.Barrels[b].Chamber.UpdateProxyDisplay();
      }
      return true;
    }

    private void UpdateVisualHammers()
    {
      for (int index = 0; index < this.Barrels.Length; ++index)
      {
        if (this.Barrels[index].HasVisibleHammer)
        {
          if (this.Barrels[index].m_isHammerCocked)
            this.SetAnimatedComponent(this.Barrels[index].Hammer, this.Barrels[index].HammerCocked, this.Barrels[index].HammerInterpStyle, this.Barrels[index].HammerAxis);
          else
            this.SetAnimatedComponent(this.Barrels[index].Hammer, this.Barrels[index].HammerUncocked, this.Barrels[index].HammerInterpStyle, this.Barrels[index].HammerAxis);
        }
      }
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      this.UpdateInputAndAnimate(hand);
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      base.EndInteraction(hand);
      this.IsLatchHeldOpen = false;
    }

    private void UpdateInputAndAnimate(FVRViveHand hand)
    {
      this.IsLatchHeldOpen = false;
      if (this.IsAltHeld)
        return;
      this.m_triggerFloat = !this.m_hasTriggeredUpSinceBegin ? 0.0f : hand.Input.TriggerFloat;
      if (!this.HasTriggerReset && (double) this.m_triggerFloat <= 0.449999988079071)
      {
        this.HasTriggerReset = true;
        this.PlayAudioEvent(FirearmAudioEventType.TriggerReset);
      }
      Vector2 touchpadAxes = hand.Input.TouchpadAxes;
      if (hand.IsInStreamlinedMode)
      {
        if (hand.Input.BYButtonPressed)
        {
          this.IsLatchHeldOpen = true;
          this.m_latchRot = 1f * this.MaxRotExtent;
        }
        else
          this.m_latchRot = Mathf.MoveTowards(this.m_latchRot, 0.0f, (float) ((double) Time.deltaTime * (double) this.MaxRotExtent * 3.0));
        if (hand.Input.AXButtonDown && this.UsesManuallyCockedHammers)
          this.CockHammer();
      }
      else
      {
        if (hand.Input.TouchpadPressed && (double) touchpadAxes.y > 0.100000001490116)
        {
          this.IsLatchHeldOpen = true;
          this.m_latchRot = touchpadAxes.y * this.MaxRotExtent;
        }
        else
          this.m_latchRot = Mathf.MoveTowards(this.m_latchRot, 0.0f, (float) ((double) Time.deltaTime * (double) this.MaxRotExtent * 3.0));
        if (hand.Input.TouchpadDown && this.UsesManuallyCockedHammers && (double) touchpadAxes.y < 0.100000001490116)
          this.CockHammer();
      }
      if (this.UsesManuallyCockedHammers && this.IsHeld && (UnityEngine.Object) this.m_hand.OtherHand != (UnityEngine.Object) null)
      {
        Vector3 velLinearWorld = this.m_hand.OtherHand.Input.VelLinearWorld;
        if ((double) Vector3.Distance(this.m_hand.OtherHand.PalmTransform.position, this.Barrels[0].Hammer.position) < 0.150000005960464 && (double) Vector3.Angle(velLinearWorld, -this.transform.forward) < 60.0 && (double) velLinearWorld.magnitude > 1.0)
          this.CockAllHammers();
      }
      if (this.m_hasLatch)
        this.Latch.localEulerAngles = new Vector3(0.0f, this.m_latchRot, 0.0f);
      for (int index = 0; index < this.Triggers.Length; ++index)
      {
        if (Mathf.Clamp(this.m_curBarrel, 0, this.Triggers.Length - 1) == index)
          this.SetAnimatedComponent(this.Triggers[index], Mathf.Lerp(this.TriggerUnpulled, this.TriggerPulled, this.m_triggerFloat), this.TriggerInterpStyle, this.TriggerAxis);
        else
          this.SetAnimatedComponent(this.Triggers[index], this.TriggerUnpulled, this.TriggerInterpStyle, this.TriggerAxis);
      }
      if ((double) this.m_triggerFloat < 0.899999976158142 || !this.HasTriggerReset || !this.m_isLatched)
        return;
      this.HasTriggerReset = false;
      this.DropHammer();
    }

    protected override void FVRFixedUpdate()
    {
      base.FVRFixedUpdate();
      for (int index = 0; index < this.Barrels.Length; ++index)
      {
        this.GasOutEffects[this.Barrels[index].GasOutIndexBarrel].GasUpdate(true);
        if (this.Barrels[index].Chamber.IsFull || !this.Barrels[index].Chamber.IsAccessible)
          this.GasOutEffects[this.Barrels[index].GasOutIndexBreach].GasUpdate(false);
        else
          this.GasOutEffects[this.Barrels[index].GasOutIndexBreach].GasUpdate(true);
      }
      if (this.m_isLatched && (double) Mathf.Abs(this.m_latchRot) > 5.0)
      {
        this.m_isLatched = false;
        this.PlayAudioEvent(FirearmAudioEventType.BreachOpen);
        JointLimits limits = this.Hinge.limits;
        limits.max = 45f;
        this.Hinge.limits = limits;
        for (int index = 0; index < this.Barrels.Length; ++index)
          this.Barrels[index].Chamber.IsAccessible = true;
      }
      if (this.m_isLatched)
        return;
      if (!this.IsLatchHeldOpen && ((double) this.Hinge.transform.localEulerAngles.x <= 1.0 && (double) Mathf.Abs(this.m_latchRot) < 5.0))
      {
        this.m_isLatched = true;
        this.PlayAudioEvent(FirearmAudioEventType.BreachClose);
        JointLimits limits = this.Hinge.limits;
        limits.max = 0.0f;
        this.Hinge.limits = limits;
        for (int index = 0; index < this.Barrels.Length; ++index)
          this.Barrels[index].Chamber.IsAccessible = false;
        this.Hinge.transform.localPosition = this.m_foreStartPos;
      }
      if ((double) Mathf.Abs(this.Hinge.transform.localEulerAngles.x) < 30.0)
        return;
      this.PopOutEmpties();
      if (this.UsesManuallyCockedHammers)
        return;
      this.CockAllHammers();
    }

    public override List<FireArmRoundClass> GetChamberRoundList()
    {
      bool flag = false;
      List<FireArmRoundClass> fireArmRoundClassList = new List<FireArmRoundClass>();
      for (int index = 0; index < this.Barrels.Length; ++index)
      {
        if (this.Barrels[index].Chamber.IsFull)
        {
          fireArmRoundClassList.Add(this.Barrels[index].Chamber.GetRound().RoundClass);
          flag = true;
        }
      }
      return flag ? fireArmRoundClassList : (List<FireArmRoundClass>) null;
    }

    public override void SetLoadedChambers(List<FireArmRoundClass> rounds)
    {
      if (rounds.Count <= 0)
        return;
      for (int index = 0; index < this.Barrels.Length; ++index)
      {
        if (index < rounds.Count)
          this.Barrels[index].Chamber.Autochamber(rounds[index]);
      }
    }

    public override List<string> GetFlagList() => (List<string>) null;

    public override void SetFromFlagList(List<string> flags)
    {
    }

    public override void ConfigureFromFlagDic(Dictionary<string, string> f)
    {
      for (int index = 0; index < this.Barrels.Length; ++index)
      {
        string key = "HammerState_" + (object) index;
        if (f.ContainsKey(key) && f[key] == "Cocked")
        {
          this.Barrels[index].m_isHammerCocked = true;
          this.UpdateVisualHammers();
        }
      }
    }

    public override Dictionary<string, string> GetFlagDic()
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      for (int index = 0; index < this.Barrels.Length; ++index)
      {
        string key = "HammerState_" + (object) index;
        string str = "Uncocked";
        if (this.Barrels[index].m_isHammerCocked)
          str = "Cocked";
        dictionary.Add(key, str);
      }
      return dictionary;
    }

    [Serializable]
    public class BreakActionBarrel
    {
      public FVRFireArmChamber Chamber;
      public Transform Hammer;
      public Transform Muzzle;
      public bool HasVisibleHammer;
      public FVRPhysicalObject.InterpStyle HammerInterpStyle = FVRPhysicalObject.InterpStyle.Rotation;
      public FVRPhysicalObject.Axis HammerAxis;
      public float HammerUncocked;
      public float HammerCocked;
      public int MuzzleIndexBarrelFire;
      public int MuzzleIndexBarrelSmoke;
      public int GasOutIndexBarrel;
      public int GasOutIndexBreach;
      [HideInInspector]
      public bool IsBreachOpenForGasOut;
      [HideInInspector]
      public bool m_isHammerCocked;
    }
  }
}
