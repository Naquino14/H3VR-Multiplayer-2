// Decompiled with JetBrains decompiler
// Type: FistVR.RollingBlock
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class RollingBlock : FVRFireArm
  {
    [Header("RollingBlock Params")]
    public FVRFireArmChamber Chamber;
    public Transform Trigger;
    public Vector2 TriggerRots;
    public Transform Hammer;
    public Vector2 HammerRots;
    private float m_curHammerRot;
    private float m_tarHammerRot;
    public Transform BreachBlock;
    public Vector2 BreachBlockRots;
    private float m_curBreachRot;
    private float m_tarBreachRot;
    public Transform EjectPos;
    private RollingBlock.RollingBlockState m_state;
    private Vector2 m_pressDownPoint = Vector2.zero;
    private bool m_isPressedDown;

    protected override void Awake()
    {
      base.Awake();
      this.IsBreachOpenForGasOut = false;
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      this.m_curHammerRot = Mathf.Lerp(this.m_curHammerRot, this.m_tarHammerRot, Time.deltaTime * 8f);
      this.m_curBreachRot = Mathf.Lerp(this.m_curBreachRot, this.m_tarBreachRot, Time.deltaTime * 12f);
      this.Hammer.localEulerAngles = new Vector3(this.m_curHammerRot, 0.0f, 0.0f);
      this.BreachBlock.localEulerAngles = new Vector3(this.m_curBreachRot, 0.0f, 0.0f);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      Vector2 touchpadAxes = hand.Input.TouchpadAxes;
      if (hand.IsInStreamlinedMode)
      {
        if (hand.Input.BYButtonDown)
          this.ClickUpward();
        if (hand.Input.AXButtonDown)
          this.ClickDownward();
      }
      else if (hand.Input.TouchpadDown)
      {
        if ((double) touchpadAxes.y > 0.0)
          this.ClickUpward();
        else
          this.ClickDownward();
      }
      float triggerFloat = hand.Input.TriggerFloat;
      if (this.m_state != RollingBlock.RollingBlockState.HammerBackBreachClosed || !hand.Input.TriggerDown || (!this.m_hasTriggeredUpSinceBegin || this.m_state == RollingBlock.RollingBlockState.HammerForward))
        return;
      this.m_isPressedDown = false;
      this.DropHammer();
      this.m_state = RollingBlock.RollingBlockState.HammerForward;
    }

    private void ClickUpward()
    {
      switch (this.m_state)
      {
        case RollingBlock.RollingBlockState.HammerBackBreachClosed:
          this.m_state = RollingBlock.RollingBlockState.HammerForward;
          this.DecockHammer();
          break;
        case RollingBlock.RollingBlockState.HammerBackBreachOpen:
          this.m_state = RollingBlock.RollingBlockState.HammerBackBreachClosed;
          this.CloseBreach();
          break;
      }
    }

    private void ClickDownward()
    {
      switch (this.m_state)
      {
        case RollingBlock.RollingBlockState.HammerForward:
          this.CockHammer();
          this.m_state = RollingBlock.RollingBlockState.HammerBackBreachClosed;
          break;
        case RollingBlock.RollingBlockState.HammerBackBreachClosed:
          this.OpenBreach();
          this.m_state = RollingBlock.RollingBlockState.HammerBackBreachOpen;
          break;
      }
    }

    private void CockHammer()
    {
      this.m_tarHammerRot = this.HammerRots.y;
      this.PlayAudioEvent(FirearmAudioEventType.Prefire);
    }

    private void DecockHammer()
    {
      this.m_tarHammerRot = this.HammerRots.x;
      this.PlayAudioEvent(FirearmAudioEventType.BreachClose);
    }

    private void OpenBreach()
    {
      this.m_tarBreachRot = this.BreachBlockRots.y;
      this.PlayAudioEvent(FirearmAudioEventType.BreachOpen);
      if (this.Chamber.IsFull)
        this.Chamber.EjectRound(this.EjectPos.position, this.transform.forward * -1f + this.transform.up * 0.5f, Vector3.right * 270f);
      this.IsBreachOpenForGasOut = true;
      this.Chamber.IsAccessible = true;
    }

    private void CloseBreach()
    {
      this.m_tarBreachRot = this.BreachBlockRots.x;
      this.PlayAudioEvent(FirearmAudioEventType.BreachClose);
      this.IsBreachOpenForGasOut = false;
      this.Chamber.IsAccessible = false;
    }

    private void DropHammer()
    {
      this.m_tarHammerRot = this.HammerRots.x;
      this.m_curHammerRot = this.HammerRots.x;
      this.PlayAudioEvent(FirearmAudioEventType.HammerHit);
      if (!this.Chamber.IsFull)
        return;
      this.Fire();
    }

    private void Fire()
    {
      if (!this.Chamber.Fire())
        return;
      this.Fire(this.Chamber, this.GetMuzzle(), true);
      this.FireMuzzleSmoke();
      this.Recoil(this.IsTwoHandStabilized(), (Object) this.AltGrip != (Object) null, this.IsShoulderStabilized());
      this.PlayAudioGunShot(this.Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
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

    public enum RollingBlockState
    {
      HammerForward,
      HammerBackBreachClosed,
      HammerBackBreachOpen,
    }
  }
}
