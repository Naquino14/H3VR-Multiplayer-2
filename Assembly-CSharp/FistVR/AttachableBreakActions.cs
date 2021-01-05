// Decompiled with JetBrains decompiler
// Type: FistVR.AttachableBreakActions
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AttachableBreakActions : AttachableFirearm
  {
    [Header("Attachable Break Action")]
    public FVRFireArmChamber Chamber;
    public Transform Breach;
    public Vector2 BreachRots = new Vector2(0.0f, 30f);
    public Transform Trigger;
    public Vector2 TriggerRange;
    public Transform Ejector;
    public Vector2 EjectorRange = new Vector2(0.0f, 0.005f);
    private bool m_isEjectorForward = true;
    public Transform EjectPos;
    private bool m_isBreachOpen;
    public Transform InertialCloseDir;

    public override void ProcessInput(FVRViveHand hand, bool fromInterface, FVRInteractiveObject o)
    {
      if (o.m_hasTriggeredUpSinceBegin)
        this.Attachment.SetAnimatedComponent(this.Trigger, Mathf.Lerp(this.TriggerRange.x, this.TriggerRange.y, hand.Input.TriggerFloat), FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
      if (hand.Input.TriggerDown && o.m_hasTriggeredUpSinceBegin && !this.m_isBreachOpen)
        this.Fire(fromInterface);
      if (hand.IsInStreamlinedMode)
      {
        if (!hand.Input.BYButtonDown)
          return;
        this.ToggleBreach();
      }
      else
      {
        if (!hand.Input.TouchpadDown || !hand.Input.TouchpadWestPressed)
          return;
        this.ToggleBreach();
      }
    }

    public void Fire(bool firedFromInterface)
    {
      if (!this.Chamber.Fire())
        return;
      this.FireMuzzleSmoke();
      if (firedFromInterface)
      {
        FVRFireArm fa = this.Attachment.curMount.MyObject as FVRFireArm;
        if ((Object) fa != (Object) null)
        {
          fa.Recoil(fa.IsTwoHandStabilized(), fa.IsForegripStabilized(), fa.IsShoulderStabilized(), this.RecoilProfile);
          this.Fire(this.Chamber, this.MuzzlePos, true, fa);
        }
        else
          this.Fire(this.Chamber, this.MuzzlePos, true, (FVRFireArm) null);
      }
      else
      {
        Debug.Log((object) "Should fire");
        this.Recoil(false, false, false);
        this.Fire(this.Chamber, this.MuzzlePos, true, (FVRFireArm) null);
      }
      this.PlayAudioGunShot(this.Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
    }

    public void Eject()
    {
      this.Chamber.EjectRound(this.EjectPos.position, -this.EjectPos.forward * 1f, Vector3.zero, true);
      this.PlayAudioEvent(FirearmAudioEventType.MagazineEjectRound);
    }

    public void ToggleBreach()
    {
      this.m_isBreachOpen = !this.m_isBreachOpen;
      if (this.m_isBreachOpen)
      {
        this.Breach.localEulerAngles = new Vector3(this.BreachRots.y, 0.0f, 0.0f);
        this.Chamber.IsAccessible = true;
        this.PlayAudioEvent(FirearmAudioEventType.BreachOpen);
        if (!this.Chamber.IsFull || !this.Chamber.IsSpent)
          return;
        this.Eject();
      }
      else
      {
        this.Breach.localEulerAngles = new Vector3(this.BreachRots.x, 0.0f, 0.0f);
        this.Chamber.IsAccessible = false;
        this.PlayAudioEvent(FirearmAudioEventType.BreachClose);
      }
    }

    private void Update()
    {
      if (this.m_isEjectorForward)
      {
        if (!this.m_isBreachOpen)
          return;
        this.m_isEjectorForward = false;
        this.Attachment.SetAnimatedComponent(this.Ejector, this.EjectorRange.x, FVRPhysicalObject.InterpStyle.Translate, FVRPhysicalObject.Axis.Z);
      }
      else
      {
        if (this.m_isBreachOpen)
          return;
        this.m_isEjectorForward = true;
        this.Attachment.SetAnimatedComponent(this.Ejector, this.EjectorRange.y, FVRPhysicalObject.InterpStyle.Translate, FVRPhysicalObject.Axis.Z);
      }
    }

    public void FixedUpdate()
    {
      bool flag = false;
      Vector3 to = Vector3.zero;
      if (this.Attachment.IsHeld)
      {
        flag = true;
        to = this.Attachment.m_hand.Input.VelLinearWorld;
      }
      else if ((Object) this.Attachment.curMount != (Object) null && (Object) this.Attachment.curMount.GetRootMount().MyObject != (Object) null && this.Attachment.curMount.GetRootMount().MyObject.IsHeld)
      {
        flag = true;
        to = this.Attachment.curMount.GetRootMount().MyObject.m_hand.Input.VelLinearWorld;
      }
      if (!flag || !this.m_isBreachOpen || ((double) Vector3.Angle(this.InertialCloseDir.forward, to) >= 80.0 || (double) to.magnitude <= 2.5))
        return;
      this.ToggleBreach();
    }
  }
}
