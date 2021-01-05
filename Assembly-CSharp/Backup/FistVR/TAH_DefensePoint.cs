// Decompiled with JetBrains decompiler
// Type: FistVR.TAH_DefensePoint
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class TAH_DefensePoint : MonoBehaviour
  {
    public TAH_DefensePoint.TAH_BotSpawner Spawner;
    public GameObject NavBlock;
    public GameObject PointCircle;
    public Transform[] StaticBotSpawnPoints;
    public wwBotWurstNavPointGroup NavGroup;
    public GameObject BeginTouch;

    public void Awake()
    {
      this.PointCircle.SetActive(false);
      this.BeginTouch.SetActive(false);
    }

    public void InitiateDefense()
    {
      this.NavBlock.SetActive(true);
      this.BeginTouch.SetActive(false);
    }

    public void EndDefense()
    {
      this.NavBlock.SetActive(false);
      this.PointCircle.SetActive(false);
    }

    public void BeginWave(TAH_WaveDefinition waveDef) => this.Spawner.Init(waveDef);

    public void EndWave() => this.Spawner.End();

    [Serializable]
    public class TAH_BotSpawner
    {
      public List<Transform> SpawnPoints = new List<Transform>();
      public TAH_BotSpawnProfile SpawnProfile;
      [HideInInspector]
      public int NumLeftToSpawn;
      [HideInInspector]
      public float TimeTilSpawnBot;
      [HideInInspector]
      public int NumSpawnPointsToUse = 1;
      [HideInInspector]
      public float SpawnCooldownTime = 1f;

      public void Init(TAH_WaveDefinition waveDef)
      {
        this.NumLeftToSpawn = waveDef.NumBots;
        this.TimeTilSpawnBot = waveDef.WarmUpToSpawnTime;
        this.NumSpawnPointsToUse = waveDef.NumSpawnPointsToUse;
        this.SpawnProfile = waveDef.BotSpawnProfiles[UnityEngine.Random.Range(0, waveDef.BotSpawnProfiles.Count)];
        this.SpawnCooldownTime = waveDef.SpawnCooldownTime;
      }

      public void End()
      {
        this.NumLeftToSpawn = 0;
        this.TimeTilSpawnBot = 0.0f;
        this.NumSpawnPointsToUse = 1;
      }

      public int GetSpawnPointIndex() => UnityEngine.Random.Range(0, Mathf.Min(this.SpawnPoints.Count, this.NumSpawnPointsToUse));
    }
  }
}
