// Decompiled with JetBrains decompiler
// Type: FistVR.TNH_SafePositionMatrix
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New SafePosMatrix Def", menuName = "TNH/SafePosMatrix", order = 0)]
  public class TNH_SafePositionMatrix : ScriptableObject
  {
    public List<TNH_SafePositionMatrix.PositionEntry> Entries_HoldPoints;
    public List<TNH_SafePositionMatrix.PositionEntry> Entries_SupplyPoints;

    [ContextMenu("GenereateSupplyPointsData")]
    public void GenereateSupplyPointsData()
    {
      this.Entries_SupplyPoints.Clear();
      int count1 = this.Entries_HoldPoints.Count;
      int count2 = this.Entries_HoldPoints[0].SafePositions_SupplyPoints.Count;
      for (int index1 = 0; index1 < count2; ++index1)
      {
        TNH_SafePositionMatrix.PositionEntry positionEntry = new TNH_SafePositionMatrix.PositionEntry();
        for (int index2 = 0; index2 < count1; ++index2)
        {
          bool positionsSupplyPoint = this.Entries_HoldPoints[index2].SafePositions_SupplyPoints[index1];
          positionEntry.SafePositions_HoldPoints.Add(positionsSupplyPoint);
        }
        this.Entries_SupplyPoints.Add(positionEntry);
      }
    }

    [ContextMenu("Fix1")]
    public void Fix1()
    {
      for (int index1 = 0; index1 < this.Entries_SupplyPoints.Count; ++index1)
      {
        for (int index2 = 0; index2 < this.Entries_SupplyPoints[index1].SafePositions_SupplyPoints.Count; ++index2)
          this.Entries_SupplyPoints[index1].SafePositions_SupplyPoints[index2] = true;
      }
    }

    [ContextMenu("Fix2")]
    public void Fix2()
    {
      for (int index1 = 0; index1 < this.Entries_HoldPoints.Count; ++index1)
      {
        for (int index2 = 0; index2 < this.Entries_HoldPoints[index1].SafePositions_HoldPoints.Count; ++index2)
        {
          if (index1 == index2)
            this.Entries_HoldPoints[index1].SafePositions_HoldPoints[index2] = false;
        }
      }
      for (int index1 = 0; index1 < this.Entries_SupplyPoints.Count; ++index1)
      {
        for (int index2 = 0; index2 < this.Entries_SupplyPoints[index1].SafePositions_SupplyPoints.Count; ++index2)
        {
          if (index1 == index2)
            this.Entries_SupplyPoints[index1].SafePositions_SupplyPoints[index2] = false;
        }
      }
    }

    [Serializable]
    public class PositionEntry
    {
      public List<bool> SafePositions_HoldPoints = new List<bool>();
      public List<bool> SafePositions_SupplyPoints = new List<bool>();
    }
  }
}
