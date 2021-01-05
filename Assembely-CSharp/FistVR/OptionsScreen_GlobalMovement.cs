using UnityEngine;

namespace FistVR
{
	public class OptionsScreen_GlobalMovement : MonoBehaviour
	{
		public OptionsPanel_ButtonSet OBS_AXButtonSnapturn;

		public OptionsPanel_ButtonSet OBS_SnapTurnMagnitude;

		public OptionsPanel_ButtonSet OBS_SmoothTurnMagnitude;

		public void Awake()
		{
			InitScreen();
		}

		public void InitScreen()
		{
			OBS_SnapTurnMagnitude.SetSelectedButton(GM.Options.MovementOptions.SnapTurnMagnitudeIndex);
			OBS_AXButtonSnapturn.SetSelectedButton((int)GM.Options.MovementOptions.AXButtonSnapTurnState);
			OBS_SmoothTurnMagnitude.SetSelectedButton(GM.Options.MovementOptions.SmoothTurnMagnitudeIndex);
		}

		public void SetCurSnapTurnMagnitude(int i)
		{
			GM.Options.MovementOptions.SnapTurnMagnitudeIndex = i;
			GM.Options.SaveToFile();
		}

		public void SetCurSmoothTurnMagnitude(int i)
		{
			GM.Options.MovementOptions.SmoothTurnMagnitudeIndex = i;
			GM.Options.SaveToFile();
		}

		public void SetAXButtonSnapTurn(int i)
		{
			GM.Options.MovementOptions.AXButtonSnapTurnState = (MovementOptions.AXButtonSnapTurnMode)i;
			GM.Options.SaveToFile();
		}
	}
}
