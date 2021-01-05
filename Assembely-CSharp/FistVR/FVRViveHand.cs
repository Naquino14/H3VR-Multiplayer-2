using System.Collections;
using Unity.Labs.SuperScience;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

namespace FistVR
{
	public class FVRViveHand : MonoBehaviour
	{
		public enum HandInitializationState
		{
			Uninitialized,
			Initialized
		}

		public enum HandState
		{
			Empty,
			GripInteracting
		}

		public enum HandMode
		{
			Neutral,
			Menu
		}

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

		private HandInitializationState m_initState;

		private HandState m_state;

		public HandMode Mode;

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

		public FVRPointable CurrentPointable
		{
			get
			{
				return m_currentPointable;
			}
			set
			{
				if (m_currentPointable == value)
				{
					if (m_currentPointable != null)
					{
						m_currentPointable.OnPoint(this);
					}
				}
				else
				{
					if (m_currentPointable != null)
					{
						m_currentPointable.EndPoint(this);
					}
					m_currentPointable = value;
					if (m_currentPointable != null)
					{
						m_currentPointable.OnPoint(this);
					}
				}
				if (m_currentPointable == null)
				{
					if (PointingLaser.gameObject.activeSelf)
					{
						PointingLaser.gameObject.SetActive(value: false);
					}
				}
				else if (!PointingLaser.gameObject.activeSelf)
				{
					PointingLaser.gameObject.SetActive(value: true);
				}
			}
		}

		public bool CanMakeGrabReleaseSound => m_canMadeGrabReleaseSoundThisFrame;

		public FVRInteractiveObject ClosestPossibleInteractable
		{
			get
			{
				return m_closestPossibleInteractable;
			}
			set
			{
				if ((value == null || m_closestPossibleInteractable != value) && m_closestPossibleInteractable != null)
				{
					m_closestPossibleInteractable.IsHovered = false;
				}
				m_closestPossibleInteractable = value;
				if (m_closestPossibleInteractable != null)
				{
					Buzz(Buzzer.Buzz_OnHoverInteractive);
					m_closestPossibleInteractable.IsHovered = true;
				}
			}
		}

		public FVRInteractiveObject CurrentInteractable
		{
			get
			{
				return m_currentInteractable;
			}
			set
			{
				if (value != null)
				{
					ClosestPossibleInteractable = null;
				}
				m_currentInteractable = value;
				if (m_currentInteractable == null)
				{
					if (!Display_InteractionSphere.activeSelf)
					{
						Display_InteractionSphere.SetActive(value: true);
					}
					if (!Display_InteractionSphere_Palm.activeSelf)
					{
						Display_InteractionSphere_Palm.SetActive(value: true);
					}
				}
				else
				{
					if (Display_InteractionSphere.activeSelf)
					{
						Display_InteractionSphere.SetActive(value: false);
					}
					if (Display_InteractionSphere_Palm.activeSelf)
					{
						Display_InteractionSphere_Palm.SetActive(value: false);
					}
				}
				if (GM.Options.QuickbeltOptions.HideControllerGeoWhenObjectHeld)
				{
					if (m_currentInteractable == null)
					{
						if (!Display_Controller.activeSelf)
						{
							Display_Controller.SetActive(value: true);
						}
					}
					else if (Display_Controller.activeSelf)
					{
						Display_Controller.SetActive(value: false);
					}
				}
				else if (!Display_Controller.activeSelf)
				{
					Display_Controller.SetActive(value: true);
				}
			}
		}

		public FVRQuickBeltSlot CurrentHoveredQuickbeltSlot
		{
			get
			{
				return m_currentHoveredQuickbeltSlot;
			}
			set
			{
				if (value == null || m_currentHoveredQuickbeltSlot != value)
				{
					if (m_currentHoveredQuickbeltSlot != null)
					{
						m_currentHoveredQuickbeltSlot.IsHovered = false;
					}
					m_currentHoveredQuickbeltSlot = value;
					if (m_currentHoveredQuickbeltSlot != null)
					{
						Buzz(Buzzer.Buzz_OnHoverInventorySlot);
						m_currentHoveredQuickbeltSlot.IsHovered = true;
					}
				}
			}
		}

		public FVRQuickBeltSlot CurrentHoveredQuickbeltSlotDirty
		{
			get
			{
				return m_currentHoveredQuickbeltSlotDirty;
			}
			set
			{
				m_currentHoveredQuickbeltSlotDirty = value;
			}
		}

		public bool HasInit => m_hasInit;

		public Transform GetMagPose()
		{
			return DMode switch
			{
				DisplayMode.Index => MagPose_Index, 
				DisplayMode.Rift => MagPose_Rift, 
				DisplayMode.Vive => MagPose_Vive, 
				DisplayMode.WMR => MagPose_WMR, 
				DisplayMode.ViveCosmos => MagPose_Rift, 
				DisplayMode.RiftS => MagPose_RiftS, 
				_ => PoseOverride, 
			};
		}

		public void SetOverrider(InputOverrider o)
		{
			m_overrider = o;
			if (o != null)
			{
				m_hasOverrider = true;
			}
		}

		public void FlushOverrideIfThis(InputOverrider o)
		{
			if (m_overrider == o)
			{
				m_overrider = null;
				m_hasOverrider = false;
			}
		}

		public Transform GetWristMenuTarget()
		{
			if (CMode == ControlMode.Index)
			{
				return WristMenuTarget_Index;
			}
			return WristMenuTarget;
		}

		public void HandMadeGrabReleaseSound()
		{
			m_canMadeGrabReleaseSoundThisFrame = false;
		}

		public void FlushFilter()
		{
			Input.Pos = PoseOverride.position;
			Input.Rot = PoseOverride.rotation;
			Input.FilteredPos = Input.Pos;
			Input.FilteredRot = Input.Rot;
			Input.FilteredPalmPos = Input.Hand.PalmTransform.position;
			Input.FilteredPalmRot = Input.Hand.PalmTransform.rotation;
			Input.FilteredForward = Input.Forward;
			Input.FilteredUp = Input.Up;
			Input.FilteredRight = Input.Right;
		}

		private void Awake()
		{
			Input.Hand = this;
			Input.Init();
			m_aud = GetComponent<AudioSource>();
			m_rb = GetComponent<Rigidbody>();
			Grabbity_GrabSphere.SetParent(null);
			Grabbity_HoverSphere.SetParent(null);
			m_storedInitialPointingTransformDir = PointingTransform.localRotation;
			m_storedInitialPointingTransformPos = PointingTransform.localPosition;
		}

