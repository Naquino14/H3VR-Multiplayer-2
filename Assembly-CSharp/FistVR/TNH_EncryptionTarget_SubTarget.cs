// Decompiled with JetBrains decompiler
// Type: FistVR.TNH_EncryptionTarget_SubTarget
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class TNH_EncryptionTarget_SubTarget : MonoBehaviour, IFVRDamageable
  {
    public TNH_EncryptionTarget Target;
    public int Index;

    public void Damage(FistVR.Damage d)
    {
      if (d.Class == FistVR.Damage.DamageClass.Explosive)
        return;
      this.Target.DisableSubtarg(this.Index);
    }
  }
}
