using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class RonchMaster : MonoBehaviour
	{
		public enum RonchState
		{
			None = 0,
			Roar = 1,
			TurnToFacing = 10,
			MoveToNode = 11,
			LeapToNode = 12,
			FireRailgun = 20,
			FireBackMissiles = 21,
			FireGau8s = 22,
			Dying = 30,
			Dead = 0x1F
		}

		public enum DamageStage
		{
			Raydome,
			Cockpit,
			Dying,
			Dead
		}

		[Header("Master Connections")]
		public RonchMover Mover;

		public List<RonchWaypoint> Waypoints;

		public RonchState CurState;

		public Queue<RonchState> StateQueue = new Queue<RonchState>();

		private bool m_advanceToNextState;

		public RonchWaypoint InitWaypoint;

		private RonchWaypoint m_lastWaypoint;

		private RonchWaypoint m_curWaypoint;

		private RonchWaypoint m_nextWaypoint;

		private Vector3 m_currentFacingDir = new Vector3(0f, 0f, 1f);

		private Vector3 m_targetFacingDir = new Vector3(0f, 0f, 1f);

		public float WalkSpeed = 5f;

		public float RunSpeed = 10f;

		[Header("Weapons")]
		public List<RonchWeapon> Gau8s;

		public RonchWeapon MissileBank;

		public RonchWeapon FlashNades;

		public RonchWeapon SmokeNades;

		public List<RonchWeapon> Gau8s_Defensive;

		public RonchWeapon LaserNads;

		public Transform Laser_BaseDir;

		public Transform Laser_RotatingPart_Horizontal;

		public Transform Laser_RotatingPart_Vertical;

		public float Laser_Range = 20f;

		public Transform TargetPoint;

		[Header("Head")]
		public Transform Head;

		public Transform BaseHeadPos;

		public Transform HeadLower;

		[Header("LifeBar")]
		public Transform LifeBarHolder;

		public Transform LifeBar;

		public DamageStage DamState;

		public RonchRayDome RayDome;

		public GameObject Entity_Cockpit;

		public RonchCockpit Cockpit;

		[Header("Dying Sequence")]
		public List<GameObject> ExplosionSetRandom;

		public List<Transform> ExplosionPlaces;

		public List<GameObject> FinalExplosionPrefabs;

		public Transform FinalExplosionPlace;

		private float m_TickDownDying = 8f;

		private float m_tickDownTilNextSplode = 0.1f;

		private int m_placeTick;

		[Header("Sight")]
		public LayerMask LM_SightBlock;

		public LayerMask LM_LaserBlock;

		[Header("Speaking")]
		public AudioEvent AudEvent_Intro;

		public AudioEvent AudEvent_Moving;

		public AudioEvent AudEvent_Turning;

		public AudioEvent AudEvent_Firing;

		public AudioEvent AudEvent_Dead;

		private HG_ModeManager_MeatleGear MM;

		private bool justFiredWeapon = true;

		private float m_turningLerp;

		private float m_timeUntilFireOpenLaser = 6f;

		private float m_timeUntilFireOpen = 15f;

		private bool m_isNextNadeFlash = true;

		private float m_timeUntilNextDefensiveMachineGun = 20f;

		public void SetModeManager(HG_ModeManager_MeatleGear m)
		{
			MM = m;
		}

		private void Start()
		{
			m_curWaypoint = InitWaypoint;
			m_lastWaypoint = InitWaypoint;
			DecideNextActions();
			Mover.transform.SetParent(null);
			TargetPoint.SetParent(null);
			SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AudEvent_Intro, HeadLower.position);
		}

		private void DecideNextActions()
		{
			if (StateQueue.Count > 0)
			{
				SetState(StateQueue.Dequeue());
				return;
			}
			if (justFiredWeapon)
			{
				QueueUpMoveToRandomNeighborToWalkTo();
				justFiredWeapon = false;
			}
			else
			{
				SetTargetFacingDir(TargetPoint.position);
				StateQueue.Enqueue(RonchState.TurnToFacing);
				QueueWeaponFiringBasedOnDistance();
				justFiredWeapon = true;
			}
			DecideNextActions();
		}

		private void QueueUpMoveToRandomNeighborToWalkTo()
		{
			m_nextWaypoint = GetRandomNeighbor();
			m_lastWaypoint = m_curWaypoint;
			SetTargetFacingDir(m_nextWaypoint);
			StateQueue.Enqueue(RonchState.TurnToFacing);
			StateQueue.Enqueue(RonchState.MoveToNode);
		}

		private void QueueUpMoveToRandomNeighborToLeapTo()
		{
			m_nextWaypoint = GetRandomNeighbor();
			m_lastWaypoint = m_curWaypoint;
			SetTargetFacingDir(m_nextWaypoint);
			StateQueue.Enqueue(RonchState.TurnToFacing);
			StateQueue.Enqueue(RonchState.MoveToNode);
		}

		private void QueueWeaponFiringBasedOnDistance()
		{
			float num = UnityEngine.Random.Range(0f, 1f);
			if (num > 0.5f)
			{
				StateQueue.Enqueue(RonchState.FireBackMissiles);
			}
			else
			{
				StateQueue.Enqueue(RonchState.FireGau8s);
			}
		}

		private RonchWaypoint GetRandomWaypoint()
		{
			return Waypoints[UnityEngine.Random.Range(0, Waypoints.Count)];
		}

		private RonchWaypoint GetRandomNeighbor()
		{
			List<RonchWaypoint> list = new List<RonchWaypoint>();
			for (int i = 0; i < m_curWaypoint.Neighbors.Count; i++)
			{
				list.Add(m_curWaypoint.Neighbors[i]);
			}
			if (list.Contains(m_lastWaypoint))
			{
				list.Remove(m_lastWaypoint);
			}
			if (list.Count > 0)
			{
				return list[UnityEngine.Random.Range(0, list.Count)];
			}
			return m_curWaypoint.Neighbors[UnityEngine.Random.Range(0, m_curWaypoint.Neighbors.Count)];
		}

		private RonchWaypoint GetClosestWaypointToEnemy()
		{
			return Waypoints[UnityEngine.Random.Range(0, Waypoints.Count)];
		}

		private void SetTargetFacingDir(RonchWaypoint wp)
		{
			Vector3 targetFacingDir = wp.transform.position - m_curWaypoint.transform.position;
			targetFacingDir.Normalize();
			m_targetFacingDir = targetFacingDir;
		}

		private void SetTargetFacingDir(Vector3 targetPoint)
		{
			Vector3 targetFacingDir = targetPoint - Mover.transform.position;
			targetFacingDir.Normalize();
			m_targetFacingDir = targetFacingDir;
		}

		private void SetState(RonchState s)
		{
			CurState = s;
			switch (CurState)
			{
			case RonchState.TurnToFacing:
				SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AudEvent_Turning, HeadLower.position);
				m_turningLerp = 0f;
				break;
			case RonchState.MoveToNode:
				SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AudEvent_Moving, HeadLower.position);
				break;
			case RonchState.FireBackMissiles:
				SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AudEvent_Firing, HeadLower.position);
				MissileBank.SetTargetPos(TargetPoint);
				MissileBank.BeginFiringSequence();
				break;
			case RonchState.FireGau8s:
			{
				SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AudEvent_Firing, HeadLower.position);
				for (int i = 0; i < Gau8s.Count; i++)
				{
					Gau8s[i].BeginFiringSequence();
				}
				break;
			}
			case RonchState.Dead:
				SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AudEvent_Dead, HeadLower.position);
				MM.GronchDied();
				break;
			}
		}

		private void Update()
		{
			Vector3 forward = GM.CurrentPlayerBody.transform.position - Mover.transform.position;
			forward.y = 0f;
			LifeBarHolder.rotation = Quaternion.LookRotation(forward, Vector3.up);
			switch (CurState)
			{
			case RonchState.None:
				DecideNextActions();
				break;
			case RonchState.TurnToFacing:
				Update_TurnToFacing();
				break;
			case RonchState.MoveToNode:
				Update_MoveToNode();
				break;
			case RonchState.FireBackMissiles:
				Update_FirebackMissiles();
				break;
			case RonchState.FireGau8s:
				Update_FireGau8s();
				break;
			case RonchState.Dying:
				Update_Dying();
				break;
			}
			UpdateHealthState();
		}

		private void UpdateHealthState()
		{
			bool flag = true;
			float x = 1f;
			switch (DamState)
			{
			case DamageStage.Raydome:
				if (RayDome != null)
				{
					x = Mathf.Clamp(RayDome.GetLifeRatio(), 0.01f, 1f);
					flag = true;
				}
				else
				{
					SetDamStateToCockpit();
					flag = true;
				}
				break;
			case DamageStage.Cockpit:
				if (Cockpit != null)
				{
					x = Mathf.Clamp(Cockpit.GetLifeRatio(), 0.01f, 1f);
					flag = true;
					if (Cockpit.GetLifeRatio() <= 0.0001f)
					{
						SetDamStateToDying();
						flag = false;
					}
				}
				break;
			}
			if (flag)
			{
				LifeBarHolder.gameObject.SetActive(value: true);
				LifeBar.localScale = new Vector3(x, 1f, 1f);
			}
			else
			{
				LifeBarHolder.gameObject.SetActive(value: false);
			}
		}

		private void SetDamStateToCockpit()
		{
			DamState = DamageStage.Cockpit;
			Cockpit.SetCanTakeDamage(b: true);
			Entity_Cockpit.gameObject.SetActive(value: true);
			HeadLower.localEulerAngles = new Vector3(25f, 0f, 0f);
		}

		private void SetDamStateToDying()
		{
			DamState = DamageStage.Dying;
			Cockpit.SetCanTakeDamage(b: false);
			Entity_Cockpit.gameObject.SetActive(value: false);
			SetState(RonchState.Dying);
			m_TickDownDying = 10f;
			for (int i = 0; i < Gau8s.Count; i++)
			{
				Gau8s[i].DestroyGun();
				Gau8s_Defensive[i].DestroyGun();
			}
			MissileBank.DestroyGun();
			FlashNades.DestroyGun();
			SmokeNades.DestroyGun();
			LaserNads.DestroyGun();
		}

		private void SetDamStateToDead()
		{
			DamState = DamageStage.Dead;
		}

		private void Update_TurnToFacing()
		{
			HeadUpdate();
			DefensiveWeaponUpdate();
			float num = Mover.UpdateWalking(Time.deltaTime * 2f);
			float value = num / Time.deltaTime;
			value = Mathf.Clamp(value, 0f, 8f);
			m_targetFacingDir.y = 0f;
			m_currentFacingDir = Vector3.RotateTowards(m_currentFacingDir, m_targetFacingDir, Time.deltaTime * value * 0.08f, 1f);
			m_currentFacingDir.y = 0f;
			Mover.SetFacing(m_currentFacingDir);
			if (Vector3.Angle(m_currentFacingDir, m_targetFacingDir) < 1f)
			{
				m_currentFacingDir = m_targetFacingDir;
				CurState = RonchState.None;
				DecideNextActions();
			}
			Debug.DrawLine(Mover.transform.position, Mover.transform.position + m_currentFacingDir * 30f, Color.yellow);
			Debug.DrawLine(Mover.transform.position, Mover.transform.position + m_targetFacingDir * 30f, Color.red);
		}

		private void Update_MoveToNode()
		{
			HeadUpdate();
			DefensiveWeaponUpdate();
			float num = Mover.UpdateWalking(Time.deltaTime * 2f);
			float value = num / Time.deltaTime;
			value = Mathf.Clamp(value, 0f, 8f);
			if (Mover.MoveTowardsPosition(m_nextWaypoint, value * 0.35f))
			{
				CurState = RonchState.None;
				m_curWaypoint = m_nextWaypoint;
				DecideNextActions();
			}
		}

		private void Update_FirebackMissiles()
		{
			HeadUpdate();
			DefensiveWeaponUpdate();
		}

		private void Update_FireGau8s()
		{
			HeadUpdate();
			DefensiveWeaponUpdate();
		}

		private void Update_Dying()
		{
			m_TickDownDying -= Time.deltaTime;
			m_tickDownTilNextSplode -= Time.deltaTime;
			if (m_tickDownTilNextSplode <= 0f)
			{
				m_tickDownTilNextSplode = UnityEngine.Random.Range(0.15f, 0.35f);
				for (int i = 0; i < ExplosionSetRandom.Count; i++)
				{
					UnityEngine.Object.Instantiate(ExplosionSetRandom[i], ExplosionPlaces[m_placeTick].position, ExplosionPlaces[m_placeTick].rotation);
					m_placeTick++;
					if (m_placeTick >= ExplosionPlaces.Count)
					{
						m_placeTick = 0;
					}
				}
				Mover.Shudder();
			}
			if (m_TickDownDying <= 0f)
			{
				for (int j = 0; j < FinalExplosionPrefabs.Count; j++)
				{
					UnityEngine.Object.Instantiate(FinalExplosionPrefabs[j], FinalExplosionPlace.position, UnityEngine.Random.rotation);
				}
				Mover.SetToDeathPose();
				SetState(RonchState.Dead);
				HeadLower.gameObject.SetActive(value: false);
			}
		}

		private void DefensiveWeaponUpdate()
		{
			Vector3 vector = TargetPoint.position - Laser_BaseDir.position;
			if (vector.magnitude < 50f)
			{
				vector.y = 0f;
				Vector3 forward = Vector3.RotateTowards(Laser_RotatingPart_Horizontal.forward, vector, Time.deltaTime * 2f, 1f);
				Laser_RotatingPart_Horizontal.rotation = Quaternion.LookRotation(forward, Vector3.up);
				Vector3 vector2 = TargetPoint.position - Laser_RotatingPart_Vertical.position;
				Vector3 target = Vector3.ProjectOnPlane(vector2, Laser_RotatingPart_Horizontal.right);
				Vector3 target2 = Vector3.RotateTowards(Laser_BaseDir.forward, target, (float)Math.PI / 3f, 1f);
				Vector3 forward2 = Vector3.RotateTowards(Laser_RotatingPart_Vertical.forward, target2, Time.deltaTime * 2f, 1f);
				Laser_RotatingPart_Vertical.rotation = Quaternion.LookRotation(forward2, Vector3.up);
				if (m_timeUntilFireOpenLaser > 0f)
				{
					m_timeUntilFireOpenLaser -= Time.deltaTime;
				}
				else if (Vector3.Angle(LaserNads.Muzzle.forward, vector) < 25f)
				{
					LaserNads.BeginFiringSequence();
					m_timeUntilFireOpenLaser = UnityEngine.Random.Range(10f, 15f);
				}
			}
			Vector3 from = TargetPoint.position - Mover.transform.position;
			from.y = 0f;
			if (from.magnitude > 40f && Vector3.Angle(from, Mover.transform.forward) < 45f)
			{
				if (m_timeUntilFireOpen > 0f)
				{
					m_timeUntilFireOpen -= Time.deltaTime;
				}
				else if (m_isNextNadeFlash)
				{
					Vector3 forward3 = GM.CurrentPlayerBody.transform.position + Vector3.up * 6f - FlashNades.Muzzle.position;
					FlashNades.Muzzle.rotation = Quaternion.LookRotation(forward3, Vector3.up);
					FlashNades.BeginFiringSequence();
					m_isNextNadeFlash = false;
					m_timeUntilFireOpen = UnityEngine.Random.Range(10f, 25f);
				}
				else
				{
					Vector3 forward4 = GM.CurrentPlayerBody.transform.position + Vector3.up * 6f - SmokeNades.Muzzle.position;
					SmokeNades.Muzzle.rotation = Quaternion.LookRotation(forward4, Vector3.up);
					SmokeNades.BeginFiringSequence();
					m_isNextNadeFlash = true;
					m_timeUntilFireOpen = UnityEngine.Random.Range(10f, 25f);
				}
			}
			Vector3 from2 = TargetPoint.position - Head.position;
			from2.y = 0f;
			if (!(Vector3.Angle(from2, Head.forward) < 30f) || !(from2.magnitude > 30f) || !(from2.magnitude < 120f))
			{
				return;
			}
			m_timeUntilNextDefensiveMachineGun -= Time.deltaTime;
			if (m_timeUntilNextDefensiveMachineGun <= 0f)
			{
				m_timeUntilNextDefensiveMachineGun = UnityEngine.Random.Range(20f, 40f);
				for (int i = 0; i < Gau8s_Defensive.Count; i++)
				{
					Gau8s_Defensive[i].BeginFiringSequence();
				}
			}
		}

		private void HeadUpdate()
		{
			Vector3 forward = BaseHeadPos.forward;
			Vector3 target = TargetPoint.position - BaseHeadPos.position;
			Vector3 target2 = Vector3.RotateTowards(forward, target, (float)Math.PI / 6f, 1f);
			Vector3 forward2 = Vector3.RotateTowards(Head.forward, target2, Time.deltaTime, 1f);
			Head.rotation = Quaternion.LookRotation(forward2, Vector3.up);
			if (DamState == DamageStage.Raydome && RayDome != null)
			{
				Vector3 to = GM.CurrentPlayerBody.transform.position - RayDome.transform.position;
				if (Vector3.Angle(RayDome.transform.forward, to) < 90f && !Physics.Linecast(RayDome.RayDomeSightPose.position, GM.CurrentPlayerBody.Head.position, LM_SightBlock, QueryTriggerInteraction.Ignore))
				{
					TargetPoint.position = GM.CurrentPlayerBody.Head.position;
				}
			}
			else if (DamState == DamageStage.Cockpit && HeadLower != null)
			{
				Vector3 to2 = GM.CurrentPlayerBody.transform.position - HeadLower.transform.position;
				if (Vector3.Angle(BaseHeadPos.transform.forward, to2) < 120f && !Physics.Linecast(BaseHeadPos.position, GM.CurrentPlayerBody.Head.position, LM_SightBlock, QueryTriggerInteraction.Ignore))
				{
					TargetPoint.position = GM.CurrentPlayerBody.Head.position;
				}
			}
		}

		public void FiringComplete()
		{
			DecideNextActions();
		}

		public void Dispose()
		{
			UnityEngine.Object.Destroy(Mover.gameObject);
			UnityEngine.Object.Destroy(TargetPoint.gameObject);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
