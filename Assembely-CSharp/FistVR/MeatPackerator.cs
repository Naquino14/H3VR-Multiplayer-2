using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class MeatPackerator : MonoBehaviour
	{
		private bool m_hasMeatCore;

		private bool m_hasFirstHerb;

		private bool m_hasSecondHerb;

		private int m_meatcoreType;

		private int m_firstHerb;

		private int m_secondHerb;

		public List<Image> Indicators;

		public List<Sprite> Sprites_Herbs;

		public List<Sprite> Sprites_Cores;

		public ParticleSystem PFX_GrindInsert;

		public AudioEvent AudEvent_Insert;

		public AudioEvent AudEvent_Fail;

		public AudioEvent AudEvent_Success;

		public FVRObject UncookedLink;

		public Transform SpawnPoint;

		private void OnTriggerEnter(Collider col)
		{
			TestCollider(col);
		}

		private void TestCollider(Collider col)
		{
			if (col.attachedRigidbody == null)
			{
				return;
			}
			bool flag = false;
			RotrwHerb component = col.attachedRigidbody.gameObject.GetComponent<RotrwHerb>();
			if (component != null)
			{
				if (m_hasFirstHerb && m_hasSecondHerb)
				{
					EjectIngredient(component);
				}
				else if (!m_hasFirstHerb)
				{
					HerbInserted(component, isFirstHerb: true);
				}
				else if (!m_hasSecondHerb)
				{
					HerbInserted(component, isFirstHerb: false);
				}
				flag = true;
			}
			RotrwMeatCore component2 = col.attachedRigidbody.gameObject.GetComponent<RotrwMeatCore>();
			if (component2 != null)
			{
				if (m_hasMeatCore)
				{
					EjectIngredient(component2);
				}
				else
				{
					GrindEffect();
					Object.Destroy(component2.gameObject);
					MeatCoreInserted(component2);
				}
				flag = true;
			}
			if (flag)
			{
				return;
			}
			if (col.attachedRigidbody.GetComponent<FVRPhysicalObject>() != null)
			{
				FVRPhysicalObject component3 = col.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
				if (component3.IsHeld)
				{
					FVRViveHand hand = component3.m_hand;
					component3.EndInteraction(hand);
					hand.ForceSetInteractable(null);
				}
			}
			col.attachedRigidbody.velocity = Vector3.up * 4f + Random.onUnitSphere * 1.5f;
		}

		private void GrindEffect()
		{
			PFX_GrindInsert.Emit(20);
			SM.PlayGenericSound(AudEvent_Insert, base.transform.position);
		}

		private void EjectIngredient(FVRPhysicalObject obj)
		{
			if (obj.IsHeld)
			{
				FVRViveHand hand = obj.m_hand;
				obj.EndInteraction(hand);
				hand.ForceSetInteractable(null);
			}
			obj.RootRigidbody.velocity = Vector3.up * 4f + Random.onUnitSphere * 1.5f;
		}

		private void MeatCoreInserted(RotrwMeatCore mc)
		{
			m_meatcoreType = (int)mc.Type;
			m_hasMeatCore = true;
			UpdateIndicators();
		}

		private void HerbInserted(RotrwHerb h, bool isFirstHerb)
		{
			if (isFirstHerb)
			{
				m_firstHerb = (int)h.Type;
				m_hasFirstHerb = true;
			}
			else
			{
				m_secondHerb = (int)h.Type;
				m_hasSecondHerb = true;
			}
			GrindEffect();
			Object.Destroy(h.gameObject);
			UpdateIndicators();
		}

		private void UpdateIndicators()
		{
			if (m_hasFirstHerb)
			{
				Indicators[0].gameObject.SetActive(value: true);
				Indicators[0].sprite = Sprites_Herbs[m_firstHerb];
			}
			else
			{
				Indicators[0].gameObject.SetActive(value: false);
			}
			if (m_hasSecondHerb)
			{
				Indicators[1].gameObject.SetActive(value: true);
				Indicators[1].sprite = Sprites_Herbs[m_secondHerb];
			}
			else
			{
				Indicators[1].gameObject.SetActive(value: false);
			}
			if (m_hasMeatCore)
			{
				Indicators[2].gameObject.SetActive(value: true);
				Indicators[2].sprite = Sprites_Cores[m_meatcoreType];
			}
			else
			{
				Indicators[2].gameObject.SetActive(value: false);
			}
		}

		private void MachineExplosion()
		{
		}

		public void Grind(int derp)
		{
			if (!m_hasMeatCore || !m_hasFirstHerb || !m_hasSecondHerb)
			{
				SM.PlayGenericSound(AudEvent_Fail, base.transform.position);
				return;
			}
			SM.PlayGenericSound(AudEvent_Success, base.transform.position);
			int num = Mathf.Min(m_firstHerb, m_secondHerb);
			int num2 = Mathf.Max(m_firstHerb, m_secondHerb);
			int powerupType = 0;
			switch (num)
			{
			case 0:
				switch (num2)
				{
				case 0:
					powerupType = 0;
					break;
				case 1:
					powerupType = 5;
					break;
				case 2:
					powerupType = 6;
					break;
				case 3:
					powerupType = 7;
					break;
				case 4:
					powerupType = -2;
					break;
				}
				break;
			case 1:
				switch (num2)
				{
				case 1:
					powerupType = 3;
					break;
				case 2:
					powerupType = 8;
					break;
				case 3:
					powerupType = 9;
					break;
				case 4:
					powerupType = 10;
					break;
				}
				break;
			case 2:
				switch (num2)
				{
				case 2:
					powerupType = 2;
					break;
				case 3:
					powerupType = 11;
					break;
				case 4:
					powerupType = 12;
					break;
				}
				break;
			case 3:
				switch (num2)
				{
				case 3:
					powerupType = 4;
					break;
				case 4:
					powerupType = -2;
					break;
				}
				break;
			case 4:
				if (num2 == 4)
				{
					powerupType = 1;
				}
				break;
			}
			m_hasFirstHerb = false;
			m_hasMeatCore = false;
			m_hasSecondHerb = false;
			UpdateIndicators();
			GameObject gameObject = Object.Instantiate(UncookedLink.GetGameObject(), SpawnPoint.position, SpawnPoint.rotation);
			RW_Powerup component = gameObject.GetComponent<RW_Powerup>();
			if (GM.ZMaster != null)
			{
				GM.ZMaster.FlagM.AddToFlag("s_c", 1);
			}
			component.Cooked = false;
			component.PowerupType = (PowerupType)powerupType;
			component.SetMCMadeWith((RotrwMeatCore.CoreType)m_meatcoreType);
			switch (m_meatcoreType)
			{
			case 0:
				component.PowerupDuration = PowerUpDuration.VeryShort;
				component.PowerupIntensity = PowerUpIntensity.Low;
				break;
			case 1:
				component.PowerupDuration = PowerUpDuration.VeryShort;
				component.PowerupIntensity = PowerUpIntensity.High;
				component.isPuke = true;
				break;
			case 2:
				component.PowerupDuration = PowerUpDuration.Short;
				component.PowerupIntensity = PowerUpIntensity.Low;
				component.isInverted = true;
				break;
			case 3:
				component.PowerupDuration = PowerUpDuration.Blip;
				component.PowerupIntensity = PowerUpIntensity.High;
				break;
			case 4:
				component.PowerupDuration = PowerUpDuration.Full;
				component.PowerupIntensity = PowerUpIntensity.High;
				break;
			case 5:
				component.PowerupDuration = PowerUpDuration.Short;
				component.PowerupIntensity = PowerUpIntensity.Medium;
				break;
			case 6:
				component.PowerupDuration = PowerUpDuration.SuperLong;
				component.PowerupIntensity = PowerUpIntensity.Low;
				break;
			case 7:
				component.PowerupDuration = PowerUpDuration.Full;
				component.PowerupIntensity = PowerUpIntensity.High;
				component.isInverted = true;
				break;
			}
		}
	}
}
