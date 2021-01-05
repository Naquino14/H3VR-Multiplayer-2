// Decompiled with JetBrains decompiler
// Type: FistVR.EncryptionBotWeakpoint
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class EncryptionBotWeakpoint : MonoBehaviour, IFVRDamageable
  {
    public EncryptionBotMine Mine;
    public EncryptionBotHardened Hard;
    public EncryptionBotMissileBoat Boat;
    public EncryptionBotAgile Agile;
    public bool PinPrick = true;
    public bool DodgePoint;

    public void Damage(FistVR.Damage d)
    {
      if (d.Class == FistVR.Damage.DamageClass.Explosive && (double) d.Dam_TotalKinetic < 2000.0)
        return;
      if ((Object) this.Mine != (Object) null)
        this.Mine.Explode();
      if ((Object) this.Hard != (Object) null)
        this.Hard.Explode();
      if ((Object) this.Boat != (Object) null)
        this.Boat.Explode();
      if (!((Object) this.Agile != (Object) null))
        return;
      if (this.DodgePoint && (double) d.Dam_TotalKinetic < 10000.0)
        this.Agile.Evade(d.strikeDir);
      else
        this.Agile.Explode();
    }
  }
}
