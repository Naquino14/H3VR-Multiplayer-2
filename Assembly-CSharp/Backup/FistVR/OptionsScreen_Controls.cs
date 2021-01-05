// Decompiled with JetBrains decompiler
// Type: FistVR.OptionsScreen_Controls
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class OptionsScreen_Controls : MonoBehaviour
  {
    public OptionsPanel_ButtonSet OBS_UseAlternateHandgunMagPose;
    public OptionsPanel_ButtonSet OBS_GripButtonObjectDropStyle;
    public OptionsPanel_ButtonSet OBS_UseEasyMagLoading;
    public OptionsPanel_ButtonSet OBS_UseGunRigMode;
    public OptionsPanel_ButtonSet OBS_UseVirtualStock;
    public OptionsPanel_ButtonSet OBS_ForceGripButtonToGrab;
    public OptionsPanel_ButtonSet OBS_MeatFingers;
    public OptionsPanel_ButtonSet OBS_HapticsMode;
    public OptionsPanel_ButtonSet OBS_WIPGrabbityState;
    public OptionsPanel_ButtonSet OBS_WIPGrabbityButton;
    public OptionsPanel_ButtonSet OBS_WristMenuMode;

    public void Awake() => this.InitScreen();

    public void InitScreen()
    {
      this.OBS_UseAlternateHandgunMagPose.SetSelectedButton(GM.Options.ControlOptions.UseInvertedHandgunMagPose);
      this.OBS_GripButtonObjectDropStyle.SetSelectedButton((int) GM.Options.ControlOptions.GripButtonDropStyle);
      this.OBS_UseEasyMagLoading.SetSelectedButton(GM.Options.ControlOptions.UseEasyMagLoading);
      this.OBS_UseGunRigMode.SetSelectedButton(GM.Options.ControlOptions.UseGunRigMode2);
      this.OBS_UseVirtualStock.SetSelectedButton(GM.Options.ControlOptions.UseVirtualStock);
      this.OBS_ForceGripButtonToGrab.SetSelectedButton((int) GM.Options.ControlOptions.GripButtonToHoldOverride);
      this.OBS_MeatFingers.SetSelectedButton((int) GM.Options.ControlOptions.MFMode);
      this.OBS_HapticsMode.SetSelectedButton((int) GM.Options.ControlOptions.HapticsState);
      this.OBS_WIPGrabbityState.SetSelectedButton((int) GM.Options.ControlOptions.WIPGrabbityState);
      this.OBS_WIPGrabbityButton.SetSelectedButton((int) GM.Options.ControlOptions.WIPGrabbityButtonState);
      this.OBS_WristMenuMode.SetSelectedButton((int) GM.Options.ControlOptions.WristMenuState);
    }

    public void SetUseAlternateHandgunMagPose(bool b)
    {
      GM.Options.ControlOptions.UseInvertedHandgunMagPose = b;
      GM.Options.SaveToFile();
    }

    public void SetGripButtonObjectDropStyle(int i)
    {
      GM.Options.ControlOptions.GripButtonDropStyle = (ControlOptions.ButtonControlStyle) i;
      GM.Options.SaveToFile();
    }

    public void SetUseEasyMagLoading(bool b)
    {
      GM.Options.ControlOptions.UseEasyMagLoading = b;
      GM.Options.SaveToFile();
    }

    public void SetUseGunRigMode(bool b)
    {
      GM.Options.ControlOptions.UseGunRigMode2 = b;
      GM.Options.SaveToFile();
    }

    public void SetUseVirtualStock(bool b)
    {
      GM.Options.ControlOptions.UseVirtualStock = b;
      GM.Options.SaveToFile();
    }

    public void SetUseForceGripButtonToGrab(int i)
    {
      GM.Options.ControlOptions.GripButtonToHoldOverride = (ControlOptions.GripButtonToHoldOverrideMode) i;
      GM.Options.SaveToFile();
    }

    public void SetHapticsMode(int i)
    {
      GM.Options.ControlOptions.HapticsState = (ControlOptions.HapticsMode) i;
      GM.Options.SaveToFile();
    }

    public void SetWIPGrabbityState(int i)
    {
      GM.Options.ControlOptions.WIPGrabbityState = (ControlOptions.WIPGrabbity) i;
      GM.Options.SaveToFile();
    }

    public void SetWIPGrabbityButtonState(int i)
    {
      GM.Options.ControlOptions.WIPGrabbityButtonState = (ControlOptions.WIPGrabbityButton) i;
      GM.Options.SaveToFile();
    }

    public void SetWristMenuMode(int i)
    {
      GM.Options.ControlOptions.WristMenuState = (ControlOptions.WristMenuMode) i;
      GM.Options.SaveToFile();
    }

    public void SetMeatFingerMode(int i)
    {
      GM.Options.ControlOptions.MFMode = (ControlOptions.MeatFingerMode) i;
      if (i == 1)
      {
        GM.CurrentMovementManager.Hands[0].SpawnSausageFingers();
        GM.CurrentMovementManager.Hands[1].SpawnSausageFingers();
      }
      GM.Options.SaveToFile();
    }
  }
}
