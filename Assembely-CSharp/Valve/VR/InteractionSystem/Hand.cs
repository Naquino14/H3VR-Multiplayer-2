using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class Hand : MonoBehaviour
	{
		[Flags]
		public enum AttachmentFlags
		{
			SnapOnAttach = 0x1,
			DetachOthers = 0x2,
			DetachFromOtherHand = 0x4,
			ParentToHand = 0x8,
			VelocityMovement = 0x10,
			TurnOnKinematic = 0x20,
			TurnOffGravity = 0x40,
			AllowSidegrade = 0x80
		}

		public struct AttachedObject
		{
			public GameObject attachedObject;

			public Interactable interactable;

			public Rigidbody attachedRigidbody;

			public CollisionDetectionMode collisionDetectionMode;

			public bool attachedRigidbodyWasKinematic;

			public bool attachedRigidbodyUsedGravity;

			public GameObject originalParent;

			public bool isParentedToHand;

			public GrabTypes grabbedWithType;

			public AttachmentFlags attachmentFlags;

			public Vector3 initialPositionalOffset;

			public Quaternion initialRotationalOffset;

			public Transform attachedOffsetTransform;

			public Transform handAttachmentPointTransform;

			public Vector3 easeSourcePosition;

			public Quaternion easeSourceRotation;

			public float attachTime;

			public bool HasAttachFlag(AttachmentFlags flag)
			{
				return (attachmentFlags & flag) == flag;
			}
		}

		public const AttachmentFlags defaultAttachmentFlags = AttachmentFlags.SnapOnAttach | AttachmentFlags.DetachOthers | AttachmentFlags.DetachFromOtherHand | AttachmentFlags.ParentToHand | AttachmentFlags.TurnOnKinematic;

		public Hand otherHand;

		public SteamVR_Input_Sources handType;

		public SteamVR_Behaviour_Pose trackedObject;

		public SteamVR_Action_Boolean grabPinchAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch");

		public SteamVR_Action_Boolean grabGripAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");

		public SteamVR_Action_Vibration hapticAction = SteamVR_Input.GetAction<SteamVR_Action_Vibration>("Haptic");

		public SteamVR_Action_Boolean uiInteractAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");

		public bool useHoverSphere = true;

		public Transform hoverSphereTransform;

		public float hoverSphereRadius = 0.05f;

		public LayerMask hoverLayerMask = -1;

		public float hoverUpdateInterval = 0.1f;

		public bool useControllerHoverComponent = true;

		public string controllerHoverComponent = "tip";

		public float controllerHoverRadius = 0.075f;

		public bool useFingerJointHover = true;

		public SteamVR_Skeleton_JointIndexEnum fingerJointHover = SteamVR_Skeleton_JointIndexEnum.indexTip;

		public float fingerJointHoverRadius = 0.025f;

		[Tooltip("A transform on the hand to center attached objects on")]
		public Transform objectAttachmentPoint;

		public Camera noSteamVRFallbackCamera;

		public float noSteamVRFallbackMaxDistanceNoItem = 10f;

		public float noSteamVRFallbackMaxDistanceWithItem = 0.5f;

		private float noSteamVRFallbackInteractorDistance = -1f;

		public GameObject renderModelPrefab;

		protected List<RenderModel> renderModels = new List<RenderModel>();

		protected RenderModel mainRenderModel;

		protected RenderModel hoverhighlightRenderModel;

		public bool showDebugText;

		public bool spewDebugText;

		public bool showDebugInteractables;

		private List<AttachedObject> attachedObjects = new List<AttachedObject>();

		private Interactable _hoveringInteractable;

		private TextMesh debugText;

		private int prevOverlappingColliders;

		private const int ColliderArraySize = 16;

		private Collider[] overlappingColliders;

		private Player playerInstance;

		private GameObject applicationLostFocusObject;

		private SteamVR_Events.Action inputFocusAction;

		protected const float MaxVelocityChange = 10f;

		protected const float VelocityMagic = 6000f;

		protected const float AngularVelocityMagic = 50f;

		protected const float MaxAngularVelocityChange = 20f;

		public ReadOnlyCollection<AttachedObject> AttachedObjects => attachedObjects.AsReadOnly();

		public bool hoverLocked
		{
			get;
			private set;
		}

		public bool isActive
		{
			get
			{
				if (trackedObject != null)
				{
					return trackedObject.isActive;
				}
				return base.gameObject.activeInHierarchy;
			}
		}

		public bool isPoseValid => trackedObject.isValid;

		public Interactable hoveringInteractable
		{
			get
			{
				return _hoveringInteractable;
			}
			set
			{
				if (!(_hoveringInteractable != value))
				{
					return;
				}
				if (_hoveringInteractable != null)
				{
					if (spewDebugText)
					{
						HandDebugLog("HoverEnd " + _hoveringInteractable.gameObject);
					}
					_hoveringInteractable.SendMessage("OnHandHoverEnd", this, SendMessageOptions.DontRequireReceiver);
					if (_hoveringInteractable != null)
					{
						BroadcastMessage("OnParentHandHoverEnd", _hoveringInteractable, SendMessageOptions.DontRequireReceiver);
					}
				}
				_hoveringInteractable = value;
				if (_hoveringInteractable != null)
				{
					if (spewDebugText)
					{
						HandDebugLog("HoverBegin " + _hoveringInteractable.gameObject);
					}
					_hoveringInteractable.SendMessage("OnHandHoverBegin", this, SendMessageOptions.DontRequireReceiver);
					if (_hoveringInteractable != null)
					{
						BroadcastMessage("OnParentHandHoverBegin", _hoveringInteractable, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
		}

		public GameObject currentAttachedObject
		{
			get
			{
				CleanUpAttachedObjectStack();
				if (attachedObjects.Count > 0)
				{
					return attachedObjects[attachedObjects.Count - 1].attachedObject;
				}
				return null;
			}
		}

		public AttachedObject? currentAttachedObjectInfo
		{
			get
			{
				CleanUpAttachedObjectStack();
				if (attachedObjects.Count > 0)
				{
					return attachedObjects[attachedObjects.Count - 1];
				}
				return null;
			}
		}

		public SteamVR_Behaviour_Skeleton skeleton
		{
			get
			{
				if (mainRenderModel != null)
				{
					return mainRenderModel.GetSkeleton();
				}
				return null;
			}
		}

		public void ShowController(bool permanent = false)
		{
			if (mainRenderModel != null)
			{
				mainRenderModel.SetControllerVisibility(state: true, permanent);
			}
			if (hoverhighlightRenderModel != null)
			{
				hoverhighlightRenderModel.SetControllerVisibility(state: true, permanent);
			}
		}

		public void HideController(bool permanent = false)
		{
			if (mainRenderModel != null)
			{
				mainRenderModel.SetControllerVisibility(state: false, permanent);
			}
			if (hoverhighlightRenderModel != null)
			{
				hoverhighlightRenderModel.SetControllerVisibility(state: false, permanent);
			}
		}

		public void ShowSkeleton(bool permanent = false)
		{
			if (mainRenderModel != null)
			{
				mainRenderModel.SetHandVisibility(state: true, permanent);
			}
			if (hoverhighlightRenderModel != null)
			{
				hoverhighlightRenderModel.SetHandVisibility(state: true, permanent);
			}
		}

		public void HideSkeleton(bool permanent = false)
		{
			if (mainRenderModel != null)
			{
				mainRenderModel.SetHandVisibility(state: false, permanent);
			}
			if (hoverhighlightRenderModel != null)
			{
				hoverhighlightRenderModel.SetHandVisibility(state: false, permanent);
			}
		}

		public bool HasSkeleton()
		{
			return mainRenderModel != null && mainRenderModel.GetSkeleton() != null;
		}

		public void Show()
		{
			SetVisibility(visible: true);
		}

		public void Hide()
		{
			SetVisibility(visible: false);
		}

		public void SetVisibility(bool visible)
		{
			if (mainRenderModel != null)
			{
				mainRenderModel.SetVisibility(visible);
			}
		}

		public void SetSkeletonRangeOfMotion(EVRSkeletalMotionRange newRangeOfMotion, float blendOverSeconds = 0.1f)
		{
			for (int i = 0; i < renderModels.Count; i++)
			{
				renderModels[i].SetSkeletonRangeOfMotion(newRangeOfMotion, blendOverSeconds);
			}
		}

		public void SetTemporarySkeletonRangeOfMotion(SkeletalMotionRangeChange temporaryRangeOfMotionChange, float blendOverSeconds = 0.1f)
		{
			for (int i = 0; i < renderModels.Count; i++)
			{
				renderModels[i].SetTemporarySkeletonRangeOfMotion(temporaryRangeOfMotionChange, blendOverSeconds);
			}
		}

		public void ResetTemporarySkeletonRangeOfMotion(float blendOverSeconds = 0.1f)
		{
			for (int i = 0; i < renderModels.Count; i++)
			{
				renderModels[i].ResetTemporarySkeletonRangeOfMotion(blendOverSeconds);
			}
		}

		public void SetAnimationState(int stateValue)
		{
			for (int i = 0; i < renderModels.Count; i++)
			{
				renderModels[i].SetAnimationState(stateValue);
			}
		}

		public void StopAnimation()
		{
			for (int i = 0; i < renderModels.Count; i++)
			{
				renderModels[i].StopAnimation();
			}
		}

		public void AttachObject(GameObject objectToAttach, GrabTypes grabbedWithType, AttachmentFlags flags = AttachmentFlags.SnapOnAttach | AttachmentFlags.DetachOthers | AttachmentFlags.DetachFromOtherHand | AttachmentFlags.ParentToHand | AttachmentFlags.TurnOnKinematic, Transform attachmentOffset = null)
		{
			AttachedObject item = default(AttachedObject);
			item.attachmentFlags = flags;
			item.attachedOffsetTransform = attachmentOffset;
			item.attachTime = Time.time;
			if (flags == (AttachmentFlags)0)
			{
				flags = AttachmentFlags.SnapOnAttach | AttachmentFlags.DetachOthers | AttachmentFlags.DetachFromOtherHand | AttachmentFlags.ParentToHand | AttachmentFlags.TurnOnKinematic;
			}
			CleanUpAttachedObjectStack();
			if (ObjectIsAttached(objectToAttach))
			{
				DetachObject(objectToAttach);
			}
			if (item.HasAttachFlag(AttachmentFlags.DetachFromOtherHand) && otherHand != null)
			{
				otherHand.DetachObject(objectToAttach);
			}
			if (item.HasAttachFlag(AttachmentFlags.DetachOthers))
			{
				while (attachedObjects.Count > 0)
				{
					DetachObject(attachedObjects[0].attachedObject);
				}
			}
			if ((bool)currentAttachedObject)
			{
				currentAttachedObject.SendMessage("OnHandFocusLost", this, SendMessageOptions.DontRequireReceiver);
			}
			item.attachedObject = objectToAttach;
			item.interactable = objectToAttach.GetComponent<Interactable>();
			item.handAttachmentPointTransform = base.transform;
			if (item.interactable != null)
			{
				if (item.interactable.attachEaseIn)
				{
					item.easeSourcePosition = item.attachedObject.transform.position;
					item.easeSourceRotation = item.attachedObject.transform.rotation;
					item.interactable.snapAttachEaseInCompleted = false;
				}
				if (item.interactable.useHandObjectAttachmentPoint)
				{
					item.handAttachmentPointTransform = objectAttachmentPoint;
				}
				if (item.interactable.hideHandOnAttach)
				{
					Hide();
				}
				if (item.interactable.hideSkeletonOnAttach && mainRenderModel != null && mainRenderModel.displayHandByDefault)
				{
					HideSkeleton();
				}
				if (item.interactable.hideControllerOnAttach && mainRenderModel != null && mainRenderModel.displayControllerByDefault)
				{
					HideController();
				}
				if (item.interactable.handAnimationOnPickup != 0)
				{
					SetAnimationState(item.interactable.handAnimationOnPickup);
				}
				if (item.interactable.setRangeOfMotionOnPickup != SkeletalMotionRangeChange.None)
				{
					SetTemporarySkeletonRangeOfMotion(item.interactable.setRangeOfMotionOnPickup);
				}
			}
			item.originalParent = ((!(objectToAttach.transform.parent != null)) ? null : objectToAttach.transform.parent.gameObject);
			item.attachedRigidbody = objectToAttach.GetComponent<Rigidbody>();
			if (item.attachedRigidbody != null)
			{
				if (item.interactable.attachedToHand != null)
				{
					for (int i = 0; i < item.interactable.attachedToHand.attachedObjects.Count; i++)
					{
						AttachedObject attachedObject = item.interactable.attachedToHand.attachedObjects[i];
						if (attachedObject.interactable == item.interactable)
						{
							item.attachedRigidbodyWasKinematic = attachedObject.attachedRigidbodyWasKinematic;
							item.attachedRigidbodyUsedGravity = attachedObject.attachedRigidbodyUsedGravity;
							item.originalParent = attachedObject.originalParent;
						}
					}
				}
				else
				{
					item.attachedRigidbodyWasKinematic = item.attachedRigidbody.isKinematic;
					item.attachedRigidbodyUsedGravity = item.attachedRigidbody.useGravity;
				}
			}
			item.grabbedWithType = grabbedWithType;
			if (item.HasAttachFlag(AttachmentFlags.ParentToHand))
			{
				objectToAttach.transform.parent = base.transform;
				item.isParentedToHand = true;
			}
			else
			{
				item.isParentedToHand = false;
			}
			if (item.HasAttachFlag(AttachmentFlags.SnapOnAttach))
			{
				if (item.interactable != null && item.interactable.skeletonPoser != null && HasSkeleton())
				{
					SteamVR_Skeleton_PoseSnapshot blendedPose = item.interactable.skeletonPoser.GetBlendedPose(skeleton);
					objectToAttach.transform.position = base.transform.TransformPoint(blendedPose.position);
					objectToAttach.transform.rotation = base.transform.rotation * blendedPose.rotation;
					item.initialPositionalOffset = item.handAttachmentPointTransform.InverseTransformPoint(objectToAttach.transform.position);
					item.initialRotationalOffset = Quaternion.Inverse(item.handAttachmentPointTransform.rotation) * objectToAttach.transform.rotation;
				}
				else
				{
					if (attachmentOffset != null)
					{
						Quaternion quaternion = Quaternion.Inverse(attachmentOffset.transform.rotation) * objectToAttach.transform.rotation;
						objectToAttach.transform.rotation = item.handAttachmentPointTransform.rotation * quaternion;
						Vector3 vector = objectToAttach.transform.position - attachmentOffset.transform.position;
						objectToAttach.transform.position = item.handAttachmentPointTransform.position + vector;
					}
					else
					{
						objectToAttach.transform.rotation = item.handAttachmentPointTransform.rotation;
						objectToAttach.transform.position = item.handAttachmentPointTransform.position;
					}
					Transform transform = objectToAttach.transform;
					item.initialPositionalOffset = item.handAttachmentPointTransform.InverseTransformPoint(transform.position);
					item.initialRotationalOffset = Quaternion.Inverse(item.handAttachmentPointTransform.rotation) * transform.rotation;
				}
			}
			else if (item.interactable != null && item.interactable.skeletonPoser != null && HasSkeleton())
			{
				item.initialPositionalOffset = item.handAttachmentPointTransform.InverseTransformPoint(objectToAttach.transform.position);
				item.initialRotationalOffset = Quaternion.Inverse(item.handAttachmentPointTransform.rotation) * objectToAttach.transform.rotation;
			}
			else if (attachmentOffset != null)
			{
				Quaternion quaternion2 = Quaternion.Inverse(attachmentOffset.transform.rotation) * objectToAttach.transform.rotation;
				Quaternion quaternion3 = item.handAttachmentPointTransform.rotation * quaternion2;
				Quaternion quaternion4 = quaternion3 * Quaternion.Inverse(objectToAttach.transform.rotation);
				Vector3 vector2 = quaternion4 * objectToAttach.transform.position - quaternion4 * attachmentOffset.transform.position;
				item.initialPositionalOffset = item.handAttachmentPointTransform.InverseTransformPoint(item.handAttachmentPointTransform.position + vector2);
				item.initialRotationalOffset = Quaternion.Inverse(item.handAttachmentPointTransform.rotation) * (item.handAttachmentPointTransform.rotation * quaternion2);
			}
			else
			{
				item.initialPositionalOffset = item.handAttachmentPointTransform.InverseTransformPoint(objectToAttach.transform.position);
				item.initialRotationalOffset = Quaternion.Inverse(item.handAttachmentPointTransform.rotation) * objectToAttach.transform.rotation;
			}
			if (item.HasAttachFlag(AttachmentFlags.TurnOnKinematic) && item.attachedRigidbody != null)
			{
				item.collisionDetectionMode = item.attachedRigidbody.collisionDetectionMode;
				if (item.collisionDetectionMode == CollisionDetectionMode.Continuous)
				{
					item.attachedRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
				}
				item.attachedRigidbody.isKinematic = true;
			}
			if (item.HasAttachFlag(AttachmentFlags.TurnOffGravity) && item.attachedRigidbody != null)
			{
				item.attachedRigidbody.useGravity = false;
			}
			if (item.interactable != null && item.interactable.attachEaseIn)
			{
				item.attachedObject.transform.position = item.easeSourcePosition;
				item.attachedObject.transform.rotation = item.easeSourceRotation;
			}
			attachedObjects.Add(item);
			UpdateHovering();
			if (spewDebugText)
			{
				HandDebugLog("AttachObject " + objectToAttach);
			}
			objectToAttach.SendMessage("OnAttachedToHand", this, SendMessageOptions.DontRequireReceiver);
		}

		public bool ObjectIsAttached(GameObject go)
		{
			for (int i = 0; i < attachedObjects.Count; i++)
			{
				if (attachedObjects[i].attachedObject == go)
				{
					return true;
				}
			}
			return false;
		}

		public void ForceHoverUnlock()
		{
			hoverLocked = false;
		}

		public void DetachObject(GameObject objectToDetach, bool restoreOriginalParent = true)
		{
			int num = attachedObjects.FindIndex((AttachedObject l) => l.attachedObject == objectToDetach);
			if (num != -1)
			{
				if (spewDebugText)
				{
					HandDebugLog("DetachObject " + objectToDetach);
				}
				GameObject currentAttachedObject = this.currentAttachedObject;
				if (attachedObjects[num].interactable != null)
				{
					if (attachedObjects[num].interactable.hideHandOnAttach)
					{
						Show();
					}
					if (attachedObjects[num].interactable.hideSkeletonOnAttach && mainRenderModel != null && mainRenderModel.displayHandByDefault)
					{
						ShowSkeleton();
					}
					if (attachedObjects[num].interactable.hideControllerOnAttach && mainRenderModel != null && mainRenderModel.displayControllerByDefault)
					{
						ShowController();
					}
					if (attachedObjects[num].interactable.handAnimationOnPickup != 0)
					{
						StopAnimation();
					}
					if (attachedObjects[num].interactable.setRangeOfMotionOnPickup != SkeletalMotionRangeChange.None)
					{
						ResetTemporarySkeletonRangeOfMotion();
					}
				}
				Transform parent = null;
				if (attachedObjects[num].isParentedToHand)
				{
					if (restoreOriginalParent && attachedObjects[num].originalParent != null)
					{
						parent = attachedObjects[num].originalParent.transform;
					}
					if (attachedObjects[num].attachedObject != null)
					{
						attachedObjects[num].attachedObject.transform.parent = parent;
					}
				}
				if (attachedObjects[num].HasAttachFlag(AttachmentFlags.TurnOnKinematic) && attachedObjects[num].attachedRigidbody != null)
				{
					attachedObjects[num].attachedRigidbody.isKinematic = attachedObjects[num].attachedRigidbodyWasKinematic;
					attachedObjects[num].attachedRigidbody.collisionDetectionMode = attachedObjects[num].collisionDetectionMode;
				}
				if (attachedObjects[num].HasAttachFlag(AttachmentFlags.TurnOffGravity) && attachedObjects[num].attachedObject != null && attachedObjects[num].attachedRigidbody != null)
				{
					attachedObjects[num].attachedRigidbody.useGravity = attachedObjects[num].attachedRigidbodyUsedGravity;
				}
				if (attachedObjects[num].interactable != null && attachedObjects[num].interactable.handFollowTransform && HasSkeleton())
				{
					skeleton.transform.localPosition = Vector3.zero;
					skeleton.transform.localRotation = Quaternion.identity;
				}
				if (attachedObjects[num].attachedObject != null)
				{
					if (attachedObjects[num].interactable == null || (attachedObjects[num].interactable != null && !attachedObjects[num].interactable.isDestroying))
					{
						attachedObjects[num].attachedObject.SetActive(value: true);
					}
					attachedObjects[num].attachedObject.SendMessage("OnDetachedFromHand", this, SendMessageOptions.DontRequireReceiver);
				}
				attachedObjects.RemoveAt(num);
				CleanUpAttachedObjectStack();
				GameObject currentAttachedObject2 = this.currentAttachedObject;
				hoverLocked = false;
				if (currentAttachedObject2 != null && currentAttachedObject2 != currentAttachedObject)
				{
					currentAttachedObject2.SetActive(value: true);
					currentAttachedObject2.SendMessage("OnHandFocusAcquired", this, SendMessageOptions.DontRequireReceiver);
				}
			}
			CleanUpAttachedObjectStack();
			if (mainRenderModel != null)
			{
				mainRenderModel.MatchHandToTransform(mainRenderModel.transform);
			}
			if (hoverhighlightRenderModel != null)
			{
				hoverhighlightRenderModel.MatchHandToTransform(hoverhighlightRenderModel.transform);
			}
		}

		public Vector3 GetTrackedObjectVelocity(float timeOffset = 0f)
		{
			if (trackedObject == null)
			{
				GetUpdatedAttachedVelocities(currentAttachedObjectInfo.Value, out var velocityTarget, out var _);
				return velocityTarget;
			}
			if (isActive)
			{
				if (timeOffset == 0f)
				{
					return Player.instance.trackingOriginTransform.TransformVector(trackedObject.GetVelocity());
				}
				trackedObject.GetVelocitiesAtTimeOffset(timeOffset, out var velocity, out var _);
				return Player.instance.trackingOriginTransform.TransformVector(velocity);
			}
			return Vector3.zero;
		}

		public Vector3 GetTrackedObjectAngularVelocity(float timeOffset = 0f)
		{
			if (trackedObject == null)
			{
				GetUpdatedAttachedVelocities(currentAttachedObjectInfo.Value, out var _, out var angularTarget);
				return angularTarget;
			}
			if (isActive)
			{
				if (timeOffset == 0f)
				{
					return Player.instance.trackingOriginTransform.TransformDirection(trackedObject.GetAngularVelocity());
				}
				trackedObject.GetVelocitiesAtTimeOffset(timeOffset, out var _, out var angularVelocity);
				return Player.instance.trackingOriginTransform.TransformDirection(angularVelocity);
			}
			return Vector3.zero;
		}

		public void GetEstimatedPeakVelocities(out Vector3 velocity, out Vector3 angularVelocity)
		{
			trackedObject.GetEstimatedPeakVelocities(out velocity, out angularVelocity);
			velocity = Player.instance.trackingOriginTransform.TransformVector(velocity);
			angularVelocity = Player.instance.trackingOriginTransform.TransformDirection(angularVelocity);
		}

		private void CleanUpAttachedObjectStack()
		{
			attachedObjects.RemoveAll((AttachedObject l) => l.attachedObject == null);
		}

		protected virtual void Awake()
		{
			inputFocusAction = SteamVR_Events.InputFocusAction(OnInputFocus);
			if (hoverSphereTransform == null)
			{
				hoverSphereTransform = base.transform;
			}
			if (objectAttachmentPoint == null)
			{
				objectAttachmentPoint = base.transform;
			}
			applicationLostFocusObject = new GameObject("_application_lost_focus");
			applicationLostFocusObject.transform.parent = base.transform;
			applicationLostFocusObject.SetActive(value: false);
			if (trackedObject == null)
			{
				trackedObject = base.gameObject.GetComponent<SteamVR_Behaviour_Pose>();
				if (trackedObject != null)
				{
					SteamVR_Behaviour_Pose steamVR_Behaviour_Pose = trackedObject;
					steamVR_Behaviour_Pose.onTransformUpdatedEvent = (SteamVR_Behaviour_Pose.UpdateHandler)Delegate.Combine(steamVR_Behaviour_Pose.onTransformUpdatedEvent, new SteamVR_Behaviour_Pose.UpdateHandler(OnTransformUpdated));
				}
			}
		}

		protected virtual void OnDestroy()
		{
			if (trackedObject != null)
			{
				SteamVR_Behaviour_Pose steamVR_Behaviour_Pose = trackedObject;
				steamVR_Behaviour_Pose.onTransformUpdatedEvent = (SteamVR_Behaviour_Pose.UpdateHandler)Delegate.Remove(steamVR_Behaviour_Pose.onTransformUpdatedEvent, new SteamVR_Behaviour_Pose.UpdateHandler(OnTransformUpdated));
			}
		}

		protected virtual void OnTransformUpdated(SteamVR_Behaviour_Pose updatedPose, SteamVR_Input_Sources updatedSource)
		{
			HandFollowUpdate();
		}

		protected virtual IEnumerator Start()
		{
			playerInstance = Player.instance;
			if (!playerInstance)
			{
				Debug.LogError("<b>[SteamVR Interaction]</b> No player instance found in Hand Start()");
			}
			overlappingColliders = new Collider[16];
			if (!noSteamVRFallbackCamera)
			{
				while (!isPoseValid)
				{
					yield return null;
				}
				InitController();
			}
		}

		protected virtual void UpdateHovering()
		{
			if ((!(noSteamVRFallbackCamera == null) || isActive) && !hoverLocked && !applicationLostFocusObject.activeSelf)
			{
				float closestDistance = float.MaxValue;
				Interactable closestInteractable = null;
				if (useHoverSphere)
				{
					float hoverRadius = hoverSphereRadius * Mathf.Abs(SteamVR_Utils.GetLossyScale(hoverSphereTransform));
					CheckHoveringForTransform(hoverSphereTransform.position, hoverRadius, ref closestDistance, ref closestInteractable, Color.green);
				}
				if (useControllerHoverComponent && mainRenderModel != null && mainRenderModel.IsControllerVisibile())
				{
					float num = controllerHoverRadius * Mathf.Abs(SteamVR_Utils.GetLossyScale(base.transform));
					CheckHoveringForTransform(mainRenderModel.GetControllerPosition(controllerHoverComponent), num / 2f, ref closestDistance, ref closestInteractable, Color.blue);
				}
				if (useFingerJointHover && mainRenderModel != null && mainRenderModel.IsHandVisibile())
				{
					float num2 = fingerJointHoverRadius * Mathf.Abs(SteamVR_Utils.GetLossyScale(base.transform));
					CheckHoveringForTransform(mainRenderModel.GetBonePosition((int)fingerJointHover), num2 / 2f, ref closestDistance, ref closestInteractable, Color.yellow);
				}
				hoveringInteractable = closestInteractable;
			}
		}

		protected virtual bool CheckHoveringForTransform(Vector3 hoverPosition, float hoverRadius, ref float closestDistance, ref Interactable closestInteractable, Color debugColor)
		{
			bool flag = false;
			for (int i = 0; i < overlappingColliders.Length; i++)
			{
				overlappingColliders[i] = null;
			}
			int num = Physics.OverlapSphereNonAlloc(hoverPosition, hoverRadius, overlappingColliders, hoverLayerMask.value);
			if (num == 16)
			{
				Debug.LogWarning("<b>[SteamVR Interaction]</b> This hand is overlapping the max number of colliders: " + 16 + ". Some collisions may be missed. Increase ColliderArraySize on Hand.cs");
			}
			int num2 = 0;
			for (int j = 0; j < overlappingColliders.Length; j++)
			{
				Collider collider = overlappingColliders[j];
				if (collider == null)
				{
					continue;
				}
				Interactable componentInParent = collider.GetComponentInParent<Interactable>();
				if (componentInParent == null)
				{
					continue;
				}
				IgnoreHovering component = collider.GetComponent<IgnoreHovering>();
				if (component != null && (component.onlyIgnoreHand == null || component.onlyIgnoreHand == this))
				{
					continue;
				}
				bool flag2 = false;
				for (int k = 0; k < attachedObjects.Count; k++)
				{
					if (attachedObjects[k].attachedObject == componentInParent.gameObject)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2 && (!otherHand || !(otherHand.hoveringInteractable == componentInParent)))
				{
					float num3 = Vector3.Distance(componentInParent.transform.position, hoverPosition);
					if (num3 < closestDistance)
					{
						closestDistance = num3;
						closestInteractable = componentInParent;
						flag = true;
					}
					num2++;
				}
			}
			if (showDebugInteractables && flag)
			{
				Debug.DrawLine(hoverPosition, closestInteractable.transform.position, debugColor, 0.05f, depthTest: false);
			}
			if (num2 > 0 && num2 != prevOverlappingColliders)
			{
				prevOverlappingColliders = num2;
				if (spewDebugText)
				{
					HandDebugLog("Found " + num2 + " overlapping colliders.");
				}
			}
			return flag;
		}

		protected virtual void UpdateNoSteamVRFallback()
		{
			if (!noSteamVRFallbackCamera)
			{
				return;
			}
			Ray ray = noSteamVRFallbackCamera.ScreenPointToRay(Input.mousePosition);
			if (attachedObjects.Count > 0)
			{
				base.transform.position = ray.origin + noSteamVRFallbackInteractorDistance * ray.direction;
				return;
			}
			Vector3 position = base.transform.position;
			base.transform.position = noSteamVRFallbackCamera.transform.forward * -1000f;
			if (Physics.Raycast(ray, out var hitInfo, noSteamVRFallbackMaxDistanceNoItem))
			{
				base.transform.position = hitInfo.point;
				noSteamVRFallbackInteractorDistance = Mathf.Min(noSteamVRFallbackMaxDistanceNoItem, hitInfo.distance);
			}
			else if (noSteamVRFallbackInteractorDistance > 0f)
			{
				base.transform.position = ray.origin + Mathf.Min(noSteamVRFallbackMaxDistanceNoItem, noSteamVRFallbackInteractorDistance) * ray.direction;
			}
			else
			{
				base.transform.position = position;
			}
		}

		private void UpdateDebugText()
		{
			if (showDebugText)
			{
				if (debugText == null)
				{
					debugText = new GameObject("_debug_text").AddComponent<TextMesh>();
					debugText.fontSize = 120;
					debugText.characterSize = 0.001f;
					debugText.transform.parent = base.transform;
					debugText.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
				}
				if (handType == SteamVR_Input_Sources.RightHand)
				{
					debugText.transform.localPosition = new Vector3(-0.05f, 0f, 0f);
					debugText.alignment = TextAlignment.Right;
					debugText.anchor = TextAnchor.UpperRight;
				}
				else
				{
					debugText.transform.localPosition = new Vector3(0.05f, 0f, 0f);
					debugText.alignment = TextAlignment.Left;
					debugText.anchor = TextAnchor.UpperLeft;
				}
				debugText.text = string.Format("Hovering: {0}\nHover Lock: {1}\nAttached: {2}\nTotal Attached: {3}\nType: {4}\n", (!hoveringInteractable) ? "null" : hoveringInteractable.gameObject.name, hoverLocked, (!currentAttachedObject) ? "null" : currentAttachedObject.name, attachedObjects.Count, handType.ToString());
			}
			else if (debugText != null)
			{
				UnityEngine.Object.Destroy(debugText.gameObject);
			}
		}

		protected virtual void OnEnable()
		{
			inputFocusAction.enabled = true;
			float time = ((!(otherHand != null) || otherHand.GetInstanceID() >= GetInstanceID()) ? 0f : (0.5f * hoverUpdateInterval));
			InvokeRepeating("UpdateHovering", time, hoverUpdateInterval);
			InvokeRepeating("UpdateDebugText", time, hoverUpdateInterval);
		}

		protected virtual void OnDisable()
		{
			inputFocusAction.enabled = false;
			CancelInvoke();
		}

		protected virtual void Update()
		{
			UpdateNoSteamVRFallback();
			GameObject currentAttachedObject = this.currentAttachedObject;
			if (currentAttachedObject != null)
			{
				currentAttachedObject.SendMessage("HandAttachedUpdate", this, SendMessageOptions.DontRequireReceiver);
			}
			if ((bool)hoveringInteractable)
			{
				hoveringInteractable.SendMessage("HandHoverUpdate", this, SendMessageOptions.DontRequireReceiver);
			}
		}

		public bool IsStillHovering(Interactable interactable)
		{
			return hoveringInteractable == interactable;
		}

		protected virtual void HandFollowUpdate()
		{
			GameObject currentAttachedObject = this.currentAttachedObject;
			if (!(currentAttachedObject != null) || !(currentAttachedObjectInfo.Value.interactable != null))
			{
				return;
			}
			SteamVR_Skeleton_PoseSnapshot steamVR_Skeleton_PoseSnapshot = null;
			if (currentAttachedObjectInfo.Value.interactable.skeletonPoser != null && HasSkeleton())
			{
				steamVR_Skeleton_PoseSnapshot = currentAttachedObjectInfo.Value.interactable.skeletonPoser.GetBlendedPose(skeleton);
			}
			if (currentAttachedObjectInfo.Value.interactable.handFollowTransform)
			{
				Quaternion handRotation;
				Vector3 handPosition;
				if (steamVR_Skeleton_PoseSnapshot == null)
				{
					Quaternion rotation = Quaternion.Inverse(base.transform.rotation) * currentAttachedObjectInfo.Value.handAttachmentPointTransform.rotation;
					handRotation = currentAttachedObjectInfo.Value.interactable.transform.rotation * Quaternion.Inverse(rotation);
					Vector3 vector = base.transform.position - currentAttachedObjectInfo.Value.handAttachmentPointTransform.position;
					Quaternion quaternion = mainRenderModel.GetHandRotation() * Quaternion.Inverse(base.transform.rotation);
					Vector3 vector2 = quaternion * vector;
					handPosition = currentAttachedObjectInfo.Value.interactable.transform.position + vector2;
				}
				else
				{
					Transform transform = currentAttachedObjectInfo.Value.attachedObject.transform;
					Vector3 position = transform.position;
					Quaternion rotation2 = transform.transform.rotation;
					transform.position = TargetItemPosition(currentAttachedObjectInfo.Value);
					transform.rotation = TargetItemRotation(currentAttachedObjectInfo.Value);
					Vector3 position2 = transform.InverseTransformPoint(base.transform.position);
					Quaternion quaternion2 = Quaternion.Inverse(transform.rotation) * base.transform.rotation;
					transform.position = position;
					transform.rotation = rotation2;
					handPosition = transform.TransformPoint(position2);
					handRotation = transform.rotation * quaternion2;
				}
				if (mainRenderModel != null)
				{
					mainRenderModel.SetHandRotation(handRotation);
				}
				if (hoverhighlightRenderModel != null)
				{
					hoverhighlightRenderModel.SetHandRotation(handRotation);
				}
				if (mainRenderModel != null)
				{
					mainRenderModel.SetHandPosition(handPosition);
				}
				if (hoverhighlightRenderModel != null)
				{
					hoverhighlightRenderModel.SetHandPosition(handPosition);
				}
			}
		}

		protected virtual void FixedUpdate()
		{
			if (!(currentAttachedObject != null))
			{
				return;
			}
			AttachedObject value = currentAttachedObjectInfo.Value;
			if (!(value.attachedObject != null))
			{
				return;
			}
			if (value.HasAttachFlag(AttachmentFlags.VelocityMovement))
			{
				if (!value.interactable.attachEaseIn || value.interactable.snapAttachEaseInCompleted)
				{
					UpdateAttachedVelocity(value);
				}
			}
			else if (value.HasAttachFlag(AttachmentFlags.ParentToHand))
			{
				value.attachedObject.transform.position = TargetItemPosition(value);
				value.attachedObject.transform.rotation = TargetItemRotation(value);
			}
			if (!value.interactable.attachEaseIn)
			{
				return;
			}
			float num = Util.RemapNumberClamped(Time.time, value.attachTime, value.attachTime + value.interactable.snapAttachEaseInTime, 0f, 1f);
			if (num < 1f)
			{
				if (value.HasAttachFlag(AttachmentFlags.VelocityMovement))
				{
					value.attachedRigidbody.velocity = Vector3.zero;
					value.attachedRigidbody.angularVelocity = Vector3.zero;
				}
				num = value.interactable.snapAttachEaseInCurve.Evaluate(num);
				value.attachedObject.transform.position = Vector3.Lerp(value.easeSourcePosition, TargetItemPosition(value), num);
				value.attachedObject.transform.rotation = Quaternion.Lerp(value.easeSourceRotation, TargetItemRotation(value), num);
			}
			else if (!value.interactable.snapAttachEaseInCompleted)
			{
				value.interactable.gameObject.SendMessage("OnThrowableAttachEaseInCompleted", this, SendMessageOptions.DontRequireReceiver);
				value.interactable.snapAttachEaseInCompleted = true;
			}
		}

		protected void UpdateAttachedVelocity(AttachedObject attachedObjectInfo)
		{
			if (GetUpdatedAttachedVelocities(attachedObjectInfo, out var velocityTarget, out var angularTarget))
			{
				float lossyScale = SteamVR_Utils.GetLossyScale(currentAttachedObjectInfo.Value.handAttachmentPointTransform);
				float maxDistanceDelta = 20f * lossyScale;
				float maxDistanceDelta2 = 10f * lossyScale;
				attachedObjectInfo.attachedRigidbody.velocity = Vector3.MoveTowards(attachedObjectInfo.attachedRigidbody.velocity, velocityTarget, maxDistanceDelta2);
				attachedObjectInfo.attachedRigidbody.angularVelocity = Vector3.MoveTowards(attachedObjectInfo.attachedRigidbody.angularVelocity, angularTarget, maxDistanceDelta);
			}
		}

		public void ResetAttachedTransform(AttachedObject attachedObject)
		{
			attachedObject.attachedObject.transform.position = TargetItemPosition(attachedObject);
			attachedObject.attachedObject.transform.rotation = TargetItemRotation(attachedObject);
		}

		protected Vector3 TargetItemPosition(AttachedObject attachedObject)
		{
			if (attachedObject.interactable != null && attachedObject.interactable.skeletonPoser != null && HasSkeleton())
			{
				Vector3 position = attachedObject.handAttachmentPointTransform.InverseTransformPoint(base.transform.TransformPoint(attachedObject.interactable.skeletonPoser.GetBlendedPose(skeleton).position));
				return currentAttachedObjectInfo.Value.handAttachmentPointTransform.TransformPoint(position);
			}
			return currentAttachedObjectInfo.Value.handAttachmentPointTransform.TransformPoint(attachedObject.initialPositionalOffset);
		}

		protected Quaternion TargetItemRotation(AttachedObject attachedObject)
		{
			if (attachedObject.interactable != null && attachedObject.interactable.skeletonPoser != null && HasSkeleton())
			{
				Quaternion quaternion = Quaternion.Inverse(attachedObject.handAttachmentPointTransform.rotation) * (base.transform.rotation * attachedObject.interactable.skeletonPoser.GetBlendedPose(skeleton).rotation);
				return currentAttachedObjectInfo.Value.handAttachmentPointTransform.rotation * quaternion;
			}
			return currentAttachedObjectInfo.Value.handAttachmentPointTransform.rotation * attachedObject.initialRotationalOffset;
		}

		protected bool GetUpdatedAttachedVelocities(AttachedObject attachedObjectInfo, out Vector3 velocityTarget, out Vector3 angularTarget)
		{
			bool flag = false;
			float num = 6000f;
			float num2 = 50f;
			Vector3 vector = TargetItemPosition(attachedObjectInfo);
			Vector3 vector2 = vector - attachedObjectInfo.attachedRigidbody.position;
			velocityTarget = vector2 * num * Time.deltaTime;
			if (!float.IsNaN(velocityTarget.x) && !float.IsInfinity(velocityTarget.x))
			{
				if ((bool)noSteamVRFallbackCamera)
				{
					velocityTarget /= 10f;
				}
				flag = true;
			}
			else
			{
				velocityTarget = Vector3.zero;
			}
			Quaternion quaternion = TargetItemRotation(attachedObjectInfo);
			(quaternion * Quaternion.Inverse(attachedObjectInfo.attachedObject.transform.rotation)).ToAngleAxis(out var angle, out var axis);
			if (angle > 180f)
			{
				angle -= 360f;
			}
			if (angle != 0f && !float.IsNaN(axis.x) && !float.IsInfinity(axis.x))
			{
				angularTarget = angle * axis * num2 * Time.deltaTime;
				if ((bool)noSteamVRFallbackCamera)
				{
					angularTarget /= 10f;
				}
				flag = flag && true;
			}
			else
			{
				angularTarget = Vector3.zero;
			}
			return flag;
		}

		protected virtual void OnInputFocus(bool hasFocus)
		{
			if (hasFocus)
			{
				DetachObject(applicationLostFocusObject);
				applicationLostFocusObject.SetActive(value: false);
				UpdateHovering();
				BroadcastMessage("OnParentHandInputFocusAcquired", SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				applicationLostFocusObject.SetActive(value: true);
				AttachObject(applicationLostFocusObject, GrabTypes.Scripted, AttachmentFlags.ParentToHand);
				BroadcastMessage("OnParentHandInputFocusLost", SendMessageOptions.DontRequireReceiver);
			}
		}

		protected virtual void OnDrawGizmos()
		{
			if (useHoverSphere && hoverSphereTransform != null)
			{
				Gizmos.color = Color.green;
				float num = hoverSphereRadius * Mathf.Abs(SteamVR_Utils.GetLossyScale(hoverSphereTransform));
				Gizmos.DrawWireSphere(hoverSphereTransform.position, num / 2f);
			}
			if (useControllerHoverComponent && mainRenderModel != null && mainRenderModel.IsControllerVisibile())
			{
				Gizmos.color = Color.blue;
				float num2 = controllerHoverRadius * Mathf.Abs(SteamVR_Utils.GetLossyScale(base.transform));
				Gizmos.DrawWireSphere(mainRenderModel.GetControllerPosition(controllerHoverComponent), num2 / 2f);
			}
			if (useFingerJointHover && mainRenderModel != null && mainRenderModel.IsHandVisibile())
			{
				Gizmos.color = Color.yellow;
				float num3 = fingerJointHoverRadius * Mathf.Abs(SteamVR_Utils.GetLossyScale(base.transform));
				Gizmos.DrawWireSphere(mainRenderModel.GetBonePosition((int)fingerJointHover), num3 / 2f);
			}
		}

		private void HandDebugLog(string msg)
		{
			if (spewDebugText)
			{
				Debug.Log("<b>[SteamVR Interaction]</b> Hand (" + base.name + "): " + msg);
			}
		}

		public void HoverLock(Interactable interactable)
		{
			if (spewDebugText)
			{
				HandDebugLog("HoverLock " + interactable);
			}
			hoverLocked = true;
			hoveringInteractable = interactable;
		}

		public void HoverUnlock(Interactable interactable)
		{
			if (spewDebugText)
			{
				HandDebugLog("HoverUnlock " + interactable);
			}
			if (hoveringInteractable == interactable)
			{
				hoverLocked = false;
			}
		}

		public void TriggerHapticPulse(ushort microSecondsDuration)
		{
			float num = (float)(int)microSecondsDuration / 1000000f;
			hapticAction.Execute(0f, num, 1f / num, 1f, handType);
		}

		public void TriggerHapticPulse(float duration, float frequency, float amplitude)
		{
			hapticAction.Execute(0f, duration, frequency, amplitude, handType);
		}

		public void ShowGrabHint()
		{
			ControllerButtonHints.ShowButtonHint(this, grabGripAction);
		}

		public void HideGrabHint()
		{
			ControllerButtonHints.HideButtonHint(this, grabGripAction);
		}

		public void ShowGrabHint(string text)
		{
			ControllerButtonHints.ShowTextHint(this, grabGripAction, text);
		}

		public GrabTypes GetGrabStarting(GrabTypes explicitType = GrabTypes.None)
		{
			if (explicitType != 0)
			{
				if ((bool)noSteamVRFallbackCamera && Input.GetMouseButtonDown(0))
				{
					return explicitType;
				}
				if (explicitType == GrabTypes.Pinch && grabPinchAction.GetStateDown(handType))
				{
					return GrabTypes.Pinch;
				}
				if (explicitType == GrabTypes.Grip && grabGripAction.GetStateDown(handType))
				{
					return GrabTypes.Grip;
				}
			}
			else
			{
				if ((bool)noSteamVRFallbackCamera && Input.GetMouseButtonDown(0))
				{
					return GrabTypes.Grip;
				}
				if (grabPinchAction.GetStateDown(handType))
				{
					return GrabTypes.Pinch;
				}
				if (grabGripAction.GetStateDown(handType))
				{
					return GrabTypes.Grip;
				}
			}
			return GrabTypes.None;
		}

		public GrabTypes GetGrabEnding(GrabTypes explicitType = GrabTypes.None)
		{
			if (explicitType != 0)
			{
				if ((bool)noSteamVRFallbackCamera && Input.GetMouseButtonUp(0))
				{
					return explicitType;
				}
				if (explicitType == GrabTypes.Pinch && grabPinchAction.GetStateUp(handType))
				{
					return GrabTypes.Pinch;
				}
				if (explicitType == GrabTypes.Grip && grabGripAction.GetStateUp(handType))
				{
					return GrabTypes.Grip;
				}
			}
			else
			{
				if ((bool)noSteamVRFallbackCamera && Input.GetMouseButtonUp(0))
				{
					return GrabTypes.Grip;
				}
				if (grabPinchAction.GetStateUp(handType))
				{
					return GrabTypes.Pinch;
				}
				if (grabGripAction.GetStateUp(handType))
				{
					return GrabTypes.Grip;
				}
			}
			return GrabTypes.None;
		}

		public bool IsGrabEnding(GameObject attachedObject)
		{
			for (int i = 0; i < attachedObjects.Count; i++)
			{
				if (attachedObjects[i].attachedObject == attachedObject)
				{
					return !IsGrabbingWithType(attachedObjects[i].grabbedWithType);
				}
			}
			return false;
		}

		public bool IsGrabbingWithType(GrabTypes type)
		{
			if ((bool)noSteamVRFallbackCamera && Input.GetMouseButton(0))
			{
				return true;
			}
			return type switch
			{
				GrabTypes.Pinch => grabPinchAction.GetState(handType), 
				GrabTypes.Grip => grabGripAction.GetState(handType), 
				_ => false, 
			};
		}

		public bool IsGrabbingWithOppositeType(GrabTypes type)
		{
			if ((bool)noSteamVRFallbackCamera && Input.GetMouseButton(0))
			{
				return true;
			}
			return type switch
			{
				GrabTypes.Pinch => grabGripAction.GetState(handType), 
				GrabTypes.Grip => grabPinchAction.GetState(handType), 
				_ => false, 
			};
		}

		public GrabTypes GetBestGrabbingType()
		{
			return GetBestGrabbingType(GrabTypes.None);
		}

		public GrabTypes GetBestGrabbingType(GrabTypes preferred, bool forcePreference = false)
		{
			if ((bool)noSteamVRFallbackCamera && Input.GetMouseButton(0))
			{
				return preferred;
			}
			if (preferred == GrabTypes.Pinch)
			{
				if (grabPinchAction.GetState(handType))
				{
					return GrabTypes.Pinch;
				}
				if (forcePreference)
				{
					return GrabTypes.None;
				}
			}
			if (preferred == GrabTypes.Grip)
			{
				if (grabGripAction.GetState(handType))
				{
					return GrabTypes.Grip;
				}
				if (forcePreference)
				{
					return GrabTypes.None;
				}
			}
			if (grabPinchAction.GetState(handType))
			{
				return GrabTypes.Pinch;
			}
			if (grabGripAction.GetState(handType))
			{
				return GrabTypes.Grip;
			}
			return GrabTypes.None;
		}

		private void InitController()
		{
			if (spewDebugText)
			{
				HandDebugLog("Hand " + base.name + " connected with type " + handType);
			}
			bool flag = mainRenderModel != null;
			EVRSkeletalMotionRange newRangeOfMotion = EVRSkeletalMotionRange.WithController;
			if (flag)
			{
				newRangeOfMotion = mainRenderModel.GetSkeletonRangeOfMotion;
			}
			foreach (RenderModel renderModel in renderModels)
			{
				if (renderModel != null)
				{
					UnityEngine.Object.Destroy(renderModel.gameObject);
				}
			}
			renderModels.Clear();
			GameObject gameObject = UnityEngine.Object.Instantiate(renderModelPrefab);
			gameObject.layer = base.gameObject.layer;
			gameObject.tag = base.gameObject.tag;
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = renderModelPrefab.transform.localScale;
			int deviceIndex = trackedObject.GetDeviceIndex();
			mainRenderModel = gameObject.GetComponent<RenderModel>();
			renderModels.Add(mainRenderModel);
			if (flag)
			{
				mainRenderModel.SetSkeletonRangeOfMotion(newRangeOfMotion);
			}
			BroadcastMessage("SetInputSource", handType, SendMessageOptions.DontRequireReceiver);
			BroadcastMessage("OnHandInitialized", deviceIndex, SendMessageOptions.DontRequireReceiver);
		}

		public void SetRenderModel(GameObject prefab)
		{
			renderModelPrefab = prefab;
			if (mainRenderModel != null && isPoseValid)
			{
				InitController();
			}
		}

		public void SetHoverRenderModel(RenderModel hoverRenderModel)
		{
			hoverhighlightRenderModel = hoverRenderModel;
			renderModels.Add(hoverRenderModel);
		}

		public int GetDeviceIndex()
		{
			return trackedObject.GetDeviceIndex();
		}
	}
}
