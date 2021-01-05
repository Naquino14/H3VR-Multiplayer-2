using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
	public class wwBotWurst : MonoBehaviour
	{
		public enum BotState
		{
			StandingAround,
			Patrolling,
			Fighting,
			Searching,
			RunAway,
			Dead
		}

		[Header("Core Refs")]
		public wwBotManager Manager;

		public wwBotWurstConfig Config;

		public NavMeshAgent Agent;

		public wwBotWurstNavPointGroup NavPointGroup;

		public wwBotWurstGun[] Guns;

		public List<wwBotWurstModernGun> ModernGuns = new List<wwBotWurstModernGun>();

		public Transform ModernGunMount;

		public wwBotWurstHat Hat;

		public GameObject DropOnDeath;

		private float tick = 4f;

		public Rigidbody RB_Head;

		public Rigidbody RB_Torso;

		public Rigidbody RB_Bottom;

		public wwBotWurstDamageablePiece[] Pieces;

		public Transform Target;

		public LayerMask LM_Sight;

		public LayerMask LM_SmokeDetect;

		public LayerMask LM_IFF;

		public bool CanBeStunned = true;

		private bool m_isStunned;

		private float m_stunTick;

		private bool m_isInSmoke;

		public float Max_LinearSpeed_Walk;

		public float Max_LinearSpeed_Combat;

		public float Max_LinearSpeed_Run;

		public float Acceleration;

		public float Max_AngularSpeed;

		public float MaxViewAngle;

		public float MaxViewDistance;

		public float MaxFiringAngle;

		public float MaxFiringDistance;

		public BotState State = BotState.Patrolling;

		private float m_timeTilNextLookRot = 1f;

		private Vector3 m_targetFacing = Vector3.zero;

		private float m_timeTilPickNewPatrolPoint = 1f;

		private Vector3 m_patrolDestinationPoint = Vector3.zero;

		private Vector3 m_fleeDestinationPoint = Vector3.zero;

		public Vector3 LastPlaceTargetSeen = Vector3.zero;

		private float m_timeSinceTargetSeen = 10f;

		private Vector3 m_combatDestinationPoint = Vector3.zero;

		private bool m_hasSearchPoint;

		private Vector3 m_searchingDestinationPoint = Vector3.zero;

		private float m_atSearchPointTick;

		private bool m_isAggrod;

		private bool m_isPosse;

		private int m_index;

		public bool CanCivGetScared = true;

		private float m_possibleSoundPlayTick = 10f;

		private bool m_canSpeak;

		private float m_senseTick;

		private float standingAroundAggroTime;

		private void Awake()
		{
			Agent.updateRotation = false;
			PickNewFacing();
			Agent.acceleration = Config.Acceleration;
			Pieces[0].ParentAttachingJoint = Pieces[0].gameObject.GetComponent<Joint>();
			Pieces[1].ParentAttachingJoint = Pieces[1].gameObject.GetComponent<Joint>();
			Pieces[1].Child = Pieces[0].transform;
			Pieces[2].Child = Pieces[1].transform;
			Pieces[0].SetIsHead(b: true);
			Pieces[0].SetLife(Config.Life_Head);
			Pieces[1].SetLife(Config.Life_Torso);
			Pieces[2].SetLife(Config.Life_Bottom);
			Max_LinearSpeed_Walk = Config.LinearSpeed_Walk;
			Max_LinearSpeed_Combat = Config.LinearSpeed_Combat;
			Max_LinearSpeed_Run = Config.LinearSpeed_Run;
			Acceleration = Config.Acceleration;
			Max_AngularSpeed = Config.MaxAngularSpeed;
			MaxViewAngle = Config.MaxViewAngle;
			MaxViewDistance = Config.MaxViewDistance;
			MaxFiringAngle = Config.AngularFiringRange;
			MaxFiringDistance = Config.MaximumFiringRange;
			if (Config != null)
			{
				m_possibleSoundPlayTick = Random.Range(Config.CalloutFrequencyRange.x, Config.CalloutFrequencyRange.y);
			}
		}

		private void Start()
		{
			if (Agent != null)
			{
				Agent.enabled = true;
			}
			StartCoroutine(MeshKill());
		}

		private IEnumerator MeshKill()
		{
			yield return null;
			if (Agent != null && !Agent.isOnNavMesh)
			{
				Object.Destroy(base.gameObject);
			}
		}

		public void BlowUpHead()
		{
			if (Pieces[0] != null)
			{
				Pieces[0].Splode();
				BotExplosionPiece(Pieces[0], Pieces[0].transform.position, -Pieces[0].transform.up);
			}
		}

		private float GetCurrentSpeedMultiplier()
		{
			if (m_isStunned)
			{
				return 0.15f;
			}
			if (m_isInSmoke)
			{
				return 0.3f;
			}
			return 1f;
		}

		public void ReConfig(wwBotWurstConfig c, float lifeMult = 1f)
		{
			if (Agent != null)
			{
				Agent.acceleration = c.Acceleration;
			}
			Pieces[0].SetLife((float)c.Life_Head * lifeMult);
			Pieces[1].SetLife((float)c.Life_Torso * lifeMult);
			Pieces[2].SetLife((float)c.Life_Bottom * lifeMult);
			Max_LinearSpeed_Walk = c.LinearSpeed_Walk * Random.Range(0.9f, 1.1f);
			Max_LinearSpeed_Combat = c.LinearSpeed_Combat * Random.Range(0.9f, 1.1f);
			Max_LinearSpeed_Run = c.LinearSpeed_Run * Random.Range(0.9f, 1.1f);
			Acceleration = c.Acceleration;
			Max_AngularSpeed = c.MaxAngularSpeed;
			MaxViewAngle = c.MaxViewAngle;
			MaxViewDistance = c.MaxViewDistance;
			MaxFiringAngle = c.AngularFiringRange;
			MaxFiringDistance = c.MaximumFiringRange;
		}

		public void HealthOverride(float head, float body, float lower)
		{
			Pieces[0].SetLife(head);
			Pieces[1].SetLife(body);
			Pieces[2].SetLife(lower);
		}

		public void ConfigBot(int index, bool isPosse, wwBotManager manager, wwBotWurstNavPointGroup navgroup, Transform targetOverride)
		{
			Manager = manager;
			m_index = index;
			m_isPosse = isPosse;
			NavPointGroup = navgroup;
			Target = targetOverride;
			if (Hat != null)
			{
				Hat.HatBanditIndex = index;
				Hat.IsPosse = isPosse;
				Hat.Manager = manager;
			}
			m_possibleSoundPlayTick = Random.Range(Config.CalloutFrequencyRange.x, Config.CalloutFrequencyRange.y);
		}

		private void Update()
		{
			if (m_timeSinceTargetSeen < 30f)
			{
				m_timeSinceTargetSeen += Time.deltaTime;
			}
			StateUpdate();
		}

		public void HatRemoved()
		{
			if (Hat != null && Hat.RootRigidbody == null)
			{
				Hat.transform.SetParent(null);
				Hat.RootRigidbody = Hat.gameObject.AddComponent<Rigidbody>();
			}
			Pieces[0].Splode();
			BotExplosionPiece(Pieces[0], Pieces[0].transform.position, -Pieces[0].transform.up);
		}

		public void BotExplosionPiece(wwBotWurstDamageablePiece p, Vector3 point, Vector3 strikeDir)
		{
			if (Agent != null)
			{
				Agent.enabled = false;
			}
			RB_Bottom.isKinematic = false;
			if (Hat != null && Hat.RootRigidbody == null)
			{
				Hat.Remove();
				if (Hat.IsPosse)
				{
					Object.Destroy(Hat.gameObject);
				}
				else
				{
					Hat.transform.SetParent(null);
					Hat.RootRigidbody = Hat.gameObject.AddComponent<Rigidbody>();
				}
			}
			wwBotWurstGun[] guns = Guns;
			foreach (wwBotWurstGun wwBotWurstGun in guns)
			{
				wwBotWurstGun.SetFireAtWill(b: false);
			}
			foreach (wwBotWurstModernGun modernGun in ModernGuns)
			{
				modernGun.SetFireAtWill(b: false);
			}
			for (int j = 0; j < Pieces.Length; j++)
			{
				if (Pieces[j] != null)
				{
					Pieces[j].StartCountingDown();
				}
			}
			if (p == Pieces[0] || p == Pieces[1] || p == Pieces[2])
			{
			}
			if (Manager != null)
			{
				Manager.SpawnedBots.Remove(base.gameObject);
			}
			if (Manager != null)
			{
				Manager.BotKilled(m_index, base.transform.position + Vector3.up * 1.5f);
			}
			GM.CurrentSceneSettings.ShotEventReceivers.Remove(base.gameObject);
			if (DropOnDeath != null)
			{
				GameObject gameObject = Object.Instantiate(DropOnDeath, base.transform.position, Quaternion.identity);
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				if (component != null)
				{
					component.velocity = Random.onUnitSphere * Random.Range(0.5f, 3f);
				}
			}
			Object.Destroy(p.gameObject);
			if (this != null)
			{
				Object.Destroy(this);
			}
		}

		public void StunBot(float f)
		{
			if (CanBeStunned)
			{
				if (!m_isStunned)
				{
					m_isStunned = true;
				}
				m_stunTick = Mathf.Max(f, m_stunTick);
				LastPlaceTargetSeen = base.transform.position + Random.onUnitSphere * Random.Range(1f, 5f);
			}
		}

		private void SensingLoop()
		{
			if (CanBeStunned && m_isStunned)
			{
				m_stunTick -= Time.deltaTime;
				if (m_stunTick <= 0f)
				{
					m_isStunned = false;
				}
			}
			if (m_senseTick > 0f)
			{
				m_senseTick -= Time.deltaTime;
				return;
			}
			m_senseTick = Random.Range(0.1f, 0.25f);
			if (Physics.CheckSphere(RB_Head.transform.position + RB_Head.transform.up * 0.3f, 0.4f, LM_SmokeDetect, QueryTriggerInteraction.Collide))
			{
				m_isInSmoke = true;
				m_timeSinceTargetSeen += Time.deltaTime * 2f;
				return;
			}
			m_isInSmoke = false;
			int num = Random.Range(0, 2);
			Vector3 zero = Vector3.zero;
			Vector3 vector = Vector3.zero;
			if (Target != null)
			{
				vector = Target.position;
			}
			else if (GM.CurrentPlayerBody != null)
			{
				vector = ((num <= 0) ? GM.CurrentPlayerBody.Torso.transform.position : GM.CurrentPlayerBody.Head.transform.position);
			}
			if (m_isStunned)
			{
				vector = base.transform.position + Random.onUnitSphere * Random.Range(3f, 10f);
			}
			zero = vector - RB_Head.transform.position;
			float num2 = Vector3.Angle(RB_Head.transform.forward, zero);
			bool flag = true;
			if (GM.CurrentPlayerBody != null && GM.CurrentPlayerBody.IsGhosted)
			{
				flag = false;
			}
			if (flag && num2 < MaxViewAngle && zero.magnitude < MaxViewDistance && !Physics.Linecast(RB_Head.transform.position, vector, LM_Sight, QueryTriggerInteraction.Ignore))
			{
				m_timeSinceTargetSeen = 0f;
				LastPlaceTargetSeen = vector;
				if (Config.CanFight)
				{
					State = BotState.Fighting;
				}
				if (!Config.CanFight && m_isAggrod && CanCivGetScared)
				{
					State = BotState.RunAway;
				}
			}
		}

		private void StateUpdate()
		{
			if (Agent == null || !Agent.isActiveAndEnabled)
			{
				return;
			}
			switch (State)
			{
			case BotState.StandingAround:
				if (Config.CanFight)
				{
					SensingLoop();
				}
				State_StandingAround();
				break;
			case BotState.Patrolling:
				SensingLoop();
				State_Patrolling();
				break;
			case BotState.Fighting:
				SensingLoop();
				State_Fighting();
				break;
			case BotState.RunAway:
				SensingLoop();
				State_RunAway();
				break;
			case BotState.Searching:
				SensingLoop();
				State_Searching();
				break;
			case BotState.Dead:
				State_Dead();
				break;
			}
			if (m_possibleSoundPlayTick > 0f)
			{
				m_possibleSoundPlayTick -= Time.deltaTime;
				m_canSpeak = false;
			}
			else
			{
				m_canSpeak = true;
			}
		}

		private void SayHook(AudioEvent hookEvent, float rangeLimit)
		{
			if (hookEvent.Clips.Count > 0)
			{
				m_possibleSoundPlayTick = Random.Range(Config.CalloutFrequencyRange.x, Config.CalloutFrequencyRange.y);
				m_canSpeak = false;
				float num = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.transform.position);
				if (num < rangeLimit)
				{
					SM.PlayCoreSound(FVRPooledAudioType.NPCBarks, hookEvent, base.transform.position);
				}
			}
		}

		private void TurnTowardsFacing(float speed)
		{
			Vector3 vector = Vector3.RotateTowards(base.transform.forward, m_targetFacing, speed * Time.deltaTime, 1f);
			vector.y = 0f;
			float num = Vector3.Angle(vector, base.transform.forward);
			RB_Head.AddTorque(Vector3.up * num * 0.4f, ForceMode.VelocityChange);
			base.transform.rotation = Quaternion.LookRotation(vector, Vector3.up);
		}

		public bool GetFireClear(Vector3 source, Vector3 target)
		{
			if (m_isInSmoke || m_isStunned)
			{
				return true;
			}
			if (Physics.Linecast(source, target, LM_IFF, QueryTriggerInteraction.Ignore))
			{
				return false;
			}
			return true;
		}

		private void FaceTowards(Vector3 point)
		{
			Vector3 targetFacing = point - base.transform.position;
			targetFacing += base.transform.forward * 0.0001f;
			targetFacing.y = 0f;
			targetFacing.Normalize();
			m_targetFacing = targetFacing;
		}

		private void PickNewFacing()
		{
			Vector3 onUnitSphere = Random.onUnitSphere;
			onUnitSphere.y = 0f;
			onUnitSphere.Normalize();
			m_targetFacing = onUnitSphere;
		}

		public void RegisterHit(Damage d, bool IsDeath)
		{
			if (Config.CanFight)
			{
				State = BotState.Fighting;
			}
			else if (CanCivGetScared)
			{
				m_isAggrod = true;
				State = BotState.RunAway;
			}
			if (base.transform != null && !m_isStunned)
			{
				LastPlaceTargetSeen = base.transform.position - d.strikeDir * 10f;
				m_timeSinceTargetSeen = 0f;
			}
			if (IsDeath)
			{
				GM.CurrentSceneSettings.OnBotKill(d);
			}
		}

		public void ShotEvent(Vector3 pos)
		{
			if (Config.CanFight && !m_isStunned && MaxViewDistance * 3f >= Vector3.Distance(base.transform.position, pos) && State != BotState.Fighting)
			{
				State = BotState.Searching;
				m_timeSinceTargetSeen = 0.15f;
				Vector3 vector = Random.onUnitSphere * 3f;
				vector.y = 0f;
				LastPlaceTargetSeen = pos + vector;
			}
		}

		private void State_StandingAround()
		{
			if (m_timeTilNextLookRot > 0f)
			{
				m_timeTilNextLookRot -= Time.deltaTime;
			}
			else
			{
				m_timeTilNextLookRot = Random.Range(Config.LookAroundNewPointFrequency.x, Config.LookAroundNewPointFrequency.y);
				PickNewFacing();
			}
			TurnTowardsFacing(Max_AngularSpeed * 0.25f);
		}

		private void State_Patrolling()
		{
			if ((!Agent.hasPath && !Agent.pathPending) || m_timeTilPickNewPatrolPoint < 0f)
			{
				m_patrolDestinationPoint = NavPointGroup.GetRandomPatrolPoint();
				SetBotDest(m_patrolDestinationPoint);
				Agent.speed = Max_LinearSpeed_Walk * GetCurrentSpeedMultiplier();
				m_timeTilPickNewPatrolPoint = Random.Range(Config.PatrolNewPointFrequency.x, Config.PatrolNewPointFrequency.y);
			}
			if (m_canSpeak)
			{
				if (Config.CanFight)
				{
					SayHook(Config.SpeakEvent_Patrol, 12f);
				}
				else if (GM.CurrentPlayerBody != null)
				{
					SayHook(Config.SpeakEvent_Greetings, 12f);
				}
				else
				{
					SayHook(Config.SpeakEvent_Patrol, 12f);
				}
			}
			m_timeTilPickNewPatrolPoint -= Time.deltaTime;
			Vector3 targetFacing = Agent.desiredVelocity.normalized + base.transform.forward * 0.0001f;
			targetFacing.y = 0f;
			m_targetFacing = targetFacing;
			TurnTowardsFacing(Max_AngularSpeed * 0.5f);
		}

		private void State_Fighting()
		{
			bool fireAtWill = false;
			float magnitude = (LastPlaceTargetSeen - base.transform.position).magnitude;
			Vector3 vector = m_combatDestinationPoint;
			if (ModernGuns.Count > 0)
			{
				if (m_canSpeak && ModernGuns[0] != null)
				{
					if (ModernGuns[0].FireState != wwBotWurstModernGun.FiringState.GoingToReload && ModernGuns[0].FireState != wwBotWurstModernGun.FiringState.Reloading && ModernGuns[0].FireState != wwBotWurstModernGun.FiringState.RecoveringFromReload)
					{
						SayHook(Config.SpeakEvent_InCombat, 24f);
					}
					else
					{
						SayHook(Config.SpeakEvent_Reloading, 24f);
					}
				}
			}
			else if (m_canSpeak && Guns[0] != null)
			{
				if (Guns[0].FireState != 0 && Guns[0].FireState != wwBotWurstGun.FiringState.cycledown && Guns[0].FireState != wwBotWurstGun.FiringState.cycleup)
				{
					SayHook(Config.SpeakEvent_InCombat, 24f);
				}
				else
				{
					SayHook(Config.SpeakEvent_Reloading, 24f);
				}
			}
			if (!Config.IsMelee)
			{
				vector = ((magnitude < Config.PreferedDistanceRange.x) ? NavPointGroup.GetBestPointToFleeTo(base.transform.position, LastPlaceTargetSeen) : ((!(magnitude > Config.PreferedDistanceRange.y)) ? NavPointGroup.GetClosestCoverFromAttacker(base.transform.position, LastPlaceTargetSeen) : NavPointGroup.GetClosestDestinationToTarget(LastPlaceTargetSeen)));
			}
			else if (Vector3.Distance(new Vector3(LastPlaceTargetSeen.x, 0f, LastPlaceTargetSeen.z), new Vector3(m_combatDestinationPoint.x, 0f, m_combatDestinationPoint.z)) > 0.5f)
			{
				vector = LastPlaceTargetSeen;
				Agent.speed = Max_LinearSpeed_Combat * GetCurrentSpeedMultiplier();
				if (GM.CurrentPlayerBody != null)
				{
					vector += GM.CurrentPlayerBody.Head.forward;
				}
			}
			if (Vector3.Distance(m_combatDestinationPoint, vector) > 1f && !Agent.pathPending)
			{
				m_combatDestinationPoint = vector;
				SetBotDest(m_combatDestinationPoint);
				Agent.speed = Max_LinearSpeed_Combat * GetCurrentSpeedMultiplier();
			}
			if (magnitude <= MaxFiringDistance)
			{
				fireAtWill = true;
			}
			FaceTowards(LastPlaceTargetSeen);
			TurnTowardsFacing(Max_AngularSpeed * 1f);
			wwBotWurstGun[] guns = Guns;
			foreach (wwBotWurstGun wwBotWurstGun in guns)
			{
				wwBotWurstGun.SetFireAtWill(fireAtWill);
			}
			foreach (wwBotWurstModernGun modernGun in ModernGuns)
			{
				modernGun.SetFireAtWill(fireAtWill);
			}
			if (m_timeSinceTargetSeen > Config.TimeBlindFiring)
			{
				State = BotState.Searching;
				m_atSearchPointTick = Random.Range(Config.WaitAtSearchPointRange.x, Config.WaitAtSearchPointRange.y);
				m_hasSearchPoint = false;
			}
		}

		private void State_Searching()
		{
			wwBotWurstGun[] guns = Guns;
			foreach (wwBotWurstGun wwBotWurstGun in guns)
			{
				wwBotWurstGun.SetFireAtWill(b: false);
			}
			foreach (wwBotWurstModernGun modernGun in ModernGuns)
			{
				modernGun.SetFireAtWill(b: false);
			}
			if (!m_hasSearchPoint)
			{
				m_searchingDestinationPoint = NavPointGroup.GetClosestDestinationToTarget(LastPlaceTargetSeen);
				SetBotDest(m_searchingDestinationPoint);
				Agent.speed = Max_LinearSpeed_Run * GetCurrentSpeedMultiplier();
			}
			if (m_canSpeak)
			{
				SayHook(Config.SpeakEvent_Searching, 16f);
			}
			if (Vector3.Distance(base.transform.position, m_searchingDestinationPoint) < 1f)
			{
				m_atSearchPointTick -= Time.deltaTime;
			}
			if (m_atSearchPointTick <= 0f)
			{
				State = BotState.Patrolling;
			}
			if (m_timeSinceTargetSeen < 0.1f)
			{
				State = BotState.Fighting;
			}
		}

		private void State_RunAway()
		{
			float num = Vector3.Distance(base.transform.position, LastPlaceTargetSeen);
			if (m_canSpeak)
			{
				SayHook(Config.SpeakEvent_RunningAway, 16f);
			}
			if (!Agent.hasPath && !Agent.pathPending && num < Config.PreferedDistanceRange.y)
			{
				m_fleeDestinationPoint = NavPointGroup.GetBestPointToFleeTo(base.transform.position, LastPlaceTargetSeen);
				SetBotDest(m_fleeDestinationPoint);
				Agent.speed = Max_LinearSpeed_Run * GetCurrentSpeedMultiplier();
			}
			if (num < Config.PreferedDistanceRange.x)
			{
				Vector3 bestPointToFleeTo = NavPointGroup.GetBestPointToFleeTo(base.transform.position, LastPlaceTargetSeen);
				float num2 = Vector3.Distance(m_fleeDestinationPoint, bestPointToFleeTo);
				if (num2 > 5f)
				{
					m_fleeDestinationPoint = bestPointToFleeTo;
					SetBotDest(m_fleeDestinationPoint);
					Agent.speed = Max_LinearSpeed_Run * GetCurrentSpeedMultiplier();
				}
			}
			FaceTowards(m_fleeDestinationPoint);
			TurnTowardsFacing(Max_AngularSpeed * 1f);
		}

		private void State_Dead()
		{
		}

		private void SetBotDest(Vector3 p)
		{
			if (Agent != null && Agent.isOnNavMesh)
			{
				Agent.SetDestination(p);
			}
		}
	}
}
