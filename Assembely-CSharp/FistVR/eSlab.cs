using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class eSlab : FVRPhysicalObject
	{
		[Header("eSlab Stuff")]
		public wwParkManager ParkManager;

		public GameObject ProxyDisc;

		private bool m_isDiscLoaded;

		private bool m_wasPlaying;

		public Image[] TargetSprites;

		public AudioSource Aud;

		private FVRObject m_discObject;

		public Transform DiscEjectPoint;

		public float insertCooldown;

		protected override void Awake()
		{
			base.Awake();
			ProxyDisc.SetActive(value: false);
		}

		public bool LoadDisc(eSlabDisc disc)
		{
			if (m_isDiscLoaded)
			{
				return false;
			}
			m_isDiscLoaded = true;
			ProxyDisc.SetActive(value: true);
			Aud.clip = disc.Clip;
			m_discObject = disc.ObjectWrapper;
			PlayDisc();
			return true;
		}

		private void PlayDisc()
		{
			ParkManager.PASystem.EngageSuppressedMode();
			Aud.Play();
		}

		private void StopDisc()
		{
			if (Aud.isPlaying)
			{
				Aud.Stop();
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (insertCooldown > 0f)
			{
				insertCooldown -= Time.deltaTime;
			}
			if (!Aud.isPlaying && m_wasPlaying)
			{
				ParkManager.PASystem.DisEngageSuppressedMode();
			}
			m_wasPlaying = Aud.isPlaying;
		}

		private void EjectDisc()
		{
			if (m_isDiscLoaded)
			{
				StopDisc();
				m_isDiscLoaded = false;
				insertCooldown = 1f;
				ProxyDisc.SetActive(value: false);
				GameObject gameObject = Object.Instantiate(m_discObject.GetGameObject(), DiscEjectPoint.position, DiscEjectPoint.rotation);
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				component.velocity = -DiscEjectPoint.right * 3f;
				m_discObject = null;
				Aud.clip = null;
			}
		}

		public void UpdateSprites(Sprite sa, Sprite sb, Sprite sc, Sprite sd)
		{
			if (sa != null)
			{
				TargetSprites[0].sprite = sa;
				TargetSprites[0].color = Color.white;
			}
			else
			{
				TargetSprites[0].sprite = null;
				TargetSprites[0].color = Color.clear;
			}
			if (sb != null)
			{
				TargetSprites[1].sprite = sb;
				TargetSprites[1].color = Color.white;
			}
			else
			{
				TargetSprites[1].sprite = null;
				TargetSprites[1].color = Color.clear;
			}
			if (sc != null)
			{
				TargetSprites[2].sprite = sc;
				TargetSprites[2].color = Color.white;
			}
			else
			{
				TargetSprites[2].sprite = null;
				TargetSprites[2].color = Color.clear;
			}
			if (sd != null)
			{
				TargetSprites[3].sprite = sd;
				TargetSprites[3].color = Color.white;
			}
			else
			{
				TargetSprites[3].sprite = null;
				TargetSprites[3].color = Color.clear;
			}
		}

		private new void OnCollisionEnter(Collision col)
		{
			if (col.relativeVelocity.magnitude > 2f)
			{
				EjectDisc();
			}
		}
	}
}
