using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class RotwienerNest_Nodule : MonoBehaviour, IFVRDamageable
	{
		public enum NoduleState
		{
			Protected,
			Unprotected,
			Naked,
			Destroyed
		}

		public enum NoduleType
		{
			Tendril,
			Core
		}

		[Serializable]
		public class BleedingEvent
		{
			public ParticleSystem m_system;

			public float mustardLeftToBleed;

			public float BleedIntensity;

			public float BleedVFXIntensity;

			public BleedingEvent(GameObject PrefabToSpawn, Transform p, float bloodAmount, Vector3 pos, Vector3 dir, float bleedIntensity, float vfxIntensity)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(PrefabToSpawn, pos, Quaternion.LookRotation(dir));
				m_system = gameObject.GetComponent<ParticleSystem>();
				mustardLeftToBleed = bloodAmount;
				gameObject.transform.SetParent(p);
				BleedIntensity = bleedIntensity;
				BleedVFXIntensity = vfxIntensity;
			}

			public float Update(float t, float totalMustardLeft)
			{
				float num;
				if (mustardLeftToBleed > 0f && totalMustardLeft > 0f)
				{
					float value = BleedIntensity * t;
					value = Mathf.Clamp(value, 0f, mustardLeftToBleed);
					num = value;
					mustardLeftToBleed -= num;
				}
				else
				{
					BleedIntensity = 0f;
					num = 0f;
				}
				if (m_system != null)
				{
					ParticleSystem.EmissionModule emission = m_system.emission;
					ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
					rateOverTime.mode = ParticleSystemCurveMode.Constant;
					float max = 10f * BleedVFXIntensity;
					rateOverTime.constant = Mathf.Clamp(BleedIntensity * 2f, 0f, max);
					emission.rateOverTime = rateOverTime;
				}
				return num;
			}

			public bool IsDone()
			{
				if (mustardLeftToBleed <= 0f && m_system.particleCount <= 0)
				{
					return true;
				}
				return false;
			}

			public void EndBleedEvent()
			{
				mustardLeftToBleed = 0f;
				if (m_system != null)
				{
					UnityEngine.Object.Destroy(m_system);
				}
			}

			public void Dispose()
			{
				if (m_system != null)
				{
					UnityEngine.Object.Destroy(m_system);
				}
			}
		}

		public NoduleState State;

		public NoduleType Type;

		public GameObject Core;

		public GameObject Wrap_Closed;

		public GameObject Wrap_Open;

		public Collider C;

		private RotwienerNest m_nest;

		private RotwienerNest_Tendril m_tendril;

		public Transform t_Core;

		public Transform t_Wrap_Closed;

		public AudioSource AudSource_loop;

		public AudioEvent AudEvent_Scream;

		public GameObject SpawnOnSpode;

		[Header("DamageMults")]
		public float Mult_Projectile = 1f;

		public float Mult_Melee = 1f;

		public float Mult_Explosive = 1f;

		public float Mult_Piercing = 1f;

		public float Mult_Cutting = 1f;

		public float Mult_Blunt = 1f;

		public float Mult_Thermal = 1f;

		private float m_timeSinceScream;

		private float m_bounceX;

		private float m_bounceY;

		private float m_bounceZ;

		private float m_speed = 1f;

		private float m_baseSpeed = 1f;

		[Header("Bleeding Logic")]
		public float Mustard = 100f;

		private float m_maxMustard = 100f;

		public float BleedDamageMult = 0.5f;

		public float BleedRateMult = 1f;

		public float BleedVFXIntensity = 0.3f;

		private bool m_isBleeding;

		private float m_bleedRate;

		private bool m_needsToSpawnBleedEvent;

		private float m_bloodLossForVFX;

		private Vector3 m_bloodLossPoint;

		private Vector3 m_bloodLossDir;

		public GameObject DamageFX_SmallMustardBurst;

		public GameObject DamageFX_LargeMustardBurst;

		public GameObject DamageFX_MustardSpoutSmall;

		public GameObject DamageFX_MustardSpoutLarge;

		public GameObject DamageFX_Explosion;

		private List<BleedingEvent> m_bleedingEvents = new List<BleedingEvent>();

		public void SetType(NoduleType type, RotwienerNest n, RotwienerNest_Tendril t)
		{
			Type = type;
			m_nest = n;
			m_tendril = t;
			m_bounceY = 0.33f;
			m_bounceZ = 0.66f;
		}

		public void SetState(NoduleState s, bool isInit)
		{
			if (s == State && !isInit)
			{
				return;
			}
			State = s;
			switch (s)
			{
			case NoduleState.Protected:
				Wrap_Closed.SetActive(value: true);
				Wrap_Open.SetActive(value: false);
				break;
			case NoduleState.Unprotected:
				Wrap_Closed.SetActive(value: false);
				Wrap_Open.SetActive(value: true);
				if (Type == NoduleType.Tendril)
				{
					AudSource_loop.pitch = UnityEngine.Random.Range(0.5f, 0.7f);
					AudSource_loop.Play();
				}
				break;
			case NoduleState.Naked:
				Wrap_Closed.SetActive(value: false);
				Wrap_Open.SetActive(value: false);
				break;
			case NoduleState.Destroyed:
				AudSource_loop.Stop();
				DestroyMe();
				if (!isInit)
				{
					UnityEngine.Object.Instantiate(SpawnOnSpode, Core.transform.position, UnityEngine.Random.rotation);
					GM.CurrentSceneSettings.OnPerceiveableSound(200f, 100f, base.transform.position, 0);
				}
				break;
			}
		}

		private void DestroyMe()
		{
			if (Type == NoduleType.Tendril)
			{
				m_tendril.NoduleDestroyed(this);
			}
			else if (Type == NoduleType.Core)
			{
				m_nest.NoduleDestroyed(this);
			}
		}

		public void Damage(Damage d)
		{
			if (State == NoduleState.Destroyed || State == NoduleState.Protected)
			{
				return;
			}
			float num = 1f;
			float num2 = 0f;
			if (d.Class == FistVR.Damage.DamageClass.Projectile || d.Class == FistVR.Damage.DamageClass.Explosive || d.Class == FistVR.Damage.DamageClass.Melee)
			{
				float num3 = d.Dam_Piercing * Mult_Piercing;
				float num4 = d.Dam_Cutting * Mult_Cutting;
				float num5 = d.Dam_Blunt * Mult_Blunt;
				float num6 = d.Dam_Thermal * Mult_Thermal;
				num2 = num3 + num4 + num5 + num6;
				if (d.Class == FistVR.Damage.DamageClass.Projectile)
				{
					num2 *= Mult_Projectile;
				}
				else if (d.Class == FistVR.Damage.DamageClass.Melee)
				{
					num2 *= Mult_Melee;
				}
				else if (d.Class == FistVR.Damage.DamageClass.Explosive)
				{
					num2 *= Mult_Explosive;
				}
				float num7 = (num2 - 50f) * 0.05f;
				num7 = Mathf.Clamp(num7 * num, 0f, 1000f);
				AccurueBleedingHit(d.point, d.strikeDir, num7);
				m_speed = 3f * m_baseSpeed;
			}
			if (num2 > 500f && m_timeSinceScream >= 3f)
			{
				SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, AudEvent_Scream, base.transform.position, Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.transform.position) / 343f);
				m_timeSinceScream = 0f;
				GM.CurrentSceneSettings.OnPerceiveableSound(50f, 60f, base.transform.position, 0);
			}
		}

		public void Update()
		{
			m_baseSpeed = Mathf.Lerp(Mustard / m_maxMustard, 3f, 1f);
			m_speed = Mathf.MoveTowards(m_speed, m_baseSpeed, Time.deltaTime * 2f * m_baseSpeed);
			if (m_timeSinceScream < 10f)
			{
				m_timeSinceScream += Time.deltaTime;
			}
			if (State == NoduleState.Protected)
			{
				m_bounceX = Mathf.Repeat(m_bounceX + Time.deltaTime * 0.3f, 1f);
				m_bounceY = Mathf.Repeat(m_bounceY + Time.deltaTime * 0.3f, 1f);
				m_bounceZ = Mathf.Repeat(m_bounceZ + Time.deltaTime * 0.3f, 1f);
				Vector3 localScale = new Vector3(1f + Mathf.Sin(m_bounceX * (float)Math.PI * 2f) * 0.025f, 1f + Mathf.Sin(m_bounceY * (float)Math.PI * 2f) * 0.025f, 1f + Mathf.Sin(m_bounceZ * (float)Math.PI * 2f) * 0.025f);
				t_Wrap_Closed.localScale = localScale;
			}
			else if (State == NoduleState.Unprotected)
			{
				m_bounceX = Mathf.Repeat(m_bounceX + Time.deltaTime * 0.8f * m_speed, 1f);
				m_bounceY = Mathf.Repeat(m_bounceY + Time.deltaTime * 0.8f * m_speed, 1f);
				m_bounceZ = Mathf.Repeat(m_bounceZ + Time.deltaTime * 0.8f * m_speed, 1f);
				Vector3 localScale2 = new Vector3(1f + Mathf.Sin(m_bounceX * (float)Math.PI * 2f) * 0.025f, 1f + Mathf.Sin(m_bounceY * (float)Math.PI * 2f) * 0.025f, 1f + Mathf.Sin(m_bounceZ * (float)Math.PI * 2f) * 0.025f);
				t_Core.localScale = localScale2;
			}
			else if (State == NoduleState.Naked)
			{
				m_bounceX = Mathf.Repeat(m_bounceX + Time.deltaTime * 0.8f * m_speed, 1f);
				m_bounceY = Mathf.Repeat(m_bounceY + Time.deltaTime * 0.8f * m_speed, 1f);
				m_bounceZ = Mathf.Repeat(m_bounceZ + Time.deltaTime * 0.8f * m_speed, 1f);
				Vector3 localScale3 = new Vector3(1f + Mathf.Sin(m_bounceX * (float)Math.PI * 2f) * 0.025f, 1f + Mathf.Sin(m_bounceY * (float)Math.PI * 2f) * 0.025f, 1f + Mathf.Sin(m_bounceZ * (float)Math.PI * 2f) * 0.025f);
				t_Core.localScale = localScale3;
			}
			BleedingUpdate();
		}

		public void AccurueBleedingHit(Vector3 point, Vector3 dir, float bloodAmount)
		{
			m_needsToSpawnBleedEvent = true;
			m_bloodLossPoint = C.ClosestPoint(point);
			m_bloodLossDir = dir;
			m_bloodLossForVFX += bloodAmount * BleedDamageMult;
		}

		private void BleedingUpdate()
		{
			if (m_needsToSpawnBleedEvent && Mustard > 0f)
			{
				m_needsToSpawnBleedEvent = false;
				if (m_bloodLossForVFX >= 10f)
				{
					UnityEngine.Object.Instantiate(DamageFX_LargeMustardBurst, m_bloodLossPoint, Quaternion.LookRotation(m_bloodLossDir));
					BleedingEvent item = new BleedingEvent(DamageFX_MustardSpoutLarge, base.transform, m_bloodLossForVFX, m_bloodLossPoint, -m_bloodLossDir, m_bloodLossForVFX * 0.25f, BleedVFXIntensity);
					m_bleedingEvents.Add(item);
				}
				if (m_bloodLossForVFX >= 1f)
				{
					UnityEngine.Object.Instantiate(DamageFX_SmallMustardBurst, m_bloodLossPoint, Quaternion.LookRotation(-m_bloodLossDir));
					BleedingEvent item2 = new BleedingEvent(DamageFX_MustardSpoutSmall, base.transform, m_bloodLossForVFX, m_bloodLossPoint, -m_bloodLossDir, m_bloodLossForVFX * 0.25f, BleedVFXIntensity);
					m_bleedingEvents.Add(item2);
				}
			}
			m_bleedRate = 0f;
			if (m_bleedingEvents.Count > 0)
			{
				float deltaTime = Time.deltaTime;
				for (int num = m_bleedingEvents.Count - 1; num >= 0; num--)
				{
					if (m_bleedingEvents[num].IsDone())
					{
						if (m_bleedingEvents[num].m_system != null)
						{
							UnityEngine.Object.Destroy(m_bleedingEvents[num].m_system.gameObject);
						}
						m_bleedingEvents.RemoveAt(num);
					}
					else
					{
						float num2 = m_bleedingEvents[num].Update(deltaTime, Mustard);
						m_bleedRate += num2;
					}
				}
			}
			if (Mustard > 0f && m_bleedRate > 0f)
			{
				Mustard -= m_bleedRate * BleedRateMult;
				if (Mustard <= 0f)
				{
					SetState(NoduleState.Destroyed, isInit: false);
				}
			}
			m_bloodLossForVFX = 0f;
		}
	}
}
