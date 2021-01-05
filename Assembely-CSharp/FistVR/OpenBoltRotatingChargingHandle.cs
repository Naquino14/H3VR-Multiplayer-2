using System;
using UnityEngine;

namespace FistVR
{
	public class OpenBoltRotatingChargingHandle : FVRInteractiveObject
	{
		public enum Placement
		{
			Forward,
			Middle,
			Rearward
		}

		[Header("ChargingHandle")]
		public Transform Handle;

		public Transform ReferenceVector;

		public float RotLimit;

		public OpenBoltReceiverBolt Bolt;

		public float ForwardSpeed = 360f;

		private float m_currentHandleZ;

		private Placement m_curPos;

		private Placement m_lastPos;

		protected override void Awake()
		{
			base.Awake();
			m_currentHandleZ = RotLimit;
			Vector3 forward = Quaternion.AngleAxis(m_currentHandleZ, ReferenceVector.up) * ReferenceVector.forward;
			Handle.rotation = Quaternion.LookRotation(forward, ReferenceVector.up);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 target = Vector3.ProjectOnPlane(m_hand.Input.Pos - Handle.transform.position, ReferenceVector.up);
			Vector3 v = Vector3.RotateTowards(ReferenceVector.forward, target, (float)Math.PI / 180f * RotLimit, 1f);
			float value = (m_currentHandleZ = AngleSigned(ReferenceVector.forward, v, ReferenceVector.up));
			Vector3 forward = Quaternion.AngleAxis(m_currentHandleZ, ReferenceVector.up) * ReferenceVector.forward;
			Handle.rotation = Quaternion.LookRotation(forward, ReferenceVector.up);
			float l = Mathf.InverseLerp(RotLimit, 0f - RotLimit, value);
			Bolt.ChargingHandleHeld(l);
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			Bolt.ChargingHandleReleased();
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			float num = Mathf.InverseLerp(RotLimit, 0f - RotLimit, m_currentHandleZ);
			if (num < 0.01f)
			{
				m_curPos = Placement.Forward;
			}
			else if (num > 0.99f)
			{
				m_curPos = Placement.Rearward;
			}
			else
			{
				m_curPos = Placement.Middle;
			}
			if (!base.IsHeld && Mathf.Abs(m_currentHandleZ - RotLimit) >= 0.01f)
			{
				m_currentHandleZ = Mathf.MoveTowards(m_currentHandleZ, RotLimit, Time.deltaTime * ForwardSpeed);
				Vector3 forward = Quaternion.AngleAxis(m_currentHandleZ, ReferenceVector.up) * ReferenceVector.forward;
				Handle.rotation = Quaternion.LookRotation(forward, ReferenceVector.up);
			}
			if (m_curPos == Placement.Forward && m_lastPos != 0)
			{
				Bolt.Receiver.PlayAudioEvent(FirearmAudioEventType.HandleForward);
			}
			else if (m_lastPos == Placement.Rearward && m_lastPos != Placement.Rearward)
			{
				Bolt.Receiver.PlayAudioEvent(FirearmAudioEventType.HandleBack);
			}
			m_lastPos = m_curPos;
		}

		public float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
		{
			return Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * 57.29578f;
		}
	}
}
