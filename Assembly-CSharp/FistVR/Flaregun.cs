// Decompiled with JetBrains decompiler
// Type: FistVR.Flaregun
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class Flaregun : FVRFireArm
  {
    [Header("Flaregun Params")]
    public Renderer[] GunUndamaged;
    public Renderer[] GunDamaged;
    public FVRFireArmChamber Chamber;
    public FVRPhysicalObject.Axis HingeAxis;
    public Transform Hinge;
    public float RotOut = 35f;
    public bool CanUnlatch = true;
    public bool CanFlick = true;
    public bool IsHighPressureTolerant;
    private bool m_isHammerCocked;
    private bool m_isTriggerReset = true;
    private bool m_isLatched = true;
    private bool m_isDestroyed;
    private float TriggerFloat;
    public Transform Hammer;
    public bool HasVisibleHammer = true;
    public bool CanCockHammer = true;
    public bool CocksOnOpen;
    private float m_hammerXRot;
    public FVRPhysicalObject.Axis HammerAxis;
    public FVRPhysicalObject.InterpStyle HammerInterp = FVRPhysicalObject.InterpStyle.Rotation;
    public float HammerMinRot;
    public float HammerMaxRot = -70f;
    public Transform Trigger;
    public Vector2 TriggerForwardBackRots;
    public Transform Muzzle;
    public ParticleSystem SmokePSystem;
    public ParticleSystem DestroyPSystem;
    public bool DeletesCartridgeOnFire;
    public bool IsFallingBlock;
    public Transform FallingBlock;
    public Vector3 FallingBlockPos_Up;
    public Vector3 FallingBlockPos_Down;

    protected override void Awake()
    {
      base.Awake();
      if (this.CanUnlatch)
        this.Chamber.IsAccessible = false;
      else
        this.Chamber.IsAccessible = true;
    }

    protected override void FVRUpdate()
    {
      if (this.HasVisibleHammer)
      {
        this.m_hammerXRot = !this.m_isHammerCocked ? Mathf.Lerp(this.m_hammerXRot, 0.0f, Time.deltaTime * 25f) : Mathf.Lerp(this.m_hammerXRot, this.HammerMaxRot, Time.deltaTime * 12f);
        this.Hammer.localEulerAngles = new Vector3(this.m_hammerXRot, 0.0f, 0.0f);
      }
      if (this.m_isLatched || (double) Vector3.Angle(Vector3.up, this.Chamber.transform.forward) >= 70.0 || (!this.Chamber.IsFull || !this.Chamber.IsSpent))
        return;
      this.PlayAudioEvent(FirearmAudioEventType.MagazineEjectRound);
      this.Chamber.EjectRound(this.Chamber.transform.position + this.Chamber.transform.forward * -0.08f, this.Chamber.transform.forward * -0.01f, Vector3.right);
    }

    public void ToggleLatchState()
    {
      if (this.m_isLatched)
      {
        this.Unlatch();
      }
      else
      {
        if (this.m_isLatched)
          return;
        this.Latch();
      }
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      float num = 0.0f;
      switch (this.HingeAxis)
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
      if ((double) num > 15.0 && this.CanUnlatch && (!this.m_isHammerCocked && this.CanFlick))
        this.Unlatch();
      else if ((double) num < -15.0 && this.CanUnlatch && this.CanFlick)
        this.Latch();
      bool flag1 = false;
      bool flag2 = false;
      if (!this.IsAltHeld)
      {
        if (hand.IsInStreamlinedMode)
        {
          if (hand.Input.BYButtonDown)
            flag2 = true;
          if (hand.Input.AXButtonDown)
            flag1 = true;
        }
        else if (hand.Input.TouchpadDown)
        {
          Vector2 touchpadAxes = hand.Input.TouchpadAxes;
          if ((double) touchpadAxes.magnitude > 0.200000002980232 && (double) Vector2.Angle(touchpadAxes, Vector2.down) < 45.0 && this.CanCockHammer)
            this.CockHammer();
          else if ((double) touchpadAxes.magnitude > 0.200000002980232 && ((double) Vector2.Angle(touchpadAxes, Vector2.left) < 45.0 || (double) Vector2.Angle(touchpadAxes, Vector2.right) < 45.0) && this.CanUnlatch)
            this.ToggleLatchState();
        }
      }
      if (flag1)
        this.CockHammer();
      if (flag2)
        this.ToggleLatchState();
      if (this.m_isDestroyed)
        return;
      this.TriggerFloat = !this.m_hasTriggeredUpSinceBegin || this.IsAltHeld ? 0.0f : hand.Input.TriggerFloat;
      this.Trigger.localEulerAngles = new Vector3(Mathf.Lerp(this.TriggerForwardBackRots.x, this.TriggerForwardBackRots.y, this.TriggerFloat), 0.0f, 0.0f);
      if ((double) this.TriggerFloat > 0.699999988079071)
      {
        if (!this.m_isTriggerReset || !this.m_isHammerCocked)
          return;
        this.m_isTriggerReset = false;
        this.m_isHammerCocked = false;
        if ((Object) this.Hammer != (Object) null)
          this.SetAnimatedComponent(this.Hammer, this.HammerMinRot, this.HammerInterp, this.HammerAxis);
        this.PlayAudioEvent(FirearmAudioEventType.HammerHit);
        this.Fire();
      }
      else
      {
        if ((double) this.TriggerFloat >= 0.200000002980232 || this.m_isTriggerReset)
          return;
        this.m_isTriggerReset = true;
      }
    }

    private void Fire()
    {
      if (!this.m_isLatched || !this.Chamber.Fire())
        return;
      this.Fire(this.Chamber, this.GetMuzzle(), true);
      this.FireMuzzleSmoke();
      bool twoHandStabilized = this.IsTwoHandStabilized();
      bool foregripStabilized = (Object) this.AltGrip != (Object) null;
      bool shoulderStabilized = this.IsShoulderStabilized();
      if (this.Chamber.GetRound().IsHighPressure && !this.IsHighPressureTolerant)
      {
        this.Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
        this.Destroy();
      }
      else if (this.Chamber.GetRound().IsHighPressure && this.IsHighPressureTolerant)
        this.Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
      this.PlayAudioGunShot(this.Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
      if (GM.CurrentSceneSettings.IsAmmoInfinite || GM.CurrentPlayerBody.IsInfiniteAmmo)
      {
        this.Chamber.IsSpent = false;
        this.Chamber.UpdateProxyDisplay();
      }
      else if (this.Chamber.GetRound().IsCaseless)
        this.Chamber.SetRound((FVRFireArmRound) null);
      if (!this.DeletesCartridgeOnFire)
        return;
      this.Chamber.SetRound((FVRFireArmRound) null);
    }

    private void Unlatch()
    {
      if (!this.m_isLatched)
        return;
      this.PlayAudioEvent(FirearmAudioEventType.BreachOpen);
      switch (this.HingeAxis)
      {
        case FVRPhysicalObject.Axis.X:
          this.Hinge.localEulerAngles = new Vector3(this.RotOut, 0.0f, 0.0f);
          break;
        case FVRPhysicalObject.Axis.Y:
          this.Hinge.localEulerAngles = new Vector3(0.0f, this.RotOut, 0.0f);
          break;
        case FVRPhysicalObject.Axis.Z:
          this.Hinge.localEulerAngles = new Vector3(0.0f, 0.0f, this.RotOut);
          break;
      }
      this.m_isLatched = false;
      this.Chamber.IsAccessible = true;
      if (this.CocksOnOpen)
        this.CockHammer();
      if (!this.IsFallingBlock)
        return;
      this.FallingBlock.localPosition = this.FallingBlockPos_Down;
    }

    private void Latch()
    {
      if (this.m_isLatched)
        return;
      this.PlayAudioEvent(FirearmAudioEventType.BreachClose);
      this.Hinge.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
      this.m_isLatched = true;
      this.Chamber.IsAccessible = false;
      if (!this.IsFallingBlock)
        return;
      this.FallingBlock.localPosition = this.FallingBlockPos_Up;
    }

    private void CockHammer()
    {
      if (this.m_isHammerCocked)
        return;
      this.PlayAudioEvent(FirearmAudioEventType.Prefire);
      this.m_isHammerCocked = true;
      if (!((Object) this.Hammer != (Object) null))
        return;
      this.SetAnimatedComponent(this.Hammer, this.HammerMaxRot, this.HammerInterp, this.HammerAxis);
    }

    private void Destroy()
    {
      if (this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      this.DestroyPSystem.Emit(25);
      for (int index = 0; index < this.GunUndamaged.Length; ++index)
      {
        this.GunUndamaged[index].enabled = false;
        this.GunDamaged[index].enabled = true;
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
  }
}
