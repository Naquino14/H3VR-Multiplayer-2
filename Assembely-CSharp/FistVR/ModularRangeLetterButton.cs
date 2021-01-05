using UnityEngine;

namespace FistVR
{
	public class ModularRangeLetterButton : FVRInteractiveObject
	{
		public Vector3 OutPosition;

		public Vector3 InPosition;

		public Transform ButtonObject;

		private float m_buttonPos;

		private AudioSource aud;

		public GameObject PokeyTarget;

		public string MessageName;

		public string Value;

		public AudioClip Yay;

		public AudioClip NotYay;

		public float VolMod = 1f;

		protected override void Awake()
		{
			base.Awake();
			aud = GetComponent<AudioSource>();
		}

		public override bool IsInteractable()
		{
			return false;
		}

		public override void Poke(FVRViveHand hand)
		{
			base.Poke(hand);
			if (!(m_buttonPos < 0.5f))
			{
				return;
			}
			m_buttonPos = 1f;
			if (aud != null)
			{
				aud.pitch = Random.Range(0.85f, 1.15f);
			}
			if (PokeyTarget != null)
			{
				PokeyTarget.SendMessage(MessageName, Value);
				if (aud != null)
				{
					aud.PlayOneShot(Yay, VolMod);
				}
			}
			else if (aud != null)
			{
				aud.PlayOneShot(NotYay, VolMod);
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			m_buttonPos = Mathf.Lerp(m_buttonPos, 0f, Time.deltaTime * 3f);
			if (ButtonObject != null)
			{
				ButtonObject.localPosition = Vector3.Lerp(OutPosition, InPosition, m_buttonPos);
			}
		}
	}
}
