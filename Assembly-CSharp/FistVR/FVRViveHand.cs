// Decompiled with JetBrains decompiler
// Type: FistVR.FVRViveHand
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using Unity.Labs.SuperScience;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

namespace FistVR
{
  public class FVRViveHand : MonoBehaviour
  {
    public SteamVR_Input_Sources HandSource = SteamVR_Input_Sources.LeftHand;
    public SteamVR_Action_Boolean Trigger_Button;
    public SteamVR_Action_Boolean Trigger_Touch;
    public SteamVR_Action_Boolean Primary2Axis_Button;
    public SteamVR_Action_Boolean Primary2Axis_Touch;
    public SteamVR_Action_Boolean Secondary2Axis_Button;
    public SteamVR_Action_Boolean Secondary2Axis_Touch;
    public SteamVR_Action_Boolean A_Button;
    public SteamVR_Action_Boolean B_Button;
    public SteamVR_Action_Boolean Grip_Button;
    public SteamVR_Action_Boolean Grip_Touch;
    public SteamVR_Action_Single Trigger_Axis;
    public SteamVR_Action_Single Grip_Squeeze;
    public SteamVR_Action_Single Thumb_Squeeze;
    public SteamVR_Action_Vector2 Primary2Axis_Axes;
    public SteamVR_Action_Vector2 Secondary2Axis_Axes;
    public SteamVR_Action_Pose Pose;
    public SteamVR_Action_Vibration Vibration;
    public SteamVR_Action_Skeleton Skeleton;
    public bool DebugPrintOutput;
    public Text DisplayOut;
    private FVRViveHand.HandInitializationState m_initState;
    private FVRViveHand.HandState m_state;
    public FVRViveHand.HandMode Mode;
    public ControlMode CMode;
    public DisplayMode DMode;
    public bool IsInDemoMode;
    [Header("Connections to Body")]
    public Transform WholeRig;
    public Transform Head;
    public FVRMovementManager MovementManager;
    public bool IsInStreamlinedMode;
    [HideInInspector]
    [Header("Connections to In-Hand Elements")]
    public GameObject Display_Controller;
    public GameObject Display_Controller_Vive;
    public GameObject Display_Controller_Touch;
    public GameObject Display_Controller_Index;
    public GameObject Display_Controller_WMR;
    public GameObject Display_Controller_RiftS;
    public GameObject Display_Controller_Cosmos;
    public GameObject Display_Controller_HPR2;
    public GameObject Display_Controller_Quest2;
    public GameObject Display_InteractionSphere;
    public GameObject Display_InteractionSphere_Palm;
    public GameObject SausageFingersPrefab;
    private GameObject m_sausageFingersSpawned;
    public SphereCollider Collider_Fingers;
    public SphereCollider Collider_Palm;
    public Transform PointingTransform;
    public Renderer TouchSphere;
    public Renderer TouchSphere_Palm;
    public Material TouchSphereMat_NoInteractable;
    public Material TouchSpheteMat_Interactable;
    private bool m_touchSphereMatInteractable;
    private bool m_touchSphereMatInteractablePalm;
    [HideInInspector]
    public Vector3 m_storedInitialPointingTransformPos;
    [HideInInspector]
    public Quaternion m_storedInitialPointingTransformDir;
    public Transform PoseOverride;
    private Rigidbody m_rigidbody;
    public Transform PalmTransform;
    [Header("Mag Posing Stuff")]
    public Transform MagPose_Vive;
    public Transform MagPose_Rift;
    public Transform MagPose_RiftS;
    public Transform MagPose_Index;
    public Transform MagPose_WMR;
    [Header("Input")]
    public HandInput Input;
    public PhysicsTracker PhysTracker;
    private bool m_hasOverrider;
    private InputOverrider m_overrider;
    [Header("Connections to Wrist Menu")]
    public Transform WristMenuTarget;
    public Transform WristMenuTarget_Index;
    private FVRWristMenu m_wristmenu;
    private bool m_isWristMenuActive;
    [Header("Connections to Grab Laser")]
    public Transform GrabLaser;
    public GameObject BlueLaser;
    public GameObject RedLaser;
    private bool m_isGrabLaserActive;
    public LayerMask GrabLaserMask;
    private RaycastHit m_grabHit;
    [Header("Connections to Pointing Laser")]
    public Transform PointingLaser;
    private FVRPointable m_currentPointable;
    public LayerMask PointingLayerMask_UI;
    public LayerMask PointingLayerMask;
    private RaycastHit m_pointingHit;
    public LayerMask BracingMask;
    public Transform TouchpadArrowTarget;
    public FVRHaptics Buzzer;
    private FVRHapticBuzzProfile m_curBuzz;
    private bool m_isBuzzing;
    private float m_buzztime;
    private bool m_canMadeGrabReleaseSoundThisFrame = true;
    [HideInInspector]
    public bool IsGripToggle;
    public FVRViveHand OtherHand;
    private AudioSource m_aud;
    [HideInInspector]
    public Rigidbody m_rb;
    private FVRInteractiveObject m_closestPossibleInteractable;
    private bool m_isClosestInteractableInPalm;
    private FVRInteractiveObject m_currentInteractable;
    private FVRQuickBeltSlot m_currentHoveredQuickbeltSlot;
    private FVRQuickBeltSlot m_currentHoveredQuickbeltSlotDirty;
    public string DeviceName;
    private bool m_hasInit;
    public bool IsThisTheRightHand;
    private ForceTubeVRInterface forcetubevr;
    private float frequency = 1f;
    private float oldFrequency;
    private float frequencyDiffPeriod;
    private float timeTillNextPulse;
    private float amplitude = 1f;
    private bool doHaptics;
    private float m_maxVel;
    private float m_maxReal;
    private float m_timeSinceLastGripButtonDown;
    private float m_timeGripButtonHasBeenHeld;
    [Header("Grabbity")]
    public LayerMask LM_Grabbity_Beam;
    public LayerMask LM_Grabbity_BeamTrigger;
    public LayerMask LM_Grabbity_Block;
    private RaycastHit m_grabbity_hit;
    public AudioEvent AudEvent_Grabbity_Grab;
    public AudioEvent AudEvent_Grabbity_Flick;
    public AudioEvent AudEvent_Grabbity_Hover;
    private FVRPhysicalObject m_grabityHoveredObject;
    private FVRPhysicalObject m_selectedObj;
    private bool m_isObjectInTransit;
    private float m_reset = 1f;
    public Transform Grabbity_HoverSphere;
    public Transform Grabbity_GrabSphere;

    public Transform GetMagPose()
    {
      switch (this.DMode)
      {
        case DisplayMode.Vive:
          return this.MagPose_Vive;
        case DisplayMode.ViveCosmos:
          return this.MagPose_Rift;
        case DisplayMode.Rift:
          return this.MagPose_Rift;
        case DisplayMode.RiftS:
          return this.MagPose_RiftS;
        case DisplayMode.WMR:
          return this.MagPose_WMR;
        case DisplayMode.Index:
          return this.MagPose_Index;
        default:
          return this.PoseOverride;
      }
    }

    public void SetOverrider(InputOverrider o)
    {
      this.m_overrider = o;
      if (o == null)
        return;
      this.m_hasOverrider = true;
    }

    public void FlushOverrideIfThis(InputOverrider o)
    {
      if (this.m_overrider != o)
        return;
      this.m_overrider = (InputOverrider) null;
      this.m_hasOverrider = false;
    }

    public Transform GetWristMenuTarget() => this.CMode == ControlMode.Index ? this.WristMenuTarget_Index : this.WristMenuTarget;

    public FVRPointable CurrentPointable
    {
      get => this.m_currentPointable;
      set
      {
        if ((Object) this.m_currentPointable == (Object) value)
        {
          if ((Object) this.m_currentPointable != (Object) null)
            this.m_currentPointable.OnPoint(this);
        }
        else
        {
          if ((Object) this.m_currentPointable != (Object) null)
            this.m_currentPointable.EndPoint(this);
          this.m_currentPointable = value;
          if ((Object) this.m_currentPointable != (Object) null)
            this.m_currentPointable.OnPoint(this);
        }
        if ((Object) this.m_currentPointable == (Object) null)
        {
          if (!this.PointingLaser.gameObject.activeSelf)
            return;
          this.PointingLaser.gameObject.SetActive(false);
        }
        else
        {
          if (this.PointingLaser.gameObject.activeSelf)
            return;
          this.PointingLaser.gameObject.SetActive(true);
        }
      }
    }

    public bool CanMakeGrabReleaseSound => this.m_canMadeGrabReleaseSoundThisFrame;

    public void HandMadeGrabReleaseSound() => this.m_canMadeGrabReleaseSoundThisFrame = false;