		private IEnumerator InitCMode()
		{
			while (SteamVR_Input.isStartupFrame)
			{
				yield return null;
			}
			yield return null;
			yield return null;
			yield return null;
			while (!Pose[HandSource].active)
			{
				yield return null;
			}
			uint id = Pose[HandSource].trackedDeviceIndex;
			Debug.Log("Setting for trackedDeviceIndex:" + id);
			string model = SteamVR.instance.GetStringProperty(ETrackedDeviceProperty.Prop_ModelNumber_String, id).ToLower();
			string controllerType = SteamVR.instance.GetStringProperty(ETrackedDeviceProperty.Prop_ControllerType_String, id).ToLower();
			string serialNumber = SteamVR.instance.GetStringProperty(ETrackedDeviceProperty.Prop_SerialNumber_String, id);
			if (controllerType.Contains("hpmotion"))
			{
				DMode = DisplayMode.WMRHPRb2;
				CMode = ControlMode.Oculus;
			}
			else if (model.Contains("index") || model.Contains("utah") || model.Contains("knuckles"))
			{
				CMode = ControlMode.Index;
				DMode = DisplayMode.Index;
			}
			else if (model.Contains("cosmos"))
			{
				CMode = ControlMode.Vive;
				DMode = DisplayMode.ViveCosmos;
			}
			else if (model.Contains("vive") || model.Contains("nolo"))
			{
				CMode = ControlMode.Vive;
				DMode = DisplayMode.Vive;
			}
			else if (model.Contains("miramar"))
			{
				DMode = DisplayMode.Quest2;
				CMode = ControlMode.Oculus;
			}
			else if (model.Contains("rift s") || model.Contains("quest"))
			{
				CMode = ControlMode.Oculus;
				DMode = DisplayMode.RiftS;
			}
			else if (model.Contains("oculus") || model.Contains("cv1"))
			{
				CMode = ControlMode.Oculus;
				DMode = DisplayMode.Rift;
			}
			else
			{
				CMode = ControlMode.WMR;
				DMode = DisplayMode.WMR;
			}
			m_hasInit = true;
			Debug.Log("Setting control mode to CMODE:" + CMode.ToString() + " and DMODE" + DMode.ToString() + " using modelname:" + model + " and controllername:" + controllerType);
			UpdateControllerDefinition();
			m_storedInitialPointingTransformDir = PointingTransform.localRotation;
			m_storedInitialPointingTransformPos = PointingTransform.localPosition;
			DoInitialize();
			if (CMode == ControlMode.Index && GM.Options.ControlOptions.MFMode == ControlOptions.MeatFingerMode.Enabled)
			{
				SpawnSausageFingers();
			}
		}

		public void SpawnSausageFingers()
		{
			if (CMode == ControlMode.Index && !(m_sausageFingersSpawned != null))
			{
				Display_Controller_Vive.SetActive(value: false);
				Display_Controller_Touch.SetActive(value: false);
				Display_Controller_Index.SetActive(value: false);
				Display_Controller_WMR.SetActive(value: false);
				GameObject gameObject = Object.Instantiate(SausageFingersPrefab, base.transform.position, base.transform.rotation);
				gameObject.transform.SetParent(base.transform);
				m_sausageFingersSpawned = gameObject;
				FVRMeatHands component = gameObject.GetComponent<FVRMeatHands>();
				component.hand = this;
			}
		}

		private void Start()
		{
			if (base.gameObject.activeInHierarchy)
			{
				StartCoroutine("InitCMode");
			}
		}

		private void DoInitialize()
		{
			m_rigidbody = GetComponent<Rigidbody>();
			if (m_rigidbody == null)
			{
				m_rigidbody = base.gameObject.AddComponent<Rigidbody>();
			}
			m_rigidbody.isKinematic = true;
			m_initState = HandInitializationState.Initialized;
		}

		public void UpdateControllerDefinition()
		{
			if (DMode == DisplayMode.Vive)
			{
				Display_Controller_Vive.SetActive(value: true);
				Display_Controller_RiftS.SetActive(value: false);
				Display_Controller_Touch.SetActive(value: false);
				Display_Controller_Index.SetActive(value: false);
				Display_Controller_WMR.SetActive(value: false);
				Display_Controller_Cosmos.SetActive(value: false);
				Display_Controller_HPR2.SetActive(value: false);
				Display_Controller_Quest2.SetActive(value: false);
				ConfigureFromControllerDefinition(0);
			}
			else if (DMode == DisplayMode.Rift)
			{
				Display_Controller_Touch.SetActive(value: true);
				Display_Controller_RiftS.SetActive(value: false);
				Display_Controller_Vive.SetActive(value: false);
				Display_Controller_Index.SetActive(value: false);
				Display_Controller_WMR.SetActive(value: false);
				Display_Controller_Cosmos.SetActive(value: false);
				Display_Controller_HPR2.SetActive(value: false);
				Display_Controller_Quest2.SetActive(value: false);
				ConfigureFromControllerDefinition(1);
			}
			else if (DMode == DisplayMode.RiftS)
			{
				Display_Controller_RiftS.SetActive(value: true);
				Display_Controller_Touch.SetActive(value: false);
				Display_Controller_Vive.SetActive(value: false);
				Display_Controller_Index.SetActive(value: false);
				Display_Controller_WMR.SetActive(value: false);
				Display_Controller_Cosmos.SetActive(value: false);
				Display_Controller_HPR2.SetActive(value: false);
				Display_Controller_Quest2.SetActive(value: false);
				ConfigureFromControllerDefinition(1);
			}
			else if (DMode == DisplayMode.Index)
			{
				Display_Controller_Index.SetActive(value: true);
				Display_Controller_RiftS.SetActive(value: false);
				Display_Controller_Vive.SetActive(value: false);
				Display_Controller_Touch.SetActive(value: false);
				Display_Controller_WMR.SetActive(value: false);
				Display_Controller_Cosmos.SetActive(value: false);
				Display_Controller_HPR2.SetActive(value: false);
				Display_Controller_Quest2.SetActive(value: false);
				ConfigureFromControllerDefinition(1);
			}
			else if (DMode == DisplayMode.Quest2)
			{
				Display_Controller_Quest2.SetActive(value: true);
				Display_Controller_RiftS.SetActive(value: false);
				Display_Controller_Vive.SetActive(value: false);
				Display_Controller_Touch.SetActive(value: false);
				Display_Controller_Index.SetActive(value: false);
				Display_Controller_Cosmos.SetActive(value: false);
				Display_Controller_HPR2.SetActive(value: false);
				Display_Controller_WMR.SetActive(value: false);
				ConfigureFromControllerDefinition(1);
			}
			else if (DMode == DisplayMode.WMR)
			{
				Display_Controller_WMR.SetActive(value: true);
				Display_Controller_RiftS.SetActive(value: false);
				Display_Controller_Vive.SetActive(value: false);
				Display_Controller_Touch.SetActive(value: false);
				Display_Controller_Index.SetActive(value: false);
				Display_Controller_Cosmos.SetActive(value: false);
				Display_Controller_HPR2.SetActive(value: false);
				Display_Controller_Quest2.SetActive(value: false);
				ConfigureFromControllerDefinition(0);
			}
			else if (DMode == DisplayMode.ViveCosmos)
			{
				Display_Controller_Cosmos.SetActive(value: true);
				Display_Controller_WMR.SetActive(value: false);
				Display_Controller_RiftS.SetActive(value: false);
				Display_Controller_Vive.SetActive(value: false);
				Display_Controller_Touch.SetActive(value: false);
				Display_Controller_Index.SetActive(value: false);
				Display_Controller_HPR2.SetActive(value: false);
				Display_Controller_Quest2.SetActive(value: false);
				ConfigureFromControllerDefinition(1);
			}
			else if (DMode == DisplayMode.WMRHPRb2)
			{
				Display_Controller_HPR2.SetActive(value: true);
				Display_Controller_RiftS.SetActive(value: false);
				Display_Controller_Vive.SetActive(value: false);
				Display_Controller_Touch.SetActive(value: false);
				Display_Controller_Index.SetActive(value: false);
				Display_Controller_Cosmos.SetActive(value: false);
				Display_Controller_WMR.SetActive(value: false);
				Display_Controller_Quest2.SetActive(value: false);
				ConfigureFromControllerDefinition(0);
			}
			PhysTracker.Reset(base.transform.localPosition, base.transform.localRotation, Vector3.zero, Vector3.zero);
		}

		public void ConfigureFromControllerDefinition(int i)
		{
			PoseOverride.localPosition = ManagerSingleton<GM>.Instance.ControllerDefinitions[i].PoseTransformOffset;
			PoseOverride.localEulerAngles = ManagerSingleton<GM>.Instance.ControllerDefinitions[i].PoseTransformRotOffset;
			PointingTransform.localPosition = ManagerSingleton<GM>.Instance.ControllerDefinitions[i].InteractionSphereOffset;
			Display_InteractionSphere.transform.localPosition = ManagerSingleton<GM>.Instance.ControllerDefinitions[i].InteractionSphereOffset;
			GetComponent<SphereCollider>().center = ManagerSingleton<GM>.Instance.ControllerDefinitions[i].InteractionSphereOffset;
		}

