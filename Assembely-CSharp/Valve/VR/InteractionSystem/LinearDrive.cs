using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(Interactable))]
	public class LinearDrive : MonoBehaviour
	{
		public Transform startPosition;

		public Transform endPosition;

		public LinearMapping linearMapping;

		public bool repositionGameObject = true;

		public bool maintainMomemntum = true;

		public float momemtumDampenRate = 5f;

		protected Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.DetachFromOtherHand;

		protected float initialMappingOffset;

		protected int numMappingChangeSamples = 5;

		protected float[] mappingChangeSamples;

		protected float prevMapping;

		protected float mappingChangeRate;

		protected int sampleCount;

		protected Interactable interactable;

		protected virtual void Awake()
		{
			mappingChangeSamples = new float[numMappingChangeSamples];
			interactable = GetComponent<Interactable>();
		}

		protected virtual void Start()
		{
			if (linearMapping == null)
			{
				linearMapping = GetComponent<LinearMapping>();
			}
			if (linearMapping == null)
			{
				linearMapping = base.gameObject.AddComponent<LinearMapping>();
			}
			initialMappingOffset = linearMapping.value;
			if (repositionGameObject)
			{
				UpdateLinearMapping(base.transform);
			}
		}

		protected virtual void HandHoverUpdate(Hand hand)
		{
			GrabTypes grabStarting = hand.GetGrabStarting();
			if (interactable.attachedToHand == null && grabStarting != 0)
			{
				initialMappingOffset = linearMapping.value - CalculateLinearMapping(hand.transform);
				sampleCount = 0;
				mappingChangeRate = 0f;
				hand.AttachObject(base.gameObject, grabStarting, attachmentFlags);
			}
		}

		protected virtual void HandAttachedUpdate(Hand hand)
		{
			UpdateLinearMapping(hand.transform);
			if (hand.IsGrabEnding(base.gameObject))
			{
				hand.DetachObject(base.gameObject);
			}
		}

		protected virtual void OnDetachedFromHand(Hand hand)
		{
			CalculateMappingChangeRate();
		}

		protected void CalculateMappingChangeRate()
		{
			mappingChangeRate = 0f;
			int num = Mathf.Min(sampleCount, mappingChangeSamples.Length);
			if (num != 0)
			{
				for (int i = 0; i < num; i++)
				{
					mappingChangeRate += mappingChangeSamples[i];
				}
				mappingChangeRate /= num;
			}
		}

		protected void UpdateLinearMapping(Transform updateTransform)
		{
			prevMapping = linearMapping.value;
			linearMapping.value = Mathf.Clamp01(initialMappingOffset + CalculateLinearMapping(updateTransform));
			mappingChangeSamples[sampleCount % mappingChangeSamples.Length] = 1f / Time.deltaTime * (linearMapping.value - prevMapping);
			sampleCount++;
			if (repositionGameObject)
			{
				base.transform.position = Vector3.Lerp(startPosition.position, endPosition.position, linearMapping.value);
			}
		}

		protected float CalculateLinearMapping(Transform updateTransform)
		{
			Vector3 rhs = endPosition.position - startPosition.position;
			float magnitude = rhs.magnitude;
			rhs.Normalize();
			Vector3 lhs = updateTransform.position - startPosition.position;
			return Vector3.Dot(lhs, rhs) / magnitude;
		}

		protected virtual void Update()
		{
			if (maintainMomemntum && mappingChangeRate != 0f)
			{
				mappingChangeRate = Mathf.Lerp(mappingChangeRate, 0f, momemtumDampenRate * Time.deltaTime);
				linearMapping.value = Mathf.Clamp01(linearMapping.value + mappingChangeRate * Time.deltaTime);
				if (repositionGameObject)
				{
					base.transform.position = Vector3.Lerp(startPosition.position, endPosition.position, linearMapping.value);
				}
			}
		}
	}
}
