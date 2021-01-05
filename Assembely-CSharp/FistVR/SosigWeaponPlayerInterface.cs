using UnityEngine;

namespace FistVR
{
	public class SosigWeaponPlayerInterface : FVRPhysicalObject
	{
		public SosigWeapon W;

		public override bool IsDistantGrabbable()
		{
			if (W.IsHeldByBot || W.IsInBotInventory)
			{
				return false;
			}
			return base.IsDistantGrabbable();
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			W.PlayerPickup();
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (!W.isFullAuto && hand.Input.TriggerDown && m_hasTriggeredUpSinceBegin)
			{
				W.FireGun(1f);
			}
			else if (W.isFullAuto && hand.Input.TriggerPressed && m_hasTriggeredUpSinceBegin)
			{
				W.FireGun(1f);
			}
		}

		public override GameObject DuplicateFromSpawnLock(FVRViveHand hand)
		{
			GameObject gameObject = base.DuplicateFromSpawnLock(hand);
			SosigWeapon component = gameObject.GetComponent<SosigWeapon>();
			component.SetAutoDestroy(b: true);
			return gameObject;
		}
	}
}
