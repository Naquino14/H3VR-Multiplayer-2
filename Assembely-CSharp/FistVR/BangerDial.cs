using UnityEngine;

namespace FistVR
{
	public class BangerDial : FVRInteractiveObject
	{
		public Banger Banger;

		public Transform Root;

		public float DialTick;

		private bool m_isPrimed;

		private bool m_hasDinged;

		public AudioEvent AudEvent_Ding;

		public AudioEvent AudEvent_WindTick;

		public AudioEvent AudEvent_TickDown;

		private int lastTickDown;

		private Vector3 lastHandForward = Vector3.zero;

		private Vector3 lastMountForward = Vector3.zero;

		private int lastWindTickRound;

		public void TickDown()
		{
			if (m_isPrimed && !base.IsHeld)
			{
				DialTick -= Time.deltaTime;
				int roundedToFactor = GetRoundedToFactor(DialTick, 1f);
				if (roundedToFactor != lastTickDown)
				{
					SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_TickDown, base.transform.position);
				}
				lastTickDown = roundedToFactor;
				if (DialTick <= 0f && !m_hasDinged)
				{
					m_hasDinged = true;
					Banger.StartExploding();
					float delay = Vector3.Distance(GM.CurrentPlayerBody.transform.position, base.transform.position) / 343f;
					SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, AudEvent_Ding, base.transform.position, delay);
				}
			}
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			lastHandForward = hand.transform.up;
			lastMountForward = Root.transform.up;
		}

		protected override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			if (base.IsHeld)
			{
				m_isPrimed = true;
				m_hasDinged = false;
				float dialTick = DialTick;
				Vector3 lhs = Vector3.ProjectOnPlane(m_hand.transform.up, base.transform.forward);
				Vector3 rhs = Vector3.ProjectOnPlane(lastHandForward, base.transform.forward);
				float num = Mathf.Atan2(Vector3.Dot(base.transform.forward, Vector3.Cross(lhs, rhs)), Vector3.Dot(lhs, rhs)) * 57.29578f;
				DialTick += num * 0.2f;
				Vector3 lhs2 = Vector3.ProjectOnPlane(Root.transform.up, base.transform.forward);
				Vector3 rhs2 = Vector3.ProjectOnPlane(lastMountForward, base.transform.forward);
				num = Mathf.Atan2(Vector3.Dot(base.transform.forward, Vector3.Cross(lhs2, rhs2)), Vector3.Dot(lhs2, rhs2)) * 57.29578f;
				DialTick += num * 0.2f;
				DialTick = Mathf.Clamp(DialTick, 0f, 60f);
				int roundedToFactor = GetRoundedToFactor(DialTick, 5f);
				if (roundedToFactor != lastWindTickRound)
				{
					SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_WindTick, base.transform.position);
				}
				lastHandForward = m_hand.transform.up;
				lastMountForward = Root.transform.up;
				lastTickDown = GetRoundedToFactor(DialTick, 1f);
				lastWindTickRound = roundedToFactor;
			}
			base.transform.localEulerAngles = new Vector3(0f, 0f, (0f - DialTick) * 6f);
		}

		private int GetRoundedToFactor(float input, float factor)
		{
			return Mathf.RoundToInt(input / factor) * (int)factor;
		}
	}
}
