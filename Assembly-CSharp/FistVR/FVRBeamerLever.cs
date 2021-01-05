// Decompiled with JetBrains decompiler
// Type: FistVR.FVRBeamerLever
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRBeamerLever : FVRInteractiveObject
  {
    [Header("Beamer Lever Config")]
    public Transform Holder;
    public HingeJoint Hinge;
    public FVRBeamer Beamer;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      Vector3 from = Vector3.ProjectOnPlane(hand.transform.position - this.Holder.position, this.Holder.right);
      Debug.DrawLine(this.Holder.position, this.Holder.position + from, Color.green);
      float num = Vector3.Angle(from, this.Holder.forward);
      float l = Mathf.Clamp(Vector3.Angle(from, this.Holder.up), 0.0f, 50f);
      if ((double) num > 90.0)
      {
        JointSpring spring = this.Hinge.spring;
        spring.targetPosition = -l;
        this.Hinge.spring = spring;
        this.Beamer.SetLocusMover(-l);
      }
      else
      {
        JointSpring spring = this.Hinge.spring;
        spring.targetPosition = l;
        this.Hinge.spring = spring;
        this.Beamer.SetLocusMover(l);
      }
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      base.EndInteraction(hand);
      JointSpring spring = this.Hinge.spring;
      spring.targetPosition = 0.0f;
      this.Hinge.spring = spring;
      this.Beamer.SetLocusMover(0.0f);
    }
  }
}
