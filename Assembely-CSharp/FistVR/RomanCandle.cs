using UnityEngine;

namespace FistVR
{
	public class RomanCandle : FVRPhysicalObject
	{
		public GameObject PEffects;

		public ParticleSystem[] PSystems;

		public Transform Muzzle;

		private bool m_isBurning;

		private bool m_isDone;

		public GameObject[] ChargePrefabs;

		public float MinDelay = 1f;

		public float MaxDelay = 1f;

		private float m_TimeTilFire = 1f;

		private int m_chargeIndex;

		private AudioSource m_aud;

		public GameObject Fuse;

		public GameObject BurningSound;

		protected override void Awake()
		{
			base.Awake();
			m_aud = GetComponent<AudioSource>();
			base.RootRigidbody.maxAngularVelocity = 20f;
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (m_isBurning && !m_isDone)
			{
				m_TimeTilFire -= Time.deltaTime;
				if (m_TimeTilFire <= 0f)
				{
					m_TimeTilFire = Random.Range(MinDelay, MaxDelay);
					FireNextCharge();
				}
			}
		}

		private void FireNextCharge()
		{
			if (m_chargeIndex < ChargePrefabs.Length)
			{
				m_aud.pitch = Random.Range(1f, 1.25f);
				m_aud.PlayOneShot(m_aud.clip, Random.Range(0.15f, 0.2f));
				GameObject gameObject = Object.Instantiate(ChargePrefabs[m_chargeIndex], Muzzle.position, Quaternion.Slerp(Muzzle.rotation, Random.rotation, 0.1f));
				gameObject.GetComponent<RomanCandleCharge>().Fire();
				if (base.IsHeld)
				{
					m_hand.Buzz(m_hand.Buzzer.Buzz_GunShot);
				}
			}
			else
			{
				Extinguish();
			}
			m_chargeIndex++;
		}

		public void Ignite()
		{
			BurningSound.SetActive(value: true);
			if (!m_isBurning && !m_isDone)
			{
				m_isBurning = true;
				PEffects.SetActive(value: true);
				m_TimeTilFire = Random.Range(MinDelay, MaxDelay);
			}
			Object.Destroy(Fuse);
		}

		private void Extinguish()
		{
			BurningSound.SetActive(value: false);
			if (m_isBurning)
			{
				m_isBurning = false;
				m_isDone = true;
				DisableAllPSystemEmission();
			}
		}

		private void DisableAllPSystemEmission()
		{
			for (int i = 0; i < PSystems.Length; i++)
			{
				ParticleSystem.EmissionModule emission = PSystems[i].emission;
				ParticleSystem.MinMaxCurve rate = emission.rate;
				rate.mode = ParticleSystemCurveMode.Constant;
				rate.constantMax = 0f;
				rate.constantMin = 0f;
				emission.rate = rate;
			}
		}
	}
}
