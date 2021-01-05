using System;
using UnityEngine;

namespace FistVR
{
	public class wwParkManager : MonoBehaviour
	{
		[Serializable]
		public class wwReward
		{
			public ItemSpawnerID[] ID;

			public Transform[] SpawnPoint;
		}

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

		public wwReward[] Rewards;

		private float dirtyFlagTick = 5f;

		private void Awake()
		{
		}

		private void Start()
		{
			Invoke("RecoverParkStateFromSaveFile", 0.1f);
			m_hasInitialized = true;
		}

		private void RecoverParkStateFromSaveFile()
		{
			TargetManager.ConfigurePuzzleStates(GM.WWSaveGame.Flags.State_TargetPuzzles, GM.WWSaveGame.Flags.State_TargetPuzzleSafes);
			BotManager.ConfigureBotState(GM.WWSaveGame.Flags.State_Bandits, GM.WWSaveGame.Flags.State_NextBanditToSpawn);
			HorseshoeManager.SetPlinthStates(GM.WWSaveGame.Flags.State_Horseshoes);
			ConfigureEventPuzzles(GM.WWSaveGame.Flags.State_EventPuzzles);
			ConfigureRewardChests(GM.WWSaveGame.Flags.State_Chests);
			ConfigureKeys(GM.WWSaveGame.Flags.State_Keys, GM.WWSaveGame.Flags.State_Chests);
			ConfigureEndDoors(GM.WWSaveGame.Flags.State_EndDoors);
			for (int i = 0; i < Rewards.Length; i++)
			{
				for (int j = 0; j < Rewards[i].ID.Length; j++)
				{
					if (GM.Rewards.RewardUnlocks.IsRewardUnlocked(Rewards[i].ID[j]))
					{
						UnityEngine.Object.Instantiate(Rewards[i].ID[j].MainObject.GetGameObject(), Rewards[i].SpawnPoint[j].position, Rewards[i].SpawnPoint[j].rotation);
					}
				}
			}
		}

		private void Update()
		{
			if (dirtyFlagTick > 0f)
			{
				dirtyFlagTick -= Time.deltaTime;
				return;
			}
			dirtyFlagTick = 5f;
			if (m_dirtyFlag && m_hasInitialized)
			{
				m_dirtyFlag = false;
				GM.WWSaveGame.SaveToFile();
			}
		}

		private void SaveGameState()
		{
			GM.WWSaveGame.SaveToFile();
			GM.Omni.SaveToFile();
		}

		public void RegisterCashChange()
		{
			m_dirtyFlag = true;
		}

		public void RegisterRewardChestStateChange(int index, int newState)
		{
			GM.WWSaveGame.Flags.State_Chests[index] = newState;
			m_dirtyFlag = true;
		}

		public void RegisterKeyStateChange(int index, int newState)
		{
			GM.WWSaveGame.Flags.State_Keys[index] = newState;
			m_dirtyFlag = true;
		}

		public void RegisterHorseshoeCompletion(int i)
		{
			GM.WWSaveGame.Flags.State_Horseshoes[i] = 1;
			m_dirtyFlag = true;
		}

		public void RegisterTargetPuzzleStateChange(int puzzle, int newState)
		{
			GM.WWSaveGame.Flags.State_TargetPuzzles[puzzle] = newState;
			m_dirtyFlag = true;
		}

		public void RegisterTargetPuzzleSafeStateChange(int puzzle, int newState)
		{
			GM.WWSaveGame.Flags.State_TargetPuzzleSafes[puzzle] = newState;
			m_dirtyFlag = true;
		}

		public void RegisterBotStateChange(int botIndex, int state)
		{
			GM.WWSaveGame.Flags.State_Bandits[botIndex] = state;
			m_dirtyFlag = true;
		}

		public void RegisterNextBotToSpawn(int bot)
		{
			GM.WWSaveGame.Flags.State_NextBanditToSpawn = bot;
			m_dirtyFlag = true;
		}

		public void RegisterDoorStateChange(int door, int state)
		{
			GM.WWSaveGame.Flags.State_EndDoors[door] = state;
			m_dirtyFlag = true;
		}

		public void RegisterEventPuzzleChange(int index, int state)
		{
			GM.WWSaveGame.Flags.State_EventPuzzles[index] = state;
			m_dirtyFlag = true;
		}

		private void ConfigureRewardChests(int[] chests)
		{
			for (int i = 0; i < chests.Length; i++)
			{
				RewardChests[i].SetState(chests[i], stateEvent: false);
			}
		}

		private void ConfigureKeys(int[] keys, int[] chests)
		{
			for (int i = 0; i < keys.Length; i++)
			{
				if (keys[i] == 0)
				{
					continue;
				}
				if (keys[i] == 1)
				{
					if (chests[i] == 2)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(KeyPrefab, RewardChests[i].KeyPosition.position, RewardChests[i].KeyPosition.rotation);
						wwKey component = gameObject.GetComponent<wwKey>();
						component.KeyIndex = i;
						component.State = 1;
						component.Manager = this;
					}
				}
				else if (keys[i] != 2)
				{
				}
			}
		}

		private void ConfigureEventPuzzles(int[] eventpuzzles)
		{
			for (int i = 0; i < EventPuzzles.Length; i++)
			{
				EventPuzzles[i].SetState(eventpuzzles[i]);
			}
		}

		private void ConfigureEndDoors(int[] doors)
		{
			for (int i = 0; i < FinaleManager.Doors.Length; i++)
			{
				FinaleManager.Doors[i].ConfigureDoorState(doors[i]);
				FinaleManager.ConfigureBlackOuts(doors);
			}
		}

		public void UnlockChest(int index)
		{
			RewardChests[index].UnlockChest();
		}

		public void ExplodeBullet()
		{
			(EventPuzzles[3] as wwEventPuzzle_SilverBullet).Explode();
		}
	}
}