		public void ForceTubeKick(byte duration)
		{
			ForceTubeVRInterface.Kick(duration);
		}

		public void ForceTubeRumble(byte intensity, float duration)
		{
			ForceTubeVRInterface.Rumble(intensity, duration);
		}

		public void Buzz(FVRHapticBuzzProfile buzz)
		{
			if (GM.Options.ControlOptions.HapticsState != ControlOptions.HapticsMode.Disabled && !IsInDemoMode)
			{
				m_curBuzz = buzz;
				m_buzztime = 0f;
				m_isBuzzing = true;
			}
		}

		private void HapticBuzzUpdate()
		{
			timeTillNextPulse -= Time.fixedDeltaTime;
			if (frequency == 0f)
			{
				frequency = 0f;
				oldFrequency = 0f;
				return;
			}
			if (oldFrequency == 0f)
			{
				timeTillNextPulse = PeriodForFreq(frequency);
				oldFrequency = frequency;
			}
			float num = PeriodForFreq(frequency);
			frequencyDiffPeriod = num - PeriodForFreq(oldFrequency);
			timeTillNextPulse += frequencyDiffPeriod;
			if (timeTillNextPulse <= 0f)
			{
				if (m_isBuzzing && m_curBuzz != null)
				{
					if (m_buzztime < m_curBuzz.BuzzLength)
					{
						m_buzztime += Time.deltaTime;
						amplitude = m_curBuzz.BuzzCurve.Evaluate(m_buzztime / m_curBuzz.BuzzLength) * m_curBuzz.AmpMult;
						frequency = m_curBuzz.Freq;
						if (amplitude > 0.01f)
						{
							Vibration.Execute(0f, Time.fixedDeltaTime, frequency, amplitude, HandSource);
						}
					}
					else
					{
						m_isBuzzing = false;
					}
				}
				else
				{
					m_isBuzzing = false;
				}
				timeTillNextPulse = num;
			}
			oldFrequency = frequency;
		}

		private float PeriodForFreq(float frequency)
		{
			return (frequency == 0f) ? float.PositiveInfinity : (1f / frequency);
		}

		private float FrequencyForTime(float time)
		{
			return 1f / time;
		}

		public void RetrieveObject(FVRPhysicalObject obj)
		{
			CurrentInteractable = obj;
			if (obj.PoseOverride != null)
			{
				AlignChild(obj.transform, obj.PoseOverride, base.transform);
			}
			else
			{
				obj.transform.position = base.transform.position;
				obj.transform.rotation = base.transform.rotation;
			}
			CurrentInteractable.BeginInteraction(this);
			m_state = HandState.GripInteracting;
		}

		public void EnableWristMenu(FVRWristMenu menu)
		{
			m_wristmenu = menu;
			m_isWristMenuActive = true;
		}

		public void DisableWristMenu()
		{
			m_wristmenu = null;
			m_isWristMenuActive = false;
		}

		public Vector3 GetThrowLinearVelWorld()
		{
			return WholeRig.TransformDirection(PhysTracker.Velocity) + GM.CurrentMovementManager.GetLastWorldDir() * GM.CurrentMovementManager.GetTopSpeedInLastSecond();
		}

		public Vector3 GetThrowAngularVelWorld()
		{
			return WholeRig.TransformDirection(PhysTracker.AngularVelocity);
		}

