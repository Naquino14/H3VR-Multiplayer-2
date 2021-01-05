using UnityEngine;

namespace FistVR
{
	public class wwExplodeableSpeaker : MonoBehaviour, IFVRDamageable
	{
		private bool m_isDestroyed;

		public GameObject Splode;

		public wwPASystem PASystem;

		public void Damage(Damage d)
		{
			if (d.Class == FistVR.Damage.DamageClass.Projectile && !m_isDestroyed)
			{
				m_isDestroyed = true;
				Object.Instantiate(Splode, base.transform.position, base.transform.rotation);
				PASystem.DestroySpeaker(this);
				Object.Destroy(base.gameObject);
			}
		}
	}
}
