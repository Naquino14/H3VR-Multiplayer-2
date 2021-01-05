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

		public static bool HasSpawnedID(string id)
		{
			if (id == string.Empty)
			{
				return false;
			}
			if (ManagerSingleton<IM>.Instance.SpawnerIDDic.ContainsKey(id))
			{
				return true;
			}
			return false;
		}

		public static ItemSpawnerID GetSpawnerID(string id)
		{
			return ManagerSingleton<IM>.Instance.SpawnerIDDic[id];
		}

		protected override void Awake()
		{
			base.Awake();
			GenerateItemDBs();
		}

		public static List<ItemSpawnerID> GetAvailableInSubCategory(ItemSpawnerID.ESubCategory subcat)
		{
			List<ItemSpawnerID> list = new List<ItemSpawnerID>();
			List<ItemSpawnerID> list2 = SCD[subcat];
			for (int i = 0; i < list2.Count; i++)
			{
				if (GM.Rewards.RewardUnlocks.IsRewardUnlocked(list2[i]) && list2[i].IsDisplayedInMainEntry)
				{
					list.Add(list2[i]);
				}
			}
			return list;
		}

		public static int GetAvailableCountInSubCategory(ItemSpawnerID.ESubCategory subcat)
		{
			int num = 0;
			List<ItemSpawnerID> list = SCD[subcat];
			for (int i = 0; i < list.Count; i++)
			{
				if (GM.Rewards.RewardUnlocks.IsRewardUnlocked(list[i]) && list[i].IsDisplayedInMainEntry)
				{
					num++;
				}
			}
			return num;
		}

		private void GenerateItemDBs()
		{
			ObjectDic = new Dictionary<string, FVRObject>();
			FVRObject[] array = Resources.LoadAll<FVRObject>("ObjectIDs");
			for (int i = 0; i < array.Length; i++)
			{
				if (!ObjectDic.ContainsKey(array[i].ItemID))
				{
					ObjectDic.Add(array[i].ItemID, array[i]);
				}
				else
				{
					Debug.Log("Duplicate Key for:" + array[i].ItemID);
				}
			}
			foreach (FVRObject fVRObject in array)
			{
				if (!odicTagCategory.ContainsKey(fVRObject.Category))
				{
					List<FVRObject> value = new List<FVRObject>();
					odicTagCategory.Add(fVRObject.Category, value);
					HashSet<FVRObject> value2 = new HashSet<FVRObject>();
					ohashTagCategory.Add(fVRObject.Category, value2);
				}
				odicTagCategory[fVRObject.Category].Add(fVRObject);
				ohashTagCategory[fVRObject.Category].Add(fVRObject);
				if (fVRObject.Category == FVRObject.ObjectCategory.Firearm)
				{
					if (!odicTagFirearmEra.ContainsKey(fVRObject.TagEra))
					{
						List<FVRObject> value3 = new List<FVRObject>();
						odicTagFirearmEra.Add(fVRObject.TagEra, value3);
						HashSet<FVRObject> value4 = new HashSet<FVRObject>();
						ohashTagFirearmEra.Add(fVRObject.TagEra, value4);
					}
					odicTagFirearmEra[fVRObject.TagEra].Add(fVRObject);
					ohashTagFirearmEra[fVRObject.TagEra].Add(fVRObject);
					if (!odicTagFirearmSize.ContainsKey(fVRObject.TagFirearmSize))
					{
						List<FVRObject> value5 = new List<FVRObject>();
						odicTagFirearmSize.Add(fVRObject.TagFirearmSize, value5);
						HashSet<FVRObject> value6 = new HashSet<FVRObject>();
						ohashTagFirearmSize.Add(fVRObject.TagFirearmSize, value6);
					}
					odicTagFirearmSize[fVRObject.TagFirearmSize].Add(fVRObject);
					ohashTagFirearmSize[fVRObject.TagFirearmSize].Add(fVRObject);
					if (!odicTagFirearmAction.ContainsKey(fVRObject.TagFirearmAction))
					{
						List<FVRObject> value7 = new List<FVRObject>();
						odicTagFirearmAction.Add(fVRObject.TagFirearmAction, value7);
						HashSet<FVRObject> value8 = new HashSet<FVRObject>();
						ohashTagFirearmAction.Add(fVRObject.TagFirearmAction, value8);
					}
					odicTagFirearmAction[fVRObject.TagFirearmAction].Add(fVRObject);
					ohashTagFirearmAction[fVRObject.TagFirearmAction].Add(fVRObject);
					for (int k = 0; k < fVRObject.TagFirearmFiringModes.Count; k++)
					{
						FVRObject.OTagFirearmFiringMode key = fVRObject.TagFirearmFiringModes[k];
						if (!odicTagFirearmFiringMode.ContainsKey(key))
						{
							List<FVRObject> value9 = new List<FVRObject>();
							odicTagFirearmFiringMode.Add(key, value9);
							HashSet<FVRObject> value10 = new HashSet<FVRObject>();
							ohashTagFirearmFiringMode.Add(key, value10);
						}
						odicTagFirearmFiringMode[key].Add(fVRObject);
						ohashTagFirearmFiringMode[key].Add(fVRObject);
					}
					for (int l = 0; l < fVRObject.TagFirearmFeedOption.Count; l++)
					{
						FVRObject.OTagFirearmFeedOption key2 = fVRObject.TagFirearmFeedOption[l];
						if (!odicTagFirearmFeedOption.ContainsKey(key2))
						{
							List<FVRObject> value11 = new List<FVRObject>();
							odicTagFirearmFeedOption.Add(key2, value11);
							HashSet<FVRObject> value12 = new HashSet<FVRObject>();
							ohashTagFirearmFeedOption.Add(key2, value12);
						}
						odicTagFirearmFeedOption[key2].Add(fVRObject);
						ohashTagFirearmFeedOption[key2].Add(fVRObject);
					}
					for (int m = 0; m < fVRObject.TagFirearmMounts.Count; m++)
					{
						FVRObject.OTagFirearmMount key3 = fVRObject.TagFirearmMounts[m];
						if (!odicTagFirearmMount.ContainsKey(key3))
						{
							List<FVRObject> value13 = new List<FVRObject>();
							odicTagFirearmMount.Add(key3, value13);
							HashSet<FVRObject> value14 = new HashSet<FVRObject>();
							ohashTagFirearmMount.Add(key3, value14);
						}
						odicTagFirearmMount[key3].Add(fVRObject);
						ohashTagFirearmMount[key3].Add(fVRObject);
					}
				}
				else if (fVRObject.Category == FVRObject.ObjectCategory.Attachment)
				{
					if (!odicTagAttachmentMount.ContainsKey(fVRObject.TagAttachmentMount))
					{
						List<FVRObject> value15 = new List<FVRObject>();
						odicTagAttachmentMount.Add(fVRObject.TagAttachmentMount, value15);
						HashSet<FVRObject> value16 = new HashSet<FVRObject>();
						ohasgTagAttachmentMount.Add(fVRObject.TagAttachmentMount, value16);
					}
					odicTagAttachmentMount[fVRObject.TagAttachmentMount].Add(fVRObject);
					ohasgTagAttachmentMount[fVRObject.TagAttachmentMount].Add(fVRObject);
					if (!odicTagAttachmentFeature.ContainsKey(fVRObject.TagAttachmentFeature))
					{
						List<FVRObject> value17 = new List<FVRObject>();
						odicTagAttachmentFeature.Add(fVRObject.TagAttachmentFeature, value17);
						HashSet<FVRObject> value18 = new HashSet<FVRObject>();
						ohasgTagAttachmentFeature.Add(fVRObject.TagAttachmentFeature, value18);
					}
					odicTagAttachmentFeature[fVRObject.TagAttachmentFeature].Add(fVRObject);
					ohasgTagAttachmentFeature[fVRObject.TagAttachmentFeature].Add(fVRObject);
				}
			}
			CategoryInfoDic = new Dictionary<ItemSpawnerID.EItemCategory, ItemSpawnerCategoryDefinitions.Category>();
			SubCategoryInfoDic = new Dictionary<ItemSpawnerID.ESubCategory, ItemSpawnerCategoryDefinitions.SubCategory>();
			for (int n = 0; n < CatDefs.Categories.Length; n++)
			{
				CategoryInfoDic.Add(CatDefs.Categories[n].Cat, CatDefs.Categories[n]);
				for (int num = 0; num < CatDefs.Categories[n].Subcats.Length; num++)
				{
					SubCategoryInfoDic.Add(CatDefs.Categories[n].Subcats[num].Subcat, CatDefs.Categories[n].Subcats[num]);
				}
			}
			CategoryRelationDic = new Dictionary<ItemSpawnerID.EItemCategory, List<ItemSpawnerCategoryDefinitions.SubCategory>>();
			for (int num2 = 0; num2 < CatDefs.Categories.Length; num2++)
			{
				for (int num3 = 0; num3 < CatDefs.Categories[num2].Subcats.Length; num3++)
				{
					if (CategoryRelationDic.ContainsKey(CatDefs.Categories[num2].Cat))
					{
						CategoryRelationDic[CatDefs.Categories[num2].Cat].Add(CatDefs.Categories[num2].Subcats[num3]);
						continue;
					}
					List<ItemSpawnerCategoryDefinitions.SubCategory> list = new List<ItemSpawnerCategoryDefinitions.SubCategory>();
					list.Add(CatDefs.Categories[num2].Subcats[num3]);
					CategoryRelationDic.Add(CatDefs.Categories[num2].Cat, list);
				}
			}
			CategoryDic = new Dictionary<ItemSpawnerID.EItemCategory, List<ItemSpawnerID>>();
			SubCategoryDic = new Dictionary<ItemSpawnerID.ESubCategory, List<ItemSpawnerID>>();
			ItemSpawnerID[] array2 = Resources.LoadAll<ItemSpawnerID>("ItemSpawnerIDs");
			SpawnerIDDic = new Dictionary<string, ItemSpawnerID>();
			for (int num4 = 0; num4 < array2.Length; num4++)
			{
				if (array2[num4].ItemID != string.Empty)
				{
					if (SpawnerIDDic.ContainsKey(array2[num4].ItemID))
					{
						Debug.Log("Oh shit, duplicate of:" + array2[num4].ItemID + " on " + array2[num4].name);
					}
					else
					{
						SpawnerIDDic.Add(array2[num4].ItemID, array2[num4]);
					}
				}
				if (CategoryDic.ContainsKey(array2[num4].Category))
				{
					CategoryDic[array2[num4].Category].Add(array2[num4]);
				}
				else
				{
					List<ItemSpawnerID> list2 = new List<ItemSpawnerID>();
					list2.Add(array2[num4]);
					CategoryDic.Add(array2[num4].Category, list2);
				}
				if (SubCategoryDic.ContainsKey(array2[num4].SubCategory))
				{
					SubCategoryDic[array2[num4].SubCategory].Add(array2[num4]);
					continue;
				}
				List<ItemSpawnerID> list3 = new List<ItemSpawnerID>();
				list3.Add(array2[num4]);
				SubCategoryDic.Add(array2[num4].SubCategory, list3);
			}
			SosigEnemyTemplate[] array3 = Resources.LoadAll<SosigEnemyTemplate>("SosigEnemyTemplates");
			for (int num5 = 0; num5 < array3.Length; num5++)
			{
				if (!olistSosigCats.Contains(array3[num5].SosigEnemyCategory))
				{
					olistSosigCats.Add(array3[num5].SosigEnemyCategory);
				}
				if (!odicSosigIDsByCategory.ContainsKey(array3[num5].SosigEnemyCategory))
				{
					List<SosigEnemyID> value19 = new List<SosigEnemyID>();
					odicSosigIDsByCategory.Add(array3[num5].SosigEnemyCategory, value19);
					List<SosigEnemyTemplate> value20 = new List<SosigEnemyTemplate>();
					odicSosigObjsByCategory.Add(array3[num5].SosigEnemyCategory, value20);
				}
				if (array3[num5].SosigEnemyID != SosigEnemyID.None)
				{
					odicSosigIDsByCategory[array3[num5].SosigEnemyCategory].Add(array3[num5].SosigEnemyID);
					odicSosigObjsByCategory[array3[num5].SosigEnemyCategory].Add(array3[num5]);
					odicSosigObjsByID.Add(array3[num5].SosigEnemyID, array3[num5]);
				}
			}
		}
	}
}
