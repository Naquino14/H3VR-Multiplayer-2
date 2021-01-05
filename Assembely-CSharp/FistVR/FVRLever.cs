using UnityEngine;

namespace FistVR
{
	public class FVRLever : FVRInteractiveObject
	{
		public enum LeverState
		{
			Off,
			Mid,
			On
		}

		public Transform LeverTransform;

		public Transform Base;

		private bool m_isForward;

		private float m_curRot = -22.5f;

		public float minValue = -22.5f;

		public float maxValue = -22.5f;

		private LeverState curState;

		private LeverState lastState;

		private AudioSource aud;

		protected override void Awake()
		{
			base.Awake();
			aud = GetComponent<AudioSource>();
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 vector = hand.transform.position - LeverTransform.position;
			Vector3 to = Vector3.ProjectOnPlane(vector, LeverTransform.right);
			if (Vector3.Dot(to.normalized, Base.up) > 0f)
			{
				m_curRot = 0f - Vector3.Angle(Base.forward, to);
			}
			else
			{
				m_curRot = Vector3.Angle(Base.forward, to);
			}
		}

		public float GetLeverValue()
		{
			return Mathf.InverseLerp(minValue, maxValue, m_curRot);
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			m_curRot = Mathf.Clamp(m_curRot, -22.5f, 22.5f);
			LeverTransform.localEulerAngles = new Vector3(m_curRot, 0f, 0f);
			if (m_curRot > 22f)
			{
				curState = LeverState.On;
			}
			else if (m_curRot < -22f)
			{
				curState = LeverState.Off;
			}
			else
			{
				curState = LeverState.Mid;
			}
			if (curState == LeverState.On && lastState != LeverState.On)
			{
				aud.PlayOneShot(aud.clip);
			}
			if (curState == LeverState.Off && lastState != 0)
			{
				aud.PlayOneShot(aud.clip);
			}
			lastState = curState;
		}
	}
}
