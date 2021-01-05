using UnityEngine;

namespace FistVR
{
	public class OpenBoltDustCover : FVRInteractiveObject
	{
		public OpenBoltReceiverBolt Bolt;

		public Transform DustCoverGeo;

		private bool m_isOpen;

		public float OpenRot;

		public float ClosedRot;

		public float RotSpeed = 360f;

		private float m_curRot;

		private float m_tarRot;

		protected override void Awake()
		{
			base.Awake();
			m_curRot = ClosedRot;
			m_tarRot = ClosedRot;
		}

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			ToggleDustCoverState();
		}

		private void ToggleDustCoverState()
		{
			if (m_isOpen)
			{
				Close();
			}
			else
			{
				Open();
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (!m_isOpen && Bolt.CurPos != 0)
			{
				Open();
			}
			if (Mathf.Abs(m_tarRot - m_curRot) > 0.01f)
			{
				m_curRot = Mathf.MoveTowards(m_curRot, m_tarRot, Time.deltaTime * RotSpeed);
				DustCoverGeo.localEulerAngles = new Vector3(0f, 0f, m_curRot);
			}
		}

		private void Open()
		{
			m_isOpen = true;
			m_tarRot = OpenRot;
			RotSpeed = 1900f;
		}

		private void Close()
		{
			m_isOpen = false;
			m_tarRot = ClosedRot;
			RotSpeed = 500f;
		}
	}
}
