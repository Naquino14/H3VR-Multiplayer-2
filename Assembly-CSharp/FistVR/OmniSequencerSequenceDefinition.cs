// Decompiled with JetBrains decompiler
// Type: FistVR.OmniSequencerSequenceDefinition
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New Sequence Def", menuName = "OmniSequencer/SequenceDefinition", order = 0)]
  public class OmniSequencerSequenceDefinition : ScriptableObject
  {
    public string SequenceName;
    [Space(10f)]
    public string SequenceID;
    [Space(10f)]
    [Multiline(8)]
    public string Description;
    [Space(10f)]
    public OmniSequencerSequenceDefinition.OmniSequenceTheme Theme;
    [Space(10f)]
    public OmniSequencerSequenceDefinition.OmniSequenceDifficulty Difficulty;
    [Space(10f)]
    public List<int> ScoreThresholds = new List<int>();
    [Space(10f)]
    public List<int> CurrencyForRanks = new List<int>();
    [Space(10f)]
    public List<OmniSequencerWaveDefinition> Waves;
    public OmniSequencerSequenceDefinition.OmniSequenceFirearmMode FirearmMode;
    public OmniSequencerSequenceDefinition.OmniSequenceAmmoMode AmmoMode;
    public List<ItemSpawnerID.ESubCategory> AllowedCats = new List<ItemSpawnerID.ESubCategory>();

    public int GetRankForScore(int score)
    {
      for (int index = 0; index < this.ScoreThresholds.Count; ++index)
      {
        if (score >= this.ScoreThresholds[index])
          return index;
      }
      return 3;
    }

    [ContextMenu("CalculateScoreThreshold")]
    public void CalculateScoreThreshold()
    {
      List<int> intList1 = new List<int>(3);
      for (int index = 0; index < 3; ++index)
        intList1.Add(0);
      for (int index1 = 0; index1 < this.Waves.Count; ++index1)
      {
        List<int> thresholdsForWave = this.Waves[index1].CalculateScoreThresholdsForWave();
        for (int index2 = 0; index2 < thresholdsForWave.Count; ++index2)
        {
          List<int> intList2;
          int index3;
          (intList2 = intList1)[index3 = index2] = intList2[index3] + thresholdsForWave[index2];
        }
      }
      this.ScoreThresholds.Clear();
      this.ScoreThresholds.Add(intList1[0]);
      this.ScoreThresholds.Add(intList1[1]);
      this.ScoreThresholds.Add(intList1[2]);
      Debug.Log((object) ("Gold Score Needed:" + (object) intList1[0]));
      Debug.Log((object) ("Silver Score Needed:" + (object) intList1[1]));
      Debug.Log((object) ("Bronze Score Needed:" + (object) intList1[2]));
    }

    public enum OmniSequenceTheme
    {
      GettingStarted,
      CasualPlinking,
      OpenSeason,
      LightningReflexes,
      SixShooter,
      MentalGymnastics,
      SprayAndPray,
      DownRange,
    }

    public enum OmniSequenceDifficulty
    {
      Beginner,
      Intermediate,
      Advanced,
      Master,
      Impossible,
    }

    public enum OmniSequenceFirearmMode
    {
      Open,
      Category,
      Fixed,
    }

    public enum OmniSequenceAmmoMode
    {
      Infinite,
      Spawnlockable,
      Fixed,
    }
  }
}
