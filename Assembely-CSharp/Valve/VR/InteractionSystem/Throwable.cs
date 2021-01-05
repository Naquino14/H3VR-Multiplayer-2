using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(Interactable))]
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(VelocityEstimator))]
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
		public float releaseVelocityTimeOffset = -0.011f;

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
			velocityEstimator = GetComponent<VelocityEstimator>();
			interactable = GetComponent<Interactable>();
			rigidbody = GetComponent<Rigidbody>();
			rigidbody.maxAngularVelocity = 50f;
			if (!(attachmentOffset != null))
			{
			}
		}

		protected virtual void OnHandHoverBegin(Hand hand)
		{
			bool flag = false;
			if (!attached && catchingSpeedThreshold != -1f)
			{
				float num = catchingSpeedThreshold * SteamVR_Utils.GetLossyScale(Player.instance.trackingOriginTransform);
				GrabTypes bestGrabbingType = hand.GetBestGrabbingType();
				if (bestGrabbingType != 0 && rigidbody.velocity.magnitude >= num)
				{
					hand.AttachObject(base.gameObject, bestGrabbingType, attachmentFlags);
					flag = false;
				}
			}
			if (flag)
			{
				hand.ShowGrabHint();
			}
		}

		protected virtual void OnHandHoverEnd(Hand hand)
		{
			hand.HideGrabHint();
		}

		protected virtual void HandHoverUpdate(Hand hand)
		{
			GrabTypes grabStarting = hand.GetGrabStarting();
			if (grabStarting != 0)
			{
				hand.AttachObject(base.gameObject, grabStarting, attachmentFlags, attachmentOffset);
				hand.HideGrabHint();
			}
		}

		protected virtual void OnAttachedToHand(Hand hand)
		{
			hadInterpolation = rigidbody.interpolation;
			attached = true;
			onPickUp.Invoke();
			hand.HoverLock(null);
			rigidbody.interpolation = RigidbodyInterpolation.None;
			velocityEstimator.BeginEstimatingVelocity();
			attachTime = Time.time;
			attachPosition = base.transform.position;
			attachRotation = base.transform.rotation;
		}

		protected virtual void OnDetachedFromHand(Hand hand)
		{
			attached = false;
			onDetachFromHand.Invoke();
			hand.HoverUnlock(null);
			rigidbody.interpolation = hadInterpolation;
			GetReleaseVelocities(hand, out var velocity, out var angularVelocity);
			rigidbody.velocity = velocity;
			rigidbody.angularVelocity = angularVelocity;
		}

		public virtual void GetReleaseVelocities(Hand hand, out Vector3 velocity, out Vector3 angularVelocity)
		{
			if ((bool)hand.noSteamVRFallbackCamera && releaseVelocityStyle != 0)
			{
				releaseVelocityStyle = ReleaseStyle.ShortEstimation;
			}
			switch (releaseVelocityStyle)
			{
			case ReleaseStyle.ShortEstimation:
				velocityEstimator.FinishEstimatingVelocity();
				velocity = velocityEstimator.GetVelocityEstimate();
				angularVelocity = velocityEstimator.GetAngularVelocityEstimate();
				break;
			case ReleaseStyle.AdvancedEstimation:
				hand.GetEstimatedPeakVelocities(out velocity, out angularVelocity);
				break;
			case ReleaseStyle.GetFromHand:
				velocity = hand.GetTrackedObjectVelocity(releaseVelocityTimeOffset);
				angularVelocity = hand.GetTrackedObjectAngularVelocity(releaseVelocityTimeOffset);
				break;
			default:
				velocity = rigidbody.velocity;
				angularVelocity = rigidbody.angularVelocity;
				break;
			}
			if (releaseVelocityStyle != 0)
			{
				velocity *= scaleReleaseVelocity;
			}
		}

		protected virtual void HandAttachedUpdate(Hand hand)
		{
			if (hand.IsGrabEnding(base.gameObject))
			{
				hand.DetachObject(base.gameObject, restoreOriginalParent);
			}
			if (onHeldUpdate != null)
			{
				onHeldUpdate.Invoke(hand);
			}
		}

		protected virtual IEnumerator LateDetach(Hand hand)
		{
			yield return new WaitForEndOfFrame();
			hand.DetachObject(base.gameObject, restoreOriginalParent);
		}

		protected virtual void OnHandFocusAcquired(Hand hand)
		{
			base.gameObject.SetActive(value: true);
			velocityEstimator.BeginEstimatingVelocity();
		}

		protected virtual void OnHandFocusLost(Hand hand)
		{
			base.gameObject.SetActive(value: false);
			velocityEstimator.FinishEstimatingVelocity();
		}
	}
}
