using UnityEngine;

namespace FistVR
{
	public class FVRMovementManager : MonoBehaviour
	{
		public enum MovementMode
		{
			Teleport,
			Dash,
			SingleTwoAxis,
			SlideToTarget,
			Armswinger,
			JoystickTeleport,
			TwinStick
		}

		public enum MovementAxialOrigin
		{
			Hands,
			Head
		}

		public enum TeleportMode
		{
			Standard,
			FrontFacingMode
		}

		public enum TwoAxisMovementMode
		{
			Standard,
			Onward,
			LeveledHand,
			LeveledHead
		}

		public enum TwoAxisMovementConfirm
		{
			OnTouch,
			OnClick
		}

		public enum TwoAxisMovementStyle
		{
			LateralIsStrafe,
			LateralIsSnapTurn,
			LateralIsSmoothTurn
		}

		[Header("Body Connections")]
		public Transform Head;

		public Transform LeftHand;

		public Transform RightHand;

		public FVRPlayerBody Body;

		public FVRViveHand[] Hands;

		private FVRSceneSettings m_sceneSettings;

		private FVRViveHand m_authoratativeHand;

		public GameObject TouchpadIndicationArrowPrefab;

		public GameObject TouchpadIndicationArrowPrefab_HorizontalOnly;

		private GameObject m_touchpadArrows;

		private GameObject m_twinStickArrowsLeft;

		private GameObject m_twinStickArrowsRight;

		public GameObject JoyStickTeleportArrowPrefab;

		private GameObject m_joystickTPArrows;

		[Header("Teleportation Rig")]
		public GameObject MovementRigPrefab;

		public FVRMovementRig MovementRig;

		public MovementMode Mode;

		[Header("Teleport, Dash and Slide")]
		public LayerMask LM_TeleCast;

		public LayerMask LM_PointSearch;

		private bool m_hasValidPoint;

		private bool doSecondCast;

		private RaycastHit m_hit_ray;

		private Vector3 m_validPoint = Vector3.zero;

		private bool m_isInRotatePickMode;

		private bool m_hasValidRotateDir;

		private Vector3 m_validRotateDir = Vector3.zero;

		public bool m_teleportUsesRotation = true;

		private float m_teleportCooldown = 0.2f;

		private float m_teleportEnergy = 1f;

		private bool m_isDashing;

		private Vector3 m_DashTarget = Vector3.zero;

		public float DashSpeed = 50f;

		private bool m_isSliding;

		private Vector3 m_SlidingTarget = Vector3.zero;

		public float SlidingSpeed = 1.5f;

		private Vector3 m_twoAxisVelocity = Vector3.zero;

		private float m_twoAxisStepHeight = 0.4f;

		private bool m_twoAxisGrounded = true;

		private Vector3 m_groundPoint = Vector3.zero;

		private bool m_sprintingEngaged;

		private float m_timeSinceSprintDownClick = 1f;

		private float m_timeSinceSnapTurn = 1f;

		private float m_curArmSwingerImpetus;

		private float m_tarArmSwingerImpetus;

		private Vector3 m_armSwingerVelocity = Vector3.zero;

		private float m_armSwingerStepHeight = 0.4f;

		private bool m_armSwingerGrounded = true;

		private Vector3 m_worldPointDir = Vector3.zero;

		private bool m_joyStickTeleportInProgress;

		private float m_joyStickTeleportCooldown = 0.25f;

		private bool m_isLeftHandActive;

		private bool m_isRightHandActive;

		private float m_delayGroundCheck;

		public GameObject FloorHelperPrefab;

		private GameObject m_floorHelper;

		private FVRHandGrabPoint m_curGrabPoint;

		private Vector3 m_lastHandPos = Vector3.zero;

		private float m_topSpeedLastSecond;

		private Vector3 m_lastDir = Vector3.one;

		private Vector3[] m_dirs = new Vector3[5];

		private int whichDir;

		private bool dirSetThisFrame;

		private int executeFrameTick;

		private Vector3 lastHeadPos = Vector3.zero;

		private Vector3 correctionDir = Vector3.zero;

		private bool startedTP;

		private float testMin = 10f;

		private float testMax = -10f;

		private Vector3 CurNeckPos;

		private Vector3 LastNeckPos;

		private bool hasXAxisReset = true;

		private bool m_isTwinStickSmoothTurningCounterClockwise;

		private bool m_isTwinStickSmoothTurningClockwise;

		public Transform[] Cylinders;

		public void Awake()
		{
			GameObject gameObject = Object.Instantiate(MovementRigPrefab, Vector3.zero, Quaternion.identity);
			MovementRig = gameObject.GetComponent<FVRMovementRig>();
			gameObject.SetActive(value: false);
			m_touchpadArrows = Object.Instantiate(TouchpadIndicationArrowPrefab);
			m_touchpadArrows.SetActive(value: false);
			m_joystickTPArrows = Object.Instantiate(JoyStickTeleportArrowPrefab);
			m_joystickTPArrows.SetActive(value: false);
			m_twinStickArrowsLeft = Object.Instantiate(TouchpadIndicationArrowPrefab);
			m_twinStickArrowsLeft.SetActive(value: false);
			m_twinStickArrowsRight = Object.Instantiate(TouchpadIndicationArrowPrefab_HorizontalOnly);
			m_twinStickArrowsRight.SetActive(value: false);
			m_floorHelper = Object.Instantiate(FloorHelperPrefab);
			m_floorHelper.SetActive(value: false);
		}

		private void Start()
		{
			lastHeadPos = GM.CurrentPlayerBody.NeckJointTransform.transform.position;
			Mode = GM.Options.MovementOptions.CurrentMovementMode;
			InitArmSwinger();
		}

		public void Init(FVRSceneSettings SceneSettings)
		{
			m_sceneSettings = SceneSettings;
			CleanupFlagsForModeSwitch();
		}

		public bool ShouldFlushTouchpad(FVRViveHand hand)
		{
			if (hand.CMode == ControlMode.Index || hand.CMode == ControlMode.WMR)
			{
				return false;
			}
			if (hand.IsInStreamlinedMode)
			{
				return false;
			}
			if ((Mode == MovementMode.SingleTwoAxis || Mode == MovementMode.JoystickTeleport) && hand == m_authoratativeHand)
			{
				return true;
			}
			if (Mode == MovementMode.TwinStick)
			{
				bool flag = hand.IsThisTheRightHand;
				if (GM.Options.MovementOptions.TwinStickLeftRightState == MovementOptions.TwinStickLeftRightSetup.RightStickMove)
				{
					flag = !flag;
				}
				if (m_isLeftHandActive && !flag)
				{
					return true;
				}
				if (m_isRightHandActive && flag)
				{
					return true;
				}
			}
			return false;
		}

		private void SetTopSpeedLastSecond(Vector3 dir)
		{
			m_lastDir = dir.normalized;
			m_lastDir += Head.forward * 0.01f;
			m_topSpeedLastSecond = Mathf.Max(dir.magnitude, m_topSpeedLastSecond);
		}

		public float GetTopSpeedInLastSecond()
		{
			return m_topSpeedLastSecond;
		}

		public Vector3 GetLastWorldDir()
		{
			return m_lastDir;
		}

		private void SetFrameSpeed(Vector3 dir)
		{
			if (!dirSetThisFrame)
			{
				dirSetThisFrame = true;
				m_dirs[whichDir] = dir;
				whichDir++;
				if (whichDir > 4)
				{
					whichDir = 0;
				}
			}
		}

		public Vector3 GetFilteredVel()
		{
			Vector3 zero = Vector3.zero;
			for (int i = 0; i < m_dirs.Length; i++)
			{
				zero += m_dirs[i];
			}
			return zero * 0.2f;
		}

		public void CycleMode()
		{
			switch (Mode)
			{
			case MovementMode.Teleport:
				Mode = MovementMode.Dash;
				break;
			case MovementMode.Dash:
				Mode = MovementMode.SlideToTarget;
				break;
			case MovementMode.SlideToTarget:
				Mode = MovementMode.SingleTwoAxis;
				break;
			case MovementMode.SingleTwoAxis:
				Mode = MovementMode.Armswinger;
				break;
			case MovementMode.Armswinger:
				Mode = MovementMode.JoystickTeleport;
				break;
			case MovementMode.JoystickTeleport:
				Mode = MovementMode.Teleport;
				break;
			}
			GM.Options.MovementOptions.CurrentMovementMode = Mode;
			GM.Options.SaveToFile();
			CleanupFlagsForModeSwitch();
			InitArmSwinger();
		}

		public void CleanupFlagsForModeSwitch()
		{
			m_isDashing = false;
			m_DashTarget = Vector3.zero;
			m_isSliding = false;
			m_SlidingTarget = Vector3.zero;
			m_twoAxisVelocity = Vector3.zero;
			m_twoAxisGrounded = false;
			m_groundPoint = Vector3.zero;
			m_sprintingEngaged = false;
			m_timeSinceSprintDownClick = 1f;
			m_curArmSwingerImpetus = 0f;
			m_tarArmSwingerImpetus = 0f;
			m_armSwingerVelocity = Vector3.zero;
			m_armSwingerGrounded = true;
			m_joyStickTeleportInProgress = false;
			m_joyStickTeleportCooldown = 0.25f;
			startedTP = false;
		}

		public void UpdateMovementWithHand(FVRViveHand hand)
		{
			switch (Mode)
			{
			case MovementMode.Teleport:
				HandUpdateTeleport(hand);
				break;
			case MovementMode.Dash:
				HandUpdateDash(hand);
				break;
			case MovementMode.SlideToTarget:
				HandUpdateSliding(hand);
				break;
			case MovementMode.SingleTwoAxis:
				HandUpdateTwoAxis(hand);
				break;
			case MovementMode.Armswinger:
				HandUpdateArmSwinger(hand);
				break;
			case MovementMode.JoystickTeleport:
				HandUpdateJoyStickTeleport(hand);
				break;
			case MovementMode.TwinStick:
				HandUpdateTwinstick(hand);
				break;
			}
			AXButtonCheck(hand);
		}

		public void Update()
		{
			if (!dirSetThisFrame)
			{
				SetFrameSpeed(Vector3.zero);
			}
			dirSetThisFrame = false;
			m_topSpeedLastSecond = Mathf.Clamp(m_topSpeedLastSecond, 0f, 8f);
			m_topSpeedLastSecond = Mathf.Lerp(m_topSpeedLastSecond, 0f, Time.deltaTime * 5f);
			executeFrameTick = 0;
		}

		public void FixedUpdate()
		{
			if (m_teleportCooldown > 0f)
			{
				m_teleportCooldown -= Time.deltaTime;
			}
			if (m_teleportEnergy < 1f)
			{
				m_teleportEnergy += Time.deltaTime;
			}
			else
			{
				m_teleportEnergy = 1f;
			}
			if (m_teleportEnergy < 0f)
			{
				m_teleportEnergy = 0f;
			}
			if (!GM.CurrentSceneSettings.DoesTeleportUseCooldown)
			{
				m_teleportEnergy = 1f;
			}
			if (executeFrameTick < 2)
			{
				executeFrameTick++;
				FU();
			}
		}

		public void FU()
		{
			if (m_delayGroundCheck > 0f)
			{
				m_delayGroundCheck -= Time.deltaTime;
			}
			if ((Mode != MovementMode.SingleTwoAxis || m_authoratativeHand == null) && m_touchpadArrows.activeSelf)
			{
				m_touchpadArrows.SetActive(value: false);
			}
			if ((Mode != MovementMode.JoystickTeleport || m_authoratativeHand == null) && m_joystickTPArrows.activeSelf)
			{
				m_joystickTPArrows.SetActive(value: false);
			}
			if (Mode != MovementMode.TwinStick)
			{
				if (m_twinStickArrowsLeft.activeSelf)
				{
					m_twinStickArrowsLeft.SetActive(value: false);
				}
				if (m_twinStickArrowsRight.activeSelf)
				{
					m_twinStickArrowsRight.SetActive(value: false);
				}
			}
			if (Mode == MovementMode.JoystickTeleport)
			{
				if (!m_floorHelper.activeSelf)
				{
					m_floorHelper.SetActive(value: true);
				}
				m_floorHelper.transform.position = GM.CurrentPlayerBody.transform.position + Vector3.up * 0.01f;
			}
			else if (m_floorHelper.activeSelf)
			{
				m_floorHelper.SetActive(value: false);
			}
			if (m_joyStickTeleportCooldown > 0f)
			{
				m_joyStickTeleportCooldown -= Time.deltaTime;
			}
			bool flag = false;
			if (m_curGrabPoint != null)
			{
				UpdateGrabPointMove();
				flag = true;
			}
			else
			{
				switch (Mode)
				{
				case MovementMode.Dash:
					UpdateModeDash();
					break;
				case MovementMode.SlideToTarget:
					UpdateModeSliding();
					break;
				case MovementMode.SingleTwoAxis:
					UpdateModeTwoAxis(IsTwinstick: false);
					break;
				case MovementMode.Armswinger:
					UpdateModeArmSwinger();
					break;
				case MovementMode.TwinStick:
					UpdateModeTwoAxis(IsTwinstick: true);
					break;
				}
			}
			correctionDir = Vector3.zero;
			Body.UpdatePlayerBodyPositions();
			if (flag)
			{
				Vector3 vector = GM.CurrentPlayerBody.NeckJointTransform.transform.position - lastHeadPos;
				if (Physics.SphereCast(lastHeadPos, 0.15f, vector.normalized, out var _, vector.magnitude, LM_TeleCast))
				{
					correctionDir = -vector;
					base.transform.position = base.transform.position + correctionDir;
				}
			}
			lastHeadPos = GM.CurrentPlayerBody.NeckJointTransform.transform.position;
		}

