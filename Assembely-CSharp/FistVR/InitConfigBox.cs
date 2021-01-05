using UnityEngine;

namespace FistVR
{
	public class InitConfigBox : MonoBehaviour
	{
		public OptionsPanel_ButtonSet OBS_Option;

		public GameObject BTN_Accept;

		public GameObject BTN_Confirm;

		public AudioEvent AudEvent_Beep;

		public AudioEvent AudEvent_Boop;

		private void Start()
		{
			if (GM.Options.ControlOptions.HasConfirmedControls)
			{
				base.gameObject.SetActive(value: false);
			}
			OBS_Option.SetSelectedButton((int)GM.Options.ControlOptions.CCM);
			BTN_Confirm.SetActive(value: false);
		}

		public void SetCCM(int i)
		{
			GM.Options.ControlOptions.CCM = (ControlOptions.CoreControlMode)i;
			GM.Options.SaveToFile();
			SM.PlayGenericSound(AudEvent_Beep, base.transform.position);
			BTN_Accept.SetActive(value: true);
			BTN_Confirm.SetActive(value: false);
		}

		public void ButtonPress_Accept()
		{
			SM.PlayGenericSound(AudEvent_Boop, base.transform.position);
			BTN_Accept.SetActive(value: false);
			BTN_Confirm.SetActive(value: true);
		}

		public void ButtonPress_Confirm()
		{
			SM.PlayGenericSound(AudEvent_Boop, base.transform.position);
			GM.Options.ControlOptions.HasConfirmedControls = true;
			base.gameObject.SetActive(value: false);
		}

		public void Update()
		{
			for (int i = 0; i < GM.CurrentMovementManager.Hands.Length; i++)
			{
				if (GM.CurrentMovementManager.Hands[i].HasInit && (GM.CurrentMovementManager.Hands[i].DMode == DisplayMode.Vive || GM.CurrentMovementManager.Hands[i].DMode == DisplayMode.WMR))
				{
					base.gameObject.SetActive(value: false);
				}
			}
		}
	}
}
