// Decompiled with JetBrains decompiler
// Type: FistVR.wwPuzzleBox
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwPuzzleBox : wwTargetPuzzle
  {
    [Header("PuzzleBox Stuff")]
    public string PuzzleID;
    public int LockedPieces = 1;
    public wwPuzzlePiece[] Pieces;
    public AudioSource Aud;
    public AudioClip Clip_LockedInPiece;

    public override void TestSequence(string s)
    {
      if (this.PuzzleState != 1 || !(s == this.CompletionCode))
        return;
      this.PuzzleState = 2;
      this.Manager.RegisterPuzzleStateChange(this.PuzzleIndex, 2);
      this.Safe.SetState(1, true, true);
    }

    private void Update()
    {
      for (int index = 0; index < this.Pieces.Length; ++index)
      {
        if (!this.Pieces[index].IsLockedIntoPlace && this.Pieces[index].IsHeld)
        {
          float num1 = Vector3.Distance(this.transform.position, this.Pieces[index].transform.position);
          float num2 = Vector3.Angle(this.transform.up, this.Pieces[index].transform.up);
          if ((double) num1 < 0.0500000007450581 && (double) num2 < 5.0)
            this.LockPiece(this.Pieces[index], true);
        }
      }
    }

    private void LockPiece(wwPuzzlePiece p, bool StateEvent)
    {
      if (!p.IsHeld)
        return;
      FVRViveHand hand = p.m_hand;
      hand.ForceSetInteractable((FVRInteractiveObject) null);
      p.EndInteraction(hand);
      p.IsLockedIntoPlace = true;
      p.RootRigidbody.isKinematic = true;
      p.transform.position = this.transform.position;
      p.transform.rotation = this.transform.rotation;
      ++this.LockedPieces;
      if (this.LockedPieces >= this.Pieces.Length)
      {
        this.PuzzleState = 1;
        this.Manager.RegisterPuzzleStateChange(this.PuzzleIndex, 1);
      }
      if (!StateEvent)
        return;
      this.Aud.clip = this.Clip_LockedInPiece;
      this.Aud.Play();
    }

    public override void SetState(int stateIndex, int safeStateIndex)
    {
      if (stateIndex > 0)
      {
        for (int index = 0; index < this.Pieces.Length; ++index)
        {
          this.Pieces[index].IsLockedIntoPlace = true;
          this.Pieces[index].RootRigidbody.isKinematic = true;
          this.Pieces[index].transform.position = this.transform.position;
          this.Pieces[index].transform.rotation = this.transform.rotation;
          this.LockedPieces = this.Pieces.Length;
        }
      }
      base.SetState(stateIndex, safeStateIndex);
    }
  }
}