		private void DelayGround(float f)
		{
			m_delayGroundCheck = f;
		}

		public void Blast(Vector3 dir, float vel)
		{
			bool flag = true;
			if (Mode == MovementMode.Armswinger || Mode == MovementMode.SingleTwoAxis || Mode == MovementMode.TwinStick)
			{
				flag = false;
			}
			if (!flag)
			{
				bool flag2 = false;
				float num = 0.4f;
				if (Mode == MovementMode.Armswinger && m_armSwingerGrounded)
				{
					flag2 = true;
				}
				if ((Mode == MovementMode.SingleTwoAxis || Mode == MovementMode.TwinStick) && m_twoAxisGrounded)
				{
					flag2 = true;
				}
				if (flag2)
				{
					num = 1f;
				}
				float f = 0.1f;
				if (!flag2 || dir.y > 0f)
				{
				}
				Vector3 vector = dir * vel * num;
				if (Mode == MovementMode.Armswinger)
				{
					DelayGround(f);
					m_armSwingerGrounded = false;
					m_armSwingerVelocity += vector;
				}
				else if (Mode == MovementMode.SingleTwoAxis || Mode == MovementMode.TwinStick)
				{
					DelayGround(f);
					m_twoAxisGrounded = false;
					m_twoAxisVelocity += vector;
				}
			}
		}

		public void RocketJump(Vector3 pos, Vector2 range, float vel)
		{
			bool flag = true;
			if (Mode == MovementMode.Armswinger || Mode == MovementMode.SingleTwoAxis || Mode == MovementMode.TwinStick)
			{
				flag = false;
			}
			if (flag)
			{
				return;
			}
			float num = Vector3.Distance(pos, GM.CurrentPlayerBody.Head.position);
			if (!(num > range.y))
			{
				float num2 = Mathf.InverseLerp(range.x, range.y, num);
				float num3 = 1f - num2;
				Vector3 vector = GM.CurrentPlayerBody.Head.position - pos;
				vector.Normalize();
				float num4 = 5f;
				switch (GM.Options.SimulationOptions.PlayerGravityMode)
				{
				case SimulationOptions.GravityMode.Realistic:
					num4 = 6f;
					break;
				case SimulationOptions.GravityMode.Playful:
					num4 = 5f;
					break;
				case SimulationOptions.GravityMode.OnTheMoon:
					num4 = 2.3f;
					break;
				case SimulationOptions.GravityMode.None:
					num4 = 1.62f;
					break;
				}
				Vector3 zero = Vector3.zero;
				bool flag2 = false;
				if (Mode == MovementMode.Armswinger && m_armSwingerGrounded)
				{
					flag2 = true;
				}
				if ((Mode == MovementMode.SingleTwoAxis || Mode == MovementMode.TwinStick) && m_twoAxisGrounded)
				{
					flag2 = true;
				}
				if (!flag2)
				{
					num3 = 1f;
				}
				if (vector.y > 0f)
				{
					zero += Vector3.up * num4 * 2.5f * num3;
				}
				if (!flag2)
				{
					Vector3 vector2 = vector;
					vector2.y = 0f;
					zero += vector2 * vel * num3;
				}
				if (Mode == MovementMode.Armswinger)
				{
					DelayGround(0.1f);
					m_armSwingerVelocity.y = Mathf.Clamp(m_armSwingerVelocity.y, 0f, m_armSwingerVelocity.y);
					m_armSwingerGrounded = false;
					m_armSwingerVelocity += zero;
				}
				else if (Mode == MovementMode.SingleTwoAxis || Mode == MovementMode.TwinStick)
				{
					DelayGround(0.1f);
					m_twoAxisVelocity.y = Mathf.Clamp(m_twoAxisVelocity.y, 0f, m_twoAxisVelocity.y);
					m_twoAxisGrounded = false;
					m_twoAxisVelocity += zero;
				}
			}
		}

		private void Jump()
		{
			if ((Mode != MovementMode.Armswinger || m_armSwingerGrounded) && ((Mode != MovementMode.SingleTwoAxis && Mode != MovementMode.TwinStick) || m_twoAxisGrounded))
			{
				DelayGround(0.1f);
				float num = 0f;
				switch (GM.Options.SimulationOptions.PlayerGravityMode)
				{
				case SimulationOptions.GravityMode.Realistic:
					num = 7.1f;
					break;
				case SimulationOptions.GravityMode.Playful:
					num = 5f;
					break;
				case SimulationOptions.GravityMode.OnTheMoon:
					num = 3f;
					break;
				case SimulationOptions.GravityMode.None:
					num = 0.001f;
					break;
				}
				num *= 0.65f;
				if (Mode == MovementMode.Armswinger)
				{
					DelayGround(0.25f);
					m_armSwingerVelocity.y = Mathf.Clamp(m_armSwingerVelocity.y, 0f, m_armSwingerVelocity.y);
					m_armSwingerVelocity.y = num;
					m_armSwingerGrounded = false;
				}
				else if (Mode == MovementMode.SingleTwoAxis || Mode == MovementMode.TwinStick)
				{
					DelayGround(0.25f);
					m_twoAxisVelocity.y = Mathf.Clamp(m_twoAxisVelocity.y, 0f, m_twoAxisVelocity.y);
					m_twoAxisVelocity.y = num;
					m_twoAxisGrounded = false;
				}
			}
		}

		private void AXButtonCheck(FVRViveHand hand)
		{
			if (hand.IsInStreamlinedMode)
			{
				return;
			}
			if (GM.Options.MovementOptions.AXButtonSnapTurnState == MovementOptions.AXButtonSnapTurnMode.Snapturn)
			{
				bool isThisTheRightHand = hand.IsThisTheRightHand;
				if (hand.Input.AXButtonDown)
				{
					if (isThisTheRightHand)
					{
						TurnClockWise();
					}
					else
					{
						TurnCounterClockWise();
					}
				}
			}
			else if (GM.Options.MovementOptions.AXButtonSnapTurnState == MovementOptions.AXButtonSnapTurnMode.Jump && hand.Input.AXButtonDown)
			{
				Jump();
			}
		}

		public void TurnClockWise()
		{
			Vector3 position = GM.CurrentPlayerBody.NeckJointTransform.position;
			position.y = GM.CurrentPlayerBody.transform.position.y;
			Vector3 forward = GM.CurrentPlayerBody.transform.forward;
			float angle = GM.Options.MovementOptions.SnapTurnMagnitudes[GM.Options.MovementOptions.SnapTurnMagnitudeIndex];
			forward = Quaternion.AngleAxis(angle, Vector3.up) * forward;
			TeleportToPoint(position, isAbsolute: false, forward);
		}

		public void TurnCounterClockWise()
		{
			Vector3 position = GM.CurrentPlayerBody.NeckJointTransform.position;
			position.y = GM.CurrentPlayerBody.transform.position.y;
			Vector3 forward = GM.CurrentPlayerBody.transform.forward;
			float num = GM.Options.MovementOptions.SnapTurnMagnitudes[GM.Options.MovementOptions.SnapTurnMagnitudeIndex];
			forward = Quaternion.AngleAxis(0f - num, Vector3.up) * forward;
			TeleportToPoint(position, isAbsolute: false, forward);
		}

		private void HandUpdateTeleport(FVRViveHand hand)
		{
			if (!hand.IsInStreamlinedMode && (hand.CMode == ControlMode.Index || hand.CMode == ControlMode.WMR))
			{
				if (hand.Input.Secondary2AxisWestDown)
				{
					TurnCounterClockWise();
				}
				else if (hand.Input.Secondary2AxisEastDown)
				{
					TurnClockWise();
				}
			}
			if (hand.IsInStreamlinedMode)
			{
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				if (hand.CMode == ControlMode.Index || hand.CMode == ControlMode.WMR)
				{
					if (hand.Input.Secondary2AxisWestDown)
					{
						flag = true;
					}
					else if (hand.Input.Secondary2AxisEastDown)
					{
						flag2 = true;
					}
					else if (hand.Input.Secondary2AxisSouthDown)
					{
						flag3 = true;
					}
				}
				else if (hand.Input.TouchpadWestDown)
				{
					flag = true;
				}
				else if (hand.Input.TouchpadEastDown)
				{
					flag2 = true;
				}
				else if (hand.Input.TouchpadSouthDown)
				{
					flag3 = true;
				}
				if (flag)
				{
					m_hasValidPoint = false;
					TurnCounterClockWise();
					ClearInProgressTeleportAction();
				}
				else if (flag2)
				{
					m_hasValidPoint = false;
					TurnClockWise();
					ClearInProgressTeleportAction();
				}
				else if (flag3)
				{
					m_hasValidPoint = false;
					ClearInProgressTeleportAction();
				}
			}
			bool flag4 = false;
			bool flag5 = false;
			if (hand.IsInStreamlinedMode)
			{
				if (hand.CMode == ControlMode.Index || hand.CMode == ControlMode.WMR)
				{
					flag4 = hand.Input.Secondary2AxisNorthPressed;
					flag5 = hand.Input.Secondary2AxisNorthUp;
				}
				else
				{
					flag4 = hand.Input.TouchpadNorthPressed;
					flag5 = hand.Input.TouchpadNorthUp;
				}
			}
			else
			{
				flag4 = hand.Input.BYButtonPressed;
				flag5 = hand.Input.BYButtonUp;
			}
			Vector3 zero = Vector3.zero;
			if (flag4 && m_teleportCooldown <= 0f)
			{
				startedTP = true;
				if (GM.Options.MovementOptions.Teleport_Mode == TeleportMode.Standard)
				{
					if (GM.Options.MovementOptions.Teleport_AxialOrigin == MovementAxialOrigin.Hands)
					{
						m_validPoint = FindValidPointCurved(hand.Input.FilteredPos, hand.PointingTransform.forward, 0.5f);
						zero = hand.PointingTransform.forward;
					}
					else
					{
						Vector3 castOrigin = GM.CurrentPlayerBody.Torso.position + GM.CurrentPlayerBody.Torso.right * 0.2f;
						Vector3 vector = GM.CurrentPlayerBody.Head.forward + Vector3.up * 0.2f;
						m_validPoint = FindValidPointCurved(castOrigin, vector, 0.5f);
						zero = vector;
					}
					if (!m_hasValidPoint)
					{
						if (MovementRig.gameObject.activeSelf)
						{
							MovementRig.gameObject.SetActive(value: false);
						}
						return;
					}
					if (!MovementRig.gameObject.activeSelf)
					{
						MovementRig.gameObject.SetActive(value: true);
					}
					Vector3 forward = Head.transform.forward;
					forward.y = 0f;
					MovementRig.transform.position = m_validPoint;
					MovementRig.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
					if (!MovementRig.CornerHolder.gameObject.activeSelf)
					{
						MovementRig.CornerHolder.gameObject.SetActive(value: true);
					}
					MovementRig.CornerHolder.rotation = hand.WholeRig.rotation;
					Vector3 vector2 = base.transform.position - Head.position;
					vector2.y = 0f;
					MovementRig.CornerHolder.position = m_validPoint + vector2;
				}
				else
				{
					if (GM.Options.MovementOptions.Teleport_Mode != TeleportMode.FrontFacingMode)
					{
						return;
					}
					if (!m_isInRotatePickMode)
					{
						m_validPoint = FindValidPointCurved(hand.Input.FilteredPos, hand.PointingTransform.forward, 0.5f);
						zero = hand.PointingTransform.forward;
						if (!m_hasValidPoint)
						{
							if (MovementRig.gameObject.activeSelf)
							{
								MovementRig.gameObject.SetActive(value: false);
							}
							return;
						}
						if (!MovementRig.gameObject.activeSelf)
						{
							MovementRig.gameObject.SetActive(value: true);
						}
						Vector3 forward2 = hand.WholeRig.forward;
						forward2.y = 0f;
						MovementRig.transform.position = m_validPoint;
						MovementRig.transform.rotation = Quaternion.LookRotation(forward2, Vector3.up);
						if (MovementRig.CornerHolder.gameObject.activeSelf)
						{
							MovementRig.CornerHolder.gameObject.SetActive(value: false);
						}
						return;
					}
					m_validRotateDir = FindValidPointCurvedForRotatedTeleport(hand.Input.FilteredPos, hand.PointingTransform.forward, 0.5f);
					zero = hand.PointingTransform.forward;
					if (!m_hasValidRotateDir)
					{
						m_validRotateDir = hand.WholeRig.forward;
						return;
					}
					if (!MovementRig.gameObject.activeSelf)
					{
						MovementRig.gameObject.SetActive(value: true);
					}
					MovementRig.transform.rotation = Quaternion.LookRotation(m_validRotateDir, Vector3.up);
					if (MovementRig.CornerHolder.gameObject.activeSelf)
					{
						MovementRig.CornerHolder.gameObject.SetActive(value: false);
					}
				}
			}
			else
			{
				if (!startedTP || !flag5)
				{
					return;
				}
				startedTP = false;
				if (GM.Options.MovementOptions.Teleport_Mode == TeleportMode.Standard)
				{
					if (MovementRig.gameObject.activeSelf)
					{
						MovementRig.gameObject.SetActive(value: false);
					}
					if (m_hasValidPoint)
					{
						m_teleportEnergy -= TeleportToPoint(m_validPoint, isAbsolute: false);
					}
					for (int i = 0; i < 20; i++)
					{
						if (Cylinders[i].gameObject.activeSelf)
						{
							Cylinders[i].gameObject.SetActive(value: false);
						}
					}
				}
				else
				{
					if (GM.Options.MovementOptions.Teleport_Mode != TeleportMode.FrontFacingMode)
					{
						return;
					}
					if (!m_isInRotatePickMode)
					{
						if (m_hasValidPoint)
						{
							m_isInRotatePickMode = true;
						}
						else if (MovementRig.gameObject.activeSelf)
						{
							MovementRig.gameObject.SetActive(value: false);
						}
					}
					else
					{
						m_isInRotatePickMode = false;
						if (m_hasValidRotateDir && m_hasValidPoint)
						{
							m_teleportEnergy -= TeleportToPoint(m_validPoint, isAbsolute: false, m_validRotateDir);
							if (MovementRig.gameObject.activeSelf)
							{
								MovementRig.gameObject.SetActive(value: false);
							}
						}
						else
						{
							if (MovementRig.gameObject.activeSelf)
							{
								MovementRig.gameObject.SetActive(value: false);
							}
							m_hasValidRotateDir = false;
							m_hasValidPoint = false;
						}
					}
					for (int j = 0; j < 20; j++)
					{
						if (Cylinders[j].gameObject.activeSelf)
						{
							Cylinders[j].gameObject.SetActive(value: false);
						}
					}
				}
			}
		}

