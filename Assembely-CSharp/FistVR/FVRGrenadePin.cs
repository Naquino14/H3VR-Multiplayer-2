using UnityEngine;

namespace FistVR
{
	public class FVRGrenadePin : FVRPhysicalObject
	{
		public FVRGrenade Grenade;

		public GameObject PinPiece;

		public bool IsSecondaryPin;

		private bool m_hasBeenPulled;

		public bool DieAfterTick = true;

		private bool m_isDying;

		private float m_dieTick = 10f;

		public override bool IsInteractable()
		{
			if (!Grenade.IsHeld)
			{
				return false;
			}
			if (!m_hasBeenPulled)
			{
				if (Grenade.QuickbeltSlot != null)
				{
					return false;
				}
				return true;
			}
			return base.IsInteractable();
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			if (!m_hasBeenPulled)
			{
				m_hasBeenPulled = true;
				base.transform.SetParent(null);
				PinPiece.transform.SetParent(base.transform);
				Rigidbody rigidbody = PinPiece.AddComponent<Rigidbody>();
				rigidbody.mass = 0.01f;
				HingeJoint component = GetComponent<HingeJoint>();
				component.connectedBody = rigidbody;
				if (IsSecondaryPin)
				{
					Grenade.PullPin2();
				}
				else
				{
					Grenade.PullPin();
				}
				m_isDying = true;
				if (UXGeo_Held != null)
				{
					Object.Destroy(UXGeo_Held);
				}
			}
			base.BeginInteraction(hand);
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (DieAfterTick && m_isDying)
			{
				m_dieTick -= Time.deltaTime;
				if (m_dieTick <= 0f)
				{
					Object.Destroy(base.gameObject);
				}
			}
		}
	}
}
