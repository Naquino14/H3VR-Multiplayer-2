// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFireArmMechanicalAccuracyChart
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New Accuacy Chart", menuName = "Ballistics/FireArmMechanicalAccuracyChart", order = 0)]
  public class FVRFireArmMechanicalAccuracyChart : ScriptableObject
  {
    public List<FVRFireArmMechanicalAccuracyChart.MechanicalAccuracyEntry> Entries = new List<FVRFireArmMechanicalAccuracyChart.MechanicalAccuracyEntry>();

    [ContextMenu("Calc")]
    public void Calc()
    {
      for (int index = 0; index < this.Entries.Count; ++index)
      {
        this.Entries[index].MinDegrees = this.Entries[index].MinMOA * 0.0166667f;
        this.Entries[index].MaxDegrees = this.Entries[index].MaxMOA * 0.0166667f;
      }
    }

    [Serializable]
    public class MechanicalAccuracyEntry
    {
      public FVRFireArmMechanicalAccuracyClass Class;
      public float MinMOA;
      public float MaxMOA;
      public float MinDegrees;
      public float MaxDegrees;
      public float DropMult;
      public float DriftMult;
      public float RecoilMult = 1f;
    }
  }
}
