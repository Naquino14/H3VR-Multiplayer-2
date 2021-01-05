// Decompiled with JetBrains decompiler
// Type: FistVR.AIWeinerBotBodyController
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AIWeinerBotBodyController : AIBodyPiece
  {
    public Transform AimingTransform;
    public AIMeleeWeapon[] Weapons;
    public AISensorSystem SensorSystem;

    public void SetFireAtWill(bool b)
    {
      for (int index = 0; index < this.Weapons.Length; ++index)
      {
        if ((Object) this.Weapons[index] != (Object) null)
          this.Weapons[index].SetFireAtWill(b);
      }
    }

    public void SetTargetPoint(Vector3 v)
    {
    }
  }
}
