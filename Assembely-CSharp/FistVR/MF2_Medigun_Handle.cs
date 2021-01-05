using UnityEngine;

namespace FistVR
{
	public class MF2_Medigun_Handle : FVRInteractiveObject
	{
		public enum HandleState
		{
			Back,
			Forward
		}

		public MF2_Medigun Gun;

		public HandleState State;

		private Quaternion m_curLocalRot;

		private Quaternion m_tarLocalRot;

		protected override void Awake()
		{
			base.Awake();
			m_curLocalRot = Gun.HandleRot_Back.localRotation;
			base.transform.localRotation = m_curLocalRot;
			m_tarLocalRot = m_curLocalRot;
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 vector = hand.transform.position - base.transform.position;
			Vector3 from = Vector3.ProjectOnPlane(vector, Gun.HandleRot_Up.forward);
			float num = Vector3.Angle(from, Gun.HandleRot_Front.forward);
			float num2 = Vector3.Angle(from, Gun.HandleRot_Back.forward);
			if (num < num2)
			{
				m_tarLocalRot = Gun.HandleRot_Front.localRotation;
				if (State == HandleState.Back)
				{
					Gun.HandleEngage();
				}
				State = HandleState.Forward;
			}
			else
			{
				m_tarLocalRot = Gun.HandleRot_Back.localRotation;
				if (State == HandleState.Forward)
				{
					Gun.HandleDisEngage();
				}
				State = HandleState.Back;
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			m_curLocalRot = Quaternion.Slerp(m_curLocalRot, m_tarLocalRot, Time.deltaTime * 10f);
			base.transform.localRotation = m_curLocalRot;
		}
	}
}
