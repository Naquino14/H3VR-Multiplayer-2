// Decompiled with JetBrains decompiler
// Type: FistVR.wwEventPuzzle_Churner
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwEventPuzzle_Churner : wwEventPuzzle
  {
    public GameObject ButterEatingSoundPrefab;
    private bool[] eatenButter = new bool[7];

    public void AteButter(int index, Vector3 v)
    {
      this.eatenButter[index] = true;
      if (!((Object) this.ButterEatingSoundPrefab != (Object) null))
        return;
      Object.Instantiate<GameObject>(this.ButterEatingSoundPrefab, v, Quaternion.identity);
    }

    public void Update()
    {
      if (this.PuzzleState != 0)
        return;
      bool flag = true;
      for (int index = 0; index < this.eatenButter.Length; ++index)
      {
        if (!this.eatenButter[index])
          flag = false;
      }
      if (!flag)
        return;
      this.PuzzleState = 1;
      this.ParkManager.RegisterEventPuzzleChange(this.PuzzleIndex, 1);
      if (this.ParkManager.RewardChests[this.ChestIndex].GetState() >= 1)
        return;
      this.ParkManager.UnlockChest(this.ChestIndex);
    }
  }
}
