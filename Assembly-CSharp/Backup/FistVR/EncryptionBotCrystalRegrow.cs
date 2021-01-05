// Decompiled with JetBrains decompiler
// Type: FistVR.EncryptionBotCrystalRegrow
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class EncryptionBotCrystalRegrow : MonoBehaviour, IFVRDamageable
  {
    public EncryptionBotCrystal Crystal;

    public void Damage(FistVR.Damage d)
    {
      if (d.Class == FistVR.Damage.DamageClass.Explosive)
        return;
      this.Crystal.Regrow();
    }
  }
}