    public FVRInteractiveObject ClosestPossibleInteractable
    {
      get => this.m_closestPossibleInteractable;
      set
      {
        if (((Object) value == (Object) null || (Object) this.m_closestPossibleInteractable != (Object) value) && (Object) this.m_closestPossibleInteractable != (Object) null)
          this.m_closestPossibleInteractable.IsHovered = false;
        this.m_closestPossibleInteractable = value;
        if (!((Object) this.m_closestPossibleInteractable != (Object) null))
          return;
        this.Buzz(this.Buzzer.Buzz_OnHoverInteractive);
        this.m_closestPossibleInteractable.IsHovered = true;
      }
    }

    public FVRInteractiveObject CurrentInteractable
    {
      get => this.m_currentInteractable;
      set
      {
        if ((Object) value != (Object) null)
          this.ClosestPossibleInteractable = (FVRInteractiveObject) null;
        this.m_currentInteractable = value;
        if ((Object) this.m_currentInteractable == (Object) null)
        {
          if (!this.Display_InteractionSphere.activeSelf)
            this.Display_InteractionSphere.SetActive(true);
          if (!this.Display_InteractionSphere_Palm.activeSelf)
            this.Display_InteractionSphere_Palm.SetActive(true);
        }
        else
        {
          if (this.Display_InteractionSphere.activeSelf)
            this.Display_InteractionSphere.SetActive(false);
          if (this.Display_InteractionSphere_Palm.activeSelf)
            this.Display_InteractionSphere_Palm.SetActive(false);
        }
        if (GM.Options.QuickbeltOptions.HideControllerGeoWhenObjectHeld)
        {
          if ((Object) this.m_currentInteractable == (Object) null)
          {
            if (this.Display_Controller.activeSelf)
              return;
            this.Display_Controller.SetActive(true);
          }
          else
          {
            if (!this.Display_Controller.activeSelf)
              return;
            this.Display_Controller.SetActive(false);
          }
        }
        else
        {
          if (this.Display_Controller.activeSelf)
            return;
          this.Display_Controller.SetActive(true);
        }
      }
    }

    public FVRQuickBeltSlot CurrentHoveredQuickbeltSlot
    {
      get => this.m_currentHoveredQuickbeltSlot;
      set
      {
        if (!((Object) value == (Object) null) && !((Object) this.m_currentHoveredQuickbeltSlot != (Object) value))
          return;
        if ((Object) this.m_currentHoveredQuickbeltSlot != (Object) null)
          this.m_currentHoveredQuickbeltSlot.IsHovered = false;
        this.m_currentHoveredQuickbeltSlot = value;
        if (!((Object) this.m_currentHoveredQuickbeltSlot != (Object) null))
          return;
        this.Buzz(this.Buzzer.Buzz_OnHoverInventorySlot);
        this.m_currentHoveredQuickbeltSlot.IsHovered = true;
      }
    }

    public FVRQuickBeltSlot CurrentHoveredQuickbeltSlotDirty
    {
      get => this.m_currentHoveredQuickbeltSlotDirty;
      set => this.m_currentHoveredQuickbeltSlotDirty = value;
    }

    public void FlushFilter()
    {
      this.Input.Pos = this.PoseOverride.position;
      this.Input.Rot = this.PoseOverride.rotation;
      this.Input.FilteredPos = this.Input.Pos;
      this.Input.FilteredRot = this.Input.Rot;
      this.Input.FilteredPalmPos = this.Input.Hand.PalmTransform.position;
      this.Input.FilteredPalmRot = this.Input.Hand.PalmTransform.rotation;
      this.Input.FilteredForward = this.Input.Forward;
      this.Input.FilteredUp = this.Input.Up;
      this.Input.FilteredRight = this.Input.Right;
    }

    private void Awake()
    {
      this.Input.Hand = this;
      this.Input.Init();
      this.m_aud = this.GetComponent<AudioSource>();
      this.m_rb = this.GetComponent<Rigidbody>();
      this.Grabbity_GrabSphere.SetParent((Transform) null);
      this.Grabbity_HoverSphere.SetParent((Transform) null);
      this.m_storedInitialPointingTransformDir = this.PointingTransform.localRotation;
      this.m_storedInitialPointingTransformPos = this.PointingTransform.localPosition;
    }

    [DebuggerHidden]
    private IEnumerator InitCMode() => (IEnumerator) new FVRViveHand.\u003CInitCMode\u003Ec__Iterator0()
    {
      \u0024this = this
    };

    public void SpawnSausageFingers()
    {
      if (this.CMode != ControlMode.Index || (Object) this.m_sausageFingersSpawned != (Object) null)
        return;
      this.Display_Controller_Vive.SetActive(false);
      this.Display_Controller_Touch.SetActive(false);
      this.Display_Controller_Index.SetActive(false);
      this.Display_Controller_WMR.SetActive(false);
      GameObject gameObject = Object.Instantiate<GameObject>(this.SausageFingersPrefab, this.transform.position, this.transform.rotation);
      gameObject.transform.SetParent(this.transform);
      this.m_sausageFingersSpawned = gameObject;
      gameObject.GetComponent<FVRMeatHands>().hand = this;
    }

    public bool HasInit => this.m_hasInit;

    private void Start()
    {
      if (!this.gameObject.activeInHierarchy)
        return;
      this.StartCoroutine("InitCMode");
    }

    private void DoInitialize()
    {
      this.m_rigidbody = this.GetComponent<Rigidbody>();
      if ((Object) this.m_rigidbody == (Object) null)
        this.m_rigidbody = this.gameObject.AddComponent<Rigidbody>();
      this.m_rigidbody.isKinematic = true;
      this.m_initState = FVRViveHand.HandInitializationState.Initialized;
    }

    public void UpdateControllerDefinition()
    {
      if (this.DMode == DisplayMode.Vive)
      {
        this.Display_Controller_Vive.SetActive(true);
        this.Display_Controller_RiftS.SetActive(false);
        this.Display_Controller_Touch.SetActive(false);
        this.Display_Controller_Index.SetActive(false);
        this.Display_Controller_WMR.SetActive(false);
        this.Display_Controller_Cosmos.SetActive(false);
        this.Display_Controller_HPR2.SetActive(false);
        this.Display_Controller_Quest2.SetActive(false);
        this.ConfigureFromControllerDefinition(0);
      }
      else if (this.DMode == DisplayMode.Rift)
      {
        this.Display_Controller_Touch.SetActive(true);
        this.Display_Controller_RiftS.SetActive(false);
        this.Display_Controller_Vive.SetActive(false);
        this.Display_Controller_Index.SetActive(false);
        this.Display_Controller_WMR.SetActive(false);
        this.Display_Controller_Cosmos.SetActive(false);
        this.Display_Controller_HPR2.SetActive(false);
        this.Display_Controller_Quest2.SetActive(false);
        this.ConfigureFromControllerDefinition(1);
      }
      else if (this.DMode == DisplayMode.RiftS)
      {
        this.Display_Controller_RiftS.SetActive(true);
        this.Display_Controller_Touch.SetActive(false);
        this.Display_Controller_Vive.SetActive(false);
        this.Display_Controller_Index.SetActive(false);
        this.Display_Controller_WMR.SetActive(false);
        this.Display_Controller_Cosmos.SetActive(false);
        this.Display_Controller_HPR2.SetActive(false);
        this.Display_Controller_Quest2.SetActive(false);
        this.ConfigureFromControllerDefinition(1);
      }
      else if (this.DMode == DisplayMode.Index)
      {
        this.Display_Controller_Index.SetActive(true);
        this.Display_Controller_RiftS.SetActive(false);
        this.Display_Controller_Vive.SetActive(false);
        this.Display_Controller_Touch.SetActive(false);
        this.Display_Controller_WMR.SetActive(false);
        this.Display_Controller_Cosmos.SetActive(false);
        this.Display_Controller_HPR2.SetActive(false);
        this.Display_Controller_Quest2.SetActive(false);
        this.ConfigureFromControllerDefinition(1);
      }
      else if (this.DMode == DisplayMode.Quest2)
      {
        this.Display_Controller_Quest2.SetActive(true);
        this.Display_Controller_RiftS.SetActive(false);
        this.Display_Controller_Vive.SetActive(false);
        this.Display_Controller_Touch.SetActive(false);
        this.Display_Controller_Index.SetActive(false);
        this.Display_Controller_Cosmos.SetActive(false);
        this.Display_Controller_HPR2.SetActive(false);
        this.Display_Controller_WMR.SetActive(false);
        this.ConfigureFromControllerDefinition(1);
      }
      else if (this.DMode == DisplayMode.WMR)
      {
        this.Display_Controller_WMR.SetActive(true);
        this.Display_Controller_RiftS.SetActive(false);
        this.Display_Controller_Vive.SetActive(false);
        this.Display_Controller_Touch.SetActive(false);
        this.Display_Controller_Index.SetActive(false);
        this.Display_Controller_Cosmos.SetActive(false);
        this.Display_Controller_HPR2.SetActive(false);
        this.Display_Controller_Quest2.SetActive(false);
        this.ConfigureFromControllerDefinition(0);
      }
      else if (this.DMode == DisplayMode.ViveCosmos)
      {
        this.Display_Controller_Cosmos.SetActive(true);
        this.Display_Controller_WMR.SetActive(false);
        this.Display_Controller_RiftS.SetActive(false);
        this.Display_Controller_Vive.SetActive(false);
        this.Display_Controller_Touch.SetActive(false);
        this.Display_Controller_Index.SetActive(false);
        this.Display_Controller_HPR2.SetActive(false);
        this.Display_Controller_Quest2.SetActive(false);
        this.ConfigureFromControllerDefinition(1);
      }
      else if (this.DMode == DisplayMode.WMRHPRb2)
      {
        this.Display_Controller_HPR2.SetActive(true);
        this.Display_Controller_RiftS.SetActive(false);
        this.Display_Controller_Vive.SetActive(false);
        this.Display_Controller_Touch.SetActive(false);
        this.Display_Controller_Index.SetActive(false);
        this.Display_Controller_Cosmos.SetActive(false);
        this.Display_Controller_WMR.SetActive(false);
        this.Display_Controller_Quest2.SetActive(false);
        this.ConfigureFromControllerDefinition(0);
      }
      this.PhysTracker.Reset(this.transform.localPosition, this.transform.localRotation, Vector3.zero, Vector3.zero);
    }

