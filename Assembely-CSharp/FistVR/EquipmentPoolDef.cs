using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Equipment Pool Def", menuName = "EquipmentPoolDef", order = 0)]
	public class EquipmentPoolDef : ScriptableObject
	{
		[Serializable]
		public class PoolEntry
		{
			public enum PoolEntryType
			{
				Firearm,
				Equipment,
				Consumable
			}

			public ObjectTableDef TableDef;

			public PoolEntryType Type;

			public int TokenCost = 1;

			public int TokenCost_Limited = 1;

			public int MinLevelAppears = 1;

			public int MaxLevelAppears = 2;

			public float Rarity = 1f;

			public int GetCost(bool limited)
			{
				if (limited)
				{
					return TokenCost_Limited;
				}
				return TokenCost;
			}

			public int GetCost(TNHSetting_EquipmentMode m)
			{
				if (m == TNHSetting_EquipmentMode.LimitedAmmo)
				{
					return TokenCost_Limited;
				}
				return TokenCost;
			}
		}

		public List<PoolEntry> Entries = new List<PoolEntry>();

		public int GetNumEntries(PoolEntry.PoolEntryType pType, int atLevel)
		{
			int num = 0;
			for (int i = 0; i < Entries.Count; i++)
			{
				if (Entries[i].Type == pType && Entries[i].MinLevelAppears <= atLevel && Entries[i].MaxLevelAppears >= atLevel)
				{
					num++;
				}
			}
			return num;
		}
	}
}
