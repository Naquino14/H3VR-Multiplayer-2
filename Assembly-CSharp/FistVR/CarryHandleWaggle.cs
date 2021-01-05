// Decompiled with JetBrains decompiler
// Type: FistVR.CarryHandleWaggle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class CarryHandleWaggle : AttachableForegrip
  {
    public FVRAlternateGrip MainForeGrip;
    public WaggleJoint Waggle;
    public Vector3 GrabEuler;
    private bool m_istranslatingGrabPoint;
    private Vector3 poseLerpLocalStart = Vector3.zero;
    private Vector3 poseLerpLocalEnd = Vector3.zero;
    private Vector3 forwardLerpLocalStart = Vector3.zero;
    private Vector3 forwardLerpLocalEnd = Vector3.zero;
    private Vector3 upLerpLocalStart = Vector3.zero;
    private Vector3 upLerpLocalEnd = Vector3.zero;
    private float poseLerp;

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      this.poseLerpLocalStart = this.PoseOverride.parent.InverseTransformPoint(this.MainForeGrip.PrimaryObject.GrabPointTransform.position);
      this.poseLerpLocalEnd = this.PoseOverride.localPosition;
      this.forwardLerpLocalStart = this.PoseOverride.parent.InverseTransformDirection(this.MainForeGrip.PrimaryObject.GrabPointTransform.forward);
      this.forwardLerpLocalEnd = this.PoseOverride.parent.InverseTransformDirection(this.PoseOverride.forward);
      this.upLerpLocalStart = this.PoseOverride.parent.InverseTransformDirection(this.MainForeGrip.PrimaryObject.GrabPointTransform.up);
      this.upLerpLocalEnd = this.PoseOverride.parent.InverseTransformDirection(this.PoseOverride.up);
      this.poseLerp = 0.0f;
      this.m_istranslatingGrabPoint = true;
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      base.EndInteraction(hand);
      this.Waggle.ResetParticlePos();
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      bool flag = false;
      if (this.m_istranslatingGrabPoint)
      {
        if ((double) this.poseLerp < 1.0)
        {
          this.poseLerp += Time.deltaTime * 4f;
          this.MainForeGrip.PrimaryObject.GrabPointTransform.position = this.PoseOverride.parent.TransformPoint(Vector3.Lerp(this.poseLerpLocalStart, this.poseLerpLocalEnd, this.poseLerp));
          Vector3 direction1 = Vector3.Lerp(this.forwardLerpLocalStart, this.forwardLerpLocalEnd, this.poseLerp);
          Vector3 direction2 = Vector3.Lerp(this.upLerpLocalStart, this.upLerpLocalEnd, this.poseLerp);
          this.MainForeGrip.PrimaryObject.GrabPointTransform.rotation = Quaternion.LookRotation(this.PoseOverride.parent.TransformDirection(direction1), this.PoseOverride.parent.TransformDirection(direction2));
        }
        else
          this.m_istranslatingGrabPoint = false;
      }
      if ((Object) this.MainForeGrip.LastGrabbedInGrip == (Object) this && (this.MainForeGrip.IsHeld || this.MainForeGrip.PrimaryObject.IsAltHeld))
        flag = true;
      if (flag)
        this.Waggle.hingeGraphic.localEulerAngles = this.GrabEuler;
      else
        this.Waggle.Execute();
    }
  }
}