    public void ConfigureFromControllerDefinition(int i)
    {
      this.PoseOverride.localPosition = ManagerSingleton<GM>.Instance.ControllerDefinitions[i].PoseTransformOffset;
      this.PoseOverride.localEulerAngles = ManagerSingleton<GM>.Instance.ControllerDefinitions[i].PoseTransformRotOffset;
      this.PointingTransform.localPosition = ManagerSingleton<GM>.Instance.ControllerDefinitions[i].InteractionSphereOffset;
      this.Display_InteractionSphere.transform.localPosition = ManagerSingleton<GM>.Instance.ControllerDefinitions[i].InteractionSphereOffset;
      this.GetComponent<SphereCollider>().center = ManagerSingleton<GM>.Instance.ControllerDefinitions[i].InteractionSphereOffset;
    }

    public void ForceTubeKick(byte duration) => ForceTubeVRInterface.Kick(duration);

    public void ForceTubeRumble(byte intensity, float duration) => ForceTubeVRInterface.Rumble(intensity, duration);

    public void Buzz(FVRHapticBuzzProfile buzz)
    {
      if (GM.Options.ControlOptions.HapticsState == ControlOptions.HapticsMode.Disabled || this.IsInDemoMode)
        return;
      this.m_curBuzz = buzz;
      this.m_buzztime = 0.0f;
      this.m_isBuzzing = true;
    }

    private void HapticBuzzUpdate()
    {
      this.timeTillNextPulse -= Time.fixedDeltaTime;
      if ((double) this.frequency == 0.0)
      {
        this.frequency = 0.0f;
        this.oldFrequency = 0.0f;
      }
      else
      {
        if ((double) this.oldFrequency == 0.0)
        {
          this.timeTillNextPulse = this.PeriodForFreq(this.frequency);
          this.oldFrequency = this.frequency;
        }
        float num = this.PeriodForFreq(this.frequency);
        this.frequencyDiffPeriod = num - this.PeriodForFreq(this.oldFrequency);
        this.timeTillNextPulse += this.frequencyDiffPeriod;
        if ((double) this.timeTillNextPulse <= 0.0)
        {
          if (this.m_isBuzzing && (Object) this.m_curBuzz != (Object) null)
          {
            if ((double) this.m_buzztime < (double) this.m_curBuzz.BuzzLength)
            {
              this.m_buzztime += Time.deltaTime;
              this.amplitude = this.m_curBuzz.BuzzCurve.Evaluate(this.m_buzztime / this.m_curBuzz.BuzzLength) * this.m_curBuzz.AmpMult;
              this.frequency = (float) this.m_curBuzz.Freq;
              if ((double) this.amplitude > 0.00999999977648258)
                this.Vibration.Execute(0.0f, Time.fixedDeltaTime, this.frequency, this.amplitude, this.HandSource);
            }
            else
              this.m_isBuzzing = false;
          }
          else
            this.m_isBuzzing = false;
          this.timeTillNextPulse = num;
        }
        this.oldFrequency = this.frequency;
      }
    }

    private float PeriodForFreq(float frequency) => (double) frequency != 0.0 ? 1f / frequency : float.PositiveInfinity;

    private float FrequencyForTime(float time) => 1f / time;

    public void RetrieveObject(FVRPhysicalObject obj)
    {
      this.CurrentInteractable = (FVRInteractiveObject) obj;
      if ((Object) obj.PoseOverride != (Object) null)
      {
        FVRViveHand.AlignChild(obj.transform, obj.PoseOverride, this.transform);
      }
      else
      {
        obj.transform.position = this.transform.position;
        obj.transform.rotation = this.transform.rotation;
      }
      this.CurrentInteractable.BeginInteraction(this);
      this.m_state = FVRViveHand.HandState.GripInteracting;
    }

    public void EnableWristMenu(FVRWristMenu menu)
    {
      this.m_wristmenu = menu;
      this.m_isWristMenuActive = true;
    }

    public void DisableWristMenu()
    {
      this.m_wristmenu = (FVRWristMenu) null;
      this.m_isWristMenuActive = false;
    }

    public Vector3 GetThrowLinearVelWorld() => this.WholeRig.TransformDirection(this.PhysTracker.Velocity) + GM.CurrentMovementManager.GetLastWorldDir() * GM.CurrentMovementManager.GetTopSpeedInLastSecond();

    public Vector3 GetThrowAngularVelWorld() => this.WholeRig.TransformDirection(this.PhysTracker.AngularVelocity);