		public void PollInput()
		{
			if (GM.Options.ControlOptions.CCM == ControlOptions.CoreControlMode.Standard)
			{
				IsInStreamlinedMode = false;
			}
			else
			{
				IsInStreamlinedMode = true;
			}
			PhysTracker.Update(base.transform.localPosition, base.transform.localRotation, Time.smoothDeltaTime);
			Input.TriggerUp = Trigger_Button.GetStateUp(HandSource);
			Input.TriggerDown = Trigger_Button.GetStateDown(HandSource);
			Input.TriggerPressed = Trigger_Button.GetState(HandSource);
			Input.TriggerFloat = Trigger_Axis.GetAxis(HandSource);
			Input.TriggerTouchUp = Trigger_Touch.GetStateUp(HandSource);
			Input.TriggerTouchDown = Trigger_Touch.GetStateDown(HandSource);
			Input.TriggerTouched = Trigger_Touch.GetState(HandSource);
			Input.GripUp = Grip_Button.GetStateUp(HandSource);
			Input.GripDown = Grip_Button.GetStateDown(HandSource);
			Input.GripPressed = Grip_Button.GetState(HandSource);
			Input.GripTouchUp = Grip_Touch.GetStateUp(HandSource);
			Input.GripTouchDown = Grip_Touch.GetStateDown(HandSource);
			Input.GripTouched = Grip_Touch.GetState(HandSource);
			Input.TouchpadUp = Primary2Axis_Button.GetStateUp(HandSource);
			Input.TouchpadDown = Primary2Axis_Button.GetStateDown(HandSource);
			Input.TouchpadPressed = Primary2Axis_Button.GetState(HandSource);
			Input.TouchpadTouchUp = Primary2Axis_Touch.GetStateUp(HandSource);
			Input.TouchpadTouchDown = Primary2Axis_Touch.GetStateDown(HandSource);
			Input.TouchpadTouched = Primary2Axis_Touch.GetState(HandSource);
			Input.TouchpadAxes = Primary2Axis_Axes.GetAxis(HandSource);
			Input.TouchpadNorthDown = false;
			Input.TouchpadSouthDown = false;
			Input.TouchpadWestDown = false;
			Input.TouchpadEastDown = false;
			Input.TouchpadNorthUp = false;
			Input.TouchpadSouthUp = false;
			Input.TouchpadWestUp = false;
			Input.TouchpadEastUp = false;
			if (Input.TouchpadAxes.magnitude < 0.5f)
			{
				if (!Input.TouchpadCenterPressed)
				{
					Input.TouchpadCenterDown = true;
				}
				if (Input.TouchpadNorthPressed)
				{
					Input.TouchpadNorthUp = true;
				}
				if (Input.TouchpadSouthPressed)
				{
					Input.TouchpadSouthUp = true;
				}
				if (Input.TouchpadWestPressed)
				{
					Input.TouchpadWestUp = true;
				}
				if (Input.TouchpadEastPressed)
				{
					Input.TouchpadEastUp = true;
				}
				Input.TouchpadNorthPressed = false;
				Input.TouchpadSouthPressed = false;
				Input.TouchpadWestPressed = false;
				Input.TouchpadEastPressed = false;
				Input.TouchpadCenterPressed = true;
			}
			else
			{
				if (Input.TouchpadCenterPressed)
				{
					Input.TouchpadCenterUp = true;
				}
				Input.TouchpadCenterPressed = false;
				if (Vector2.Angle(Input.TouchpadAxes, Vector2.up) <= 45f)
				{
					if (!Input.TouchpadNorthPressed)
					{
						Input.TouchpadNorthDown = true;
					}
					Input.TouchpadNorthPressed = true;
					Input.TouchpadSouthPressed = false;
					Input.TouchpadWestPressed = false;
					Input.TouchpadEastPressed = false;
				}
				else if (Vector2.Angle(Input.TouchpadAxes, Vector2.down) <= 45f)
				{
					if (!Input.TouchpadSouthPressed)
					{
						Input.TouchpadSouthDown = true;
					}
					Input.TouchpadSouthPressed = true;
					Input.TouchpadNorthPressed = false;
					Input.TouchpadWestPressed = false;
					Input.TouchpadEastPressed = false;
				}
				else if (Vector2.Angle(Input.TouchpadAxes, Vector2.left) <= 45f)
				{
					if (!Input.TouchpadWestPressed)
					{
						Input.TouchpadWestDown = true;
					}
					Input.TouchpadWestPressed = true;
					Input.TouchpadNorthPressed = false;
					Input.TouchpadSouthPressed = false;
					Input.TouchpadEastPressed = false;
				}
				else if (Vector2.Angle(Input.TouchpadAxes, Vector2.right) <= 45f)
				{
					if (!Input.TouchpadEastPressed)
					{
						Input.TouchpadEastDown = true;
					}
					Input.TouchpadEastPressed = true;
					Input.TouchpadNorthPressed = false;
					Input.TouchpadSouthPressed = false;
					Input.TouchpadWestPressed = false;
				}
			}
			Input.BYButtonUp = B_Button.GetStateUp(HandSource);
			Input.BYButtonDown = B_Button.GetStateDown(HandSource);
			Input.BYButtonPressed = B_Button.GetState(HandSource);
			Input.AXButtonUp = A_Button.GetStateUp(HandSource);
			Input.AXButtonDown = A_Button.GetStateDown(HandSource);
			Input.AXButtonPressed = A_Button.GetState(HandSource);
			Input.Secondary2AxisInputUp = Secondary2Axis_Button.GetStateUp(HandSource);
			Input.Secondary2AxisInputDown = Secondary2Axis_Button.GetStateDown(HandSource);
			Input.Secondary2AxisInputPressed = Secondary2Axis_Button.GetState(HandSource);
			Input.Secondary2AxisInputTouchUp = Secondary2Axis_Touch.GetStateUp(HandSource);
			Input.Secondary2AxisInputTouchDown = Secondary2Axis_Touch.GetStateDown(HandSource);
			Input.Secondary2AxisInputTouched = Secondary2Axis_Touch.GetState(HandSource);
			Input.Secondary2AxisInputAxes = Secondary2Axis_Axes.GetAxis(HandSource);
			Input.Secondary2AxisNorthDown = false;
			Input.Secondary2AxisSouthDown = false;
			Input.Secondary2AxisWestDown = false;
			Input.Secondary2AxisEastDown = false;
			Input.Secondary2AxisNorthUp = false;
			Input.Secondary2AxisSouthUp = false;
			Input.Secondary2AxisWestUp = false;
			Input.Secondary2AxisEastUp = false;
			if (Input.Secondary2AxisInputAxes.magnitude < 0.5f)
			{
				if (!Input.Secondary2AxisCenterPressed)
				{
					Input.Secondary2AxisCenterDown = true;
				}
				if (Input.Secondary2AxisNorthPressed)
				{
					Input.Secondary2AxisNorthUp = true;
				}
				if (Input.Secondary2AxisSouthPressed)
				{
					Input.Secondary2AxisSouthUp = true;
				}
				if (Input.Secondary2AxisWestPressed)
				{
					Input.Secondary2AxisWestUp = true;
				}
				if (Input.Secondary2AxisEastPressed)
				{
					Input.Secondary2AxisEastUp = true;
				}
				Input.Secondary2AxisNorthPressed = false;
				Input.Secondary2AxisSouthPressed = false;
				Input.Secondary2AxisWestPressed = false;
				Input.Secondary2AxisEastPressed = false;
				Input.Secondary2AxisCenterPressed = true;
			}
			else
			{
				if (Input.Secondary2AxisCenterPressed)
				{
					Input.Secondary2AxisCenterUp = true;
				}
				Input.Secondary2AxisCenterPressed = false;
				if (Vector2.Angle(Input.Secondary2AxisInputAxes, Vector2.up) <= 45f)
				{
					if (!Input.Secondary2AxisNorthPressed)
					{
						Input.Secondary2AxisNorthDown = true;
					}
					Input.Secondary2AxisNorthPressed = true;
					Input.Secondary2AxisSouthPressed = false;
					Input.Secondary2AxisWestPressed = false;
					Input.Secondary2AxisEastPressed = false;
				}
				else if (Vector2.Angle(Input.Secondary2AxisInputAxes, Vector2.down) <= 45f)
				{
					if (!Input.Secondary2AxisSouthPressed)
					{
						Input.Secondary2AxisSouthDown = true;
					}
					Input.Secondary2AxisSouthPressed = true;
					Input.Secondary2AxisNorthPressed = false;
					Input.Secondary2AxisWestPressed = false;
					Input.Secondary2AxisEastPressed = false;
				}
				else if (Vector2.Angle(Input.Secondary2AxisInputAxes, Vector2.left) <= 45f)
				{
					if (!Input.Secondary2AxisWestPressed)
					{
						Input.Secondary2AxisWestDown = true;
					}
					Input.Secondary2AxisWestPressed = true;
					Input.Secondary2AxisNorthPressed = false;
					Input.Secondary2AxisSouthPressed = false;
					Input.Secondary2AxisEastPressed = false;
				}
				else if (Vector2.Angle(Input.Secondary2AxisInputAxes, Vector2.right) <= 45f)
				{
					if (!Input.Secondary2AxisEastPressed)
					{
						Input.Secondary2AxisEastDown = true;
					}
					Input.Secondary2AxisEastPressed = true;
					Input.Secondary2AxisNorthPressed = false;
					Input.Secondary2AxisSouthPressed = false;
					Input.Secondary2AxisWestPressed = false;
				}
			}
			Input.FingerCurl_Thumb = Skeleton.fingerCurls[0];
			Input.FingerCurl_Index = Skeleton.fingerCurls[1];
			Input.FingerCurl_Middle = Skeleton.fingerCurls[2];
			Input.FingerCurl_Ring = Skeleton.fingerCurls[3];
			Input.FingerCurl_Pinky = Skeleton.fingerCurls[4];
			Input.Pos = PoseOverride.position;
			Input.Rot = PoseOverride.rotation;
			Input.UpdateEuroFilter();
			float num = Mathf.Min(Vector3.Distance(Input.Pos, Input.OneEuroPosition) / 0.05f, 1f);
			num *= num;
			Input.FilteredPos = (1f - num) * Input.OneEuroPosition + num * Input.Pos;
			float num2 = Mathf.Min(Quaternion.Angle(Input.Rot, Input.OneEuroRotation) / 3f, 1f);
			num2 *= num2;
			Input.FilteredRot = Quaternion.Slerp(Input.OneEuroRotation, Input.Rot, num2);
			float num3 = Mathf.Min(Vector3.Distance(Input.Hand.PalmTransform.position, Input.OneEuroPalmPosition) / 0.05f, 1f);
			num3 *= num3;
			Input.FilteredPalmPos = (1f - num3) * Input.OneEuroPalmPosition + num3 * Input.Hand.PalmTransform.position;
			float num4 = Mathf.Min(Quaternion.Angle(Input.Hand.PalmTransform.rotation, Input.OneEuroPalmRotation) / 3f, 1f);
			num4 *= num4;
			Input.FilteredPalmRot = Quaternion.Slerp(Input.OneEuroPalmRotation, Input.Hand.PalmTransform.rotation, num4);
			Input.Forward = PoseOverride.forward;
			Input.Up = PoseOverride.up;
			Input.Right = PoseOverride.right;
			float num5 = Mathf.Min(Vector3.Distance(Input.FilteredForward, Input.Forward) / 0.015f, 1f);
			Input.FilteredForward = (1f - num5) * Input.FilteredForward + num5 * Input.Forward;
			float num6 = Mathf.Min(Vector3.Distance(Input.FilteredUp, Input.Up) / 0.015f, 1f);
			Input.FilteredUp = (1f - num6) * Input.FilteredUp + num6 * Input.Up;
			float num7 = Mathf.Min(Vector3.Distance(Input.FilteredRight, Input.Right) / 0.015f, 1f);
			Input.FilteredRight = (1f - num7) * Input.FilteredRight + num7 * Input.Right;
			Input.VelLinearLocal = Pose.GetVelocity(HandSource);
			Input.VelAngularLocal = Pose.GetAngularVelocity(HandSource);
			Input.VelLinearWorld = WholeRig.TransformDirection(Input.VelLinearLocal);
			Input.VelAngularWorld = WholeRig.TransformDirection(Input.VelAngularLocal);
			ControlMode controlMode = CMode;
			if (GM.Options.ControlOptions.GripButtonToHoldOverride == ControlOptions.GripButtonToHoldOverrideMode.OculusOverride)
			{
				controlMode = ControlMode.Oculus;
			}
			else if (GM.Options.ControlOptions.GripButtonToHoldOverride == ControlOptions.GripButtonToHoldOverrideMode.ViveOverride)
			{
				controlMode = ControlMode.Vive;
			}
			switch (controlMode)
			{
			case ControlMode.Index:
			{
				bool flag = false;
				float num8 = (Input.FingerCurl_Middle * 2f + Input.FingerCurl_Ring * 1f) / 3f;
				float num9 = num8 - Input.LastCurlAverage;
				if (Input.TriggerPressed)
				{
					flag = true;
				}
				else if (Input.IsGrabbing && (double)num8 >= 0.5)
				{
					flag = true;
				}
				else if (Input.IsGrabbing && Input.TriggerTouched)
				{
					flag = true;
				}
				else if (Input.IsGrabbing && Input.GripTouched)
				{
					flag = true;
				}
				else if (Input.GripPressed)
				{
					flag = true;
				}
				if (Input.IsGrabbing && !Input.TriggerPressed && !Input.GripPressed && ((num9 < -0.3f && num8 < 0.7f) || num9 < -0.5f))
				{
					flag = false;
				}
				if (Input.IsGrabbing && !flag)
				{
					Input.IsGrabUp = true;
				}
				else
				{
					Input.IsGrabUp = false;
				}
				if ((!Input.IsGrabbing && flag) || Input.TriggerDown)
				{
					Input.IsGrabDown = true;
				}
				else
				{
					Input.IsGrabDown = false;
				}
				Input.IsGrabbing = flag;
				Input.LastCurlAverage = num8;
				break;
			}
			case ControlMode.Vive:
			case ControlMode.WMR:
				Input.IsGrabUp = Input.TriggerUp;
				Input.IsGrabDown = Input.TriggerDown;
				Input.IsGrabbing = Input.TriggerPressed;
				break;
			case ControlMode.Oculus:
				Input.IsGrabUp = Input.GripUp;
				Input.IsGrabDown = Input.GripDown;
				Input.IsGrabbing = Input.GripPressed;
				break;
			}
			if (m_timeSinceLastGripButtonDown < 5f)
			{
				m_timeSinceLastGripButtonDown += Time.deltaTime;
			}
			Input.LastPalmPos2 = Input.LastPalmPos1;
			Input.LastPalmPos1 = PalmTransform.position;
		}

