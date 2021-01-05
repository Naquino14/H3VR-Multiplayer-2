// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Sample.trackObj
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
  public class trackObj : MonoBehaviour
  {
    public Transform target;
    public float speed;
    public bool negative;

    private void Update()
    {
      Vector3 forward = this.target.position - this.transform.position;
      if (this.negative)
        forward = -forward;
      if ((double) this.speed == 0.0)
        this.transform.rotation = Quaternion.LookRotation(forward);
      else
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(forward), this.speed * Time.deltaTime);
    }
  }
}
