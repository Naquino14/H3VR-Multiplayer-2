using UnityEngine;

namespace FistVR
{
	public class FlamingSwordFire : MonoBehaviour
	{
		public AudioSource BurningAudio;

		private float m_burningVolume;

		public FVRPhysicalObject PObject;

		public ParticleSystem PSystem;

		private float m_curVolume;

		private void Start()
		{
		}

		private void Update()
		{
			float num = 0f;
			float target = 0f;
			if (PObject.IsHeld)
			{
				num = 200f;
				target = 0.3f;
			}
			else
			{
				num = 0f;
			}
			m_curVolume = Mathf.MoveTowards(m_curVolume, target, Time.deltaTime);
			if (m_curVolume < 0.01f)
			{
				if (BurningAudio.isPlaying)
				{
					BurningAudio.Stop();
				}
			}
			else
			{
				if (!BurningAudio.isPlaying)
				{
					BurningAudio.Play();
				}
				BurningAudio.volume = m_curVolume;
			}
			ParticleSystem.EmissionModule emission = PSystem.emission;
			ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
			rateOverTime.mode = ParticleSystemCurveMode.Constant;
			rateOverTime.constantMax = num;
			rateOverTime.constantMin = num;
			emission.rateOverTime = rateOverTime;
		}
	}
}
