using UnityEngine;

namespace FistVR
{
	public class OptionsScreen_TeleportAdvanced : MonoBehaviour
	{
		public OptionsPanel_ButtonSet OBS_AxialSetting;

		public OptionsPanel_ButtonSet OBS_Teleportmode;

		public void Awake()
		{
			InitScreen();
		}

		public void InitScreen()
		{
			OBS_AxialSetting.SetSelectedButton((int)GM.Options.MovementOptions.Teleport_AxialOrigin);
			OBS_Teleportmode.SetSelectedButton((int)GM.Options.MovementOptions.Teleport_Mode);
		}

		public void SetCurTeleport_AxialOrigin(int i)
		{
			GM.Options.MovementOptions.Teleport_AxialOrigin = (FVRMovementManager.MovementAxialOrigin)i;
			GM.Options.SaveToFile();
		}

		public void SetCurTeleportmode(int i)
		{
			GM.Options.MovementOptions.Teleport_Mode = (FVRMovementManager.TeleportMode)i;
			GM.Options.SaveToFile();
		}
	}
}
