using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class Chainsaw : FVRPhysicalObject
	{
		[Header("ChainsawParms")]
		public FVRAlternateGrip Foregrip;

		public AudioSource SawAudio;

		public AudioSource StartingAudio;

		public AudioClip AudClip_Start;

		public AudioClip AudClip_Idle;

		public AudioClip AudClip_Buzzing;

		public AudioClip AudClip_Hitting;

		private bool m_isRunning;

		private float m_currentCableLength;

		private float m_lastCableLength;

		private float m_motorSpeed;

		private float triggerAmount;

		private float m_sawingIntesity;

		public bool UsesBladeSolidBits = true;

		public Renderer BladeSolid;

		public Renderer BladeBits;

		private Material m_matBladeSolid;

		private Material m_matBladeBits;

		public Collider[] BladeCols;

		private HashSet<Collider> m_bladeCols = new HashSet<Collider>();

		public ParticleSystem Sparks;

		private ParticleSystem.EmitParams emitParams;

		public Transform BladePoint1;

		public Transform BladePoint2;

		private List<IFVRDamageable> DamageablesToDo;

		private HashSet<IFVRDamageable> DamageablesToDoHS;

		private List<Vector3> DamageableHitPoints;

		private List<Vector3> DamageableHitNormals;

		private float TimeSinceDamageDealing = 0.2f;

		public ParticleSystem EngineSmoke;

		public bool UsesEngineRot = true;

		public Transform EngineRot;

		public float PerceptibleEventVolume = 50f;

		public float PerceptibleEventRange = 30f;

		private float m_timeTilPerceptibleEventTick = 0.2f;

		private float timeSinceCollision = 1f;

		private int framesTilFlash;

		protected override void Awake()
		{
			base.Awake();
			emitParams = default(ParticleSystem.EmitParams);
			if (UsesBladeSolidBits)
			{
				m_matBladeSolid = BladeSolid.materials[0];
				m_matBladeBits = BladeBits.material;
			}
			for (int i = 0; i < BladeCols.Length; i++)
			{
				m_bladeCols.Add(BladeCols[i]);
			}
			DamageablesToDo = new List<IFVRDamageable>();
			DamageablesToDoHS = new HashSet<IFVRDamageable>();
			DamageableHitPoints = new List<Vector3>();
			DamageableHitNormals = new List<Vector3>();
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (m_hasTriggeredUpSinceBegin && !IsAltHeld)
			{
				triggerAmount = hand.Input.TriggerFloat;
				if ((hand.IsInStreamlinedMode && hand.Input.BYButtonDown) || (!hand.IsInStreamlinedMode && hand.Input.TouchpadDown))
				{
					m_isRunning = false;
					m_motorSpeed = 0.98f;
				}
			}
		}

		private void OnCollisionStay(Collision col)
		{
			if (!m_isRunning || !(m_sawingIntesity > 0.1f))
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < col.contacts.Length; i++)
			{
				if (!m_bladeCols.Contains(col.contacts[i].thisCollider))
				{
					continue;
				}
				IFVRDamageable component = col.contacts[i].otherCollider.transform.gameObject.GetComponent<IFVRDamageable>();
				if (component != null && DamageablesToDoHS.Add(component))
				{
					DamageablesToDo.Add(component);
					DamageableHitPoints.Add(col.contacts[i].point);
					DamageableHitNormals.Add(col.contacts[i].normal);
				}
				if (component == null && col.contacts[i].otherCollider.attachedRigidbody != null)
				{
					component = col.contacts[i].otherCollider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
					if (DamageablesToDoHS.Add(component))
					{
						DamageablesToDo.Add(component);
						DamageableHitPoints.Add(col.contacts[i].point);
						DamageableHitNormals.Add(col.contacts[i].normal);
					}
				}
				if (num < 2)
				{
					timeSinceCollision = 0f;
					num++;
					Vector3 closestValidPoint = GetClosestValidPoint(BladePoint1.position, BladePoint2.position, col.contacts[i].point);
					Vector3 vector = col.contacts[i].point - closestValidPoint;
					vector = Vector3.ClampMagnitude(vector, 0.04f);
					Vector3 vector2 = closestValidPoint + vector;
					emitParams.position = vector2;
					Vector3 velocity = Vector3.Cross(vector.normalized, base.transform.right) * Random.Range(1f, 10f);
					velocity += Random.onUnitSphere * 3f;
					velocity += vector * 2f;
					emitParams.velocity = velocity;
					Sparks.Emit(emitParams, 1);
					if (framesTilFlash <= 0)
					{
						framesTilFlash = Random.Range(3, 7);
						FXM.InitiateMuzzleFlash(vector2, col.contacts[i].normal, Random.Range(0.25f, 2f), Color.white, Random.Range(0.5f, 1f));
					}
				}
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (framesTilFlash > 0)
			{
				framesTilFlash--;
			}
			if (timeSinceCollision < 1f)
			{
				timeSinceCollision += Time.deltaTime;
			}
			if (TimeSinceDamageDealing > 0f)
			{
				TimeSinceDamageDealing -= Time.deltaTime;
			}
			else
			{
				Damage damage = new Damage();
				damage.Dam_Blunt = 100f * Mathf.Clamp(base.RootRigidbody.velocity.magnitude, 1f, 3f);
				damage.Dam_Cutting = 250f * Mathf.Clamp(base.RootRigidbody.velocity.magnitude, 1f, 2f);
				damage.Dam_TotalKinetic = damage.Dam_Cutting + damage.Dam_Blunt;
				damage.Class = Damage.DamageClass.Melee;
				for (int i = 0; i < DamageablesToDo.Count; i++)
				{
					if ((MonoBehaviour)DamageablesToDo[i] != null)
					{
						damage.hitNormal = DamageableHitNormals[i];
						damage.point = DamageableHitPoints[i];
						damage.strikeDir = -damage.hitNormal;
						DamageablesToDo[i].Damage(damage);
					}
				}
				DamageablesToDo.Clear();
				DamageablesToDoHS.Clear();
				DamageableHitPoints.Clear();
				DamageableHitNormals.Clear();
				TimeSinceDamageDealing = 0.1f;
			}
			if (!m_isRunning)
			{
				SawAudio.volume = m_motorSpeed * 0.7f;
				StartingAudio.volume = m_motorSpeed;
				if (m_motorSpeed <= 0f && StartingAudio.isPlaying)
				{
					StartingAudio.Stop();
				}
				if (UsesBladeSolidBits)
				{
					m_matBladeSolid.SetVector("_MainTexVelocity", new Vector2(0f, 0f));
					m_matBladeBits.SetVector("_MainTexVelocity", new Vector2(0f, 0f));
				}
				ParticleSystem.EmissionModule emission = EngineSmoke.emission;
				ParticleSystem.MinMaxCurve rate = emission.rate;
				rate.mode = ParticleSystemCurveMode.Constant;
				rate.constantMax = 0f;
				rate.constantMin = 0f;
				emission.rate = rate;
			}
			else
			{
				if (!SawAudio.isPlaying)
				{
					SawAudio.Play();
				}
				triggerAmount += Random.Range(-0.05f, 0.05f);
				if (base.IsHeld)
				{
					m_sawingIntesity = Mathf.Lerp(m_sawingIntesity, triggerAmount, Time.deltaTime * 5f);
				}
				else
				{
					m_sawingIntesity = Mathf.Lerp(m_sawingIntesity, 0f, Time.deltaTime * 2f);
				}
				if (m_sawingIntesity > 0.1f)
				{
					SawAudio.volume = (0.8f + m_sawingIntesity * 0.5f) * 0.3f;
					SawAudio.pitch = 0.6f + m_sawingIntesity * 0.7f;
					if ((double)timeSinceCollision < 0.2)
					{
						if (SawAudio.clip != AudClip_Hitting)
						{
							SawAudio.clip = AudClip_Hitting;
						}
					}
					else if (SawAudio.clip != AudClip_Buzzing)
					{
						SawAudio.clip = AudClip_Buzzing;
					}
					if (UsesBladeSolidBits)
					{
						m_matBladeSolid.SetVector("_MainTexVelocity", new Vector2(m_sawingIntesity, 0f));
						m_matBladeBits.SetVector("_MainTexVelocity", new Vector2(m_sawingIntesity, 0f));
					}
				}
				else
				{
					SawAudio.volume = 0.25f;
					SawAudio.pitch = 1f;
					if (SawAudio.clip != AudClip_Idle)
					{
						SawAudio.clip = AudClip_Idle;
					}
					if (UsesBladeSolidBits)
					{
						m_matBladeSolid.SetVector("_MainTexVelocity", new Vector2(0.01f, 0f));
						m_matBladeBits.SetVector("_MainTexVelocity", new Vector2(0.01f, 0f));
					}
				}
				ParticleSystem.EmissionModule emission2 = EngineSmoke.emission;
				ParticleSystem.MinMaxCurve rate2 = emission2.rate;
				rate2.mode = ParticleSystemCurveMode.Constant;
				rate2.constantMax = m_motorSpeed * 2f + m_sawingIntesity * 20f;
				rate2.constantMin = m_motorSpeed * 2f + m_sawingIntesity * 20f;
				emission2.rate = rate2;
			}
			if (m_motorSpeed >= 1f)
			{
				m_isRunning = true;
			}
			else
			{
				m_motorSpeed -= Time.deltaTime * 3f;
				m_motorSpeed = Mathf.Clamp(m_motorSpeed, 0f, 1f);
			}
			if (UsesEngineRot)
			{
				float x = EngineRot.localEulerAngles.x;
				x = ((!(m_sawingIntesity > 0f)) ? (x + Time.deltaTime * (360f * m_motorSpeed)) : (x + Time.deltaTime * (360f + m_sawingIntesity * 1200f)));
				x = Mathf.Repeat(x, 360f);
				EngineRot.localEulerAngles = new Vector3(x, 0f, 0f);
			}
			if (m_isRunning)
			{
				m_timeTilPerceptibleEventTick -= Time.deltaTime;
				if (m_timeTilPerceptibleEventTick <= 0f)
				{
					m_timeTilPerceptibleEventTick = Random.Range(0.2f, 0.3f);
					GM.CurrentSceneSettings.OnPerceiveableSound(PerceptibleEventVolume * m_motorSpeed * m_sawingIntesity * 0.5f, PerceptibleEventRange * m_motorSpeed * m_sawingIntesity * 0.5f, base.transform.position, GM.CurrentPlayerBody.GetPlayerIFF());
				}
			}
		}

		protected override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			if (m_isRunning)
			{
				float num = 0.1f;
				num += m_sawingIntesity * 0.3f;
				base.RootRigidbody.velocity += Random.onUnitSphere * num;
				base.RootRigidbody.angularVelocity += Random.onUnitSphere * num;
			}
		}

		public void SetCableLength(float f)
		{
			if (!m_isRunning && f > m_currentCableLength)
			{
				if (!StartingAudio.isPlaying)
				{
					StartingAudio.Play();
				}
				m_motorSpeed += (f - m_currentCableLength) * 1.5f;
			}
			m_currentCableLength = f;
		}
	}
}
