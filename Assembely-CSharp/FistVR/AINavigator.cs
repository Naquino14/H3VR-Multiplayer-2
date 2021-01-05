using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
	public class AINavigator : AIBodyPiece
	{
		public enum TopoPointSampleMode
		{
			CurrentPos,
			RandomRay
		}

		public class TopoPoint : IComparable<TopoPoint>
		{
			public Vector3 point;

			public Transform origin;

			public int CompareTo(TopoPoint other)
			{
				if (other == null)
				{
					return 1;
				}
				if (Vector3.Distance(point, origin.position) > Vector3.Distance(other.point, origin.position))
				{
					return -1;
				}
				if (Vector3.Distance(point, origin.position) < Vector3.Distance(other.point, origin.position))
				{
					return 1;
				}
				return 0;
			}
		}

		public class TopoPointSet
		{
			public List<TopoPoint> m_points;

			public List<TopoPoint> m_sortedPoints;

			private int m_arraySize;

			private int m_arrayIndex;

			private float m_tick;

			private float m_maxTick;

			private TopoPointSampleMode m_sampleMode;

			private NavMeshHit m_hit;

			private Transform m_origin;

			public TopoPointSet(int size, float interval, Transform origin, TopoPointSampleMode mode)
			{
				m_points = new List<TopoPoint>();
				m_sortedPoints = new List<TopoPoint>();
				m_tick = UnityEngine.Random.Range(0f, interval);
				m_maxTick = interval;
				m_sampleMode = mode;
				m_arraySize = size;
				m_arrayIndex = 0;
				m_origin = origin;
				for (int i = 0; i < size; i++)
				{
					TopoPoint item = new TopoPoint
					{
						origin = origin,
						point = origin.position
					};
					m_points.Add(item);
				}
				for (int j = 0; j < size; j++)
				{
					TopoPoint item2 = new TopoPoint
					{
						origin = origin,
						point = origin.position
					};
					m_sortedPoints.Add(item2);
				}
			}

			public void Update()
			{
				if (m_tick <= 0f)
				{
					m_tick = m_maxTick;
					SampleNewPoint();
				}
				else
				{
					m_tick -= Time.deltaTime;
				}
			}

			private void SampleNewPoint()
			{
				switch (m_sampleMode)
				{
				case TopoPointSampleMode.CurrentPos:
					m_points[m_arrayIndex].point = m_points[m_arrayIndex].origin.position;
					break;
				case TopoPointSampleMode.RandomRay:
				{
					Vector3 targetPosition = new Vector3(m_origin.position.x + UnityEngine.Random.Range(-20f, 20f), m_origin.position.y, m_origin.position.z + UnityEngine.Random.Range(-20f, 20f));
					NavMesh.Raycast(m_origin.position, targetPosition, out m_hit, -1);
					m_points[m_arrayIndex].point = m_hit.position;
					break;
				}
				}
				m_arrayIndex++;
				if (m_arrayIndex >= m_arraySize)
				{
					m_arrayIndex = 0;
				}
			}

			private void CopyAndSort()
			{
				for (int i = 0; i < m_arraySize; i++)
				{
					m_sortedPoints[i].point = m_points[i].point;
				}
				m_sortedPoints.Sort();
			}

			public Vector3 GetRandomPoint()
			{
				return m_points[UnityEngine.Random.Range(0, m_arraySize)].point;
			}

			public Vector3 GetFurthestPointFrom(Vector3 point)
			{
				Vector3 result = Vector3.zero;
				float num = 0f;
				for (int i = 0; i < m_points.Count; i++)
				{
					float num2 = Vector3.Distance(m_points[i].point, point);
					if (num2 >= num)
					{
						num = num2;
						result = m_points[i].point;
					}
				}
				return result;
			}

			public Vector3 GetNearestPointTo(Vector3 point)
			{
				Vector3 result = Vector3.zero;
				float num = 9000f;
				for (int i = 0; i < m_points.Count; i++)
				{
					float num2 = Vector3.Distance(m_points[i].point, point);
					if (num2 <= num)
					{
						num = num2;
						result = m_points[i].point;
					}
				}
				return result;
			}
		}

		[Header("AI Navigator BaseClass Params")]
		public NavMeshAgent Agent;

		public Rigidbody BaseRB;

		public float MaxLinearSpeed = 1f;

		public float MaxTurningSpeed = 120f;

		private float currentTurningSpeed = 1f;

		private float currentLinearSpeed = 1f;

		private float currentMovementIntensity = 0.1f;

		public float DestinationThreshold = 1f;

		public bool IsAtDestination;

		protected Vector3 m_currentDestination;

		protected Vector3 m_currentNavTarget;

		protected NavMeshHit m_nHit;

		public LayerMask LMGround;

		private RaycastHit m_gHit;

		public Transform[] GroundCastPoints;

		private Vector3[] GroundContactPoints = new Vector3[3];

		private TopoPointSet m_tPCloseRandom;

		private TopoPointSet m_tPPastTravelled;

		public float TimeTilPlatesReset = 10f;

		public bool IsPermanentlyDisabled;

		private float m_damageReset;

		private Vector3 localForward = Vector3.zero;

		private Vector3 localUp = Vector3.zero;

		private Vector3 localRight = Vector3.zero;

		public Vector3 GetDestination()
		{
			return m_currentDestination;
		}

		public override void Awake()
		{
			base.Awake();
			IsAtDestination = true;
			m_currentDestination = BaseRB.position;
			m_currentNavTarget = BaseRB.position;
			Agent.updatePosition = false;
			Agent.updateRotation = false;
			for (int i = 0; i < GroundContactPoints.Length; i++)
			{
				ref Vector3 reference = ref GroundContactPoints[i];
				reference = GroundCastPoints[i].position;
			}
			m_tPCloseRandom = new TopoPointSet(20, 1f, Agent.transform, TopoPointSampleMode.RandomRay);
			m_tPPastTravelled = new TopoPointSet(20, 10f, Agent.transform, TopoPointSampleMode.CurrentPos);
		}

		private void UpdateTPPoints()
		{
			m_tPCloseRandom.Update();
			m_tPPastTravelled.Update();
		}

		public override void DestroyEvent()
		{
			IsPermanentlyDisabled = true;
			base.DestroyEvent();
		}

		public void UpdateNavigationSystem()
		{
			UpdateTPPoints();
			CheckIfAtDestination();
			CheckIfNeedNewNavAgentPath();
		}

		public override void Update()
		{
			base.Update();
			if (IsPlateDisabled || IsPermanentlyDisabled)
			{
				currentTurningSpeed = 0f;
				currentLinearSpeed = 0f;
				Agent.Stop();
			}
			else if (IsPlateDamaged)
			{
				currentTurningSpeed = UnityEngine.Random.Range(0f, MaxTurningSpeed * 0.15f);
				currentLinearSpeed = Mathf.Lerp(0.1f, UnityEngine.Random.Range(0f, MaxLinearSpeed * 0.08f), currentMovementIntensity);
				m_damageReset -= Time.deltaTime;
				if (m_damageReset <= 0f)
				{
					ResetAllPlates();
				}
			}
			else
			{
				currentTurningSpeed = MaxTurningSpeed;
				currentLinearSpeed = Mathf.Lerp(0.1f, MaxLinearSpeed, currentMovementIntensity);
			}
		}

		public override bool SetPlateDamaged(bool b)
		{
			if (base.SetPlateDamaged(b))
			{
				m_damageReset = TimeTilPlatesReset;
				return true;
			}
			return false;
		}

		public override bool SetPlateDisabled(bool b)
		{
			if (base.SetPlateDisabled(b))
			{
				Debug.Log("DEAD NAV");
				return true;
			}
			return false;
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			MoveAgent();
		}

		private void MoveAgent()
		{
			for (int i = 0; i < GroundCastPoints.Length; i++)
			{
				if (Physics.Raycast(GroundCastPoints[i].position + Vector3.up, Vector3.down, out m_gHit, 1.25f, LMGround))
				{
					ref Vector3 reference = ref GroundContactPoints[i];
					reference = m_gHit.point;
					Debug.DrawLine(GroundContactPoints[i], GroundContactPoints[i] + Vector3.up, Color.yellow);
				}
				else
				{
					ref Vector3 reference2 = ref GroundContactPoints[i];
					reference2 = GroundCastPoints[i].position;
					Debug.DrawLine(GroundContactPoints[i], GroundContactPoints[i] + Vector3.up, Color.magenta);
				}
			}
			Plane plane = new Plane(GroundContactPoints[0], GroundContactPoints[1], GroundContactPoints[2]);
			Vector3 vector = (GroundContactPoints[0] + GroundContactPoints[2]) * 0.5f;
			localForward = (vector - GroundContactPoints[1]).normalized;
			localUp = plane.normal;
			localUp = Vector3.up;
			localRight = Vector3.Cross(localUp, localForward);
			Debug.DrawLine(BaseRB.position, BaseRB.position + localRight, Color.red);
			Debug.DrawLine(BaseRB.position, BaseRB.position + localUp, Color.green);
			Debug.DrawLine(BaseRB.position, BaseRB.position + localForward, Color.blue);
			if (!(Agent.desiredVelocity.magnitude < 0.01f))
			{
				Vector3 normalized = Agent.desiredVelocity.normalized;
				if (Vector3.Angle(normalized, localUp) <= 0.001f)
				{
					localUp += UnityEngine.Random.onUnitSphere * 0.001f;
				}
				Vector3 forward = Vector3.ProjectOnPlane(normalized, localUp);
				Quaternion to = Quaternion.LookRotation(forward, localUp);
				Quaternion rot = Quaternion.RotateTowards(BaseRB.rotation, to, currentTurningSpeed * Time.deltaTime);
				BaseRB.MoveRotation(rot);
				Vector3 to2 = Vector3.ProjectOnPlane(base.transform.forward, Vector3.up);
				float num = Vector3.Angle(normalized, to2);
				Agent.speed = Mathf.Lerp(currentLinearSpeed, 0.1f, num * 0.1f - 2f);
				BaseRB.MovePosition(Agent.nextPosition);
			}
		}

		public Vector3 GetRandomNearDestination()
		{
			return m_tPCloseRandom.GetRandomPoint();
		}

		public Vector3 GetFurthestNearPointFrom(Vector3 targ)
		{
			return m_tPCloseRandom.GetFurthestPointFrom(targ);
		}

		public Vector3 GetNearestNearPointFrom(Vector3 targ)
		{
			return m_tPCloseRandom.GetNearestPointTo(targ);
		}

		public void SetMovementIntensity(float f)
		{
			currentMovementIntensity = f;
		}

		public void SetNewNavDestination(Vector3 dest)
		{
			m_currentDestination = dest;
			IsAtDestination = false;
		}

		public void RotateTowards(Vector3 point)
		{
			Vector3 normalized = (point - base.transform.position).normalized;
			normalized.y = 0f;
			Quaternion b = Quaternion.LookRotation(normalized, Vector3.up);
			Agent.transform.rotation = Quaternion.Slerp(Agent.transform.rotation, b, Time.deltaTime * MaxTurningSpeed * 0.1f);
		}

		public void TryToSetDestinationTo(Vector3 point)
		{
			Vector3 vector = point;
			Vector3 vector2 = Vector3.down * 0.5f;
			if (NavMesh.SamplePosition(vector + vector2, out m_nHit, 1.9f, -1))
			{
				vector = m_nHit.position;
			}
			SetNewNavDestination(vector);
		}

		private void CheckIfAtDestination()
		{
			float num = Vector3.Distance(m_currentDestination, BaseRB.position);
			if (num < DestinationThreshold)
			{
				IsAtDestination = true;
			}
			else
			{
				IsAtDestination = false;
			}
		}

		private void CheckIfNeedNewNavAgentPath()
		{
			if (IsAtDestination)
			{
				return;
			}
			if (!Agent.hasPath && !Agent.pathPending)
			{
				float num = Vector3.Distance(m_currentDestination, Agent.transform.position);
				if (num > DestinationThreshold)
				{
					Agent.SetDestination(m_currentDestination);
				}
			}
			else
			{
				float num2 = Vector3.Distance(m_currentDestination, Agent.destination);
				if (num2 > DestinationThreshold)
				{
					Agent.SetDestination(m_currentDestination);
				}
			}
		}

		private void CheckIfNeedNewPath(Vector3 target)
		{
			Vector3 vector = m_currentNavTarget;
			Vector3 vector2 = UnityEngine.Random.onUnitSphere * 0.5f;
			vector2.y = 0f;
			Vector3 vector3 = Vector3.down * 0.5f;
			if (NavMesh.SamplePosition(target + vector2 + vector3, out m_nHit, 1.9f, -1))
			{
				vector = m_nHit.position;
			}
			float num = Vector3.Distance(vector, m_currentNavTarget);
			if (num > 2f)
			{
				m_currentNavTarget = vector;
				Agent.SetDestination(vector);
			}
		}
	}
}
