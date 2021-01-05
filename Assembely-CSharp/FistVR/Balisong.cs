using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class Balisong : FVRMeleeWeapon
	{
		public enum BalisongState
		{
			LockedClosed,
			LockedOpen,
			Free
		}

		public List<WaggleJoint> Joints;

		public List<Transform> JointTransforms;

		private bool m_isLocked;

		public Vector2 BladeYRotRange = new Vector2(-90f, 90f);

		public Vector2 SwingYRotRange = new Vector2(-87.2f, 87.4f);

		public Vector2 LockYRotRange = new Vector2(-92.77f, 90f);

		private BalisongState bState;

		public AudioEvent AudEvent_BalisongLatch;

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (bState == BalisongState.Free)
			{
				for (int i = 0; i < Joints.Count; i++)
				{
					Joints[i].Execute();
				}
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (base.IsHeld && m_hand.Input.TriggerDown && m_hasTriggeredUpSinceBegin)
			{
				ToggleBalisonLock();
			}
		}

		private void ToggleBalisonLock()
		{
			if (MP.IsJointedToObject)
			{
				return;
			}
			if (bState == BalisongState.LockedClosed || bState == BalisongState.LockedOpen)
			{
				bState = BalisongState.Free;
				SetLockRot(0.5f);
				MP.CanNewStab = false;
				for (int i = 0; i < Joints.Count; i++)
				{
					Joints[i].ResetParticlePos();
				}
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_BalisongLatch, base.transform.position);
			}
			else if (CanLockOpen())
			{
				SetBalisonRot(1f);
				bState = BalisongState.LockedOpen;
				SetLockRot(1f);
				MP.CanNewStab = true;
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_BalisongLatch, base.transform.position);
			}
			else if (CanLockShut())
			{
				SetBalisonRot(0f);
				bState = BalisongState.LockedClosed;
				SetLockRot(0f);
				MP.CanNewStab = false;
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_BalisongLatch, base.transform.position);
			}
		}

		private bool CanLockOpen()
		{
			float num = Mathf.Abs(JointTransforms[0].localEulerAngles.x - BladeYRotRange.y);
			float num2 = Mathf.Abs(JointTransforms[1].localEulerAngles.x - SwingYRotRange.y);
			if (num < 3f && num2 < 3f)
			{
				return true;
			}
			return false;
		}

		private bool CanLockShut()
		{
			float num = JointTransforms[0].localEulerAngles.x;
			float num2 = JointTransforms[1].localEulerAngles.x;
			if (num < -90.1f)
			{
				num += 180f;
			}
			if (num2 < -90.1f)
			{
				num2 += 180f;
			}
			float num3 = Mathf.Abs(num - BladeYRotRange.x);
			float num4 = Mathf.Abs(num2 - SwingYRotRange.x);
			if (Mathf.Abs(num3) > 359f)
			{
				num3 = Mathf.Abs(num3) - 359f;
			}
			if (Mathf.Abs(num4) > 359f)
			{
				num4 = Mathf.Abs(num4) - 359f;
			}
			if (num3 < 3f && num4 < 3f)
			{
				return true;
			}
			return false;
		}

		private void SetBalisonRot(float f)
		{
			JointTransforms[0].localEulerAngles = new Vector3(Mathf.Lerp(BladeYRotRange.x, BladeYRotRange.y, f), 0f, 0f);
			JointTransforms[1].localEulerAngles = new Vector3(Mathf.Lerp(SwingYRotRange.x, SwingYRotRange.y, f), 0f, 0f);
		}

		private void SetLockRot(float f)
		{
			JointTransforms[2].localEulerAngles = new Vector3(Mathf.Lerp(LockYRotRange.x, LockYRotRange.y, f), 0f, 0f);
		}
	}
}
