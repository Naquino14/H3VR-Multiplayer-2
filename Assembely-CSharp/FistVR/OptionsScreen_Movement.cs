using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class OptionsScreen_Movement : MonoBehaviour
	{
		public OptionsPanel_ButtonSet OBS_MovementMode;

		public Text TXT_ModeName;

		public Text TXT_ModeDescrip;

		public List<string> ModeNames;

		[Multiline(9)]
		public List<string> Descrips;

		public void Awake()
		{
			InitScreen();
		}

		public void InitScreen()
		{
			OBS_MovementMode.SetSelectedButton((int)GM.Options.MovementOptions.CurrentMovementMode);
			TXT_ModeName.text = ModeNames[(int)GM.Options.MovementOptions.CurrentMovementMode];
			TXT_ModeDescrip.text = Descrips[(int)GM.Options.MovementOptions.CurrentMovementMode];
		}

		public void SetMovementMode(int i)
		{
			GM.CurrentMovementManager.Mode = (FVRMovementManager.MovementMode)i;
			GM.Options.MovementOptions.CurrentMovementMode = (FVRMovementManager.MovementMode)i;
			TXT_ModeName.text = ModeNames[(int)GM.Options.MovementOptions.CurrentMovementMode];
			TXT_ModeDescrip.text = Descrips[(int)GM.Options.MovementOptions.CurrentMovementMode];
			GM.Options.SaveToFile();
		}
	}
}
