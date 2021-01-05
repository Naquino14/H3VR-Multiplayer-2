using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class Drill : FVRPhysicalObject
	{
		private float m_spinnyZ;

		public Transform SpinnyBit;

		public Transform Trigger;

		public AudioSource DrillAudio;

		public AudioClip AudClipRevving;

		public AudioClip AudClipDrilling;

		private float m_triggerFloat;

		private float m_motorSpeed;

		public ParticleSystem Sparks;

		private ParticleSystem.EmitParams emitParams;

		public Collider DrillBit;

		private List<IFVRDamageable> DamageablesToDo;

		private HashSet<IFVRDamageable> DamageablesToDoHS;

		private List<Vector3> DamageableHitPoints;

		private List<Vector3> DamageableHitNormals;

		private float TimeSinceDamageDealing = 0.2f;

		public ParticleSystem EngineSmoke;

		public float PerceptibleEventVolume = 50f;

		public float PerceptibleEventRange = 30f;

		private float m_timeTilPerceptibleEventTick = 0.2f;

		private float timeSinceCollision = 1f;

		private int framesTilFlash;

		protected override void Awake()
		{
			base.Awake();
			emitParams = default(ParticleSystem.EmitParams);
			DamageablesToDo = new List<IFVRDamageable>();
			DamageablesToDoHS = new HashSet<IFVRDamageable>();
			DamageableHitPoints = new List<Vector3>();
			DamageableHitNormals = new List<Vector3>();
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (m_hasTriggeredUpSinceBegin)
			{
				m_triggerFloat = hand.Input.TriggerFloat;
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (!base.IsHeld)
			{
				m_triggerFloat = 0f;
			}
			Trigger.localPosition = new Vector3(0f, 0f, m_triggerFloat * -0.0117f);
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
				damage.Dam_Blunt = 50f * Mathf.Clamp(base.RootRigidbody.velocity.magnitude, 1f, 3f);
				damage.Dam_Piercing = 250f * Mathf.Clamp(base.RootRigidbody.velocity.magnitude, 1f, 2f);
				damage.Dam_TotalKinetic = damage.Dam_Cutting + damage.Dam_Blunt;
				damage.Class = Damage.DamageClass.Melee;
				for (int i = 0; i < DamageablesToDo.Count; i++)
				{
					if ((MonoBehaviour)DamageablesToDo[i] != null)
					{
						damage.hitNormal = DamageableHitNormals[i];
						damage.point = DamageableHitPoints[i];
						damage.strikeDir = -base.transform.forward;
						DamageablesToDo[i].Damage(damage);
					}
				}
				DamageablesToDo.Clear();
				DamageablesToDoHS.Clear();
				DamageableHitPoints.Clear();
				DamageableHitNormals.Clear();
				TimeSinceDamageDealing = 0.1f;
			}
			if (base.IsHeld)
			{
				m_motorSpeed = Mathf.Lerp(m_motorSpeed, m_triggerFloat, Time.deltaTime * 4f);
			}
			else
			{
				m_motorSpeed = Mathf.Lerp(m_motorSpeed, 0f, Time.deltaTime * 4f);
			}
			if (m_motorSpeed <= 0.025f && DrillAudio.isPlaying)
			{
				DrillAudio.Stop();
			}
			else if (m_motorSpeed > 0.025f && !DrillAudio.isPlaying)
			{
				DrillAudio.Play();
			}
			if (timeSinceCollision < 0.2f)
			{
				DrillAudio.volume = 0.1f + m_motorSpeed * 0.15f;
				DrillAudio.pitch = 0.8f + m_motorSpeed * 0.2f;
			}
			else
			{
				DrillAudio.volume = 0.1f + m_motorSpeed * 0.15f;
				DrillAudio.pitch = 0.3f + m_motorSpeed * 0.7f;
			}
			if (timeSinceCollision < 0.2f && DrillAudio.clip != AudClipDrilling)
			{
				DrillAudio.clip = AudClipDrilling;
				ParticleSystem.EmissionModule emission = EngineSmoke.emission;
				ParticleSystem.MinMaxCurve rate = emission.rate;
				rate.mode = ParticleSystemCurveMode.Constant;
				rate.constantMax = 10f;
				rate.constantMin = 10f;
				emission.rate = rate;
			}
			else if (timeSinceCollision >= 0.2f && DrillAudio.clip != AudClipRevving)
			{
				DrillAudio.clip = AudClipRevving;
				ParticleSystem.EmissionModule emission2 = EngineSmoke.emission;
				ParticleSystem.MinMaxCurve rate2 = emission2.rate;
				rate2.mode = ParticleSystemCurveMode.Constant;
				rate2.constantMax = 0f;
				rate2.constantMin = 0f;
				emission2.rate = rate2;
			}
			if (m_motorSpeed > 0.1f)
			{
				m_spinnyZ += Time.deltaTime * (6000f * m_motorSpeed);
				m_spinnyZ = Mathf.Repeat(m_spinnyZ, 360f);
				SpinnyBit.localEulerAngles = new Vector3(0f, 0f, 0f - m_spinnyZ);
				m_timeTilPerceptibleEventTick -= Time.deltaTime;
				if (m_timeTilPerceptibleEventTick <= 0f)
				{
					m_timeTilPerceptibleEventTick = Random.Range(0.2f, 0.3f);
					GM.CurrentSceneSettings.OnPerceiveableSound(PerceptibleEventVolume * m_motorSpeed, PerceptibleEventRange * m_motorSpeed, base.transform.position, GM.CurrentPlayerBody.GetPlayerIFF());
				}
			}
		}

		private void OnCollisionStay(Collision col)
		{
			if (!(m_motorSpeed > 0.1f))
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < col.contacts.Length; i++)
			{
				if (!(col.contacts[i].thisCollider == DrillBit))
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
					Vector3 point = col.contacts[i].point;
					Vector3 vector = -SpinnyBit.forward;
					Vector3 vector2 = point;
					emitParams.position = vector2;
					Vector3 velocity = vector * Random.Range(0.02f, 10f) * m_motorSpeed;
					velocity += Random.onUnitSphere * 3f * m_motorSpeed;
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
	}
}
