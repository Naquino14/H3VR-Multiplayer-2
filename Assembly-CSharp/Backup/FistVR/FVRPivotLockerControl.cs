// Decompiled with JetBrains decompiler
// Type: FistVR.FVRPivotLockerControl
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRPivotLockerControl : FVRInteractiveObject
  {
    public FVRPivotLocker Locker;
    public string Axis = "X";
    private Vector3 m_lastPos = Vector3.zero;
    public bool isRotControl;

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      this.m_lastPos = this.Locker.transform.InverseTransformPoint(hand.Input.Pos);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      Vector3 vector3 = this.Locker.transform.InverseTransformPoint(hand.Input.Pos) - this.m_lastPos;
      if (!this.isRotControl)
      {
        switch (this.Axis)
        {
          case "X":
            vector3 = Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(vector3, Vector3.up), Vector3.forward);
            break;
          case "Y":
            vector3 = Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(vector3, Vector3.right), Vector3.forward);
            break;
          case "Z":
            vector3 = Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(vector3, Vector3.right), Vector3.up);
            break;
        }
        this.Locker.SlideOnAxis(vector3);
      }
      else
      {
        switch (this.Axis)
        {
          case "X":
            this.Locker.RotateOnAxis("X", Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(vector3, Vector3.right), Vector3.up).z * 45f);
            break;
          case "Y":
            this.Locker.RotateOnAxis("Y", Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(vector3, Vector3.up), Vector3.forward).x * 45f);
            break;
          case "Z":
            this.Locker.RotateOnAxis("Z", (float) (-(double) Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(vector3, Vector3.up), Vector3.forward).x * 45.0));
            break;
        }
      }
      this.m_lastPos = this.Locker.transform.InverseTransformPoint(hand.Input.Pos);
    }
  }
}
