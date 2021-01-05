// Decompiled with JetBrains decompiler
// Type: FistVR.OptionsScreen_Armswinger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class OptionsScreen_Armswinger : MonoBehaviour
  {
    public OptionsPanel_ButtonSet OBS_ArmSwingerInertialJump;
    public OptionsPanel_ButtonSet OBS_ArmSwingerSnapturn;
    public OptionsPanel_ButtonSet OBS_ArmSwingerBaseSpeed_Left;
    public OptionsPanel_ButtonSet OBS_ArmSwingerBaseSpeed_Right;

    public void Awake() => this.InitScreen();

    public void InitScreen()
    {
      this.OBS_ArmSwingerSnapturn.SetSelectedButton((int) GM.Options.MovementOptions.ArmSwingerSnapTurnState);
      this.OBS_ArmSwingerBaseSpeed_Left.SetSelectedButton(GM.Options.MovementOptions.ArmSwingerBaseSpeed_Left);
      this.OBS_ArmSwingerBaseSpeed_Right.SetSelectedButton(GM.Options.MovementOptions.ArmSwingerBaseSpeed_Right);
      this.OBS_ArmSwingerInertialJump.SetSelectedButton((int) GM.Options.MovementOptions.ArmSwingerJumpState);
    }

    public void SetArmSwingerSnapturn(int i)
    {
      GM.Options.MovementOptions.ArmSwingerSnapTurnState = (MovementOptions.ArmSwingerSnapTurnMode) i;
      GM.Options.SaveToFile();
    }

    public void SetArmSwingerBaseSpeedLeft(int i)
    {
      GM.Options.MovementOptions.ArmSwingerBaseSpeed_Left = i;
      GM.Options.SaveToFile();
    }

    public void SetArmSwingerBaseSpeedRight(int i)
    {
      GM.Options.MovementOptions.ArmSwingerBaseSpeed_Right = i;
      GM.Options.SaveToFile();
    }

    public void SetArmSwingerJump(int i)
    {
      GM.Options.MovementOptions.ArmSwingerJumpState = (MovementOptions.ArmSwingerJumpMode) i;
      GM.Options.SaveToFile();
    }
  }
}
