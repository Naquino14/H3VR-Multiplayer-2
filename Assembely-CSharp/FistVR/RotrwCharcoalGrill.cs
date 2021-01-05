using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class RotrwCharcoalGrill : MonoBehaviour
	{
		public RotrwCharcoalGrillHandle Lid;

		public RotrwCharcoalGrillGrate Grate;

		public Transform GrillGrateSpot;

		private float m_timeSincePickup = 1f;

		public AudioSource AudSource_Fire;

		private float m_currentVolume;

		private float m_targetVolume;

		private int m_numCharcoalActive;

		public LayerMask LM_Search;

		public Transform CheckPoint;

		public Transform CheckPointSausages;

		public float CheckRadius = 0.2f;

		private float m_charcoalCheckTick = 1f;

		public List<FVRObject> GrilledPowerups;

		public AudioEvent AudEvent_Success;

		public List<GameObject> Explosions;

		public bool CanPickupGrate()
		{
			if (m_timeSincePickup >= 4f)
			{
				return true;
			}
			return false;
		}

		public void DemountGrate()
		{
			m_timeSincePickup = 0f;
		}

		public void MountGrate()
		{
			m_timeSincePickup = 0f;
		}

		protected void Update()
		{
			if (m_timeSincePickup < 5f)
			{
				m_timeSincePickup += Time.deltaTime;
			}
			m_targetVolume = Mathf.Clamp((float)m_numCharcoalActive * 0.08f, 0f, 0.4f);
			m_currentVolume = Mathf.MoveTowards(m_currentVolume, m_targetVolume, Time.deltaTime * 0.3f);
			if (m_currentVolume > 0f && !AudSource_Fire.isPlaying)
			{
				AudSource_Fire.Play();
			}
			else if (m_currentVolume <= 0f && AudSource_Fire.isPlaying)
			{
				AudSource_Fire.Stop();
			}
			if (m_currentVolume > 0f)
			{
				AudSource_Fire.volume = m_currentVolume;
			}
			if (m_charcoalCheckTick > 0f)
			{
				m_charcoalCheckTick -= Time.deltaTime;
				return;
			}
			m_charcoalCheckTick = Random.Range(1f, 2f);
			Collider[] array = Physics.OverlapSphere(CheckPoint.position, CheckRadius, LM_Search, QueryTriggerInteraction.Collide);
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				RotrwCharcoal component = array[i].gameObject.GetComponent<RotrwCharcoal>();
				if (component != null && component.IsOnFire)
				{
					num++;
				}
			}
			m_numCharcoalActive = num;
			if (!Lid.IsLidClosed() || m_numCharcoalActive <= 0)
			{
				return;
			}
			bool flag = false;
			List<RW_Powerup> list = new List<RW_Powerup>();
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j].attachedRigidbody != null)
				{
					RW_Powerup component2 = array[j].attachedRigidbody.gameObject.GetComponent<RW_Powerup>();
					if (component2 != null && !list.Contains(component2) && !component2.Cooked)
					{
						list.Add(component2);
						flag = true;
					}
				}
			}
			for (int num2 = list.Count - 1; num2 >= 0; num2--)
			{
				RW_Powerup rW_Powerup = list[num2];
				int powerupType = (int)rW_Powerup.PowerupType;
				PowerUpDuration powerupDuration = rW_Powerup.PowerupDuration;
				PowerUpIntensity powerupIntensity = rW_Powerup.PowerupIntensity;
				bool isPuke = rW_Powerup.isPuke;
				bool isInverted = rW_Powerup.isInverted;
				RotrwMeatCore.CoreType mCMadeWith = rW_Powerup.GetMCMadeWith();
				Vector3 position = list[num2].transform.position;
				Quaternion rotation = list[num2].transform.rotation;
				Object.Destroy(list[num2].gameObject);
				if (powerupType >= 0)
				{
					GameObject gameObject = Object.Instantiate(GrilledPowerups[powerupType].GetGameObject(), position, rotation);
					RW_Powerup component3 = gameObject.GetComponent<RW_Powerup>();
					component3.SetParams(component3.PowerupType, powerupIntensity, powerupDuration, isPuke, isInverted);
					component3.SetMCMadeWith(mCMadeWith);
					SetDisplayFlags(component3);
				}
				else
				{
					for (int k = 0; k < Explosions.Count; k++)
					{
						Object.Instantiate(Explosions[k], position, Random.rotation);
					}
				}
			}
			if (flag)
			{
				SM.PlayGenericSound(AudEvent_Success, GrillGrateSpot.position);
			}
		}

		private void SetDisplayFlags(RW_Powerup p)
		{
			if (!(GM.ZMaster == null))
			{
				string empty = string.Empty;
				if (!p.isInverted)
				{
					int powerupType = (int)p.PowerupType;
					empty = "flagRecipe" + powerupType;
				}
				else
				{
					int powerupType2 = (int)p.PowerupType;
					empty = "flagRecipeInverted" + powerupType2;
				}
				string flag = "flagCoreName" + (int)p.GetMCMadeWith();
				string flag2 = "flagCoreIntensity" + (int)p.GetMCMadeWith();
				string flag3 = "flagCoreDuration" + (int)p.GetMCMadeWith();
				string flag4 = "flagCoreSpecial" + (int)p.GetMCMadeWith();
				GM.ZMaster.FlagM.SetFlag(empty, 1);
				GM.ZMaster.FlagM.SetFlag(flag, 1);
				GM.ZMaster.FlagM.SetFlag(flag4, 1);
				switch (p.PowerupType)
				{
				case PowerupType.Health:
					GM.ZMaster.FlagM.SetFlag(flag2, 1);
					break;
				case PowerupType.QuadDamage:
					GM.ZMaster.FlagM.SetFlag(flag2, 1);
					GM.ZMaster.FlagM.SetFlag(flag3, 1);
					break;
				case PowerupType.InfiniteAmmo:
					GM.ZMaster.FlagM.SetFlag(flag3, 1);
					break;
				case PowerupType.Invincibility:
					GM.ZMaster.FlagM.SetFlag(flag2, 1);
					GM.ZMaster.FlagM.SetFlag(flag3, 1);
					break;
				case PowerupType.Ghosted:
					GM.ZMaster.FlagM.SetFlag(flag3, 1);
					break;
				case PowerupType.FarOutMeat:
					GM.ZMaster.FlagM.SetFlag(flag3, 1);
					break;
				case PowerupType.MuscleMeat:
					GM.ZMaster.FlagM.SetFlag(flag2, 1);
					GM.ZMaster.FlagM.SetFlag(flag3, 1);
					break;
				case PowerupType.HomeTown:
					break;
				case PowerupType.SnakeEye:
					GM.ZMaster.FlagM.SetFlag(flag3, 1);
					break;
				case PowerupType.Blort:
					GM.ZMaster.FlagM.SetFlag(flag3, 1);
					break;
				case PowerupType.Regen:
					GM.ZMaster.FlagM.SetFlag(flag2, 1);
					GM.ZMaster.FlagM.SetFlag(flag3, 1);
					break;
				case PowerupType.Cyclops:
					GM.ZMaster.FlagM.SetFlag(flag3, 1);
					break;
				case PowerupType.WheredIGo:
					break;
				}
			}
		}
	}
}
