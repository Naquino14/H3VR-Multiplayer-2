// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigDestroyable
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class ZosigDestroyable : MonoBehaviour, IFVRDamageable
  {
    public string AssociatedFlag;
    public int AssociatedFlagValue = 1;
    public GameObject Visuals_UnDestroyed;
    public GameObject Visuals_Destroyed;
    [Header("Damage Stuff")]
    public float Integrity = 30000f;
    private bool m_isDestroyed;
    private ZosigGameManager M;
    public GameObject OnDestroyPrefab;
    public Transform OnDestroyPoint;

    public void Init(ZosigGameManager m)
    {
      this.M = m;
      this.InitializeFromFlagM();
    }

    private void InitializeFromFlagM()
    {
      if (this.M.FlagM.GetFlagValue(this.AssociatedFlag) < this.AssociatedFlagValue)
        return;
      this.m_isDestroyed = true;
      this.SetDestroyedVisual(true);
      if (!GM.ZMaster.IsVerboseDebug)
        return;
      Debug.Log((object) (this.gameObject.name + " set to destroyed by flag:" + this.AssociatedFlag));
    }

    private void SetDestroyedVisual(bool b)
    {
      if (b)
      {
        if ((Object) this.Visuals_Destroyed != (Object) null)
          this.Visuals_Destroyed.SetActive(true);
        if (!((Object) this.Visuals_UnDestroyed != (Object) null))
          return;
        this.Visuals_UnDestroyed.SetActive(false);
      }
      else
      {
        if ((Object) this.Visuals_Destroyed != (Object) null)
          this.Visuals_Destroyed.SetActive(false);
        if (!((Object) this.Visuals_UnDestroyed != (Object) null))
          return;
        this.Visuals_UnDestroyed.SetActive(true);
      }
    }

    public void Damage(FistVR.Damage d)
    {
      if (this.m_isDestroyed)
        return;
      float damTotalKinetic = d.Dam_TotalKinetic;
      if (d.Class == FistVR.Damage.DamageClass.Explosive)
        damTotalKinetic *= 300f;
      if (d.Class == FistVR.Damage.DamageClass.Melee)
        damTotalKinetic *= 0.01f;
      this.Integrity -= damTotalKinetic;
      if ((double) this.Integrity >= 0.0)
        return;
      this.DestroyMe();
    }

    private void DestroyMe()
    {
      if (this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      this.SetDestroyedVisual(true);
      if (this.M.FlagM.GetFlagValue(this.AssociatedFlag) < this.AssociatedFlagValue)
        this.M.FlagM.SetFlag(this.AssociatedFlag, this.AssociatedFlagValue);
      if (!((Object) this.OnDestroyPrefab != (Object) null))
        return;
      Object.Instantiate<GameObject>(this.OnDestroyPrefab, this.OnDestroyPoint.position, this.OnDestroyPoint.rotation);
    }
  }
}
