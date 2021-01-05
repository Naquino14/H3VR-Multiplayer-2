using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class MF2_Sapper : FVRPhysicalObject
	{
		[Header("VFX")]
		public GameObject SapperEffect;

		public Transform Dial;

		public Transform DialPointUp;

		public Transform DialPointDown;

		private float m_sappingPower = 15f;

		private bool m_isSapperActive;

		private float detectPulse = 0.1f;

		public GameObject PulseExplosion;

		private bool[] switches = new bool[2];

		public Transform Switch0;

		public Transform Switch1;

		private HashSet<Rigidbody> rbs = new HashSet<Rigidbody>();

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (m_isSapperActive)
			{
				detectPulse -= Time.deltaTime;
				if (detectPulse <= 0f)
				{
					detectPulse = 0.1f;
					Pulse();
				}
				m_sappingPower -= Time.deltaTime;
				if (m_sappingPower <= 0f)
				{
					ShutOff();
				}
			}
			else if (m_sappingPower < 15f)
			{
				m_sappingPower += Time.deltaTime;
			}
			Dial.localPosition = Vector3.Lerp(DialPointUp.localPosition, DialPointDown.localPosition, m_sappingPower / 15f);
		}

		private void Pulse()
		{
			Object.Instantiate(PulseExplosion, base.transform.position, Quaternion.identity);
		}

		private void SetPower(bool isOn)
		{
			if (!isOn)
			{
				m_isSapperActive = false;
				SapperEffect.SetActive(value: false);
			}
			else if (isOn)
			{
				m_isSapperActive = true;
				SapperEffect.SetActive(value: true);
			}
		}

		private void UpdatePowerState()
		{
			if (switches[0] && switches[1])
			{
				SetPower(isOn: true);
			}
			else
			{
				SetPower(isOn: false);
			}
			if (switches[0])
			{
				Switch0.localEulerAngles = new Vector3(-18f, 0f, 0f);
			}
			else
			{
				Switch0.localEulerAngles = new Vector3(18f, 0f, 0f);
			}
			if (switches[1])
			{
				Switch1.localEulerAngles = new Vector3(-18f, 0f, 0f);
			}
			else
			{
				Switch1.localEulerAngles = new Vector3(18f, 0f, 0f);
			}
		}

		private void ShutOff()
		{
			switches[0] = false;
			switches[1] = false;
			UpdatePowerState();
		}

		public void ToggleSwitch(int which)
		{
			switches[which] = !switches[which];
			UpdatePowerState();
		}
	}
}
