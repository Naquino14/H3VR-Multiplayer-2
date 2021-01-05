using UnityEngine;

namespace FistVR
{
	public class BAPHandle : FVRInteractiveObject
	{
		public enum HandleSlideState
		{
			Forward,
			Mid,
			Rear
		}

		public enum HandleRotState
		{
			Closed,
			Mid,
			Open
		}

		public BAP Frame;

		public Transform HandleRoot;

		public Transform Handle;

		private float m_rotAngle;

		public float MinRot;

		public float MaxRot = 90f;

		public float UnlockThreshold = 87f;

		public Transform Point_Forward;

		public Transform Point_Rearward;

		public HandleSlideState CurHandleSlideState;

		public HandleSlideState LastHandleSlideState;

		public HandleRotState CurHandleRotState;

		public HandleRotState LastHandleRotState;

		private Vector3 lastHandForward = Vector3.zero;

		private Vector3 lastMountForward = Vector3.zero;

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			lastHandForward = m_hand.transform.up;
			lastMountForward = Frame.transform.up;
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			if (CurHandleSlideState == HandleSlideState.Forward)
			{
				float rotAngle = m_rotAngle;
				float rotAngle2 = m_rotAngle;
				Vector3 lhs = Vector3.ProjectOnPlane(hand.transform.up, Frame.transform.forward);
				Vector3 rhs = Vector3.ProjectOnPlane(lastHandForward, Frame.transform.forward);
				float num = Mathf.Atan2(Vector3.Dot(base.transform.forward, Vector3.Cross(lhs, rhs)), Vector3.Dot(lhs, rhs)) * 57.29578f;
				rotAngle2 -= num;
				Vector3 lhs2 = Vector3.ProjectOnPlane(Frame.transform.up, base.transform.forward);
				Vector3 rhs2 = Vector3.ProjectOnPlane(lastMountForward, base.transform.forward);
				num = Mathf.Atan2(Vector3.Dot(base.transform.forward, Vector3.Cross(lhs2, rhs2)), Vector3.Dot(lhs2, rhs2)) * 57.29578f;
				rotAngle2 += num;
				rotAngle2 = (m_rotAngle = Mathf.Clamp(rotAngle2, MinRot, MaxRot));
				lastHandForward = m_hand.transform.up;
				lastMountForward = Frame.transform.up;
				Handle.localEulerAngles = new Vector3(0f, 0f, m_rotAngle);
				if (m_rotAngle >= UnlockThreshold)
				{
					CurHandleRotState = HandleRotState.Open;
				}
				else if (Mathf.Abs(m_rotAngle - MinRot) < 3f)
				{
					CurHandleRotState = HandleRotState.Closed;
				}
				else
				{
					CurHandleRotState = HandleRotState.Mid;
				}
				if (CurHandleRotState == HandleRotState.Open && LastHandleRotState != HandleRotState.Open)
				{
					Frame.PlayAudioEvent(FirearmAudioEventType.HandleUp);
				}
				else if (CurHandleRotState == HandleRotState.Closed && LastHandleRotState != 0)
				{
					HandleRoot.localPosition = Point_Forward.localPosition;
					Frame.PlayAudioEvent(FirearmAudioEventType.HandleDown);
				}
				LastHandleRotState = CurHandleRotState;
			}
			if (m_rotAngle >= UnlockThreshold)
			{
				Vector3 closestValidPoint = GetClosestValidPoint(Point_Forward.position, Point_Rearward.position, hand.Input.FilteredPos);
				HandleRoot.position = closestValidPoint;
			}
			float num2 = Mathf.InverseLerp(Point_Forward.localPosition.z, Point_Rearward.localPosition.z, HandleRoot.localPosition.z);
			if (num2 < 0.05f)
			{
				CurHandleSlideState = HandleSlideState.Forward;
			}
			else if (num2 > 0.95f)
			{
				CurHandleSlideState = HandleSlideState.Rear;
			}
			else
			{
				CurHandleSlideState = HandleSlideState.Mid;
			}
			Frame.UpdateBolt(num2);
			LastHandleSlideState = CurHandleSlideState;
		}
	}
}
