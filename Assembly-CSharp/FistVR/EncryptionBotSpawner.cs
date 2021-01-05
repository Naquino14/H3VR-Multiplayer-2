// Decompiled with JetBrains decompiler
// Type: FistVR.EncryptionBotSpawner
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class EncryptionBotSpawner : MonoBehaviour
  {
    public GameObject BotToSpawn;
    public List<Transform> PossibleSpawns;
    public int NumToSpawn;
    public bool Respawns;
    public Vector2 RespawnTimes = new Vector2(180f, 360f);
    private List<GameObject> m_spawns = new List<GameObject>();
    private List<float> m_reSpawnTimes = new List<float>();

    private void Start()
    {
      this.PossibleSpawns.Shuffle<Transform>();
      this.PossibleSpawns.Shuffle<Transform>();
      for (int index = 0; index < this.NumToSpawn; ++index)
      {
        Vector3 onUnitSphere = Random.onUnitSphere;
        onUnitSphere.y = 0.0f;
        this.m_spawns.Add(Object.Instantiate<GameObject>(this.BotToSpawn, this.PossibleSpawns[index].position, Quaternion.LookRotation(onUnitSphere, Vector3.up)));
        if (this.Respawns)
          this.m_reSpawnTimes.Add(Random.Range(this.RespawnTimes.x, this.RespawnTimes.y));
      }
    }

    public void ClearAll()
    {
      if (this.m_spawns.Count <= 0)
        return;
      for (int index = this.m_spawns.Count - 1; index >= 0; --index)
        Object.Destroy((Object) this.m_spawns[index]);
    }

    private void Update()
    {
      if (!this.Respawns)
        return;
      for (int index1 = 0; index1 < this.m_spawns.Count; ++index1)
      {
        if ((Object) this.m_spawns[index1] == (Object) null)
        {
          List<float> reSpawnTimes;
          int index2;
          (reSpawnTimes = this.m_reSpawnTimes)[index2 = index1] = reSpawnTimes[index2] - Time.deltaTime;
          if ((double) this.m_reSpawnTimes[index1] <= 0.0)
          {
            this.PossibleSpawns.Shuffle<Transform>();
            Vector3 onUnitSphere = Random.onUnitSphere;
            onUnitSphere.y = 0.0f;
            GameObject gameObject = Object.Instantiate<GameObject>(this.BotToSpawn, this.PossibleSpawns[index1].position, Quaternion.LookRotation(onUnitSphere, Vector3.up));
            this.m_spawns[index1] = gameObject;
            this.m_reSpawnTimes[index1] = Random.Range(this.RespawnTimes.x, this.RespawnTimes.y);
          }
        }
      }
    }
  }
}
