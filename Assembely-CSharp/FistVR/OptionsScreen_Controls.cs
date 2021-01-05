using UnityEngine;

namespace FistVR
{
	public class OptionsScreen_Controls : MonoBehaviour
	{
		public OptionsPanel_ButtonSet OBS_UseAlternateHandgunMagPose;

		public OptionsPanel_ButtonSet OBS_GripButtonObjectDropStyle;

		public OptionsPanel_ButtonSet OBS_UseEasyMagLoading;

		public OptionsPanel_ButtonSet OBS_UseGunRigMode;

		public OptionsPanel_ButtonSet OBS_UseVirtualStock;

		public OptionsPanel_ButtonSet OBS_ForceGripButtonToGrab;

		public OptionsPanel_ButtonSet OBS_MeatFingers;

		public OptionsPanel_ButtonSet OBS_HapticsMode;

		public OptionsPanel_ButtonSet OBS_WIPGrabbityState;

		public OptionsPanel_ButtonSet OBS_WIPGrabbityButton;

		public OptionsPanel_ButtonSet OBS_WristMenuMode;

		public void Awake()
		{
			InitScreen();
		}

		public void InitScreen()
		{
			OBS_UseAlternateHandgunMagPose.SetSelectedButton(GM.Options.ControlOptions.UseInvertedHandgunMagPose);
			OBS_GripButtonObjectDropStyle.SetSelectedButton((int)GM.Options.ControlOptions.GripButtonDropStyle);
			OBS_UseEasyMagLoading.SetSelectedButton(GM.Options.ControlOptions.UseEasyMagLoading);
			OBS_UseGunRigMode.SetSelectedButton(GM.Options.ControlOptions.UseGunRigMode2);
			OBS_UseVirtualStock.SetSelectedButton(GM.Options.ControlOptions.UseVirtualStock);
			OBS_ForceGripButtonToGrab.SetSelectedButton((int)GM.Options.ControlOptions.GripButtonToHoldOverride);
			OBS_MeatFingers.SetSelectedButton((int)GM.Options.ControlOptions.MFMode);
			OBS_HapticsMode.SetSelectedButton((int)GM.Options.ControlOptions.HapticsState);
			OBS_WIPGrabbityState.SetSelectedButton((int)GM.Options.ControlOptions.WIPGrabbityState);
			OBS_WIPGrabbityButton.SetSelectedButton((int)GM.Options.ControlOptions.WIPGrabbityButtonState);
			OBS_WristMenuMode.SetSelectedButton((int)GM.Options.ControlOptions.WristMenuState);
		}

		public void SetUseAlternateHandgunMagPose(bool b)
		{
			GM.Options.ControlOptions.UseInvertedHandgunMagPose = b;
			GM.Options.SaveToFile();
		}

		public void SetGripButtonObjectDropStyle(int i)
		{
			GM.Options.ControlOptions.GripButtonDropStyle = (ControlOptions.ButtonControlStyle)i;
			GM.Options.SaveToFile();
		}

		public void SetUseEasyMagLoading(bool b)
		{
			GM.Options.ControlOptions.UseEasyMagLoading = b;
			GM.Options.SaveToFile();
		}

		public void SetUseGunRigMode(bool b)
		{
			GM.Options.ControlOptions.UseGunRigMode2 = b;
			GM.Options.SaveToFile();
		}

		public void SetUseVirtualStock(bool b)
		{
			GM.Options.ControlOptions.UseVirtualStock = b;
			GM.Options.SaveToFile();
		}

		public void SetUseForceGripButtonToGrab(int i)
		{
			GM.Options.ControlOptions.GripButtonToHoldOverride = (ControlOptions.GripButtonToHoldOverrideMode)i;
			GM.Options.SaveToFile();
		}

		public void SetHapticsMode(int i)
		{
			GM.Options.ControlOptions.HapticsState = (ControlOptions.HapticsMode)i;
			GM.Options.SaveToFile();
		}

		public void SetWIPGrabbityState(int i)
		{
			GM.Options.ControlOptions.WIPGrabbityState = (ControlOptions.WIPGrabbity)i;
			GM.Options.SaveToFile();
		}

		public void SetWIPGrabbityButtonState(int i)
		{
			GM.Options.ControlOptions.WIPGrabbityButtonState = (ControlOptions.WIPGrabbityButton)i;
			GM.Options.SaveToFile();
		}

		public void SetWristMenuMode(int i)
		{
			GM.Options.ControlOptions.WristMenuState = (ControlOptions.WristMenuMode)i;
			GM.Options.SaveToFile();
		}

		public void SetMeatFingerMode(int i)
		{
			GM.Options.ControlOptions.MFMode = (ControlOptions.MeatFingerMode)i;
			if (i == 1)
			{
				GM.CurrentMovementManager.Hands[0].SpawnSausageFingers();
				GM.CurrentMovementManager.Hands[1].SpawnSausageFingers();
			}
			GM.Options.SaveToFile();
		}
	}
}
