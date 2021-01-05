using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class GasPumperPal : MonoBehaviour
	{
		public Transform JerryCanPlacement;

		public List<GameObject> FueledObjects;

		private bool m_hasFuel;

		public string FlagWhenFueled;

		public int FlagValueWhenFueled;

		public FVRObject JerryCan;

		private JerryCan m_can;

		public AudioEvent AudEvent_Click;

		public Transform OnButton;

		public Transform OffButton;

		public AudioEvent AudEvent_Load;

		public void Start()
		{
			if (GM.ZMaster.FlagM.GetFlagValue(FlagWhenFueled) >= FlagValueWhenFueled)
			{
				SetFueledState(f: true);
			}
			else
			{
				SetFueledState(f: false);
			}
		}

		public bool HasFuel()
		{
			return m_hasFuel;
		}

		private void Update()
		{
			if (!m_hasFuel && GM.ZMaster.FlagM.GetFlagValue(FlagWhenFueled) >= FlagValueWhenFueled)
			{
				m_hasFuel = true;
			}
		}

		private void SetFueledState(bool f)
		{
			if (f)
			{
				m_hasFuel = true;
				return;
			}
			m_hasFuel = false;
			TurnOff(1);
		}

		public void TurnOn(int af)
		{
			SM.PlayGenericSound(AudEvent_Click, OnButton.position);
			if (m_hasFuel)
			{
				for (int i = 0; i < FueledObjects.Count; i++)
				{
					FueledObjects[i].SendMessage("TurnOn", SendMessageOptions.DontRequireReceiver);
				}
			}
		}

		public void TurnOff(int af)
		{
			SM.PlayGenericSound(AudEvent_Click, OffButton.position);
			for (int i = 0; i < FueledObjects.Count; i++)
			{
				FueledObjects[i].SendMessage("TurnOff", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
