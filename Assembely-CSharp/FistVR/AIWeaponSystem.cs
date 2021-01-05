using UnityEngine;

namespace FistVR
{
	public class AIWeaponSystem : MonoBehaviour
	{
		public Transform Muzzle;

		public float FiringAngleThreshold = 5f;

		private int m_burstLength = 3;

		private int m_burstShotIndex;

		private float m_boltRefireRate = 0.42f;

		private float m_boltRefireTick;

		private float m_burstRefireRate = 1.2f;

		private float m_burstRefireTick;

		public GameObject BulletPrefab;

		public ParticleSystem SmokePSystem;

		private bool shouldFire;

		private AudioSource m_gunAudio;

		public AudioClip[] Aud_GunFire;

		public AudioClip Aud_Reload;

		private void Awake()
		{
			m_gunAudio = GetComponent<AudioSource>();
		}

		public void SetShouldFire(bool b)
		{
			shouldFire = b;
		}

		public void UpdateWeaponSystem()
		{
			if (m_burstRefireTick <= 0f)
			{
				if (m_boltRefireTick <= 0f)
				{
					if (shouldFire)
					{
						if (m_burstShotIndex < m_burstLength)
						{
							m_boltRefireTick = m_boltRefireRate;
							m_burstShotIndex++;
							FireBullet();
						}
						else
						{
							m_gunAudio.PlayOneShot(Aud_Reload, 1f);
							m_burstShotIndex = 0;
							m_burstRefireTick = m_burstRefireRate;
						}
					}
				}
				else
				{
					m_boltRefireTick -= Time.deltaTime;
				}
			}
			else
			{
				m_burstRefireTick -= Time.deltaTime;
			}
		}

		private void FireBullet()
		{
			m_gunAudio.PlayOneShot(Aud_GunFire[Random.Range(0, Aud_GunFire.Length)], 1f);
			SmokePSystem.Emit(1);
			GameObject gameObject = Object.Instantiate(BulletPrefab, Muzzle.position, Muzzle.rotation);
		}
	}
}
