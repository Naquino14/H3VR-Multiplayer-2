using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class SosigHand
	{
		public enum SosigHandPose
		{
			AtRest,
			HipFire,
			Aimed,
			Melee,
			ShieldHold
		}

		public Sosig S;

		public Transform Root;

		public Transform Target;

		public bool IsHoldingObject;

		public SosigWeapon HeldObject;

		public bool IsRightHand = true;

		private Transform m_posedToward;

		public SosigHandPose Pose;

		public Transform Point_Aimed;

		public Transform Point_HipFire;

		public Transform Point_AtRest;

		public Transform Point_ShieldHold;

		private bool HasActiveAimPoint;

		private Vector3 m_aimTowardPoint = Vector3.zero;

		private List<float> vertOffsets = new List<float>
		{
			0f,
			0.2f,
			0.4f,
			0.65f,
			-0.1f,
			-0.2f
		};

		private List<float> forwardOffsets = new List<float>
		{
			0f,
			0.15f,
			0.1f,
			0.1f,
			0.05f,
			0.1f
		};

		private List<float> tiltLerpOffsets = new List<float>
		{
			0f,
			0.2f,
			0.4f,
			0.7f,
			0f,
			0f
		};

		private int m_curFiringPose_Hip;

		private int m_prevFiringPose_Hip;

		private int m_curFiringPose_Aimed;

		private int m_prevFiringPose_Aimed;

		private int m_nextPoseToTestToTarget;

		private float m_timeSincePoseChange;

		private RaycastHit m_hit;

		private List<bool> m_sightValid_Hip = new List<bool>
		{
			true,
			false,
			false
		};

		private List<bool> m_sightValid_Aimed = new List<bool>
		{
			true,
			false,
			false
		};

		private float m_timeAwayFromTarget;

		private Vector3 m_lastPos = Vector3.zero;

		public void SetHandPose(SosigHandPose s)
		{
			if (Pose == s)
			{
				return;
			}
			Pose = s;
			if (IsHoldingObject && HeldObject.Type == SosigWeapon.SosigWeaponType.Melee && HeldObject.MeleeType == SosigWeapon.SosigMeleeWeaponType.Shield)
			{
				PoseToward(Point_ShieldHold);
				return;
			}
			switch (s)
			{
			case SosigHandPose.AtRest:
				PoseToward(Point_AtRest);
				break;
			case SosigHandPose.HipFire:
				PoseToward(Point_HipFire);
				break;
			case SosigHandPose.Aimed:
				PoseToward(Point_Aimed);
				break;
			}
		}

		private void UpdateGunHandlingPose()
		{
			if (!HasActiveAimPoint)
			{
				return;
			}
			LayerMask lM_VisualOcclusionCheck = S.E.LM_VisualOcclusionCheck;
			Transform transform = S.Links[1].transform;
			Vector3 vector = Point_HipFire.position + transform.up * vertOffsets[m_nextPoseToTestToTarget];
			Vector3 vector2 = m_aimTowardPoint + UnityEngine.Random.onUnitSphere * 0.05f;
			Vector3 vector3 = vector2 - vector;
			if (!Physics.Raycast(vector, vector3.normalized, out m_hit, Mathf.Min(vector3.magnitude, 300f), lM_VisualOcclusionCheck, QueryTriggerInteraction.Ignore))
			{
				m_sightValid_Hip[m_nextPoseToTestToTarget] = true;
			}
			else
			{
				m_sightValid_Hip[m_nextPoseToTestToTarget] = false;
			}
			vector = Point_Aimed.position + transform.up * vertOffsets[m_nextPoseToTestToTarget];
			vector2 = m_aimTowardPoint + UnityEngine.Random.onUnitSphere * 0.05f;
			vector3 = vector2 - vector;
			if (!Physics.Raycast(vector, vector3.normalized, out m_hit, Mathf.Min(vector3.magnitude, 300f), lM_VisualOcclusionCheck, QueryTriggerInteraction.Ignore))
			{
				m_sightValid_Aimed[m_nextPoseToTestToTarget] = true;
			}
			else
			{
				m_sightValid_Aimed[m_nextPoseToTestToTarget] = false;
			}
			m_nextPoseToTestToTarget++;
			if (m_nextPoseToTestToTarget >= m_sightValid_Hip.Count)
			{
				m_nextPoseToTestToTarget = 0;
			}
			if (m_timeSincePoseChange < 1f)
			{
				m_timeSincePoseChange += Time.deltaTime * 3f;
				return;
			}
			if (!m_sightValid_Hip[m_curFiringPose_Hip])
			{
				int num = -1;
				for (int i = 0; i < m_sightValid_Hip.Count; i++)
				{
					if (m_sightValid_Hip[i])
					{
						num = i;
						break;
					}
				}
				if (num > -1)
				{
					m_prevFiringPose_Hip = m_curFiringPose_Hip;
					m_curFiringPose_Hip = num;
					m_timeSincePoseChange = 0f;
				}
			}
			if (m_sightValid_Aimed[m_curFiringPose_Aimed])
			{
				return;
			}
			int num2 = -1;
			for (int j = 0; j < m_sightValid_Aimed.Count; j++)
			{
				if (m_sightValid_Aimed[j])
				{
					num2 = j;
					break;
				}
			}
			if (num2 > -1)
			{
				m_prevFiringPose_Aimed = m_curFiringPose_Aimed;
				m_curFiringPose_Aimed = num2;
				m_timeSincePoseChange = 0f;
			}
		}

		public void SetAimTowardPoint(Vector3 v)
		{
			m_aimTowardPoint = v;
			if (S.IsPanicFiring())
			{
				m_aimTowardPoint += UnityEngine.Random.onUnitSphere * 10f;
			}
			HasActiveAimPoint = true;
		}

		public void ClearAimPoint()
		{
			HasActiveAimPoint = false;
		}

		public void PoseToward(Transform t)
		{
			if (!(m_posedToward == t))
			{
				m_posedToward = t;
			}
		}

		public void SetPoseDirect(Vector3 pos, Quaternion rot)
		{
			Target.position = pos;
			Target.rotation = rot;
		}

		public void PickUp(SosigWeapon o)
		{
			HeldObject = o;
			HeldObject.IsHeldByBot = true;
			HeldObject.SosigHoldingThis = S;
			HeldObject.HandHoldingThis = this;
			IsHoldingObject = true;
			o.BotPickup(S);
			if (!S.IgnoreRBs.Contains(o.O.RootRigidbody))
			{
				S.IgnoreRBs.Add(o.O.RootRigidbody);
			}
			for (int i = 0; i < S.Links.Count; i++)
			{
			}
		}

		public void PutAwayHeldObject()
		{
		}

		public void DropHeldObject()
		{
			if (HeldObject != null && S.IgnoreRBs.Contains(HeldObject.O.RootRigidbody))
			{
				S.IgnoreRBs.Remove(HeldObject.O.RootRigidbody);
			}
			if (IsHoldingObject)
			{
				IsHoldingObject = false;
				HeldObject.SosigHoldingThis = null;
				HeldObject.IsHeldByBot = false;
				HeldObject.HandHoldingThis = null;
				HeldObject.BotDrop();
				HeldObject = null;
			}
		}

		public void ThrowObject(Vector3 velocity, Vector3 targPoint)
		{
			if (HeldObject != null && S.IgnoreRBs.Contains(HeldObject.O.RootRigidbody))
			{
				S.IgnoreRBs.Remove(HeldObject.O.RootRigidbody);
			}
			if (IsHoldingObject)
			{
				float magnitude = velocity.magnitude;
				velocity = velocity.normalized * Mathf.Max(magnitude, 5f);
				IsHoldingObject = false;
				HeldObject.SosigHoldingThis = null;
				HeldObject.IsHeldByBot = false;
				HeldObject.HandHoldingThis = null;
				HeldObject.BotDrop();
				HeldObject.O.transform.position = HeldObject.O.transform.position + velocity.normalized;
				HeldObject.O.transform.rotation = Quaternion.LookRotation(targPoint - HeldObject.O.transform.position);
				HeldObject.O.RootRigidbody.velocity = velocity;
				HeldObject.O.RootRigidbody.angularVelocity = Vector3.zero;
				HeldObject.O.RootRigidbody.angularDrag = 3f;
				HeldObject = null;
			}
		}

		public void Hold()
		{
			if (!IsHoldingObject || HeldObject == null || Root == null)
			{
				return;
			}
			UpdateGunHandlingPose();
			Vector3 position = Target.position;
			Quaternion rotation = Target.rotation;
			Vector3 position2 = HeldObject.RecoilHolder.position;
			Quaternion rotation2 = HeldObject.RecoilHolder.rotation;
			if (HeldObject.O.IsHeld)
			{
				float num = Vector3.Distance(position, position2);
				if (num > 0.7f)
				{
					DropHeldObject();
					return;
				}
			}
			else
			{
				float num2 = Vector3.Distance(position, position2);
				if (num2 < 0.2f)
				{
					m_timeAwayFromTarget = 0f;
				}
				else
				{
					m_timeAwayFromTarget += Time.deltaTime;
					if (m_timeAwayFromTarget > 1f)
					{
						HeldObject.O.RootRigidbody.position = position;
						HeldObject.O.RootRigidbody.rotation = rotation;
					}
				}
			}
			if ((HeldObject.Type == SosigWeapon.SosigWeaponType.Melee || HeldObject.Type == SosigWeapon.SosigWeaponType.Grenade) && HeldObject.O.MP.IsMeleeWeapon)
			{
				Vector3 v = Target.position - m_lastPos;
				v *= 1f / Time.deltaTime;
				HeldObject.O.SetFakeHand(v, Target.position);
			}
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			if (m_posedToward != null && Pose != SosigHandPose.Melee)
			{
				if (HasActiveAimPoint)
				{
					if (Pose == SosigHandPose.Aimed)
					{
						num3 = vertOffsets[m_curFiringPose_Aimed];
						num4 = forwardOffsets[m_curFiringPose_Aimed];
						num5 = tiltLerpOffsets[m_curFiringPose_Aimed];
					}
					else if (Pose == SosigHandPose.HipFire)
					{
						num3 = vertOffsets[m_curFiringPose_Hip];
						num4 = forwardOffsets[m_curFiringPose_Hip];
						num5 = tiltLerpOffsets[m_curFiringPose_Hip];
					}
				}
				Transform transform = S.Links[1].transform;
				float num6 = 4f;
				if (S.IsFrozen)
				{
					num6 = 0.25f;
				}
				if (S.IsSpeedUp)
				{
					num6 = 8f;
				}
				Target.position = Vector3.Lerp(position, m_posedToward.position + transform.up * num3 + m_posedToward.forward * num4, Time.deltaTime * num6);
				Target.rotation = Quaternion.Slerp(rotation, m_posedToward.rotation, Time.deltaTime * num6);
			}
			Vector3 vector = position2;
			Quaternion rotation3 = rotation2;
			Vector3 vector2 = position;
			Quaternion quaternion = rotation;
			if (HasActiveAimPoint && (Pose == SosigHandPose.HipFire || Pose == SosigHandPose.Aimed))
			{
				float num7 = 0f;
				float num8 = 0f;
				if (Pose == SosigHandPose.HipFire)
				{
					num7 = HeldObject.Hipfire_HorizontalLimit;
					num8 = HeldObject.Hipfire_VerticalLimit;
				}
				if (Pose == SosigHandPose.Aimed)
				{
					num7 = HeldObject.Aim_HorizontalLimit;
					num8 = HeldObject.Aim_VerticalLimit;
				}
				Vector3 vector3 = m_aimTowardPoint - position;
				Vector3 forward = Target.forward;
				Vector3 current = Vector3.RotateTowards(forward, Vector3.ProjectOnPlane(vector3, Target.right), num8 * 0.0174533f, 0f);
				Vector3 forward2 = Vector3.RotateTowards(current, vector3, num7 * 0.0174533f, 0f);
				if (num5 > 0f)
				{
					Vector3 localPosition = Target.transform.localPosition;
					localPosition.z = 0f;
					localPosition.y = 0f;
					localPosition.Normalize();
					Vector3 upwards = Vector3.Slerp(Target.up, localPosition.x * -Target.right, num5);
					quaternion = Quaternion.LookRotation(forward2, upwards);
				}
				else
				{
					quaternion = Quaternion.LookRotation(forward2, Target.up);
				}
			}
			Vector3 vector4 = vector2 - vector;
			Quaternion quaternion2 = quaternion * Quaternion.Inverse(rotation3);
			float deltaTime = Time.deltaTime;
			quaternion2.ToAngleAxis(out var angle, out var axis);
			float num9 = 0.5f;
			if (S.IsConfused)
			{
				num9 = 0.1f;
			}
			if (S.IsStunned)
			{
				num9 = 0.02f;
			}
			if (angle > 180f)
			{
				angle -= 360f;
			}
			if (angle != 0f)
			{
				Vector3 target = deltaTime * angle * axis * S.AttachedRotationMultiplier * num9;
				HeldObject.O.RootRigidbody.angularVelocity = Vector3.MoveTowards(HeldObject.O.RootRigidbody.angularVelocity, target, S.AttachedRotationFudge * 0.5f * Time.fixedDeltaTime);
			}
			Vector3 target2 = vector4 * S.AttachedPositionMultiplier * 0.5f * deltaTime;
			HeldObject.O.RootRigidbody.velocity = Vector3.MoveTowards(HeldObject.O.RootRigidbody.velocity, target2, S.AttachedPositionFudge * 0.5f * deltaTime);
			m_lastPos = Target.position;
		}
	}
}
