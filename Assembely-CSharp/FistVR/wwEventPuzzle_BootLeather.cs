using UnityEngine;

namespace FistVR
{
	public class wwEventPuzzle_BootLeather : wwEventPuzzle
	{
		public Transform ArrowPiece1;

		public Transform ArrowPiece2;

		public Rigidbody RB;

		public override void SetState(int stateIndex)
		{
			base.SetState(stateIndex);
		}

		public void Update()
		{
			if (RB.angularVelocity.magnitude < 0.25f && Vector3.Angle(ArrowPiece1.forward, ArrowPiece2.forward) < 3.5f)
			{
				SolvePuzzle();
			}
		}

		private void SolvePuzzle()
		{
			if (PuzzleState == 0)
			{
				PuzzleState = 1;
				ParkManager.RegisterEventPuzzleChange(PuzzleIndex, 1);
				if (ParkManager.RewardChests[ChestIndex].GetState() < 1)
				{
					ParkManager.UnlockChest(ChestIndex);
				}
			}
		}
	}
}
