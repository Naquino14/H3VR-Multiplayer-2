using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class BallisticProjectile : MonoBehaviour
	{
		[Serializable]
		public class Submunition
		{
			public enum SubmunitionType
			{
				GameObject,
				Projectile,
				Rigidbody,
				StickyBomb,
				MeleeThrown,
				Demonade
			}

			public enum SubmunitionTrajectoryType
			{
				Random,
				RicochetDir,
				Backwards,
				Forwards,
				ForwardsCone
			}

			public enum SubmunitionSpawnLogic
			{
				Outside,
				Inside,
				On
			}

			public List<GameObject> Prefabs;

			public int NumToSpawn;

			public SubmunitionTrajectoryType Trajectory;

			public SubmunitionType Type;

			public SubmunitionSpawnLogic SpawnLogic;

			public Vector2 Speed = default(Vector2);

			public float ConeLerp = 0.85f;
		}

		[Header("Projectile Parameters")]
		public float Mass;

		public Vector3 Dimensions;

		public float FrontArea;

		public float MuzzleVelocityBase;

		public BallisticProjectileType ProjType;

		public bool DoesIgniteOnHit;

		public float IgnitionChance = 0.2f;

		public float KETotalForHit;

		public float KEPerSquareMeterBase;

		public float FlightVelocityMultiplier = 1f;

		private float m_debugFlightVelGlobal = 1f;

		public float AirDragMultiplier = 1f;

		public float GravityMultiplier = 1f;

		public bool IsDisabledOnFirstImpact;

		public bool GeneratesImpactSound;

		public ImpactType ImpactSoundType = ImpactType.GunshotImpact;

		public bool GeneratesImpactDecals;

		public ImpactEffectMagnitude ImpactFXMagnitude = ImpactEffectMagnitude.Medium;

		public bool GeneratesSuppressionEvent = true;

		public float SuppressionIntensity = 1f;

		public float SuppressionRange = 5f;

		public bool DeletesOnStraightDown = true;

		public int Source_IFF;

		public bool UsesIFFMatSwap;

		public List<Material> IFFSwapMats;

		[Header("Life and Timeouts")]
		public float MaxRange = 500f;

		public float MaxRangeRandom;

		private float m_distanceTraveled;

		private float m_TrailDieTimer = 5f;

		private float m_TrailDieTimerMax = 5f;

		public float m_dieTimerMax = 5f;

		private float m_dieTimerTick = 5f;

		[Header("Tracer")]
		public Transform tracer;

		public Renderer TracerRenderer;

		private bool m_hasTracer;

		public Renderer BulletRenderer;

		private bool m_hasBulletRenderer;

		public GameObject ExtraDisplay;

		private bool m_hasExtraDisplay;

		public float TracerLengthMultiplier = 1f;

		public float TracerWidthMultiplier = 1f;

		[Header("Trails")]
		public bool UsesTrails = true;

		public VRTrail Trail;

		public Color TrailStartColor;

		private int newTrailTick;

		private float m_tracerDistanceScaleFactor = 0.02f;

		private bool m_isMoving;

		private bool m_isInWater;

		private bool m_isTumbling;

		private int m_stallFrames;

		private LayerMask LM;

		private RaycastHit m_hit;

		private Vector3 m_velocity = Vector3.zero;

		private Vector3 m_forward = Vector3.forward;

		private Vector3 m_lastPoint = Vector3.zero;

		private float m_gravMag = 9.81f;

		private float m_initialMuzzleVelocity;

		public BallisticImpactEffectType ImpactEffectTypeOverride = BallisticImpactEffectType.None;

		public BulletHoleDecalType BulletHoleDecalOverride;

		[Header("Submunitions")]
		public List<Submunition> Submunitions;

		private bool m_usesSubmunitions;

		private bool m_hasFiredSubmunitions;

		public bool PassesFirearmReferenceToSubmunitions;

		[Header("BlastJump")]
		public bool DoesBlastJumpOnFire;

		public float BlastJumpAmount;

		private Transform m_cachedHead;

		private FVRFireArm tempFA;

		private bool waitOneFrame;

		private bool hasTurnedOffRends;

		private bool m_isInReferenceTransform;

		private Vector3 m_localReferencePoint;

		private Transform m_transReference;

		private bool needsSecondCast;

		private Collider m_lastColliderHit;

		private float distMoved;

		private bool m_hasPlayedWhoosh;

		private float m_distanceFromPlayer;

		public void SetSource_IFF(int i)
		{
			Source_IFF = i;
			if (UsesIFFMatSwap)
			{
				i = Mathf.Clamp(i, 0, 2);
				BulletRenderer.material = IFFSwapMats[i];
			}
		}

		private void Awake()
		{
			m_cachedHead = GM.CurrentPlayerBody.Head;
			m_distanceFromPlayer = Vector3.Distance(base.transform.position, m_cachedHead.position);
			MaxRange = Mathf.Clamp(MaxRange, MaxRange, GM.CurrentSceneSettings.MaxProjectileRange);
			MaxRange += UnityEngine.Random.Range(0f, MaxRangeRandom);
			if (Submunitions.Count > 0)
			{
				m_usesSubmunitions = true;
			}
			LM = AM.PLM;
			switch (GM.Options.SimulationOptions.BallisticGravityMode)
			{
			case SimulationOptions.GravityMode.Realistic:
				m_gravMag = 9.81f;
				break;
			case SimulationOptions.GravityMode.Playful:
				m_gravMag = 5f;
				break;
			case SimulationOptions.GravityMode.OnTheMoon:
				m_gravMag = 1.622f;
				break;
			case SimulationOptions.GravityMode.None:
				m_gravMag = 0f;
				break;
			}
			if (TracerRenderer != null)
			{
				m_hasTracer = true;
				tracer.gameObject.SetActive(value: true);
				TracerRenderer.enabled = true;
			}
			if (BulletRenderer != null)
			{
				m_hasBulletRenderer = true;
			}
			if (ExtraDisplay != null)
			{
				m_hasExtraDisplay = true;
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

		public void Fire(Vector3 forwardDir, FVRFireArm firearm)
		{
			Fire(MuzzleVelocityBase, forwardDir, firearm);
		}

		public void Fire(float muzzleVelocity, Vector3 forwardDir, FVRFireArm firearm)
		{
			if (firearm != null)
			{
				SetSource_IFF(GM.CurrentPlayerBody.GetPlayerIFF());
			}
			if (PassesFirearmReferenceToSubmunitions && firearm != null)
			{
				tempFA = firearm;
			}
			if (DoesBlastJumpOnFire)
			{
				GM.CurrentMovementManager.Blast(-forwardDir, BlastJumpAmount);
			}
			m_initialMuzzleVelocity = muzzleVelocity;
			m_isMoving = true;
			m_velocity = forwardDir.normalized * muzzleVelocity;
			m_lastPoint = base.transform.position;
			if (Trail != null)
			{
				Trail.AddPosition(base.transform.position);
			}
			UpdateBulletPath();
		}

		private Vector3 ApplyDrag(Vector3 velocity, float materialDensity, float time)
		{
			float num = (float)Math.PI * Mathf.Pow(Dimensions.x * 0.5f, 2f);
			float magnitude = velocity.magnitude;
			Vector3 normalized = velocity.normalized;
			float currentDragCoefficient = GetCurrentDragCoefficient(velocity.magnitude);
			return normalized * Mathf.Clamp(magnitude - (-velocity * (materialDensity * 0.5f * currentDragCoefficient * num / Mass) * magnitude).magnitude * time, 0f, magnitude);
		}

		private float GetCurrentDragCoefficient(float velocityMS)
		{
			return AM.BDCC.Evaluate(velocityMS * 0.00291545f);
		}

		private void TickDownToDeath()
		{
			if (!waitOneFrame)
			{
				waitOneFrame = true;
			}
			else if (!hasTurnedOffRends)
			{
				hasTurnedOffRends = true;
				if (m_hasBulletRenderer)
				{
					BulletRenderer.enabled = false;
				}
				if (m_hasTracer)
				{
					TracerRenderer.enabled = false;
				}
				if (m_hasExtraDisplay)
				{
					ExtraDisplay.SetActive(value: false);
				}
			}
			m_dieTimerTick -= Time.deltaTime;
			if (m_dieTimerTick <= 0f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
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
				Vector3 normalized = m_velocity.normalized;
				FireSubmunitions(normalized, normalized, base.transform.position, m_velocity.magnitude);
			}
		}

		private void UpdateBulletPath()
		{
			if (m_stallFrames > 0)
			{
				m_stallFrames--;
				return;
			}
			bool flag = false;
			Vector3 position = base.transform.position;
			if (m_lastPoint != position)
			{
				flag = true;
			}
			m_tracerDistanceScaleFactor = Mathf.MoveTowards(m_tracerDistanceScaleFactor, 1f, Time.deltaTime * 4f * FlightVelocityMultiplier * m_debugFlightVelGlobal);
			if (flag)
			{
				Vector3 forward = position - m_lastPoint;
				base.transform.rotation = Quaternion.LookRotation(forward);
				if (m_hasTracer)
				{
					float magnitude = forward.magnitude;
					Vector3 localScale = new Vector3(TracerWidthMultiplier * 0.1f * m_tracerDistanceScaleFactor, TracerWidthMultiplier * 0.1f * m_tracerDistanceScaleFactor, magnitude * TracerLengthMultiplier);
					tracer.localScale = localScale;
				}
			}
			if (m_lastPoint != position)
			{
				m_lastPoint = position;
			}
			if (!m_isMoving)
			{
				TickDownToDeath();
				return;
			}
			float deltaTime = Time.deltaTime;
			UpdateVelocity(deltaTime);
			MoveBullet(deltaTime);
			if (Trail != null)
			{
				if (newTrailTick > 0)
				{
					newTrailTick--;
					return;
				}
				Trail.AddPosition(base.transform.position);
				newTrailTick = Mathf.RoundToInt(m_distanceTraveled / 100f);
			}
		}

		private void UpdateVelocity(float t)
		{
			if (m_velocity.magnitude < 0.1f || base.transform.position.y < -350f)
			{
				m_isMoving = false;
				return;
			}
			m_velocity += Vector3.down * m_gravMag * t * GravityMultiplier;
			float num = 1.225f * AirDragMultiplier;
			if (m_isTumbling)
			{
				num *= 5f;
			}
			m_velocity = ApplyDrag(m_velocity, num, t);
		}

		private void MoveBullet(float t)
		{
			if ((m_velocity.y < 0f && DeletesOnStraightDown && new Vector3(m_velocity.x, 0f, m_velocity.z).magnitude < 0.1f) || base.transform.position.y < -350f)
			{
				m_isMoving = false;
				return;
			}
			Vector3 vector = base.transform.position;
			if (m_isInReferenceTransform && m_transReference != null)
			{
				vector = m_transReference.TransformPoint(m_localReferencePoint);
			}
			m_isInReferenceTransform = false;
			m_transReference = null;
			Vector3 normalized = m_velocity.normalized;
			float magnitude = m_velocity.magnitude;
			float num = magnitude * t * FlightVelocityMultiplier * m_debugFlightVelGlobal;
			if (needsSecondCast)
			{
				num = Mathf.Clamp(num - distMoved, 0f, num);
			}
			needsSecondCast = false;
			if (!Physics.Raycast(vector, normalized, out m_hit, num, LM, QueryTriggerInteraction.Collide))
			{
				base.transform.position = vector + m_velocity * t * FlightVelocityMultiplier * m_debugFlightVelGlobal;
				if (!m_hasPlayedWhoosh && Source_IFF != GM.CurrentPlayerBody.GetPlayerIFF())
				{
					Vector3 closestValidPoint = GetClosestValidPoint(vector, base.transform.position, m_cachedHead.position);
					float num2 = Vector3.Distance(closestValidPoint, m_cachedHead.position);
					if (num2 < 3f)
					{
						float volumeMult = Mathf.Lerp(1f, 0.2f, Mathf.Clamp(num2 - 1f, 0f, 2f) * 0.5f);
						Vector3 normalized2 = (m_cachedHead.position - vector).normalized;
						if (Vector3.Dot(normalized2, normalized) > 0f)
						{
							m_hasPlayedWhoosh = true;
							SM.PlayBulletImpactHit(BulletImpactSoundType.ZWhooshes, closestValidPoint, volumeMult, 1f);
						}
					}
				}
				if (!(m_velocity.normalized != Vector3.zero))
				{
				}
			}
			else
			{
				m_isTumbling = true;
				if (m_lastColliderHit != null && m_hit.collider == m_lastColliderHit)
				{
					needsSecondCast = true;
					distMoved = m_hit.distance;
					base.transform.position = m_hit.point - m_hit.normal * 0.001f;
					MoveBullet(t);
					return;
				}
				bool flag = false;
				if (m_hit.collider.attachedRigidbody != null)
				{
					flag = true;
				}
				m_lastColliderHit = m_hit.collider;
				IFVRDamageable iFVRDamageable = null;
				bool flag2 = false;
				iFVRDamageable = m_hit.collider.transform.gameObject.GetComponent<IFVRDamageable>();
				if (iFVRDamageable == null && flag)
				{
					iFVRDamageable = m_hit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
				}
				if (iFVRDamageable != null)
				{
					flag2 = true;
				}
				if (DoesIgniteOnHit && UnityEngine.Random.Range(0f, 1f) < IgnitionChance)
				{
					FVRIgnitable component = m_hit.collider.transform.gameObject.GetComponent<FVRIgnitable>();
					if (component != null)
					{
						FXM.Ignite(component, 1f);
					}
					else if (flag)
					{
						component = m_hit.collider.attachedRigidbody.GetComponent<FVRIgnitable>();
						if (component != null)
						{
							FXM.Ignite(component, 1f);
						}
					}
				}
				PMat pMat = null;
				MatDef matDef = null;
				bool flag3 = false;
				pMat = m_hit.collider.transform.gameObject.GetComponent<PMat>();
				if (pMat == null && flag)
				{
					pMat = m_hit.collider.attachedRigidbody.gameObject.GetComponent<PMat>();
				}
				if (pMat != null && pMat.MatDef != null)
				{
					flag3 = true;
				}
				Rigidbody rigidbody = null;
				bool flag4 = false;
				if (flag)
				{
					rigidbody = m_hit.collider.attachedRigidbody;
					flag4 = true;
					m_isInReferenceTransform = true;
					m_transReference = m_hit.collider.attachedRigidbody.gameObject.transform;
				}
				matDef = (flag3 ? pMat.MatDef : PM.DefaultMatDef);
				if (GeneratesImpactSound && GM.Options.SimulationOptions.HitSoundMode == SimulationOptions.HitSounds.Enabled)
				{
					float pitchmult = 0.8f;
					float num3 = 1f;
					if (ImpactFXMagnitude == ImpactEffectMagnitude.Tiny || ImpactFXMagnitude == ImpactEffectMagnitude.Small)
					{
						num3 = 0.4f;
						pitchmult = 1.3f;
					}
					else if (ImpactFXMagnitude == ImpactEffectMagnitude.Medium)
					{
						num3 = 0.7f;
						pitchmult = 1f;
					}
					num3 *= Mathf.InverseLerp(0f, m_initialMuzzleVelocity, magnitude);
					SM.PlayBulletImpactHit(matDef.BulletImpactSound, m_hit.point, 25f, num3, pitchmult);
				}
				if (!flag && GeneratesImpactDecals && GM.Options.SimulationOptions.HitDecalMode == SimulationOptions.HitDecals.Enabled)
				{
					BulletHoleDecalType bulletHoleDecalType = matDef.BulletHoleType;
					if (bulletHoleDecalType != 0 && BulletHoleDecalOverride != 0)
					{
						bulletHoleDecalType = BulletHoleDecalOverride;
					}
					float damageSize = Mathf.Clamp(Dimensions.x, 0.0001f, 0.02f);
					if (bulletHoleDecalType != 0)
					{
						FXM.SpawnBulletDecal(bulletHoleDecalType, m_hit.point, m_hit.normal, damageSize);
					}
				}
				BallisticMatSeries matSeries = PM.GetMatSeries(matDef.BallisticType, ProjType);
				float ricochetLimit = matSeries.RicochetLimit;
				float num4 = Vector3.Angle(normalized, -m_hit.normal);
				bool flag5 = false;
				bool flag6 = false;
				bool flag7 = false;
				bool flag8 = false;
				float num5 = 0.5f * Mass * Mathf.Pow(magnitude, 2f);
				float num6 = num5 / FrontArea * 0.01f / 140f;
				float num7 = 0.5f * Mass * Mathf.Pow(magnitude, 2f);
				float num8 = 0f;
				float num9 = 0f;
				float num10 = 0f;
				Vector3 vector2 = vector;
				Vector3 vector3 = normalized;
				Vector3 velocity = m_velocity;
				if (num4 >= ricochetLimit)
				{
					flag5 = true;
				}
				float a = 1f - num4 / 90f;
				a = Mathf.Max(a, matSeries.MinAngularAbsord);
				float num11 = Mathf.Lerp(0f, matSeries.Absorption, a);
				float num12 = num6 * a;
				if (num12 > matSeries.PenThreshold && !IsDisabledOnFirstImpact)
				{
					flag6 = true;
					float num13 = matSeries.PenThreshold / num12;
					float num14 = Mathf.Clamp(num13 + num11, 0f, 1f);
					num8 = num14 * num7;
					num9 = num7 - num8;
					num10 = num14 * num6;
				}
				else if (num8 > matSeries.ShatterThreshold || IsDisabledOnFirstImpact)
				{
					flag7 = true;
					num8 = num7;
					num9 = 0f;
					num10 = num6;
				}
				else
				{
					num8 = num7 * a;
					num9 = num7 - num8;
					num10 = num6 * a;
					flag5 = true;
					float num15 = num8 / num7;
					float num16 = Mathf.Clamp(num15 + num11, 0f, 1f);
					num8 = num16 * num7;
					num9 = num7 - num8;
					num10 = num6 * num16;
				}
				if (m_usesSubmunitions)
				{
					flag7 = true;
					flag8 = true;
				}
				Vector3 vector4 = vector3;
				Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
				if (flag5 && !flag7)
				{
					vector2 = m_hit.point + m_hit.normal * 0.001f;
					vector3 = Vector3.Lerp(Vector3.Reflect(normalized, m_hit.normal), onUnitSphere, matSeries.Roughness);
					vector3 = Vector3.Lerp(Vector3.ProjectOnPlane(vector3, m_hit.normal), vector3, Mathf.Clamp(a * a, 0.1f, 1f));
					vector4 = vector3;
				}
				else if (flag6 && !flag7)
				{
					if (matSeries.StopsOnPen)
					{
						num8 = num7;
						num9 = 0f;
						num10 = num6;
						flag8 = true;
					}
					vector2 = m_hit.point - m_hit.normal * 0.001f;
					vector3 = Vector3.Lerp(normalized, onUnitSphere, matSeries.Roughness * a);
					if (Vector3.Angle(vector3, m_hit.normal) < 90f)
					{
						vector3 = Vector3.Reflect(vector3, m_hit.normal);
					}
					if (matSeries.DownGradesOnPen)
					{
						ProjType = matSeries.DownGradesTo;
					}
				}
				else
				{
					vector2 = m_hit.point;
					vector4 = Vector3.Lerp(Vector3.Reflect(normalized, m_hit.normal), onUnitSphere, matSeries.Roughness);
					vector4 = Vector3.Lerp(Vector3.ProjectOnPlane(vector3, m_hit.normal), vector3, Mathf.Clamp(a * a, 0.1f, 1f));
				}
				float num17 = 0f;
				if (!flag8 && !flag7 && num9 > 0f)
				{
					num17 = Mathf.Sqrt(2f * num9) / Mathf.Sqrt(Mass);
					velocity = vector3 * num17;
				}
				base.transform.position = vector2;
				if (m_isInReferenceTransform)
				{
					m_localReferencePoint = m_transReference.InverseTransformPoint(vector2);
				}
				m_velocity = velocity;
				if (flag8 || flag7 || num17 <= 0f || num9 <= 0f)
				{
					m_isMoving = false;
					m_velocity = Vector3.zero;
					if (Trail != null)
					{
						Trail.AddPosition(base.transform.position);
					}
				}
				float f = num8;
				float num18 = 0f;
				if (matSeries.MaterialType == MatBallisticType.MeatSolid || matSeries.MaterialType == MatBallisticType.MeatThick)
				{
					num18 = Mathf.Clamp(num9, 0f, matSeries.PenThreshold * 2f * a);
				}
				if (flag2)
				{
					Damage damage = new Damage();
					damage.Class = Damage.DamageClass.Projectile;
					damage.Dam_TotalKinetic = num8;
					if (flag6)
					{
						switch (ProjType)
						{
						case BallisticProjectileType.Penetrator:
							damage.Dam_Piercing = num10 * 0.9f;
							damage.Dam_Blunt = num8 * 0.1f;
							break;
						case BallisticProjectileType.FMJ:
							damage.Dam_Piercing = num10 * 0.7f;
							damage.Dam_Blunt = num8 * 0.3f;
							break;
						case BallisticProjectileType.Expanding:
							damage.Dam_Piercing = num10 * 0.5f;
							damage.Dam_Blunt = num8 * 0.5f;
							break;
						case BallisticProjectileType.Slug:
							damage.Dam_Piercing = num10 * 0.3f;
							damage.Dam_Blunt = num8 * 0.7f;
							break;
						}
						damage.Dam_Piercing += num18 * 1f;
					}
					else if (flag7)
					{
						switch (ProjType)
						{
						case BallisticProjectileType.Penetrator:
							damage.Dam_Piercing = num10 * 0.5f;
							damage.Dam_Blunt = num8 * 0.5f;
							break;
						case BallisticProjectileType.FMJ:
							damage.Dam_Piercing = num10 * 0.4f;
							damage.Dam_Blunt = num8 * 0.6f;
							break;
						case BallisticProjectileType.Expanding:
							damage.Dam_Piercing = num10 * 0.3f;
							damage.Dam_Blunt = num8 * 0.7f;
							break;
						case BallisticProjectileType.Slug:
							damage.Dam_Piercing = num10 * 0.1f;
							damage.Dam_Blunt = num8 * 0.9f;
							break;
						}
					}
					else if (flag5)
					{
						damage.Dam_Blunt = num8;
					}
					if (DoesIgniteOnHit)
					{
						damage.Dam_Thermal = 50f;
					}
					if (Source_IFF == GM.CurrentPlayerBody.GetPlayerIFF() && (GM.CurrentPlayerBody.isDamPowerUp || GM.CurrentPlayerBody.IsDamPowerDown))
					{
						float damageMult = GM.CurrentPlayerBody.GetDamageMult();
						damage.Dam_Piercing *= damageMult;
						damage.Dam_Blunt *= damageMult;
						damage.Dam_Cutting *= damageMult;
						damage.Dam_TotalKinetic *= damageMult;
						damage.Dam_Blinding *= damageMult;
						damage.Dam_Chilling *= damageMult;
						damage.Dam_EMP *= damageMult;
						damage.Dam_Stunning *= damageMult;
						damage.Dam_Thermal *= damageMult;
						damage.Dam_TotalEnergetic *= damageMult;
					}
					damage.point = m_hit.point;
					damage.hitNormal = m_hit.normal;
					damage.strikeDir = normalized;
					damage.damageSize = Dimensions.x;
					damage.Source_IFF = Source_IFF;
					f = damage.Dam_Blunt;
					iFVRDamageable.Damage(damage);
				}
				if (flag4)
				{
					rigidbody.AddForceAtPosition(normalized * Mathf.Sqrt(f), m_hit.point, ForceMode.Force);
				}
				BallisticImpactEffectType mat = matDef.ImpactEffectType;
				if (ImpactEffectTypeOverride != BallisticImpactEffectType.None)
				{
					mat = ImpactEffectTypeOverride;
				}
				if (flag7 || flag5)
				{
					if (magnitude > 100f)
					{
						FXM.SpawnImpactEffect(m_hit.point, vector4, (int)mat, ImpactFXMagnitude, forwardBack: false);
					}
				}
				else if (flag6)
				{
					if (magnitude > 100f)
					{
						if (flag8 || num17 == 0f)
						{
							FXM.SpawnImpactEffect(m_hit.point, -normalized, (int)mat, ImpactFXMagnitude, forwardBack: false);
						}
						else
						{
							FXM.SpawnImpactEffect(m_hit.point, -normalized, (int)mat, ImpactFXMagnitude, forwardBack: true);
						}
					}
				}
				else
				{
					Debug.Log("No effect played, that's weird");
				}
				if (GeneratesSuppressionEvent)
				{
					GeneratesSuppressionEvent = false;
					GM.CurrentSceneSettings.OnSuppressingEvent(vector2, normalized, Source_IFF, SuppressionIntensity, SuppressionRange);
				}
				FireSubmunitions(vector4, normalized, m_hit.point, m_velocity.magnitude);
				m_stallFrames = UnityEngine.Random.Range(0, 3);
			}
			m_distanceTraveled += Vector3.Distance(base.transform.position, vector);
		}

		private void FireSubmunitions(Vector3 shatterRicochetDir, Vector3 velNorm, Vector3 hitPoint, float VelocityOverride)
		{
			if (!m_usesSubmunitions || m_hasFiredSubmunitions)
			{
				return;
			}
			m_hasFiredSubmunitions = true;
			for (int i = 0; i < Submunitions.Count; i++)
			{
				Submunition submunition = Submunitions[i];
				Vector3 vector = shatterRicochetDir;
				Vector3 position = hitPoint;
				for (int j = 0; j < submunition.NumToSpawn; j++)
				{
					GameObject original = submunition.Prefabs[UnityEngine.Random.Range(0, submunition.Prefabs.Count)];
					float num = UnityEngine.Random.Range(submunition.Speed.x, submunition.Speed.y);
					switch (submunition.Trajectory)
					{
					case Submunition.SubmunitionTrajectoryType.Random:
						vector = UnityEngine.Random.onUnitSphere;
						break;
					case Submunition.SubmunitionTrajectoryType.Forwards:
						vector = velNorm;
						break;
					case Submunition.SubmunitionTrajectoryType.ForwardsCone:
						vector = Vector3.Lerp(UnityEngine.Random.onUnitSphere, velNorm, submunition.ConeLerp);
						break;
					case Submunition.SubmunitionTrajectoryType.Backwards:
						vector = Vector3.Lerp(-velNorm, shatterRicochetDir, 0.5f);
						break;
					}
					switch (submunition.SpawnLogic)
					{
					case Submunition.SubmunitionSpawnLogic.Inside:
						position -= m_hit.normal * 0.001f;
						break;
					case Submunition.SubmunitionSpawnLogic.Outside:
						position += m_hit.normal * 0.001f;
						break;
					}
					GameObject gameObject = UnityEngine.Object.Instantiate(original, position, Quaternion.LookRotation(vector));
					switch (submunition.Type)
					{
					case Submunition.SubmunitionType.Projectile:
					{
						BallisticProjectile component4 = gameObject.GetComponent<BallisticProjectile>();
						component4.Source_IFF = Source_IFF;
						component4.Fire(num, gameObject.transform.forward, null);
						break;
					}
					case Submunition.SubmunitionType.Rigidbody:
						gameObject.GetComponent<Rigidbody>().velocity = vector * num;
						break;
					case Submunition.SubmunitionType.StickyBomb:
						gameObject.GetComponent<Rigidbody>().velocity = vector * VelocityOverride;
						if (PassesFirearmReferenceToSubmunitions)
						{
							MF2_StickyBomb component3 = gameObject.GetComponent<MF2_StickyBomb>();
							component3.SetIFF(Source_IFF);
							if (component3 != null && tempFA != null && (tempFA as ClosedBoltWeapon).UsesStickyDetonation)
							{
								(tempFA as ClosedBoltWeapon).RegisterStickyBomb(component3);
							}
						}
						break;
					case Submunition.SubmunitionType.MeleeThrown:
						gameObject.GetComponent<Rigidbody>().velocity = vector * num;
						break;
					case Submunition.SubmunitionType.GameObject:
					{
						Explosion component = gameObject.GetComponent<Explosion>();
						if (component != null)
						{
							component.IFF = Source_IFF;
						}
						ExplosionSound component2 = gameObject.GetComponent<ExplosionSound>();
						if (component2 != null)
						{
							component2.IFF = Source_IFF;
						}
						break;
					}
					case Submunition.SubmunitionType.Demonade:
						gameObject.GetComponent<Rigidbody>().velocity = vector * num;
						gameObject.GetComponent<MF2_Demonade>().SetIFF(Source_IFF);
						break;
					}
				}
			}
		}

		public Vector3 GetClosestValidPoint(Vector3 vA, Vector3 vB, Vector3 vPoint)
		{
			Vector3 rhs = vPoint - vA;
			Vector3 normalized = (vB - vA).normalized;
			float num = Vector3.Distance(vA, vB);
			float num2 = Vector3.Dot(normalized, rhs);
			if (num2 <= 0f)
			{
				return vA;
			}
			if (num2 >= num)
			{
				return vB;
			}
			Vector3 vector = normalized * num2;
			return vA + vector;
		}

		[ContextMenu("Migrate")]
		public void Migrate()
		{
			FVRProjectile component = GetComponent<FVRProjectile>();
			Mass = component.Mass;
			Dimensions = component.Dimensions;
			MuzzleVelocityBase = component.MuzzleVelocity;
			ImpactFXMagnitude = component.ImpactFXMagnitude;
			MaxRange = component.MaxRange;
			MaxRangeRandom = component.MaxRangeRandom;
			m_dieTimerMax = component.m_dieTimerMax;
			tracer = component.tracer;
			TracerRenderer = component.m_tracerRenderer;
			BulletRenderer = component.m_bulletRenderer;
			ExtraDisplay = component.ExtraDisplay;
			TracerLengthMultiplier = component.TracerLengthMultiplier;
			TracerWidthMultiplier = component.TracerWidthMultiplier;
			UsesTrails = component.UsesTrails;
			TrailStartColor = component.TrailStartColor;
		}

		[ContextMenu("AreaCalc")]
		public void AreaCalc()
		{
			float num = (FrontArea = (float)Math.PI * Mathf.Pow(Dimensions.x * 0.5f, 2f));
			KEPerSquareMeterBase = (KETotalForHit = 0.5f * Mass * Mathf.Pow(MuzzleVelocityBase, 2f)) / num * 0.01f / 140f;
		}
	}
}
