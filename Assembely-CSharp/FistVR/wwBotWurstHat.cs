using UnityEngine;

namespace FistVR
{
	public class wwBotWurstHat : FVRPhysicalObject
	{
		[Header("Hat Stuff")]
		public wwBotManager Manager;

		public wwBotWurst Wurst;

		public Collider[] Cols;

		private bool m_isRemovedYet;

		public int HatBanditIndex;

		public bool IsPosse;

		public override bool IsDistantGrabbable()
		{
			return m_isRemovedYet;
		}

		public override bool IsInteractable()
		{
			if (IsPosse)
			{
				return false;
			}
			return base.IsInteractable();
		}

		public void Remove()
		{
			m_isRemovedYet = true;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			if (!m_isRemovedYet && Wurst != null)
			{
				m_isRemovedYet = true;
				DistantGrabbable = true;
				Collider[] cols = Cols;
				foreach (Collider collider in cols)
				{
					collider.enabled = true;
				}
				Wurst.HatRemoved();
			}
		}

		public void OnTriggerEnter(Collider col)
		{
			if (col.gameObject.CompareTag("wwHatReturn"))
			{
				Manager.BotHatRetrieved(HatBanditIndex);
				if (base.IsHeld)
				{
					FVRViveHand hand = m_hand;
					EndInteraction(m_hand);
					hand.ForceSetInteractable(null);
				}
				Object.Destroy(base.gameObject);
			}
		}
	}
}
