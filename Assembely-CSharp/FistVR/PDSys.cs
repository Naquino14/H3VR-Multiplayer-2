using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class PDSys
	{
		public enum PowerState
		{
			Powered,
			EmergencyPower,
			Unpowered
		}

		public enum CoolantState
		{
			Pressurized,
			Unpressurized
		}

		public enum ThermalState
		{
			Cooled,
			Uncooled
		}

		public enum DamageState
		{
			Undamaged,
			Compromised,
			Nonfunctional
		}

		public PDComponentID ID;

		public PDSysType SysType;

		public PDArmorType ArmType;

		public PDOmniModuleType OmnType;

		public bool NeedsPower = true;

		public bool NeedsCoolant = true;

		public bool TakesDamFromThermal = true;

		public bool CanIgnite = true;

		public PowerState PState;

		public CoolantState CState;

		public ThermalState TState;

		public DamageState DState;

		protected float m_integrity = 100f;

		protected float m_coolantPressure = 100f;

		protected float m_heat;

		public float MaxPowerAvailable = 1f;

		public PD PD;

		[SerializeField]
		protected List<int> m_powerParents = new List<int>();

		[SerializeField]
		protected List<int> m_powerChildren = new List<int>();

		[SerializeField]
		protected List<int> m_coolantParents = new List<int>();

		[SerializeField]
		protected List<int> m_coolantChildren = new List<int>();

		protected PDInterface m_interface;

		protected bool m_hasInterface;

		public float GetIntegrity()
		{
			return m_integrity;
		}

		public float GetHeat()
		{
			return m_heat;
		}

		public void RemoveHeat(float h)
		{
			m_heat -= h;
		}

		public void AddHeat(float h)
		{
			m_heat += h;
		}

		public void ConnectInterface(PDInterface Interface)
		{
			m_interface = Interface;
			if (m_interface != null)
			{
				m_hasInterface = true;
			}
		}

		public void AddPowerChild(PDComponentID child)
		{
			if (!m_powerChildren.Contains((int)child))
			{
				m_powerChildren.Add((int)child);
			}
		}

		public void AddPowerParent(PDComponentID parent)
		{
			if (!m_powerParents.Contains((int)parent))
			{
				m_powerParents.Add((int)parent);
			}
		}

		public void AddCoolantChild(PDComponentID child)
		{
			if (!m_coolantChildren.Contains((int)child))
			{
				m_coolantChildren.Add((int)child);
			}
		}

		public void AddCoolantParent(PDComponentID parent)
		{
			if (!m_coolantParents.Contains((int)parent))
			{
				m_coolantParents.Add((int)parent);
			}
		}

		public void UpdatePowerState()
		{
			PState = PowerState.Unpowered;
			float num = 0f;
			for (int i = 0; i < m_powerParents.Count; i++)
			{
				PState = (PowerState)Mathf.Min((int)PState, (int)PD.GetSys(m_powerParents[i]).PState);
				num = Mathf.Max((int)PD.GetSys(m_powerParents[i]).MaxPowerAvailable, num);
			}
			MaxPowerAvailable = num;
		}

		public void UpdateCoolantState(float t)
		{
			CState = CoolantState.Unpressurized;
			float num = 0f;
			if (ID == PDComponentID.SYS_CCS)
			{
				CState = CoolantState.Pressurized;
				num = PD.TempCoolantRate;
				m_heat -= m_heat * 2f * t;
			}
			for (int i = 0; i < m_coolantParents.Count; i++)
			{
				CState = (CoolantState)Mathf.Min((int)CState, (int)PD.GetSys(m_coolantParents[i]).CState);
				num += (float)(int)PD.GetSys(m_coolantParents[i]).m_coolantPressure;
			}
			if (m_coolantParents.Count > 0)
			{
				num /= (float)m_coolantParents.Count;
			}
			m_coolantPressure = Mathf.MoveTowards(m_coolantPressure, num, t * 10f);
			if (m_coolantChildren.Count > 0)
			{
				for (int j = 0; j < m_coolantChildren.Count; j++)
				{
					float num2 = m_coolantPressure / 100f * PD.GetSys(m_coolantChildren[j]).GetHeat() * t;
					PD.GetSys(m_coolantChildren[j]).RemoveHeat(num2);
					m_heat += num2;
				}
				m_heat = Mathf.Clamp(m_heat, 0f, 200f);
			}
		}

		public void Print(string s)
		{
			m_interface.PrintEventMessage(this, s);
		}

		public string GetIdentifier()
		{
			return ID.ToString();
		}
	}
}
