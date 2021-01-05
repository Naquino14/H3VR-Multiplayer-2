// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Throwable
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
  [RequireComponent(typeof (Interactable))]
  [RequireComponent(typeof (Rigidbody))]
  [RequireComponent(typeof (VelocityEstimator))]
  public class Throwable : MonoBehaviour
  {
    [EnumFlags]
    [Tooltip("The flags used to attach this object to the hand.")]
    public Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.TurnOnKinematic;
    [Tooltip("The local point which acts as a positional and rotational offset to use while held")]
    public Transform attachmentOffset;
    [Tooltip("How fast must this object be moving to attach due to a trigger hold instead of a trigger press? (-1 to disable)")]
    public float catchingSpeedThreshold = -1f;
    public ReleaseStyle releaseVelocityStyle = ReleaseStyle.GetFromHand;
    [Tooltip("The time offset used when releasing the object with the RawFromHand option")]
    public float releaseVelocityTimeOffset = -11f / 1000f;
    public float scaleReleaseVelocity = 1.1f;
    [Tooltip("When detaching the object, should it return to its original parent?")]
    public bool restoreOriginalParent;
    protected VelocityEstimator velocityEstimator;
    protected bool attached;
    protected float attachTime;
    protected Vector3 attachPosition;
    protected Quaternion attachRotation;
    protected Transform attachEaseInTransform;
    public UnityEvent onPickUp;
    public UnityEvent onDetachFromHand;
    public UnityEvent<Hand> onHeldUpdate;
    protected RigidbodyInterpolation hadInterpolation;
    protected Rigidbody rigidbody;
    [HideInInspector]
    public Interactable interactable;

    protected virtual void Awake()
    {
      this.velocityEstimator = this.GetComponent<VelocityEstimator>();
      this.interactable = this.GetComponent<Interactable>();
      this.rigidbody = this.GetComponent<Rigidbody>();
      this.rigidbody.maxAngularVelocity = 50f;
      if (!((Object) this.attachmentOffset != (Object) null))
        ;
    }

    protected virtual void OnHandHoverBegin(Hand hand)
    {
      bool flag = false;
      if (!this.attached && (double) this.catchingSpeedThreshold != -1.0)
      {
        float num = this.catchingSpeedThreshold * SteamVR_Utils.GetLossyScale(Player.instance.trackingOriginTransform);
        GrabTypes bestGrabbingType = hand.GetBestGrabbingType();
        if (bestGrabbingType != GrabTypes.None && (double) this.rigidbody.velocity.magnitude >= (double) num)
        {
          hand.AttachObject(this.gameObject, bestGrabbingType, this.attachmentFlags);
          flag = false;
        }
      }
      if (!flag)
        return;
      hand.ShowGrabHint();
    }

    protected virtual void OnHandHoverEnd(Hand hand) => hand.HideGrabHint();

    protected virtual void HandHoverUpdate(Hand hand)
    {
      GrabTypes grabStarting = hand.GetGrabStarting();
      if (grabStarting == GrabTypes.None)
        return;
      hand.AttachObject(this.gameObject, grabStarting, this.attachmentFlags, this.attachmentOffset);
      hand.HideGrabHint();
    }

    protected virtual void OnAttachedToHand(Hand hand)
    {
      this.hadInterpolation = this.rigidbody.interpolation;
      this.attached = true;
      this.onPickUp.Invoke();
      hand.HoverLock((Interactable) null);
      this.rigidbody.interpolation = RigidbodyInterpolation.None;
      this.velocityEstimator.BeginEstimatingVelocity();
      this.attachTime = Time.time;
      this.attachPosition = this.transform.position;
      this.attachRotation = this.transform.rotation;
    }

    protected virtual void OnDetachedFromHand(Hand hand)
    {
      this.attached = false;
      this.onDetachFromHand.Invoke();
      hand.HoverUnlock((Interactable) null);
      this.rigidbody.interpolation = this.hadInterpolation;
      Vector3 velocity;
      Vector3 angularVelocity;
      this.GetReleaseVelocities(hand, out velocity, out angularVelocity);
      this.rigidbody.velocity = velocity;
      this.rigidbody.angularVelocity = angularVelocity;
    }

    public virtual void GetReleaseVelocities(
      Hand hand,
      out Vector3 velocity,
      out Vector3 angularVelocity)
    {
      if ((bool) (Object) hand.noSteamVRFallbackCamera && this.releaseVelocityStyle != ReleaseStyle.NoChange)
        this.releaseVelocityStyle = ReleaseStyle.ShortEstimation;
      switch (this.releaseVelocityStyle)
      {
        case ReleaseStyle.GetFromHand:
          velocity = hand.GetTrackedObjectVelocity(this.releaseVelocityTimeOffset);
          angularVelocity = hand.GetTrackedObjectAngularVelocity(this.releaseVelocityTimeOffset);
          break;
        case ReleaseStyle.ShortEstimation:
          this.velocityEstimator.FinishEstimatingVelocity();
          velocity = this.velocityEstimator.GetVelocityEstimate();
          angularVelocity = this.velocityEstimator.GetAngularVelocityEstimate();
          break;
        case ReleaseStyle.AdvancedEstimation:
          hand.GetEstimatedPeakVelocities(out velocity, out angularVelocity);
          break;
        default:
          velocity = this.rigidbody.velocity;
          angularVelocity = this.rigidbody.angularVelocity;
          break;
      }
      if (this.releaseVelocityStyle == ReleaseStyle.NoChange)
        return;
      velocity *= this.scaleReleaseVelocity;
    }

    protected virtual void HandAttachedUpdate(Hand hand)
    {
      if (hand.IsGrabEnding(this.gameObject))
        hand.DetachObject(this.gameObject, this.restoreOriginalParent);
      if (this.onHeldUpdate == null)
        return;
      this.onHeldUpdate.Invoke(hand);
    }

    [DebuggerHidden]
    protected virtual IEnumerator LateDetach(Hand hand) => (IEnumerator) new Throwable.\u003CLateDetach\u003Ec__Iterator0()
    {
      hand = hand,
      \u0024this = this
    };

    protected virtual void OnHandFocusAcquired(Hand hand)
    {
      this.gameObject.SetActive(true);
      this.velocityEstimator.BeginEstimatingVelocity();
    }

    protected virtual void OnHandFocusLost(Hand hand)
    {
      this.gameObject.SetActive(false);
      this.velocityEstimator.FinishEstimatingVelocity();
    }
  }
}
