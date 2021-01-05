using UnityEngine;

namespace FistVR
{
	public class MF2_CloakingWatch : FVRPhysicalObject
	{
		public enum WatchState
		{
			Inactive,
			Cloaking,
			Cloaked,
			Uncloaking,
			ShortedOut
		}

		[Header("Reticle Display")]
		public Renderer HealthRingRend;

		[Header("Cloaking VFX")]
		public Renderer WatchRend;

		public GameObject ShortingOutFX;

		public WatchState State;

		private float m_cloakingLerp;

		private float m_shortOutCooldown = 10f;

		private float m_cloakingEnergy = 10f;

		[Header("Cloaking Audio")]
		public AudioEvent AudEvent_Cloak;

		public AudioEvent AudEvent_Uncloak;

		private void ShortOut()
		{
			State = WatchState.ShortedOut;
			m_shortOutCooldown = 10f;
			ShortingOutFX.SetActive(value: true);
			m_cloakingEnergy = 0f;
			GM.CurrentPlayerBody.DeActivatePowerIfActive(PowerupType.Ghosted);
			m_cloakingLerp = 0f;
			UpdateCloakEffect();
		}

		private void PulseCloaking()
		{
			if (base.IsHeld)
			{
				GM.CurrentPlayerBody.ActivatePower(PowerupType.Ghosted, PowerUpIntensity.High, PowerUpDuration.Blip, isPuke: false, isInverted: false, 1.05f);
			}
			m_cloakingEnergy -= Time.deltaTime;
			if (m_cloakingEnergy <= 0f)
			{
				m_cloakingLerp = 1f;
				State = WatchState.Uncloaking;
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_Uncloak, base.transform.position);
			}
		}

		private void UpdatePowerDial()
		{
			float num = Mathf.Clamp(m_cloakingEnergy / 10f, 0f, 1f);
			num = 0f - num;
			num += 0.5f;
			HealthRingRend.material.SetTextureOffset("_MainTex", new Vector2(0f, num));
		}

		private void UpdateCloakEffect()
		{
			WatchRend.material.SetFloat("_DissolveCutoff", m_cloakingLerp);
		}

		private void ShortOutCheck()
		{
			if (State == WatchState.Cloaked && base.IsHeld && m_hand.OtherHand.CurrentInteractable != null)
			{
				ShortOut();
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			UpdatePowerDial();
			ShortOutCheck();
			switch (State)
			{
			case WatchState.Inactive:
				if (m_cloakingEnergy < 10f)
				{
					m_cloakingEnergy += Time.deltaTime;
				}
				ShortOutCheck();
				break;
			case WatchState.Cloaking:
				m_cloakingLerp += Time.deltaTime;
				if (m_cloakingLerp >= 1f)
				{
					m_cloakingLerp = 1f;
					State = WatchState.Cloaked;
				}
				UpdateCloakEffect();
				break;
			case WatchState.Uncloaking:
				m_cloakingLerp -= Time.deltaTime;
				if (m_cloakingLerp <= 0f)
				{
					m_cloakingLerp = 0f;
					State = WatchState.Inactive;
				}
				UpdateCloakEffect();
				break;
			case WatchState.Cloaked:
				PulseCloaking();
				break;
			case WatchState.ShortedOut:
				m_shortOutCooldown -= Time.deltaTime;
				if (m_shortOutCooldown <= 0f)
				{
					State = WatchState.Inactive;
					ShortingOutFX.SetActive(value: false);
				}
				break;
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (m_hasTriggeredUpSinceBegin && hand.Input.TriggerDown)
			{
				if (State == WatchState.Inactive)
				{
					SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_Cloak, base.transform.position);
					State = WatchState.Cloaking;
				}
				else if (State == WatchState.Cloaked)
				{
					SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_Uncloak, base.transform.position);
					State = WatchState.Uncloaking;
				}
			}
		}
	}
}
