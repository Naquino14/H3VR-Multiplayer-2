using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class MF2_Dispenser : MonoBehaviour, IFVRDamageable
	{
		public AIEntity E;

		public List<MF2_DispenserButton> Buttons_Front;

		public List<MF2_DispenserButton> Buttons_Rear;

		public List<Transform> EnergyArrows;

		public Transform Bay_Front;

		public Transform Bay_Rear;

		public Transform BayPoint_Front;

		public Transform BayPoint_Rear;

		public Transform BaySpawnPoint_Front;

		public Transform BaySpawnPoint_Rear;

		private float m_bayRotFront;

		private float m_bayRotRear;

		public FVRObject TurretTippyToy;

		public GameObject TurretMagazine;

		private bool m_isHealingActive;

		private bool m_isReloadingActive;

		private bool m_isDamageBuffActive;

		private float m_energy;

		private float m_tickTilVendCheck = 1f;

		public Transform SpinnyBit;

		private float m_spinnerRot;

		public List<Transform> ShotgunPoints;

		public FVRObject ShotgunToSpawn;

		[Header("Audio Functionality")]
		public AudioEvent AudEvent_ButtonPress_Engage;

		public AudioEvent AudEvent_ButtonPress_Reset;

		public AudioEvent AudEvent_GenerateMetal;

		public AudioEvent AudEvent_ReloadPulse;

		public AudioEvent AudEvent_HealingEngage;

		public AudioEvent AudEvent_HealingPulse;

		public AudioEvent AudEvent_Fail;

		[Header("Healing Functionality")]
		public LayerMask HealDetect;

		public float HealRange = 10f;

		private float m_HealTick = 1f;

		[Header("FX")]
		public ParticleSystem PSystem_Health;

		public ParticleSystem PSystem_Ammo;

		[Header("Damage")]
		public float DamageRemaining = 10000f;

		public Transform SpawnOnDeathPoint;

		public List<GameObject> SpawnOnDestroy;

		public List<Transform> ShardPoints;

		public List<GameObject> Shards;

		private bool m_hasInvokedDestroy;

		private bool m_isDestroyed;

		private float hitRefire = 1f;

		private void Start()
		{
			ResetOn(makeSound: false);
			m_HealTick = Random.Range(0.5f, 1f);
			if (E.IFFCode == 1 && (GM.CurrentPlayerBody.GetPlayerIFF() == 0 || GM.CurrentPlayerBody.GetPlayerIFF() == -3))
			{
				E.IFFCode = GM.CurrentPlayerBody.GetPlayerIFF();
			}
		}

		public void Damage(Damage d)
		{
			if (!m_hasInvokedDestroy && !m_isDestroyed && d.Source_IFF != E.IFFCode)
			{
				DamageRemaining -= d.Dam_TotalKinetic;
				if (DamageRemaining < 0f)
				{
					m_hasInvokedDestroy = true;
					Invoke("DestroyMe", Random.Range(0.1f, 0.2f));
				}
			}
		}

		private void DestroyMe()
		{
			if (!m_isDestroyed)
			{
				m_isDestroyed = true;
				for (int i = 0; i < Shards.Count; i++)
				{
					Object.Instantiate(Shards[i], ShardPoints[i].position, ShardPoints[i].rotation);
				}
				for (int j = 0; j < SpawnOnDestroy.Count; j++)
				{
					Object.Instantiate(SpawnOnDestroy[j], SpawnOnDeathPoint.position, SpawnOnDeathPoint.rotation);
				}
				Object.Destroy(base.gameObject);
			}
		}

		public void ButtonPressed(MF2_DispenserButton.DispenserButtonType t, MF2_DispenserButton.DispenserSide s)
		{
			Transform transform = BaySpawnPoint_Front;
			if (s == MF2_DispenserButton.DispenserSide.Rear)
			{
				transform = BaySpawnPoint_Rear;
			}
			switch (t)
			{
			case MF2_DispenserButton.DispenserButtonType.Heal:
				if (!m_isHealingActive)
				{
					HealOn();
				}
				break;
			case MF2_DispenserButton.DispenserButtonType.Reload:
				if (!m_isReloadingActive)
				{
					ReloadOn();
				}
				break;
			case MF2_DispenserButton.DispenserButtonType.Reset:
				if (m_isHealingActive || m_isReloadingActive)
				{
					ResetOn(makeSound: true);
				}
				break;
			case MF2_DispenserButton.DispenserButtonType.Turret:
				if (m_energy >= 90f)
				{
					m_energy -= 90f;
					Object.Instantiate(TurretTippyToy.GetGameObject(), transform.position, transform.rotation);
					SM.PlayGenericSound(AudEvent_ReloadPulse, base.transform.position);
				}
				else
				{
					SM.PlayGenericSound(AudEvent_Fail, base.transform.position);
				}
				break;
			case MF2_DispenserButton.DispenserButtonType.Magazine:
				if (m_energy >= 20f)
				{
					m_energy -= 20f;
					Object.Instantiate(TurretMagazine, transform.position, transform.rotation);
					SM.PlayGenericSound(AudEvent_ReloadPulse, base.transform.position);
				}
				else
				{
					SM.PlayGenericSound(AudEvent_Fail, base.transform.position);
				}
				break;
			}
		}

		private void Update()
		{
			float num = 0f;
			if (m_isHealingActive)
			{
				num += 1f;
			}
			if (m_isReloadingActive)
			{
				num += 2f;
			}
			if (m_isHealingActive && m_isReloadingActive)
			{
				num += 3f;
			}
			m_energy -= num * Time.deltaTime;
			if (m_energy <= 0f && (m_isHealingActive || m_isReloadingActive))
			{
				ResetOn(makeSound: true);
			}
			float num2 = 0f;
			if (m_isHealingActive)
			{
				num2 += 360f;
			}
			if (m_isReloadingActive)
			{
				num2 += 720f;
			}
			if (m_isHealingActive && m_isReloadingActive)
			{
				num2 += 720f;
			}
			m_spinnerRot += Time.deltaTime * num2;
			m_spinnerRot = Mathf.Repeat(m_spinnerRot, 360f);
			SpinnyBit.localEulerAngles = new Vector3(0f, 0f, m_spinnerRot);
			m_HealTick -= Time.deltaTime;
			if (m_HealTick <= 0f)
			{
				m_HealTick = 1f;
				if (m_isHealingActive || m_isReloadingActive)
				{
					HealPulse();
				}
			}
			if (E.IFFCode == GM.CurrentPlayerBody.GetPlayerIFF())
			{
				float a = Vector3.Distance(BayPoint_Front.position, GM.CurrentPlayerBody.LeftHand.position);
				a = Mathf.Min(a, Vector3.Distance(BayPoint_Front.position, GM.CurrentPlayerBody.RightHand.position));
				float a2 = Vector3.Distance(BayPoint_Rear.position, GM.CurrentPlayerBody.LeftHand.position);
				a2 = Mathf.Min(a2, Vector3.Distance(BayPoint_Rear.position, GM.CurrentPlayerBody.RightHand.position));
				float num3 = 0f;
				float num4 = 0f;
				if (a <= 0.2f)
				{
					a = Mathf.Clamp(a - 0.15f, 0f, a);
					num3 = Mathf.Lerp(90f, 0f, a * 20f);
				}
				if (a2 <= 0.2f)
				{
					a2 = Mathf.Clamp(a2 - 0.15f, 0f, a2);
					num4 = Mathf.Lerp(-90f, 0f, a2 * 20f);
				}
				if (m_bayRotFront != num3)
				{
					m_bayRotFront = num3;
					Bay_Front.localEulerAngles = new Vector3(m_bayRotFront, 0f, 0f);
				}
				if (m_bayRotRear != num4)
				{
					m_bayRotRear = num4;
					Bay_Rear.localEulerAngles = new Vector3(num4, 0f, 0f);
				}
			}
			m_energy = Mathf.Clamp(m_energy, 0f, 100f);
			float z = Mathf.Lerp(-90f, 90f, m_energy * 0.01f);
			for (int i = 0; i < EnergyArrows.Count; i++)
			{
				EnergyArrows[i].localEulerAngles = new Vector3(0f, 0f, z);
			}
			if (hitRefire > 0f)
			{
				hitRefire -= Time.deltaTime;
			}
		}

		public void HitCharge()
		{
			if (!(hitRefire > 0f))
			{
				m_energy += 8f;
				m_energy = Mathf.Clamp(m_energy, 0f, 100f);
				hitRefire = 0.5f;
			}
		}

		private void HealPulse()
		{
			bool flag = false;
			bool flag2 = false;
			Collider[] array = Physics.OverlapSphere(base.transform.position, HealRange, HealDetect, QueryTriggerInteraction.Collide);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].attachedRigidbody == null)
				{
					continue;
				}
				SosigLink component = array[i].attachedRigidbody.gameObject.GetComponent<SosigLink>();
				if (component != null && component.S.E.IFFCode == E.IFFCode)
				{
					if (m_isHealingActive)
					{
						component.S.BuffHealing_Engage(1f, 20f);
						flag = true;
					}
					if (m_isReloadingActive)
					{
						component.S.ActivatePower(PowerupType.InfiniteAmmo, PowerUpIntensity.High, PowerUpDuration.Blip, isPuke: false, isInverted: false);
						flag2 = true;
					}
				}
			}
			if (E.IFFCode == GM.CurrentPlayerBody.GetPlayerIFF() && Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.transform.position) <= HealRange)
			{
				if (m_isHealingActive)
				{
					flag = true;
					GM.CurrentPlayerBody.ActivatePower(PowerupType.Regen, PowerUpIntensity.Low, PowerUpDuration.Blip, isPuke: false, isInverted: false);
				}
				if (m_isReloadingActive)
				{
					flag2 = true;
					GM.CurrentPlayerBody.ActivatePower(PowerupType.InfiniteAmmo, PowerUpIntensity.Low, PowerUpDuration.Blip, isPuke: false, isInverted: false);
				}
			}
			if (flag)
			{
				SM.PlayGenericSound(AudEvent_HealingPulse, base.transform.position);
				PSystem_Health.Emit(10);
			}
			if (flag2)
			{
				SM.PlayGenericSound(AudEvent_ReloadPulse, base.transform.position);
				PSystem_Ammo.Emit(10);
			}
		}

		private void HealOn()
		{
			if (m_energy > 0f)
			{
				m_isHealingActive = true;
				SM.PlayGenericSound(AudEvent_HealingEngage, base.transform.position);
				Buttons_Front[0].transform.localPosition = new Vector3(0f, 0f, -0.035f);
				Buttons_Rear[0].transform.localPosition = new Vector3(0f, 0f, 0.035f);
				Buttons_Front[1].transform.localPosition = new Vector3(0f, 0f, 0f);
				Buttons_Rear[1].transform.localPosition = new Vector3(0f, 0f, 0f);
				SM.PlayGenericSound(AudEvent_ButtonPress_Reset, base.transform.position);
			}
		}

		private void ReloadOn()
		{
			if (m_energy > 0f)
			{
				m_isReloadingActive = true;
				SM.PlayGenericSound(AudEvent_GenerateMetal, base.transform.position);
				Buttons_Front[2].transform.localPosition = new Vector3(0f, 0f, -0.035f);
				Buttons_Rear[2].transform.localPosition = new Vector3(0f, 0f, 0.035f);
				Buttons_Front[1].transform.localPosition = new Vector3(0f, 0f, 0f);
				Buttons_Rear[1].transform.localPosition = new Vector3(0f, 0f, 0f);
				SM.PlayGenericSound(AudEvent_ButtonPress_Engage, base.transform.position);
			}
		}

		private void ResetOn(bool makeSound)
		{
			if (makeSound)
			{
				SM.PlayGenericSound(AudEvent_ButtonPress_Reset, base.transform.position);
			}
			m_isHealingActive = false;
			m_isReloadingActive = false;
			Buttons_Front[0].transform.localPosition = new Vector3(0f, 0f, 0f);
			Buttons_Rear[0].transform.localPosition = new Vector3(0f, 0f, 0f);
			Buttons_Front[1].transform.localPosition = new Vector3(0f, 0f, -0.035f);
			Buttons_Rear[1].transform.localPosition = new Vector3(0f, 0f, 0.035f);
			Buttons_Front[2].transform.localPosition = new Vector3(0f, 0f, 0f);
			Buttons_Rear[2].transform.localPosition = new Vector3(0f, 0f, 0f);
		}
	}
}
