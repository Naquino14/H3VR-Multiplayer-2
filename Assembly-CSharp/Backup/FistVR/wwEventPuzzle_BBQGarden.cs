// Decompiled with JetBrains decompiler
// Type: FistVR.wwEventPuzzle_BBQGarden
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

namespace FistVR
{
  public class wwEventPuzzle_BBQGarden : wwEventPuzzle
  {
    private List<int> m_lastStruckForks = new List<int>();

    public override void SetState(int stateIndex) => base.SetState(stateIndex);

    public void ForkHit(int index)
    {
      if (this.m_lastStruckForks.Count >= 3)
        this.m_lastStruckForks.RemoveAt(0);
      this.m_lastStruckForks.Add(index);
    }

    private void Update()
    {
      if (this.m_lastStruckForks.Count < 3 || this.m_lastStruckForks[0] != 0 || (this.m_lastStruckForks[1] != 1 || this.m_lastStruckForks[2] != 2))
        return;
      this.SolvePuzzle();
    }

    private void SolvePuzzle()
    {
      if (this.PuzzleState != 0)
        return;
      this.PuzzleState = 1;
      this.ParkManager.RegisterEventPuzzleChange(this.PuzzleIndex, 1);
      if (this.ParkManager.RewardChests[this.ChestIndex].GetState() >= 1)
        return;
      this.ParkManager.UnlockChest(this.ChestIndex);
    }
  }
}
