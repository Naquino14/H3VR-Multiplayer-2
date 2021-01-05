using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class FVRTriggerGUIButton : FVRInteractiveObject
	{
		private Button button;

		private float coolDown;

		public float pressCooldown = 0.5f;

		public AudioClip PressedSound;

		public AudioSource AudioSource;

		public float volume;

		private new void Awake()
		{
			button = GetComponent<Button>();
		}

		public void Update()
		{
			if (coolDown > 0f)
			{
				coolDown -= Time.deltaTime;
			}
		}

		public override void Poke(FVRViveHand hand)
		{
			if (coolDown <= 0f)
			{
				base.Poke(hand);
				button.onClick.Invoke();
				if (AudioSource != null)
				{
					AudioSource.PlayOneShot(PressedSound, volume);
				}
				coolDown = Mathf.Max(pressCooldown, 0.5f);
			}
		}
	}
}
