using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New TNH_CharacterDef", menuName = "CharacterDef", order = 0)]
	public class TNH_CharacterDef : ScriptableObject
	{
		[Serializable]
		public class LoadoutEntry
		{
			public List<ObjectTableDef> TableDefs;

			public List<FVRObject> ListOverride;

			public int Num_Mags_SL_Clips;

			public int Num_Rounds;

			public FVRObject AmmoObjectOverride;
		}

		public enum CharacterGroup
		{
			DaringDefaults,
			WienersThroughTime,
			MemetasticMeats
		}

		[Header("MainParams")]
		public string DisplayName;

		public TNH_Char CharacterID;

		public CharacterGroup Group;

		public string TableID;

		public Sprite Picture;

		public int StartingTokens;

		public bool ForceAllAgentWeapons;

		[Multiline(6)]
		public string Description;

		public EquipmentPoolDef EquipmentPool;

		public ObjectTableDef RequireSightTable;

		public List<FVRObject.OTagEra> ValidAmmoEras;

		public List<FVRObject.OTagSet> ValidAmmoSets;

		public bool UsesPurchasePriceIncrement = true;

		[Header("Progressions")]
		public List<TNH_Progression> Progressions;

		public List<TNH_Progression> Progressions_Endless;

		[Header("Loadout")]
		public bool Has_Weapon_Primary;

		public bool Has_Weapon_Secondary;

		public bool Has_Weapon_Tertiary;

		public bool Has_Item_Primary;

		public bool Has_Item_Secondary;

		public bool Has_Item_Tertiary;

		public bool Has_Item_Shield;

		[Space(10f)]
		public LoadoutEntry Weapon_Primary;

		[Space(10f)]
		public LoadoutEntry Weapon_Secondary;

		[Space(10f)]
		public LoadoutEntry Weapon_Tertiary;

		[Space(10f)]
		public LoadoutEntry Item_Primary;

		[Space(10f)]
		public LoadoutEntry Item_Secondary;

		[Space(10f)]
		public LoadoutEntry Item_Tertiary;

		[Space(10f)]
		public LoadoutEntry Item_Shield;
	}
}
