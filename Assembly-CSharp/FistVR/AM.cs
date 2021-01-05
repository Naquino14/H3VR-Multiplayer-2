// Decompiled with JetBrains decompiler
// Type: FistVR.AM
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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
      this.GenerateFireArmRoundDictionaries();
    }

    public static float GetFireArmMechanicalSpread(FVRFireArmMechanicalAccuracyClass c) => Random.Range(AM.SMechanicalAccuracyDic[c].MinDegrees, AM.SMechanicalAccuracyDic[c].MaxDegrees) * 0.5f;

    public static float GetDropMult(FVRFireArmMechanicalAccuracyClass c) => AM.SMechanicalAccuracyDic[c].DropMult;

    public static float GetDriftMult(FVRFireArmMechanicalAccuracyClass c) => AM.SMechanicalAccuracyDic[c].DriftMult;

    public static float GetRecoilMult(FVRFireArmMechanicalAccuracyClass c) => AM.SMechanicalAccuracyDic[c].RecoilMult;

    public static float GetChamberVelMult(FireArmRoundType rt, float lengthMeters) => AM.SRoundDisplayDataDic[rt].VelMultByBarrelLengthCurve.Evaluate(lengthMeters * 39.3701f);

    private void GenerateFireArmRoundDictionaries()
    {
      FVRFireArmRoundDisplayData[] roundDisplayDataArray = Resources.LoadAll<FVRFireArmRoundDisplayData>("FVRFireArmRoundDisplayData");
      for (int index1 = 0; index1 < roundDisplayDataArray.Length; ++index1)
      {
        List<FireArmRoundClass> fireArmRoundClassList1 = new List<FireArmRoundClass>();
        Dictionary<FireArmRoundClass, FVRFireArmRoundDisplayData.DisplayDataClass> dictionary;
        List<FireArmRoundClass> fireArmRoundClassList2;
        if (this.TypeDic.ContainsKey(roundDisplayDataArray[index1].Type))
        {
          dictionary = this.TypeDic[roundDisplayDataArray[index1].Type];
          fireArmRoundClassList2 = this.TypeClassLists[roundDisplayDataArray[index1].Type];
        }
        else
        {
          this.TypeList.Add(roundDisplayDataArray[index1].Type);
          dictionary = new Dictionary<FireArmRoundClass, FVRFireArmRoundDisplayData.DisplayDataClass>();
          this.TypeDic.Add(roundDisplayDataArray[index1].Type, dictionary);
          fireArmRoundClassList2 = new List<FireArmRoundClass>();
          this.TypeClassLists.Add(roundDisplayDataArray[index1].Type, fireArmRoundClassList2);
          this.RoundDisplayDataDic.Add(roundDisplayDataArray[index1].Type, roundDisplayDataArray[index1]);
        }
        for (int index2 = 0; index2 < roundDisplayDataArray[index1].Classes.Length; ++index2)
        {
          dictionary.Add(roundDisplayDataArray[index1].Classes[index2].Class, roundDisplayDataArray[index1].Classes[index2]);
          fireArmRoundClassList2.Add(roundDisplayDataArray[index1].Classes[index2].Class);
        }
      }
      for (int index = 0; index < this.AccuracyChart.Entries.Count; ++index)
        this.MechanicalAccuracyDic.Add(this.AccuracyChart.Entries[index].Class, this.AccuracyChart.Entries[index]);
    }

    public static Material GetRoundMaterial(
      FireArmRoundType rType,
      FireArmRoundClass rClass)
    {
      return ManagerSingleton<AM>.Instance.getRoundMaterial(rType, rClass);
    }

    public Material getRoundMaterial(FireArmRoundType rType, FireArmRoundClass rClass) => ManagerSingleton<AM>.Instance.TypeDic[rType][rClass].Material;

    public static Mesh GetRoundMesh(FireArmRoundType rType, FireArmRoundClass rClass) => ManagerSingleton<AM>.Instance.getRoundMesh(rType, rClass);

    public Mesh getRoundMesh(FireArmRoundType rType, FireArmRoundClass rClass) => ManagerSingleton<AM>.Instance.TypeDic[rType][rClass].Mesh;

    public static FVRObject GetRoundSelfPrefab(
      FireArmRoundType rType,
      FireArmRoundClass rClass)
    {
      return ManagerSingleton<AM>.Instance.getRoundSelfPrefab(rType, rClass);
    }

    public FVRObject getRoundSelfPrefab(FireArmRoundType rType, FireArmRoundClass rClass) => ManagerSingleton<AM>.Instance.TypeDic[rType][rClass].ObjectID;

    public static FireArmRoundClass GetRandomValidRoundClass(FireArmRoundType rType) => ManagerSingleton<AM>.Instance.getRandomValidRoundClass(rType);

    public static FireArmRoundClass GetRandomNonDefaultRoundClass(
      FireArmRoundType rType)
    {
      return ManagerSingleton<AM>.Instance.getRandomNonDefaultRoundClass(rType);
    }

    public static FireArmRoundClass GetDefaultRoundClass(FireArmRoundType rType) => ManagerSingleton<AM>.Instance.getDefaultRoundClass(rType);

    public FireArmRoundClass getRandomValidRoundClass(FireArmRoundType rType) => AM.STypeClassLists[rType][Random.Range(0, AM.STypeClassLists[rType].Count)];

    public FireArmRoundClass getDefaultRoundClass(FireArmRoundType rType) => AM.STypeClassLists[rType][0];

    public FireArmRoundClass getRandomNonDefaultRoundClass(FireArmRoundType rType) => AM.STypeClassLists[rType].Count > 1 ? AM.STypeClassLists[rType][Random.Range(1, AM.STypeClassLists[rType].Count)] : this.getDefaultRoundClass(rType);

    public static bool DoesClassExistForType(FireArmRoundClass rClass, FireArmRoundType rType) => ManagerSingleton<AM>.Instance.doesClassExistForType(rClass, rType);

    public bool doesClassExistForType(FireArmRoundClass rClass, FireArmRoundType rType) => ManagerSingleton<AM>.Instance.TypeDic[rType].ContainsKey(rClass);

    public static FVRObject.OTagFirearmRoundPower GetRoundPower(FireArmRoundType rType) => ManagerSingleton<AM>.Instance.getRoundPower(rType);

    public FVRObject.OTagFirearmRoundPower getRoundPower(FireArmRoundType rType) => this.RoundDisplayDataDic[rType].RoundPower;

    public static string GetFullRoundName(FireArmRoundType rType, FireArmRoundClass rClass)
    {
      FVRFireArmRoundDisplayData.DisplayDataClass displayClass = AM.SRoundDisplayDataDic[rType].GetDisplayClass(rClass);
      string str = string.Empty;
      if (displayClass != null)
        str = displayClass.Name;
      return AM.SRoundDisplayDataDic[rType].DisplayName + "\n" + str;
    }
  }
}
