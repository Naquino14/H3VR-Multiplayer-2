// Decompiled with JetBrains decompiler
// Type: FistVR.PerformanceOptions
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace FistVR
{
  [Serializable]
  public class PerformanceOptions
  {
    public PerformanceOptions.QualitySetting CurrentQualitySetting = PerformanceOptions.QualitySetting.High;
    public bool IsPostEnabled_AO;
    public bool IsPostEnabled_CC = true;
    public bool IsPostEnabled_Bloom;

    public void InitializeFromSaveFile(ES2Reader reader)
    {
      if (reader.TagExists("CurrentQualitySetting"))
        this.CurrentQualitySetting = reader.Read<PerformanceOptions.QualitySetting>("CurrentQualitySetting");
      if (reader.TagExists("IsPostEnabled_AO"))
        this.IsPostEnabled_AO = reader.Read<bool>("IsPostEnabled_AO");
      if (reader.TagExists("IsPostEnabled_CC"))
        this.IsPostEnabled_CC = reader.Read<bool>("IsPostEnabled_CC");
      if (!reader.TagExists("IsPostEnabled_Bloom"))
        return;
      this.IsPostEnabled_Bloom = reader.Read<bool>("IsPostEnabled_Bloom");
    }

    public void SaveToFile(ES2Writer writer)
    {
      writer.Write<PerformanceOptions.QualitySetting>(this.CurrentQualitySetting, "CurrentQualitySetting");
      writer.Write<bool>(this.IsPostEnabled_AO, "IsPostEnabled_AO");
      writer.Write<bool>(this.IsPostEnabled_CC, "IsPostEnabled_CC");
      writer.Write<bool>(this.IsPostEnabled_Bloom, "IsPostEnabled_Bloom");
    }

    public enum QualitySetting
    {
      Ultra,
      High,
      Medium,
      Low,
      Potato,
      TurboPotato,
    }
  }
}
