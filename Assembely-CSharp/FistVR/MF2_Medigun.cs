using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class MF2_Medigun : FVRPhysicalObject
	{
		[Header("Connections")]
		public MedigunBeam Beam;

		public Transform BeamTarget;

		public MF2_Medigun_Handle Handle;

		public Transform HandleRot_Front;

		public Transform HandleRot_Back;

		public Transform HandleRot_Up;

		public Transform Muzzle;

		[Header("Functionality")]
		public LayerMask LatchingMask;

		public LayerMask BlockingMask;

		private float EngageRange = 10f;

		private float MaxRange = 20f;

		private RaycastHit m_hit;

		private bool m_isBeamEngaged;

		private bool m_hasSosigTarget;

		private Sosig m_targettedSosig;

		private bool m_isUberReady;

		private bool m_isUberActive;

		private float m_uberChargeUp;

		private float m_uberChargeOut = 8f;

		[Header("Audio")]
		public AudioSource AudSource_Loop_Heal;

		public AudioSource AudSource_Loop_Charged;

		public AudioEvent AudEvent_Engage;

		public AudioEvent AudEvent_Disengage;

		public AudioEvent AudEvent_Fail;

		public AudioEvent AudEvent_UberReady;

		public AudioEvent AudEvent_UberEngage;

		public AudioEvent AudEvent_UberDisengage;

		[Header("Materials")]
		public Material[] GunMatsByTeam;

		public Material[] MagMatsByTeam;

		public Material[] GunMatsByTeam_Uber;

		public Material[] MagMatsByTeam_Uber;

		public Renderer[] GunRends;

		public Renderer MagRend;

		public Transform Fore;

		private int storedIFF = -3;

		public Renderer Gauge;

		protected override void Awake()
		{
			base.Awake();
			BeamTarget.SetParent(null);
			storedIFF = GM.CurrentPlayerBody.GetPlayerIFF();
			Beam.Initialize();
			SetBaseMat();
		}

		private void SetBaseMat()
		{
			int value = storedIFF;
			value = Mathf.Clamp(value, 0, 2);
			for (int i = 0; i < GunRends.Length; i++)
			{
				GunRends[i].material = GunMatsByTeam[value];
			}
			MagRend.material = MagMatsByTeam[value];
			Beam.SetLineColor(value);
			Beam.SetElectricityColor(value);
			Gauge.material = GunMatsByTeam_Uber[value];
		}

		private void SetUberMat()
		{
			int value = storedIFF;
			value = Mathf.Clamp(value, 0, 2);
			for (int i = 0; i < GunRends.Length; i++)
			{
				GunRends[i].material = GunMatsByTeam_Uber[value];
			}
			MagRend.material = MagMatsByTeam_Uber[value];
			Gauge.material = GunMatsByTeam_Uber[value];
		}

		private void TryEngageBeam()
		{
			Collider[] array = Physics.OverlapCapsule(Muzzle.position, Muzzle.position + Muzzle.forward * EngageRange, 2f, LatchingMask);
			List<Rigidbody> list = new List<Rigidbody>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].attachedRigidbody != null && !list.Contains(array[i].attachedRigidbody))
				{
					list.Add(array[i].attachedRigidbody);
				}
			}
			bool flag = false;
			SosigLink sosigLink = null;
			float num = 180f;
			for (int j = 0; j < list.Count; j++)
			{
				SosigLink component = list[j].GetComponent<SosigLink>();
				if (!(component == null) && component.S.BodyState != Sosig.SosigBodyState.Dead && (storedIFF <= 0 || component.S.E.IFFCode == storedIFF || component.S.E.IFFCode < 0))
				{
					Vector3 from = list[j].transform.transform.position - Muzzle.position;
					float num2 = Vector3.Angle(from, Muzzle.forward);
					if (num2 < num)
					{
						num = num2;
						sosigLink = component;
						flag = true;
					}
				}
			}
			if (flag)
			{
				Sosig s = sosigLink.S;
				SosigLink sosigLink2 = s.Links[1];
				if (!Physics.Linecast(Muzzle.position, sosigLink2.transform.position, BlockingMask, QueryTriggerInteraction.Ignore))
				{
					EngageBeam(s);
					return;
				}
			}
			SM.PlayGenericSound(AudEvent_Fail, Muzzle.position);
			m_isBeamEngaged = false;
		}

		private void EngageBeam(Sosig S)
		{
			SM.PlayGenericSound(AudEvent_Engage, Muzzle.position);
			m_isBeamEngaged = true;
			m_hasSosigTarget = true;
			m_targettedSosig = S;
			BeamTarget.transform.position = m_targettedSosig.Links[1].transform.position;
			Beam.StartBeam(BeamTarget);
		}

		private void DisEngageBeam()
		{
			if (m_isBeamEngaged)
			{
				SM.PlayGenericSound(AudEvent_Disengage, Muzzle.position);
				m_isBeamEngaged = false;
				Beam.StopBeam();
				m_hasSosigTarget = false;
				m_targettedSosig = null;
			}
		}

		private void EngageUber()
		{
			SM.PlayGenericSound(AudEvent_UberEngage, Muzzle.position);
			m_uberChargeUp = 0f;
			m_uberChargeOut = 8f;
			m_isUberReady = false;
			m_isUberActive = true;
			SetUberMat();
		}

		private void DisEngageUber()
		{
			SM.PlayGenericSound(AudEvent_UberDisengage, Muzzle.position);
			m_isUberReady = false;
			m_isUberActive = false;
			m_uberChargeUp = 0f;
			m_uberChargeOut = 0f;
			SetBaseMat();
		}

		public void HandleEngage()
		{
			TryEngageBeam();
		}

		public void HandleDisEngage()
		{
			DisEngageBeam();
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (m_isUberReady && !m_isUberActive && m_isBeamEngaged && hand.Input.TriggerDown && m_hasTriggeredUpSinceBegin)
			{
				EngageUber();
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			int playerIFF = GM.CurrentPlayerBody.GetPlayerIFF();
			if (storedIFF != playerIFF)
			{
				storedIFF = playerIFF;
				if (m_isUberActive)
				{
					SetUberMat();
				}
				else
				{
					SetBaseMat();
				}
			}
			if (m_isUberActive)
			{
				Gauge.transform.localScale = new Vector3(3.5f, 1f, Mathf.Lerp(0.25f, 8.5f, m_uberChargeOut / 8f));
			}
			else
			{
				Gauge.transform.localScale = new Vector3(3.5f, 1f, Mathf.Lerp(0.25f, 8.5f, m_uberChargeUp / 100f));
			}
			if (m_isBeamEngaged)
			{
				if (m_targettedSosig == null)
				{
					DisEngageBeam();
				}
				else if (m_targettedSosig.BodyState == Sosig.SosigBodyState.Dead)
				{
					DisEngageBeam();
				}
				else
				{
					Transform transform = m_targettedSosig.Links[1].transform;
					if (Physics.Linecast(Muzzle.position, transform.transform.position, BlockingMask, QueryTriggerInteraction.Ignore))
					{
						DisEngageBeam();
					}
					else
					{
						Beam.target.position = transform.position;
						float num = Vector3.Distance(transform.position, Muzzle.position);
						if (num > MaxRange)
						{
							DisEngageBeam();
						}
					}
				}
			}
			if (m_isBeamEngaged)
			{
				if (!m_isUberReady && !m_isUberActive)
				{
					m_uberChargeUp += Time.deltaTime * 5f;
				}
				if (m_uberChargeUp >= 100f)
				{
					m_isUberReady = true;
				}
				if (!AudSource_Loop_Heal.isPlaying)
				{
					AudSource_Loop_Heal.Play();
				}
				m_targettedSosig.BuffHealing_Engage(0.1f, 20f);
				m_targettedSosig.BuffDamResist_Engage(0.1f, 0.6f);
				if (m_isUberActive)
				{
					m_targettedSosig.BuffInvuln_Engage(0.1f);
					GM.CurrentPlayerBody.ActivatePower(PowerupType.Regen, PowerUpIntensity.High, PowerUpDuration.Blip, isPuke: false, isInverted: false);
					m_uberChargeOut -= Time.deltaTime;
					if (m_uberChargeOut <= 0f)
					{
						DisEngageUber();
					}
				}
				Fore.localEulerAngles = Random.onUnitSphere * 0.4f;
			}
			else
			{
				if (AudSource_Loop_Heal.isPlaying)
				{
					AudSource_Loop_Heal.Stop();
				}
				Fore.localEulerAngles = Vector3.zero;
			}
			if (m_isUberReady || m_isUberActive)
			{
				Beam.Electricity.SetActive(value: true);
				if (!AudSource_Loop_Charged.isPlaying)
				{
					AudSource_Loop_Charged.Play();
				}
			}
			else
			{
				Beam.Electricity.SetActive(value: false);
				if (AudSource_Loop_Charged.isPlaying)
				{
					AudSource_Loop_Charged.Stop();
				}
			}
		}
	}
}
