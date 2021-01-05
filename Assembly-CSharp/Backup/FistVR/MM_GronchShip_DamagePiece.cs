// Decompiled with JetBrains decompiler
// Type: FistVR.MM_GronchShip_DamagePiece
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MM_GronchShip_DamagePiece : MonoBehaviour, IFVRDamageable
  {
    private bool m_isDestroyed;
    public float Life = 1000f;
    private Renderer m_rend;
    private Collider m_col;
    public GameObject VFXPrefab;

    public bool IsDestroyed() => this.m_isDestroyed;

    public void Damage(FistVR.Damage d)
    {
      if (this.m_isDestroyed || (double) d.Dam_TotalKinetic <= 1.0)
        return;
      this.Life -= d.Dam_TotalKinetic;
      if ((double) this.Life >= 0.0)
        return;
      this.Explode();
    }

    private void Start()
    {
      this.m_rend = this.GetComponent<Renderer>();
      this.m_col = this.GetComponent<Collider>();
    }

    private void Explode()
    {
      this.m_isDestroyed = true;
      Object.Instantiate<GameObject>(this.VFXPrefab, this.transform.position, this.transform.rotation);
      this.m_rend.enabled = false;
      this.m_col.enabled = false;
    }
  }
}
