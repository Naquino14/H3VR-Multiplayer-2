using UnityEngine;

namespace FistVR
{
	public class HandCrankFrankCrank : FVRInteractiveObject
	{
		public OpenBoltReceiver Gun;

		private Vector3 m_curCrankDir;

		private float m_CrankDelta;

		private float m_crankedAmount;

		public bool m_isCrankHeld;

		public Transform YUpTarget;

		[Header("GunStuff")]
		public Transform GunBarrels;

		private float m_curRot;

		private float m_rotTilShot = 36f;

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
				CrankGun(value);
				m_curCrankDir = Vector3.RotateTowards(m_curCrankDir, normalized2, value * 0.0174533f, 0f);
				m_curCrankDir = Vector3.ProjectOnPlane(m_curCrankDir, YUpTarget.up);
				base.transform.rotation = Quaternion.LookRotation(m_curCrankDir, YUpTarget.up);
			}
		}

		public void CrankGun(float crank)
		{
			float value = crank * 0.4f;
			value = Mathf.Clamp(value, 0f, 4f);
			m_curRot += value;
			if (m_curRot > 180f)
			{
				m_curRot -= 360f;
			}
			GunBarrels.transform.localEulerAngles = new Vector3(0f, 0f, m_curRot);
			m_rotTilShot -= value;
			if (m_rotTilShot <= 0f)
			{
				FireShot();
				m_rotTilShot = 36f;
			}
			else if (m_rotTilShot <= 18f && Gun.Bolt.GetBoltLerpBetweenLockAndFore() > 0.9f)
			{
				PrimeShot();
			}
		}

		private void PrimeShot()
		{
			Gun.Bolt.SetBoltToRear();
			Gun.EngageSeer();
		}

		private void FireShot()
		{
			Gun.ReleaseSeer();
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
		}
	}
}
