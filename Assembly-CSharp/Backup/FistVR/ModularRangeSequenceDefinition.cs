// Decompiled with JetBrains decompiler
// Type: FistVR.ModularRangeSequenceDefinition
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New Definition", menuName = "ModularRange/SequenceDefinition", order = 0)]
  public class ModularRangeSequenceDefinition : ScriptableObject
  {
    public ModularRangeSequenceDefinition.SequenceMetaData MetaData;
    public ModularRangeSequenceDefinition.WaveDefinition[] Waves;

    public enum TargetLayout
    {
      HorizontalLeft,
      HorizontalRight,
      VerticalUp,
      VerticalDown,
      DiagonalLeftUp,
      DiagonalRightUp,
      DiagonalLeftDown,
      DiagonalRightDown,
      SquareUp,
      SquareDown,
      CircleClockWise,
      CircleCounterClockWise,
    }

    public enum TargetTiming
    {
      SequentialOnHit,
      SequentialTimed,
      RandomOnHit,
      RandomTimed,
      Flood,
    }

    public enum TargetMovementStyle
    {
      Static,
      SinusoidX,
      SinusoidY,
      WhipX,
      WhipY,
      WhipZ,
      TowardCenterWhipZ,
      RandomDir,
      TowardCenter,
      WhipXWhipZ,
      WhipYWhipZ,
    }

    [Serializable]
    public class WaveDefinition
    {
      public GameObject[] TargetPrefabs;
      public GameObject[] NoShootTargetPrefabs;
      public ModularRangeSequenceDefinition.TargetLayout Layout;
      public ModularRangeSequenceDefinition.TargetTiming Timing;
      public ModularRangeSequenceDefinition.TargetMovementStyle MovementStyle;
      public int TargetNum;
      public int NumNoShootTarget;
      public float Distance;
      public float EndDistance;
      public float TimeForWave;
      public float TimePerTarget;
      public float DelayPerTarget;
      public float TimeForReload;
    }

    public enum SequenceCategory
    {
      Reflex,
      Recognition,
      Precision,
      Cognition,
    }

    public enum SequenceDifficulty
    {
      Easy,
      Intermediate,
      Advanced,
      Nightmarish,
    }

    public enum SequenceRange
    {
      Short,
      Medium,
      Long,
      Mixed,
    }

    [Serializable]
    public class SequenceMetaData
    {
      public string DisplayName;
      public string EndPointName;
      public ModularRangeSequenceDefinition.SequenceCategory Category;
      public ModularRangeSequenceDefinition.SequenceDifficulty Difficulty;
      public string Capacity;
      public int WaveCount;
      public ModularRangeSequenceDefinition.SequenceRange Range;
    }
  }
}
