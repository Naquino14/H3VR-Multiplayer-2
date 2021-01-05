using UnityEngine;

namespace FistVR
{
	public class LAPD2019BatteryReleaseTrigger : FVRInteractiveObject
	{
		public LAPD2019 Gun;

		public override bool IsInteractable()
		{
			if (Gun.HasBattery)
			{
				return true;
			}
			return false;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			float num = Gun.ExtractBattery(hand);
			if (num >= -0.1f)
			{
				GameObject gameObject = Object.Instantiate(Gun.BatteryPrefab.GetGameObject(), hand.transform.position, hand.transform.rotation);
				LAPD2019Battery component = gameObject.GetComponent<LAPD2019Battery>();
				hand.ForceSetInteractable(component);
				component.BeginInteraction(hand);
				component.SetEnergy(num);
			}
		}
	}
}
