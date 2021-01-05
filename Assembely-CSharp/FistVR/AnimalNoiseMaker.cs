using UnityEngine;

namespace FistVR
{
	public class AnimalNoiseMaker : FVRPhysicalObject
	{
		private AudioSource m_aud;

		private float soundChargedUp;

		private float m_curVolume;

		private float m_tarVolume;

		public bool UsesMultipleClips;

		public AudioClip[] clips;

		private int m_numClip;

		private bool m_hasSoundPlayed;

		public float SoundDrainSpeed = 1f;

		public string BangerText;

		public GameObject BangerSplosion;

		[Header("SpawnOnHit")]
		public bool SpawnOnHit;

		public FVRObject SpawnOnHitObject;

		private bool m_isPrimed;

		private bool m_hasSpawned;

		public GameObject SpawnCloud;

		protected override void Awake()
		{
			base.Awake();
			m_aud = GetComponent<AudioSource>();
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (base.QuickbeltSlot != null)
			{
				m_isPrimed = false;
			}
			m_curVolume = Mathf.Lerp(m_curVolume, m_tarVolume, Time.deltaTime * 3f);
			m_aud.volume = m_curVolume;
			if (Vector3.Dot(base.transform.up, Vector3.up) < -0.1f)
			{
				soundChargedUp += Time.deltaTime * 2f;
				soundChargedUp = Mathf.Clamp(soundChargedUp, 0f, 1.7f);
				m_tarVolume = 0f;
				m_hasSoundPlayed = false;
			}
			else
			{
				if (!(Vector3.Dot(base.transform.up, Vector3.up) > 0.3f))
				{
					return;
				}
				if (soundChargedUp > 0f)
				{
					soundChargedUp -= Time.deltaTime * SoundDrainSpeed;
					if (!m_aud.isPlaying && !m_hasSoundPlayed)
					{
						m_hasSoundPlayed = true;
						if (UsesMultipleClips)
						{
							m_numClip++;
							if (m_numClip >= clips.Length)
							{
								m_numClip = 0;
							}
							m_aud.clip = clips[m_numClip];
						}
						m_isPrimed = true;
						m_aud.Play();
					}
					m_tarVolume = 1f;
				}
				else
				{
					m_tarVolume = 0f;
				}
				m_aud.pitch = Mathf.Clamp(Vector3.Dot(base.transform.up, Vector3.down) * -2f, 0.9f, 1f);
			}
		}

		public override void OnCollisionEnter(Collision col)
		{
			base.OnCollisionEnter(col);
			if (SpawnOnHit && m_isPrimed && !m_hasSpawned && !(col.collider.attachedRigidbody != null))
			{
				Vector3 normal = col.contacts[0].normal;
				if (!(Vector3.Angle(normal, Vector3.up) > 15f))
				{
					m_hasSpawned = true;
					Vector3 point = col.contacts[0].point;
					Vector3 forward = GM.CurrentPlayerBody.Head.forward;
					forward.y = 0f;
					forward.Normalize();
					Object.Instantiate(SpawnOnHitObject.GetGameObject(), point, Quaternion.LookRotation(forward, Vector3.up));
					Object.Instantiate(SpawnCloud, point, Quaternion.LookRotation(forward, Vector3.up));
					Object.Destroy(base.gameObject);
				}
			}
		}
	}
}
