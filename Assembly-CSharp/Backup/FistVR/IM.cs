// Decompiled with JetBrains decompiler
// Type: FistVR.IM
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class IM : ManagerSingleton<IM>
  {
    public ItemSpawnerCategoryDefinitions CatDefs;
    private Dictionary<ItemSpawnerID.EItemCategory, List<ItemSpawnerID>> CategoryDic;
    private Dictionary<ItemSpawnerID.ESubCategory, List<ItemSpawnerID>> SubCategoryDic;
    private Dictionary<ItemSpawnerID.EItemCategory, ItemSpawnerCategoryDefinitions.Category> CategoryInfoDic;
    private Dictionary<ItemSpawnerID.ESubCategory, ItemSpawnerCategoryDefinitions.SubCategory> SubCategoryInfoDic;
    private Dictionary<ItemSpawnerID.EItemCategory, List<ItemSpawnerCategoryDefinitions.SubCategory>> CategoryRelationDic;
    private Dictionary<string, FVRObject> ObjectDic;
    private Dictionary<string, ItemSpawnerID> SpawnerIDDic;
    public Dictionary<FVRObject.ObjectCategory, List<FVRObject>> odicTagCategory = new Dictionary<FVRObject.ObjectCategory, List<FVRObject>>();
    public Dictionary<FVRObject.ObjectCategory, HashSet<FVRObject>> ohashTagCategory = new Dictionary<FVRObject.ObjectCategory, HashSet<FVRObject>>();
    public Dictionary<FVRObject.OTagEra, List<FVRObject>> odicTagFirearmEra = new Dictionary<FVRObject.OTagEra, List<FVRObject>>();
    public Dictionary<FVRObject.OTagEra, HashSet<FVRObject>> ohashTagFirearmEra = new Dictionary<FVRObject.OTagEra, HashSet<FVRObject>>();
    public Dictionary<FVRObject.OTagFirearmSize, List<FVRObject>> odicTagFirearmSize = new Dictionary<FVRObject.OTagFirearmSize, List<FVRObject>>();
    public Dictionary<FVRObject.OTagFirearmSize, HashSet<FVRObject>> ohashTagFirearmSize = new Dictionary<FVRObject.OTagFirearmSize, HashSet<FVRObject>>();
    public Dictionary<FVRObject.OTagFirearmAction, List<FVRObject>> odicTagFirearmAction = new Dictionary<FVRObject.OTagFirearmAction, List<FVRObject>>();
    public Dictionary<FVRObject.OTagFirearmAction, HashSet<FVRObject>> ohashTagFirearmAction = new Dictionary<FVRObject.OTagFirearmAction, HashSet<FVRObject>>();
    public Dictionary<FVRObject.OTagFirearmFiringMode, List<FVRObject>> odicTagFirearmFiringMode = new Dictionary<FVRObject.OTagFirearmFiringMode, List<FVRObject>>();
    public Dictionary<FVRObject.OTagFirearmFiringMode, HashSet<FVRObject>> ohashTagFirearmFiringMode = new Dictionary<FVRObject.OTagFirearmFiringMode, HashSet<FVRObject>>();
    public Dictionary<FVRObject.OTagFirearmFeedOption, List<FVRObject>> odicTagFirearmFeedOption = new Dictionary<FVRObject.OTagFirearmFeedOption, List<FVRObject>>();
    public Dictionary<FVRObject.OTagFirearmFeedOption, HashSet<FVRObject>> ohashTagFirearmFeedOption = new Dictionary<FVRObject.OTagFirearmFeedOption, HashSet<FVRObject>>();
    public Dictionary<FVRObject.OTagFirearmMount, List<FVRObject>> odicTagFirearmMount = new Dictionary<FVRObject.OTagFirearmMount, List<FVRObject>>();
    public Dictionary<FVRObject.OTagFirearmMount, HashSet<FVRObject>> ohashTagFirearmMount = new Dictionary<FVRObject.OTagFirearmMount, HashSet<FVRObject>>();
    public Dictionary<FVRObject.OTagFirearmMount, List<FVRObject>> odicTagAttachmentMount = new Dictionary<FVRObject.OTagFirearmMount, List<FVRObject>>();
    public Dictionary<FVRObject.OTagFirearmMount, HashSet<FVRObject>> ohasgTagAttachmentMount = new Dictionary<FVRObject.OTagFirearmMount, HashSet<FVRObject>>();
    public Dictionary<FVRObject.OTagAttachmentFeature, List<FVRObject>> odicTagAttachmentFeature = new Dictionary<FVRObject.OTagAttachmentFeature, List<FVRObject>>();
    public Dictionary<FVRObject.OTagAttachmentFeature, HashSet<FVRObject>> ohasgTagAttachmentFeature = new Dictionary<FVRObject.OTagAttachmentFeature, HashSet<FVRObject>>();
    public List<SosigEnemyCategory> olistSosigCats = new List<SosigEnemyCategory>();
    public Dictionary<SosigEnemyID, SosigEnemyTemplate> odicSosigObjsByID = new Dictionary<SosigEnemyID, SosigEnemyTemplate>();
    public Dictionary<SosigEnemyCategory, List<SosigEnemyID>> odicSosigIDsByCategory = new Dictionary<SosigEnemyCategory, List<SosigEnemyID>>();
    public Dictionary<SosigEnemyCategory, List<SosigEnemyTemplate>> odicSosigObjsByCategory = new Dictionary<SosigEnemyCategory, List<SosigEnemyTemplate>>();

    public static ItemSpawnerCategoryDefinitions CDefs => ManagerSingleton<IM>.Instance.CatDefs;

    public static Dictionary<ItemSpawnerID.EItemCategory, ItemSpawnerCategoryDefinitions.Category> CDefInfo => ManagerSingleton<IM>.Instance.CategoryInfoDic;

    public static Dictionary<ItemSpawnerID.ESubCategory, ItemSpawnerCategoryDefinitions.SubCategory> CDefSubInfo => ManagerSingleton<IM>.Instance.SubCategoryInfoDic;

    public static Dictionary<ItemSpawnerID.EItemCategory, List<ItemSpawnerCategoryDefinitions.SubCategory>> CDefSubs => ManagerSingleton<IM>.Instance.CategoryRelationDic;

    public static Dictionary<ItemSpawnerID.EItemCategory, List<ItemSpawnerID>> CD => ManagerSingleton<IM>.Instance.CategoryDic;

    public static Dictionary<ItemSpawnerID.ESubCategory, List<ItemSpawnerID>> SCD => ManagerSingleton<IM>.Instance.SubCategoryDic;

    public static Dictionary<string, FVRObject> OD => ManagerSingleton<IM>.Instance.ObjectDic;

    public static bool HasSpawnedID(string id) => !(id == string.Empty) && ManagerSingleton<IM>.Instance.SpawnerIDDic.ContainsKey(id);

    public static ItemSpawnerID GetSpawnerID(string id) => ManagerSingleton<IM>.Instance.SpawnerIDDic[id];

    protected override void Awake()
    {
      base.Awake();
      this.GenerateItemDBs();
    }

    public static List<ItemSpawnerID> GetAvailableInSubCategory(
      ItemSpawnerID.ESubCategory subcat)
    {
      List<ItemSpawnerID> itemSpawnerIdList1 = new List<ItemSpawnerID>();
      List<ItemSpawnerID> itemSpawnerIdList2 = IM.SCD[subcat];
      for (int index = 0; index < itemSpawnerIdList2.Count; ++index)
      {
        if (GM.Rewards.RewardUnlocks.IsRewardUnlocked(itemSpawnerIdList2[index]) && itemSpawnerIdList2[index].IsDisplayedInMainEntry)
          itemSpawnerIdList1.Add(itemSpawnerIdList2[index]);
      }
      return itemSpawnerIdList1;
    }

    public static int GetAvailableCountInSubCategory(ItemSpawnerID.ESubCategory subcat)
    {
      int num = 0;
      List<ItemSpawnerID> itemSpawnerIdList = IM.SCD[subcat];
      for (int index = 0; index < itemSpawnerIdList.Count; ++index)
      {
        if (GM.Rewards.RewardUnlocks.IsRewardUnlocked(itemSpawnerIdList[index]) && itemSpawnerIdList[index].IsDisplayedInMainEntry)
          ++num;
      }
      return num;
    }

    private void GenerateItemDBs()
    {
      this.ObjectDic = new Dictionary<string, FVRObject>();
      FVRObject[] fvrObjectArray = Resources.LoadAll<FVRObject>("ObjectIDs");
      for (int index = 0; index < fvrObjectArray.Length; ++index)
      {
        if (!this.ObjectDic.ContainsKey(fvrObjectArray[index].ItemID))
          this.ObjectDic.Add(fvrObjectArray[index].ItemID, fvrObjectArray[index]);
        else
          Debug.Log((object) ("Duplicate Key for:" + fvrObjectArray[index].ItemID));
      }
      for (int index1 = 0; index1 < fvrObjectArray.Length; ++index1)
      {
        FVRObject fvrObject = fvrObjectArray[index1];
        if (!this.odicTagCategory.ContainsKey(fvrObject.Category))
        {
          List<FVRObject> fvrObjectList = new List<FVRObject>();
          this.odicTagCategory.Add(fvrObject.Category, fvrObjectList);
          HashSet<FVRObject> fvrObjectSet = new HashSet<FVRObject>();
          this.ohashTagCategory.Add(fvrObject.Category, fvrObjectSet);
        }
        this.odicTagCategory[fvrObject.Category].Add(fvrObject);
        this.ohashTagCategory[fvrObject.Category].Add(fvrObject);
        if (fvrObject.Category == FVRObject.ObjectCategory.Firearm)
        {
          if (!this.odicTagFirearmEra.ContainsKey(fvrObject.TagEra))
          {
            List<FVRObject> fvrObjectList = new List<FVRObject>();
            this.odicTagFirearmEra.Add(fvrObject.TagEra, fvrObjectList);
            HashSet<FVRObject> fvrObjectSet = new HashSet<FVRObject>();
            this.ohashTagFirearmEra.Add(fvrObject.TagEra, fvrObjectSet);
          }
          this.odicTagFirearmEra[fvrObject.TagEra].Add(fvrObject);
          this.ohashTagFirearmEra[fvrObject.TagEra].Add(fvrObject);
          if (!this.odicTagFirearmSize.ContainsKey(fvrObject.TagFirearmSize))
          {
            List<FVRObject> fvrObjectList = new List<FVRObject>();
            this.odicTagFirearmSize.Add(fvrObject.TagFirearmSize, fvrObjectList);
            HashSet<FVRObject> fvrObjectSet = new HashSet<FVRObject>();
            this.ohashTagFirearmSize.Add(fvrObject.TagFirearmSize, fvrObjectSet);
          }
          this.odicTagFirearmSize[fvrObject.TagFirearmSize].Add(fvrObject);
          this.ohashTagFirearmSize[fvrObject.TagFirearmSize].Add(fvrObject);
          if (!this.odicTagFirearmAction.ContainsKey(fvrObject.TagFirearmAction))
          {
            List<FVRObject> fvrObjectList = new List<FVRObject>();
            this.odicTagFirearmAction.Add(fvrObject.TagFirearmAction, fvrObjectList);
            HashSet<FVRObject> fvrObjectSet = new HashSet<FVRObject>();
            this.ohashTagFirearmAction.Add(fvrObject.TagFirearmAction, fvrObjectSet);
          }
          this.odicTagFirearmAction[fvrObject.TagFirearmAction].Add(fvrObject);
          this.ohashTagFirearmAction[fvrObject.TagFirearmAction].Add(fvrObject);
          for (int index2 = 0; index2 < fvrObject.TagFirearmFiringModes.Count; ++index2)
          {
            FVRObject.OTagFirearmFiringMode firearmFiringMode = fvrObject.TagFirearmFiringModes[index2];
            if (!this.odicTagFirearmFiringMode.ContainsKey(firearmFiringMode))
            {
              List<FVRObject> fvrObjectList = new List<FVRObject>();
              this.odicTagFirearmFiringMode.Add(firearmFiringMode, fvrObjectList);
              HashSet<FVRObject> fvrObjectSet = new HashSet<FVRObject>();
              this.ohashTagFirearmFiringMode.Add(firearmFiringMode, fvrObjectSet);
            }
            this.odicTagFirearmFiringMode[firearmFiringMode].Add(fvrObject);
            this.ohashTagFirearmFiringMode[firearmFiringMode].Add(fvrObject);
          }
          for (int index2 = 0; index2 < fvrObject.TagFirearmFeedOption.Count; ++index2)
          {
            FVRObject.OTagFirearmFeedOption key = fvrObject.TagFirearmFeedOption[index2];
            if (!this.odicTagFirearmFeedOption.ContainsKey(key))
            {
              List<FVRObject> fvrObjectList = new List<FVRObject>();
              this.odicTagFirearmFeedOption.Add(key, fvrObjectList);
              HashSet<FVRObject> fvrObjectSet = new HashSet<FVRObject>();
              this.ohashTagFirearmFeedOption.Add(key, fvrObjectSet);
            }
            this.odicTagFirearmFeedOption[key].Add(fvrObject);
            this.ohashTagFirearmFeedOption[key].Add(fvrObject);
          }
          for (int index2 = 0; index2 < fvrObject.TagFirearmMounts.Count; ++index2)
          {
            FVRObject.OTagFirearmMount tagFirearmMount = fvrObject.TagFirearmMounts[index2];
            if (!this.odicTagFirearmMount.ContainsKey(tagFirearmMount))
            {
              List<FVRObject> fvrObjectList = new List<FVRObject>();
              this.odicTagFirearmMount.Add(tagFirearmMount, fvrObjectList);
              HashSet<FVRObject> fvrObjectSet = new HashSet<FVRObject>();
              this.ohashTagFirearmMount.Add(tagFirearmMount, fvrObjectSet);
            }
            this.odicTagFirearmMount[tagFirearmMount].Add(fvrObject);
            this.ohashTagFirearmMount[tagFirearmMount].Add(fvrObject);
          }
        }
        else if (fvrObject.Category == FVRObject.ObjectCategory.Attachment)
        {
          if (!this.odicTagAttachmentMount.ContainsKey(fvrObject.TagAttachmentMount))
          {
            List<FVRObject> fvrObjectList = new List<FVRObject>();
            this.odicTagAttachmentMount.Add(fvrObject.TagAttachmentMount, fvrObjectList);
            HashSet<FVRObject> fvrObjectSet = new HashSet<FVRObject>();
            this.ohasgTagAttachmentMount.Add(fvrObject.TagAttachmentMount, fvrObjectSet);
          }
          this.odicTagAttachmentMount[fvrObject.TagAttachmentMount].Add(fvrObject);
          this.ohasgTagAttachmentMount[fvrObject.TagAttachmentMount].Add(fvrObject);
          if (!this.odicTagAttachmentFeature.ContainsKey(fvrObject.TagAttachmentFeature))
          {
            List<FVRObject> fvrObjectList = new List<FVRObject>();
            this.odicTagAttachmentFeature.Add(fvrObject.TagAttachmentFeature, fvrObjectList);
            HashSet<FVRObject> fvrObjectSet = new HashSet<FVRObject>();
            this.ohasgTagAttachmentFeature.Add(fvrObject.TagAttachmentFeature, fvrObjectSet);
          }
          this.odicTagAttachmentFeature[fvrObject.TagAttachmentFeature].Add(fvrObject);
          this.ohasgTagAttachmentFeature[fvrObject.TagAttachmentFeature].Add(fvrObject);
        }
      }
      this.CategoryInfoDic = new Dictionary<ItemSpawnerID.EItemCategory, ItemSpawnerCategoryDefinitions.Category>();
      this.SubCategoryInfoDic = new Dictionary<ItemSpawnerID.ESubCategory, ItemSpawnerCategoryDefinitions.SubCategory>();
      for (int index1 = 0; index1 < this.CatDefs.Categories.Length; ++index1)
      {
        this.CategoryInfoDic.Add(this.CatDefs.Categories[index1].Cat, this.CatDefs.Categories[index1]);
        for (int index2 = 0; index2 < this.CatDefs.Categories[index1].Subcats.Length; ++index2)
          this.SubCategoryInfoDic.Add(this.CatDefs.Categories[index1].Subcats[index2].Subcat, this.CatDefs.Categories[index1].Subcats[index2]);
      }
      this.CategoryRelationDic = new Dictionary<ItemSpawnerID.EItemCategory, List<ItemSpawnerCategoryDefinitions.SubCategory>>();
      for (int index1 = 0; index1 < this.CatDefs.Categories.Length; ++index1)
      {
        for (int index2 = 0; index2 < this.CatDefs.Categories[index1].Subcats.Length; ++index2)
        {
          if (this.CategoryRelationDic.ContainsKey(this.CatDefs.Categories[index1].Cat))
            this.CategoryRelationDic[this.CatDefs.Categories[index1].Cat].Add(this.CatDefs.Categories[index1].Subcats[index2]);
          else
            this.CategoryRelationDic.Add(this.CatDefs.Categories[index1].Cat, new List<ItemSpawnerCategoryDefinitions.SubCategory>()
            {
              this.CatDefs.Categories[index1].Subcats[index2]
            });
        }
      }
      this.CategoryDic = new Dictionary<ItemSpawnerID.EItemCategory, List<ItemSpawnerID>>();
      this.SubCategoryDic = new Dictionary<ItemSpawnerID.ESubCategory, List<ItemSpawnerID>>();
      ItemSpawnerID[] itemSpawnerIdArray = Resources.LoadAll<ItemSpawnerID>("ItemSpawnerIDs");
      this.SpawnerIDDic = new Dictionary<string, ItemSpawnerID>();
      for (int index = 0; index < itemSpawnerIdArray.Length; ++index)
      {
        if (itemSpawnerIdArray[index].ItemID != string.Empty)
        {
          if (this.SpawnerIDDic.ContainsKey(itemSpawnerIdArray[index].ItemID))
            Debug.Log((object) ("Oh shit, duplicate of:" + itemSpawnerIdArray[index].ItemID + " on " + itemSpawnerIdArray[index].name));
          else
            this.SpawnerIDDic.Add(itemSpawnerIdArray[index].ItemID, itemSpawnerIdArray[index]);
        }
        if (this.CategoryDic.ContainsKey(itemSpawnerIdArray[index].Category))
          this.CategoryDic[itemSpawnerIdArray[index].Category].Add(itemSpawnerIdArray[index]);
        else
          this.CategoryDic.Add(itemSpawnerIdArray[index].Category, new List<ItemSpawnerID>()
          {
            itemSpawnerIdArray[index]
          });
        if (this.SubCategoryDic.ContainsKey(itemSpawnerIdArray[index].SubCategory))
          this.SubCategoryDic[itemSpawnerIdArray[index].SubCategory].Add(itemSpawnerIdArray[index]);
        else
          this.SubCategoryDic.Add(itemSpawnerIdArray[index].SubCategory, new List<ItemSpawnerID>()
          {
            itemSpawnerIdArray[index]
          });
      }
      SosigEnemyTemplate[] sosigEnemyTemplateArray = Resources.LoadAll<SosigEnemyTemplate>("SosigEnemyTemplates");
      for (int index = 0; index < sosigEnemyTemplateArray.Length; ++index)
      {
        if (!this.olistSosigCats.Contains(sosigEnemyTemplateArray[index].SosigEnemyCategory))
          this.olistSosigCats.Add(sosigEnemyTemplateArray[index].SosigEnemyCategory);
        if (!this.odicSosigIDsByCategory.ContainsKey(sosigEnemyTemplateArray[index].SosigEnemyCategory))
        {
          List<SosigEnemyID> sosigEnemyIdList = new List<SosigEnemyID>();
          this.odicSosigIDsByCategory.Add(sosigEnemyTemplateArray[index].SosigEnemyCategory, sosigEnemyIdList);
          List<SosigEnemyTemplate> sosigEnemyTemplateList = new List<SosigEnemyTemplate>();
          this.odicSosigObjsByCategory.Add(sosigEnemyTemplateArray[index].SosigEnemyCategory, sosigEnemyTemplateList);
        }
        if (sosigEnemyTemplateArray[index].SosigEnemyID != SosigEnemyID.None)
        {
          this.odicSosigIDsByCategory[sosigEnemyTemplateArray[index].SosigEnemyCategory].Add(sosigEnemyTemplateArray[index].SosigEnemyID);
          this.odicSosigObjsByCategory[sosigEnemyTemplateArray[index].SosigEnemyCategory].Add(sosigEnemyTemplateArray[index]);
          this.odicSosigObjsByID.Add(sosigEnemyTemplateArray[index].SosigEnemyID, sosigEnemyTemplateArray[index]);
        }
      }
    }
  }
}
