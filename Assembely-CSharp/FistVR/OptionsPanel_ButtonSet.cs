using System;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class OptionsPanel_ButtonSet : MonoBehaviour
	{
		public Color SelectedColor;

		public Color UnSelectedColor;

		public Color HighlightedColor;

		public Image[] ButtonImagesInSet;

		public bool UsesPointableButtons;

		public FVRPointableButton[] ButtonsInSet;

		private bool m_isHighLighted;

		public int selectedButton;

		private void Awake()
		{
			UpdateButtonVisual();
		}

		public void SetSelectedButton(int index)
		{
			selectedButton = index;
			UpdateButtonVisual();
		}

		public void SetSelectedButton(bool b)
		{
			selectedButton = Convert.ToInt32(!b);
			UpdateButtonVisual();
		}

		private void UpdateButtonVisual()
		{
			if (UsesPointableButtons)
			{
				for (int i = 0; i < ButtonsInSet.Length; i++)
				{
					ButtonsInSet[i].ColorUnselected = UnSelectedColor;
					ButtonsInSet[i].ColorSelected = HighlightedColor;
				}
				ButtonsInSet[selectedButton].ColorUnselected = SelectedColor;
				ButtonsInSet[selectedButton].ColorSelected = HighlightedColor;
				for (int j = 0; j < ButtonsInSet.Length; j++)
				{
					ButtonsInSet[j].ForceUpdate();
				}
			}
			else
			{
				for (int k = 0; k < ButtonImagesInSet.Length; k++)
				{
					ButtonImagesInSet[k].color = UnSelectedColor;
				}
				ButtonImagesInSet[selectedButton].color = SelectedColor;
			}
		}

		public void EnableAllButtons()
		{
			for (int i = 0; i < ButtonImagesInSet.Length; i++)
			{
				ButtonImagesInSet[i].color = SelectedColor;
			}
		}
	}
}
