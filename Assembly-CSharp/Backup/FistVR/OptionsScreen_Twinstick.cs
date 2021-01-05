// Decompiled with JetBrains decompiler
// Type: FistVR.OptionsScreen_Twinstick
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class OptionsScreen_Twinstick : MonoBehaviour
  {
    public OptionsPanel_ButtonSet OBS_AxialOrientation;
    public OptionsPanel_ButtonSet OBS_MovementOccursWhen;
    public OptionsPanel_ButtonSet OBS_MoveSpeed;
    public OptionsPanel_ButtonSet OBS_TwinStickMode;
    public OptionsPanel_ButtonSet OBS_TwinStickSprintToggle;
    public OptionsPanel_ButtonSet OBS_TwinStickSnapTurn;
    public OptionsPanel_ButtonSet OBS_TwinStickJump;
    public OptionsPanel_ButtonSet OBS_TwinStickLeftRight;

    public void Awake() => this.InitScreen();

    public void InitScreen()
    {
      this.OBS_AxialOrientation.SetSelectedButton((int) GM.Options.MovementOptions.Touchpad_MovementMode);
      this.OBS_MovementOccursWhen.SetSelectedButton((int) GM.Options.MovementOptions.Touchpad_Confirm);
      this.OBS_MoveSpeed.SetSelectedButton(GM.Options.MovementOptions.TPLocoSpeedIndex);
      this.OBS_TwinStickMode.SetSelectedButton((int) GM.Options.MovementOptions.TwinStickSprintState);
      this.OBS_TwinStickSprintToggle.SetSelectedButton((int) GM.Options.MovementOptions.TwinStickSprintToggleState);
      this.OBS_TwinStickSnapTurn.SetSelectedButton((int) GM.Options.MovementOptions.TwinStickSnapturnState);
      this.OBS_TwinStickJump.SetSelectedButton((int) GM.Options.MovementOptions.TwinStickJumpState);
      this.OBS_TwinStickLeftRight.SetSelectedButton((int) GM.Options.MovementOptions.TwinStickLeftRightState);
    }

    public void SetTwinStickMode(int i)
    {
      GM.Options.MovementOptions.TwinStickSprintState = (MovementOptions.TwinStickSprintMode) i;
      GM.Options.SaveToFile();
    }

    public void SetTwinStickSprintToggle(int i)
    {
      GM.Options.MovementOptions.TwinStickSprintToggleState = (MovementOptions.TwinStickSprintToggleMode) i;
      GM.Options.SaveToFile();
    }

    public void SetTwinStickSnapTurn(int i)
    {
      GM.Options.MovementOptions.TwinStickSnapturnState = (MovementOptions.TwinStickSnapturnMode) i;
      GM.Options.SaveToFile();
    }

    public void SetTwinStickJump(int i)
    {
      GM.Options.MovementOptions.TwinStickJumpState = (MovementOptions.TwinStickJumpMode) i;
      GM.Options.SaveToFile();
    }

    public void SetTwinStickLeftRight(int i)
    {
      GM.Options.MovementOptions.TwinStickLeftRightState = (MovementOptions.TwinStickLeftRightSetup) i;
      GM.Options.SaveToFile();
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

    public void SetCurMoveSpeed(int i)
    {
      GM.Options.MovementOptions.TPLocoSpeedIndex = i;
      GM.Options.SaveToFile();
    }
  }
}
