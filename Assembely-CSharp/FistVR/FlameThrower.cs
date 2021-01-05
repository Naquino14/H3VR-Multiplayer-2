using UnityEngine;

namespace FistVR
{
	public class FlameThrower : FVRFireArm
	{
		[Header("FlameThrower Params")]
		public FlameThrowerValve Valve;

		public bool UsesValve = true;

		public MF2_FlamethrowerValve MF2Valve;

		public bool UsesMF2Valve;

		[Header("Trigger Config")]
		public Transform Trigger;

		public float TriggerFiringThreshold = 0.8f;

		public float Trigger_ForwardValue;

		public float Trigger_RearwardValue;

		public InterpStyle TriggerInterpStyle = InterpStyle.Rotation;

		private float m_triggerFloat;

		[Header("Special Audio Config")]
		public AudioEvent AudEvent_Ignite;

		public AudioEvent AudEvent_Extinguish;

		public AudioSource AudSource_FireLoop;

		private float m_triggerHasBeenHeldFor;

		private bool m_hasFiredStartSound;

		private bool m_isFiring;

		public ParticleSystem FireParticles;

		public Vector2 FireWidthRange;

		public Vector2 SpeedRangeMin;

		public Vector2 SpeedRangeMax;

		public Vector2 SizeRangeMin;

		public Vector2 SizeRangeMax;

		public Vector2 AudioPitchRange = new Vector2(1.5f, 0.5f);

		public float ParticleVolume = 40f;

		public bool UsesPilotLightSystem;

		public bool UsesAirBlastSystem;

		[Header("PilotLight")]
		public Transform PilotLight;

		private bool m_isPilotLightOn;

		public AudioEvent AudEvent_PilotOn;

		[Header("Airblast")]
		public bool UsesAirBlast;

		public Transform AirBlastCenter;

		public GameObject AirBlastGo;

		private float m_airBurstRecovery;

		protected override void Start()
		{
			if (UsesPilotLightSystem)
			{
				PilotLight.gameObject.SetActive(value: false);
			}
			base.Start();
		}

		private float GetVLerp()
		{
			if (UsesValve)
			{
				return Valve.ValvePos;
			}
			if (UsesMF2Valve)
			{
				return MF2Valve.Lerp;
			}
			return 0.5f;
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			UpdateControls();
			UpdateFire();
			if (!UsesPilotLightSystem)
			{
				return;
			}
			if (Magazine != null && Magazine.FuelAmountLeft > 0f)
			{
				if (!m_isPilotLightOn)
				{
					PilotOn();
				}
			}
			else if (m_isPilotLightOn)
			{
				PilotOff();
			}
			if (m_isPilotLightOn)
			{
				PilotLight.localScale = Vector3.one + Random.onUnitSphere * 0.05f;
			}
		}

		private void PilotOn()
		{
			m_isPilotLightOn = true;
			SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_PilotOn, GetMuzzle().position);
			PilotLight.gameObject.SetActive(value: true);
		}

		private void PilotOff()
		{
			m_isPilotLightOn = false;
			PilotLight.gameObject.SetActive(value: false);
		}

		private void AirBlast()
		{
			GameObject gameObject = Object.Instantiate(AirBlastGo, AirBlastCenter.position, AirBlastCenter.rotation);
			gameObject.GetComponent<Explosion>().IFF = GM.CurrentPlayerBody.GetPlayerIFF();
			gameObject.GetComponent<ExplosionSound>().IFF = GM.CurrentPlayerBody.GetPlayerIFF();
		}

