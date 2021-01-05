using UnityEngine;

namespace FistVR
{
	public class FVRPivotLockerControl : FVRInteractiveObject
	{
		public FVRPivotLocker Locker;

		public string Axis = "X";

		private Vector3 m_lastPos = Vector3.zero;

		public bool isRotControl;

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			m_lastPos = Locker.transform.InverseTransformPoint(hand.Input.Pos);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 vector = Locker.transform.InverseTransformPoint(hand.Input.Pos);
			Vector3 vector2 = vector - m_lastPos;
			if (!isRotControl)
			{
				switch (Axis)
				{
				case "X":
					vector2 = Vector3.ProjectOnPlane(vector2, Vector3.up);
					vector2 = Vector3.ProjectOnPlane(vector2, Vector3.forward);
					break;
				case "Y":
					vector2 = Vector3.ProjectOnPlane(vector2, Vector3.right);
					vector2 = Vector3.ProjectOnPlane(vector2, Vector3.forward);
					break;
				case "Z":
					vector2 = Vector3.ProjectOnPlane(vector2, Vector3.right);
					vector2 = Vector3.ProjectOnPlane(vector2, Vector3.up);
					break;
				}
				Locker.SlideOnAxis(vector2);
			}
			else
			{
				switch (Axis)
				{
				case "X":
					vector2 = Vector3.ProjectOnPlane(vector2, Vector3.right);
					vector2 = Vector3.ProjectOnPlane(vector2, Vector3.up);
					Locker.RotateOnAxis("X", vector2.z * 45f);
					break;
				case "Y":
					vector2 = Vector3.ProjectOnPlane(vector2, Vector3.up);
					vector2 = Vector3.ProjectOnPlane(vector2, Vector3.forward);
					Locker.RotateOnAxis("Y", vector2.x * 45f);
					break;
				case "Z":
					vector2 = Vector3.ProjectOnPlane(vector2, Vector3.up);
					vector2 = Vector3.ProjectOnPlane(vector2, Vector3.forward);
					Locker.RotateOnAxis("Z", (0f - vector2.x) * 45f);
					break;
				}
			}
			m_lastPos = Locker.transform.InverseTransformPoint(hand.Input.Pos);
		}
	}
}
