using UnityEngine;

namespace FistVR
{
	public class GBeamerModeLever : FVRInteractiveObject
	{
		public enum LeverMode
		{
			Forward,
			Rearward,
			InBetween
		}

		public GBeamer GBeam;

		public Transform Point_Forward;

		public Transform Point_Rearward;

		public LeverMode Mode = LeverMode.Rearward;

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			Vector3 pos = hand.Input.Pos;
			pos = GetClosestValidPoint(Point_Forward.position, Point_Rearward.position, pos);
			if (Vector3.Distance(pos, Point_Forward.position) < Vector3.Distance(pos, Point_Rearward.position))
			{
				Mode = LeverMode.Forward;
				base.transform.rotation = Quaternion.LookRotation(Point_Forward.position - base.transform.position, GBeam.transform.right);
			}
			else
			{
				Mode = LeverMode.Rearward;
				base.transform.rotation = Quaternion.LookRotation(Point_Rearward.position - base.transform.position, GBeam.transform.right);
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 pos = hand.Input.Pos;
			pos = GetClosestValidPoint(Point_Forward.position, Point_Rearward.position, pos);
			base.transform.rotation = Quaternion.LookRotation(pos - base.transform.position, GBeam.transform.right);
			if (Vector3.Distance(pos, Point_Forward.position) < 0.1f)
			{
				Mode = LeverMode.Forward;
			}
			else if (Vector3.Distance(pos, Point_Rearward.position) < 0.1f)
			{
				Mode = LeverMode.Rearward;
			}
			else
			{
				Mode = LeverMode.InBetween;
			}
		}
	}
}
