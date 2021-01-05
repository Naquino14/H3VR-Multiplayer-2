// Decompiled with JetBrains decompiler
// Type: FistVR.ItemSpawnerUI
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class ItemSpawnerUI : MonoBehaviour
  {
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
    private ItemSpawnerUI.ItemSpawnerPageMode m_curMode;
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
      this.SetMode(ItemSpawnerUI.ItemSpawnerPageMode.Home);
      this.ControlPoster.gameObject.SetActive(false);
    }

    private void Update()
    {
      if ((double) this.refireTick > 0.0)
        this.refireTick -= Time.deltaTime;
      this.UpdateCurrencyDisplay();
    }

    public void PurchaseItem()
    {
      int index = 0;
      bool SandboxMode = !GM.CurrentSceneSettings.UsesUnlockSystem;
      if (this.m_curMode != ItemSpawnerUI.ItemSpawnerPageMode.Details || SandboxMode || GM.Omni.OmniUnlocks.IsEquipmentUnlocked(this.Tiles_Details[index].Item, SandboxMode))
        return;
      int unlockCost = this.Tiles_Details[index].Item.UnlockCost;
      if (GM.Omni.OmniUnlocks.HasCurrencyForPurchase(unlockCost))
      {
        this.ButtonPress(4);
        GM.Omni.OmniUnlocks.SpendCurrency(unlockCost);
        GM.Omni.OmniUnlocks.UnlockEquipment(this.Tiles_Details[index].Item, SandboxMode);
        GM.Omni.SaveUnlocksToFile();
        this.UpdateDetailSelectedItem();
      }
      else
        this.ButtonPress(3);
    }

    public void ButtonPress(int soundIndex)
    {
      this.AUD.PlayOneShot(this.AudClip_Chirps[soundIndex], 0.3f);
      this.refireTick = 0.2f;
    }

    public void ButtonPress_GoToVault()
    {
      if ((double) this.refireTick > 0.0)
        return;
      this.ButtonPress(0);
      this.SetMode_Vault();
    }

    private void UpdateSavedPage(ItemSpawnerID.ESubCategory cat, int index)
    {
      if (this.SavedPageIndexes.ContainsKey(cat))
        this.SavedPageIndexes[cat] = index;
      else
        this.SavedPageIndexes.Add(cat, index);
    }

    private int GetSavedPageIndex(ItemSpawnerID.ESubCategory cat) => this.SavedPageIndexes.ContainsKey(cat) ? this.SavedPageIndexes[cat] : 0;

    public void ButtonPress_Next()
    {
      if ((double) this.refireTick > 0.0)
        return;
      this.ButtonPress(0);
      switch (this.m_curMode)
      {
        case ItemSpawnerUI.ItemSpawnerPageMode.SubCategory:
          ++this.m_curPage;
          if (this.m_curPage < this.m_maxPage)
            this.B_Next.SetActive(true);
          else
            this.B_Next.SetActive(false);
          if (this.m_curPage > 0)
            this.B_Prev.SetActive(true);
          else
            this.B_Prev.SetActive(false);
          this.UpdateSavedPage(this.m_curSubCategory, this.m_curPage);
          this.UpdatePageIndicator(this.m_curPage, this.m_maxPage);
          this.Draw_Tiles_SubCategory(this.m_curSubCategory);
          break;
        case ItemSpawnerUI.ItemSpawnerPageMode.Vault:
          ++this.m_curVaultPage;
          if (this.m_curVaultPage < this.m_maxVaultPage)
            this.B_Next.SetActive(true);
          else
            this.B_Next.SetActive(false);
          if (this.m_curVaultPage > 0)
            this.B_Prev.SetActive(true);
          else
            this.B_Prev.SetActive(false);
          this.UpdatePageIndicator(this.m_curVaultPage, this.m_maxVaultPage);
          this.Draw_Tiles_Vault(this.m_curVaultPage);
          break;
      }
    }

    public void ButtonPress_Prev()
    {
      if ((double) this.refireTick > 0.0)
        return;
      this.ButtonPress(1);
      switch (this.m_curMode)
      {
        case ItemSpawnerUI.ItemSpawnerPageMode.SubCategory:
          --this.m_curPage;
          if (this.m_curPage < this.m_maxPage)
            this.B_Next.SetActive(true);
          else
            this.B_Next.SetActive(false);
          if (this.m_curPage > 0)
            this.B_Prev.SetActive(true);
          else
            this.B_Prev.SetActive(false);
          this.UpdateSavedPage(this.m_curSubCategory, this.m_curPage);
          this.UpdatePageIndicator(this.m_curPage, this.m_maxPage);
          this.Draw_Tiles_SubCategory(this.m_curSubCategory);
          break;
        case ItemSpawnerUI.ItemSpawnerPageMode.Vault:
          --this.m_curVaultPage;
          if (this.m_curVaultPage < this.m_maxVaultPage)
            this.B_Next.SetActive(true);
          else
            this.B_Next.SetActive(false);
          if (this.m_curVaultPage > 0)
            this.B_Prev.SetActive(true);
          else
            this.B_Prev.SetActive(false);
          this.UpdatePageIndicator(this.m_curVaultPage, this.m_maxVaultPage);
          this.Draw_Tiles_Vault(this.m_curVaultPage);
          break;
      }
    }

    public void ButtonPress_Back()
    {
      if ((double) this.refireTick > 0.0)
        return;
      this.ButtonPress(1);
      switch (this.m_curMode)
      {
        case ItemSpawnerUI.ItemSpawnerPageMode.Category:
          this.SetMode_Home();
          break;
        case ItemSpawnerUI.ItemSpawnerPageMode.SubCategory:
          this.SetMode_Category();
          break;
        case ItemSpawnerUI.ItemSpawnerPageMode.Details:
          this.SetMode_SubCategory();
          break;
        case ItemSpawnerUI.ItemSpawnerPageMode.Vault:
          this.SetMode_Home();
          break;
      }
    }

    public void ButtonPress_SelectionTile(int i)
    {
      if ((double) this.refireTick > 0.0)
        return;
      this.ButtonPress(0);
      switch (this.m_curMode)
      {
        case ItemSpawnerUI.ItemSpawnerPageMode.Home:
          this.m_curCategory = this.Tiles_SelectionPage[i].Category;
          this.SetMode_Category();
          break;
        case ItemSpawnerUI.ItemSpawnerPageMode.Category:
          this.m_curSubCategory = this.Tiles_SelectionPage[i].SubCategory;
          this.SetMode_SubCategory();
          break;
        case ItemSpawnerUI.ItemSpawnerPageMode.SubCategory:
          this.m_curID = this.Tiles_SelectionPage[i].Item;
          this.m_IDSelectedForSpawn = this.Tiles_SelectionPage[i].Item;
          this.SetMode_Details();
          break;
      }
    }

    public void ButtonPress_DetailTile(int i)
    {
      if ((double) this.refireTick > 0.0)
        return;
      this.ButtonPress(0);
      Sprite sprite = this.Tiles_Details[0].Image.sprite;
      string text = this.Tiles_Details[0].Text.text;
      ItemSpawnerID.EItemCategory category = this.Tiles_Details[0].Category;
      ItemSpawnerID.ESubCategory subCategory = this.Tiles_Details[0].SubCategory;
      ItemSpawnerID itemSpawnerId = this.Tiles_Details[0].Item;
      this.Tiles_Details[0].Image.sprite = this.Tiles_Details[i].Image.sprite;
      this.Tiles_Details[0].Text.text = this.Tiles_Details[i].Text.text;
      this.Tiles_Details[0].Category = this.Tiles_Details[i].Category;
      this.Tiles_Details[0].SubCategory = this.Tiles_Details[i].SubCategory;
      this.Tiles_Details[0].Item = this.Tiles_Details[i].Item;
      this.Tiles_Details[i].Image.sprite = sprite;
      this.Tiles_Details[i].Text.text = text;
      this.Tiles_Details[i].Category = category;
      this.Tiles_Details[i].SubCategory = subCategory;
      this.Tiles_Details[i].Item = itemSpawnerId;
      this.m_IDSelectedForSpawn = this.Tiles_Details[0].Item;
      this.T_Details_Title.text = this.Tiles_Details[0].Item.DisplayName;
      this.T_Details_Subheading.text = this.Tiles_Details[0].Item.SubHeading;
      this.T_Details_Info.text = this.Tiles_Details[0].Item.Description;
      bool SandboxMode = !GM.CurrentSceneSettings.UsesUnlockSystem;
      if (GM.Omni.OmniUnlocks.IsEquipmentUnlocked(this.Tiles_Details[0].Item, SandboxMode))
      {
        this.Tiles_Details[0].IsSpawnable = true;
        this.Tiles_Details[0].LockedCorner.gameObject.SetActive(false);
      }
      else
      {
        this.Tiles_Details[0].IsSpawnable = false;
        this.Tiles_Details[0].LockedCorner.gameObject.SetActive(true);
        this.Tiles_Details[0].LockedText.text = "Buy [" + (object) this.Tiles_Details[0].Item.UnlockCost + " S.A.U.C.E.]";
      }
      if (GM.Omni.OmniUnlocks.IsEquipmentUnlocked(this.Tiles_Details[i].Item, SandboxMode))
      {
        this.Tiles_Details[i].IsSpawnable = true;
        this.Tiles_Details[i].LockedCorner.gameObject.SetActive(false);
      }
      else
      {
        this.Tiles_Details[i].IsSpawnable = false;
        this.Tiles_Details[i].LockedCorner.gameObject.SetActive(true);
      }
      this.B_Spawn.SetActive(this.Tiles_Details[0].IsSpawnable);
    }

    public void ButtonPress_Spawn()
    {
      if ((double) this.refireTick > 0.0)
        return;
      AnvilManager.Run(this.SpawnItems());
    }

    private void SpawnItemz()
    {
      this.ButtonPress(2);
      GameObject gameObject1 = (GameObject) null;
      ItemSpawnerID selectedForSpawn = this.m_IDSelectedForSpawn;
      if ((UnityEngine.Object) selectedForSpawn == (UnityEngine.Object) null)
        return;
      GameObject gameObject2 = !selectedForSpawn.UsesLargeSpawnPad ? (!selectedForSpawn.UsesHugeSpawnPad ? UnityEngine.Object.Instantiate<GameObject>(selectedForSpawn.MainObject.GetGameObject(), this.SpawnPos_Small[this.m_curSmall].position, this.SpawnPos_Small[this.m_curSmall].rotation) : UnityEngine.Object.Instantiate<GameObject>(selectedForSpawn.MainObject.GetGameObject(), this.SpawnPos_Huge.position + Vector3.up * 0.2f, this.SpawnPos_Huge.rotation)) : UnityEngine.Object.Instantiate<GameObject>(selectedForSpawn.MainObject.GetGameObject(), this.SpawnPos_Main.position + Vector3.up * 0.2f, this.SpawnPos_Main.rotation);
      ++this.m_curSmall;
      if (this.m_curSmall >= this.SpawnPos_Small.Length)
        this.m_curSmall = 0;
      if ((UnityEngine.Object) selectedForSpawn.SecondObject != (UnityEngine.Object) null && (UnityEngine.Object) selectedForSpawn.SecondObject.GetGameObject() != (UnityEngine.Object) null)
      {
        gameObject1 = UnityEngine.Object.Instantiate<GameObject>(selectedForSpawn.SecondObject.GetGameObject(), this.SpawnPos_Small[this.m_curSmall].position, this.SpawnPos_Small[this.m_curSmall].rotation);
        ++this.m_curSmall;
        if (this.m_curSmall >= this.SpawnPos_Small.Length)
          this.m_curSmall = 0;
      }
      if ((UnityEngine.Object) gameObject2 != (UnityEngine.Object) null)
        gameObject2.GetComponent<FVRPhysicalObject>().IDSpawnedFrom = selectedForSpawn;
      if (!((UnityEngine.Object) gameObject1 != (UnityEngine.Object) null))
        return;
      gameObject1.GetComponent<FVRPhysicalObject>().IDSpawnedFrom = selectedForSpawn;
    }

    [DebuggerHidden]
    private IEnumerator SpawnItems() => (IEnumerator) new ItemSpawnerUI.\u003CSpawnItems\u003Ec__Iterator0()
    {
      \u0024this = this
    };

    public void ButtonPress_Scan()
    {
      if (this.FirearmSaver.TryToScanGun())
      {
        this.ButtonPress(6);
        this.refireTick = 1f;
        this.ReCalcVaultPage();
        this.UpdatePageIndicator(this.m_curVaultPage, this.m_maxVaultPage);
        this.Draw_Tiles_Vault(this.m_curVaultPage);
      }
      else
      {
        this.ButtonPress(5);
        this.refireTick = 1f;
      }
    }

    public void ButtonPress_SpawnVaultGun(int i)
    {
      if ((double) this.refireTick > 0.0)
        return;
      if (this.Tiles_Vault[i].LockedCorner.gameObject.activeSelf)
      {
        this.ButtonPress(5);
        this.refireTick = 1f;
      }
      else
      {
        this.ButtonPress(2);
        this.ReCalcVaultPage();
        this.m_isVaultGunAssembling = true;
        this.FirearmSaver.SpawnFirearm(this.Tiles_Vault[i].SavedGun);
        this.refireTick = 1f;
      }
    }

    public void VaultGunAssemblyFinished() => this.m_isVaultGunAssembling = false;

    public void ButtonPress_SetVaultFilter(int i)
    {
      this.ButtonPress(0);
      this.m_currentVaultFilter = i != -1 ? this.SubCategoriesByButton[i] : ItemSpawnerID.ESubCategory.None;
      this.ReCalcVaultPage();
      this.UpdatePageIndicator(this.m_curVaultPage, this.m_maxVaultPage);
      this.Draw_Tiles_Vault(this.m_curVaultPage);
    }

    public void ButtonPress_DeleteVaultGun(int i)
    {
      if ((double) this.refireTick > 0.0)
        return;
      this.ButtonPress(2);
      this.ReCalcVaultPage();
      this.UpdatePageIndicator(this.m_curVaultPage, this.m_maxVaultPage);
      this.Tiles_Vault[i].DeleteButton.SetActive(false);
      this.Tiles_Vault[i].ConfirmButton.SetActive(true);
      this.Draw_Tiles_Vault(this.m_curVaultPage);
    }

    public void ButtonPress_ConfirmDeleteVaultGun(int i)
    {
      if ((double) this.refireTick > 0.0)
        return;
      this.ButtonPress(2);
      this.RemoveVaultGunAtIndex(i);
      this.ReCalcVaultPage();
      this.UpdatePageIndicator(this.m_curVaultPage, this.m_maxVaultPage);
      this.Draw_Tiles_Vault(this.m_curVaultPage);
    }

    private void ReCalcVaultPage()
    {
      this.m_vaultCulledList.Clear();
      for (int index = 0; index < this.m_vaultSavedGunList.Count; ++index)
      {
        if (this.m_currentVaultFilter == ItemSpawnerID.ESubCategory.None)
        {
          this.m_vaultCulledList.Add(this.m_vaultSavedGunList[index]);
        }
        else
        {
          FVRObject fvrObject = IM.OD[this.m_vaultSavedGunList[index].Components[0].ObjectID];
          if (IM.HasSpawnedID(fvrObject.SpawnedFromId))
          {
            if (IM.GetSpawnerID(fvrObject.SpawnedFromId).SubCategory == this.m_currentVaultFilter)
              this.m_vaultCulledList.Add(this.m_vaultSavedGunList[index]);
          }
          else
            UnityEngine.Debug.Log((object) ("Oh dear, looks like we can't find ItemSpawnerID for: " + fvrObject.ItemID));
        }
      }
      this.m_maxVaultPage = Mathf.FloorToInt((float) ((this.m_vaultCulledList.Count - 1) / 5));
      if (this.m_curVaultPage > this.m_maxVaultPage)
        this.m_curVaultPage = this.m_maxVaultPage;
      if (this.m_curVaultPage < this.m_maxVaultPage)
        this.B_Next.SetActive(true);
      else
        this.B_Next.SetActive(false);
      if (this.m_curVaultPage > 0)
        this.B_Prev.SetActive(true);
      else
        this.B_Prev.SetActive(false);
      for (int index = 0; index < this.Tiles_Vault.Length; ++index)
      {
        this.Tiles_Vault[index].DeleteButton.SetActive(true);
        this.Tiles_Vault[index].ConfirmButton.SetActive(false);
      }
      if (this.m_maxVaultPage > 0)
        this.B_PageIndicator.SetActive(true);
      else
        this.B_PageIndicator.SetActive(false);
    }

    private void UpdatePageIndicator(int a, int b) => this.T_Page_Indicator.text = (a + 1).ToString() + "/" + (b + 1).ToString();

    private void UpdateCurrencyDisplay()
    {
      if (GM.CurrentSceneSettings.UsesUnlockSystem)
      {
        if (!this.T_Currency_Readout.gameObject.activeSelf)
          this.T_Currency_Readout.gameObject.SetActive(true);
        this.T_Currency_Readout.text = "S.A.U.C.E. - " + (object) GM.Omni.OmniUnlocks.SaucePackets;
      }
      else
      {
        if (!this.T_Currency_Readout.gameObject.activeSelf)
          return;
        this.T_Currency_Readout.gameObject.SetActive(false);
      }
    }

    private void SetMode(ItemSpawnerUI.ItemSpawnerPageMode mode)
    {
      this.UpdateCurrencyDisplay();
      switch (mode)
      {
        case ItemSpawnerUI.ItemSpawnerPageMode.Home:
          this.SetMode_Home();
          break;
        case ItemSpawnerUI.ItemSpawnerPageMode.Category:
          this.SetMode_Category();
          break;
        case ItemSpawnerUI.ItemSpawnerPageMode.SubCategory:
          this.SetMode_SubCategory();
          break;
        case ItemSpawnerUI.ItemSpawnerPageMode.Details:
          this.SetMode_Details();
          break;
      }
    }

    private void SetMode_Home()
    {
      this.m_curMode = ItemSpawnerUI.ItemSpawnerPageMode.Home;
      this.P_Tiles.SetActive(true);
      this.P_Details.SetActive(false);
      this.P_Vault.SetActive(false);
      this.B_PageIndicator.SetActive(false);
      this.B_Back.SetActive(false);
      this.B_Spawn.SetActive(false);
      this.B_Next.SetActive(false);
      this.B_Prev.SetActive(false);
      this.B_Vault.SetActive(true);
      this.B_Scan.SetActive(false);
      this.T_TopBar.text = "HOME";
      this.Draw_Tiles_Home();
      this.ControlPoster.gameObject.SetActive(false);
    }

    private void SetMode_Category()
    {
      this.m_curMode = ItemSpawnerUI.ItemSpawnerPageMode.Category;
      this.P_Tiles.SetActive(true);
      this.P_Details.SetActive(false);
      this.P_Vault.SetActive(false);
      this.B_PageIndicator.SetActive(false);
      this.B_Back.SetActive(true);
      this.B_Spawn.SetActive(false);
      this.B_Next.SetActive(false);
      this.B_Prev.SetActive(false);
      this.B_Vault.SetActive(false);
      this.B_Scan.SetActive(false);
      this.T_TopBar.text = "HOME";
      this.T_TopBar.text += " | ";
      this.T_TopBar.text += IM.CDefInfo[this.m_curCategory].DisplayName;
      this.Draw_Tiles_Category(this.m_curCategory);
      this.ControlPoster.gameObject.SetActive(false);
    }

    private void SetMode_SubCategory()
    {
      this.m_curMode = ItemSpawnerUI.ItemSpawnerPageMode.SubCategory;
      this.P_Tiles.SetActive(true);
      this.P_Details.SetActive(false);
      this.P_Vault.SetActive(false);
      this.m_curPage = this.GetSavedPageIndex(this.m_curSubCategory);
      this.m_maxPage = Mathf.FloorToInt((float) ((IM.GetAvailableCountInSubCategory(this.m_curSubCategory) - 1) / 10));
      if (this.m_curPage > this.m_maxPage)
      {
        this.m_curPage = this.m_maxPage;
        this.UpdateSavedPage(this.m_curSubCategory, this.m_curPage);
      }
      this.B_Back.SetActive(true);
      this.B_Spawn.SetActive(false);
      if (this.m_maxPage > 0)
      {
        if (this.m_curPage > 0)
          this.B_Prev.SetActive(true);
        else
          this.B_Prev.SetActive(false);
        if (this.m_curPage < this.m_maxPage)
          this.B_Next.SetActive(true);
        else
          this.B_Next.SetActive(false);
        this.B_PageIndicator.SetActive(true);
      }
      else
      {
        this.B_Next.SetActive(false);
        this.B_Prev.SetActive(false);
        this.B_PageIndicator.SetActive(false);
      }
      this.B_Vault.SetActive(false);
      this.B_Scan.SetActive(false);
      this.T_TopBar.text = "HOME";
      this.T_TopBar.text += " | ";
      this.T_TopBar.text += IM.CDefInfo[this.m_curCategory].DisplayName;
      this.T_TopBar.text += " | ";
      this.T_TopBar.text += IM.CDefSubInfo[this.m_curSubCategory].DisplayName;
      this.Draw_Tiles_SubCategory(this.m_curSubCategory);
      this.UpdatePageIndicator(this.m_curPage, this.m_maxPage);
      this.ControlPoster.gameObject.SetActive(false);
    }

    private void SetMode_Details()
    {
      this.m_curMode = ItemSpawnerUI.ItemSpawnerPageMode.Details;
      this.P_Tiles.SetActive(false);
      this.P_Details.SetActive(true);
      this.P_Vault.SetActive(false);
      this.B_PageIndicator.SetActive(false);
      this.B_Back.SetActive(true);
      this.B_Next.SetActive(false);
      this.B_Prev.SetActive(false);
      this.B_Vault.SetActive(false);
      this.B_Scan.SetActive(false);
      this.T_TopBar.text = "HOME";
      this.T_TopBar.text += " | ";
      this.T_TopBar.text += IM.CDefInfo[this.m_curCategory].DisplayName;
      this.T_TopBar.text += " | ";
      this.T_TopBar.text += IM.CDefSubInfo[this.m_curSubCategory].DisplayName;
      this.T_TopBar.text += " | ";
      this.T_TopBar.text += this.m_curID.DisplayName;
      this.Draw_Tiles_Detail(this.m_curID);
      this.T_Details_Title.text = this.m_curID.DisplayName;
      this.T_Details_Subheading.text = this.m_curID.SubHeading;
      this.T_Details_Info.text = this.m_curID.Description;
      this.UpdatePageIndicator(this.m_curPage, this.m_maxPage);
      if ((UnityEngine.Object) this.m_curID.Infographic == (UnityEngine.Object) null)
      {
        this.ControlPoster.gameObject.SetActive(false);
      }
      else
      {
        this.ControlPoster.gameObject.SetActive(true);
        this.ControlPoster.material.SetTexture("_MainTex", (Texture) this.m_curID.Infographic.Poster);
      }
    }

    private void SetMode_Vault()
    {
      this.m_curMode = ItemSpawnerUI.ItemSpawnerPageMode.Vault;
      this.P_Tiles.SetActive(false);
      this.P_Details.SetActive(false);
      this.P_Vault.SetActive(true);
      this.B_Back.SetActive(true);
      this.B_Spawn.SetActive(false);
      this.UpdateVaultFileNameList();
      this.ReCalcVaultPage();
      this.B_Vault.SetActive(false);
      this.B_Scan.SetActive(true);
      this.T_TopBar.text = "VAULT: SAVED GUN CONFIGS";
      this.Draw_Tiles_Vault(this.m_curVaultPage);
      this.UpdatePageIndicator(this.m_curVaultPage, this.m_maxVaultPage);
      this.ControlPoster.gameObject.SetActive(false);
    }

    private void Draw_Tiles_Home()
    {
      int index1 = 0;
      for (int index2 = 0; index2 < IM.CDefs.Categories.Length; ++index2)
      {
        if ((!GM.CurrentSceneSettings.UsesUnlockSystem ? IM.CDefs.Categories[index2].DoesDisplay_Sandbox : IM.CDefs.Categories[index2].DoesDisplay_Unlocks) && IM.CD.ContainsKey(IM.CDefs.Categories[index2].Cat))
        {
          this.Tiles_SelectionPage[index1].gameObject.SetActive(true);
          this.Tiles_SelectionPage[index1].Image.sprite = IM.CDefs.Categories[index2].Sprite;
          this.Tiles_SelectionPage[index1].Text.text = IM.CDefs.Categories[index2].DisplayName;
          this.Tiles_SelectionPage[index1].Category = IM.CDefs.Categories[index2].Cat;
          this.Tiles_SelectionPage[index1].LockedCorner.gameObject.SetActive(false);
          ++index1;
        }
      }
      for (int index2 = index1; index2 < this.Tiles_SelectionPage.Length; ++index2)
        this.Tiles_SelectionPage[index2].gameObject.SetActive(false);
    }

    private void Draw_Tiles_Category(ItemSpawnerID.EItemCategory Category)
    {
      int index1 = 0;
      for (int index2 = 0; index2 < IM.CDefSubs[Category].Count; ++index2)
      {
        if ((!GM.CurrentSceneSettings.UsesUnlockSystem ? IM.CDefSubs[Category][index2].DoesDisplay_Sandbox : IM.CDefSubs[Category][index2].DoesDisplay_Unlocks) && IM.SCD.ContainsKey(IM.CDefSubs[Category][index2].Subcat))
        {
          this.Tiles_SelectionPage[index1].gameObject.SetActive(true);
          this.Tiles_SelectionPage[index1].Image.sprite = IM.CDefSubs[Category][index2].Sprite;
          this.Tiles_SelectionPage[index1].Text.text = IM.CDefSubs[Category][index2].DisplayName;
          this.Tiles_SelectionPage[index1].SubCategory = IM.CDefSubs[Category][index2].Subcat;
          this.Tiles_SelectionPage[index1].LockedCorner.gameObject.SetActive(false);
          ++index1;
        }
      }
      for (int index2 = index1; index2 < this.Tiles_SelectionPage.Length; ++index2)
        this.Tiles_SelectionPage[index2].gameObject.SetActive(false);
    }

    private void Draw_Tiles_SubCategory(ItemSpawnerID.ESubCategory SubCategory)
    {
      int index1 = 0;
      bool SandboxMode = !GM.CurrentSceneSettings.UsesUnlockSystem;
      List<ItemSpawnerID> availableInSubCategory = IM.GetAvailableInSubCategory(SubCategory);
      for (int index2 = this.m_curPage * 10; index2 < availableInSubCategory.Count; ++index2)
      {
        if (index1 < Mathf.Min(10 + this.m_curPage * 10, 10))
        {
          this.Tiles_SelectionPage[index1].gameObject.SetActive(true);
          this.Tiles_SelectionPage[index1].Image.sprite = availableInSubCategory[index2].Sprite;
          this.Tiles_SelectionPage[index1].Text.text = availableInSubCategory[index2].DisplayName;
          this.Tiles_SelectionPage[index1].Item = availableInSubCategory[index2];
          if (GM.Omni.OmniUnlocks.IsEquipmentUnlocked(this.Tiles_SelectionPage[index1].Item, SandboxMode))
          {
            this.Tiles_SelectionPage[index1].IsSpawnable = true;
            this.Tiles_SelectionPage[index1].LockedCorner.gameObject.SetActive(false);
          }
          else
          {
            this.Tiles_SelectionPage[index1].IsSpawnable = false;
            this.Tiles_SelectionPage[index1].LockedCorner.gameObject.SetActive(true);
          }
          ++index1;
        }
      }
      for (int index2 = index1; index2 < this.Tiles_SelectionPage.Length; ++index2)
        this.Tiles_SelectionPage[index2].gameObject.SetActive(false);
    }

    private void UpdateDetailSelectedItem()
    {
      int index = 0;
      bool SandboxMode = !GM.CurrentSceneSettings.UsesUnlockSystem;
      if (GM.Omni.OmniUnlocks.IsEquipmentUnlocked(this.Tiles_Details[index].Item, SandboxMode))
      {
        this.Tiles_Details[index].IsSpawnable = true;
        this.Tiles_Details[index].LockedCorner.gameObject.SetActive(false);
      }
      else
      {
        this.Tiles_Details[index].IsSpawnable = false;
        this.Tiles_Details[index].LockedCorner.gameObject.SetActive(true);
        this.Tiles_Details[index].LockedText.text = "Buy [" + (object) this.Tiles_Details[index].Item.UnlockCost + " S.A.U.C.E.]";
      }
      this.B_Spawn.SetActive(this.Tiles_Details[0].IsSpawnable);
    }

    private void Draw_Tiles_Detail(ItemSpawnerID item)
    {
      int index1 = 0;
      bool SandboxMode = !GM.CurrentSceneSettings.UsesUnlockSystem;
      this.Tiles_Details[index1].gameObject.SetActive(true);
      this.Tiles_Details[index1].Image.sprite = item.Sprite;
      this.Tiles_Details[index1].Text.text = item.DisplayName;
      this.Tiles_Details[index1].Item = item;
      if (GM.Omni.OmniUnlocks.IsEquipmentUnlocked(this.Tiles_Details[index1].Item, SandboxMode))
      {
        this.Tiles_Details[index1].IsSpawnable = true;
        this.Tiles_Details[index1].LockedCorner.gameObject.SetActive(false);
      }
      else
      {
        this.Tiles_Details[index1].IsSpawnable = false;
        this.Tiles_Details[index1].LockedCorner.gameObject.SetActive(true);
        this.Tiles_Details[index1].LockedText.text = "Buy [" + (object) this.Tiles_Details[index1].Item.UnlockCost + " S.A.U.C.E.]";
      }
      this.B_Spawn.SetActive(this.Tiles_Details[0].IsSpawnable);
      int index2 = 1;
      for (int index3 = 0; index3 < item.Secondaries.Length; ++index3)
      {
        if (index2 < 13)
        {
          this.Tiles_Details[index2].gameObject.SetActive(true);
          this.Tiles_Details[index2].Image.sprite = item.Secondaries[index3].Sprite;
          this.Tiles_Details[index2].Text.text = item.Secondaries[index3].DisplayName;
          this.Tiles_Details[index2].Item = item.Secondaries[index3];
          if (GM.Omni.OmniUnlocks.IsEquipmentUnlocked(this.Tiles_Details[index2].Item, SandboxMode))
          {
            this.Tiles_Details[index2].IsSpawnable = true;
            this.Tiles_Details[index2].LockedCorner.gameObject.SetActive(false);
          }
          else
          {
            this.Tiles_Details[index2].IsSpawnable = false;
            this.Tiles_Details[index2].LockedCorner.gameObject.SetActive(true);
          }
          ++index2;
        }
      }
      for (int index3 = index2; index3 < this.Tiles_Details.Length; ++index3)
        this.Tiles_Details[index3].gameObject.SetActive(false);
    }

    private void UpdateVaultFileNameList()
    {
      this.m_vaultFileNames.Clear();
      string[] files = ES2.GetFiles(string.Empty, "*.txt");
      for (int index = 0; index < files.Length; ++index)
      {
        if (files[index].Contains(this.m_staticVaultVersionToken))
          this.m_vaultFileNames.Add(files[index]);
      }
      this.m_vaultSavedGunList.Clear();
      for (int index = 0; index < this.m_vaultFileNames.Count; ++index)
      {
        if (ES2.Exists(this.m_vaultFileNames[index]))
        {
          using (ES2Reader es2Reader = ES2Reader.Create(this.m_vaultFileNames[index]))
            this.m_vaultSavedGunList.Add(es2Reader.Read<SavedGun>("SavedGun"));
        }
      }
      this.m_vaultForWhichSubCatExists.Clear();
      for (int index = 0; index < this.m_vaultSavedGunList.Count; ++index)
      {
        FVRObject fvrObject = IM.OD[this.m_vaultSavedGunList[index].Components[0].ObjectID];
        if (IM.HasSpawnedID(fvrObject.SpawnedFromId))
          this.m_vaultForWhichSubCatExists.Add(IM.GetSpawnerID(fvrObject.SpawnedFromId).SubCategory);
      }
      for (int index = 0; index < this.SubCategoriesByButton.Count; ++index)
      {
        if (this.m_vaultForWhichSubCatExists.Contains(this.SubCategoriesByButton[index]))
          this.SubCategoryButtons[index].SetActive(true);
        else
          this.SubCategoryButtons[index].SetActive(false);
      }
      if (this.m_currentVaultFilter == ItemSpawnerID.ESubCategory.None || this.m_vaultForWhichSubCatExists.Contains(this.m_currentVaultFilter))
        return;
      this.m_currentVaultFilter = ItemSpawnerID.ESubCategory.None;
    }

    public void RemoveVaultGunAtIndex(int i)
    {
      string fileName = this.Tiles_Vault[i].SavedGun.FileName;
      if (ES2.Exists(fileName))
        ES2.Delete(fileName);
      this.UpdateVaultFileNameList();
    }

    public void SaveGunToVault(SavedGun g)
    {
      FVRObject fvrObject = IM.OD[g.Components[0].ObjectID];
      string identifier = Environment.UserName + "_" + g.Components[0].ObjectID + "_" + DateTime.Now.ToString("o") + "_" + (object) this.safeIncrement + "_" + this.m_staticVaultVersionToken + ".txt";
      ++this.safeIncrement;
      if (this.safeIncrement > 9)
        this.safeIncrement = 0;
      foreach (char invalidFileNameChar in Path.GetInvalidFileNameChars())
        identifier = identifier.Replace(invalidFileNameChar, '_');
      g.FileName = identifier;
      using (ES2Writer es2Writer = ES2Writer.Create(identifier))
      {
        es2Writer.Write<SavedGun>(g, "SavedGun");
        es2Writer.Save();
      }
      this.UpdateVaultFileNameList();
    }

    private void Draw_Tiles_Vault(int page)
    {
      this.UpdateVaultFileNameList();
      int num1 = page * 5;
      int num2 = this.m_vaultCulledList.Count - 1;
      for (int index1 = 0; index1 < this.Tiles_Vault.Length; ++index1)
      {
        int index2 = num1 + index1;
        if (index2 <= num2)
        {
          this.Tiles_Vault[index1].gameObject.SetActive(true);
          SavedGun vaultCulled = this.m_vaultCulledList[index2];
          this.Tiles_Vault[index1].SavedGun = vaultCulled;
          FVRObject fvrObject1 = IM.OD[vaultCulled.Components[0].ObjectID];
          ItemSpawnerID spawnerId = IM.GetSpawnerID(fvrObject1.SpawnedFromId);
          this.Tiles_Vault[index1].Image.sprite = spawnerId.Sprite;
          this.Tiles_Vault[index1].Text_Name.text = (index2 + 1).ToString() + ":" + fvrObject1.DisplayName;
          this.Tiles_Vault[index1].Text_Date.text = vaultCulled.DateMade.ToString("dd-MM-yy");
          int num3 = vaultCulled.Components.Count - 1;
          for (int index3 = 0; index3 < this.Tiles_Vault[index1].AttachedComponents.Length; ++index3)
          {
            if (index3 < num3)
            {
              this.Tiles_Vault[index1].AttachedComponents[index3].gameObject.SetActive(true);
              FVRObject fvrObject2 = IM.OD[vaultCulled.Components[index3 + 1].ObjectID];
              if (IM.HasSpawnedID(fvrObject2.SpawnedFromId))
              {
                Sprite sprite = IM.GetSpawnerID(fvrObject2.SpawnedFromId).Sprite;
                this.Tiles_Vault[index1].AttachedComponents[index3].sprite = sprite;
              }
              else
                this.Tiles_Vault[index1].AttachedComponents[index3].gameObject.SetActive(false);
            }
            else
              this.Tiles_Vault[index1].AttachedComponents[index3].gameObject.SetActive(false);
          }
          if (GM.CurrentSceneSettings.UsesUnlockSystem)
            this.Tiles_Vault[index1].LockedCorner.gameObject.SetActive(!this.IsAggregateUnlocked(vaultCulled));
          else
            this.Tiles_Vault[index1].LockedCorner.gameObject.SetActive(false);
        }
        else
        {
          this.Tiles_Vault[index1].gameObject.SetActive(false);
          this.Tiles_Vault[index1].SavedGun = (SavedGun) null;
        }
      }
    }

    private bool IsAggregateUnlocked(SavedGun gun)
    {
      for (int index = 0; index < gun.Components.Count; ++index)
      {
        if (!GM.Omni.OmniUnlocks.IsEquipmentUnlocked(IM.OD[gun.Components[index].ObjectID].SpawnedFromId, false))
          return false;
      }
      return true;
    }

    public enum ItemSpawnerPageMode
    {
      Home,
      Category,
      SubCategory,
      Details,
      Vault,
    }
  }
}
