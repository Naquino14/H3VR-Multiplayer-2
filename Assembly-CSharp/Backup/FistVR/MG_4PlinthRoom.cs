// Decompiled with JetBrains decompiler
// Type: FistVR.MG_4PlinthRoom
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class MG_4PlinthRoom : MonoBehaviour, IRoomTriggerable
  {
    public RedRoom m_room;
    private bool m_isTriggered;
    private bool m_hasFinished;
    private bool m_hasPickupAGun;
    private bool m_isGreedy;
    private float greedyTick = 60f;
    private int m_numTaken;
    public GameObject[] Plinths;
    public Transform[] SpawnPoints;
    public GameObject Warning1;
    public GameObject Warning2;
    private List<FVRPhysicalObject> SpawnedPOs = new List<FVRPhysicalObject>();
    public Transform[] EnemySpawnPoints;
    private bool m_hasSpawnedBaddies;

    public void Init(int size, RedRoom room)
    {
      this.m_room = room;
      this.m_room.CloseDoors(true);
      this.SpawnItemsOntoPlinths();
      if (this.m_isTriggered)
        return;
      this.m_isTriggered = true;
    }

    public void SetRoom(RedRoom room) => this.m_room = room;

    private void Update() => this.WeaponCheck();

    private void WeaponCheck()
    {
      if (!this.m_isTriggered || this.m_hasFinished)
        return;
      int num = 0;
      for (int index = 0; index < this.SpawnedPOs.Count; ++index)
      {
        if ((Object) this.SpawnedPOs[index] != (Object) null && !this.SpawnedPOs[index].RootRigidbody.isKinematic)
          ++num;
      }
      if (num > 0 && !this.m_hasPickupAGun)
        this.m_hasPickupAGun = true;
      if (this.m_hasPickupAGun && this.Warning1.activeSelf)
      {
        this.Warning1.SetActive(false);
        this.m_room.OpenDoors(true);
        this.greedyTick = 10f;
      }
      if (num > 1 && !this.m_hasSpawnedBaddies)
      {
        this.m_hasSpawnedBaddies = true;
        this.m_room.CloseDoors(true);
        this.Warning2.SetActive(true);
        this.m_isGreedy = true;
        this.greedyTick = 30f;
        this.SpawnSlicer();
        this.ClearOtherGuns();
      }
      if (!this.m_hasPickupAGun)
        return;
      this.greedyTick -= Time.deltaTime;
      if ((double) this.greedyTick > 0.0)
        return;
      this.m_room.OpenDoors(true);
      this.ClearOtherGuns();
      this.m_hasFinished = true;
    }

    private void SpawnSlicer()
    {
      Object.Instantiate<GameObject>(GM.MGMaster.SlicerPrefab, this.SpawnPoints[Random.Range(0, this.SpawnPoints.Length - 1)].position, Random.rotation);
      GM.MGMaster.Narrator.PlayMonsterRebuilt();
    }

    private void ClearOtherGuns()
    {
      for (int index = this.SpawnedPOs.Count - 1; index >= 0; --index)
      {
        if ((Object) this.SpawnedPOs[index] != (Object) null && this.SpawnedPOs[index].RootRigidbody.isKinematic)
          Object.Destroy((Object) this.SpawnedPOs[index].gameObject);
      }
      this.SpawnedPOs.Clear();
    }

    private void SpawnEnemies()
    {
    }

    private void SpawnItemsOntoPlinths()
    {
      this.Warning1.SetActive(true);
      for (int index = 0; index < this.Plinths.Length; ++index)
        this.Plinths[index].SetActive(true);
      this.SpawnedPOs.Add(Object.Instantiate<GameObject>(GM.MGMaster.LTEntry_Shotgun2.GetGameObject(), this.SpawnPoints[0].position, this.SpawnPoints[0].rotation).GetComponent<FVRPhysicalObject>());
      this.SpawnedPOs[0].RootRigidbody.isKinematic = true;
      this.SpawnedPOs.Add(Object.Instantiate<GameObject>(GM.MGMaster.LTEntry_RareGun1.GetGameObject(), this.SpawnPoints[1].position, this.SpawnPoints[1].rotation).GetComponent<FVRPhysicalObject>());
      this.SpawnedPOs[1].RootRigidbody.isKinematic = true;
      this.SpawnedPOs.Add(Object.Instantiate<GameObject>(GM.MGMaster.LTEntry_Handgun2.GetGameObject(), this.SpawnPoints[2].position, this.SpawnPoints[2].rotation).GetComponent<FVRPhysicalObject>());
      this.SpawnedPOs[2].RootRigidbody.isKinematic = true;
      this.SpawnedPOs.Add(Object.Instantiate<GameObject>(GM.MGMaster.LTEntry_RareGun2.GetGameObject(), this.SpawnPoints[3].position, this.SpawnPoints[3].rotation).GetComponent<FVRPhysicalObject>());
      this.SpawnedPOs[3].RootRigidbody.isKinematic = true;
    }
  }
}
