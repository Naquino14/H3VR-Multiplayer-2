using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class LootTable
	{
		public enum LootTableType
		{
			Firearm,
			Powerup,
			Thrown,
			Attachments,
			Melee
		}

		public List<FVRObject> Loot = new List<FVRObject>();

		public int MinCapacity = -1;

		public int MaxCapacity = -1;

		public List<FVRObject.OTagEra> Eras;

		public void Initialize(LootTableType type, List<FVRObject.OTagEra> eras = null, List<FVRObject.OTagFirearmSize> sizes = null, List<FVRObject.OTagFirearmAction> actions = null, List<FVRObject.OTagFirearmFiringMode> modes = null, List<FVRObject.OTagFirearmFiringMode> excludeModes = null, List<FVRObject.OTagFirearmFeedOption> feedoptions = null, List<FVRObject.OTagFirearmMount> mounts = null, List<FVRObject.OTagFirearmRoundPower> roundPowers = null, List<FVRObject.OTagAttachmentFeature> features = null, List<FVRObject.OTagMeleeStyle> meleeStyles = null, List<FVRObject.OTagMeleeHandedness> meleeHandedness = null, List<FVRObject.OTagPowerupType> powerupTypes = null, List<FVRObject.OTagThrownType> thrownTypes = null, int minCapacity = -1, int maxCapacity = -1)
		{
			MinCapacity = minCapacity;
			MaxCapacity = maxCapacity;
			Eras = eras;
			switch (type)
			{
			case LootTableType.Firearm:
				InitializeFirearmTable(eras, sizes, actions, modes, excludeModes, feedoptions, mounts, roundPowers, features, meleeStyles, meleeHandedness, minCapacity, maxCapacity);
				break;
			case LootTableType.Powerup:
				InitializePowerupTable(powerupTypes);
				break;
			case LootTableType.Thrown:
				InitializeThrownTable(eras, thrownTypes);
				break;
			case LootTableType.Melee:
				InitializeMeleeTable(eras, meleeStyles, meleeHandedness);
				break;
			case LootTableType.Attachments:
				InitializeAttachmentTable(eras, mounts, features);
				break;
			}
		}

		private void InitializeFirearmTable(List<FVRObject.OTagEra> eras = null, List<FVRObject.OTagFirearmSize> sizes = null, List<FVRObject.OTagFirearmAction> actions = null, List<FVRObject.OTagFirearmFiringMode> modes = null, List<FVRObject.OTagFirearmFiringMode> excludeModes = null, List<FVRObject.OTagFirearmFeedOption> feedoptions = null, List<FVRObject.OTagFirearmMount> mounts = null, List<FVRObject.OTagFirearmRoundPower> roundPowers = null, List<FVRObject.OTagAttachmentFeature> features = null, List<FVRObject.OTagMeleeStyle> meleeStyles = null, List<FVRObject.OTagMeleeHandedness> meleeHandedness = null, int minCapacity = -1, int maxCapacity = -1)
		{
			Loot = new List<FVRObject>(ManagerSingleton<IM>.Instance.odicTagCategory[FVRObject.ObjectCategory.Firearm]);
			for (int num = Loot.Count - 1; num >= 0; num--)
			{
				FVRObject fVRObject = Loot[num];
				if (!fVRObject.OSple)
				{
					Loot.RemoveAt(num);
					continue;
				}
				if (minCapacity > -1 && fVRObject.MaxCapacityRelated < minCapacity)
				{
					Loot.RemoveAt(num);
					continue;
				}
				if (maxCapacity > -1 && fVRObject.MinCapacityRelated > maxCapacity)
				{
					Loot.RemoveAt(num);
					continue;
				}
				if (eras != null && !eras.Contains(fVRObject.TagEra))
				{
					Loot.RemoveAt(num);
					continue;
				}
				if (sizes != null && !sizes.Contains(fVRObject.TagFirearmSize))
				{
					Loot.RemoveAt(num);
					continue;
				}
				if (actions != null && !actions.Contains(fVRObject.TagFirearmAction))
				{
					Loot.RemoveAt(num);
					continue;
				}
				if (roundPowers != null && !roundPowers.Contains(fVRObject.TagFirearmRoundPower))
				{
					Loot.RemoveAt(num);
					continue;
				}
				if (modes != null)
				{
					bool flag = false;
					for (int i = 0; i < modes.Count; i++)
					{
						if (!fVRObject.TagFirearmFiringModes.Contains(modes[i]))
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						Loot.RemoveAt(num);
						continue;
					}
				}
				if (excludeModes != null)
				{
					bool flag2 = false;
					for (int j = 0; j < excludeModes.Count; j++)
					{
						if (fVRObject.TagFirearmFiringModes.Contains(excludeModes[j]))
						{
							flag2 = true;
							break;
						}
					}
					if (flag2)
					{
						Loot.RemoveAt(num);
						continue;
					}
				}
				if (feedoptions != null)
				{
					bool flag3 = false;
					for (int k = 0; k < feedoptions.Count; k++)
					{
						if (!fVRObject.TagFirearmFeedOption.Contains(feedoptions[k]))
						{
							flag3 = true;
							break;
						}
					}
					if (flag3)
					{
						Loot.RemoveAt(num);
						continue;
					}
				}
				if (mounts == null)
				{
					continue;
				}
				bool flag4 = false;
				for (int l = 0; l < mounts.Count; l++)
				{
					if (!fVRObject.TagFirearmMounts.Contains(mounts[l]))
					{
						flag4 = true;
						break;
					}
				}
				if (flag4)
				{
					Loot.RemoveAt(num);
				}
			}
		}

		private void InitializePowerupTable(List<FVRObject.OTagPowerupType> powerupTypes = null)
		{
			Loot = new List<FVRObject>(ManagerSingleton<IM>.Instance.odicTagCategory[FVRObject.ObjectCategory.Powerup]);
			for (int num = Loot.Count - 1; num >= 0; num--)
			{
				FVRObject fVRObject = Loot[num];
				if (powerupTypes != null && !powerupTypes.Contains(fVRObject.TagPowerupType))
				{
					Loot.RemoveAt(num);
				}
			}
		}

		private void InitializeThrownTable(List<FVRObject.OTagEra> eras = null, List<FVRObject.OTagThrownType> thrownTypes = null)
		{
			Loot = new List<FVRObject>(ManagerSingleton<IM>.Instance.odicTagCategory[FVRObject.ObjectCategory.Thrown]);
			for (int num = Loot.Count - 1; num >= 0; num--)
			{
				FVRObject fVRObject = Loot[num];
				if (thrownTypes != null && !thrownTypes.Contains(fVRObject.TagThrownType))
				{
					Loot.RemoveAt(num);
				}
				else if (eras != null && !eras.Contains(fVRObject.TagEra))
				{
					Loot.RemoveAt(num);
				}
			}
		}

		private void InitializeMeleeTable(List<FVRObject.OTagEra> eras = null, List<FVRObject.OTagMeleeStyle> meleeStyles = null, List<FVRObject.OTagMeleeHandedness> meleeHandedness = null)
		{
			Loot = new List<FVRObject>(ManagerSingleton<IM>.Instance.odicTagCategory[FVRObject.ObjectCategory.MeleeWeapon]);
			for (int num = Loot.Count - 1; num >= 0; num--)
			{
				FVRObject fVRObject = Loot[num];
				if (eras != null && !eras.Contains(fVRObject.TagEra))
				{
					Loot.RemoveAt(num);
				}
				else if (meleeStyles != null && !meleeStyles.Contains(fVRObject.TagMeleeStyle))
				{
					Loot.RemoveAt(num);
				}
				else if (meleeHandedness != null && !meleeHandedness.Contains(fVRObject.TagMeleeHandedness))
				{
					Loot.RemoveAt(num);
				}
			}
		}

		private void InitializeAttachmentTable(List<FVRObject.OTagEra> eras = null, List<FVRObject.OTagFirearmMount> mounts = null, List<FVRObject.OTagAttachmentFeature> features = null)
		{
			Loot = new List<FVRObject>(ManagerSingleton<IM>.Instance.odicTagCategory[FVRObject.ObjectCategory.Attachment]);
			for (int num = Loot.Count - 1; num >= 0; num--)
			{
				FVRObject fVRObject = Loot[num];
				if (eras != null && !eras.Contains(fVRObject.TagEra))
				{
					Loot.RemoveAt(num);
				}
				else if (mounts != null && !mounts.Contains(fVRObject.TagAttachmentMount))
				{
					Loot.RemoveAt(num);
				}
				else if (features != null && !features.Contains(fVRObject.TagAttachmentFeature))
				{
					Loot.RemoveAt(num);
				}
			}
		}

		public FVRObject GetRandomObject()
		{
			if (Loot.Count > 0)
			{
				return Loot[UnityEngine.Random.Range(0, Loot.Count)];
			}
			return null;
		}

		public FVRObject GetRandomAmmoObject(FVRObject o, List<FVRObject.OTagEra> eras = null)
		{
			if (o.CompatibleMagazines.Count > 0)
			{
				List<FVRObject> list = new List<FVRObject>();
				for (int i = 0; i < o.CompatibleMagazines.Count; i++)
				{
					if ((o.CompatibleMagazines[i].MagazineCapacity <= MaxCapacity || MaxCapacity == -1) && (o.CompatibleMagazines[i].MagazineCapacity >= MinCapacity || MinCapacity == -1))
					{
						list.Add(o.CompatibleMagazines[i]);
					}
				}
				if (list.Count > 0)
				{
					return list[UnityEngine.Random.Range(0, list.Count)];
				}
				return o.CompatibleMagazines[0];
			}
			if (o.CompatibleClips.Count > 0)
			{
				return o.CompatibleClips[UnityEngine.Random.Range(0, o.CompatibleClips.Count)];
			}
			if (o.CompatibleSpeedLoaders.Count > 0)
			{
				return o.CompatibleSpeedLoaders[UnityEngine.Random.Range(0, o.CompatibleSpeedLoaders.Count)];
			}
			if (o.CompatibleSingleRounds.Count > 0)
			{
				if (eras == null)
				{
					return o.CompatibleSingleRounds[UnityEngine.Random.Range(0, o.CompatibleSingleRounds.Count)];
				}
				List<FVRObject> list2 = new List<FVRObject>();
				for (int j = 0; j < o.CompatibleSingleRounds.Count; j++)
				{
					if (eras.Contains(o.CompatibleSingleRounds[j].TagEra))
					{
						list2.Add(o.CompatibleSingleRounds[j]);
					}
				}
				if (list2.Count > 0)
				{
					FVRObject result = list2[UnityEngine.Random.Range(0, list2.Count)];
					list2.Clear();
					return result;
				}
				return o.CompatibleSingleRounds[0];
			}
			return null;
		}

		public FVRObject GetRandomBespokeAttachment(FVRObject o)
		{
			if (o.BespokeAttachments.Count > 0)
			{
				return o.BespokeAttachments[UnityEngine.Random.Range(0, o.BespokeAttachments.Count)];
			}
			return null;
		}
	}
}
