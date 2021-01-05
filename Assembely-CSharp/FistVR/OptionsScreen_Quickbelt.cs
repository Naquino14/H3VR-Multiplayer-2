using UnityEngine;

namespace FistVR
{
	public class OptionsScreen_Quickbelt : MonoBehaviour
	{
		public OptionsPanel_ButtonSet OBS_Handedness;

		public OptionsPanel_ButtonSet OBS_SlotStyle;

		public void Awake()
		{
			InitScreen();
		}

		public void InitScreen()
		{
			OBS_Handedness.SetSelectedButton(GM.Options.QuickbeltOptions.QuickbeltHandedness);
			int quickbeltPreset = GM.Options.QuickbeltOptions.QuickbeltPreset;
			OBS_SlotStyle.SetSelectedButton(quickbeltPreset);
		}

		public void SetHandedness(int i)
		{
			if (GM.CurrentSceneSettings.IsQuickbeltSwappingAllowed)
			{
				GM.Options.QuickbeltOptions.QuickbeltHandedness = i;
				GM.CurrentPlayerBody.ConfigureQuickbelt(GM.Options.QuickbeltOptions.QuickbeltPreset);
				GM.Options.SaveToFile();
			}
		}

		public void SetSlotStyle(int i)
		{
			if (GM.CurrentSceneSettings.IsQuickbeltSwappingAllowed)
			{
				GM.Options.QuickbeltOptions.QuickbeltPreset = i;
				GM.CurrentPlayerBody.ConfigureQuickbelt(i);
				GM.Options.SaveToFile();
			}
		}
	}
}