    public void PollInput()
    {
      this.IsInStreamlinedMode = GM.Options.ControlOptions.CCM != ControlOptions.CoreControlMode.Standard;
      this.PhysTracker.Update(this.transform.localPosition, this.transform.localRotation, Time.smoothDeltaTime);
      this.Input.TriggerUp = this.Trigger_Button.GetStateUp(this.HandSource);
      this.Input.TriggerDown = this.Trigger_Button.GetStateDown(this.HandSource);
      this.Input.TriggerPressed = this.Trigger_Button.GetState(this.HandSource);
      this.Input.TriggerFloat = this.Trigger_Axis.GetAxis(this.HandSource);
      this.Input.TriggerTouchUp = this.Trigger_Touch.GetStateUp(this.HandSource);
      this.Input.TriggerTouchDown = this.Trigger_Touch.GetStateDown(this.HandSource);
      this.Input.TriggerTouched = this.Trigger_Touch.GetState(this.HandSource);
      this.Input.GripUp = this.Grip_Button.GetStateUp(this.HandSource);
      this.Input.GripDown = this.Grip_Button.GetStateDown(this.HandSource);
      this.Input.GripPressed = this.Grip_Button.GetState(this.HandSource);
      this.Input.GripTouchUp = this.Grip_Touch.GetStateUp(this.HandSource);
      this.Input.GripTouchDown = this.Grip_Touch.GetStateDown(this.HandSource);
      this.Input.GripTouched = this.Grip_Touch.GetState(this.HandSource);
      this.Input.TouchpadUp = this.Primary2Axis_Button.GetStateUp(this.HandSource);
      this.Input.TouchpadDown = this.Primary2Axis_Button.GetStateDown(this.HandSource);
      this.Input.TouchpadPressed = this.Primary2Axis_Button.GetState(this.HandSource);
      this.Input.TouchpadTouchUp = this.Primary2Axis_Touch.GetStateUp(this.HandSource);
      this.Input.TouchpadTouchDown = this.Primary2Axis_Touch.GetStateDown(this.HandSource);
      this.Input.TouchpadTouched = this.Primary2Axis_Touch.GetState(this.HandSource);
      this.Input.TouchpadAxes = this.Primary2Axis_Axes.GetAxis(this.HandSource);
      this.Input.TouchpadNorthDown = false;
      this.Input.TouchpadSouthDown = false;
      this.Input.TouchpadWestDown = false;
      this.Input.TouchpadEastDown = false;
      this.Input.TouchpadNorthUp = false;
      this.Input.TouchpadSouthUp = false;
      this.Input.TouchpadWestUp = false;
      this.Input.TouchpadEastUp = false;
      if ((double) this.Input.TouchpadAxes.magnitude < 0.5)
      {
        if (!this.Input.TouchpadCenterPressed)
          this.Input.TouchpadCenterDown = true;
        if (this.Input.TouchpadNorthPressed)
          this.Input.TouchpadNorthUp = true;
        if (this.Input.TouchpadSouthPressed)
          this.Input.TouchpadSouthUp = true;
        if (this.Input.TouchpadWestPressed)
          this.Input.TouchpadWestUp = true;
        if (this.Input.TouchpadEastPressed)
          this.Input.TouchpadEastUp = true;
        this.Input.TouchpadNorthPressed = false;
        this.Input.TouchpadSouthPressed = false;
        this.Input.TouchpadWestPressed = false;
        this.Input.TouchpadEastPressed = false;
        this.Input.TouchpadCenterPressed = true;
      }
      else
      {
        if (this.Input.TouchpadCenterPressed)
          this.Input.TouchpadCenterUp = true;
        this.Input.TouchpadCenterPressed = false;
        if ((double) Vector2.Angle(this.Input.TouchpadAxes, Vector2.up) <= 45.0)
        {
          if (!this.Input.TouchpadNorthPressed)
            this.Input.TouchpadNorthDown = true;
          this.Input.TouchpadNorthPressed = true;
          this.Input.TouchpadSouthPressed = false;
          this.Input.TouchpadWestPressed = false;
          this.Input.TouchpadEastPressed = false;
        }
        else if ((double) Vector2.Angle(this.Input.TouchpadAxes, Vector2.down) <= 45.0)
        {
          if (!this.Input.TouchpadSouthPressed)
            this.Input.TouchpadSouthDown = true;
          this.Input.TouchpadSouthPressed = true;
          this.Input.TouchpadNorthPressed = false;
          this.Input.TouchpadWestPressed = false;
          this.Input.TouchpadEastPressed = false;
        }
        else if ((double) Vector2.Angle(this.Input.TouchpadAxes, Vector2.left) <= 45.0)
        {
          if (!this.Input.TouchpadWestPressed)
            this.Input.TouchpadWestDown = true;
          this.Input.TouchpadWestPressed = true;
          this.Input.TouchpadNorthPressed = false;
          this.Input.TouchpadSouthPressed = false;
          this.Input.TouchpadEastPressed = false;
        }
        else if ((double) Vector2.Angle(this.Input.TouchpadAxes, Vector2.right) <= 45.0)
        {
          if (!this.Input.TouchpadEastPressed)
            this.Input.TouchpadEastDown = true;
          this.Input.TouchpadEastPressed = true;
          this.Input.TouchpadNorthPressed = false;
          this.Input.TouchpadSouthPressed = false;
          this.Input.TouchpadWestPressed = false;
        }
      }
      this.Input.BYButtonUp = this.B_Button.GetStateUp(this.HandSource);
      this.Input.BYButtonDown = this.B_Button.GetStateDown(this.HandSource);
      this.Input.BYButtonPressed = this.B_Button.GetState(this.HandSource);
      this.Input.AXButtonUp = this.A_Button.GetStateUp(this.HandSource);
      this.Input.AXButtonDown = this.A_Button.GetStateDown(this.HandSource);
      this.Input.AXButtonPressed = this.A_Button.GetState(this.HandSource);
      this.Input.Secondary2AxisInputUp = this.Secondary2Axis_Button.GetStateUp(this.HandSource);
      this.Input.Secondary2AxisInputDown = this.Secondary2Axis_Button.GetStateDown(this.HandSource);
      this.Input.Secondary2AxisInputPressed = this.Secondary2Axis_Button.GetState(this.HandSource);
      this.Input.Secondary2AxisInputTouchUp = this.Secondary2Axis_Touch.GetStateUp(this.HandSource);
      this.Input.Secondary2AxisInputTouchDown = this.Secondary2Axis_Touch.GetStateDown(this.HandSource);
      this.Input.Secondary2AxisInputTouched = this.Secondary2Axis_Touch.GetState(this.HandSource);
      this.Input.Secondary2AxisInputAxes = this.Secondary2Axis_Axes.GetAxis(this.HandSource);
      this.Input.Secondary2AxisNorthDown = false;
      this.Input.Secondary2AxisSouthDown = false;
      this.Input.Secondary2AxisWestDown = false;
      this.Input.Secondary2AxisEastDown = false;
      this.Input.Secondary2AxisNorthUp = false;
      this.Input.Secondary2AxisSouthUp = false;
      this.Input.Secondary2AxisWestUp = false;
      this.Input.Secondary2AxisEastUp = false;
      if ((double) this.Input.Secondary2AxisInputAxes.magnitude < 0.5)
      {
        if (!this.Input.Secondary2AxisCenterPressed)
          this.Input.Secondary2AxisCenterDown = true;
        if (this.Input.Secondary2AxisNorthPressed)
          this.Input.Secondary2AxisNorthUp = true;
        if (this.Input.Secondary2AxisSouthPressed)
          this.Input.Secondary2AxisSouthUp = true;
        if (this.Input.Secondary2AxisWestPressed)
          this.Input.Secondary2AxisWestUp = true;
        if (this.Input.Secondary2AxisEastPressed)
          this.Input.Secondary2AxisEastUp = true;
        this.Input.Secondary2AxisNorthPressed = false;
        this.Input.Secondary2AxisSouthPressed = false;
        this.Input.Secondary2AxisWestPressed = false;
        this.Input.Secondary2AxisEastPressed = false;
        this.Input.Secondary2AxisCenterPressed = true;
      }
      else
      {
        if (this.Input.Secondary2AxisCenterPressed)
          this.Input.Secondary2AxisCenterUp = true;
        this.Input.Secondary2AxisCenterPressed = false;
        if ((double) Vector2.Angle(this.Input.Secondary2AxisInputAxes, Vector2.up) <= 45.0)
        {
          if (!this.Input.Secondary2AxisNorthPressed)
            this.Input.Secondary2AxisNorthDown = true;
          this.Input.Secondary2AxisNorthPressed = true;
          this.Input.Secondary2AxisSouthPressed = false;
          this.Input.Secondary2AxisWestPressed = false;
          this.Input.Secondary2AxisEastPressed = false;
        }
        else if ((double) Vector2.Angle(this.Input.Secondary2AxisInputAxes, Vector2.down) <= 45.0)
        {
          if (!this.Input.Secondary2AxisSouthPressed)
            this.Input.Secondary2AxisSouthDown = true;
          this.Input.Secondary2AxisSouthPressed = true;
          this.Input.Secondary2AxisNorthPressed = false;
          this.Input.Secondary2AxisWestPressed = false;
          this.Input.Secondary2AxisEastPressed = false;
        }
        else if ((double) Vector2.Angle(this.Input.Secondary2AxisInputAxes, Vector2.left) <= 45.0)
        {
          if (!this.Input.Secondary2AxisWestPressed)
            this.Input.Secondary2AxisWestDown = true;
          this.Input.Secondary2AxisWestPressed = true;
          this.Input.Secondary2AxisNorthPressed = false;
          this.Input.Secondary2AxisSouthPressed = false;
          this.Input.Secondary2AxisEastPressed = false;
        }
        else if ((double) Vector2.Angle(this.Input.Secondary2AxisInputAxes, Vector2.right) <= 45.0)
        {
          if (!this.Input.Secondary2AxisEastPressed)
            this.Input.Secondary2AxisEastDown = true;
          this.Input.Secondary2AxisEastPressed = true;
          this.Input.Secondary2AxisNorthPressed = false;
          this.Input.Secondary2AxisSouthPressed = false;
          this.Input.Secondary2AxisWestPressed = false;
        }
      }
      this.Input.FingerCurl_Thumb = this.Skeleton.fingerCurls[0];
      this.Input.FingerCurl_Index = this.Skeleton.fingerCurls[1];
      this.Input.FingerCurl_Middle = this.Skeleton.fingerCurls[2];
      this.Input.FingerCurl_Ring = this.Skeleton.fingerCurls[3];
      this.Input.FingerCurl_Pinky = this.Skeleton.fingerCurls[4];
      this.Input.Pos = this.PoseOverride.position;
      this.Input.Rot = this.PoseOverride.rotation;
      this.Input.UpdateEuroFilter();
      float num1 = Mathf.Min(Vector3.Distance(this.Input.Pos, this.Input.OneEuroPosition) / 0.05f, 1f);
      float num2 = num1 * num1;
      this.Input.FilteredPos = (1f - num2) * this.Input.OneEuroPosition + num2 * this.Input.Pos;
      float num3 = Mathf.Min(Quaternion.Angle(this.Input.Rot, this.Input.OneEuroRotation) / 3f, 1f);
      this.Input.FilteredRot = Quaternion.Slerp(this.Input.OneEuroRotation, this.Input.Rot, num3 * num3);
      float num4 = Mathf.Min(Vector3.Distance(this.Input.Hand.PalmTransform.position, this.Input.OneEuroPalmPosition) / 0.05f, 1f);
      float num5 = num4 * num4;
      this.Input.FilteredPalmPos = (1f - num5) * this.Input.OneEuroPalmPosition + num5 * this.Input.Hand.PalmTransform.position;
      float num6 = Mathf.Min(Quaternion.Angle(this.Input.Hand.PalmTransform.rotation, this.Input.OneEuroPalmRotation) / 3f, 1f);
      this.Input.FilteredPalmRot = Quaternion.Slerp(this.Input.OneEuroPalmRotation, this.Input.Hand.PalmTransform.rotation, num6 * num6);
      this.Input.Forward = this.PoseOverride.forward;
      this.Input.Up = this.PoseOverride.up;
      this.Input.Right = this.PoseOverride.right;
      float num7 = Mathf.Min(Vector3.Distance(this.Input.FilteredForward, this.Input.Forward) / 0.015f, 1f);
      this.Input.FilteredForward = (1f - num7) * this.Input.FilteredForward + num7 * this.Input.Forward;
      float num8 = Mathf.Min(Vector3.Distance(this.Input.FilteredUp, this.Input.Up) / 0.015f, 1f);
      this.Input.FilteredUp = (1f - num8) * this.Input.FilteredUp + num8 * this.Input.Up;
      float num9 = Mathf.Min(Vector3.Distance(this.Input.FilteredRight, this.Input.Right) / 0.015f, 1f);
      this.Input.FilteredRight = (1f - num9) * this.Input.FilteredRight + num9 * this.Input.Right;
      this.Input.VelLinearLocal = this.Pose.GetVelocity(this.HandSource);
      this.Input.VelAngularLocal = this.Pose.GetAngularVelocity(this.HandSource);
      this.Input.VelLinearWorld = this.WholeRig.TransformDirection(this.Input.VelLinearLocal);
      this.Input.VelAngularWorld = this.WholeRig.TransformDirection(this.Input.VelAngularLocal);
      ControlMode controlMode = this.CMode;
      if (GM.Options.ControlOptions.GripButtonToHoldOverride == ControlOptions.GripButtonToHoldOverrideMode.OculusOverride)
        controlMode = ControlMode.Oculus;
      else if (GM.Options.ControlOptions.GripButtonToHoldOverride == ControlOptions.GripButtonToHoldOverrideMode.ViveOverride)
        controlMode = ControlMode.Vive;
      switch (controlMode)
      {
        case ControlMode.Vive:
        case ControlMode.WMR:
          this.Input.IsGrabUp = this.Input.TriggerUp;
          this.Input.IsGrabDown = this.Input.TriggerDown;
          this.Input.IsGrabbing = this.Input.TriggerPressed;
          break;
        case ControlMode.Oculus:
          this.Input.IsGrabUp = this.Input.GripUp;
          this.Input.IsGrabDown = this.Input.GripDown;
          this.Input.IsGrabbing = this.Input.GripPressed;
          break;
        case ControlMode.Index:
          bool flag = false;
          float num10 = (float) (((double) this.Input.FingerCurl_Middle * 2.0 + (double) this.Input.FingerCurl_Ring * 1.0) / 3.0);
          float num11 = num10 - this.Input.LastCurlAverage;
          if (this.Input.TriggerPressed)
            flag = true;
          else if (this.Input.IsGrabbing && (double) num10 >= 0.5)
            flag = true;
          else if (this.Input.IsGrabbing && this.Input.TriggerTouched)
            flag = true;
          else if (this.Input.IsGrabbing && this.Input.GripTouched)
            flag = true;
          else if (this.Input.GripPressed)
            flag = true;
          if (this.Input.IsGrabbing && !this.Input.TriggerPressed && !this.Input.GripPressed && ((double) num11 < -0.300000011920929 && (double) num10 < 0.699999988079071 || (double) num11 < -0.5))
            flag = false;
          this.Input.IsGrabUp = this.Input.IsGrabbing && !flag;
          this.Input.IsGrabDown = !this.Input.IsGrabbing && flag || this.Input.TriggerDown;
          this.Input.IsGrabbing = flag;
          this.Input.LastCurlAverage = num10;
          break;
      }
      if ((double) this.m_timeSinceLastGripButtonDown < 5.0)
        this.m_timeSinceLastGripButtonDown += Time.deltaTime;
      this.Input.LastPalmPos2 = this.Input.LastPalmPos1;
      this.Input.LastPalmPos1 = this.PalmTransform.position;
    }