		private void UpdateControls()
		{
			if (base.IsHeld)
			{
				if (m_hasTriggeredUpSinceBegin && !IsAltHeld)
				{
					m_triggerFloat = m_hand.Input.TriggerFloat;
				}
				else
				{
					m_triggerFloat = 0f;
				}
				if (UsesAirBlast && m_airBurstRecovery <= 0f && HasFuel() && ((m_hand.IsInStreamlinedMode && m_hand.Input.BYButtonDown) || (!m_hand.IsInStreamlinedMode && m_hand.Input.TouchpadDown)))
				{
					m_airBurstRecovery = 1f;
					AirBlast();
					Magazine.DrainFuel(5f);
				}
				if (m_airBurstRecovery > 0f)
				{
					m_airBurstRecovery -= Time.deltaTime;
				}
				if (m_triggerFloat > 0.2f && HasFuel() && m_airBurstRecovery <= 0f)
				{
					if (m_triggerHasBeenHeldFor < 2f)
					{
						m_triggerHasBeenHeldFor += Time.deltaTime;
					}
					m_isFiring = true;
					if (!m_hasFiredStartSound)
					{
						m_hasFiredStartSound = true;
						SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_Ignite, GetMuzzle().position);
					}
					float volume = Mathf.Clamp(m_triggerHasBeenHeldFor * 2f, 0f, 0.4f);
					AudSource_FireLoop.volume = volume;
					float vLerp = GetVLerp();
					AudSource_FireLoop.pitch = Mathf.Lerp(AudioPitchRange.x, AudioPitchRange.y, vLerp);
					if (!AudSource_FireLoop.isPlaying)
					{
						AudSource_FireLoop.Play();
					}
					Magazine.DrainFuel(Time.deltaTime);
				}
				else
				{
					m_triggerHasBeenHeldFor = 0f;
					StopFiring();
				}
			}
			else
			{
				m_triggerFloat = 0f;
			}
			if (m_triggerFloat <= 0f)
			{
				StopFiring();
			}
		}

		public void UpdateFire()
		{
			ParticleSystem.EmissionModule emission = FireParticles.emission;
			ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
			if (m_isFiring)
			{
				rateOverTime.mode = ParticleSystemCurveMode.Constant;
				rateOverTime.constantMax = ParticleVolume;
				rateOverTime.constantMin = ParticleVolume;
				float vLerp = GetVLerp();
				ParticleSystem.MainModule main = FireParticles.main;
				ParticleSystem.MinMaxCurve startSpeed = main.startSpeed;
				startSpeed.mode = ParticleSystemCurveMode.TwoConstants;
				startSpeed.constantMax = Mathf.Lerp(SpeedRangeMax.x, SpeedRangeMax.y, vLerp);
				startSpeed.constantMin = Mathf.Lerp(SpeedRangeMin.x, SpeedRangeMin.y, vLerp);
				main.startSpeed = startSpeed;
				ParticleSystem.MinMaxCurve startSize = main.startSize;
				startSize.mode = ParticleSystemCurveMode.TwoConstants;
				startSize.constantMax = Mathf.Lerp(SizeRangeMax.x, SizeRangeMax.y, vLerp);
				startSize.constantMin = Mathf.Lerp(SizeRangeMin.x, SizeRangeMin.y, vLerp);
				main.startSize = startSize;
				ParticleSystem.ShapeModule shape = FireParticles.shape;
				shape.angle = Mathf.Lerp(FireWidthRange.x, FireWidthRange.y, vLerp);
			}
			else
			{
				rateOverTime.mode = ParticleSystemCurveMode.Constant;
				rateOverTime.constantMax = 0f;
				rateOverTime.constantMin = 0f;
			}
			emission.rateOverTime = rateOverTime;
		}

		private bool HasFuel()
		{
			if (Magazine == null)
			{
				return false;
			}
			if (Magazine.FuelAmountLeft <= 0f)
			{
				return false;
			}
			return true;
		}

		private void StopFiring()
		{
			if (m_isFiring)
			{
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_Extinguish, GetMuzzle().position);
				AudSource_FireLoop.Stop();
				AudSource_FireLoop.volume = 0f;
			}
			m_isFiring = false;
			m_hasFiredStartSound = false;
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (Trigger != null)
			{
				if (TriggerInterpStyle == InterpStyle.Translate)
				{
					Trigger.localPosition = new Vector3(0f, 0f, Mathf.Lerp(Trigger_ForwardValue, Trigger_RearwardValue, m_triggerFloat));
				}
				else if (TriggerInterpStyle == InterpStyle.Rotation)
				{
					Trigger.localEulerAngles = new Vector3(Mathf.Lerp(Trigger_ForwardValue, Trigger_RearwardValue, m_triggerFloat), 0f, 0f);
				}
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			m_triggerFloat = 0f;
			if (Trigger != null)
			{
				if (TriggerInterpStyle == InterpStyle.Translate)
				{
					Trigger.localPosition = new Vector3(0f, 0f, Mathf.Lerp(Trigger_ForwardValue, Trigger_RearwardValue, m_triggerFloat));
				}
				else if (TriggerInterpStyle == InterpStyle.Rotation)
				{
					Trigger.localEulerAngles = new Vector3(Mathf.Lerp(Trigger_ForwardValue, Trigger_RearwardValue, m_triggerFloat), 0f, 0f);
				}
			}
		}
	}
}
