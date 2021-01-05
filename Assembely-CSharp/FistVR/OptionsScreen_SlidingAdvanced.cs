using UnityEngine;

namespace FistVR
{
	public class OptionsScreen_SlidingAdvanced : MonoBehaviour
	{
		public OptionsPanel_ButtonSet OBS_AxialSetting;

		public OptionsPanel_ButtonSet OBS_Speed;

		public void Awake()
		{
			InitScreen();
		}

		public void InitScreen()
		{
			OBS_AxialSetting.SetSelectedButton((int)GM.Options.MovementOptions.Slide_AxialOrigin);
			OBS_Speed.SetSelectedButton(GM.Options.MovementOptions.SlidingSpeedTick);
		}

		public void SetCurSlide_AxialOrigin(int i)
		{
			GM.Options.MovementOptions.Slide_AxialOrigin = (FVRMovementManager.MovementAxialOrigin)i;
			GM.Options.SaveToFile();
		}

		public void SetCurSlidingSpeed(int i)
		{
			GM.Options.MovementOptions.SlidingSpeedTick = i;
			GM.Options.SaveToFile();
		}
	}
}
