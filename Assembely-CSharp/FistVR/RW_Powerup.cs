using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class RW_Powerup : FVRPhysicalObject
	{
		[Header("Powerup Stuff")]
		public PowerupType PowerupType;

		public PowerUpIntensity PowerupIntensity;

		public PowerUpDuration PowerupDuration;

		public bool isPuke;

		public bool isInverted;

		private bool hasTriggered;

		public bool Cooked = true;

		public GameObject Payload;

		public AudioEvent AudEvent_DIng;

		public GameObject NameDisplay_Normal;

		public GameObject NameDisplay_Inverted;

		public List<GameObject> Symbols_Duration;

		public List<GameObject> Symbols_Intensity;

		public GameObject Mold;

		private RotrwMeatCore.CoreType m_madeWithMeatCore;

		public void SetMCMadeWith(RotrwMeatCore.CoreType c)
		{
			m_madeWithMeatCore = c;
		}

		public RotrwMeatCore.CoreType GetMCMadeWith()
		{
			return m_madeWithMeatCore;
		}

		protected override void Awake()
		{
			base.Awake();
			UpdateSymbols();
		}

		public void SetParams(PowerupType t, PowerUpIntensity intensity, PowerUpDuration d, bool p, bool inv)
		{
			PowerupType = t;
			PowerupIntensity = intensity;
			PowerupDuration = d;
			isPuke = p;
			isInverted = inv;
			if (inv)
			{
				if (NameDisplay_Inverted != null)
				{
					NameDisplay_Inverted.SetActive(value: true);
				}
				if (NameDisplay_Normal != null)
				{
					NameDisplay_Normal.SetActive(value: false);
				}
			}
			else
			{
				if (NameDisplay_Inverted != null)
				{
					NameDisplay_Inverted.SetActive(value: false);
				}
				if (NameDisplay_Normal != null)
				{
					NameDisplay_Normal.SetActive(value: true);
				}
			}
			if (isPuke && Mold != null)
			{
				Mold.SetActive(value: true);
			}
			UpdateSymbols();
		}

		private void UpdateSymbols()
		{
			bool flag = false;
			bool flag2 = false;
			switch (PowerupType)
			{
			case PowerupType.Health:
				flag = true;
				break;
			case PowerupType.QuadDamage:
				flag = true;
				flag2 = true;
				break;
			case PowerupType.InfiniteAmmo:
				flag2 = true;
				break;
			case PowerupType.Invincibility:
				flag = true;
				flag2 = true;
				break;
			case PowerupType.Ghosted:
				flag2 = true;
				break;
			case PowerupType.FarOutMeat:
				flag2 = true;
				break;
			case PowerupType.MuscleMeat:
				flag = true;
				flag2 = true;
				break;
			case PowerupType.SnakeEye:
				flag2 = true;
				break;
			case PowerupType.Blort:
				flag2 = true;
				break;
			case PowerupType.Regen:
				flag = true;
				flag2 = true;
				break;
			case PowerupType.Cyclops:
				flag2 = true;
				break;
			}
			for (int i = 0; i < Symbols_Duration.Count; i++)
			{
				if (PowerupDuration == (PowerUpDuration)i && flag2)
				{
					if (Symbols_Duration[i] != null)
					{
						Symbols_Duration[i].SetActive(value: true);
					}
				}
				else if (Symbols_Duration[i] != null)
				{
					Symbols_Duration[i].SetActive(value: false);
				}
			}
			for (int j = 0; j < Symbols_Intensity.Count; j++)
			{
				if (PowerupIntensity == (PowerUpIntensity)j && flag)
				{
					if (Symbols_Intensity[j] != null)
					{
						Symbols_Intensity[j].SetActive(value: true);
					}
				}
				else if (Symbols_Intensity[j] != null)
				{
					Symbols_Intensity[j].SetActive(value: false);
				}
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 b = GM.CurrentPlayerBody.Head.transform.position + Vector3.up * -0.1f;
			if (Vector3.Distance(base.transform.position, b) < 0.15f && Cooked)
			{
				PowerUp(hand);
			}
		}

		public void Detonate()
		{
			if (Payload != null)
			{
				GameObject gameObject = Object.Instantiate(Payload, base.transform.position, base.transform.rotation);
				PowerUp_Cloud component = gameObject.GetComponent<PowerUp_Cloud>();
				component.SetParams(PowerupType, PowerupIntensity, PowerupDuration, isPuke, isInverted);
			}
			Object.Destroy(GameObject);
		}

		private void PowerUp(FVRViveHand hand)
		{
			if (!hasTriggered)
			{
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_DIng, base.transform.position);
				GM.CurrentSceneSettings.OnPowerupUse(PowerupType);
				hasTriggered = true;
				EndInteraction(hand);
				hand.ForceSetInteractable(null);
				GM.CurrentPlayerBody.ActivatePower(PowerupType, PowerupIntensity, PowerupDuration, isPuke, isInverted);
				Object.Destroy(base.gameObject);
			}
		}
	}
}
