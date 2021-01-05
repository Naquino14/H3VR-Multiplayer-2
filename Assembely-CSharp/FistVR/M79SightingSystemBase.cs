using UnityEngine;

namespace FistVR
{
	public class M79SightingSystemBase : FVRInteractiveObject
	{
		public float m_xRotMin;

		public float m_xRotMax = 90f;

		public float m_xRotCur;

		public bool IsFlippedUp;

		public override void SimpleInteraction(FVRViveHand hand)
		{
			IsFlippedUp = !IsFlippedUp;
			if (IsFlippedUp)
			{
				m_xRotCur = m_xRotMax;
				base.transform.localEulerAngles = new Vector3(m_xRotCur, 0f, 0f);
			}
			else
			{
				m_xRotCur = m_xRotMin;
				base.transform.localEulerAngles = new Vector3(m_xRotCur, 0f, 0f);
			}
		}
	}
}
