using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class TNH_ObjectConstructor : MonoBehaviour
	{
		public enum ConstructorState
		{
			Bootup,
			EntryList,
			Confirm
		}

		public TNH_Manager M;

		public List<TNH_ObjectConstructorIcon> Icons;

		private EquipmentPoolDef m_pool;

		private List<EquipmentPoolDef.PoolEntry> m_poolEntries = new List<EquipmentPoolDef.PoolEntry>();

		private List<int> m_poolAddedCost = new List<int>
		{
			0,
			0,
			0
		};

		public List<Image> TokenList;

		[Header("Spawn Points")]
		public List<Transform> SpawnPoints_GunsSize;

		public Transform SpawnPoint_Mag;

		public Transform SpawnPoint_Ammo;

		public Transform SpawnPoint_Grenade;

		public Transform SpawnPoint_Melee;

		public Transform SpawnPoint_Shield;

		public Transform SpawnPoint_Object;

		public Transform SpawnPoint_Case;

		private List<GameObject> m_trackedObjects = new List<GameObject>();

		private GameObject m_spawnedCase;

		private int m_selectedEntry = -1;

		private int m_numTokensSelected;

		public Color Token_Empty;

		public Color Token_Unselected;

		public Color Token_Selected;

		public AudioEvent AudEvent_Select;

		public AudioEvent AudEvent_Fail;

		public AudioEvent AudEvent_Back;

		public AudioEvent AudEvent_Spawn;

		private int m_rerollCost;

		public List<GameObject> RerollButtons;

		public List<GameObject> RerollButtonImages;

		private bool m_hasRequiredPicatinnyTable;

		private ObjectTableDef m_requiredPicatinnyTable;

		private ObjectTable m_requiredSightTable;

		private List<FVRObject.OTagEra> m_validEras = new List<FVRObject.OTagEra>();

		private List<FVRObject.OTagSet> m_validSets = new List<FVRObject.OTagSet>();

		public ConstructorState State;

		private int m_curLevel;

		private bool allowEntry = true;

		public void SetValidErasSets(List<FVRObject.OTagEra> eras, List<FVRObject.OTagSet> sets)
		{
			for (int i = 0; i < eras.Count; i++)
			{
				m_validEras.Add(eras[i]);
			}
			for (int j = 0; j < sets.Count; j++)
			{
				m_validSets.Add(sets[j]);
			}
		}

		private void Start()
		{
			for (int i = 0; i < Icons.Count; i++)
			{
				Icons[i].Init();
			}
		}

		public void ClearCase()
		{
			if (m_spawnedCase != null)
			{
				Object.Destroy(m_spawnedCase.gameObject);
				m_spawnedCase = null;
			}
		}

		public void SetRequiredPicatinnySightTable(ObjectTableDef def)
		{
			m_requiredPicatinnyTable = def;
			if (m_requiredPicatinnyTable != null)
			{
				m_hasRequiredPicatinnyTable = true;
				m_requiredSightTable = M.GetObjectTable(m_requiredPicatinnyTable);
			}
		}

		public void SetEntries(TNH_Manager m, int m_level, EquipmentPoolDef pool, EquipmentPoolDef.PoolEntry pool1, EquipmentPoolDef.PoolEntry pool2, EquipmentPoolDef.PoolEntry pool3)
		{
			M = m;
			m_curLevel = m_level;
			m_pool = pool;
			m_poolEntries.Clear();
			m_poolEntries.Add(pool1);
			m_poolEntries.Add(pool2);
			m_poolEntries.Add(pool3);
			SetState(ConstructorState.EntryList, 0);
			M.TokenCountChangeEvent += UpdateTokenDisplay;
			UpdateTokenDisplay(M.GetNumTokens());
		}

		public void ResetEntry(int m_level, EquipmentPoolDef pool, EquipmentPoolDef.PoolEntry.PoolEntryType pType, int index)
		{
			m_curLevel = m_level;
			m_poolEntries[index] = GetPoolEntry(m_level, pool, pType, m_poolEntries[index]);
			SetState(ConstructorState.EntryList, 0);
		}

		private void UpdateTokenDisplay(int numTokens)
		{
			int num = numTokens - 1;
			int num2 = num - m_numTokensSelected;
			for (int i = 0; i < TokenList.Count; i++)
			{
				if (i <= num)
				{
					if (i <= num2)
					{
						TokenList[i].color = Token_Unselected;
					}
					else
					{
						TokenList[i].color = Token_Selected;
					}
				}
				else
				{
					TokenList[i].color = Token_Empty;
				}
			}
		}

		private void OnDestroy()
		{
			if (M != null)
			{
				M.TokenCountChangeEvent -= UpdateTokenDisplay;
			}
		}

		private void UpdateRerollButtonState(bool disable)
		{
			if (disable)
			{
				for (int i = 0; i < RerollButtons.Count; i++)
				{
					RerollButtons[i].SetActive(value: false);
					RerollButtonImages[i].SetActive(value: false);
				}
				return;
			}
			if (m_pool.GetNumEntries(EquipmentPoolDef.PoolEntry.PoolEntryType.Firearm, m_curLevel) < 2)
			{
				RerollButtons[0].SetActive(value: false);
				RerollButtonImages[0].SetActive(value: false);
			}
			else
			{
				RerollButtons[0].SetActive(value: true);
				RerollButtonImages[0].SetActive(value: true);
			}
			if (m_pool.GetNumEntries(EquipmentPoolDef.PoolEntry.PoolEntryType.Equipment, m_curLevel) < 2)
			{
				RerollButtons[1].SetActive(value: false);
				RerollButtonImages[1].SetActive(value: false);
			}
			else
			{
				RerollButtons[1].SetActive(value: true);
				RerollButtonImages[1].SetActive(value: true);
			}
			if (m_pool.GetNumEntries(EquipmentPoolDef.PoolEntry.PoolEntryType.Consumable, m_curLevel) < 2)
			{
				RerollButtons[2].SetActive(value: false);
				RerollButtonImages[2].SetActive(value: false);
			}
			else
			{
				RerollButtons[2].SetActive(value: true);
				RerollButtonImages[2].SetActive(value: true);
			}
		}

		public void ButtonClicked(int i)
		{
			UpdateRerollButtonState(disable: false);
			if (!allowEntry)
			{
				return;
			}
			if (State == ConstructorState.EntryList)
			{
				int num = m_poolEntries[i].GetCost(M.EquipmentMode) + m_poolAddedCost[i];
				int numTokens = M.GetNumTokens();
				if (numTokens >= num)
				{
					SetState(ConstructorState.Confirm, i);
					SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_Select, base.transform.position);
				}
				else
				{
					SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_Fail, base.transform.position);
				}
			}
			else
			{
				if (State != ConstructorState.Confirm)
				{
					return;
				}
				switch (i)
				{
				case 0:
					SetState(ConstructorState.EntryList, 0);
					m_selectedEntry = -1;
					SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_Back, base.transform.position);
					break;
				case 2:
				{
					int num2 = m_poolEntries[m_selectedEntry].GetCost(M.EquipmentMode) + m_poolAddedCost[m_selectedEntry];
					int numTokens2 = M.GetNumTokens();
					if (numTokens2 >= num2)
					{
						if (CanSpawnObject(m_poolEntries[m_selectedEntry]))
						{
							AnvilManager.Run(SpawnObject(m_poolEntries[m_selectedEntry]));
							m_numTokensSelected = 0;
							M.SubtractTokens(num2);
							SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_Spawn, base.transform.position);
							if (M.C.UsesPurchasePriceIncrement)
							{
								m_poolAddedCost[m_selectedEntry]++;
							}
							SetState(ConstructorState.EntryList, 0);
							m_selectedEntry = -1;
						}
						else
						{
							SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_Fail, base.transform.position);
						}
					}
					else
					{
						SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_Fail, base.transform.position);
					}
					break;
				}
				}
			}
		}

		public void ButtonClicked_Reroll(int which)
		{
			int num = 1;
			int numTokens = M.GetNumTokens();
			if (numTokens >= num)
			{
				SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_Select, base.transform.position);
				M.RegenerateConstructor(this, which);
				M.SubtractTokens(num);
				UpdateTokenDisplay(M.GetNumTokens());
				m_poolAddedCost[which] = 0;
			}
			else
			{
				SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_Fail, base.transform.position);
			}
		}

		public EquipmentPoolDef.PoolEntry GetPoolEntry(int level, EquipmentPoolDef poolDef, EquipmentPoolDef.PoolEntry.PoolEntryType t, EquipmentPoolDef.PoolEntry prior = null)
		{
			float num = 0f;
			for (int i = 0; i < poolDef.Entries.Count; i++)
			{
				if (poolDef.Entries[i].Type == t && poolDef.Entries[i].MinLevelAppears <= level && poolDef.Entries[i].MaxLevelAppears >= level && (poolDef.Entries.Count <= 1 || poolDef.Entries[i] != prior))
				{
					num += poolDef.Entries[i].Rarity;
				}
			}
			float num2 = Random.Range(0f, num);
			float num3 = 0f;
			EquipmentPoolDef.PoolEntry result = poolDef.Entries[0];
			for (int j = 0; j < poolDef.Entries.Count; j++)
			{
				if (poolDef.Entries[j].Type == t && poolDef.Entries[j].MinLevelAppears <= level && poolDef.Entries[j].MaxLevelAppears >= level && (poolDef.Entries.Count <= 1 || poolDef.Entries[j] != prior))
				{
					num3 += poolDef.Entries[j].Rarity;
					if (num3 >= num2)
					{
						result = poolDef.Entries[j];
						break;
					}
				}
			}
			return result;
		}

		private void SetState(ConstructorState s, int whichItem)
		{
			State = s;
			switch (State)
			{
			case ConstructorState.EntryList:
				UpdateRerollButtonState(disable: false);
				m_numTokensSelected = 0;
				m_selectedEntry = -1;
				Icons[0].SetOption(TNH_ObjectConstructorIcon.IconState.Item, m_poolEntries[0].TableDef.Icon, m_poolEntries[0].GetCost(M.EquipmentMode) + m_poolAddedCost[0]);
				Icons[1].SetOption(TNH_ObjectConstructorIcon.IconState.Item, m_poolEntries[1].TableDef.Icon, m_poolEntries[1].GetCost(M.EquipmentMode) + m_poolAddedCost[1]);
				Icons[2].SetOption(TNH_ObjectConstructorIcon.IconState.Item, m_poolEntries[2].TableDef.Icon, m_poolEntries[2].GetCost(M.EquipmentMode) + m_poolAddedCost[2]);
				break;
			case ConstructorState.Confirm:
				UpdateRerollButtonState(disable: true);
				m_selectedEntry = whichItem;
				m_numTokensSelected = m_poolEntries[m_selectedEntry].GetCost(M.EquipmentMode) + m_poolAddedCost[m_selectedEntry];
				Icons[0].SetOption(TNH_ObjectConstructorIcon.IconState.Cancel, null, 0);
				Icons[1].SetOption(TNH_ObjectConstructorIcon.IconState.Item, m_poolEntries[m_selectedEntry].TableDef.Icon, m_poolEntries[m_selectedEntry].GetCost(M.EquipmentMode) + m_poolAddedCost[m_selectedEntry]);
				Icons[2].SetOption(TNH_ObjectConstructorIcon.IconState.Accept, null, 0);
				break;
			}
			UpdateTokenDisplay(M.GetNumTokens());
		}

		private Transform GetSpawnBasedOnSize(FVRObject.OTagFirearmSize s)
		{
			return SpawnPoints_GunsSize[(int)(s - 1)];
		}

		private bool IsEntryCase(EquipmentPoolDef.PoolEntry entry)
		{
			if (entry.TableDef.SpawnsInSmallCase || entry.TableDef.SpawnsInLargeCase)
			{
				return true;
			}
			return false;
		}

		private bool CanSpawnObject(EquipmentPoolDef.PoolEntry entry)
		{
			ObjectTable objectTable = M.GetObjectTable(entry.TableDef);
			if ((entry.TableDef.SpawnsInSmallCase || entry.TableDef.SpawnsInLargeCase) && m_spawnedCase != null)
			{
				return false;
			}
			return true;
		}

		private IEnumerator SpawnObject(EquipmentPoolDef.PoolEntry entry)
		{
			allowEntry = false;
			ObjectTable table = M.GetObjectTable(entry.TableDef);
			if (entry.TableDef.SpawnsInSmallCase || entry.TableDef.SpawnsInLargeCase)
			{
				GameObject caseFab = M.Prefab_WeaponCaseLarge;
				if (entry.TableDef.SpawnsInSmallCase)
				{
					caseFab = M.Prefab_WeaponCaseSmall;
				}
				FVRObject randomObject = table.GetRandomObject();
				(m_spawnedCase = M.SpawnWeaponCase(caseFab, SpawnPoint_Case.position, SpawnPoint_Case.forward, randomObject, 3, 8, entry.TableDef.MinAmmoCapacity, entry.TableDef.MaxAmmoCapacity)).GetComponent<TNH_WeaponCrate>().M = M;
			}
			else
			{
				FVRObject obj = table.GetRandomObject();
				if (obj.Category == FVRObject.ObjectCategory.Firearm)
				{
					FVRObject ammoObject2 = obj.GetRandomAmmoObject(obj, m_validEras, table.MinCapacity, table.MaxCapacity, m_validSets);
					int numAmmo3 = 0;
					numAmmo3 = ((!(ammoObject2 != null) || ammoObject2.Category != FVRObject.ObjectCategory.Cartridge) ? 3 : 8);
					Transform sp_gun = GetSpawnBasedOnSize(obj.TagFirearmSize);
					yield return obj.GetGameObjectAsync();
					GameObject Ggun = Object.Instantiate(obj.GetGameObject(), sp_gun.position, sp_gun.rotation);
					m_trackedObjects.Add(Ggun);
					if (ammoObject2 != null)
					{
						Transform sp_ammo4 = null;
						sp_ammo4 = ((ammoObject2.Category != FVRObject.ObjectCategory.Cartridge) ? SpawnPoint_Mag : SpawnPoint_Ammo);
						Vector3 point2 = sp_ammo4.position;
						yield return ammoObject2.GetGameObjectAsync();
						for (int l = 0; l < numAmmo3; l++)
						{
							switch (ammoObject2.Category)
							{
							case FVRObject.ObjectCategory.Cartridge:
							{
								GameObject item4 = Object.Instantiate(ammoObject2.GetGameObject(), point2, sp_ammo4.rotation);
								m_trackedObjects.Add(item4);
								break;
							}
							case FVRObject.ObjectCategory.Clip:
							{
								GameObject item3 = Object.Instantiate(ammoObject2.GetGameObject(), point2, sp_ammo4.rotation);
								m_trackedObjects.Add(item3);
								break;
							}
							case FVRObject.ObjectCategory.Magazine:
							{
								GameObject item2 = Object.Instantiate(ammoObject2.GetGameObject(), point2, sp_ammo4.rotation);
								m_trackedObjects.Add(item2);
								break;
							}
							case FVRObject.ObjectCategory.SpeedLoader:
							{
								GameObject item = Object.Instantiate(ammoObject2.GetGameObject(), point2, sp_ammo4.rotation);
								m_trackedObjects.Add(item);
								break;
							}
							}
							point2 += Vector3.up * 0.15f;
						}
					}
					if (obj.RequiredSecondaryPieces.Count > 0)
					{
						for (int k = 0; k < obj.RequiredSecondaryPieces.Count; k++)
						{
							yield return obj.RequiredSecondaryPieces[k].GetGameObjectAsync();
							GameObject objGO10 = Object.Instantiate(obj.RequiredSecondaryPieces[k].GetGameObject(), SpawnPoint_Grenade.position + Vector3.up * 0.2f * (k + 1), SpawnPoint_Object.rotation);
							m_trackedObjects.Add(objGO10);
						}
					}
					if (obj.RequiresPicatinnySight && m_hasRequiredPicatinnyTable)
					{
						FVRObject reqSight1 = m_requiredSightTable.GetRandomObject();
						yield return reqSight1.GetGameObjectAsync();
						GameObject objGO7 = Object.Instantiate(reqSight1.GetGameObject(), SpawnPoint_Object.position, SpawnPoint_Object.rotation);
						m_trackedObjects.Add(objGO7);
						if (reqSight1.RequiredSecondaryPieces.Count > 0)
						{
							for (int j = 0; j < reqSight1.RequiredSecondaryPieces.Count; j++)
							{
								yield return reqSight1.RequiredSecondaryPieces[j].GetGameObjectAsync();
								GameObject objGO9 = Object.Instantiate(reqSight1.RequiredSecondaryPieces[j].GetGameObject(), SpawnPoint_Object.position + Vector3.up * 0.2f * (j + 1), SpawnPoint_Object.rotation);
								m_trackedObjects.Add(objGO9);
							}
						}
					}
					else if (obj.BespokeAttachments.Count > 0 && Random.Range(0f, 1f) > 0.5f)
					{
						FVRObject BespokeObj = obj.BespokeAttachments[Random.Range(0, obj.BespokeAttachments.Count)];
						yield return BespokeObj.GetGameObjectAsync();
						GameObject objGO6 = Object.Instantiate(BespokeObj.GetGameObject(), SpawnPoint_Object.position, SpawnPoint_Object.rotation);
						m_trackedObjects.Add(objGO6);
					}
				}
				else if (obj.Category == FVRObject.ObjectCategory.Explosive || obj.Category == FVRObject.ObjectCategory.Thrown)
				{
					yield return obj.GetGameObjectAsync();
					GameObject objGO = Object.Instantiate(obj.GetGameObject(), SpawnPoint_Grenade.position, SpawnPoint_Grenade.rotation);
					m_trackedObjects.Add(objGO);
					if (obj.RequiredSecondaryPieces.Count > 0)
					{
						GameObject item5 = Object.Instantiate(obj.RequiredSecondaryPieces[0].GetGameObject(), SpawnPoint_Object.position, SpawnPoint_Object.rotation);
						m_trackedObjects.Add(item5);
					}
				}
				else if (obj.Category == FVRObject.ObjectCategory.MeleeWeapon)
				{
					yield return obj.GetGameObjectAsync();
					GameObject objGO5 = Object.Instantiate(obj.GetGameObject(), SpawnPoint_Melee.position, SpawnPoint_Melee.rotation);
					m_trackedObjects.Add(objGO5);
				}
				else if (obj.Category == FVRObject.ObjectCategory.MeleeWeapon && obj.TagMeleeStyle == FVRObject.OTagMeleeStyle.Shield)
				{
					yield return obj.GetGameObjectAsync();
					GameObject objGO4 = Object.Instantiate(obj.GetGameObject(), SpawnPoint_Shield.position, SpawnPoint_Shield.rotation);
					m_trackedObjects.Add(objGO4);
				}
				else if (obj.Category == FVRObject.ObjectCategory.Attachment)
				{
					yield return obj.GetGameObjectAsync();
					GameObject objGO3 = Object.Instantiate(obj.GetGameObject(), SpawnPoint_Object.position, SpawnPoint_Object.rotation);
					m_trackedObjects.Add(objGO3);
					if (obj.RequiredSecondaryPieces.Count > 0)
					{
						for (int i = 0; i < obj.RequiredSecondaryPieces.Count; i++)
						{
							yield return obj.RequiredSecondaryPieces[i].GetGameObjectAsync();
							GameObject objGO8 = Object.Instantiate(obj.RequiredSecondaryPieces[i].GetGameObject(), SpawnPoint_Object.position + Vector3.up * 0.2f * (i + 1), SpawnPoint_Object.rotation);
							m_trackedObjects.Add(objGO8);
						}
					}
					if (obj.TagAttachmentFeature == FVRObject.OTagAttachmentFeature.ProjectileWeapon)
					{
						FVRObject ammoObject = obj.GetRandomAmmoObject(obj);
						int numAmmo = 3;
						if (ammoObject != null)
						{
							Transform sp_ammo2 = null;
							sp_ammo2 = ((ammoObject.Category != FVRObject.ObjectCategory.Cartridge) ? SpawnPoint_Mag : SpawnPoint_Ammo);
							Vector3 point = sp_ammo2.position;
							yield return ammoObject.GetGameObjectAsync();
							for (int m = 0; m < numAmmo; m++)
							{
								FVRObject.ObjectCategory category = ammoObject.Category;
								if (category == FVRObject.ObjectCategory.Cartridge)
								{
									GameObject item6 = Object.Instantiate(ammoObject.GetGameObject(), point, sp_ammo2.rotation);
									m_trackedObjects.Add(item6);
								}
								point += Vector3.up * 0.15f;
							}
						}
					}
				}
				else
				{
					yield return obj.GetGameObjectAsync();
					GameObject objGO2 = Object.Instantiate(obj.GetGameObject(), SpawnPoint_Object.position, SpawnPoint_Object.rotation);
					m_trackedObjects.Add(objGO2);
				}
			}
			allowEntry = true;
		}
	}
}
