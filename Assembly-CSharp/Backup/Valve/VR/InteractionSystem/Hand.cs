// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Hand
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
  public class Hand : MonoBehaviour
  {
    public const Hand.AttachmentFlags defaultAttachmentFlags = Hand.AttachmentFlags.SnapOnAttach | Hand.AttachmentFlags.DetachOthers | Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.TurnOnKinematic;
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
    public LayerMask hoverLayerMask = (LayerMask) -1;
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
    private List<Hand.AttachedObject> attachedObjects = new List<Hand.AttachedObject>();
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

    public ReadOnlyCollection<Hand.AttachedObject> AttachedObjects => this.attachedObjects.AsReadOnly();

    public bool hoverLocked { get; private set; }

    public bool isActive => (UnityEngine.Object) this.trackedObject != (UnityEngine.Object) null ? this.trackedObject.isActive : this.gameObject.activeInHierarchy;

    public bool isPoseValid => this.trackedObject.isValid;

    public Interactable hoveringInteractable
    {
      get => this._hoveringInteractable;
      set
      {
        if (!((UnityEngine.Object) this._hoveringInteractable != (UnityEngine.Object) value))
          return;
        if ((UnityEngine.Object) this._hoveringInteractable != (UnityEngine.Object) null)
        {
          if (this.spewDebugText)
            this.HandDebugLog("HoverEnd " + (object) this._hoveringInteractable.gameObject);
          this._hoveringInteractable.SendMessage("OnHandHoverEnd", (object) this, SendMessageOptions.DontRequireReceiver);
          if ((UnityEngine.Object) this._hoveringInteractable != (UnityEngine.Object) null)
            this.BroadcastMessage("OnParentHandHoverEnd", (object) this._hoveringInteractable, SendMessageOptions.DontRequireReceiver);
        }
        this._hoveringInteractable = value;
        if (!((UnityEngine.Object) this._hoveringInteractable != (UnityEngine.Object) null))
          return;
        if (this.spewDebugText)
          this.HandDebugLog("HoverBegin " + (object) this._hoveringInteractable.gameObject);
        this._hoveringInteractable.SendMessage("OnHandHoverBegin", (object) this, SendMessageOptions.DontRequireReceiver);
        if (!((UnityEngine.Object) this._hoveringInteractable != (UnityEngine.Object) null))
          return;
        this.BroadcastMessage("OnParentHandHoverBegin", (object) this._hoveringInteractable, SendMessageOptions.DontRequireReceiver);
      }
    }

    public GameObject currentAttachedObject
    {
      get
      {
        this.CleanUpAttachedObjectStack();
        return this.attachedObjects.Count > 0 ? this.attachedObjects[this.attachedObjects.Count - 1].attachedObject : (GameObject) null;
      }
    }

    public Hand.AttachedObject? currentAttachedObjectInfo
    {
      get
      {
        this.CleanUpAttachedObjectStack();
        return this.attachedObjects.Count > 0 ? new Hand.AttachedObject?(this.attachedObjects[this.attachedObjects.Count - 1]) : new Hand.AttachedObject?();
      }
    }

    public SteamVR_Behaviour_Skeleton skeleton => (UnityEngine.Object) this.mainRenderModel != (UnityEngine.Object) null ? this.mainRenderModel.GetSkeleton() : (SteamVR_Behaviour_Skeleton) null;

    public void ShowController(bool permanent = false)
    {
      if ((UnityEngine.Object) this.mainRenderModel != (UnityEngine.Object) null)
        this.mainRenderModel.SetControllerVisibility(true, permanent);
      if (!((UnityEngine.Object) this.hoverhighlightRenderModel != (UnityEngine.Object) null))
        return;
      this.hoverhighlightRenderModel.SetControllerVisibility(true, permanent);
    }

    public void HideController(bool permanent = false)
    {
      if ((UnityEngine.Object) this.mainRenderModel != (UnityEngine.Object) null)
        this.mainRenderModel.SetControllerVisibility(false, permanent);
      if (!((UnityEngine.Object) this.hoverhighlightRenderModel != (UnityEngine.Object) null))
        return;
      this.hoverhighlightRenderModel.SetControllerVisibility(false, permanent);
    }

    public void ShowSkeleton(bool permanent = false)
    {
      if ((UnityEngine.Object) this.mainRenderModel != (UnityEngine.Object) null)
        this.mainRenderModel.SetHandVisibility(true, permanent);
      if (!((UnityEngine.Object) this.hoverhighlightRenderModel != (UnityEngine.Object) null))
        return;
      this.hoverhighlightRenderModel.SetHandVisibility(true, permanent);
    }

    public void HideSkeleton(bool permanent = false)
    {
      if ((UnityEngine.Object) this.mainRenderModel != (UnityEngine.Object) null)
        this.mainRenderModel.SetHandVisibility(false, permanent);
      if (!((UnityEngine.Object) this.hoverhighlightRenderModel != (UnityEngine.Object) null))
        return;
      this.hoverhighlightRenderModel.SetHandVisibility(false, permanent);
    }

    public bool HasSkeleton() => (UnityEngine.Object) this.mainRenderModel != (UnityEngine.Object) null && (UnityEngine.Object) this.mainRenderModel.GetSkeleton() != (UnityEngine.Object) null;

    public void Show() => this.SetVisibility(true);

    public void Hide() => this.SetVisibility(false);

    public void SetVisibility(bool visible)
    {
      if (!((UnityEngine.Object) this.mainRenderModel != (UnityEngine.Object) null))
        return;
      this.mainRenderModel.SetVisibility(visible);
    }

    public void SetSkeletonRangeOfMotion(
      EVRSkeletalMotionRange newRangeOfMotion,
      float blendOverSeconds = 0.1f)
    {
      for (int index = 0; index < this.renderModels.Count; ++index)
        this.renderModels[index].SetSkeletonRangeOfMotion(newRangeOfMotion, blendOverSeconds);
    }

    public void SetTemporarySkeletonRangeOfMotion(
      SkeletalMotionRangeChange temporaryRangeOfMotionChange,
      float blendOverSeconds = 0.1f)
    {
      for (int index = 0; index < this.renderModels.Count; ++index)
        this.renderModels[index].SetTemporarySkeletonRangeOfMotion(temporaryRangeOfMotionChange, blendOverSeconds);
    }

    public void ResetTemporarySkeletonRangeOfMotion(float blendOverSeconds = 0.1f)
    {
      for (int index = 0; index < this.renderModels.Count; ++index)
        this.renderModels[index].ResetTemporarySkeletonRangeOfMotion(blendOverSeconds);
    }

    public void SetAnimationState(int stateValue)
    {
      for (int index = 0; index < this.renderModels.Count; ++index)
        this.renderModels[index].SetAnimationState(stateValue);
    }

    public void StopAnimation()
    {
      for (int index = 0; index < this.renderModels.Count; ++index)
        this.renderModels[index].StopAnimation();
    }

    public void AttachObject(
      GameObject objectToAttach,
      GrabTypes grabbedWithType,
      Hand.AttachmentFlags flags = Hand.AttachmentFlags.SnapOnAttach | Hand.AttachmentFlags.DetachOthers | Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.TurnOnKinematic,
      Transform attachmentOffset = null)
    {
      Hand.AttachedObject attachedObject1 = new Hand.AttachedObject();
      attachedObject1.attachmentFlags = flags;
      attachedObject1.attachedOffsetTransform = attachmentOffset;
      attachedObject1.attachTime = Time.time;
      if (flags == (Hand.AttachmentFlags) 0)
        flags = Hand.AttachmentFlags.SnapOnAttach | Hand.AttachmentFlags.DetachOthers | Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.TurnOnKinematic;
      this.CleanUpAttachedObjectStack();
      if (this.ObjectIsAttached(objectToAttach))
        this.DetachObject(objectToAttach);
      if (attachedObject1.HasAttachFlag(Hand.AttachmentFlags.DetachFromOtherHand) && (UnityEngine.Object) this.otherHand != (UnityEngine.Object) null)
        this.otherHand.DetachObject(objectToAttach);
      if (attachedObject1.HasAttachFlag(Hand.AttachmentFlags.DetachOthers))
      {
        while (this.attachedObjects.Count > 0)
          this.DetachObject(this.attachedObjects[0].attachedObject);
      }
      if ((bool) (UnityEngine.Object) this.currentAttachedObject)
        this.currentAttachedObject.SendMessage("OnHandFocusLost", (object) this, SendMessageOptions.DontRequireReceiver);
      attachedObject1.attachedObject = objectToAttach;
      attachedObject1.interactable = objectToAttach.GetComponent<Interactable>();
      attachedObject1.handAttachmentPointTransform = this.transform;
      if ((UnityEngine.Object) attachedObject1.interactable != (UnityEngine.Object) null)
      {
        if (attachedObject1.interactable.attachEaseIn)
        {
          attachedObject1.easeSourcePosition = attachedObject1.attachedObject.transform.position;
          attachedObject1.easeSourceRotation = attachedObject1.attachedObject.transform.rotation;
          attachedObject1.interactable.snapAttachEaseInCompleted = false;
        }
        if (attachedObject1.interactable.useHandObjectAttachmentPoint)
          attachedObject1.handAttachmentPointTransform = this.objectAttachmentPoint;
        if (attachedObject1.interactable.hideHandOnAttach)
          this.Hide();
        if (attachedObject1.interactable.hideSkeletonOnAttach && (UnityEngine.Object) this.mainRenderModel != (UnityEngine.Object) null && this.mainRenderModel.displayHandByDefault)
          this.HideSkeleton();
        if (attachedObject1.interactable.hideControllerOnAttach && (UnityEngine.Object) this.mainRenderModel != (UnityEngine.Object) null && this.mainRenderModel.displayControllerByDefault)
          this.HideController();
        if (attachedObject1.interactable.handAnimationOnPickup != 0)
          this.SetAnimationState(attachedObject1.interactable.handAnimationOnPickup);
        if (attachedObject1.interactable.setRangeOfMotionOnPickup != SkeletalMotionRangeChange.None)
          this.SetTemporarySkeletonRangeOfMotion(attachedObject1.interactable.setRangeOfMotionOnPickup);
      }
      attachedObject1.originalParent = !((UnityEngine.Object) objectToAttach.transform.parent != (UnityEngine.Object) null) ? (GameObject) null : objectToAttach.transform.parent.gameObject;
      attachedObject1.attachedRigidbody = objectToAttach.GetComponent<Rigidbody>();
      if ((UnityEngine.Object) attachedObject1.attachedRigidbody != (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) attachedObject1.interactable.attachedToHand != (UnityEngine.Object) null)
        {
          for (int index = 0; index < attachedObject1.interactable.attachedToHand.attachedObjects.Count; ++index)
          {
            Hand.AttachedObject attachedObject2 = attachedObject1.interactable.attachedToHand.attachedObjects[index];
            if ((UnityEngine.Object) attachedObject2.interactable == (UnityEngine.Object) attachedObject1.interactable)
            {
              attachedObject1.attachedRigidbodyWasKinematic = attachedObject2.attachedRigidbodyWasKinematic;
              attachedObject1.attachedRigidbodyUsedGravity = attachedObject2.attachedRigidbodyUsedGravity;
              attachedObject1.originalParent = attachedObject2.originalParent;
            }
          }
        }
        else
        {
          attachedObject1.attachedRigidbodyWasKinematic = attachedObject1.attachedRigidbody.isKinematic;
          attachedObject1.attachedRigidbodyUsedGravity = attachedObject1.attachedRigidbody.useGravity;
        }
      }
      attachedObject1.grabbedWithType = grabbedWithType;
      if (attachedObject1.HasAttachFlag(Hand.AttachmentFlags.ParentToHand))
      {
        objectToAttach.transform.parent = this.transform;
        attachedObject1.isParentedToHand = true;
      }
      else
        attachedObject1.isParentedToHand = false;
      if (attachedObject1.HasAttachFlag(Hand.AttachmentFlags.SnapOnAttach))
      {
        if ((UnityEngine.Object) attachedObject1.interactable != (UnityEngine.Object) null && (UnityEngine.Object) attachedObject1.interactable.skeletonPoser != (UnityEngine.Object) null && this.HasSkeleton())
        {
          SteamVR_Skeleton_PoseSnapshot blendedPose = attachedObject1.interactable.skeletonPoser.GetBlendedPose(this.skeleton);
          objectToAttach.transform.position = this.transform.TransformPoint(blendedPose.position);
          objectToAttach.transform.rotation = this.transform.rotation * blendedPose.rotation;
          attachedObject1.initialPositionalOffset = attachedObject1.handAttachmentPointTransform.InverseTransformPoint(objectToAttach.transform.position);
          attachedObject1.initialRotationalOffset = Quaternion.Inverse(attachedObject1.handAttachmentPointTransform.rotation) * objectToAttach.transform.rotation;
        }
        else
        {
          if ((UnityEngine.Object) attachmentOffset != (UnityEngine.Object) null)
          {
            Quaternion quaternion = Quaternion.Inverse(attachmentOffset.transform.rotation) * objectToAttach.transform.rotation;
            objectToAttach.transform.rotation = attachedObject1.handAttachmentPointTransform.rotation * quaternion;
            Vector3 vector3 = objectToAttach.transform.position - attachmentOffset.transform.position;
            objectToAttach.transform.position = attachedObject1.handAttachmentPointTransform.position + vector3;
          }
          else
          {
            objectToAttach.transform.rotation = attachedObject1.handAttachmentPointTransform.rotation;
            objectToAttach.transform.position = attachedObject1.handAttachmentPointTransform.position;
          }
          Transform transform = objectToAttach.transform;
          attachedObject1.initialPositionalOffset = attachedObject1.handAttachmentPointTransform.InverseTransformPoint(transform.position);
          attachedObject1.initialRotationalOffset = Quaternion.Inverse(attachedObject1.handAttachmentPointTransform.rotation) * transform.rotation;
        }
      }
      else if ((UnityEngine.Object) attachedObject1.interactable != (UnityEngine.Object) null && (UnityEngine.Object) attachedObject1.interactable.skeletonPoser != (UnityEngine.Object) null && this.HasSkeleton())
      {
        attachedObject1.initialPositionalOffset = attachedObject1.handAttachmentPointTransform.InverseTransformPoint(objectToAttach.transform.position);
        attachedObject1.initialRotationalOffset = Quaternion.Inverse(attachedObject1.handAttachmentPointTransform.rotation) * objectToAttach.transform.rotation;
      }
      else if ((UnityEngine.Object) attachmentOffset != (UnityEngine.Object) null)
      {
        Quaternion quaternion1 = Quaternion.Inverse(attachmentOffset.transform.rotation) * objectToAttach.transform.rotation;
        Quaternion quaternion2 = attachedObject1.handAttachmentPointTransform.rotation * quaternion1 * Quaternion.Inverse(objectToAttach.transform.rotation);
        Vector3 vector3 = quaternion2 * objectToAttach.transform.position - quaternion2 * attachmentOffset.transform.position;
        attachedObject1.initialPositionalOffset = attachedObject1.handAttachmentPointTransform.InverseTransformPoint(attachedObject1.handAttachmentPointTransform.position + vector3);
        attachedObject1.initialRotationalOffset = Quaternion.Inverse(attachedObject1.handAttachmentPointTransform.rotation) * (attachedObject1.handAttachmentPointTransform.rotation * quaternion1);
      }
      else
      {
        attachedObject1.initialPositionalOffset = attachedObject1.handAttachmentPointTransform.InverseTransformPoint(objectToAttach.transform.position);
        attachedObject1.initialRotationalOffset = Quaternion.Inverse(attachedObject1.handAttachmentPointTransform.rotation) * objectToAttach.transform.rotation;
      }
      if (attachedObject1.HasAttachFlag(Hand.AttachmentFlags.TurnOnKinematic) && (UnityEngine.Object) attachedObject1.attachedRigidbody != (UnityEngine.Object) null)
      {
        attachedObject1.collisionDetectionMode = attachedObject1.attachedRigidbody.collisionDetectionMode;
        if (attachedObject1.collisionDetectionMode == CollisionDetectionMode.Continuous)
          attachedObject1.attachedRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        attachedObject1.attachedRigidbody.isKinematic = true;
      }
      if (attachedObject1.HasAttachFlag(Hand.AttachmentFlags.TurnOffGravity) && (UnityEngine.Object) attachedObject1.attachedRigidbody != (UnityEngine.Object) null)
        attachedObject1.attachedRigidbody.useGravity = false;
      if ((UnityEngine.Object) attachedObject1.interactable != (UnityEngine.Object) null && attachedObject1.interactable.attachEaseIn)
      {
        attachedObject1.attachedObject.transform.position = attachedObject1.easeSourcePosition;
        attachedObject1.attachedObject.transform.rotation = attachedObject1.easeSourceRotation;
      }
      this.attachedObjects.Add(attachedObject1);
      this.UpdateHovering();
      if (this.spewDebugText)
        this.HandDebugLog("AttachObject " + (object) objectToAttach);
      objectToAttach.SendMessage("OnAttachedToHand", (object) this, SendMessageOptions.DontRequireReceiver);
    }

    public bool ObjectIsAttached(GameObject go)
    {
      for (int index = 0; index < this.attachedObjects.Count; ++index)
      {
        if ((UnityEngine.Object) this.attachedObjects[index].attachedObject == (UnityEngine.Object) go)
          return true;
      }
      return false;
    }

    public void ForceHoverUnlock() => this.hoverLocked = false;

    public void DetachObject(GameObject objectToDetach, bool restoreOriginalParent = true)
    {
      int index = this.attachedObjects.FindIndex((Predicate<Hand.AttachedObject>) (l => (UnityEngine.Object) l.attachedObject == (UnityEngine.Object) objectToDetach));
      if (index != -1)
      {
        if (this.spewDebugText)
          this.HandDebugLog("DetachObject " + (object) objectToDetach);
        GameObject currentAttachedObject1 = this.currentAttachedObject;
        if ((UnityEngine.Object) this.attachedObjects[index].interactable != (UnityEngine.Object) null)
        {
          if (this.attachedObjects[index].interactable.hideHandOnAttach)
            this.Show();
          if (this.attachedObjects[index].interactable.hideSkeletonOnAttach && (UnityEngine.Object) this.mainRenderModel != (UnityEngine.Object) null && this.mainRenderModel.displayHandByDefault)
            this.ShowSkeleton();
          if (this.attachedObjects[index].interactable.hideControllerOnAttach && (UnityEngine.Object) this.mainRenderModel != (UnityEngine.Object) null && this.mainRenderModel.displayControllerByDefault)
            this.ShowController();
          if (this.attachedObjects[index].interactable.handAnimationOnPickup != 0)
            this.StopAnimation();
          if (this.attachedObjects[index].interactable.setRangeOfMotionOnPickup != SkeletalMotionRangeChange.None)
            this.ResetTemporarySkeletonRangeOfMotion();
        }
        Transform transform = (Transform) null;
        if (this.attachedObjects[index].isParentedToHand)
        {
          if (restoreOriginalParent && (UnityEngine.Object) this.attachedObjects[index].originalParent != (UnityEngine.Object) null)
            transform = this.attachedObjects[index].originalParent.transform;
          if ((UnityEngine.Object) this.attachedObjects[index].attachedObject != (UnityEngine.Object) null)
            this.attachedObjects[index].attachedObject.transform.parent = transform;
        }
        if (this.attachedObjects[index].HasAttachFlag(Hand.AttachmentFlags.TurnOnKinematic) && (UnityEngine.Object) this.attachedObjects[index].attachedRigidbody != (UnityEngine.Object) null)
        {
          this.attachedObjects[index].attachedRigidbody.isKinematic = this.attachedObjects[index].attachedRigidbodyWasKinematic;
          this.attachedObjects[index].attachedRigidbody.collisionDetectionMode = this.attachedObjects[index].collisionDetectionMode;
        }
        if (this.attachedObjects[index].HasAttachFlag(Hand.AttachmentFlags.TurnOffGravity) && (UnityEngine.Object) this.attachedObjects[index].attachedObject != (UnityEngine.Object) null && (UnityEngine.Object) this.attachedObjects[index].attachedRigidbody != (UnityEngine.Object) null)
          this.attachedObjects[index].attachedRigidbody.useGravity = this.attachedObjects[index].attachedRigidbodyUsedGravity;
        if ((UnityEngine.Object) this.attachedObjects[index].interactable != (UnityEngine.Object) null && (this.attachedObjects[index].interactable.handFollowTransform && this.HasSkeleton()))
        {
          this.skeleton.transform.localPosition = Vector3.zero;
          this.skeleton.transform.localRotation = Quaternion.identity;
        }
        if ((UnityEngine.Object) this.attachedObjects[index].attachedObject != (UnityEngine.Object) null)
        {
          if ((UnityEngine.Object) this.attachedObjects[index].interactable == (UnityEngine.Object) null || (UnityEngine.Object) this.attachedObjects[index].interactable != (UnityEngine.Object) null && !this.attachedObjects[index].interactable.isDestroying)
            this.attachedObjects[index].attachedObject.SetActive(true);
          this.attachedObjects[index].attachedObject.SendMessage("OnDetachedFromHand", (object) this, SendMessageOptions.DontRequireReceiver);
        }
        this.attachedObjects.RemoveAt(index);
        this.CleanUpAttachedObjectStack();
        GameObject currentAttachedObject2 = this.currentAttachedObject;
        this.hoverLocked = false;
        if ((UnityEngine.Object) currentAttachedObject2 != (UnityEngine.Object) null && (UnityEngine.Object) currentAttachedObject2 != (UnityEngine.Object) currentAttachedObject1)
        {
          currentAttachedObject2.SetActive(true);
          currentAttachedObject2.SendMessage("OnHandFocusAcquired", (object) this, SendMessageOptions.DontRequireReceiver);
        }
      }
      this.CleanUpAttachedObjectStack();
      if ((UnityEngine.Object) this.mainRenderModel != (UnityEngine.Object) null)
        this.mainRenderModel.MatchHandToTransform(this.mainRenderModel.transform);
      if (!((UnityEngine.Object) this.hoverhighlightRenderModel != (UnityEngine.Object) null))
        return;
      this.hoverhighlightRenderModel.MatchHandToTransform(this.hoverhighlightRenderModel.transform);
    }

    public Vector3 GetTrackedObjectVelocity(float timeOffset = 0.0f)
    {
      if ((UnityEngine.Object) this.trackedObject == (UnityEngine.Object) null)
      {
        Vector3 velocityTarget;
        this.GetUpdatedAttachedVelocities(this.currentAttachedObjectInfo.Value, out velocityTarget, out Vector3 _);
        return velocityTarget;
      }
      if (!this.isActive)
        return Vector3.zero;
      if ((double) timeOffset == 0.0)
        return Player.instance.trackingOriginTransform.TransformVector(this.trackedObject.GetVelocity());
      Vector3 velocity;
      this.trackedObject.GetVelocitiesAtTimeOffset(timeOffset, out velocity, out Vector3 _);
      return Player.instance.trackingOriginTransform.TransformVector(velocity);
    }

    public Vector3 GetTrackedObjectAngularVelocity(float timeOffset = 0.0f)
    {
      if ((UnityEngine.Object) this.trackedObject == (UnityEngine.Object) null)
      {
        Vector3 angularTarget;
        this.GetUpdatedAttachedVelocities(this.currentAttachedObjectInfo.Value, out Vector3 _, out angularTarget);
        return angularTarget;
      }
      if (!this.isActive)
        return Vector3.zero;
      if ((double) timeOffset == 0.0)
        return Player.instance.trackingOriginTransform.TransformDirection(this.trackedObject.GetAngularVelocity());
      Vector3 angularVelocity;
      this.trackedObject.GetVelocitiesAtTimeOffset(timeOffset, out Vector3 _, out angularVelocity);
      return Player.instance.trackingOriginTransform.TransformDirection(angularVelocity);
    }

    public void GetEstimatedPeakVelocities(out Vector3 velocity, out Vector3 angularVelocity)
    {
      this.trackedObject.GetEstimatedPeakVelocities(out velocity, out angularVelocity);
      velocity = Player.instance.trackingOriginTransform.TransformVector(velocity);
      angularVelocity = Player.instance.trackingOriginTransform.TransformDirection(angularVelocity);
    }

    private void CleanUpAttachedObjectStack() => this.attachedObjects.RemoveAll((Predicate<Hand.AttachedObject>) (l => (UnityEngine.Object) l.attachedObject == (UnityEngine.Object) null));

    protected virtual void Awake()
    {
      this.inputFocusAction = SteamVR_Events.InputFocusAction(new UnityAction<bool>(this.OnInputFocus));
      if ((UnityEngine.Object) this.hoverSphereTransform == (UnityEngine.Object) null)
        this.hoverSphereTransform = this.transform;
      if ((UnityEngine.Object) this.objectAttachmentPoint == (UnityEngine.Object) null)
        this.objectAttachmentPoint = this.transform;
      this.applicationLostFocusObject = new GameObject("_application_lost_focus");
      this.applicationLostFocusObject.transform.parent = this.transform;
      this.applicationLostFocusObject.SetActive(false);
      if (!((UnityEngine.Object) this.trackedObject == (UnityEngine.Object) null))
        return;
      this.trackedObject = this.gameObject.GetComponent<SteamVR_Behaviour_Pose>();
      if (!((UnityEngine.Object) this.trackedObject != (UnityEngine.Object) null))
        return;
      this.trackedObject.onTransformUpdatedEvent += new SteamVR_Behaviour_Pose.UpdateHandler(this.OnTransformUpdated);
    }

    protected virtual void OnDestroy()
    {
      if (!((UnityEngine.Object) this.trackedObject != (UnityEngine.Object) null))
        return;
      this.trackedObject.onTransformUpdatedEvent -= new SteamVR_Behaviour_Pose.UpdateHandler(this.OnTransformUpdated);
    }

    protected virtual void OnTransformUpdated(
      SteamVR_Behaviour_Pose updatedPose,
      SteamVR_Input_Sources updatedSource)
    {
      this.HandFollowUpdate();
    }

    [DebuggerHidden]
    protected virtual IEnumerator Start() => (IEnumerator) new Hand.\u003CStart\u003Ec__Iterator0()
    {
      \u0024this = this
    };

    protected virtual void UpdateHovering()
    {
      if ((UnityEngine.Object) this.noSteamVRFallbackCamera == (UnityEngine.Object) null && !this.isActive || (this.hoverLocked || this.applicationLostFocusObject.activeSelf))
        return;
      float maxValue = float.MaxValue;
      Interactable closestInteractable = (Interactable) null;
      if (this.useHoverSphere)
        this.CheckHoveringForTransform(this.hoverSphereTransform.position, this.hoverSphereRadius * Mathf.Abs(SteamVR_Utils.GetLossyScale(this.hoverSphereTransform)), ref maxValue, ref closestInteractable, Color.green);
      if (this.useControllerHoverComponent && (UnityEngine.Object) this.mainRenderModel != (UnityEngine.Object) null && this.mainRenderModel.IsControllerVisibile())
      {
        float num = this.controllerHoverRadius * Mathf.Abs(SteamVR_Utils.GetLossyScale(this.transform));
        this.CheckHoveringForTransform(this.mainRenderModel.GetControllerPosition(this.controllerHoverComponent), num / 2f, ref maxValue, ref closestInteractable, Color.blue);
      }
      if (this.useFingerJointHover && (UnityEngine.Object) this.mainRenderModel != (UnityEngine.Object) null && this.mainRenderModel.IsHandVisibile())
      {
        float num = this.fingerJointHoverRadius * Mathf.Abs(SteamVR_Utils.GetLossyScale(this.transform));
        this.CheckHoveringForTransform(this.mainRenderModel.GetBonePosition((int) this.fingerJointHover), num / 2f, ref maxValue, ref closestInteractable, Color.yellow);
      }
      this.hoveringInteractable = closestInteractable;
    }

    protected virtual bool CheckHoveringForTransform(
      Vector3 hoverPosition,
      float hoverRadius,
      ref float closestDistance,
      ref Interactable closestInteractable,
      Color debugColor)
    {
      bool flag1 = false;
      for (int index = 0; index < this.overlappingColliders.Length; ++index)
        this.overlappingColliders[index] = (Collider) null;
      if (Physics.OverlapSphereNonAlloc(hoverPosition, hoverRadius, this.overlappingColliders, this.hoverLayerMask.value) == 16)
        UnityEngine.Debug.LogWarning((object) ("<b>[SteamVR Interaction]</b> This hand is overlapping the max number of colliders: " + (object) 16 + ". Some collisions may be missed. Increase ColliderArraySize on Hand.cs"));
      int num1 = 0;
      for (int index1 = 0; index1 < this.overlappingColliders.Length; ++index1)
      {
        Collider overlappingCollider = this.overlappingColliders[index1];
        if (!((UnityEngine.Object) overlappingCollider == (UnityEngine.Object) null))
        {
          Interactable componentInParent = overlappingCollider.GetComponentInParent<Interactable>();
          if (!((UnityEngine.Object) componentInParent == (UnityEngine.Object) null))
          {
            IgnoreHovering component = overlappingCollider.GetComponent<IgnoreHovering>();
            if (!((UnityEngine.Object) component != (UnityEngine.Object) null) || !((UnityEngine.Object) component.onlyIgnoreHand == (UnityEngine.Object) null) && !((UnityEngine.Object) component.onlyIgnoreHand == (UnityEngine.Object) this))
            {
              bool flag2 = false;
              for (int index2 = 0; index2 < this.attachedObjects.Count; ++index2)
              {
                if ((UnityEngine.Object) this.attachedObjects[index2].attachedObject == (UnityEngine.Object) componentInParent.gameObject)
                {
                  flag2 = true;
                  break;
                }
              }
              if (!flag2 && (!(bool) (UnityEngine.Object) this.otherHand || !((UnityEngine.Object) this.otherHand.hoveringInteractable == (UnityEngine.Object) componentInParent)))
              {
                float num2 = Vector3.Distance(componentInParent.transform.position, hoverPosition);
                if ((double) num2 < (double) closestDistance)
                {
                  closestDistance = num2;
                  closestInteractable = componentInParent;
                  flag1 = true;
                }
                ++num1;
              }
            }
          }
        }
      }
      if (this.showDebugInteractables && flag1)
        UnityEngine.Debug.DrawLine(hoverPosition, closestInteractable.transform.position, debugColor, 0.05f, false);
      if (num1 > 0 && num1 != this.prevOverlappingColliders)
      {
        this.prevOverlappingColliders = num1;
        if (this.spewDebugText)
          this.HandDebugLog("Found " + (object) num1 + " overlapping colliders.");
      }
      return flag1;
    }

    protected virtual void UpdateNoSteamVRFallback()
    {
      if (!(bool) (UnityEngine.Object) this.noSteamVRFallbackCamera)
        return;
      Ray ray = this.noSteamVRFallbackCamera.ScreenPointToRay(Input.mousePosition);
      if (this.attachedObjects.Count > 0)
      {
        this.transform.position = ray.origin + this.noSteamVRFallbackInteractorDistance * ray.direction;
      }
      else
      {
        Vector3 position = this.transform.position;
        this.transform.position = this.noSteamVRFallbackCamera.transform.forward * -1000f;
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, this.noSteamVRFallbackMaxDistanceNoItem))
        {
          this.transform.position = hitInfo.point;
          this.noSteamVRFallbackInteractorDistance = Mathf.Min(this.noSteamVRFallbackMaxDistanceNoItem, hitInfo.distance);
        }
        else if ((double) this.noSteamVRFallbackInteractorDistance > 0.0)
          this.transform.position = ray.origin + Mathf.Min(this.noSteamVRFallbackMaxDistanceNoItem, this.noSteamVRFallbackInteractorDistance) * ray.direction;
        else
          this.transform.position = position;
      }
    }

    private void UpdateDebugText()
    {
      if (this.showDebugText)
      {
        if ((UnityEngine.Object) this.debugText == (UnityEngine.Object) null)
        {
          this.debugText = new GameObject("_debug_text").AddComponent<TextMesh>();
          this.debugText.fontSize = 120;
          this.debugText.characterSize = 1f / 1000f;
          this.debugText.transform.parent = this.transform;
          this.debugText.transform.localRotation = Quaternion.Euler(90f, 0.0f, 0.0f);
        }
        if (this.handType == SteamVR_Input_Sources.RightHand)
        {
          this.debugText.transform.localPosition = new Vector3(-0.05f, 0.0f, 0.0f);
          this.debugText.alignment = TextAlignment.Right;
          this.debugText.anchor = TextAnchor.UpperRight;
        }
        else
        {
          this.debugText.transform.localPosition = new Vector3(0.05f, 0.0f, 0.0f);
          this.debugText.alignment = TextAlignment.Left;
          this.debugText.anchor = TextAnchor.UpperLeft;
        }
        this.debugText.text = string.Format("Hovering: {0}\nHover Lock: {1}\nAttached: {2}\nTotal Attached: {3}\nType: {4}\n", !(bool) (UnityEngine.Object) this.hoveringInteractable ? (object) "null" : (object) this.hoveringInteractable.gameObject.name, (object) this.hoverLocked, !(bool) (UnityEngine.Object) this.currentAttachedObject ? (object) "null" : (object) this.currentAttachedObject.name, (object) this.attachedObjects.Count, (object) this.handType.ToString());
      }
      else
      {
        if (!((UnityEngine.Object) this.debugText != (UnityEngine.Object) null))
          return;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.debugText.gameObject);
      }
    }

    protected virtual void OnEnable()
    {
      this.inputFocusAction.enabled = true;
      float time = !((UnityEngine.Object) this.otherHand != (UnityEngine.Object) null) || this.otherHand.GetInstanceID() >= this.GetInstanceID() ? 0.0f : 0.5f * this.hoverUpdateInterval;
      this.InvokeRepeating("UpdateHovering", time, this.hoverUpdateInterval);
      this.InvokeRepeating("UpdateDebugText", time, this.hoverUpdateInterval);
    }

    protected virtual void OnDisable()
    {
      this.inputFocusAction.enabled = false;
      this.CancelInvoke();
    }

    protected virtual void Update()
    {
      this.UpdateNoSteamVRFallback();
      GameObject currentAttachedObject = this.currentAttachedObject;
      if ((UnityEngine.Object) currentAttachedObject != (UnityEngine.Object) null)
        currentAttachedObject.SendMessage("HandAttachedUpdate", (object) this, SendMessageOptions.DontRequireReceiver);
      if (!(bool) (UnityEngine.Object) this.hoveringInteractable)
        return;
      this.hoveringInteractable.SendMessage("HandHoverUpdate", (object) this, SendMessageOptions.DontRequireReceiver);
    }

    public bool IsStillHovering(Interactable interactable) => (UnityEngine.Object) this.hoveringInteractable == (UnityEngine.Object) interactable;

    protected virtual void HandFollowUpdate()
    {
      if (!((UnityEngine.Object) this.currentAttachedObject != (UnityEngine.Object) null) || !((UnityEngine.Object) this.currentAttachedObjectInfo.Value.interactable != (UnityEngine.Object) null))
        return;
      SteamVR_Skeleton_PoseSnapshot skeletonPoseSnapshot = (SteamVR_Skeleton_PoseSnapshot) null;
      if ((UnityEngine.Object) this.currentAttachedObjectInfo.Value.interactable.skeletonPoser != (UnityEngine.Object) null && this.HasSkeleton())
        skeletonPoseSnapshot = this.currentAttachedObjectInfo.Value.interactable.skeletonPoser.GetBlendedPose(this.skeleton);
      if (!this.currentAttachedObjectInfo.Value.interactable.handFollowTransform)
        return;
      Quaternion newRotation;
      Vector3 newPosition;
      if (skeletonPoseSnapshot == null)
      {
        newRotation = this.currentAttachedObjectInfo.Value.interactable.transform.rotation * Quaternion.Inverse(Quaternion.Inverse(this.transform.rotation) * this.currentAttachedObjectInfo.Value.handAttachmentPointTransform.rotation);
        Vector3 vector3 = this.transform.position - this.currentAttachedObjectInfo.Value.handAttachmentPointTransform.position;
        newPosition = this.currentAttachedObjectInfo.Value.interactable.transform.position + this.mainRenderModel.GetHandRotation() * Quaternion.Inverse(this.transform.rotation) * vector3;
      }
      else
      {
        Transform transform = this.currentAttachedObjectInfo.Value.attachedObject.transform;
        Vector3 position1 = transform.position;
        Quaternion rotation = transform.transform.rotation;
        transform.position = this.TargetItemPosition(this.currentAttachedObjectInfo.Value);
        transform.rotation = this.TargetItemRotation(this.currentAttachedObjectInfo.Value);
        Vector3 position2 = transform.InverseTransformPoint(this.transform.position);
        Quaternion quaternion = Quaternion.Inverse(transform.rotation) * this.transform.rotation;
        transform.position = position1;
        transform.rotation = rotation;
        newPosition = transform.TransformPoint(position2);
        newRotation = transform.rotation * quaternion;
      }
      if ((UnityEngine.Object) this.mainRenderModel != (UnityEngine.Object) null)
        this.mainRenderModel.SetHandRotation(newRotation);
      if ((UnityEngine.Object) this.hoverhighlightRenderModel != (UnityEngine.Object) null)
        this.hoverhighlightRenderModel.SetHandRotation(newRotation);
      if ((UnityEngine.Object) this.mainRenderModel != (UnityEngine.Object) null)
        this.mainRenderModel.SetHandPosition(newPosition);
      if (!((UnityEngine.Object) this.hoverhighlightRenderModel != (UnityEngine.Object) null))
        return;
      this.hoverhighlightRenderModel.SetHandPosition(newPosition);
    }

    protected virtual void FixedUpdate()
    {
      if (!((UnityEngine.Object) this.currentAttachedObject != (UnityEngine.Object) null))
        return;
      Hand.AttachedObject attachedObject = this.currentAttachedObjectInfo.Value;
      if (!((UnityEngine.Object) attachedObject.attachedObject != (UnityEngine.Object) null))
        return;
      if (attachedObject.HasAttachFlag(Hand.AttachmentFlags.VelocityMovement))
      {
        if (!attachedObject.interactable.attachEaseIn || attachedObject.interactable.snapAttachEaseInCompleted)
          this.UpdateAttachedVelocity(attachedObject);
      }
      else if (attachedObject.HasAttachFlag(Hand.AttachmentFlags.ParentToHand))
      {
        attachedObject.attachedObject.transform.position = this.TargetItemPosition(attachedObject);
        attachedObject.attachedObject.transform.rotation = this.TargetItemRotation(attachedObject);
      }
      if (!attachedObject.interactable.attachEaseIn)
        return;
      float time = Util.RemapNumberClamped(Time.time, attachedObject.attachTime, attachedObject.attachTime + attachedObject.interactable.snapAttachEaseInTime, 0.0f, 1f);
      if ((double) time < 1.0)
      {
        if (attachedObject.HasAttachFlag(Hand.AttachmentFlags.VelocityMovement))
        {
          attachedObject.attachedRigidbody.velocity = Vector3.zero;
          attachedObject.attachedRigidbody.angularVelocity = Vector3.zero;
        }
        float t = attachedObject.interactable.snapAttachEaseInCurve.Evaluate(time);
        attachedObject.attachedObject.transform.position = Vector3.Lerp(attachedObject.easeSourcePosition, this.TargetItemPosition(attachedObject), t);
        attachedObject.attachedObject.transform.rotation = Quaternion.Lerp(attachedObject.easeSourceRotation, this.TargetItemRotation(attachedObject), t);
      }
      else
      {
        if (attachedObject.interactable.snapAttachEaseInCompleted)
          return;
        attachedObject.interactable.gameObject.SendMessage("OnThrowableAttachEaseInCompleted", (object) this, SendMessageOptions.DontRequireReceiver);
        attachedObject.interactable.snapAttachEaseInCompleted = true;
      }
    }

    protected void UpdateAttachedVelocity(Hand.AttachedObject attachedObjectInfo)
    {
      Vector3 velocityTarget;
      Vector3 angularTarget;
      if (!this.GetUpdatedAttachedVelocities(attachedObjectInfo, out velocityTarget, out angularTarget))
        return;
      float lossyScale = SteamVR_Utils.GetLossyScale(this.currentAttachedObjectInfo.Value.handAttachmentPointTransform);
      float maxDistanceDelta1 = 20f * lossyScale;
      float maxDistanceDelta2 = 10f * lossyScale;
      attachedObjectInfo.attachedRigidbody.velocity = Vector3.MoveTowards(attachedObjectInfo.attachedRigidbody.velocity, velocityTarget, maxDistanceDelta2);
      attachedObjectInfo.attachedRigidbody.angularVelocity = Vector3.MoveTowards(attachedObjectInfo.attachedRigidbody.angularVelocity, angularTarget, maxDistanceDelta1);
    }

    public void ResetAttachedTransform(Hand.AttachedObject attachedObject)
    {
      attachedObject.attachedObject.transform.position = this.TargetItemPosition(attachedObject);
      attachedObject.attachedObject.transform.rotation = this.TargetItemRotation(attachedObject);
    }

    protected Vector3 TargetItemPosition(Hand.AttachedObject attachedObject) => (UnityEngine.Object) attachedObject.interactable != (UnityEngine.Object) null && (UnityEngine.Object) attachedObject.interactable.skeletonPoser != (UnityEngine.Object) null && this.HasSkeleton() ? this.currentAttachedObjectInfo.Value.handAttachmentPointTransform.TransformPoint(attachedObject.handAttachmentPointTransform.InverseTransformPoint(this.transform.TransformPoint(attachedObject.interactable.skeletonPoser.GetBlendedPose(this.skeleton).position))) : this.currentAttachedObjectInfo.Value.handAttachmentPointTransform.TransformPoint(attachedObject.initialPositionalOffset);

    protected Quaternion TargetItemRotation(Hand.AttachedObject attachedObject) => (UnityEngine.Object) attachedObject.interactable != (UnityEngine.Object) null && (UnityEngine.Object) attachedObject.interactable.skeletonPoser != (UnityEngine.Object) null && this.HasSkeleton() ? this.currentAttachedObjectInfo.Value.handAttachmentPointTransform.rotation * (Quaternion.Inverse(attachedObject.handAttachmentPointTransform.rotation) * (this.transform.rotation * attachedObject.interactable.skeletonPoser.GetBlendedPose(this.skeleton).rotation)) : this.currentAttachedObjectInfo.Value.handAttachmentPointTransform.rotation * attachedObject.initialRotationalOffset;

    protected bool GetUpdatedAttachedVelocities(
      Hand.AttachedObject attachedObjectInfo,
      out Vector3 velocityTarget,
      out Vector3 angularTarget)
    {
      bool flag = false;
      float num1 = 6000f;
      float num2 = 50f;
      Vector3 vector3 = this.TargetItemPosition(attachedObjectInfo) - attachedObjectInfo.attachedRigidbody.position;
      velocityTarget = vector3 * num1 * Time.deltaTime;
      if (!float.IsNaN(velocityTarget.x) && !float.IsInfinity(velocityTarget.x))
      {
        if ((bool) (UnityEngine.Object) this.noSteamVRFallbackCamera)
          velocityTarget /= 10f;
        flag = true;
      }
      else
        velocityTarget = Vector3.zero;
      float angle;
      Vector3 axis;
      (this.TargetItemRotation(attachedObjectInfo) * Quaternion.Inverse(attachedObjectInfo.attachedObject.transform.rotation)).ToAngleAxis(out angle, out axis);
      if ((double) angle > 180.0)
        angle -= 360f;
      if ((double) angle != 0.0 && !float.IsNaN(axis.x) && !float.IsInfinity(axis.x))
      {
        angularTarget = angle * axis * num2 * Time.deltaTime;
        if ((bool) (UnityEngine.Object) this.noSteamVRFallbackCamera)
          angularTarget /= 10f;
        flag = ((flag ? 1 : 0) & 1) != 0;
      }
      else
        angularTarget = Vector3.zero;
      return flag;
    }

    protected virtual void OnInputFocus(bool hasFocus)
    {
      if (hasFocus)
      {
        this.DetachObject(this.applicationLostFocusObject);
        this.applicationLostFocusObject.SetActive(false);
        this.UpdateHovering();
        this.BroadcastMessage("OnParentHandInputFocusAcquired", SendMessageOptions.DontRequireReceiver);
      }
      else
      {
        this.applicationLostFocusObject.SetActive(true);
        this.AttachObject(this.applicationLostFocusObject, GrabTypes.Scripted, Hand.AttachmentFlags.ParentToHand);
        this.BroadcastMessage("OnParentHandInputFocusLost", SendMessageOptions.DontRequireReceiver);
      }
    }

    protected virtual void OnDrawGizmos()
    {
      if (this.useHoverSphere && (UnityEngine.Object) this.hoverSphereTransform != (UnityEngine.Object) null)
      {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.hoverSphereTransform.position, this.hoverSphereRadius * Mathf.Abs(SteamVR_Utils.GetLossyScale(this.hoverSphereTransform)) / 2f);
      }
      if (this.useControllerHoverComponent && (UnityEngine.Object) this.mainRenderModel != (UnityEngine.Object) null && this.mainRenderModel.IsControllerVisibile())
      {
        Gizmos.color = Color.blue;
        float num = this.controllerHoverRadius * Mathf.Abs(SteamVR_Utils.GetLossyScale(this.transform));
        Gizmos.DrawWireSphere(this.mainRenderModel.GetControllerPosition(this.controllerHoverComponent), num / 2f);
      }
      if (!this.useFingerJointHover || !((UnityEngine.Object) this.mainRenderModel != (UnityEngine.Object) null) || !this.mainRenderModel.IsHandVisibile())
        return;
      Gizmos.color = Color.yellow;
      float num1 = this.fingerJointHoverRadius * Mathf.Abs(SteamVR_Utils.GetLossyScale(this.transform));
      Gizmos.DrawWireSphere(this.mainRenderModel.GetBonePosition((int) this.fingerJointHover), num1 / 2f);
    }

    private void HandDebugLog(string msg)
    {
      if (!this.spewDebugText)
        return;
      UnityEngine.Debug.Log((object) ("<b>[SteamVR Interaction]</b> Hand (" + this.name + "): " + msg));
    }

    public void HoverLock(Interactable interactable)
    {
      if (this.spewDebugText)
        this.HandDebugLog("HoverLock " + (object) interactable);
      this.hoverLocked = true;
      this.hoveringInteractable = interactable;
    }

    public void HoverUnlock(Interactable interactable)
    {
      if (this.spewDebugText)
        this.HandDebugLog("HoverUnlock " + (object) interactable);
      if (!((UnityEngine.Object) this.hoveringInteractable == (UnityEngine.Object) interactable))
        return;
      this.hoverLocked = false;
    }

    public void TriggerHapticPulse(ushort microSecondsDuration)
    {
      float durationSeconds = (float) microSecondsDuration / 1000000f;
      this.hapticAction.Execute(0.0f, durationSeconds, 1f / durationSeconds, 1f, this.handType);
    }

    public void TriggerHapticPulse(float duration, float frequency, float amplitude) => this.hapticAction.Execute(0.0f, duration, frequency, amplitude, this.handType);

    public void ShowGrabHint() => ControllerButtonHints.ShowButtonHint(this, (ISteamVR_Action_In_Source) this.grabGripAction);

    public void HideGrabHint() => ControllerButtonHints.HideButtonHint(this, (ISteamVR_Action_In_Source) this.grabGripAction);

    public void ShowGrabHint(string text) => ControllerButtonHints.ShowTextHint(this, (ISteamVR_Action_In_Source) this.grabGripAction, text);

    public GrabTypes GetGrabStarting(GrabTypes explicitType = GrabTypes.None)
    {
      if (explicitType != GrabTypes.None)
      {
        if ((bool) (UnityEngine.Object) this.noSteamVRFallbackCamera && Input.GetMouseButtonDown(0))
          return explicitType;
        if (explicitType == GrabTypes.Pinch && this.grabPinchAction.GetStateDown(this.handType))
          return GrabTypes.Pinch;
        if (explicitType == GrabTypes.Grip && this.grabGripAction.GetStateDown(this.handType))
          return GrabTypes.Grip;
      }
      else
      {
        if ((bool) (UnityEngine.Object) this.noSteamVRFallbackCamera && Input.GetMouseButtonDown(0))
          return GrabTypes.Grip;
        if (this.grabPinchAction.GetStateDown(this.handType))
          return GrabTypes.Pinch;
        if (this.grabGripAction.GetStateDown(this.handType))
          return GrabTypes.Grip;
      }
      return GrabTypes.None;
    }

    public GrabTypes GetGrabEnding(GrabTypes explicitType = GrabTypes.None)
    {
      if (explicitType != GrabTypes.None)
      {
        if ((bool) (UnityEngine.Object) this.noSteamVRFallbackCamera && Input.GetMouseButtonUp(0))
          return explicitType;
        if (explicitType == GrabTypes.Pinch && this.grabPinchAction.GetStateUp(this.handType))
          return GrabTypes.Pinch;
        if (explicitType == GrabTypes.Grip && this.grabGripAction.GetStateUp(this.handType))
          return GrabTypes.Grip;
      }
      else
      {
        if ((bool) (UnityEngine.Object) this.noSteamVRFallbackCamera && Input.GetMouseButtonUp(0))
          return GrabTypes.Grip;
        if (this.grabPinchAction.GetStateUp(this.handType))
          return GrabTypes.Pinch;
        if (this.grabGripAction.GetStateUp(this.handType))
          return GrabTypes.Grip;
      }
      return GrabTypes.None;
    }

    public bool IsGrabEnding(GameObject attachedObject)
    {
      for (int index = 0; index < this.attachedObjects.Count; ++index)
      {
        if ((UnityEngine.Object) this.attachedObjects[index].attachedObject == (UnityEngine.Object) attachedObject)
          return !this.IsGrabbingWithType(this.attachedObjects[index].grabbedWithType);
      }
      return false;
    }

    public bool IsGrabbingWithType(GrabTypes type)
    {
      if ((bool) (UnityEngine.Object) this.noSteamVRFallbackCamera && Input.GetMouseButton(0))
        return true;
      if (type == GrabTypes.Pinch)
        return this.grabPinchAction.GetState(this.handType);
      return type == GrabTypes.Grip && this.grabGripAction.GetState(this.handType);
    }

    public bool IsGrabbingWithOppositeType(GrabTypes type)
    {
      if ((bool) (UnityEngine.Object) this.noSteamVRFallbackCamera && Input.GetMouseButton(0))
        return true;
      if (type == GrabTypes.Pinch)
        return this.grabGripAction.GetState(this.handType);
      return type == GrabTypes.Grip && this.grabPinchAction.GetState(this.handType);
    }

    public GrabTypes GetBestGrabbingType() => this.GetBestGrabbingType(GrabTypes.None);

    public GrabTypes GetBestGrabbingType(GrabTypes preferred, bool forcePreference = false)
    {
      if ((bool) (UnityEngine.Object) this.noSteamVRFallbackCamera && Input.GetMouseButton(0))
        return preferred;
      if (preferred == GrabTypes.Pinch)
      {
        if (this.grabPinchAction.GetState(this.handType))
          return GrabTypes.Pinch;
        if (forcePreference)
          return GrabTypes.None;
      }
      if (preferred == GrabTypes.Grip)
      {
        if (this.grabGripAction.GetState(this.handType))
          return GrabTypes.Grip;
        if (forcePreference)
          return GrabTypes.None;
      }
      if (this.grabPinchAction.GetState(this.handType))
        return GrabTypes.Pinch;
      return this.grabGripAction.GetState(this.handType) ? GrabTypes.Grip : GrabTypes.None;
    }

    private void InitController()
    {
      if (this.spewDebugText)
        this.HandDebugLog("Hand " + this.name + " connected with type " + this.handType.ToString());
      bool flag = (UnityEngine.Object) this.mainRenderModel != (UnityEngine.Object) null;
      EVRSkeletalMotionRange newRangeOfMotion = EVRSkeletalMotionRange.WithController;
      if (flag)
        newRangeOfMotion = this.mainRenderModel.GetSkeletonRangeOfMotion;
      foreach (RenderModel renderModel in this.renderModels)
      {
        if ((UnityEngine.Object) renderModel != (UnityEngine.Object) null)
          UnityEngine.Object.Destroy((UnityEngine.Object) renderModel.gameObject);
      }
      this.renderModels.Clear();
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.renderModelPrefab);
      gameObject.layer = this.gameObject.layer;
      gameObject.tag = this.gameObject.tag;
      gameObject.transform.parent = this.transform;
      gameObject.transform.localPosition = Vector3.zero;
      gameObject.transform.localRotation = Quaternion.identity;
      gameObject.transform.localScale = this.renderModelPrefab.transform.localScale;
      int deviceIndex = this.trackedObject.GetDeviceIndex();
      this.mainRenderModel = gameObject.GetComponent<RenderModel>();
      this.renderModels.Add(this.mainRenderModel);
      if (flag)
        this.mainRenderModel.SetSkeletonRangeOfMotion(newRangeOfMotion);
      this.BroadcastMessage("SetInputSource", (object) this.handType, SendMessageOptions.DontRequireReceiver);
      this.BroadcastMessage("OnHandInitialized", (object) deviceIndex, SendMessageOptions.DontRequireReceiver);
    }

    public void SetRenderModel(GameObject prefab)
    {
      this.renderModelPrefab = prefab;
      if (!((UnityEngine.Object) this.mainRenderModel != (UnityEngine.Object) null) || !this.isPoseValid)
        return;
      this.InitController();
    }

    public void SetHoverRenderModel(RenderModel hoverRenderModel)
    {
      this.hoverhighlightRenderModel = hoverRenderModel;
      this.renderModels.Add(hoverRenderModel);
    }

    public int GetDeviceIndex() => this.trackedObject.GetDeviceIndex();

    [Flags]
    public enum AttachmentFlags
    {
      SnapOnAttach = 1,
      DetachOthers = 2,
      DetachFromOtherHand = 4,
      ParentToHand = 8,
      VelocityMovement = 16, // 0x00000010
      TurnOnKinematic = 32, // 0x00000020
      TurnOffGravity = 64, // 0x00000040
      AllowSidegrade = 128, // 0x00000080
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
      public Hand.AttachmentFlags attachmentFlags;
      public Vector3 initialPositionalOffset;
      public Quaternion initialRotationalOffset;
      public Transform attachedOffsetTransform;
      public Transform handAttachmentPointTransform;
      public Vector3 easeSourcePosition;
      public Quaternion easeSourceRotation;
      public float attachTime;

      public bool HasAttachFlag(Hand.AttachmentFlags flag) => (this.attachmentFlags & flag) == flag;
    }
  }
}
