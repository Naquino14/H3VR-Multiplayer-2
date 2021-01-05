// Decompiled with JetBrains decompiler
// Type: FistVR.MF2_ClampedRotatePiece
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MF2_ClampedRotatePiece : FVRInteractiveObject
  {
    public Transform RefFrameForward;
    public Vector2 RotRange = new Vector2(-50f, 50f);
    private float m_rot;
    public Transform Rotpiece;
    public FVRPhysicalObject.Axis RotAxis;
    private Vector3 refDir = Vector3.one;

    public override void BeginInteraction(FVRViveHand hand)
    {
      this.refDir = Vector3.ProjectOnPlane(hand.Input.Up, this.RefFrameForward.forward).normalized;
      base.BeginInteraction(hand);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      Vector3 normalized = Vector3.ProjectOnPlane(hand.Input.Up, this.RefFrameForward.forward).normalized;
      Vector3 refDir = this.refDir;
      this.m_rot += Mathf.Clamp(Mathf.Atan2(Vector3.Dot(this.RefFrameForward.forward, Vector3.Cross(refDir, normalized)), Vector3.Dot(refDir, normalized)) * 57.29578f, -5f, 5f);
      this.m_rot = Mathf.Clamp(this.m_rot, this.RotRange.x, this.RotRange.y);
      if (this.RotAxis == FVRPhysicalObject.Axis.X)
        this.Rotpiece.localEulerAngles = new Vector3(this.m_rot, 0.0f, 0.0f);
      else if (this.RotAxis == FVRPhysicalObject.Axis.Y)
        this.Rotpiece.localEulerAngles = new Vector3(0.0f, this.m_rot, 0.0f);
      else if (this.RotAxis == FVRPhysicalObject.Axis.Z)
        this.Rotpiece.localEulerAngles = new Vector3(0.0f, 0.0f, this.m_rot);
      this.refDir = normalized;
      base.UpdateInteraction(hand);
    }
  }
}
