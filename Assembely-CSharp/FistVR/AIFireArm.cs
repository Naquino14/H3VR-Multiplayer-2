using UnityEngine;

namespace FistVR
{
	public class AIFireArm : FVRDestroyableObject
	{
		[Header("AIFireArm Config")]
		public Transform Muzzle;

		public ParticleSystem PSystem_Smoke;

		public int SmokeAmount = 1;

		public float TrajectoryMuzzleVelocity;

		public float TrajectoryGravityMultiplier = 1f;

		public float ProjectileSpread;

		public int BurstLengthMin = 2;

		public int BurstLengthMax = 2;

		private int m_curBurstLength = 2;

		private int m_burstShotIndex;

		public float BoltMinRefireRate = 0.35f;

		public float BoltMaxRefireRate = 0.42f;

		private float m_boltRefireTick;

		public float BurstMinRefireRate = 1.2f;

		public float BurstMaxRefireRate = 1.6f;

		private float m_burstRefireTick;

		public GameObject ProjectilePrefab;

		private bool shouldFire;

		private AudioSource m_gunAudio;

		public AudioClip[] Aud_GunFire;

		public AudioClip Aud_Reload;

		public float FiringAngleThreshold = 5f;

		private new void Awake()
		{
			base.Awake();
			m_gunAudio = GetComponent<AudioSource>();
			m_curBurstLength = BurstLengthMin;
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
						if (m_burstShotIndex < m_curBurstLength)
						{
							m_boltRefireTick = Random.Range(BoltMinRefireRate, BoltMaxRefireRate);
							m_burstShotIndex++;
							FireBullet();
						}
						else
						{
							m_gunAudio.PlayOneShot(Aud_Reload, 1f);
							m_burstShotIndex = 0;
							m_curBurstLength = Random.Range(BurstLengthMin, BurstLengthMax + 1);
							m_burstRefireTick = Random.Range(BurstMinRefireRate, BurstMaxRefireRate);
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
			PSystem_Smoke.Emit(SmokeAmount);
			GameObject gameObject = Object.Instantiate(ProjectilePrefab, Muzzle.position, Muzzle.rotation);
			gameObject.transform.Rotate(new Vector3(Random.Range(0f - ProjectileSpread, ProjectileSpread), Random.Range(0f - ProjectileSpread, ProjectileSpread), 0f));
			BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
			component.Fire(gameObject.transform.forward, null);
		}
	}
}