    private void FlushTouchpadData()
    {
      this.Input.TouchpadUp = false;
      this.Input.TouchpadDown = false;
      this.Input.TouchpadPressed = false;
      this.Input.TouchpadTouchUp = false;
      this.Input.TouchpadTouchDown = false;
      this.Input.TouchpadTouched = false;
      this.Input.TouchpadAxes = Vector2.zero;
    }

    private void Update()
    {
      if (this.m_initState == FVRViveHand.HandInitializationState.Uninitialized)
        return;
      if ((Object) this.m_selectedObj != (Object) null && this.m_selectedObj.IsHeld)
      {
        this.m_selectedObj = (FVRPhysicalObject) null;
        this.m_reset = 0.0f;
        this.m_isObjectInTransit = false;
      }
      if ((double) this.m_reset >= 0.0 && this.m_isObjectInTransit)
      {
        if ((Object) this.m_selectedObj != (Object) null && (double) Vector3.Distance(this.m_selectedObj.transform.position, this.transform.position) < 0.400000005960464)
        {
          Vector3 vector = Vector3.Lerp(this.m_selectedObj.RootRigidbody.velocity, this.transform.position - this.m_selectedObj.transform.position, Time.deltaTime * 2f);
          this.m_selectedObj.RootRigidbody.velocity = Vector3.ClampMagnitude(vector, this.m_selectedObj.RootRigidbody.velocity.magnitude);
          this.m_selectedObj.RootRigidbody.velocity = vector;
          this.m_selectedObj.RootRigidbody.drag = 1f;
          this.m_selectedObj.RootRigidbody.angularDrag = 8f;
          this.m_reset -= Time.deltaTime * 0.4f;
        }
        else
          this.m_reset -= Time.deltaTime;
        if ((double) this.m_reset <= 0.0)
        {
          this.m_isObjectInTransit = false;
          if ((Object) this.m_selectedObj != (Object) null)
          {
            this.m_selectedObj.RecoverDrag();
            this.m_selectedObj = (FVRPhysicalObject) null;
          }
        }
      }
      this.HapticBuzzUpdate();
      this.TestQuickBeltDistances();
      this.PollInput();
      if (this.m_hasOverrider && this.m_overrider != null)
        this.m_overrider.Process(ref this.Input);
      else
        this.m_hasOverrider = false;
      if (!((Object) this.m_currentInteractable != (Object) null) || !this.Input.TriggerPressed)
        ;
      if ((Object) this.ClosestPossibleInteractable == (Object) null)
      {
        if (this.m_touchSphereMatInteractable)
        {
          this.m_touchSphereMatInteractable = false;
          this.TouchSphere.material = this.TouchSphereMat_NoInteractable;
        }
        if (this.m_touchSphereMatInteractablePalm)
        {
          this.m_touchSphereMatInteractablePalm = false;
          this.TouchSphere_Palm.material = this.TouchSphereMat_NoInteractable;
        }
      }
      else if (!this.m_touchSphereMatInteractable && !this.m_isClosestInteractableInPalm)
      {
        this.m_touchSphereMatInteractable = true;
        this.TouchSphere.material = this.TouchSpheteMat_Interactable;
        this.m_touchSphereMatInteractablePalm = false;
        this.TouchSphere_Palm.material = this.TouchSphereMat_NoInteractable;
      }
      else if (!this.m_touchSphereMatInteractablePalm && this.m_isClosestInteractableInPalm)
      {
        this.m_touchSphereMatInteractablePalm = true;
        this.TouchSphere_Palm.material = this.TouchSpheteMat_Interactable;
        this.m_touchSphereMatInteractable = false;
        this.TouchSphere.material = this.TouchSphereMat_NoInteractable;
      }
      float num1 = 1f / GM.CurrentPlayerBody.transform.localScale.x;
      if (this.m_state == FVRViveHand.HandState.Empty && !this.Input.BYButtonPressed && (!this.Input.TouchpadPressed && (Object) this.ClosestPossibleInteractable == (Object) null) && ((Object) this.CurrentHoveredQuickbeltSlot == (Object) null && (Object) this.CurrentInteractable == (Object) null && !this.m_isWristMenuActive))
      {
        if (Physics.Raycast(this.PointingTransform.position, this.PointingTransform.forward, out this.m_pointingHit, GM.CurrentSceneSettings.MaxPointingDistance, (int) this.PointingLayerMask, QueryTriggerInteraction.Collide) && (bool) (Object) this.m_pointingHit.collider.gameObject.GetComponent<FVRPointable>())
        {
          FVRPointable component = this.m_pointingHit.collider.gameObject.GetComponent<FVRPointable>();
          if ((double) this.m_pointingHit.distance <= (double) component.MaxPointingRange)
          {
            this.CurrentPointable = component;
            this.PointingLaser.position = this.PointingTransform.position;
            this.PointingLaser.rotation = this.PointingTransform.rotation;
            this.PointingLaser.localScale = new Vector3(1f / 500f, 1f / 500f, this.m_pointingHit.distance) * num1;
          }
          else
            this.CurrentPointable = (FVRPointable) null;
        }
        else
          this.CurrentPointable = (FVRPointable) null;
      }
      else
        this.CurrentPointable = (FVRPointable) null;
      this.MovementManager.UpdateMovementWithHand(this);
      if (this.MovementManager.ShouldFlushTouchpad(this))
        this.FlushTouchpadData();
      bool flag1;
      bool flag2;
      if (this.IsInStreamlinedMode)
      {
        flag1 = this.Input.BYButtonDown;
        flag2 = this.Input.BYButtonPressed;
      }
      else
      {
        flag1 = this.Input.TouchpadDown;
        flag2 = this.Input.TouchpadPressed;
      }
      if (this.m_state == FVRViveHand.HandState.Empty && (Object) this.CurrentHoveredQuickbeltSlot == (Object) null)
      {
        if (flag2)
        {
          if (!this.GrabLaser.gameObject.activeSelf)
            this.GrabLaser.gameObject.SetActive(true);
          bool flag3 = false;
          FVRPhysicalObject fvrPhysicalObject = (FVRPhysicalObject) null;
          if (Physics.Raycast(this.PointingTransform.position, this.PointingTransform.forward, out this.m_grabHit, 3f, (int) this.GrabLaserMask, QueryTriggerInteraction.Collide))
          {
            if ((Object) this.m_grabHit.collider.attachedRigidbody != (Object) null && (bool) (Object) this.m_grabHit.collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>())
            {
              fvrPhysicalObject = this.m_grabHit.collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
              if ((Object) fvrPhysicalObject != (Object) null && !fvrPhysicalObject.IsHeld && fvrPhysicalObject.IsDistantGrabbable())
                flag3 = true;
            }
            this.GrabLaser.localScale = new Vector3(0.004f, 0.004f, this.m_grabHit.distance) * num1;
          }
          else
            this.GrabLaser.localScale = new Vector3(0.004f, 0.004f, 3f) * num1;
          this.GrabLaser.position = this.PointingTransform.position;
          this.GrabLaser.rotation = this.PointingTransform.rotation;
          if (flag3)
          {
            if (!this.BlueLaser.activeSelf)
              this.BlueLaser.SetActive(true);
            if (this.RedLaser.activeSelf)
              this.RedLaser.SetActive(false);
            if (this.Input.IsGrabDown && (Object) fvrPhysicalObject != (Object) null)
            {
              this.RetrieveObject(fvrPhysicalObject);
              if (this.GrabLaser.gameObject.activeSelf)
                this.GrabLaser.gameObject.SetActive(false);
            }
          }
          else
          {
            if (this.BlueLaser.activeSelf)
              this.BlueLaser.SetActive(false);
            if (!this.RedLaser.activeSelf)
              this.RedLaser.SetActive(true);
          }
        }
        else if (this.GrabLaser.gameObject.activeSelf)
          this.GrabLaser.gameObject.SetActive(false);
      }
      else if (this.GrabLaser.gameObject.activeSelf)
        this.GrabLaser.gameObject.SetActive(false);
      if (this.Mode == FVRViveHand.HandMode.Neutral && this.m_state == FVRViveHand.HandState.Empty && flag1)
      {
        bool spawnLockingEnabled = GM.CurrentSceneSettings.IsSpawnLockingEnabled;
        if ((Object) this.ClosestPossibleInteractable != (Object) null && this.ClosestPossibleInteractable is FVRPhysicalObject)
        {
          FVRPhysicalObject possibleInteractable = this.ClosestPossibleInteractable as FVRPhysicalObject;
          if ((possibleInteractable.SpawnLockable && spawnLockingEnabled || possibleInteractable.Harnessable) && (Object) possibleInteractable.QuickbeltSlot != (Object) null)
            possibleInteractable.ToggleQuickbeltState();
        }
        else if ((Object) this.CurrentHoveredQuickbeltSlot != (Object) null && (Object) this.CurrentHoveredQuickbeltSlot.HeldObject != (Object) null)
        {
          FVRPhysicalObject heldObject = this.CurrentHoveredQuickbeltSlot.HeldObject as FVRPhysicalObject;
          if (heldObject.SpawnLockable && spawnLockingEnabled || heldObject.Harnessable)
            heldObject.ToggleQuickbeltState();
        }
      }
      this.UpdateGrabityDisplay();
      if (this.Mode == FVRViveHand.HandMode.Neutral)
      {
        if (this.m_state == FVRViveHand.HandState.Empty)
        {
          bool flag3 = false;
          if (this.Input.IsGrabDown)
          {
            if ((Object) this.CurrentHoveredQuickbeltSlot != (Object) null && (Object) this.CurrentHoveredQuickbeltSlot.CurObject != (Object) null)
            {
              this.CurrentInteractable = (FVRInteractiveObject) this.CurrentHoveredQuickbeltSlot.CurObject;
              this.m_state = FVRViveHand.HandState.GripInteracting;
              this.CurrentInteractable.BeginInteraction(this);
              this.Buzz(this.Buzzer.Buzz_BeginInteraction);
              flag3 = true;
            }
            else if ((Object) this.ClosestPossibleInteractable != (Object) null && !this.ClosestPossibleInteractable.IsSimpleInteract)
            {
              this.CurrentInteractable = this.ClosestPossibleInteractable;
              this.m_state = FVRViveHand.HandState.GripInteracting;
              this.CurrentInteractable.BeginInteraction(this);
              this.Buzz(this.Buzzer.Buzz_BeginInteraction);
              flag3 = true;
            }
          }
          bool flag4 = false;
          if (!flag3 && this.Input.TriggerDown && (!((Object) this.CurrentHoveredQuickbeltSlot != (Object) null) || !((Object) this.CurrentHoveredQuickbeltSlot.CurObject != (Object) null)) && ((Object) this.ClosestPossibleInteractable != (Object) null && this.ClosestPossibleInteractable.IsSimpleInteract))
          {
            this.ClosestPossibleInteractable.SimpleInteraction(this);
            flag4 = true;
          }
          if (GM.Options.ControlOptions.WIPGrabbityState == ControlOptions.WIPGrabbity.Enabled && !flag3 && !flag4)
          {
            if ((Object) this.m_selectedObj == (Object) null)
              this.CastToFindHover();
            else
              this.SetGrabbityHovered((FVRPhysicalObject) null);
            bool flag5;
            bool flag6;
            if (GM.Options.ControlOptions.WIPGrabbityButtonState == ControlOptions.WIPGrabbityButton.Grab)
            {
              flag5 = this.Input.GripDown;
              flag6 = this.Input.GripUp;
            }
            else
            {
              flag5 = this.Input.TriggerDown;
              flag6 = this.Input.TriggerUp;
            }
            if (flag5 && (Object) this.m_grabityHoveredObject != (Object) null && (Object) this.m_selectedObj == (Object) null)
              this.CastToGrab();
            if (flag6 && !this.m_isObjectInTransit)
              this.m_selectedObj = (FVRPhysicalObject) null;
            if ((Object) this.m_selectedObj != (Object) null && !this.m_isObjectInTransit)
            {
              float num2 = 3.5f;
              if ((double) Mathf.Abs(this.Input.VelAngularLocal.x) > (double) num2 || (double) Mathf.Abs(this.Input.VelAngularLocal.y) > (double) num2)
                this.BeginFlick(this.m_selectedObj);
            }
          }
          else
            this.SetGrabbityHovered((FVRPhysicalObject) null);
          if (GM.Options.ControlOptions.WIPGrabbityState == ControlOptions.WIPGrabbity.Enabled && !flag3 && (!flag4 && this.Input.IsGrabDown) && (this.m_isObjectInTransit && (Object) this.m_selectedObj != (Object) null && (double) Vector3.Distance(this.transform.position, this.m_selectedObj.transform.position) < 0.5))
          {
            if (this.m_selectedObj.UseGripRotInterp)
            {
              this.CurrentInteractable = (FVRInteractiveObject) this.m_selectedObj;
              this.CurrentInteractable.BeginInteraction(this);
              this.m_state = FVRViveHand.HandState.GripInteracting;
            }
            else
              this.RetrieveObject(this.m_selectedObj);
            this.m_selectedObj = (FVRPhysicalObject) null;
            this.m_isObjectInTransit = false;
            this.SetGrabbityHovered((FVRPhysicalObject) null);
          }
        }
        else if (this.m_state == FVRViveHand.HandState.GripInteracting)
        {
          this.SetGrabbityHovered((FVRPhysicalObject) null);
          bool flag3 = false;
          if ((Object) this.CurrentInteractable != (Object) null)
          {
            ControlMode controlMode = this.CMode;
            if (GM.Options.ControlOptions.GripButtonToHoldOverride == ControlOptions.GripButtonToHoldOverrideMode.OculusOverride)
              controlMode = ControlMode.Oculus;
            else if (GM.Options.ControlOptions.GripButtonToHoldOverride == ControlOptions.GripButtonToHoldOverrideMode.ViveOverride)
              controlMode = ControlMode.Vive;
            if (controlMode == ControlMode.Vive || controlMode == ControlMode.WMR)
            {
              if (this.CurrentInteractable.ControlType == FVRInteractionControlType.GrabHold)
              {
                if (this.Input.TriggerUp)
                  flag3 = true;
              }
              else if (this.CurrentInteractable.ControlType == FVRInteractionControlType.GrabToggle)
              {
                switch (GM.Options.ControlOptions.GripButtonDropStyle)
                {
                  case ControlOptions.ButtonControlStyle.Instant:
                    if (!this.Input.TriggerPressed && this.Input.GripDown)
                    {
                      flag3 = true;
                      break;
                    }
                    break;
                  case ControlOptions.ButtonControlStyle.Hold1Second:
                    if (!this.Input.TriggerPressed && (double) this.m_timeGripButtonHasBeenHeld > 1.0)
                    {
                      flag3 = true;
                      break;
                    }
                    break;
                  case ControlOptions.ButtonControlStyle.DoubleClick:
                    if (!this.Input.TriggerPressed && this.Input.GripDown && ((double) this.m_timeSinceLastGripButtonDown > 0.0500000007450581 && (double) this.m_timeSinceLastGripButtonDown < 0.400000005960464))
                    {
                      flag3 = true;
                      break;
                    }
                    break;
                }
              }
            }
            else if (this.Input.IsGrabUp)
              flag3 = true;
            if (flag3)
            {
              if (this.CurrentInteractable is FVRPhysicalObject && (Object) ((FVRPhysicalObject) this.CurrentInteractable).QuickbeltSlot == (Object) null && (!((FVRPhysicalObject) this.CurrentInteractable).IsPivotLocked && (Object) this.CurrentHoveredQuickbeltSlot != (Object) null) && ((Object) this.CurrentHoveredQuickbeltSlot.HeldObject == (Object) null && ((FVRPhysicalObject) this.CurrentInteractable).QBSlotType == this.CurrentHoveredQuickbeltSlot.Type && this.CurrentHoveredQuickbeltSlot.SizeLimit >= ((FVRPhysicalObject) this.CurrentInteractable).Size))
                ((FVRPhysicalObject) this.CurrentInteractable).EndInteractionIntoInventorySlot(this, this.CurrentHoveredQuickbeltSlot);
              else
                this.CurrentInteractable.EndInteraction(this);
              this.CurrentInteractable = (FVRInteractiveObject) null;
              this.m_state = FVRViveHand.HandState.Empty;
            }
            else
              this.CurrentInteractable.UpdateInteraction(this);
          }
          else
            this.m_state = FVRViveHand.HandState.Empty;
        }
      }
      if (this.Input.GripPressed)
      {
        this.m_timeSinceLastGripButtonDown = 0.0f;
        this.m_timeGripButtonHasBeenHeld += Time.deltaTime;
      }
      else
        this.m_timeGripButtonHasBeenHeld = 0.0f;
      this.m_canMadeGrabReleaseSoundThisFrame = true;
    }

