using UnityEngine;

namespace FistVR
{
	public class MF2_FlamethrowerValve : FVRInteractiveObject
	{
		public Transform RefFrame;

		public Vector2 ValveRotRange = new Vector2(-50f, 50f);

		private float m_valveRot;

		public Transform Valve;

		public float Lerp = 0.5f;

		private Vector3 refDir = Vector3.one;

		public override void BeginInteraction(FVRViveHand hand)
		{
			Vector3 up = hand.Input.Up;
			refDir = Vector3.ProjectOnPlane(up, RefFrame.forward).normalized;
			base.BeginInteraction(hand);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			Vector3 up = hand.Input.Up;
			Vector3 normalized = Vector3.ProjectOnPlane(up, RefFrame.forward).normalized;
			Vector3 lhs = refDir;
			float value = Mathf.Atan2(Vector3.Dot(RefFrame.forward, Vector3.Cross(lhs, normalized)), Vector3.Dot(lhs, normalized)) * 57.29578f;
			value = Mathf.Clamp(value, -5f, 5f);
			m_valveRot += value;
			m_valveRot = Mathf.Clamp(m_valveRot, -50f, 50f);
			Valve.localEulerAngles = new Vector3(0f, 0f, m_valveRot);
			Lerp = Mathf.InverseLerp(ValveRotRange.x, ValveRotRange.y, m_valveRot);
			refDir = normalized;
			base.UpdateInteraction(hand);
		}
	}
}
