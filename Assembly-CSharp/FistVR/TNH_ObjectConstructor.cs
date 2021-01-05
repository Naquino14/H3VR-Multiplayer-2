// Decompiled with JetBrains decompiler
// Type: FistVR.TNH_ObjectConstructor
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class TNH_ObjectConstructor : MonoBehaviour
  {
    public TNH_Manager M;
    public List<TNH_ObjectConstructorIcon> Icons;
    private EquipmentPoolDef m_pool;
    private List<EquipmentPoolDef.PoolEntry> m_poolEntries = new List<EquipmentPoolDef.PoolEntry>();
    private List<int> m_poolAddedCost = new List<int>()
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
    public TNH_ObjectConstructor.ConstructorState State;
    private int m_curLevel;
    private bool allowEntry = true;

    public void SetValidErasSets(List<FVRObject.OTagEra> eras, List<FVRObject.OTagSet> sets)
    {
      for (int index = 0; index < eras.Count; ++index)
        this.m_validEras.Add(eras[index]);
      for (int index = 0; index < sets.Count; ++index)
        this.m_validSets.Add(sets[index]);
    }

    private void Start()
    {
      for (int index = 0; index < this.Icons.Count; ++index)
        this.Icons[index].Init();
    }

    public void ClearCase()
    {
      if (!((Object) this.m_spawnedCase != (Object) null))
        return;
      Object.Destroy((Object) this.m_spawnedCase.gameObject);
      this.m_spawnedCase = (GameObject) null;
    }

    public void SetRequiredPicatinnySightTable(ObjectTableDef def)
    {
      this.m_requiredPicatinnyTable = def;
      if (!((Object) this.m_requiredPicatinnyTable != (Object) null))
        return;
      this.m_hasRequiredPicatinnyTable = true;
      this.m_requiredSightTable = this.M.GetObjectTable(this.m_requiredPicatinnyTable);
    }

    public void SetEntries(
      TNH_Manager m,
      int m_level,
      EquipmentPoolDef pool,
      EquipmentPoolDef.PoolEntry pool1,
      EquipmentPoolDef.PoolEntry pool2,
      EquipmentPoolDef.PoolEntry pool3)
    {
      this.M = m;
      this.m_curLevel = m_level;
      this.m_pool = pool;
      this.m_poolEntries.Clear();
      this.m_poolEntries.Add(pool1);
      this.m_poolEntries.Add(pool2);
      this.m_poolEntries.Add(pool3);
      this.SetState(TNH_ObjectConstructor.ConstructorState.EntryList, 0);
      this.M.TokenCountChangeEvent += new TNH_Manager.TokenCountChange(this.UpdateTokenDisplay);
      this.UpdateTokenDisplay(this.M.GetNumTokens());
    }

    public void ResetEntry(
      int m_level,
      EquipmentPoolDef pool,
      EquipmentPoolDef.PoolEntry.PoolEntryType pType,
      int index)
    {
      this.m_curLevel = m_level;
      this.m_poolEntries[index] = this.GetPoolEntry(m_level, pool, pType, this.m_poolEntries[index]);
      this.SetState(TNH_ObjectConstructor.ConstructorState.EntryList, 0);
    }

    private void UpdateTokenDisplay(int numTokens)
    {
      int num1 = numTokens - 1;
      int num2 = num1 - this.m_numTokensSelected;
      for (int index = 0; index < this.TokenList.Count; ++index)
      {
        if (index <= num1)
        {
          if (index <= num2)
            this.TokenList[index].color = this.Token_Unselected;
          else
            this.TokenList[index].color = this.Token_Selected;
        }
        else
          this.TokenList[index].color = this.Token_Empty;
      }
    }

    private void OnDestroy()
    {
      if (!((Object) this.M != (Object) null))
        return;
      this.M.TokenCountChangeEvent -= new TNH_Manager.TokenCountChange(this.UpdateTokenDisplay);
    }

    private void UpdateRerollButtonState(bool disable)
    {
      if (disable)
      {
        for (int index = 0; index < this.RerollButtons.Count; ++index)
        {
          this.RerollButtons[index].SetActive(false);
          this.RerollButtonImages[index].SetActive(false);
        }
      }
      else
      {
        if (this.m_pool.GetNumEntries(EquipmentPoolDef.PoolEntry.PoolEntryType.Firearm, this.m_curLevel) < 2)
        {
          this.RerollButtons[0].SetActive(false);
          this.RerollButtonImages[0].SetActive(false);
        }
        else
        {
          this.RerollButtons[0].SetActive(true);
          this.RerollButtonImages[0].SetActive(true);
        }
        if (this.m_pool.GetNumEntries(EquipmentPoolDef.PoolEntry.PoolEntryType.Equipment, this.m_curLevel) < 2)
        {
          this.RerollButtons[1].SetActive(false);
          this.RerollButtonImages[1].SetActive(false);
        }
        else
        {
          this.RerollButtons[1].SetActive(true);
          this.RerollButtonImages[1].SetActive(true);
        }
        if (this.m_pool.GetNumEntries(EquipmentPoolDef.PoolEntry.PoolEntryType.Consumable, this.m_curLevel) < 2)
        {
          this.RerollButtons[2].SetActive(false);
          this.RerollButtonImages[2].SetActive(false);
        }
        else
        {
          this.RerollButtons[2].SetActive(true);
          this.RerollButtonImages[2].SetActive(true);
        }
      }
    }

    public void ButtonClicked(int i)
    {
      this.UpdateRerollButtonState(false);
      if (!this.allowEntry)
        return;
      if (this.State == TNH_ObjectConstructor.ConstructorState.EntryList)
      {
        int num = this.m_poolEntries[i].GetCost(this.M.EquipmentMode) + this.m_poolAddedCost[i];
        if (this.M.GetNumTokens() >= num)
        {
          this.SetState(TNH_ObjectConstructor.ConstructorState.Confirm, i);
          SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_Select, this.transform.position);
        }
        else
          SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_Fail, this.transform.position);
      }
      else
      {
        if (this.State != TNH_ObjectConstructor.ConstructorState.Confirm)
          return;
        switch (i)
        {
          case 0:
            this.SetState(TNH_ObjectConstructor.ConstructorState.EntryList, 0);
            this.m_selectedEntry = -1;
            SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_Back, this.transform.position);
            break;
          case 2:
            int i1 = this.m_poolEntries[this.m_selectedEntry].GetCost(this.M.EquipmentMode) + this.m_poolAddedCost[this.m_selectedEntry];
            if (this.M.GetNumTokens() >= i1)
            {
              if (this.CanSpawnObject(this.m_poolEntries[this.m_selectedEntry]))
              {
                AnvilManager.Run(this.SpawnObject(this.m_poolEntries[this.m_selectedEntry]));
                this.m_numTokensSelected = 0;
                this.M.SubtractTokens(i1);
                SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_Spawn, this.transform.position);
                if (this.M.C.UsesPurchasePriceIncrement)
                {
                  List<int> poolAddedCost;
                  int selectedEntry;
                  (poolAddedCost = this.m_poolAddedCost)[selectedEntry = this.m_selectedEntry] = poolAddedCost[selectedEntry] + 1;
                }
                this.SetState(TNH_ObjectConstructor.ConstructorState.EntryList, 0);
                this.m_selectedEntry = -1;
                break;
              }
              SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_Fail, this.transform.position);
              break;
            }
            SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_Fail, this.transform.position);
            break;
        }
      }
    }

    public void ButtonClicked_Reroll(int which)
    {
      int i = 1;
      if (this.M.GetNumTokens() >= i)
      {
        SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_Select, this.transform.position);
        this.M.RegenerateConstructor(this, which);
        this.M.SubtractTokens(i);
        this.UpdateTokenDisplay(this.M.GetNumTokens());
        this.m_poolAddedCost[which] = 0;
      }
      else
        SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_Fail, this.transform.position);
    }

    public EquipmentPoolDef.PoolEntry GetPoolEntry(
      int level,
      EquipmentPoolDef poolDef,
      EquipmentPoolDef.PoolEntry.PoolEntryType t,
      EquipmentPoolDef.PoolEntry prior = null)
    {
      float max = 0.0f;
      for (int index = 0; index < poolDef.Entries.Count; ++index)
      {
        if (poolDef.Entries[index].Type == t && poolDef.Entries[index].MinLevelAppears <= level && poolDef.Entries[index].MaxLevelAppears >= level && (poolDef.Entries.Count <= 1 || poolDef.Entries[index] != prior))
          max += poolDef.Entries[index].Rarity;
      }
      float num1 = Random.Range(0.0f, max);
      float num2 = 0.0f;
      EquipmentPoolDef.PoolEntry entry = poolDef.Entries[0];
      for (int index = 0; index < poolDef.Entries.Count; ++index)
      {
        if (poolDef.Entries[index].Type == t && poolDef.Entries[index].MinLevelAppears <= level && poolDef.Entries[index].MaxLevelAppears >= level && (poolDef.Entries.Count <= 1 || poolDef.Entries[index] != prior))
        {
          num2 += poolDef.Entries[index].Rarity;
          if ((double) num2 >= (double) num1)
          {
            entry = poolDef.Entries[index];
            break;
          }
        }
      }
      return entry;
    }

    private void SetState(TNH_ObjectConstructor.ConstructorState s, int whichItem)
    {
      this.State = s;
      switch (this.State)
      {
        case TNH_ObjectConstructor.ConstructorState.EntryList:
          this.UpdateRerollButtonState(false);
          this.m_numTokensSelected = 0;
          this.m_selectedEntry = -1;
          this.Icons[0].SetOption(TNH_ObjectConstructorIcon.IconState.Item, this.m_poolEntries[0].TableDef.Icon, this.m_poolEntries[0].GetCost(this.M.EquipmentMode) + this.m_poolAddedCost[0]);
          this.Icons[1].SetOption(TNH_ObjectConstructorIcon.IconState.Item, this.m_poolEntries[1].TableDef.Icon, this.m_poolEntries[1].GetCost(this.M.EquipmentMode) + this.m_poolAddedCost[1]);
          this.Icons[2].SetOption(TNH_ObjectConstructorIcon.IconState.Item, this.m_poolEntries[2].TableDef.Icon, this.m_poolEntries[2].GetCost(this.M.EquipmentMode) + this.m_poolAddedCost[2]);
          break;
        case TNH_ObjectConstructor.ConstructorState.Confirm:
          this.UpdateRerollButtonState(true);
          this.m_selectedEntry = whichItem;
          this.m_numTokensSelected = this.m_poolEntries[this.m_selectedEntry].GetCost(this.M.EquipmentMode) + this.m_poolAddedCost[this.m_selectedEntry];
          this.Icons[0].SetOption(TNH_ObjectConstructorIcon.IconState.Cancel, (Sprite) null, 0);
          this.Icons[1].SetOption(TNH_ObjectConstructorIcon.IconState.Item, this.m_poolEntries[this.m_selectedEntry].TableDef.Icon, this.m_poolEntries[this.m_selectedEntry].GetCost(this.M.EquipmentMode) + this.m_poolAddedCost[this.m_selectedEntry]);
          this.Icons[2].SetOption(TNH_ObjectConstructorIcon.IconState.Accept, (Sprite) null, 0);
          break;
      }
      this.UpdateTokenDisplay(this.M.GetNumTokens());
    }

    private Transform GetSpawnBasedOnSize(FVRObject.OTagFirearmSize s) => this.SpawnPoints_GunsSize[(int) (s - 1)];

    private bool IsEntryCase(EquipmentPoolDef.PoolEntry entry) => entry.TableDef.SpawnsInSmallCase || entry.TableDef.SpawnsInLargeCase;

    private bool CanSpawnObject(EquipmentPoolDef.PoolEntry entry)
    {
      this.M.GetObjectTable(entry.TableDef);
      return !entry.TableDef.SpawnsInSmallCase && !entry.TableDef.SpawnsInLargeCase || !((Object) this.m_spawnedCase != (Object) null);
    }

    [DebuggerHidden]
    private IEnumerator SpawnObject(EquipmentPoolDef.PoolEntry entry) => (IEnumerator) new TNH_ObjectConstructor.\u003CSpawnObject\u003Ec__Iterator0()
    {
      entry = entry,
      \u0024this = this
    };

    public enum ConstructorState
    {
      Bootup,
      EntryList,
      Confirm,
    }
  }
}
