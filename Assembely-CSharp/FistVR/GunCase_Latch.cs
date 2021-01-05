using UnityEngine;

namespace FistVR
{
	public class GunCase_Latch : FVRInteractiveObject, IFVRDamageable
	{
		private bool m_isOpen;

		public AudioEvent LatchOpen;

		public bool IsOpen()
		{
			return m_isOpen;
		}

		protected override void Awake()
		{
			base.Awake();
		}

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			if (!m_isOpen)
			{
				Open();
			}
		}

		public void Damage(Damage d)
		{
			if (!m_isOpen)
			{
				Open();
			}
		}

		private void Open()
		{
			m_isOpen = true;
			SM.PlayGenericSound(LatchOpen, base.transform.position);
			base.transform.localEulerAngles = new Vector3(-45f, 0f, 0f);
		}

		public void Reset()
		{
			base.transform.localEulerAngles = Vector3.zero;
			m_isOpen = false;
		}
	}
}