		private void FlushTouchpadData()
		{
			Input.TouchpadUp = false;
			Input.TouchpadDown = false;
			Input.TouchpadPressed = false;
			Input.TouchpadTouchUp = false;
			Input.TouchpadTouchDown = false;
			Input.TouchpadTouched = false;
			Input.TouchpadAxes = Vector2.zero;
		}

		private void Update()
		{
			if (m_initState == HandInitializationState.Uninitialized)
			{
				return;
			}
			if (m_selectedObj != null && m_selectedObj.IsHeld)
			{
				m_selectedObj = null;
				m_reset = 0f;
				m_isObjectInTransit = false;
			}
			if (m_reset >= 0f && m_isObjectInTransit)
			{
				if (m_selectedObj != null && Vector3.Distance(m_selectedObj.transform.position, base.transform.position) < 0.4f)
				{
					Vector3 b = base.transform.position - m_selectedObj.transform.position;
					Vector3 vector = Vector3.Lerp(m_selectedObj.RootRigidbody.velocity, b, Time.deltaTime * 2f);
					m_selectedObj.RootRigidbody.velocity = Vector3.ClampMagnitude(vector, m_selectedObj.RootRigidbody.velocity.magnitude);
					m_selectedObj.RootRigidbody.velocity = vector;
					m_selectedObj.RootRigidbody.drag = 1f;
					m_selectedObj.RootRigidbody.angularDrag = 8f;
					m_reset -= Time.deltaTime * 0.4f;
				}
				else
				{
					m_reset -= Time.deltaTime;
				}
				if (m_reset <= 0f)
				{
					m_isObjectInTransit = false;
					if (m_selectedObj != null)
					{
						m_selectedObj.RecoverDrag();
						m_selectedObj = null;
					}
				}
			}
			HapticBuzzUpdate();
			TestQuickBeltDistances();
			PollInput();
			if (m_hasOverrider && m_overrider != null)
			{
				m_overrider.Process(ref Input);
			}
			else
			{
				m_hasOverrider = false;
			}
			if (!(m_currentInteractable != null) || Input.TriggerPressed)
			{
			}
			if (ClosestPossibleInteractable == null)
			{
				if (m_touchSphereMatInteractable)
				{
					m_touchSphereMatInteractable = false;
					TouchSphere.material = TouchSphereMat_NoInteractable;
				}
				if (m_touchSphereMatInteractablePalm)
				{
					m_touchSphereMatInteractablePalm = false;
					TouchSphere_Palm.material = TouchSphereMat_NoInteractable;
				}
			}
			else if (!m_touchSphereMatInteractable && !m_isClosestInteractableInPalm)
			{
				m_touchSphereMatInteractable = true;
				TouchSphere.material = TouchSpheteMat_Interactable;
				m_touchSphereMatInteractablePalm = false;
				TouchSphere_Palm.material = TouchSphereMat_NoInteractable;
			}
			else if (!m_touchSphereMatInteractablePalm && m_isClosestInteractableInPalm)
			{
				m_touchSphereMatInteractablePalm = true;
				TouchSphere_Palm.material = TouchSpheteMat_Interactable;
				m_touchSphereMatInteractable = false;
				TouchSphere.material = TouchSphereMat_NoInteractable;
			}
			float num = 1f / GM.CurrentPlayerBody.transform.localScale.x;
			if (m_state == HandState.Empty && !Input.BYButtonPressed && !Input.TouchpadPressed && ClosestPossibleInteractable == null && CurrentHoveredQuickbeltSlot == null && CurrentInteractable == null && !m_isWristMenuActive)
			{
				if (Physics.Raycast(PointingTransform.position, PointingTransform.forward, out m_pointingHit, GM.CurrentSceneSettings.MaxPointingDistance, PointingLayerMask, QueryTriggerInteraction.Collide) && (bool)m_pointingHit.collider.gameObject.GetComponent<FVRPointable>())
				{
					FVRPointable component = m_pointingHit.collider.gameObject.GetComponent<FVRPointable>();
					if (m_pointingHit.distance <= component.MaxPointingRange)
					{
						CurrentPointable = component;
						PointingLaser.position = PointingTransform.position;
						PointingLaser.rotation = PointingTransform.rotation;
						PointingLaser.localScale = new Vector3(0.002f, 0.002f, m_pointingHit.distance) * num;
					}
					else
					{
						CurrentPointable = null;
					}
				}
				else
				{
					CurrentPointable = null;
				}
			}
			else
			{
				CurrentPointable = null;
			}
			MovementManager.UpdateMovementWithHand(this);
			if (MovementManager.ShouldFlushTouchpad(this))
			{
				FlushTouchpadData();
			}
			bool flag = false;
			bool flag2 = false;
			if (IsInStreamlinedMode)
			{
				flag = Input.BYButtonDown;
				flag2 = Input.BYButtonPressed;
			}
			else
			{
				flag = Input.TouchpadDown;
				flag2 = Input.TouchpadPressed;
			}
			if (m_state == HandState.Empty && CurrentHoveredQuickbeltSlot == null)
			{
				if (flag2)
				{
					if (!GrabLaser.gameObject.activeSelf)
					{
						GrabLaser.gameObject.SetActive(value: true);
					}
					bool flag3 = false;
					FVRPhysicalObject fVRPhysicalObject = null;
					if (Physics.Raycast(PointingTransform.position, PointingTransform.forward, out m_grabHit, 3f, GrabLaserMask, QueryTriggerInteraction.Collide))
					{
						if (m_grabHit.collider.attachedRigidbody != null && (bool)m_grabHit.collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>())
						{
							fVRPhysicalObject = m_grabHit.collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
							if (fVRPhysicalObject != null && !fVRPhysicalObject.IsHeld && fVRPhysicalObject.IsDistantGrabbable())
							{
								flag3 = true;
							}
						}
						GrabLaser.localScale = new Vector3(0.004f, 0.004f, m_grabHit.distance) * num;
					}
					else
					{
						GrabLaser.localScale = new Vector3(0.004f, 0.004f, 3f) * num;
					}
					GrabLaser.position = PointingTransform.position;
					GrabLaser.rotation = PointingTransform.rotation;
					if (flag3)
					{
						if (!BlueLaser.activeSelf)
						{
							BlueLaser.SetActive(value: true);
						}
						if (RedLaser.activeSelf)
						{
							RedLaser.SetActive(value: false);
						}
						if (Input.IsGrabDown && fVRPhysicalObject != null)
						{
							RetrieveObject(fVRPhysicalObject);
							if (GrabLaser.gameObject.activeSelf)
							{
								GrabLaser.gameObject.SetActive(value: false);
							}
						}
					}
					else
					{
						if (BlueLaser.activeSelf)
						{
							BlueLaser.SetActive(value: false);
						}
						if (!RedLaser.activeSelf)
						{
							RedLaser.SetActive(value: true);
						}
					}
				}
				else if (GrabLaser.gameObject.activeSelf)
				{
					GrabLaser.gameObject.SetActive(value: false);
				}
			}
			else if (GrabLaser.gameObject.activeSelf)
			{
				GrabLaser.gameObject.SetActive(value: false);
			}
			if (Mode == HandMode.Neutral && m_state == HandState.Empty && flag)
			{
				bool isSpawnLockingEnabled = GM.CurrentSceneSettings.IsSpawnLockingEnabled;
				if (ClosestPossibleInteractable != null && ClosestPossibleInteractable is FVRPhysicalObject)
				{
					FVRPhysicalObject fVRPhysicalObject2 = ClosestPossibleInteractable as FVRPhysicalObject;
					if (((fVRPhysicalObject2.SpawnLockable && isSpawnLockingEnabled) || fVRPhysicalObject2.Harnessable) && fVRPhysicalObject2.QuickbeltSlot != null)
					{
						fVRPhysicalObject2.ToggleQuickbeltState();
					}
				}
				else if (CurrentHoveredQuickbeltSlot != null && CurrentHoveredQuickbeltSlot.HeldObject != null)
				{
					FVRPhysicalObject fVRPhysicalObject3 = CurrentHoveredQuickbeltSlot.HeldObject as FVRPhysicalObject;
					if ((fVRPhysicalObject3.SpawnLockable && isSpawnLockingEnabled) || fVRPhysicalObject3.Harnessable)
					{
						fVRPhysicalObject3.ToggleQuickbeltState();
					}
				}
			}
			UpdateGrabityDisplay();
			if (Mode == HandMode.Neutral)
			{
				if (m_state == HandState.Empty)
				{
					bool flag4 = false;
					if (Input.IsGrabDown)
					{
						if (CurrentHoveredQuickbeltSlot != null && CurrentHoveredQuickbeltSlot.CurObject != null)
						{
							CurrentInteractable = CurrentHoveredQuickbeltSlot.CurObject;
							m_state = HandState.GripInteracting;
							CurrentInteractable.BeginInteraction(this);
							Buzz(Buzzer.Buzz_BeginInteraction);
							flag4 = true;
						}
						else if (ClosestPossibleInteractable != null && !ClosestPossibleInteractable.IsSimpleInteract)
						{
							CurrentInteractable = ClosestPossibleInteractable;
							m_state = HandState.GripInteracting;
							CurrentInteractable.BeginInteraction(this);
							Buzz(Buzzer.Buzz_BeginInteraction);
							flag4 = true;
						}
					}
					bool flag5 = false;
					if (!flag4 && Input.TriggerDown && (!(CurrentHoveredQuickbeltSlot != null) || !(CurrentHoveredQuickbeltSlot.CurObject != null)) && ClosestPossibleInteractable != null && ClosestPossibleInteractable.IsSimpleInteract)
					{
						ClosestPossibleInteractable.SimpleInteraction(this);
						flag5 = true;
					}
					if (GM.Options.ControlOptions.WIPGrabbityState == ControlOptions.WIPGrabbity.Enabled && !flag4 && !flag5)
					{
						if (m_selectedObj == null)
						{
							CastToFindHover();
						}
						else
						{
							SetGrabbityHovered(null);
						}
						bool flag6 = false;
						bool flag7 = false;
						if (GM.Options.ControlOptions.WIPGrabbityButtonState == ControlOptions.WIPGrabbityButton.Grab)
						{
							flag6 = Input.GripDown;
							flag7 = Input.GripUp;
						}
						else
						{
							flag6 = Input.TriggerDown;
							flag7 = Input.TriggerUp;
						}
						if (flag6 && m_grabityHoveredObject != null && m_selectedObj == null)
						{
							CastToGrab();
						}
						if (flag7 && !m_isObjectInTransit)
						{
							m_selectedObj = null;
						}
						if (m_selectedObj != null && !m_isObjectInTransit)
						{
							float num2 = 3.5f;
							if (Mathf.Abs(Input.VelAngularLocal.x) > num2 || Mathf.Abs(Input.VelAngularLocal.y) > num2)
							{
								BeginFlick(m_selectedObj);
							}
						}
					}
					else
					{
						SetGrabbityHovered(null);
					}
					if (GM.Options.ControlOptions.WIPGrabbityState == ControlOptions.WIPGrabbity.Enabled && !flag4 && !flag5 && Input.IsGrabDown && m_isObjectInTransit && m_selectedObj != null)
					{
						float num3 = Vector3.Distance(base.transform.position, m_selectedObj.transform.position);
						if (num3 < 0.5f)
						{
							if (m_selectedObj.UseGripRotInterp)
							{
								CurrentInteractable = m_selectedObj;
								CurrentInteractable.BeginInteraction(this);
								m_state = HandState.GripInteracting;
							}
							else
							{
								RetrieveObject(m_selectedObj);
							}
							m_selectedObj = null;
							m_isObjectInTransit = false;
							SetGrabbityHovered(null);
						}
					}
				}
				else if (m_state == HandState.GripInteracting)
				{
					SetGrabbityHovered(null);
					bool flag8 = false;
					if (CurrentInteractable != null)
					{
						ControlMode controlMode = CMode;
						if (GM.Options.ControlOptions.GripButtonToHoldOverride == ControlOptions.GripButtonToHoldOverrideMode.OculusOverride)
						{
							controlMode = ControlMode.Oculus;
						}
						else if (GM.Options.ControlOptions.GripButtonToHoldOverride == ControlOptions.GripButtonToHoldOverrideMode.ViveOverride)
						{
							controlMode = ControlMode.Vive;
						}
						if (controlMode == ControlMode.Vive || controlMode == ControlMode.WMR)
						{
							if (CurrentInteractable.ControlType == FVRInteractionControlType.GrabHold)
							{
								if (Input.TriggerUp)
								{
									flag8 = true;
								}
							}
							else if (CurrentInteractable.ControlType == FVRInteractionControlType.GrabToggle)
							{
								switch (GM.Options.ControlOptions.GripButtonDropStyle)
								{
								case ControlOptions.ButtonControlStyle.Instant:
									if (!Input.TriggerPressed && Input.GripDown)
									{
										flag8 = true;
									}
									break;
								case ControlOptions.ButtonControlStyle.Hold1Second:
									if (!Input.TriggerPressed && m_timeGripButtonHasBeenHeld > 1f)
									{
										flag8 = true;
									}
									break;
								case ControlOptions.ButtonControlStyle.DoubleClick:
									if (!Input.TriggerPressed && Input.GripDown && m_timeSinceLastGripButtonDown > 0.05f && m_timeSinceLastGripButtonDown < 0.4f)
									{
										flag8 = true;
									}
									break;
								}
							}
						}
						else if (Input.IsGrabUp)
						{
							flag8 = true;
						}
						if (flag8)
						{
							if (CurrentInteractable is FVRPhysicalObject && ((FVRPhysicalObject)CurrentInteractable).QuickbeltSlot == null && !((FVRPhysicalObject)CurrentInteractable).IsPivotLocked && CurrentHoveredQuickbeltSlot != null && CurrentHoveredQuickbeltSlot.HeldObject == null && ((FVRPhysicalObject)CurrentInteractable).QBSlotType == CurrentHoveredQuickbeltSlot.Type && CurrentHoveredQuickbeltSlot.SizeLimit >= ((FVRPhysicalObject)CurrentInteractable).Size)
							{
								((FVRPhysicalObject)CurrentInteractable).EndInteractionIntoInventorySlot(this, CurrentHoveredQuickbeltSlot);
							}
							else
							{
								CurrentInteractable.EndInteraction(this);
							}
							CurrentInteractable = null;
							m_state = HandState.Empty;
						}
						else
						{
							CurrentInteractable.UpdateInteraction(this);
						}
					}
					else
					{
						m_state = HandState.Empty;
					}
				}
			}
			if (Input.GripPressed)
			{
				m_timeSinceLastGripButtonDown = 0f;
				m_timeGripButtonHasBeenHeld += Time.deltaTime;
			}
			else
			{
				m_timeGripButtonHasBeenHeld = 0f;
			}
			m_canMadeGrabReleaseSoundThisFrame = true;
		}

