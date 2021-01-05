using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Object Table", menuName = "ObjectTableDef", order = 0)]
	public class ObjectTableDef : ScriptableObject
	{
		public Sprite Icon;

		public FVRObject.ObjectCategory Category;

		public List<FVRObject.OTagEra> Eras;

		public List<FVRObject.OTagSet> Sets;

		public List<FVRObject.OTagFirearmSize> Sizes;

		public List<FVRObject.OTagFirearmAction> Actions;

		public List<FVRObject.OTagFirearmFiringMode> Modes;

		public List<FVRObject.OTagFirearmFiringMode> ExcludeModes;

		public List<FVRObject.OTagFirearmFeedOption> Feedoptions;

		public List<FVRObject.OTagFirearmMount> MountsAvailable;

		public List<FVRObject.OTagFirearmRoundPower> RoundPowers;

		public List<FVRObject.OTagAttachmentFeature> Features;

		public List<FVRObject.OTagMeleeStyle> MeleeStyles;

		public List<FVRObject.OTagMeleeHandedness> MeleeHandedness;

		public List<FVRObject.OTagFirearmMount> MountTypes;

		public List<FVRObject.OTagPowerupType> PowerupTypes;

		public List<FVRObject.OTagThrownType> ThrownTypes;

		public List<FVRObject.OTagThrownDamageType> ThrownDamageTypes;

		public int MinAmmoCapacity = -1;

		public int MaxAmmoCapacity = -1;

		public int RequiredExactCapacity = -1;

		public bool IsBlanked;

		public bool SpawnsInSmallCase;

		public bool SpawnsInLargeCase;

		public bool UseIDListOverride;

		public List<string> IDOverride;

		public List<FVRObject> Objs;

		[ContextMenu("LoadObjs")]
		public void LoadObjs()
		{
			for (int i = 0; i < Objs.Count; i++)
			{
				IDOverride.Add(Objs[i].ItemID);
			}
			Objs.Clear();
		}

		[ContextMenu("TestTable")]
		public void TestTable()
		{
			ObjectTable objectTable = new ObjectTable();
			objectTable.Initialize(this, Category, Eras, Sets, Sizes, Actions, Modes, ExcludeModes, Feedoptions, MountsAvailable, RoundPowers, Features, MeleeStyles, MeleeHandedness, MountTypes, PowerupTypes, ThrownTypes, ThrownDamageTypes, MinAmmoCapacity, MaxAmmoCapacity, RequiredExactCapacity, IsBlanked);
			Debug.Log("----Listing Objects----");
			for (int i = 0; i < objectTable.Objs.Count; i++)
			{
				Debug.Log("Object " + i + " is " + objectTable.Objs[i].DisplayName);
			}
			Debug.Log("Table Created and has " + objectTable.Objs.Count + " in it.");
		}
	}
}
