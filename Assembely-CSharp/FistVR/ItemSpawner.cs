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
			PrimeDefs();
			UpdateDisplay();
		}

		private void PrimeDefs()
		{
			UnityEngine.Object[] array = Resources.LoadAll("ItemSpawnerDefinitions", typeof(ItemSpawnerObjectDefinition));
			UnityEngine.Object[] array2 = array;
			foreach (UnityEngine.Object @object in array2)
			{
				Definitions.Add((ItemSpawnerObjectDefinition)@object);
			}
			for (int j = 0; j < Definitions.Count; j++)
			{
				if (DefDic.ContainsKey(Definitions[j].Category))
				{
					DefDic[Definitions[j].Category].Add(Definitions[j]);
					continue;
				}
				List<ItemSpawnerObjectDefinition> value = new List<ItemSpawnerObjectDefinition>();
				DefDic.Add(Definitions[j].Category, value);
				DefDic[Definitions[j].Category].Add(Definitions[j]);
				CurrentItemIndex.Add(Definitions[j].Category, 0);
			}
		}

		private void UpdateDisplay()
		{
			if (CurrentCategory == ItemSpawnerObjectDefinition.ItemSpawnerCategory.None)
			{
				ItemName.text = "Please Select Category";
				ItemPic.sprite = null;
				ItemPic.gameObject.SetActive(value: false);
				ItemName.gameObject.SetActive(value: false);
			}
			else
			{
				ItemPic.sprite = DefDic[CurrentCategory][CurrentItemIndex[CurrentCategory]].Sprite;
				ItemName.text = DefDic[CurrentCategory][CurrentItemIndex[CurrentCategory]].DisplayName;
				ItemPic.gameObject.SetActive(value: true);
				ItemName.gameObject.SetActive(value: true);
			}
		}

		public void SetCategory(string cat)
		{
			CurrentCategory = (ItemSpawnerObjectDefinition.ItemSpawnerCategory)Enum.Parse(typeof(ItemSpawnerObjectDefinition.ItemSpawnerCategory), cat);
			UpdateDisplay();
		}

		public void SetCategory(ItemSpawnerObjectDefinition.ItemSpawnerCategory cat)
		{
			CurrentCategory = cat;
			UpdateDisplay();
		}

		public void NextItem(int i)
		{
			if (CurrentCategory != ItemSpawnerObjectDefinition.ItemSpawnerCategory.None)
			{
				int num = CurrentItemIndex[CurrentCategory];
				int count = DefDic[CurrentCategory].Count;
				num++;
				if (num >= count)
				{
					num -= count;
				}
				CurrentItemIndex[CurrentCategory] = num;
			}
			UpdateDisplay();
		}

		public void PrevItem(int i)
		{
			if (CurrentCategory != ItemSpawnerObjectDefinition.ItemSpawnerCategory.None)
			{
				int num = CurrentItemIndex[CurrentCategory];
				int count = DefDic[CurrentCategory].Count;
				num--;
				if (num < 0)
				{
					num += count;
				}
				CurrentItemIndex[CurrentCategory] = num;
			}
			UpdateDisplay();
		}

		public void SpawnItem()
		{
			if (CurrentCategory == ItemSpawnerObjectDefinition.ItemSpawnerCategory.None)
			{
				return;
			}
			UnityEngine.Object.Instantiate(DefDic[CurrentCategory][CurrentItemIndex[CurrentCategory]].Prefab, SpawnPoint.position, SpawnPoint.rotation);
			if (DefDic[CurrentCategory][CurrentItemIndex[CurrentCategory]].AdditionalPrefabs.Length > 0)
			{
				for (int i = 0; i < DefDic[CurrentCategory][CurrentItemIndex[CurrentCategory]].AdditionalPrefabs.Length; i++)
				{
					Vector3 vector = new Vector3(0f, 0.2f * (float)(i + 1), 0f);
					UnityEngine.Object.Instantiate(DefDic[CurrentCategory][CurrentItemIndex[CurrentCategory]].AdditionalPrefabs[i], SpawnPoint.position + vector, SpawnPoint.rotation);
				}
			}
		}
	}
}
