using UnityEngine;

namespace FistVR
{
	public class SosigGrenadePin : FVRInteractiveObject
	{
		public SosigWeapon Grenade;

		public GameObject PinDiscardGO;

		public Renderer PinRend;

		private bool hasSpawned;

		public override void SimpleInteraction(FVRViveHand hand)
		{
			if (Grenade.O.QuickbeltSlot == null)
			{
				Grenade.FuseGrenade();
				SpawnPinAndDisableProxy();
			}
			base.SimpleInteraction(hand);
		}

		public void ForceExpelPin()
		{
			if (!hasSpawned)
			{
				Grenade.FuseGrenade();
				SpawnPinAndDisableProxy();
			}
		}

		private void SpawnPinAndDisableProxy()
		{
			if (!hasSpawned)
			{
				hasSpawned = true;
				if (PinRend != null)
				{
					PinRend.enabled = false;
				}
				GameObject gameObject = Object.Instantiate(PinDiscardGO, base.transform.position, base.transform.rotation);
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				component.velocity = Grenade.transform.right;
			}
		}
	}
}
