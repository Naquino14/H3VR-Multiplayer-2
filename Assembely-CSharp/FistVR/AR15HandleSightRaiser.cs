using UnityEngine;

namespace FistVR
{
	public class AR15HandleSightRaiser : FVRInteractiveObject
	{
		public enum SightHeights
		{
			Lowest,
			Low,
			Mid,
			High,
			Highest
		}

		public Transform SightPiece;

		public Transform SightBottomPoint;

		public Transform SightTopPoint;

		private float m_sightHeight;

		private Vector2 TotalAmountMoved;

		private SightHeights height;

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			switch (height)
			{
			case SightHeights.Lowest:
				height = SightHeights.Low;
				m_sightHeight = 0.25f;
				break;
			case SightHeights.Low:
				height = SightHeights.Mid;
				m_sightHeight = 0.5f;
				break;
			case SightHeights.Mid:
				height = SightHeights.High;
				m_sightHeight = 0.75f;
				break;
			case SightHeights.High:
				height = SightHeights.Highest;
				m_sightHeight = 1f;
				break;
			case SightHeights.Highest:
				height = SightHeights.Lowest;
				m_sightHeight = 0f;
				break;
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			SightPiece.transform.position = Vector3.Lerp(SightBottomPoint.position, SightTopPoint.position, m_sightHeight);
		}
	}
}
