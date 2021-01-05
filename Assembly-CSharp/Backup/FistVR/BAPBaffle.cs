// Decompiled with JetBrains decompiler
// Type: FistVR.BAPBaffle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class BAPBaffle : FVRPhysicalObject
  {
    [Header("Baffle")]
    public int BaffleState;
    public List<GameObject> BaffleStates;

    public void SetState(int s)
    {
      this.BaffleState = s;
      for (int index = 0; index < this.BaffleStates.Count; ++index)
      {
        if (index == this.BaffleState)
          this.BaffleStates[index].SetActive(true);
        else
          this.BaffleStates[index].SetActive(false);
      }
    }

    public override GameObject DuplicateFromSpawnLock(FVRViveHand hand)
    {
      GameObject gameObject = base.DuplicateFromSpawnLock(hand);
      gameObject.GetComponent<BAPBaffle>().SetState(this.BaffleState);
      return gameObject;
    }
  }
}
