// Decompiled with JetBrains decompiler
// Type: FistVR.SMEME
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class SMEME : MonoBehaviour
  {
    public Text LBL_Money;
    public List<SMEMErow> Rows_Inventory;
    public List<SMEMErow> Rows_MarketPlace;
    public List<ItemSpawnerID> DontUnlock;
    public List<ItemSpawnerID> IDs;
    public List<SMEME.HatRarity> Rarities;
    private List<int> m_mp_quantities = new List<int>();
    private List<int> m_mp_buycosts = new List<int>();
    private List<int> m_mp_sellcosts = new List<int>();
    public List<ItemSpawnerID> H_BB;
    public List<ItemSpawnerID> H_RA;
    public List<ItemSpawnerID> H_MR;
    public List<ItemSpawnerID> H_ME;
    public List<ItemSpawnerID> H_MW;
    public List<ItemSpawnerID> H_WD;
    private List<float> m_c = new List<float>()
    {
      -1f,
      -1f,
      0.0f,
      80f,
      90f,
      96f,
      99f,
      99.75f
    };
    public AudioEvent AudEvent_click;
    public Transform SpawnPoint;
    [Header("Inventory")]
    public Text PageNumberCurrent_Inventory;
    private int m_inventory_page;
    private List<ItemSpawnerID> m_inventory = new List<ItemSpawnerID>();
    private List<int> m_inventory_indicies = new List<int>();
    [Header("Marketplace")]
    public Text PageNumberCurrent_Marketplace;
    private int m_market_page;

    private void Start()
    {
      this.GenerateInitialMarketPlace();
      this.DrawGlobal();
      this.UpdateInventory();
      this.m_inventory_page = 0;
      this.DrawInventory();
      this.m_market_page = 0;
      this.DrawMarketPlace();
    }

    private void GenerateInitialMarketPlace()
    {
      for (int index = 0; index < this.IDs.Count; ++index)
      {
        switch (this.Rarities[index])
        {
          case SMEME.HatRarity.Case:
            this.m_mp_quantities.Add(UnityEngine.Random.Range(5100, 87000));
            this.m_mp_buycosts.Add(UnityEngine.Random.Range(119, 147));
            this.m_mp_sellcosts.Add(UnityEngine.Random.Range(79, 94));
            break;
          case SMEME.HatRarity.Key:
            this.m_mp_quantities.Add(UnityEngine.Random.Range(15100, 54000));
            this.m_mp_buycosts.Add(UnityEngine.Random.Range(249, 249));
            this.m_mp_sellcosts.Add(UnityEngine.Random.Range(179, 239));
            break;
          case SMEME.HatRarity.WellDone:
            this.m_mp_quantities.Add(UnityEngine.Random.Range(87100, 230000));
            this.m_mp_buycosts.Add(UnityEngine.Random.Range(170, 189));
            this.m_mp_sellcosts.Add(UnityEngine.Random.Range(19, 35));
            break;
          case SMEME.HatRarity.MemeWell:
            this.m_mp_quantities.Add(UnityEngine.Random.Range(32100, 69000));
            this.m_mp_buycosts.Add(UnityEngine.Random.Range(340, 490));
            this.m_mp_sellcosts.Add(UnityEngine.Random.Range(125, 179));
            break;
          case SMEME.HatRarity.Meme:
            this.m_mp_quantities.Add(UnityEngine.Random.Range(15100, 34000));
            this.m_mp_buycosts.Add(UnityEngine.Random.Range(890, 1530));
            this.m_mp_sellcosts.Add(UnityEngine.Random.Range(289, 689));
            break;
          case SMEME.HatRarity.MemeRare:
            this.m_mp_quantities.Add(UnityEngine.Random.Range(7500, 17000));
            this.m_mp_buycosts.Add(UnityEngine.Random.Range(2500, 4500));
            this.m_mp_sellcosts.Add(UnityEngine.Random.Range(1200, 1900));
            break;
          case SMEME.HatRarity.Rare:
            this.m_mp_quantities.Add(UnityEngine.Random.Range(3500, 7000));
            this.m_mp_buycosts.Add(UnityEngine.Random.Range(4500, 9500));
            this.m_mp_sellcosts.Add(UnityEngine.Random.Range(2800, 3700));
            break;
          case SMEME.HatRarity.BlackAndBlue:
            this.m_mp_quantities.Add(UnityEngine.Random.Range(600, 1500));
            this.m_mp_buycosts.Add(UnityEngine.Random.Range(13000, 37000));
            this.m_mp_sellcosts.Add(UnityEngine.Random.Range(8500, 11000));
            break;
        }
      }
    }

    private void TickMarket()
    {
    }

    private bool CanBuy(int i) => GM.MMFlags.GB >= i;

    public void BTN_Spawn(int i)
    {
      SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_click, this.transform.position);
      ItemSpawnerID id = this.Rows_Inventory[i].GetID();
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(id.MainObject.GetGameObject(), this.SpawnPoint.position, this.SpawnPoint.rotation);
      if (this.DontUnlock.Contains(id))
      {
        GM.MMFlags.RemoveHat(id.ItemID);
        GM.MMFlags.SaveToFile();
        if (!GM.MMFlags.HasHat(id.ItemID))
        {
          GM.Rewards.RewardUnlocks.LockReward(id.ItemID);
          GM.Rewards.SaveToFile();
        }
        this.UpdateInventory();
        this.DrawInventory();
      }
      GronchHatCase component = gameObject.GetComponent<GronchHatCase>();
      if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
        return;
      this.Generate(component);
    }

    private void Generate(GronchHatCase c)
    {
      float num = UnityEngine.Random.Range(0.0f, 100f);
      for (int index = this.m_c.Count - 1; index >= 2; --index)
      {
        if ((double) num >= (double) this.m_c[index])
        {
          this.Generate(c, index);
          break;
        }
      }
    }

    private void Generate(GronchHatCase c, int i)
    {
      switch (i)
      {
        case 2:
          this.Generate(c, this.H_WD[UnityEngine.Random.Range(0, this.H_WD.Count)]);
          break;
        case 3:
          this.Generate(c, this.H_MW[UnityEngine.Random.Range(0, this.H_MW.Count)]);
          break;
        case 4:
          this.Generate(c, this.H_ME[UnityEngine.Random.Range(0, this.H_ME.Count)]);
          break;
        case 5:
          this.Generate(c, this.H_MR[UnityEngine.Random.Range(0, this.H_MR.Count)]);
          break;
        case 6:
          this.Generate(c, this.H_RA[UnityEngine.Random.Range(0, this.H_RA.Count)]);
          break;
        case 7:
          this.Generate(c, this.H_BB[UnityEngine.Random.Range(0, this.H_BB.Count)]);
          break;
      }
    }

    private void Generate(GronchHatCase c, ItemSpawnerID id) => c.HID = id.ItemID;

    public void BTN_Buy(int i)
    {
      SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_click, this.transform.position);
      ItemSpawnerID id = this.Rows_MarketPlace[i].GetID();
      int price = this.Rows_MarketPlace[i].GetPrice();
      if (!this.CanBuy(price))
        return;
      GM.MMFlags.SGB(price);
      GM.MMFlags.AddHat(id.ItemID);
      GM.MMFlags.SaveToFile();
      if (!this.DontUnlock.Contains(id))
      {
        GM.Rewards.RewardUnlocks.UnlockReward(id.ItemID);
        GM.Rewards.SaveToFile();
      }
      this.UpdateInventory();
      this.TickMarket();
      this.DrawGlobal();
      this.DrawInventory();
      this.DrawMarketPlace();
    }

    public void BTN_Sell(int i)
    {
      SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_click, this.transform.position);
      ItemSpawnerID id = this.Rows_Inventory[i].GetID();
      GM.MMFlags.AGB(this.Rows_Inventory[i].GetPrice());
      GM.MMFlags.RemoveHat(id.ItemID);
      GM.MMFlags.SaveToFile();
      if (!GM.MMFlags.HasHat(id.ItemID))
      {
        GM.Rewards.RewardUnlocks.LockReward(id.ItemID);
        GM.Rewards.SaveToFile();
      }
      this.UpdateInventory();
      this.TickMarket();
      this.DrawGlobal();
      this.DrawInventory();
      this.DrawMarketPlace();
    }

    public void DrawGlobal() => this.LBL_Money.text = "G" + ((float) GM.MMFlags.GB * 0.01f).ToString("C", (IFormatProvider) new CultureInfo("en-US"));

    public void NextPage_Inventory()
    {
      ++this.m_inventory_page;
      this.DrawInventory();
      SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_click, this.transform.position);
    }

    public void PrevPage_Inventory()
    {
      --this.m_inventory_page;
      if (this.m_inventory_page < 0)
        this.m_inventory_page = 0;
      this.DrawInventory();
      SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_click, this.transform.position);
    }

    public void UpdateInventory()
    {
      this.m_inventory.Clear();
      this.m_inventory_indicies.Clear();
      for (int index = 0; index < this.IDs.Count; ++index)
      {
        if (GM.MMFlags.NumHat(this.IDs[index].ItemID) > 0)
        {
          this.m_inventory.Add(this.IDs[index]);
          this.m_inventory_indicies.Add(index);
        }
      }
    }

    public void DrawInventory()
    {
      if (this.m_inventory_page > Mathf.CeilToInt((float) (this.m_inventory.Count / 7)))
        this.m_inventory_page = 9;
      this.PageNumberCurrent_Inventory.text = (this.m_inventory_page + 1).ToString();
      int num1 = this.m_inventory_page * 7;
      int num2 = Mathf.Min(num1 + 7, this.m_inventory.Count);
      int index1 = num1;
      for (int index2 = 0; index2 < this.Rows_Inventory.Count; ++index2)
      {
        if (index1 < num2)
        {
          this.Rows_Inventory[index2].gameObject.SetActive(true);
          this.Rows_Inventory[index2].SetMarketPlaceListing(this.m_inventory[index1], this.Rarities[this.m_inventory_indicies[index1]], GM.MMFlags.NumHat(this.m_inventory[index1].ItemID), this.m_mp_sellcosts[this.m_inventory_indicies[index1]]);
        }
        else
          this.Rows_Inventory[index2].gameObject.SetActive(false);
        ++index1;
      }
    }

    public void NextPage_Marketplace()
    {
      ++this.m_market_page;
      this.DrawMarketPlace();
      SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_click, this.transform.position);
    }

    public void PrevPage_Marketplace()
    {
      --this.m_market_page;
      if (this.m_market_page < 0)
        this.m_market_page = 0;
      this.DrawMarketPlace();
      SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_click, this.transform.position);
    }

    private void DrawMarketPlace()
    {
      if (this.m_market_page > 9)
        this.m_market_page = 9;
      this.PageNumberCurrent_Marketplace.text = (this.m_market_page + 1).ToString();
      int num1 = this.m_market_page * 7;
      int num2 = Mathf.Min(num1 + 7, 69);
      int index1 = num1;
      for (int index2 = 0; index2 < this.Rows_MarketPlace.Count; ++index2)
      {
        if (index1 < num2)
        {
          this.Rows_MarketPlace[index2].gameObject.SetActive(true);
          this.Rows_MarketPlace[index2].SetMarketPlaceListing(this.IDs[index1], this.Rarities[index1], this.m_mp_quantities[index1], this.m_mp_buycosts[index1]);
        }
        else
          this.Rows_MarketPlace[index2].gameObject.SetActive(false);
        ++index1;
      }
    }

    public enum HatRarity
    {
      Case,
      Key,
      WellDone,
      MemeWell,
      Meme,
      MemeRare,
      Rare,
      BlackAndBlue,
    }
  }
}
