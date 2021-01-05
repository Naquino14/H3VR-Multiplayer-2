// Decompiled with JetBrains decompiler
// Type: FistVR.wwEventPuzzle_BootLeather
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwEventPuzzle_BootLeather : wwEventPuzzle
  {
    public Transform ArrowPiece1;
    public Transform ArrowPiece2;
    public Rigidbody RB;

    public override void SetState(int stateIndex) => base.SetState(stateIndex);

    public void Update()
    {
      if ((double) this.RB.angularVelocity.magnitude >= 0.25 || (double) Vector3.Angle(this.ArrowPiece1.forward, this.ArrowPiece2.forward) >= 3.5)
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
