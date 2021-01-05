using UnityEngine;

namespace FistVR
{
	public class CarryHandleWaggle : AttachableForegrip
	{
		public FVRAlternateGrip MainForeGrip;

		public WaggleJoint Waggle;

		public Vector3 GrabEuler;

		private bool m_istranslatingGrabPoint;

		private Vector3 poseLerpLocalStart = Vector3.zero;

		private Vector3 poseLerpLocalEnd = Vector3.zero;

		private Vector3 forwardLerpLocalStart = Vector3.zero;

		private Vector3 forwardLerpLocalEnd = Vector3.zero;

		private Vector3 upLerpLocalStart = Vector3.zero;

		private Vector3 upLerpLocalEnd = Vector3.zero;

		private float poseLerp;

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			poseLerpLocalStart = PoseOverride.parent.InverseTransformPoint(MainForeGrip.PrimaryObject.GrabPointTransform.position);
			poseLerpLocalEnd = PoseOverride.localPosition;
			forwardLerpLocalStart = PoseOverride.parent.InverseTransformDirection(MainForeGrip.PrimaryObject.GrabPointTransform.forward);
			forwardLerpLocalEnd = PoseOverride.parent.InverseTransformDirection(PoseOverride.forward);
			upLerpLocalStart = PoseOverride.parent.InverseTransformDirection(MainForeGrip.PrimaryObject.GrabPointTransform.up);
			upLerpLocalEnd = PoseOverride.parent.InverseTransformDirection(PoseOverride.up);
			poseLerp = 0f;
			m_istranslatingGrabPoint = true;
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			Waggle.ResetParticlePos();
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			bool flag = false;
			if (m_istranslatingGrabPoint)
			{
				if (poseLerp < 1f)
				{
					poseLerp += Time.deltaTime * 4f;
					Vector3 position = Vector3.Lerp(poseLerpLocalStart, poseLerpLocalEnd, poseLerp);
					Vector3 position2 = PoseOverride.parent.TransformPoint(position);
					MainForeGrip.PrimaryObject.GrabPointTransform.position = position2;
					Vector3 direction = Vector3.Lerp(forwardLerpLocalStart, forwardLerpLocalEnd, poseLerp);
					Vector3 direction2 = Vector3.Lerp(upLerpLocalStart, upLerpLocalEnd, poseLerp);
					Vector3 forward = PoseOverride.parent.TransformDirection(direction);
					Vector3 upwards = PoseOverride.parent.TransformDirection(direction2);
					MainForeGrip.PrimaryObject.GrabPointTransform.rotation = Quaternion.LookRotation(forward, upwards);
				}
				else
				{
					m_istranslatingGrabPoint = false;
				}
			}
			if (MainForeGrip.LastGrabbedInGrip == this && (MainForeGrip.IsHeld || MainForeGrip.PrimaryObject.IsAltHeld))
			{
				flag = true;
			}
			if (flag)
			{
				Waggle.hingeGraphic.localEulerAngles = GrabEuler;
			}
			else
			{
				Waggle.Execute();
			}
		}
	}
}
