using UnityEngine;

namespace FistVR
{
	public class wwEventPuzzle : MonoBehaviour
	{
		public wwParkManager ParkManager;

		public int PuzzleIndex;

		public int PuzzleState;

		public int ChestIndex;

		public virtual void SetState(int stateIndex)
		{
			PuzzleState = stateIndex;
		}
	}
}
