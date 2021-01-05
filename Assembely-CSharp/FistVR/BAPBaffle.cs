using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class BAPBaffle : FVRPhysicalObject
	{
		[Header("Baffle")]
		public int BaffleState;

		public List<GameObject> BaffleStates;

		public void SetState(int s)
		{
			BaffleState = s;
			for (int i = 0; i < BaffleStates.Count; i++)
			{
				if (i == BaffleState)
				{
					BaffleStates[i].SetActive(value: true);
				}
				else
				{
					BaffleStates[i].SetActive(value: false);
				}
			}
		}

		public override GameObject DuplicateFromSpawnLock(FVRViveHand hand)
		{
			GameObject gameObject = base.DuplicateFromSpawnLock(hand);
			BAPBaffle component = gameObject.GetComponent<BAPBaffle>();
			component.SetState(BaffleState);
			return gameObject;
		}
	}
}
