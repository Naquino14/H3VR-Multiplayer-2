using UnityEngine;

namespace FistVR
{
	public class M72InteractionZone : FVRInteractiveObject
	{
		public enum M72InteractionType
		{
			Safety,
			Cap,
			TubeRear
		}

		public M72 M72;

		public M72InteractionType IntType;

		public override bool IsInteractable()
		{
			return IntType switch
			{
				M72InteractionType.Cap => M72.CanCapBeToggled(), 
				M72InteractionType.TubeRear => M72.CanTubeBeGrabbed(), 
				_ => base.IsInteractable(), 
			};
		}

		public override void SimpleInteraction(FVRViveHand hand)
		{
			switch (IntType)
			{
			case M72InteractionType.Safety:
				M72.ToggleSafety();
				break;
			case M72InteractionType.Cap:
				M72.ToggleCap();
				break;
			}
			base.SimpleInteraction(hand);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (IntType == M72InteractionType.TubeRear)
			{
				base.transform.position = GetClosestValidPoint(M72.Tube_Front.position, M72.Tube_Rear.position, base.m_handPos);
				float num = Mathf.InverseLerp(M72.Tube_Front.localPosition.z, M72.Tube_Rear.localPosition.z, base.transform.localPosition.z);
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			if (IntType == M72InteractionType.TubeRear)
			{
				float num = Vector3.Distance(base.transform.position, M72.Tube_Front.position);
				float num2 = Vector3.Distance(base.transform.position, M72.Tube_Rear.position);
				if (num < 0.01f)
				{
					M72.TState = M72.TubeState.Forward;
					M72.PlayAudioEvent(FirearmAudioEventType.StockClosed);
				}
				else if (num2 < 0.01f)
				{
					M72.TState = M72.TubeState.Rear;
					M72.PlayAudioEvent(FirearmAudioEventType.StockOpen);
					M72.PopUpRearSight();
				}
				else
				{
					M72.TState = M72.TubeState.Mid;
				}
			}
		}
	}
}
