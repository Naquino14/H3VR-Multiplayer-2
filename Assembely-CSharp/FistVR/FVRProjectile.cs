using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FVRProjectile : MonoBehaviour
	{
		[Serializable]
		public class ProjectilePayload
		{
			public GameObject[] Submunitions;

			public int SubmunitionNumber;

			public bool IsRandomRot = true;

			public bool IsSubmunitionRomanCandle;

			public bool IsBackwards;

			public bool IsSubmunitionRigidbody;

			public Vector2 RBSpeed = default(Vector2);
		}

		private bool m_isActive;

		private bool m_isMoving;

		private Transform m_shotOrigin;

		[Header("Bullet Parameters")]
		public string DisplayName;

		public float PointsDamage = 1f;

		public bool DoesIgnite;

		public bool DoesFreeze;

		public bool DoesDisrupt;

		public float Mass;

		public Vector3 Dimensions;

		public float NoseContactRadius = 0.0005f;

		public bool IsExpanding;

		private bool m_isExpanded;

		public float MuzzleVelocity;

		public bool UsesAirDrag = true;

		public float GravityMultiplier = 1f;

		private bool m_isInWater;

		public PMat PMat;

		private PMaterialDefinition m_pMatDef;

		public ImpactEffectMagnitude ImpactFXMagnitude = ImpactEffectMagnitude.Medium;

		[Header("Submunitions")]
		public ProjectilePayload[] Payloads;

		public bool IsDisabledOnFirstImpact = true;

		private bool hasFiredSubmunition;

		[Header("References")]
		public Transform tracer;

		public Renderer m_tracerRenderer;

		public Renderer m_bulletRenderer;

		public GameObject ExtraDisplay;

		public float TracerLengthMultiplier = 1f;

		public float TracerWidthMultiplier = 1f;

		public bool UsesTrails = true;

		public VRTrail Trail;

		public Color TrailStartColor;

		private LayerMask LM;

		private RaycastHit m_hit;

		[Header("Life and Timeouts")]
		public float MaxRange = 500f;

		public float MaxRangeRandom;

		private float m_distanceTraveled;

		private float m_TrailDieTimer = 5f;

		private float m_TrailDieTimerMax = 5f;

		public float m_dieTimerMax = 5f;

		private float m_dieTimerTick = 5f;

		private Vector3 m_velocity = Vector3.zero;

		private Vector3 m_forward = Vector3.forward;

		private float m_penetration;

		private float m_tumbling;

		private float m_initialMuzzleVelocity;

		private Collider m_lastHitCollider;

		private Rigidbody m_lastHitRigidbody;

		private Vector3 m_lastHitPoint;

		private PMat m_lastHitPMat;

		private IFVRReceiveDamageable m_lastHitDamageable;

		private Vector2 m_lastHitUVCoords = Vector2.zero;

		private List<Vector3> m_pastPositions = new List<Vector3>();

		private List<float> m_pastVelocities = new List<float>();

		private FVRFireArm m_firearmSource;

		private bool m_isPlayer;

		public bool DeletesOnStraightDown = true;

		private int newTrailTick;

		private void Awake()
		{
			MaxRange += UnityEngine.Random.Range(0f, MaxRangeRandom);
			LM = AM.PLM;
			CalculatePenetrationStat();
			if (tracer != null)
			{
				tracer.gameObject.SetActive(value: true);
				Renderer component = tracer.GetComponent<Renderer>();
				if (component != null)
				{
					m_tracerRenderer = component;
				}
			}
			if (GM.Options.QuickbeltOptions.AreBulletTrailsEnabled && UsesTrails)
			{
				Trail = base.gameObject.AddComponent<VRTrail>();
				Trail.Color = TrailStartColor;
			}
			m_TrailDieTimerMax = GM.Options.QuickbeltOptions.TrailDecayTimes[GM.Options.QuickbeltOptions.TrailDecaySetting];
			m_TrailDieTimer = m_TrailDieTimerMax;
			m_dieTimerTick = m_dieTimerMax;
			if (Trail != null && m_TrailDieTimer > m_dieTimerTick)
			{
				m_dieTimerTick = m_TrailDieTimer;
			}
		}

		public void SetDamageType(DamageDealt.DamageType type)
		{
		}

		public void SetPlayerDamage(bool b)
		{
			m_isPlayer = b;
		}

		public void Fire(Vector3 forwardDir, FVRFireArm firearm)
		{
			Fire(MuzzleVelocity, forwardDir, firearm);
		}

		public void SetIsInWater(bool b)
		{
			m_isInWater = b;
		}

		public void Fire(float muzzleVelocity, Vector3 forwardDir, FVRFireArm firearm)
		{
			m_initialMuzzleVelocity = muzzleVelocity;
			m_isActive = true;
			m_isMoving = true;
			m_velocity = forwardDir.normalized * muzzleVelocity;
			m_pastPositions.Add(base.transform.position);
			m_pastVelocities.Add(m_velocity.magnitude);
			if (Trail != null)
			{
				Trail.AddPosition(base.transform.position);
			}
			m_firearmSource = firearm;
			UpdateBulletPath();
		}

		private void TickDownToDeath()
		{
			if (tracer != null)
			{
				tracer.localScale = new Vector3(0.01f, 0.01f, 0.01f);
				if (m_tracerRenderer != null)
				{
					m_tracerRenderer.enabled = false;
				}
			}
			if (m_bulletRenderer != null)
			{
				m_bulletRenderer.enabled = false;
			}
			if (ExtraDisplay != null)
			{
				ExtraDisplay.SetActive(value: false);
			}
			m_dieTimerTick -= Time.deltaTime;
			if (m_dieTimerTick <= 0f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private void ClearLastHitData()
		{
			m_lastHitCollider = null;
			m_lastHitRigidbody = null;
			m_lastHitPMat = null;
			m_lastHitDamageable = null;
		}

		private void ExpandBullet()
		{
			if (IsExpanding && !m_isExpanded)
			{
				m_isExpanded = true;
				Dimensions.x += Dimensions.z;
				Dimensions.y = Dimensions.x;
				Dimensions.z *= 0.25f;
				NoseContactRadius *= 2f;
			}
		}

		private void CalculatePenetrationStat()
		{
			if (m_isExpanded)
			{
				m_penetration = 1f;
				return;
			}
			float num = Dimensions.x * 0.5f;
			float num2 = Mathf.Clamp(1f - NoseContactRadius / num, 0.1f, 1f);
			m_penetration = num2 * 10f;
		}

		private Vector3 ApplyDrag(Vector3 velocity, float materialDensity, float cascadedTime, bool isContact, bool isAirDrag)
		{
			float num = 0f;
			num = ((!isAirDrag) ? GetCurrentBulletArea(isContact) : ((float)Math.PI * Mathf.Pow(Dimensions.x * 0.5f, 2f)));
			float currentDragCoefficient = GetCurrentDragCoefficient(velocity.magnitude);
			Vector3 vector = -velocity * (materialDensity * 0.5f * currentDragCoefficient * num / Mass) * velocity.magnitude;
			return velocity.normalized * Mathf.Clamp(velocity.magnitude - vector.magnitude * cascadedTime, 0f, velocity.magnitude);
		}

		private float GetCurrentBulletArea(bool isContact)
		{
			float num = 0.1f;
			if (isContact)
			{
				float a = (float)Math.PI * Mathf.Pow(NoseContactRadius, 2f);
				float b = NoseContactRadius * Dimensions.z;
				return Mathf.Lerp(a, b, m_tumbling);
			}
			float b2 = (float)Math.PI * Dimensions.x * 0.5f * Mathf.Sqrt(Mathf.Pow(Dimensions.z * 0.5f, 2f) + Mathf.Pow(Dimensions.x * 0.5f, 2f));
			float a2 = (float)Math.PI * Mathf.Pow(Dimensions.x * 0.5f, 2f) + (float)Math.PI * 2f * Dimensions.x * 0.5f * Dimensions.z;
			return Mathf.Lerp(a2, b2, m_penetration * 0.1f);
		}

		private float GetCurrentDragCoefficient(float velocityMS)
		{
			if (m_isExpanded)
			{
				return 1f;
			}
			return AM.BDCC.Evaluate(velocityMS * 0.00291545f);
		}

		private void FireSubmunitions(Vector3 point)
		{
			if (Payloads.Length <= 0 || hasFiredSubmunition)
			{
				return;
			}
			hasFiredSubmunition = true;
			if (IsDisabledOnFirstImpact)
			{
				m_isMoving = false;
			}
			for (int i = 0; i < Payloads.Length; i++)
			{
				if (Payloads[i].Submunitions.Length <= 0)
				{
					continue;
				}
				for (int j = 0; j < Payloads[i].SubmunitionNumber; j++)
				{
					int num = UnityEngine.Random.Range(0, Payloads[i].Submunitions.Length);
					Quaternion rotation = base.transform.rotation;
					if (Payloads[i].IsRandomRot)
					{
						rotation = UnityEngine.Random.rotation;
					}
					if (Payloads[i].IsBackwards)
					{
						rotation = Quaternion.LookRotation(Vector3.up);
					}
					GameObject gameObject = UnityEngine.Object.Instantiate(Payloads[i].Submunitions[num], point, rotation);
					FVRProjectile component = gameObject.GetComponent<FVRProjectile>();
					if (component != null)
					{
						if (Payloads[i].IsBackwards)
						{
							component.Fire(component.MuzzleVelocity, gameObject.transform.forward, m_firearmSource);
						}
						else
						{
							component.Fire(component.MuzzleVelocity, Vector3.Lerp(base.transform.forward, UnityEngine.Random.onUnitSphere, UnityEngine.Random.Range(0.05f, 0.7f)), m_firearmSource);
						}
					}
					if (Payloads[i].IsSubmunitionRomanCandle)
					{
						RomanCandleCharge component2 = gameObject.GetComponent<RomanCandleCharge>();
						component2.Fire();
					}
					if (Payloads[i].IsSubmunitionRigidbody)
					{
						gameObject.GetComponent<Rigidbody>().velocity = UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(Payloads[i].RBSpeed.x, Payloads[i].RBSpeed.y);
					}
				}
			}
		}

		private void FixedUpdate()
		{
			if (Trail != null)
			{
				m_TrailDieTimer -= Time.deltaTime;
				if (m_TrailDieTimer < 0f)
				{
					UnityEngine.Object.Destroy(Trail);
				}
				else
				{
					Trail.Color.a = m_TrailDieTimer / m_TrailDieTimerMax;
				}
			}
			UpdateBulletPath();
			if (m_distanceTraveled > MaxRange && m_isMoving)
			{
				m_isMoving = false;
				FireSubmunitions(base.transform.position);
			}
		}

		private float GetDamageVelocityScaled(float MaxDam, Vector3 velocity)
		{
			float magnitude = velocity.magnitude;
			float num = magnitude / m_initialMuzzleVelocity;
			return MaxDam * num;
		}

		private void UpdateBulletPath()
		{
			if (!m_isActive || !m_isMoving)
			{
				TickDownToDeath();
				return;
			}
			UpdateVelocity(Time.fixedDeltaTime);
			MoveBullet(Time.fixedDeltaTime);
			m_pastPositions.Add(base.transform.position);
			m_pastVelocities.Add(m_velocity.magnitude);
			if (Trail != null)
			{
				if (newTrailTick > 0)
				{
					newTrailTick--;
				}
				else
				{
					Trail.AddPosition(base.transform.position);
					newTrailTick = Mathf.RoundToInt(m_distanceTraveled / 100f);
				}
			}
			if (tracer != null)
			{
				tracer.localScale = new Vector3(0.04f * TracerWidthMultiplier, 0.04f * TracerWidthMultiplier, Mathf.Min(m_velocity.magnitude * Time.deltaTime * TracerLengthMultiplier, m_distanceTraveled));
			}
		}

		private void UpdateVelocity(float cascadedTime)
		{
			if (m_velocity.magnitude < 0.1f || base.transform.position.y < -100f)
			{
				m_isMoving = false;
				return;
			}
			if (m_lastHitCollider == null)
			{
				float num = 9.81f;
				switch (GM.Options.SimulationOptions.BallisticGravityMode)
				{
				case SimulationOptions.GravityMode.Realistic:
					num = 9.81f;
					break;
				case SimulationOptions.GravityMode.Playful:
					num = 5f;
					break;
				case SimulationOptions.GravityMode.OnTheMoon:
					num = 1.622f;
					break;
				case SimulationOptions.GravityMode.None:
					num = 0f;
					break;
				}
				if (m_isInWater)
				{
					num = 0f;
				}
				m_velocity += Vector3.down * num * cascadedTime * GravityMultiplier;
			}
			float materialDensity = 1.225f;
			if (!UsesAirDrag)
			{
				materialDensity = 0f;
			}
			if (m_isInWater)
			{
				materialDensity = 16f;
			}
			Vector3 vector = m_velocity * Mass;
			float num2 = 0f;
			bool flag = false;
			bool flag2 = false;
			if (m_lastHitCollider != null)
			{
				if (base.transform.position != m_lastHitPoint && m_lastHitCollider.Raycast(new Ray(base.transform.position, m_lastHitPoint - base.transform.position), out m_hit, (m_lastHitPoint - base.transform.position).magnitude))
				{
					m_tumbling += Mathf.Lerp(0f, Mathf.Clamp(1f - Vector3.Dot(base.transform.forward, m_hit.normal), 0f, 1f), m_lastHitPMat.GetStiffness() * 0.3f);
					float magnitude = m_velocity.magnitude;
					Vector3 normalized = Vector3.ProjectOnPlane(m_velocity, m_hit.normal).normalized;
					m_velocity = Vector3.Lerp(m_velocity, normalized * magnitude, Mathf.Clamp(m_tumbling, 0f, 0.5f));
					flag = true;
				}
				else
				{
					ExpandBullet();
					CalculatePenetrationStat();
					m_tumbling += cascadedTime * (m_lastHitPMat.GetStiffness() / PMat.Def.stiffness) * m_lastHitPMat.GetBounciness();
					Vector3 vector2 = UnityEngine.Random.onUnitSphere;
					if (Vector3.Dot(vector2, m_velocity.normalized) < 0f)
					{
						vector2 = -vector2;
					}
					m_velocity = Vector3.Lerp(m_velocity, vector2 * m_velocity.magnitude, Mathf.Clamp(m_tumbling, 0f, 0.9f));
					Vector3 vector3 = base.transform.position + m_velocity * cascadedTime;
					float magnitude2 = (base.transform.position - vector3).magnitude;
					if (base.transform.position != vector3 && m_lastHitCollider.Raycast(new Ray(vector3, base.transform.position - vector3), out m_hit, magnitude2))
					{
						float num3 = Vector3.Distance(m_hit.point, base.transform.position);
						num2 = Mathf.Clamp(num3 / magnitude2, 0f, 1f);
						flag2 = true;
					}
					else
					{
						num2 = 1f;
					}
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				ClearLastHitData();
			}
			if (num2 > 0f)
			{
				m_velocity = ApplyDrag(m_velocity, m_lastHitPMat.GetDensity(), cascadedTime * num2, isContact: false, isAirDrag: false);
			}
			else if (m_isInWater)
			{
				m_velocity = ApplyDrag(m_velocity, materialDensity, cascadedTime, isContact: false, isAirDrag: false);
			}
			else
			{
				m_velocity = ApplyDrag(m_velocity, materialDensity, cascadedTime, isContact: false, isAirDrag: true);
			}
			Vector3 vector4 = m_velocity * Mass;
			if (m_lastHitPMat != null)
			{
				DamageDealt dam = default(DamageDealt);
				Vector3 vector5 = (vector4 - vector) / (Time.fixedDeltaTime * num2);
				float currentBulletArea = GetCurrentBulletArea(isContact: false);
				float num4 = vector5.magnitude / currentBulletArea;
				float mPa = num4 / 1000000f;
				float mPaRootMeter = Mathf.Pow(currentBulletArea, 0.25f) * num4 / 1000000f;
				dam.force = vector - vector4;
				dam.PointsDamage = GetDamageVelocityScaled(PointsDamage, m_velocity) * (Time.fixedDeltaTime * num2);
				dam.MPa = mPa;
				dam.MPaRootMeter = mPaRootMeter;
				dam.point = base.transform.position;
				dam.hitNormal = -base.transform.forward;
				dam.strikeDir = base.transform.forward;
				dam.uvCoords = Vector2.zero;
				dam.SourceFirearm = m_firearmSource;
				dam.IsPlayer = m_isPlayer;
				dam.IsInitialContact = false;
				dam.IsInside = true;
				dam.DoesIgnite = DoesIgnite;
				dam.DoesFreeze = DoesFreeze;
				dam.DoesDisrupt = DoesDisrupt;
				if (m_lastHitDamageable != null)
				{
					m_lastHitDamageable.Damage(dam);
				}
			}
			if (m_lastHitRigidbody != null)
			{
				m_lastHitRigidbody.AddForceAtPosition(vector - vector4, base.transform.position, ForceMode.Impulse);
			}
			if (flag2)
			{
				ClearLastHitData();
			}
		}

		private void MoveBullet(float cascadedTime)
		{
			if ((m_velocity.y < 0f && DeletesOnStraightDown && new Vector3(m_velocity.x, 0f, m_velocity.z).magnitude < 0.1f) || base.transform.position.y < -100f)
			{
				m_isMoving = false;
				return;
			}
			Vector3 position = base.transform.position;
			if (Physics.Raycast(base.transform.position, m_velocity.normalized, out m_hit, m_velocity.magnitude * cascadedTime, LM, QueryTriggerInteraction.Collide) && !m_hit.collider.gameObject.CompareTag("PlayerHand"))
			{
				GravityMultiplier = 1f;
				IFVRReceiveDamageable iFVRReceiveDamageable = null;
				iFVRReceiveDamageable = m_hit.collider.transform.gameObject.GetComponent<IFVRReceiveDamageable>();
				if (iFVRReceiveDamageable == null && m_hit.collider.attachedRigidbody != null)
				{
					iFVRReceiveDamageable = m_hit.collider.attachedRigidbody.gameObject.GetComponent<IFVRReceiveDamageable>();
				}
				if (iFVRReceiveDamageable != null)
				{
					m_lastHitDamageable = iFVRReceiveDamageable;
				}
				else
				{
					m_lastHitDamageable = null;
				}
				m_lastHitUVCoords = m_hit.textureCoord;
				if (m_hit.collider.gameObject.GetComponent<PMat>() != null)
				{
					m_lastHitPMat = m_hit.collider.gameObject.GetComponent<PMat>();
				}
				else
				{
					m_lastHitPMat = PM.DefaultMat;
				}
				if (m_hit.collider.attachedRigidbody != null)
				{
					m_lastHitRigidbody = m_hit.collider.attachedRigidbody;
				}
				else
				{
					m_lastHitRigidbody = null;
				}
				DamageDealt dam = default(DamageDealt);
				dam.point = m_hit.point;
				dam.PointsDamage = GetDamageVelocityScaled(PointsDamage, m_velocity);
				dam.ShotOrigin = m_shotOrigin;
				dam.hitNormal = m_hit.normal;
				dam.strikeDir = base.transform.forward;
				dam.uvCoords = m_lastHitUVCoords;
				dam.SourceFirearm = m_firearmSource;
				dam.IsPlayer = m_isPlayer;
				dam.IsInitialContact = true;
				dam.IsInside = false;
				dam.DoesIgnite = DoesIgnite;
				dam.DoesFreeze = DoesFreeze;
				dam.DoesDisrupt = DoesDisrupt;
				if (m_velocity.magnitude > 100f)
				{
					FXM.SpawnImpactEffect(m_hit.point, m_hit.normal, (int)m_lastHitPMat.Def.impactCategory, ImpactFXMagnitude, forwardBack: false);
				}
				Vector3 vector = m_velocity * Mass;
				Vector3 vector2 = UnityEngine.Random.onUnitSphere;
				if (Vector3.Dot(vector2, m_hit.normal) > 0f)
				{
					vector2 = -vector2;
				}
				Vector3 vector3 = Vector3.Slerp(m_hit.normal, vector2, UnityEngine.Random.Range(0f, m_lastHitPMat.GetRoughness()));
				Vector3 normalized = m_velocity.normalized;
				float num = Vector3.Dot(normalized, -vector3);
				Vector3 normalized2 = m_velocity.normalized;
				float num2 = Vector3.Dot(normalized2, vector3);
				float num3 = 1f / ((1f + m_lastHitPMat.GetStiffness()) / (1f + m_penetration));
				float num4 = 1f - num2 * num2;
				float num5 = 1f - num4 / (num3 * num3);
				bool flag = true;
				float num6 = 1f;
				float num7 = 0f;
				float num8 = 0f;
				float num9 = 0f;
				if (num5 > 0f)
				{
					m_lastHitCollider = m_hit.collider;
					m_lastHitPoint = m_hit.point;
					float num10 = Mathf.Sqrt(num5);
					m_velocity = (num3 * normalized2 + ((0f - num3) * num2 - num10) * -vector3).normalized * m_velocity.magnitude;
					if (Vector3.Dot(m_velocity.normalized, m_hit.normal) > 0f)
					{
						m_velocity = Vector3.Reflect(m_velocity, m_hit.normal);
					}
					float num11 = m_velocity.magnitude * Mass;
					num9 = GetCurrentBulletArea(isContact: true);
					float num12 = num11 / (Time.fixedDeltaTime * 0.1f);
					num7 = num12 / num9;
					num8 = num7 / 1000000f;
					if (num8 >= m_lastHitPMat.GetYieldStrength())
					{
						flag = false;
						float num13 = (num8 - m_lastHitPMat.GetYieldStrength()) / num8;
						m_velocity = Vector3.ClampMagnitude(m_velocity, m_velocity.magnitude * num13);
					}
					else
					{
						flag = true;
						num6 = Mathf.Clamp(1f - num8 / m_lastHitPMat.GetYieldStrength(), 0f, 1f);
					}
				}
				if (!flag)
				{
					Vector3 vector4 = m_velocity * Mass;
					base.transform.position = m_hit.point - m_hit.normal * 0.0025f;
					num7 = ((vector4 - vector) / (Time.fixedDeltaTime * 0.1f)).magnitude / num9;
					num8 = num7 / 1000000f;
					float mPaRootMeter = Mathf.Pow(num9, 0.25f) * num7 / 1000000f;
					dam.force = vector - vector4;
					dam.MPa = num8;
					dam.MPaRootMeter = mPaRootMeter;
					ExpandBullet();
					CalculatePenetrationStat();
					if (m_lastHitRigidbody != null && m_lastHitDamageable == null)
					{
						m_lastHitRigidbody.AddForceAtPosition(vector - vector4, m_hit.point, ForceMode.Impulse);
					}
					FireSubmunitions(m_hit.point + m_hit.normal * 0.05f);
					if (m_velocity.normalized != Vector3.zero)
					{
						base.transform.rotation = Quaternion.LookRotation(m_velocity.normalized);
					}
					if (m_lastHitDamageable != null)
					{
						m_lastHitDamageable.Damage(dam);
					}
				}
				if (flag)
				{
					base.transform.position = m_hit.point + m_hit.normal * 0.0025f;
					m_velocity = Vector3.Reflect(m_velocity, vector3);
					base.transform.rotation = Quaternion.LookRotation(m_velocity.normalized);
					float num14 = Mathf.Clamp((PMat.Def.bounciness + m_lastHitPMat.GetBounciness()) * 0.5f, 0f, 1f);
					m_velocity *= num14;
					m_velocity *= num6;
					m_tumbling += Mathf.Abs(Vector3.Dot(vector3, normalized));
					Vector3 vector5 = m_velocity * Mass;
					Vector3 vector6 = (vector5 - vector) / (Time.fixedDeltaTime * 0.1f);
					num9 = GetCurrentBulletArea(isContact: true);
					num7 = vector6.magnitude / num9;
					num8 = num7 / 1000000f;
					float mPaRootMeter2 = Mathf.Pow(num9, 0.25f) * num7 / 1000000f;
					dam.force = vector - vector5;
					dam.MPa = num8;
					dam.MPaRootMeter = mPaRootMeter2;
					if (m_lastHitRigidbody != null)
					{
						m_lastHitRigidbody.AddForceAtPosition(vector - vector5, m_hit.point, ForceMode.Impulse);
					}
					if (m_lastHitDamageable != null)
					{
						m_lastHitDamageable.Damage(dam);
					}
					FireSubmunitions(m_hit.point + m_hit.normal * 0.05f);
				}
			}
			else
			{
				base.transform.position = base.transform.position + m_velocity * cascadedTime;
				if (m_velocity.normalized != Vector3.zero)
				{
					base.transform.rotation = Quaternion.LookRotation(m_velocity);
				}
			}
			m_distanceTraveled += Vector3.Distance(base.transform.position, position);
		}
	}
}
