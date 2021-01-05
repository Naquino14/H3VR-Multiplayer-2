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
			PuzzleState = stateIndex;
			Safe.SetState(safeStateIndex, playSound: false, stateEvent: false);
		}
	}
}
