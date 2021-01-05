// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigJammerBox
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class ZosigJammerBox : MonoBehaviour, IFVRDamageable
  {
    public List<GameObject> Prefab_OnDestroy;
    public Transform DestroySpawnPoint;
    public GameObject Obj_Undestroyed;
    public GameObject Obj_Destroyed;
    public ZosigJammerBox.JammerBoxState BState;
    private float m_lifeLeft = 3000f;

    private void Start()
    {
    }

    private void Update()
    {
    }

    public void Damage(FistVR.Damage d)
    {
      if (this.BState == ZosigJammerBox.JammerBoxState.Destroyed)
        return;
      this.m_lifeLeft -= d.Dam_TotalEnergetic + d.Dam_TotalKinetic;
      if ((double) this.m_lifeLeft > 0.0)
        return;
      this.Splode();
    }

    private void Splode()
    {
      this.SetDestroyed();
      for (int index = 0; index < this.Prefab_OnDestroy.Count; ++index)
        Object.Instantiate<GameObject>(this.Prefab_OnDestroy[index], this.DestroySpawnPoint.position, this.DestroySpawnPoint.rotation);
    }

    public void SetDestroyed()
    {
      this.BState = ZosigJammerBox.JammerBoxState.Destroyed;
      this.Obj_Destroyed.SetActive(true);
      this.Obj_Undestroyed.SetActive(false);
    }

    public enum JammerBoxState
    {
      Functioning,
      Destroyed,
    }
  }
}
