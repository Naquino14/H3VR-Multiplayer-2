using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FlameTrap : MonoBehaviour
	{
		public enum FlameTrapType
		{
			Waving,
			Spinning
		}

		public FlameTrapType Type;

		public Transform WavingPiece;

		public List<ParticleSystem> ParticleSystems;

		private bool m_isOn;

		public float MaxEmission = 30f;

		public float SpinningSpeed = 1f;

		public AudioSource AudSource_Loop;

		private float spinningVal;

		public void Start()
		{
		}

		public void TurnOn()
		{
			if (!m_isOn)
			{
				m_isOn = true;
				AudSource_Loop.Play();
				for (int i = 0; i < ParticleSystems.Count; i++)
				{
					ParticleSystem.EmissionModule emission = ParticleSystems[i].emission;
					ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
					rateOverTime.mode = ParticleSystemCurveMode.Constant;
					rateOverTime.constant = MaxEmission;
					emission.rateOverTime = rateOverTime;
				}
			}
		}

		public void TurnOff()
		{
			if (m_isOn)
			{
				m_isOn = false;
				AudSource_Loop.Stop();
				for (int i = 0; i < ParticleSystems.Count; i++)
				{
					ParticleSystem.EmissionModule emission = ParticleSystems[i].emission;
					ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
					rateOverTime.mode = ParticleSystemCurveMode.Constant;
					rateOverTime.constant = 0f;
					emission.rateOverTime = rateOverTime;
				}
			}
		}

		public void Update()
		{
			switch (Type)
			{
			case FlameTrapType.Waving:
				spinningVal = Mathf.Sin(Time.time * SpinningSpeed) * 70f;
				WavingPiece.localEulerAngles = new Vector3(0f, 0f, spinningVal);
				break;
			case FlameTrapType.Spinning:
				spinningVal += Time.deltaTime * SpinningSpeed;
				spinningVal = Mathf.Repeat(spinningVal, 360f);
				WavingPiece.localEulerAngles = new Vector3(0f, spinningVal, 0f);
				break;
			}
		}
	}
}
