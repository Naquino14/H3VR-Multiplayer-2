// Decompiled with JetBrains decompiler
// Type: FistVR.OmniSequencerWaveDefinition
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New Wave Def", menuName = "OmniSequencer/WaveDefinition", order = 0)]
  public class OmniSequencerWaveDefinition : ScriptableObject
  {
    public float TimeForWarmup;
    public float TimeForWarmupRandomAddition;
    public float TimeForWave;
    public float ScoreMultiplier_Points = 1f;
    public float ScoreMultiplier_Time = 1f;
    public float ScoreMultiplier_Range = 1f;
    [Space(10f)]
    public bool UsesQuickDraw;
    public bool UsesReflex;
    [Space(10f)]
    public List<OmniSequencerWaveDefinition.SpawnerAtRange> Spawners;

    public List<int> CalculateScoreThresholdsForWave()
    {
      List<int> intList1 = new List<int>(3);
      for (int index = 0; index < 3; ++index)
        intList1.Add(0);
      for (int index1 = 0; index1 < this.Spawners.Count; ++index1)
      {
        List<int> spawnerScoreThresholds = this.Spawners[index1].Def.CalculateSpawnerScoreThresholds();
        for (int index2 = 0; index2 < spawnerScoreThresholds.Count; ++index2)
        {
          float num1 = (float) spawnerScoreThresholds[index2] * this.ScoreMultiplier_Points;
          float num2 = num1 + num1 * this.ScoreMultiplier_Range * this.RangeToScoreMultiplier(this.Spawners[index1].Range);
          List<int> intList2;
          int index3;
          (intList2 = intList1)[index3 = index2] = intList2[index3] + (int) num2;
        }
      }
      int num = (int) ((double) this.TimeForWave * 0.200000002980232 * 100.0 * (double) this.ScoreMultiplier_Time);
      intList1[0] = (int) ((double) intList1[0] * 0.899999976158142);
      intList1[1] = (int) ((double) intList1[1] * 0.899999976158142);
      intList1[2] = (int) ((double) intList1[2] * 0.899999976158142);
      return intList1;
    }

    public float RangeToScoreMultiplier(OmniWaveEngagementRange range)
    {
      switch (range)
      {
        case OmniWaveEngagementRange.m5:
          return 0.0f;
        case OmniWaveEngagementRange.m10:
          return 0.1f;
        case OmniWaveEngagementRange.m15:
          return 0.25f;
        case OmniWaveEngagementRange.m20:
          return 0.5f;
        case OmniWaveEngagementRange.m25:
          return 1f;
        case OmniWaveEngagementRange.m50:
          return 2f;
        case OmniWaveEngagementRange.m100:
          return 3f;
        case OmniWaveEngagementRange.m150:
          return 4f;
        case OmniWaveEngagementRange.m200:
          return 5f;
        default:
          return 1f;
      }
    }

    [Serializable]
    public class SpawnerAtRange
    {
      public OmniSpawnDef Def;
      public OmniWaveEngagementRange Range;
    }
  }
}
