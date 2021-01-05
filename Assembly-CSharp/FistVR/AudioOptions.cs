// Decompiled with JetBrains decompiler
// Type: FistVR.AudioOptions
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace FistVR
{
  [Serializable]
  public class AudioOptions
  {
    public float MasterVolume = 1f;
    public float FXVolume = 1f;
    public float AmbientVolume = 1f;
    public float MusicVolume = 1f;

    public void InitializeFromSaveFile(ES2Reader reader)
    {
      if (reader.TagExists("MasterVolume"))
        this.MasterVolume = reader.Read<float>("MasterVolume");
      if (reader.TagExists("FXVolume"))
        this.FXVolume = reader.Read<float>("FXVolume");
      if (reader.TagExists("AmbientVolume"))
        this.AmbientVolume = reader.Read<float>("AmbientVolume");
      if (!reader.TagExists("MusicVolume"))
        return;
      this.MusicVolume = reader.Read<float>("MusicVolume");
    }

    public void SaveToFile(ES2Writer writer)
    {
      writer.Write<float>(this.MasterVolume, "MasterVolume");
      writer.Write<float>(this.FXVolume, "FXVolume");
      writer.Write<float>(this.AmbientVolume, "AmbientVolume");
      writer.Write<float>(this.MusicVolume, "MusicVolume");
    }
  }
}
