using System.Collections.Generic;

namespace FistVR
{
	public class wwEventPuzzle_BBQGarden : wwEventPuzzle
	{
		private List<int> m_lastStruckForks = new List<int>();

		public override void SetState(int stateIndex)
		{
			base.SetState(stateIndex);
		}

		public void ForkHit(int index)
		{
			if (m_lastStruckForks.Count >= 3)
			{
				m_lastStruckForks.RemoveAt(0);
			}
			m_lastStruckForks.Add(index);
		}

		private void Update()
		{
			if (m_lastStruckForks.Count >= 3 && m_lastStruckForks[0] == 0 && m_lastStruckForks[1] == 1 && m_lastStruckForks[2] == 2)
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
