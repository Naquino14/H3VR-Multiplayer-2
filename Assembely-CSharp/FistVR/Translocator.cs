using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class Translocator : MonoBehaviour
	{
		[Serializable]
		public class RessemblerCoreSlot
		{
			public Translocator T;

			public bool IsPowered;

			public GameObject CoreTrigger;

			public string FlagForBeingEnabled;

			public Renderer PoweredRenderer;

			private ZosigFlagManager M;

			public void InitFromFlagM(ZosigFlagManager m, Translocator t)
			{
				T = t;
				M = m;
				if (m.GetFlagValue(FlagForBeingEnabled) > 0)
				{
					SetPoweredState(b: true, SetTStateAfter: false);
				}
				else
				{
					SetPoweredState(b: false, SetTStateAfter: false);
				}
			}

			public void SetPoweredState(bool b, bool SetTStateAfter)
			{
				IsPowered = b;
				if (b)
				{
					PoweredRenderer.enabled = true;
					CoreTrigger.SetActive(value: false);
					M.SetFlagMaxBlend(FlagForBeingEnabled, 1);
				}
				else
				{
					PoweredRenderer.enabled = false;
					CoreTrigger.SetActive(value: true);
				}
				if (SetTStateAfter)
				{
					T.UpdatePowerState();
				}
			}
		}

		public string FlagWhenPowered;

		public int FlagValueWhenPowered;

		public List<RessemblerCoreSlot> Slots;

		public AudioSource AudSource_TeleportHum;

		public GameObject GlowStuff;

		public bool RequiresSlots = true;

		public Translocator EndPoint;

		public Transform ExitPoint;

		public AudioEvent AudEvent_Insert;

		public AudioEvent AudEvent_Arrive;

		private ZosigFlagManager M;

		public AudioSource AudSource_ChargeUp;

		public GameObject TeleportArriveEffect;

		private float m_cycleUpEnergy;

		private float m_recycleTick;

		private bool m_isPoweredUp;

		public void Init(ZosigFlagManager m)
		{
			M = m;
			for (int i = 0; i < Slots.Count; i++)
			{
				Slots[i].InitFromFlagM(m, this);
			}
			if (m.GetFlagValue(FlagWhenPowered) >= FlagValueWhenPowered)
			{
				InitPowerState(b: true);
			}
			else
			{
				InitPowerState(b: false);
			}
		}

		private void InitPowerState(bool b)
		{
			if (b)
			{
				for (int i = 0; i < Slots.Count; i++)
				{
					Slots[i].SetPoweredState(b: true, SetTStateAfter: false);
				}
			}
			UpdatePowerState();
		}

		public bool InsertCoreToSlot(int i)
		{
			if (i < Slots.Count)
			{
				SM.PlayGenericSound(AudEvent_Insert, Slots[i].CoreTrigger.transform.position);
				Slots[i].SetPoweredState(b: true, SetTStateAfter: true);
				return true;
			}
			return false;
		}

		public bool IsPowered()
		{
			if (!RequiresSlots)
			{
				return true;
			}
			bool result = true;
			for (int i = 0; i < Slots.Count; i++)
			{
				if (!Slots[i].IsPowered)
				{
					result = false;
					break;
				}
			}
			return result;
		}

		private void Update()
		{
			UpdatePlayerStuff();
		}

		private void UpdatePlayerStuff()
		{
			if (!m_isPoweredUp)
			{
				return;
			}
			if (m_recycleTick > 0f)
			{
				m_recycleTick -= Time.deltaTime;
				return;
			}
			Vector3 position = GM.CurrentPlayerBody.Head.position;
			position.y = 0f;
			Vector3 position2 = base.transform.position;
			position2.y = 0f;
			float num = Vector3.Distance(position, position2);
			float num2 = Mathf.Abs(GM.CurrentPlayerBody.transform.position.y - base.transform.position.y);
			if (num < 0.6f && (double)num2 < 0.6 && EndPoint != null)
			{
				m_cycleUpEnergy += Time.deltaTime;
				if (!AudSource_ChargeUp.isPlaying)
				{
					AudSource_ChargeUp.Play();
				}
				if (m_cycleUpEnergy > 1f)
				{
					TeleportToEndPoint(EndPoint);
				}
			}
			else
			{
				m_cycleUpEnergy = 0f;
				if (AudSource_ChargeUp.isPlaying)
				{
					AudSource_ChargeUp.Stop();
				}
			}
		}

		private void TeleportToEndPoint(Translocator e)
		{
			m_cycleUpEnergy = 0f;
			m_recycleTick = 2f;
			GM.CurrentMovementManager.TeleportToPoint(e.ExitPoint.position, isAbsolute: true);
			e.PlayArriveSound();
			float num = UnityEngine.Random.Range(0f, 1f);
			if (!(num < 0.02f))
			{
			}
		}

		public void PlayArriveSound()
		{
			SM.PlayGenericSound(AudEvent_Arrive, base.transform.position);
			UnityEngine.Object.Instantiate(TeleportArriveEffect, base.transform.position, base.transform.rotation);
		}

		private void UpdatePowerState()
		{
			bool flag = (m_isPoweredUp = IsPowered());
			if (m_isPoweredUp)
			{
				M.SetFlagMaxBlend(FlagWhenPowered, FlagValueWhenPowered);
				GlowStuff.SetActive(value: true);
				if (!AudSource_TeleportHum.isPlaying)
				{
					AudSource_TeleportHum.Play();
				}
			}
			else
			{
				GlowStuff.SetActive(value: false);
				if (AudSource_TeleportHum.isPlaying)
				{
					AudSource_TeleportHum.Stop();
				}
			}
		}
	}
}
