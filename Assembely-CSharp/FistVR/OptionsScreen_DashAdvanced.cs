using UnityEngine;

namespace FistVR
{
	public class OptionsScreen_DashAdvanced : MonoBehaviour
	{
		public OptionsPanel_ButtonSet OBS_AxialSetting;

		public void Awake()
		{
			InitScreen();
		}

		public void InitScreen()
		{
			OBS_AxialSetting.SetSelectedButton((int)GM.Options.MovementOptions.Dash_AxialOrigin);
		}

		public void SetCurDash_AxialOrigin(int i)
		{
			GM.Options.MovementOptions.Dash_AxialOrigin = (FVRMovementManager.MovementAxialOrigin)i;
			GM.Options.SaveToFile();
		}
	}
}
