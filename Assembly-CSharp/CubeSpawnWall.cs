// Decompiled with JetBrains decompiler
// Type: CubeSpawnWall
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubeSpawnWall : MonoBehaviour
{
  public Transform[] Spawns_Center;
  public Transform[] Spawns_Up;
  public Transform[] Spawns_Down;
  public Transform[] Spawns_Boss;
  public Text WaveText;
  public Text TimeText;
  public Text ScoreText;
  public GameObject TargetWarning;

  public List<Transform> GetSpawns(int num, CubeSpawnWall.SpawnWallType type)
  {
    Transform[] transformArray = new Transform[0];
    switch (type)
    {
      case CubeSpawnWall.SpawnWallType.Center:
        transformArray = this.Spawns_Center;
        break;
      case CubeSpawnWall.SpawnWallType.Up:
        transformArray = this.Spawns_Up;
        break;
      case CubeSpawnWall.SpawnWallType.Down:
        transformArray = this.Spawns_Down;
        break;
      case CubeSpawnWall.SpawnWallType.Boss:
        transformArray = this.Spawns_Boss;
        break;
    }
    List<Transform> transformList = new List<Transform>();
    for (int index = 0; index < Mathf.Min(num, transformArray.Length); ++index)
    {
      Transform transform = transformArray[Random.Range(0, transformArray.Length)];
      if (!transformList.Contains(transform))
        transformList.Add(transform);
      else
        --index;
    }
    return transformList;
  }

  public enum SpawnWallType
  {
    Center,
    Up,
    Down,
    Boss,
  }
}
