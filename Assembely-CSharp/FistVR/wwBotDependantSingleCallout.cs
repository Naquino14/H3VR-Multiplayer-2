using UnityEngine;

namespace FistVR
{
	public class wwBotDependantSingleCallout : MonoBehaviour
	{
		public GameObject Bot;

		public AudioSource Aud;

		private bool m_hasPlayed;

		private void OnTriggerEnter()
		{
			if (!m_hasPlayed)
			{
				m_hasPlayed = true;
				Aud.Play();
			}
		}
	}
}
