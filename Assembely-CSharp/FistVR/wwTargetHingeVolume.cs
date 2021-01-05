using UnityEngine;

namespace FistVR
{
	public class wwTargetHingeVolume : MonoBehaviour
	{
		public AudioSource Aud;

		public HingeJoint Joint;

		private float m_lastAngle;

		public float volMult;

		public float volCutoff;

		public Vector2 PitchClamp;

		public float pitchMult;

		public float pitchMod;

		private void Update()
		{
			float angle = Joint.angle;
			float num = Mathf.Abs(angle - m_lastAngle) * Time.deltaTime;
			if (Aud != null && Aud.isPlaying)
			{
				Aud.volume = num * volMult - volCutoff;
				Aud.pitch = Mathf.Clamp(num * pitchMult - pitchMod, PitchClamp.x, PitchClamp.y);
			}
			m_lastAngle = angle;
		}
	}
}