		public void ClearInProgressTeleportAction()
		{
			MovementRig.gameObject.SetActive(value: false);
			m_hasValidPoint = false;
			startedTP = false;
			for (int i = 0; i < 20; i++)
			{
				if (Cylinders[i].gameObject.activeSelf)
				{
					Cylinders[i].gameObject.SetActive(value: false);
				}
			}
		}

		private void HandUpdateDash(FVRViveHand hand)
		{
			if (!hand.IsInStreamlinedMode && (hand.CMode == ControlMode.Index || hand.CMode == ControlMode.WMR))
			{
				if (hand.Input.Secondary2AxisWestDown)
				{
					TurnCounterClockWise();
				}
				else if (hand.Input.Secondary2AxisEastDown)
				{
					TurnClockWise();
				}
			}
			if (hand.IsInStreamlinedMode)
			{
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				if (hand.CMode == ControlMode.Index || hand.CMode == ControlMode.WMR)
				{
					if (hand.Input.Secondary2AxisWestDown)
					{
						flag = true;
					}
					else if (hand.Input.Secondary2AxisEastDown)
					{
						flag2 = true;
					}
					else if (hand.Input.Secondary2AxisSouthDown)
					{
						flag3 = true;
					}
				}
				else if (hand.Input.TouchpadWestDown)
				{
					flag = true;
				}
				else if (hand.Input.TouchpadEastDown)
				{
					flag2 = true;
				}
				else if (hand.Input.TouchpadSouthDown)
				{
					flag3 = true;
				}
				if (flag)
				{
					m_hasValidPoint = false;
					TurnCounterClockWise();
					ClearInProgressTeleportAction();
				}
				else if (flag2)
				{
					m_hasValidPoint = false;
					TurnClockWise();
					ClearInProgressTeleportAction();
				}
				else if (flag3)
				{
					m_hasValidPoint = false;
					ClearInProgressTeleportAction();
				}
			}
			bool flag4 = false;
			bool flag5 = false;
			if (hand.IsInStreamlinedMode)
			{
				if (hand.CMode == ControlMode.Index || hand.CMode == ControlMode.WMR)
				{
					flag4 = hand.Input.Secondary2AxisNorthPressed;
					flag5 = hand.Input.Secondary2AxisNorthUp;
				}
				else
				{
					flag4 = hand.Input.TouchpadNorthPressed;
					flag5 = hand.Input.TouchpadNorthUp;
				}
			}
			else
			{
				flag4 = hand.Input.BYButtonPressed;
				flag5 = hand.Input.BYButtonUp;
			}
			if (flag4 && m_teleportCooldown <= 0f)
			{
				startedTP = true;
				if (GM.Options.MovementOptions.Dash_AxialOrigin == MovementAxialOrigin.Hands)
				{
					m_validPoint = FindValidPointCurved(hand.Input.FilteredPos, hand.PointingTransform.forward, 0.5f);
				}
				else
				{
					Vector3 castOrigin = GM.CurrentPlayerBody.Torso.position + GM.CurrentPlayerBody.Torso.right * 0.2f;
					Vector3 castDir = GM.CurrentPlayerBody.Head.forward + Vector3.up * 0.2f;
					m_validPoint = FindValidPointCurved(castOrigin, castDir, 0.5f);
				}
				if (!m_hasValidPoint)
				{
					if (MovementRig.gameObject.activeSelf)
					{
						MovementRig.gameObject.SetActive(value: false);
					}
					return;
				}
				if (!MovementRig.gameObject.activeSelf)
				{
					MovementRig.gameObject.SetActive(value: true);
				}
				Vector3 forward = Head.transform.forward;
				forward.y = 0f;
				MovementRig.transform.position = m_validPoint;
				MovementRig.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
				MovementRig.CornerHolder.rotation = hand.WholeRig.rotation;
				Vector3 vector = base.transform.position - Head.position;
				vector.y = 0f;
				MovementRig.CornerHolder.position = m_validPoint + vector;
			}
			else
			{
				if (!startedTP || !flag5)
				{
					return;
				}
				startedTP = false;
				if (MovementRig.gameObject.activeSelf)
				{
					MovementRig.gameObject.SetActive(value: false);
				}
				if (m_hasValidPoint)
				{
					m_teleportEnergy -= SetDashDestination(m_validPoint);
				}
				for (int i = 0; i < 20; i++)
				{
					if (Cylinders[i].gameObject.activeSelf)
					{
						Cylinders[i].gameObject.SetActive(value: false);
					}
				}
			}
		}

		private void ClearTPRig()
		{
			if (MovementRig.gameObject.activeSelf)
			{
				MovementRig.gameObject.SetActive(value: false);
			}
			for (int i = 0; i < 20; i++)
			{
				if (Cylinders[i].gameObject.activeSelf)
				{
					Cylinders[i].gameObject.SetActive(value: false);
				}
			}
		}

		private void UpdateModeDash()
		{
			if (m_isDashing)
			{
				Vector3 position = base.transform.position;
				base.transform.position = Vector3.MoveTowards(base.transform.position, m_DashTarget, DashSpeed * Time.deltaTime);
				Vector3 vector = base.transform.position - position;
				Body.MoveQuickbeltContents(vector);
				SetTopSpeedLastSecond(vector);
				if (Vector3.Distance(base.transform.position, m_DashTarget) < 0.01f)
				{
					m_isDashing = false;
				}
			}
		}

		private void HandUpdateSliding(FVRViveHand hand)
		{
			if (hand.Input.BYButtonPressed)
			{
				if (GM.Options.MovementOptions.Slide_AxialOrigin == MovementAxialOrigin.Hands)
				{
					m_validPoint = FindValidPointCurved(hand.Input.FilteredPos, hand.PointingTransform.forward, 0.5f);
				}
				else
				{
					Vector3 castOrigin = GM.CurrentPlayerBody.Torso.position + GM.CurrentPlayerBody.Torso.right * 0.2f;
					Vector3 castDir = GM.CurrentPlayerBody.Head.forward + Vector3.up * 0.2f;
					m_validPoint = FindValidPointCurved(castOrigin, castDir, 0.5f);
				}
				if (!m_hasValidPoint)
				{
					if (MovementRig.gameObject.activeSelf)
					{
						MovementRig.gameObject.SetActive(value: false);
					}
					return;
				}
				if (!MovementRig.gameObject.activeSelf)
				{
					MovementRig.gameObject.SetActive(value: true);
				}
				Vector3 forward = Head.transform.forward;
				forward.y = 0f;
				MovementRig.transform.position = m_validPoint;
				MovementRig.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
				MovementRig.CornerHolder.rotation = hand.WholeRig.rotation;
				Vector3 vector = base.transform.position - Head.position;
				vector.y = 0f;
				MovementRig.CornerHolder.position = m_validPoint + vector;
			}
			else
			{
				if (!hand.Input.BYButtonUp)
				{
					return;
				}
				if (MovementRig.gameObject.activeSelf)
				{
					MovementRig.gameObject.SetActive(value: false);
				}
				if (m_hasValidPoint)
				{
					SetSlidingDestination(m_validPoint);
				}
				for (int i = 0; i < 20; i++)
				{
					if (Cylinders[i].gameObject.activeSelf)
					{
						Cylinders[i].gameObject.SetActive(value: false);
					}
				}
			}
		}

		private void UpdateModeSliding()
		{
			if (m_isSliding)
			{
				Vector3 position = base.transform.position;
				SlidingSpeed = GM.Options.MovementOptions.SlidingSpeeds[GM.Options.MovementOptions.SlidingSpeedTick];
				base.transform.position = Vector3.MoveTowards(base.transform.position, m_SlidingTarget, SlidingSpeed * Time.deltaTime);
				Vector3 vector = base.transform.position - position;
				Body.MoveQuickbeltContents(vector);
				SetTopSpeedLastSecond(vector);
				SetFrameSpeed(vector);
				if (Vector3.Distance(base.transform.position, m_SlidingTarget) < 0.01f)
				{
					m_isSliding = false;
				}
			}
		}

