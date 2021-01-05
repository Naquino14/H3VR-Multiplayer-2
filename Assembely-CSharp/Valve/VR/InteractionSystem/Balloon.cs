using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class Balloon : MonoBehaviour
	{
		public enum BalloonColor
		{
			Red,
			OrangeRed,
			Orange,
			YellowOrange,
			Yellow,
			GreenYellow,
			Green,
			BlueGreen,
			Blue,
			VioletBlue,
			Violet,
			RedViolet,
			LightGray,
			DarkGray,
			Random
		}

		private Hand hand;

		public GameObject popPrefab;

		public float maxVelocity = 5f;

		public float lifetime = 15f;

		public bool burstOnLifetimeEnd;

		public GameObject lifetimeEndParticlePrefab;

		public SoundPlayOneshot lifetimeEndSound;

		private float destructTime;

		private float releaseTime = 99999f;

		public SoundPlayOneshot collisionSound;

		private float lastSoundTime;

		private float soundDelay = 0.2f;

		private Rigidbody balloonRigidbody;

		private bool bParticlesSpawned;

		private static float s_flLastDeathSound;

		private void Start()
		{
			destructTime = Time.time + lifetime + Random.value;
			hand = GetComponentInParent<Hand>();
			balloonRigidbody = GetComponent<Rigidbody>();
		}

		private void Update()
		{
			if (destructTime != 0f && Time.time > destructTime)
			{
				if (burstOnLifetimeEnd)
				{
					SpawnParticles(lifetimeEndParticlePrefab, lifetimeEndSound);
				}
				Object.Destroy(base.gameObject);
			}
		}

		private void SpawnParticles(GameObject particlePrefab, SoundPlayOneshot sound)
		{
			if (bParticlesSpawned)
			{
				return;
			}
			bParticlesSpawned = true;
			if (particlePrefab != null)
			{
				GameObject gameObject = Object.Instantiate(particlePrefab, base.transform.position, base.transform.rotation);
				gameObject.GetComponent<ParticleSystem>().Play();
				Object.Destroy(gameObject, 2f);
			}
			if (sound != null)
			{
				float num = Time.time - s_flLastDeathSound;
				if (num < 0.1f)
				{
					sound.volMax *= 0.25f;
					sound.volMin *= 0.25f;
				}
				sound.Play();
				s_flLastDeathSound = Time.time;
			}
		}

		private void FixedUpdate()
		{
			if (balloonRigidbody.velocity.sqrMagnitude > maxVelocity)
			{
				balloonRigidbody.velocity *= 0.97f;
			}
		}

		private void ApplyDamage()
		{
			SpawnParticles(popPrefab, null);
			Object.Destroy(base.gameObject);
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (bParticlesSpawned)
			{
				return;
			}
			Hand hand = null;
			BalloonHapticBump component = collision.gameObject.GetComponent<BalloonHapticBump>();
			if (component != null && component.physParent != null)
			{
				hand = component.physParent.GetComponentInParent<Hand>();
			}
			if (Time.time > lastSoundTime + soundDelay)
			{
				if (hand != null)
				{
					if (Time.time > releaseTime + soundDelay)
					{
						collisionSound.Play();
						lastSoundTime = Time.time;
					}
				}
				else
				{
					collisionSound.Play();
					lastSoundTime = Time.time;
				}
			}
			if (!(destructTime > 0f))
			{
				if (balloonRigidbody.velocity.magnitude > maxVelocity * 10f)
				{
					balloonRigidbody.velocity = balloonRigidbody.velocity.normalized * maxVelocity;
				}
				if (this.hand != null)
				{
					ushort microSecondsDuration = (ushort)Mathf.Clamp(Util.RemapNumber(collision.relativeVelocity.magnitude, 0f, 3f, 500f, 800f), 500f, 800f);
					this.hand.TriggerHapticPulse(microSecondsDuration);
				}
			}
		}

		public void SetColor(BalloonColor color)
		{
			GetComponentInChildren<MeshRenderer>().material.color = BalloonColorToRGB(color);
		}

		private Color BalloonColorToRGB(BalloonColor balloonColorVar)
		{
			Color result = new Color(255f, 0f, 0f);
			switch (balloonColorVar)
			{
			case BalloonColor.Red:
				return new Color(237f, 29f, 37f, 255f) / 255f;
			case BalloonColor.OrangeRed:
				return new Color(241f, 91f, 35f, 255f) / 255f;
			case BalloonColor.Orange:
				return new Color(245f, 140f, 31f, 255f) / 255f;
			case BalloonColor.YellowOrange:
				return new Color(253f, 185f, 19f, 255f) / 255f;
			case BalloonColor.Yellow:
				return new Color(254f, 243f, 0f, 255f) / 255f;
			case BalloonColor.GreenYellow:
				return new Color(172f, 209f, 54f, 255f) / 255f;
			case BalloonColor.Green:
				return new Color(0f, 167f, 79f, 255f) / 255f;
			case BalloonColor.BlueGreen:
				return new Color(108f, 202f, 189f, 255f) / 255f;
			case BalloonColor.Blue:
				return new Color(0f, 119f, 178f, 255f) / 255f;
			case BalloonColor.VioletBlue:
				return new Color(82f, 80f, 162f, 255f) / 255f;
			case BalloonColor.Violet:
				return new Color(102f, 46f, 143f, 255f) / 255f;
			case BalloonColor.RedViolet:
				return new Color(182f, 36f, 102f, 255f) / 255f;
			case BalloonColor.LightGray:
				return new Color(192f, 192f, 192f, 255f) / 255f;
			case BalloonColor.DarkGray:
				return new Color(128f, 128f, 128f, 255f) / 255f;
			case BalloonColor.Random:
			{
				int balloonColorVar2 = Random.Range(0, 12);
				return BalloonColorToRGB((BalloonColor)balloonColorVar2);
			}
			default:
				return result;
			}
		}
	}
}
