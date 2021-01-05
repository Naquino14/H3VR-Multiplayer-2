using UnityEngine;

namespace FistVR
{
	public class OpenBoltChargingHandle : FVRInteractiveObject
	{
		public enum BoltHandlePos
		{
			Forward,
			Middle,
			Rear
		}

		[Header("ChargingHandle")]
		public OpenBoltReceiver Receiver;

		public Transform Point_Fore;

		public Transform Point_Rear;

		public OpenBoltReceiverBolt Bolt;

		public float ForwardSpeed = 1f;

		private float m_boltZ_forward;

		private float m_boltZ_rear;

		private float m_currentHandleZ;

		public BoltHandlePos CurPos;

		public BoltHandlePos LastPos;

		[Header("Rotating Bit")]
		public bool HasRotatingPart;

		public Transform RotatingPart;

		public Vector3 RotatingPartNeutralEulers;

		public Vector3 RotatingPartLeftEulers;

		public Vector3 RotatingPartRightEulers;

		protected override void Awake()
		{
			base.Awake();
			m_boltZ_forward = Point_Fore.localPosition.z;
			m_boltZ_rear = Point_Rear.localPosition.z;
			m_currentHandleZ = base.transform.localPosition.z;
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 closestValidPoint = GetClosestValidPoint(Point_Fore.position, Point_Rear.position, m_hand.Input.Pos);
			base.transform.position = closestValidPoint;
			m_currentHandleZ = base.transform.localPosition.z;
			float l = Mathf.InverseLerp(m_boltZ_forward, m_boltZ_rear, m_currentHandleZ);
			Bolt.ChargingHandleHeld(l);
			if (HasRotatingPart)
			{
				Vector3 normalized = (base.transform.position - m_hand.PalmTransform.position).normalized;
				if (Vector3.Dot(normalized, base.transform.right) > 0f)
				{
					RotatingPart.localEulerAngles = RotatingPartLeftEulers;
				}
				else
				{
					RotatingPart.localEulerAngles = RotatingPartRightEulers;
				}
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			if (HasRotatingPart)
			{
				RotatingPart.localEulerAngles = RotatingPartNeutralEulers;
			}
			base.EndInteraction(hand);
			Bolt.ChargingHandleReleased();
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (!base.IsHeld && Mathf.Abs(m_currentHandleZ - m_boltZ_forward) > 0.001f)
			{
				m_currentHandleZ = Mathf.MoveTowards(m_currentHandleZ, m_boltZ_forward, Time.deltaTime * ForwardSpeed);
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, m_currentHandleZ);
			}
			if (Mathf.Abs(m_currentHandleZ - m_boltZ_forward) < 0.005f)
			{
				CurPos = BoltHandlePos.Forward;
			}
			else if (Mathf.Abs(m_currentHandleZ - m_boltZ_rear) < 0.005f)
			{
				CurPos = BoltHandlePos.Rear;
			}
			else
			{
				CurPos = BoltHandlePos.Middle;
			}
			if (CurPos == BoltHandlePos.Forward && LastPos != 0)
			{
				if (Receiver != null)
				{
					Receiver.PlayAudioEvent(FirearmAudioEventType.HandleForward);
				}
			}
			else if (CurPos == BoltHandlePos.Rear && LastPos != BoltHandlePos.Rear && Receiver != null)
			{
				Receiver.PlayAudioEvent(FirearmAudioEventType.HandleBack);
			}
			LastPos = CurPos;
		}
	}
}
