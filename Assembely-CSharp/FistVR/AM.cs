using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class AM : ManagerSingleton<AM>
	{
		private Dictionary<FireArmRoundType, Dictionary<FireArmRoundClass, FVRFireArmRoundDisplayData.DisplayDataClass>> TypeDic = new Dictionary<FireArmRoundType, Dictionary<FireArmRoundClass, FVRFireArmRoundDisplayData.DisplayDataClass>>();

		private List<FireArmRoundType> TypeList = new List<FireArmRoundType>();

		private Dictionary<FireArmRoundType, List<FireArmRoundClass>> TypeClassLists = new Dictionary<FireArmRoundType, List<FireArmRoundClass>>();

		private Dictionary<FireArmRoundType, FVRFireArmRoundDisplayData> RoundDisplayDataDic = new Dictionary<FireArmRoundType, FVRFireArmRoundDisplayData>();

		public LayerMask ProjectileLayerMask;

		public AnimationCurve BulletDragCoefficientCurve;

		public GameObject Prefab_OpticUI;

		public FVRFireArmMechanicalAccuracyChart AccuracyChart;

		private Dictionary<FVRFireArmMechanicalAccuracyClass, FVRFireArmMechanicalAccuracyChart.MechanicalAccuracyEntry> MechanicalAccuracyDic = new Dictionary<FVRFireArmMechanicalAccuracyClass, FVRFireArmMechanicalAccuracyChart.MechanicalAccuracyEntry>();

		public static Dictionary<FireArmRoundType, Dictionary<FireArmRoundClass, FVRFireArmRoundDisplayData.DisplayDataClass>> STypeDic => ManagerSingleton<AM>.Instance.TypeDic;

		public static List<FireArmRoundType> STypeList => ManagerSingleton<AM>.Instance.TypeList;

		public static Dictionary<FireArmRoundType, List<FireArmRoundClass>> STypeClassLists => ManagerSingleton<AM>.Instance.TypeClassLists;

		public static Dictionary<FireArmRoundType, FVRFireArmRoundDisplayData> SRoundDisplayDataDic => ManagerSingleton<AM>.Instance.RoundDisplayDataDic;

		public static LayerMask PLM => ManagerSingleton<AM>.Instance.ProjectileLayerMask;

		public static AnimationCurve BDCC => ManagerSingleton<AM>.Instance.BulletDragCoefficientCurve;

		public static Dictionary<FVRFireArmMechanicalAccuracyClass, FVRFireArmMechanicalAccuracyChart.MechanicalAccuracyEntry> SMechanicalAccuracyDic => ManagerSingleton<AM>.Instance.MechanicalAccuracyDic;

		protected override void Awake()
		{
			base.Awake();
			GenerateFireArmRoundDictionaries();
		}

		public static float GetFireArmMechanicalSpread(FVRFireArmMechanicalAccuracyClass c)
		{
			return Random.Range(SMechanicalAccuracyDic[c].MinDegrees, SMechanicalAccuracyDic[c].MaxDegrees) * 0.5f;
		}

		public static float GetDropMult(FVRFireArmMechanicalAccuracyClass c)
		{
			return SMechanicalAccuracyDic[c].DropMult;
		}

		public static float GetDriftMult(FVRFireArmMechanicalAccuracyClass c)
		{
			return SMechanicalAccuracyDic[c].DriftMult;
		}

		public static float GetRecoilMult(FVRFireArmMechanicalAccuracyClass c)
		{
			return SMechanicalAccuracyDic[c].RecoilMult;
		}

		public static float GetChamberVelMult(FireArmRoundType rt, float lengthMeters)
		{
			FVRFireArmRoundDisplayData fVRFireArmRoundDisplayData = SRoundDisplayDataDic[rt];
			float time = lengthMeters * 39.3701f;
			return fVRFireArmRoundDisplayData.VelMultByBarrelLengthCurve.Evaluate(time);
		}

		private void GenerateFireArmRoundDictionaries()
		{
			FVRFireArmRoundDisplayData[] array = Resources.LoadAll<FVRFireArmRoundDisplayData>("FVRFireArmRoundDisplayData");
			for (int i = 0; i < array.Length; i++)
			{
				List<FireArmRoundClass> list = new List<FireArmRoundClass>();
				Dictionary<FireArmRoundClass, FVRFireArmRoundDisplayData.DisplayDataClass> dictionary;
				if (TypeDic.ContainsKey(array[i].Type))
				{
					dictionary = TypeDic[array[i].Type];
					list = TypeClassLists[array[i].Type];
				}
				else
				{
					TypeList.Add(array[i].Type);
					dictionary = new Dictionary<FireArmRoundClass, FVRFireArmRoundDisplayData.DisplayDataClass>();
					TypeDic.Add(array[i].Type, dictionary);
					list = new List<FireArmRoundClass>();
					TypeClassLists.Add(array[i].Type, list);
					RoundDisplayDataDic.Add(array[i].Type, array[i]);
				}
				for (int j = 0; j < array[i].Classes.Length; j++)
				{
					dictionary.Add(array[i].Classes[j].Class, array[i].Classes[j]);
					list.Add(array[i].Classes[j].Class);
				}
			}
			for (int k = 0; k < AccuracyChart.Entries.Count; k++)
			{
				MechanicalAccuracyDic.Add(AccuracyChart.Entries[k].Class, AccuracyChart.Entries[k]);
			}
		}

		public static Material GetRoundMaterial(FireArmRoundType rType, FireArmRoundClass rClass)
		{
			return ManagerSingleton<AM>.Instance.getRoundMaterial(rType, rClass);
		}

		public Material getRoundMaterial(FireArmRoundType rType, FireArmRoundClass rClass)
		{
			return ManagerSingleton<AM>.Instance.TypeDic[rType][rClass].Material;
		}

		public static Mesh GetRoundMesh(FireArmRoundType rType, FireArmRoundClass rClass)
		{
			return ManagerSingleton<AM>.Instance.getRoundMesh(rType, rClass);
		}

		public Mesh getRoundMesh(FireArmRoundType rType, FireArmRoundClass rClass)
		{
			return ManagerSingleton<AM>.Instance.TypeDic[rType][rClass].Mesh;
		}

		public static FVRObject GetRoundSelfPrefab(FireArmRoundType rType, FireArmRoundClass rClass)
		{
			return ManagerSingleton<AM>.Instance.getRoundSelfPrefab(rType, rClass);
		}

		public FVRObject getRoundSelfPrefab(FireArmRoundType rType, FireArmRoundClass rClass)
		{
			return ManagerSingleton<AM>.Instance.TypeDic[rType][rClass].ObjectID;
		}

		public static FireArmRoundClass GetRandomValidRoundClass(FireArmRoundType rType)
		{
			return ManagerSingleton<AM>.Instance.getRandomValidRoundClass(rType);
		}

		public static FireArmRoundClass GetRandomNonDefaultRoundClass(FireArmRoundType rType)
		{
			return ManagerSingleton<AM>.Instance.getRandomNonDefaultRoundClass(rType);
		}

		public static FireArmRoundClass GetDefaultRoundClass(FireArmRoundType rType)
		{
			return ManagerSingleton<AM>.Instance.getDefaultRoundClass(rType);
		}

		public FireArmRoundClass getRandomValidRoundClass(FireArmRoundType rType)
		{
			return STypeClassLists[rType][Random.Range(0, STypeClassLists[rType].Count)];
		}

		public FireArmRoundClass getDefaultRoundClass(FireArmRoundType rType)
		{
			return STypeClassLists[rType][0];
		}

		public FireArmRoundClass getRandomNonDefaultRoundClass(FireArmRoundType rType)
		{
			if (STypeClassLists[rType].Count > 1)
			{
				return STypeClassLists[rType][Random.Range(1, STypeClassLists[rType].Count)];
			}
			return getDefaultRoundClass(rType);
		}

		public static bool DoesClassExistForType(FireArmRoundClass rClass, FireArmRoundType rType)
		{
			return ManagerSingleton<AM>.Instance.doesClassExistForType(rClass, rType);
		}

		public bool doesClassExistForType(FireArmRoundClass rClass, FireArmRoundType rType)
		{
			return ManagerSingleton<AM>.Instance.TypeDic[rType].ContainsKey(rClass);
		}

		public static FVRObject.OTagFirearmRoundPower GetRoundPower(FireArmRoundType rType)
		{
			return ManagerSingleton<AM>.Instance.getRoundPower(rType);
		}

		public FVRObject.OTagFirearmRoundPower getRoundPower(FireArmRoundType rType)
		{
			return RoundDisplayDataDic[rType].RoundPower;
		}

		public static string GetFullRoundName(FireArmRoundType rType, FireArmRoundClass rClass)
		{
			FVRFireArmRoundDisplayData.DisplayDataClass displayClass = SRoundDisplayDataDic[rType].GetDisplayClass(rClass);
			string text = string.Empty;
			if (displayClass != null)
			{
				text = displayClass.Name;
			}
			return SRoundDisplayDataDic[rType].DisplayName + "\n" + text;
		}
	}
}
