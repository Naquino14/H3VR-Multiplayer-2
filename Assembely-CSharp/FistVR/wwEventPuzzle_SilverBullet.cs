using UnityEngine;

namespace FistVR
{
	public class wwEventPuzzle_SilverBullet : wwEventPuzzle
	{
		public GameObject[] StaticBullet;

		public GameObject[] TurnOnBullet;

		public GameObject[] ExplosionPrefabs;

		public void Explode()
		{
			if (PuzzleState == 0)
			{
				GameObject[] staticBullet = StaticBullet;
				foreach (GameObject gameObject in staticBullet)
				{
					gameObject.SetActive(value: false);
				}
				GameObject[] turnOnBullet = TurnOnBullet;
				foreach (GameObject gameObject2 in turnOnBullet)
				{
					gameObject2.SetActive(value: true);
				}
				GameObject[] explosionPrefabs = ExplosionPrefabs;
				foreach (GameObject original in explosionPrefabs)
				{
					Object.Instantiate(original, base.transform.position, base.transform.rotation);
				}
				PuzzleState = 1;
				ParkManager.RegisterEventPuzzleChange(PuzzleIndex, 1);
				if (ParkManager.RewardChests[ChestIndex].GetState() < 1)
				{
					ParkManager.UnlockChest(ChestIndex);
				}
			}
		}

		public override void SetState(int stateIndex)
		{
			base.SetState(stateIndex);
			if (PuzzleState == 0)
			{
				GameObject[] staticBullet = StaticBullet;
				foreach (GameObject gameObject in staticBullet)
				{
					gameObject.SetActive(value: true);
				}
				GameObject[] turnOnBullet = TurnOnBullet;
				foreach (GameObject gameObject2 in turnOnBullet)
				{
					gameObject2.SetActive(value: false);
				}
			}
			else
			{
				GameObject[] staticBullet2 = StaticBullet;
				foreach (GameObject gameObject3 in staticBullet2)
				{
					gameObject3.SetActive(value: false);
				}
				GameObject[] turnOnBullet2 = TurnOnBullet;
				foreach (GameObject gameObject4 in turnOnBullet2)
				{
					gameObject4.SetActive(value: true);
				}
			}
		}
	}
}
