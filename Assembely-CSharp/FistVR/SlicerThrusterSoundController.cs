using UnityEngine;

namespace FistVR
{
	public class SlicerThrusterSoundController : MonoBehaviour
	{
		public AudioSource ThrusterAudio;

		private float m_curThrusterAudioIntensity;

		private float m_tarThrusterAudioIntensity;

		public AIThrusterControlBox[] ControlBoxes;

		private void Update()
		{
			float num = 0f;
			for (int i = 0; i < ControlBoxes.Length; i++)
			{
				if (!(ControlBoxes[i] != null))
				{
					continue;
				}
				for (int j = 0; j < ControlBoxes[i].Thrusters.Length; j++)
				{
					if (ControlBoxes[i].Thrusters[j] != null)
					{
						num += ControlBoxes[i].Thrusters[j].GetMagnitude();
					}
				}
			}
			m_tarThrusterAudioIntensity = num * 0.35f;
			float num2 = 0f;
			if (m_tarThrusterAudioIntensity > m_curThrusterAudioIntensity)
			{
				m_curThrusterAudioIntensity = Mathf.Lerp(m_curThrusterAudioIntensity, m_tarThrusterAudioIntensity, Time.deltaTime * 5f);
				num2 = Mathf.Clamp((m_tarThrusterAudioIntensity - m_curThrusterAudioIntensity) * 0.15f, 0f, 0.15f);
			}
			else
			{
				m_curThrusterAudioIntensity = Mathf.Lerp(m_curThrusterAudioIntensity, m_tarThrusterAudioIntensity, Time.deltaTime * 1f);
				num2 = 0f - Mathf.Clamp((m_curThrusterAudioIntensity - m_tarThrusterAudioIntensity) * 0.15f, 0f, 0.15f);
			}
			ThrusterAudio.pitch = 1f + num2;
			ThrusterAudio.volume = m_curThrusterAudioIntensity * 0.15f;
		}
	}
}
