// Decompiled with JetBrains decompiler
// Type: FistVR.AIThrusterControlBox
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AIThrusterControlBox : FVRDestroyableObject
  {
    public AIThruster[] Thrusters;

    public bool Thrust(Vector3 thrust, ref float magnitude)
    {
      bool flag = false;
      for (int index = 0; index < this.Thrusters.Length; ++index)
      {
        if ((Object) this.Thrusters[index] != (Object) null)
        {
          if ((double) Vector3.Dot(-this.Thrusters[index].transform.forward, thrust.normalized) > 0.5)
          {
            flag = true;
            magnitude += this.Thrusters[index].Thrust();
          }
          else
            this.Thrusters[index].KillThrust();
        }
      }
      return flag;
    }

    public override void DestroyEvent()
    {
      for (int index = 0; index < this.Thrusters.Length; ++index)
      {
        if (!((Object) this.Thrusters[index] != (Object) null))
          ;
      }
      base.DestroyEvent();
    }
  }
}
