// Decompiled with JetBrains decompiler
// Type: FistVR.WarehouseRangeMainPanel
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class WarehouseRangeMainPanel : MonoBehaviour
  {
    public GameObject Targets;
    private GameObject m_curTargets;
    public Transform[] SlicerSpawns;
    public GameObject SlicerPrefab;
    public List<GameObject> CurSlicers = new List<GameObject>();

    private void Awake()
    {
      this.Targets.SetActive(false);
      this.m_curTargets = Object.Instantiate<GameObject>(this.Targets, this.Targets.transform.position, this.Targets.transform.rotation);
      this.m_curTargets.SetActive(true);
    }

    public void ResetTargets()
    {
      Object.Destroy((Object) this.m_curTargets);
      this.m_curTargets = Object.Instantiate<GameObject>(this.Targets, this.Targets.transform.position, this.Targets.transform.rotation);
      this.m_curTargets.SetActive(true);
    }

    public void SpawnSlicerAttack()
    {
      if (this.CurSlicers.Count > 0)
      {
        for (int index = this.CurSlicers.Count - 1; index >= 0; --index)
        {
          if ((Object) this.CurSlicers[index] == (Object) null)
            this.CurSlicers.RemoveAt(index);
        }
      }
      if (this.CurSlicers.Count >= 4)
        return;
      for (int index = 0; index < 3; ++index)
        this.CurSlicers.Add(Object.Instantiate<GameObject>(this.SlicerPrefab, this.SlicerSpawns[Random.Range(0, this.SlicerSpawns.Length)].position + Random.onUnitSphere * 2f, Random.rotation));
    }

    public void SpawnSingleSlicer()
    {
      if (this.CurSlicers.Count >= 4)
        return;
      this.CurSlicers.Add(Object.Instantiate<GameObject>(this.SlicerPrefab, this.SlicerSpawns[Random.Range(0, this.SlicerSpawns.Length)].position + Random.onUnitSphere * 2f, Random.rotation));
    }

    public void KillAllSlicers()
    {
      if (this.CurSlicers.Count > 0)
      {
        for (int index = this.CurSlicers.Count - 1; index >= 0; --index)
        {
          if ((Object) this.CurSlicers[index] != (Object) null)
          {
            Object.Destroy((Object) this.CurSlicers[index]);
            this.CurSlicers.RemoveAt(index);
          }
        }
      }
      this.CurSlicers.Clear();
    }
  }
}
