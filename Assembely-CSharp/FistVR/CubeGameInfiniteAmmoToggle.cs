using UnityEngine;

namespace FistVR
{
	public class CubeGameInfiniteAmmoToggle : MonoBehaviour
	{
		public OptionsPanel_ButtonSet OBS_Ammo;

		private void Start()
		{
			OBS_Ammo.SetSelectedButton(0);
		}

		public void SetAmmo(bool b)
		{
			if (b)
			{
				GM.CurrentSceneSettings.IsAmmoInfinite = true;
			}
			else
			{
				GM.CurrentSceneSettings.IsAmmoInfinite = false;
			}
		}
	}
}
