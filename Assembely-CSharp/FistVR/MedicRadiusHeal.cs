using UnityEngine;

namespace FistVR
{
	public class MedicRadiusHeal : SosigWearable
	{
		[Header("Healing Functionality")]
		public LayerMask HealDetect;

		public float HealRange = 10f;

		private float m_HealTick = 1f;

		[Header("FX")]
		public ParticleSystem PSystem_Health;

		[Header("Audio Functionality")]
		public AudioEvent AudEvent_HealingPulse;

		public AudioEvent AudEvent_Uber;

		private float m_uberCharge;

		private bool m_isUbering;

		private bool m_healedAggroLastTick;

		public override void Start()
		{
			base.Start();
			m_HealTick = Random.Range(0.5f, 1f);
		}

		private void Update()
		{
			if (S == null)
			{
				return;
			}
			m_HealTick -= Time.deltaTime;
			if (m_HealTick <= 0f)
			{
				m_HealTick = 1f;
				if (S.BodyState == Sosig.SosigBodyState.InControl && S.CurrentOrder != 0)
				{
					HealPulse();
				}
			}
			if (!m_isUbering && m_uberCharge >= 40f && (S.CurrentOrder == Sosig.SosigOrder.Skirmish || m_healedAggroLastTick))
			{
				SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_Uber, base.transform.position);
				m_isUbering = true;
			}
		}

		private void HealPulse()
		{
			bool flag = false;
			m_healedAggroLastTick = false;
			Collider[] array = Physics.OverlapSphere(base.transform.position, HealRange, HealDetect, QueryTriggerInteraction.Collide);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].attachedRigidbody == null)
				{
					continue;
				}
				SosigLink component = array[i].attachedRigidbody.gameObject.GetComponent<SosigLink>();
				if (!(component != null) || component.S.E.IFFCode != S.E.IFFCode)
				{
					continue;
				}
				if (m_isUbering)
				{
					if (S != component.S)
					{
						component.S.BuffInvuln_Engage(1.01f);
					}
					else
					{
						component.S.BuffInvuln_Engage(1.01f);
					}
				}
				else if (S != component.S)
				{
					component.S.BuffHealing_Engage(1.01f, 20f);
					flag = true;
					if (S.CurrentOrder == Sosig.SosigOrder.Skirmish)
					{
						m_healedAggroLastTick = true;
					}
				}
				else
				{
					component.S.BuffHealing_Engage(1.01f, 3f);
					flag = true;
				}
			}
			if (S.E.IFFCode == GM.CurrentPlayerBody.GetPlayerIFF() && Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.transform.position) <= HealRange)
			{
				if (m_isUbering)
				{
					GM.CurrentPlayerBody.ActivatePower(PowerupType.Invincibility, PowerUpIntensity.High, PowerUpDuration.Blip, isPuke: false, isInverted: false);
				}
				else
				{
					flag = true;
					GM.CurrentPlayerBody.ActivatePower(PowerupType.Regen, PowerUpIntensity.Medium, PowerUpDuration.Blip, isPuke: false, isInverted: false);
				}
			}
			if (flag && Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.transform.position) < 20f)
			{
				SM.PlayGenericSound(AudEvent_HealingPulse, base.transform.position);
				PSystem_Health.Emit(5);
			}
			if (flag && !m_isUbering)
			{
				m_uberCharge += 1f;
			}
			if (m_isUbering)
			{
				m_uberCharge -= 5f;
				if (m_uberCharge <= 0f)
				{
					m_isUbering = false;
				}
			}
		}
	}
}
