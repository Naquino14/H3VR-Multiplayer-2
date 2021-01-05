// Decompiled with JetBrains decompiler
// Type: FistVR.ObjectTable
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

namespace FistVR
{
  [Serializable]
  public class ObjectTable
  {
    public List<FVRObject> Objs = new List<FVRObject>();
    public int MinCapacity = -1;
    public int MaxCapacity = -1;

    public void Initialize(ObjectTableDef d) => this.Initialize(d, d.Category, d.Eras, d.Sets, d.Sizes, d.Actions, d.Modes, d.ExcludeModes, d.Feedoptions, d.MountsAvailable, d.RoundPowers, d.Features, d.MeleeStyles, d.MeleeHandedness, d.MountTypes, d.PowerupTypes, d.ThrownTypes, d.ThrownDamageTypes, d.MinAmmoCapacity, d.MaxAmmoCapacity, d.RequiredExactCapacity, d.IsBlanked);

    public void Initialize(
      ObjectTableDef Def,
      FVRObject.ObjectCategory category,
      List<FVRObject.OTagEra> eras = null,
      List<FVRObject.OTagSet> sets = null,
      List<FVRObject.OTagFirearmSize> sizes = null,
      List<FVRObject.OTagFirearmAction> actions = null,
      List<FVRObject.OTagFirearmFiringMode> modes = null,
      List<FVRObject.OTagFirearmFiringMode> excludeModes = null,
      List<FVRObject.OTagFirearmFeedOption> feedoptions = null,
      List<FVRObject.OTagFirearmMount> mountsavailable = null,
      List<FVRObject.OTagFirearmRoundPower> roundPowers = null,
      List<FVRObject.OTagAttachmentFeature> features = null,
      List<FVRObject.OTagMeleeStyle> meleeStyles = null,
      List<FVRObject.OTagMeleeHandedness> meleeHandedness = null,
      List<FVRObject.OTagFirearmMount> mounttype = null,
      List<FVRObject.OTagPowerupType> powerupTypes = null,
      List<FVRObject.OTagThrownType> thrownTypes = null,
      List<FVRObject.OTagThrownDamageType> thrownDamageTypes = null,
      int minCapacity = -1,
      int maxCapacity = -1,
      int requiredExactCapacity = -1,
      bool isBlanked = false)
    {
      this.MinCapacity = minCapacity;
      this.MaxCapacity = maxCapacity;
      if (isBlanked)
        return;
      if (Def.UseIDListOverride)
      {
        for (int index = 0; index < Def.IDOverride.Count; ++index)
          this.Objs.Add(IM.OD[Def.IDOverride[index]]);
      }
      else
      {
        this.Objs = new List<FVRObject>((IEnumerable<FVRObject>) ManagerSingleton<IM>.Instance.odicTagCategory[category]);
        for (int index1 = this.Objs.Count - 1; index1 >= 0; --index1)
        {
          FVRObject o = this.Objs[index1];
          if (!o.OSple)
            this.Objs.RemoveAt(index1);
          else if (minCapacity > -1 && o.MaxCapacityRelated < minCapacity)
            this.Objs.RemoveAt(index1);
          else if (maxCapacity > -1 && o.MinCapacityRelated > maxCapacity)
            this.Objs.RemoveAt(index1);
          else if (requiredExactCapacity > -1 && !this.DoesGunMatchExactCapacity(o))
            this.Objs.RemoveAt(index1);
          else if (eras != null && eras.Count > 0 && !eras.Contains(o.TagEra))
            this.Objs.RemoveAt(index1);
          else if (sets != null && sets.Count > 0 && !sets.Contains(o.TagSet))
            this.Objs.RemoveAt(index1);
          else if (sizes != null && sizes.Count > 0 && !sizes.Contains(o.TagFirearmSize))
            this.Objs.RemoveAt(index1);
          else if (actions != null && actions.Count > 0 && !actions.Contains(o.TagFirearmAction))
            this.Objs.RemoveAt(index1);
          else if (roundPowers != null && roundPowers.Count > 0 && !roundPowers.Contains(o.TagFirearmRoundPower))
          {
            this.Objs.RemoveAt(index1);
          }
          else
          {
            if (modes != null && modes.Count > 0)
            {
              bool flag = false;
              for (int index2 = 0; index2 < modes.Count; ++index2)
              {
                if (!o.TagFirearmFiringModes.Contains(modes[index2]))
                {
                  flag = true;
                  break;
                }
              }
              if (flag)
              {
                this.Objs.RemoveAt(index1);
                continue;
              }
            }
            if (excludeModes != null)
            {
              bool flag = false;
              for (int index2 = 0; index2 < excludeModes.Count; ++index2)
              {
                if (o.TagFirearmFiringModes.Contains(excludeModes[index2]))
                {
                  flag = true;
                  break;
                }
              }
              if (flag)
              {
                this.Objs.RemoveAt(index1);
                continue;
              }
            }
            if (feedoptions != null)
            {
              bool flag = false;
              for (int index2 = 0; index2 < feedoptions.Count; ++index2)
              {
                if (!o.TagFirearmFeedOption.Contains(feedoptions[index2]))
                {
                  flag = true;
                  break;
                }
              }
              if (flag)
              {
                this.Objs.RemoveAt(index1);
                continue;
              }
            }
            if (mountsavailable != null)
            {
              bool flag = false;
              for (int index2 = 0; index2 < mountsavailable.Count; ++index2)
              {
                if (!o.TagFirearmMounts.Contains(mountsavailable[index2]))
                {
                  flag = true;
                  break;
                }
              }
              if (flag)
              {
                this.Objs.RemoveAt(index1);
                continue;
              }
            }
            if (powerupTypes != null && powerupTypes.Count > 0 && !powerupTypes.Contains(o.TagPowerupType))
              this.Objs.RemoveAt(index1);
            else if (thrownTypes != null && thrownTypes.Count > 0 && !thrownTypes.Contains(o.TagThrownType))
              this.Objs.RemoveAt(index1);
            else if (thrownTypes != null && thrownTypes.Count > 0 && !thrownTypes.Contains(o.TagThrownType))
              this.Objs.RemoveAt(index1);
            else if (meleeStyles != null && meleeStyles.Count > 0 && !meleeStyles.Contains(o.TagMeleeStyle))
              this.Objs.RemoveAt(index1);
            else if (meleeHandedness != null && meleeHandedness.Count > 0 && !meleeHandedness.Contains(o.TagMeleeHandedness))
              this.Objs.RemoveAt(index1);
            else if (mounttype != null && mounttype.Count > 0 && !mounttype.Contains(o.TagAttachmentMount))
              this.Objs.RemoveAt(index1);
            else if (features != null && features.Count > 0 && !features.Contains(o.TagAttachmentFeature))
              this.Objs.RemoveAt(index1);
          }
        }
      }
    }

    public FVRObject GetRandomObject() => this.Objs.Count > 0 ? this.Objs[UnityEngine.Random.Range(0, this.Objs.Count)] : (FVRObject) null;

    private bool DoesGunMatchExactCapacity(FVRObject o) => true;

    public FVRObject GetRandomBespokeAttachment(FVRObject o) => o.BespokeAttachments.Count > 0 ? o.BespokeAttachments[UnityEngine.Random.Range(0, o.BespokeAttachments.Count)] : (FVRObject) null;
  }
}
