using UnityEngine;

namespace FistVR
{
	public class HandgunGiantFiringPin : MonoBehaviour, IFVRDamageable
	{
		public Handgun Handgun;

		public void Damage(Damage d)
		{
			Handgun.DropHammer(isManual: false);
		}
	}
}