		private void SetGrabbityHovered(FVRPhysicalObject o)
		{
			if (o == null)
			{
				m_grabityHoveredObject = o;
			}
			else if (m_grabityHoveredObject != o)
			{
				m_grabityHoveredObject = o;
				SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_Grabbity_Hover, base.transform.position);
			}
		}

		private void UpdateGrabityDisplay()
		{
			if (m_grabityHoveredObject != null && (m_grabityHoveredObject.IsHeld || !m_grabityHoveredObject.IsDistantGrabbable() || m_grabityHoveredObject.QuickbeltSlot != null))
			{
				m_grabityHoveredObject = null;
			}
			if (m_selectedObj != null && (m_selectedObj.IsHeld || !m_selectedObj.IsDistantGrabbable() || m_selectedObj.QuickbeltSlot != null))
			{
				m_selectedObj = null;
			}
			if (m_grabityHoveredObject != null)
			{
				Grabbity_HoverSphere.gameObject.SetActive(value: true);
				Grabbity_HoverSphere.position = m_grabityHoveredObject.transform.position;
			}
			else
			{
				Grabbity_HoverSphere.gameObject.SetActive(value: false);
			}
			if (m_selectedObj != null)
			{
				Grabbity_GrabSphere.gameObject.SetActive(value: true);
				Grabbity_GrabSphere.position = m_selectedObj.transform.position;
			}
			else
			{
				Grabbity_GrabSphere.gameObject.SetActive(value: false);
			}
		}

