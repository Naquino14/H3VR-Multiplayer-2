// Decompiled with JetBrains decompiler
// Type: FistVR.TNH_Progression
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New TNH_Progression", menuName = "TNH/TNH_Progression", order = 0)]
  public class TNH_Progression : ScriptableObject
  {
    public List<TNH_Progression.Level> Levels;

    [Serializable]
    public class Level
    {
      public TNH_TakeChallenge TakeChallenge;
      public TNH_HoldChallenge HoldChallenge;
      public TNH_TakeChallenge SupplyChallenge;
      public TNH_PatrolChallenge PatrolChallenge;
      public TNH_TrapsChallenge TrapsChallenge;
      public int NumOverrideTokensForHold;
    }
  }
}
