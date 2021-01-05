// Decompiled with JetBrains decompiler
// Type: FistVR.F18Manager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class F18Manager : MonoBehaviour
  {
    public List<Transform> SpawnPoints;
    public GameObject PlanePrefab;
    public int MaxPlanes = 4;
    private float m_tickTilSpawn = 30f;
    private List<GameObject> Planes = new List<GameObject>();

    private void Start()
    {
      this.SpawnAttempt();
      this.SpawnAttempt();
      this.SpawnAttempt();
      this.SpawnAttempt();
    }

    private void Update()
    {
      this.m_tickTilSpawn -= Time.deltaTime;
      if ((double) this.m_tickTilSpawn < 0.0)
      {
        this.m_tickTilSpawn = Random.Range(15f, 30f);
        this.SpawnAttempt();
      }
      if (this.Planes.Count <= 0)
        return;
      for (int index = this.Planes.Count - 1; index >= 0; --index)
      {
        if ((Object) this.Planes[index] == (Object) null)
          this.Planes.RemoveAt(index);
      }
    }

    private void SpawnAttempt()
    {
      if (this.Planes.Count >= this.MaxPlanes)
        return;
      Transform spawnPoint = this.SpawnPoints[Random.Range(0, this.SpawnPoints.Count)];
      this.Planes.Add(Object.Instantiate<GameObject>(this.PlanePrefab, spawnPoint.position, spawnPoint.rotation));
    }
  }
}
