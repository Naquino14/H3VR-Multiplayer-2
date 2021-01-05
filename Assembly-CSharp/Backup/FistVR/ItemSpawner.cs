// Decompiled with JetBrains decompiler
// Type: FistVR.ItemSpawner
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class ItemSpawner : MonoBehaviour
  {
    private List<ItemSpawnerObjectDefinition> Definitions = new List<ItemSpawnerObjectDefinition>();
    private Dictionary<ItemSpawnerObjectDefinition.ItemSpawnerCategory, List<ItemSpawnerObjectDefinition>> DefDic = new Dictionary<ItemSpawnerObjectDefinition.ItemSpawnerCategory, List<ItemSpawnerObjectDefinition>>();
    private ItemSpawnerObjectDefinition.ItemSpawnerCategory CurrentCategory = ItemSpawnerObjectDefinition.ItemSpawnerCategory.None;
    private Dictionary<ItemSpawnerObjectDefinition.ItemSpawnerCategory, int> CurrentItemIndex = new Dictionary<ItemSpawnerObjectDefinition.ItemSpawnerCategory, int>();
    public Text ItemName;
    public Image ItemPic;
    public Transform SpawnPoint;

    private void Awake()
    {
      this.PrimeDefs();
      this.UpdateDisplay();
    }

    private void PrimeDefs()
    {
      foreach (ItemSpawnerObjectDefinition objectDefinition in UnityEngine.Resources.LoadAll("ItemSpawnerDefinitions", typeof (ItemSpawnerObjectDefinition)))
        this.Definitions.Add(objectDefinition);
      for (int index = 0; index < this.Definitions.Count; ++index)
      {
        if (this.DefDic.ContainsKey(this.Definitions[index].Category))
        {
          this.DefDic[this.Definitions[index].Category].Add(this.Definitions[index]);
        }
        else
        {
          List<ItemSpawnerObjectDefinition> objectDefinitionList = new List<ItemSpawnerObjectDefinition>();
          this.DefDic.Add(this.Definitions[index].Category, objectDefinitionList);
          this.DefDic[this.Definitions[index].Category].Add(this.Definitions[index]);
          this.CurrentItemIndex.Add(this.Definitions[index].Category, 0);
        }
      }
    }

    private void UpdateDisplay()
    {
      if (this.CurrentCategory == ItemSpawnerObjectDefinition.ItemSpawnerCategory.None)
      {
        this.ItemName.text = "Please Select Category";
        this.ItemPic.sprite = (Sprite) null;
        this.ItemPic.gameObject.SetActive(false);
        this.ItemName.gameObject.SetActive(false);
      }
      else
      {
        this.ItemPic.sprite = this.DefDic[this.CurrentCategory][this.CurrentItemIndex[this.CurrentCategory]].Sprite;
        this.ItemName.text = this.DefDic[this.CurrentCategory][this.CurrentItemIndex[this.CurrentCategory]].DisplayName;
        this.ItemPic.gameObject.SetActive(true);
        this.ItemName.gameObject.SetActive(true);
      }
    }

    public void SetCategory(string cat)
    {
      this.CurrentCategory = (ItemSpawnerObjectDefinition.ItemSpawnerCategory) Enum.Parse(typeof (ItemSpawnerObjectDefinition.ItemSpawnerCategory), cat);
      this.UpdateDisplay();
    }

    public void SetCategory(
      ItemSpawnerObjectDefinition.ItemSpawnerCategory cat)
    {
      this.CurrentCategory = cat;
      this.UpdateDisplay();
    }

    public void NextItem(int i)
    {
      if (this.CurrentCategory != ItemSpawnerObjectDefinition.ItemSpawnerCategory.None)
      {
        int num1 = this.CurrentItemIndex[this.CurrentCategory];
        int count = this.DefDic[this.CurrentCategory].Count;
        int num2 = num1 + 1;
        if (num2 >= count)
          num2 -= count;
        this.CurrentItemIndex[this.CurrentCategory] = num2;
      }
      this.UpdateDisplay();
    }

    public void PrevItem(int i)
    {
      if (this.CurrentCategory != ItemSpawnerObjectDefinition.ItemSpawnerCategory.None)
      {
        int num1 = this.CurrentItemIndex[this.CurrentCategory];
        int count = this.DefDic[this.CurrentCategory].Count;
        int num2 = num1 - 1;
        if (num2 < 0)
          num2 += count;
        this.CurrentItemIndex[this.CurrentCategory] = num2;
      }
      this.UpdateDisplay();
    }

    public void SpawnItem()
    {
      if (this.CurrentCategory == ItemSpawnerObjectDefinition.ItemSpawnerCategory.None)
        return;
      UnityEngine.Object.Instantiate<GameObject>(this.DefDic[this.CurrentCategory][this.CurrentItemIndex[this.CurrentCategory]].Prefab, this.SpawnPoint.position, this.SpawnPoint.rotation);
      if (this.DefDic[this.CurrentCategory][this.CurrentItemIndex[this.CurrentCategory]].AdditionalPrefabs.Length <= 0)
        return;
      for (int index = 0; index < this.DefDic[this.CurrentCategory][this.CurrentItemIndex[this.CurrentCategory]].AdditionalPrefabs.Length; ++index)
      {
        Vector3 vector3 = new Vector3(0.0f, 0.2f * (float) (index + 1), 0.0f);
        UnityEngine.Object.Instantiate<GameObject>(this.DefDic[this.CurrentCategory][this.CurrentItemIndex[this.CurrentCategory]].AdditionalPrefabs[index], this.SpawnPoint.position + vector3, this.SpawnPoint.rotation);
      }
    }
  }
}
