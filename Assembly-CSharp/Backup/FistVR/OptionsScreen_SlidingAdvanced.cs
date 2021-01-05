// Decompiled with JetBrains decompiler
// Type: FistVR.OptionsScreen_SlidingAdvanced
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class OptionsScreen_SlidingAdvanced : MonoBehaviour
  {
    public OptionsPanel_ButtonSet OBS_AxialSetting;
    public OptionsPanel_ButtonSet OBS_Speed;

    public void Awake() => this.InitScreen();

    public void InitScreen()
    {
      this.OBS_AxialSetting.SetSelectedButton((int) GM.Options.MovementOptions.Slide_AxialOrigin);
      this.OBS_Speed.SetSelectedButton(GM.Options.MovementOptions.SlidingSpeedTick);
    }

    public void SetCurSlide_AxialOrigin(int i)
    {
      GM.Options.MovementOptions.Slide_AxialOrigin = (FVRMovementManager.MovementAxialOrigin) i;
      GM.Options.SaveToFile();
    }

    public void SetCurSlidingSpeed(int i)
    {
      GM.Options.MovementOptions.SlidingSpeedTick = i;
      GM.Options.SaveToFile();
    }
  }
}
