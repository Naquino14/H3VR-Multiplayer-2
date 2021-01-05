// Decompiled with JetBrains decompiler
// Type: FistVR.MG_Cabinet
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class MG_Cabinet : MonoBehaviour, IMGSpawnIntoAble
  {
    public List<MG_CabinetDrawer> Drawers;
    private List<MG_CabinetDrawer> SpawnedDrawers;
    private bool m_canBeenSpawnedInto = true;

    public bool CanBeSpawnedInto() => this.m_canBeenSpawnedInto;

    public void PlaceObjectInto(GameObject obj)
    {
      int index = Random.Range(0, this.Drawers.Count);
      this.Drawers[index].SpawnIntoCabinet(obj);
      this.Drawers.RemoveAt(index);
      if (this.Drawers.Count > 0)
        return;
      this.m_canBeenSpawnedInto = false;
    }

    public void PlaceObjectInto(GameObject[] objs)
    {
      int index = Random.Range(0, this.Drawers.Count);
      this.Drawers[index].SpawnIntoCabinet(objs);
      this.Drawers.RemoveAt(index);
      if (this.Drawers.Count > 0)
        return;
      this.m_canBeenSpawnedInto = false;
    }

    public void Init()
    {
      for (int index = 0; index < this.Drawers.Count; ++index)
        this.Drawers[index].Init();
    }

    public Transform GetRandomSpawnTransform() => this.Drawers[Random.Range(0, this.Drawers.Count)].ItemPoint;
  }
}
