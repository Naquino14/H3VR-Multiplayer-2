using UnityEngine;

namespace FistVR
{
	public class FVRMatchhead : MonoBehaviour, IFVRDamageable
	{
		public FVRStrikeAnyWhereMatch Match;

		public Collider collider;

		private void OnTriggerEnter(Collider col)
		{
			collider.enabled = false;
			collider.gameObject.layer = LayerMask.NameToLayer("NoCol");
			Match.Ignite();
		}

		public void Ignite()
		{
			Match.Ignite();
		}

		public void Damage(Damage d)
		{
			if (d.Dam_Thermal > 0f)
			{
				Ignite();
			}
		}
	}
}
