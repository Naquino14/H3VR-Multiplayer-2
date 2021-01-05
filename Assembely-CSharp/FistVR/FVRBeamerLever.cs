using UnityEngine;

namespace FistVR
{
	public class FVRBeamerLever : FVRInteractiveObject
	{
		[Header("Beamer Lever Config")]
		public Transform Holder;

		public HingeJoint Hinge;

		public FVRBeamer Beamer;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 vector = hand.transform.position - Holder.position;
			Vector3 vector2 = Vector3.ProjectOnPlane(vector, Holder.right);
			Debug.DrawLine(Holder.position, Holder.position + vector2, Color.green);
			float num = Vector3.Angle(vector2, Holder.forward);
			float value = Vector3.Angle(vector2, Holder.up);
			value = Mathf.Clamp(value, 0f, 50f);
			if (num > 90f)
			{
				JointSpring spring = Hinge.spring;
				spring.targetPosition = 0f - value;
				Hinge.spring = spring;
				Beamer.SetLocusMover(0f - value);
			}
			else
			{
				JointSpring spring2 = Hinge.spring;
				spring2.targetPosition = value;
				Hinge.spring = spring2;
				Beamer.SetLocusMover(value);
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			JointSpring spring = Hinge.spring;
			spring.targetPosition = 0f;
			Hinge.spring = spring;
			Beamer.SetLocusMover(0f);
		}
	}
}
