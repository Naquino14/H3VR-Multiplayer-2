// Decompiled with JetBrains decompiler
// Type: FistVR.QuickbeltOptions
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace FistVR
{
  [Serializable]
  public class QuickbeltOptions
  {
    public int QuickbeltPreset;
    public int QuickbeltHandedness;
    public bool AreBulletTrailsEnabled;
    public bool HideControllerGeoWhenObjectHeld = true;
    public QuickbeltOptions.ObjectToHandConnectionMode ObjectToHandMode;
    public int TrailDecaySetting = 3;
    public float[] TrailDecayTimes = new float[5]
    {
      0.25f,
      0.5f,
      1f,
      5f,
      60f
    };
    public QuickbeltOptions.BoltActionMode BoltActionModeSetting;

    public void InitializeFromSaveFile(ES2Reader reader)
    {
      if (reader.TagExists("QuickbeltPreset"))
        this.QuickbeltPreset = reader.Read<int>("QuickbeltPreset");
      if (reader.TagExists("TrailDecaySetting"))
        this.TrailDecaySetting = reader.Read<int>("TrailDecaySetting");
      if (reader.TagExists("QuickbeltHandedness"))
        this.QuickbeltHandedness = reader.Read<int>("QuickbeltHandedness");
      if (reader.TagExists("AreBulletTrailsEnabled"))
        this.AreBulletTrailsEnabled = reader.Read<bool>("AreBulletTrailsEnabled");
      if (reader.TagExists("HideControllerGeoWhenObjectHeld"))
        this.HideControllerGeoWhenObjectHeld = reader.Read<bool>("HideControllerGeoWhenObjectHeld");
      if (reader.TagExists("ObjectToHandMode"))
        this.ObjectToHandMode = reader.Read<QuickbeltOptions.ObjectToHandConnectionMode>("ObjectToHandMode");
      if (!reader.TagExists("BoltActionModeSetting"))
        return;
      this.BoltActionModeSetting = reader.Read<QuickbeltOptions.BoltActionMode>("BoltActionModeSetting");
    }

    public void SaveToFile(ES2Writer writer)
    {
      writer.Write<int>(this.QuickbeltPreset, "QuickbeltPreset");
      writer.Write<int>(this.QuickbeltHandedness, "QuickbeltHandedness");
      writer.Write<int>(this.TrailDecaySetting, "TrailDecaySetting");
      writer.Write<bool>(this.AreBulletTrailsEnabled, "AreBulletTrailsEnabled");
      writer.Write<bool>(this.HideControllerGeoWhenObjectHeld, "HideControllerGeoWhenObjectHeld");
      writer.Write<QuickbeltOptions.ObjectToHandConnectionMode>(this.ObjectToHandMode, "ObjectToHandMode");
      writer.Write<QuickbeltOptions.BoltActionMode>(this.BoltActionModeSetting, "BoltActionModeSetting");
    }

    public enum ObjectToHandConnectionMode
    {
      Direct,
      Floating,
    }

    public enum BoltActionMode
    {
      Quickbolting,
      Slidebolting,
    }
  }
}
