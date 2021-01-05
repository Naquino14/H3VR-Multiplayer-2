using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class Kabot : MonoBehaviour, IFVRDamageable
	{
		public enum KabotState
		{
			Lingering,
			Locomoting
		}

		[Serializable]
		public class KSpike
		{
			public enum SpikeState
			{
				Retracted,
				Retracting,
				Anchoring,
				Anchored
			}

			public Kabot K;

			public Transform T;

			public Collider C;

			public SpikeState State;

			private float m_lerp;

			private bool m_isGoingToAnchor;

			private RaycastHit m_hit;

			private float m_anchorDistance = 1f;

			private Vector3 m_anchorPoint;

			private float m_fireSpeed = 1f;

			public int Index;

			public bool CanFireOut = true;

			public Vector3 GetAnchorPoint()
			{
				return m_anchorPoint;
			}

			public void SetAnchorDistance(float f)
			{
				m_anchorDistance = f;
			}

			public void FireOut(Vector3 forward)
			{
				if (!CanFireOut)
				{
					return;
				}
				C.enabled = false;
				T.rotation = Quaternion.LookRotation(forward, Vector3.up);
				State = SpikeState.Anchoring;
				m_lerp = 0f;
				m_fireSpeed = UnityEngine.Random.Range(K.SpikeSpeedFire * 0.9f, K.SpikeSpeedFire * 1f);
				if (Physics.Raycast(T.position + T.forward * 0.32f, T.forward, out m_hit, K.MaxSpikeRange - 0.32f, K.LM_Spike, QueryTriggerInteraction.Ignore))
				{
					m_anchorPoint = m_hit.point + T.forward * 0.15f;
					m_anchorDistance = m_hit.distance + 0.15f;
					if (m_hit.distance > 0.7f)
					{
						m_isGoingToAnchor = true;
					}
					else
					{
						m_isGoingToAnchor = false;
					}
				}
				else
				{
					m_anchorPoint = T.position + T.forward * K.MaxSpikeRange;
					m_anchorDistance = K.MaxSpikeRange;
					m_isGoingToAnchor = false;
				}
			}

			public void Retract()
			{
				C.enabled = false;
				State = SpikeState.Retracting;
			}

			public void Tick(float t)
			{
				switch (State)
				{
				case SpikeState.Anchoring:
					m_lerp += t * m_fireSpeed;
					T.localScale = new Vector3(K.ScaleXY, K.ScaleXY, Mathf.Lerp(K.ScaleXY, m_anchorDistance, m_lerp));
					if (!(m_lerp >= 1f))
					{
						break;
					}
					if (Physics.Raycast(T.position + T.forward * 0.32f, T.forward, out m_hit, m_anchorDistance - 0.32f, K.LM_DamageCast, QueryTriggerInteraction.Collide))
					{
						IFVRDamageable iFVRDamageable = null;
						iFVRDamageable = m_hit.collider.transform.gameObject.GetComponent<IFVRDamageable>();
						if (iFVRDamageable == null && m_hit.collider.attachedRigidbody != null)
						{
							iFVRDamageable = m_hit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
						}
						if (iFVRDamageable != null)
						{
							Damage damage = new Damage();
							damage.Class = FistVR.Damage.DamageClass.Melee;
							damage.Source_IFF = 0;
							damage.strikeDir = T.forward;
							damage.Dam_Piercing = 1000f;
							damage.Dam_TotalKinetic = 1000f;
							damage.hitNormal = -T.forward;
							damage.point = m_hit.point;
							iFVRDamageable.Damage(damage);
						}
					}
					if (m_isGoingToAnchor)
					{
						FXM.SpawnImpactEffect(m_anchorPoint, -T.forward, (int)K.MatDefImpact.impactCategory, ImpactEffectMagnitude.Large, forwardBack: false);
						State = SpikeState.Anchored;
						K.PlayHitSound(Index, m_anchorPoint);
						C.enabled = true;
					}
					else
					{
						State = SpikeState.Retracting;
					}
					break;
				case SpikeState.Retracting:
					m_lerp -= t * K.SpikeSpeedRetract;
					T.localScale = new Vector3(K.ScaleXY, K.ScaleXY, Mathf.Lerp(K.ScaleXY, m_anchorDistance, m_lerp));
					if (m_lerp <= 0f)
					{
						State = SpikeState.Retracted;
					}
					break;
				}
			}
		}

		[Header("Refs")]
		public Rigidbody RB;

		public Transform Center;

		public List<KSpike> Spikes;

		public KabotState State;

		[Header("GeneralParams")]
		public float Life = 30000f;

		[Header("Animation")]
		public AnimationCurve PulsingCurve;

		public Vector2 PulseScaleRange;

		private float m_pulseXTick;

		private float m_pulseYTick = 0.3333f;

		private float m_pulseZTick = 0.6666f;

		[Header("MovementLogic")]
		public Vector2 LingerRange;

		public float MoveLerpMult = 1f;

		[Header("SpikeLogic")]
		public float ScaleXY = 0.2f;

		public float MaxSpikeRange = 10f;

		public float SpikeSpeedFire = 3f;

		public float SpikeSpeedRetract = 1f;

		public LayerMask LM_Spike;

		public LayerMask LM_DamageCast;

		public AudioEvent AudEvent_SpikeHit;

		public PMaterialDefinition MatDefImpact;

		public DamageDealt DamageOnHit;

		public GameObject SpawnOnDie;

		public AudioSource Pulse;

		private bool m_isPulseActive;

		private float m_distCheckTick = 0.25f;

		private float m_timeToLinger;

		private float m_moveLerp;

		private Vector3 m_fromPos;

		private Vector3 m_toPos;

		private bool m_isDead;

		private void Start()
		{
			InitiateLingering();
			float num = UnityEngine.Random.Range(0f, 1f);
			m_pulseXTick += num;
			m_pulseYTick += num;
			m_pulseZTick += num;
			base.transform.rotation = Quaternion.LookRotation(UnityEngine.Random.onUnitSphere, Vector3.up);
			for (int i = 0; i < Spikes.Count; i++)
			{
				Spikes[i].Index = i;
			}
			m_distCheckTick = UnityEngine.Random.Range(0.1f, 0.25f);
		}

		private void Update()
		{
			if (m_isDead)
			{
				bool flag = true;
				for (int i = 0; i < Spikes.Count; i++)
				{
					if (Spikes[i].State != 0)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					Explode();
				}
			}
			else
			{
				m_distCheckTick -= Time.deltaTime;
				if (m_distCheckTick <= 0f)
				{
					m_distCheckTick = UnityEngine.Random.Range(0.1f, 0.25f);
					SoundCheck();
				}
			}
			KUpdate();
			m_pulseXTick += Time.deltaTime;
			m_pulseYTick += Time.deltaTime;
			m_pulseZTick += Time.deltaTime;
			m_pulseXTick = Mathf.Repeat(m_pulseXTick, 1f);
			m_pulseYTick = Mathf.Repeat(m_pulseYTick, 1f);
			m_pulseZTick = Mathf.Repeat(m_pulseZTick, 1f);
			Center.localScale = new Vector3(Mathf.Lerp(PulseScaleRange.x, PulseScaleRange.y, PulsingCurve.Evaluate(m_pulseXTick)), Mathf.Lerp(PulseScaleRange.x, PulseScaleRange.y, PulsingCurve.Evaluate(m_pulseYTick)), Mathf.Lerp(PulseScaleRange.x, PulseScaleRange.y, PulsingCurve.Evaluate(m_pulseZTick)));
		}

		private void SoundCheck()
		{
			float num = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.transform.position);
			if (num < 6f && !m_isPulseActive)
			{
				m_isPulseActive = true;
				Pulse.Play();
			}
			else if (num > 8f && m_isPulseActive)
			{
				m_isPulseActive = false;
				Pulse.Stop();
			}
		}

		private void KUpdate()
		{
			switch (State)
			{
			case KabotState.Lingering:
				KUpdate_Lingering();
				break;
			case KabotState.Locomoting:
				KUpdate_Locomoting();
				break;
			}
			float deltaTime = Time.deltaTime;
			for (int i = 0; i < Spikes.Count; i++)
			{
				Spikes[i].Tick(deltaTime);
			}
		}

		private void InitiateLingering()
		{
			if (!Spikes[0].C.enabled)
			{
				Spikes[0].C.enabled = true;
			}
			State = KabotState.Lingering;
			m_timeToLinger = UnityEngine.Random.Range(LingerRange.x, LingerRange.y);
			for (int i = 1; i <= 7; i++)
			{
				Spikes[i].FireOut(UnityEngine.Random.onUnitSphere);
			}
			float num = Vector3.Distance(GM.CurrentPlayerRoot.position, base.transform.position);
			float delay = num / 343f;
			if (num < 40f)
			{
				SM.PlayCoreSoundDelayed(FVRPooledAudioType.Generic, AudEvent_SpikeHit, base.transform.position, delay);
			}
		}

		private void InitiateLocomoting()
		{
			State = KabotState.Locomoting;
			m_moveLerp = 0f;
			m_fromPos = base.transform.position;
			m_toPos = Vector3.Lerp(base.transform.position, Spikes[0].GetAnchorPoint(), UnityEngine.Random.Range(0.3f, 0.8f));
			for (int i = 1; i < Spikes.Count; i++)
			{
				Spikes[i].Retract();
			}
		}

		private void KUpdate_Lingering()
		{
			m_timeToLinger -= Time.deltaTime;
			if (!(m_timeToLinger > 0f))
			{
				if (Spikes[0].State == KSpike.SpikeState.Anchored)
				{
					InitiateLocomoting();
				}
				else if (Spikes[0].State == KSpike.SpikeState.Retracted)
				{
					Spikes[0].FireOut(UnityEngine.Random.onUnitSphere);
				}
			}
		}

		private void KUpdate_Locomoting()
		{
			bool flag = true;
			for (int i = 1; i < Spikes.Count; i++)
			{
				if (Spikes[i].State != 0)
				{
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
			if (Spikes[0].C.enabled)
			{
				Spikes[0].C.enabled = false;
			}
			if (m_moveLerp < 1f)
			{
				m_moveLerp += Time.deltaTime;
				Vector3 position = base.transform.position;
				Vector3 vector = Vector3.Lerp(m_fromPos, m_toPos, m_moveLerp);
				Vector3 vector2 = vector - position;
				if (Physics.SphereCast(new Ray(position, vector2.normalized), 0.36f, vector2.magnitude, LM_Spike, QueryTriggerInteraction.Ignore))
				{
					m_moveLerp = 1f;
					return;
				}
				RB.MovePosition(vector);
				float num = Vector3.Distance(base.transform.position, Spikes[0].GetAnchorPoint());
				Spikes[0].T.localScale = new Vector3(ScaleXY, ScaleXY, num);
				Spikes[0].SetAnchorDistance(num);
			}
			else if (Spikes[0].State == KSpike.SpikeState.Anchored)
			{
				Spikes[0].Retract();
				InitiateLingering();
			}
			else if (Spikes[0].State != 0)
			{
			}
		}

		public void PlayHitSound(int i, Vector3 p)
		{
			if (i == 0 || i == 8 || i == 9)
			{
				float num = Vector3.Distance(GM.CurrentPlayerRoot.position, base.transform.position);
				float delay = num / 343f;
				if (num < 40f)
				{
					SM.PlayCoreSoundDelayed(FVRPooledAudioType.Generic, AudEvent_SpikeHit, p, delay);
				}
			}
		}

		public void Damage(Damage d)
		{
			Life -= d.Dam_TotalKinetic;
			if (Life <= 0f)
			{
				Die();
			}
			else
			{
				ResponseDir(-d.strikeDir);
			}
		}

		private void Die()
		{
			if (!m_isDead)
			{
				m_isDead = true;
				for (int i = 0; i < Spikes.Count; i++)
				{
					Spikes[i].CanFireOut = false;
					Spikes[i].Retract();
				}
				SpikeSpeedRetract = 10f;
			}
		}

		private void Explode()
		{
			UnityEngine.Object.Instantiate(SpawnOnDie, base.transform.position, UnityEngine.Random.rotation);
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void ResponseDir(Vector3 v)
		{
			if (Spikes[8].State == KSpike.SpikeState.Retracted)
			{
				Spikes[8].FireOut(Vector3.Lerp(v, UnityEngine.Random.onUnitSphere, UnityEngine.Random.Range(0f, 0.2f)));
			}
			else if (Spikes[8].State == KSpike.SpikeState.Anchored)
			{
				Spikes[8].Retract();
			}
			if (Spikes[9].State == KSpike.SpikeState.Retracted)
			{
				Spikes[9].FireOut(Vector3.Lerp(v, UnityEngine.Random.onUnitSphere, UnityEngine.Random.Range(0f, 0.2f)));
			}
			else if (Spikes[9].State == KSpike.SpikeState.Anchored)
			{
				Spikes[9].Retract();
			}
		}
	}
}
