using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class ObjectTable
	{
		public List<FVRObject> Objs = new List<FVRObject>();

		public int MinCapacity = -1;

		public int MaxCapacity = -1;

		public void Initialize(ObjectTableDef d)
		{
			Initialize(d, d.Category, d.Eras, d.Sets, d.Sizes, d.Actions, d.Modes, d.ExcludeModes, d.Feedoptions, d.MountsAvailable, d.RoundPowers, d.Features, d.MeleeStyles, d.MeleeHandedness, d.MountTypes, d.PowerupTypes, d.ThrownTypes, d.ThrownDamageTypes, d.MinAmmoCapacity, d.MaxAmmoCapacity, d.RequiredExactCapacity, d.IsBlanked);
		}

		public void Initialize(ObjectTableDef Def, FVRObject.ObjectCategory category, List<FVRObject.OTagEra> eras = null, List<FVRObject.OTagSet> sets = null, List<FVRObject.OTagFirearmSize> sizes = null, List<FVRObject.OTagFirearmAction> actions = null, List<FVRObject.OTagFirearmFiringMode> modes = null, List<FVRObject.OTagFirearmFiringMode> excludeModes = null, List<FVRObject.OTagFirearmFeedOption> feedoptions = null, List<FVRObject.OTagFirearmMount> mountsavailable = null, List<FVRObject.OTagFirearmRoundPower> roundPowers = null, List<FVRObject.OTagAttachmentFeature> features = null, List<FVRObject.OTagMeleeStyle> meleeStyles = null, List<FVRObject.OTagMeleeHandedness> meleeHandedness = null, List<FVRObject.OTagFirearmMount> mounttype = null, List<FVRObject.OTagPowerupType> powerupTypes = null, List<FVRObject.OTagThrownType> thrownTypes = null, List<FVRObject.OTagThrownDamageType> thrownDamageTypes = null, int minCapacity = -1, int maxCapacity = -1, int requiredExactCapacity = -1, bool isBlanked = false)
		{
			MinCapacity = minCapacity;
			MaxCapacity = maxCapacity;
			if (isBlanked)
			{
				return;
			}
			if (Def.UseIDListOverride)
			{
				for (int i = 0; i < Def.IDOverride.Count; i++)
				{
					Objs.Add(IM.OD[Def.IDOverride[i]]);
				}
				return;
			}
			Objs = new List<FVRObject>(ManagerSingleton<IM>.Instance.odicTagCategory[category]);
			for (int num = Objs.Count - 1; num >= 0; num--)
			{
				FVRObject fVRObject = Objs[num];
				if (!fVRObject.OSple)
				{
					Objs.RemoveAt(num);
					continue;
				}
				if (minCapacity > -1 && fVRObject.MaxCapacityRelated < minCapacity)
				{
					Objs.RemoveAt(num);
					continue;
				}
				if (maxCapacity > -1 && fVRObject.MinCapacityRelated > maxCapacity)
				{
					Objs.RemoveAt(num);
					continue;
				}
				if (requiredExactCapacity > -1 && !DoesGunMatchExactCapacity(fVRObject))
				{
					Objs.RemoveAt(num);
					continue;
				}
				if (eras != null && eras.Count > 0 && !eras.Contains(fVRObject.TagEra))
				{
					Objs.RemoveAt(num);
					continue;
				}
				if (sets != null && sets.Count > 0 && !sets.Contains(fVRObject.TagSet))
				{
					Objs.RemoveAt(num);
					continue;
				}
				if (sizes != null && sizes.Count > 0 && !sizes.Contains(fVRObject.TagFirearmSize))
				{
					Objs.RemoveAt(num);
					continue;
				}
				if (actions != null && actions.Count > 0 && !actions.Contains(fVRObject.TagFirearmAction))
				{
					Objs.RemoveAt(num);
					continue;
				}
				if (roundPowers != null && roundPowers.Count > 0 && !roundPowers.Contains(fVRObject.TagFirearmRoundPower))
				{
					Objs.RemoveAt(num);
					continue;
				}
				if (modes != null && modes.Count > 0)
				{
					bool flag = false;
					for (int j = 0; j < modes.Count; j++)
					{
						if (!fVRObject.TagFirearmFiringModes.Contains(modes[j]))
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						Objs.RemoveAt(num);
						continue;
					}
				}
				if (excludeModes != null)
				{
					bool flag2 = false;
					for (int k = 0; k < excludeModes.Count; k++)
					{
						if (fVRObject.TagFirearmFiringModes.Contains(excludeModes[k]))
						{
							flag2 = true;
							break;
						}
					}
					if (flag2)
					{
						Objs.RemoveAt(num);
						continue;
					}
				}
				if (feedoptions != null)
				{
					bool flag3 = false;
					for (int l = 0; l < feedoptions.Count; l++)
					{
						if (!fVRObject.TagFirearmFeedOption.Contains(feedoptions[l]))
						{
							flag3 = true;
							break;
						}
					}
					if (flag3)
					{
						Objs.RemoveAt(num);
						continue;
					}
				}
				if (mountsavailable != null)
				{
					bool flag4 = false;
					for (int m = 0; m < mountsavailable.Count; m++)
					{
						if (!fVRObject.TagFirearmMounts.Contains(mountsavailable[m]))
						{
							flag4 = true;
							break;
						}
					}
					if (flag4)
					{
						Objs.RemoveAt(num);
						continue;
					}
				}
				if (powerupTypes != null && powerupTypes.Count > 0 && !powerupTypes.Contains(fVRObject.TagPowerupType))
				{
					Objs.RemoveAt(num);
				}
				else if (thrownTypes != null && thrownTypes.Count > 0 && !thrownTypes.Contains(fVRObject.TagThrownType))
				{
					Objs.RemoveAt(num);
				}
				else if (thrownTypes != null && thrownTypes.Count > 0 && !thrownTypes.Contains(fVRObject.TagThrownType))
				{
					Objs.RemoveAt(num);
				}
				else if (meleeStyles != null && meleeStyles.Count > 0 && !meleeStyles.Contains(fVRObject.TagMeleeStyle))
				{
					Objs.RemoveAt(num);
				}
				else if (meleeHandedness != null && meleeHandedness.Count > 0 && !meleeHandedness.Contains(fVRObject.TagMeleeHandedness))
				{
					Objs.RemoveAt(num);
				}
				else if (mounttype != null && mounttype.Count > 0 && !mounttype.Contains(fVRObject.TagAttachmentMount))
				{
					Objs.RemoveAt(num);
				}
				else if (features != null && features.Count > 0 && !features.Contains(fVRObject.TagAttachmentFeature))
				{
					Objs.RemoveAt(num);
				}
			}
		}

		public FVRObject GetRandomObject()
		{
			if (Objs.Count > 0)
			{
				return Objs[UnityEngine.Random.Range(0, Objs.Count)];
			}
			return null;
		}

		private bool DoesGunMatchExactCapacity(FVRObject o)
		{
			return true;
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
