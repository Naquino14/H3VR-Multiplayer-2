// Decompiled with JetBrains decompiler
// Type: FistVR.wwMazePuzzle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwMazePuzzle : wwTargetPuzzle
  {
    [Header("Maze Puzzle Stuff")]
    public wwMazePuzzleMarble Marble;
    public Transform Goal;
    public float DistToGoalThreshold;
    public GameObject Solution;

    public override void TestSequence(string s)
    {
      if (this.PuzzleState != 1 || !(s == this.CompletionCode))
        return;
      this.PuzzleState = 2;
      this.Manager.RegisterPuzzleStateChange(this.PuzzleIndex, 2);
      if (!((Object) this.Safe != (Object) null))
        return;
      this.Safe.SetState(1, true, true);
    }

    private void Update()
    {
      if (this.PuzzleState != 0 || !this.Marble.IsHeld || (double) Vector3.Distance(this.Marble.transform.position, this.Goal.position) >= (double) this.DistToGoalThreshold)
        return;
      this.Marble.transform.position = this.Goal.position;
      this.Marble.EndInteractionDistance = 0.0f;
      this.Marble.IsMarbleLocked = true;
      this.PuzzleState = 1;
      this.Manager.RegisterPuzzleStateChange(this.PuzzleIndex, this.PuzzleState);
      this.Solution.SetActive(true);
    }

    public override void SetState(int stateIndex, int safeStateIndex)
    {
      if (stateIndex > 0)
      {
        this.Solution.SetActive(true);
        this.Marble.transform.position = this.Goal.position;
        this.Marble.EndInteractionDistance = 0.0f;
        this.Marble.IsMarbleLocked = true;
      }
      base.SetState(stateIndex, safeStateIndex);
    }
  }
}
