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
			if (PuzzleState == 1 && s == CompletionCode)
			{
				PuzzleState = 2;
				Manager.RegisterPuzzleStateChange(PuzzleIndex, 2);
				Safe.SetState(1, playSound: true, stateEvent: true);
			}
		}

		private void Update()
		{
			for (int i = 0; i < Pieces.Length; i++)
			{
				if (!Pieces[i].IsLockedIntoPlace && Pieces[i].IsHeld)
				{
					float num = Vector3.Distance(base.transform.position, Pieces[i].transform.position);
					float num2 = Vector3.Angle(base.transform.up, Pieces[i].transform.up);
					if (num < 0.05f && num2 < 5f)
					{
						LockPiece(Pieces[i], StateEvent: true);
					}
				}
			}
		}

		private void LockPiece(wwPuzzlePiece p, bool StateEvent)
		{
			if (p.IsHeld)
			{
				FVRViveHand hand = p.m_hand;
				hand.ForceSetInteractable(null);
				p.EndInteraction(hand);
				p.IsLockedIntoPlace = true;
				p.RootRigidbody.isKinematic = true;
				p.transform.position = base.transform.position;
				p.transform.rotation = base.transform.rotation;
				LockedPieces++;
				if (LockedPieces >= Pieces.Length)
				{
					PuzzleState = 1;
					Manager.RegisterPuzzleStateChange(PuzzleIndex, 1);
				}
				if (StateEvent)
				{
					Aud.clip = Clip_LockedInPiece;
					Aud.Play();
				}
			}
		}

		public override void SetState(int stateIndex, int safeStateIndex)
		{
			if (stateIndex > 0)
			{
				for (int i = 0; i < Pieces.Length; i++)
				{
					Pieces[i].IsLockedIntoPlace = true;
					Pieces[i].RootRigidbody.isKinematic = true;
					Pieces[i].transform.position = base.transform.position;
					Pieces[i].transform.rotation = base.transform.rotation;
					LockedPieces = Pieces.Length;
				}
			}
			base.SetState(stateIndex, safeStateIndex);
		}
	}
}
