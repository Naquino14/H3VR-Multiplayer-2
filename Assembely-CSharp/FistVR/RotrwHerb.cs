using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class RotrwHerb : FVRPhysicalObject
	{
		public enum HerbType
		{
			KatchupLeaf,
			MustardWillow,
			PricklyPickle,
			GiantBlueRaspberry,
			DeadlyEggplant
		}

		public AudioEvent AudEvent_Eat;

		public List<GameObject> PayloadOnDetonate;

		public HerbType Type;

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
					GM.ZMaster.EatHerb(Type);
				}
				Object.Destroy(base.gameObject);
			}
		}
	}
}
