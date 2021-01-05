using System;
using UnityEngine;

namespace FistVR
{
	public class AIMeleeWeapon : FVRDestroyableObject
	{
		[Serializable]
		public class MeleeParticleEffectEvent
		{
			public ParticleSystem System;

			public Vector2 EmitAmount;
		}

		[Header("Melee Weapon Params")]
		public Transform Spike;

		public Vector3 RetractedLocalPos;

		public Vector3 RetractedLocalScale;

		public Vector3 ExtendedLocalPos;

		public Vector3 ExtendedLocalScale;

		private Vector3 m_curLocalPos;

		private Vector3 m_tarLocalPos;

		private Vector3 m_curLocalScale;

		private Vector3 m_tarLocalScale;

		public MeleeParticleEffectEvent[] Effects;

		public DamageDealt Dam;

		public Transform DamageCastPoint;

		public LayerMask DamageLM_world;

		public LayerMask DamageLM_player;

		private RaycastHit m_hit;

		public float DamageCastDistance;

		public Vector2 RefireRange;

		private float m_refireTick;

		private bool m_IsEnabled = true;

		private bool m_FireAtWill;

		public AudioSource MeleeAudSource;

		public AudioClip[] MeleeAudClips;

		public Vector2 MeleeVolumeRange;

		public Vector2 MeleePitchRange;

		public override void Awake()
		{
			base.Awake();
			m_curLocalPos = RetractedLocalPos;
			m_tarLocalPos = RetractedLocalPos;
			m_curLocalScale = RetractedLocalScale;
			m_tarLocalScale = RetractedLocalScale;
		}

		public override void Update()
		{
			base.Update();
			FiringSystem();
			m_curLocalPos = Vector3.Lerp(m_curLocalPos, m_tarLocalPos, Time.deltaTime * 32f);
			m_curLocalScale = Vector3.Lerp(m_curLocalScale, m_tarLocalScale, Time.deltaTime * 32f);
			Spike.localPosition = m_curLocalPos;
			Spike.localScale = m_curLocalScale;
		}

		private void FiringSystem()
		{
			if (m_FireAtWill && m_IsEnabled)
			{
				if (m_refireTick > 0f)
				{
					m_refireTick -= Time.deltaTime;
					return;
				}
				Fire();
				m_refireTick = UnityEngine.Random.Range(RefireRange.x, RefireRange.y);
			}
		}

		public void SetFireAtWill(bool b)
		{
			m_FireAtWill = b;
		}

		public override void DestroyEvent()
		{
			m_IsEnabled = false;
			Invoke("KillThis", 20f);
			base.DestroyEvent();
		}

		private void KillThis()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void Fire()
		{
			MeleeAudSource.pitch = UnityEngine.Random.Range(MeleePitchRange.x, MeleePitchRange.y);
			MeleeAudSource.PlayOneShot(MeleeAudClips[UnityEngine.Random.Range(0, MeleeAudClips.Length)], UnityEngine.Random.Range(MeleeVolumeRange.x, MeleeVolumeRange.y));
			for (int i = 0; i < Effects.Length; i++)
			{
				Effects[i].System.Emit(UnityEngine.Random.Range((int)Effects[i].EmitAmount.x, (int)Effects[i].EmitAmount.y));
			}
			float num = 1f;
			if (Physics.Raycast(DamageCastPoint.position, DamageCastPoint.forward, out m_hit, DamageCastDistance, DamageLM_world, QueryTriggerInteraction.Ignore))
			{
				Damage damage = new Damage();
				damage.Class = FistVR.Damage.DamageClass.Melee;
				damage.Dam_Piercing = 700f;
				damage.Dam_TotalKinetic = 700f;
				damage.point = m_hit.point;
				damage.hitNormal = m_hit.normal;
				damage.strikeDir = base.transform.forward;
				IFVRDamageable component = m_hit.collider.transform.gameObject.GetComponent<IFVRDamageable>();
				if (component != null)
				{
					component.Damage(damage);
				}
				else if (component == null && m_hit.collider.attachedRigidbody != null)
				{
					m_hit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>()?.Damage(damage);
				}
				if (Spike != null)
				{
					num = 1f - m_hit.distance / DamageCastDistance;
					m_tarLocalPos = Vector3.Lerp(RetractedLocalPos, ExtendedLocalPos, num);
					m_tarLocalScale = Vector3.Lerp(RetractedLocalScale, ExtendedLocalScale, num);
					Invoke("Retract", 0.2f);
				}
			}
			else if (Spike != null)
			{
				m_tarLocalPos = ExtendedLocalPos;
				m_tarLocalScale = ExtendedLocalScale;
				Invoke("Retract", 0.2f);
			}
			if (Physics.Raycast(DamageCastPoint.position, DamageCastPoint.forward, out m_hit, num * DamageCastDistance, DamageLM_player, QueryTriggerInteraction.Collide))
			{
				Dam.force = DamageCastPoint.forward;
				Dam.hitNormal = m_hit.normal;
				Dam.IsInitialContact = true;
				Dam.IsInside = false;
				Dam.IsMelee = true;
				Dam.point = m_hit.point;
				Dam.ShotOrigin = null;
				Dam.PointsDamage = 700f;
				Dam.strikeDir = Dam.force;
				IFVRReceiveDamageable component2 = m_hit.collider.transform.gameObject.GetComponent<IFVRReceiveDamageable>();
				if (component2 != null)
				{
					component2.Damage(Dam);
				}
				else if (component2 == null && m_hit.collider.attachedRigidbody != null)
				{
					m_hit.collider.attachedRigidbody.gameObject.GetComponent<IFVRReceiveDamageable>()?.Damage(Dam);
				}
			}
		}

		private void Retract()
		{
			if (Spike != null)
			{
				m_tarLocalPos = RetractedLocalPos;
				m_tarLocalScale = RetractedLocalScale;
			}
		}
	}
}
