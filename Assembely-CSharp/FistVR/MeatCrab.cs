using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
	public class MeatCrab : FVRPhysicalObject, IFVRDamageable
	{
		public enum MeatCrabState
		{
			Still,
			Hopping,
			Lunging,
			Ballistic,
			Attached
		}

		[Header("Refs")]
		public AIEntity E;

		public AITargetPrioritySystem Priority;

		private bool m_hasPriority;

		public Transform GroundPoint;

		public Transform MeatMesh;

		public Transform[] Spikes;

		public Transform Drill;

		public float Radius;

		public float Height;

		public float MeatMeshOGScale = 0.58f;

		public Vector2 DrillExtents;

		public Vector2 SpikeXRotations;

		private float[] m_initialSpikeRots = new float[3];

		private float m_spikeLerp;

		private Vector3 m_initialDrillLocalPos;

		private float m_drillRot;

		[Header("Decision Params")]
		public Vector2 ThinkTimeRange;

		public MeatCrabState State;

		private float m_thinkingTick = 1f;

		private Vector3 m_navPoint;

		private NavMeshHit m_hit;

		private Vector3 m_targPos;

		private Quaternion m_targRot;

		private bool m_controlledMovement = true;

		[Header("Hopping Params")]
		public float HopSpeed;

		public Vector2 HopDistanceRange;

		public Vector2 HopHeightRange;

		public AnimationCurve HopHeightCurve;

		private float m_hopLerp;

		private Vector3 m_hopFrom;

		private Vector3 m_hopTo;

		private Vector3 m_hopForwardFrom;

		private Vector3 m_hopForwardTo;

		private float m_speedMult = 1f;

		private float m_heightMult = 1f;

		[Header("Lunging Params")]
		public float LungeRange;

		public float LungeVelocity;

		public LayerMask LM_LungeAttack;

		public float LungeAttackAngle = 50f;

		[Header("Ballistic Params")]
		public float RecoveryCheckTime = 0.1f;

		private float m_recoveryCheckTick = 0.5f;

		public LayerMask LM_Recovery;

		private RaycastHit m_rayHit;

		public float DownwardRecoveryCheckDistance = 0.2f;

		private float m_recoverySampleDistance = 0.5f;

		private float m_timeWhileBallistic = 1f;

		[Header("AttachedParams")]
		public float DetachForce = 4f;

		private Transform m_attachTransform;

		private Vector3 m_attachLocalPos;

		private Vector3 m_attachLocalFace;

		private Vector3 m_attachLocalUp;

		private float m_timeAttached;

		protected float AttachedRotationMultiplier2 = 60f;

		protected float AttachedPositionMultiplier2 = 9000f;

		protected float AttachedRotationFudge2 = 1000f;

		protected float AttachedPositionFudge2 = 1000f;

		public AudioEvent AudEvent_HitGround;

		public AudioEvent AudEvent_Leap;

		public AudioEvent AudEvent_HitTarget;

		public AudioSource AudSource_DrillSound;

		private float m_drillLerp;

		[Header("DamageStuff")]
		public float PointsLife = 4000f;

		public float LungeDamage = 250f;

		public bool DoesPoisonAttack;

		public GameObject DeathFX;

		private bool m_dead;

		protected override void Start()
		{
			base.Start();
			Init();
			if (Priority != null)
			{
				m_hasPriority = true;
				Priority.Init(E, 3, 5f, 3f);
			}
		}

		private void Init()
		{
			E.AIEventReceiveEvent += EventReceive;
			InitiateThinking(GroundPoint.position, GroundPoint.forward);
			for (int i = 0; i < m_initialSpikeRots.Length; i++)
			{
				m_initialSpikeRots[i] = Spikes[i].localEulerAngles.y;
			}
			m_initialDrillLocalPos = Drill.localPosition;
			if (GM.TNH_Manager != null)
			{
				GM.TNH_Manager.AddToMiscEnemies(base.gameObject);
			}
		}

		public void EventReceive(AIEvent e)
		{
			if ((!e.IsEntity || e.Entity.IFFCode != E.IFFCode) && e.Type == AIEvent.AIEType.Visual && m_hasPriority)
			{
				Priority.ProcessEvent(e);
			}
		}

		public override bool IsInteractable()
		{
			if (State == MeatCrabState.Still)
			{
				return true;
			}
			return false;
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (base.IsHeld)
			{
				State = MeatCrabState.Ballistic;
				m_controlledMovement = false;
			}
			Crabdate();
			if (State == MeatCrabState.Ballistic || State == MeatCrabState.Hopping || State == MeatCrabState.Lunging)
			{
				DistantGrabbable = true;
			}
			else
			{
				DistantGrabbable = false;
			}
		}

		protected override void FVRFixedUpdate()
		{
			CrabMover();
			base.FVRFixedUpdate();
		}

		private void Crabdate()
		{
			Priority.Compute();
			switch (State)
			{
			case MeatCrabState.Still:
				Crabdate_Still();
				break;
			case MeatCrabState.Hopping:
				Crabdate_Hopping();
				break;
			case MeatCrabState.Lunging:
				Crabdate_Lunging();
				break;
			case MeatCrabState.Ballistic:
				Crabdate_Ballistic();
				break;
			case MeatCrabState.Attached:
				Crabdate_Attached();
				break;
			}
		}

		private void InitiateThinking(Vector3 navPoint, Vector3 forward)
		{
			m_controlledMovement = true;
			m_navPoint = navPoint;
			State = MeatCrabState.Still;
			m_targPos = navPoint + Vector3.up * Height;
			m_targRot = Quaternion.LookRotation(forward, Vector3.up);
			m_thinkingTick = Random.Range(ThinkTimeRange.x, ThinkTimeRange.y);
		}

		private void InitiateHop(Vector3 posToHopTo, Vector3 newForwardDir)
		{
			m_controlledMovement = true;
			State = MeatCrabState.Hopping;
			m_hopLerp = 0f;
			m_hopFrom = m_navPoint;
			m_hopTo = posToHopTo;
			m_hopForwardFrom = Vector3.ProjectOnPlane(base.transform.forward, Vector3.up);
			m_hopForwardTo = newForwardDir;
		}

		private void InitiateLunge(Vector3 pointToLungeAt)
		{
			m_controlledMovement = false;
			State = MeatCrabState.Lunging;
			if (Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.transform.position) < 10f)
			{
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_Leap, base.transform.position);
			}
			Vector3 normalized = (pointToLungeAt - base.transform.position).normalized;
			base.RootRigidbody.velocity = LungeVelocity * normalized;
		}

		private void InitiateBallistic()
		{
			m_controlledMovement = false;
			base.RootRigidbody.mass = 1f;
			m_timeWhileBallistic = Random.Range(0f, 1f);
			State = MeatCrabState.Ballistic;
		}

		private void InitiateAttached(Transform t, Vector3 pos, Vector3 facingDir, Vector3 upDir)
		{
			m_controlledMovement = true;
			State = MeatCrabState.Attached;
			base.RootRigidbody.mass = 0.1f;
			m_attachTransform = t;
			m_attachLocalPos = t.InverseTransformPoint(pos);
			m_attachLocalFace = t.InverseTransformDirection(facingDir);
			m_attachLocalUp = t.InverseTransformDirection(upDir);
			m_timeAttached = 0f;
		}

		private void Crabdate_Still()
		{
			m_thinkingTick -= Time.deltaTime;
			if (m_thinkingTick > 0f)
			{
				return;
			}
			bool flag = true;
			if (Priority.HasFreshTarget())
			{
				Vector3 targetPoint = Priority.GetTargetPoint();
				Vector3 vector = Vector3.ProjectOnPlane(targetPoint - base.transform.position, Vector3.up);
				float magnitude = vector.magnitude;
				vector.Normalize();
				if (magnitude > LungeRange)
				{
					float num = Random.Range(HopDistanceRange.x, HopDistanceRange.y);
					m_speedMult = 1f + num;
					m_heightMult = num / HopDistanceRange.y;
					Vector3 posToHopTo = m_navPoint + vector * num;
					Vector3 targetPosition = m_navPoint + vector * num;
					if (!NavMesh.Raycast(m_navPoint, targetPosition, out m_hit, -1))
					{
						flag = false;
						InitiateHop(posToHopTo, vector);
					}
				}
				else
				{
					flag = false;
					InitiateLunge(targetPoint + Vector3.up * Random.Range(0.1f, 0.7f));
				}
			}
			if (flag)
			{
				Vector3 normalized = Vector3.ProjectOnPlane(Vector3.Slerp(base.transform.forward, Random.onUnitSphere, 0.25f), Vector3.up).normalized;
				float num2 = Random.Range(HopDistanceRange.x, HopDistanceRange.y);
				m_speedMult = 1f + num2;
				m_heightMult = num2 / HopDistanceRange.y;
				Vector3 posToHopTo2 = m_navPoint + normalized * num2;
				Vector3 targetPosition2 = m_navPoint + normalized * num2;
				if (NavMesh.Raycast(m_navPoint, targetPosition2, out m_hit, -1))
				{
					m_speedMult = 1f;
					m_heightMult = 0.25f;
					InitiateHop(m_navPoint, Vector3.ProjectOnPlane(Random.onUnitSphere, Vector3.up).normalized);
				}
				else
				{
					InitiateHop(posToHopTo2, normalized);
				}
			}
		}

		private void Crabdate_Hopping()
		{
			m_hopLerp += HopSpeed * m_speedMult * Time.deltaTime;
			Vector3 vector = Vector3.Lerp(m_hopFrom, m_hopTo, m_hopLerp);
			float num = HopHeightCurve.Evaluate(m_hopLerp);
			vector.y += num * HopHeightRange.x * m_heightMult;
			MeatMesh.localScale = new Vector3(MeatMeshOGScale, MeatMeshOGScale + num * 0.1f, MeatMeshOGScale);
			Quaternion targRot = Quaternion.LookRotation(Vector3.Slerp(m_hopForwardFrom, m_hopForwardTo, m_hopLerp), Vector3.up);
			m_targPos = vector + Vector3.up * Height;
			m_targRot = targRot;
			if (m_hopLerp >= 1f)
			{
				if (Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.transform.position) < 10f)
				{
					SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_HitGround, base.transform.position);
				}
				InitiateThinking(m_hopTo, m_hopForwardTo);
			}
		}

		private void Crabdate_Lunging()
		{
			Vector3 position = base.transform.position;
			Vector3 normalized = base.RootRigidbody.velocity.normalized;
			float maxDistance = base.RootRigidbody.velocity.magnitude * Time.deltaTime * 2f;
			if (!Physics.SphereCast(new Ray(position, normalized), Radius, out m_rayHit, maxDistance, LM_LungeAttack, QueryTriggerInteraction.Collide))
			{
				return;
			}
			IFVRDamageable iFVRDamageable = null;
			iFVRDamageable = m_rayHit.collider.transform.gameObject.GetComponent<IFVRDamageable>();
			if (iFVRDamageable == null && m_rayHit.collider.attachedRigidbody != null)
			{
				iFVRDamageable = m_rayHit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
			}
			if (iFVRDamageable != null)
			{
				Damage damage = new Damage();
				damage.Class = FistVR.Damage.DamageClass.Melee;
				damage.hitNormal = base.transform.up;
				damage.Dam_TotalKinetic = LungeDamage;
				damage.Dam_Blunt = LungeDamage;
				damage.Dam_Stunning = 3f;
				damage.strikeDir = m_rayHit.normal;
				damage.point = m_rayHit.point;
				damage.Source_IFF = E.IFFCode;
				damage.damageSize = 0.1f;
				iFVRDamageable.Damage(damage);
			}
			bool flag = false;
			AIEntity component = m_rayHit.collider.attachedRigidbody.GetComponent<AIEntity>();
			AIEntity component2 = m_rayHit.collider.GetComponent<AIEntity>();
			AIEntity aIEntity = null;
			if (component2 != null)
			{
				aIEntity = component2;
			}
			else if (component != null)
			{
				aIEntity = component;
			}
			if (aIEntity == null && m_rayHit.collider.attachedRigidbody != null)
			{
				FVRPlayerHitbox component3 = m_rayHit.collider.attachedRigidbody.gameObject.GetComponent<FVRPlayerHitbox>();
				if (component3 != null)
				{
					aIEntity = component3.MyE;
					flag = true;
					if (DoesPoisonAttack)
					{
						GM.CurrentPlayerBody.ActivatePower(PowerupType.Invincibility, PowerUpIntensity.Medium, PowerUpDuration.Short, isPuke: false, isInverted: true);
					}
				}
				if (aIEntity == null)
				{
					SosigLink component4 = m_rayHit.collider.attachedRigidbody.gameObject.GetComponent<SosigLink>();
					if (component4 != null)
					{
						aIEntity = component4.S.E;
						component4.S.ActivatePower(PowerupType.Invincibility, PowerUpIntensity.Medium, PowerUpDuration.Short, isPuke: false, isInverted: true);
					}
				}
			}
			if (Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.transform.position) < 10f)
			{
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_HitTarget, base.transform.position);
			}
			if (aIEntity != null && m_rayHit.collider.attachedRigidbody != null && aIEntity.IFFCode != E.IFFCode)
			{
				Vector3 threatFacing = aIEntity.GetThreatFacing();
				if (Vector3.Angle(threatFacing, -normalized) < LungeAttackAngle || !flag)
				{
					Vector3 vector = m_rayHit.point + normalized * (0f - Radius);
					Vector3 normalized2 = (vector - m_rayHit.collider.transform.position).normalized;
					Vector3 rhs = Vector3.Cross(-Vector3.up, normalized2);
					Vector3 facingDir = Vector3.Cross(normalized2, rhs);
					Vector3 up = aIEntity.transform.up;
					Vector3 vector2 = vector - m_rayHit.collider.transform.position;
					InitiateAttached(m_rayHit.collider.transform, vector, facingDir, normalized2);
					return;
				}
			}
			InitiateBallistic();
			base.RootRigidbody.velocity = Vector3.Slerp(-normalized, Random.onUnitSphere, 0.3f) * Random.Range(LungeVelocity * 0.3f, LungeVelocity * 0.7f);
		}

		private void Crabdate_Ballistic()
		{
			if (m_spikeLerp > 0f)
			{
				m_spikeLerp -= Time.deltaTime * 10f;
				for (int i = 0; i < Spikes.Length; i++)
				{
					Spikes[i].localEulerAngles = new Vector3(Mathf.Lerp(SpikeXRotations.x, SpikeXRotations.y, m_spikeLerp), m_initialSpikeRots[i], 0f);
				}
			}
			if (m_drillLerp > 0f)
			{
				m_drillLerp -= Time.deltaTime * 5f;
				AudSource_DrillSound.volume = m_drillLerp * 0.5f;
				AudSource_DrillSound.pitch = Mathf.Lerp(0.5f, 1f, m_drillLerp);
				m_drillRot += Time.deltaTime * m_drillLerp * 4000f;
				m_drillRot = Mathf.Repeat(m_drillRot, 360f);
				Drill.localEulerAngles = new Vector3(0f, m_drillRot, 0f);
				Drill.localPosition = new Vector3(m_initialDrillLocalPos.x, Mathf.Lerp(DrillExtents.x, DrillExtents.y, m_drillLerp), m_initialDrillLocalPos.z);
			}
			else if (AudSource_DrillSound.isPlaying)
			{
				AudSource_DrillSound.Stop();
			}
			if (m_timeWhileBallistic < 2f)
			{
				m_timeWhileBallistic += Time.deltaTime;
			}
			if (m_recoveryCheckTick > 0f)
			{
				m_recoveryCheckTick -= Time.deltaTime;
				return;
			}
			m_recoveryCheckTick = RecoveryCheckTime;
			if (m_timeWhileBallistic < 1.5f || !Physics.Raycast(base.transform.position, Vector3.down, out m_rayHit, DownwardRecoveryCheckDistance, LM_Recovery, QueryTriggerInteraction.Ignore))
			{
				return;
			}
			Vector3 point = m_rayHit.point;
			if (NavMesh.SamplePosition(point, out m_hit, m_recoverySampleDistance, -1))
			{
				InitiateThinking(m_hit.position, Vector3.ProjectOnPlane(Random.onUnitSphere, Vector3.up));
				return;
			}
			m_recoverySampleDistance += 0.1f;
			if (m_recoverySampleDistance > 2f)
			{
				Die();
			}
		}

		private void Crabdate_Attached()
		{
			m_timeAttached += Time.deltaTime;
			if ((double)m_timeAttached > 8.0)
			{
				InitiateBallistic();
			}
			if (m_spikeLerp < 1f)
			{
				m_spikeLerp += Time.deltaTime * 4f;
				for (int i = 0; i < Spikes.Length; i++)
				{
					Spikes[i].localEulerAngles = new Vector3(Mathf.Lerp(SpikeXRotations.x, SpikeXRotations.y, m_spikeLerp), m_initialSpikeRots[i], 0f);
				}
			}
			if (m_drillLerp < 1f)
			{
				m_drillLerp += Time.deltaTime * 0.25f;
				if (!AudSource_DrillSound.isPlaying)
				{
					AudSource_DrillSound.Play();
				}
				AudSource_DrillSound.volume = m_drillLerp * 0.5f;
				AudSource_DrillSound.pitch = Mathf.Lerp(0.5f, 1f, m_drillLerp);
				m_drillRot += Time.deltaTime * m_drillLerp * 4000f;
				m_drillRot = Mathf.Repeat(m_drillRot, 360f);
				Drill.localEulerAngles = new Vector3(0f, m_drillRot, 0f);
				Drill.localPosition = new Vector3(m_initialDrillLocalPos.x, Mathf.Lerp(DrillExtents.x, DrillExtents.y, m_drillLerp), m_initialDrillLocalPos.z);
			}
			if (m_drillLerp > 0.8f && Physics.Raycast(base.transform.position, -base.transform.up, out m_rayHit, 1f, LM_LungeAttack, QueryTriggerInteraction.Collide))
			{
				IFVRDamageable iFVRDamageable = null;
				iFVRDamageable = m_rayHit.collider.transform.gameObject.GetComponent<IFVRDamageable>();
				if (iFVRDamageable == null && m_rayHit.collider.attachedRigidbody != null)
				{
					iFVRDamageable = m_rayHit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
				}
				if (iFVRDamageable != null)
				{
					Damage damage = new Damage();
					damage.Class = FistVR.Damage.DamageClass.Melee;
					damage.hitNormal = base.transform.up;
					damage.Dam_TotalKinetic = 250f;
					damage.Dam_Blunt = 1000f;
					damage.Dam_Stunning = 3f;
					damage.strikeDir = m_rayHit.normal;
					damage.point = m_rayHit.point;
					damage.Source_IFF = E.IFFCode;
					damage.damageSize = 0.1f;
					iFVRDamageable.Damage(damage);
				}
			}
		}

		private void CrabMover()
		{
			if (!m_controlledMovement)
			{
				return;
			}
			Vector3 position = base.transform.position;
			Quaternion rotation = base.transform.rotation;
			Vector3 vector = m_targPos - position;
			Quaternion quaternion = m_targRot * Quaternion.Inverse(rotation);
			if (State == MeatCrabState.Attached)
			{
				if (m_attachTransform == null)
				{
					InitiateBallistic();
					return;
				}
				Vector3 vector2 = m_attachTransform.TransformPoint(m_attachLocalPos);
				Vector3 forward = m_attachTransform.TransformDirection(m_attachLocalFace);
				Vector3 upwards = m_attachTransform.TransformDirection(m_attachLocalUp);
				Quaternion quaternion2 = Quaternion.LookRotation(forward, upwards);
				vector = vector2 - position;
				quaternion = quaternion2 * Quaternion.Inverse(rotation);
			}
			float deltaTime = Time.deltaTime;
			quaternion.ToAngleAxis(out var angle, out var axis);
			if (angle > 180f)
			{
				angle -= 360f;
			}
			if (angle != 0f)
			{
				Vector3 target = deltaTime * angle * axis * AttachedRotationMultiplier2;
				base.RootRigidbody.angularVelocity = Vector3.MoveTowards(base.RootRigidbody.angularVelocity, target, AttachedRotationFudge2 * Time.fixedDeltaTime);
			}
			Vector3 target2 = vector * AttachedPositionMultiplier2 * deltaTime;
			base.RootRigidbody.velocity = Vector3.MoveTowards(base.RootRigidbody.velocity, target2, AttachedPositionFudge2 * deltaTime);
		}

		public override void OnCollisionEnter(Collision col)
		{
			bool flag = false;
			bool flag2 = false;
			if (State == MeatCrabState.Attached && (col.collider.attachedRigidbody == null || col.collider.gameObject.layer == LayerMask.NameToLayer("AgentBody")))
			{
				flag2 = true;
			}
			switch (State)
			{
			case MeatCrabState.Still:
				if (!flag2 && col.collider.attachedRigidbody != null && col.relativeVelocity.magnitude > 3f)
				{
					flag = true;
				}
				break;
			case MeatCrabState.Lunging:
				flag = true;
				break;
			case MeatCrabState.Hopping:
				if (!flag2 && col.collider.attachedRigidbody != null && col.relativeVelocity.magnitude > 1f)
				{
					flag = true;
				}
				break;
			case MeatCrabState.Attached:
				if (!flag2 && col.collider.attachedRigidbody != null && col.relativeVelocity.magnitude > DetachForce)
				{
					flag = true;
				}
				break;
			}
			if (flag)
			{
				InitiateBallistic();
			}
			base.OnCollisionEnter(col);
		}

		public void Damage(Damage d)
		{
			if (d.Dam_TotalKinetic > 50f)
			{
				InitiateBallistic();
				base.RootRigidbody.AddForceAtPosition(d.Dam_TotalKinetic * 0.01f * d.strikeDir, d.point);
			}
			PointsLife -= d.Dam_TotalKinetic;
			PointsLife -= d.Dam_Thermal * 0.2f;
			if (PointsLife < 0f)
			{
				Die();
			}
		}

		private void Die()
		{
			if (!m_dead)
			{
				m_dead = true;
				Object.Instantiate(DeathFX, base.transform.position, Random.rotation);
				Object.Destroy(base.gameObject);
			}
		}
	}
}
