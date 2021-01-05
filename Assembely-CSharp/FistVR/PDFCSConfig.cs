using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New PD FCS Config", menuName = "PancakeDrone/FCS Definition", order = 0)]
	public class PDFCSConfig : ScriptableObject
	{
		[Serializable]
		public class ThrusterConfig
		{
			[SearchableEnum]
			public PDComponentID Sys;

			public PDFCS.Thruster.Facing TDir;

			public bool IsAng;

			public bool IsGAB;

			public PDThrusterConfig TConfig;
		}

		public List<ThrusterConfig> Configs;
	}
}
