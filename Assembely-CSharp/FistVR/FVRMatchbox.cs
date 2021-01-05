using UnityEngine;

namespace FistVR
{
	public class FVRMatchbox : FVRPhysicalObject
	{
		[Header("Matchbox Config")]
		public Transform InnerBox;

		public GameObject NewMatchTrigger;

		private float m_openZ = 0.04f;

		private bool m_isOpen;

		protected override void FVRUpdate()
		{
			if (m_isOpen)
			{
				NewMatchTrigger.SetActive(value: true);
				InnerBox.transform.localPosition = Vector3.Lerp(InnerBox.transform.localPosition, new Vector3(0f, 0f, m_openZ), Time.deltaTime * 4f);
			}
			else
			{
				NewMatchTrigger.SetActive(value: false);
				InnerBox.transform.localPosition = Vector3.Lerp(InnerBox.transform.localPosition, new Vector3(0f, 0f, 0f), Time.deltaTime * 4f);
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			bool flag = false;
			if (hand.Input.TriggerDown)
			{
				flag = true;
			}
			if (hand.IsInStreamlinedMode && hand.Input.BYButtonDown)
			{
				flag = true;
			}
			else if (!hand.IsInStreamlinedMode && hand.Input.TouchpadDown)
			{
				flag = true;
			}
			if (flag)
			{
				m_isOpen = !m_isOpen;
			}
		}
	}
}
