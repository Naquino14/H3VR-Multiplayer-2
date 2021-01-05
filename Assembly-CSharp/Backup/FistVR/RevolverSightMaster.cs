// Decompiled with JetBrains decompiler
// Type: FistVR.RevolverSightMaster
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class RevolverSightMaster : MonoBehaviour
  {
    public FVRFireArm FA;
    public Transform Sight;

    [ContextMenu("Zero")]
    public void Zero()
    {
      Vector3 vector3 = this.Sight.position - this.transform.position;
      vector3.Normalize();
      this.FA.MuzzlePos.transform.LookAt(this.transform.position + vector3 * 25f, Vector3.up);
    }
  }
}
