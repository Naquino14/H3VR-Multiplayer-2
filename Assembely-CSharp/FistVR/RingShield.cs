using UnityEngine;

namespace FistVR
{
	public class RingShield : MonoBehaviour
	{
		public Renderer ShieldGeo;

		private float weight;

		private bool isOn;

		public Cubegame Game;

		private AudioSource aud;

		private void Awake()
		{
			aud = GetComponent<AudioSource>();
		}

		private void OnCollisionEnter(Collision col)
		{
			if (col.collider.gameObject.tag != "Harmless")
			{
				if (!isOn)
				{
					isOn = true;
					ShieldGeo.gameObject.SetActive(value: true);
				}
				weight = 1f;
				Game.DamagePlayer();
				aud.PlayOneShot(aud.clip);
			}
		}

		private void Update()
		{
			ShieldGeo.material.SetFloat("_TintWeight", weight);
			if (weight > 0.01f)
			{
				weight = Mathf.Lerp(weight, 0f, Time.deltaTime * 6f);
				return;
			}
			weight = 0f;
			isOn = false;
			ShieldGeo.gameObject.SetActive(value: false);
		}
	}
}
