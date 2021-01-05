using UnityEngine;

namespace FistVR
{
	public class MomStuff : MonoBehaviour
	{
		public AudioClip[] OrnamentShatterSounds;

		private int m_ShatterIndex;

		public AudioSource MomSound;

		private float curHeight = -800f;

		private float tarHeight = -800f;

		private bool isSpeaking;

		private void Awake()
		{
			MomSound = GetComponent<AudioSource>();
		}

		public void InitiateMom()
		{
			if (!isSpeaking && !MomSound.isPlaying && m_ShatterIndex < OrnamentShatterSounds.Length)
			{
				isSpeaking = true;
				tarHeight = -113f;
				MomSound.clip = OrnamentShatterSounds[m_ShatterIndex];
				Invoke("Speak", 2f);
				m_ShatterIndex++;
			}
		}

		private void Speak()
		{
			MomSound.Play();
			isSpeaking = false;
		}

		private void Update()
		{
			if (isSpeaking)
			{
				curHeight = Mathf.Lerp(curHeight, tarHeight, Time.deltaTime * 1f);
			}
			else
			{
				curHeight = Mathf.Lerp(curHeight, tarHeight, Time.deltaTime * 0.2f);
			}
			base.transform.position = new Vector3(base.transform.position.x, curHeight, base.transform.position.z);
			if (!MomSound.isPlaying && !isSpeaking)
			{
				tarHeight = -800f;
			}
		}
	}
}
