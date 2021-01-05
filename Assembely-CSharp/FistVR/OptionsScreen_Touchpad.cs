using UnityEngine;

namespace FistVR
{
	public class OptionsScreen_Touchpad : MonoBehaviour
	{
		public OptionsPanel_ButtonSet OBS_AxialOrientation;

		public OptionsPanel_ButtonSet OBS_MovementOccursWhen;

		public OptionsPanel_ButtonSet OBS_TouchpadStyle;

		public OptionsPanel_ButtonSet OBS_MoveSpeed;

		public void Awake()
		{
			InitScreen();
		}

		public void InitScreen()
		{
			OBS_AxialOrientation.SetSelectedButton((int)GM.Options.MovementOptions.Touchpad_MovementMode);
			OBS_MovementOccursWhen.SetSelectedButton((int)GM.Options.MovementOptions.Touchpad_Confirm);
			OBS_TouchpadStyle.SetSelectedButton((int)GM.Options.MovementOptions.Touchpad_Style);
			OBS_MoveSpeed.SetSelectedButton(GM.Options.MovementOptions.TPLocoSpeedIndex);
		}

		public void SetCurAxialOrientation(int i)
		{
			GM.Options.MovementOptions.Touchpad_MovementMode = (FVRMovementManager.TwoAxisMovementMode)i;
			GM.Options.SaveToFile();
		}

		public void SetCurMovementOccursWhen(int i)
		{
			GM.Options.MovementOptions.Touchpad_Confirm = (FVRMovementManager.TwoAxisMovementConfirm)i;
			GM.Options.SaveToFile();
		}

		public void SetCurTouchpadStyle(int i)
		{
			GM.Options.MovementOptions.Touchpad_Style = (FVRMovementManager.TwoAxisMovementStyle)i;
			GM.Options.SaveToFile();
		}

		public void SetCurMoveSpeed(int i)
		{
			GM.Options.MovementOptions.TPLocoSpeedIndex = i;
			GM.Options.SaveToFile();
		}
	}
}