    private void SetGrabbityHovered(FVRPhysicalObject o)
    {
      if ((Object) o == (Object) null)
      {
        this.m_grabityHoveredObject = o;
      }
      else
      {
        if (!((Object) this.m_grabityHoveredObject != (Object) o))
          return;
        this.m_grabityHoveredObject = o;
        SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_Grabbity_Hover, this.transform.position);
      }
    }

    private void UpdateGrabityDisplay()
    {
      if ((Object) this.m_grabityHoveredObject != (Object) null && (this.m_grabityHoveredObject.IsHeld || !this.m_grabityHoveredObject.IsDistantGrabbable() || (Object) this.m_grabityHoveredObject.QuickbeltSlot != (Object) null))
        this.m_grabityHoveredObject = (FVRPhysicalObject) null;
      if ((Object) this.m_selectedObj != (Object) null && (this.m_selectedObj.IsHeld || !this.m_selectedObj.IsDistantGrabbable() || (Object) this.m_selectedObj.QuickbeltSlot != (Object) null))
        this.m_selectedObj = (FVRPhysicalObject) null;
      if ((Object) this.m_grabityHoveredObject != (Object) null)
      {
        this.Grabbity_HoverSphere.gameObject.SetActive(true);
        this.Grabbity_HoverSphere.position = this.m_grabityHoveredObject.transform.position;
      }
      else
        this.Grabbity_HoverSphere.gameObject.SetActive(false);
      if ((Object) this.m_selectedObj != (Object) null)
      {
        this.Grabbity_GrabSphere.gameObject.SetActive(true);
        this.Grabbity_GrabSphere.position = this.m_selectedObj.transform.position;
      }
      else
        this.Grabbity_GrabSphere.gameObject.SetActive(false);
    }

