using UnityEngine;

namespace FistVR
{
	public class wwSpinnyRing : FVRInteractiveObject
	{
		public wwSpinnyPuzzle Puzzle;

		private bool m_isLocked;

		public Transform LockPos;

		private float m_speed;

		private Vector3 m_lastForward = Vector3.zero;

		public AudioSource Aud;

		public override bool IsInteractable()
		{
			if (m_isLocked)
			{
				return false;
			}
			return base.IsInteractable();
		}

		protected override void Awake()
		{
			base.Awake();
			m_lastForward = base.transform.forward;
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 vector = hand.transform.position - base.transform.position;
			vector = Vector3.ProjectOnPlane(vector, base.transform.up);
			vector.Normalize();
			Quaternion rotation = Quaternion.LookRotation(vector, base.transform.up);
			base.transform.rotation = rotation;
			m_speed = Vector3.Angle(m_lastForward, base.transform.forward) * Time.deltaTime;
			m_lastForward = base.transform.forward;
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			if (!m_isLocked && Vector3.Angle(base.transform.forward, LockPos.forward) < 5f)
			{
				LockPiece(stateEvent: true);
			}
		}

		public void LockPiece(bool stateEvent)
		{
			m_isLocked = true;
			base.transform.localEulerAngles = LockPos.localEulerAngles;
			base.transform.localPosition = new Vector3(0f, -0.03f, 0f);
			if (stateEvent)
			{
				Aud.Play();
			}
			Puzzle.PieceLocked();
		}
	}
}
