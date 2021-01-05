using UnityEngine;

namespace FistVR
{
	public class FVRFoldingStockXAxis : FVRInteractiveObject
	{
		public enum StockPos
		{
			Closed,
			Mid,
			Open
		}

		public Transform Root;

		public Transform Stock;

		public Transform EndPiece;

		private float rotAngle;

		public float MinRot;

		public float MaxRot;

		public float EndPieceMinRot;

		public float EndPieceMaxRot;

		public StockPos m_curPos;

		public StockPos m_lastPos;

		public bool isMinClosed = true;

		public FVRFireArm FireArm;

		public bool DoesToggleStockPoint = true;

		public bool InvertZRoot;

		protected override void Start()
		{
			base.Start();
			if (isMinClosed)
			{
				rotAngle = MinRot;
			}
			else
			{
				rotAngle = MaxRot;
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 vector = hand.transform.position - Root.position;
			vector = Vector3.ProjectOnPlane(vector, Root.right).normalized;
			Vector3 lhs = Stock.forward;
			if (InvertZRoot)
			{
				lhs = -Stock.forward;
			}
			float value = Mathf.Atan2(Vector3.Dot(Root.right, Vector3.Cross(lhs, vector)), Vector3.Dot(lhs, vector)) * 57.29578f;
			value = Mathf.Clamp(value, -10f, 10f);
			rotAngle += value;
			rotAngle = Mathf.Clamp(rotAngle, MinRot, MaxRot);
			if (Mathf.Abs(rotAngle - MinRot) < 5f)
			{
				rotAngle = MinRot;
			}
			if (Mathf.Abs(rotAngle - MaxRot) < 5f)
			{
				rotAngle = MaxRot;
			}
			if (rotAngle >= MinRot && rotAngle <= MaxRot)
			{
				Stock.localEulerAngles = new Vector3(rotAngle, 0f, 0f);
				float num = Mathf.InverseLerp(MinRot, MaxRot, rotAngle);
				if (EndPiece != null)
				{
					EndPiece.localEulerAngles = new Vector3(Mathf.Lerp(EndPieceMinRot, EndPieceMaxRot, num), 0f, 0f);
				}
				if (isMinClosed)
				{
					if (num < 0.02f)
					{
						m_curPos = StockPos.Closed;
						if (DoesToggleStockPoint)
						{
							FireArm.HasActiveShoulderStock = false;
						}
					}
					else if (num > 0.9f)
					{
						m_curPos = StockPos.Open;
						if (DoesToggleStockPoint)
						{
							FireArm.HasActiveShoulderStock = true;
						}
					}
					else
					{
						m_curPos = StockPos.Mid;
						if (DoesToggleStockPoint)
						{
							FireArm.HasActiveShoulderStock = false;
						}
					}
				}
				else if (num < 0.1f)
				{
					m_curPos = StockPos.Open;
					if (DoesToggleStockPoint)
					{
						FireArm.HasActiveShoulderStock = true;
					}
				}
				else if (num > 0.98f)
				{
					m_curPos = StockPos.Closed;
					if (DoesToggleStockPoint)
					{
						FireArm.HasActiveShoulderStock = false;
					}
				}
				else
				{
					m_curPos = StockPos.Mid;
					if (DoesToggleStockPoint)
					{
						FireArm.HasActiveShoulderStock = false;
					}
				}
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
