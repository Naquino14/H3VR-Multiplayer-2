using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class PM : ManagerSingleton<PM>
	{
		[Serializable]
		public class HitSoundSet
		{
			public PMatSoundCategory Category;

			public AudioClip[] ClipList;
		}

		public PMat PMat_Air;

		public PMat PMat_Default;

		public HitSoundSet[] SoundSets;

		private Dictionary<PMaterial, PMatSoundCategory> MaterialSoundBindings;

		private Dictionary<PMatSoundCategory, AudioClip[]> ClipDic;

		public MatDef MatDef_Default;

		private Dictionary<BallisticProjectileType, Dictionary<MatBallisticType, BallisticMatSeries>> BallisticDic = new Dictionary<BallisticProjectileType, Dictionary<MatBallisticType, BallisticMatSeries>>();

		public static PMat AirMat => ManagerSingleton<PM>.Instance.PMat_Air;

		public static PMat DefaultMat => ManagerSingleton<PM>.Instance.PMat_Default;

		public static MatDef DefaultMatDef => ManagerSingleton<PM>.Instance.MatDef_Default;

		public static Dictionary<BallisticProjectileType, Dictionary<MatBallisticType, BallisticMatSeries>> SBallisticDic => ManagerSingleton<PM>.Instance.BallisticDic;

		protected override void Awake()
		{
			base.Awake();
			MaterialSoundBindings = new Dictionary<PMaterial, PMatSoundCategory>();
			ClipDic = new Dictionary<PMatSoundCategory, AudioClip[]>();
			GenerateMaterialDB();
		}

		private void GenerateMaterialDB()
		{
			PMaterialDefinition[] array = Resources.LoadAll<PMaterialDefinition>("PMaterialDefinitions");
			for (int i = 0; i < array.Length; i++)
			{
				MaterialSoundBindings.Add(array[i].material, array[i].soundCategory);
			}
			for (int j = 0; j < SoundSets.Length; j++)
			{
				ClipDic.Add(SoundSets[j].Category, SoundSets[j].ClipList);
			}
			BallisticChart[] array2 = Resources.LoadAll<BallisticChart>("BallisticCharts");
			for (int k = 0; k < array2.Length; k++)
			{
				Dictionary<MatBallisticType, BallisticMatSeries> dictionary;
				if (BallisticDic.ContainsKey(array2[k].ProjectileType))
				{
					dictionary = BallisticDic[array2[k].ProjectileType];
				}
				else
				{
					dictionary = new Dictionary<MatBallisticType, BallisticMatSeries>();
					BallisticDic.Add(array2[k].ProjectileType, dictionary);
				}
				for (int l = 0; l < array2[k].Mats.Count; l++)
				{
					dictionary.Add(array2[k].Mats[l].MaterialType, array2[k].Mats[l]);
				}
			}
		}

		public static PMatSoundCategory GetSoundCategoryFromMat(PMaterial mat)
		{
			return ManagerSingleton<PM>.Instance.GetSoundCategory(mat);
		}

		private PMatSoundCategory GetSoundCategory(PMaterial mat)
		{
			return MaterialSoundBindings[mat];
		}

		public static AudioClip GetRandomImpactClip(PMaterial mat)
		{
			return ManagerSingleton<PM>.Instance.GetRandomClip(mat);
		}

		private AudioClip GetRandomClip(PMaterial mat)
		{
			return ClipDic[GetSoundCategory(mat)][UnityEngine.Random.Range(0, ClipDic[GetSoundCategory(mat)].Length)];
		}

		public static BallisticMatSeries GetMatSeries(MatBallisticType matType, BallisticProjectileType projType)
		{
			return ManagerSingleton<PM>.Instance.BallisticDic[projType][matType];
		}
	}
}
