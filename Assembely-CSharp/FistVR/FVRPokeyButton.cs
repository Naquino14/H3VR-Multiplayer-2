using UnityEngine;

namespace FistVR
{
	public class FVRPokeyButton : FVRInteractiveObject
	{
		public Vector3 OutPosition;

		public Vector3 InPosition;

		public Transform ButtonObject;

		private float m_buttonPos;

		private AudioSource aud;

		public GameObject PokeyTarget;

		public string MessageName;

		public int NumValue;

		public AudioClip Yay;

		public AudioClip NotYay;

		public float VolMod = 1f;

		private bool m_hasAud;

		protected override void Awake()
		{
			base.Awake();
			aud = GetComponent<AudioSource>();
			if (aud != null)
			{
				m_hasAud = true;
			}
		}

		public override bool IsInteractable()
		{
			return false;
		}

		public override void Poke(FVRViveHand hand)
		{
			base.Poke(hand);
			hand.Buzz(hand.Buzzer.Buzz_GunShot);
			if (!(m_buttonPos < 0.5f))
			{
				return;
			}
			m_buttonPos = 1f;
			if (m_hasAud)
			{
				aud.pitch = Random.Range(0.85f, 1.15f);
			}
			if (PokeyTarget != null)
			{
				PokeyTarget.SendMessage(MessageName, NumValue);
				if (m_hasAud)
				{
					aud.PlayOneShot(Yay, VolMod);
				}
			}
			else if (m_hasAud)
			{
				aud.PlayOneShot(NotYay, VolMod);
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (m_buttonPos > 0f)
			{
				m_buttonPos = Mathf.MoveTowards(m_buttonPos, 0f, 3f);
				if (ButtonObject != null)
				{
					ButtonObject.localPosition = Vector3.Lerp(OutPosition, InPosition, m_buttonPos);
				}
			}
		}
	}
}
