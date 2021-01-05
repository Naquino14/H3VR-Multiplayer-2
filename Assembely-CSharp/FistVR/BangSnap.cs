using UnityEngine;

namespace FistVR
{
	public class BangSnap : FVRPhysicalObject
	{
		private bool m_hasSploded;

		private AudioSource m_aud;

		public AudioClip Audclip_Splode;

		public AudioClip Audclip_Fizzle;

		public BangSnapFlameTrigger FlameTrigger;

		public Renderer Unsploded;

		public Renderer Sploded;

		public GameObject SplodePrefab;

		public GameObject FizzlePrefab;

		public GameObject SecondarySplodePrefab;

		public float HitThreshold = 4f;

		protected override void Awake()
		{
			base.Awake();
			m_aud = GetComponent<AudioSource>();
		}

		public override void OnCollisionEnter(Collision col)
		{
			if (!(col.relativeVelocity.magnitude > HitThreshold) || m_hasSploded)
			{
				return;
			}
			m_hasSploded = true;
			Splode();
			if (col.collider.attachedRigidbody != null)
			{
				IFVRDamageable component = col.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
				if (component != null)
				{
					Damage damage = new Damage();
					damage.Dam_Thermal = 50f;
					damage.Dam_TotalEnergetic = 50f;
					damage.point = col.contacts[0].point;
					damage.hitNormal = col.contacts[0].normal;
					damage.strikeDir = base.transform.forward;
					component.Damage(damage);
				}
			}
		}

		private void Splode()
		{
			m_aud.pitch = Random.Range(0.85f, 1.05f);
			m_aud.PlayOneShot(Audclip_Splode, Random.Range(0.9f, 1f));
			Object.Destroy(FlameTrigger.gameObject);
			Unsploded.enabled = false;
			Sploded.enabled = true;
			GameObject gameObject = Object.Instantiate(SplodePrefab, base.transform.position, Quaternion.identity);
			if (SecondarySplodePrefab != null)
			{
				Object.Instantiate(SecondarySplodePrefab, base.transform.position, Quaternion.identity);
			}
			Invoke("KillMe", 5f);
		}

		public void Fizzle()
		{
			if (!m_hasSploded)
			{
				m_hasSploded = true;
				m_aud.PlayOneShot(Audclip_Fizzle, 1f);
				Unsploded.enabled = false;
				Sploded.enabled = true;
				GameObject gameObject = Object.Instantiate(FizzlePrefab, base.transform.position, base.transform.rotation);
				gameObject.transform.SetParent(base.transform);
				Invoke("KillMe", 5f);
			}
		}

		private void KillMe()
		{
			Object.Destroy(base.gameObject);
		}
	}
}
