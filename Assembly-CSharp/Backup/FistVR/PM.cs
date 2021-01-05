// Decompiled with JetBrains decompiler
// Type: FistVR.PM
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class PM : ManagerSingleton<PM>
  {
    public PMat PMat_Air;
    public PMat PMat_Default;
    public PM.HitSoundSet[] SoundSets;
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
      this.MaterialSoundBindings = new Dictionary<PMaterial, PMatSoundCategory>();
      this.ClipDic = new Dictionary<PMatSoundCategory, AudioClip[]>();
      this.GenerateMaterialDB();
    }

    private void GenerateMaterialDB()
    {
      PMaterialDefinition[] pmaterialDefinitionArray = Resources.LoadAll<PMaterialDefinition>("PMaterialDefinitions");
      for (int index = 0; index < pmaterialDefinitionArray.Length; ++index)
        this.MaterialSoundBindings.Add(pmaterialDefinitionArray[index].material, pmaterialDefinitionArray[index].soundCategory);
      for (int index = 0; index < this.SoundSets.Length; ++index)
        this.ClipDic.Add(this.SoundSets[index].Category, this.SoundSets[index].ClipList);
      BallisticChart[] ballisticChartArray = Resources.LoadAll<BallisticChart>("BallisticCharts");
      for (int index1 = 0; index1 < ballisticChartArray.Length; ++index1)
      {
        Dictionary<MatBallisticType, BallisticMatSeries> dictionary;
        if (this.BallisticDic.ContainsKey(ballisticChartArray[index1].ProjectileType))
        {
          dictionary = this.BallisticDic[ballisticChartArray[index1].ProjectileType];
        }
        else
        {
          dictionary = new Dictionary<MatBallisticType, BallisticMatSeries>();
          this.BallisticDic.Add(ballisticChartArray[index1].ProjectileType, dictionary);
        }
        for (int index2 = 0; index2 < ballisticChartArray[index1].Mats.Count; ++index2)
          dictionary.Add(ballisticChartArray[index1].Mats[index2].MaterialType, ballisticChartArray[index1].Mats[index2]);
      }
    }

    public static PMatSoundCategory GetSoundCategoryFromMat(PMaterial mat) => ManagerSingleton<PM>.Instance.GetSoundCategory(mat);

    private PMatSoundCategory GetSoundCategory(PMaterial mat) => this.MaterialSoundBindings[mat];

    public static AudioClip GetRandomImpactClip(PMaterial mat) => ManagerSingleton<PM>.Instance.GetRandomClip(mat);

    private AudioClip GetRandomClip(PMaterial mat) => this.ClipDic[this.GetSoundCategory(mat)][UnityEngine.Random.Range(0, this.ClipDic[this.GetSoundCategory(mat)].Length)];

    public static BallisticMatSeries GetMatSeries(
      MatBallisticType matType,
      BallisticProjectileType projType)
    {
      return ManagerSingleton<PM>.Instance.BallisticDic[projType][matType];
    }

    [Serializable]
    public class HitSoundSet
    {
      public PMatSoundCategory Category;
      public AudioClip[] ClipList;
    }
  }
}