		private void CastToFindHover()
		{
			bool flag = false;
			if (Physics.Raycast(PointingTransform.position, Input.FilteredForward, out m_grabbity_hit, 10f, LM_Grabbity_Beam, QueryTriggerInteraction.Collide) && m_grabbity_hit.collider.attachedRigidbody != null)
			{
				FVRPhysicalObject component = m_grabbity_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
				if (component != null && !component.IsHeld && component.IsDistantGrabbable() && component.QuickbeltSlot == null && !component.RootRigidbody.isKinematic && !Physics.Linecast(PointingTransform.position, PointingTransform.position + PointingTransform.forward * m_grabbity_hit.distance, LM_Grabbity_Block, QueryTriggerInteraction.Ignore))
				{
					SetGrabbityHovered(component);
					flag = true;
				}
			}
			if (!flag && Physics.SphereCast(PointingTransform.position, 0.2f, PointingTransform.forward, out m_grabbity_hit, 10f, LM_Grabbity_BeamTrigger, QueryTriggerInteraction.Collide) && m_grabbity_hit.collider.attachedRigidbody != null)
			{
				FVRPhysicalObject component2 = m_grabbity_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
				if (component2 != null && !component2.IsHeld && component2.IsDistantGrabbable() && component2.QuickbeltSlot == null && !component2.RootRigidbody.isKinematic && !Physics.Linecast(PointingTransform.position, PointingTransform.position + PointingTransform.forward * m_grabbity_hit.distance, LM_Grabbity_Block, QueryTriggerInteraction.Ignore))
				{
					SetGrabbityHovered(component2);
					flag = true;
				}
			}
			if (!flag && Physics.SphereCast(PointingTransform.position, 0.2f, PointingTransform.forward, out m_grabbity_hit, 10f, LM_Grabbity_Beam, QueryTriggerInteraction.Collide) && m_grabbity_hit.collider.attachedRigidbody != null)
			{
				FVRPhysicalObject component3 = m_grabbity_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
				if (component3 != null && !component3.IsHeld && component3.IsDistantGrabbable() && component3.QuickbeltSlot == null && !component3.RootRigidbody.isKinematic && !Physics.Linecast(PointingTransform.position, PointingTransform.position + PointingTransform.forward * m_grabbity_hit.distance, LM_Grabbity_Block, QueryTriggerInteraction.Ignore))
				{
					SetGrabbityHovered(component3);
					flag = true;
				}
			}
			if (!flag)
			{
				SetGrabbityHovered(null);
			}
		}

