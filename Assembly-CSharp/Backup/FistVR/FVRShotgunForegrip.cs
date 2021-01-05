// Decompiled with JetBrains decompiler
// Type: FistVR.FVRShotgunForegrip
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRShotgunForegrip : FVRAlternateGrip
  {
    public Transform ShotgunBase;
    public HingeJoint Hinge;
    private Vector3 localPosStart;
    private Rigidbody RB;
    private BreakActionWeapon Wep;

    protected override void Awake()
    {
      base.Awake();
      this.localPosStart = this.Hinge.transform.localPosition;
      this.RB = this.Hinge.gameObject.GetComponent<Rigidbody>();
      this.Wep = this.Hinge.connectedBody.gameObject.GetComponent<BreakActionWeapon>();
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if ((double) Vector3.Distance(this.Hinge.transform.localPosition, this.localPosStart) <= 0.00999999977648258)
        return;
      this.Hinge.transform.localPosition = this.localPosStart;
    }

    protected override void FVRFixedUpdate()
    {
      base.FVRFixedUpdate();
      if (this.Wep.IsHeld && this.Wep.IsAltHeld)
        this.RB.mass = 1f / 1000f;
      else
        this.RB.mass = 0.1f;
    }

    public override bool IsInteractable() => true;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      Vector3 from = Vector3.ProjectOnPlane(hand.transform.position - this.Hinge.transform.position, this.ShotgunBase.right);
      if ((double) Vector3.Angle(from, -this.ShotgunBase.up) > 90.0)
        from = this.ShotgunBase.forward;
      if ((double) Vector3.Angle(from, this.ShotgunBase.forward) > 90.0)
        from = -this.ShotgunBase.up;
      float num = Vector3.Angle(from, this.ShotgunBase.forward);
      JointSpring spring = this.Hinge.spring;
      spring.spring = 10f;
      spring.damper = 0.0f;
      spring.targetPosition = Mathf.Clamp(num, 0.0f, this.Hinge.limits.max);
      this.Hinge.spring = spring;
      this.Hinge.transform.localPosition = this.localPosStart;
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      JointSpring spring = this.Hinge.spring;
      spring.spring = 0.5f;
      spring.damper = 0.05f;
      spring.targetPosition = 45f;
      this.Hinge.spring = spring;
      base.EndInteraction(hand);
    }
  }
}
