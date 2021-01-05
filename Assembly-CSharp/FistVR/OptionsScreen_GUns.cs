// Decompiled with JetBrains decompiler
// Type: FistVR.OptionsScreen_GUns
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class OptionsScreen_GUns : MonoBehaviour
  {
    public OptionsPanel_ButtonSet OBS_AreBulletTrailsEnabled;
    public OptionsPanel_ButtonSet OBS_HideControllerGeoWhenObjectHeld;
    public OptionsPanel_ButtonSet OBS_BulletTrailDecayTime;
    public OptionsPanel_ButtonSet OBS_ObjectToHandMode;
    public OptionsPanel_ButtonSet OBS_BoltActionMode;

    public void Awake() => this.InitScreen();

    public void InitScreen()
    {
      this.OBS_AreBulletTrailsEnabled.SetSelectedButton(GM.Options.QuickbeltOptions.AreBulletTrailsEnabled);
      this.OBS_HideControllerGeoWhenObjectHeld.SetSelectedButton(GM.Options.QuickbeltOptions.HideControllerGeoWhenObjectHeld);
      this.OBS_BulletTrailDecayTime.SetSelectedButton(GM.Options.QuickbeltOptions.TrailDecaySetting);
      this.OBS_ObjectToHandMode.SetSelectedButton((int) GM.Options.QuickbeltOptions.ObjectToHandMode);
      this.OBS_BoltActionMode.SetSelectedButton((int) GM.Options.QuickbeltOptions.BoltActionModeSetting);
    }

    public void SetAreBulletTrailsEnabled(bool b)
    {
      GM.Options.QuickbeltOptions.AreBulletTrailsEnabled = b;
      GM.Options.SaveToFile();
    }

    public void SetQuickbeltMagDefaultLoad(bool b)
    {
    }

    public void SetHideControllerGeoWhenObjectHeld(bool b)
    {
      GM.Options.QuickbeltOptions.HideControllerGeoWhenObjectHeld = b;
      GM.Options.SaveToFile();
    }

    public void SetTrailDecayTime(int i)
    {
      GM.Options.QuickbeltOptions.TrailDecaySetting = i;
      GM.Options.SaveToFile();
    }

    public void SetObjectToHandMode(int i)
    {
      GM.Options.QuickbeltOptions.ObjectToHandMode = (QuickbeltOptions.ObjectToHandConnectionMode) i;
      GM.Options.SaveToFile();
    }

    public void SetBoltActionMode(int i)
    {
      GM.Options.QuickbeltOptions.BoltActionModeSetting = (QuickbeltOptions.BoltActionMode) i;
      GM.Options.SaveToFile();
    }
  }
}
