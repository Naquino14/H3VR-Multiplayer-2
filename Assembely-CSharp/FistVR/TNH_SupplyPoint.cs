using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class TNH_SupplyPoint : MonoBehaviour
	{
		public enum SupplyPanelType
		{
			AmmoReloader,
			MagDuplicator,
			GunRecycler
		}

		public TNH_Manager M;

		public TNH_TakeChallenge T;

		public Transform Bounds;

		public List<AICoverPoint> CoverPoints;

		public Transform SpawnPoint_PlayerSpawn;

		public List<Transform> SpawnPoints_Sosigs_Defense;

		public List<Transform> SpawnPoints_Turrets;

		public List<Transform> SpawnPoints_Panels;

		public List<Transform> SpawnPoints_Boxes;

		public List<Transform> SpawnPoint_Tables;

		public Transform SpawnPoint_CaseLarge;

		public Transform SpawnPoint_CaseSmall;

		public Transform SpawnPoint_Melee;

		public List<Transform> SpawnPoints_SmallItem;

		public Transform SpawnPoint_Shield;

		[Header("Debug")]
		public bool ShowPoint_PlayerSpawn;

		public bool ShowPoints_Sosigs_Defense;

		public bool ShowPoints_Turrets;

		public bool ShowPoints_Screens;

		public bool ShowPoints_Boxes;

		public bool ShowPoints_SpawnTable;

		public Mesh GizmoMesh_SosigAttack;

		public Mesh GizmoMesh_Panel;

		public Mesh GizmoMesh_Box;

		public Mesh GizmoMesh_Table;

		public Mesh GizmoMesh_CaseLarge;

		public Mesh GizmoMesh_CaseSmall;

		public Mesh GizmoMesh_Melee;

		private List<GameObject> m_trackedObjects = new List<GameObject>();

		private List<Sosig> m_activeSosigs = new List<Sosig>();

		private List<AutoMeater> m_activeTurrets = new List<AutoMeater>();

		private List<GameObject> m_spawnBoxes = new List<GameObject>();

		private GameObject m_constructor;

		private GameObject m_panel;

		private bool m_hasBeenVisited;

		private TAH_ReticleContact m_contact;

		public void SetContact(TAH_ReticleContact c)
		{
			m_contact = c;
		}

		private void Start()
		{
			GM.CurrentSceneSettings.SosigKillEvent += CheckIfDeadSosigWasMine;
		}

		private void OnDestroy()
		{
			GM.CurrentSceneSettings.SosigKillEvent -= CheckIfDeadSosigWasMine;
		}

		public void CheckIfDeadSosigWasMine(Sosig s)
		{
			if (m_activeSosigs.Contains(s))
			{
				s.TickDownToClear(3f);
				m_activeSosigs.Remove(s);
			}
		}

		public void Configure(TNH_TakeChallenge t, bool spawnSosigs, bool spawnDefenses, bool spawnConstructor, SupplyPanelType panelType, int minBoxPiles, int maxBoxPiles)
		{
			T = t;
			if (spawnSosigs)
			{
				SpawnTakeEnemyGroup();
			}
			if (spawnDefenses)
			{
				SpawnDefenses();
			}
			if (spawnConstructor)
			{
				SpawnConstructor();
				SpawnSecondaryPanel(panelType);
			}
			if (maxBoxPiles > 0)
			{
				SpawnBoxes(minBoxPiles, maxBoxPiles);
			}
			m_hasBeenVisited = false;
		}

		public void TestVisited()
		{
			if (!m_hasBeenVisited && TestVolumeBool(Bounds, GM.CurrentPlayerBody.transform.position))
			{
				m_hasBeenVisited = true;
				if (m_contact != null)
				{
					m_contact.SetVisited(b: true);
				}
			}
		}

		private void SpawnConstructor()
		{
			SpawnPoints_Panels.Shuffle();
			m_constructor = M.SpawnObjectConstructor(SpawnPoints_Panels[0]);
		}

		private void SpawnSecondaryPanel(SupplyPanelType t)
		{
			if (M.EquipmentMode == TNHSetting_EquipmentMode.Spawnlocking && t == SupplyPanelType.MagDuplicator)
			{
				t = SupplyPanelType.GunRecycler;
			}
			switch (t)
			{
			case SupplyPanelType.AmmoReloader:
				m_panel = M.SpawnAmmoReloader(SpawnPoints_Panels[1]);
				break;
			case SupplyPanelType.MagDuplicator:
				m_panel = M.SpawnMagDuplicator(SpawnPoints_Panels[1]);
				break;
			case SupplyPanelType.GunRecycler:
				m_panel = M.SpawnGunRecycler(SpawnPoints_Panels[1]);
				break;
			}
		}

		private void SpawnBoxes(int min, int max)
		{
			float num = Random.Range(0f, 1f);
			bool flag = false;
			if (num > 0.2f)
			{
				flag = true;
			}
			float num2 = Random.Range(0f, 1f);
			bool flag2 = false;
			if (num > 0.1f)
			{
				flag2 = true;
			}
			float num3 = Random.Range(0f, 1f);
			bool flag3 = false;
			if (num > 0.6f)
			{
				flag3 = true;
			}
			float num4 = Random.Range(0f, 1f);
			bool flag4 = false;
			if (num > 0.8f)
			{
				flag4 = true;
			}
			SpawnPoints_Boxes.Shuffle();
			int num5 = Random.Range(min, max + 1);
			if (num5 < 1)
			{
				return;
			}
			for (int i = 0; i < num5; i++)
			{
				Transform transform = SpawnPoints_Boxes[i];
				int num6 = Random.Range(1, 3);
				for (int j = 0; j < num6; j++)
				{
					Vector3 position = transform.position + Vector3.up * 0.1f + Vector3.up * 0.85f * j;
					Quaternion rotation = Quaternion.Slerp(transform.rotation, Random.rotation, 0.1f);
					GameObject item = Object.Instantiate(M.Prefabs_ShatterableCrates[Random.Range(0, M.Prefabs_ShatterableCrates.Count)], position, rotation);
					m_spawnBoxes.Add(item);
				}
			}
			m_spawnBoxes.Shuffle();
			int num7 = 0;
			if (flag && m_spawnBoxes.Count > num7)
			{
				GameObject gameObject = m_spawnBoxes[num7];
				TNH_ShatterableCrate component = gameObject.GetComponent<TNH_ShatterableCrate>();
				component.SetHoldingToken(M);
				num7++;
			}
			if (flag2 && m_spawnBoxes.Count > num7)
			{
				GameObject gameObject2 = m_spawnBoxes[num7];
				TNH_ShatterableCrate component2 = gameObject2.GetComponent<TNH_ShatterableCrate>();
				component2.SetHoldingHealth(M);
				num7++;
			}
			if (flag3 && m_spawnBoxes.Count > num7)
			{
				GameObject gameObject3 = m_spawnBoxes[num7];
				TNH_ShatterableCrate component3 = gameObject3.GetComponent<TNH_ShatterableCrate>();
				component3.SetHoldingHealth(M);
				num7++;
			}
			if (flag4 && m_spawnBoxes.Count > num7)
			{
				GameObject gameObject4 = m_spawnBoxes[num7];
				TNH_ShatterableCrate component4 = gameObject4.GetComponent<TNH_ShatterableCrate>();
				component4.SetHoldingHealth(M);
				num7++;
			}
		}

		private void SpawnTakeEnemyGroup()
		{
			SpawnPoints_Sosigs_Defense.Shuffle();
			SpawnPoints_Sosigs_Defense.Shuffle();
			int value = Random.Range(T.NumGuards - 1, T.NumGuards + 1);
			value = Mathf.Clamp(value, 0, 5);
			for (int i = 0; i < value; i++)
			{
				Transform transform = SpawnPoints_Sosigs_Defense[i];
				SosigEnemyTemplate t = ManagerSingleton<IM>.Instance.odicSosigObjsByID[T.GID];
				Sosig item = M.SpawnEnemy(t, transform, T.IFFUsed, IsAssault: false, transform.position, AllowAllWeapons: false);
				m_activeSosigs.Add(item);
			}
		}

		private void SpawnDefenses()
		{
			TNH_TurretType turretType = T.TurretType;
			FVRObject turretPrefab = M.GetTurretPrefab(turretType);
			int value = Random.Range(T.NumTurrets - 1, T.NumTurrets + 1);
			value = Mathf.Clamp(value, 0, 5);
			for (int i = 0; i < value; i++)
			{
				Vector3 position = SpawnPoints_Turrets[i].position + Vector3.up * 0.25f;
				GameObject gameObject = Object.Instantiate(turretPrefab.GetGameObject(), position, SpawnPoints_Turrets[i].rotation);
				m_activeTurrets.Add(gameObject.GetComponent<AutoMeater>());
			}
		}

		public void ConfigureAtBeginning(TNH_CharacterDef c)
		{
			m_trackedObjects.Clear();
			if (M.ItemSpawnerMode == TNH_ItemSpawnerMode.On)
			{
				M.ItemSpawner.transform.position = SpawnPoints_Panels[0].position + Vector3.up * 0.8f;
				M.ItemSpawner.transform.rotation = SpawnPoints_Panels[0].rotation;
				M.ItemSpawner.SetActive(value: true);
			}
			for (int i = 0; i < SpawnPoint_Tables.Count; i++)
			{
				GameObject item = Object.Instantiate(M.Prefab_MetalTable, SpawnPoint_Tables[i].position, SpawnPoint_Tables[i].rotation);
				m_trackedObjects.Add(item);
			}
			if (c.Has_Weapon_Primary)
			{
				TNH_CharacterDef.LoadoutEntry weapon_Primary = c.Weapon_Primary;
				FVRObject fVRObject = null;
				int minAmmo = -1;
				int maxAmmo = -1;
				if (weapon_Primary.ListOverride.Count > 0)
				{
					fVRObject = weapon_Primary.ListOverride[Random.Range(0, weapon_Primary.ListOverride.Count)];
				}
				else
				{
					ObjectTableDef objectTableDef = weapon_Primary.TableDefs[Random.Range(0, weapon_Primary.TableDefs.Count)];
					ObjectTable objectTable = new ObjectTable();
					objectTable.Initialize(objectTableDef);
					fVRObject = objectTable.GetRandomObject();
					minAmmo = objectTableDef.MinAmmoCapacity;
					maxAmmo = objectTableDef.MaxAmmoCapacity;
				}
				GameObject gameObject = M.SpawnWeaponCase(M.Prefab_WeaponCaseLarge, SpawnPoint_CaseLarge.position, SpawnPoint_CaseLarge.forward, fVRObject, weapon_Primary.Num_Mags_SL_Clips, weapon_Primary.Num_Rounds, minAmmo, maxAmmo, weapon_Primary.AmmoObjectOverride);
				m_trackedObjects.Add(gameObject);
				gameObject.GetComponent<TNH_WeaponCrate>().M = M;
			}
			if (c.Has_Weapon_Secondary)
			{
				TNH_CharacterDef.LoadoutEntry weapon_Secondary = c.Weapon_Secondary;
				FVRObject fVRObject2 = null;
				int minAmmo2 = -1;
				int maxAmmo2 = -1;
				if (weapon_Secondary.ListOverride.Count > 0)
				{
					fVRObject2 = weapon_Secondary.ListOverride[Random.Range(0, weapon_Secondary.ListOverride.Count)];
				}
				else
				{
					ObjectTableDef objectTableDef2 = weapon_Secondary.TableDefs[Random.Range(0, weapon_Secondary.TableDefs.Count)];
					ObjectTable objectTable2 = new ObjectTable();
					objectTable2.Initialize(objectTableDef2);
					fVRObject2 = objectTable2.GetRandomObject();
					minAmmo2 = objectTableDef2.MinAmmoCapacity;
					maxAmmo2 = objectTableDef2.MaxAmmoCapacity;
				}
				GameObject gameObject2 = M.SpawnWeaponCase(M.Prefab_WeaponCaseSmall, SpawnPoint_CaseSmall.position, SpawnPoint_CaseSmall.forward, fVRObject2, weapon_Secondary.Num_Mags_SL_Clips, weapon_Secondary.Num_Rounds, minAmmo2, maxAmmo2, weapon_Secondary.AmmoObjectOverride);
				m_trackedObjects.Add(gameObject2);
				gameObject2.GetComponent<TNH_WeaponCrate>().M = M;
			}
			if (c.Has_Weapon_Tertiary)
			{
				TNH_CharacterDef.LoadoutEntry weapon_Tertiary = c.Weapon_Tertiary;
				FVRObject fVRObject3 = null;
				if (weapon_Tertiary.ListOverride.Count > 0)
				{
					fVRObject3 = weapon_Tertiary.ListOverride[Random.Range(0, weapon_Tertiary.ListOverride.Count)];
				}
				else
				{
					ObjectTableDef d = weapon_Tertiary.TableDefs[Random.Range(0, weapon_Tertiary.TableDefs.Count)];
					ObjectTable objectTable3 = new ObjectTable();
					objectTable3.Initialize(d);
					fVRObject3 = objectTable3.GetRandomObject();
				}
				GameObject g = Object.Instantiate(fVRObject3.GetGameObject(), SpawnPoint_Melee.position, SpawnPoint_Melee.rotation);
				M.AddObjectToTrackedList(g);
			}
			if (c.Has_Item_Primary)
			{
				TNH_CharacterDef.LoadoutEntry item_Primary = c.Item_Primary;
				FVRObject fVRObject4 = null;
				if (item_Primary.ListOverride.Count > 0)
				{
					fVRObject4 = item_Primary.ListOverride[Random.Range(0, item_Primary.ListOverride.Count)];
				}
				else
				{
					ObjectTableDef d2 = item_Primary.TableDefs[Random.Range(0, item_Primary.TableDefs.Count)];
					ObjectTable objectTable4 = new ObjectTable();
					objectTable4.Initialize(d2);
					fVRObject4 = objectTable4.GetRandomObject();
				}
				GameObject g2 = Object.Instantiate(fVRObject4.GetGameObject(), SpawnPoints_SmallItem[0].position, SpawnPoints_SmallItem[0].rotation);
				M.AddObjectToTrackedList(g2);
			}
			if (c.Has_Item_Secondary)
			{
				TNH_CharacterDef.LoadoutEntry item_Secondary = c.Item_Secondary;
				FVRObject fVRObject5 = null;
				if (item_Secondary.ListOverride.Count > 0)
				{
					fVRObject5 = item_Secondary.ListOverride[Random.Range(0, item_Secondary.ListOverride.Count)];
				}
				else
				{
					ObjectTableDef d3 = item_Secondary.TableDefs[Random.Range(0, item_Secondary.TableDefs.Count)];
					ObjectTable objectTable5 = new ObjectTable();
					objectTable5.Initialize(d3);
					fVRObject5 = objectTable5.GetRandomObject();
				}
				GameObject g3 = Object.Instantiate(fVRObject5.GetGameObject(), SpawnPoints_SmallItem[1].position, SpawnPoints_SmallItem[1].rotation);
				M.AddObjectToTrackedList(g3);
			}
			if (c.Has_Item_Tertiary)
			{
				TNH_CharacterDef.LoadoutEntry item_Tertiary = c.Item_Tertiary;
				FVRObject fVRObject6 = null;
				if (item_Tertiary.ListOverride.Count > 0)
				{
					fVRObject6 = item_Tertiary.ListOverride[Random.Range(0, item_Tertiary.ListOverride.Count)];
				}
				else
				{
					ObjectTableDef d4 = item_Tertiary.TableDefs[Random.Range(0, item_Tertiary.TableDefs.Count)];
					ObjectTable objectTable6 = new ObjectTable();
					objectTable6.Initialize(d4);
					fVRObject6 = objectTable6.GetRandomObject();
				}
				GameObject g4 = Object.Instantiate(fVRObject6.GetGameObject(), SpawnPoints_SmallItem[2].position, SpawnPoints_SmallItem[2].rotation);
				M.AddObjectToTrackedList(g4);
			}
			if (c.Has_Item_Shield)
			{
				TNH_CharacterDef.LoadoutEntry item_Shield = c.Item_Shield;
				FVRObject fVRObject7 = null;
				if (item_Shield.ListOverride.Count > 0)
				{
					fVRObject7 = item_Shield.ListOverride[Random.Range(0, item_Shield.ListOverride.Count)];
				}
				else
				{
					ObjectTableDef d5 = item_Shield.TableDefs[Random.Range(0, item_Shield.TableDefs.Count)];
					ObjectTable objectTable7 = new ObjectTable();
					objectTable7.Initialize(d5);
					fVRObject7 = objectTable7.GetRandomObject();
				}
				GameObject g5 = Object.Instantiate(fVRObject7.GetGameObject(), SpawnPoint_Shield.position, SpawnPoint_Shield.rotation);
				M.AddObjectToTrackedList(g5);
			}
		}

		public void ClearConfiguration()
		{
			DeleteAllActiveEntities();
		}

		public void DeleteAllActiveEntities()
		{
			DeleteSosigs();
			DeleteTurrets();
			DeleteBoxes();
			if (m_trackedObjects.Count > 0)
			{
				for (int num = m_trackedObjects.Count - 1; num >= 0; num--)
				{
					if (m_trackedObjects[num] != null)
					{
						Object.Destroy(m_trackedObjects[num].gameObject);
					}
				}
			}
			if (m_constructor != null)
			{
				m_constructor.GetComponent<TNH_ObjectConstructor>().ClearCase();
				Object.Destroy(m_constructor);
				m_constructor = null;
			}
			if (m_panel != null)
			{
				Object.Destroy(m_panel);
				m_panel = null;
			}
		}

		private void DeleteSosigs()
		{
			for (int num = m_activeSosigs.Count - 1; num >= 0; num--)
			{
				if (m_activeSosigs[num] != null)
				{
					m_activeSosigs[num].DeSpawnSosig();
				}
			}
			m_activeSosigs.Clear();
		}

		private void DeleteTurrets()
		{
			for (int num = m_activeTurrets.Count - 1; num >= 0; num--)
			{
				if (m_activeTurrets[num] != null)
				{
					Object.Destroy(m_activeTurrets[num].gameObject);
				}
			}
			m_activeTurrets.Clear();
		}

		public bool IsPointInBounds(Vector3 p)
		{
			if (TestVolumeBool(Bounds, p))
			{
				return true;
			}
			return false;
		}

		private void DeleteBoxes()
		{
			for (int num = m_spawnBoxes.Count - 1; num >= 0; num--)
			{
				if (m_spawnBoxes != null)
				{
					Object.Destroy(m_spawnBoxes[num]);
				}
			}
			m_spawnBoxes.Clear();
		}

		public bool TestVolumeBool(Transform t, Vector3 pos)
		{
			bool result = true;
			Vector3 vector = t.InverseTransformPoint(pos);
			if (Mathf.Abs(vector.x) > 0.5f || Mathf.Abs(vector.y) > 0.5f || Mathf.Abs(vector.z) > 0.5f)
			{
				result = false;
			}
			return result;
		}

		private void OnDrawGizmos()
		{
			if (ShowPoints_SpawnTable)
			{
				Gizmos.color = new Color(1f, 0.2f, 0.2f);
				for (int i = 0; i < SpawnPoint_Tables.Count; i++)
				{
					Gizmos.DrawMesh(GizmoMesh_Table, SpawnPoint_Tables[i].position, SpawnPoint_Tables[i].rotation);
				}
				Gizmos.DrawMesh(GizmoMesh_CaseLarge, SpawnPoint_CaseLarge.position, SpawnPoint_CaseLarge.rotation);
				Gizmos.DrawMesh(GizmoMesh_CaseSmall, SpawnPoint_CaseSmall.position, SpawnPoint_CaseSmall.rotation);
				for (int j = 0; j < SpawnPoints_SmallItem.Count; j++)
				{
					Gizmos.DrawSphere(SpawnPoints_SmallItem[j].position, 0.1f);
				}
				Gizmos.DrawMesh(GizmoMesh_Melee, SpawnPoint_Melee.position, SpawnPoint_Melee.rotation);
				Gizmos.DrawCube(SpawnPoint_Shield.position, new Vector3(0.6f, 1f, 0.6f));
			}
			if (ShowPoint_PlayerSpawn)
			{
				Gizmos.color = new Color(1f, 0.2f, 0.2f);
				Gizmos.DrawMesh(GizmoMesh_SosigAttack, SpawnPoint_PlayerSpawn.position, SpawnPoint_PlayerSpawn.rotation);
			}
			if (ShowPoints_Sosigs_Defense)
			{
				for (int k = 0; k < SpawnPoints_Sosigs_Defense.Count; k++)
				{
					Gizmos.color = new Color(0f, 0.8f, 0.8f);
					Gizmos.DrawMesh(GizmoMesh_SosigAttack, SpawnPoints_Sosigs_Defense[k].position, SpawnPoints_Sosigs_Defense[k].rotation);
				}
			}
			if (ShowPoints_Turrets)
			{
				for (int l = 0; l < SpawnPoints_Turrets.Count; l++)
				{
					Gizmos.color = new Color(0f, 0.2f, 1f);
					Gizmos.DrawMesh(GizmoMesh_SosigAttack, SpawnPoints_Turrets[l].position, SpawnPoints_Turrets[l].rotation);
				}
			}
			if (ShowPoints_Screens)
			{
				for (int m = 0; m < SpawnPoints_Panels.Count; m++)
				{
					Gizmos.color = new Color(0f, 1f, 1f);
					Gizmos.DrawMesh(GizmoMesh_Panel, SpawnPoints_Panels[m].position, SpawnPoints_Panels[m].rotation);
				}
			}
			if (ShowPoints_Boxes)
			{
				for (int n = 0; n < SpawnPoints_Boxes.Count; n++)
				{
					Gizmos.color = new Color(1f, 1f, 0f);
					Gizmos.DrawMesh(GizmoMesh_Box, SpawnPoints_Boxes[n].position, SpawnPoints_Boxes[n].rotation);
				}
			}
		}
	}
}
