using UnityEngine;

namespace FistVR
{
	public class MM_Currency : FVRPhysicalObject
	{
		[Header("Currency Stuff")]
		public MMCurrency Type;

		private bool hasBeenCollected;

		public GameObject OnCollectSpawn;

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			GM.MMFlags.LearnCurrency(Type);
			GM.MMFlags.SaveToFile();
		}

		public void Update()
		{
			Vector3 b = GM.CurrentPlayerBody.Head.transform.position + Vector3.up * -0.1f;
			if (Vector3.Distance(base.transform.position, b) < 0.15f)
			{
				Collect(m_hand);
			}
		}

		private void Collect(FVRViveHand hand)
		{
			if (!hasBeenCollected)
			{
				hasBeenCollected = true;
				GM.MMFlags.CollectCurrency(Type, 1);
				if (OnCollectSpawn != null)
				{
					Object.Instantiate(OnCollectSpawn, base.transform.position, base.transform.rotation);
				}
				if (hand != null)
				{
					EndInteraction(hand);
					hand.ForceSetInteractable(null);
				}
				GM.MMFlags.SaveToFile();
				Object.Destroy(base.gameObject);
			}
		}
	}
}
