using UnityEngine;

namespace FistVR
{
	public class OptionsScreen_MeatGrinder : MonoBehaviour
	{
		public OptionsPanel_ButtonSet OBS_Combo;

		public OptionsPanel_ButtonSet OBS_Side1;

		public OptionsPanel_ButtonSet OBS_Side2;

		public OptionsPanel_ButtonSet OBS_Side3;

		public OptionsPanel_ButtonSet OBS_Dessert;

		public void Awake()
		{
			InitScreen();
		}

		public void InitScreen()
		{
			OBS_Combo.SetSelectedButton((int)GM.Options.MeatGrinderFlags.MGMode);
			OBS_Side1.SetSelectedButton((int)GM.Options.MeatGrinderFlags.AIMood);
			OBS_Side2.SetSelectedButton((int)GM.Options.MeatGrinderFlags.PrimaryLight);
			OBS_Side3.SetSelectedButton((int)GM.Options.MeatGrinderFlags.SecondaryLight);
			OBS_Dessert.SetSelectedButton((int)GM.Options.MeatGrinderFlags.NarratorMode);
		}

		public void SetCombo(int i)
		{
			GM.Options.MeatGrinderFlags.MGMode = (MeatGrinderFlags.MeatGrinderMode)i;
			GM.Options.SaveToFile();
		}

		public void SetSide1(int i)
		{
			GM.Options.MeatGrinderFlags.AIMood = (MeatGrinderMaster.EventAI.EventAIMood)i;
			GM.Options.SaveToFile();
		}

		public void SetSide2(int i)
		{
			GM.Options.MeatGrinderFlags.PrimaryLight = (MeatGrinderFlags.LightSourceOption)i;
			GM.Options.SaveToFile();
		}

		public void SetSide3(int i)
		{
			GM.Options.MeatGrinderFlags.SecondaryLight = (MeatGrinderFlags.LightSourceOption)i;
			GM.Options.SaveToFile();
		}

		public void SetDessert(int i)
		{
			GM.Options.MeatGrinderFlags.NarratorMode = (MeatGrinderFlags.MeatGrinderNarratorMode)i;
			GM.Options.SaveToFile();
		}
	}
}
