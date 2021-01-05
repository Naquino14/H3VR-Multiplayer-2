using UnityEngine;

namespace FistVR
{
	public class SimpleLauncherFireOnDamage : MonoBehaviour, IFVRDamageable
	{
		public SimpleLauncher SL;

		public void Damage(Damage d)
		{
			SL.Fire();
		}
	}
}
