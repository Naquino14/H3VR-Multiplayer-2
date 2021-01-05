// Decompiled with JetBrains decompiler
// Type: FistVR.LootTable
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

namespace FistVR
{
  [Serializable]
  public class LootTable
  {
    public List<FVRObject> Loot = new List<FVRObject>();
    public int MinCapacity = -1;
    public int MaxCapacity = -1;
    public List<FVRObject.OTagEra> Eras;

    public void Initialize(
      LootTable.LootTableType type,
      List<FVRObject.OTagEra> eras = null,
      List<FVRObject.OTagFirearmSize> sizes = null,
      List<FVRObject.OTagFirearmAction> actions = null,
      List<FVRObject.OTagFirearmFiringMode> modes = null,
      List<FVRObject.OTagFirearmFiringMode> excludeModes = null,
      List<FVRObject.OTagFirearmFeedOption> feedoptions = null,
      List<FVRObject.OTagFirearmMount> mounts = null,
      List<FVRObject.OTagFirearmRoundPower> roundPowers = null,
      List<FVRObject.OTagAttachmentFeature> features = null,
      List<FVRObject.OTagMeleeStyle> meleeStyles = null,
      List<FVRObject.OTagMeleeHandedness> meleeHandedness = null,
      List<FVRObject.OTagPowerupType> powerupTypes = null,
      List<FVRObject.OTagThrownType> thrownTypes = null,
      int minCapacity = -1,
      int maxCapacity = -1)
    {
      this.MinCapacity = minCapacity;
      this.MaxCapacity = maxCapacity;
      this.Eras = eras;
      switch (type)
      {
        case LootTable.LootTableType.Firearm:
          this.InitializeFirearmTable(eras, sizes, actions, modes, excludeModes, feedoptions, mounts, roundPowers, features, meleeStyles, meleeHandedness, minCapacity, maxCapacity);
          break;
        case LootTable.LootTableType.Powerup:
          this.InitializePowerupTable(powerupTypes);
          break;
        case LootTable.LootTableType.Thrown:
          this.InitializeThrownTable(eras, thrownTypes);
          break;
        case LootTable.LootTableType.Attachments:
          this.InitializeAttachmentTable(eras, mounts, features);
          break;
        case LootTable.LootTableType.Melee:
          this.InitializeMeleeTable(eras, meleeStyles, meleeHandedness);
          break;
      }
    }

