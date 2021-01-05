using UnityEngine;

namespace FistVR
{
	public class SuppressorInterface : MuzzleDeviceInterface
	{
		private Suppressor tempSup;

		private Vector3 lastHandForward = Vector3.zero;

		private Vector3 lastMountForward = Vector3.zero;

		protected override void Awake()
		{
			base.Awake();
			tempSup = Attachment as Suppressor;
		}

		public override void OnAttach()
		{
			tempSup = Attachment as Suppressor;
			if (Attachment.curMount.GetRootMount().Parent is FVRFireArm)
			{
				FVRFireArm fVRFireArm = Attachment.curMount.GetRootMount().Parent as FVRFireArm;
				fVRFireArm.RegisterSuppressor(tempSup);
			}
			base.OnAttach();
		}

		public override void OnDetach()
		{
			if (Attachment.curMount.GetRootMount().Parent is FVRFireArm)
			{
				FVRFireArm fVRFireArm = Attachment.curMount.GetRootMount().Parent as FVRFireArm;
				fVRFireArm.RegisterSuppressor(null);
			}
			base.OnDetach();
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			lastHandForward = m_hand.transform.up;
			lastMountForward = Attachment.curMount.transform.up;
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (base.IsHeld)
			{
				Attachment.transform.localEulerAngles = new Vector3(0f, 0f, tempSup.CatchRot);
			}
			if (tempSup.CatchRot > 5f)
			{
				IsLocked = true;
				return;
			}
			IsLocked = false;
			Vector3 closestValidPoint = GetClosestValidPoint(Attachment.curMount.Point_Front.position, (Attachment.curMount.GetRootMount().MyObject as FVRFireArm).MuzzlePos.position, base.transform.position);
			if (Vector3.Distance(m_hand.transform.position, closestValidPoint) > 0.08f)
			{
				base.EndInteraction(hand);
				hand.ForceSetInteractable(Attachment);
				Attachment.BeginInteraction(hand);
			}
		}

		protected override void FVRFixedUpdate()
		{
			if (base.IsHeld)
			{
				float catchRot = tempSup.CatchRot;
				Vector3 lhs = Vector3.ProjectOnPlane(m_hand.transform.up, base.transform.forward);
				Vector3 rhs = Vector3.ProjectOnPlane(lastHandForward, base.transform.forward);
				float num = Mathf.Atan2(Vector3.Dot(base.transform.forward, Vector3.Cross(lhs, rhs)), Vector3.Dot(lhs, rhs)) * 57.29578f;
				tempSup.CatchRot -= num;
				Vector3 lhs2 = Vector3.ProjectOnPlane(Attachment.curMount.transform.up, base.transform.forward);
				Vector3 rhs2 = Vector3.ProjectOnPlane(lastMountForward, base.transform.forward);
				num = Mathf.Atan2(Vector3.Dot(base.transform.forward, Vector3.Cross(lhs2, rhs2)), Vector3.Dot(lhs2, rhs2)) * 57.29578f;
				tempSup.CatchRot += num;
				tempSup.CatchRot = Mathf.Clamp(tempSup.CatchRot, 0f, 360f);
				tempSup.CatchRotDeltaAdd(Mathf.Abs(tempSup.CatchRot - catchRot));
				lastHandForward = m_hand.transform.up;
				lastMountForward = Attachment.curMount.transform.up;
			}
			base.FVRFixedUpdate();
		}
	}
}
