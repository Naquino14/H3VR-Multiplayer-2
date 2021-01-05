// Decompiled with JetBrains decompiler
// Type: FistVR.MR_PumpLottoBase
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MR_PumpLottoBase : MonoBehaviour, IMG_HandlePumpable
  {
    public PumpLottoRoom Room;
    public MR_PumpLottoBase.PumpLottoType Type;
    public Transform SlicerSpawn;
    public Transform WeinerBotSpawn;
    public Transform[] LootSpawnPoint;
    private bool m_hasFired;
    public GameObject SlicerPrefab;
    public GameObject WeinerBotPrefab;
    public GameObject WeinerArriveExplosion;
    public GameObject FlamingMeatPrefab;

    public void SetPLType(MR_PumpLottoBase.PumpLottoType t) => this.Type = t;

    public void Pump(float delta)
    {
      switch (this.Type)
      {
        case MR_PumpLottoBase.PumpLottoType.OpenDoor:
          this.Room.m_room.OpenDoors(true);
          break;
        case MR_PumpLottoBase.PumpLottoType.CloseDoor:
          this.Room.m_room.CloseDoors(true);
          break;
        case MR_PumpLottoBase.PumpLottoType.Goof:
          if (!this.m_hasFired)
          {
            this.SpawnGoof();
            break;
          }
          break;
        case MR_PumpLottoBase.PumpLottoType.WeinerBot:
          if (!this.m_hasFired)
          {
            this.SpawnSlicer();
            break;
          }
          break;
        case MR_PumpLottoBase.PumpLottoType.Slicer:
          if (!this.m_hasFired)
          {
            this.SpawnSlicer();
            break;
          }
          break;
        case MR_PumpLottoBase.PumpLottoType.Loot:
          if (!this.m_hasFired)
          {
            this.SpawnLoot();
            break;
          }
          break;
      }
      this.m_hasFired = true;
    }

    private void SpawnWeinerBot()
    {
      Object.Instantiate<GameObject>(this.WeinerBotPrefab, this.WeinerBotSpawn.position, this.WeinerBotSpawn.rotation);
      Object.Instantiate<GameObject>(this.WeinerArriveExplosion, this.WeinerBotSpawn.position, this.WeinerBotSpawn.rotation);
      GM.MGMaster.Narrator.PlayMonsterCloset();
    }

    private void SpawnSlicer()
    {
      Object.Instantiate<GameObject>(this.SlicerPrefab, this.SlicerSpawn.position, this.SlicerSpawn.rotation);
      GM.MGMaster.Narrator.PlayMonsterRebuilt();
    }

    private void SpawnGoof()
    {
      Object.Instantiate<GameObject>(this.FlamingMeatPrefab, this.LootSpawnPoint[0].position, Random.rotation);
      Object.Instantiate<GameObject>(this.FlamingMeatPrefab, this.LootSpawnPoint[2].position, Random.rotation);
      Object.Instantiate<GameObject>(this.FlamingMeatPrefab, this.LootSpawnPoint[4].position, Random.rotation);
    }

    private void SpawnLoot()
    {
      float num = Random.Range(0.0f, 1f);
      if ((double) num > 0.949999988079071)
        GM.MGMaster.SpawnGunAmmoPairToTransformList(GM.MGMaster.LTEntry_RareGun2, this.LootSpawnPoint);
      else if ((double) num > 0.899999976158142)
        GM.MGMaster.SpawnGunAmmoPairToTransformList(GM.MGMaster.LTEntry_RareGun3, this.LootSpawnPoint);
      else if ((double) num > 0.800000011920929)
        GM.MGMaster.SpawnGunAmmoPairToTransformList(GM.MGMaster.LTEntry_Shotgun3, this.LootSpawnPoint);
      else if ((double) num > 0.600000023841858)
        GM.MGMaster.SpawnGunAmmoPairToTransformList(GM.MGMaster.LTEntry_Shotgun2, this.LootSpawnPoint);
      else if ((double) num > 0.400000005960464)
        GM.MGMaster.SpawnGunAmmoPairToTransformList(GM.MGMaster.LTEntry_Handgun3, this.LootSpawnPoint);
      else if ((double) num > 0.200000002980232)
      {
        GM.MGMaster.SpawnGunAmmoPairToTransformList(GM.MGMaster.LTEntry_Handgun2, this.LootSpawnPoint);
      }
      else
      {
        if (!GM.MGMaster.m_hasSpawnedSeconaryLight)
          GM.MGMaster.SpawnLight(this.LootSpawnPoint[1].position, Random.rotation, true, GM.Options.MeatGrinderFlags.SecondaryLight);
        GM.MGMaster.SpawnObjectAtPlace(GM.MGMaster.LT_Melee.GetRandomObject(), this.LootSpawnPoint[0]);
      }
    }

    public enum PumpLottoType
    {
      OpenDoor,
      CloseDoor,
      Goof,
      WeinerBot,
      Slicer,
      Loot,
    }
  }
}
