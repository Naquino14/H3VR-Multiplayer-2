using UnityEngine;

namespace FistVR
{
	public class BAPInteractable : FVRInteractiveObject
	{
		public BAP Frame;

		public override bool IsInteractable()
		{
			if (!Frame.HasBaffle && !Frame.HasCap)
			{
				return false;
			}
			return true;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			if (Frame.HasCap)
			{
				EndInteraction(hand);
				GameObject gameObject = Object.Instantiate(Frame.Prefab_Cap, Frame.PPoint_Cap.position, Frame.PPoint_Cap.rotation);
				BAPCap component = gameObject.GetComponent<BAPCap>();
				Frame.SetCapState(hasC: false);
				hand.ForceSetInteractable(component);
				component.BeginInteraction(hand);
				Frame.RemoveThing();
			}
			else if (Frame.HasBaffle)
			{
				EndInteraction(hand);
				GameObject gameObject2 = Object.Instantiate(Frame.Prefab_Baffle, Frame.PPoint_Baffle.position, Frame.PPoint_Baffle.rotation);
				BAPBaffle component2 = gameObject2.GetComponent<BAPBaffle>();
				component2.SetState(Frame.BaffleState);
				Frame.SetBaffleState(hasB: false, 0);
				hand.ForceSetInteractable(component2);
				component2.BeginInteraction(hand);
				Frame.RemoveThing();
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (Frame.CanDetectPiece() && (!Frame.HasBaffle || !Frame.HasCap))
			{
				if (!Frame.HasCap && !Frame.HasBaffle && other.gameObject.GetComponent<BAPBaffle>() != null)
				{
					BAPBaffle component = other.gameObject.GetComponent<BAPBaffle>();
					Frame.SetBaffleState(hasB: true, component.BaffleState);
					Object.Destroy(component.GameObject);
				}
				else if (!Frame.HasCap && other.gameObject.GetComponent<BAPCap>() != null)
				{
					BAPCap component2 = other.gameObject.GetComponent<BAPCap>();
					Frame.SetCapState(hasC: true);
					Object.Destroy(component2.GameObject);
				}
			}
		}
	}
}
