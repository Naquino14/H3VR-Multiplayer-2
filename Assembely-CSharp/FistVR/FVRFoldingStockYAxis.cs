using UnityEngine;

namespace FistVR
{
	public class FVRFoldingStockYAxis : FVRInteractiveObject
	{
		public enum StockPos
		{
			Closed,
			Mid,
			Open
		}

		public Transform Root;

		public Transform Stock;

		private float rotAngle;

		public float MinRot;

		public float MaxRot;

		public StockPos m_curPos;

		public StockPos m_lastPos;

		public bool isMinClosed = true;

		public FVRFireArm FireArm;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 vector = hand.transform.position - Root.position;
			vector = Vector3.ProjectOnPlane(vector, Root.up).normalized;
			Vector3 lhs = -Root.transform.forward;
			rotAngle = Mathf.Atan2(Vector3.Dot(Root.up, Vector3.Cross(lhs, vector)), Vector3.Dot(lhs, vector)) * 57.29578f;
			if (Mathf.Abs(rotAngle - MinRot) < 5f)
			{
				rotAngle = MinRot;
			}
			if (Mathf.Abs(rotAngle - MaxRot) < 5f)
			{
				rotAngle = MaxRot;
			}
			if (!(rotAngle >= MinRot) || !(rotAngle <= MaxRot))
			{
				return;
			}
			Stock.localEulerAngles = new Vector3(0f, rotAngle, 0f);
			float num = Mathf.InverseLerp(MinRot, MaxRot, rotAngle);
			if (isMinClosed)
			{
				if (num < 0.02f)
				{
					m_curPos = StockPos.Closed;
					FireArm.HasActiveShoulderStock = false;
				}
				else if (num > 0.9f)
				{
					m_curPos = StockPos.Open;
					FireArm.HasActiveShoulderStock = true;
				}
				else
				{
					m_curPos = StockPos.Mid;
					FireArm.HasActiveShoulderStock = false;
				}
			}
			else if (num < 0.1f)
			{
				m_curPos = StockPos.Open;
				FireArm.HasActiveShoulderStock = true;
			}
			else if (num > 0.98f)
			{
				m_curPos = StockPos.Closed;
				FireArm.HasActiveShoulderStock = false;
			}
			else
			{
				m_curPos = StockPos.Mid;
				FireArm.HasActiveShoulderStock = false;
			}
			if (m_curPos == StockPos.Open && m_lastPos != StockPos.Open)
			{
				FireArm.PlayAudioEvent(FirearmAudioEventType.StockOpen);
			}
			if (m_curPos == StockPos.Closed && m_lastPos != 0)
			{
				FireArm.PlayAudioEvent(FirearmAudioEventType.StockClosed);
			}
			m_lastPos = m_curPos;
		}
	}
}
