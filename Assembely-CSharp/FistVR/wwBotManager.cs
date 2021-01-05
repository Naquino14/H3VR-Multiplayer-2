using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class wwBotManager : MonoBehaviour
	{
		[Serializable]
		public class BotProfile
		{
			public GameObject BanditPrefab;

			public GameObject[] PossePrefabs;

			public wwBotWurstNavPointGroup NavGroup;

			public wwBotWurstSpawnPointGroup SpawnGroup;

			public int RewardWhenReturned;
		}

		public wwParkManager ParkManager;

		private bool m_ShouldBotsBeActive;

		public wwDeputyBadge Badge;

		public List<GameObject> SpawnedBots = new List<GameObject>();

		private int[] m_state_Bandits = new int[10];

		private int m_nextBotToSpawn;

		public Renderer[] Posters;

		public Texture2D[] PosterTextures;

		public GameObject[] CompletionCheckMarks;

		private float CheckBanditsCompletionTick = 0.1f;

		public BotProfile[] BanditProfiles;

		public BotProfile[] BonanzaBotProfiles;

		[Header("Civilian Bot Config")]
		public BotProfile CivilianBotProfile_StartingArea;

		private List<GameObject> SpawnedCivilianBots_StartingArea = new List<GameObject>();

		private float CivBot_SpawnTick_StartingArea;

		public BotProfile CivilianBotProfile_MidTown;

		private List<GameObject> SpawnedCivilianBots_MidTown = new List<GameObject>();

		private float CivBot_SpawnTick_MidTown;

		public BotProfile CivilianBotProfile_DownTown;

		private List<GameObject> SpawnedCivilianBots_DownTown = new List<GameObject>();

		private float CivBot_SpawnTick_DownTown;

		private bool m_isInBonanzaMode;

		[Header("Other Config")]
		public AudioEvent PayoutSoundEvent;

		public ParticleSystem PSystemPayout;

		public GameObject PayoutPuffPrefab;

		private int BonanzaIndex;

		private void Start()
		{
			for (int i = 0; i < 3; i++)
			{
				SpawnCivvieBot(CivilianBotProfile_StartingArea, SpawnedCivilianBots_StartingArea);
			}
			for (int j = 0; j < 3; j++)
			{
				SpawnCivvieBot(CivilianBotProfile_MidTown, SpawnedCivilianBots_MidTown);
			}
			for (int k = 0; k < 3; k++)
			{
				SpawnCivvieBot(CivilianBotProfile_DownTown, SpawnedCivilianBots_DownTown);
			}
		}

		public void ConfigureBotState(int[] states, int nextBotToSpawn)
		{
			m_state_Bandits = states;
			m_nextBotToSpawn = nextBotToSpawn;
			if (m_state_Bandits[m_nextBotToSpawn] == 1 || m_state_Bandits[m_nextBotToSpawn] == 2)
			{
				m_state_Bandits[m_nextBotToSpawn] = 0;
				ParkManager.RegisterBotStateChange(m_nextBotToSpawn, 0);
			}
			if (m_nextBotToSpawn == 9 && states[9] == 3)
			{
				m_isInBonanzaMode = true;
			}
			if (m_nextBotToSpawn < 9)
			{
				if (PosterTextures[m_nextBotToSpawn] != null)
				{
					Renderer[] posters = Posters;
					foreach (Renderer renderer in posters)
					{
						renderer.materials[0].SetTexture("_MainTex", PosterTextures[m_nextBotToSpawn]);
					}
				}
			}
			else
			{
				Renderer[] posters2 = Posters;
				foreach (Renderer renderer2 in posters2)
				{
					renderer2.materials[0].SetTexture("_MainTex", PosterTextures[10]);
				}
			}
			UpdateCheckMarks(states);
		}

		public void UpdateCheckMarks(int[] states)
		{
			for (int i = 0; i < states.Length; i++)
			{
				if (states[i] == 0)
				{
					CompletionCheckMarks[i].SetActive(value: false);
				}
				else
				{
					CompletionCheckMarks[i].SetActive(value: true);
				}
			}
		}

		public void DespawnActiveBots()
		{
			if (SpawnedBots.Count <= 0)
			{
				return;
			}
			if (m_state_Bandits[m_nextBotToSpawn] == 1)
			{
				m_state_Bandits[m_nextBotToSpawn] = 0;
				ParkManager.RegisterBotStateChange(m_nextBotToSpawn, 0);
			}
			for (int num = SpawnedBots.Count - 1; num >= 0; num--)
			{
				if (SpawnedBots[num] != null)
				{
					UnityEngine.Object.Destroy(SpawnedBots[num]);
				}
			}
			SpawnedBots.Clear();
		}

		public void SpawnBot(int index)
		{
			m_state_Bandits[index] = 1;
			ParkManager.RegisterBotStateChange(index, 1);
			BotProfile botProfile = BanditProfiles[m_nextBotToSpawn];
			GameObject gameObject = UnityEngine.Object.Instantiate(botProfile.BanditPrefab, botProfile.SpawnGroup.Spawn_MainBandit.position, botProfile.SpawnGroup.Spawn_MainBandit.rotation);
			wwBotWurst component = gameObject.GetComponent<wwBotWurst>();
			component.ConfigBot(m_nextBotToSpawn, isPosse: false, this, botProfile.NavGroup, null);
			SpawnedBots.Add(gameObject);
			GM.CurrentSceneSettings.ShotEventReceivers.Add(gameObject);
			for (int i = 0; i < botProfile.PossePrefabs.Length; i++)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(botProfile.PossePrefabs[i], botProfile.SpawnGroup.Spawns_Posse[i].position, botProfile.SpawnGroup.Spawns_Posse[i].rotation);
				wwBotWurst component2 = gameObject2.GetComponent<wwBotWurst>();
				component2.ConfigBot(-1, isPosse: true, this, botProfile.NavGroup, null);
				SpawnedBots.Add(gameObject2);
				GM.CurrentSceneSettings.ShotEventReceivers.Add(gameObject2);
			}
		}

		public void BotKilled(int index, Vector3 v)
		{
			if (index >= 0 && index < 10)
			{
				m_state_Bandits[index] = 2;
				ParkManager.RegisterBotStateChange(index, 2);
			}
			else if (index >= 10 || index < 0)
			{
				GM.Omni.OmniUnlocks.GainCurrency(100);
				GM.Omni.SaveUnlocksToFile();
				UnityEngine.Object.Instantiate(PayoutPuffPrefab, v, Quaternion.identity);
			}
		}

		public void BotHatRetrieved(int index)
		{
			m_state_Bandits[index] = 3;
			GM.Omni.OmniUnlocks.GainCurrency(BanditProfiles[m_nextBotToSpawn].RewardWhenReturned);
			GM.Omni.SaveUnlocksToFile();
			ParkManager.RegisterBotStateChange(index, 3);
			SM.PlayGenericSound(PayoutSoundEvent, GM.CurrentPlayerBody.transform.position);
			PSystemPayout.Emit(100);
			if (m_nextBotToSpawn < 9)
			{
				m_nextBotToSpawn++;
				ParkManager.RegisterNextBotToSpawn(m_nextBotToSpawn);
				if (PosterTextures[m_nextBotToSpawn] != null)
				{
					Renderer[] posters = Posters;
					foreach (Renderer renderer in posters)
					{
						renderer.materials[0].SetTexture("_MainTex", PosterTextures[m_nextBotToSpawn]);
					}
				}
			}
			else
			{
				ParkManager.UnlockChest(1);
				Renderer[] posters2 = Posters;
				foreach (Renderer renderer2 in posters2)
				{
					renderer2.materials[0].SetTexture("_MainTex", PosterTextures[10]);
				}
			}
			CompletionCheckMarks[index].SetActive(value: true);
		}

		private void SpawnRandomBonanzaBotSet()
		{
			BotProfile botProfile = BonanzaBotProfiles[BonanzaIndex];
			BonanzaIndex++;
			if (BonanzaIndex >= BonanzaBotProfiles.Length)
			{
				BonanzaIndex = 4;
			}
			for (int i = 0; i < botProfile.PossePrefabs.Length; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(botProfile.PossePrefabs[i], botProfile.SpawnGroup.Spawns_Posse[i].position, botProfile.SpawnGroup.Spawns_Posse[i].rotation);
				wwBotWurst component = gameObject.GetComponent<wwBotWurst>();
				component.ConfigBot(11, isPosse: true, this, botProfile.NavGroup, null);
				SpawnedBots.Add(gameObject);
				GM.CurrentSceneSettings.ShotEventReceivers.Add(gameObject);
			}
		}

		private void CivilianBotTracker()
		{
			TrackCivBots(CivilianBotProfile_StartingArea, SpawnedCivilianBots_StartingArea, ref CivBot_SpawnTick_StartingArea);
			TrackCivBots(CivilianBotProfile_MidTown, SpawnedCivilianBots_MidTown, ref CivBot_SpawnTick_MidTown);
			TrackCivBots(CivilianBotProfile_DownTown, SpawnedCivilianBots_DownTown, ref CivBot_SpawnTick_DownTown);
		}

		private void TrackCivBots(BotProfile profile, List<GameObject> botList, ref float tick)
		{
			if (tick > 0f)
			{
				tick -= Time.deltaTime;
			}
			if (botList.Count > 0)
			{
				for (int num = botList.Count - 1; num >= 0; num--)
				{
					if (botList[num] == null)
					{
						botList.RemoveAt(num);
					}
				}
			}
			if (botList.Count < 3 && tick <= 0f)
			{
				tick = UnityEngine.Random.Range(10f, 15f);
				SpawnCivvieBot(profile, botList);
			}
		}

		private void SpawnCivvieBot(BotProfile profile, List<GameObject> botList)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(profile.BanditPrefab, profile.SpawnGroup.Spawns_Posse[UnityEngine.Random.Range(0, profile.SpawnGroup.Spawns_Posse.Length)].position, profile.SpawnGroup.Spawns_Posse[UnityEngine.Random.Range(0, profile.SpawnGroup.Spawns_Posse.Length)].rotation);
			wwBotWurst component = gameObject.GetComponent<wwBotWurst>();
			component.NavPointGroup = profile.NavGroup;
			botList.Add(gameObject);
		}

		private void Update()
		{
			CivilianBotTracker();
			if (Badge.QuickbeltSlot == null)
			{
				m_ShouldBotsBeActive = false;
			}
			else
			{
				m_ShouldBotsBeActive = true;
			}
			if (m_nextBotToSpawn == 9 && GM.WWSaveGame.Flags.State_Bandits[9] == 3)
			{
				m_isInBonanzaMode = true;
			}
			if (!m_ShouldBotsBeActive)
			{
				DespawnActiveBots();
			}
			else if (m_isInBonanzaMode)
			{
				if (SpawnedBots.Count <= 0)
				{
					SpawnRandomBonanzaBotSet();
				}
			}
			else if (m_state_Bandits[m_nextBotToSpawn] == 0)
			{
				SpawnBot(m_nextBotToSpawn);
			}
			if (ParkManager.RewardChests[1].GetState() >= 1)
			{
				return;
			}
			if (CheckBanditsCompletionTick > 0f)
			{
				CheckBanditsCompletionTick -= Time.deltaTime;
				return;
			}
			CheckBanditsCompletionTick = 1f;
			bool flag = false;
			if (m_nextBotToSpawn == 9 && GM.WWSaveGame.Flags.State_Bandits[9] == 3)
			{
				flag = true;
			}
			if (flag)
			{
				ParkManager.UnlockChest(1);
			}
		}
	}
}
