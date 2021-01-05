using UnityEngine;

namespace FistVR
{
	public class FVRAlternateGrip : FVRInteractiveObject
	{
		[Header("Alternate Grip Config")]
		public FVRPhysicalObject PrimaryObject;

		public bool FunctionalityEnabled = true;

		private Vector3 m_origPoseOverridePos = Vector3.zero;

		private bool m_wasGrabbedFromAttachableForegrip;

		private AttachableForegrip m_lastGrabbedInGrip;

		private bool m_hasSavedPalmPoint;

		private Vector3 m_savedRigPalmPos = Vector3.zero;

		public bool DoesBracing = true;

		private bool tempFlag;

		public AttachableForegrip LastGrabbedInGrip => m_lastGrabbedInGrip;

		protected override void Awake()
		{
			base.Awake();
			m_origPoseOverridePos = PoseOverride.localPosition;
		}

		private void SetSavedRigLocalPos()
		{
			m_savedRigPalmPos = m_hand.PalmTransform.position;
			m_hasSavedPalmPoint = true;
			PrimaryObject.UseFilteredHandPosition = true;
			PrimaryObject.UseFilteredHandRotation = true;
		}

		private void ClearSavedPalmPoint()
		{
			if (m_hasSavedPalmPoint)
			{
				PrimaryObject.UseFilteredHandPosition = false;
				PrimaryObject.UseFilteredHandRotation = false;
				m_hasSavedPalmPoint = false;
				if (m_hand != null)
				{
					m_hand.Buzz(m_hand.Buzzer.Buzz_OnHoverInteractive);
				}
			}
		}

		private void AttemptToGenerateSavedPalmPoint()
		{
			if (Physics.CheckSphere(m_hand.PalmTransform.position, 0.1f, m_hand.BracingMask, QueryTriggerInteraction.Ignore))
			{
				SetSavedRigLocalPos();
			}
		}

		public Vector3 GetPalmPos(bool isDirectParent)
		{
			if (m_hasSavedPalmPoint)
			{
				return m_savedRigPalmPos;
			}
			if (isDirectParent)
			{
				return m_hand.PalmTransform.position;
			}
			return Vector3.Lerp(m_hand.Input.LastPalmPos1, m_hand.PalmTransform.position, 0f);
		}

		public bool HasLastGrabbedGrip()
		{
			return tempFlag;
		}

		public AttachableForegrip GetLastGrabbedGrip()
		{
			return m_lastGrabbedInGrip;
		}

		public override bool IsInteractable()
		{
			return true;
		}

		public override void PlayGrabSound(bool isHard, FVRViveHand hand)
		{
			if (PrimaryObject.IsHeld)
			{
				isHard = false;
			}
			if (HandlingGrabSound != 0)
			{
				if (hand.CanMakeGrabReleaseSound)
				{
					SM.PlayHandlingGrabSound(HandlingGrabSound, hand.Input.Pos, isHard);
					hand.HandMadeGrabReleaseSound();
				}
			}
			else if (hand.CanMakeGrabReleaseSound)
			{
				SM.PlayHandlingGrabSound(PrimaryObject.HandlingGrabSound, hand.Input.Pos, isHard);
				hand.HandMadeGrabReleaseSound();
			}
		}

		public void BeginInteractionFromAttachedGrip(AttachableForegrip aGrip, FVRViveHand hand)
		{
			if (aGrip == null)
			{
				m_wasGrabbedFromAttachableForegrip = false;
				tempFlag = false;
				m_lastGrabbedInGrip = null;
				BeginInteraction(hand);
			}
			else
			{
				PoseOverride.position = aGrip.ForePosePoint.position;
				m_wasGrabbedFromAttachableForegrip = true;
				BeginInteraction(hand);
				m_lastGrabbedInGrip = aGrip;
				tempFlag = true;
			}
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			PlayGrabSound(!base.IsHeld, hand);
			if (PrimaryObject is FVRFireArm)
			{
				FVRFireArm fVRFireArm = PrimaryObject as FVRFireArm;
				if (fVRFireArm.Bipod != null && fVRFireArm.Bipod.IsBipodActive)
				{
					fVRFireArm.Bipod.Deactivate();
				}
			}
			if (!m_wasGrabbedFromAttachableForegrip)
			{
				PoseOverride.localPosition = m_origPoseOverridePos;
				if (base.GrabPointTransform != null)
				{
					base.GrabPointTransform.localPosition = m_origPoseOverridePos;
				}
				m_lastGrabbedInGrip = null;
			}
			if (!PrimaryObject.IsHeld || PrimaryObject.IsAltHeld)
			{
				PrimaryObject.BeginInteractionThroughAltGrip(hand, this);
				return;
			}
			base.BeginInteraction(hand);
			if (FunctionalityEnabled)
			{
				PrimaryObject.AltGrip = this;
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector2 touchpadAxes = hand.Input.TouchpadAxes;
			bool flag = true;
			if (!DoesBracing)
			{
				flag = false;
			}
			if (m_wasGrabbedFromAttachableForegrip && !m_lastGrabbedInGrip.DoesBracing)
			{
				flag = false;
			}
			if (PrimaryObject.IsAltHeld)
			{
				flag = false;
			}
			if (flag && hand.Input.TriggerPressed)
			{
				if (!m_hasSavedPalmPoint)
				{
					AttemptToGenerateSavedPalmPoint();
					if (m_hasSavedPalmPoint)
					{
						hand.Buzz(hand.Buzzer.Buzz_BeginInteraction);
					}
				}
				else if (Vector3.Distance(m_savedRigPalmPos, m_hand.PalmTransform.position) > 0.2f)
				{
					ClearSavedPalmPoint();
				}
			}
			else if (m_hasSavedPalmPoint)
			{
				ClearSavedPalmPoint();
			}
			if (hand.Input.TriggerUp)
			{
				ClearSavedPalmPoint();
			}
			if (m_wasGrabbedFromAttachableForegrip && m_lastGrabbedInGrip != null && m_lastGrabbedInGrip.Attachment != null && m_lastGrabbedInGrip.Attachment.curMount != null && !m_lastGrabbedInGrip.HasAttachmentsOnIt() && m_lastGrabbedInGrip.Attachment.CanDetach())
			{
				bool flag2 = false;
				if (hand.IsInStreamlinedMode)
				{
					if (hand.Input.AXButtonDown)
					{
						flag2 = true;
					}
				}
				else if (hand.Input.TouchpadDown && Vector2.Angle(Vector2.down, touchpadAxes) < 45f && touchpadAxes.magnitude > 0.2f)
				{
					flag2 = true;
				}
				if (flag2)
				{
					EndInteraction(hand);
					m_lastGrabbedInGrip.DetachRoutine(hand);
				}
			}
			PassHandInput(hand, this);
		}

		public virtual void PassHandInput(FVRViveHand hand, FVRInteractiveObject o)
		{
			if (m_lastGrabbedInGrip != null)
			{
				m_lastGrabbedInGrip.PassHandInput(hand, o);
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			ClearSavedPalmPoint();
			PoseOverride.localPosition = m_origPoseOverridePos;
			m_wasGrabbedFromAttachableForegrip = false;
			if (FunctionalityEnabled && PrimaryObject.AltGrip == this)
			{
				PrimaryObject.AltGrip = null;
			}
		}

		public override void TestHandDistance()
		{
		}
	}
}
