using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class SosigTestingPanel1 : MonoBehaviour
	{
		public Transform[] SosigSpawningPoint;

		public List<GameObject> SosigPrefabs;

		public List<GameObject> SosigGunPrefabs;

		public SosigConfigTemplate BaseTemplate;

		public SosigConfigTemplate Template;

		public SosigWearableConfig[] WearableConfig_Team1;

		public SosigWearableConfig[] WearableConfig_Team2;

		public List<GameObject> Sosigcessories_Face;

		public List<GameObject> Sosigcessories_Helmet;

		public List<GameObject> Sosigcessories_Head;

		public List<GameObject> Sosigcessories_Torso;

		public List<GameObject> Sosigcessories_Backpack;

		public List<GameObject> Sosigcessories_UpperLink;

		public List<GameObject> Sosigcessories_LowerLink;

		public List<Material> SosigMats;

		public List<Transform> RespawnPoints;

		private List<GameObject> m_spawnedSosigs = new List<GameObject>();

		private List<GameObject> m_spawnedSosigGuns = new List<GameObject>();

		[Header("Team Fight Annex")]
		public List<Transform> AssaultPoints_Team1;

		public List<Transform> AssaultPoints_Team2;

		public List<Transform> SpawnPoints_Team1;

		public List<Transform> SpawnPoints_Team2;

		public List<GameObject> SosigWeapons_SMGs;

		public List<GameObject> SosigWeapons_Rifles;

		public List<GameObject> SosigWeapons_Handguns;

		public List<GameObject> SosigWeapons_Shotguns;

		public List<GameObject> SosigWeapons_Melee;

		public List<GameObject> SosigWeapons_Shield;

		public List<GameObject> SosigWeapons_Grenades;

		public bool m_isTeamFightEnabled;

		public int m_maxTeamSizeTeam1;

		public int m_maxTeamSizeTeam2;

		public int m_team1EquipMode;

		public int m_team2EquipMode;

		public int m_team1ArmorMode;

		public int m_team2ArmorMode;

		private int m_nextSpawnPointTeam1;

		private int m_nextSpawnPointTeam2;

		private List<Sosig> m_spawnedTeam1 = new List<Sosig>();

		private List<Sosig> m_spawnedTeam2 = new List<Sosig>();

		private float m_spawnSpeed = 3f;

		private float m_nextTeam1Spawn = 3f;

		private float m_nextTeam2Spawn = 3f;

		public Text NumBotsActive;

		public void TeamFight_Enabled()
		{
			m_isTeamFightEnabled = true;
		}

		public void TeamFight_Disabled()
		{
			m_isTeamFightEnabled = false;
			if (m_spawnedTeam1.Count > 0)
			{
				for (int num = m_spawnedTeam1.Count - 1; num >= 0; num--)
				{
					if (m_spawnedTeam1[num] != null)
					{
						m_spawnedTeam1[num].ClearSosig();
					}
				}
				m_spawnedTeam1.Clear();
			}
			if (m_spawnedTeam2.Count <= 0)
			{
				return;
			}
			for (int num2 = m_spawnedTeam2.Count - 1; num2 >= 0; num2--)
			{
				if (m_spawnedTeam2[num2] != null)
				{
					m_spawnedTeam2[num2].ClearSosig();
				}
			}
			m_spawnedTeam2.Clear();
		}

		public void SetTeam1EquipMode(int i)
		{
			m_team1EquipMode = i;
		}

		public void SetTeam2EquipMode(int i)
		{
			m_team2EquipMode = i;
		}

		public void SetTeam1ArmorMode(int i)
		{
			m_team1ArmorMode = i;
		}

		public void SetTeam2ArmorMode(int i)
		{
			m_team2ArmorMode = i;
		}

		public void SetTeamSizeTeam1(int i)
		{
			m_maxTeamSizeTeam1 = i;
		}

		public void SetTeamSizeTeam2(int i)
		{
			m_maxTeamSizeTeam2 = i;
		}

		public void Update()
		{
			UpdateTeamFight();
		}

		public void SetPlayerHealth(int i)
		{
			GM.CurrentPlayerBody.SetHealthThreshold(i);
		}

		private void UpdateTeamFight()
		{
			if (NumBotsActive != null)
			{
				NumBotsActive.text = m_spawnedTeam1.Count + m_spawnedTeam2.Count + " Bots";
			}
			if (m_spawnedTeam1.Count > 0)
			{
				for (int num = m_spawnedTeam1.Count - 1; num >= 0; num--)
				{
					if (m_spawnedTeam1[num].BodyState == Sosig.SosigBodyState.Dead)
					{
						m_spawnedTeam1[num].TickDownToClear(5f);
						m_spawnedTeam1.RemoveAt(num);
					}
				}
			}
			if (m_spawnedTeam2.Count > 0)
			{
				for (int num2 = m_spawnedTeam2.Count - 1; num2 >= 0; num2--)
				{
					if (m_spawnedTeam2[num2].BodyState == Sosig.SosigBodyState.Dead)
					{
						m_spawnedTeam2[num2].TickDownToClear(5f);
						m_spawnedTeam2.RemoveAt(num2);
					}
				}
			}
			if (m_isTeamFightEnabled)
			{
				if (m_spawnedTeam1.Count < m_maxTeamSizeTeam1 && m_nextTeam1Spawn > 0f)
				{
					m_nextTeam1Spawn -= Time.deltaTime;
				}
				if (m_spawnedTeam2.Count < m_maxTeamSizeTeam2 && m_nextTeam2Spawn > 0f)
				{
					m_nextTeam2Spawn -= Time.deltaTime;
				}
				if (m_nextTeam1Spawn <= 0f)
				{
					m_nextTeam1Spawn = Random.Range(m_spawnSpeed, m_spawnSpeed * 1.2f);
					SpawnTeamFightSosig(0);
				}
				if (m_nextTeam2Spawn <= 0f)
				{
					m_nextTeam2Spawn = Random.Range(m_spawnSpeed, m_spawnSpeed * 1.2f);
					SpawnTeamFightSosig(1);
				}
			}
		}

		private void SpawnTeamFightSosig(int team)
		{
			Transform transform = null;
			Transform transform2 = null;
			List<Sosig> list = null;
			if (team == 0)
			{
				transform = SpawnPoints_Team1[Random.Range(0, m_nextSpawnPointTeam1)];
				m_nextSpawnPointTeam1++;
				if (m_nextSpawnPointTeam1 >= SpawnPoints_Team1.Count)
				{
					m_nextSpawnPointTeam1 = 0;
				}
				transform2 = AssaultPoints_Team1[Random.Range(0, AssaultPoints_Team1.Count)];
				list = m_spawnedTeam1;
			}
			else
			{
				transform = SpawnPoints_Team2[Random.Range(0, m_nextSpawnPointTeam2)];
				m_nextSpawnPointTeam2++;
				if (m_nextSpawnPointTeam2 >= SpawnPoints_Team2.Count)
				{
					m_nextSpawnPointTeam2 = 0;
				}
				transform2 = AssaultPoints_Team2[Random.Range(0, AssaultPoints_Team2.Count)];
				list = m_spawnedTeam2;
			}
			SosigWearableConfig w = null;
			switch (team)
			{
			case 0:
			{
				int num2 = m_team1ArmorMode;
				if (num2 == 0)
				{
					num2 = Random.Range(1, 4);
				}
				w = WearableConfig_Team1[num2 - 1];
				break;
			}
			case 1:
			{
				int num = m_team2ArmorMode;
				if (num == 0)
				{
					num = Random.Range(1, 4);
				}
				w = WearableConfig_Team2[num - 1];
				break;
			}
			}
			Sosig sosig = SpawnEnemySosig(SosigPrefabs[0], transform.position, transform.rotation, Template, w);
			sosig.E.IFFCode = team + 1;
			sosig.CommandAssaultPoint(transform2.position);
			list.Add(sosig);
			SosigWeapon teamFightGun = GetTeamFightGun(team, grenade: false);
			teamFightGun.transform.position = transform.position;
			teamFightGun.SetAutoDestroy(b: true);
			if (teamFightGun != null)
			{
				sosig.InitHands();
				sosig.ForceEquip(teamFightGun);
			}
		}

		private void SpawnAccesoryToLink(List<GameObject> gs, SosigLink l)
		{
			if (gs.Count >= 1)
			{
				GameObject gameObject = Object.Instantiate(gs[Random.Range(0, gs.Count)], l.transform.position, l.transform.rotation);
				gameObject.transform.SetParent(l.transform);
				SosigWearable component = gameObject.GetComponent<SosigWearable>();
				component.RegisterWearable(l);
			}
		}

		private Sosig SpawnEnemySosig(GameObject prefab, Vector3 pos, Quaternion rot, SosigConfigTemplate t, SosigWearableConfig w)
		{
			GameObject gameObject = Object.Instantiate(prefab, pos, rot);
			Sosig componentInChildren = gameObject.GetComponentInChildren<Sosig>();
			componentInChildren.Inventory.FillAllAmmo();
			float num = 0f;
			num = Random.Range(0f, 1f);
			if (num < w.Chance_Headwear)
			{
				SpawnAccesoryToLink(w.Headwear, componentInChildren.Links[0]);
			}
			num = Random.Range(0f, 1f);
			if (num < w.Chance_Facewear)
			{
				SpawnAccesoryToLink(w.Facewear, componentInChildren.Links[0]);
			}
			num = Random.Range(0f, 1f);
			if (num < w.Chance_Torsowear)
			{
				SpawnAccesoryToLink(w.Torsowear, componentInChildren.Links[1]);
			}
			num = Random.Range(0f, 1f);
			if (num < w.Chance_Pantswear)
			{
				SpawnAccesoryToLink(w.Pantswear, componentInChildren.Links[2]);
			}
			num = Random.Range(0f, 1f);
			if (num < w.Chance_Pantswear_Lower)
			{
				SpawnAccesoryToLink(w.Pantswear_Lower, componentInChildren.Links[3]);
			}
			num = Random.Range(0f, 1f);
			if (num < w.Chance_Backpacks)
			{
				SpawnAccesoryToLink(w.Backpacks, componentInChildren.Links[1]);
			}
			componentInChildren.Configure(t);
			return componentInChildren;
		}

		public SosigWeapon GetTeamFightGun(int team, bool grenade, bool isShield = false)
		{
			int num = 0;
			switch (team)
			{
			case 0:
				num = m_team1EquipMode;
				break;
			case 1:
				num = m_team2EquipMode;
				break;
			}
			List<GameObject> list = SosigWeapons_SMGs;
			if (num == 0)
			{
				num = Random.Range(1, 6);
			}
			switch (num)
			{
			case 1:
				list = SosigWeapons_SMGs;
				break;
			case 2:
				list = SosigWeapons_Rifles;
				break;
			case 3:
				list = SosigWeapons_Handguns;
				break;
			case 4:
				list = SosigWeapons_Shotguns;
				break;
			case 5:
				list = SosigWeapons_Melee;
				break;
			}
			if (grenade)
			{
				list = SosigWeapons_Grenades;
			}
			if (isShield)
			{
				list = SosigWeapons_Shield;
			}
			GameObject original = list[Random.Range(0, list.Count)];
			GameObject gameObject = Object.Instantiate(original, new Vector3(0f, 30f, 0f), Quaternion.identity);
			return gameObject.GetComponent<SosigWeapon>();
		}

		public void Spawn1Sosigs()
		{
			for (int i = 0; i < 1; i++)
			{
				Transform transform = SosigSpawningPoint[Random.Range(0, SosigSpawningPoint.Length)];
				SpawnASosig(transform.position, transform.rotation, Random.Range(0, SosigMats.Count));
			}
		}

		public void Spawn5Sosigs()
		{
			for (int i = 0; i < 5; i++)
			{
				Transform transform = SosigSpawningPoint[Random.Range(0, SosigSpawningPoint.Length)];
				SpawnASosig(transform.position, transform.rotation, Random.Range(0, SosigMats.Count));
			}
		}

		private Sosig SpawnASosig(Vector3 pointToSpawn, Quaternion rotation, int matIndex)
		{
			GameObject gameObject = Object.Instantiate(SosigPrefabs[Random.Range(0, SosigPrefabs.Count)], pointToSpawn, Quaternion.identity);
			m_spawnedSosigs.Add(gameObject);
			Sosig componentInChildren = gameObject.GetComponentInChildren<Sosig>();
			componentInChildren.Inventory.FillAllAmmo();
			float num = Random.Range(0f, 1f);
			float num2 = Random.Range(0f, 1f);
			if (num > 0.6f)
			{
				SpawnAccesoryToLink(Sosigcessories_Face, componentInChildren.Links[0], matIndex);
			}
			num = Random.Range(0f, 1f);
			if (num > 0.1f)
			{
				SpawnAccesoryToLink(Sosigcessories_Helmet, componentInChildren.Links[0], matIndex);
				num2 = Random.Range(0f, 1f);
				if (num2 > 0.8f)
				{
					SpawnAccesoryToLink(Sosigcessories_Head, componentInChildren.Links[0], matIndex);
				}
			}
			num = Random.Range(0f, 1f);
			if (num > 0.2f)
			{
				SpawnAccesoryToLink(Sosigcessories_Torso, componentInChildren.Links[1], matIndex);
			}
			num = Random.Range(0f, 1f);
			if (num > 0.5f)
			{
				SpawnAccesoryToLink(Sosigcessories_UpperLink, componentInChildren.Links[2], matIndex);
				num2 = Random.Range(0f, 1f);
				if (num2 > 0.8f)
				{
					SpawnAccesoryToLink(Sosigcessories_LowerLink, componentInChildren.Links[3], matIndex);
				}
			}
			num = Random.Range(0f, 1f);
			if (num > 0.8f)
			{
				SpawnAccesoryToLink(Sosigcessories_Backpack, componentInChildren.Links[1], matIndex);
				num2 = Random.Range(0f, 1f);
				if (num2 > 0.8f)
				{
					SpawnAccesoryToLink(Sosigcessories_Backpack, componentInChildren.Links[2], matIndex);
				}
			}
			componentInChildren.Configure(Template);
			return componentInChildren;
		}

		private void SpawnAccesoryToLink(List<GameObject> gs, SosigLink l, int MatIndex)
		{
			if (gs.Count >= 1)
			{
				GameObject gameObject = Object.Instantiate(gs[Random.Range(0, gs.Count)], l.transform.position, l.transform.rotation);
				gameObject.transform.SetParent(l.transform);
				SosigWearable component = gameObject.GetComponent<SosigWearable>();
				component.RegisterWearable(l);
				MeshRenderer component2 = gameObject.GetComponent<MeshRenderer>();
				if (component2 != null && SosigMats.Contains(component2.sharedMaterial))
				{
					component2.material = SosigMats[MatIndex];
				}
			}
		}

		public void SpawnSosigun(int gun)
		{
			for (int i = 0; i < 1; i++)
			{
				Transform transform = SosigSpawningPoint[Random.Range(0, SosigSpawningPoint.Length)];
				GameObject item = Object.Instantiate(SosigGunPrefabs[gun], transform.position + Vector3.up * 2f, Random.rotation);
				m_spawnedSosigGuns.Add(item);
			}
		}

		public void SpawnRandomSosigun()
		{
			for (int i = 0; i < 5; i++)
			{
				Transform transform = SosigSpawningPoint[Random.Range(0, SosigSpawningPoint.Length)];
				GameObject item = Object.Instantiate(SosigGunPrefabs[Random.Range(0, SosigGunPrefabs.Count)], transform.position + Vector3.up * 2f, Random.rotation);
				m_spawnedSosigGuns.Add(item);
			}
		}

		public void ClearSosigs()
		{
			if (m_spawnedSosigs.Count == 0)
			{
				return;
			}
			for (int num = m_spawnedSosigs.Count - 1; num >= 0; num--)
			{
				if (m_spawnedSosigs[num] != null)
				{
					Object.Destroy(m_spawnedSosigs[num]);
				}
			}
			SosigLink[] array = Object.FindObjectsOfType<SosigLink>();
			if (array.Length > 0)
			{
				for (int num2 = array.Length - 1; num2 >= 0; num2--)
				{
					if (array[num2] != null)
					{
						Object.Destroy(array[num2].gameObject);
					}
				}
			}
			m_spawnedSosigs.Clear();
		}

		public void ClearSosigGuns()
		{
			if (m_spawnedSosigGuns.Count == 0)
			{
				return;
			}
			SetSosigOrder(0);
			for (int num = m_spawnedSosigGuns.Count - 1; num >= 0; num--)
			{
				if (m_spawnedSosigGuns[num] != null)
				{
					Object.Destroy(m_spawnedSosigGuns[num]);
				}
			}
			m_spawnedSosigGuns.Clear();
		}

		public void SetPlayerIFF(int iff)
		{
			switch (iff)
			{
			case 0:
				GM.CurrentPlayerBody.SetPlayerIFF(-3);
				break;
			case 1:
				GM.CurrentPlayerBody.SetPlayerIFF(0);
				GM.CurrentSceneSettings.DeathResetPoint = RespawnPoints[0];
				break;
			case 2:
				GM.CurrentPlayerBody.SetPlayerIFF(1);
				GM.CurrentSceneSettings.DeathResetPoint = RespawnPoints[1];
				break;
			case 3:
				GM.CurrentPlayerBody.SetPlayerIFF(2);
				GM.CurrentSceneSettings.DeathResetPoint = RespawnPoints[2];
				break;
			}
		}

		public void SetSosigIFF(int iff)
		{
			Sosig[] array = Object.FindObjectsOfType<Sosig>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].CurrentOrder == Sosig.SosigOrder.Disabled)
				{
					continue;
				}
				switch (iff)
				{
				case 0:
					array[i].E.IFFCode = 1;
					break;
				case 1:
					array[i].E.IFFCode = i + 1;
					break;
				case 2:
					if ((float)i < (float)array.Length / 2f)
					{
						array[i].E.IFFCode = 1;
					}
					else
					{
						array[i].E.IFFCode = 2;
					}
					break;
				}
			}
		}

		public void SetSosigOrder(int order)
		{
			Sosig[] array = Object.FindObjectsOfType<Sosig>();
			for (int i = 0; i < array.Length; i++)
			{
				if (order == 0)
				{
					array[i].CurrentOrder = Sosig.SosigOrder.Disabled;
					array[i].FallbackOrder = Sosig.SosigOrder.Disabled;
				}
				else
				{
					array[i].CurrentOrder = Sosig.SosigOrder.Wander;
					array[i].FallbackOrder = Sosig.SosigOrder.Wander;
				}
			}
		}
	}
}
