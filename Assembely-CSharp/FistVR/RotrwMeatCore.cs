using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class RotrwMeatCore : FVRPhysicalObject
	{
		public enum CoreType
		{
			Tasty,
			Moldy,
			Spikey,
			Zippy,
			Weighty,
			Juicy,
			Shiny,
			Burny
		}

		public CoreType Type;

		public AudioEvent AudEvent_Eat;

		public List<GameObject> BangerSplosions;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 b = GM.CurrentPlayerBody.Head.transform.position + GM.CurrentPlayerBody.Head.transform.up * -0.2f;
			if (Vector3.Distance(base.transform.position, b) < 0.15f)
			{
				EndInteraction(hand);
				hand.ForceSetInteractable(null);
				SM.PlayGenericSound(AudEvent_Eat, base.transform.position);
				if (GM.ZMaster != null)
				{
					GM.ZMaster.EatMeatCore(Type);
				}
				Object.Destroy(base.gameObject);
			}
		}
	}
}