		private void HandUpdateTwoAxis(FVRViveHand hand)
		{
			if (hand.Input.BYButtonDown)
			{
				if (hand != m_authoratativeHand)
				{
					m_authoratativeHand = hand;
				}
				else if (hand == m_authoratativeHand)
				{
					m_authoratativeHand = null;
				}
			}
			if (m_authoratativeHand == null)
			{
				m_isTwinStickSmoothTurningClockwise = false;
				m_isTwinStickSmoothTurningCounterClockwise = false;
			}
			if (m_authoratativeHand == null || hand != m_authoratativeHand)
			{
				return;
			}
			if (!m_touchpadArrows.activeSelf)
			{
				m_touchpadArrows.SetActive(value: true);
			}
			if (m_touchpadArrows.transform.parent != m_authoratativeHand.TouchpadArrowTarget)
			{
				m_touchpadArrows.transform.parent = m_authoratativeHand.TouchpadArrowTarget;
				m_touchpadArrows.transform.localPosition = Vector3.zero;
				m_touchpadArrows.transform.localRotation = Quaternion.identity;
			}
			if (m_timeSinceSprintDownClick < 2f)
			{
				m_timeSinceSprintDownClick += Time.deltaTime;
			}
			if (m_timeSinceSnapTurn < 2f)
			{
				m_timeSinceSnapTurn += Time.deltaTime;
			}
			if (hand.Input.TouchpadDown && m_timeSinceSprintDownClick < 0.3f)
			{
				m_sprintingEngaged = true;
			}
			if (hand.Input.TouchpadTouchUp)
			{
				m_sprintingEngaged = false;
			}
			Vector3 vector = Vector3.zero;
			float y = hand.Input.TouchpadAxes.y;
			float num = hand.Input.TouchpadAxes.x;
			if (GM.Options.MovementOptions.Touchpad_Style == TwoAxisMovementStyle.LateralIsSnapTurn)
			{
				num = 0f;
				if (Mathf.Abs(hand.Input.TouchpadAxes.x) > 0.7f && m_timeSinceSnapTurn > 0.3f && ((GM.Options.MovementOptions.Touchpad_Confirm == TwoAxisMovementConfirm.OnClick && hand.Input.TouchpadDown) || (GM.Options.MovementOptions.Touchpad_Confirm == TwoAxisMovementConfirm.OnTouch && hand.Input.TouchpadTouched)))
				{
					m_timeSinceSnapTurn = 0f;
					Vector3 position = GM.CurrentPlayerBody.Head.position;
					position.y = base.transform.position.y;
					Vector3 forward = base.transform.forward;
					float num2 = GM.Options.MovementOptions.SnapTurnMagnitudes[GM.Options.MovementOptions.SnapTurnMagnitudeIndex];
					TeleportToPoint(lookDir: (!(hand.Input.TouchpadAxes.x > 0f)) ? (Quaternion.AngleAxis(0f - num2, Vector3.up) * forward) : (Quaternion.AngleAxis(num2, Vector3.up) * forward), point: position, isAbsolute: false);
				}
			}
			else if (GM.Options.MovementOptions.Touchpad_Style == TwoAxisMovementStyle.LateralIsSmoothTurn)
			{
				num = 0f;
				m_isTwinStickSmoothTurningClockwise = false;
				m_isTwinStickSmoothTurningCounterClockwise = false;
				if (hand.CMode == ControlMode.Oculus)
				{
					if (hand.Input.TouchpadWestPressed)
					{
						m_isTwinStickSmoothTurningCounterClockwise = true;
					}
					else if (hand.Input.TouchpadEastPressed)
					{
						m_isTwinStickSmoothTurningClockwise = true;
					}
				}
				else if (GM.Options.MovementOptions.Touchpad_Confirm == TwoAxisMovementConfirm.OnClick)
				{
					if (hand.Input.TouchpadPressed)
					{
						if (hand.Input.TouchpadWestPressed)
						{
							m_isTwinStickSmoothTurningCounterClockwise = true;
						}
						else if (hand.Input.TouchpadEastPressed)
						{
							m_isTwinStickSmoothTurningClockwise = true;
						}
					}
				}
				else if (hand.Input.TouchpadWestPressed)
				{
					m_isTwinStickSmoothTurningCounterClockwise = true;
				}
				else if (hand.Input.TouchpadEastPressed)
				{
					m_isTwinStickSmoothTurningClockwise = true;
				}
			}
			switch (GM.Options.MovementOptions.Touchpad_MovementMode)
			{
			case TwoAxisMovementMode.Standard:
				vector = y * hand.PointingTransform.forward + num * hand.PointingTransform.right * 0.75f;
				vector.y = 0f;
				break;
			case TwoAxisMovementMode.Onward:
				vector = y * hand.transform.forward + num * hand.transform.right * 0.75f;
				break;
			case TwoAxisMovementMode.LeveledHand:
			{
				Vector3 forward3 = hand.transform.forward;
				forward3.y = 0f;
				forward3.Normalize();
				Vector3 right2 = hand.transform.right;
				right2.y = 0f;
				right2.Normalize();
				vector = y * forward3 + num * right2 * 0.75f;
				break;
			}
			case TwoAxisMovementMode.LeveledHead:
			{
				Vector3 forward2 = GM.CurrentPlayerBody.Head.forward;
				forward2.y = 0f;
				forward2.Normalize();
				Vector3 right = GM.CurrentPlayerBody.Head.right;
				right.y = 0f;
				right.Normalize();
				vector = y * forward2 + num * right * 0.75f;
				break;
			}
			}
			Vector3 normalized = vector.normalized;
			vector *= GM.Options.MovementOptions.TPLocoSpeeds[GM.Options.MovementOptions.TPLocoSpeedIndex];
			if (GM.Options.MovementOptions.Touchpad_Confirm == TwoAxisMovementConfirm.OnClick)
			{
				if (!hand.Input.TouchpadPressed)
				{
					vector = Vector3.zero;
				}
				else if (m_sprintingEngaged && GM.Options.MovementOptions.TPLocoSpeedIndex < 5)
				{
					vector += normalized * 2f;
				}
			}
			else if (m_sprintingEngaged && GM.Options.MovementOptions.TPLocoSpeedIndex < 5)
			{
				vector += normalized * 2f;
			}
			if (m_twoAxisGrounded)
			{
				m_twoAxisVelocity.x = vector.x;
				m_twoAxisVelocity.z = vector.z;
				if (GM.CurrentSceneSettings.UsesMaxSpeedClamp)
				{
					Vector2 vector2 = new Vector2(m_twoAxisVelocity.x, m_twoAxisVelocity.z);
					if (vector2.magnitude > GM.CurrentSceneSettings.MaxSpeedClamp)
					{
						vector2 = vector2.normalized * GM.CurrentSceneSettings.MaxSpeedClamp;
						m_twoAxisVelocity.x = vector2.x;
						m_twoAxisVelocity.z = vector2.y;
					}
				}
			}
			else if (GM.CurrentSceneSettings.DoesAllowAirControl)
			{
				Vector3 vector3 = new Vector3(m_twoAxisVelocity.x, 0f, m_twoAxisVelocity.z);
				m_twoAxisVelocity.x += vector.x * Time.deltaTime;
				m_twoAxisVelocity.z += vector.z * Time.deltaTime;
				Vector3 vector4 = new Vector3(m_twoAxisVelocity.x, 0f, m_twoAxisVelocity.z);
				float maxLength = Mathf.Max(1f, vector3.magnitude);
				vector4 = Vector3.ClampMagnitude(vector4, maxLength);
				m_twoAxisVelocity.x = vector4.x;
				m_twoAxisVelocity.z = vector4.z;
			}
			else
			{
				Vector3 vector5 = new Vector3(m_twoAxisVelocity.x, 0f, m_twoAxisVelocity.z);
				m_twoAxisVelocity.x += vector.x * Time.deltaTime * 0.3f;
				m_twoAxisVelocity.z += vector.z * Time.deltaTime * 0.3f;
				Vector3 vector6 = new Vector3(m_twoAxisVelocity.x, 0f, m_twoAxisVelocity.z);
				float maxLength2 = Mathf.Max(1f, vector5.magnitude);
				vector6 = Vector3.ClampMagnitude(vector6, maxLength2);
				m_twoAxisVelocity.x = vector6.x;
				m_twoAxisVelocity.z = vector6.z;
			}
			if (hand.Input.TouchpadDown)
			{
				m_timeSinceSprintDownClick = 0f;
			}
		}

