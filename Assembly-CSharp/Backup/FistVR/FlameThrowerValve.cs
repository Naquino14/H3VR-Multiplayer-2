// Decompiled with JetBrains decompiler
// Type: FistVR.FlameThrowerValve
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FlameThrowerValve : FVRInteractiveObject
  {
    public float MaxRot = 20f;
    public Transform RefVector;
    public float ValvePos = 0.5f;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      Vector3 vector3_1 = Vector3.ProjectOnPlane(hand.transform.position - this.transform.position, this.transform.up);
      float a = Vector3.Angle(this.RefVector.forward, vector3_1);
      Vector3 vector3_2 = Vector3.RotateTowards(this.RefVector.forward, vector3_1, Mathf.Min(a, this.MaxRot) * 0.0174533f, 0.0f);
      this.transform.rotation = Quaternion.LookRotation(vector3_2, this.RefVector.up);
      this.ValvePos = (float) (((double) Vector3.Angle(this.RefVector.right, vector3_2) - 90.0) / ((double) this.MaxRot * 2.0) + 0.5);
    }
  }
}
