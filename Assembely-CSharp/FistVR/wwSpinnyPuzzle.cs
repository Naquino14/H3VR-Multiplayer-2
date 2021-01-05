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
			if (PuzzleState == 1 && s == CompletionCode)
			{
				PuzzleState = 2;
				Manager.RegisterPuzzleStateChange(PuzzleIndex, PuzzleState);
				if (Safe != null)
				{
					Safe.SetState(1, playSound: true, stateEvent: true);
				}
			}
		}

		private void Update()
		{
			if (LockedPieces >= NumLockedPiecesRequired && PuzzleState == 0)
			{
				PuzzleState = 1;
				Manager.RegisterPuzzleStateChange(PuzzleIndex, PuzzleState);
				for (int i = 0; i < Rends.Length; i++)
				{
					Rends[i].material.SetTexture("_DecalTex", CompletedTexture);
				}
			}
			if (PuzzleState == 2 && Safe.SafeState == 0 && Safe != null)
			{
				Safe.SetState(1, playSound: true, stateEvent: true);
			}
		}

		public void PieceLocked()
		{
			LockedPieces++;
		}

		public override void SetState(int stateIndex, int safeStateIndex)
		{
			if (stateIndex > 0)
			{
				for (int i = 0; i < Rings.Length; i++)
				{
					Rings[i].LockPiece(stateEvent: false);
				}
				LockedPieces = NumLockedPiecesRequired;
				for (int j = 0; j < Rends.Length; j++)
				{
					Rends[j].material.SetTexture("_DecalTex", CompletedTexture);
				}
			}
			base.SetState(stateIndex, safeStateIndex);
		}
	}
}
