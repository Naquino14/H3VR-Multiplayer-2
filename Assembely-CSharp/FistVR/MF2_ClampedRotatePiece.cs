using UnityEngine;

namespace FistVR
{
	public class MF2_ClampedRotatePiece : FVRInteractiveObject
	{
		public Transform RefFrameForward;

		public Vector2 RotRange = new Vector2(-50f, 50f);

		private float m_rot;

		public Transform Rotpiece;

		public FVRPhysicalObject.Axis RotAxis;

		private Vector3 refDir = Vector3.one;

		public override void BeginInteraction(FVRViveHand hand)
		{
			Vector3 up = hand.Input.Up;
			refDir = Vector3.ProjectOnPlane(up, RefFrameForward.forward).normalized;
			base.BeginInteraction(hand);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			Vector3 up = hand.Input.Up;
			Vector3 normalized = Vector3.ProjectOnPlane(up, RefFrameForward.forward).normalized;
			Vector3 lhs = refDir;
			float value = Mathf.Atan2(Vector3.Dot(RefFrameForward.forward, Vector3.Cross(lhs, normalized)), Vector3.Dot(lhs, normalized)) * 57.29578f;
			value = Mathf.Clamp(value, -5f, 5f);
			m_rot += value;
			m_rot = Mathf.Clamp(m_rot, RotRange.x, RotRange.y);
			if (RotAxis == FVRPhysicalObject.Axis.X)
			{
				Rotpiece.localEulerAngles = new Vector3(m_rot, 0f, 0f);
			}
			else if (RotAxis == FVRPhysicalObject.Axis.Y)
			{
				Rotpiece.localEulerAngles = new Vector3(0f, m_rot, 0f);
			}
			else if (RotAxis == FVRPhysicalObject.Axis.Z)
			{
				Rotpiece.localEulerAngles = new Vector3(0f, 0f, m_rot);
			}
			refDir = normalized;
			base.UpdateInteraction(hand);
		}
	}
}
