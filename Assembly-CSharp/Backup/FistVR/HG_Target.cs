// Decompiled with JetBrains decompiler
// Type: FistVR.HG_Target
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class HG_Target : MonoBehaviour, IFVRDamageable
  {
    private bool m_isDestroyed;
    private HG_ModeManager m_manager;
    private HG_Zone m_zone;
    public List<GameObject> SpawnOnDestruction;
    public List<Rigidbody> ActivateOnDestruction;
    public float Life = 1f;
    public bool RequiresMeleeDamage;
    private FistVR.Damage.DamageClass m_destroyedDamClass = FistVR.Damage.DamageClass.Projectile;

    public HG_Zone GetZone() => this.m_zone;

    public void Init(HG_ModeManager m, HG_Zone z)
    {
      this.m_manager = m;
      this.m_zone = z;
    }

    public FistVR.Damage.DamageClass GetClassThatKilledMe() => this.m_destroyedDamClass;

    public void Damage(FistVR.Damage dam)
    {
      if (this.m_isDestroyed || this.RequiresMeleeDamage && dam.Class != FistVR.Damage.DamageClass.Melee)
        return;
      this.Life -= dam.Dam_TotalKinetic;
      this.Life -= dam.Dam_TotalEnergetic;
      if ((double) this.Life > 0.0)
        return;
      this.m_destroyedDamClass = dam.Class;
      for (int index = 0; index < this.SpawnOnDestruction.Count; ++index)
        Object.Instantiate<GameObject>(this.SpawnOnDestruction[index], this.transform.position, this.transform.rotation);
      for (int index = 0; index < this.ActivateOnDestruction.Count; ++index)
      {
        this.ActivateOnDestruction[index].transform.SetParent((Transform) null);
        this.ActivateOnDestruction[index].gameObject.SetActive(true);
      }
      this.m_isDestroyed = true;
      this.m_manager.TargetDestroyed(this);
      Object.Destroy((Object) this.gameObject);
    }
  }
}
