// Decompiled with JetBrains decompiler
// Type: FistVR.OptionsScreen_Simulation
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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

    public void Awake() => this.InitScreen();

    public void InitScreen()
    {
      this.OBS_GravityObject.SetSelectedButton((int) GM.Options.SimulationOptions.ObjectGravityMode);
      this.OBS_GravityPlayer.SetSelectedButton((int) GM.Options.SimulationOptions.PlayerGravityMode);
      this.OBS_GravityBallistics.SetSelectedButton((int) GM.Options.SimulationOptions.BallisticGravityMode);
      this.OBS_ShellTime.SetSelectedButton((int) GM.Options.SimulationOptions.ShellTime);
      this.OBS_Clowns.SetSelectedButton(GM.Options.SimulationOptions.SosigClownMode);
    }

    public void SetGravityObject(int i)
    {
      GM.Options.SimulationOptions.ObjectGravityMode = (SimulationOptions.GravityMode) i;
      GM.Options.SaveToFile();
    }

    public void SetGravityPlayer(int i)
    {
      GM.Options.SimulationOptions.PlayerGravityMode = (SimulationOptions.GravityMode) i;
      GM.Options.SaveToFile();
    }

    public void SetGravityBallistics(int i)
    {
      GM.Options.SimulationOptions.BallisticGravityMode = (SimulationOptions.GravityMode) i;
      GM.Options.SaveToFile();
    }

    public void SetShellTime(int i)
    {
      GM.Options.SimulationOptions.ShellTime = (SimulationOptions.SpentShellDespawnTime) i;
      GM.Options.SaveToFile();
    }

    public void SetClowns(bool b)
    {
      GM.Options.SimulationOptions.SosigClownMode = b;
      GM.Options.SaveToFile();
    }
  }
}
