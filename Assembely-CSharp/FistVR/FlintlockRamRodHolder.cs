using UnityEngine;

namespace FistVR
{
	public class FlintlockRamRodHolder : MonoBehaviour
	{
		public FlintlockWeapon Weapon;

		public void OnTriggerEnter(Collider other)
		{
			if (other.attachedRigidbody == null)
			{
				return;
			}
			GameObject gameObject = other.attachedRigidbody.gameObject;
			if (gameObject.CompareTag("flintlock_ramrod"))
			{
				FlintlockRamRod component = gameObject.GetComponent<FlintlockRamRod>();
				if (component.IsHeld)
				{
					FVRViveHand hand = component.m_hand;
					component.ForceBreakInteraction();
					Weapon.RamRod.gameObject.SetActive(value: true);
					Weapon.RamRod.RState = FlintlockPseudoRamRod.RamRodState.Lower;
					Weapon.RamRod.MountToUnder(hand);
					hand.ForceSetInteractable(Weapon.RamRod);
					Weapon.RamRod.BeginInteraction(hand);
					Object.Destroy(other.gameObject);
				}
			}
		}
	}
}
