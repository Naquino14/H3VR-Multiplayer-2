using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class OpticUI : MonoBehaviour
	{
		public List<Text> SettingNames;

		public Transform Frame;

		public GameObject Arrow_Left;

		public GameObject Arrow_Right;

		private Amplifier m_amp;

		public void SetAmp(Amplifier a)
		{
			m_amp = a;
		}

		public void UISetSetting(int i)
		{
			if (m_amp != null)
			{
				m_amp.GoToSetting(i);
				UpdateUI(m_amp);
			}
		}

		public void UIArrowUp()
		{
			if (m_amp != null)
			{
				m_amp.SetCurSettingUp(cycle: false);
				UpdateUI(m_amp);
			}
		}

		public void UIArrowDown()
		{
			if (m_amp != null)
			{
				m_amp.SetCurSettingDown();
				UpdateUI(m_amp);
			}
		}

		public void UpdateUI(LadderSight L)
		{
			for (int i = 0; i < SettingNames.Count; i++)
			{
				UpdateTextAndArrows(L, i);
			}
			UpdateFrame(L);
		}

		public void UpdateUI(Amplifier A)
		{
			for (int i = 0; i < SettingNames.Count; i++)
			{
				UpdateTextAndArrows(A, i);
			}
			UpdateFrame(A);
		}

		private void UpdateTextAndArrows(LadderSight L, int index)
		{
			if (index >= 1)
			{
				SettingNames[index].text = string.Empty;
				SettingNames[index].gameObject.SetActive(value: false);
				return;
			}
			SettingNames[index].gameObject.SetActive(value: true);
			SettingNames[index].text = L.RangeNames[L.Setting];
			if (L.Setting > 0)
			{
				Arrow_Left.SetActive(value: true);
			}
			else
			{
				Arrow_Left.SetActive(value: false);
			}
			if (L.Setting < L.RangeNames.Count - 1)
			{
				Arrow_Right.SetActive(value: true);
			}
			else
			{
				Arrow_Right.SetActive(value: false);
			}
		}

		private void UpdateTextAndArrows(Amplifier A, int index)
		{
			if (index >= A.OptionTypes.Count)
			{
				SettingNames[index].text = string.Empty;
				return;
			}
			switch (A.OptionTypes[index])
			{
			case OpticOptionType.Zero:
				SettingNames[index].text = "Base Zero: " + A.ZeroDistances[A.ZeroDistanceIndex] + "m";
				if (A.ZeroDistanceIndex > 0)
				{
					Arrow_Left.SetActive(value: true);
				}
				else
				{
					Arrow_Left.SetActive(value: false);
				}
				if (A.ZeroDistanceIndex < A.ZeroDistances.Count - 1)
				{
					Arrow_Right.SetActive(value: true);
				}
				else
				{
					Arrow_Right.SetActive(value: false);
				}
				break;
			case OpticOptionType.Magnification:
				SettingNames[index].text = "Magnification: " + A.ZoomSettings[A.m_zoomSettingIndex].Magnification + "x";
				if (A.m_zoomSettingIndex > 0)
				{
					Arrow_Left.SetActive(value: true);
				}
				else
				{
					Arrow_Left.SetActive(value: false);
				}
				if (A.m_zoomSettingIndex < A.ZoomSettings.Count - 1)
				{
					Arrow_Right.SetActive(value: true);
				}
				else
				{
					Arrow_Right.SetActive(value: false);
				}
				break;
			case OpticOptionType.ReticleLum:
				break;
			case OpticOptionType.ReticleType:
				break;
			case OpticOptionType.FlipState:
				break;
			case OpticOptionType.ElevationTweak:
				SettingNames[index].text = "Elevation: " + (float)A.ElevationStep * 0.25f + "MOA";
				Arrow_Left.SetActive(value: true);
				Arrow_Right.SetActive(value: true);
				break;
			case OpticOptionType.WindageTweak:
				SettingNames[index].text = "Windage: " + (float)A.WindageStep * 0.25f + "MOA";
				Arrow_Left.SetActive(value: true);
				Arrow_Right.SetActive(value: true);
				break;
			}
		}

		private void UpdateFrame(Amplifier A)
		{
			if (A.OptionTypes.Count == 0)
			{
				Frame.gameObject.SetActive(value: false);
				return;
			}
			int curSelectedOptionIndex = A.CurSelectedOptionIndex;
			Frame.localPosition = SettingNames[curSelectedOptionIndex].transform.localPosition;
		}

		private void UpdateFrame(LadderSight L)
		{
			int index = 0;
			Frame.localPosition = SettingNames[index].transform.localPosition;
		}
	}
}