    private void InitializeFirearmTable(
      List<FVRObject.OTagEra> eras = null,
      List<FVRObject.OTagFirearmSize> sizes = null,
      List<FVRObject.OTagFirearmAction> actions = null,
      List<FVRObject.OTagFirearmFiringMode> modes = null,
      List<FVRObject.OTagFirearmFiringMode> excludeModes = null,
      List<FVRObject.OTagFirearmFeedOption> feedoptions = null,
      List<FVRObject.OTagFirearmMount> mounts = null,
      List<FVRObject.OTagFirearmRoundPower> roundPowers = null,
      List<FVRObject.OTagAttachmentFeature> features = null,
      List<FVRObject.OTagMeleeStyle> meleeStyles = null,
      List<FVRObject.OTagMeleeHandedness> meleeHandedness = null,
      int minCapacity = -1,
      int maxCapacity = -1)
    {
      this.Loot = new List<FVRObject>((IEnumerable<FVRObject>) ManagerSingleton<IM>.Instance.odicTagCategory[FVRObject.ObjectCategory.Firearm]);
      for (int index1 = this.Loot.Count - 1; index1 >= 0; --index1)
      {
        FVRObject fvrObject = this.Loot[index1];
        if (!fvrObject.OSple)
          this.Loot.RemoveAt(index1);
        else if (minCapacity > -1 && fvrObject.MaxCapacityRelated < minCapacity)
          this.Loot.RemoveAt(index1);
        else if (maxCapacity > -1 && fvrObject.MinCapacityRelated > maxCapacity)
          this.Loot.RemoveAt(index1);
        else if (eras != null && !eras.Contains(fvrObject.TagEra))
          this.Loot.RemoveAt(index1);
        else if (sizes != null && !sizes.Contains(fvrObject.TagFirearmSize))
          this.Loot.RemoveAt(index1);
        else if (actions != null && !actions.Contains(fvrObject.TagFirearmAction))
          this.Loot.RemoveAt(index1);
        else if (roundPowers != null && !roundPowers.Contains(fvrObject.TagFirearmRoundPower))
        {
          this.Loot.RemoveAt(index1);
        }
        else
        {
          if (modes != null)
          {
            bool flag = false;
            for (int index2 = 0; index2 < modes.Count; ++index2)
            {
              if (!fvrObject.TagFirearmFiringModes.Contains(modes[index2]))
              {
                flag = true;
                break;
              }
            }
            if (flag)
            {
              this.Loot.RemoveAt(index1);
              continue;
            }
          }
          if (excludeModes != null)
          {
            bool flag = false;
            for (int index2 = 0; index2 < excludeModes.Count; ++index2)
            {
              if (fvrObject.TagFirearmFiringModes.Contains(excludeModes[index2]))
              {
                flag = true;
                break;
              }
            }
            if (flag)
            {
              this.Loot.RemoveAt(index1);
              continue;
            }
          }
          if (feedoptions != null)
          {
            bool flag = false;
            for (int index2 = 0; index2 < feedoptions.Count; ++index2)
            {
              if (!fvrObject.TagFirearmFeedOption.Contains(feedoptions[index2]))
              {
                flag = true;
                break;
              }
            }
            if (flag)
            {
              this.Loot.RemoveAt(index1);
              continue;
            }
          }
          if (mounts != null)
          {
            bool flag = false;
            for (int index2 = 0; index2 < mounts.Count; ++index2)
            {
              if (!fvrObject.TagFirearmMounts.Contains(mounts[index2]))
              {
                flag = true;
                break;
              }
            }
            if (flag)
              this.Loot.RemoveAt(index1);
          }
        }
      }
    }

    private void InitializePowerupTable(List<FVRObject.OTagPowerupType> powerupTypes = null)
    {
      this.Loot = new List<FVRObject>((IEnumerable<FVRObject>) ManagerSingleton<IM>.Instance.odicTagCategory[FVRObject.ObjectCategory.Powerup]);
      for (int index = this.Loot.Count - 1; index >= 0; --index)
      {
        FVRObject fvrObject = this.Loot[index];
        if (powerupTypes != null && !powerupTypes.Contains(fvrObject.TagPowerupType))
          this.Loot.RemoveAt(index);
      }
    }

    private void InitializeThrownTable(
      List<FVRObject.OTagEra> eras = null,
      List<FVRObject.OTagThrownType> thrownTypes = null)
    {
      this.Loot = new List<FVRObject>((IEnumerable<FVRObject>) ManagerSingleton<IM>.Instance.odicTagCategory[FVRObject.ObjectCategory.Thrown]);
      for (int index = this.Loot.Count - 1; index >= 0; --index)
      {
        FVRObject fvrObject = this.Loot[index];
        if (thrownTypes != null && !thrownTypes.Contains(fvrObject.TagThrownType))
          this.Loot.RemoveAt(index);
        else if (eras != null && !eras.Contains(fvrObject.TagEra))
          this.Loot.RemoveAt(index);
      }
    }

    private void InitializeMeleeTable(
      List<FVRObject.OTagEra> eras = null,
      List<FVRObject.OTagMeleeStyle> meleeStyles = null,
      List<FVRObject.OTagMeleeHandedness> meleeHandedness = null)
    {
      this.Loot = new List<FVRObject>((IEnumerable<FVRObject>) ManagerSingleton<IM>.Instance.odicTagCategory[FVRObject.ObjectCategory.MeleeWeapon]);
      for (int index = this.Loot.Count - 1; index >= 0; --index)
      {
        FVRObject fvrObject = this.Loot[index];
        if (eras != null && !eras.Contains(fvrObject.TagEra))
          this.Loot.RemoveAt(index);
        else if (meleeStyles != null && !meleeStyles.Contains(fvrObject.TagMeleeStyle))
          this.Loot.RemoveAt(index);
        else if (meleeHandedness != null && !meleeHandedness.Contains(fvrObject.TagMeleeHandedness))
          this.Loot.RemoveAt(index);
      }
    }

