using UnityEngine;

namespace FistVR
{
	public class BoltActionRifle_Handle : FVRInteractiveObject
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

		public BoltActionRifle Rifle;

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

		private bool m_wasTPInitiated;

		public bool UsesExtraRotationPiece;

		public Transform ExtraRotationPiece;

		public BoltActionHandleState HandleState;

		public BoltActionHandleState LastHandleState;

		public BoltActionHandleRot HandleRot = BoltActionHandleRot.Down;

		public BoltActionHandleRot LastHandleRot = BoltActionHandleRot.Down;

		private Vector3 m_localHandPos_BoltDown;

		private Vector3 m_localHandPos_BoltUp;

		private Vector3 m_localHandPos_BoltBack;

		private float fakeBoltDrive;

		protected override void Awake()
		{
			base.Awake();
			CalculateHandPoses();
		}

		public void TPInitiate()
		{
			m_wasTPInitiated = true;
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			m_wasTPInitiated = false;
			base.EndInteraction(hand);
		}

		public override bool IsInteractable()
		{
			if (!Rifle.CanBoltMove())
			{
				return false;
			}
			return base.IsInteractable();
		}

		private void CalculateHandPoses()
		{
			m_localHandPos_BoltDown = Rifle.transform.InverseTransformPoint(base.transform.position);
			Vector3 vector = base.transform.position - BoltActionHandleRoot.position;
			vector = Quaternion.AngleAxis(Mathf.Abs(MaxRot - MinRot) + 10f, BoltActionHandleRoot.forward) * vector;
			vector += BoltActionHandleRoot.position;
			m_localHandPos_BoltUp = Rifle.transform.InverseTransformPoint(vector);
			Vector3 position = vector + -BoltActionHandleRoot.forward * (0.005f + Vector3.Distance(Point_Forward.position, Point_Rearward.position));
			m_localHandPos_BoltBack = Rifle.transform.InverseTransformPoint(position);
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			Debug.DrawLine(Rifle.transform.TransformPoint(m_localHandPos_BoltDown), Rifle.transform.TransformPoint(m_localHandPos_BoltUp), Color.red);
			Debug.DrawLine(Rifle.transform.TransformPoint(m_localHandPos_BoltUp), Rifle.transform.TransformPoint(m_localHandPos_BoltBack), Color.blue);
		}

		public void DriveBolt(float amount)
		{
			if (Rifle.Clip != null)
			{
				Rifle.EjectClip();
				return;
			}
			fakeBoltDrive += amount;
			fakeBoltDrive = Mathf.Clamp(fakeBoltDrive, 0f, 1f);
			Vector3 position = ((!(fakeBoltDrive < 0.5f)) ? Vector3.Lerp(m_localHandPos_BoltUp, m_localHandPos_BoltBack, (fakeBoltDrive - 0.5f) * 2f) : Vector3.Slerp(m_localHandPos_BoltDown, m_localHandPos_BoltUp, fakeBoltDrive * 2f));
			position = Rifle.transform.TransformPoint(position);
			ManipulateBoltUsingPosition(position, touchpadDrive: true);
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
			Rifle.UpdateBolt(HandleState, num);
			LastHandleState = HandleState;
		}

		private bool ManipulateBoltUsingPosition(Vector3 pos, bool touchpadDrive)
		{
			bool result = false;
			if (HandleState == BoltActionHandleState.Forward)
			{
				Vector3 vector = pos - BoltActionHandle.position;
				vector = Vector3.ProjectOnPlane(vector, BoltActionHandleRoot.transform.forward).normalized;
				Vector3 right = BoltActionHandleRoot.transform.right;
				rotAngle = Mathf.Atan2(Vector3.Dot(BoltActionHandleRoot.forward, Vector3.Cross(right, vector)), Vector3.Dot(right, vector)) * 57.29578f;
				rotAngle += BaseRotOffset;
				rotAngle = Mathf.Clamp(rotAngle, MinRot, MaxRot);
				BoltActionHandle.localEulerAngles = new Vector3(0f, 0f, rotAngle);
				if (UsesExtraRotationPiece)
				{
					ExtraRotationPiece.localEulerAngles = new Vector3(0f, 0f, rotAngle);
				}
				if (rotAngle >= UnlockThreshold)
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
					Rifle.PlayAudioEvent(FirearmAudioEventType.HandleUp);
					if (Rifle.CockType == BoltActionRifle.HammerCockType.OnUp)
					{
						Rifle.CockHammer();
					}
				}
				else if (HandleRot == BoltActionHandleRot.Down && LastHandleRot != BoltActionHandleRot.Down)
				{
					Rifle.PlayAudioEvent(FirearmAudioEventType.HandleDown);
					if (Rifle.CockType == BoltActionRifle.HammerCockType.OnClose)
					{
						Rifle.CockHammer();
					}
					result = true;
				}
				LastHandleRot = HandleRot;
			}
			if (rotAngle >= UnlockThreshold)
			{
				Vector3 vector2 = HandPosOffset.x * BoltActionHandleRoot.right + HandPosOffset.y * BoltActionHandleRoot.up + HandPosOffset.z * BoltActionHandleRoot.forward;
				Vector3 closestValidPoint = GetClosestValidPoint(Point_Forward.position, Point_Rearward.position, pos - vector2);
				BoltActionHandleRoot.position = closestValidPoint;
			}
			return result;
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			bool flag = false;
			if (Rifle.Clip != null)
			{
				Rifle.EjectClip();
				return;
			}
			flag = ManipulateBoltUsingPosition(hand.transform.position, touchpadDrive: false);
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
			if ((HandleState == BoltActionHandleState.Forward && LastHandleState != 0) || HandleState != BoltActionHandleState.Rear || LastHandleState != BoltActionHandleState.Rear)
			{
			}
			Rifle.UpdateBolt(HandleState, num);
			LastHandleState = HandleState;
			base.UpdateInteraction(hand);
			if (flag && UsesQuickRelease && m_wasTPInitiated && (Rifle.IsAltHeld || !Rifle.IsHeld))
			{
				hand.Buzz(hand.Buzzer.Buzz_BeginInteraction);
				EndInteraction(hand);
				Rifle.BeginInteraction(hand);
				hand.ForceSetInteractable(Rifle);
				if (!hand.Input.TriggerPressed)
				{
					Rifle.SetHasTriggeredUp();
				}
			}
		}
	}
}
