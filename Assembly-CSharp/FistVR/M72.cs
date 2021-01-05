// Decompiled with JetBrains decompiler
// Type: FistVR.M72
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class M72 : FVRFireArm
  {
    [Header("M72 Params")]
    public FVRFireArmChamber Chamber;
    public Transform Point_BackBlast;
    public bool AlsoPlaysSuppressedSound = true;
    public bool DeletesCartridgeOnFire = true;
    [Header("Trigger Params")]
    public Transform Trigger;
    public FVRPhysicalObject.Axis Trigger_Axis;
    public FVRPhysicalObject.InterpStyle Trigger_Interp;
    public Vector2 Trigger_ValRange;
    [Header("Cap Params")]
    public Transform Cap;
    public FVRPhysicalObject.Axis Cap_Axis;
    public FVRPhysicalObject.InterpStyle Cap_Interp;
    public Vector2 Cap_ValRange;
    [Header("Tube Params")]
    public Transform Tube;
    public Transform Tube_Front;
    public Transform Tube_Rear;
    [Header("Safety Params")]
    public Transform Safety;
    public FVRPhysicalObject.Axis Safety_Axis;
    public FVRPhysicalObject.InterpStyle Safety_Interp;
    public Vector2 Safety_ValRange;
    [Header("RearSight Params")]
    public Transform RearSight;
    public FVRPhysicalObject.Axis RearSight_Axis;
    public FVRPhysicalObject.InterpStyle RearSight_Interp;
    public Vector2 RearSight_ValRange;
    private bool m_isSafetyEngaged = true;
    private bool m_isCapOpen;
    public M72.TubeState TState;
    private float m_triggerVal;

    public bool CanCapBeToggled() => this.TState == M72.TubeState.Forward;

    public bool CanTubeBeGrabbed() => this.m_isCapOpen;

    public void ToggleSafety()
    {
      this.m_isSafetyEngaged = !this.m_isSafetyEngaged;
      if (this.m_isSafetyEngaged)
        this.SetAnimatedComponent(this.Safety, this.Safety_ValRange.x, this.Safety_Interp, this.Safety_Axis);
      else
        this.SetAnimatedComponent(this.Safety, this.Safety_ValRange.y, this.Safety_Interp, this.Safety_Axis);
      this.PlayAudioEvent(FirearmAudioEventType.FireSelector);
    }

    public void PopUpRearSight() => this.SetAnimatedComponent(this.RearSight, this.RearSight_ValRange.y, this.RearSight_Interp, this.RearSight_Axis);

    public void ToggleCap()
    {
      if (this.CanCapBeToggled())
        this.m_isCapOpen = !this.m_isCapOpen;
      if (this.m_isCapOpen)
      {
        this.SetAnimatedComponent(this.Cap, this.Cap_ValRange.y, this.Cap_Interp, this.Cap_Axis);
        this.PlayAudioEvent(FirearmAudioEventType.BreachOpen);
      }
      else
      {
        this.SetAnimatedComponent(this.Cap, this.Cap_ValRange.x, this.Cap_Interp, this.Cap_Axis);
        this.PlayAudioEvent(FirearmAudioEventType.BreachClose);
      }
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (!this.IsAltHeld && hand.Input.TriggerDown && (this.m_hasTriggeredUpSinceBegin && !this.m_isSafetyEngaged) && this.TState == M72.TubeState.Rear)
        this.Fire();
      if ((double) this.m_triggerVal == (double) hand.Input.TriggerFloat)
        return;
      this.m_triggerVal = hand.Input.TriggerFloat;
      this.SetAnimatedComponent(this.Trigger, Mathf.Lerp(this.Trigger_ValRange.x, this.Trigger_ValRange.y, this.m_triggerVal), this.Trigger_Interp, this.Trigger_Axis);
    }

    public void Fire()
    {
      if (!this.Chamber.IsFull || this.Chamber.IsSpent)
        return;
      this.Chamber.Fire();
      this.Fire(this.Chamber, this.GetMuzzle(), true);
      this.FireMuzzleSmoke();
      this.Recoil(this.IsTwoHandStabilized(), (Object) this.AltGrip != (Object) null, this.IsShoulderStabilized());
      this.PlayAudioGunShot(this.Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
      if (this.AlsoPlaysSuppressedSound)
        this.PlayAudioEvent(FirearmAudioEventType.Shots_Suppressed);
      if (GM.CurrentSceneSettings.IsAmmoInfinite || GM.CurrentPlayerBody.IsInfiniteAmmo)
      {
        this.Chamber.IsSpent = false;
        this.Chamber.UpdateProxyDisplay();
      }
      else
      {
        if (!this.DeletesCartridgeOnFire)
          return;
        this.Chamber.SetRound((FVRFireArmRound) null);
      }
    }

    public enum TubeState
    {
      Forward,
      Mid,
      Rear,
    }
  }
}
