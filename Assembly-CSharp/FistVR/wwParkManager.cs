// Decompiled with JetBrains decompiler
// Type: FistVR.wwParkManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  public class wwParkManager : MonoBehaviour
  {
    [Header("Managers")]
    public wwTargetManager TargetManager;
    public wwBotManager BotManager;
    public wwHorseShoeGame HorseshoeManager;
    public wwRewardChest[] RewardChests;
    public wwFinaleManager FinaleManager;
    public GameObject KeyPrefab;
    public wwEventPuzzle[] EventPuzzles;
    public wwPASystem PASystem;
    private bool m_dirtyFlag;
    private bool m_hasInitialized;
    public wwParkManager.wwReward[] Rewards;
    private float dirtyFlagTick = 5f;

    private void Awake()
    {
    }

    private void Start()
    {
      this.Invoke("RecoverParkStateFromSaveFile", 0.1f);
      this.m_hasInitialized = true;
    }

    private void RecoverParkStateFromSaveFile()
    {
      this.TargetManager.ConfigurePuzzleStates(GM.WWSaveGame.Flags.State_TargetPuzzles, GM.WWSaveGame.Flags.State_TargetPuzzleSafes);
      this.BotManager.ConfigureBotState(GM.WWSaveGame.Flags.State_Bandits, GM.WWSaveGame.Flags.State_NextBanditToSpawn);
      this.HorseshoeManager.SetPlinthStates(GM.WWSaveGame.Flags.State_Horseshoes);
      this.ConfigureEventPuzzles(GM.WWSaveGame.Flags.State_EventPuzzles);
      this.ConfigureRewardChests(GM.WWSaveGame.Flags.State_Chests);
      this.ConfigureKeys(GM.WWSaveGame.Flags.State_Keys, GM.WWSaveGame.Flags.State_Chests);
      this.ConfigureEndDoors(GM.WWSaveGame.Flags.State_EndDoors);
      for (int index1 = 0; index1 < this.Rewards.Length; ++index1)
      {
        for (int index2 = 0; index2 < this.Rewards[index1].ID.Length; ++index2)
        {
          if (GM.Rewards.RewardUnlocks.IsRewardUnlocked(this.Rewards[index1].ID[index2]))
            UnityEngine.Object.Instantiate<GameObject>(this.Rewards[index1].ID[index2].MainObject.GetGameObject(), this.Rewards[index1].SpawnPoint[index2].position, this.Rewards[index1].SpawnPoint[index2].rotation);
        }
      }
    }

    private void Update()
    {
      if ((double) this.dirtyFlagTick > 0.0)
      {
        this.dirtyFlagTick -= Time.deltaTime;
      }
      else
      {
        this.dirtyFlagTick = 5f;
        if (!this.m_dirtyFlag || !this.m_hasInitialized)
          return;
        this.m_dirtyFlag = false;
        GM.WWSaveGame.SaveToFile();
      }
    }

    private void SaveGameState()
    {
      GM.WWSaveGame.SaveToFile();
      GM.Omni.SaveToFile();
    }

    public void RegisterCashChange() => this.m_dirtyFlag = true;

    public void RegisterRewardChestStateChange(int index, int newState)
    {
      GM.WWSaveGame.Flags.State_Chests[index] = newState;
      this.m_dirtyFlag = true;
    }

    public void RegisterKeyStateChange(int index, int newState)
    {
      GM.WWSaveGame.Flags.State_Keys[index] = newState;
      this.m_dirtyFlag = true;
    }

    public void RegisterHorseshoeCompletion(int i)
    {
      GM.WWSaveGame.Flags.State_Horseshoes[i] = 1;
      this.m_dirtyFlag = true;
    }

    public void RegisterTargetPuzzleStateChange(int puzzle, int newState)
    {
      GM.WWSaveGame.Flags.State_TargetPuzzles[puzzle] = newState;
      this.m_dirtyFlag = true;
    }

    public void RegisterTargetPuzzleSafeStateChange(int puzzle, int newState)
    {
      GM.WWSaveGame.Flags.State_TargetPuzzleSafes[puzzle] = newState;
      this.m_dirtyFlag = true;
    }

    public void RegisterBotStateChange(int botIndex, int state)
    {
      GM.WWSaveGame.Flags.State_Bandits[botIndex] = state;
      this.m_dirtyFlag = true;
    }

    public void RegisterNextBotToSpawn(int bot)
    {
      GM.WWSaveGame.Flags.State_NextBanditToSpawn = bot;
      this.m_dirtyFlag = true;
    }

    public void RegisterDoorStateChange(int door, int state)
    {
      GM.WWSaveGame.Flags.State_EndDoors[door] = state;
      this.m_dirtyFlag = true;
    }

    public void RegisterEventPuzzleChange(int index, int state)
    {
      GM.WWSaveGame.Flags.State_EventPuzzles[index] = state;
      this.m_dirtyFlag = true;
    }

    private void ConfigureRewardChests(int[] chests)
    {
      for (int index = 0; index < chests.Length; ++index)
        this.RewardChests[index].SetState(chests[index], false);
    }

    private void ConfigureKeys(int[] keys, int[] chests)
    {
      for (int index = 0; index < keys.Length; ++index)
      {
        if (keys[index] != 0)
        {
          if (keys[index] == 1)
          {
            if (chests[index] == 2)
            {
              wwKey component = UnityEngine.Object.Instantiate<GameObject>(this.KeyPrefab, this.RewardChests[index].KeyPosition.position, this.RewardChests[index].KeyPosition.rotation).GetComponent<wwKey>();
              component.KeyIndex = index;
              component.State = 1;
              component.Manager = this;
            }
          }
          else if (keys[index] != 2)
            ;
        }
      }
    }

    private void ConfigureEventPuzzles(int[] eventpuzzles)
    {
      for (int index = 0; index < this.EventPuzzles.Length; ++index)
        this.EventPuzzles[index].SetState(eventpuzzles[index]);
    }

    private void ConfigureEndDoors(int[] doors)
    {
      for (int index = 0; index < this.FinaleManager.Doors.Length; ++index)
      {
        this.FinaleManager.Doors[index].ConfigureDoorState(doors[index]);
        this.FinaleManager.ConfigureBlackOuts(doors);
      }
    }

    public void UnlockChest(int index) => this.RewardChests[index].UnlockChest();

    public void ExplodeBullet() => (this.EventPuzzles[3] as wwEventPuzzle_SilverBullet).Explode();

    [Serializable]
    public class wwReward
    {
      public ItemSpawnerID[] ID;
      public Transform[] SpawnPoint;
    }
  }
}
