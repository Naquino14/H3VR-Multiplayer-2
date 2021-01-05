using UnityEngine;

namespace FistVR
{
	public class OptionsScreen_GUns : MonoBehaviour
	{
		public OptionsPanel_ButtonSet OBS_AreBulletTrailsEnabled;

		public OptionsPanel_ButtonSet OBS_HideControllerGeoWhenObjectHeld;

		public OptionsPanel_ButtonSet OBS_BulletTrailDecayTime;

		public OptionsPanel_ButtonSet OBS_ObjectToHandMode;

		public OptionsPanel_ButtonSet OBS_BoltActionMode;

		public void Awake()
		{
			InitScreen();
		}

		public void InitScreen()
		{
			OBS_AreBulletTrailsEnabled.SetSelectedButton(GM.Options.QuickbeltOptions.AreBulletTrailsEnabled);
			OBS_HideControllerGeoWhenObjectHeld.SetSelectedButton(GM.Options.QuickbeltOptions.HideControllerGeoWhenObjectHeld);
			OBS_BulletTrailDecayTime.SetSelectedButton(GM.Options.QuickbeltOptions.TrailDecaySetting);
			OBS_ObjectToHandMode.SetSelectedButton((int)GM.Options.QuickbeltOptions.ObjectToHandMode);
			OBS_BoltActionMode.SetSelectedButton((int)GM.Options.QuickbeltOptions.BoltActionModeSetting);
		}

		public void SetAreBulletTrailsEnabled(bool b)
		{
			GM.Options.QuickbeltOptions.AreBulletTrailsEnabled = b;
			GM.Options.SaveToFile();
		}

		public void SetQuickbeltMagDefaultLoad(bool b)
		{
		}

		public void SetHideControllerGeoWhenObjectHeld(bool b)
		{
			GM.Options.QuickbeltOptions.HideControllerGeoWhenObjectHeld = b;
			GM.Options.SaveToFile();
		}

		public void SetTrailDecayTime(int i)
		{
			GM.Options.QuickbeltOptions.TrailDecaySetting = i;
			GM.Options.SaveToFile();
		}

		public void SetObjectToHandMode(int i)
		{
			GM.Options.QuickbeltOptions.ObjectToHandMode = (QuickbeltOptions.ObjectToHandConnectionMode)i;
			GM.Options.SaveToFile();
		}

		public void SetBoltActionMode(int i)
		{
			GM.Options.QuickbeltOptions.BoltActionModeSetting = (QuickbeltOptions.BoltActionMode)i;
			GM.Options.SaveToFile();
		}
	}
}
