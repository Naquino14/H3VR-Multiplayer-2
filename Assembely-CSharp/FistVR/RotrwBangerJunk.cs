using UnityEngine;

namespace FistVR
{
	public class RotrwBangerJunk : MonoBehaviour
	{
		public enum BangerJunkType
		{
			TinCan,
			CoffeeCan,
			Bucket,
			Radio,
			FishFinder,
			BangSnaps,
			EggTimer
		}

		public FVRPhysicalObject O;

		public AudioEvent AudEvent_Eat;

		public BangerJunkType Type;

		public int MatIndex;

		public int ContainerSize = 1;

		public void Update()
		{
			if (!(O != null) || !O.IsHeld)
			{
				return;
			}
			FVRViveHand hand = O.m_hand;
			Vector3 b = GM.CurrentPlayerBody.Head.transform.position + GM.CurrentPlayerBody.Head.transform.up * -0.2f;
			if (Vector3.Distance(base.transform.position, b) < 0.15f)
			{
				O.EndInteraction(O.m_hand);
				hand.ForceSetInteractable(null);
				SM.PlayGenericSound(AudEvent_Eat, base.transform.position);
				if (GM.ZMaster != null)
				{
					GM.ZMaster.EatBangerJunk(Type, MatIndex);
				}
				Object.Destroy(base.gameObject);
			}
		}
	}
}
