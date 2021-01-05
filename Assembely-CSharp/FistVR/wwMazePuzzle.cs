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
			if (PuzzleState == 1 && s == CompletionCode)
			{
				PuzzleState = 2;
				Manager.RegisterPuzzleStateChange(PuzzleIndex, 2);
				if (Safe != null)
				{
					Safe.SetState(1, playSound: true, stateEvent: true);
				}
			}
		}

		private void Update()
		{
			if (PuzzleState == 0 && Marble.IsHeld)
			{
				float num = Vector3.Distance(Marble.transform.position, Goal.position);
				if (num < DistToGoalThreshold)
				{
					Marble.transform.position = Goal.position;
					Marble.EndInteractionDistance = 0f;
					Marble.IsMarbleLocked = true;
					PuzzleState = 1;
					Manager.RegisterPuzzleStateChange(PuzzleIndex, PuzzleState);
					Solution.SetActive(value: true);
				}
			}
		}

		public override void SetState(int stateIndex, int safeStateIndex)
		{
			if (stateIndex > 0)
			{
				Solution.SetActive(value: true);
				Marble.transform.position = Goal.position;
				Marble.EndInteractionDistance = 0f;
				Marble.IsMarbleLocked = true;
			}
			base.SetState(stateIndex, safeStateIndex);
		}
	}
}
