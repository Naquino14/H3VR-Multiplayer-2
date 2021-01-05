// Decompiled with JetBrains decompiler
// Type: RotateOverTime
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class RotateOverTime : MonoBehaviour
{
  public Vector3 angularVelocity = Vector3.forward * 45f;

  private void Update()
  {
    Vector3 vector3 = this.transform.localEulerAngles + this.angularVelocity * Time.deltaTime;
    vector3.x = Mathf.Repeat(vector3.x, 360f);
    vector3.y = Mathf.Repeat(vector3.y, 360f);
    vector3.z = Mathf.Repeat(vector3.z, 360f);
    this.transform.localEulerAngles = vector3;
  }
}
