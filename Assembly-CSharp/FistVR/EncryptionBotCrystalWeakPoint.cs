// Decompiled with JetBrains decompiler
// Type: FistVR.EncryptionBotCrystalWeakPoint
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class EncryptionBotCrystalWeakPoint : MonoBehaviour, IFVRDamageable
  {
    public EncryptionBotCrystal Crystal;
    private EncryptionBotCrystal.Crystal m_c;
    public int WhichCrystal;

    public void SetMC(EncryptionBotCrystal.Crystal c) => this.m_c = c;

    public void Damage(FistVR.Damage d)
    {
      float damTotalKinetic = d.Dam_TotalKinetic;
      if (d.Class == FistVR.Damage.DamageClass.Explosive)
        damTotalKinetic *= 0.4f;
      if (d.Class == FistVR.Damage.DamageClass.Projectile)
        damTotalKinetic *= 1.5f;
      this.Crystal.CrystalHit(this.m_c, damTotalKinetic);
    }
  }
}
