// Decompiled with JetBrains decompiler
// Type: FistVR.FVRMovementManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRMovementManager : MonoBehaviour
  {
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
    public FVRMovementManager.MovementMode Mode;
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
      GameObject gameObject = Object.Instantiate<GameObject>(this.MovementRigPrefab, Vector3.zero, Quaternion.identity);
      this.MovementRig = gameObject.GetComponent<FVRMovementRig>();
      gameObject.SetActive(false);
      this.m_touchpadArrows = Object.Instantiate<GameObject>(this.TouchpadIndicationArrowPrefab);
      this.m_touchpadArrows.SetActive(false);
      this.m_joystickTPArrows = Object.Instantiate<GameObject>(this.JoyStickTeleportArrowPrefab);
      this.m_joystickTPArrows.SetActive(false);
      this.m_twinStickArrowsLeft = Object.Instantiate<GameObject>(this.TouchpadIndicationArrowPrefab);
      this.m_twinStickArrowsLeft.SetActive(false);
      this.m_twinStickArrowsRight = Object.Instantiate<GameObject>(this.TouchpadIndicationArrowPrefab_HorizontalOnly);
      this.m_twinStickArrowsRight.SetActive(false);
      this.m_floorHelper = Object.Instantiate<GameObject>(this.FloorHelperPrefab);
      this.m_floorHelper.SetActive(false);
    }

    private void Start()
    {
      this.lastHeadPos = GM.CurrentPlayerBody.NeckJointTransform.transform.position;
      this.Mode = GM.Options.MovementOptions.CurrentMovementMode;
      this.InitArmSwinger();
    }

    public void Init(FVRSceneSettings SceneSettings)
    {
      this.m_sceneSettings = SceneSettings;
      this.CleanupFlagsForModeSwitch();
    }

    public bool ShouldFlushTouchpad(FVRViveHand hand)
    {
      if (hand.CMode == ControlMode.Index || hand.CMode == ControlMode.WMR || hand.IsInStreamlinedMode)
        return false;
      if ((this.Mode == FVRMovementManager.MovementMode.SingleTwoAxis || this.Mode == FVRMovementManager.MovementMode.JoystickTeleport) && (Object) hand == (Object) this.m_authoratativeHand)
        return true;
      if (this.Mode == FVRMovementManager.MovementMode.TwinStick)
      {
        bool flag = hand.IsThisTheRightHand;
        if (GM.Options.MovementOptions.TwinStickLeftRightState == MovementOptions.TwinStickLeftRightSetup.RightStickMove)
          flag = !flag;
        if (this.m_isLeftHandActive && !flag || this.m_isRightHandActive && flag)
          return true;
      }
      return false;
    }

    private void SetTopSpeedLastSecond(Vector3 dir)
    {
      this.m_lastDir = dir.normalized;
      this.m_lastDir += this.Head.forward * 0.01f;
      this.m_topSpeedLastSecond = Mathf.Max(dir.magnitude, this.m_topSpeedLastSecond);
    }

    public float GetTopSpeedInLastSecond() => this.m_topSpeedLastSecond;

    public Vector3 GetLastWorldDir() => this.m_lastDir;

    private void SetFrameSpeed(Vector3 dir)
    {
      if (this.dirSetThisFrame)
        return;
      this.dirSetThisFrame = true;
      this.m_dirs[this.whichDir] = dir;
      ++this.whichDir;
      if (this.whichDir <= 4)
        return;
      this.whichDir = 0;
    }

    public Vector3 GetFilteredVel()
    {
      Vector3 zero = Vector3.zero;
      for (int index = 0; index < this.m_dirs.Length; ++index)
        zero += this.m_dirs[index];
      return zero * 0.2f;
    }

    public void CycleMode()
    {
      switch (this.Mode)
      {
        case FVRMovementManager.MovementMode.Teleport:
          this.Mode = FVRMovementManager.MovementMode.Dash;
          break;
        case FVRMovementManager.MovementMode.Dash:
          this.Mode = FVRMovementManager.MovementMode.SlideToTarget;
          break;
        case FVRMovementManager.MovementMode.SingleTwoAxis:
          this.Mode = FVRMovementManager.MovementMode.Armswinger;
          break;
        case FVRMovementManager.MovementMode.SlideToTarget:
          this.Mode = FVRMovementManager.MovementMode.SingleTwoAxis;
          break;
        case FVRMovementManager.MovementMode.Armswinger:
          this.Mode = FVRMovementManager.MovementMode.JoystickTeleport;
          break;
        case FVRMovementManager.MovementMode.JoystickTeleport:
          this.Mode = FVRMovementManager.MovementMode.Teleport;
          break;
      }
      GM.Options.MovementOptions.CurrentMovementMode = this.Mode;
      GM.Options.SaveToFile();
      this.CleanupFlagsForModeSwitch();
      this.InitArmSwinger();
    }

    public void CleanupFlagsForModeSwitch()
    {
      this.m_isDashing = false;
      this.m_DashTarget = Vector3.zero;
      this.m_isSliding = false;
      this.m_SlidingTarget = Vector3.zero;
      this.m_twoAxisVelocity = Vector3.zero;
      this.m_twoAxisGrounded = false;
      this.m_groundPoint = Vector3.zero;
      this.m_sprintingEngaged = false;
      this.m_timeSinceSprintDownClick = 1f;
      this.m_curArmSwingerImpetus = 0.0f;
      this.m_tarArmSwingerImpetus = 0.0f;
      this.m_armSwingerVelocity = Vector3.zero;
      this.m_armSwingerGrounded = true;
      this.m_joyStickTeleportInProgress = false;
      this.m_joyStickTeleportCooldown = 0.25f;
      this.startedTP = false;
    }

    public void UpdateMovementWithHand(FVRViveHand hand)
    {
      switch (this.Mode)
      {
        case FVRMovementManager.MovementMode.Teleport:
          this.HandUpdateTeleport(hand);
          break;
        case FVRMovementManager.MovementMode.Dash:
          this.HandUpdateDash(hand);
          break;
        case FVRMovementManager.MovementMode.SingleTwoAxis:
          this.HandUpdateTwoAxis(hand);
          break;
        case FVRMovementManager.MovementMode.SlideToTarget:
          this.HandUpdateSliding(hand);
          break;
        case FVRMovementManager.MovementMode.Armswinger:
          this.HandUpdateArmSwinger(hand);
          break;
        case FVRMovementManager.MovementMode.JoystickTeleport:
          this.HandUpdateJoyStickTeleport(hand);
          break;
        case FVRMovementManager.MovementMode.TwinStick:
          this.HandUpdateTwinstick(hand);
          break;
      }
      this.AXButtonCheck(hand);
    }

    public void Update()
    {
      if (!this.dirSetThisFrame)
        this.SetFrameSpeed(Vector3.zero);
      this.dirSetThisFrame = false;
      this.m_topSpeedLastSecond = Mathf.Clamp(this.m_topSpeedLastSecond, 0.0f, 8f);
      this.m_topSpeedLastSecond = Mathf.Lerp(this.m_topSpeedLastSecond, 0.0f, Time.deltaTime * 5f);
      this.executeFrameTick = 0;
    }

    public void FixedUpdate()
    {
      if ((double) this.m_teleportCooldown > 0.0)
        this.m_teleportCooldown -= Time.deltaTime;
      if ((double) this.m_teleportEnergy < 1.0)
        this.m_teleportEnergy += Time.deltaTime;
      else
        this.m_teleportEnergy = 1f;
      if ((double) this.m_teleportEnergy < 0.0)
        this.m_teleportEnergy = 0.0f;
      if (!GM.CurrentSceneSettings.DoesTeleportUseCooldown)
        this.m_teleportEnergy = 1f;
      if (this.executeFrameTick >= 2)
        return;
      ++this.executeFrameTick;
      this.FU();
    }

    public void FU()
    {
      if ((double) this.m_delayGroundCheck > 0.0)
        this.m_delayGroundCheck -= Time.deltaTime;
      if ((this.Mode != FVRMovementManager.MovementMode.SingleTwoAxis || (Object) this.m_authoratativeHand == (Object) null) && this.m_touchpadArrows.activeSelf)
        this.m_touchpadArrows.SetActive(false);
      if ((this.Mode != FVRMovementManager.MovementMode.JoystickTeleport || (Object) this.m_authoratativeHand == (Object) null) && this.m_joystickTPArrows.activeSelf)
        this.m_joystickTPArrows.SetActive(false);
      if (this.Mode != FVRMovementManager.MovementMode.TwinStick)
      {
        if (this.m_twinStickArrowsLeft.activeSelf)
          this.m_twinStickArrowsLeft.SetActive(false);
        if (this.m_twinStickArrowsRight.activeSelf)
          this.m_twinStickArrowsRight.SetActive(false);
      }
      if (this.Mode == FVRMovementManager.MovementMode.JoystickTeleport)
      {
        if (!this.m_floorHelper.activeSelf)
          this.m_floorHelper.SetActive(true);
        this.m_floorHelper.transform.position = GM.CurrentPlayerBody.transform.position + Vector3.up * 0.01f;
      }
      else if (this.m_floorHelper.activeSelf)
        this.m_floorHelper.SetActive(false);
      if ((double) this.m_joyStickTeleportCooldown > 0.0)
        this.m_joyStickTeleportCooldown -= Time.deltaTime;
      bool flag = false;
      if ((Object) this.m_curGrabPoint != (Object) null)
      {
        this.UpdateGrabPointMove();
        flag = true;
      }
      else
      {
        switch (this.Mode)
        {
          case FVRMovementManager.MovementMode.Dash:
            this.UpdateModeDash();
            break;
          case FVRMovementManager.MovementMode.SingleTwoAxis:
            this.UpdateModeTwoAxis(false);
            break;
          case FVRMovementManager.MovementMode.SlideToTarget:
            this.UpdateModeSliding();
            break;
          case FVRMovementManager.MovementMode.Armswinger:
            this.UpdateModeArmSwinger();
            break;
          case FVRMovementManager.MovementMode.TwinStick:
            this.UpdateModeTwoAxis(true);
            break;
        }
      }
      this.correctionDir = Vector3.zero;
      this.Body.UpdatePlayerBodyPositions();
      if (flag)
      {
        Vector3 vector3 = GM.CurrentPlayerBody.NeckJointTransform.transform.position - this.lastHeadPos;
        if (Physics.SphereCast(this.lastHeadPos, 0.15f, vector3.normalized, out RaycastHit _, vector3.magnitude, (int) this.LM_TeleCast))
        {
          this.correctionDir = -vector3;
          this.transform.position = this.transform.position + this.correctionDir;
        }
      }
      this.lastHeadPos = GM.CurrentPlayerBody.NeckJointTransform.transform.position;
    }

    private void DelayGround(float f) => this.m_delayGroundCheck = f;

    public void Blast(Vector3 dir, float vel)
    {
      bool flag1 = true;
      if (this.Mode == FVRMovementManager.MovementMode.Armswinger || this.Mode == FVRMovementManager.MovementMode.SingleTwoAxis || this.Mode == FVRMovementManager.MovementMode.TwinStick)
        flag1 = false;
      if (flag1)
        return;
      bool flag2 = false;
      float num = 0.4f;
      if (this.Mode == FVRMovementManager.MovementMode.Armswinger && this.m_armSwingerGrounded)
        flag2 = true;
      if ((this.Mode == FVRMovementManager.MovementMode.SingleTwoAxis || this.Mode == FVRMovementManager.MovementMode.TwinStick) && this.m_twoAxisGrounded)
        flag2 = true;
      if (flag2)
        num = 1f;
      float f = 0.1f;
      if (!flag2 || (double) dir.y <= 0.0)
        ;
      Vector3 vector3 = dir * vel * num;
      if (this.Mode == FVRMovementManager.MovementMode.Armswinger)
      {
        this.DelayGround(f);
        this.m_armSwingerGrounded = false;
        this.m_armSwingerVelocity += vector3;
      }
      else
      {
        if (this.Mode != FVRMovementManager.MovementMode.SingleTwoAxis && this.Mode != FVRMovementManager.MovementMode.TwinStick)
          return;
        this.DelayGround(f);
        this.m_twoAxisGrounded = false;
        this.m_twoAxisVelocity += vector3;
      }
    }

    public void RocketJump(Vector3 pos, Vector2 range, float vel)
    {
      bool flag1 = true;
      if (this.Mode == FVRMovementManager.MovementMode.Armswinger || this.Mode == FVRMovementManager.MovementMode.SingleTwoAxis || this.Mode == FVRMovementManager.MovementMode.TwinStick)
        flag1 = false;
      if (flag1)
        return;
      float num1 = Vector3.Distance(pos, GM.CurrentPlayerBody.Head.position);
      if ((double) num1 > (double) range.y)
        return;
      float num2 = 1f - Mathf.InverseLerp(range.x, range.y, num1);
      Vector3 vector3_1 = GM.CurrentPlayerBody.Head.position - pos;
      vector3_1.Normalize();
      float num3 = 5f;
      switch (GM.Options.SimulationOptions.PlayerGravityMode)
      {
        case SimulationOptions.GravityMode.Realistic:
          num3 = 6f;
          break;
        case SimulationOptions.GravityMode.Playful:
          num3 = 5f;
          break;
        case SimulationOptions.GravityMode.OnTheMoon:
          num3 = 2.3f;
          break;
        case SimulationOptions.GravityMode.None:
          num3 = 1.62f;
          break;
      }
      Vector3 zero = Vector3.zero;
      bool flag2 = false;
      if (this.Mode == FVRMovementManager.MovementMode.Armswinger && this.m_armSwingerGrounded)
        flag2 = true;
      if ((this.Mode == FVRMovementManager.MovementMode.SingleTwoAxis || this.Mode == FVRMovementManager.MovementMode.TwinStick) && this.m_twoAxisGrounded)
        flag2 = true;
      if (!flag2)
        num2 = 1f;
      if ((double) vector3_1.y > 0.0)
        zero += Vector3.up * num3 * 2.5f * num2;
      if (!flag2)
      {
        Vector3 vector3_2 = vector3_1;
        vector3_2.y = 0.0f;
        zero += vector3_2 * vel * num2;
      }
      if (this.Mode == FVRMovementManager.MovementMode.Armswinger)
      {
        this.DelayGround(0.1f);
        this.m_armSwingerVelocity.y = Mathf.Clamp(this.m_armSwingerVelocity.y, 0.0f, this.m_armSwingerVelocity.y);
        this.m_armSwingerGrounded = false;
        this.m_armSwingerVelocity += zero;
      }
      else
      {
        if (this.Mode != FVRMovementManager.MovementMode.SingleTwoAxis && this.Mode != FVRMovementManager.MovementMode.TwinStick)
          return;
        this.DelayGround(0.1f);
        this.m_twoAxisVelocity.y = Mathf.Clamp(this.m_twoAxisVelocity.y, 0.0f, this.m_twoAxisVelocity.y);
        this.m_twoAxisGrounded = false;
        this.m_twoAxisVelocity += zero;
      }
    }

    private void Jump()
    {
      if (this.Mode == FVRMovementManager.MovementMode.Armswinger && !this.m_armSwingerGrounded || (this.Mode == FVRMovementManager.MovementMode.SingleTwoAxis || this.Mode == FVRMovementManager.MovementMode.TwinStick) && !this.m_twoAxisGrounded)
        return;
      this.DelayGround(0.1f);
      float num1 = 0.0f;
      switch (GM.Options.SimulationOptions.PlayerGravityMode)
      {
        case SimulationOptions.GravityMode.Realistic:
          num1 = 7.1f;
          break;
        case SimulationOptions.GravityMode.Playful:
          num1 = 5f;
          break;
        case SimulationOptions.GravityMode.OnTheMoon:
          num1 = 3f;
          break;
        case SimulationOptions.GravityMode.None:
          num1 = 1f / 1000f;
          break;
      }
      float num2 = num1 * 0.65f;
      if (this.Mode == FVRMovementManager.MovementMode.Armswinger)
      {
        this.DelayGround(0.25f);
        this.m_armSwingerVelocity.y = Mathf.Clamp(this.m_armSwingerVelocity.y, 0.0f, this.m_armSwingerVelocity.y);
        this.m_armSwingerVelocity.y = num2;
        this.m_armSwingerGrounded = false;
      }
      else
      {
        if (this.Mode != FVRMovementManager.MovementMode.SingleTwoAxis && this.Mode != FVRMovementManager.MovementMode.TwinStick)
          return;
        this.DelayGround(0.25f);
        this.m_twoAxisVelocity.y = Mathf.Clamp(this.m_twoAxisVelocity.y, 0.0f, this.m_twoAxisVelocity.y);
        this.m_twoAxisVelocity.y = num2;
        this.m_twoAxisGrounded = false;
      }
    }

    private void AXButtonCheck(FVRViveHand hand)
    {
      if (hand.IsInStreamlinedMode)
        return;
      if (GM.Options.MovementOptions.AXButtonSnapTurnState == MovementOptions.AXButtonSnapTurnMode.Snapturn)
      {
        bool thisTheRightHand = hand.IsThisTheRightHand;
        if (!hand.Input.AXButtonDown)
          return;
        if (thisTheRightHand)
          this.TurnClockWise();
        else
          this.TurnCounterClockWise();
      }
      else
      {
        if (GM.Options.MovementOptions.AXButtonSnapTurnState != MovementOptions.AXButtonSnapTurnMode.Jump || !hand.Input.AXButtonDown)
          return;
        this.Jump();
      }
    }

    public void TurnClockWise()
    {
      Vector3 position = GM.CurrentPlayerBody.NeckJointTransform.position;
      position.y = GM.CurrentPlayerBody.transform.position.y;
      Vector3 forward = GM.CurrentPlayerBody.transform.forward;
      Vector3 lookDir = Quaternion.AngleAxis(GM.Options.MovementOptions.SnapTurnMagnitudes[GM.Options.MovementOptions.SnapTurnMagnitudeIndex], Vector3.up) * forward;
      double point = (double) this.TeleportToPoint(position, false, lookDir);
    }

    public void TurnCounterClockWise()
    {
      Vector3 position = GM.CurrentPlayerBody.NeckJointTransform.position;
      position.y = GM.CurrentPlayerBody.transform.position.y;
      Vector3 forward = GM.CurrentPlayerBody.transform.forward;
      Vector3 lookDir = Quaternion.AngleAxis(-GM.Options.MovementOptions.SnapTurnMagnitudes[GM.Options.MovementOptions.SnapTurnMagnitudeIndex], Vector3.up) * forward;
      double point = (double) this.TeleportToPoint(position, false, lookDir);
    }

    private void HandUpdateTeleport(FVRViveHand hand)
    {
      if (!hand.IsInStreamlinedMode && (hand.CMode == ControlMode.Index || hand.CMode == ControlMode.WMR))
      {
        if (hand.Input.Secondary2AxisWestDown)
          this.TurnCounterClockWise();
        else if (hand.Input.Secondary2AxisEastDown)
          this.TurnClockWise();
      }
      if (hand.IsInStreamlinedMode)
      {
        bool flag1 = false;
        bool flag2 = false;
        bool flag3 = false;
        if (hand.CMode == ControlMode.Index || hand.CMode == ControlMode.WMR)
        {
          if (hand.Input.Secondary2AxisWestDown)
            flag1 = true;
          else if (hand.Input.Secondary2AxisEastDown)
            flag2 = true;
          else if (hand.Input.Secondary2AxisSouthDown)
            flag3 = true;
        }
        else if (hand.Input.TouchpadWestDown)
          flag1 = true;
        else if (hand.Input.TouchpadEastDown)
          flag2 = true;
        else if (hand.Input.TouchpadSouthDown)
          flag3 = true;
        if (flag1)
        {
          this.m_hasValidPoint = false;
          this.TurnCounterClockWise();
          this.ClearInProgressTeleportAction();
        }
        else if (flag2)
        {
          this.m_hasValidPoint = false;
          this.TurnClockWise();
          this.ClearInProgressTeleportAction();
        }
        else if (flag3)
        {
          this.m_hasValidPoint = false;
          this.ClearInProgressTeleportAction();
        }
      }
      bool flag4;
      bool flag5;
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
      Vector3 vector3_1 = Vector3.zero;
      if (flag4 && (double) this.m_teleportCooldown <= 0.0)
      {
        this.startedTP = true;
        if (GM.Options.MovementOptions.Teleport_Mode == FVRMovementManager.TeleportMode.Standard)
        {
          if (GM.Options.MovementOptions.Teleport_AxialOrigin == FVRMovementManager.MovementAxialOrigin.Hands)
          {
            this.m_validPoint = this.FindValidPointCurved(hand.Input.FilteredPos, hand.PointingTransform.forward, 0.5f);
            vector3_1 = hand.PointingTransform.forward;
          }
          else
          {
            Vector3 castOrigin = GM.CurrentPlayerBody.Torso.position + GM.CurrentPlayerBody.Torso.right * 0.2f;
            Vector3 castDir = GM.CurrentPlayerBody.Head.forward + Vector3.up * 0.2f;
            this.m_validPoint = this.FindValidPointCurved(castOrigin, castDir, 0.5f);
            vector3_1 = castDir;
          }
          if (!this.m_hasValidPoint)
          {
            if (!this.MovementRig.gameObject.activeSelf)
              return;
            this.MovementRig.gameObject.SetActive(false);
          }
          else
          {
            if (!this.MovementRig.gameObject.activeSelf)
              this.MovementRig.gameObject.SetActive(true);
            Vector3 forward = this.Head.transform.forward;
            forward.y = 0.0f;
            this.MovementRig.transform.position = this.m_validPoint;
            this.MovementRig.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            if (!this.MovementRig.CornerHolder.gameObject.activeSelf)
              this.MovementRig.CornerHolder.gameObject.SetActive(true);
            this.MovementRig.CornerHolder.rotation = hand.WholeRig.rotation;
            Vector3 vector3_2 = this.transform.position - this.Head.position;
            vector3_2.y = 0.0f;
            this.MovementRig.CornerHolder.position = this.m_validPoint + vector3_2;
          }
        }
        else
        {
          if (GM.Options.MovementOptions.Teleport_Mode != FVRMovementManager.TeleportMode.FrontFacingMode)
            return;
          if (!this.m_isInRotatePickMode)
          {
            this.m_validPoint = this.FindValidPointCurved(hand.Input.FilteredPos, hand.PointingTransform.forward, 0.5f);
            vector3_1 = hand.PointingTransform.forward;
            if (!this.m_hasValidPoint)
            {
              if (!this.MovementRig.gameObject.activeSelf)
                return;
              this.MovementRig.gameObject.SetActive(false);
            }
            else
            {
              if (!this.MovementRig.gameObject.activeSelf)
                this.MovementRig.gameObject.SetActive(true);
              Vector3 forward = hand.WholeRig.forward;
              forward.y = 0.0f;
              this.MovementRig.transform.position = this.m_validPoint;
              this.MovementRig.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
              if (!this.MovementRig.CornerHolder.gameObject.activeSelf)
                return;
              this.MovementRig.CornerHolder.gameObject.SetActive(false);
            }
          }
          else
          {
            this.m_validRotateDir = this.FindValidPointCurvedForRotatedTeleport(hand.Input.FilteredPos, hand.PointingTransform.forward, 0.5f);
            vector3_1 = hand.PointingTransform.forward;
            if (!this.m_hasValidRotateDir)
            {
              this.m_validRotateDir = hand.WholeRig.forward;
            }
            else
            {
              if (!this.MovementRig.gameObject.activeSelf)
                this.MovementRig.gameObject.SetActive(true);
              this.MovementRig.transform.rotation = Quaternion.LookRotation(this.m_validRotateDir, Vector3.up);
              if (!this.MovementRig.CornerHolder.gameObject.activeSelf)
                return;
              this.MovementRig.CornerHolder.gameObject.SetActive(false);
            }
          }
        }
      }
      else
      {
        if (!this.startedTP || !flag5)
          return;
        this.startedTP = false;
        if (GM.Options.MovementOptions.Teleport_Mode == FVRMovementManager.TeleportMode.Standard)
        {
          if (this.MovementRig.gameObject.activeSelf)
            this.MovementRig.gameObject.SetActive(false);
          if (this.m_hasValidPoint)
            this.m_teleportEnergy -= this.TeleportToPoint(this.m_validPoint, false);
          for (int index = 0; index < 20; ++index)
          {
            if (this.Cylinders[index].gameObject.activeSelf)
              this.Cylinders[index].gameObject.SetActive(false);
          }
        }
        else
        {
          if (GM.Options.MovementOptions.Teleport_Mode != FVRMovementManager.TeleportMode.FrontFacingMode)
            return;
          if (!this.m_isInRotatePickMode)
          {
            if (this.m_hasValidPoint)
              this.m_isInRotatePickMode = true;
            else if (this.MovementRig.gameObject.activeSelf)
              this.MovementRig.gameObject.SetActive(false);
          }
          else
          {
            this.m_isInRotatePickMode = false;
            if (this.m_hasValidRotateDir && this.m_hasValidPoint)
            {
              this.m_teleportEnergy -= this.TeleportToPoint(this.m_validPoint, false, this.m_validRotateDir);
              if (this.MovementRig.gameObject.activeSelf)
                this.MovementRig.gameObject.SetActive(false);
            }
            else
            {
              if (this.MovementRig.gameObject.activeSelf)
                this.MovementRig.gameObject.SetActive(false);
              this.m_hasValidRotateDir = false;
              this.m_hasValidPoint = false;
            }
          }
          for (int index = 0; index < 20; ++index)
          {
            if (this.Cylinders[index].gameObject.activeSelf)
              this.Cylinders[index].gameObject.SetActive(false);
          }
        }
      }
    }

    public void ClearInProgressTeleportAction()
    {
      this.MovementRig.gameObject.SetActive(false);
      this.m_hasValidPoint = false;
      this.startedTP = false;
      for (int index = 0; index < 20; ++index)
      {
        if (this.Cylinders[index].gameObject.activeSelf)
          this.Cylinders[index].gameObject.SetActive(false);
      }
    }

    private void HandUpdateDash(FVRViveHand hand)
    {
      if (!hand.IsInStreamlinedMode && (hand.CMode == ControlMode.Index || hand.CMode == ControlMode.WMR))
      {
        if (hand.Input.Secondary2AxisWestDown)
          this.TurnCounterClockWise();
        else if (hand.Input.Secondary2AxisEastDown)
          this.TurnClockWise();
      }
      if (hand.IsInStreamlinedMode)
      {
        bool flag1 = false;
        bool flag2 = false;
        bool flag3 = false;
        if (hand.CMode == ControlMode.Index || hand.CMode == ControlMode.WMR)
        {
          if (hand.Input.Secondary2AxisWestDown)
            flag1 = true;
          else if (hand.Input.Secondary2AxisEastDown)
            flag2 = true;
          else if (hand.Input.Secondary2AxisSouthDown)
            flag3 = true;
        }
        else if (hand.Input.TouchpadWestDown)
          flag1 = true;
        else if (hand.Input.TouchpadEastDown)
          flag2 = true;
        else if (hand.Input.TouchpadSouthDown)
          flag3 = true;
        if (flag1)
        {
          this.m_hasValidPoint = false;
          this.TurnCounterClockWise();
          this.ClearInProgressTeleportAction();
        }
        else if (flag2)
        {
          this.m_hasValidPoint = false;
          this.TurnClockWise();
          this.ClearInProgressTeleportAction();
        }
        else if (flag3)
        {
          this.m_hasValidPoint = false;
          this.ClearInProgressTeleportAction();
        }
      }
      bool flag4;
      bool flag5;
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
      if (flag4 && (double) this.m_teleportCooldown <= 0.0)
      {
        this.startedTP = true;
        this.m_validPoint = GM.Options.MovementOptions.Dash_AxialOrigin != FVRMovementManager.MovementAxialOrigin.Hands ? this.FindValidPointCurved(GM.CurrentPlayerBody.Torso.position + GM.CurrentPlayerBody.Torso.right * 0.2f, GM.CurrentPlayerBody.Head.forward + Vector3.up * 0.2f, 0.5f) : this.FindValidPointCurved(hand.Input.FilteredPos, hand.PointingTransform.forward, 0.5f);
        if (!this.m_hasValidPoint)
        {
          if (!this.MovementRig.gameObject.activeSelf)
            return;
          this.MovementRig.gameObject.SetActive(false);
        }
        else
        {
          if (!this.MovementRig.gameObject.activeSelf)
            this.MovementRig.gameObject.SetActive(true);
          Vector3 forward = this.Head.transform.forward;
          forward.y = 0.0f;
          this.MovementRig.transform.position = this.m_validPoint;
          this.MovementRig.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
          this.MovementRig.CornerHolder.rotation = hand.WholeRig.rotation;
          Vector3 vector3 = this.transform.position - this.Head.position;
          vector3.y = 0.0f;
          this.MovementRig.CornerHolder.position = this.m_validPoint + vector3;
        }
      }
      else
      {
        if (!this.startedTP || !flag5)
          return;
        this.startedTP = false;
        if (this.MovementRig.gameObject.activeSelf)
          this.MovementRig.gameObject.SetActive(false);
        if (this.m_hasValidPoint)
          this.m_teleportEnergy -= this.SetDashDestination(this.m_validPoint);
        for (int index = 0; index < 20; ++index)
        {
          if (this.Cylinders[index].gameObject.activeSelf)
            this.Cylinders[index].gameObject.SetActive(false);
        }
      }
    }

    private void ClearTPRig()
    {
      if (this.MovementRig.gameObject.activeSelf)
        this.MovementRig.gameObject.SetActive(false);
      for (int index = 0; index < 20; ++index)
      {
        if (this.Cylinders[index].gameObject.activeSelf)
          this.Cylinders[index].gameObject.SetActive(false);
      }
    }

    private void UpdateModeDash()
    {
      if (!this.m_isDashing)
        return;
      Vector3 position = this.transform.position;
      this.transform.position = Vector3.MoveTowards(this.transform.position, this.m_DashTarget, this.DashSpeed * Time.deltaTime);
      Vector3 dir = this.transform.position - position;
      this.Body.MoveQuickbeltContents(dir);
      this.SetTopSpeedLastSecond(dir);
      if ((double) Vector3.Distance(this.transform.position, this.m_DashTarget) >= 0.00999999977648258)
        return;
      this.m_isDashing = false;
    }

    private void HandUpdateSliding(FVRViveHand hand)
    {
      if (hand.Input.BYButtonPressed)
      {
        this.m_validPoint = GM.Options.MovementOptions.Slide_AxialOrigin != FVRMovementManager.MovementAxialOrigin.Hands ? this.FindValidPointCurved(GM.CurrentPlayerBody.Torso.position + GM.CurrentPlayerBody.Torso.right * 0.2f, GM.CurrentPlayerBody.Head.forward + Vector3.up * 0.2f, 0.5f) : this.FindValidPointCurved(hand.Input.FilteredPos, hand.PointingTransform.forward, 0.5f);
        if (!this.m_hasValidPoint)
        {
          if (!this.MovementRig.gameObject.activeSelf)
            return;
          this.MovementRig.gameObject.SetActive(false);
        }
        else
        {
          if (!this.MovementRig.gameObject.activeSelf)
            this.MovementRig.gameObject.SetActive(true);
          Vector3 forward = this.Head.transform.forward;
          forward.y = 0.0f;
          this.MovementRig.transform.position = this.m_validPoint;
          this.MovementRig.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
          this.MovementRig.CornerHolder.rotation = hand.WholeRig.rotation;
          Vector3 vector3 = this.transform.position - this.Head.position;
          vector3.y = 0.0f;
          this.MovementRig.CornerHolder.position = this.m_validPoint + vector3;
        }
      }
      else
      {
        if (!hand.Input.BYButtonUp)
          return;
        if (this.MovementRig.gameObject.activeSelf)
          this.MovementRig.gameObject.SetActive(false);
        if (this.m_hasValidPoint)
          this.SetSlidingDestination(this.m_validPoint);
        for (int index = 0; index < 20; ++index)
        {
          if (this.Cylinders[index].gameObject.activeSelf)
            this.Cylinders[index].gameObject.SetActive(false);
        }
      }
    }

    private void UpdateModeSliding()
    {
      if (!this.m_isSliding)
        return;
      Vector3 position = this.transform.position;
      this.SlidingSpeed = GM.Options.MovementOptions.SlidingSpeeds[GM.Options.MovementOptions.SlidingSpeedTick];
      this.transform.position = Vector3.MoveTowards(this.transform.position, this.m_SlidingTarget, this.SlidingSpeed * Time.deltaTime);
      Vector3 dir = this.transform.position - position;
      this.Body.MoveQuickbeltContents(dir);
      this.SetTopSpeedLastSecond(dir);
      this.SetFrameSpeed(dir);
      if ((double) Vector3.Distance(this.transform.position, this.m_SlidingTarget) >= 0.00999999977648258)
        return;
      this.m_isSliding = false;
    }

    private void HandUpdateTwoAxis(FVRViveHand hand)
    {
      if (hand.Input.BYButtonDown)
      {
        if ((Object) hand != (Object) this.m_authoratativeHand)
          this.m_authoratativeHand = hand;
        else if ((Object) hand == (Object) this.m_authoratativeHand)
          this.m_authoratativeHand = (FVRViveHand) null;
      }
      if ((Object) this.m_authoratativeHand == (Object) null)
      {
        this.m_isTwinStickSmoothTurningClockwise = false;
        this.m_isTwinStickSmoothTurningCounterClockwise = false;
      }
      if ((Object) this.m_authoratativeHand == (Object) null || (Object) hand != (Object) this.m_authoratativeHand)
        return;
      if (!this.m_touchpadArrows.activeSelf)
        this.m_touchpadArrows.SetActive(true);
      if ((Object) this.m_touchpadArrows.transform.parent != (Object) this.m_authoratativeHand.TouchpadArrowTarget)
      {
        this.m_touchpadArrows.transform.parent = this.m_authoratativeHand.TouchpadArrowTarget;
        this.m_touchpadArrows.transform.localPosition = Vector3.zero;
        this.m_touchpadArrows.transform.localRotation = Quaternion.identity;
      }
      if ((double) this.m_timeSinceSprintDownClick < 2.0)
        this.m_timeSinceSprintDownClick += Time.deltaTime;
      if ((double) this.m_timeSinceSnapTurn < 2.0)
        this.m_timeSinceSnapTurn += Time.deltaTime;
      if (hand.Input.TouchpadDown && (double) this.m_timeSinceSprintDownClick < 0.300000011920929)
        this.m_sprintingEngaged = true;
      if (hand.Input.TouchpadTouchUp)
        this.m_sprintingEngaged = false;
      Vector3 vector3_1 = Vector3.zero;
      float y = hand.Input.TouchpadAxes.y;
      float num = hand.Input.TouchpadAxes.x;
      if (GM.Options.MovementOptions.Touchpad_Style == FVRMovementManager.TwoAxisMovementStyle.LateralIsSnapTurn)
      {
        num = 0.0f;
        if ((double) Mathf.Abs(hand.Input.TouchpadAxes.x) > 0.699999988079071 && (double) this.m_timeSinceSnapTurn > 0.300000011920929 && (GM.Options.MovementOptions.Touchpad_Confirm == FVRMovementManager.TwoAxisMovementConfirm.OnClick && hand.Input.TouchpadDown || GM.Options.MovementOptions.Touchpad_Confirm == FVRMovementManager.TwoAxisMovementConfirm.OnTouch && hand.Input.TouchpadTouched))
        {
          this.m_timeSinceSnapTurn = 0.0f;
          Vector3 position = GM.CurrentPlayerBody.Head.position;
          position.y = this.transform.position.y;
          Vector3 forward = this.transform.forward;
          float snapTurnMagnitude = GM.Options.MovementOptions.SnapTurnMagnitudes[GM.Options.MovementOptions.SnapTurnMagnitudeIndex];
          Vector3 lookDir = (double) hand.Input.TouchpadAxes.x <= 0.0 ? Quaternion.AngleAxis(-snapTurnMagnitude, Vector3.up) * forward : Quaternion.AngleAxis(snapTurnMagnitude, Vector3.up) * forward;
          double point = (double) this.TeleportToPoint(position, false, lookDir);
        }
      }
      else if (GM.Options.MovementOptions.Touchpad_Style == FVRMovementManager.TwoAxisMovementStyle.LateralIsSmoothTurn)
      {
        num = 0.0f;
        this.m_isTwinStickSmoothTurningClockwise = false;
        this.m_isTwinStickSmoothTurningCounterClockwise = false;
        if (hand.CMode == ControlMode.Oculus)
        {
          if (hand.Input.TouchpadWestPressed)
            this.m_isTwinStickSmoothTurningCounterClockwise = true;
          else if (hand.Input.TouchpadEastPressed)
            this.m_isTwinStickSmoothTurningClockwise = true;
        }
        else if (GM.Options.MovementOptions.Touchpad_Confirm == FVRMovementManager.TwoAxisMovementConfirm.OnClick)
        {
          if (hand.Input.TouchpadPressed)
          {
            if (hand.Input.TouchpadWestPressed)
              this.m_isTwinStickSmoothTurningCounterClockwise = true;
            else if (hand.Input.TouchpadEastPressed)
              this.m_isTwinStickSmoothTurningClockwise = true;
          }
        }
        else if (hand.Input.TouchpadWestPressed)
          this.m_isTwinStickSmoothTurningCounterClockwise = true;
        else if (hand.Input.TouchpadEastPressed)
          this.m_isTwinStickSmoothTurningClockwise = true;
      }
      switch (GM.Options.MovementOptions.Touchpad_MovementMode)
      {
        case FVRMovementManager.TwoAxisMovementMode.Standard:
          vector3_1 = y * hand.PointingTransform.forward + num * hand.PointingTransform.right * 0.75f;
          vector3_1.y = 0.0f;
          break;
        case FVRMovementManager.TwoAxisMovementMode.Onward:
          vector3_1 = y * hand.transform.forward + num * hand.transform.right * 0.75f;
          break;
        case FVRMovementManager.TwoAxisMovementMode.LeveledHand:
          Vector3 forward1 = hand.transform.forward;
          forward1.y = 0.0f;
          forward1.Normalize();
          Vector3 right1 = hand.transform.right;
          right1.y = 0.0f;
          right1.Normalize();
          vector3_1 = y * forward1 + num * right1 * 0.75f;
          break;
        case FVRMovementManager.TwoAxisMovementMode.LeveledHead:
          Vector3 forward2 = GM.CurrentPlayerBody.Head.forward;
          forward2.y = 0.0f;
          forward2.Normalize();
          Vector3 right2 = GM.CurrentPlayerBody.Head.right;
          right2.y = 0.0f;
          right2.Normalize();
          vector3_1 = y * forward2 + num * right2 * 0.75f;
          break;
      }
      Vector3 normalized = vector3_1.normalized;
      Vector3 vector3_2 = vector3_1 * GM.Options.MovementOptions.TPLocoSpeeds[GM.Options.MovementOptions.TPLocoSpeedIndex];
      if (GM.Options.MovementOptions.Touchpad_Confirm == FVRMovementManager.TwoAxisMovementConfirm.OnClick)
      {
        if (!hand.Input.TouchpadPressed)
          vector3_2 = Vector3.zero;
        else if (this.m_sprintingEngaged && GM.Options.MovementOptions.TPLocoSpeedIndex < 5)
          vector3_2 += normalized * 2f;
      }
      else if (this.m_sprintingEngaged && GM.Options.MovementOptions.TPLocoSpeedIndex < 5)
        vector3_2 += normalized * 2f;
      if (this.m_twoAxisGrounded)
      {
        this.m_twoAxisVelocity.x = vector3_2.x;
        this.m_twoAxisVelocity.z = vector3_2.z;
        if (GM.CurrentSceneSettings.UsesMaxSpeedClamp)
        {
          Vector2 vector2 = new Vector2(this.m_twoAxisVelocity.x, this.m_twoAxisVelocity.z);
          if ((double) vector2.magnitude > (double) GM.CurrentSceneSettings.MaxSpeedClamp)
          {
            vector2 = vector2.normalized * GM.CurrentSceneSettings.MaxSpeedClamp;
            this.m_twoAxisVelocity.x = vector2.x;
            this.m_twoAxisVelocity.z = vector2.y;
          }
        }
      }
      else if (GM.CurrentSceneSettings.DoesAllowAirControl)
      {
        Vector3 vector3_3 = new Vector3(this.m_twoAxisVelocity.x, 0.0f, this.m_twoAxisVelocity.z);
        this.m_twoAxisVelocity.x += vector3_2.x * Time.deltaTime;
        this.m_twoAxisVelocity.z += vector3_2.z * Time.deltaTime;
        Vector3 vector3_4 = Vector3.ClampMagnitude(new Vector3(this.m_twoAxisVelocity.x, 0.0f, this.m_twoAxisVelocity.z), Mathf.Max(1f, vector3_3.magnitude));
        this.m_twoAxisVelocity.x = vector3_4.x;
        this.m_twoAxisVelocity.z = vector3_4.z;
      }
      else
      {
        Vector3 vector3_3 = new Vector3(this.m_twoAxisVelocity.x, 0.0f, this.m_twoAxisVelocity.z);
        this.m_twoAxisVelocity.x += (float) ((double) vector3_2.x * (double) Time.deltaTime * 0.300000011920929);
        this.m_twoAxisVelocity.z += (float) ((double) vector3_2.z * (double) Time.deltaTime * 0.300000011920929);
        Vector3 vector3_4 = Vector3.ClampMagnitude(new Vector3(this.m_twoAxisVelocity.x, 0.0f, this.m_twoAxisVelocity.z), Mathf.Max(1f, vector3_3.magnitude));
        this.m_twoAxisVelocity.x = vector3_4.x;
        this.m_twoAxisVelocity.z = vector3_4.z;
      }
      if (!hand.Input.TouchpadDown)
        return;
      this.m_timeSinceSprintDownClick = 0.0f;
    }

    private void UpdateModeTwoAxis(bool IsTwinstick)
    {
      this.CurNeckPos = GM.CurrentPlayerBody.NeckJointTransform.position;
      Vector3 vector3_1 = this.LastNeckPos - this.CurNeckPos;
      Vector3 lastNeckPos = this.LastNeckPos;
      Vector3 vector3_2 = this.CurNeckPos - this.LastNeckPos;
      RaycastHit hitInfo1;
      if (Physics.SphereCast(this.LastNeckPos, 0.15f, vector3_2.normalized, out hitInfo1, vector3_2.magnitude, (int) this.LM_TeleCast))
        this.correctionDir = -vector3_2 * 1f;
      if (IsTwinstick)
      {
        if (!this.m_isLeftHandActive && this.m_twoAxisGrounded)
        {
          this.m_twoAxisVelocity.x = 0.0f;
          this.m_twoAxisVelocity.z = 0.0f;
        }
      }
      else if ((Object) this.m_authoratativeHand == (Object) null && this.m_twoAxisGrounded)
      {
        this.m_twoAxisVelocity.x = 0.0f;
        this.m_twoAxisVelocity.z = 0.0f;
      }
      Vector3 vector3_3 = lastNeckPos;
      Vector3 b1 = vector3_3;
      vector3_3.y = Mathf.Max(vector3_3.y, this.transform.position.y + this.m_armSwingerStepHeight);
      b1.y = this.transform.position.y;
      float maxDistance1 = Vector3.Distance(vector3_3, b1);
      if ((double) this.m_delayGroundCheck > 0.0)
        maxDistance1 *= 0.5f;
      bool flag1 = false;
      Vector3 planeNormal = Vector3.up;
      bool flag2 = false;
      Vector3 vector3_4 = Vector3.up;
      Vector3 vector3_5 = vector3_3 + -Vector3.up * maxDistance1;
      Vector3 vector3_6 = vector3_3 + -Vector3.up * maxDistance1;
      float a1 = 90f;
      float a2 = -1000f;
      if (Physics.SphereCast(vector3_3, 0.2f, -Vector3.up, out this.m_hit_ray, maxDistance1, (int) this.LM_TeleCast))
      {
        vector3_4 = this.m_hit_ray.normal;
        vector3_5 = this.m_hit_ray.point;
        vector3_6 = this.m_hit_ray.point;
        a1 = Vector3.Angle(Vector3.up, this.m_hit_ray.normal);
        a2 = vector3_5.y;
        flag2 = true;
      }
      if (Physics.Raycast(vector3_3, -Vector3.up, out this.m_hit_ray, maxDistance1, (int) this.LM_TeleCast))
      {
        a1 = Mathf.Min(a1, Vector3.Angle(Vector3.up, this.m_hit_ray.normal));
        vector3_4 = this.m_hit_ray.normal;
        vector3_5.y = Mathf.Max(vector3_5.y, this.m_hit_ray.point.y);
        vector3_6.y = Mathf.Min(vector3_5.y, this.m_hit_ray.point.y);
        a2 = Mathf.Max(a2, this.m_hit_ray.point.y);
        flag2 = true;
      }
      Vector3 vector1 = this.Head.forward;
      vector1.y = 0.0f;
      vector1.Normalize();
      vector1 = Vector3.ClampMagnitude(vector1, 0.1f);
      Vector3 vector2 = this.Head.right;
      vector2.y = 0.0f;
      vector2.Normalize();
      vector2 = Vector3.ClampMagnitude(vector2, 0.1f);
      Vector3 vector3_7 = -vector1;
      Vector3 vector3_8 = -vector2;
      if (Physics.Raycast(vector3_3 + vector1, -Vector3.up, out this.m_hit_ray, maxDistance1, (int) this.LM_TeleCast))
      {
        a1 = Mathf.Min(a1, Vector3.Angle(Vector3.up, this.m_hit_ray.normal));
        vector3_5.y = Mathf.Max(vector3_5.y, this.m_hit_ray.point.y);
        vector3_6.y = Mathf.Min(vector3_5.y, this.m_hit_ray.point.y);
        a2 = Mathf.Max(a2, this.m_hit_ray.point.y);
        if (!flag2)
        {
          vector3_4 = this.m_hit_ray.normal;
          flag2 = true;
        }
      }
      if (Physics.Raycast(vector3_3 + vector2, -Vector3.up, out this.m_hit_ray, maxDistance1, (int) this.LM_TeleCast))
      {
        a1 = Mathf.Min(a1, Vector3.Angle(Vector3.up, this.m_hit_ray.normal));
        vector3_5.y = Mathf.Max(vector3_5.y, this.m_hit_ray.point.y);
        vector3_6.y = Mathf.Min(vector3_5.y, this.m_hit_ray.point.y);
        a2 = Mathf.Max(a2, this.m_hit_ray.point.y);
        if (!flag2)
        {
          vector3_4 = this.m_hit_ray.normal;
          flag2 = true;
        }
      }
      if (Physics.Raycast(vector3_3 + vector3_7, -Vector3.up, out this.m_hit_ray, maxDistance1, (int) this.LM_TeleCast))
      {
        a1 = Mathf.Min(a1, Vector3.Angle(Vector3.up, this.m_hit_ray.normal));
        vector3_5.y = Mathf.Max(vector3_5.y, this.m_hit_ray.point.y);
        vector3_6.y = Mathf.Min(vector3_5.y, this.m_hit_ray.point.y);
        a2 = Mathf.Max(a2, this.m_hit_ray.point.y);
        if (!flag2)
        {
          vector3_4 = this.m_hit_ray.normal;
          flag2 = true;
        }
      }
      if (Physics.Raycast(vector3_3 + vector3_8, -Vector3.up, out this.m_hit_ray, maxDistance1, (int) this.LM_TeleCast))
      {
        a1 = Mathf.Min(a1, Vector3.Angle(Vector3.up, this.m_hit_ray.normal));
        vector3_5.y = Mathf.Max(vector3_5.y, this.m_hit_ray.point.y);
        vector3_6.y = Mathf.Min(vector3_5.y, this.m_hit_ray.point.y);
        Mathf.Max(a2, this.m_hit_ray.point.y);
        if (!flag2)
        {
          vector3_4 = this.m_hit_ray.normal;
          flag2 = true;
        }
      }
      if (flag2)
      {
        if ((double) a1 > 70.0)
        {
          flag1 = true;
          this.m_twoAxisGrounded = false;
          planeNormal = vector3_4;
          this.m_groundPoint = vector3_6;
        }
        else
        {
          this.m_twoAxisGrounded = true;
          this.m_groundPoint = vector3_5;
        }
      }
      else
      {
        this.m_twoAxisGrounded = false;
        this.m_groundPoint = vector3_3 - Vector3.up * maxDistance1;
      }
      Vector3 vector3_9 = lastNeckPos;
      Vector3 b2 = vector3_9;
      b2.y = this.transform.position.y + 2.15f * GM.CurrentPlayerBody.transform.localScale.y;
      float maxDistance2 = Vector3.Distance(vector3_9, b2);
      float num1 = vector3_9.y + 0.15f;
      if (Physics.SphereCast(vector3_9, 0.15f, Vector3.up, out this.m_hit_ray, maxDistance2, (int) this.LM_TeleCast))
      {
        Vector3 point = this.m_hit_ray.point;
        Vector3.Distance(vector3_9, new Vector3(vector3_9.x, point.y, vector3_9.z));
        num1 = this.m_hit_ray.point.y - 0.15f;
        float num2 = Mathf.Clamp(GM.CurrentPlayerBody.Head.localPosition.y, 0.3f, 2.5f);
        float y = this.m_groundPoint.y;
        float min = y - (num2 - 0.2f);
        this.m_groundPoint.y = Mathf.Clamp((float) ((double) num1 - (double) num2 - 0.150000005960464), min, y);
      }
      if (this.m_twoAxisGrounded)
      {
        this.m_twoAxisVelocity.y = 0.0f;
      }
      else
      {
        float num2 = 5f;
        switch (GM.Options.SimulationOptions.PlayerGravityMode)
        {
          case SimulationOptions.GravityMode.Realistic:
            num2 = 9.81f;
            break;
          case SimulationOptions.GravityMode.Playful:
            num2 = 5f;
            break;
          case SimulationOptions.GravityMode.OnTheMoon:
            num2 = 1.62f;
            break;
          case SimulationOptions.GravityMode.None:
            num2 = 1f / 1000f;
            break;
        }
        if (!flag1)
        {
          this.m_twoAxisVelocity.y -= num2 * Time.deltaTime;
        }
        else
        {
          this.m_twoAxisVelocity += Vector3.ProjectOnPlane(-Vector3.up * num2, planeNormal) * Time.deltaTime;
          this.m_twoAxisVelocity = Vector3.ProjectOnPlane(this.m_twoAxisVelocity, planeNormal);
        }
      }
      Mathf.Abs(lastNeckPos.y - GM.CurrentPlayerBody.transform.position.y);
      Vector3 point1 = lastNeckPos;
      Vector3 point2 = lastNeckPos;
      point1.y = Mathf.Min(point1.y, num1 - 0.01f);
      point2.y = Mathf.Max(this.transform.position.y, this.m_groundPoint.y) + (this.m_armSwingerStepHeight + 0.2f);
      point1.y = Mathf.Max(point1.y, point2.y);
      Vector3 direction = this.m_twoAxisVelocity;
      float maxLength = this.m_twoAxisVelocity.magnitude * Time.deltaTime;
      if (Physics.CapsuleCast(point1, point2, 0.15f, this.m_twoAxisVelocity, out this.m_hit_ray, (float) ((double) this.m_twoAxisVelocity.magnitude * (double) Time.deltaTime + 0.100000001490116), (int) this.LM_TeleCast))
      {
        direction = Vector3.ProjectOnPlane(this.m_twoAxisVelocity, this.m_hit_ray.normal);
        maxLength = this.m_hit_ray.distance * 0.5f;
        if (this.m_twoAxisGrounded)
          direction.y = 0.0f;
        RaycastHit hitInfo2;
        if (Physics.CapsuleCast(point1, point2, 0.15f, direction, out hitInfo2, (float) ((double) direction.magnitude * (double) Time.deltaTime + 0.100000001490116), (int) this.LM_TeleCast))
          maxLength = hitInfo2.distance * 0.5f;
      }
      this.m_twoAxisVelocity = direction;
      if (this.m_twoAxisGrounded)
        this.m_twoAxisVelocity.y = 0.0f;
      Vector3 position = this.transform.position;
      Vector3 vector3_10 = Vector3.ClampMagnitude(this.m_twoAxisVelocity * Time.deltaTime, maxLength);
      Vector3 vector3_11 = this.transform.position + vector3_10;
      if (this.m_twoAxisGrounded)
        vector3_11.y = Mathf.MoveTowards(vector3_11.y, this.m_groundPoint.y, 8f * Time.deltaTime * Mathf.Abs(this.transform.position.y - this.m_groundPoint.y));
      Vector3 vector3_12 = this.CurNeckPos + vector3_10 - this.LastNeckPos;
      if (Physics.SphereCast(this.LastNeckPos, 0.15f, vector3_12.normalized, out hitInfo1, vector3_12.magnitude, (int) this.LM_TeleCast))
        this.correctionDir = -vector3_12 * 1f;
      if (GM.Options.MovementOptions.AXButtonSnapTurnState == MovementOptions.AXButtonSnapTurnMode.Smoothturn)
      {
        for (int index = 0; index < this.Hands.Length; ++index)
        {
          if (!this.Hands[index].IsInStreamlinedMode)
          {
            if (this.Hands[index].IsThisTheRightHand)
            {
              if (this.Hands[index].Input.AXButtonPressed)
                this.m_isTwinStickSmoothTurningClockwise = true;
            }
            else if (this.Hands[index].Input.AXButtonPressed)
              this.m_isTwinStickSmoothTurningCounterClockwise = true;
          }
        }
      }
      if (!this.m_isTwinStickSmoothTurningClockwise && !this.m_isTwinStickSmoothTurningCounterClockwise)
      {
        this.transform.position = vector3_11 + this.correctionDir;
      }
      else
      {
        Vector3 point = vector3_11 + this.correctionDir;
        Vector3 forward = GM.CurrentPlayerBody.transform.forward;
        float num2 = GM.Options.MovementOptions.SmoothTurnMagnitudes[GM.Options.MovementOptions.SmoothTurnMagnitudeIndex] * Time.deltaTime;
        if (this.m_isTwinStickSmoothTurningCounterClockwise)
          num2 = -num2;
        this.transform.SetPositionAndRotation(this.RotatePointAroundPivotWithEuler(point, this.CurNeckPos, new Vector3(0.0f, num2, 0.0f)), Quaternion.LookRotation(Quaternion.AngleAxis(num2, Vector3.up) * forward, Vector3.up));
      }
      this.SetTopSpeedLastSecond(this.m_twoAxisVelocity);
      this.SetFrameSpeed(this.m_twoAxisVelocity);
      this.LastNeckPos = GM.CurrentPlayerBody.NeckJointTransform.position;
    }

    private void HandUpdateArmSwinger(FVRViveHand hand)
    {
      if (GM.Options.MovementOptions.ArmSwingerSnapTurnState != MovementOptions.ArmSwingerSnapTurnMode.Enabled)
        return;
      if (hand.CMode == ControlMode.Index || hand.CMode == ControlMode.WMR)
      {
        if (hand.Input.Secondary2AxisWestDown)
          this.TurnCounterClockWise();
        else if (hand.Input.Secondary2AxisEastDown)
          this.TurnClockWise();
      }
      if (!hand.IsInStreamlinedMode)
        return;
      if (hand.Input.TouchpadWestDown)
      {
        this.m_hasValidPoint = false;
        this.TurnCounterClockWise();
      }
      else
      {
        if (!hand.Input.TouchpadEastDown)
          return;
        this.m_hasValidPoint = false;
        this.TurnClockWise();
      }
    }

    public void InitArmSwinger()
    {
      this.CurNeckPos = GM.CurrentPlayerBody.NeckJointTransform.position;
      this.LastNeckPos = GM.CurrentPlayerBody.NeckJointTransform.position;
    }

    private void UpdateModeArmSwinger()
    {
      bool flag1 = false;
      bool flag2 = false;
      if (GM.Options.MovementOptions.ArmSwingerSnapTurnState == MovementOptions.ArmSwingerSnapTurnMode.Smooth)
      {
        for (int index = 0; index < this.Hands.Length; ++index)
        {
          if (this.Hands[index].CMode == ControlMode.Index || this.Hands[index].CMode == ControlMode.WMR)
          {
            if (this.Hands[index].Input.Secondary2AxisWestPressed)
              flag2 = true;
            else if (this.Hands[index].Input.Secondary2AxisEastPressed)
              flag1 = true;
          }
          else if (this.Hands[index].IsInStreamlinedMode)
          {
            if (this.Hands[index].Input.TouchpadWestDown)
              flag2 = true;
            else if (this.Hands[index].Input.TouchpadEastDown)
              flag1 = true;
          }
        }
        if (flag1 && flag2)
        {
          flag1 = false;
          flag2 = false;
        }
      }
      this.CurNeckPos = GM.CurrentPlayerBody.NeckJointTransform.position;
      Vector3 vector3_1 = this.LastNeckPos - this.CurNeckPos;
      Vector3 lastNeckPos = this.LastNeckPos;
      Vector3 vector3_2 = this.CurNeckPos - this.LastNeckPos;
      RaycastHit hitInfo1;
      if (Physics.SphereCast(this.LastNeckPos, 0.15f, vector3_2.normalized, out hitInfo1, vector3_2.magnitude, (int) this.LM_TeleCast))
        this.correctionDir = -vector3_2 * 1f;
      float num1 = 0.0f;
      bool flag3 = false;
      bool flag4 = false;
      for (int index = 0; index < this.Hands.Length; ++index)
      {
        if (!this.Hands[index].IsInStreamlinedMode ? this.Hands[index].Input.BYButtonPressed : (this.Hands[index].CMode == ControlMode.Index || this.Hands[index].CMode == ControlMode.WMR ? this.Hands[index].Input.Secondary2AxisNorthPressed : this.Hands[index].Input.TouchpadNorthPressed))
        {
          float magnitude = this.Hands[index].Input.VelLinearWorld.magnitude;
          float min = !this.Hands[index].IsThisTheRightHand ? GM.Options.MovementOptions.ArmSwingerBaseSpeeMagnitudes[GM.Options.MovementOptions.ArmSwingerBaseSpeed_Left] : GM.Options.MovementOptions.ArmSwingerBaseSpeeMagnitudes[GM.Options.MovementOptions.ArmSwingerBaseSpeed_Right];
          float num2 = Mathf.Clamp(magnitude, min, magnitude);
          num1 += num2;
          switch (index)
          {
            case 0:
              flag3 = true;
              continue;
            case 1:
              flag4 = true;
              continue;
            default:
              continue;
          }
        }
      }
      this.m_tarArmSwingerImpetus = num1;
      this.m_curArmSwingerImpetus = Mathf.MoveTowards(this.m_curArmSwingerImpetus, this.m_tarArmSwingerImpetus, 4f * Time.deltaTime);
      if (this.m_armSwingerGrounded)
      {
        Vector3 vector3_3 = Vector3.zero;
        if (flag3)
          vector3_3 += this.Hands[0].PointingTransform.forward;
        if (flag4)
          vector3_3 += this.Hands[1].PointingTransform.forward;
        if (flag3 && flag4)
          vector3_3 *= 0.5f;
        vector3_3.y = 0.0f;
        vector3_3.Normalize();
        vector3_3 = vector3_3 * this.m_curArmSwingerImpetus * 1.5f;
        this.m_armSwingerVelocity.x = vector3_3.x;
        this.m_armSwingerVelocity.z = vector3_3.z;
      }
      else if (GM.CurrentSceneSettings.DoesAllowAirControl)
      {
        Vector3 vector3_3 = Vector3.zero;
        if (flag3)
          vector3_3 += this.Hands[0].PointingTransform.forward;
        if (flag4)
          vector3_3 += this.Hands[1].PointingTransform.forward;
        if (flag3 && flag4)
          vector3_3 *= 0.5f;
        vector3_3.y = 0.0f;
        vector3_3.Normalize();
        vector3_3 = vector3_3 * this.m_curArmSwingerImpetus * 1.5f;
        Vector3 vector3_4 = new Vector3(this.m_armSwingerVelocity.x, 0.0f, this.m_armSwingerVelocity.z);
        this.m_armSwingerVelocity.x += vector3_3.x * Time.deltaTime;
        this.m_armSwingerVelocity.z += vector3_3.z * Time.deltaTime;
        Vector3 vector3_5 = Vector3.ClampMagnitude(new Vector3(this.m_armSwingerVelocity.x, 0.0f, this.m_armSwingerVelocity.z), Mathf.Max(1f, vector3_4.magnitude));
        this.m_armSwingerVelocity.x = vector3_5.x;
        this.m_armSwingerVelocity.z = vector3_5.z;
      }
      else
      {
        Vector3 vector3_3 = Vector3.zero;
        if (flag3)
          vector3_3 += this.Hands[0].PointingTransform.forward;
        if (flag4)
          vector3_3 += this.Hands[1].PointingTransform.forward;
        if (flag3 && flag4)
          vector3_3 *= 0.5f;
        vector3_3.y = 0.0f;
        vector3_3.Normalize();
        vector3_3 = vector3_3 * this.m_curArmSwingerImpetus * 0.3f;
        Vector3 vector3_4 = new Vector3(this.m_armSwingerVelocity.x, 0.0f, this.m_armSwingerVelocity.z);
        this.m_armSwingerVelocity.x += vector3_3.x * Time.deltaTime;
        this.m_armSwingerVelocity.z += vector3_3.z * Time.deltaTime;
        Vector3 vector3_5 = Vector3.ClampMagnitude(new Vector3(this.m_armSwingerVelocity.x, 0.0f, this.m_armSwingerVelocity.z), Mathf.Max(1f, vector3_4.magnitude));
        this.m_armSwingerVelocity.x = vector3_5.x;
        this.m_armSwingerVelocity.z = vector3_5.z;
      }
      Vector3 vector3_6 = lastNeckPos;
      Vector3 b1 = vector3_6;
      vector3_6.y = Mathf.Max(vector3_6.y, this.transform.position.y + this.m_armSwingerStepHeight);
      b1.y = this.transform.position.y;
      float maxDistance1 = Vector3.Distance(vector3_6, b1);
      if ((double) this.m_delayGroundCheck > 0.0)
        maxDistance1 *= 0.5f;
      bool flag5 = false;
      Vector3 planeNormal = Vector3.up;
      bool flag6 = false;
      Vector3 vector3_7 = Vector3.up;
      Vector3 vector3_8 = vector3_6 + -Vector3.up * maxDistance1;
      Vector3 vector3_9 = vector3_6 + -Vector3.up * maxDistance1;
      float a1 = 90f;
      float a2 = -1000f;
      if (Physics.SphereCast(vector3_6, 0.2f, -Vector3.up, out this.m_hit_ray, maxDistance1, (int) this.LM_TeleCast))
      {
        vector3_7 = this.m_hit_ray.normal;
        vector3_8 = this.m_hit_ray.point;
        vector3_9 = this.m_hit_ray.point;
        a2 = vector3_8.y;
        flag6 = true;
        a1 = !Physics.Raycast(new Vector3(this.m_hit_ray.point.x, vector3_6.y, this.m_hit_ray.point.z), -Vector3.up, out this.m_hit_ray, maxDistance1, (int) this.LM_TeleCast) ? 45f : Vector3.Angle(Vector3.up, this.m_hit_ray.normal);
      }
      if (Physics.Raycast(vector3_6, -Vector3.up, out this.m_hit_ray, maxDistance1, (int) this.LM_TeleCast))
      {
        a1 = Mathf.Min(a1, Vector3.Angle(Vector3.up, this.m_hit_ray.normal));
        vector3_7 = this.m_hit_ray.normal;
        vector3_8.y = Mathf.Max(vector3_8.y, this.m_hit_ray.point.y);
        vector3_9.y = Mathf.Min(vector3_8.y, this.m_hit_ray.point.y);
        a2 = Mathf.Max(a2, this.m_hit_ray.point.y);
        flag6 = true;
      }
      Vector3 forward1 = this.Head.forward;
      forward1.y = 0.0f;
      forward1.Normalize();
      Vector3 vector3_10 = Vector3.ClampMagnitude(forward1, 0.1f);
      Vector3 vector = this.Head.right;
      vector.y = 0.0f;
      vector.Normalize();
      vector = Vector3.ClampMagnitude(vector, 0.1f);
      Vector3 vector3_11 = -vector3_10;
      Vector3 vector3_12 = -vector;
      if (Physics.Raycast(vector3_6 + vector3_10, -Vector3.up, out this.m_hit_ray, maxDistance1, (int) this.LM_TeleCast))
      {
        a1 = Mathf.Min(a1, Vector3.Angle(Vector3.up, this.m_hit_ray.normal));
        vector3_8.y = Mathf.Max(vector3_8.y, this.m_hit_ray.point.y);
        vector3_9.y = Mathf.Min(vector3_8.y, this.m_hit_ray.point.y);
        a2 = Mathf.Max(a2, this.m_hit_ray.point.y);
        if (!flag6)
        {
          vector3_7 = this.m_hit_ray.normal;
          flag6 = true;
        }
      }
      if (Physics.Raycast(vector3_6 + vector, -Vector3.up, out this.m_hit_ray, maxDistance1, (int) this.LM_TeleCast))
      {
        a1 = Mathf.Min(a1, Vector3.Angle(Vector3.up, this.m_hit_ray.normal));
        vector3_8.y = Mathf.Max(vector3_8.y, this.m_hit_ray.point.y);
        vector3_9.y = Mathf.Min(vector3_8.y, this.m_hit_ray.point.y);
        a2 = Mathf.Max(a2, this.m_hit_ray.point.y);
        if (!flag6)
        {
          vector3_7 = this.m_hit_ray.normal;
          flag6 = true;
        }
      }
      if (Physics.Raycast(vector3_6 + vector3_11, -Vector3.up, out this.m_hit_ray, maxDistance1, (int) this.LM_TeleCast))
      {
        a1 = Mathf.Min(a1, Vector3.Angle(Vector3.up, this.m_hit_ray.normal));
        vector3_8.y = Mathf.Max(vector3_8.y, this.m_hit_ray.point.y);
        vector3_9.y = Mathf.Min(vector3_8.y, this.m_hit_ray.point.y);
        a2 = Mathf.Max(a2, this.m_hit_ray.point.y);
        if (!flag6)
        {
          vector3_7 = this.m_hit_ray.normal;
          flag6 = true;
        }
      }
      if (Physics.Raycast(vector3_6 + vector3_12, -Vector3.up, out this.m_hit_ray, maxDistance1, (int) this.LM_TeleCast))
      {
        a1 = Mathf.Min(a1, Vector3.Angle(Vector3.up, this.m_hit_ray.normal));
        vector3_8.y = Mathf.Max(vector3_8.y, this.m_hit_ray.point.y);
        vector3_9.y = Mathf.Min(vector3_8.y, this.m_hit_ray.point.y);
        Mathf.Max(a2, this.m_hit_ray.point.y);
        if (!flag6)
        {
          vector3_7 = this.m_hit_ray.normal;
          flag6 = true;
        }
      }
      if (flag6)
      {
        if ((double) a1 > 70.0)
        {
          flag5 = true;
          this.m_armSwingerGrounded = false;
          planeNormal = vector3_7;
          this.m_groundPoint = vector3_9;
        }
        else
        {
          this.m_armSwingerGrounded = true;
          this.m_groundPoint = vector3_8;
        }
      }
      else
      {
        this.m_armSwingerGrounded = false;
        this.m_groundPoint = vector3_6 - Vector3.up * maxDistance1;
      }
      Vector3 vector3_13 = lastNeckPos;
      Vector3 b2 = vector3_13;
      b2.y = this.transform.position.y + 2.15f * GM.CurrentPlayerBody.transform.localScale.y;
      float maxDistance2 = Vector3.Distance(vector3_13, b2);
      float num3 = vector3_13.y + 0.15f;
      if (Physics.SphereCast(vector3_13, 0.15f, Vector3.up, out this.m_hit_ray, maxDistance2, (int) this.LM_TeleCast))
      {
        Vector3 point = this.m_hit_ray.point;
        Vector3.Distance(vector3_13, new Vector3(vector3_13.x, point.y, vector3_13.z));
        num3 = this.m_hit_ray.point.y - 0.15f;
        float num2 = Mathf.Clamp(GM.CurrentPlayerBody.Head.localPosition.y, 0.3f, 2.5f);
        float y = this.m_groundPoint.y;
        float min = y - (num2 - 0.2f);
        this.m_groundPoint.y = Mathf.Clamp((float) ((double) num3 - (double) num2 - 0.150000005960464), min, y);
      }
      if (this.m_armSwingerGrounded)
      {
        this.m_armSwingerVelocity.y = 0.0f;
      }
      else
      {
        float num2 = 5f;
        switch (GM.Options.SimulationOptions.PlayerGravityMode)
        {
          case SimulationOptions.GravityMode.Realistic:
            num2 = 9.81f;
            break;
          case SimulationOptions.GravityMode.Playful:
            num2 = 5f;
            break;
          case SimulationOptions.GravityMode.OnTheMoon:
            num2 = 1.62f;
            break;
          case SimulationOptions.GravityMode.None:
            num2 = 1f / 1000f;
            break;
        }
        if (!flag5)
        {
          this.m_armSwingerVelocity.y -= num2 * Time.deltaTime;
        }
        else
        {
          this.m_armSwingerVelocity += Vector3.ProjectOnPlane(-Vector3.up * num2, planeNormal) * Time.deltaTime;
          this.m_armSwingerVelocity = Vector3.ProjectOnPlane(this.m_armSwingerVelocity, planeNormal);
        }
      }
      Mathf.Abs(lastNeckPos.y - GM.CurrentPlayerBody.transform.position.y);
      Vector3 point1 = lastNeckPos;
      Vector3 point2 = lastNeckPos;
      point1.y = Mathf.Min(point1.y, num3 - 0.01f);
      point2.y = Mathf.Max(this.transform.position.y, this.m_groundPoint.y) + (this.m_armSwingerStepHeight + 0.2f);
      point1.y = Mathf.Max(point1.y, point2.y);
      Vector3 direction = this.m_armSwingerVelocity;
      float maxLength = this.m_armSwingerVelocity.magnitude * Time.deltaTime;
      if (Physics.CapsuleCast(point1, point2, 0.15f, this.m_armSwingerVelocity, out this.m_hit_ray, (float) ((double) this.m_armSwingerVelocity.magnitude * (double) Time.deltaTime + 0.100000001490116), (int) this.LM_TeleCast))
      {
        direction = Vector3.ProjectOnPlane(this.m_armSwingerVelocity, this.m_hit_ray.normal);
        maxLength = this.m_hit_ray.distance * 0.5f;
        if (this.m_armSwingerGrounded)
          direction.y = 0.0f;
        RaycastHit hitInfo2;
        if (Physics.CapsuleCast(point1, point2, 0.15f, direction, out hitInfo2, (float) ((double) direction.magnitude * (double) Time.deltaTime + 0.100000001490116), (int) this.LM_TeleCast))
          maxLength = hitInfo2.distance * 0.5f;
      }
      this.m_armSwingerVelocity = direction;
      if (this.m_armSwingerGrounded)
        this.m_armSwingerVelocity.y = 0.0f;
      Vector3 position = this.transform.position;
      Vector3 vector3_14 = Vector3.ClampMagnitude(this.m_armSwingerVelocity * Time.deltaTime, maxLength);
      Vector3 vector3_15 = this.transform.position + vector3_14;
      if (this.m_armSwingerGrounded)
        vector3_15.y = Mathf.MoveTowards(vector3_15.y, this.m_groundPoint.y, 8f * Time.deltaTime * Mathf.Abs(this.transform.position.y - this.m_groundPoint.y));
      Vector3 vector3_16 = this.CurNeckPos + vector3_14 - this.LastNeckPos;
      if (Physics.SphereCast(this.LastNeckPos, 0.15f, vector3_16.normalized, out hitInfo1, vector3_16.magnitude, (int) this.LM_TeleCast))
        this.correctionDir = -vector3_16 * 1f;
      if (GM.Options.MovementOptions.AXButtonSnapTurnState == MovementOptions.AXButtonSnapTurnMode.Smoothturn)
      {
        for (int index = 0; index < this.Hands.Length; ++index)
        {
          if (!this.Hands[index].IsInStreamlinedMode)
          {
            if (this.Hands[index].IsThisTheRightHand)
            {
              if (this.Hands[index].Input.AXButtonPressed)
                flag1 = true;
            }
            else if (this.Hands[index].Input.AXButtonPressed)
              flag2 = true;
          }
        }
      }
      if (!flag1 && !flag2)
      {
        this.transform.position = vector3_15 + this.correctionDir;
      }
      else
      {
        Vector3 point = vector3_15 + this.correctionDir;
        Vector3 forward2 = GM.CurrentPlayerBody.transform.forward;
        float num2 = GM.Options.MovementOptions.SmoothTurnMagnitudes[GM.Options.MovementOptions.SmoothTurnMagnitudeIndex] * Time.deltaTime;
        if (flag2)
          num2 = -num2;
        this.transform.SetPositionAndRotation(this.RotatePointAroundPivotWithEuler(point, this.CurNeckPos, new Vector3(0.0f, num2, 0.0f)), Quaternion.LookRotation(Quaternion.AngleAxis(num2, Vector3.up) * forward2, Vector3.up));
      }
      if (GM.Options.MovementOptions.ArmSwingerJumpState == MovementOptions.ArmSwingerJumpMode.Enabled && (double) this.Hands[0].transform.position.y > (double) this.Head.position.y && ((double) this.Hands[1].transform.position.y > (double) this.Head.position.y && (double) this.Hands[0].Input.VelLinearWorld.y > 2.0 && (double) this.Hands[1].Input.VelLinearWorld.y > 2.0))
        this.Jump();
      this.SetTopSpeedLastSecond(this.m_armSwingerVelocity);
      this.SetFrameSpeed(this.m_armSwingerVelocity);
      this.LastNeckPos = GM.CurrentPlayerBody.NeckJointTransform.position;
    }

    public void BeginGrabPointMove(FVRHandGrabPoint grabPoint)
    {
      this.CleanupFlagsForModeSwitch();
      this.m_curGrabPoint = grabPoint;
      this.m_lastHandPos = this.m_curGrabPoint.m_hand.Input.Pos;
    }

    public void EndGrabPointMove(FVRHandGrabPoint grabPoint)
    {
      if ((Object) this.m_curGrabPoint == (Object) grabPoint)
        this.m_curGrabPoint = (FVRHandGrabPoint) null;
      this.m_twoAxisVelocity = -grabPoint.m_hand.Input.VelLinearWorld;
      this.m_armSwingerVelocity = -grabPoint.m_hand.Input.VelLinearWorld;
      this.m_twoAxisGrounded = false;
      this.m_armSwingerGrounded = false;
      this.InitArmSwinger();
    }

    public void UpdateGrabPointMove()
    {
      Vector3 position = this.transform.position;
      this.transform.position = this.transform.position + (this.m_lastHandPos - this.m_curGrabPoint.m_hand.Input.Pos);
      this.m_lastHandPos = this.m_curGrabPoint.m_hand.Input.Pos;
    }

    private void HandUpdateJoyStickTeleport(FVRViveHand hand)
    {
      if (hand.Input.BYButtonDown && !this.m_joyStickTeleportInProgress)
      {
        if ((Object) hand != (Object) this.m_authoratativeHand)
          this.m_authoratativeHand = hand;
        else if ((Object) hand == (Object) this.m_authoratativeHand)
          this.m_authoratativeHand = (FVRViveHand) null;
      }
      if ((double) this.m_timeSinceSnapTurn < 2.0)
        this.m_timeSinceSnapTurn += Time.deltaTime;
      if ((Object) this.m_authoratativeHand == (Object) null || (Object) hand != (Object) this.m_authoratativeHand)
        return;
      if (!this.m_joystickTPArrows.activeSelf)
        this.m_joystickTPArrows.SetActive(true);
      if ((Object) this.m_joystickTPArrows.transform.parent != (Object) this.m_authoratativeHand.TouchpadArrowTarget)
      {
        this.m_joystickTPArrows.transform.parent = this.m_authoratativeHand.TouchpadArrowTarget;
        this.m_joystickTPArrows.transform.localPosition = Vector3.zero;
        this.m_joystickTPArrows.transform.localRotation = Quaternion.identity;
      }
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      bool flag4 = false;
      Vector2 vector2;
      bool flag5;
      bool flag6;
      if (hand.CMode == ControlMode.Vive || hand.CMode == ControlMode.Oculus || hand.CMode == ControlMode.WMR)
      {
        flag1 = hand.Input.TouchpadUp;
        flag2 = hand.Input.TouchpadDown;
        flag3 = hand.Input.TouchpadPressed;
        vector2 = hand.Input.TouchpadAxes;
        flag4 = hand.Input.TouchpadCenterDown;
        flag5 = hand.Input.TouchpadTouchUp;
        flag6 = hand.Input.TouchpadTouched;
      }
      else
      {
        flag1 = hand.Input.Secondary2AxisInputUp;
        flag2 = hand.Input.Secondary2AxisInputDown;
        flag3 = hand.Input.Secondary2AxisInputPressed;
        vector2 = hand.Input.Secondary2AxisInputAxes;
        flag4 = hand.Input.Secondary2AxisCenterDown;
        flag5 = hand.Input.Secondary2AxisInputTouchUp;
        flag6 = hand.Input.Secondary2AxisInputTouched;
      }
      if ((double) Mathf.Abs(vector2.x) < 0.300000011920929)
        this.hasXAxisReset = true;
      if (flag5 && this.m_joyStickTeleportInProgress)
      {
        this.m_joyStickTeleportCooldown = 0.25f;
        this.m_joyStickTeleportInProgress = false;
        if (this.MovementRig.gameObject.activeSelf)
          this.MovementRig.gameObject.SetActive(false);
        if (this.m_hasValidPoint)
          this.m_teleportEnergy -= this.TeleportToPoint(this.m_validPoint, true, this.m_worldPointDir);
        for (int index = 0; index < 20; ++index)
        {
          if (this.Cylinders[index].gameObject.activeSelf)
            this.Cylinders[index].gameObject.SetActive(false);
        }
      }
      else if (flag6 && (double) this.m_joyStickTeleportCooldown <= 0.0 && ((double) vector2.magnitude > 0.800000011920929 && this.m_joyStickTeleportInProgress || (double) Mathf.Abs(vector2.y) > 0.800000011920929))
      {
        this.m_joyStickTeleportInProgress = true;
        Vector3 vector3 = Vector3.zero;
        this.m_validPoint = this.FindValidPointCurved(hand.Input.FilteredPos, hand.PointingTransform.forward, 0.5f);
        vector3 = hand.PointingTransform.forward;
        if (!this.m_hasValidPoint)
        {
          if (!this.MovementRig.gameObject.activeSelf)
            return;
          this.MovementRig.gameObject.SetActive(false);
        }
        else
        {
          if (!this.MovementRig.gameObject.activeSelf)
            this.MovementRig.gameObject.SetActive(true);
          this.Head.transform.forward.y = 0.0f;
          this.MovementRig.transform.position = this.m_validPoint;
          Vector3 forward = hand.transform.forward * vector2.y + hand.transform.right * vector2.x;
          forward.y = 0.0f;
          forward.Normalize();
          this.m_worldPointDir = forward;
          this.MovementRig.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
          if (!this.MovementRig.CornerHolder.gameObject.activeSelf)
            return;
          this.MovementRig.CornerHolder.gameObject.SetActive(false);
        }
      }
      else
      {
        if (this.m_joyStickTeleportInProgress || (double) Mathf.Abs(vector2.x) <= 0.800000011920929 || (!this.hasXAxisReset || !flag6))
          return;
        this.hasXAxisReset = false;
        Vector3 position = GM.CurrentPlayerBody.Head.position;
        position.y = this.transform.position.y;
        Vector3 forward = this.transform.forward;
        float snapTurnMagnitude = GM.Options.MovementOptions.SnapTurnMagnitudes[GM.Options.MovementOptions.SnapTurnMagnitudeIndex];
        Vector3 lookDir = (double) vector2.x <= 0.0 ? Quaternion.AngleAxis(-snapTurnMagnitude, Vector3.up) * forward : Quaternion.AngleAxis(snapTurnMagnitude, Vector3.up) * forward;
        this.m_teleportEnergy -= this.TeleportToPoint(position, false, lookDir);
        this.m_teleportCooldown = 0.2f;
      }
    }

    private void HandUpdateTwinstick(FVRViveHand hand)
    {
      bool flag1 = hand.IsThisTheRightHand;
      if (GM.Options.MovementOptions.TwinStickLeftRightState == MovementOptions.TwinStickLeftRightSetup.RightStickMove)
        flag1 = !flag1;
      if (!hand.IsInStreamlinedMode && (hand.CMode == ControlMode.Vive || hand.CMode == ControlMode.Oculus))
      {
        if (hand.Input.BYButtonDown)
        {
          if (flag1)
            this.m_isRightHandActive = !this.m_isRightHandActive;
          if (!flag1)
            this.m_isLeftHandActive = !this.m_isLeftHandActive;
        }
      }
      else
      {
        this.m_isLeftHandActive = true;
        this.m_isRightHandActive = true;
      }
      if (flag1 && !this.m_isRightHandActive)
      {
        if (this.m_twinStickArrowsRight.activeSelf)
          this.m_twinStickArrowsRight.SetActive(false);
        this.m_isTwinStickSmoothTurningCounterClockwise = false;
        this.m_isTwinStickSmoothTurningClockwise = false;
      }
      else if (!flag1 && !this.m_isLeftHandActive)
      {
        if (!this.m_twinStickArrowsLeft.activeSelf)
          return;
        this.m_twinStickArrowsLeft.SetActive(false);
      }
      else
      {
        if (!hand.IsInStreamlinedMode && (hand.CMode == ControlMode.Vive || hand.CMode == ControlMode.Oculus))
        {
          if (flag1)
          {
            if (!this.m_twinStickArrowsRight.activeSelf)
              this.m_twinStickArrowsRight.SetActive(true);
            if ((Object) this.m_twinStickArrowsRight.transform.parent != (Object) hand.TouchpadArrowTarget)
            {
              this.m_twinStickArrowsRight.transform.SetParent(hand.TouchpadArrowTarget);
              this.m_twinStickArrowsRight.transform.localPosition = Vector3.zero;
              this.m_twinStickArrowsRight.transform.localRotation = Quaternion.identity;
            }
          }
          else
          {
            if (!this.m_twinStickArrowsLeft.activeSelf)
              this.m_twinStickArrowsLeft.SetActive(true);
            if ((Object) this.m_twinStickArrowsLeft.transform.parent != (Object) hand.TouchpadArrowTarget)
            {
              this.m_twinStickArrowsLeft.transform.SetParent(hand.TouchpadArrowTarget);
              this.m_twinStickArrowsLeft.transform.localPosition = Vector3.zero;
              this.m_twinStickArrowsLeft.transform.localRotation = Quaternion.identity;
            }
          }
        }
        if ((double) this.m_timeSinceSprintDownClick < 2.0)
          this.m_timeSinceSprintDownClick += Time.deltaTime;
        if ((double) this.m_timeSinceSnapTurn < 2.0)
          this.m_timeSinceSnapTurn += Time.deltaTime;
        bool flag2 = false;
        bool flag3;
        bool flag4;
        Vector2 vector2_1;
        bool flag5;
        bool flag6;
        if (hand.CMode == ControlMode.Vive || hand.CMode == ControlMode.Oculus)
        {
          flag2 = hand.Input.TouchpadUp;
          flag3 = hand.Input.TouchpadDown;
          flag4 = hand.Input.TouchpadPressed;
          vector2_1 = hand.Input.TouchpadAxes;
          flag5 = hand.Input.TouchpadNorthDown;
          flag6 = hand.Input.TouchpadNorthPressed;
        }
        else
        {
          flag2 = hand.Input.Secondary2AxisInputUp;
          flag3 = hand.Input.Secondary2AxisInputDown;
          flag4 = hand.Input.Secondary2AxisInputPressed;
          vector2_1 = hand.Input.Secondary2AxisInputAxes;
          flag5 = hand.Input.Secondary2AxisNorthDown;
          flag6 = hand.Input.Secondary2AxisNorthPressed;
        }
        if (flag1)
        {
          this.m_isTwinStickSmoothTurningCounterClockwise = false;
          this.m_isTwinStickSmoothTurningClockwise = false;
          if (GM.Options.MovementOptions.TwinStickSnapturnState == MovementOptions.TwinStickSnapturnMode.Enabled)
          {
            if (hand.CMode == ControlMode.Oculus)
            {
              if (hand.Input.TouchpadWestDown)
                this.TurnCounterClockWise();
              else if (hand.Input.TouchpadEastDown)
                this.TurnClockWise();
            }
            else if (hand.CMode == ControlMode.Vive)
            {
              if (GM.Options.MovementOptions.Touchpad_Confirm == FVRMovementManager.TwoAxisMovementConfirm.OnClick)
              {
                if (hand.Input.TouchpadDown)
                {
                  if (hand.Input.TouchpadWestPressed)
                    this.TurnCounterClockWise();
                  else if (hand.Input.TouchpadEastPressed)
                    this.TurnClockWise();
                }
              }
              else if (hand.Input.TouchpadWestDown)
                this.TurnCounterClockWise();
              else if (hand.Input.TouchpadEastDown)
                this.TurnClockWise();
            }
            else if (hand.Input.Secondary2AxisWestDown)
              this.TurnCounterClockWise();
            else if (hand.Input.Secondary2AxisEastDown)
              this.TurnClockWise();
          }
          else if (GM.Options.MovementOptions.TwinStickSnapturnState == MovementOptions.TwinStickSnapturnMode.Smooth)
          {
            if (hand.CMode == ControlMode.Oculus)
            {
              if (hand.Input.TouchpadWestPressed)
                this.m_isTwinStickSmoothTurningCounterClockwise = true;
              else if (hand.Input.TouchpadEastPressed)
                this.m_isTwinStickSmoothTurningClockwise = true;
            }
            else if (hand.CMode == ControlMode.Vive)
            {
              if (GM.Options.MovementOptions.Touchpad_Confirm == FVRMovementManager.TwoAxisMovementConfirm.OnClick)
              {
                if (hand.Input.TouchpadPressed)
                {
                  if (hand.Input.TouchpadWestPressed)
                    this.m_isTwinStickSmoothTurningCounterClockwise = true;
                  else if (hand.Input.TouchpadEastPressed)
                    this.m_isTwinStickSmoothTurningClockwise = true;
                }
              }
              else if (hand.Input.TouchpadWestPressed)
                this.m_isTwinStickSmoothTurningCounterClockwise = true;
              else if (hand.Input.TouchpadEastPressed)
                this.m_isTwinStickSmoothTurningClockwise = true;
            }
            else if (hand.Input.Secondary2AxisWestPressed)
              this.m_isTwinStickSmoothTurningCounterClockwise = true;
            else if (hand.Input.Secondary2AxisEastPressed)
              this.m_isTwinStickSmoothTurningClockwise = true;
          }
          if (GM.Options.MovementOptions.TwinStickJumpState == MovementOptions.TwinStickJumpMode.Enabled)
          {
            if (hand.CMode == ControlMode.Oculus)
            {
              if (hand.Input.TouchpadSouthDown)
                this.Jump();
            }
            else if (hand.CMode == ControlMode.Vive)
            {
              if (GM.Options.MovementOptions.Touchpad_Confirm == FVRMovementManager.TwoAxisMovementConfirm.OnClick)
              {
                if (hand.Input.TouchpadDown && hand.Input.TouchpadSouthPressed)
                  this.Jump();
              }
              else if (hand.Input.TouchpadSouthDown)
                this.Jump();
            }
            else if (hand.Input.Secondary2AxisSouthDown)
              this.Jump();
          }
          if (GM.Options.MovementOptions.TwinStickSprintState != MovementOptions.TwinStickSprintMode.RightStickForward)
            return;
          if (GM.Options.MovementOptions.TwinStickSprintToggleState == MovementOptions.TwinStickSprintToggleMode.Disabled)
          {
            if (flag6)
              this.m_sprintingEngaged = true;
            else
              this.m_sprintingEngaged = false;
          }
          else
          {
            if (!flag5)
              return;
            this.m_sprintingEngaged = !this.m_sprintingEngaged;
          }
        }
        else
        {
          if (GM.Options.MovementOptions.TwinStickSprintState == MovementOptions.TwinStickSprintMode.LeftStickClick)
          {
            if (GM.Options.MovementOptions.TwinStickSprintToggleState == MovementOptions.TwinStickSprintToggleMode.Disabled)
              this.m_sprintingEngaged = flag4;
            else if (flag3)
              this.m_sprintingEngaged = !this.m_sprintingEngaged;
          }
          Vector3 vector3_1 = Vector3.zero;
          float y = vector2_1.y;
          float x = vector2_1.x;
          switch (GM.Options.MovementOptions.Touchpad_MovementMode)
          {
            case FVRMovementManager.TwoAxisMovementMode.Standard:
              vector3_1 = y * hand.PointingTransform.forward + x * hand.PointingTransform.right * 0.75f;
              vector3_1.y = 0.0f;
              break;
            case FVRMovementManager.TwoAxisMovementMode.Onward:
              vector3_1 = y * hand.transform.forward + x * hand.transform.right * 0.75f;
              break;
            case FVRMovementManager.TwoAxisMovementMode.LeveledHand:
              Vector3 forward1 = hand.transform.forward;
              forward1.y = 0.0f;
              forward1.Normalize();
              Vector3 right1 = hand.transform.right;
              right1.y = 0.0f;
              right1.Normalize();
              vector3_1 = y * forward1 + x * right1 * 0.75f;
              break;
            case FVRMovementManager.TwoAxisMovementMode.LeveledHead:
              Vector3 forward2 = GM.CurrentPlayerBody.Head.forward;
              forward2.y = 0.0f;
              forward2.Normalize();
              Vector3 right2 = GM.CurrentPlayerBody.Head.right;
              right2.y = 0.0f;
              right2.Normalize();
              vector3_1 = y * forward2 + x * right2 * 0.75f;
              break;
          }
          Vector3 normalized = vector3_1.normalized;
          vector3_1 *= GM.Options.MovementOptions.TPLocoSpeeds[GM.Options.MovementOptions.TPLocoSpeedIndex];
          if (hand.CMode == ControlMode.Vive && GM.Options.MovementOptions.Touchpad_Confirm == FVRMovementManager.TwoAxisMovementConfirm.OnClick)
          {
            if (!flag4)
              vector3_1 = Vector3.zero;
            else if (this.m_sprintingEngaged && GM.Options.MovementOptions.TPLocoSpeedIndex < 5)
              vector3_1 += normalized * 2f;
          }
          else if (this.m_sprintingEngaged && GM.Options.MovementOptions.TPLocoSpeedIndex < 5)
            vector3_1 += normalized * 2f;
          if (this.m_twoAxisGrounded)
          {
            this.m_twoAxisVelocity.x = vector3_1.x;
            this.m_twoAxisVelocity.z = vector3_1.z;
            if (GM.CurrentSceneSettings.UsesMaxSpeedClamp)
            {
              Vector2 vector2_2 = new Vector2(this.m_twoAxisVelocity.x, this.m_twoAxisVelocity.z);
              if ((double) vector2_2.magnitude > (double) GM.CurrentSceneSettings.MaxSpeedClamp)
              {
                Vector2 vector2_3 = vector2_2.normalized * GM.CurrentSceneSettings.MaxSpeedClamp;
                this.m_twoAxisVelocity.x = vector2_3.x;
                this.m_twoAxisVelocity.z = vector2_3.y;
              }
            }
          }
          else if (GM.CurrentSceneSettings.DoesAllowAirControl)
          {
            Vector3 vector3_2 = new Vector3(this.m_twoAxisVelocity.x, 0.0f, this.m_twoAxisVelocity.z);
            this.m_twoAxisVelocity.x += vector3_1.x * Time.deltaTime;
            this.m_twoAxisVelocity.z += vector3_1.z * Time.deltaTime;
            Vector3 vector3_3 = Vector3.ClampMagnitude(new Vector3(this.m_twoAxisVelocity.x, 0.0f, this.m_twoAxisVelocity.z), Mathf.Max(1f, vector3_2.magnitude));
            this.m_twoAxisVelocity.x = vector3_3.x;
            this.m_twoAxisVelocity.z = vector3_3.z;
          }
          else
          {
            Vector3 vector3_2 = new Vector3(this.m_twoAxisVelocity.x, 0.0f, this.m_twoAxisVelocity.z);
            this.m_twoAxisVelocity.x += (float) ((double) vector3_1.x * (double) Time.deltaTime * 0.300000011920929);
            this.m_twoAxisVelocity.z += (float) ((double) vector3_1.z * (double) Time.deltaTime * 0.300000011920929);
            Vector3 vector = new Vector3(this.m_twoAxisVelocity.x, 0.0f, this.m_twoAxisVelocity.z);
            float maxLength = Mathf.Max(1f, vector3_2.magnitude);
            vector = Vector3.ClampMagnitude(vector, maxLength);
            this.m_twoAxisVelocity.x = vector.x;
            this.m_twoAxisVelocity.z = vector.z;
          }
          if (!flag3)
            return;
          this.m_timeSinceSprintDownClick = 0.0f;
        }
      }
    }

    public Vector3 FindValidPointCurved(
      Vector3 castOrigin,
      Vector3 castDir,
      float initialVel)
    {
      this.m_hasValidPoint = false;
      Vector3 vector3_1 = Vector3.zero;
      Vector3 vector3_2 = castDir * initialVel * Mathf.Clamp(this.m_teleportEnergy, 0.1f, 1f);
      float num1 = 1f / GM.CurrentPlayerBody.transform.localScale.x;
      int num2 = 0;
      for (int index = 0; index < 20; ++index)
      {
        if (index == 19)
          vector3_2 = -Vector3.up * 10f + vector3_2 * 0.01f;
        if (Physics.Raycast(castOrigin, vector3_2, out this.m_hit_ray, vector3_2.magnitude, (int) this.LM_TeleCast) && !this.m_hit_ray.transform.gameObject.CompareTag("NoTeleport"))
        {
          if ((double) Vector3.Dot(this.m_hit_ray.normal, Vector3.up) >= 0.5 && !Physics.CheckCapsule(this.m_hit_ray.point + Vector3.up * 0.4f, this.m_hit_ray.point + Vector3.up * (this.Head.transform.localPosition.y - 0.2f), 0.2f, (int) this.LM_PointSearch))
          {
            this.m_hasValidPoint = true;
            vector3_1 = this.m_hit_ray.point;
          }
          else
            break;
        }
        this.Cylinders[index].gameObject.SetActive(true);
        this.Cylinders[index].position = castOrigin;
        this.Cylinders[index].rotation = Quaternion.LookRotation(vector3_2);
        this.Cylinders[index].localScale = new Vector3(0.1f, 0.1f, vector3_2.magnitude) * num1;
        if (this.m_hasValidPoint)
        {
          this.Cylinders[index].localScale = new Vector3(0.1f, 0.1f, this.m_hit_ray.distance) * num1;
          num2 = index + 1;
          break;
        }
        castOrigin += vector3_2;
        vector3_2 *= 0.95f;
        vector3_2 += -Vector3.up * 9.8f * (3f / 1000f);
      }
      for (int index = num2; index < 20; ++index)
      {
        if (this.Cylinders[index].gameObject.activeSelf)
          this.Cylinders[index].gameObject.SetActive(false);
      }
      return vector3_1;
    }

    public Vector3 FindValidPointCurvedForRotatedTeleport(
      Vector3 castOrigin,
      Vector3 castDir,
      float initialVel)
    {
      this.m_hasValidRotateDir = false;
      Vector3 vector3_1 = Vector3.zero;
      if (!this.m_hasValidPoint)
        return vector3_1;
      Vector3 vector3_2 = castDir * initialVel * Mathf.Clamp(this.m_teleportEnergy, 0.1f, 1f);
      int num = 0;
      for (int index = 0; index < 20; ++index)
      {
        if (index == 19)
          vector3_2 = -Vector3.up * 10f + vector3_2 * 0.01f;
        if (Physics.Raycast(castOrigin, vector3_2, out this.m_hit_ray, vector3_2.magnitude, (int) this.LM_TeleCast) && !this.m_hit_ray.transform.gameObject.CompareTag("NoTeleport"))
        {
          if ((double) Vector3.Dot(this.m_hit_ray.normal, Vector3.up) >= 0.5 && !Physics.CheckCapsule(this.m_hit_ray.point + Vector3.up * 0.4f, this.m_hit_ray.point + Vector3.up * (this.Head.transform.localPosition.y - 0.2f), 0.2f, (int) this.LM_PointSearch))
          {
            this.m_hasValidRotateDir = true;
            vector3_1 = this.m_hit_ray.point;
          }
          else
            break;
        }
        this.Cylinders[index].gameObject.SetActive(true);
        this.Cylinders[index].position = castOrigin;
        this.Cylinders[index].rotation = Quaternion.LookRotation(vector3_2);
        this.Cylinders[index].localScale = new Vector3(0.05f, 0.05f, vector3_2.magnitude);
        if (this.m_hasValidRotateDir)
        {
          this.Cylinders[index].localScale = new Vector3(0.05f, 0.05f, this.m_hit_ray.distance);
          num = index + 1;
          break;
        }
        castOrigin += vector3_2;
        vector3_2 *= 0.95f;
        vector3_2 += -Vector3.up * 9.8f * (3f / 1000f);
      }
      for (int index = num; index < 20; ++index)
      {
        if (this.Cylinders[index].gameObject.activeSelf)
          this.Cylinders[index].gameObject.SetActive(false);
      }
      Vector3 vector3_3 = vector3_1 - this.m_validPoint;
      vector3_3.y = 0.0f;
      vector3_3.Normalize();
      return vector3_3;
    }

    public Vector3 RotatePointAroundPivotWithEuler(
      Vector3 point,
      Vector3 pivot,
      Vector3 angles)
    {
      return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }

    public float TeleportToPoint(Vector3 point, bool isAbsolute, Vector3 lookDir)
    {
      lookDir.y = 0.0f;
      this.transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
      return this.TeleportToPoint(point, isAbsolute);
    }

    public float TeleportToPoint(Vector3 point, bool isAbsolute)
    {
      this.m_isDashing = false;
      this.m_DashTarget = Vector3.zero;
      this.m_isSliding = false;
      this.m_SlidingTarget = Vector3.zero;
      Vector3 position = this.transform.position;
      for (int index = 0; index < this.Hands.Length; ++index)
      {
        this.Hands[index].PollInput();
        this.Hands[index].FlushFilter();
      }
      if (isAbsolute)
      {
        this.transform.position = point;
      }
      else
      {
        Vector3 vector3 = this.transform.position - GM.CurrentPlayerBody.NeckJointTransform.position;
        vector3.y = 0.0f;
        this.transform.position = point + vector3;
      }
      this.Body.UpdatePlayerBodyPositions();
      Vector3 dir = this.transform.position - position;
      this.Body.MoveQuickbeltContents(dir);
      this.SetTopSpeedLastSecond(dir);
      for (int index = 0; index < this.Hands.Length; ++index)
      {
        if ((Object) this.Hands[index].CurrentInteractable != (Object) null && this.Hands[index].CurrentInteractable is FVRPhysicalObject && (!(this.Hands[index].CurrentInteractable as FVRPhysicalObject).DoesDirectParent && !(this.Hands[index].CurrentInteractable as FVRPhysicalObject).IsPivotLocked))
          (this.Hands[index].CurrentInteractable as FVRPhysicalObject).transform.position = (this.Hands[index].CurrentInteractable as FVRPhysicalObject).transform.position + (this.transform.position - position);
      }
      this.lastHeadPos = GM.CurrentPlayerBody.NeckJointTransform.transform.position;
      if (isAbsolute)
        this.InitArmSwinger();
      return Mathf.Clamp(Vector3.Distance(position, this.transform.position) * 0.3f, 0.0f, 1f);
    }

    public float SetDashDestination(Vector3 point)
    {
      this.m_isDashing = true;
      Vector3 vector3 = this.transform.position - this.Head.position;
      vector3.y = 0.0f;
      this.m_DashTarget = point + vector3;
      return Mathf.Clamp(Vector3.Distance(point, this.transform.position) * 0.3f, 0.0f, 1f);
    }

    public void SetSlidingDestination(Vector3 point)
    {
      this.m_isSliding = true;
      Vector3 vector3 = this.transform.position - this.Head.position;
      vector3.y = 0.0f;
      this.m_SlidingTarget = point + vector3;
    }

    public enum MovementMode
    {
      Teleport,
      Dash,
      SingleTwoAxis,
      SlideToTarget,
      Armswinger,
      JoystickTeleport,
      TwinStick,
    }

    public enum MovementAxialOrigin
    {
      Hands,
      Head,
    }

    public enum TeleportMode
    {
      Standard,
      FrontFacingMode,
    }

    public enum TwoAxisMovementMode
    {
      Standard,
      Onward,
      LeveledHand,
      LeveledHead,
    }

    public enum TwoAxisMovementConfirm
    {
      OnTouch,
      OnClick,
    }

    public enum TwoAxisMovementStyle
    {
      LateralIsStrafe,
      LateralIsSnapTurn,
      LateralIsSmoothTurn,
    }
  }
}