		private void UpdateModeTwoAxis(bool IsTwinstick)
		{
			CurNeckPos = GM.CurrentPlayerBody.NeckJointTransform.position;
			Vector3 vector = LastNeckPos - CurNeckPos;
			Vector3 lastNeckPos = LastNeckPos;
			Vector3 vector2 = CurNeckPos - LastNeckPos;
			if (Physics.SphereCast(LastNeckPos, 0.15f, vector2.normalized, out var hitInfo, vector2.magnitude, LM_TeleCast))
			{
				correctionDir = -vector2 * 1f;
			}
			if (IsTwinstick)
			{
				if (!m_isLeftHandActive && m_twoAxisGrounded)
				{
					m_twoAxisVelocity.x = 0f;
					m_twoAxisVelocity.z = 0f;
				}
			}
			else if (m_authoratativeHand == null && m_twoAxisGrounded)
			{
				m_twoAxisVelocity.x = 0f;
				m_twoAxisVelocity.z = 0f;
			}
			Vector3 vector3 = lastNeckPos;
			Vector3 b = vector3;
			vector3.y = Mathf.Max(vector3.y, base.transform.position.y + m_armSwingerStepHeight);
			b.y = base.transform.position.y;
			float num = Vector3.Distance(vector3, b);
			if (m_delayGroundCheck > 0f)
			{
				num *= 0.5f;
			}
			bool flag = false;
			Vector3 planeNormal = Vector3.up;
			bool flag2 = false;
			Vector3 vector4 = Vector3.up;
			Vector3 groundPoint = vector3 + -Vector3.up * num;
			Vector3 groundPoint2 = vector3 + -Vector3.up * num;
			float num2 = 90f;
			float a = -1000f;
			if (Physics.SphereCast(vector3, 0.2f, -Vector3.up, out m_hit_ray, num, LM_TeleCast))
			{
				vector4 = m_hit_ray.normal;
				groundPoint = m_hit_ray.point;
				groundPoint2 = m_hit_ray.point;
				num2 = Vector3.Angle(Vector3.up, m_hit_ray.normal);
				a = groundPoint.y;
				flag2 = true;
			}
			if (Physics.Raycast(vector3, -Vector3.up, out m_hit_ray, num, LM_TeleCast))
			{
				num2 = Mathf.Min(num2, Vector3.Angle(Vector3.up, m_hit_ray.normal));
				vector4 = m_hit_ray.normal;
				groundPoint.y = Mathf.Max(groundPoint.y, m_hit_ray.point.y);
				groundPoint2.y = Mathf.Min(groundPoint.y, m_hit_ray.point.y);
				a = Mathf.Max(a, m_hit_ray.point.y);
				flag2 = true;
			}
			Vector3 vector5 = Head.forward;
			vector5.y = 0f;
			vector5.Normalize();
			vector5 = Vector3.ClampMagnitude(vector5, 0.1f);
			Vector3 vector6 = Head.right;
			vector6.y = 0f;
			vector6.Normalize();
			vector6 = Vector3.ClampMagnitude(vector6, 0.1f);
			Vector3 vector7 = -vector5;
			Vector3 vector8 = -vector6;
			if (Physics.Raycast(vector3 + vector5, -Vector3.up, out m_hit_ray, num, LM_TeleCast))
			{
				num2 = Mathf.Min(num2, Vector3.Angle(Vector3.up, m_hit_ray.normal));
				groundPoint.y = Mathf.Max(groundPoint.y, m_hit_ray.point.y);
				groundPoint2.y = Mathf.Min(groundPoint.y, m_hit_ray.point.y);
				a = Mathf.Max(a, m_hit_ray.point.y);
				if (!flag2)
				{
					vector4 = m_hit_ray.normal;
					flag2 = true;
				}
			}
			if (Physics.Raycast(vector3 + vector6, -Vector3.up, out m_hit_ray, num, LM_TeleCast))
			{
				num2 = Mathf.Min(num2, Vector3.Angle(Vector3.up, m_hit_ray.normal));
				groundPoint.y = Mathf.Max(groundPoint.y, m_hit_ray.point.y);
				groundPoint2.y = Mathf.Min(groundPoint.y, m_hit_ray.point.y);
				a = Mathf.Max(a, m_hit_ray.point.y);
				if (!flag2)
				{
					vector4 = m_hit_ray.normal;
					flag2 = true;
				}
			}
			if (Physics.Raycast(vector3 + vector7, -Vector3.up, out m_hit_ray, num, LM_TeleCast))
			{
				num2 = Mathf.Min(num2, Vector3.Angle(Vector3.up, m_hit_ray.normal));
				groundPoint.y = Mathf.Max(groundPoint.y, m_hit_ray.point.y);
				groundPoint2.y = Mathf.Min(groundPoint.y, m_hit_ray.point.y);
				a = Mathf.Max(a, m_hit_ray.point.y);
				if (!flag2)
				{
					vector4 = m_hit_ray.normal;
					flag2 = true;
				}
			}
			if (Physics.Raycast(vector3 + vector8, -Vector3.up, out m_hit_ray, num, LM_TeleCast))
			{
				num2 = Mathf.Min(num2, Vector3.Angle(Vector3.up, m_hit_ray.normal));
				groundPoint.y = Mathf.Max(groundPoint.y, m_hit_ray.point.y);
				groundPoint2.y = Mathf.Min(groundPoint.y, m_hit_ray.point.y);
				a = Mathf.Max(a, m_hit_ray.point.y);
				if (!flag2)
				{
					vector4 = m_hit_ray.normal;
					flag2 = true;
				}
			}
			if (flag2)
			{
				if (num2 > 70f)
				{
					flag = true;
					m_twoAxisGrounded = false;
					planeNormal = vector4;
					m_groundPoint = groundPoint2;
				}
				else
				{
					m_twoAxisGrounded = true;
					m_groundPoint = groundPoint;
				}
			}
			else
			{
				m_twoAxisGrounded = false;
				m_groundPoint = vector3 - Vector3.up * num;
			}
			Vector3 vector9 = lastNeckPos;
			Vector3 b2 = vector9;
			b2.y = base.transform.position.y + 2.15f * GM.CurrentPlayerBody.transform.localScale.y;
			float maxDistance = Vector3.Distance(vector9, b2);
			float num3 = vector9.y + 0.15f;
			if (Physics.SphereCast(vector9, 0.15f, Vector3.up, out m_hit_ray, maxDistance, LM_TeleCast))
			{
				Vector3 point = m_hit_ray.point;
				float num4 = Vector3.Distance(vector9, new Vector3(vector9.x, point.y, vector9.z));
				num3 = m_hit_ray.point.y - 0.15f;
				float num5 = Mathf.Clamp(GM.CurrentPlayerBody.Head.localPosition.y, 0.3f, 2.5f);
				float y = m_groundPoint.y;
				float min = y - (num5 - 0.2f);
				float y2 = Mathf.Clamp(num3 - num5 - 0.15f, min, y);
				m_groundPoint.y = y2;
			}
			if (m_twoAxisGrounded)
			{
				m_twoAxisVelocity.y = 0f;
			}
			else
			{
				float num6 = 5f;
				switch (GM.Options.SimulationOptions.PlayerGravityMode)
				{
				case SimulationOptions.GravityMode.Realistic:
					num6 = 9.81f;
					break;
				case SimulationOptions.GravityMode.Playful:
					num6 = 5f;
					break;
				case SimulationOptions.GravityMode.OnTheMoon:
					num6 = 1.62f;
					break;
				case SimulationOptions.GravityMode.None:
					num6 = 0.001f;
					break;
				}
				if (!flag)
				{
					m_twoAxisVelocity.y -= num6 * Time.deltaTime;
				}
				else
				{
					Vector3 vector10 = Vector3.ProjectOnPlane(-Vector3.up * num6, planeNormal);
					m_twoAxisVelocity += vector10 * Time.deltaTime;
					m_twoAxisVelocity = Vector3.ProjectOnPlane(m_twoAxisVelocity, planeNormal);
				}
			}
			float num7 = Mathf.Abs(lastNeckPos.y - GM.CurrentPlayerBody.transform.position.y);
			Vector3 point2 = lastNeckPos;
			Vector3 point3 = lastNeckPos;
			point2.y = Mathf.Min(point2.y, num3 - 0.01f);
			point3.y = Mathf.Max(base.transform.position.y, m_groundPoint.y) + (m_armSwingerStepHeight + 0.2f);
			point2.y = Mathf.Max(point2.y, point3.y);
			Vector3 vector11 = m_twoAxisVelocity;
			float maxLength = m_twoAxisVelocity.magnitude * Time.deltaTime;
			if (Physics.CapsuleCast(point2, point3, 0.15f, m_twoAxisVelocity, out m_hit_ray, m_twoAxisVelocity.magnitude * Time.deltaTime + 0.1f, LM_TeleCast))
			{
				vector11 = Vector3.ProjectOnPlane(m_twoAxisVelocity, m_hit_ray.normal);
				maxLength = m_hit_ray.distance * 0.5f;
				if (m_twoAxisGrounded)
				{
					vector11.y = 0f;
				}
				if (Physics.CapsuleCast(point2, point3, 0.15f, vector11, out var hitInfo2, vector11.magnitude * Time.deltaTime + 0.1f, LM_TeleCast))
				{
					maxLength = hitInfo2.distance * 0.5f;
				}
			}
			m_twoAxisVelocity = vector11;
			if (m_twoAxisGrounded)
			{
				m_twoAxisVelocity.y = 0f;
			}
			Vector3 vector12 = base.transform.position;
			Vector3 vector13 = m_twoAxisVelocity * Time.deltaTime;
			vector13 = Vector3.ClampMagnitude(vector13, maxLength);
			vector12 = base.transform.position + vector13;
			if (m_twoAxisGrounded)
			{
				vector12.y = Mathf.MoveTowards(vector12.y, m_groundPoint.y, 8f * Time.deltaTime * Mathf.Abs(base.transform.position.y - m_groundPoint.y));
			}
			Vector3 vector14 = CurNeckPos + vector13;
			vector2 = vector14 - LastNeckPos;
			if (Physics.SphereCast(LastNeckPos, 0.15f, vector2.normalized, out hitInfo, vector2.magnitude, LM_TeleCast))
			{
				correctionDir = -vector2 * 1f;
			}
			if (GM.Options.MovementOptions.AXButtonSnapTurnState == MovementOptions.AXButtonSnapTurnMode.Smoothturn)
			{
				for (int i = 0; i < Hands.Length; i++)
				{
					if (Hands[i].IsInStreamlinedMode)
					{
						continue;
					}
					if (Hands[i].IsThisTheRightHand)
					{
						if (Hands[i].Input.AXButtonPressed)
						{
							m_isTwinStickSmoothTurningClockwise = true;
						}
					}
					else if (Hands[i].Input.AXButtonPressed)
					{
						m_isTwinStickSmoothTurningCounterClockwise = true;
					}
				}
			}
			if (!m_isTwinStickSmoothTurningClockwise && !m_isTwinStickSmoothTurningCounterClockwise)
			{
				base.transform.position = vector12 + correctionDir;
			}
			else
			{
				Vector3 point4 = vector12 + correctionDir;
				Vector3 forward = GM.CurrentPlayerBody.transform.forward;
				float num8 = GM.Options.MovementOptions.SmoothTurnMagnitudes[GM.Options.MovementOptions.SmoothTurnMagnitudeIndex] * Time.deltaTime;
				if (m_isTwinStickSmoothTurningCounterClockwise)
				{
					num8 = 0f - num8;
				}
				point4 = RotatePointAroundPivotWithEuler(point4, CurNeckPos, new Vector3(0f, num8, 0f));
				forward = Quaternion.AngleAxis(num8, Vector3.up) * forward;
				base.transform.SetPositionAndRotation(point4, Quaternion.LookRotation(forward, Vector3.up));
			}
			SetTopSpeedLastSecond(m_twoAxisVelocity);
			SetFrameSpeed(m_twoAxisVelocity);
			LastNeckPos = GM.CurrentPlayerBody.NeckJointTransform.position;
		}

		private void HandUpdateArmSwinger(FVRViveHand hand)
		{
			if (GM.Options.MovementOptions.ArmSwingerSnapTurnState != MovementOptions.ArmSwingerSnapTurnMode.Enabled)
			{
				return;
			}
			if (hand.CMode == ControlMode.Index || hand.CMode == ControlMode.WMR)
			{
				if (hand.Input.Secondary2AxisWestDown)
				{
					TurnCounterClockWise();
				}
				else if (hand.Input.Secondary2AxisEastDown)
				{
					TurnClockWise();
				}
			}
			if (hand.IsInStreamlinedMode)
			{
				if (hand.Input.TouchpadWestDown)
				{
					m_hasValidPoint = false;
					TurnCounterClockWise();
				}
				else if (hand.Input.TouchpadEastDown)
				{
					m_hasValidPoint = false;
					TurnClockWise();
				}
			}
		}

		public void InitArmSwinger()
		{
			CurNeckPos = GM.CurrentPlayerBody.NeckJointTransform.position;
			LastNeckPos = GM.CurrentPlayerBody.NeckJointTransform.position;
		}

