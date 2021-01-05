using System.Collections;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(Interactable))]
	public class Longbow : MonoBehaviour
	{
		public enum Handedness
		{
			Left,
			Right
		}

		public Handedness currentHandGuess;

		private float timeOfPossibleHandSwitch;

		private float timeBeforeConfirmingHandSwitch = 1.5f;

		private bool possibleHandSwitch;

		public Transform pivotTransform;

		public Transform handleTransform;

		private Hand hand;

		private ArrowHand arrowHand;

		public Transform nockTransform;

		public Transform nockRestTransform;

		public bool autoSpawnArrowHand = true;

		public ItemPackage arrowHandItemPackage;

		public GameObject arrowHandPrefab;

		public bool nocked;

		public bool pulled;

		private const float minPull = 0.05f;

		private const float maxPull = 0.5f;

		private float nockDistanceTravelled;

		private float hapticDistanceThreshold = 0.01f;

		private float lastTickDistance;

		private const float bowPullPulseStrengthLow = 100f;

		private const float bowPullPulseStrengthHigh = 500f;

		private Vector3 bowLeftVector;

		public float arrowMinVelocity = 3f;

		public float arrowMaxVelocity = 30f;

		private float arrowVelocity = 30f;

		private float minStrainTickTime = 0.1f;

		private float maxStrainTickTime = 0.5f;

		private float nextStrainTick;

		private bool lerpBackToZeroRotation;

		private float lerpStartTime;

		private float lerpDuration = 0.15f;

		private Quaternion lerpStartRotation;

		private float nockLerpStartTime;

		private Quaternion nockLerpStartRotation;

		public float drawOffset = 0.06f;

		public LinearMapping bowDrawLinearMapping;

		private Vector3 lateUpdatePos;

		private Quaternion lateUpdateRot;

		public SoundBowClick drawSound;

		private float drawTension;

		public SoundPlayOneshot arrowSlideSound;

		public SoundPlayOneshot releaseSound;

		public SoundPlayOneshot nockSound;

		private SteamVR_Events.Action newPosesAppliedAction;

		private void OnAttachedToHand(Hand attachedHand)
		{
			hand = attachedHand;
		}

		private void HandAttachedUpdate(Hand hand)
		{
			EvaluateHandedness();
			if (nocked)
			{
				Vector3 lhs = arrowHand.arrowNockTransform.parent.position - nockRestTransform.position;
				float num = Util.RemapNumberClamped(Time.time, nockLerpStartTime, nockLerpStartTime + lerpDuration, 0f, 1f);
				float num2 = Util.RemapNumberClamped(lhs.magnitude, 0.05f, 0.5f, 0f, 1f);
				Vector3 normalized = (Player.instance.hmdTransform.position + Vector3.down * 0.05f - arrowHand.arrowNockTransform.parent.position).normalized;
				Vector3 vector = arrowHand.arrowNockTransform.parent.position + normalized * drawOffset * num2;
				Vector3 normalized2 = (vector - pivotTransform.position).normalized;
				Vector3 normalized3 = (handleTransform.position - pivotTransform.position).normalized;
				bowLeftVector = -Vector3.Cross(normalized3, normalized2);
				pivotTransform.rotation = Quaternion.Lerp(nockLerpStartRotation, Quaternion.LookRotation(normalized2, bowLeftVector), num);
				if (Vector3.Dot(lhs, -nockTransform.forward) > 0f)
				{
					float num3 = lhs.magnitude * num;
					nockTransform.localPosition = new Vector3(0f, 0f, Mathf.Clamp(0f - num3, -0.5f, 0f));
					nockDistanceTravelled = 0f - nockTransform.localPosition.z;
					arrowVelocity = Util.RemapNumber(nockDistanceTravelled, 0.05f, 0.5f, arrowMinVelocity, arrowMaxVelocity);
					drawTension = Util.RemapNumberClamped(nockDistanceTravelled, 0f, 0.5f, 0f, 1f);
					bowDrawLinearMapping.value = drawTension;
					if (nockDistanceTravelled > 0.05f)
					{
						pulled = true;
					}
					else
					{
						pulled = false;
					}
					if (nockDistanceTravelled > lastTickDistance + hapticDistanceThreshold || nockDistanceTravelled < lastTickDistance - hapticDistanceThreshold)
					{
						ushort microSecondsDuration = (ushort)Util.RemapNumber(nockDistanceTravelled, 0f, 0.5f, 100f, 500f);
						hand.TriggerHapticPulse(microSecondsDuration);
						hand.otherHand.TriggerHapticPulse(microSecondsDuration);
						drawSound.PlayBowTensionClicks(drawTension);
						lastTickDistance = nockDistanceTravelled;
					}
					if (nockDistanceTravelled >= 0.5f && Time.time > nextStrainTick)
					{
						hand.TriggerHapticPulse(400);
						hand.otherHand.TriggerHapticPulse(400);
						drawSound.PlayBowTensionClicks(drawTension);
						nextStrainTick = Time.time + Random.Range(minStrainTickTime, maxStrainTickTime);
					}
				}
				else
				{
					nockTransform.localPosition = new Vector3(0f, 0f, 0f);
					bowDrawLinearMapping.value = 0f;
				}
			}
			else if (lerpBackToZeroRotation)
			{
				float num4 = Util.RemapNumber(Time.time, lerpStartTime, lerpStartTime + lerpDuration, 0f, 1f);
				pivotTransform.localRotation = Quaternion.Lerp(lerpStartRotation, Quaternion.identity, num4);
				if (num4 >= 1f)
				{
					lerpBackToZeroRotation = false;
				}
			}
		}

		public void ArrowReleased()
		{
			nocked = false;
			hand.HoverUnlock(GetComponent<Interactable>());
			hand.otherHand.HoverUnlock(arrowHand.GetComponent<Interactable>());
			if (releaseSound != null)
			{
				releaseSound.Play();
			}
			StartCoroutine(ResetDrawAnim());
		}

		private IEnumerator ResetDrawAnim()
		{
			float startTime = Time.time;
			float startLerp = drawTension;
			while (Time.time < startTime + 0.02f)
			{
				float lerp = Util.RemapNumberClamped(Time.time, startTime, startTime + 0.02f, startLerp, 0f);
				bowDrawLinearMapping.value = lerp;
				yield return null;
			}
			bowDrawLinearMapping.value = 0f;
		}

		public float GetArrowVelocity()
		{
			return arrowVelocity;
		}

		public void StartRotationLerp()
		{
			lerpStartTime = Time.time;
			lerpBackToZeroRotation = true;
			lerpStartRotation = pivotTransform.localRotation;
			Util.ResetTransform(nockTransform);
		}

		public void StartNock(ArrowHand currentArrowHand)
		{
			arrowHand = currentArrowHand;
			hand.HoverLock(GetComponent<Interactable>());
			nocked = true;
			nockLerpStartTime = Time.time;
			nockLerpStartRotation = pivotTransform.rotation;
			arrowSlideSound.Play();
			DoHandednessCheck();
		}

		private void EvaluateHandedness()
		{
			SteamVR_Input_Sources handType = hand.handType;
			if (handType == SteamVR_Input_Sources.LeftHand)
			{
				if (possibleHandSwitch && currentHandGuess == Handedness.Left)
				{
					possibleHandSwitch = false;
				}
				if (!possibleHandSwitch && currentHandGuess == Handedness.Right)
				{
					possibleHandSwitch = true;
					timeOfPossibleHandSwitch = Time.time;
				}
				if (possibleHandSwitch && Time.time > timeOfPossibleHandSwitch + timeBeforeConfirmingHandSwitch)
				{
					currentHandGuess = Handedness.Left;
					possibleHandSwitch = false;
				}
			}
			else
			{
				if (possibleHandSwitch && currentHandGuess == Handedness.Right)
				{
					possibleHandSwitch = false;
				}
				if (!possibleHandSwitch && currentHandGuess == Handedness.Left)
				{
					possibleHandSwitch = true;
					timeOfPossibleHandSwitch = Time.time;
				}
				if (possibleHandSwitch && Time.time > timeOfPossibleHandSwitch + timeBeforeConfirmingHandSwitch)
				{
					currentHandGuess = Handedness.Right;
					possibleHandSwitch = false;
				}
			}
		}

		private void DoHandednessCheck()
		{
			if (currentHandGuess == Handedness.Left)
			{
				pivotTransform.localScale = new Vector3(1f, 1f, 1f);
			}
			else
			{
				pivotTransform.localScale = new Vector3(1f, -1f, 1f);
			}
		}

		public void ArrowInPosition()
		{
			DoHandednessCheck();
			if (nockSound != null)
			{
				nockSound.Play();
			}
		}

		public void ReleaseNock()
		{
			nocked = false;
			hand.HoverUnlock(GetComponent<Interactable>());
			StartCoroutine(ResetDrawAnim());
		}

		private void ShutDown()
		{
			if (hand != null && hand.otherHand.currentAttachedObject != null && hand.otherHand.currentAttachedObject.GetComponent<ItemPackageReference>() != null && hand.otherHand.currentAttachedObject.GetComponent<ItemPackageReference>().itemPackage == arrowHandItemPackage)
			{
				hand.otherHand.DetachObject(hand.otherHand.currentAttachedObject);
			}
		}

		private void OnHandFocusLost(Hand hand)
		{
			base.gameObject.SetActive(value: false);
		}

		private void OnHandFocusAcquired(Hand hand)
		{
			base.gameObject.SetActive(value: true);
			OnAttachedToHand(hand);
		}

		private void OnDetachedFromHand(Hand hand)
		{
			Object.Destroy(base.gameObject);
		}

		private void OnDestroy()
		{
			ShutDown();
		}
	}
}
