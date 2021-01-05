// Decompiled with JetBrains decompiler
// Type: FistVR.MR_DamageOnEnter
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MR_DamageOnEnter : MonoBehaviour
  {
    public int DamageP = 2;
    public bool m_isCheaterDamage;

    private void OnTriggerEnter(Collider col)
    {
      FVRPlayerHitbox component = col.GetComponent<FVRPlayerHitbox>();
      if (!((Object) component != (Object) null))
        return;
      component.Damage(new Damage()
      {
        strikeDir = -Vector3.up,
        hitNormal = Vector3.up,
        point = this.transform.position,
        Dam_Piercing = (float) this.DamageP,
        Dam_TotalKinetic = (float) this.DamageP,
        Class = Damage.DamageClass.Abstract
      });
      if (!this.m_isCheaterDamage)
        return;
      GM.MGMaster.Narrator.PlayDiedCheating();
    }
  }
}
