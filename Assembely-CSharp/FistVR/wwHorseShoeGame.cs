using UnityEngine;

namespace FistVR
{
	public class wwHorseShoeGame : MonoBehaviour
	{
		public wwParkManager ParkManager;

		public wwHorseShoePlinth[] Plinths;

		public GameObject[] CompletionCheckMarks;

		private float CheckComplettionTick = 1f;

		public void RegisterSuccess(int i)
		{
			ParkManager.RegisterHorseshoeCompletion(i);
			Debug.Log("Registering Success");
			CompletionCheckMarks[i].SetActive(value: true);
		}

		public void UpdateCheckMarks(int[] states)
		{
			for (int i = 0; i < states.Length; i++)
			{
				if (states[i] == 0)
				{
					CompletionCheckMarks[i].SetActive(value: false);
				}
				else
				{
					CompletionCheckMarks[i].SetActive(value: true);
				}
			}
		}

		public void SetPlinthStates(int[] states)
		{
			for (int i = 0; i < states.Length; i++)
			{
				if (states[i] > 0)
				{
					Plinths[i].SetCompleted();
				}
			}
			UpdateCheckMarks(states);
		}

		public void Update()
		{
			if (ParkManager.RewardChests[2].GetState() >= 1)
			{
				return;
			}
			if (CheckComplettionTick > 0f)
			{
				CheckComplettionTick -= Time.deltaTime;
				return;
			}
			CheckComplettionTick = 1f;
			bool flag = true;
			for (int i = 0; i < Plinths.Length; i++)
			{
				if (!Plinths[i].IsCompleted())
				{
					flag = false;
				}
			}
			if (flag)
			{
				ParkManager.UnlockChest(2);
			}
		}
	}
}
