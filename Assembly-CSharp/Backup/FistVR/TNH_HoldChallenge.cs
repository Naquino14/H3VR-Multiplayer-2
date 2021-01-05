// Decompiled with JetBrains decompiler
// Type: FistVR.TNH_HoldChallenge
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New TNH_HoldChallenge", menuName = "TNH/TNH_HoldChallenge", order = 0)]
  public class TNH_HoldChallenge : ScriptableObject
  {
    public List<TNH_HoldChallenge.Phase> Phases;

    [Serializable]
    public class Phase
    {
      public TNH_EncryptionType Encryption;
      public int MinTargets;
      public int MaxTargets;
      [SearchableEnum]
      public SosigEnemyID EType;
      [SearchableEnum]
      public SosigEnemyID LType;
      public int MinEnemies;
      public int MaxEnemies;
      public float SpawnCadence;
      public int MaxEnemiesAlive;
      public int MaxDirections;
      public float ScanTime;
      public float WarmUp = 7f;
      public int IFFUsed = 1;
    }
  }
}
