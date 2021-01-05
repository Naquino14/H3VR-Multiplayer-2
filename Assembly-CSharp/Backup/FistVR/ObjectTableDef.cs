// Decompiled with JetBrains decompiler
// Type: FistVR.ObjectTableDef
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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
      for (int index = 0; index < this.Objs.Count; ++index)
        this.IDOverride.Add(this.Objs[index].ItemID);
      this.Objs.Clear();
    }

    [ContextMenu("TestTable")]
    public void TestTable()
    {
      ObjectTable objectTable = new ObjectTable();
      objectTable.Initialize(this, this.Category, this.Eras, this.Sets, this.Sizes, this.Actions, this.Modes, this.ExcludeModes, this.Feedoptions, this.MountsAvailable, this.RoundPowers, this.Features, this.MeleeStyles, this.MeleeHandedness, this.MountTypes, this.PowerupTypes, this.ThrownTypes, this.ThrownDamageTypes, this.MinAmmoCapacity, this.MaxAmmoCapacity, this.RequiredExactCapacity, this.IsBlanked);
      Debug.Log((object) "----Listing Objects----");
      for (int index = 0; index < objectTable.Objs.Count; ++index)
        Debug.Log((object) ("Object " + (object) index + " is " + objectTable.Objs[index].DisplayName));
      Debug.Log((object) ("Table Created and has " + (object) objectTable.Objs.Count + " in it."));
    }
  }
}
