using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class PDElectricalSystem : MonoBehaviour
	{
		[Serializable]
		public class PowerSource
		{
			public PDElectricalSystem ESys;

			public PDComponentID ID;

			private PDSys.PowerState BaseState;

			public void Init()
			{
				ESys.PD.GetSys(5).PState = BaseState;
			}

			public void Tick(float t)
			{
			}
		}

		public PD PD;

		public List<PowerSource> PowerSources;

		public void Init()
		{
			for (int i = 0; i < PowerSources.Count; i++)
			{
				PowerSources[i].ESys = this;
				PowerSources[i].Init();
			}
		}

		public void Tick(float t)
		{
			for (int i = 0; i < PowerSources.Count; i++)
			{
				PowerSources[i].Tick(t);
			}
			PD.GetSys(3).MaxPowerAvailable = 1f;
			PD.GetSys(5).MaxPowerAvailable = 0.2f;
			PD.GetSys(6).MaxPowerAvailable = 0.2f;
			PD.GetSys(5).PState = PDSys.PowerState.EmergencyPower;
			PD.GetSys(6).PState = PDSys.PowerState.EmergencyPower;
		}
	}
}