		private void UpdateModeArmSwinger()
		{
			bool flag = false;
			bool flag2 = false;
			if (GM.Options.MovementOptions.ArmSwingerSnapTurnState == MovementOptions.ArmSwingerSnapTurnMode.Smooth)
			{
				for (int i = 0; i < Hands.Length; i++)
				{
					if (Hands[i].CMode == ControlMode.Index || Hands[i].CMode == ControlMode.WMR)
					{
						if (Hands[i].Input.Secondary2AxisWestPressed)
						{
							flag2 = true;
						}
						else if (Hands[i].Input.Secondary2AxisEastPressed)
						{
							flag = true;
						}
					}
					else if (Hands[i].IsInStreamlinedMode)
					{
						if (Hands[i].Input.TouchpadWestDown)
						{
							flag2 = true;
						}
						else if (Hands[i].Input.TouchpadEastDown)
						{
							flag = true;
						}
					}
				}
				if (flag && flag2)
				{
					flag = false;
					flag2 = false;
				}
			}
			CurNeckPos = GM.CurrentPlayerBody.NeckJointTransform.position;
			Vector3 vector = LastNeckPos - CurNeckPos;
			Vector3 lastNeckPos = LastNeckPos;
			Vector3 vector2 = CurNeckPos - LastNeckPos;
			if (Physics.SphereCast(LastNeckPos, 0.15f, vector2.normalized, out var hitInfo, vector2.magnitude, LM_TeleCast))
			{
				correctionDir = -vector2 * 1f;
			}
			float num = 0f;
			bool flag3 = false;
			bool flag4 = false;
			for (int j = 0; j < Hands.Length; j++)
			{
				bool flag5 = false;
				if ((!Hands[j].IsInStreamlinedMode) ? Hands[j].Input.BYButtonPressed : ((Hands[j].CMode != ControlMode.Index && Hands[j].CMode != ControlMode.WMR) ? Hands[j].Input.TouchpadNorthPressed : Hands[j].Input.Secondary2AxisNorthPressed))
				{
					float magnitude = Hands[j].Input.VelLinearWorld.magnitude;
					float num2 = 0f;
					num2 = ((!Hands[j].IsThisTheRightHand) ? GM.Options.MovementOptions.ArmSwingerBaseSpeeMagnitudes[GM.Options.MovementOptions.ArmSwingerBaseSpeed_Left] : GM.Options.MovementOptions.ArmSwingerBaseSpeeMagnitudes[GM.Options.MovementOptions.ArmSwingerBaseSpeed_Right]);
					magnitude = Mathf.Clamp(magnitude, num2, magnitude);
					num += magnitude;
					switch (j)
					{
					case 0:
						flag3 = true;
						break;
					case 1:
						flag4 = true;
						break;
					}
				}
			}
			m_tarArmSwingerImpetus = num;
			m_curArmSwingerImpetus = Mathf.MoveTowards(m_curArmSwingerImpetus, m_tarArmSwingerImpetus, 4f * Time.deltaTime);
			if (m_armSwingerGrounded)
			{
				Vector3 vector3 = Vector3.zero;
				if (flag3)
				{
					vector3 += Hands[0].PointingTransform.forward;
				}
				if (flag4)
				{
					vector3 += Hands[1].PointingTransform.forward;
				}
				if (flag3 && flag4)
				{
					vector3 *= 0.5f;
				}
				vector3.y = 0f;
				vector3.Normalize();
				vector3 = vector3 * m_curArmSwingerImpetus * 1.5f;
				m_armSwingerVelocity.x = vector3.x;
				m_armSwingerVelocity.z = vector3.z;
			}
			else if (GM.CurrentSceneSettings.DoesAllowAirControl)
			{
				Vector3 vector4 = Vector3.zero;
				if (flag3)
				{
					vector4 += Hands[0].PointingTransform.forward;
				}
				if (flag4)
				{
					vector4 += Hands[1].PointingTransform.forward;
				}
				if (flag3 && flag4)
				{
					vector4 *= 0.5f;
				}
				vector4.y = 0f;
				vector4.Normalize();
				vector4 = vector4 * m_curArmSwingerImpetus * 1.5f;
				Vector3 vector5 = new Vector3(m_armSwingerVelocity.x, 0f, m_armSwingerVelocity.z);
				m_armSwingerVelocity.x += vector4.x * Time.deltaTime;
				m_armSwingerVelocity.z += vector4.z * Time.deltaTime;
				Vector3 vector6 = new Vector3(m_armSwingerVelocity.x, 0f, m_armSwingerVelocity.z);
				float maxLength = Mathf.Max(1f, vector5.magnitude);
				vector6 = Vector3.ClampMagnitude(vector6, maxLength);
				m_armSwingerVelocity.x = vector6.x;
				m_armSwingerVelocity.z = vector6.z;
			}
			else
			{
				Vector3 vector7 = Vector3.zero;
				if (flag3)
				{
					vector7 += Hands[0].PointingTransform.forward;
				}
				if (flag4)
				{
					vector7 += Hands[1].PointingTransform.forward;
				}
				if (flag3 && flag4)
				{
					vector7 *= 0.5f;
				}
				vector7.y = 0f;
				vector7.Normalize();
				vector7 = vector7 * m_curArmSwingerImpetus * 0.3f;
				Vector3 vector8 = new Vector3(m_armSwingerVelocity.x, 0f, m_armSwingerVelocity.z);
				m_armSwingerVelocity.x += vector7.x * Time.deltaTime;
				m_armSwingerVelocity.z += vector7.z * Time.deltaTime;
				Vector3 vector9 = new Vector3(m_armSwingerVelocity.x, 0f, m_armSwingerVelocity.z);
				float maxLength2 = Mathf.Max(1f, vector8.magnitude);
				vector9 = Vector3.ClampMagnitude(vector9, maxLength2);
				m_armSwingerVelocity.x = vector9.x;
				m_armSwingerVelocity.z = vector9.z;
			}
			Vector3 vector10 = lastNeckPos;
			Vector3 b = vector10;
			vector10.y = Mathf.Max(vector10.y, base.transform.position.y + m_armSwingerStepHeight);
			b.y = base.transform.position.y;
			float num3 = Vector3.Distance(vector10, b);
			if (m_delayGroundCheck > 0f)
			{
				num3 *= 0.5f;
			}
			bool flag6 = false;
			Vector3 planeNormal = Vector3.up;
			bool flag7 = false;
			Vector3 vector11 = Vector3.up;
			Vector3 groundPoint = vector10 + -Vector3.up * num3;
			Vector3 groundPoint2 = vector10 + -Vector3.up * num3;
			float num4 = 90f;
			float a = -1000f;
			if (Physics.SphereCast(vector10, 0.2f, -Vector3.up, out m_hit_ray, num3, LM_TeleCast))
			{
				vector11 = m_hit_ray.normal;
				groundPoint = m_hit_ray.point;
				groundPoint2 = m_hit_ray.point;
				num4 = 90f;
				a = groundPoint.y;
				flag7 = true;
				Vector3 origin = new Vector3(m_hit_ray.point.x, vector10.y, m_hit_ray.point.z);
				num4 = ((!Physics.Raycast(origin, -Vector3.up, out m_hit_ray, num3, LM_TeleCast)) ? 45f : Vector3.Angle(Vector3.up, m_hit_ray.normal));
			}
			if (Physics.Raycast(vector10, -Vector3.up, out m_hit_ray, num3, LM_TeleCast))
			{
				num4 = Mathf.Min(num4, Vector3.Angle(Vector3.up, m_hit_ray.normal));
				vector11 = m_hit_ray.normal;
				groundPoint.y = Mathf.Max(groundPoint.y, m_hit_ray.point.y);
				groundPoint2.y = Mathf.Min(groundPoint.y, m_hit_ray.point.y);
				a = Mathf.Max(a, m_hit_ray.point.y);
				flag7 = true;
			}
			Vector3 vector12 = Head.forward;
			vector12.y = 0f;
			vector12.Normalize();
			vector12 = Vector3.ClampMagnitude(vector12, 0.1f);
			Vector3 vector13 = Head.right;
			vector13.y = 0f;
			vector13.Normalize();
			vector13 = Vector3.ClampMagnitude(vector13, 0.1f);
			Vector3 vector14 = -vector12;
			Vector3 vector15 = -vector13;
			if (Physics.Raycast(vector10 + vector12, -Vector3.up, out m_hit_ray, num3, LM_TeleCast))
			{
				num4 = Mathf.Min(num4, Vector3.Angle(Vector3.up, m_hit_ray.normal));
				groundPoint.y = Mathf.Max(groundPoint.y, m_hit_ray.point.y);
				groundPoint2.y = Mathf.Min(groundPoint.y, m_hit_ray.point.y);
				a = Mathf.Max(a, m_hit_ray.point.y);
				if (!flag7)
				{
					vector11 = m_hit_ray.normal;
					flag7 = true;
				}
			}
			if (Physics.Raycast(vector10 + vector13, -Vector3.up, out m_hit_ray, num3, LM_TeleCast))
			{
				num4 = Mathf.Min(num4, Vector3.Angle(Vector3.up, m_hit_ray.normal));
				groundPoint.y = Mathf.Max(groundPoint.y, m_hit_ray.point.y);
				groundPoint2.y = Mathf.Min(groundPoint.y, m_hit_ray.point.y);
				a = Mathf.Max(a, m_hit_ray.point.y);
				if (!flag7)
				{
					vector11 = m_hit_ray.normal;
					flag7 = true;
				}
			}
			if (Physics.Raycast(vector10 + vector14, -Vector3.up, out m_hit_ray, num3, LM_TeleCast))
			{
				num4 = Mathf.Min(num4, Vector3.Angle(Vector3.up, m_hit_ray.normal));
				groundPoint.y = Mathf.Max(groundPoint.y, m_hit_ray.point.y);
				groundPoint2.y = Mathf.Min(groundPoint.y, m_hit_ray.point.y);
				a = Mathf.Max(a, m_hit_ray.point.y);
				if (!flag7)
				{
					vector11 = m_hit_ray.normal;
					flag7 = true;
				}
			}
			if (Physics.Raycast(vector10 + vector15, -Vector3.up, out m_hit_ray, num3, LM_TeleCast))
			{
				num4 = Mathf.Min(num4, Vector3.Angle(Vector3.up, m_hit_ray.normal));
				groundPoint.y = Mathf.Max(groundPoint.y, m_hit_ray.point.y);
				groundPoint2.y = Mathf.Min(groundPoint.y, m_hit_ray.point.y);
				a = Mathf.Max(a, m_hit_ray.point.y);
				if (!flag7)
				{
					vector11 = m_hit_ray.normal;
					flag7 = true;
				}
			}
			if (flag7)
			{
				if (num4 > 70f)
				{
					flag6 = true;
					m_armSwingerGrounded = false;
					planeNormal = vector11;
					m_groundPoint = groundPoint2;
				}
				else
				{
					m_armSwingerGrounded = true;
					m_groundPoint = groundPoint;
				}
			}
			else
			{
				m_armSwingerGrounded = false;
				m_groundPoint = vector10 - Vector3.up * num3;
			}
			Vector3 vector16 = lastNeckPos;
			Vector3 b2 = vector16;
			b2.y = base.transform.position.y + 2.15f * GM.CurrentPlayerBody.transform.localScale.y;
			float maxDistance = Vector3.Distance(vector16, b2);
			float num5 = vector16.y + 0.15f;
			if (Physics.SphereCast(vector16, 0.15f, Vector3.up, out m_hit_ray, maxDistance, LM_TeleCast))
			{
				Vector3 point = m_hit_ray.point;
				float num6 = Vector3.Distance(vector16, new Vector3(vector16.x, point.y, vector16.z));
				num5 = m_hit_ray.point.y - 0.15f;
				float num7 = Mathf.Clamp(GM.CurrentPlayerBody.Head.localPosition.y, 0.3f, 2.5f);
				float y = m_groundPoint.y;
				float min = y - (num7 - 0.2f);
				float y2 = Mathf.Clamp(num5 - num7 - 0.15f, min, y);
				m_groundPoint.y = y2;
			}
			if (m_armSwingerGrounded)
			{
				m_armSwingerVelocity.y = 0f;
			}
			else
			{
				float num8 = 5f;
				switch (GM.Options.SimulationOptions.PlayerGravityMode)
				{
				case SimulationOptions.GravityMode.Realistic:
					num8 = 9.81f;
					break;
				case SimulationOptions.GravityMode.Playful:
					num8 = 5f;
					break;
				case SimulationOptions.GravityMode.OnTheMoon:
					num8 = 1.62f;
					break;
				case SimulationOptions.GravityMode.None:
					num8 = 0.001f;
					break;
				}
				if (!flag6)
				{
					m_armSwingerVelocity.y -= num8 * Time.deltaTime;
				}
				else
				{
					Vector3 vector17 = Vector3.ProjectOnPlane(-Vector3.up * num8, planeNormal);
					m_armSwingerVelocity += vector17 * Time.deltaTime;
					m_armSwingerVelocity = Vector3.ProjectOnPlane(m_armSwingerVelocity, planeNormal);
				}
			}
			float num9 = Mathf.Abs(lastNeckPos.y - GM.CurrentPlayerBody.transform.position.y);
			Vector3 point2 = lastNeckPos;
			Vector3 point3 = lastNeckPos;
			point2.y = Mathf.Min(point2.y, num5 - 0.01f);
			point3.y = Mathf.Max(base.transform.position.y, m_groundPoint.y) + (m_armSwingerStepHeight + 0.2f);
			point2.y = Mathf.Max(point2.y, point3.y);
			Vector3 vector18 = m_armSwingerVelocity;
			float maxLength3 = m_armSwingerVelocity.magnitude * Time.deltaTime;
			if (Physics.CapsuleCast(point2, point3, 0.15f, m_armSwingerVelocity, out m_hit_ray, m_armSwingerVelocity.magnitude * Time.deltaTime + 0.1f, LM_TeleCast))
			{
				vector18 = Vector3.ProjectOnPlane(m_armSwingerVelocity, m_hit_ray.normal);
				maxLength3 = m_hit_ray.distance * 0.5f;
				if (m_armSwingerGrounded)
				{
					vector18.y = 0f;
				}
				if (Physics.CapsuleCast(point2, point3, 0.15f, vector18, out var hitInfo2, vector18.magnitude * Time.deltaTime + 0.1f, LM_TeleCast))
				{
					maxLength3 = hitInfo2.distance * 0.5f;
				}
			}
			m_armSwingerVelocity = vector18;
			if (m_armSwingerGrounded)
			{
				m_armSwingerVelocity.y = 0f;
			}
			Vector3 vector19 = base.transform.position;
			Vector3 vector20 = m_armSwingerVelocity * Time.deltaTime;
			vector20 = Vector3.ClampMagnitude(vector20, maxLength3);
			vector19 = base.transform.position + vector20;
			if (m_armSwingerGrounded)
			{
				vector19.y = Mathf.MoveTowards(vector19.y, m_groundPoint.y, 8f * Time.deltaTime * Mathf.Abs(base.transform.position.y - m_groundPoint.y));
			}
			Vector3 vector21 = CurNeckPos + vector20;
			vector2 = vector21 - LastNeckPos;
			if (Physics.SphereCast(LastNeckPos, 0.15f, vector2.normalized, out hitInfo, vector2.magnitude, LM_TeleCast))
			{
				correctionDir = -vector2 * 1f;
			}
			if (GM.Options.MovementOptions.AXButtonSnapTurnState == MovementOptions.AXButtonSnapTurnMode.Smoothturn)
			{
				for (int k = 0; k < Hands.Length; k++)
				{
					if (Hands[k].IsInStreamlinedMode)
					{
						continue;
					}
					if (Hands[k].IsThisTheRightHand)
					{
						if (Hands[k].Input.AXButtonPressed)
						{
							flag = true;
						}
					}
					else if (Hands[k].Input.AXButtonPressed)
					{
						flag2 = true;
					}
				}
			}
			if (!flag && !flag2)
			{
				base.transform.position = vector19 + correctionDir;
			}
			else
			{
				Vector3 point4 = vector19 + correctionDir;
				Vector3 forward = GM.CurrentPlayerBody.transform.forward;
				float num10 = GM.Options.MovementOptions.SmoothTurnMagnitudes[GM.Options.MovementOptions.SmoothTurnMagnitudeIndex] * Time.deltaTime;
				if (flag2)
				{
					num10 = 0f - num10;
				}
				point4 = RotatePointAroundPivotWithEuler(point4, CurNeckPos, new Vector3(0f, num10, 0f));
				forward = Quaternion.AngleAxis(num10, Vector3.up) * forward;
				base.transform.SetPositionAndRotation(point4, Quaternion.LookRotation(forward, Vector3.up));
			}
			if (GM.Options.MovementOptions.ArmSwingerJumpState == MovementOptions.ArmSwingerJumpMode.Enabled && Hands[0].transform.position.y > Head.position.y && Hands[1].transform.position.y > Head.position.y && Hands[0].Input.VelLinearWorld.y > 2f && Hands[1].Input.VelLinearWorld.y > 2f)
			{
				Jump();
			}
			SetTopSpeedLastSecond(m_armSwingerVelocity);
			SetFrameSpeed(m_armSwingerVelocity);
			LastNeckPos = GM.CurrentPlayerBody.NeckJointTransform.position;
		}

		public void BeginGrabPointMove(FVRHandGrabPoint grabPoint)
		{
			CleanupFlagsForModeSwitch();
			m_curGrabPoint = grabPoint;
			m_lastHandPos = m_curGrabPoint.m_hand.Input.Pos;
		}

