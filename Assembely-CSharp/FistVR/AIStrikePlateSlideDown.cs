using UnityEngine;

namespace FistVR
{
	public class AIStrikePlateSlideDown : AIStrikePlate
	{
		public Vector3 UpperPosition;

		public Vector3 LowerPosition;

		public GameObject Target;

		public string Message;

		public override void Damage(Damage dam)
		{
			base.Damage(dam);
			base.transform.localPosition = Vector3.Lerp(LowerPosition, UpperPosition, (float)NumStrikesLeft / (float)m_originalNumStrikesLeft);
		}

		public override void Reset()
		{
			base.transform.localPosition = UpperPosition;
			base.Reset();
		}

		public override void PlateFelled()
		{
			Target.SendMessage(Message);
		}
	}
}
