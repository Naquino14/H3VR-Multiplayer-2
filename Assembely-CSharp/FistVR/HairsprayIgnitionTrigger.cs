using UnityEngine;

namespace FistVR
{
	public class HairsprayIgnitionTrigger : MonoBehaviour, IFVRDamageable
	{
		public HairsprayCan can;

		public void Damage(Damage d)
		{
			if (d.Dam_Thermal > 0f)
			{
				can.Ignite();
			}
		}

		public void OnParticleCollision(GameObject other)
		{
			if (other.CompareTag("IgnitorSystem"))
			{
				can.Ignite();
			}
		}
	}
}