    private void InitializeAttachmentTable(
      List<FVRObject.OTagEra> eras = null,
      List<FVRObject.OTagFirearmMount> mounts = null,
      List<FVRObject.OTagAttachmentFeature> features = null)
    {
      this.Loot = new List<FVRObject>((IEnumerable<FVRObject>) ManagerSingleton<IM>.Instance.odicTagCategory[FVRObject.ObjectCategory.Attachment]);
      for (int index = this.Loot.Count - 1; index >= 0; --index)
      {
        FVRObject fvrObject = this.Loot[index];
        if (eras != null && !eras.Contains(fvrObject.TagEra))
          this.Loot.RemoveAt(index);
        else if (mounts != null && !mounts.Contains(fvrObject.TagAttachmentMount))
          this.Loot.RemoveAt(index);
        else if (features != null && !features.Contains(fvrObject.TagAttachmentFeature))
          this.Loot.RemoveAt(index);
      }
    }

    public FVRObject GetRandomObject() => this.Loot.Count > 0 ? this.Loot[UnityEngine.Random.Range(0, this.Loot.Count)] : (FVRObject) null;

    public FVRObject GetRandomAmmoObject(FVRObject o, List<FVRObject.OTagEra> eras = null)
    {
      if (o.CompatibleMagazines.Count > 0)
      {
        List<FVRObject> fvrObjectList = new List<FVRObject>();
        for (int index = 0; index < o.CompatibleMagazines.Count; ++index)
        {
          if ((o.CompatibleMagazines[index].MagazineCapacity <= this.MaxCapacity || this.MaxCapacity == -1) && (o.CompatibleMagazines[index].MagazineCapacity >= this.MinCapacity || this.MinCapacity == -1))
            fvrObjectList.Add(o.CompatibleMagazines[index]);
        }
        return fvrObjectList.Count > 0 ? fvrObjectList[UnityEngine.Random.Range(0, fvrObjectList.Count)] : o.CompatibleMagazines[0];
      }
      if (o.CompatibleClips.Count > 0)
        return o.CompatibleClips[UnityEngine.Random.Range(0, o.CompatibleClips.Count)];
      if (o.CompatibleSpeedLoaders.Count > 0)
        return o.CompatibleSpeedLoaders[UnityEngine.Random.Range(0, o.CompatibleSpeedLoaders.Count)];
      if (o.CompatibleSingleRounds.Count <= 0)
        return (FVRObject) null;
      if (eras == null)
        return o.CompatibleSingleRounds[UnityEngine.Random.Range(0, o.CompatibleSingleRounds.Count)];
      List<FVRObject> fvrObjectList1 = new List<FVRObject>();
      for (int index = 0; index < o.CompatibleSingleRounds.Count; ++index)
      {
        if (eras.Contains(o.CompatibleSingleRounds[index].TagEra))
          fvrObjectList1.Add(o.CompatibleSingleRounds[index]);
      }
      if (fvrObjectList1.Count <= 0)
        return o.CompatibleSingleRounds[0];
      FVRObject fvrObject = fvrObjectList1[UnityEngine.Random.Range(0, fvrObjectList1.Count)];
      fvrObjectList1.Clear();
      return fvrObject;
    }

    public FVRObject GetRandomBespokeAttachment(FVRObject o) => o.BespokeAttachments.Count > 0 ? o.BespokeAttachments[UnityEngine.Random.Range(0, o.BespokeAttachments.Count)] : (FVRObject) null;

    public enum LootTableType
    {
      Firearm,
      Powerup,
      Thrown,
      Attachments,
      Melee,
    }
  }
}
