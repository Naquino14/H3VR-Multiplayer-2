using UnityEngine;

namespace FistVR
{
	public class FVRBeamer : FVRPhysicalObject
	{
		public enum BeamerStatusLight
		{
			Red,
			Yellow,
			Green
		}

		public enum BeamerPowerState
		{
			Off,
			On
		}

		[Header("Beamer Config")]
		public Renderer Light0;

		public Renderer Light1;

		public Renderer Light2;

		public Material MatLightGreen;

		public Material MatLightYellow;

		public Material MatLightRed;

		public FVRBeamerSwitch BatterySwitch;

		public FVRBeamerSwitch CapacitorSwitch;

		public FVRBeamerSwitch MotorSwitch;

		private bool m_isBatterySwitchedOn;

		private bool m_isCapacitorSwitchedOn;

		private bool m_isMotorSwitchedOn;

		private BeamerStatusLight m_batteryLight;

		private BeamerStatusLight m_capacitorLight;

		private BeamerStatusLight m_motorLight;

		private float curMotorSpeed;

		private float tarMotorSpeed;

		public AudioSource AudioMotor;

		public AudioSource AudioDrone;

		public AudioSource AudioDroneActive;

		public AudioSource AudioElectricity;

		public AudioSource AudioShunt;

		private BeamerPowerState m_powerState;

		private bool m_isManipulating;

		public Transform[] SpinnyParts;

		public Transform Aperture;

		public ParticleSystem RotorParticles;

		private AudioSource m_aud;

		[Header("Beamer Locus Config")]
		public FVRBeamerLocus GravLocus;

		private float m_locusDistance = 1f;

		private float m_locusMinDistance = 0.2f;

		private float m_locusMaxDistance = 50f;

		private float m_locusMover;

		private bool m_hasTriggeredUpSinceFiring = true;

		public void SetLocusMover(float l)
		{
			m_locusMover = l;
		}