		private void CastToGrab()
		{
			if (m_grabityHoveredObject != null)
			{
				if (m_selectedObj != null)
				{
					m_selectedObj.RecoverDrag();
				}
				m_selectedObj = m_grabityHoveredObject;
				SetGrabbityHovered(null);
				SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_Grabbity_Grab, base.transform.position);
				m_reset = 2f;
			}
		}

		private void BeginFlick(FVRPhysicalObject o)
		{
			Vector3 vector = base.transform.position - o.transform.position;
			float num = Mathf.Clamp(Vector3.Distance(base.transform.position, o.transform.position) * 1f, 0.1f, 10f);
			int num2 = 0;
			Vector3 velocity;
			if (Mathf.Abs(Physics.gravity.y) > 0.1f)
			{
				num2 = fts.solve_ballistic_arc(o.transform.position, num, base.transform.position, Mathf.Abs(Physics.gravity.y), out var s, out var s2);
				if (num2 < 1)
				{
					num = Mathf.Clamp(num * 1.5f, 2f, 15f);
					num2 = fts.solve_ballistic_arc(o.transform.position, num, base.transform.position, Mathf.Abs(Physics.gravity.y), out s, out s2);
				}
				if (num2 < 1)
				{
					num = Mathf.Clamp(num * 1.5f, 3f, 25f);
					num2 = fts.solve_ballistic_arc(o.transform.position, num, base.transform.position, Mathf.Abs(Physics.gravity.y), out s, out s2);
				}
				if (num2 < 1)
				{
					for (int i = 0; i < 10; i++)
					{
						num2 = fts.solve_ballistic_arc(o.transform.position, num, base.transform.position, Mathf.Abs(Physics.gravity.y), out s, out s2);
						if (num2 > 0)
						{
							break;
						}
						num += 1f;
					}
					velocity = s;
				}
				else
				{
					velocity = s;
				}
			}
			else
			{
				velocity = vector;
			}
			SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_Grabbity_Flick, base.transform.position);
			m_selectedObj.MP.DeJoint();
			m_selectedObj.RootRigidbody.drag = 0f;
			m_selectedObj.RootRigidbody.angularDrag = 0f;
			m_selectedObj.RootRigidbody.velocity = velocity;
			m_isObjectInTransit = true;
			m_reset = vector.magnitude / velocity.magnitude * 1.5f;
		}

		public void UpdateHandInput()
		{
		}

		public void EndInteractionIfHeld(FVRInteractiveObject inter)
		{
			if (inter == CurrentInteractable)
			{
				CurrentInteractable = null;
				ClosestPossibleInteractable = null;
			}
		}

		public void ForceSetInteractable(FVRInteractiveObject inter)
		{
			if (CurrentInteractable != null)
			{
				CurrentInteractable.EndInteraction(this);
			}
			CurrentInteractable = inter;
			if (inter == null)
			{
				CurrentInteractable = null;
				ClosestPossibleInteractable = null;
				m_state = HandState.Empty;
			}
			else
			{
				m_state = HandState.GripInteracting;
			}
		}

		public void TestCollider(Collider collider, bool isEnter, bool isPalm)
		{
			if (isEnter)
			{
				if (collider.gameObject.GetComponent<FVRInteractiveObject>() != null)
				{
					FVRInteractiveObject component = collider.gameObject.GetComponent<FVRInteractiveObject>();
					component.Poke(this);
				}
			}
			else
			{
				if (m_state != 0 || !(collider.gameObject.GetComponent<FVRInteractiveObject>() != null))
				{
					return;
				}
				FVRInteractiveObject component2 = collider.gameObject.GetComponent<FVRInteractiveObject>();
				if (!(component2 != null) || !component2.IsInteractable() || component2.IsSelectionRestricted())
				{
					return;
				}
				float num = Vector3.Distance(collider.transform.position, Display_InteractionSphere.transform.position);
				float num2 = Vector3.Distance(collider.transform.position, Display_InteractionSphere_Palm.transform.position);
				if (ClosestPossibleInteractable == null)
				{
					ClosestPossibleInteractable = component2;
					if (num < num2)
					{
						m_isClosestInteractableInPalm = false;
					}
					else
					{
						m_isClosestInteractableInPalm = true;
					}
				}
				else if (ClosestPossibleInteractable != component2)
				{
					float num3 = Vector3.Distance(ClosestPossibleInteractable.transform.position, Display_InteractionSphere.transform.position);
					float num4 = Vector3.Distance(ClosestPossibleInteractable.transform.position, Display_InteractionSphere_Palm.transform.position);
					bool flag = true;
					if (num < num2)
					{
						flag = false;
					}
					if (flag && num2 < num4 && m_isClosestInteractableInPalm)
					{
						m_isClosestInteractableInPalm = true;
						ClosestPossibleInteractable = component2;
					}
					else if (!flag && num < num3)
					{
						m_isClosestInteractableInPalm = false;
						ClosestPossibleInteractable = component2;
					}
				}
			}
		}

		public void HandTriggerExit(Collider collider, bool isPalm)
		{
			if (collider.gameObject.GetComponent<FVRInteractiveObject>() != null)
			{
				FVRInteractiveObject component = collider.GetComponent<FVRInteractiveObject>();
				if (ClosestPossibleInteractable == component)
				{
					ClosestPossibleInteractable = null;
					m_isClosestInteractableInPalm = false;
				}
			}
		}

		private void TestQuickBeltDistances()
		{
			if (CurrentHoveredQuickbeltSlot != null && !CurrentHoveredQuickbeltSlot.IsSelectable)
			{
				CurrentHoveredQuickbeltSlot = null;
			}
			if (CurrentHoveredQuickbeltSlotDirty != null && !CurrentHoveredQuickbeltSlotDirty.IsSelectable)
			{
				CurrentHoveredQuickbeltSlotDirty = null;
			}
			FVRQuickBeltSlot fVRQuickBeltSlot = null;
			Vector3 v = PoseOverride.position;
			if (CurrentInteractable != null)
			{
				v = ((!(CurrentInteractable.PoseOverride != null)) ? CurrentInteractable.transform.position : CurrentInteractable.PoseOverride.position);
			}
			for (int i = 0; i < GM.CurrentPlayerBody.QuickbeltSlots.Count; i++)
			{
				if (GM.CurrentPlayerBody.QuickbeltSlots[i].IsPointInsideMe(v))
				{
					fVRQuickBeltSlot = GM.CurrentPlayerBody.QuickbeltSlots[i];
					break;
				}
			}
			if (fVRQuickBeltSlot == null)
			{
				if (CurrentHoveredQuickbeltSlot != null)
				{
					CurrentHoveredQuickbeltSlot = null;
				}
				CurrentHoveredQuickbeltSlotDirty = null;
				return;
			}
			CurrentHoveredQuickbeltSlotDirty = fVRQuickBeltSlot;
			if (m_state == HandState.Empty)
			{
				if (fVRQuickBeltSlot.CurObject != null && !fVRQuickBeltSlot.CurObject.IsHeld)
				{
					CurrentHoveredQuickbeltSlot = fVRQuickBeltSlot;
				}
			}
			else if (m_state == HandState.GripInteracting && m_currentInteractable != null && m_currentInteractable is FVRPhysicalObject)
			{
				FVRPhysicalObject fVRPhysicalObject = (FVRPhysicalObject)m_currentInteractable;
				if (fVRQuickBeltSlot.CurObject == null && fVRQuickBeltSlot.SizeLimit >= fVRPhysicalObject.Size && fVRPhysicalObject.QBSlotType == fVRQuickBeltSlot.Type)
				{
					CurrentHoveredQuickbeltSlot = fVRQuickBeltSlot;
				}
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			TestCollider(collider, isEnter: true, isPalm: false);
		}

		private void OnTriggerStay(Collider collider)
		{
			TestCollider(collider, isEnter: false, isPalm: false);
		}

		private void OnTriggerExit(Collider collider)
		{
			HandTriggerExit(collider, isPalm: false);
		}

		public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
		{
			Quaternion result = default(Quaternion);
			result.w = Mathf.Sqrt(Mathf.Max(0f, 1f + m[0, 0] + m[1, 1] + m[2, 2])) / 2f;
			result.x = Mathf.Sqrt(Mathf.Max(0f, 1f + m[0, 0] - m[1, 1] - m[2, 2])) / 2f;
			result.y = Mathf.Sqrt(Mathf.Max(0f, 1f - m[0, 0] + m[1, 1] - m[2, 2])) / 2f;
			result.z = Mathf.Sqrt(Mathf.Max(0f, 1f - m[0, 0] - m[1, 1] + m[2, 2])) / 2f;
			result.x *= Mathf.Sign(result.x * (m[2, 1] - m[1, 2]));
			result.y *= Mathf.Sign(result.y * (m[0, 2] - m[2, 0]));
			result.z *= Mathf.Sign(result.z * (m[1, 0] - m[0, 1]));
			return result;
		}

		public static void AlignChild(Transform main, Transform child, Transform alignTo)
		{
			Matrix4x4 matrix4x = Matrix4x4.TRS(child.position, child.rotation, Vector3.one);
			Matrix4x4 matrix4x2 = Matrix4x4.TRS(alignTo.position, alignTo.rotation, Vector3.one);
			Matrix4x4 m = matrix4x2 * matrix4x.inverse * main.localToWorldMatrix;
			main.position = m.GetColumn(3);
			main.rotation = QuaternionFromMatrix(m);
		}
	}
}
