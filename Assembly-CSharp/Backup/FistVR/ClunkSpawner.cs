// Decompiled with JetBrains decompiler
// Type: FistVR.ClunkSpawner
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class ClunkSpawner : MonoBehaviour
  {
    public GameObject ClunkPrefabEasy;
    public GameObject ClunkMk3Prefab;
    public Transform[] ClunkSpawnPositions;
    private List<GameObject> Clunks = new List<GameObject>();
    private List<GameObject> Slicers = new List<GameObject>();
    public GameObject Slicer;
    public Transform[] SlicerSpawnPositions;
    private List<GameObject> SoldierBots = new List<GameObject>();
    public wwBotWurstNavPointGroup NavGroup;
    public GameObject[] SoldierBots_Slowbullets;
    public GameObject[] SoldierBots_Fastbullets;

    private int[] Choose3RandomIndicies(int length)
    {
      int[] numArray = new int[length];
      for (int index = 0; index < numArray.Length; ++index)
        numArray[index] = index;
      for (int min = 0; min < numArray.Length; ++min)
      {
        int num = numArray[min];
        int index = Random.Range(min, length);
        numArray[min] = numArray[index];
        numArray[index] = num;
      }
      return numArray;
    }

    public void SpawnSlicer()
    {
      int[] numArray = this.Choose3RandomIndicies(3);
      for (int index = 0; index < numArray.Length; ++index)
        this.Clunks.Add(Object.Instantiate<GameObject>(this.Slicer, this.SlicerSpawnPositions[numArray[index]].position, this.SlicerSpawnPositions[numArray[index]].rotation));
    }

    public void SpawnClunksEasy()
    {
      int[] numArray = this.Choose3RandomIndicies(3);
      for (int index = 0; index < numArray.Length; ++index)
        this.Clunks.Add(Object.Instantiate<GameObject>(this.ClunkPrefabEasy, this.ClunkSpawnPositions[numArray[index]].position, this.ClunkSpawnPositions[numArray[index]].rotation));
    }

    public void SpawnClunkMk3s()
    {
      int[] numArray = this.Choose3RandomIndicies(3);
      for (int index = 0; index < numArray.Length; ++index)
        this.Clunks.Add(Object.Instantiate<GameObject>(this.ClunkMk3Prefab, this.ClunkSpawnPositions[numArray[index]].position, this.ClunkSpawnPositions[numArray[index]].rotation));
    }

    public void SpawnSoldierBots_SlowBullets()
    {
      int[] numArray = this.Choose3RandomIndicies(Random.Range(3, 7));
      for (int index = 0; index < numArray.Length; ++index)
      {
        GameObject gameObject = Object.Instantiate<GameObject>(this.SoldierBots_Slowbullets[Random.Range(0, this.SoldierBots_Slowbullets.Length)], this.ClunkSpawnPositions[numArray[index]].position, this.ClunkSpawnPositions[numArray[index]].rotation);
        gameObject.GetComponent<wwBotWurst>().NavPointGroup = this.NavGroup;
        GM.CurrentSceneSettings.ShotEventReceivers.Add(gameObject);
        this.SoldierBots.Add(gameObject);
      }
    }

    public void SpawnSoldierBots_FastBullets()
    {
      int[] numArray = this.Choose3RandomIndicies(Random.Range(3, 7));
      for (int index = 0; index < numArray.Length; ++index)
      {
        GameObject gameObject = Object.Instantiate<GameObject>(this.SoldierBots_Fastbullets[Random.Range(0, this.SoldierBots_Fastbullets.Length)], this.ClunkSpawnPositions[numArray[index]].position, this.ClunkSpawnPositions[numArray[index]].rotation);
        gameObject.GetComponent<wwBotWurst>().NavPointGroup = this.NavGroup;
        GM.CurrentSceneSettings.ShotEventReceivers.Add(gameObject);
        this.SoldierBots.Add(gameObject);
      }
    }

    public void DeleteBots()
    {
      if (this.Clunks.Count > 0)
      {
        for (int index = this.Clunks.Count - 1; index >= 0; --index)
          Object.Destroy((Object) this.Clunks[index]);
      }
      this.Clunks.Clear();
      if (this.Slicers.Count > 0)
      {
        for (int index = this.Slicers.Count - 1; index >= 0; --index)
          Object.Destroy((Object) this.Slicers[index]);
      }
      this.Slicers.Clear();
      if (this.SoldierBots.Count > 0)
      {
        for (int index = this.SoldierBots.Count - 1; index >= 0; --index)
          Object.Destroy((Object) this.SoldierBots[index]);
      }
      this.SoldierBots.Clear();
    }

    public void DeleteClunks()
    {
      if (this.Clunks.Count > 0)
      {
        for (int index = this.Clunks.Count - 1; index >= 0; --index)
          Object.Destroy((Object) this.Clunks[index]);
      }
      this.Clunks.Clear();
    }

    public void ResetSlicers()
    {
      this.DeleteClunks();
      this.SpawnSlicer();
    }

    public void ResetClunksEasy()
    {
      this.DeleteClunks();
      this.SpawnClunksEasy();
    }

    public void ResetClunkMk3s()
    {
      this.DeleteClunks();
      this.SpawnClunkMk3s();
    }

    public void DeleteMagazines()
    {
      FVRFireArmMagazine[] objectsOfType1 = Object.FindObjectsOfType<FVRFireArmMagazine>();
      for (int index = objectsOfType1.Length - 1; index >= 0; --index)
      {
        if (!objectsOfType1[index].IsHeld && (Object) objectsOfType1[index].QuickbeltSlot == (Object) null && (Object) objectsOfType1[index].FireArm == (Object) null)
          Object.Destroy((Object) objectsOfType1[index].gameObject);
      }
      FVRFireArmRound[] objectsOfType2 = Object.FindObjectsOfType<FVRFireArmRound>();
      for (int index = objectsOfType2.Length - 1; index >= 0; --index)
      {
        if (!objectsOfType2[index].IsHeld && (Object) objectsOfType2[index].QuickbeltSlot == (Object) null)
          Object.Destroy((Object) objectsOfType2[index].gameObject);
      }
    }

    public void PlayerDied() => this.DeleteBots();
  }
}
