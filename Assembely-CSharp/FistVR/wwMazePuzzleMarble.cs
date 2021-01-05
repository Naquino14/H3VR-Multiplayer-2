using UnityEngine;

namespace FistVR
{
	public class wwMazePuzzleMarble : FVRInteractiveObject
	{
		public bool IsMarbleLocked;

		public Transform Maze;

		public Collider ToRayCastAgainst;

		public LayerMask CastMask;

		public float SphereRadius = 0.1f;

		public override bool IsInteractable()
		{
			if (IsMarbleLocked)
			{
				return false;
			}
			return base.IsInteractable();
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 vector = hand.transform.position - base.transform.position;
			Vector3 vector2 = Vector3.ProjectOnPlane(vector, Maze.up);
			if (!Physics.SphereCast(base.transform.position, SphereRadius, vector2.normalized, out var _, vector2.magnitude, CastMask, QueryTriggerInteraction.Ignore))
			{
				base.transform.position = base.transform.position + vector2;
			}
		}
	}
}
