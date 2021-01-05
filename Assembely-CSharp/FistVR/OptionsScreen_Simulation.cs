using UnityEngine;

namespace FistVR
{
	public class OptionsScreen_Simulation : MonoBehaviour
	{
		public OptionsPanel_ButtonSet OBS_GravityObject;

		public OptionsPanel_ButtonSet OBS_GravityPlayer;

		public OptionsPanel_ButtonSet OBS_GravityBallistics;

		public OptionsPanel_ButtonSet OBS_ShellTime;

		public OptionsPanel_ButtonSet OBS_Clowns;

		public void Awake()
		{
			InitScreen();
		}

		public void InitScreen()
		{
			OBS_GravityObject.SetSelectedButton((int)GM.Options.SimulationOptions.ObjectGravityMode);
			OBS_GravityPlayer.SetSelectedButton((int)GM.Options.SimulationOptions.PlayerGravityMode);
			OBS_GravityBallistics.SetSelectedButton((int)GM.Options.SimulationOptions.BallisticGravityMode);
			OBS_ShellTime.SetSelectedButton((int)GM.Options.SimulationOptions.ShellTime);
			OBS_Clowns.SetSelectedButton(GM.Options.SimulationOptions.SosigClownMode);
		}

		public void SetGravityObject(int i)
		{
			GM.Options.SimulationOptions.ObjectGravityMode = (SimulationOptions.GravityMode)i;
			GM.Options.SaveToFile();
		}

		public void SetGravityPlayer(int i)
		{
			GM.Options.SimulationOptions.PlayerGravityMode = (SimulationOptions.GravityMode)i;
			GM.Options.SaveToFile();
		}

		public void SetGravityBallistics(int i)
		{
			GM.Options.SimulationOptions.BallisticGravityMode = (SimulationOptions.GravityMode)i;
			GM.Options.SaveToFile();
		}

		public void SetShellTime(int i)
		{
			GM.Options.SimulationOptions.ShellTime = (SimulationOptions.SpentShellDespawnTime)i;
			GM.Options.SaveToFile();
		}

		public void SetClowns(bool b)
		{
			GM.Options.SimulationOptions.SosigClownMode = b;
			GM.Options.SaveToFile();
		}
	}
}
