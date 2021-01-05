// Decompiled with JetBrains decompiler
// Type: FistVR.OptionsScreen_Quickbelt
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class OptionsScreen_Quickbelt : MonoBehaviour
  {
    public OptionsPanel_ButtonSet OBS_Handedness;
    public OptionsPanel_ButtonSet OBS_SlotStyle;

    public void Awake() => this.InitScreen();

    public void InitScreen()
    {
      this.OBS_Handedness.SetSelectedButton(GM.Options.QuickbeltOptions.QuickbeltHandedness);
      this.OBS_SlotStyle.SetSelectedButton(GM.Options.QuickbeltOptions.QuickbeltPreset);
    }

    public void SetHandedness(int i)
    {
      if (!GM.CurrentSceneSettings.IsQuickbeltSwappingAllowed)
        return;
      GM.Options.QuickbeltOptions.QuickbeltHandedness = i;
      GM.CurrentPlayerBody.ConfigureQuickbelt(GM.Options.QuickbeltOptions.QuickbeltPreset);
      GM.Options.SaveToFile();
    }

    public void SetSlotStyle(int i)
    {
      if (!GM.CurrentSceneSettings.IsQuickbeltSwappingAllowed)
        return;
      GM.Options.QuickbeltOptions.QuickbeltPreset = i;
      GM.CurrentPlayerBody.ConfigureQuickbelt(i);
      GM.Options.SaveToFile();
    }
  }
}
