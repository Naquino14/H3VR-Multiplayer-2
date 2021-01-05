using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FVRPointable : MonoBehaviour
	{
		public float MaxPointingRange = 1f;

		protected List<FVRViveHand> PointingHands = new List<FVRViveHand>();

		protected bool m_isBeingPointedAt;

		public virtual void OnPoint(FVRViveHand hand)
		{
			if (!PointingHands.Contains(hand))
			{
				PointingHands.Add(hand);
				if (!m_isBeingPointedAt)
				{
					m_isBeingPointedAt = true;
					BeginHoverDisplay();
				}
			}
		}

		public virtual void EndPoint(FVRViveHand hand)
		{
			if (PointingHands.Contains(hand))
			{
				PointingHands.Remove(hand);
			}
			if (PointingHands.Count <= 0)
			{
				m_isBeingPointedAt = false;
				EndHoverDisplay();
			}
		}

		public virtual void Update()
		{
			if (m_isBeingPointedAt)
			{
				OnHoverDisplay();
			}
		}

		public virtual void BeginHoverDisplay()
		{
		}

		public virtual void EndHoverDisplay()
		{
		}

		public virtual void OnHoverDisplay()
		{
		}
	}
}
