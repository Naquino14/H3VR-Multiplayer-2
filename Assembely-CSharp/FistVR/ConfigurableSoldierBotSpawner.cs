using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class ConfigurableSoldierBotSpawner : MonoBehaviour
	{
		public ClunkSpawner OldSpawner;

		public Text BotAmount;

		public Text BotAutoSpawnLabel;

		public wwBotWurstNavPointGroup NavGroup;

		public GameObject[] BotModernGunPrefabs;

		private List<List<GameObject>> BotModernGunsChart = new List<List<GameObject>>();

		public List<GameObject> BotModernGuns_Pistol;

		public List<GameObject> BotModernGuns_Shotgun;

		public List<GameObject> BotModernGuns_SMG;

		public List<GameObject> BotModernGuns_Rifle;

		public GameObject[] BotTypePrefabs;

		public GameObject HealthDrop;

		public wwBotWurstConfig[] BotConfigs;

		public GameObject[] BotSpecialPrefabs;

		public List<GameObject> BotSpecialWeapons;

		public wwBotWurstConfig[] BotSpecialConfigs;

		public int GunSetting = 4;

		public int ArmorSetting = 4;

		public int SpecialSetting;

		public int HealthSetting = 1;

		public int PerceptionSetting = 1;

		public int MovementSetting = 1;

		public int ManualSpawnSetting;

		public int AutoSpawnMaxSetting = 1;

		public int AutoSpawnSpeedSetting;

		private float[] HeadHealths = new float[3]
		{
			250f,
			2500f,
			25000f
		};

		private float[] BodyHealths = new float[3]
		{
			800f,
			8000f,
			80000f
		};

		private float[] MaxViewAngles = new float[3]
		{
			35f,
			70f,
			89f
		};

		private float[] MaxViewDistances = new float[3]
		{
			20f,
			50f,
			100f
		};

		private float[] MaxFiringAngles = new float[3]
		{
			15f,
			25f,
			35f
		};

		private float[] MaxFiringDistances = new float[3]
		{
			35f,
			55f,
			120f
		};

		private float[] MaxSpeed_Walks = new float[3]
		{
			0.4f,
			0.75f,
			1.2f
		};

		private float[] MaxSpeed_Combats = new float[3]
		{
			0.8f,
			1.5f,
			2f
		};

		private float[] MaxSpeed_Runs = new float[3]
		{
			1.5f,
			2.75f,
			4f
		};

		private float[] Accelerations = new float[3]
		{
			0.7f,
			1.5f,
			3f
		};

		private float[] MaxAngularSpeeds = new float[3]
		{
			1f,
			2f,
			3f
		};

		private int[] BotManualSpawnAmounts = new int[3]
		{
			1,
			2,
			3
		};

		public int[] AutoSpawnMaxAmounts = new int[5]
		{
			1,
			3,
			6,
			9,
			12
		};

		private float[] AutoSpawnSpeedTicks = new float[3]
		{
			10f,
			5f,
			1f
		};

		public OptionsPanel_ButtonSet OBS_GunSetting;

		public OptionsPanel_ButtonSet OBS_ArmorSetting;

		public OptionsPanel_ButtonSet OBS_SpecialSetting;

		public OptionsPanel_ButtonSet OBS_HealthSetting;

		public OptionsPanel_ButtonSet OBS_PerceptionSetting;

		public OptionsPanel_ButtonSet OBS_MovementSetting;

		public OptionsPanel_ButtonSet OBS_ManualSpawnAmount;

		public OptionsPanel_ButtonSet OBS_AutoSpawnMax;

		public OptionsPanel_ButtonSet OBS_AutoSpawnSpeed;

		public OptionsPanel_ButtonSet OBS_FastBullets;

		private List<GameObject> SpawnedBots = new List<GameObject>();

		public Transform[] SpawnPositions;

		public float DesiredSpawnRange = 10f;

		public float DesiredSpawnRangeMax = 10f;

		public bool UsesNavPointsToSpawn;

		private bool m_isAutoSpawnOn;

		public bool UsesDropList;

		public float DropChance = 0.1f;

		public SV_LootTable DropLootTable;

		[Header("GronchStuff")]
		public bool IsGronchActiveInScene;

		private bool m_isGronchFighting;

		public GameObject GronchPrefab;

		public GameObject GronchSpawningHotdogPrefab;

		private GameObject m_gronch;

		public Transform[] GronchNavPoints;

		public Transform[] GronchSpawnPoints;

		private Vector3 DisabledPos;

		private Vector3 m_InitialPos;

		public Transform HotDogSpawnPoint;

		private GameObject m_spawnedHotDog;

		private float SpawnTick;

		private Vector2 SpawnTimeRange = new Vector2(3f, 10f);

		private bool m_usingFastBullets;

		public void Awake()
		{
			OBS_GunSetting.SetSelectedButton(GunSetting);
			OBS_ArmorSetting.SetSelectedButton(ArmorSetting);
			if (OBS_SpecialSetting != null)
			{
				OBS_SpecialSetting.SetSelectedButton(SpecialSetting);
			}
			OBS_HealthSetting.SetSelectedButton(HealthSetting);
			OBS_PerceptionSetting.SetSelectedButton(PerceptionSetting);
			OBS_MovementSetting.SetSelectedButton(MovementSetting);
			OBS_ManualSpawnAmount.SetSelectedButton(ManualSpawnSetting);
			OBS_AutoSpawnMax.SetSelectedButton(AutoSpawnMaxSetting);
			OBS_AutoSpawnSpeed.SetSelectedButton(AutoSpawnSpeedSetting);
			if (OBS_FastBullets != null)
			{
				OBS_FastBullets.SetSelectedButton(0);
			}
			m_InitialPos = base.transform.root.position;
			DisabledPos = new Vector3(m_InitialPos.x, -1000f, m_InitialPos.z);
			BotModernGunsChart.Add(BotModernGuns_Pistol);
			BotModernGunsChart.Add(BotModernGuns_Shotgun);
			BotModernGunsChart.Add(BotModernGuns_SMG);
			BotModernGunsChart.Add(BotModernGuns_Rifle);
		}

		private void Update()
		{
			SpawnTimeRange = new Vector2(AutoSpawnSpeedTicks[AutoSpawnSpeedSetting], AutoSpawnSpeedTicks[AutoSpawnSpeedSetting] * 3f);
			if (m_isAutoSpawnOn && SpawnedBots.Count < AutoSpawnMaxAmounts[AutoSpawnMaxSetting])
			{
				if (SpawnTick > 0f)
				{
					SpawnTick -= Time.deltaTime;
				}
				else
				{
					SpawnTick = Random.Range(SpawnTimeRange.x, SpawnTimeRange.y);
					SpawnBot(GetRandomDistantSpawnPoint());
				}
			}
			for (int num = SpawnedBots.Count - 1; num >= 0; num--)
			{
				if (SpawnedBots[num] == null)
				{
					SpawnedBots.RemoveAt(num);
				}
			}
			BotAmount.text = "Bots Spawned = " + SpawnedBots.Count;
			if (IsGronchActiveInScene && !m_isGronchFighting && m_spawnedHotDog == null)
			{
				GameObject gameObject = Object.Instantiate(GronchSpawningHotdogPrefab, HotDogSpawnPoint.position, HotDogSpawnPoint.rotation);
				gameObject.GetComponent<InvasionDog>().Spawner = this;
				m_spawnedHotDog = gameObject;
			}
		}

		public void ToggleAutoSpawn()
		{
			m_isAutoSpawnOn = !m_isAutoSpawnOn;
			if (m_isAutoSpawnOn)
			{
				BotAutoSpawnLabel.text = "Turn Off AutoSpawn";
			}
			else
			{
				BotAutoSpawnLabel.text = "Turn On AutoSpawn";
			}
		}

		public void SetSetting_Gun(int i)
		{
			GunSetting = i;
		}

		public void SetSetting_Armor(int i)
		{
			ArmorSetting = i;
		}

		public void SetSetting_SpecialTypes(int i)
		{
			SpecialSetting = i;
		}

		public void SetSetting_Health(int i)
		{
			HealthSetting = i;
		}

		public void SetSetting_Perception(int i)
		{
			PerceptionSetting = i;
		}

		public void SetSetting_Movement(int i)
		{
			MovementSetting = i;
		}

		public void SetSetting_ManualSpawnAmount(int i)
		{
			ManualSpawnSetting = i;
		}

		public void SetSetting_AutoSpawnMaxBots(int i)
		{
			AutoSpawnMaxSetting = i;
		}

		public void SetSetting_AutoSpawnSpeed(int i)
		{
			AutoSpawnSpeedSetting = i;
		}

		public void SetSetting_FastBullets(bool b)
		{
			m_usingFastBullets = b;
		}

		public void SpawnManual()
		{
			int num = BotManualSpawnAmounts[ManualSpawnSetting];
			for (int i = 0; i < num; i++)
			{
				SpawnBot(GetRandomDistantSpawnPoint());
			}
		}

		public void SpawnManualAtPoint(Transform point)
		{
			SpawnBot(point);
		}

		public Transform GetRandomDistantSpawnPoint()
		{
			if (UsesNavPointsToSpawn)
			{
				List<wwBotWurstNavPoint> dynamicSet = NavGroup.GetDynamicSet();
				for (int i = 0; i < dynamicSet.Count; i++)
				{
					float num = Vector3.Distance(dynamicSet[i].transform.position, GM.CurrentPlayerBody.transform.position);
					if (num > DesiredSpawnRange && num < DesiredSpawnRangeMax)
					{
						return dynamicSet[i].transform;
					}
				}
				return dynamicSet[0].transform;
			}
			int[] array = ChooseRandomIndicies(SpawnPositions.Length);
			for (int j = 0; j < array.Length; j++)
			{
				float num2 = Vector3.Distance(SpawnPositions[array[j]].position, GM.CurrentPlayerBody.transform.position);
				if (num2 > DesiredSpawnRange)
				{
					return SpawnPositions[array[j]];
				}
			}
			return SpawnPositions[array[0]];
		}

		private int[] ChooseRandomIndicies(int length)
		{
			int[] array = new int[length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = i;
			}
			for (int j = 0; j < array.Length; j++)
			{
				int num = array[j];
				int num2 = Random.Range(j, length);
				array[j] = array[num2];
				array[num2] = num;
			}
			return array;
		}

		public void SpawnBot(Transform spawnPoint)
		{
			bool flag = false;
			if (SpecialSetting == 1)
			{
				float num = Random.Range(0f, 1f);
				if (num > 0.5f)
				{
					flag = true;
				}
			}
			else if (SpecialSetting == 2)
			{
				flag = true;
			}
			int num2 = 0;
			GameObject gameObject;
			if (flag)
			{
				num2 = Random.Range(0, BotSpecialPrefabs.Length);
				gameObject = Object.Instantiate(BotSpecialPrefabs[num2], spawnPoint.position, spawnPoint.rotation);
			}
			else
			{
				num2 = ArmorSetting;
				if (ArmorSetting >= 4)
				{
					num2 = Random.Range(0, 4);
				}
				gameObject = Object.Instantiate(BotTypePrefabs[num2], spawnPoint.position, spawnPoint.rotation);
			}
			wwBotWurst component = gameObject.GetComponent<wwBotWurst>();
			int num3 = 0;
			GameObject gameObject2 = null;
			if (flag)
			{
				num3 = num2;
				gameObject2 = BotSpecialWeapons[num3];
			}
			else
			{
				num3 = GunSetting;
				if (GunSetting >= 4)
				{
					num3 = Random.Range(0, 4);
				}
				List<GameObject> list = BotModernGunsChart[num3];
				gameObject2 = list[Random.Range(0, list.Count)];
			}
			GameObject gameObject3 = Object.Instantiate(gameObject2, component.ModernGunMount.position, component.ModernGunMount.rotation);
			wwBotWurstModernGun component2 = gameObject3.GetComponent<wwBotWurstModernGun>();
			component.ModernGuns.Add(component2);
			component2.Bot = component;
			component2.transform.SetParent(component.ModernGunMount.parent);
			if (m_usingFastBullets)
			{
				component2.SetUseFastProjectile(b: true);
			}
			component.Config = BotConfigs[num3];
			component.ReConfig(BotConfigs[num3]);
			int num4 = HealthSetting;
			if (HealthSetting >= 3)
			{
				num4 = Random.Range(0, 3);
			}
			component.Pieces[0].SetLife(HeadHealths[num4]);
			component.Pieces[1].SetLife(BodyHealths[num4]);
			component.Pieces[2].SetLife(BodyHealths[num4]);
			int perceptionSetting = PerceptionSetting;
			if (PerceptionSetting >= 3)
			{
				perceptionSetting = Random.Range(0, 3);
			}
			int num5 = MovementSetting;
			if (MovementSetting >= 3)
			{
				num5 = Random.Range(0, 3);
			}
			component.Max_LinearSpeed_Walk = MaxSpeed_Walks[num5];
			component.Max_LinearSpeed_Combat = MaxSpeed_Combats[num5];
			component.Max_LinearSpeed_Run = MaxSpeed_Runs[num5];
			component.Acceleration = Accelerations[num5];
			component.Max_AngularSpeed = MaxAngularSpeeds[num5];
			component.NavPointGroup = NavGroup;
			float num6 = Random.Range(0f, 1f);
			if (UsesDropList)
			{
				if (num6 < DropChance)
				{
					SV_LootTableEntry weightedRandomEntry = DropLootTable.GetWeightedRandomEntry();
					component.DropOnDeath = weightedRandomEntry.MainObj.GetGameObject();
				}
			}
			else if (num6 > 0.75f)
			{
				component.DropOnDeath = HealthDrop;
			}
			SpawnedBots.Add(gameObject);
			GM.CurrentSceneSettings.ShotEventReceivers.Add(gameObject);
		}

		public void DeleteBots()
		{
			if (SpawnedBots.Count > 0)
			{
				for (int num = SpawnedBots.Count - 1; num >= 0; num--)
				{
					Object.Destroy(SpawnedBots[num]);
				}
			}
			SpawnedBots.Clear();
		}

		public void PlayerDied()
		{
			DeleteBots();
			m_isAutoSpawnOn = false;
			BotAutoSpawnLabel.text = "Turn On AutoSpawn";
			base.transform.root.position = m_InitialPos;
			if (OldSpawner != null)
			{
				OldSpawner.PlayerDied();
			}
			if (IsGronchActiveInScene)
			{
				if (m_gronch != null)
				{
					Object.Destroy(m_gronch);
				}
				m_isGronchFighting = false;
				if (m_spawnedHotDog == null)
				{
					GameObject gameObject = Object.Instantiate(GronchSpawningHotdogPrefab, HotDogSpawnPoint.position, HotDogSpawnPoint.rotation);
					gameObject.GetComponent<InvasionDog>().Spawner = this;
					m_spawnedHotDog = gameObject;
				}
			}
		}

		public void SpawnGronch()
		{
			if (!m_isGronchFighting)
			{
				m_isGronchFighting = true;
				GameObject gameObject = Object.Instantiate(GronchPrefab, GronchNavPoints[0].position, GronchNavPoints[0].rotation);
				MM_GronchShip component = gameObject.GetComponent<MM_GronchShip>();
				m_gronch = gameObject;
				component.BotSpawner = this;
				component.NavPoints = GronchNavPoints;
				component.SpawnPoints = GronchSpawnPoints;
				base.transform.root.position = DisabledPos;
				DeleteBots();
				m_isAutoSpawnOn = false;
				BotAutoSpawnLabel.text = "Turn On AutoSpawn";
			}
		}

		public void GronchIsDead()
		{
			m_isGronchFighting = false;
			if (m_spawnedHotDog == null)
			{
				GameObject gameObject = Object.Instantiate(GronchSpawningHotdogPrefab, HotDogSpawnPoint.position, HotDogSpawnPoint.rotation);
				gameObject.GetComponent<InvasionDog>().Spawner = this;
				m_spawnedHotDog = gameObject;
			}
			base.transform.root.position = m_InitialPos;
		}
	}
}
