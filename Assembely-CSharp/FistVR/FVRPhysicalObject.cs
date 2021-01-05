using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FVRPhysicalObject : FVRInteractiveObject
	{
		public struct RigidbodyStoredParams
		{
			public float Mass;

			public float Drag;

			public float AngularDrag;

			public RigidbodyInterpolation Interpolation;

			public CollisionDetectionMode ColDetectMode;
		}

		public enum FVRPhysicalObjectSize
		{
			Small = 0,
			Medium = 1,
			Large = 2,
			Massive = 3,
			CantCarryBig = 5
		}

		[Serializable]
		public class FVRPhysicalSoundParams
		{
			public AudioClip[] Clips;

			public float ColSoundCooldown = 0.65f;

			public float ColSoundVolume = 0.1f;

			[HideInInspector]
			public bool m_hasCollisionSound;

			[HideInInspector]
			public AudioSource m_audioCollision;

			[HideInInspector]
			public float m_colSoundTick = 1f;
		}

		public enum ObjectToHandOverrideMode
		{
			None,
			Direct,
			Floating
		}

		public enum InterpStyle
		{
			Translate,
			Rotation
		}

		public enum Axis
		{
			X,
			Y,
			Z
		}

		[Serializable]
		public class MeleeParams
		{
			private FVRPhysicalObject m_obj;

			public bool IsMeleeWeapon;

			public List<Rigidbody> IgnoreRBs = new List<Rigidbody>();

			[Header("Transforms")]
			public Transform HandPoint;

			public Transform EndPoint;

			[Header("Damage Params")]
			public Vector3 BaseDamageBCP = new Vector3(0f, 0f, 0f);

			public Vector3 HighDamageBCP = new Vector3(0f, 0f, 0f);

			public Vector3 StabDamageBCP = new Vector3(0f, 0f, 0f);

			public Vector3 TearDamageBCP = new Vector3(0f, 0f, 0f);

			public List<Collider> HighDamageColliders;

			public List<Transform> HighDamageVectors;

			[Header("Pose Params")]
			public bool DoesCyclePosePoints;

			public Transform[] PosePoints;

			protected int m_poseIndex;

			[Header("Thrown Params")]
			public bool IsThrownDisposable;

			private bool m_isCountingDownToDispose;

			public bool m_isThrownAutoAim;

			private float m_countDownToDestroy = 10f;

			public LayerMask ThrownDetectMask;

			public bool StartThrownDisposalTickdownOnSpawn;

			public bool IsLongThrowable;

			public bool IsThrowableDirInverted = true;

			[Header("Stabbing Params")]
			public bool CanNewStab;

			public bool ForceStab;

			public float BladeLength = 1f;

			public float MassWhileStabbed = 10f;

			public Transform StabDirection;

			public float StabAngularThreshold = 20f;

			public List<Collider> StabColliders;

			public float StabVelocityRequirement = 1f;

			public bool CanTearOut;

			public float TearOutVelThreshold = 3f;

			protected bool m_isJointedToObject;

			protected FixedJoint m_stabJoint;

			protected Rigidbody m_stabTargetRB;

			protected Vector3 m_initialStabPointWorld;

			protected Vector3 m_initialStabPointLocal;

			protected Vector3 m_relativeStabDir;

			protected SosigLink m_stabbedLink;

			protected Vector3 m_initialPosOfStabbedThingLocal;

			[Header("Lodging Params")]
			public bool CanNewLodge;

			public float LodgeDepth = 0.04f;

			public float MassWhileLodged = 10f;

			public Transform[] LodgeDirections;

			public List<Collider> LodgeColliders;

			public float LodgeVelocityRequirement = 4f;

			public float DeLodgeVelocityRequirement = 1f;

			private bool m_isLodgedToObject;

			private FixedJoint m_lodgeJoint;

			private Rigidbody m_lodgeTargetRB;

			private Vector3 m_initialLodgeNormal;

			[Header("Sweep Damage")]
			public bool UsesSweepTesting;

			public bool UsesSweepDebug;

			public List<Transform> TestCols = new List<Transform>();

			public Transform SweepTransformStart;

			public Transform SweepTransformEnd;

			public LayerMask LM_DamageTest;

			protected bool m_isReadyToAim;

			private Vector3 m_lastHandPoint = Vector3.zero;

			private Vector3 m_lastEndPoint = Vector3.zero;

			private Vector3 handPointVelocity = Vector3.zero;

			private Vector3 endPointVelocity = Vector3.zero;

			private float m_pointDistance;

			private float m_lastangFlick;

			private float m_lastangFlickLinear;

			private Vector3 m_SweepPointStart;

			private Vector3 m_SweepPointEnd;

			private Vector3 m_lastSweepPointStart;

			private Vector3 m_lastSweepPointEnd;

			private RaycastHit m_dhit;

			private float m_timeSinceLastDamageDone;

			protected float timeSinceStateChange = 10f;

			protected Vector3 stabDistantPoint = Vector3.one;

			protected Vector3 stabInsidePoint = Vector3.one;

			protected float m_initialMass;

			protected bool m_initRot;

			public int PoseIndex => m_poseIndex;

			public bool IsCountingDownToDispose
			{
				get
				{
					return m_isCountingDownToDispose;
				}
				set
				{
					m_isCountingDownToDispose = value;
				}
			}

			public bool IsJointedToObject => m_isJointedToObject;

			public bool IsLodgedToObject => m_isLodgedToObject;

			public void SetReadyToAim(bool b)
			{
				m_isReadyToAim = b;
			}

			public bool GetReadyToAim()
			{
				return m_isReadyToAim;
			}

			public void InitMeleeParams(FVRPhysicalObject o)
			{
				m_obj = o;
				m_lastHandPoint = HandPoint.position;
				m_lastEndPoint = EndPoint.position;
				m_SweepPointStart = HandPoint.position;
				m_SweepPointEnd = HandPoint.position;
				m_lastSweepPointStart = HandPoint.position;
				m_lastSweepPointEnd = HandPoint.position;
				m_pointDistance = Vector3.Distance(HandPoint.position, EndPoint.position);
				if (m_obj.PoseOverride_Touch != null && GM.HMDMode == ControlMode.Oculus && PosePoints.Length > 0)
				{
					PosePoints[0].localPosition = m_obj.PoseOverride_Touch.localPosition;
					PosePoints[0].localRotation = m_obj.PoseOverride_Touch.localRotation;
				}
				if (StartThrownDisposalTickdownOnSpawn)
				{
					IsThrownDisposable = true;
					m_isCountingDownToDispose = true;
				}
			}

			public void SetPose(int i)
			{
				m_poseIndex = i;
			}

			public void UpdateTick(float t)
			{
				if (!IsMeleeWeapon)
				{
					return;
				}
				if (m_obj.IsHeld || m_obj.QuickbeltSlot != null)
				{
					m_countDownToDestroy = 10f;
				}
				if (!m_obj.IsHeld && m_isCountingDownToDispose)
				{
					m_countDownToDestroy -= Time.deltaTime;
					if (m_countDownToDestroy <= 0f)
					{
						UnityEngine.Object.Destroy(m_obj.gameObject);
					}
				}
			}

			public void MeleeUpdateInteraction(FVRViveHand hand)
			{
				if (!DoesCyclePosePoints)
				{
					return;
				}
				if (hand.IsInStreamlinedMode)
				{
					if (hand.Input.BYButtonDown)
					{
						m_poseIndex--;
						if (m_poseIndex < 0)
						{
							m_poseIndex = PosePoints.Length - 1;
						}
					}
					if (hand.Input.AXButtonDown)
					{
						m_poseIndex++;
						if (m_poseIndex >= PosePoints.Length)
						{
							m_poseIndex = 0;
						}
					}
				}
				else
				{
					if (!hand.Input.TouchpadDown)
					{
						return;
					}
					if (Vector2.Angle(hand.Input.TouchpadAxes, Vector2.left) < 45f)
					{
						m_poseIndex--;
						if (m_poseIndex < 0)
						{
							m_poseIndex = PosePoints.Length - 1;
						}
					}
					else if (Vector2.Angle(hand.Input.TouchpadAxes, Vector2.right) < 45f)
					{
						m_poseIndex++;
						if (m_poseIndex >= PosePoints.Length)
						{
							m_poseIndex = 0;
						}
					}
				}
			}

			public void MeleeEndInteraction(FVRViveHand hand)
			{
				if (!IsMeleeWeapon || !GetReadyToAim())
				{
					return;
				}
				SetReadyToAim(b: false);
				Vector3 velocity = m_obj.RootRigidbody.velocity;
				if (!(m_obj.RootRigidbody.velocity.magnitude > 0.3f))
				{
					return;
				}
				Collider[] array = Physics.OverlapCapsule(m_obj.transform.position, m_obj.transform.position + GM.CurrentPlayerBody.Head.forward * 40f, 4f, ThrownDetectMask, QueryTriggerInteraction.Collide);
				if (array.Length <= 0)
				{
					return;
				}
				float num = 40f;
				Collider collider = null;
				bool flag = false;
				Vector3 vector = Vector3.one;
				for (int i = 0; i < array.Length; i++)
				{
					AIEntity component = array[i].transform.gameObject.GetComponent<AIEntity>();
					if (component.IFFCode < 0)
					{
						continue;
					}
					Vector3 vector2 = array[i].transform.position - m_obj.transform.position;
					float num2 = Vector3.Angle(velocity, vector2);
					if (!(num2 > 25f) && !(num2 < 1f))
					{
						float magnitude = vector2.magnitude;
						if (magnitude < num)
						{
							num = magnitude;
							collider = array[i];
							vector = vector2;
							flag = true;
						}
					}
				}
				if (flag)
				{
					Debug.Log("Found one");
					vector.y = GM.CurrentPlayerBody.Head.forward.y + 0.1f;
					m_obj.RootRigidbody.velocity = vector.normalized * velocity.magnitude * 2f;
				}
			}

			public void FixedUpdate(float t)
			{
				if (!IsMeleeWeapon)
				{
					return;
				}
				if (IsLongThrowable && !m_obj.IsHeld && !m_obj.RootRigidbody.isKinematic && m_obj.RootRigidbody.velocity.magnitude > 5f)
				{
					Vector3 vector = m_obj.RootRigidbody.velocity.normalized;
					if (IsThrowableDirInverted)
					{
						vector = -vector;
					}
					m_obj.RootRigidbody.MoveRotation(Quaternion.Slerp(m_obj.RootRigidbody.rotation, Quaternion.LookRotation(vector, Vector3.up), Time.deltaTime * 18.5f));
				}
				if (timeSinceStateChange < 2f)
				{
					timeSinceStateChange += Time.deltaTime * 1f;
				}
				if (m_obj.RootRigidbody != null && HandPoint != null && EndPoint != null)
				{
					if (DoesCyclePosePoints)
					{
						m_obj.PoseOverride.position = Vector3.Slerp(m_obj.PoseOverride.position, PosePoints[m_poseIndex].position, Time.deltaTime * 6f);
						m_obj.PoseOverride.rotation = Quaternion.Slerp(m_obj.PoseOverride.rotation, PosePoints[m_poseIndex].rotation, Time.deltaTime * 6f);
					}
					if (m_timeSinceLastDamageDone < 1f)
					{
						m_timeSinceLastDamageDone += Time.deltaTime;
					}
					handPointVelocity = Vector3.ClampMagnitude(handPointVelocity, Mathf.Lerp(handPointVelocity.magnitude, 0f, Time.deltaTime * 3f));
					endPointVelocity = Vector3.ClampMagnitude(endPointVelocity, Mathf.Lerp(endPointVelocity.magnitude, 0f, Time.deltaTime * 3f));
					handPointVelocity += m_obj.RootRigidbody.GetPointVelocity(HandPoint.position) * Time.deltaTime;
					endPointVelocity += m_obj.RootRigidbody.GetPointVelocity(EndPoint.position) * Time.deltaTime;
				}
				if (UsesSweepTesting)
				{
					m_SweepPointStart = m_obj.GetClosestValidPoint(HandPoint.position, EndPoint.position, SweepTransformStart.position);
					m_SweepPointEnd = m_obj.GetClosestValidPoint(HandPoint.position, EndPoint.position, SweepTransformEnd.position);
				}
				HashSet<IFVRDamageable> hashSet = new HashSet<IFVRDamageable>();
				if (UsesSweepTesting && (m_obj.RootRigidbody.velocity.magnitude > 3.5f || m_obj.RootRigidbody.angularVelocity.magnitude > 3.5f) && m_obj.IsHeld)
				{
					bool flag = false;
					for (int i = 0; i < 10; i++)
					{
						float t2 = (float)i / 10f;
						Vector3 vector2 = Vector3.Lerp(m_lastSweepPointStart, m_lastSweepPointEnd, t2);
						Vector3 vector3 = Vector3.Lerp(m_SweepPointStart, m_SweepPointEnd, t2);
						Vector3 vector4 = vector3 - vector2;
						Vector3 vector5 = vector3 + vector4;
						if (UsesSweepDebug)
						{
							TestCols[i].transform.position = vector3;
							TestCols[i].transform.rotation = Quaternion.LookRotation(vector3 - vector2);
							TestCols[i].transform.localScale = new Vector3(0.01f, 0.01f, Vector3.Distance(vector3, vector2) * 3f);
						}
						Vector3 normalized = vector4.normalized;
						if (!Physics.Raycast(vector2, normalized, out m_dhit, vector4.magnitude, LM_DamageTest, QueryTriggerInteraction.Ignore))
						{
							continue;
						}
						IFVRDamageable component = m_dhit.collider.transform.gameObject.GetComponent<IFVRDamageable>();
						if (component == null && m_dhit.collider.attachedRigidbody != null)
						{
							component = m_dhit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
						}
						if (component == null || hashSet.Contains(component))
						{
							continue;
						}
						Damage damage = new Damage();
						damage.Class = Damage.DamageClass.Melee;
						damage.point = m_dhit.point;
						damage.hitNormal = m_dhit.normal;
						damage.Source_IFF = GM.CurrentPlayerBody.GetPlayerIFF();
						damage.strikeDir = m_obj.RootRigidbody.GetPointVelocity(m_dhit.point).normalized;
						damage.damageSize = 0.02f;
						damage.edgeNormal = m_obj.transform.forward;
						float num = Mathf.Clamp(GetIntertiaFromPoint(m_dhit.point).magnitude, 0f, 1f);
						if (num > 0.25f)
						{
							float multiplierForStrikeDir = GetMultiplierForStrikeDir(normalized, HighDamageVectors);
							if (multiplierForStrikeDir > 0.5f)
							{
								damage.Dam_Blunt = HighDamageBCP.x * num;
								damage.Dam_Cutting = HighDamageBCP.y * num;
								damage.Dam_Piercing = HighDamageBCP.z * num;
							}
							else
							{
								damage.Dam_Blunt = BaseDamageBCP.x * num;
								damage.Dam_Cutting = BaseDamageBCP.y * num;
								damage.Dam_Piercing = BaseDamageBCP.z * num;
							}
							damage.Dam_TotalKinetic = damage.Dam_Blunt + damage.Dam_Cutting + damage.Dam_Piercing;
							if (GM.CurrentPlayerBody.IsMuscleMeat || GM.CurrentPlayerBody.IsWeakMeat)
							{
								damage.Dam_Blunt *= GM.CurrentPlayerBody.GetMuscleMeatPower();
								damage.Dam_Piercing *= GM.CurrentPlayerBody.GetMuscleMeatPower();
								damage.Dam_Piercing *= GM.CurrentPlayerBody.GetMuscleMeatPower();
								damage.Dam_TotalKinetic *= GM.CurrentPlayerBody.GetMuscleMeatPower();
							}
							m_timeSinceLastDamageDone = 0f;
							hashSet.Add(component);
							component.Damage(damage);
							flag = true;
						}
					}
					if (flag)
					{
						handPointVelocity = Vector3.zero;
						endPointVelocity = Vector3.zero;
					}
				}
				if (m_timeSinceLastDamageDone > 0.2f)
				{
					hashSet.Clear();
				}
				if (m_isJointedToObject && (m_stabJoint == null || m_stabTargetRB == null || m_stabbedLink == null))
				{
					m_isJointedToObject = false;
					m_obj.SetCollidersToLayer(StabColliders, triggersToo: false, "Default");
					m_obj.RootRigidbody.mass = m_initialMass;
					timeSinceStateChange = 0f;
				}
				if (m_isLodgedToObject && (m_lodgeJoint == null || m_lodgeTargetRB == null))
				{
					m_isLodgedToObject = false;
					m_obj.SetCollidersToLayer(LodgeColliders, triggersToo: false, "Default");
					m_obj.RootRigidbody.mass = m_initialMass;
					timeSinceStateChange = 0f;
				}
				if (m_isJointedToObject && (m_obj.IsHeld || m_obj.GettimeSinceFakeVelWorldTransfered() < 0.1f))
				{
					Vector3 handPosWorld = m_obj.GetHandPosWorld();
					Vector3 vector6 = handPosWorld - m_obj.PoseOverride.position;
					Vector3 position = m_obj.transform.position + vector6;
					Vector3 vector7 = m_stabTargetRB.transform.InverseTransformPoint(position);
					Vector3 closestValidPoint = m_obj.GetClosestValidPoint(stabDistantPoint, stabInsidePoint, vector7);
					m_stabJoint.connectedAnchor = closestValidPoint;
					if (CanStateChange())
					{
						bool flag2 = false;
						if (CanTearOut)
						{
							Vector3 handVelWorld = m_obj.GetHandVelWorld();
							if (handVelWorld.magnitude > TearOutVelThreshold && GetMultiplierForStrikeDir(handVelWorld, HighDamageVectors) >= 0.5f)
							{
								Vector3 point = Vector3.Lerp(EndPoint.position, EndPoint.position - StabDirection.forward * BladeLength * 0.5f, 0.5f);
								m_stabbedLink.S.SpawnLargeMustardBurst(point, handVelWorld);
								DoTearOutDamage(handVelWorld.magnitude, point, handVelWorld.normalized);
								flag2 = true;
								UnityEngine.Object.Destroy(m_stabJoint);
								m_isJointedToObject = false;
								m_stabTargetRB = null;
								m_stabbedLink = null;
								m_obj.SetCollidersToLayer(StabColliders, triggersToo: false, "Default");
								m_obj.RootRigidbody.mass = m_initialMass;
								timeSinceStateChange = 0f;
							}
						}
						if (!flag2 && Vector3.Distance(vector7, stabInsidePoint) > BladeLength)
						{
							UnityEngine.Object.Destroy(m_stabJoint);
							m_isJointedToObject = false;
							m_stabTargetRB = null;
							m_stabbedLink = null;
							m_obj.SetCollidersToLayer(StabColliders, triggersToo: false, "Default");
							m_obj.RootRigidbody.mass = m_initialMass;
							timeSinceStateChange = 0f;
						}
					}
				}
				if (CanStateChange() && m_isLodgedToObject && m_obj.IsHeld)
				{
					Vector3 handVelWorld2 = m_obj.GetHandVelWorld();
					if (handVelWorld2.magnitude > DeLodgeVelocityRequirement && Vector3.Angle(handVelWorld2, m_initialLodgeNormal) < 80f)
					{
						UnityEngine.Object.Destroy(m_lodgeJoint);
						m_isLodgedToObject = false;
						m_obj.SetCollidersToLayer(LodgeColliders, triggersToo: false, "Default");
						m_obj.RootRigidbody.mass = m_initialMass;
						timeSinceStateChange = 0f;
					}
				}
				m_lastSweepPointStart = m_SweepPointStart;
				m_lastSweepPointEnd = m_SweepPointEnd;
				m_lastHandPoint = HandPoint.position;
				m_lastEndPoint = EndPoint.position;
			}

			public void DeJoint()
			{
				if (m_isJointedToObject)
				{
					UnityEngine.Object.Destroy(m_stabJoint);
					m_isJointedToObject = false;
					m_stabTargetRB = null;
					m_stabbedLink = null;
					m_obj.SetCollidersToLayer(StabColliders, triggersToo: false, "Default");
					m_obj.RootRigidbody.mass = m_initialMass;
					timeSinceStateChange = 0f;
				}
				if (m_isLodgedToObject)
				{
					UnityEngine.Object.Destroy(m_lodgeJoint);
					m_isLodgedToObject = false;
					m_lodgeTargetRB = null;
					m_stabbedLink = null;
					m_obj.SetCollidersToLayer(StabColliders, triggersToo: false, "Default");
					m_obj.RootRigidbody.mass = m_initialMass;
					timeSinceStateChange = 0f;
				}
			}

			private bool GetInside(Collider col)
			{
				if (HighDamageColliders.Contains(col))
				{
					return true;
				}
				return false;
			}

			private bool GetIsLodgeCollider(Collider col)
			{
				if (LodgeColliders.Contains(col))
				{
					return true;
				}
				return false;
			}

			private Vector3 GetIntertiaFromPoint(Vector3 point)
			{
				Vector3 closestValidPoint = m_obj.GetClosestValidPoint(HandPoint.position, EndPoint.position, point);
				float num = Vector3.Distance(HandPoint.position, closestValidPoint);
				float t = num / m_pointDistance;
				float num2 = 1f;
				return Vector3.Lerp(handPointVelocity, endPointVelocity, t) * num2;
			}

			private float GetMultiplierForStrikeDir(Vector3 dir, Transform[] dirsToUse)
			{
				float num = 0f;
				for (int i = 0; i < dirsToUse.Length; i++)
				{
					num = Mathf.Max(num, Vector3.Dot(dir.normalized, dirsToUse[i].forward));
				}
				return num;
			}

			private float GetMultiplierForStrikeDir(Vector3 dir, List<Transform> dirsToUse)
			{
				float num = 0f;
				for (int i = 0; i < dirsToUse.Count; i++)
				{
					num = Mathf.Max(num, Vector3.Dot(dir.normalized, dirsToUse[i].forward));
				}
				return num;
			}

			private float GetMultiplierForStrikeDir(Vector3 dir, Transform dirToUse)
			{
				float a = 0f;
				return Mathf.Max(a, Vector3.Dot(dir.normalized, dirToUse.forward));
			}

			private void DoTearOutDamage(float mag, Vector3 point, Vector3 dir)
			{
				if (m_obj.IsHeld)
				{
					m_obj.m_hand.Buzz(m_obj.m_hand.Buzzer.Buzz_GunShot);
				}
				bool flag = false;
				IFVRDamageable iFVRDamageable = null;
				Damage damage = new Damage();
				damage.Class = Damage.DamageClass.Melee;
				damage.point = point;
				damage.hitNormal = -dir;
				damage.strikeDir = dir;
				damage.damageSize = 0.02f;
				damage.edgeNormal = m_obj.transform.forward;
				Vector3 zero = Vector3.zero;
				Vector3 zero2 = Vector3.zero;
				Vector3 zero3 = Vector3.zero;
				IFVRDamageable component = m_stabbedLink.gameObject.GetComponent<IFVRDamageable>();
				if (component != null)
				{
					iFVRDamageable = component;
					flag = true;
					Vector3 intertiaFromPoint = GetIntertiaFromPoint(point);
					zero = intertiaFromPoint;
					zero2 = point;
					zero3 = -dir;
					float num = Mathf.Clamp(zero.magnitude, 0f, 1f);
					if (m_obj.AltGrip != null && !m_obj.IsAltHeld)
					{
						num *= 3f;
					}
					num = Mathf.Clamp(num, 1f, 2f);
					if (flag && num > 0.25f)
					{
						damage.Dam_Blunt = TearDamageBCP.x * num;
						damage.Dam_Cutting = TearDamageBCP.y * num;
						damage.Dam_Piercing = TearDamageBCP.z * num;
						damage.Dam_TotalKinetic = damage.Dam_Blunt + damage.Dam_Cutting + damage.Dam_Piercing;
						iFVRDamageable.Damage(damage);
					}
					handPointVelocity = Vector3.zero;
					endPointVelocity = Vector3.zero;
				}
			}

			public virtual bool CanStateChange()
			{
				if (timeSinceStateChange > 0.25f)
				{
					return true;
				}
				return false;
			}

			public bool GetIsStabCollider(Collider col)
			{
				if (StabColliders.Contains(col))
				{
					return true;
				}
				return false;
			}

			protected void DoStabDamage(float mag, Collision col)
			{
				if (m_obj.IsHeld)
				{
					m_obj.m_hand.Buzz(m_obj.m_hand.Buzzer.Buzz_GunShot);
				}
				bool flag = false;
				bool flag2 = false;
				IFVRDamageable iFVRDamageable = null;
				Damage damage = new Damage();
				damage.Class = Damage.DamageClass.Melee;
				damage.point = col.contacts[0].point;
				damage.hitNormal = col.contacts[0].normal;
				damage.strikeDir = m_obj.RootRigidbody.GetPointVelocity(col.contacts[0].point).normalized;
				damage.damageSize = 0.02f;
				damage.edgeNormal = m_obj.transform.forward;
				Vector3 vector = Vector3.zero;
				Vector3 zero = Vector3.zero;
				Vector3 zero2 = Vector3.zero;
				for (int i = 0; i < col.contacts.Length; i++)
				{
					IFVRDamageable component = col.contacts[i].otherCollider.transform.gameObject.GetComponent<IFVRDamageable>();
					if (component == null && col.contacts[i].otherCollider.attachedRigidbody != null)
					{
						component = col.contacts[i].otherCollider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
					}
					if (component != null)
					{
						iFVRDamageable = component;
						flag2 = true;
						Vector3 intertiaFromPoint = GetIntertiaFromPoint(col.contacts[i].point);
						if (intertiaFromPoint.magnitude > vector.magnitude)
						{
							vector = intertiaFromPoint;
							zero = col.contacts[i].point;
							zero2 = col.contacts[i].normal;
						}
						if (GetIsStabCollider(col.contacts[i].thisCollider))
						{
							flag = true;
						}
					}
				}
				float num = Mathf.Clamp(vector.magnitude, 0f, 1f);
				if (m_obj.AltGrip != null && !m_obj.IsAltHeld)
				{
					num *= 2f;
				}
				num = Mathf.Clamp(num, 1f, 2f);
				if (flag2 && num > 0.25f)
				{
					float multiplierForStrikeDir = GetMultiplierForStrikeDir(vector.normalized, StabDirection);
					if (flag && multiplierForStrikeDir > 0.25f)
					{
						damage.Dam_Blunt = StabDamageBCP.x * num;
						damage.Dam_Cutting = StabDamageBCP.y * num;
						damage.Dam_Piercing = StabDamageBCP.z * num;
					}
					else
					{
						damage.Dam_Blunt = BaseDamageBCP.x * num;
						damage.Dam_Cutting = BaseDamageBCP.y * num;
						damage.Dam_Piercing = BaseDamageBCP.z * num;
					}
					damage.Dam_TotalKinetic = damage.Dam_Blunt + damage.Dam_Cutting + damage.Dam_Piercing;
					iFVRDamageable.Damage(damage);
				}
				handPointVelocity = Vector3.zero;
				endPointVelocity = Vector3.zero;
			}

			protected void JointMeObjectForStab(Rigidbody targetRB)
			{
				if (!m_isJointedToObject)
				{
					m_isJointedToObject = true;
					m_initRot = false;
					timeSinceStateChange = 0f;
					m_stabTargetRB = targetRB;
					m_stabJoint = m_obj.gameObject.AddComponent<FixedJoint>();
					m_stabJoint.autoConfigureConnectedAnchor = true;
					m_stabJoint.connectedBody = targetRB;
					Vector3 vector = m_stabJoint.connectedBody.transform.TransformPoint(m_stabJoint.connectedAnchor);
					stabDistantPoint = m_stabJoint.connectedAnchor;
					Vector3 vector2 = m_stabJoint.connectedBody.transform.InverseTransformDirection(StabDirection.forward);
					stabInsidePoint = stabDistantPoint + vector2 * BladeLength;
					m_stabJoint.autoConfigureConnectedAnchor = false;
					m_stabJoint.connectedAnchor = Vector3.Lerp(stabDistantPoint, stabInsidePoint, 0.5f);
					m_obj.SetCollidersToLayer(StabColliders, triggersToo: false, "NoCol");
					m_initialMass = m_obj.RootRigidbody.mass;
					m_obj.RootRigidbody.mass = MassWhileStabbed;
				}
			}

			private void JointToObjectForLodge(Rigidbody targetRB, Vector3 anchorPoint)
			{
				if (!m_isLodgedToObject)
				{
					m_isLodgedToObject = true;
					timeSinceStateChange = 0f;
					m_lodgeTargetRB = targetRB;
					m_lodgeJoint = targetRB.gameObject.AddComponent<FixedJoint>();
					m_lodgeJoint.enableCollision = false;
					m_lodgeJoint.connectedBody = m_obj.RootRigidbody;
					m_obj.SetCollidersToLayer(LodgeColliders, triggersToo: false, "NoCol");
					m_initialMass = m_obj.RootRigidbody.mass;
					m_obj.RootRigidbody.mass = MassWhileStabbed;
				}
			}

			public void OnCollisionEnter(Collision col)
			{
				bool flag = false;
				if (col.collider.attachedRigidbody != null)
				{
					flag = true;
				}
				if (flag && IgnoreRBs.Contains(col.collider.attachedRigidbody))
				{
					return;
				}
				float magnitude = col.relativeVelocity.magnitude;
				bool flag2 = false;
				bool flag3 = false;
				if (CanNewStab && !m_isLodgedToObject && !m_isJointedToObject && CanStateChange() && GetIsStabCollider(col.contacts[0].thisCollider) && magnitude > StabVelocityRequirement && col.collider.attachedRigidbody != null && Vector3.Angle(col.contacts[0].normal, StabDirection.forward) > 120f)
				{
					Vector3 to = -col.relativeVelocity;
					if (m_obj.IsHeld)
					{
						to = m_obj.m_hand.Input.VelLinearWorld;
					}
					float num = Vector3.Angle(StabDirection.forward, to);
					if (num < StabAngularThreshold)
					{
						flag2 = true;
						if (col.collider.transform.parent != null)
						{
							SosigLink component = col.collider.transform.parent.gameObject.GetComponent<SosigLink>();
							if (component != null)
							{
								DoStabDamage(magnitude, col);
								bool flag4 = false;
								SosigWearable outwear = null;
								if (component.HitsWearable(col.contacts[0].point + col.contacts[0].normal, -col.contacts[0].normal, 1.1f, out outwear) && !outwear.IsStabbable && !ForceStab)
								{
									flag4 = true;
								}
								if (!component.S.CanBeStabbed && !ForceStab)
								{
									flag4 = true;
								}
								if (!flag4)
								{
									m_stabbedLink = component;
									JointMeObjectForStab(col.collider.attachedRigidbody);
								}
							}
						}
					}
				}
				if (flag2 || !(magnitude > 1f) || !(m_timeSinceLastDamageDone > 0.2f))
				{
					return;
				}
				if (m_obj.IsHeld)
				{
					m_obj.m_hand.Buzz(m_obj.m_hand.Buzzer.Buzz_GunShot);
				}
				bool flag5 = false;
				IFVRDamageable iFVRDamageable = null;
				Damage damage = new Damage();
				damage.Class = Damage.DamageClass.Melee;
				damage.point = col.contacts[0].point;
				damage.hitNormal = col.contacts[0].normal;
				damage.strikeDir = m_obj.RootRigidbody.GetPointVelocity(col.contacts[0].point).normalized;
				damage.damageSize = 0.02f;
				damage.Source_IFF = GM.CurrentPlayerBody.GetPlayerIFF();
				damage.edgeNormal = m_obj.transform.forward;
				Vector3 vector = Vector3.zero;
				Vector3 zero = Vector3.zero;
				Vector3 zero2 = Vector3.zero;
				for (int i = 0; i < col.contacts.Length; i++)
				{
					IFVRDamageable component2 = col.contacts[i].otherCollider.transform.gameObject.GetComponent<IFVRDamageable>();
					if (component2 == null && col.contacts[i].otherCollider.attachedRigidbody != null)
					{
						component2 = col.contacts[i].otherCollider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
					}
					if (component2 != null)
					{
						iFVRDamageable = component2;
						flag5 = true;
						Vector3 intertiaFromPoint = GetIntertiaFromPoint(col.contacts[i].point);
						if (intertiaFromPoint.magnitude > vector.magnitude)
						{
							vector = intertiaFromPoint;
							zero = col.contacts[i].point;
							zero2 = col.contacts[i].normal;
						}
						if (GetInside(col.contacts[i].thisCollider))
						{
							flag3 = true;
						}
					}
				}
				float num2 = Mathf.Clamp(vector.magnitude, 0f, 1f);
				if (m_obj.AltGrip != null && !m_obj.IsAltHeld)
				{
					num2 *= 2f;
				}
				if (flag5 && num2 > 0.25f)
				{
					float multiplierForStrikeDir = GetMultiplierForStrikeDir(vector.normalized, HighDamageVectors);
					if (flag3 && multiplierForStrikeDir > 0.25f)
					{
						damage.Dam_Blunt = HighDamageBCP.x * num2;
						damage.Dam_Cutting = HighDamageBCP.y * num2;
						damage.Dam_Piercing = HighDamageBCP.z * num2;
					}
					else
					{
						damage.Dam_Blunt = BaseDamageBCP.x * num2;
						damage.Dam_Cutting = BaseDamageBCP.y * num2;
						damage.Dam_Piercing = BaseDamageBCP.z * num2;
					}
					damage.Dam_TotalKinetic = damage.Dam_Blunt + damage.Dam_Cutting + damage.Dam_Piercing;
					iFVRDamageable.Damage(damage);
				}
				if (CanNewLodge && !m_isLodgedToObject && !m_isLodgedToObject && CanStateChange() && m_obj.RootRigidbody.GetPointVelocity(col.contacts[0].point).magnitude > LodgeVelocityRequirement && GetIsLodgeCollider(col.contacts[0].thisCollider) && col.collider.attachedRigidbody != null)
				{
					Vector3 dir = -col.relativeVelocity;
					if (m_obj.IsHeld)
					{
						dir = m_obj.m_hand.Input.VelLinearWorld;
					}
					float multiplierForStrikeDir2 = GetMultiplierForStrikeDir(dir, LodgeDirections);
					if (multiplierForStrikeDir2 > 0.6f)
					{
						SosigLink component3 = col.collider.attachedRigidbody.gameObject.GetComponent<SosigLink>();
						if (component3 != null && !component3.GetHasJustBeenSevered())
						{
							bool flag6 = false;
							SosigWearable outwear2 = null;
							if (component3.HitsWearable(col.contacts[0].point + col.contacts[0].normal, -col.contacts[0].normal, 1.1f, out outwear2) && !outwear2.IsLodgeable)
							{
								flag6 = true;
							}
							if (!flag6)
							{
								m_initialLodgeNormal = col.contacts[0].normal;
								JointToObjectForLodge(col.collider.attachedRigidbody, col.contacts[0].point);
							}
						}
					}
				}
				handPointVelocity = Vector3.zero;
				endPointVelocity = Vector3.zero;
			}
		}

		[Header("Physical Object Config")]
		public FVRObject ObjectWrapper;

		public bool SpawnLockable;

		public bool Harnessable;

		public HandlingReleaseIntoSlotType HandlingReleaseIntoSlotSound = HandlingReleaseIntoSlotType.Generic;

		[HideInInspector]
		public bool m_isSpawnLock;

		[HideInInspector]
		public bool m_isHardnessed;

		public FVRPhysicalObjectSize Size;

		public FVRQuickBeltSlot.QuickbeltSlotType QBSlotType;

		public float ThrowVelMultiplier = 1f;

		public float ThrowAngMultiplier = 1f;

		protected float AttachedRotationMultiplier = 60f;

		protected float AttachedPositionMultiplier = 9000f;

		protected float AttachedRotationFudge = 10000f;

		protected float AttachedPositionFudge = 10000f;

		public bool UsesGravity = true;

		public Rigidbody[] DependantRBs;

		protected RigidbodyStoredParams StoredRBParams = default(RigidbodyStoredParams);

		public bool DistantGrabbable = true;

		public bool IsDebug;

		public bool IsAltHeld;

		public bool IsKinematicLocked;

		public bool DoesQuickbeltSlotFollowHead;

		public bool IsInWater;

		private Vector3 m_storedCOMLocal = Vector3.zero;

		public List<FVRFireArmAttachmentMount> AttachmentMounts;

		private List<FVRFireArmAttachment> AttachmentsList = new List<FVRFireArmAttachment>();

		private HashSet<FVRFireArmAttachment> AttachmentsHash = new HashSet<FVRFireArmAttachment>();

		private FVRQuickBeltSlot m_quickbeltSlot;

		private float m_timeSinceInQuickbelt = 10f;

		private FVRAlternateGrip m_altGrip;

		private FVRAlternateGrip savedGrip;

		public bool IsAltToAltTransfer;

		private FVRFireArmBipod m_bipod;

		private bool m_isPivotLocked;

		private Vector3 m_pivotLockedPos = Vector3.zero;

		private Quaternion m_pivotLockedRot = Quaternion.identity;

		public FVRPhysicalSoundParams CollisionSound;

		private ItemSpawnerID m_IDSpawnedFrom;

		public bool IsPickUpLocked;

		private bool m_doesDirectParent = true;

		public ObjectToHandOverrideMode OverridesObjectToHand;

		protected AudioImpactController audioImpactController;

		private bool m_hasImpactController;

		[Header("Melee Stuff")]
		public MeleeParams MP;

		private float CheckDestroyTick = 1f;

		private Vector3 fakeHandPosWorld = Vector3.zero;

		private Vector3 fakeHandVelWorld = Vector3.zero;

		private float m_timeSinceFakeVelWorldTransfered = 1f;

		[HideInInspector]
		public Rigidbody RootRigidbody
		{
			get;
			set;
		}

		public List<FVRFireArmAttachment> Attachments => AttachmentsList;

		public float TimeSinceInQuickbelt => m_timeSinceInQuickbelt;

		[HideInInspector]
		public FVRQuickBeltSlot QuickbeltSlot => m_quickbeltSlot;

		[HideInInspector]
		public FVRAlternateGrip AltGrip
		{
			get
			{
				return m_altGrip;
			}
			set
			{
				if (!(m_altGrip != null) || value != m_altGrip)
				{
				}
				if (value != null)
				{
				}
				m_altGrip = value;
			}
		}

		[HideInInspector]
		public FVRFireArmBipod Bipod
		{
			get
			{
				return m_bipod;
			}
			set
			{
				if (!(m_bipod != null) || value != m_bipod)
				{
				}
				if (value != null)
				{
				}
				m_bipod = value;
			}
		}

		public bool IsPivotLocked
		{
			get
			{
				return m_isPivotLocked;
			}
			set
			{
				m_isPivotLocked = value;
			}
		}

		public Vector3 PivotLockPos
		{
			set
			{
				m_pivotLockedPos = value;
			}
		}

		public Quaternion PivotLockRot
		{
			set
			{
				m_pivotLockedRot = value;
			}
		}

		public ItemSpawnerID IDSpawnedFrom
		{
			get
			{
				return m_IDSpawnedFrom;
			}
			set
			{
				m_IDSpawnedFrom = value;
			}
		}

		public bool DoesDirectParent => m_doesDirectParent;

		public AudioImpactController AudioImpactController => audioImpactController;

		public bool HasImpactController => m_hasImpactController;

		public virtual void SetQuickBeltSlot(FVRQuickBeltSlot slot)
		{
			if (slot != null && !base.IsHeld)
			{
				if (AttachmentsList.Count > 0)
				{
					for (int i = 0; i < AttachmentsList.Count; i++)
					{
						if (AttachmentsList[i] != null)
						{
							AttachmentsList[i].SetAllCollidersToLayer(triggersToo: false, "NoCol");
						}
					}
				}
			}
			else if (AttachmentsList.Count > 0)
			{
				for (int j = 0; j < AttachmentsList.Count; j++)
				{
					if (AttachmentsList[j] != null)
					{
						AttachmentsList[j].SetAllCollidersToLayer(triggersToo: false, "Default");
					}
				}
			}
			if (m_quickbeltSlot != null && slot != m_quickbeltSlot)
			{
				m_quickbeltSlot.HeldObject = null;
				m_quickbeltSlot.CurObject = null;
				m_quickbeltSlot.IsKeepingTrackWithHead = false;
			}
			if (slot != null && !base.IsHeld)
			{
				SetAllCollidersToLayer(triggersToo: false, "NoCol");
				slot.HeldObject = this;
				slot.CurObject = this;
				slot.IsKeepingTrackWithHead = DoesQuickbeltSlotFollowHead;
			}
			else
			{
				SetAllCollidersToLayer(triggersToo: false, "Default");
			}
			m_quickbeltSlot = slot;
		}

		public void SetIFF(int iff)
		{
			if (m_hasImpactController)
			{
				audioImpactController.SetIFF(iff);
			}
		}

		protected override void Awake()
		{
			if (GM.Options.QuickbeltOptions.ObjectToHandMode == QuickbeltOptions.ObjectToHandConnectionMode.Floating)
			{
				m_doesDirectParent = false;
			}
			if (OverridesObjectToHand == ObjectToHandOverrideMode.Floating)
			{
				m_doesDirectParent = false;
			}
			else if (OverridesObjectToHand == ObjectToHandOverrideMode.Direct)
			{
				m_doesDirectParent = true;
			}
			if (ObjectWrapper == null)
			{
				SpawnLockable = false;
			}
			base.Awake();
			if ((GetComponent<AudioSource>() != null) & (CollisionSound.Clips.Length > 0))
			{
				CollisionSound.m_colSoundTick = 1f;
				CollisionSound.m_hasCollisionSound = true;
				CollisionSound.m_audioCollision = GetComponent<AudioSource>();
			}
			RootRigidbody = GetComponent<Rigidbody>();
			if (RootRigidbody != null)
			{
				m_storedCOMLocal = RootRigidbody.centerOfMass;
				RootRigidbody.interpolation = RigidbodyInterpolation.None;
				RootRigidbody.maxAngularVelocity = 100f;
				StoredRBParams.Mass = RootRigidbody.mass;
				StoredRBParams.Drag = RootRigidbody.drag;
				StoredRBParams.AngularDrag = RootRigidbody.angularDrag;
				StoredRBParams.Interpolation = RootRigidbody.interpolation;
				StoredRBParams.ColDetectMode = RootRigidbody.collisionDetectionMode;
			}
			audioImpactController = GetComponent<AudioImpactController>();
			if (audioImpactController != null)
			{
				m_hasImpactController = true;
				audioImpactController.SetIFF(-3);
			}
			if (MP.IsMeleeWeapon)
			{
				MP.InitMeleeParams(this);
			}
		}

		private void UpdatePosesBasedOnCMode(FVRViveHand hand)
		{
			if ((hand.CMode == ControlMode.Oculus || hand.CMode == ControlMode.Index) && PoseOverride_Touch != null)
			{
				PoseOverride.localPosition = PoseOverride_Touch.localPosition;
				PoseOverride.localRotation = PoseOverride_Touch.localRotation;
			}
		}

		public virtual int GetTutorialState()
		{
			return 0;
		}

		public override bool IsInteractable()
		{
			if (IsPickUpLocked)
			{
				return false;
			}
			return base.IsInteractable();
		}

		public override bool IsSelectionRestricted()
		{
			if (QuickbeltSlot != null && !QuickbeltSlot.IsSelectable)
			{
				return true;
			}
			return base.IsSelectionRestricted();
		}

		public override bool IsDistantGrabbable()
		{
			if (IsPivotLocked)
			{
				return false;
			}
			if (Bipod != null && Bipod.IsBipodActive)
			{
				return false;
			}
			if (IsPickUpLocked)
			{
				return false;
			}
			return DistantGrabbable;
		}

		public void ToggleKinematicLocked()
		{
			if (QuickbeltSlot == null)
			{
				SetIsKinematicLocked(!IsKinematicLocked);
			}
		}

		public void SetIsKinematicLocked(bool b)
		{
			if (QuickbeltSlot == null)
			{
				IsKinematicLocked = b;
				if (!IsKinematicLocked && RootRigidbody.isKinematic)
				{
					RootRigidbody.isKinematic = false;
				}
				else if (IsKinematicLocked && !base.IsHeld)
				{
					RootRigidbody.isKinematic = true;
				}
			}
		}

		public virtual void BipodActivated()
		{
			UseFilteredHandPosition = true;
			UseSecondStepRotationFiltering = true;
			if (IsAltHeld && m_hand != null)
			{
				m_hand.EndInteractionIfHeld(this);
				EndInteraction(m_hand);
			}
			if (AltGrip != null && AltGrip.m_hand != null)
			{
				Debug.Log("ending");
				AltGrip.m_hand.ForceSetInteractable(null);
				AltGrip = null;
			}
			SetParentage(null);
		}

		public virtual void BipodDeactivated()
		{
			if (RootRigidbody.isKinematic)
			{
				RootRigidbody.isKinematic = false;
			}
			UseFilteredHandPosition = false;
			UseSecondStepRotationFiltering = false;
			if (base.IsHeld && base.transform.parent != m_hand.WholeRig)
			{
				SetParentage(m_hand.WholeRig);
			}
		}

		private void ResetClampCOM()
		{
			RootRigidbody.ResetCenterOfMass();
			RootRigidbody.centerOfMass = new Vector3(Mathf.Clamp(RootRigidbody.centerOfMass.x, m_storedCOMLocal.x - 0.15f, m_storedCOMLocal.x + 0.15f), Mathf.Clamp(RootRigidbody.centerOfMass.y, m_storedCOMLocal.y - 0.15f, m_storedCOMLocal.y + 0.15f), Mathf.Clamp(RootRigidbody.centerOfMass.z, m_storedCOMLocal.z - 0.15f, m_storedCOMLocal.z + 0.15f));
		}

		public void RegisterAttachment(FVRFireArmAttachment attachment)
		{
			if (AttachmentsHash.Add(attachment))
			{
				AttachmentsList.Add(attachment);
				ResetClampCOM();
			}
		}

		public void DeRegisterAttachment(FVRFireArmAttachment attachment)
		{
			if (AttachmentsHash.Remove(attachment))
			{
				AttachmentsList.Remove(attachment);
				ResetClampCOM();
			}
		}

		protected virtual Vector3 GetGrabPos()
		{
			Vector3 position = Transform.position;
			if (QBPoseOverride != null && QuickbeltSlot != null && !base.IsHeld)
			{
				return QBPoseOverride.position;
			}
			if (PoseOverride != null)
			{
				position = PoseOverride.position;
			}
			if (UseGrabPointChild && UseGripRotInterp && m_grabPointTransform != null && !IsAltHeld)
			{
				float pos_interp_tick = m_pos_interp_tick;
				return Vector3.Lerp(m_grabPointTransform.position, position, pos_interp_tick);
			}
			if ((UseGrabPointChild || IsAltHeld) && m_grabPointTransform != null && base.IsHeld)
			{
				return m_grabPointTransform.position;
			}
			return position;
		}

		protected virtual Quaternion GetGrabRot()
		{
			Quaternion quaternion = Transform.rotation;
			if (!base.IsHeld && QuickbeltSlot != null && QBPoseOverride != null)
			{
				quaternion = ((!QuickbeltSlot.UseStraightAxisAlignment) ? QBPoseOverride.rotation : base.transform.rotation);
			}
			else if (PoseOverride != null)
			{
				quaternion = PoseOverride.rotation;
			}
			if (UseGrabPointChild && UseGripRotInterp && m_grabPointTransform != null && !IsAltHeld && AltGrip == null)
			{
				float rot_interp_tick = m_rot_interp_tick;
				return Quaternion.Slerp(m_grabPointTransform.rotation, quaternion, rot_interp_tick);
			}
			if ((UseGrabPointChild || IsAltHeld) && m_grabPointTransform != null && AltGrip == null && base.IsHeld)
			{
				return m_grabPointTransform.rotation;
			}
			return quaternion;
		}

		protected virtual Vector3 GetPosTarget()
		{
			if (QuickbeltSlot != null && !base.IsHeld)
			{
				return QuickbeltSlot.PoseOverride.position;
			}
			return base.m_handPos;
		}

		protected virtual Quaternion GetRotTarget()
		{
			if (QuickbeltSlot != null && !base.IsHeld)
			{
				return QuickbeltSlot.PoseOverride.rotation;
			}
			return base.m_handRot;
		}

		protected override void FVRUpdate()
		{
			MP.UpdateTick(Time.deltaTime);
			if (m_timeSinceFakeVelWorldTransfered < 1f)
			{
				m_timeSinceFakeVelWorldTransfered += Time.deltaTime;
			}
		}

		protected override void FVRFixedUpdate()
		{
			if (Bipod != null)
			{
				if (AltGrip != null && Bipod.IsBipodActive)
				{
					Bipod.Deactivate();
				}
				else
				{
					Bipod.UpdateBipod();
				}
			}
			MP.FixedUpdate(Time.deltaTime);
			base.FVRFixedUpdate();
			FU();
		}

		private void FU()
		{
			float fixedDeltaTime = Time.fixedDeltaTime;
			if (m_timeSinceInQuickbelt < 10f)
			{
				m_timeSinceInQuickbelt += fixedDeltaTime;
			}
			if (m_quickbeltSlot != null)
			{
				m_timeSinceInQuickbelt = 0f;
			}
			if (CheckDestroyTick > 0f)
			{
				CheckDestroyTick -= fixedDeltaTime;
			}
			else
			{
				CheckDestroyTick = UnityEngine.Random.Range(1f, 1.5f);
				if (Transform.position.y < -1000f)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
			if (CollisionSound.m_hasCollisionSound && CollisionSound.m_colSoundTick > 0f)
			{
				CollisionSound.m_colSoundTick -= fixedDeltaTime;
			}
			if (!base.IsHeld && !(QuickbeltSlot != null) && !IsPivotLocked)
			{
				return;
			}
			if (RootRigidbody == null)
			{
				RecoverRigidbody();
			}
			if (UseGrabPointChild && UseGripRotInterp && !IsAltHeld)
			{
				if (Bipod != null && Bipod.IsBipodActive)
				{
					m_pos_interp_tick = 1f;
				}
				else if (m_pos_interp_tick < 1f)
				{
					m_pos_interp_tick += fixedDeltaTime * PositionInterpSpeed;
				}
				else
				{
					m_pos_interp_tick = 1f;
				}
				if (Bipod != null && Bipod.IsBipodActive)
				{
					m_rot_interp_tick = 1f;
				}
				else if (m_rot_interp_tick < 1f)
				{
					m_rot_interp_tick += fixedDeltaTime * RotationInterpSpeed;
				}
				else
				{
					m_rot_interp_tick = 1f;
				}
			}
			Vector3 vector;
			Vector3 vector2;
			Quaternion rotation;
			Quaternion quaternion;
			if (IsPivotLocked)
			{
				vector = m_pivotLockedPos;
				quaternion = m_pivotLockedRot;
				vector2 = base.transform.position;
				rotation = base.transform.rotation;
			}
			else
			{
				vector = GetPosTarget();
				quaternion = GetRotTarget();
				vector2 = GetGrabPos();
				rotation = GetGrabRot();
			}
			Vector3 vector3 = vector - vector2;
			Quaternion b = quaternion * Quaternion.Inverse(rotation);
			bool flag = false;
			if (Bipod != null && Bipod.IsBipodActive)
			{
				flag = true;
			}
			bool flag2 = false;
			if (this is BreakActionWeapon && (this as BreakActionWeapon).AltGrip != null && !(this as BreakActionWeapon).IsLatched)
			{
				flag2 = true;
			}
			if (IsPivotLocked)
			{
				vector3 = vector - vector2;
				b = quaternion * Quaternion.Inverse(rotation);
			}
			else if (((AltGrip != null && AltGrip.FunctionalityEnabled && !flag2) || flag) && !GM.Options.ControlOptions.UseGunRigMode2)
			{
				Vector3 position;
				Vector3 vector4;
				if (AltGrip != null)
				{
					position = AltGrip.GetPalmPos(m_doesDirectParent);
					vector4 = base.transform.InverseTransformPoint(AltGrip.PoseOverride.position);
				}
				else
				{
					position = Bipod.GetOffsetSavedWorldPoint();
					vector4 = base.transform.InverseTransformPoint(Bipod.GetBipodRootWorld());
				}
				Vector3 vector5 = base.transform.InverseTransformPoint(position);
				Vector3 vector6 = base.transform.InverseTransformPoint(PoseOverride.position);
				float z = Mathf.Max(PoseOverride.localPosition.z + 0.05f, vector5.z);
				Vector3 position2 = new Vector3(vector5.x - vector4.x, vector5.y - vector4.y, z);
				Vector3 vector7 = base.transform.TransformPoint(position2);
				Vector3 vector8 = Vector3.Cross(vector7 - base.transform.position, m_hand.transform.right);
				if (flag)
				{
					Vector3 from = Vector3.ProjectOnPlane(vector8, base.transform.forward);
					Vector3 vector9 = Vector3.ProjectOnPlane(Vector3.up, base.transform.forward);
					float num = Vector3.Angle(from, vector9);
					float t = Mathf.Clamp(num - 20f, 0f, 30f) * 0.1f;
					vector8 = Vector3.Slerp(vector9, vector8, t);
				}
				quaternion = Quaternion.LookRotation((vector7 - base.transform.position).normalized, vector8) * PoseOverride.localRotation;
				b = quaternion * Quaternion.Inverse(rotation);
				if (!flag && GM.Options.ControlOptions.UseVirtualStock && this is FVRFireArm && (this as FVRFireArm).HasActiveShoulderStock && (this as FVRFireArm).StockPos != null)
				{
					FVRFireArm fVRFireArm = this as FVRFireArm;
					Vector3 vector10 = fVRFireArm.transform.InverseTransformPoint(fVRFireArm.StockPos.position);
					float num2 = Mathf.Abs(vector10.z - position2.z) - (this as FVRFireArm).GetRecoilZ();
					Vector3 vector11 = fVRFireArm.transform.InverseTransformPoint(vector2);
					float num3 = Mathf.Abs(vector10.z - vector11.z) - (this as FVRFireArm).GetRecoilZ();
					Transform head = GM.CurrentPlayerBody.Head;
					Vector3 vector12 = head.transform.InverseTransformPoint(vector7);
					Vector3 vector13 = head.transform.InverseTransformPoint(vector);
					Vector3 position3 = GM.CurrentPlayerBody.Head.position - GM.CurrentPlayerBody.Head.forward * 0.1f - GM.CurrentPlayerBody.Head.up * 0.05f;
					Vector3 position4 = head.transform.InverseTransformPoint(position3);
					Vector3 vector14 = GM.CurrentPlayerBody.Head.transform.InverseTransformPoint(vector);
					position4.x += vector14.x;
					position4.y += vector14.y + 0.05f;
					Vector3 vector15 = head.TransformPoint(position4);
					Vector3 normalized = (vector7 - vector15).normalized;
					Vector3 vector16 = vector15 + normalized * num3;
					Vector3 a = vector16 - vector2;
					Quaternion quaternion2 = Quaternion.LookRotation((vector7 - vector15).normalized, vector8) * PoseOverride.localRotation;
					Quaternion a2 = quaternion2 * Quaternion.Inverse(rotation);
					float num4 = Vector3.Distance(head.position, vector);
					num4 = Mathf.Clamp(num4 - 0.1f, 0f, 1f);
					float t2 = num4 * 5f;
					vector3 = Vector3.Lerp(a, vector3, t2);
					b = Quaternion.Slerp(a2, b, t2);
				}
			}
			else if (base.IsHeld && AltGrip == null && !IsAltHeld && !GM.Options.ControlOptions.UseGunRigMode2 && GM.Options.ControlOptions.UseVirtualStock && this is FVRFireArm && (this as FVRFireArm).HasActiveShoulderStock && (this as FVRFireArm).StockPos != null)
			{
				FVRFireArm fVRFireArm2 = this as FVRFireArm;
				float num5 = Mathf.Abs(fVRFireArm2.transform.InverseTransformPoint(fVRFireArm2.StockPos.position).z);
				Transform head2 = GM.CurrentPlayerBody.Head;
				Vector3 position5 = GM.CurrentPlayerBody.Head.position - head2.forward * 0.1f - head2.up * 0.05f;
				Vector3 position6 = head2.transform.InverseTransformPoint(position5);
				Vector3 vector17 = head2.TransformPoint(position6);
				Vector3 normalized2 = (fVRFireArm2.PoseOverride.position - vector17).normalized;
				Vector3 vector18 = vector17 + normalized2 * num5;
				Vector3 a3 = vector18 - vector2;
				Quaternion quaternion3 = Quaternion.LookRotation((fVRFireArm2.PoseOverride.position - vector17).normalized, m_hand.PointingTransform.up) * PoseOverride.localRotation;
				Quaternion a4 = quaternion3 * Quaternion.Inverse(rotation);
				float num6 = Vector3.Distance(head2.position, vector);
				num6 = Mathf.Clamp(num6 - 0.1f, 0f, 1f);
				float a5 = num6 * 5f;
				float t3 = Vector3.Angle(head2.forward, m_hand.PointingTransform.forward) / 40f - 0.2f;
				a5 = Mathf.Lerp(a5, 1f, t3);
				vector3 = Vector3.Lerp(a3, vector3, a5);
				b = Quaternion.Slerp(a4, b, a5);
			}
			float num7 = 1f;
			b.ToAngleAxis(out var angle, out var axis);
			if (angle > 180f)
			{
				angle -= 360f;
			}
			if (angle != 0f)
			{
				Vector3 target = fixedDeltaTime * angle * axis * AttachedRotationMultiplier;
				RootRigidbody.angularVelocity = num7 * Vector3.MoveTowards(RootRigidbody.angularVelocity, target, AttachedRotationFudge * Time.fixedDeltaTime);
				if (UseSecondStepRotationFiltering)
				{
					float num8 = Mathf.Clamp(RootRigidbody.angularVelocity.magnitude * 0.35f, 0f, 1f);
					num8 *= num8;
					RootRigidbody.angularVelocity *= num8;
				}
			}
			Vector3 target2 = vector3 * AttachedPositionMultiplier * fixedDeltaTime;
			RootRigidbody.velocity = Vector3.MoveTowards(RootRigidbody.velocity, target2, AttachedPositionFudge * fixedDeltaTime);
		}

		public void SetFakeHand(Vector3 v, Vector3 p)
		{
			fakeHandPosWorld = p;
			fakeHandVelWorld = v;
			m_timeSinceFakeVelWorldTransfered = 0f;
		}

		public float GettimeSinceFakeVelWorldTransfered()
		{
			return m_timeSinceFakeVelWorldTransfered;
		}

		public Vector3 GetHandPosWorld()
		{
			if (m_timeSinceFakeVelWorldTransfered < 0.25f)
			{
				return fakeHandPosWorld;
			}
			if (base.IsHeld)
			{
				return m_hand.Input.Pos;
			}
			return Vector3.zero;
		}

		public Vector3 GetHandVelWorld()
		{
			if (m_timeSinceFakeVelWorldTransfered < 0.25f)
			{
				return fakeHandVelWorld;
			}
			if (base.IsHeld)
			{
				return m_hand.Input.VelLinearWorld;
			}
			return Vector3.zero;
		}

		public virtual GameObject DuplicateFromSpawnLock(FVRViveHand hand)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(ObjectWrapper.GetGameObject(), Transform.position, Transform.rotation);
			FVRPhysicalObject component = gameObject.GetComponent<FVRPhysicalObject>();
			if (component is FVREntityProxy)
			{
				(component as FVREntityProxy).Data.PrimeDataLists((component as FVREntityProxy).Flags);
			}
			hand.ForceSetInteractable(component);
			component.SetQuickBeltSlot(null);
			component.BeginInteraction(hand);
			if (MP.IsMeleeWeapon && component.MP.IsThrownDisposable)
			{
				component.MP.IsCountingDownToDispose = true;
				if (component.MP.m_isThrownAutoAim)
				{
					component.MP.SetReadyToAim(b: true);
					component.MP.SetPose(MP.PoseIndex);
				}
			}
			return gameObject;
		}

		public virtual void BeginInteractionThroughAltGrip(FVRViveHand hand, FVRAlternateGrip grip)
		{
			if (m_hasImpactController)
			{
				audioImpactController.SetIFF(GM.CurrentPlayerBody.GetPlayerIFF());
			}
			IsAltToAltTransfer = IsAltHeld;
			IsAltHeld = true;
			savedGrip = grip;
			BeginInteraction(hand);
			m_hand.ForceSetInteractable(this);
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			if (Bipod != null && Bipod.IsBipodActive)
			{
				RootRigidbody.isKinematic = false;
			}
			UpdatePosesBasedOnCMode(hand);
			RecoverDrag();
			if (m_hasImpactController)
			{
				audioImpactController.SetIFF(GM.CurrentPlayerBody.GetPlayerIFF());
			}
			if (m_isSpawnLock)
			{
				hand.EndInteractionIfHeld(this);
				DuplicateFromSpawnLock(hand);
			}
			else
			{
				if (base.IsHeld && IsAltHeld && savedGrip != null && m_hand != null)
				{
					if (!IsAltToAltTransfer)
					{
						IsAltHeld = false;
						AltGrip = savedGrip;
						if (AltGrip.HasLastGrabbedGrip())
						{
							AltGrip.BeginInteractionFromAttachedGrip(AltGrip.GetLastGrabbedGrip(), m_hand);
						}
						else
						{
							AltGrip.BeginInteractionFromAttachedGrip(null, m_hand);
						}
						m_hand.CurrentInteractable = AltGrip;
						savedGrip = null;
					}
					else
					{
						IsAltHeld = true;
						AltGrip = savedGrip;
						m_hand.CurrentInteractable = null;
					}
				}
				if (RootRigidbody != null)
				{
					if (IsKinematicLocked && RootRigidbody.isKinematic)
					{
						RootRigidbody.isKinematic = false;
					}
					if (!IsKinematicLocked)
					{
						RootRigidbody.isKinematic = false;
					}
					RecoverDrag();
				}
				if (m_doesDirectParent && (Transform.parent == null || Transform.parent != hand.WholeRig))
				{
					SetParentage(hand.WholeRig);
				}
				if (QuickbeltSlot != null && !m_isHardnessed)
				{
					SetQuickBeltSlot(null);
				}
				IsAltToAltTransfer = false;
				base.BeginInteraction(hand);
			}
			SetQuickBeltSlot(m_quickbeltSlot);
			if (Bipod != null && Bipod.IsBipodActive)
			{
				SetParentage(null);
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (MP.IsMeleeWeapon)
			{
				MP.MeleeUpdateInteraction(hand);
			}
			if (IsAltHeld && savedGrip != null)
			{
				savedGrip.PassHandInput(hand, this);
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			if (RootRigidbody != null)
			{
				if (IsKinematicLocked)
				{
					RootRigidbody.isKinematic = true;
				}
				else
				{
					RootRigidbody.isKinematic = false;
				}
			}
			if (MP.IsMeleeWeapon && (MP.IsJointedToObject || MP.IsLodgedToObject))
			{
				ClearQuickbeltState();
			}
			RootRigidbody.useGravity = UsesGravity;
			if (m_hand != null)
			{
				float num = 1f;
				if (GM.CurrentPlayerBody.IsMuscleMeat || GM.CurrentPlayerBody.IsWeakMeat)
				{
					num = GM.CurrentPlayerBody.GetMuscleMeatPower();
				}
				RootRigidbody.angularVelocity = m_hand.Input.VelAngularWorld * ThrowAngMultiplier;
				RootRigidbody.velocity = m_hand.GetThrowLinearVelWorld() * ThrowVelMultiplier * num;
			}
			SetParentage(null);
			Rigidbody[] dependantRBs = DependantRBs;
			foreach (Rigidbody rigidbody in dependantRBs)
			{
				if (rigidbody != null)
				{
					rigidbody.velocity = m_hand.Input.VelLinearWorld * ThrowVelMultiplier;
				}
			}
			base.EndInteraction(hand);
			if (IsAltHeld)
			{
				IsAltHeld = false;
			}
			if (AltGrip != null)
			{
				IsAltHeld = true;
				savedGrip = AltGrip;
				AltGrip.m_hand.HandMadeGrabReleaseSound();
				BeginInteraction(AltGrip.m_hand);
				AltGrip.m_hand.ForceSetInteractable(this);
			}
			else
			{
				IsAltHeld = false;
			}
			SetQuickBeltSlot(m_quickbeltSlot);
			MP.MeleeEndInteraction(hand);
			if (Bipod != null && Bipod.IsBipodActive)
			{
				RootRigidbody.isKinematic = true;
				SetParentage(null);
			}
		}

		public virtual void ForceObjectIntoInventorySlot(FVRQuickBeltSlot slot)
		{
			SetQuickBeltSlot(slot);
			SetParentage(QuickbeltSlot.QuickbeltRoot);
			if (m_grabPointTransform != null)
			{
				if (QBPoseOverride != null)
				{
					m_grabPointTransform.position = QBPoseOverride.position;
					m_grabPointTransform.rotation = QBPoseOverride.rotation;
				}
				else if (PoseOverride != null)
				{
					m_grabPointTransform.position = PoseOverride.position;
					m_grabPointTransform.rotation = PoseOverride.rotation;
				}
			}
			if (IsAltHeld)
			{
				IsAltHeld = false;
			}
			SetQuickBeltSlot(m_quickbeltSlot);
		}

		public virtual void EndInteractionIntoInventorySlot(FVRViveHand hand, FVRQuickBeltSlot slot)
		{
			if (Bipod != null)
			{
				Bipod.Contract(playSound: true);
				RootRigidbody.isKinematic = false;
			}
			SetQuickBeltSlot(hand.CurrentHoveredQuickbeltSlot);
			SetParentage(QuickbeltSlot.QuickbeltRoot);
			if (HandlingReleaseIntoSlotSound != 0)
			{
				SM.PlayHandlingReleaseIntoSlotSound(HandlingReleaseIntoSlotSound, hand.Input.Pos);
			}
			if (m_grabPointTransform != null)
			{
				if (QBPoseOverride != null)
				{
					m_grabPointTransform.position = QBPoseOverride.position;
					m_grabPointTransform.rotation = QBPoseOverride.rotation;
				}
				else if (PoseOverride != null)
				{
					m_grabPointTransform.position = PoseOverride.position;
					m_grabPointTransform.rotation = PoseOverride.rotation;
				}
			}
			if (IsAltHeld)
			{
				IsAltHeld = false;
			}
			if (AltGrip != null)
			{
				AltGrip.m_hand.EndInteractionIfHeld(AltGrip);
				AltGrip.EndInteraction(AltGrip.m_hand);
			}
			base.EndInteraction(hand);
			SetQuickBeltSlot(m_quickbeltSlot);
		}

		public override void ForceBreakInteraction()
		{
			base.ForceBreakInteraction();
			if (RootRigidbody != null)
			{
				RootRigidbody.useGravity = UsesGravity;
			}
		}

		public virtual void ClearQuickbeltState()
		{
			m_isSpawnLock = false;
			m_isHardnessed = false;
			SetQuickBeltSlot(null);
		}

		public virtual void ToggleQuickbeltState()
		{
			if (SpawnLockable)
			{
				m_isSpawnLock = !m_isSpawnLock;
			}
			else if (Harnessable)
			{
				m_isHardnessed = !m_isHardnessed;
				if (!m_isHardnessed && base.IsHeld)
				{
					SetQuickBeltSlot(null);
				}
				else
				{
					SetQuickBeltSlot(m_quickbeltSlot);
				}
			}
		}

		public virtual void OnCollisionEnter(Collision col)
		{
			if (base.gameObject.activeInHierarchy && CollisionSound.m_hasCollisionSound && CollisionSound.m_colSoundTick <= 0f && (double)col.relativeVelocity.magnitude >= 0.1 && col.collider.attachedRigidbody == null)
			{
				CollisionSound.m_colSoundTick = CollisionSound.ColSoundCooldown;
				CollisionSound.m_audioCollision.PlayOneShot(CollisionSound.Clips[UnityEngine.Random.Range(0, CollisionSound.Clips.Length)], CollisionSound.ColSoundVolume * Mathf.Clamp(col.relativeVelocity.magnitude * 0.5f, 0f, 1f));
			}
			if (MP.IsMeleeWeapon)
			{
				MP.OnCollisionEnter(col);
			}
		}

		public void StoreAndDestroyRigidbody()
		{
			if (RootRigidbody != null)
			{
				StoredRBParams.Mass = RootRigidbody.mass;
				StoredRBParams.Drag = RootRigidbody.drag;
				StoredRBParams.AngularDrag = RootRigidbody.angularDrag;
				StoredRBParams.Interpolation = RootRigidbody.interpolation;
				StoredRBParams.ColDetectMode = RootRigidbody.collisionDetectionMode;
				UnityEngine.Object.Destroy(RootRigidbody);
			}
		}

		public void RecoverRigidbody()
		{
			if (RootRigidbody == null)
			{
				RootRigidbody = base.gameObject.AddComponent<Rigidbody>();
				RootRigidbody.mass = StoredRBParams.Mass;
				RootRigidbody.drag = StoredRBParams.Drag;
				RootRigidbody.angularDrag = StoredRBParams.AngularDrag;
				RootRigidbody.interpolation = StoredRBParams.Interpolation;
				RootRigidbody.collisionDetectionMode = StoredRBParams.ColDetectMode;
				RootRigidbody.maxAngularVelocity = 100f;
			}
		}

		public void RecoverDrag()
		{
			if (RootRigidbody != null)
			{
				RootRigidbody.drag = StoredRBParams.Drag;
				RootRigidbody.angularDrag = StoredRBParams.AngularDrag;
			}
		}

		public void SetParentage(Transform t)
		{
			Transform.SetParent(t);
		}

		public virtual void ConfigureFromFlagDic(Dictionary<string, string> f)
		{
		}

		public virtual Dictionary<string, string> GetFlagDic()
		{
			return null;
		}

		public override void TestHandDistance()
		{
		}

		[ContextMenu("TestCollidersNegative")]
		public void TestCollidersNegative()
		{
		}

		public void SetAnimatedComponent(Transform t, float val, InterpStyle interp, Axis axis)
		{
			switch (interp)
			{
			case InterpStyle.Rotation:
			{
				Vector3 zero = Vector3.zero;
				switch (axis)
				{
				case Axis.X:
					zero.x = val;
					break;
				case Axis.Y:
					zero.y = val;
					break;
				case Axis.Z:
					zero.z = val;
					break;
				}
				t.localEulerAngles = zero;
				break;
			}
			case InterpStyle.Translate:
			{
				Vector3 localPosition = t.localPosition;
				switch (axis)
				{
				case Axis.X:
					localPosition.x = val;
					break;
				case Axis.Y:
					localPosition.y = val;
					break;
				case Axis.Z:
					localPosition.z = val;
					break;
				}
				t.localPosition = localPosition;
				break;
			}
			}
		}
	}
}
