using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(Interactable))]
	public class HapticRack : MonoBehaviour
	{
		[Tooltip("The linear mapping driving the haptic rack")]
		public LinearMapping linearMapping;

		[Tooltip("The number of haptic pulses evenly distributed along the mapping")]
		public int teethCount = 128;

		[Tooltip("Minimum duration of the haptic pulse")]
		public int minimumPulseDuration = 500;

		[Tooltip("Maximum duration of the haptic pulse")]
		public int maximumPulseDuration = 900;

		[Tooltip("This event is triggered every time a haptic pulse is made")]
		public UnityEvent onPulse;

		private Hand hand;

		private int previousToothIndex = -1;

		private void Awake()
		{
			if (linearMapping == null)
			{
				linearMapping = GetComponent<LinearMapping>();
			}
		}

		private void OnHandHoverBegin(Hand hand)
		{
			this.hand = hand;
		}

		private void OnHandHoverEnd(Hand hand)
		{
			this.hand = null;
		}

		private void Update()
		{
			int num = Mathf.RoundToInt(linearMapping.value * (float)teethCount - 0.5f);
			if (num != previousToothIndex)
			{
				Pulse();
				previousToothIndex = num;
			}
		}

		private void Pulse()
		{
			if ((bool)hand && hand.isActive && hand.GetBestGrabbingType() != 0)
			{
				ushort microSecondsDuration = (ushort)Random.Range(minimumPulseDuration, maximumPulseDuration + 1);
				hand.TriggerHapticPulse(microSecondsDuration);
				onPulse.Invoke();
			}
		}
	}
}
