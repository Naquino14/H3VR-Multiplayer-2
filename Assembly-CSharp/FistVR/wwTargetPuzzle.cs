// Decompiled with JetBrains decompiler
// Type: FistVR.wwTargetPuzzle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwTargetPuzzle : MonoBehaviour
  {
    public wwTargetManager Manager;
    public string CompletionCode;
    public int PuzzleIndex;
    public int PuzzleState;
    public wwBankSafe Safe;

    public virtual void TestSequence(string s)
    {
    }

    public virtual void SetState(int stateIndex, int safeStateIndex)
    {
      this.PuzzleState = stateIndex;
      this.Safe.SetState(safeStateIndex, false, false);
    }
  }
}
