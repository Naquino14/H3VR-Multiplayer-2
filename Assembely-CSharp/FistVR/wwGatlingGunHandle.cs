using UnityEngine;

namespace FistVR
{
	public class wwGatlingGunHandle : FVRInteractiveObject
	{
		public wwGatlingGun Gun;

		private Vector3 m_curCrankDir;

		private float m_CrankDelta;

		private float m_crankedAmount;

		public bool m_isCrankHeld;

		public Transform YUpTarget;

		protected override void Awake()
		{
			base.Awake();
			m_curCrankDir = base.transform.forward;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			m_isCrankHeld = true;
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 normalized = (hand.transform.position - base.transform.position).normalized;
			Vector3 normalized2 = Vector3.ProjectOnPlane(normalized, YUpTarget.up).normalized;
			Vector3 forward = base.transform.forward;
			float value = Mathf.Atan2(Vector3.Dot(YUpTarget.up, Vector3.Cross(forward, normalized2)), Vector3.Dot(forward, normalized2)) * 57.29578f;
			value = Mathf.Clamp(value, 0f, 10f);
			if (value > 0f)
			{
				Gun.CrankGun(value);
				m_curCrankDir = Vector3.RotateTowards(m_curCrankDir, normalized2, value * 0.0174533f, 0f);
				m_curCrankDir = Vector3.ProjectOnPlane(m_curCrankDir, YUpTarget.up);
				base.transform.rotation = Quaternion.LookRotation(m_curCrankDir, YUpTarget.up);
			}
		}
	}
}
