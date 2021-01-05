using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class FireSource : MonoBehaviour
	{
		public GameObject fireParticlePrefab;

		public bool startActive;

		private GameObject fireObject;

		public ParticleSystem customParticles;

		public bool isBurning;

		public float burnTime;

		public float ignitionDelay;

		private float ignitionTime;

		private Hand hand;

		public AudioSource ignitionSound;

		public bool canSpreadFromThisSource = true;

		private void Start()
		{
			if (startActive)
			{
				StartBurning();
			}
		}

		private void Update()
		{
			if (burnTime != 0f && Time.time > ignitionTime + burnTime && isBurning)
			{
				isBurning = false;
				if (customParticles != null)
				{
					customParticles.Stop();
				}
				else
				{
					Object.Destroy(fireObject);
				}
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (isBurning && canSpreadFromThisSource)
			{
				other.SendMessageUpwards("FireExposure", SendMessageOptions.DontRequireReceiver);
			}
		}

		private void FireExposure()
		{
			if (fireObject == null)
			{
				Invoke("StartBurning", ignitionDelay);
			}
			if ((bool)(hand = GetComponentInParent<Hand>()))
			{
				hand.TriggerHapticPulse(1000);
			}
		}

		private void StartBurning()
		{
			isBurning = true;
			ignitionTime = Time.time;
			if (ignitionSound != null)
			{
				ignitionSound.Play();
			}
			if (customParticles != null)
			{
				customParticles.Play();
			}
			else if (fireParticlePrefab != null)
			{
				fireObject = Object.Instantiate(fireParticlePrefab, base.transform.position, base.transform.rotation);
				fireObject.transform.parent = base.transform;
			}
		}
	}
}
