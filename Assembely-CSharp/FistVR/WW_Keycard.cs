using UnityEngine;

namespace FistVR
{
	public class WW_Keycard : FVRPhysicalObject
	{
		public int TierType;

		private float m_TimeToExpire = 300f;

		public Transform TickDownBar;

		public Vector3 BarFull;

		public Vector3 BarEmpty;

		public float HealPercent = 0.2f;

		public AudioEvent AudEvent_Eat;

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (base.transform.position.y < -50f)
			{
				Object.Destroy(base.gameObject);
			}
			m_TimeToExpire -= Time.deltaTime;
			TickDownBar.transform.localScale = Vector3.Lerp(BarEmpty, BarFull, m_TimeToExpire / 300f);
			if (m_TimeToExpire <= 0f)
			{
				Object.Destroy(base.gameObject);
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 b = GM.CurrentPlayerBody.Head.transform.position + GM.CurrentPlayerBody.Head.transform.up * -0.2f;
			if (Vector3.Distance(base.transform.position, b) < 0.15f)
			{
				EndInteraction(hand);
				hand.ForceSetInteractable(null);
				SM.PlayGenericSound(AudEvent_Eat, base.transform.position);
				GM.CurrentPlayerBody.HealPercent(HealPercent);
				Object.Destroy(base.gameObject);
			}
		}
	}
}
