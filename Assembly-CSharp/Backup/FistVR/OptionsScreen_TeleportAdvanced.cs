// Decompiled with JetBrains decompiler
// Type: FistVR.OptionsScreen_TeleportAdvanced
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class OptionsScreen_TeleportAdvanced : MonoBehaviour
  {
    public OptionsPanel_ButtonSet OBS_AxialSetting;
    public OptionsPanel_ButtonSet OBS_Teleportmode;

    public void Awake() => this.InitScreen();

    public void InitScreen()
    {
      this.OBS_AxialSetting.SetSelectedButton((int) GM.Options.MovementOptions.Teleport_AxialOrigin);
      this.OBS_Teleportmode.SetSelectedButton((int) GM.Options.MovementOptions.Teleport_Mode);
    }

    public void SetCurTeleport_AxialOrigin(int i)
    {
      GM.Options.MovementOptions.Teleport_AxialOrigin = (FVRMovementManager.MovementAxialOrigin) i;
      GM.Options.SaveToFile();
    }

    public void SetCurTeleportmode(int i)
    {
      GM.Options.MovementOptions.Teleport_Mode = (FVRMovementManager.TeleportMode) i;
      GM.Options.SaveToFile();
    }
  }
}
