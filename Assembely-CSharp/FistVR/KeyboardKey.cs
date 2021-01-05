using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class KeyboardKey : FVRInteractiveObject
	{
		public enum KeyBoardKeyType
		{
			AlphaNumeric,
			Space,
			Tab,
			Shift,
			Caps,
			Backspace,
			Enter
		}

		[Header("Key Options")]
		public Keyboard Keyboard;

		public Text Text;

		public KeyBoardKeyType KeyType;

		public string LowerCase;

		public string UpperCase;

		private float timeSinceHit = 1f;

		public float reHitThreshold = 0.1f;

		protected override void Awake()
		{
			base.Awake();
			Text = GetComponent<Text>();
		}

		public override void Poke(FVRViveHand hand)
		{
			base.Poke(hand);
			if (timeSinceHit >= reHitThreshold)
			{
				timeSinceHit = 0f;
				Keyboard.KeyInput(KeyType, LowerCase, UpperCase);
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (timeSinceHit < 1f)
			{
				timeSinceHit += Time.deltaTime;
			}
		}

		public void Press()
		{
			Keyboard.KeyInput(KeyType, LowerCase, UpperCase);
		}
	}
}
