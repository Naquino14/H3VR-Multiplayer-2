using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New PD SystemList Config", menuName = "PancakeDrone/SystemList Definition", order = 0)]
	public class PDSystemListConfig : ScriptableObject
	{
		[Serializable]
		public class ComponentConfig
		{
			public PDComponentID ID;

			public PDSysType SysType;

			public PDArmorType ArmType;

			public PDOmniModuleType OmnType;

			public bool Pow = true;

			public bool Col = true;

			public bool TTD = true;

			public bool CIG = true;
		}

		public List<ComponentConfig> Components = new List<ComponentConfig>();

		[ContextMenu("Prime")]
		public void Prime()
		{
			Components.Clear();
			for (int i = 1; i < 255; i++)
			{
				if (Enum.IsDefined(typeof(PDComponentID), i))
				{
					ComponentConfig componentConfig = new ComponentConfig();
					componentConfig.ID = (PDComponentID)i;
					string text = componentConfig.ID.ToString();
					if (text.Contains("PDL"))
					{
						componentConfig.SysType = PDSysType.PDL;
						componentConfig.Col = false;
						componentConfig.TTD = false;
					}
					else if (text.Contains("CDL"))
					{
						componentConfig.SysType = PDSysType.CDL;
						componentConfig.Pow = false;
						componentConfig.TTD = false;
						componentConfig.CIG = false;
					}
					else if (text.Contains("ISR") || text.Contains("ISC") || text.Contains("ISS") || text.Contains("ARH") || text.Contains("ARL") || text.Contains("ART"))
					{
						componentConfig.SysType = PDSysType.STR;
						componentConfig.Pow = false;
						componentConfig.Col = false;
						componentConfig.TTD = false;
						componentConfig.CIG = false;
					}
					Components.Add(componentConfig);
				}
			}
		}
	}
}
