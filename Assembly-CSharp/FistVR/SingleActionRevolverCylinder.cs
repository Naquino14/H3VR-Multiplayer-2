// Decompiled with JetBrains decompiler
// Type: FistVR.SingleActionRevolverCylinder
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class SingleActionRevolverCylinder : MonoBehaviour
  {
    public int NumChambers = 6;
    public FVRFireArmChamber[] Chambers;

    public int GetClosestChamberIndex() => Mathf.CeilToInt(Mathf.Repeat(-this.transform.localEulerAngles.z + (float) (360.0 / (double) this.NumChambers * 0.5), 360f) / (360f / (float) this.NumChambers)) - 1;

    public Quaternion GetLocalRotationFromCylinder(int cylinder) => Quaternion.Euler(new Vector3(0.0f, 0.0f, Mathf.Repeat((float) ((double) cylinder * (360.0 / (double) this.NumChambers) * -1.0), 360f)));
  }
}
