using UnityEngine;

namespace FistVR
{
	public class wwTarget : MonoBehaviour, IFVRDamageable
	{
		public string Ident;

		public Sprite TargetSprite;

		[Header("Hit Details")]
		public float TargetStruckRefire = 1f;

		private float m_targetStruckRefire = 1f;

		[Header("Sound Settings")]
		public float SoundRefire = 1f;

		private float m_soundRefireTick = 1f;

		public bool HasAudioHitSound = true;

		public AudioEvent AudioEvent;

		public FVRPooledAudioType AudioType;

		public wwTargetManager Manager;

		protected Vector3 m_originalPos;

		protected Quaternion m_originalRot;

		protected float m_originalScale;

		public float RespawnTime = 10f;

		public bool DoesRescale;

		protected bool hasManager;

		public void Awake()
		{
			m_targetStruckRefire = TargetStruckRefire;
			m_soundRefireTick = SoundRefire;
		}

		public void Start()
		{
			m_originalPos = base.transform.position;
			m_originalRot = base.transform.rotation;
			m_originalScale = base.transform.localScale.y;
		}

		public void SetManager(wwTargetManager m)
		{
			Manager = m;
			hasManager = true;
		}

		public void SetupAfterSpawn(wwTargetManager m, Vector3 pos, Quaternion rot, float scale, bool doesScale)
		{
			hasManager = true;
			Manager = m;
			m_originalPos = pos;
			m_originalRot = rot;
			m_originalScale = scale;
			DoesRescale = doesScale;
		}

		public virtual void Damage(Damage d)
		{
			if (d.Class == FistVR.Damage.DamageClass.Projectile)
			{
				TargetStruck(d, sendStruckEvent: true);
			}
			else
			{
				TargetStruck(d, sendStruckEvent: false);
			}
		}

		public virtual void Update()
		{
			if (m_targetStruckRefire > 0f)
			{
				m_targetStruckRefire -= Time.deltaTime;
			}
			if (m_soundRefireTick > 0f)
			{
				m_soundRefireTick -= Time.deltaTime;
			}
		}

		public virtual void TargetStruck(Damage dam, bool sendStruckEvent)
		{
			if (m_soundRefireTick <= 0f && PlaySoundEvent())
			{
				m_soundRefireTick = SoundRefire;
			}
			if (sendStruckEvent && m_targetStruckRefire <= 0f)
			{
				m_targetStruckRefire = TargetStruckRefire;
				SendStruckEvent();
			}
		}

		public virtual bool PlaySoundEvent()
		{
			if (HasAudioHitSound)
			{
				float num = Vector3.Distance(GM.CurrentPlayerRoot.position, base.transform.position);
				float delay = num / 343f;
				SM.PlayCoreSoundDelayed(AudioType, AudioEvent, base.transform.position, delay);
				return true;
			}
			return false;
		}

		public void SendStruckEvent()
		{
			if (hasManager)
			{
				Manager.StruckEvent(this);
			}
		}
	}
}
