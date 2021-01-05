using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class ArrowHand : MonoBehaviour
	{
		private Hand hand;

		private Longbow bow;

		private GameObject currentArrow;

		public GameObject arrowPrefab;

		public Transform arrowNockTransform;

		public float nockDistance = 0.1f;

		public float lerpCompleteDistance = 0.08f;

		public float rotationLerpThreshold = 0.15f;

		public float positionLerpThreshold = 0.15f;

		private bool allowArrowSpawn = true;

		private bool nocked;

		private GrabTypes nockedWithType;

		private bool inNockRange;

		private bool arrowLerpComplete;

		public SoundPlayOneshot arrowSpawnSound;

		private AllowTeleportWhileAttachedToHand allowTeleport;

		public int maxArrowCount = 10;

		private List<GameObject> arrowList;

		private void Awake()
		{
			allowTeleport = GetComponent<AllowTeleportWhileAttachedToHand>();
			allowTeleport.overrideHoverLock = false;
			arrowList = new List<GameObject>();
		}

		private void OnAttachedToHand(Hand attachedHand)
		{
			hand = attachedHand;
			FindBow();
		}

		private GameObject InstantiateArrow()
		{
			GameObject gameObject = Object.Instantiate(arrowPrefab, arrowNockTransform.position, arrowNockTransform.rotation);
			gameObject.name = "Bow Arrow";
			gameObject.transform.parent = arrowNockTransform;
			Util.ResetTransform(gameObject.transform);
			arrowList.Add(gameObject);
			while (arrowList.Count > maxArrowCount)
			{
				GameObject gameObject2 = arrowList[0];
				arrowList.RemoveAt(0);
				if ((bool)gameObject2)
				{
					Object.Destroy(gameObject2);
				}
			}
			return gameObject;
		}

		private void HandAttachedUpdate(Hand hand)
		{
			if (bow == null)
			{
				FindBow();
			}
			if (bow == null)
			{
				return;
			}
			if (allowArrowSpawn && currentArrow == null)
			{
				currentArrow = InstantiateArrow();
				arrowSpawnSound.Play();
			}
			float num = Vector3.Distance(base.transform.parent.position, bow.nockTransform.position);
			if (!nocked)
			{
				if (num < rotationLerpThreshold)
				{
					float t = Util.RemapNumber(num, rotationLerpThreshold, lerpCompleteDistance, 0f, 1f);
					arrowNockTransform.rotation = Quaternion.Lerp(arrowNockTransform.parent.rotation, bow.nockRestTransform.rotation, t);
				}
				else
				{
					arrowNockTransform.localRotation = Quaternion.identity;
				}
				if (num < positionLerpThreshold)
				{
					float value = Util.RemapNumber(num, positionLerpThreshold, lerpCompleteDistance, 0f, 1f);
					value = Mathf.Clamp(value, 0f, 1f);
					arrowNockTransform.position = Vector3.Lerp(arrowNockTransform.parent.position, bow.nockRestTransform.position, value);
				}
				else
				{
					arrowNockTransform.position = arrowNockTransform.parent.position;
				}
				if (num < lerpCompleteDistance)
				{
					if (!arrowLerpComplete)
					{
						arrowLerpComplete = true;
						hand.TriggerHapticPulse(500);
					}
				}
				else if (arrowLerpComplete)
				{
					arrowLerpComplete = false;
				}
				if (num < nockDistance)
				{
					if (!inNockRange)
					{
						inNockRange = true;
						bow.ArrowInPosition();
					}
				}
				else if (inNockRange)
				{
					inNockRange = false;
				}
				GrabTypes bestGrabbingType = hand.GetBestGrabbingType(GrabTypes.Pinch, forcePreference: true);
				if (num < nockDistance && bestGrabbingType != 0 && !nocked)
				{
					if (currentArrow == null)
					{
						currentArrow = InstantiateArrow();
					}
					nocked = true;
					nockedWithType = bestGrabbingType;
					bow.StartNock(this);
					hand.HoverLock(GetComponent<Interactable>());
					allowTeleport.teleportAllowed = false;
					currentArrow.transform.parent = bow.nockTransform;
					Util.ResetTransform(currentArrow.transform);
					Util.ResetTransform(arrowNockTransform);
				}
			}
			if (nocked && !hand.IsGrabbingWithType(nockedWithType))
			{
				if (bow.pulled)
				{
					FireArrow();
				}
				else
				{
					arrowNockTransform.rotation = currentArrow.transform.rotation;
					currentArrow.transform.parent = arrowNockTransform;
					Util.ResetTransform(currentArrow.transform);
					nocked = false;
					nockedWithType = GrabTypes.None;
					bow.ReleaseNock();
					hand.HoverUnlock(GetComponent<Interactable>());
					allowTeleport.teleportAllowed = true;
				}
				bow.StartRotationLerp();
			}
		}

		private void OnDetachedFromHand(Hand hand)
		{
			Object.Destroy(base.gameObject);
		}

		private void FireArrow()
		{
			currentArrow.transform.parent = null;
			Arrow component = currentArrow.GetComponent<Arrow>();
			component.shaftRB.isKinematic = false;
			component.shaftRB.useGravity = true;
			component.shaftRB.transform.GetComponent<BoxCollider>().enabled = true;
			component.arrowHeadRB.isKinematic = false;
			component.arrowHeadRB.useGravity = true;
			component.arrowHeadRB.transform.GetComponent<BoxCollider>().enabled = true;
			component.arrowHeadRB.AddForce(currentArrow.transform.forward * bow.GetArrowVelocity(), ForceMode.VelocityChange);
			component.arrowHeadRB.AddTorque(currentArrow.transform.forward * 10f);
			nocked = false;
			nockedWithType = GrabTypes.None;
			currentArrow.GetComponent<Arrow>().ArrowReleased(bow.GetArrowVelocity());
			bow.ArrowReleased();
			allowArrowSpawn = false;
			Invoke("EnableArrowSpawn", 0.5f);
			StartCoroutine(ArrowReleaseHaptics());
			currentArrow = null;
			allowTeleport.teleportAllowed = true;
		}

		private void EnableArrowSpawn()
		{
			allowArrowSpawn = true;
		}

		private IEnumerator ArrowReleaseHaptics()
		{
			yield return new WaitForSeconds(0.05f);
			hand.otherHand.TriggerHapticPulse(1500);
			yield return new WaitForSeconds(0.05f);
			hand.otherHand.TriggerHapticPulse(800);
			yield return new WaitForSeconds(0.05f);
			hand.otherHand.TriggerHapticPulse(500);
			yield return new WaitForSeconds(0.05f);
			hand.otherHand.TriggerHapticPulse(300);
		}

		private void OnHandFocusLost(Hand hand)
		{
			base.gameObject.SetActive(value: false);
		}

		private void OnHandFocusAcquired(Hand hand)
		{
			base.gameObject.SetActive(value: true);
		}

		private void FindBow()
		{
			bow = hand.otherHand.GetComponentInChildren<Longbow>();
		}
	}
}
