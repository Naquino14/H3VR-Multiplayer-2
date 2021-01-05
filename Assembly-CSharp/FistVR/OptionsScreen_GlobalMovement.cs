// Decompiled with JetBrains decompiler
// Type: FistVR.OptionsScreen_GlobalMovement
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class OptionsScreen_GlobalMovement : MonoBehaviour
  {
    public OptionsPanel_ButtonSet OBS_AXButtonSnapturn;
    public OptionsPanel_ButtonSet OBS_SnapTurnMagnitude;
    public OptionsPanel_ButtonSet OBS_SmoothTurnMagnitude;

    public void Awake() => this.InitScreen();

    public void InitScreen()
    {
      this.OBS_SnapTurnMagnitude.SetSelectedButton(GM.Options.MovementOptions.SnapTurnMagnitudeIndex);
      this.OBS_AXButtonSnapturn.SetSelectedButton((int) GM.Options.MovementOptions.AXButtonSnapTurnState);
      this.OBS_SmoothTurnMagnitude.SetSelectedButton(GM.Options.MovementOptions.SmoothTurnMagnitudeIndex);
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
      GM.Options.MovementOptions.AXButtonSnapTurnState = (MovementOptions.AXButtonSnapTurnMode) i;
      GM.Options.SaveToFile();
    }
  }
}
