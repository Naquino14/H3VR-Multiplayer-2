using UnityEngine;

namespace FistVR
{
	public class FlintlockDamageableSensor : MonoBehaviour, IFVRDamageable
	{
		public FlintlockFlashPan Pan;

		public FlintlockBarrel Barrel;

		public void Damage(Damage d)
		{
			if (d.Dam_Thermal > 1f)
			{
				if (Pan != null && Pan.FrizenState == FlintlockFlashPan.FState.Up)
				{
					Pan.Ignite();
				}
				if (Barrel != null)
				{
					Barrel.BurnOffOuter();
				}
			}
		}
	}
}