		protected override void Awake()
		{
			base.Awake();
			m_aud = GetComponent<AudioSource>();
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (m_isBatterySwitchedOn && m_isCapacitorSwitchedOn && m_isMotorSwitchedOn)
			{
				m_powerState = BeamerPowerState.On;
			}
			else
			{
				m_powerState = BeamerPowerState.Off;
			}
			if (m_powerState == BeamerPowerState.Off)
			{
				tarMotorSpeed = 0f;
			}
			else if (m_isManipulating)
			{
				tarMotorSpeed = 1f;
			}
			else
			{
				tarMotorSpeed = 0.5f;
			}
			curMotorSpeed = Mathf.Lerp(curMotorSpeed, tarMotorSpeed, Time.deltaTime * 1.6f);
			if (curMotorSpeed < 0.05f)
			{
				AudioMotor.Stop();
				AudioDrone.Stop();
				AudioDroneActive.Stop();
				ParticleSystem.EmissionModule emission = RotorParticles.emission;
				ParticleSystem.MinMaxCurve rate = emission.rate;
				rate.constantMax = 0f;
				emission.rate = rate;
			}
			else
			{
				if (!AudioMotor.isPlaying)
				{
					AudioMotor.Play();
				}
				if (!AudioDrone.isPlaying)
				{
					AudioDrone.Play();
				}
				if (!AudioDroneActive.isPlaying)
				{
					AudioDroneActive.Play();
				}
				AudioMotor.volume = (curMotorSpeed * 0.25f - 0.1f) * 0.3f;
				AudioMotor.pitch = curMotorSpeed * 0.5f + 0.5f;
				AudioDrone.volume = (curMotorSpeed * 0.4f - 0.1f) * 0.3f;
				AudioDrone.pitch = curMotorSpeed * 0.3f + 0.7f;
				AudioDroneActive.volume = (curMotorSpeed * 1.1f - 0.5f) * 0.3f;
				AudioDroneActive.pitch = curMotorSpeed * 0.5f + 0.5f;
				ParticleSystem.EmissionModule emission2 = RotorParticles.emission;
				ParticleSystem.MinMaxCurve rate2 = emission2.rate;
				rate2.constantMax = (curMotorSpeed - 0.5f) * 20f;
				emission2.rate = rate2;
			}
			Transform[] spinnyParts = SpinnyParts;
			foreach (Transform transform in spinnyParts)
			{
				float num = curMotorSpeed * 2000f;
				transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z + num * Time.deltaTime);
			}
			if (m_powerState == BeamerPowerState.On)
			{
				if (!GravLocus.gameObject.activeSelf)
				{
					GravLocus.gameObject.transform.position = Aperture.position + Aperture.forward * m_locusDistance;
				}
				GravLocus.gameObject.SetActive(value: true);
				GravLocus.SetExistence(b: true);
				if (Mathf.Abs(m_locusMover) < 3f)
				{
					m_locusMover = 0f;
				}
				m_locusDistance += m_locusMover * Time.deltaTime * 0.15f;
				m_locusDistance = Mathf.Clamp(m_locusDistance, m_locusMinDistance, m_locusMaxDistance);
				GravLocus.SetTargetPoint(Aperture.position + Aperture.forward * m_locusDistance);
				if (m_isManipulating)
				{
					GravLocus.SetGrav(b: true);
				}
				else
				{
					GravLocus.SetGrav(b: false);
				}
			}
			else
			{
				GravLocus.SetExistence(b: false);
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (m_powerState == BeamerPowerState.On)
			{
				if (hand.Input.TriggerPressed && m_hasTriggeredUpSinceFiring)
				{
					m_isManipulating = true;
					if (Random.value > 0.75f)
					{
						FXM.InitiateMuzzleFlash(GravLocus.transform.position, Aperture.forward, Random.Range(0.5f, 1.5f), Color.white, Random.Range(0.5f, 2f));
					}
					if ((hand.IsInStreamlinedMode && hand.Input.BYButtonDown) || (!hand.IsInStreamlinedMode && hand.Input.TouchpadDown))
					{
						GravLocus.Shunt();
						m_locusDistance = 0.25f;
						curMotorSpeed = 1.6f;
						m_isManipulating = false;
						AudioShunt.Stop();
						AudioShunt.Play();
						m_hasTriggeredUpSinceFiring = false;
					}
				}
				else
				{
					m_isManipulating = false;
				}
			}
			if (hand.Input.TriggerUp)
			{
				m_hasTriggeredUpSinceFiring = true;
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			m_isManipulating = false;
		}

		public void Shunt()
		{
			m_locusDistance = 0.25f;
		}

		public void SetSwitchState(int index, bool b)
		{
			switch (index)
			{
			case 0:
				m_isBatterySwitchedOn = b;
				break;
			case 1:
				m_isCapacitorSwitchedOn = b;
				if (b)
				{
					AudioElectricity.Play();
				}
				break;
			case 2:
				m_isMotorSwitchedOn = b;
				break;
			}
			UpdateStatusLights();
		}

		private void UpdateStatusLights()
		{
			if (m_isBatterySwitchedOn)
			{
				m_batteryLight = BeamerStatusLight.Green;
			}
			else
			{
				m_batteryLight = BeamerStatusLight.Red;
			}
			if (m_isBatterySwitchedOn && m_isCapacitorSwitchedOn)
			{
				m_capacitorLight = BeamerStatusLight.Green;
			}
			else if (m_isCapacitorSwitchedOn)
			{
				m_capacitorLight = BeamerStatusLight.Yellow;
			}
			else
			{
				m_capacitorLight = BeamerStatusLight.Red;
			}
			if (m_isBatterySwitchedOn && m_isCapacitorSwitchedOn && m_isMotorSwitchedOn)
			{
				m_motorLight = BeamerStatusLight.Green;
			}
			else if (m_isMotorSwitchedOn)
			{
				m_motorLight = BeamerStatusLight.Yellow;
			}
			else
			{
				m_motorLight = BeamerStatusLight.Red;
			}
			UpdateLightDisplay(Light0, m_batteryLight);
			UpdateLightDisplay(Light1, m_capacitorLight);
			UpdateLightDisplay(Light2, m_motorLight);
		}

		private void UpdateLightDisplay(Renderer rend, BeamerStatusLight lightStatus)
		{
			switch (lightStatus)
			{
			case BeamerStatusLight.Green:
				rend.material = MatLightGreen;
				break;
			case BeamerStatusLight.Yellow:
				rend.material = MatLightYellow;
				break;
			case BeamerStatusLight.Red:
				rend.material = MatLightRed;
				break;
			}
		}
	}
}
