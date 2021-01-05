// Decompiled with JetBrains decompiler
// Type: FistVR.OptionsScreen_Touchpad
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class OptionsScreen_Touchpad : MonoBehaviour
  {
    public OptionsPanel_ButtonSet OBS_AxialOrientation;
    public OptionsPanel_ButtonSet OBS_MovementOccursWhen;
    public OptionsPanel_ButtonSet OBS_TouchpadStyle;
    public OptionsPanel_ButtonSet OBS_MoveSpeed;

    public void Awake() => this.InitScreen();

    public void InitScreen()
    {
      this.OBS_AxialOrientation.SetSelectedButton((int) GM.Options.MovementOptions.Touchpad_MovementMode);
      this.OBS_MovementOccursWhen.SetSelectedButton((int) GM.Options.MovementOptions.Touchpad_Confirm);
      this.OBS_TouchpadStyle.SetSelectedButton((int) GM.Options.MovementOptions.Touchpad_Style);
      this.OBS_MoveSpeed.SetSelectedButton(GM.Options.MovementOptions.TPLocoSpeedIndex);
    }

    public void SetCurAxialOrientation(int i)
    {
      GM.Options.MovementOptions.Touchpad_MovementMode = (FVRMovementManager.TwoAxisMovementMode) i;
      GM.Options.SaveToFile();
    }

    public void SetCurMovementOccursWhen(int i)
    {
      GM.Options.MovementOptions.Touchpad_Confirm = (FVRMovementManager.TwoAxisMovementConfirm) i;
      GM.Options.SaveToFile();
    }

    public void SetCurTouchpadStyle(int i)
    {
      GM.Options.MovementOptions.Touchpad_Style = (FVRMovementManager.TwoAxisMovementStyle) i;
      GM.Options.SaveToFile();
    }

    public void SetCurMoveSpeed(int i)
    {
      GM.Options.MovementOptions.TPLocoSpeedIndex = i;
      GM.Options.SaveToFile();
    }
  }
}
