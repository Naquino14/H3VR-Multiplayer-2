// Decompiled with JetBrains decompiler
// Type: FistVR.wwSpinnyPuzzle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwSpinnyPuzzle : wwTargetPuzzle
  {
    [Header("Spinnypuzzle Stuff")]
    public int LockedPieces;
    public int NumLockedPiecesRequired = 2;
    public wwSpinnyRing[] Rings;
    public AudioSource Aud;
    public AudioClip Clip_LockedInPiece;
    public Renderer[] Rends;
    public Texture2D CompletedTexture;

    public override void TestSequence(string s)
    {
      if (this.PuzzleState != 1 || !(s == this.CompletionCode))
        return;
      this.PuzzleState = 2;
      this.Manager.RegisterPuzzleStateChange(this.PuzzleIndex, this.PuzzleState);
      if (!((Object) this.Safe != (Object) null))
        return;
      this.Safe.SetState(1, true, true);
    }

    private void Update()
    {
      if (this.LockedPieces >= this.NumLockedPiecesRequired && this.PuzzleState == 0)
      {
        this.PuzzleState = 1;
        this.Manager.RegisterPuzzleStateChange(this.PuzzleIndex, this.PuzzleState);
        for (int index = 0; index < this.Rends.Length; ++index)
          this.Rends[index].material.SetTexture("_DecalTex", (Texture) this.CompletedTexture);
      }
      if (this.PuzzleState != 2 || this.Safe.SafeState != 0 || !((Object) this.Safe != (Object) null))
        return;
      this.Safe.SetState(1, true, true);
    }

    public void PieceLocked() => ++this.LockedPieces;

    public override void SetState(int stateIndex, int safeStateIndex)
    {
      if (stateIndex > 0)
      {
        for (int index = 0; index < this.Rings.Length; ++index)
          this.Rings[index].LockPiece(false);
        this.LockedPieces = this.NumLockedPiecesRequired;
        for (int index = 0; index < this.Rends.Length; ++index)
          this.Rends[index].material.SetTexture("_DecalTex", (Texture) this.CompletedTexture);
      }
      base.SetState(stateIndex, safeStateIndex);
    }
  }
}
