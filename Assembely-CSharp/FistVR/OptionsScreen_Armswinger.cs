using UnityEngine;

namespace FistVR
{
	public class OptionsScreen_Armswinger : MonoBehaviour
	{
		public OptionsPanel_ButtonSet OBS_ArmSwingerInertialJump;

		public OptionsPanel_ButtonSet OBS_ArmSwingerSnapturn;

		public OptionsPanel_ButtonSet OBS_ArmSwingerBaseSpeed_Left;

		public OptionsPanel_ButtonSet OBS_ArmSwingerBaseSpeed_Right;

		public void Awake()
		{
			InitScreen();
		}

		public void InitScreen()
		{
			OBS_ArmSwingerSnapturn.SetSelectedButton((int)GM.Options.MovementOptions.ArmSwingerSnapTurnState);
			OBS_ArmSwingerBaseSpeed_Left.SetSelectedButton(GM.Options.MovementOptions.ArmSwingerBaseSpeed_Left);
			OBS_ArmSwingerBaseSpeed_Right.SetSelectedButton(GM.Options.MovementOptions.ArmSwingerBaseSpeed_Right);
			OBS_ArmSwingerInertialJump.SetSelectedButton((int)GM.Options.MovementOptions.ArmSwingerJumpState);
		}

		public void SetArmSwingerSnapturn(int i)
		{
			GM.Options.MovementOptions.ArmSwingerSnapTurnState = (MovementOptions.ArmSwingerSnapTurnMode)i;
			GM.Options.SaveToFile();
		}

		public void SetArmSwingerBaseSpeedLeft(int i)
		{
			GM.Options.MovementOptions.ArmSwingerBaseSpeed_Left = i;
			GM.Options.SaveToFile();
		}

		public void SetArmSwingerBaseSpeedRight(int i)
		{
			GM.Options.MovementOptions.ArmSwingerBaseSpeed_Right = i;
			GM.Options.SaveToFile();
		}

		public void SetArmSwingerJump(int i)
		{
			GM.Options.MovementOptions.ArmSwingerJumpState = (MovementOptions.ArmSwingerJumpMode)i;
			GM.Options.SaveToFile();
		}
	}
}
