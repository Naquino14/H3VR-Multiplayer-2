// Decompiled with JetBrains decompiler
// Type: FistVR.TAH_MobSpawnGroup
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class TAH_MobSpawnGroup : MonoBehaviour
  {
    public AnimationCurve SpawnChanceByDifficulty;
    public int BaseWhenSpawning;
    public float DifficultyMultBonus;
    public int MinWhenSpawning;
    public int MaxWhenSpawning;
    public List<Transform> SpawnPoints;
    public List<GameObject> EnemyPrefab;
    public float MinDifficulty;
    public float MinDistance = 20f;

    public bool GetShouldSpawn(float difficulty, Vector3 playerPos)
    {
      if ((double) this.MinDifficulty > (double) difficulty)
        return false;
      float num = this.SpawnChanceByDifficulty.Evaluate(difficulty);
      return (double) Random.Range(0.0f, 1f) <= (double) num && (double) Vector3.Distance(this.transform.position, playerPos) > (double) this.MinDistance;
    }

    public List<GameObject> SpawnMobs(float difficulty)
    {
      List<GameObject> gameObjectList = new List<GameObject>();
      int num = this.MinWhenSpawning;
      if ((double) this.DifficultyMultBonus > 0.0)
        num += Mathf.RoundToInt(this.DifficultyMultBonus * difficulty);
      if (num > this.MaxWhenSpawning)
        num = this.MaxWhenSpawning;
      List<Transform> transformList = new List<Transform>();
      for (int index = 0; index < this.SpawnPoints.Count; ++index)
        transformList.Add(this.SpawnPoints[index]);
      for (int index1 = 0; index1 < num; ++index1)
      {
        int index2 = Random.Range(0, transformList.Count);
        GameObject gameObject = Object.Instantiate<GameObject>(this.EnemyPrefab[Random.Range(0, this.EnemyPrefab.Count)], transformList[index2].position, transformList[index2].rotation);
        gameObjectList.Add(gameObject);
        transformList.RemoveAt(index2);
      }
      transformList.Clear();
      return gameObjectList;
    }
  }
}