		public void EndGrabPointMove(FVRHandGrabPoint grabPoint)
		{
			if (m_curGrabPoint == grabPoint)
			{
				m_curGrabPoint = null;
			}
			m_twoAxisVelocity = -grabPoint.m_hand.Input.VelLinearWorld;
			m_armSwingerVelocity = -grabPoint.m_hand.Input.VelLinearWorld;
			m_twoAxisGrounded = false;
			m_armSwingerGrounded = false;
			InitArmSwinger();
		}

		public void UpdateGrabPointMove()
		{
			Vector3 position = base.transform.position;
			Vector3 vector = m_lastHandPos - m_curGrabPoint.m_hand.Input.Pos;
			Vector3 position2 = base.transform.position;
			position2 += vector;
			base.transform.position = position2;
			m_lastHandPos = m_curGrabPoint.m_hand.Input.Pos;
		}

		private void HandUpdateJoyStickTeleport(FVRViveHand hand)
		{
			if (hand.Input.BYButtonDown && !m_joyStickTeleportInProgress)
			{
				if (hand != m_authoratativeHand)
				{
					m_authoratativeHand = hand;
				}
				else if (hand == m_authoratativeHand)
				{
					m_authoratativeHand = null;
				}
			}
			if (m_timeSinceSnapTurn < 2f)
			{
				m_timeSinceSnapTurn += Time.deltaTime;
			}
			if (m_authoratativeHand == null || hand != m_authoratativeHand)
			{
				return;
			}
			if (!m_joystickTPArrows.activeSelf)
			{
				m_joystickTPArrows.SetActive(value: true);
			}
			if (m_joystickTPArrows.transform.parent != m_authoratativeHand.TouchpadArrowTarget)
			{
				m_joystickTPArrows.transform.parent = m_authoratativeHand.TouchpadArrowTarget;
				m_joystickTPArrows.transform.localPosition = Vector3.zero;
				m_joystickTPArrows.transform.localRotation = Quaternion.identity;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			Vector2 vector;
			if (hand.CMode == ControlMode.Vive || hand.CMode == ControlMode.Oculus || hand.CMode == ControlMode.WMR)
			{
				flag = hand.Input.TouchpadUp;
				flag2 = hand.Input.TouchpadDown;
				flag3 = hand.Input.TouchpadPressed;
				vector = hand.Input.TouchpadAxes;
				flag4 = hand.Input.TouchpadCenterDown;
				flag5 = hand.Input.TouchpadTouchUp;
				flag6 = hand.Input.TouchpadTouched;
			}
			else
			{
				flag = hand.Input.Secondary2AxisInputUp;
				flag2 = hand.Input.Secondary2AxisInputDown;
				flag3 = hand.Input.Secondary2AxisInputPressed;
				vector = hand.Input.Secondary2AxisInputAxes;
				flag4 = hand.Input.Secondary2AxisCenterDown;
				flag5 = hand.Input.Secondary2AxisInputTouchUp;
				flag6 = hand.Input.Secondary2AxisInputTouched;
			}
			if (Mathf.Abs(vector.x) < 0.3f)
			{
				hasXAxisReset = true;
			}
			if (flag5 && m_joyStickTeleportInProgress)
			{
				m_joyStickTeleportCooldown = 0.25f;
				m_joyStickTeleportInProgress = false;
				if (MovementRig.gameObject.activeSelf)
				{
					MovementRig.gameObject.SetActive(value: false);
				}
				if (m_hasValidPoint)
				{
					m_teleportEnergy -= TeleportToPoint(m_validPoint, isAbsolute: true, m_worldPointDir);
				}
				for (int i = 0; i < 20; i++)
				{
					if (Cylinders[i].gameObject.activeSelf)
					{
						Cylinders[i].gameObject.SetActive(value: false);
					}
				}
			}
			else if (flag6 && m_joyStickTeleportCooldown <= 0f && ((vector.magnitude > 0.8f && m_joyStickTeleportInProgress) || Mathf.Abs(vector.y) > 0.8f))
			{
				m_joyStickTeleportInProgress = true;
				Vector3 zero = Vector3.zero;
				m_validPoint = FindValidPointCurved(hand.Input.FilteredPos, hand.PointingTransform.forward, 0.5f);
				zero = hand.PointingTransform.forward;
				if (!m_hasValidPoint)
				{
					if (MovementRig.gameObject.activeSelf)
					{
						MovementRig.gameObject.SetActive(value: false);
					}
					return;
				}
				if (!MovementRig.gameObject.activeSelf)
				{
					MovementRig.gameObject.SetActive(value: true);
				}
				Vector3 forward = Head.transform.forward;
				forward.y = 0f;
				MovementRig.transform.position = m_validPoint;
				Vector3 vector2 = hand.transform.forward * vector.y + hand.transform.right * vector.x;
				vector2.y = 0f;
				vector2.Normalize();
				m_worldPointDir = vector2;
				MovementRig.transform.rotation = Quaternion.LookRotation(vector2, Vector3.up);
				if (MovementRig.CornerHolder.gameObject.activeSelf)
				{
					MovementRig.CornerHolder.gameObject.SetActive(value: false);
				}
			}
			else if (!m_joyStickTeleportInProgress && Mathf.Abs(vector.x) > 0.8f && hasXAxisReset && flag6)
			{
				hasXAxisReset = false;
				Vector3 position = GM.CurrentPlayerBody.Head.position;
				position.y = base.transform.position.y;
				Vector3 forward2 = base.transform.forward;
				float num = GM.Options.MovementOptions.SnapTurnMagnitudes[GM.Options.MovementOptions.SnapTurnMagnitudeIndex];
				m_teleportEnergy -= TeleportToPoint(lookDir: (!(vector.x > 0f)) ? (Quaternion.AngleAxis(0f - num, Vector3.up) * forward2) : (Quaternion.AngleAxis(num, Vector3.up) * forward2), point: position, isAbsolute: false);
				m_teleportCooldown = 0.2f;
			}
		}

		private void HandUpdateTwinstick(FVRViveHand hand)
		{
			bool flag = hand.IsThisTheRightHand;
			if (GM.Options.MovementOptions.TwinStickLeftRightState == MovementOptions.TwinStickLeftRightSetup.RightStickMove)
			{
				flag = !flag;
			}
			if (!hand.IsInStreamlinedMode && (hand.CMode == ControlMode.Vive || hand.CMode == ControlMode.Oculus))
			{
				if (hand.Input.BYButtonDown)
				{
					if (flag)
					{
						m_isRightHandActive = !m_isRightHandActive;
					}
					if (!flag)
					{
						m_isLeftHandActive = !m_isLeftHandActive;
					}
				}
			}
			else
			{
				m_isLeftHandActive = true;
				m_isRightHandActive = true;
			}
			if (flag && !m_isRightHandActive)
			{
				if (m_twinStickArrowsRight.activeSelf)
				{
					m_twinStickArrowsRight.SetActive(value: false);
				}
				m_isTwinStickSmoothTurningCounterClockwise = false;
				m_isTwinStickSmoothTurningClockwise = false;
				return;
			}
			if (!flag && !m_isLeftHandActive)
			{
				if (m_twinStickArrowsLeft.activeSelf)
				{
					m_twinStickArrowsLeft.SetActive(value: false);
				}
				return;
			}
			if (!hand.IsInStreamlinedMode && (hand.CMode == ControlMode.Vive || hand.CMode == ControlMode.Oculus))
			{
				if (flag)
				{
					if (!m_twinStickArrowsRight.activeSelf)
					{
						m_twinStickArrowsRight.SetActive(value: true);
					}
					if (m_twinStickArrowsRight.transform.parent != hand.TouchpadArrowTarget)
					{
						m_twinStickArrowsRight.transform.SetParent(hand.TouchpadArrowTarget);
						m_twinStickArrowsRight.transform.localPosition = Vector3.zero;
						m_twinStickArrowsRight.transform.localRotation = Quaternion.identity;
					}
				}
				else
				{
					if (!m_twinStickArrowsLeft.activeSelf)
					{
						m_twinStickArrowsLeft.SetActive(value: true);
					}
					if (m_twinStickArrowsLeft.transform.parent != hand.TouchpadArrowTarget)
					{
						m_twinStickArrowsLeft.transform.SetParent(hand.TouchpadArrowTarget);
						m_twinStickArrowsLeft.transform.localPosition = Vector3.zero;
						m_twinStickArrowsLeft.transform.localRotation = Quaternion.identity;
					}
				}
			}
			if (m_timeSinceSprintDownClick < 2f)
			{
				m_timeSinceSprintDownClick += Time.deltaTime;
			}
			if (m_timeSinceSnapTurn < 2f)
			{
				m_timeSinceSnapTurn += Time.deltaTime;
			}
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			Vector2 vector;
			if (hand.CMode == ControlMode.Vive || hand.CMode == ControlMode.Oculus)
			{
				flag2 = hand.Input.TouchpadUp;
				flag3 = hand.Input.TouchpadDown;
				flag4 = hand.Input.TouchpadPressed;
				vector = hand.Input.TouchpadAxes;
				flag5 = hand.Input.TouchpadNorthDown;
				flag6 = hand.Input.TouchpadNorthPressed;
			}
			else
			{
				flag2 = hand.Input.Secondary2AxisInputUp;
				flag3 = hand.Input.Secondary2AxisInputDown;
				flag4 = hand.Input.Secondary2AxisInputPressed;
				vector = hand.Input.Secondary2AxisInputAxes;
				flag5 = hand.Input.Secondary2AxisNorthDown;
				flag6 = hand.Input.Secondary2AxisNorthPressed;
			}
			if (flag)
			{
				m_isTwinStickSmoothTurningCounterClockwise = false;
				m_isTwinStickSmoothTurningClockwise = false;
				if (GM.Options.MovementOptions.TwinStickSnapturnState == MovementOptions.TwinStickSnapturnMode.Enabled)
				{
					if (hand.CMode == ControlMode.Oculus)
					{
						if (hand.Input.TouchpadWestDown)
						{
							TurnCounterClockWise();
						}
						else if (hand.Input.TouchpadEastDown)
						{
							TurnClockWise();
						}
					}
					else if (hand.CMode == ControlMode.Vive)
					{
						if (GM.Options.MovementOptions.Touchpad_Confirm == TwoAxisMovementConfirm.OnClick)
						{
							if (hand.Input.TouchpadDown)
							{
								if (hand.Input.TouchpadWestPressed)
								{
									TurnCounterClockWise();
								}
								else if (hand.Input.TouchpadEastPressed)
								{
									TurnClockWise();
								}
							}
						}
						else if (hand.Input.TouchpadWestDown)
						{
							TurnCounterClockWise();
						}
						else if (hand.Input.TouchpadEastDown)
						{
							TurnClockWise();
						}
					}
					else if (hand.Input.Secondary2AxisWestDown)
					{
						TurnCounterClockWise();
					}
					else if (hand.Input.Secondary2AxisEastDown)
					{
						TurnClockWise();
					}
				}
				else if (GM.Options.MovementOptions.TwinStickSnapturnState == MovementOptions.TwinStickSnapturnMode.Smooth)
				{
					if (hand.CMode == ControlMode.Oculus)
					{
						if (hand.Input.TouchpadWestPressed)
						{
							m_isTwinStickSmoothTurningCounterClockwise = true;
						}
						else if (hand.Input.TouchpadEastPressed)
						{
							m_isTwinStickSmoothTurningClockwise = true;
						}
					}
					else if (hand.CMode == ControlMode.Vive)
					{
						if (GM.Options.MovementOptions.Touchpad_Confirm == TwoAxisMovementConfirm.OnClick)
						{
							if (hand.Input.TouchpadPressed)
							{
								if (hand.Input.TouchpadWestPressed)
								{
									m_isTwinStickSmoothTurningCounterClockwise = true;
								}
								else if (hand.Input.TouchpadEastPressed)
								{
									m_isTwinStickSmoothTurningClockwise = true;
								}
							}
						}
						else if (hand.Input.TouchpadWestPressed)
						{
							m_isTwinStickSmoothTurningCounterClockwise = true;
						}
						else if (hand.Input.TouchpadEastPressed)
						{
							m_isTwinStickSmoothTurningClockwise = true;
						}
					}
					else if (hand.Input.Secondary2AxisWestPressed)
					{
						m_isTwinStickSmoothTurningCounterClockwise = true;
					}
					else if (hand.Input.Secondary2AxisEastPressed)
					{
						m_isTwinStickSmoothTurningClockwise = true;
					}
				}
				if (GM.Options.MovementOptions.TwinStickJumpState == MovementOptions.TwinStickJumpMode.Enabled)
				{
					if (hand.CMode == ControlMode.Oculus)
					{
						if (hand.Input.TouchpadSouthDown)
						{
							Jump();
						}
					}
					else if (hand.CMode == ControlMode.Vive)
					{
						if (GM.Options.MovementOptions.Touchpad_Confirm == TwoAxisMovementConfirm.OnClick)
						{
							if (hand.Input.TouchpadDown && hand.Input.TouchpadSouthPressed)
							{
								Jump();
							}
						}
						else if (hand.Input.TouchpadSouthDown)
						{
							Jump();
						}
					}
					else if (hand.Input.Secondary2AxisSouthDown)
					{
						Jump();
					}
				}
				if (GM.Options.MovementOptions.TwinStickSprintState != MovementOptions.TwinStickSprintMode.RightStickForward)
				{
					return;
				}
				if (GM.Options.MovementOptions.TwinStickSprintToggleState == MovementOptions.TwinStickSprintToggleMode.Disabled)
				{
					if (flag6)
					{
						m_sprintingEngaged = true;
					}
					else
					{
						m_sprintingEngaged = false;
					}
				}
				else if (flag5)
				{
					m_sprintingEngaged = !m_sprintingEngaged;
				}
				return;
			}
			if (GM.Options.MovementOptions.TwinStickSprintState == MovementOptions.TwinStickSprintMode.LeftStickClick)
			{
				if (GM.Options.MovementOptions.TwinStickSprintToggleState == MovementOptions.TwinStickSprintToggleMode.Disabled)
				{
					if (flag4)
					{
						m_sprintingEngaged = true;
					}
					else
					{
						m_sprintingEngaged = false;
					}
				}
				else if (flag3)
				{
					m_sprintingEngaged = !m_sprintingEngaged;
				}
			}
			Vector3 vector2 = Vector3.zero;
			float y = vector.y;
			float x = vector.x;
			switch (GM.Options.MovementOptions.Touchpad_MovementMode)
			{
			case TwoAxisMovementMode.Standard:
				vector2 = y * hand.PointingTransform.forward + x * hand.PointingTransform.right * 0.75f;
				vector2.y = 0f;
				break;
			case TwoAxisMovementMode.Onward:
				vector2 = y * hand.transform.forward + x * hand.transform.right * 0.75f;
				break;
			case TwoAxisMovementMode.LeveledHand:
			{
				Vector3 forward2 = hand.transform.forward;
				forward2.y = 0f;
				forward2.Normalize();
				Vector3 right2 = hand.transform.right;
				right2.y = 0f;
				right2.Normalize();
				vector2 = y * forward2 + x * right2 * 0.75f;
				break;
			}
			case TwoAxisMovementMode.LeveledHead:
			{
				Vector3 forward = GM.CurrentPlayerBody.Head.forward;
				forward.y = 0f;
				forward.Normalize();
				Vector3 right = GM.CurrentPlayerBody.Head.right;
				right.y = 0f;
				right.Normalize();
				vector2 = y * forward + x * right * 0.75f;
				break;
			}
			}
			Vector3 normalized = vector2.normalized;
			vector2 *= GM.Options.MovementOptions.TPLocoSpeeds[GM.Options.MovementOptions.TPLocoSpeedIndex];
			if (hand.CMode == ControlMode.Vive && GM.Options.MovementOptions.Touchpad_Confirm == TwoAxisMovementConfirm.OnClick)
			{
				if (!flag4)
				{
					vector2 = Vector3.zero;
				}
				else if (m_sprintingEngaged && GM.Options.MovementOptions.TPLocoSpeedIndex < 5)
				{
					vector2 += normalized * 2f;
				}
			}
			else if (m_sprintingEngaged && GM.Options.MovementOptions.TPLocoSpeedIndex < 5)
			{
				vector2 += normalized * 2f;
			}
			if (m_twoAxisGrounded)
			{
				m_twoAxisVelocity.x = vector2.x;
				m_twoAxisVelocity.z = vector2.z;
				if (GM.CurrentSceneSettings.UsesMaxSpeedClamp)
				{
					Vector2 vector3 = new Vector2(m_twoAxisVelocity.x, m_twoAxisVelocity.z);
					if (vector3.magnitude > GM.CurrentSceneSettings.MaxSpeedClamp)
					{
						vector3 = vector3.normalized * GM.CurrentSceneSettings.MaxSpeedClamp;
						m_twoAxisVelocity.x = vector3.x;
						m_twoAxisVelocity.z = vector3.y;
					}
				}
			}
			else if (GM.CurrentSceneSettings.DoesAllowAirControl)
			{
				Vector3 vector4 = new Vector3(m_twoAxisVelocity.x, 0f, m_twoAxisVelocity.z);
				m_twoAxisVelocity.x += vector2.x * Time.deltaTime;
				m_twoAxisVelocity.z += vector2.z * Time.deltaTime;
				Vector3 vector5 = new Vector3(m_twoAxisVelocity.x, 0f, m_twoAxisVelocity.z);
				float maxLength = Mathf.Max(1f, vector4.magnitude);
				vector5 = Vector3.ClampMagnitude(vector5, maxLength);
				m_twoAxisVelocity.x = vector5.x;
				m_twoAxisVelocity.z = vector5.z;
			}
			else
			{
				Vector3 vector6 = new Vector3(m_twoAxisVelocity.x, 0f, m_twoAxisVelocity.z);
				m_twoAxisVelocity.x += vector2.x * Time.deltaTime * 0.3f;
				m_twoAxisVelocity.z += vector2.z * Time.deltaTime * 0.3f;
				Vector3 vector7 = new Vector3(m_twoAxisVelocity.x, 0f, m_twoAxisVelocity.z);
				float maxLength2 = Mathf.Max(1f, vector6.magnitude);
				vector7 = Vector3.ClampMagnitude(vector7, maxLength2);
				m_twoAxisVelocity.x = vector7.x;
				m_twoAxisVelocity.z = vector7.z;
			}
			if (flag3)
			{
				m_timeSinceSprintDownClick = 0f;
			}
		}

		public Vector3 FindValidPointCurved(Vector3 castOrigin, Vector3 castDir, float initialVel)
		{
			m_hasValidPoint = false;
			Vector3 result = Vector3.zero;
			Vector3 vector = castDir * initialVel * Mathf.Clamp(m_teleportEnergy, 0.1f, 1f);
			float num = 1f / GM.CurrentPlayerBody.transform.localScale.x;
			int num2 = 0;
			for (int i = 0; i < 20; i++)
			{
				if (i == 19)
				{
					vector = -Vector3.up * 10f + vector * 0.01f;
				}
				if (Physics.Raycast(castOrigin, vector, out m_hit_ray, vector.magnitude, LM_TeleCast) && !m_hit_ray.transform.gameObject.CompareTag("NoTeleport"))
				{
					if (!(Vector3.Dot(m_hit_ray.normal, Vector3.up) >= 0.5f) || Physics.CheckCapsule(m_hit_ray.point + Vector3.up * 0.4f, m_hit_ray.point + Vector3.up * (Head.transform.localPosition.y - 0.2f), 0.2f, LM_PointSearch))
					{
						break;
					}
					m_hasValidPoint = true;
					result = m_hit_ray.point;
				}
				Cylinders[i].gameObject.SetActive(value: true);
				Cylinders[i].position = castOrigin;
				Cylinders[i].rotation = Quaternion.LookRotation(vector);
				Cylinders[i].localScale = new Vector3(0.1f, 0.1f, vector.magnitude) * num;
				if (m_hasValidPoint)
				{
					Cylinders[i].localScale = new Vector3(0.1f, 0.1f, m_hit_ray.distance) * num;
					num2 = i + 1;
					break;
				}
				castOrigin += vector;
				vector *= 0.95f;
				vector += -Vector3.up * 9.8f * 0.003f;
			}
			for (int j = num2; j < 20; j++)
			{
				if (Cylinders[j].gameObject.activeSelf)
				{
					Cylinders[j].gameObject.SetActive(value: false);
				}
			}
			return result;
		}

		public Vector3 FindValidPointCurvedForRotatedTeleport(Vector3 castOrigin, Vector3 castDir, float initialVel)
		{
			m_hasValidRotateDir = false;
			Vector3 vector = Vector3.zero;
			if (!m_hasValidPoint)
			{
				return vector;
			}
			Vector3 vector2 = castDir * initialVel * Mathf.Clamp(m_teleportEnergy, 0.1f, 1f);
			int num = 0;
			for (int i = 0; i < 20; i++)
			{
				if (i == 19)
				{
					vector2 = -Vector3.up * 10f + vector2 * 0.01f;
				}
				if (Physics.Raycast(castOrigin, vector2, out m_hit_ray, vector2.magnitude, LM_TeleCast) && !m_hit_ray.transform.gameObject.CompareTag("NoTeleport"))
				{
					if (!(Vector3.Dot(m_hit_ray.normal, Vector3.up) >= 0.5f) || Physics.CheckCapsule(m_hit_ray.point + Vector3.up * 0.4f, m_hit_ray.point + Vector3.up * (Head.transform.localPosition.y - 0.2f), 0.2f, LM_PointSearch))
					{
						break;
					}
					m_hasValidRotateDir = true;
					vector = m_hit_ray.point;
				}
				Cylinders[i].gameObject.SetActive(value: true);
				Cylinders[i].position = castOrigin;
				Cylinders[i].rotation = Quaternion.LookRotation(vector2);
				Cylinders[i].localScale = new Vector3(0.05f, 0.05f, vector2.magnitude);
				if (m_hasValidRotateDir)
				{
					Cylinders[i].localScale = new Vector3(0.05f, 0.05f, m_hit_ray.distance);
					num = i + 1;
					break;
				}
				castOrigin += vector2;
				vector2 *= 0.95f;
				vector2 += -Vector3.up * 9.8f * 0.003f;
			}
			for (int j = num; j < 20; j++)
			{
				if (Cylinders[j].gameObject.activeSelf)
				{
					Cylinders[j].gameObject.SetActive(value: false);
				}
			}
			Vector3 result = vector - m_validPoint;
			result.y = 0f;
			result.Normalize();
			return result;
		}

		public Vector3 RotatePointAroundPivotWithEuler(Vector3 point, Vector3 pivot, Vector3 angles)
		{
			return Quaternion.Euler(angles) * (point - pivot) + pivot;
		}

		public float TeleportToPoint(Vector3 point, bool isAbsolute, Vector3 lookDir)
		{
			lookDir.y = 0f;
			base.transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
			return TeleportToPoint(point, isAbsolute);
		}

		public float TeleportToPoint(Vector3 point, bool isAbsolute)
		{
			m_isDashing = false;
			m_DashTarget = Vector3.zero;
			m_isSliding = false;
			m_SlidingTarget = Vector3.zero;
			Vector3 position = base.transform.position;
			for (int i = 0; i < Hands.Length; i++)
			{
				Hands[i].PollInput();
				Hands[i].FlushFilter();
			}
			if (isAbsolute)
			{
				base.transform.position = point;
			}
			else
			{
				Vector3 vector = base.transform.position - GM.CurrentPlayerBody.NeckJointTransform.position;
				vector.y = 0f;
				base.transform.position = point + vector;
			}
			Body.UpdatePlayerBodyPositions();
			Vector3 vector2 = base.transform.position - position;
			Body.MoveQuickbeltContents(vector2);
			SetTopSpeedLastSecond(vector2);
			for (int j = 0; j < Hands.Length; j++)
			{
				if (Hands[j].CurrentInteractable != null && Hands[j].CurrentInteractable is FVRPhysicalObject && !(Hands[j].CurrentInteractable as FVRPhysicalObject).DoesDirectParent && !(Hands[j].CurrentInteractable as FVRPhysicalObject).IsPivotLocked)
				{
					(Hands[j].CurrentInteractable as FVRPhysicalObject).transform.position = (Hands[j].CurrentInteractable as FVRPhysicalObject).transform.position + (base.transform.position - position);
				}
			}
			lastHeadPos = GM.CurrentPlayerBody.NeckJointTransform.transform.position;
			if (isAbsolute)
			{
				InitArmSwinger();
			}
			return Mathf.Clamp(Vector3.Distance(position, base.transform.position) * 0.3f, 0f, 1f);
		}

		public float SetDashDestination(Vector3 point)
		{
			m_isDashing = true;
			Vector3 vector = base.transform.position - Head.position;
			vector.y = 0f;
			m_DashTarget = point + vector;
			return Mathf.Clamp(Vector3.Distance(point, base.transform.position) * 0.3f, 0f, 1f);
		}

		public void SetSlidingDestination(Vector3 point)
		{
			m_isSliding = true;
			Vector3 vector = base.transform.position - Head.position;
			vector.y = 0f;
			m_SlidingTarget = point + vector;
		}
	}
}
