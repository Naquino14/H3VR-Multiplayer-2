using UnityEngine;

namespace FistVR
{
	public class BangSnapFlameTrigger : MonoBehaviour, IFVRDamageable
	{
		public BangSnap BS;

		public bool IsFizzlable = true;

		private void OnTriggerEnter(Collider col)
		{
			if (IsFizzlable)
			{
				IsFizzlable = false;
				BS.Fizzle();
				Object.Destroy(base.gameObject);
			}
		}

		private void Ignite()
		{
			IsFizzlable = false;
			BS.Fizzle();
			Object.Destroy(base.gameObject);
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