    private void CastToFindHover()
    {
      bool flag = false;
      if (Physics.Raycast(this.PointingTransform.position, this.Input.FilteredForward, out this.m_grabbity_hit, 10f, (int) this.LM_Grabbity_Beam, QueryTriggerInteraction.Collide) && (Object) this.m_grabbity_hit.collider.attachedRigidbody != (Object) null)
      {
        FVRPhysicalObject component = this.m_grabbity_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
        if ((Object) component != (Object) null && !component.IsHeld && (component.IsDistantGrabbable() && (Object) component.QuickbeltSlot == (Object) null) && (!component.RootRigidbody.isKinematic && !Physics.Linecast(this.PointingTransform.position, this.PointingTransform.position + this.PointingTransform.forward * this.m_grabbity_hit.distance, (int) this.LM_Grabbity_Block, QueryTriggerInteraction.Ignore)))
        {
          this.SetGrabbityHovered(component);
          flag = true;
        }
      }
      if (!flag && Physics.SphereCast(this.PointingTransform.position, 0.2f, this.PointingTransform.forward, out this.m_grabbity_hit, 10f, (int) this.LM_Grabbity_BeamTrigger, QueryTriggerInteraction.Collide) && (Object) this.m_grabbity_hit.collider.attachedRigidbody != (Object) null)
      {
        FVRPhysicalObject component = this.m_grabbity_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
        if ((Object) component != (Object) null && !component.IsHeld && (component.IsDistantGrabbable() && (Object) component.QuickbeltSlot == (Object) null) && (!component.RootRigidbody.isKinematic && !Physics.Linecast(this.PointingTransform.position, this.PointingTransform.position + this.PointingTransform.forward * this.m_grabbity_hit.distance, (int) this.LM_Grabbity_Block, QueryTriggerInteraction.Ignore)))
        {
          this.SetGrabbityHovered(component);
          flag = true;
        }
      }
      if (!flag && Physics.SphereCast(this.PointingTransform.position, 0.2f, this.PointingTransform.forward, out this.m_grabbity_hit, 10f, (int) this.LM_Grabbity_Beam, QueryTriggerInteraction.Collide) && (Object) this.m_grabbity_hit.collider.attachedRigidbody != (Object) null)
      {
        FVRPhysicalObject component = this.m_grabbity_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
        if ((Object) component != (Object) null && !component.IsHeld && (component.IsDistantGrabbable() && (Object) component.QuickbeltSlot == (Object) null) && (!component.RootRigidbody.isKinematic && !Physics.Linecast(this.PointingTransform.position, this.PointingTransform.position + this.PointingTransform.forward * this.m_grabbity_hit.distance, (int) this.LM_Grabbity_Block, QueryTriggerInteraction.Ignore)))
        {
          this.SetGrabbityHovered(component);
          flag = true;
        }
      }
      if (flag)
        return;
      this.SetGrabbityHovered((FVRPhysicalObject) null);
    }

