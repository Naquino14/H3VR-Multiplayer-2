using UnityEngine;

namespace FistVR
{
	public class wwEventPuzzle_Churner : wwEventPuzzle
	{
		public GameObject ButterEatingSoundPrefab;

		private bool[] eatenButter = new bool[7];

		public void AteButter(int index, Vector3 v)
		{
			eatenButter[index] = true;
			if (ButterEatingSoundPrefab != null)
			{
				Object.Instantiate(ButterEatingSoundPrefab, v, Quaternion.identity);
			}
		}

		public void Update()
		{
			if (PuzzleState != 0)
			{
				return;
			}
			bool flag = true;
			for (int i = 0; i < eatenButter.Length; i++)
			{
				if (!eatenButter[i])
				{
					flag = false;
				}
			}
			if (flag)
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
