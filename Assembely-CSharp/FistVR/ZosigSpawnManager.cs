using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ZosigSpawnManager : MonoBehaviour
	{
		private List<ZosigTestSpawnEnemy> m_enemyTestSpawns = new List<ZosigTestSpawnEnemy>();

		private int m_spawnNPCTestIndex;

		private List<ZosigSpawnFromTable> m_spawnsFromTable = new List<ZosigSpawnFromTable>();

		private int m_spawnFromTableCheckIndex;

		private List<ZosigNpcSpawnPoint> m_npcSpawns = new List<ZosigNpcSpawnPoint>();

		private int m_spawnNPCCheckIndex;

		private List<ZosigEnemySpawnZone> m_zosigSpawnZones = new List<ZosigEnemySpawnZone>();

		private int m_zosigSpawnZoneIndex;

		private List<ZosigTurretSpawn> m_zosigTurrets = new List<ZosigTurretSpawn>();

		private int m_zosigTurretSpawnIndex;

		private bool m_hasInit;

		[Header("NPC STUFF")]
		public GameObject TempNPCPrefabEventuallyUseProfile;

		public GameObject NPCSpeechInferfacePrefab;

		public List<ZosigNPCProfile> Profiles;

		public List<SosigSpeechSet> NPCSpeechSets;

		public void Init()
		{
			m_hasInit = true;
			ZosigSpawnFromTable[] array = Object.FindObjectsOfType<ZosigSpawnFromTable>();
			for (int i = 0; i < array.Length; i++)
			{
				m_spawnsFromTable.Add(array[i]);
				array[i].Init();
			}
			ZosigNpcSpawnPoint[] array2 = Object.FindObjectsOfType<ZosigNpcSpawnPoint>();
			for (int j = 0; j < array2.Length; j++)
			{
				m_npcSpawns.Add(array2[j]);
			}
			ZosigTestSpawnEnemy[] array3 = Object.FindObjectsOfType<ZosigTestSpawnEnemy>();
			for (int k = 0; k < array3.Length; k++)
			{
				m_enemyTestSpawns.Add(array3[k]);
			}
			ZosigEnemySpawnZone[] array4 = Object.FindObjectsOfType<ZosigEnemySpawnZone>();
			for (int l = 0; l < array4.Length; l++)
			{
				m_zosigSpawnZones.Add(array4[l]);
				array4[l].M = this;
				array4[l].Init(GM.ZMaster);
			}
			ZosigTurretSpawn[] array5 = Object.FindObjectsOfType<ZosigTurretSpawn>();
			for (int m = 0; m < array5.Length; m++)
			{
				m_zosigTurrets.Add(array5[m]);
			}
		}

		public void Update()
		{
			if (m_hasInit)
			{
				if (m_spawnsFromTable.Count > 0)
				{
					SpawnFromTableLoop();
				}
				if (m_npcSpawns.Count > 0)
				{
					NPCSpawnLoop();
				}
				if (m_enemyTestSpawns.Count > 0)
				{
					TestEnemySpawnLoop();
				}
				if (m_zosigSpawnZones.Count > 0)
				{
					ZosigSpawnLoop();
				}
				if (m_zosigTurrets.Count > 0)
				{
					TurretSpawnLoop();
				}
			}
		}

		public void TurretSpawnLoop()
		{
			m_zosigTurretSpawnIndex++;
			if (m_zosigTurretSpawnIndex >= m_zosigTurrets.Count)
			{
				m_zosigTurretSpawnIndex = 0;
			}
			m_zosigTurrets[m_zosigTurretSpawnIndex].SpawnKernel(Time.deltaTime);
		}

		public void ZosigSpawnLoop()
		{
			float deltaTime = Time.deltaTime;
			for (int i = 0; i < m_zosigSpawnZones.Count; i++)
			{
				m_zosigSpawnZones[i].Tick(deltaTime);
			}
			m_zosigSpawnZoneIndex++;
			if (m_zosigSpawnZoneIndex >= m_zosigSpawnZones.Count)
			{
				m_zosigSpawnZoneIndex = 0;
			}
			m_zosigSpawnZones[m_zosigSpawnZoneIndex].Check();
		}

		public void SpawnFromTableLoop()
		{
			m_spawnFromTableCheckIndex++;
			if (m_spawnFromTableCheckIndex >= m_spawnsFromTable.Count)
			{
				m_spawnFromTableCheckIndex = 0;
			}
			m_spawnsFromTable[m_spawnFromTableCheckIndex].SpawnKernel();
		}

		public void TestEnemySpawnLoop()
		{
			m_spawnNPCTestIndex++;
			if (m_spawnNPCTestIndex >= m_enemyTestSpawns.Count)
			{
				m_spawnNPCTestIndex = 0;
			}
			ZosigTestSpawnEnemy zosigTestSpawnEnemy = m_enemyTestSpawns[m_spawnNPCTestIndex];
			if (!zosigTestSpawnEnemy.HasSpawned)
			{
				zosigTestSpawnEnemy.HasSpawned = true;
				SosigConfigTemplate t = zosigTestSpawnEnemy.Template.ConfigTemplates[0];
				SosigOutfitConfig o = zosigTestSpawnEnemy.Template.OutfitConfig[0];
				SpawnEnemySosig(zosigTestSpawnEnemy.Template.SosigPrefabs[0].GetGameObject(), zosigTestSpawnEnemy.Template.WeaponOptions[Random.Range(0, zosigTestSpawnEnemy.Template.WeaponOptions.Count)].GetGameObject(), zosigTestSpawnEnemy.transform.position, zosigTestSpawnEnemy.transform.rotation, t, o, 1);
			}
		}

		private Sosig SpawnEnemySosig(GameObject prefab, GameObject weapon, Vector3 pos, Quaternion rot, SosigConfigTemplate t, SosigOutfitConfig o, int IFF)
		{
			GameObject gameObject = Object.Instantiate(prefab, pos, rot);
			Sosig componentInChildren = gameObject.GetComponentInChildren<Sosig>();
			componentInChildren.Configure(t);
			componentInChildren.Inventory.FillAllAmmo();
			componentInChildren.E.IFFCode = IFF;
			SosigWeapon sosigWeapon = null;
			if (weapon != null)
			{
				sosigWeapon = Object.Instantiate(weapon, pos + Vector3.up * 2f + Vector3.right * 0.6f, rot).GetComponent<SosigWeapon>();
				sosigWeapon.SetAutoDestroy(b: true);
				sosigWeapon.O.SpawnLockable = false;
			}
			componentInChildren.CurrentOrder = Sosig.SosigOrder.GuardPoint;
			componentInChildren.FallbackOrder = Sosig.SosigOrder.GuardPoint;
			float num = 0f;
			num = Random.Range(0f, 1f);
			if (num < o.Chance_Headwear)
			{
				SpawnAccesoryToLink(o.Headwear, componentInChildren.Links[0]);
			}
			num = Random.Range(0f, 1f);
			if (num < o.Chance_Facewear)
			{
				SpawnAccesoryToLink(o.Facewear, componentInChildren.Links[0]);
			}
			num = Random.Range(0f, 1f);
			if (num < o.Chance_Eyewear)
			{
				SpawnAccesoryToLink(o.Eyewear, componentInChildren.Links[0]);
			}
			num = Random.Range(0f, 1f);
			if (num < o.Chance_Torsowear)
			{
				SpawnAccesoryToLink(o.Torsowear, componentInChildren.Links[1]);
			}
			num = Random.Range(0f, 1f);
			if (num < o.Chance_Pantswear)
			{
				SpawnAccesoryToLink(o.Pantswear, componentInChildren.Links[2]);
			}
			num = Random.Range(0f, 1f);
			if (num < o.Chance_Pantswear_Lower)
			{
				SpawnAccesoryToLink(o.Pantswear_Lower, componentInChildren.Links[3]);
			}
			num = Random.Range(0f, 1f);
			if (num < o.Chance_Backpacks)
			{
				SpawnAccesoryToLink(o.Backpacks, componentInChildren.Links[1]);
			}
			if (t.UsesLinkSpawns)
			{
				for (int i = 0; i < componentInChildren.Links.Count; i++)
				{
					float num2 = Random.Range(0f, 1f);
					if (num2 < t.LinkSpawnChance[i])
					{
						componentInChildren.Links[i].RegisterSpawnOnDestroy(t.LinkSpawns[i]);
					}
				}
			}
			if (sosigWeapon != null)
			{
				componentInChildren.InitHands();
				componentInChildren.ForceEquip(sosigWeapon);
			}
			return componentInChildren;
		}

		private void SpawnAccesoryToLink(List<FVRObject> gs, SosigLink l)
		{
			GameObject gameObject = Object.Instantiate(gs[Random.Range(0, gs.Count)].GetGameObject(), l.transform.position, l.transform.rotation);
			gameObject.transform.SetParent(l.transform);
			SosigWearable component = gameObject.GetComponent<SosigWearable>();
			component.RegisterWearable(l);
		}

		public void NPCSpawnLoop()
		{
			m_spawnNPCCheckIndex++;
			if (m_spawnNPCCheckIndex >= m_npcSpawns.Count)
			{
				m_spawnNPCCheckIndex = 0;
			}
			ZosigNpcSpawnPoint zosigNpcSpawnPoint = m_npcSpawns[m_spawnNPCCheckIndex];
			if (!zosigNpcSpawnPoint.HasSpawned && (!zosigNpcSpawnPoint.NeedsFlag || GM.ZMaster.FlagM.GetFlagValue(zosigNpcSpawnPoint.FlagToSpawn) >= zosigNpcSpawnPoint.FlagValueOrHigherToSpawn))
			{
				Sosig sosig = SpawnEnemySosig(zosigNpcSpawnPoint.Template.SosigPrefabs[0].GetGameObject(), zosigNpcSpawnPoint.Template.WeaponOptions[0].GetGameObject(), zosigNpcSpawnPoint.transform.position, zosigNpcSpawnPoint.transform.rotation, zosigNpcSpawnPoint.Template.ConfigTemplates[0], zosigNpcSpawnPoint.Template.OutfitConfig[0], 0);
				GameObject gameObject = Object.Instantiate(NPCSpeechInferfacePrefab, sosig.Links[0].transform.position, sosig.Links[0].transform.rotation);
				ZosigNPCInterface component = gameObject.GetComponent<ZosigNPCInterface>();
				sosig.Configure(zosigNpcSpawnPoint.Template.ConfigTemplates[0]);
				component.S = sosig;
				component.Profile = Profiles[zosigNpcSpawnPoint.NPCIndex];
				sosig.Speech = NPCSpeechSets[zosigNpcSpawnPoint.NPCIndex];
				component.S.E.IFFCode = 0;
				zosigNpcSpawnPoint.HasSpawned = true;
			}
		}

		public Sosig SpawnNPCToPoint(SosigEnemyTemplate Template, int index, Transform point)
		{
			Sosig sosig = SpawnEnemySosig(Template.SosigPrefabs[0].GetGameObject(), Template.WeaponOptions[0].GetGameObject(), point.transform.position, point.transform.rotation, Template.ConfigTemplates[0], Template.OutfitConfig[0], 0);
			sosig.Configure(Template.ConfigTemplates[0]);
			sosig.Speech = NPCSpeechSets[index];
			sosig.E.IFFCode = 0;
			return sosig;
		}
	}
}
