using UnityEngine;

namespace FistVR
{
	public class GP25_RangingSight : FVRInteractiveObject
	{
		public GP25 GP25;

		public Transform RangingPiece;

		private int m_rangingIndex;

		private float[] m_rangingAmounts = new float[7]
		{
			3.5f,
			6f,
			13f,
			21f,
			43f,
			66f,
			73f
		};

		public override void UpdateInteraction(FVRViveHand hand)
		{
			bool flag = false;
			bool flag2 = false;
			if (hand.IsInStreamlinedMode)
			{
				if (hand.Input.BYButtonDown)
				{
					flag = true;
				}
				if (hand.Input.AXButtonDown)
				{
					flag2 = true;
				}
			}
			else if (hand.Input.TouchpadDown)
			{
				if (hand.Input.TouchpadWestPressed)
				{
					flag = true;
				}
				if (hand.Input.TouchpadEastPressed)
				{
					flag2 = true;
				}
			}
			if (flag && m_rangingIndex > 0)
			{
				m_rangingIndex--;
				UpdateRangingPiece();
			}
			if (flag2 && m_rangingIndex < m_rangingAmounts.Length - 1)
			{
				m_rangingIndex++;
				UpdateRangingPiece();
			}
			base.UpdateInteraction(hand);
		}

		private void UpdateRangingPiece()
		{
			GP25.PlayAudioEvent(FirearmAudioEventType.Safety);
			RangingPiece.localEulerAngles = new Vector3(m_rangingAmounts[m_rangingIndex], 0f, 0f);
		}
	}
}
