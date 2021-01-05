// Decompiled with JetBrains decompiler
// Type: FistVR.OptionsScreen_BetaWIP
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class OptionsScreen_BetaWIP : MonoBehaviour
  {
    public OptionsPanel_ButtonSet OBS_HitDecals;
    public OptionsPanel_ButtonSet OBS_HitSounds;
    public OptionsPanel_ButtonSet OBS_MaxHitDecals;
    public OptionsPanel_ButtonSet OBS_CoreControlMode;

    public void Awake() => this.InitScreen();

    public void InitScreen()
    {
      this.OBS_HitDecals.SetSelectedButton((int) GM.Options.SimulationOptions.HitDecalMode);
      this.OBS_HitSounds.SetSelectedButton((int) GM.Options.SimulationOptions.HitSoundMode);
      this.OBS_MaxHitDecals.SetSelectedButton(GM.Options.SimulationOptions.MaxHitDecalIndex);
      this.OBS_CoreControlMode.SetSelectedButton((int) GM.Options.ControlOptions.CCM);
    }

    public void SetHitDecalsMode(int i)
    {
      GM.Options.SimulationOptions.HitDecalMode = (SimulationOptions.HitDecals) i;
      GM.Options.SaveToFile();
    }

    public void SetHitSoundsMode(int i)
    {
      GM.Options.SimulationOptions.HitSoundMode = (SimulationOptions.HitSounds) i;
      GM.Options.SaveToFile();
    }

    public void SetMaxHitDecals(int i)
    {
      GM.Options.SimulationOptions.MaxHitDecalIndex = i;
      GM.Options.SaveToFile();
      FXM.ResetDecals();
    }

    public void SetCoreControlMode(int i)
    {
      GM.Options.ControlOptions.CCM = (ControlOptions.CoreControlMode) i;
      GM.Options.SaveToFile();
      if (GM.Options.ControlOptions.CCM != ControlOptions.CoreControlMode.Streamlined || GM.CurrentMovementManager.Mode != FVRMovementManager.MovementMode.JoystickTeleport && GM.CurrentMovementManager.Mode != FVRMovementManager.MovementMode.SlideToTarget && GM.CurrentMovementManager.Mode != FVRMovementManager.MovementMode.SingleTwoAxis)
        return;
      GM.CurrentMovementManager.Mode = FVRMovementManager.MovementMode.TwinStick;
      GM.CurrentMovementManager.CleanupFlagsForModeSwitch();
      GM.CurrentMovementManager.InitArmSwinger();
      GM.Options.MovementOptions.CurrentMovementMode = (FVRMovementManager.MovementMode) i;
    }
  }
}
