using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(Interactable))]
	public class ComplexThrowable : MonoBehaviour
	{
		public enum AttachMode
		{
			FixedJoint,
			Force
		}

		public float attachForce = 800f;

		public float attachForceDamper = 25f;

		public AttachMode attachMode;

		[EnumFlags]
		public Hand.AttachmentFlags attachmentFlags;

		private List<Hand> holdingHands = new List<Hand>();

		private List<Rigidbody> holdingBodies = new List<Rigidbody>();

		private List<Vector3> holdingPoints = new List<Vector3>();

		private List<Rigidbody> rigidBodies = new List<Rigidbody>();

		private void Awake()
		{
			GetComponentsInChildren(rigidBodies);
		}

		private void Update()
		{
			for (int i = 0; i < holdingHands.Count; i++)
			{
				if (holdingHands[i].IsGrabEnding(base.gameObject))
				{
					PhysicsDetach(holdingHands[i]);
				}
			}
		}

		private void OnHandHoverBegin(Hand hand)
		{
			if (holdingHands.IndexOf(hand) == -1 && hand.isActive)
			{
				hand.TriggerHapticPulse(800);
			}
		}

		private void OnHandHoverEnd(Hand hand)
		{
			if (holdingHands.IndexOf(hand) == -1 && hand.isActive)
			{
				hand.TriggerHapticPulse(500);
			}
		}

		private void HandHoverUpdate(Hand hand)
		{
			GrabTypes grabStarting = hand.GetGrabStarting();
			if (grabStarting != 0)
			{
				PhysicsAttach(hand, grabStarting);
			}
		}

		private void PhysicsAttach(Hand hand, GrabTypes startingGrabType)
		{
			PhysicsDetach(hand);
			Rigidbody rigidbody = null;
			Vector3 zero = Vector3.zero;
			float num = float.MaxValue;
			for (int i = 0; i < rigidBodies.Count; i++)
			{
				float num2 = Vector3.Distance(rigidBodies[i].worldCenterOfMass, hand.transform.position);
				if (num2 < num)
				{
					rigidbody = rigidBodies[i];
					num = num2;
				}
			}
			if (!(rigidbody == null))
			{
				if (attachMode == AttachMode.FixedJoint)
				{
					Rigidbody rigidbody2 = Util.FindOrAddComponent<Rigidbody>(hand.gameObject);
					rigidbody2.isKinematic = true;
					FixedJoint fixedJoint = hand.gameObject.AddComponent<FixedJoint>();
					fixedJoint.connectedBody = rigidbody;
				}
				hand.HoverLock(null);
				Vector3 vector = hand.transform.position - rigidbody.worldCenterOfMass;
				vector = Mathf.Min(vector.magnitude, 1f) * vector.normalized;
				zero = rigidbody.transform.InverseTransformPoint(rigidbody.worldCenterOfMass + vector);
				hand.AttachObject(base.gameObject, startingGrabType, attachmentFlags);
				holdingHands.Add(hand);
				holdingBodies.Add(rigidbody);
				holdingPoints.Add(zero);
			}
		}

		private bool PhysicsDetach(Hand hand)
		{
			int num = holdingHands.IndexOf(hand);
			if (num != -1)
			{
				holdingHands[num].DetachObject(base.gameObject, restoreOriginalParent: false);
				holdingHands[num].HoverUnlock(null);
				if (attachMode == AttachMode.FixedJoint)
				{
					Object.Destroy(holdingHands[num].GetComponent<FixedJoint>());
				}
				Util.FastRemove(holdingHands, num);
				Util.FastRemove(holdingBodies, num);
				Util.FastRemove(holdingPoints, num);
				return true;
			}
			return false;
		}

		private void FixedUpdate()
		{
			if (attachMode == AttachMode.Force)
			{
				for (int i = 0; i < holdingHands.Count; i++)
				{
					Vector3 vector = holdingBodies[i].transform.TransformPoint(holdingPoints[i]);
					Vector3 vector2 = holdingHands[i].transform.position - vector;
					holdingBodies[i].AddForceAtPosition(attachForce * vector2, vector, ForceMode.Acceleration);
					holdingBodies[i].AddForceAtPosition((0f - attachForceDamper) * holdingBodies[i].GetPointVelocity(vector), vector, ForceMode.Acceleration);
				}
			}
		}
	}
}
