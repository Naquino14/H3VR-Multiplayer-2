using UnityEngine;

namespace FistVR
{
	public class LAPD2019BoltHandle : FVRInteractiveObject
	{
		public enum BoltActionHandleState
		{
			Forward,
			Mid,
			Rear
		}

		public enum BoltActionHandleRot
		{
			Up,
			Mid,
			Down
		}

		public LAPD2019 Gun;

		public bool UsesQuickRelease;

		public Transform BoltActionHandleRoot;

		public Transform BoltActionHandle;

		public float BaseRotOffset;

		private float rotAngle;

		public float MinRot;

		public float MaxRot;

		public float UnlockThreshold = 70f;

		public Transform Point_Forward;

		public Transform Point_Rearward;

		public Vector3 HandPosOffset = new Vector3(0f, 0f, 0f);

		[Header("CartridgeDoor")]
		public Transform CartridgeDoor;

		private float m_cartridgeDoorClosed;

		private float m_cartridgeDoorOpen = 72f;

		private float m_curCartridgeDoorRot;

		public BoltActionHandleState HandleState;

		public BoltActionHandleState LastHandleState;

		public BoltActionHandleRot HandleRot = BoltActionHandleRot.Down;

		public BoltActionHandleRot LastHandleRot = BoltActionHandleRot.Down;

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			Vector3 vector = BoltActionHandleRoot.position;
			bool flag = false;
			if (base.IsHeld)
			{
				if (HandleState == BoltActionHandleState.Forward)
				{
					Vector3 vector2 = m_hand.Input.Pos - BoltActionHandle.position;
					vector2 = Vector3.ProjectOnPlane(vector2, BoltActionHandleRoot.transform.forward).normalized;
					Vector3 right = BoltActionHandleRoot.transform.right;
					rotAngle = Mathf.Atan2(Vector3.Dot(BoltActionHandleRoot.forward, Vector3.Cross(right, vector2)), Vector3.Dot(right, vector2)) * 57.29578f;
					rotAngle += BaseRotOffset;
					rotAngle = Mathf.Clamp(rotAngle, MinRot, MaxRot);
					BoltActionHandle.localEulerAngles = new Vector3(0f, 0f, rotAngle);
					float t = Mathf.InverseLerp(MinRot, MaxRot, rotAngle);
					float z = Mathf.Lerp(m_cartridgeDoorClosed, m_cartridgeDoorOpen, t);
					CartridgeDoor.localEulerAngles = new Vector3(0f, 0f, z);
					if (Mathf.Abs(rotAngle - MaxRot) < 2f)
					{
						HandleRot = BoltActionHandleRot.Up;
					}
					else if (Mathf.Abs(rotAngle - MinRot) < 2f)
					{
						HandleRot = BoltActionHandleRot.Down;
					}
					else
					{
						HandleRot = BoltActionHandleRot.Mid;
					}
					if (HandleRot == BoltActionHandleRot.Up && LastHandleRot != 0)
					{
						Gun.PlayAudioEvent(FirearmAudioEventType.HandleUp);
					}
					else if (HandleRot == BoltActionHandleRot.Down && LastHandleRot != BoltActionHandleRot.Down)
					{
						Gun.PlayAudioEvent(FirearmAudioEventType.HandleDown);
					}
					LastHandleRot = HandleRot;
				}
				if (rotAngle >= UnlockThreshold)
				{
					Vector3 vector3 = HandPosOffset.x * BoltActionHandleRoot.right + HandPosOffset.y * BoltActionHandleRoot.up + HandPosOffset.z * BoltActionHandleRoot.forward;
					vector = GetClosestValidPoint(Point_Forward.position, Point_Rearward.position, m_hand.Input.Pos - vector3);
					flag = true;
				}
			}
			else if (HandleState != 0)
			{
				vector = Vector3.Lerp(vector, Point_Forward.position, Time.deltaTime * 12f);
				flag = true;
			}
			if (flag)
			{
				BoltActionHandleRoot.position = vector;
			}
			float num = Mathf.InverseLerp(Point_Forward.localPosition.z, Point_Rearward.localPosition.z, BoltActionHandleRoot.localPosition.z);
			if (num < 0.05f)
			{
				HandleState = BoltActionHandleState.Forward;
			}
			else if (num > 0.95f)
			{
				HandleState = BoltActionHandleState.Rear;
			}
			else
			{
				HandleState = BoltActionHandleState.Mid;
			}
			if (HandleState == BoltActionHandleState.Forward && LastHandleState != 0)
			{
				Gun.PlayAudioEvent(FirearmAudioEventType.HandleForward);
			}
			else if (HandleState == BoltActionHandleState.Rear && LastHandleState != BoltActionHandleState.Rear)
			{
				Gun.PlayAudioEvent(FirearmAudioEventType.HandleBack);
				Gun.EjectThermalClip();
			}
			LastHandleState = HandleState;
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
		}
	}
}
