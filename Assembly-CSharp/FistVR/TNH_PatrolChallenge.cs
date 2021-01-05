// Decompiled with JetBrains decompiler
// Type: FistVR.TNH_PatrolChallenge
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New TNH_PatrolChallenge", menuName = "TNH/TNH_PatrolChallenge", order = 0)]
  public class TNH_PatrolChallenge : ScriptableObject
  {
    public List<TNH_PatrolChallenge.Patrol> Patrols;

    [Serializable]
    public class Patrol
    {
      [SearchableEnum]
      public SosigEnemyID EType;
      [SearchableEnum]
      public SosigEnemyID LType;
      public int PatrolSize;
      public int MaxPatrols;
      public int MaxPatrols_LimitedAmmo;
      public float TimeTilRegen;
      public float TimeTilRegen_LimitedAmmo;
      public int IFFUsed = 1;
    }
  }
}
