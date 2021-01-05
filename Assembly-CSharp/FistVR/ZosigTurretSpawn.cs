// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigTurretSpawn
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class ZosigTurretSpawn : MonoBehaviour
  {
    public List<FVRObject> PossibleTurrets;
    public int IFF;
    public float SpawnChance = 1f;
    private bool m_hasSpawned;

    public void SpawnKernel(float t)
    {
      if (this.m_hasSpawned)
        return;
      this.m_hasSpawned = true;
      if ((double) Random.Range(0.0f, 1f) > (double) this.SpawnChance)
        return;
      AutoMeater component = Object.Instantiate<GameObject>(this.PossibleTurrets[Random.Range(0, this.PossibleTurrets.Count)].GetGameObject(), this.transform.position + Vector3.up * 0.2f, this.transform.rotation).GetComponent<AutoMeater>();
      component.SetUseFastProjectile(true);
      component.E.IFFCode = this.IFF;
    }
  }
}
