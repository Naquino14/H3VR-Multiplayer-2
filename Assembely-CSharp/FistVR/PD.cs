using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class PD : MonoBehaviour
	{
		[SerializeField]
		protected List<int> m_validSys = new List<int>();

		[SerializeField]
		protected PDSys[] m_sys = new PDSys[256];

		public float TempCoolantRate = 50f;

		[Header("System Connections")]
		public PDElectricalSystem ESystem;

		public PDFCS FCS;

		[Header("Configs")]
		public PDSystemListConfig SLC;

		public PDHierarchyConfig HC_Power;

		public PDHierarchyConfig HC_Coolant;

		[Header("DebugTexture")]
		public Texture2D debugTex_Power;

		public Texture2D debugTex_Coolant;

		public Texture2D debugTex_Heat;

		public Color32 cPower_On;

		public Color32 cPower_Off;

		public Color32 cPower_Emergency;

		public Color32 cPower_None;

		public Color32 cCoolant_On;

		public Color32 cCoolant_Off;

		public Color32 cCoolant_None;

		public Color32 cHeat_Max;

		public Color32 cHeat_Min;

		public Color32 cHeat_None;

		public Color32[] m_colorsDebugPower;

		public Color32[] m_colorsDebugCoolant;

		public Color32[] m_colorsDebugHeat;

		private void Start()
		{
			ESystem.Init();
		}

		private void Update()
		{
			float deltaTime = Time.deltaTime;
			ESystem.Tick(deltaTime);
			if (Input.GetKeyDown(KeyCode.H))
			{
			}
			if (Input.GetKeyDown(KeyCode.J))
			{
			}
			if (Input.GetKeyDown(KeyCode.K))
			{
			}
			if (Input.GetKeyDown(KeyCode.L))
			{
			}
			int num = debugTex_Power.width * debugTex_Power.height;
			if (num != m_colorsDebugPower.Length)
			{
				m_colorsDebugPower = new Color32[num];
			}
			num = debugTex_Coolant.width * debugTex_Coolant.height;
			if (num != m_colorsDebugCoolant.Length)
			{
				m_colorsDebugCoolant = new Color32[num];
			}
			num = debugTex_Heat.width * debugTex_Heat.height;
			if (num != m_colorsDebugHeat.Length)
			{
				m_colorsDebugHeat = new Color32[num];
			}
			for (int i = 0; i < m_validSys.Count; i++)
			{
				PDSys pDSys = m_sys[m_validSys[i]];
				if (pDSys.NeedsPower)
				{
					pDSys.UpdatePowerState();
				}
				if (pDSys.NeedsCoolant || pDSys.TakesDamFromThermal)
				{
					pDSys.UpdateCoolantState(deltaTime);
				}
				if (pDSys.NeedsPower || pDSys.ID == PDComponentID.SYS_PDU || pDSys.ID == PDComponentID.SYS_EPO0 || pDSys.ID == PDComponentID.SYS_EPO1)
				{
					if (pDSys.PState == PDSys.PowerState.Powered)
					{
						ref Color32 reference = ref m_colorsDebugPower[m_validSys[i]];
						reference = cPower_On;
					}
					else if (pDSys.PState == PDSys.PowerState.EmergencyPower)
					{
						ref Color32 reference2 = ref m_colorsDebugPower[m_validSys[i]];
						reference2 = cPower_Emergency;
					}
					else
					{
						ref Color32 reference3 = ref m_colorsDebugPower[m_validSys[i]];
						reference3 = cPower_Off;
					}
				}
				else
				{
					ref Color32 reference4 = ref m_colorsDebugPower[m_validSys[i]];
					reference4 = cPower_None;
				}
				if (pDSys.NeedsCoolant || pDSys.ID == PDComponentID.SYS_CCS)
				{
					if (pDSys.CState == PDSys.CoolantState.Pressurized)
					{
						ref Color32 reference5 = ref m_colorsDebugCoolant[m_validSys[i]];
						reference5 = cCoolant_On;
					}
					else
					{
						ref Color32 reference6 = ref m_colorsDebugCoolant[m_validSys[i]];
						reference6 = cCoolant_Off;
					}
					float t = pDSys.GetHeat() / 200f;
					Color32 color = Color32.Lerp(cHeat_Min, cHeat_Max, t);
					m_colorsDebugHeat[m_validSys[i]] = color;
				}
				else
				{
					ref Color32 reference7 = ref m_colorsDebugCoolant[m_validSys[i]];
					reference7 = cCoolant_None;
					ref Color32 reference8 = ref m_colorsDebugHeat[m_validSys[i]];
					reference8 = cHeat_None;
				}
			}
			debugTex_Heat.SetPixels32(m_colorsDebugHeat);
			debugTex_Heat.Apply();
			debugTex_Coolant.SetPixels32(m_colorsDebugCoolant);
			debugTex_Coolant.Apply();
			debugTex_Power.SetPixels32(m_colorsDebugPower);
			debugTex_Power.Apply();
		}

		private int GetZone(Vector3 worldPoint)
		{
			return 0;
		}

		public PDSys GetSys(int i)
		{
			return m_sys[i];
		}

		[ContextMenu("ConstructDroneFromConfigFiles")]
		private void ConstructDroneFromConfigFiles()
		{
			m_validSys.Clear();
			for (int i = 0; i < SLC.Components.Count; i++)
			{
				PDSys pDSys = new PDSys();
				PDSystemListConfig.ComponentConfig componentConfig = SLC.Components[i];
				pDSys.PD = this;
				pDSys.ID = componentConfig.ID;
				pDSys.SysType = componentConfig.SysType;
				pDSys.ArmType = componentConfig.ArmType;
				pDSys.OmnType = componentConfig.OmnType;
				pDSys.NeedsPower = componentConfig.Pow;
				pDSys.NeedsCoolant = componentConfig.Col;
				pDSys.TakesDamFromThermal = componentConfig.TTD;
				pDSys.CanIgnite = componentConfig.CIG;
				m_validSys.Add((int)pDSys.ID);
				m_sys[(int)pDSys.ID] = pDSys;
			}
			for (int j = 0; j < HC_Power.IDs.Count; j++)
			{
				PDHierarchyConfig.PDChildren pDChildren = HC_Power.IDs[j];
				for (int k = 0; k < pDChildren.Children.Count; k++)
				{
					m_sys[(int)pDChildren.ID].AddPowerChild(pDChildren.Children[k]);
					m_sys[(int)pDChildren.Children[k]].AddPowerParent(pDChildren.ID);
				}
			}
			for (int l = 0; l < HC_Coolant.IDs.Count; l++)
			{
				PDHierarchyConfig.PDChildren pDChildren2 = HC_Coolant.IDs[l];
				for (int m = 0; m < pDChildren2.Children.Count; m++)
				{
					m_sys[(int)pDChildren2.ID].AddCoolantChild(pDChildren2.Children[m]);
					m_sys[(int)pDChildren2.Children[m]].AddCoolantParent(pDChildren2.ID);
				}
			}
		}
	}
}
