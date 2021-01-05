// Decompiled with JetBrains decompiler
// Type: FistVR.wwGatlingControlHandle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwGatlingControlHandle : FVRInteractiveObject
  {
    public Transform BaseFrame;
    public Transform MountingBracket;
    public float HorizontalClamp = 25f;
    public float VerticalClamp = 20f;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      Vector3 vector = -(hand.transform.position - this.transform.position);
      Vector3 forward1 = Vector3.RotateTowards(this.BaseFrame.forward, Vector3.ProjectOnPlane(vector, this.BaseFrame.up), this.HorizontalClamp * 0.0174533f, 0.0f);
      Vector3 forward2 = Vector3.RotateTowards(this.MountingBracket.forward, Vector3.ProjectOnPlane(vector, this.MountingBracket.right), this.VerticalClamp * 0.0174533f, 0.0f);
      this.MountingBracket.rotation = Quaternion.Slerp(this.MountingBracket.rotation, Quaternion.LookRotation(forward1, Vector3.up), Time.deltaTime * 4f);
      this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(forward2, Vector3.up), Time.deltaTime * 4f);
    }
  }
}
