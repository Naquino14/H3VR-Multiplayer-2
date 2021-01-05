using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class ItemSpawnerUI : MonoBehaviour
	{
		public enum ItemSpawnerPageMode
		{
			Home,
			Category,
			SubCategory,
			Details,
			Vault
		}

		public FirearmSaver FirearmSaver;

		public Text T_TopBar;

		public Text T_PageIndicator;

		[Space(10f)]
		public GameObject B_PageIndicator;

		public GameObject B_Back;

		public GameObject B_Spawn;

		public GameObject B_Next;

		public GameObject B_Prev;

		public GameObject B_Vault;

		public GameObject B_Scan;

		[Space(10f)]
		public GameObject P_Tiles;

		public GameObject P_Details;

		public GameObject P_Vault;

		public ItemSpawnerUITile[] Tiles_SelectionPage;

		public ItemSpawnerUITile[] Tiles_Details;

		public ItemSpawnerVaultTile[] Tiles_Vault;

		public Text T_Details_Title;

		public Text T_Details_Subheading;

		public Text T_Details_Info;

		public Text T_Page_Indicator;

		public Text T_Currency_Readout;

		private ItemSpawnerPageMode m_curMode;

		private int m_curPage;

		private int m_maxPage;

		private int m_curVaultPage;

		private int m_maxVaultPage;

		private ItemSpawnerID.EItemCategory m_curCategory;

		private ItemSpawnerID.ESubCategory m_curSubCategory;

		private ItemSpawnerID m_curID;

		private ItemSpawnerID m_IDSelectedForSpawn;

		private Dictionary<ItemSpawnerID.ESubCategory, int> SavedPageIndexes = new Dictionary<ItemSpawnerID.ESubCategory, int>();

		public AudioSource AUD;

		public AudioClip[] AudClip_Chirps;

		private float refireTick;

		public Transform SpawnPos_Main;

		public Transform[] SpawnPos_Small;

		public Transform SpawnPos_Huge;

		private int m_curSmall;

		public Renderer ControlPoster;

		[Header("Vault Specific")]
		public List<GameObject> SubCategoryButtons;

		public List<ItemSpawnerID.ESubCategory> SubCategoriesByButton;

		private string m_staticVaultVersionToken = "DONTREMOVETHISPARTOFFILENAMEV02a";

		private List<string> m_vaultFileNames = new List<string>();

		private List<SavedGun> m_vaultSavedGunList = new List<SavedGun>();

		private HashSet<ItemSpawnerID.ESubCategory> m_vaultForWhichSubCatExists = new HashSet<ItemSpawnerID.ESubCategory>();

		private ItemSpawnerID.ESubCategory m_currentVaultFilter;

		private List<SavedGun> m_vaultCulledList = new List<SavedGun>();

		private bool m_isVaultGunAssembling;

		private int safeIncrement;

		private void Start()
		{
			SetMode(ItemSpawnerPageMode.Home);
			ControlPoster.gameObject.SetActive(value: false);
		}

		private void Update()
		{
			if (refireTick > 0f)
			{
				refireTick -= Time.deltaTime;
			}
			UpdateCurrencyDisplay();
		}

		public void PurchaseItem()
		{
			int num = 0;
			bool flag = !GM.CurrentSceneSettings.UsesUnlockSystem;
			if (m_curMode == ItemSpawnerPageMode.Details && !flag && !GM.Omni.OmniUnlocks.IsEquipmentUnlocked(Tiles_Details[num].Item, flag))
			{
				int unlockCost = Tiles_Details[num].Item.UnlockCost;
				if (GM.Omni.OmniUnlocks.HasCurrencyForPurchase(unlockCost))
				{
					ButtonPress(4);
					GM.Omni.OmniUnlocks.SpendCurrency(unlockCost);
					GM.Omni.OmniUnlocks.UnlockEquipment(Tiles_Details[num].Item, flag);
					GM.Omni.SaveUnlocksToFile();
					UpdateDetailSelectedItem();
				}
				else
				{
					ButtonPress(3);
				}
			}
		}

		public void ButtonPress(int soundIndex)
		{
			AUD.PlayOneShot(AudClip_Chirps[soundIndex], 0.3f);
			refireTick = 0.2f;
		}

		public void ButtonPress_GoToVault()
		{
			if (!(refireTick > 0f))
			{
				ButtonPress(0);
				SetMode_Vault();
			}
		}

		private void UpdateSavedPage(ItemSpawnerID.ESubCategory cat, int index)
		{
			if (SavedPageIndexes.ContainsKey(cat))
			{
				SavedPageIndexes[cat] = index;
			}
			else
			{
				SavedPageIndexes.Add(cat, index);
			}
		}

		private int GetSavedPageIndex(ItemSpawnerID.ESubCategory cat)
		{
			if (SavedPageIndexes.ContainsKey(cat))
			{
				return SavedPageIndexes[cat];
			}
			return 0;
		}

		public void ButtonPress_Next()
		{
			if (refireTick > 0f)
			{
				return;
			}
			ButtonPress(0);
			switch (m_curMode)
			{
			case ItemSpawnerPageMode.SubCategory:
				m_curPage++;
				if (m_curPage < m_maxPage)
				{
					B_Next.SetActive(value: true);
				}
				else
				{
					B_Next.SetActive(value: false);
				}
				if (m_curPage > 0)
				{
					B_Prev.SetActive(value: true);
				}
				else
				{
					B_Prev.SetActive(value: false);
				}
				UpdateSavedPage(m_curSubCategory, m_curPage);
				UpdatePageIndicator(m_curPage, m_maxPage);
				Draw_Tiles_SubCategory(m_curSubCategory);
				break;
			case ItemSpawnerPageMode.Vault:
				m_curVaultPage++;
				if (m_curVaultPage < m_maxVaultPage)
				{
					B_Next.SetActive(value: true);
				}
				else
				{
					B_Next.SetActive(value: false);
				}
				if (m_curVaultPage > 0)
				{
					B_Prev.SetActive(value: true);
				}
				else
				{
					B_Prev.SetActive(value: false);
				}
				UpdatePageIndicator(m_curVaultPage, m_maxVaultPage);
				Draw_Tiles_Vault(m_curVaultPage);
				break;
			}
		}

		public void ButtonPress_Prev()
		{
			if (refireTick > 0f)
			{
				return;
			}
			ButtonPress(1);
			switch (m_curMode)
			{
			case ItemSpawnerPageMode.SubCategory:
				m_curPage--;
				if (m_curPage < m_maxPage)
				{
					B_Next.SetActive(value: true);
				}
				else
				{
					B_Next.SetActive(value: false);
				}
				if (m_curPage > 0)
				{
					B_Prev.SetActive(value: true);
				}
				else
				{
					B_Prev.SetActive(value: false);
				}
				UpdateSavedPage(m_curSubCategory, m_curPage);
				UpdatePageIndicator(m_curPage, m_maxPage);
				Draw_Tiles_SubCategory(m_curSubCategory);
				break;
			case ItemSpawnerPageMode.Vault:
				m_curVaultPage--;
				if (m_curVaultPage < m_maxVaultPage)
				{
					B_Next.SetActive(value: true);
				}
				else
				{
					B_Next.SetActive(value: false);
				}
				if (m_curVaultPage > 0)
				{
					B_Prev.SetActive(value: true);
				}
				else
				{
					B_Prev.SetActive(value: false);
				}
				UpdatePageIndicator(m_curVaultPage, m_maxVaultPage);
				Draw_Tiles_Vault(m_curVaultPage);
				break;
			}
		}

		public void ButtonPress_Back()
		{
			if (!(refireTick > 0f))
			{
				ButtonPress(1);
				switch (m_curMode)
				{
				case ItemSpawnerPageMode.Category:
					SetMode_Home();
					break;
				case ItemSpawnerPageMode.SubCategory:
					SetMode_Category();
					break;
				case ItemSpawnerPageMode.Details:
					SetMode_SubCategory();
					break;
				case ItemSpawnerPageMode.Vault:
					SetMode_Home();
					break;
				}
			}
		}

		public void ButtonPress_SelectionTile(int i)
		{
			if (!(refireTick > 0f))
			{
				ButtonPress(0);
				switch (m_curMode)
				{
				case ItemSpawnerPageMode.Home:
					m_curCategory = Tiles_SelectionPage[i].Category;
					SetMode_Category();
					break;
				case ItemSpawnerPageMode.Category:
					m_curSubCategory = Tiles_SelectionPage[i].SubCategory;
					SetMode_SubCategory();
					break;
				case ItemSpawnerPageMode.SubCategory:
					m_curID = Tiles_SelectionPage[i].Item;
					m_IDSelectedForSpawn = Tiles_SelectionPage[i].Item;
					SetMode_Details();
					break;
				case ItemSpawnerPageMode.Details:
					break;
				}
			}
		}

		public void ButtonPress_DetailTile(int i)
		{
			if (!(refireTick > 0f))
			{
				ButtonPress(0);
				Sprite sprite = Tiles_Details[0].Image.sprite;
				string text = Tiles_Details[0].Text.text;
				ItemSpawnerID.EItemCategory category = Tiles_Details[0].Category;
				ItemSpawnerID.ESubCategory subCategory = Tiles_Details[0].SubCategory;
				ItemSpawnerID item = Tiles_Details[0].Item;
				Tiles_Details[0].Image.sprite = Tiles_Details[i].Image.sprite;
				Tiles_Details[0].Text.text = Tiles_Details[i].Text.text;
				Tiles_Details[0].Category = Tiles_Details[i].Category;
				Tiles_Details[0].SubCategory = Tiles_Details[i].SubCategory;
				Tiles_Details[0].Item = Tiles_Details[i].Item;
				Tiles_Details[i].Image.sprite = sprite;
				Tiles_Details[i].Text.text = text;
				Tiles_Details[i].Category = category;
				Tiles_Details[i].SubCategory = subCategory;
				Tiles_Details[i].Item = item;
				m_IDSelectedForSpawn = Tiles_Details[0].Item;
				T_Details_Title.text = Tiles_Details[0].Item.DisplayName;
				T_Details_Subheading.text = Tiles_Details[0].Item.SubHeading;
				T_Details_Info.text = Tiles_Details[0].Item.Description;
				bool sandboxMode = !GM.CurrentSceneSettings.UsesUnlockSystem;
				if (GM.Omni.OmniUnlocks.IsEquipmentUnlocked(Tiles_Details[0].Item, sandboxMode))
				{
					Tiles_Details[0].IsSpawnable = true;
					Tiles_Details[0].LockedCorner.gameObject.SetActive(value: false);
				}
				else
				{
					Tiles_Details[0].IsSpawnable = false;
					Tiles_Details[0].LockedCorner.gameObject.SetActive(value: true);
					Tiles_Details[0].LockedText.text = "Buy [" + Tiles_Details[0].Item.UnlockCost + " S.A.U.C.E.]";
				}
				if (GM.Omni.OmniUnlocks.IsEquipmentUnlocked(Tiles_Details[i].Item, sandboxMode))
				{
					Tiles_Details[i].IsSpawnable = true;
					Tiles_Details[i].LockedCorner.gameObject.SetActive(value: false);
				}
				else
				{
					Tiles_Details[i].IsSpawnable = false;
					Tiles_Details[i].LockedCorner.gameObject.SetActive(value: true);
				}
				B_Spawn.SetActive(Tiles_Details[0].IsSpawnable);
			}
		}

		public void ButtonPress_Spawn()
		{
			if (!(refireTick > 0f))
			{
				AnvilManager.Run(SpawnItems());
			}
		}

		private void SpawnItemz()
		{
			ButtonPress(2);
			GameObject gameObject = null;
			GameObject gameObject2 = null;
			ItemSpawnerID iDSelectedForSpawn = m_IDSelectedForSpawn;
			if (iDSelectedForSpawn == null)
			{
				return;
			}
			gameObject = (iDSelectedForSpawn.UsesLargeSpawnPad ? UnityEngine.Object.Instantiate(iDSelectedForSpawn.MainObject.GetGameObject(), SpawnPos_Main.position + Vector3.up * 0.2f, SpawnPos_Main.rotation) : ((!iDSelectedForSpawn.UsesHugeSpawnPad) ? UnityEngine.Object.Instantiate(iDSelectedForSpawn.MainObject.GetGameObject(), SpawnPos_Small[m_curSmall].position, SpawnPos_Small[m_curSmall].rotation) : UnityEngine.Object.Instantiate(iDSelectedForSpawn.MainObject.GetGameObject(), SpawnPos_Huge.position + Vector3.up * 0.2f, SpawnPos_Huge.rotation)));
			m_curSmall++;
			if (m_curSmall >= SpawnPos_Small.Length)
			{
				m_curSmall = 0;
			}
			if (iDSelectedForSpawn.SecondObject != null && iDSelectedForSpawn.SecondObject.GetGameObject() != null)
			{
				gameObject2 = UnityEngine.Object.Instantiate(iDSelectedForSpawn.SecondObject.GetGameObject(), SpawnPos_Small[m_curSmall].position, SpawnPos_Small[m_curSmall].rotation);
				m_curSmall++;
				if (m_curSmall >= SpawnPos_Small.Length)
				{
					m_curSmall = 0;
				}
			}
			if (gameObject != null)
			{
				gameObject.GetComponent<FVRPhysicalObject>().IDSpawnedFrom = iDSelectedForSpawn;
			}
			if (gameObject2 != null)
			{
				gameObject2.GetComponent<FVRPhysicalObject>().IDSpawnedFrom = iDSelectedForSpawn;
			}
		}

		private IEnumerator SpawnItems()
		{
			ButtonPress(2);
			GameObject tempGO2 = null;
			GameObject tempGO3 = null;
			ItemSpawnerID tempID = m_IDSelectedForSpawn;
			if (tempID == null)
			{
				yield break;
			}
			yield return tempID.MainObject.GetGameObjectAsync();
			tempGO2 = ((!tempID.UsesLargeSpawnPad) ? UnityEngine.Object.Instantiate(tempID.MainObject.GetGameObject(), SpawnPos_Small[m_curSmall].position, SpawnPos_Small[m_curSmall].rotation) : UnityEngine.Object.Instantiate(tempID.MainObject.GetGameObject(), SpawnPos_Main.position + Vector3.up * 0.2f, SpawnPos_Main.rotation));
			tempGO2.SetActive(value: true);
			m_curSmall++;
			if (m_curSmall >= SpawnPos_Small.Length)
			{
				m_curSmall = 0;
			}
			if (tempID.SecondObject != null)
			{
				yield return tempID.SecondObject.GetGameObjectAsync();
				if (tempID.SecondObject.GetGameObject() != null)
				{
					tempGO3 = UnityEngine.Object.Instantiate(tempID.SecondObject.GetGameObject(), SpawnPos_Small[m_curSmall].position, SpawnPos_Small[m_curSmall].rotation);
					m_curSmall++;
					if (m_curSmall >= SpawnPos_Small.Length)
					{
						m_curSmall = 0;
					}
					tempGO3.SetActive(value: true);
				}
			}
			if (tempGO2 != null)
			{
				tempGO2.GetComponent<FVRPhysicalObject>().IDSpawnedFrom = tempID;
			}
			if (tempGO3 != null)
			{
				tempGO3.GetComponent<FVRPhysicalObject>().IDSpawnedFrom = tempID;
			}
		}

		public void ButtonPress_Scan()
		{
			if (FirearmSaver.TryToScanGun())
			{
				ButtonPress(6);
				refireTick = 1f;
				ReCalcVaultPage();
				UpdatePageIndicator(m_curVaultPage, m_maxVaultPage);
				Draw_Tiles_Vault(m_curVaultPage);
			}
			else
			{
				ButtonPress(5);
				refireTick = 1f;
			}
		}

		public void ButtonPress_SpawnVaultGun(int i)
		{
			if (!(refireTick > 0f))
			{
				if (Tiles_Vault[i].LockedCorner.gameObject.activeSelf)
				{
					ButtonPress(5);
					refireTick = 1f;
					return;
				}
				ButtonPress(2);
				ReCalcVaultPage();
				m_isVaultGunAssembling = true;
				FirearmSaver.SpawnFirearm(Tiles_Vault[i].SavedGun);
				refireTick = 1f;
			}
		}

		public void VaultGunAssemblyFinished()
		{
			m_isVaultGunAssembling = false;
		}

		public void ButtonPress_SetVaultFilter(int i)
		{
			ButtonPress(0);
			if (i == -1)
			{
				m_currentVaultFilter = ItemSpawnerID.ESubCategory.None;
			}
			else
			{
				m_currentVaultFilter = SubCategoriesByButton[i];
			}
			ReCalcVaultPage();
			UpdatePageIndicator(m_curVaultPage, m_maxVaultPage);
			Draw_Tiles_Vault(m_curVaultPage);
		}

		public void ButtonPress_DeleteVaultGun(int i)
		{
			if (!(refireTick > 0f))
			{
				ButtonPress(2);
				ReCalcVaultPage();
				UpdatePageIndicator(m_curVaultPage, m_maxVaultPage);
				Tiles_Vault[i].DeleteButton.SetActive(value: false);
				Tiles_Vault[i].ConfirmButton.SetActive(value: true);
				Draw_Tiles_Vault(m_curVaultPage);
			}
		}

		public void ButtonPress_ConfirmDeleteVaultGun(int i)
		{
			if (!(refireTick > 0f))
			{
				ButtonPress(2);
				RemoveVaultGunAtIndex(i);
				ReCalcVaultPage();
				UpdatePageIndicator(m_curVaultPage, m_maxVaultPage);
				Draw_Tiles_Vault(m_curVaultPage);
			}
		}

		private void ReCalcVaultPage()
		{
			m_vaultCulledList.Clear();
			for (int i = 0; i < m_vaultSavedGunList.Count; i++)
			{
				if (m_currentVaultFilter == ItemSpawnerID.ESubCategory.None)
				{
					m_vaultCulledList.Add(m_vaultSavedGunList[i]);
					continue;
				}
				FVRObject fVRObject = IM.OD[m_vaultSavedGunList[i].Components[0].ObjectID];
				if (IM.HasSpawnedID(fVRObject.SpawnedFromId))
				{
					ItemSpawnerID spawnerID = IM.GetSpawnerID(fVRObject.SpawnedFromId);
					if (spawnerID.SubCategory == m_currentVaultFilter)
					{
						m_vaultCulledList.Add(m_vaultSavedGunList[i]);
					}
				}
				else
				{
					Debug.Log("Oh dear, looks like we can't find ItemSpawnerID for: " + fVRObject.ItemID);
				}
			}
			m_maxVaultPage = Mathf.FloorToInt((m_vaultCulledList.Count - 1) / 5);
			if (m_curVaultPage > m_maxVaultPage)
			{
				m_curVaultPage = m_maxVaultPage;
			}
			if (m_curVaultPage < m_maxVaultPage)
			{
				B_Next.SetActive(value: true);
			}
			else
			{
				B_Next.SetActive(value: false);
			}
			if (m_curVaultPage > 0)
			{
				B_Prev.SetActive(value: true);
			}
			else
			{
				B_Prev.SetActive(value: false);
			}
			for (int j = 0; j < Tiles_Vault.Length; j++)
			{
				Tiles_Vault[j].DeleteButton.SetActive(value: true);
				Tiles_Vault[j].ConfirmButton.SetActive(value: false);
			}
			if (m_maxVaultPage > 0)
			{
				B_PageIndicator.SetActive(value: true);
			}
			else
			{
				B_PageIndicator.SetActive(value: false);
			}
		}

		private void UpdatePageIndicator(int a, int b)
		{
			T_Page_Indicator.text = a + 1 + "/" + (b + 1);
		}

		private void UpdateCurrencyDisplay()
		{
			if (GM.CurrentSceneSettings.UsesUnlockSystem)
			{
				if (!T_Currency_Readout.gameObject.activeSelf)
				{
					T_Currency_Readout.gameObject.SetActive(value: true);
				}
				T_Currency_Readout.text = "S.A.U.C.E. - " + GM.Omni.OmniUnlocks.SaucePackets;
			}
			else if (T_Currency_Readout.gameObject.activeSelf)
			{
				T_Currency_Readout.gameObject.SetActive(value: false);
			}
		}

		private void SetMode(ItemSpawnerPageMode mode)
		{
			UpdateCurrencyDisplay();
			switch (mode)
			{
			case ItemSpawnerPageMode.Home:
				SetMode_Home();
				break;
			case ItemSpawnerPageMode.Category:
				SetMode_Category();
				break;
			case ItemSpawnerPageMode.SubCategory:
				SetMode_SubCategory();
				break;
			case ItemSpawnerPageMode.Details:
				SetMode_Details();
				break;
			}
		}

		private void SetMode_Home()
		{
			m_curMode = ItemSpawnerPageMode.Home;
			P_Tiles.SetActive(value: true);
			P_Details.SetActive(value: false);
			P_Vault.SetActive(value: false);
			B_PageIndicator.SetActive(value: false);
			B_Back.SetActive(value: false);
			B_Spawn.SetActive(value: false);
			B_Next.SetActive(value: false);
			B_Prev.SetActive(value: false);
			B_Vault.SetActive(value: true);
			B_Scan.SetActive(value: false);
			T_TopBar.text = "HOME";
			Draw_Tiles_Home();
			ControlPoster.gameObject.SetActive(value: false);
		}

		private void SetMode_Category()
		{
			m_curMode = ItemSpawnerPageMode.Category;
			P_Tiles.SetActive(value: true);
			P_Details.SetActive(value: false);
			P_Vault.SetActive(value: false);
			B_PageIndicator.SetActive(value: false);
			B_Back.SetActive(value: true);
			B_Spawn.SetActive(value: false);
			B_Next.SetActive(value: false);
			B_Prev.SetActive(value: false);
			B_Vault.SetActive(value: false);
			B_Scan.SetActive(value: false);
			T_TopBar.text = "HOME";
			T_TopBar.text += " | ";
			T_TopBar.text += IM.CDefInfo[m_curCategory].DisplayName;
			Draw_Tiles_Category(m_curCategory);
			ControlPoster.gameObject.SetActive(value: false);
		}

		private void SetMode_SubCategory()
		{
			m_curMode = ItemSpawnerPageMode.SubCategory;
			P_Tiles.SetActive(value: true);
			P_Details.SetActive(value: false);
			P_Vault.SetActive(value: false);
			m_curPage = GetSavedPageIndex(m_curSubCategory);
			m_maxPage = Mathf.FloorToInt((IM.GetAvailableCountInSubCategory(m_curSubCategory) - 1) / 10);
			if (m_curPage > m_maxPage)
			{
				m_curPage = m_maxPage;
				UpdateSavedPage(m_curSubCategory, m_curPage);
			}
			B_Back.SetActive(value: true);
			B_Spawn.SetActive(value: false);
			if (m_maxPage > 0)
			{
				if (m_curPage > 0)
				{
					B_Prev.SetActive(value: true);
				}
				else
				{
					B_Prev.SetActive(value: false);
				}
				if (m_curPage < m_maxPage)
				{
					B_Next.SetActive(value: true);
				}
				else
				{
					B_Next.SetActive(value: false);
				}
				B_PageIndicator.SetActive(value: true);
			}
			else
			{
				B_Next.SetActive(value: false);
				B_Prev.SetActive(value: false);
				B_PageIndicator.SetActive(value: false);
			}
			B_Vault.SetActive(value: false);
			B_Scan.SetActive(value: false);
			T_TopBar.text = "HOME";
			T_TopBar.text += " | ";
			T_TopBar.text += IM.CDefInfo[m_curCategory].DisplayName;
			T_TopBar.text += " | ";
			T_TopBar.text += IM.CDefSubInfo[m_curSubCategory].DisplayName;
			Draw_Tiles_SubCategory(m_curSubCategory);
			UpdatePageIndicator(m_curPage, m_maxPage);
			ControlPoster.gameObject.SetActive(value: false);
		}

		private void SetMode_Details()
		{
			m_curMode = ItemSpawnerPageMode.Details;
			P_Tiles.SetActive(value: false);
			P_Details.SetActive(value: true);
			P_Vault.SetActive(value: false);
			B_PageIndicator.SetActive(value: false);
			B_Back.SetActive(value: true);
			B_Next.SetActive(value: false);
			B_Prev.SetActive(value: false);
			B_Vault.SetActive(value: false);
			B_Scan.SetActive(value: false);
			T_TopBar.text = "HOME";
			T_TopBar.text += " | ";
			T_TopBar.text += IM.CDefInfo[m_curCategory].DisplayName;
			T_TopBar.text += " | ";
			T_TopBar.text += IM.CDefSubInfo[m_curSubCategory].DisplayName;
			T_TopBar.text += " | ";
			T_TopBar.text += m_curID.DisplayName;
			Draw_Tiles_Detail(m_curID);
			T_Details_Title.text = m_curID.DisplayName;
			T_Details_Subheading.text = m_curID.SubHeading;
			T_Details_Info.text = m_curID.Description;
			UpdatePageIndicator(m_curPage, m_maxPage);
			if (m_curID.Infographic == null)
			{
				ControlPoster.gameObject.SetActive(value: false);
				return;
			}
			ControlPoster.gameObject.SetActive(value: true);
			ControlPoster.material.SetTexture("_MainTex", m_curID.Infographic.Poster);
		}

		private void SetMode_Vault()
		{
			m_curMode = ItemSpawnerPageMode.Vault;
			P_Tiles.SetActive(value: false);
			P_Details.SetActive(value: false);
			P_Vault.SetActive(value: true);
			B_Back.SetActive(value: true);
			B_Spawn.SetActive(value: false);
			UpdateVaultFileNameList();
			ReCalcVaultPage();
			B_Vault.SetActive(value: false);
			B_Scan.SetActive(value: true);
			T_TopBar.text = "VAULT: SAVED GUN CONFIGS";
			Draw_Tiles_Vault(m_curVaultPage);
			UpdatePageIndicator(m_curVaultPage, m_maxVaultPage);
			ControlPoster.gameObject.SetActive(value: false);
		}

		private void Draw_Tiles_Home()
		{
			int num = 0;
			for (int i = 0; i < IM.CDefs.Categories.Length; i++)
			{
				bool flag = false;
				flag = ((!GM.CurrentSceneSettings.UsesUnlockSystem) ? IM.CDefs.Categories[i].DoesDisplay_Sandbox : IM.CDefs.Categories[i].DoesDisplay_Unlocks);
				if (flag && IM.CD.ContainsKey(IM.CDefs.Categories[i].Cat))
				{
					Tiles_SelectionPage[num].gameObject.SetActive(value: true);
					Tiles_SelectionPage[num].Image.sprite = IM.CDefs.Categories[i].Sprite;
					Tiles_SelectionPage[num].Text.text = IM.CDefs.Categories[i].DisplayName;
					Tiles_SelectionPage[num].Category = IM.CDefs.Categories[i].Cat;
					Tiles_SelectionPage[num].LockedCorner.gameObject.SetActive(value: false);
					num++;
				}
			}
			for (int j = num; j < Tiles_SelectionPage.Length; j++)
			{
				Tiles_SelectionPage[j].gameObject.SetActive(value: false);
			}
		}

		private void Draw_Tiles_Category(ItemSpawnerID.EItemCategory Category)
		{
			int num = 0;
			for (int i = 0; i < IM.CDefSubs[Category].Count; i++)
			{
				bool flag = false;
				flag = ((!GM.CurrentSceneSettings.UsesUnlockSystem) ? IM.CDefSubs[Category][i].DoesDisplay_Sandbox : IM.CDefSubs[Category][i].DoesDisplay_Unlocks);
				if (flag && IM.SCD.ContainsKey(IM.CDefSubs[Category][i].Subcat))
				{
					Tiles_SelectionPage[num].gameObject.SetActive(value: true);
					Tiles_SelectionPage[num].Image.sprite = IM.CDefSubs[Category][i].Sprite;
					Tiles_SelectionPage[num].Text.text = IM.CDefSubs[Category][i].DisplayName;
					Tiles_SelectionPage[num].SubCategory = IM.CDefSubs[Category][i].Subcat;
					Tiles_SelectionPage[num].LockedCorner.gameObject.SetActive(value: false);
					num++;
				}
			}
			for (int j = num; j < Tiles_SelectionPage.Length; j++)
			{
				Tiles_SelectionPage[j].gameObject.SetActive(value: false);
			}
		}

		private void Draw_Tiles_SubCategory(ItemSpawnerID.ESubCategory SubCategory)
		{
			int num = 0;
			bool sandboxMode = !GM.CurrentSceneSettings.UsesUnlockSystem;
			List<ItemSpawnerID> availableInSubCategory = IM.GetAvailableInSubCategory(SubCategory);
			for (int i = m_curPage * 10; i < availableInSubCategory.Count; i++)
			{
				if (num < Mathf.Min(10 + m_curPage * 10, 10))
				{
					Tiles_SelectionPage[num].gameObject.SetActive(value: true);
					Tiles_SelectionPage[num].Image.sprite = availableInSubCategory[i].Sprite;
					Tiles_SelectionPage[num].Text.text = availableInSubCategory[i].DisplayName;
					Tiles_SelectionPage[num].Item = availableInSubCategory[i];
					if (GM.Omni.OmniUnlocks.IsEquipmentUnlocked(Tiles_SelectionPage[num].Item, sandboxMode))
					{
						Tiles_SelectionPage[num].IsSpawnable = true;
						Tiles_SelectionPage[num].LockedCorner.gameObject.SetActive(value: false);
					}
					else
					{
						Tiles_SelectionPage[num].IsSpawnable = false;
						Tiles_SelectionPage[num].LockedCorner.gameObject.SetActive(value: true);
					}
					num++;
				}
			}
			for (int j = num; j < Tiles_SelectionPage.Length; j++)
			{
				Tiles_SelectionPage[j].gameObject.SetActive(value: false);
			}
		}

		private void UpdateDetailSelectedItem()
		{
			int num = 0;
			bool sandboxMode = !GM.CurrentSceneSettings.UsesUnlockSystem;
			if (GM.Omni.OmniUnlocks.IsEquipmentUnlocked(Tiles_Details[num].Item, sandboxMode))
			{
				Tiles_Details[num].IsSpawnable = true;
				Tiles_Details[num].LockedCorner.gameObject.SetActive(value: false);
			}
			else
			{
				Tiles_Details[num].IsSpawnable = false;
				Tiles_Details[num].LockedCorner.gameObject.SetActive(value: true);
				Tiles_Details[num].LockedText.text = "Buy [" + Tiles_Details[num].Item.UnlockCost + " S.A.U.C.E.]";
			}
			B_Spawn.SetActive(Tiles_Details[0].IsSpawnable);
		}

		private void Draw_Tiles_Detail(ItemSpawnerID item)
		{
			int num = 0;
			bool sandboxMode = !GM.CurrentSceneSettings.UsesUnlockSystem;
			Tiles_Details[num].gameObject.SetActive(value: true);
			Tiles_Details[num].Image.sprite = item.Sprite;
			Tiles_Details[num].Text.text = item.DisplayName;
			Tiles_Details[num].Item = item;
			if (GM.Omni.OmniUnlocks.IsEquipmentUnlocked(Tiles_Details[num].Item, sandboxMode))
			{
				Tiles_Details[num].IsSpawnable = true;
				Tiles_Details[num].LockedCorner.gameObject.SetActive(value: false);
			}
			else
			{
				Tiles_Details[num].IsSpawnable = false;
				Tiles_Details[num].LockedCorner.gameObject.SetActive(value: true);
				Tiles_Details[num].LockedText.text = "Buy [" + Tiles_Details[num].Item.UnlockCost + " S.A.U.C.E.]";
			}
			B_Spawn.SetActive(Tiles_Details[0].IsSpawnable);
			num = 1;
			for (int i = 0; i < item.Secondaries.Length; i++)
			{
				if (num < 13)
				{
					Tiles_Details[num].gameObject.SetActive(value: true);
					Tiles_Details[num].Image.sprite = item.Secondaries[i].Sprite;
					Tiles_Details[num].Text.text = item.Secondaries[i].DisplayName;
					Tiles_Details[num].Item = item.Secondaries[i];
					if (GM.Omni.OmniUnlocks.IsEquipmentUnlocked(Tiles_Details[num].Item, sandboxMode))
					{
						Tiles_Details[num].IsSpawnable = true;
						Tiles_Details[num].LockedCorner.gameObject.SetActive(value: false);
					}
					else
					{
						Tiles_Details[num].IsSpawnable = false;
						Tiles_Details[num].LockedCorner.gameObject.SetActive(value: true);
					}
					num++;
				}
			}
			for (int j = num; j < Tiles_Details.Length; j++)
			{
				Tiles_Details[j].gameObject.SetActive(value: false);
			}
		}

		private void UpdateVaultFileNameList()
		{
			m_vaultFileNames.Clear();
			string[] files = ES2.GetFiles(string.Empty, "*.txt");
			for (int i = 0; i < files.Length; i++)
			{
				if (files[i].Contains(m_staticVaultVersionToken))
				{
					m_vaultFileNames.Add(files[i]);
				}
			}
			m_vaultSavedGunList.Clear();
			for (int j = 0; j < m_vaultFileNames.Count; j++)
			{
				if (ES2.Exists(m_vaultFileNames[j]))
				{
					using ES2Reader eS2Reader = ES2Reader.Create(m_vaultFileNames[j]);
					SavedGun item = eS2Reader.Read<SavedGun>("SavedGun");
					m_vaultSavedGunList.Add(item);
				}
			}
			m_vaultForWhichSubCatExists.Clear();
			for (int k = 0; k < m_vaultSavedGunList.Count; k++)
			{
				FVRObject fVRObject = IM.OD[m_vaultSavedGunList[k].Components[0].ObjectID];
				if (IM.HasSpawnedID(fVRObject.SpawnedFromId))
				{
					ItemSpawnerID spawnerID = IM.GetSpawnerID(fVRObject.SpawnedFromId);
					m_vaultForWhichSubCatExists.Add(spawnerID.SubCategory);
				}
			}
			for (int l = 0; l < SubCategoriesByButton.Count; l++)
			{
				if (m_vaultForWhichSubCatExists.Contains(SubCategoriesByButton[l]))
				{
					SubCategoryButtons[l].SetActive(value: true);
				}
				else
				{
					SubCategoryButtons[l].SetActive(value: false);
				}
			}
			if (m_currentVaultFilter != 0 && !m_vaultForWhichSubCatExists.Contains(m_currentVaultFilter))
			{
				m_currentVaultFilter = ItemSpawnerID.ESubCategory.None;
			}
		}

		public void RemoveVaultGunAtIndex(int i)
		{
			string fileName = Tiles_Vault[i].SavedGun.FileName;
			if (ES2.Exists(fileName))
			{
				ES2.Delete(fileName);
			}
			UpdateVaultFileNameList();
		}

		public void SaveGunToVault(SavedGun g)
		{
			FVRObject fVRObject = IM.OD[g.Components[0].ObjectID];
			string text = Environment.UserName + "_" + g.Components[0].ObjectID + "_" + DateTime.Now.ToString("o") + "_" + safeIncrement + "_" + m_staticVaultVersionToken + ".txt";
			safeIncrement++;
			if (safeIncrement > 9)
			{
				safeIncrement = 0;
			}
			char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
			foreach (char oldChar in invalidFileNameChars)
			{
				text = text.Replace(oldChar, '_');
			}
			g.FileName = text;
			using (ES2Writer eS2Writer = ES2Writer.Create(text))
			{
				eS2Writer.Write(g, "SavedGun");
				eS2Writer.Save();
			}
			UpdateVaultFileNameList();
		}

		private void Draw_Tiles_Vault(int page)
		{
			UpdateVaultFileNameList();
			int num = page * 5;
			int num2 = m_vaultCulledList.Count - 1;
			for (int i = 0; i < Tiles_Vault.Length; i++)
			{
				int num3 = num + i;
				if (num3 <= num2)
				{
					Tiles_Vault[i].gameObject.SetActive(value: true);
					SavedGun savedGun = m_vaultCulledList[num3];
					Tiles_Vault[i].SavedGun = savedGun;
					FVRObject fVRObject = IM.OD[savedGun.Components[0].ObjectID];
					ItemSpawnerID spawnerID = IM.GetSpawnerID(fVRObject.SpawnedFromId);
					Tiles_Vault[i].Image.sprite = spawnerID.Sprite;
					Tiles_Vault[i].Text_Name.text = num3 + 1 + ":" + fVRObject.DisplayName;
					Tiles_Vault[i].Text_Date.text = savedGun.DateMade.ToString("dd-MM-yy");
					int num4 = savedGun.Components.Count - 1;
					for (int j = 0; j < Tiles_Vault[i].AttachedComponents.Length; j++)
					{
						if (j < num4)
						{
							Tiles_Vault[i].AttachedComponents[j].gameObject.SetActive(value: true);
							string objectID = savedGun.Components[j + 1].ObjectID;
							FVRObject fVRObject2 = IM.OD[objectID];
							if (IM.HasSpawnedID(fVRObject2.SpawnedFromId))
							{
								ItemSpawnerID spawnerID2 = IM.GetSpawnerID(fVRObject2.SpawnedFromId);
								Sprite sprite = spawnerID2.Sprite;
								Tiles_Vault[i].AttachedComponents[j].sprite = sprite;
							}
							else
							{
								Tiles_Vault[i].AttachedComponents[j].gameObject.SetActive(value: false);
							}
						}
						else
						{
							Tiles_Vault[i].AttachedComponents[j].gameObject.SetActive(value: false);
						}
					}
					if (GM.CurrentSceneSettings.UsesUnlockSystem)
					{
						Tiles_Vault[i].LockedCorner.gameObject.SetActive(!IsAggregateUnlocked(savedGun));
					}
					else
					{
						Tiles_Vault[i].LockedCorner.gameObject.SetActive(value: false);
					}
				}
				else
				{
					Tiles_Vault[i].gameObject.SetActive(value: false);
					Tiles_Vault[i].SavedGun = null;
				}
			}
		}

		private bool IsAggregateUnlocked(SavedGun gun)
		{
			for (int i = 0; i < gun.Components.Count; i++)
			{
				if (!GM.Omni.OmniUnlocks.IsEquipmentUnlocked(IM.OD[gun.Components[i].ObjectID].SpawnedFromId, SandboxMode: false))
				{
					return false;
				}
			}
			return true;
		}
	}
}
