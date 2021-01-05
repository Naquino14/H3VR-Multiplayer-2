using UnityEngine;

namespace FistVR
{
	public class MR_Crank : FVRInteractiveObject
	{
		private float m_crankedAmount;

		private Vector2 m_crankLimits = new Vector2(0f, 7200f);

		private Vector3 m_lastCrankDir;

		private Vector3 m_curCrankDir;

		public Transform YAxisPoint;

		public Transform SpikeTrap;

		public bool m_isCrankHeld;

		private float m_CrankDelta;

		public AudioSource CrankAudio;

		protected override void Awake()
		{
			base.Awake();
			m_lastCrankDir = base.transform.forward;
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
			Vector3 vector = hand.transform.position - base.transform.position;
			m_curCrankDir = Vector3.ProjectOnPlane(vector, base.transform.up).normalized;
			Vector3 lastCrankDir = m_lastCrankDir;
			float value = Mathf.Atan2(Vector3.Dot(base.transform.up, Vector3.Cross(lastCrankDir, m_curCrankDir)), Vector3.Dot(lastCrankDir, m_curCrankDir)) * 57.29578f;
			value = 0f - Mathf.Clamp(value, -1.2f, 1.2f);
			if (Crank(value))
			{
				base.transform.localEulerAngles = new Vector3(0f, 0f - m_crankedAmount, 0f);
				m_lastCrankDir = base.transform.forward;
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			m_isCrankHeld = false;
			base.EndInteraction(hand);
		}

		private bool Crank(float angle)
		{
			if (m_crankedAmount + angle > m_crankLimits.x && m_crankedAmount + angle < m_crankLimits.y)
			{
				m_crankedAmount += angle;
				m_crankedAmount = Mathf.Clamp(m_crankedAmount, 0f, 3600f);
				m_CrankDelta = Mathf.Abs(angle);
				SpikeTrap.transform.localPosition = new Vector3(0f, m_crankedAmount * 0.001f, 0f);
				return true;
			}
			return false;
		}

		public void Update()
		{
			if (!m_isCrankHeld && m_crankedAmount > 0f && m_crankedAmount < 7000f)
			{
				m_crankedAmount -= Time.deltaTime * 360f;
				m_crankedAmount = Mathf.Clamp(m_crankedAmount, 0f, 3600f);
				base.transform.localEulerAngles = new Vector3(0f, 0f - m_crankedAmount, 0f);
				m_lastCrankDir = base.transform.forward;
				SpikeTrap.transform.localPosition = new Vector3(0f, m_crankedAmount * 0.001f, 0f);
				m_CrankDelta = 1f;
				CrankAudio.pitch = 0.8f;
			}
			else
			{
				CrankAudio.pitch = 0.5f;
			}
			if (m_CrankDelta > 0f)
			{
				m_CrankDelta -= Time.deltaTime * 6f;
			}
			CrankAudio.volume = m_CrankDelta;
			if (m_CrankDelta <= 0f && CrankAudio.isPlaying)
			{
				CrankAudio.Stop();
			}
			if (m_CrankDelta > 0f && !CrankAudio.isPlaying)
			{
				CrankAudio.Play();
			}
		}
	}
}
