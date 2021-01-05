// Decompiled with JetBrains decompiler
// Type: FistVR.OptionsScreen_Quality
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class OptionsScreen_Quality : MonoBehaviour
  {
    public OptionsPanel_ButtonSet OBS_CurQualitySetting;
    public OptionsPanel_ButtonSet OBS_Post_CC;
    public OptionsPanel_ButtonSet OBS_Post_Bloom;
    public OptionsPanel_ButtonSet OBS_Post_AO;

    public void Awake() => this.InitScreen();

    public void InitScreen()
    {
      this.OBS_CurQualitySetting.SetSelectedButton((int) GM.Options.PerformanceOptions.CurrentQualitySetting);
      this.OBS_Post_CC.SetSelectedButton(!GM.Options.PerformanceOptions.IsPostEnabled_CC);
      this.OBS_Post_Bloom.SetSelectedButton(!GM.Options.PerformanceOptions.IsPostEnabled_Bloom);
      this.OBS_Post_AO.SetSelectedButton(!GM.Options.PerformanceOptions.IsPostEnabled_AO);
    }

    public void SetCurQualitySetting(int i)
    {
      GM.Options.PerformanceOptions.CurrentQualitySetting = (PerformanceOptions.QualitySetting) i;
      GM.Options.SaveToFile();
      GM.RefreshQuality();
    }

    public void SetCurPost_CC(bool b)
    {
      GM.Options.PerformanceOptions.IsPostEnabled_CC = b;
      GM.Options.SaveToFile();
      GM.RefreshQuality();
    }

    public void SetCurPost_Bloom(bool b)
    {
      GM.Options.PerformanceOptions.IsPostEnabled_Bloom = b;
      GM.Options.SaveToFile();
      GM.RefreshQuality();
    }

    public void SetCurPost_AO(bool b)
    {
      GM.Options.PerformanceOptions.IsPostEnabled_AO = b;
      GM.Options.SaveToFile();
      GM.RefreshQuality();
    }
  }
}