    private void CastToGrab()
    {
      if (!((Object) this.m_grabityHoveredObject != (Object) null))
        return;
      if ((Object) this.m_selectedObj != (Object) null)
        this.m_selectedObj.RecoverDrag();
      this.m_selectedObj = this.m_grabityHoveredObject;
      this.SetGrabbityHovered((FVRPhysicalObject) null);
      SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_Grabbity_Grab, this.transform.position);
      this.m_reset = 2f;
    }

    private void BeginFlick(FVRPhysicalObject o)
    {
      Vector3 vector3_1 = this.transform.position - o.transform.position;
      float proj_speed = Mathf.Clamp(Vector3.Distance(this.transform.position, o.transform.position) * 1f, 0.1f, 10f);
      Vector3 vector3_2;
      if ((double) Mathf.Abs(Physics.gravity.y) > 0.100000001490116)
      {
        Vector3 s0;
        Vector3 s1;
        int num = fts.solve_ballistic_arc(o.transform.position, proj_speed, this.transform.position, Mathf.Abs(Physics.gravity.y), out s0, out s1);
        if (num < 1)
        {
          proj_speed = Mathf.Clamp(proj_speed * 1.5f, 2f, 15f);
          num = fts.solve_ballistic_arc(o.transform.position, proj_speed, this.transform.position, Mathf.Abs(Physics.gravity.y), out s0, out s1);
        }
        if (num < 1)
        {
          proj_speed = Mathf.Clamp(proj_speed * 1.5f, 3f, 25f);
          num = fts.solve_ballistic_arc(o.transform.position, proj_speed, this.transform.position, Mathf.Abs(Physics.gravity.y), out s0, out s1);
        }
        if (num < 1)
        {
          for (int index = 0; index < 10 && fts.solve_ballistic_arc(o.transform.position, proj_speed, this.transform.position, Mathf.Abs(Physics.gravity.y), out s0, out s1) <= 0; ++index)
            ++proj_speed;
          vector3_2 = s0;
        }
        else
          vector3_2 = s0;
      }
      else
        vector3_2 = vector3_1;
      SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_Grabbity_Flick, this.transform.position);
      this.m_selectedObj.MP.DeJoint();
      this.m_selectedObj.RootRigidbody.drag = 0.0f;
      this.m_selectedObj.RootRigidbody.angularDrag = 0.0f;
      this.m_selectedObj.RootRigidbody.velocity = vector3_2;
      this.m_isObjectInTransit = true;
      this.m_reset = (float) ((double) vector3_1.magnitude / (double) vector3_2.magnitude * 1.5);
    }

    public void UpdateHandInput()
    {
    }

    public void EndInteractionIfHeld(FVRInteractiveObject inter)
    {
      if (!((Object) inter == (Object) this.CurrentInteractable))
        return;
      this.CurrentInteractable = (FVRInteractiveObject) null;
      this.ClosestPossibleInteractable = (FVRInteractiveObject) null;
    }

    public void ForceSetInteractable(FVRInteractiveObject inter)
    {
      if ((Object) this.CurrentInteractable != (Object) null)
        this.CurrentInteractable.EndInteraction(this);
      this.CurrentInteractable = inter;
      if ((Object) inter == (Object) null)
      {
        this.CurrentInteractable = (FVRInteractiveObject) null;
        this.ClosestPossibleInteractable = (FVRInteractiveObject) null;
        this.m_state = FVRViveHand.HandState.Empty;
      }
      else
        this.m_state = FVRViveHand.HandState.GripInteracting;
    }

    public void TestCollider(Collider collider, bool isEnter, bool isPalm)
    {
      if (isEnter)
      {
        if (!((Object) collider.gameObject.GetComponent<FVRInteractiveObject>() != (Object) null))
          return;
        collider.gameObject.GetComponent<FVRInteractiveObject>().Poke(this);
      }
      else
      {
        if (this.m_state != FVRViveHand.HandState.Empty || !((Object) collider.gameObject.GetComponent<FVRInteractiveObject>() != (Object) null))
          return;
        FVRInteractiveObject component = collider.gameObject.GetComponent<FVRInteractiveObject>();
        if (!((Object) component != (Object) null) || !component.IsInteractable() || component.IsSelectionRestricted())
          return;
        float num1 = Vector3.Distance(collider.transform.position, this.Display_InteractionSphere.transform.position);
        float num2 = Vector3.Distance(collider.transform.position, this.Display_InteractionSphere_Palm.transform.position);
        if ((Object) this.ClosestPossibleInteractable == (Object) null)
        {
          this.ClosestPossibleInteractable = component;
          if ((double) num1 < (double) num2)
            this.m_isClosestInteractableInPalm = false;
          else
            this.m_isClosestInteractableInPalm = true;
        }
        else
        {
          if (!((Object) this.ClosestPossibleInteractable != (Object) component))
            return;
          float num3 = Vector3.Distance(this.ClosestPossibleInteractable.transform.position, this.Display_InteractionSphere.transform.position);
          float num4 = Vector3.Distance(this.ClosestPossibleInteractable.transform.position, this.Display_InteractionSphere_Palm.transform.position);
          bool flag = true;
          if ((double) num1 < (double) num2)
            flag = false;
          if (flag && (double) num2 < (double) num4 && this.m_isClosestInteractableInPalm)
          {
            this.m_isClosestInteractableInPalm = true;
            this.ClosestPossibleInteractable = component;
          }
          else
          {
            if (flag || (double) num1 >= (double) num3)
              return;
            this.m_isClosestInteractableInPalm = false;
            this.ClosestPossibleInteractable = component;
          }
        }
      }
    }

    public void HandTriggerExit(Collider collider, bool isPalm)
    {
      if (!((Object) collider.gameObject.GetComponent<FVRInteractiveObject>() != (Object) null) || !((Object) this.ClosestPossibleInteractable == (Object) collider.GetComponent<FVRInteractiveObject>()))
        return;
      this.ClosestPossibleInteractable = (FVRInteractiveObject) null;
      this.m_isClosestInteractableInPalm = false;
    }

    private void TestQuickBeltDistances()
    {
      if ((Object) this.CurrentHoveredQuickbeltSlot != (Object) null && !this.CurrentHoveredQuickbeltSlot.IsSelectable)
        this.CurrentHoveredQuickbeltSlot = (FVRQuickBeltSlot) null;
      if ((Object) this.CurrentHoveredQuickbeltSlotDirty != (Object) null && !this.CurrentHoveredQuickbeltSlotDirty.IsSelectable)
        this.CurrentHoveredQuickbeltSlotDirty = (FVRQuickBeltSlot) null;
      FVRQuickBeltSlot fvrQuickBeltSlot = (FVRQuickBeltSlot) null;
      Vector3 v = this.PoseOverride.position;
      if ((Object) this.CurrentInteractable != (Object) null)
        v = !((Object) this.CurrentInteractable.PoseOverride != (Object) null) ? this.CurrentInteractable.transform.position : this.CurrentInteractable.PoseOverride.position;
      for (int index = 0; index < GM.CurrentPlayerBody.QuickbeltSlots.Count; ++index)
      {
        if (GM.CurrentPlayerBody.QuickbeltSlots[index].IsPointInsideMe(v))
        {
          fvrQuickBeltSlot = GM.CurrentPlayerBody.QuickbeltSlots[index];
          break;
        }
      }
      if ((Object) fvrQuickBeltSlot == (Object) null)
      {
        if ((Object) this.CurrentHoveredQuickbeltSlot != (Object) null)
          this.CurrentHoveredQuickbeltSlot = (FVRQuickBeltSlot) null;
        this.CurrentHoveredQuickbeltSlotDirty = (FVRQuickBeltSlot) null;
      }
      else
      {
        this.CurrentHoveredQuickbeltSlotDirty = fvrQuickBeltSlot;
        if (this.m_state == FVRViveHand.HandState.Empty)
        {
          if (!((Object) fvrQuickBeltSlot.CurObject != (Object) null) || fvrQuickBeltSlot.CurObject.IsHeld)
            return;
          this.CurrentHoveredQuickbeltSlot = fvrQuickBeltSlot;
        }
        else
        {
          if (this.m_state != FVRViveHand.HandState.GripInteracting || !((Object) this.m_currentInteractable != (Object) null) || !(this.m_currentInteractable is FVRPhysicalObject))
            return;
          FVRPhysicalObject currentInteractable = (FVRPhysicalObject) this.m_currentInteractable;
          if (!((Object) fvrQuickBeltSlot.CurObject == (Object) null) || fvrQuickBeltSlot.SizeLimit < currentInteractable.Size || currentInteractable.QBSlotType != fvrQuickBeltSlot.Type)
            return;
          this.CurrentHoveredQuickbeltSlot = fvrQuickBeltSlot;
        }
      }
    }

    private void OnTriggerEnter(Collider collider) => this.TestCollider(collider, true, false);

    private void OnTriggerStay(Collider collider) => this.TestCollider(collider, false, false);

    private void OnTriggerExit(Collider collider) => this.HandTriggerExit(collider, false);

    public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
    {
      Quaternion quaternion = new Quaternion();
      quaternion.w = Mathf.Sqrt(Mathf.Max(0.0f, 1f + m[0, 0] + m[1, 1] + m[2, 2])) / 2f;
      quaternion.x = Mathf.Sqrt(Mathf.Max(0.0f, 1f + m[0, 0] - m[1, 1] - m[2, 2])) / 2f;
      quaternion.y = Mathf.Sqrt(Mathf.Max(0.0f, 1f - m[0, 0] + m[1, 1] - m[2, 2])) / 2f;
      quaternion.z = Mathf.Sqrt(Mathf.Max(0.0f, 1f - m[0, 0] - m[1, 1] + m[2, 2])) / 2f;
      quaternion.x *= Mathf.Sign(quaternion.x * (m[2, 1] - m[1, 2]));
      quaternion.y *= Mathf.Sign(quaternion.y * (m[0, 2] - m[2, 0]));
      quaternion.z *= Mathf.Sign(quaternion.z * (m[1, 0] - m[0, 1]));
      return quaternion;
    }

    public static void AlignChild(Transform main, Transform child, Transform alignTo)
    {
      Matrix4x4 matrix4x4 = Matrix4x4.TRS(child.position, child.rotation, Vector3.one);
      Matrix4x4 m = Matrix4x4.TRS(alignTo.position, alignTo.rotation, Vector3.one) * matrix4x4.inverse * main.localToWorldMatrix;
      main.position = (Vector3) m.GetColumn(3);
      main.rotation = FVRViveHand.QuaternionFromMatrix(m);
    }

    public enum HandInitializationState
    {
      Uninitialized,
      Initialized,
    }

    public enum HandState
    {
      Empty,
      GripInteracting,
    }

    public enum HandMode
    {
      Neutral,
      Menu,
    }
  }
}
