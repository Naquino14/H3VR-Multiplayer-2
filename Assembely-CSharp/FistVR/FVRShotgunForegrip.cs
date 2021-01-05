using UnityEngine;

namespace FistVR
{
	public class FVRShotgunForegrip : FVRAlternateGrip
	{
		public Transform ShotgunBase;

		public HingeJoint Hinge;

		private Vector3 localPosStart;

		private Rigidbody RB;

		private BreakActionWeapon Wep;

		protected override void Awake()
		{
			base.Awake();
			localPosStart = Hinge.transform.localPosition;
			RB = Hinge.gameObject.GetComponent<Rigidbody>();
			Wep = Hinge.connectedBody.gameObject.GetComponent<BreakActionWeapon>();
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (Vector3.Distance(Hinge.transform.localPosition, localPosStart) > 0.01f)
			{
				Hinge.transform.localPosition = localPosStart;
			}
		}

		protected override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			if (Wep.IsHeld && Wep.IsAltHeld)
			{
				RB.mass = 0.001f;
			}
			else
			{
				RB.mass = 0.1f;
			}
		}

		public override bool IsInteractable()
		{
			return true;
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 vector = hand.transform.position - Hinge.transform.position;
			Vector3 from = Vector3.ProjectOnPlane(vector, ShotgunBase.right);
			if (Vector3.Angle(from, -ShotgunBase.up) > 90f)
			{
				from = ShotgunBase.forward;
			}
			if (Vector3.Angle(from, ShotgunBase.forward) > 90f)
			{
				from = -ShotgunBase.up;
			}
			float value = Vector3.Angle(from, ShotgunBase.forward);
			JointSpring spring = Hinge.spring;
			spring.spring = 10f;
			spring.damper = 0f;
			spring.targetPosition = Mathf.Clamp(value, 0f, Hinge.limits.max);
			Hinge.spring = spring;
			Hinge.transform.localPosition = localPosStart;
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			JointSpring spring = Hinge.spring;
			spring.spring = 0.5f;
			spring.damper = 0.05f;
			spring.targetPosition = 45f;
			Hinge.spring = spring;
			base.EndInteraction(hand);
		}
	}
}
