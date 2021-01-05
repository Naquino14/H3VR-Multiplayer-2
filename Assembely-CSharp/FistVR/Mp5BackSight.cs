using UnityEngine;

namespace FistVR
{
	public class Mp5BackSight : FVRInteractiveObject
	{
		public Transform BackSight;

		public Vector3[] SightPositions;

		private int m_sightPosition;

		public override void SimpleInteraction(FVRViveHand hand)
		{
			m_sightPosition++;
			if (m_sightPosition >= SightPositions.Length)
			{
				m_sightPosition = 0;
			}
			BackSight.localEulerAngles = SightPositions[m_sightPosition];
			base.SimpleInteraction(hand);
		}
	}
}
