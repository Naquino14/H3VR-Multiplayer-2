using UnityEngine;

namespace FistVR
{
	public class FVRSoundOnShot : MonoBehaviour, IFVRDamageable
	{
		private AudioSource m_aud;

		private float timeSinceShot;

		private void Awake()
		{
			m_aud = GetComponent<AudioSource>();
		}

		private void Update()
		{
			if (timeSinceShot < 1f)
			{
				timeSinceShot += Time.deltaTime;
			}
		}

		public void Damage(Damage dam)
		{
			if (dam.Class == FistVR.Damage.DamageClass.Projectile && timeSinceShot >= 0.2f)
			{
				timeSinceShot = 0f;
				m_aud.pitch = Random.Range(0.95f, 1.05f);
				m_aud.PlayOneShot(m_aud.clip, Random.Range(0.2f, 0.3f));
			}
		}
	}
}
