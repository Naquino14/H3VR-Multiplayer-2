using UnityEngine;

namespace FistVR
{
	public class PokeBuzzer : FVRInteractiveObject
	{
		public Material Mat_Unpushed;

		public Material Mat_Pushed;

		private Renderer m_rend;

		private AudioSource m_aud;

		private bool m_hasBeenPressed;

		public GameObject Target;

		public string Method;

		protected override void Awake()
		{
			base.Awake();
			m_rend = GetComponent<Renderer>();
			m_aud = GetComponent<AudioSource>();
		}

		public override void Poke(FVRViveHand hand)
		{
			base.Poke(hand);
			if (!m_hasBeenPressed)
			{
				m_hasBeenPressed = true;
				Press();
			}
		}

		private void Press()
		{
			m_rend.material = Mat_Pushed;
			if (Target != null)
			{
				Target.SendMessage(Method);
			}
			m_aud.PlayOneShot(m_aud.clip, 0.5f);
		}

		private void Reset()
		{
			m_rend.material = Mat_Unpushed;
			m_hasBeenPressed = false;
		}
	}
}
