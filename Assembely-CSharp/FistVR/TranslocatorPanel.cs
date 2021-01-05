using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class TranslocatorPanel : MonoBehaviour
	{
		public bool RequiresFlags = true;

		public List<Translocator> EndPoints;

		private float m_panelUpdateTick = 1f;

		public List<string> FlagsForPoweredState = new List<string>();

		public List<int> FlagsValueNeededForPoweredState = new List<int>();

		public List<bool> IsEndPointPowered = new List<bool>();

		public List<string> EndPointNames = new List<string>();

		public List<Text> Labels = new List<Text>();

		private int m_selectedOption = -1;

		public Translocator ControlledTranslocator;

		public AudioEvent AudEvent_Success;

		public AudioEvent AudEvent_Failure;

		public Color Color_Selected;

		public Color Color_UnSelected;

		private void Start()
		{
			for (int i = 0; i < Labels.Count; i++)
			{
				Labels[i].color = Color_UnSelected;
			}
		}

		private void Update()
		{
			m_panelUpdateTick -= Time.deltaTime;
			if (m_panelUpdateTick <= 0f)
			{
				m_panelUpdateTick = 1f;
				UpdatePanelText();
			}
		}

		private void UpdatePanelText()
		{
			if (!RequiresFlags)
			{
				return;
			}
			for (int i = 0; i < FlagsForPoweredState.Count; i++)
			{
				if (GM.ZMaster.FlagM.GetFlagValue(FlagsForPoweredState[i]) >= FlagsValueNeededForPoweredState[i])
				{
					IsEndPointPowered[i] = true;
					Labels[i].text = EndPointNames[i];
				}
				else
				{
					IsEndPointPowered[i] = false;
					Labels[i].text = "-OFFLINE-";
				}
			}
		}

		private void SetSelectedOption(int index)
		{
			SM.PlayGenericSound(AudEvent_Success, base.transform.position);
			for (int i = 0; i < Labels.Count; i++)
			{
				if (i == index)
				{
					Labels[i].color = Color_Selected;
				}
				else
				{
					Labels[i].color = Color_UnSelected;
				}
			}
		}

		public void SelectItem0(int i)
		{
			if (IsEndPointPowered[0])
			{
				SetSelectedOption(0);
				ControlledTranslocator.EndPoint = EndPoints[0];
			}
			else
			{
				SM.PlayGenericSound(AudEvent_Failure, base.transform.position);
			}
		}

		public void SelectItem1(int i)
		{
			if (IsEndPointPowered[1])
			{
				SetSelectedOption(1);
				ControlledTranslocator.EndPoint = EndPoints[1];
			}
			else
			{
				SM.PlayGenericSound(AudEvent_Failure, base.transform.position);
			}
		}

		public void SelectItem2(int i)
		{
			if (IsEndPointPowered[2])
			{
				SetSelectedOption(2);
				ControlledTranslocator.EndPoint = EndPoints[2];
			}
			else
			{
				SM.PlayGenericSound(AudEvent_Failure, base.transform.position);
			}
		}

		public void SelectItem3(int i)
		{
			if (IsEndPointPowered[3])
			{
				SetSelectedOption(3);
				ControlledTranslocator.EndPoint = EndPoints[3];
			}
			else
			{
				SM.PlayGenericSound(AudEvent_Failure, base.transform.position);
			}
		}

		public void SelectItem4(int i)
		{
			if (IsEndPointPowered[4])
			{
				SetSelectedOption(4);
				ControlledTranslocator.EndPoint = EndPoints[4];
			}
			else
			{
				SM.PlayGenericSound(AudEvent_Failure, base.transform.position);
			}
		}

		public void SelectItem5(int i)
		{
			if (IsEndPointPowered[5])
			{
				SetSelectedOption(5);
				ControlledTranslocator.EndPoint = EndPoints[5];
			}
			else
			{
				SM.PlayGenericSound(AudEvent_Failure, base.transform.position);
			}
		}

		public void SelectItem6(int i)
		{
			if (IsEndPointPowered[6])
			{
				SetSelectedOption(6);
				ControlledTranslocator.EndPoint = EndPoints[6];
			}
			else
			{
				SM.PlayGenericSound(AudEvent_Failure, base.transform.position);
			}
		}
	}
}
