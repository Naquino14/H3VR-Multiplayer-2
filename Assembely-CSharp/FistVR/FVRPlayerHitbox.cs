using UnityEngine;

namespace FistVR
{
	public class FVRPlayerHitbox : MonoBehaviour, IFVRDamageable
	{
		public enum PlayerHitBoxType
		{
			Head,
			Torso,
			Hand
		}

		public bool IsActivated;

		private AudioSource m_aud;

		private Rigidbody m_rb;

		public AudioClip AudClip_Reset;

		public AudioClip AudClip_Hit;

		public FVRPlayerBody Body;

		public FVRViveHand Hand;

		public float DamageResist;

		public float DamageMultiplier = 1f;

		public AIEntity MyE;

		public Transform TransformTarget;

		public PlayerHitBoxType Type;

		private void Awake()
		{
			m_aud = GetComponent<AudioSource>();
			m_rb = GetComponent<Rigidbody>();
		}

		public void UpdatePositions()
		{
			if (TransformTarget != null)
			{
				m_rb.MovePosition(TransformTarget.position);
				m_rb.MoveRotation(TransformTarget.rotation);
			}
		}

		public virtual void Damage(float i)
		{
			if (!GM.CurrentSceneSettings.DoesDamageGetRegistered)
			{
				return;
			}
			float num = Mathf.Clamp(i * DamageMultiplier - DamageResist, 0f, 10000f);
			if (num > 0.1f && IsActivated)
			{
				if (Body.RegisterPlayerHit(num, FromSelf: false))
				{
					m_aud.PlayOneShot(AudClip_Reset, 1f);
				}
				else if (!GM.IsDead())
				{
					m_aud.PlayOneShot(AudClip_Hit, 1f);
				}
			}
		}

		public void Damage(Damage d)
		{
			if (!GM.CurrentSceneSettings.DoesDamageGetRegistered)
			{
				return;
			}
			if (d.Dam_Blinding > 0f && Type == PlayerHitBoxType.Head)
			{
				float num = Vector3.Angle(d.strikeDir, GM.CurrentPlayerBody.Head.forward);
				if (num > 90f)
				{
					GM.CurrentPlayerBody.BlindPlayer(d.Dam_Blinding);
				}
			}
			if (GM.CurrentPlayerBody.IsBlort)
			{
				d.Dam_TotalEnergetic = 0f;
			}
			else if (GM.CurrentPlayerBody.IsDlort)
			{
				d.Dam_TotalEnergetic *= 3f;
			}
			float num2 = d.Dam_TotalKinetic + d.Dam_TotalEnergetic * 1f;
			if (GM.CurrentPlayerBody.IsDamResist || GM.CurrentPlayerBody.IsDamMult)
			{
				float damageResist = GM.CurrentPlayerBody.GetDamageResist();
				if (damageResist <= 0.01f)
				{
					return;
				}
				num2 *= damageResist;
			}
			if (num2 > 0.1f && IsActivated)
			{
				if (Body.RegisterPlayerHit(num2, FromSelf: false))
				{
					m_aud.PlayOneShot(AudClip_Reset, 0.4f);
				}
				else if (!GM.IsDead())
				{
					m_aud.PlayOneShot(AudClip_Hit, 0.4f);
				}
			}
		}

		public virtual void Damage(DamageDealt dam)
		{
			if (!GM.CurrentSceneSettings.DoesDamageGetRegistered || (dam.SourceFirearm != null && Type == PlayerHitBoxType.Hand))
			{
				return;
			}
			float num = Mathf.Clamp(dam.PointsDamage * DamageMultiplier - DamageResist, 0f, 10000f);
			if (dam.IsInitialContact && num > 0.1f && IsActivated)
			{
				if (Body.RegisterPlayerHit(num, dam.IsPlayer))
				{
					m_aud.PlayOneShot(AudClip_Reset, 0.4f);
				}
				else if (!GM.IsDead())
				{
					m_aud.PlayOneShot(AudClip_Hit, 0.4f);
				}
			}
		}
	}
}
