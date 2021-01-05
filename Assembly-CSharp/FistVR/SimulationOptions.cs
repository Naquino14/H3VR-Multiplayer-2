// Decompiled with JetBrains decompiler
// Type: FistVR.SimulationOptions
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace FistVR
{
  [Serializable]
  public class SimulationOptions
  {
    public SimulationOptions.GravityMode ObjectGravityMode = SimulationOptions.GravityMode.Playful;
    public SimulationOptions.GravityMode PlayerGravityMode = SimulationOptions.GravityMode.Playful;
    public SimulationOptions.GravityMode BallisticGravityMode;
    public SimulationOptions.SpentShellDespawnTime ShellTime = SimulationOptions.SpentShellDespawnTime.Seconds_10;
    public bool SosigClownMode;
    public SimulationOptions.HitDecals HitDecalMode;
    public SimulationOptions.HitSounds HitSoundMode;
    public int MaxHitDecalIndex = 2;
    public int[] MaxHitDecals = new int[5]
    {
      5,
      25,
      100,
      250,
      1000
    };

    public void InitializeFromSaveFile(ES2Reader reader)
    {
      if (reader.TagExists("ObjectGravityMode"))
        this.ObjectGravityMode = reader.Read<SimulationOptions.GravityMode>("ObjectGravityMode");
      if (reader.TagExists("PlayerGravityMode"))
        this.PlayerGravityMode = reader.Read<SimulationOptions.GravityMode>("PlayerGravityMode");
      if (reader.TagExists("BallisticGravityMode"))
        this.BallisticGravityMode = reader.Read<SimulationOptions.GravityMode>("BallisticGravityMode");
      if (reader.TagExists("ShellTime"))
        this.ShellTime = reader.Read<SimulationOptions.SpentShellDespawnTime>("ShellTime");
      if (reader.TagExists("SosigClownMode"))
        this.SosigClownMode = reader.Read<bool>("SosigClownMode");
      if (reader.TagExists("HitDecalMode"))
        this.HitDecalMode = reader.Read<SimulationOptions.HitDecals>("HitDecalMode");
      if (reader.TagExists("HitSoundMode"))
        this.HitSoundMode = reader.Read<SimulationOptions.HitSounds>("HitSoundMode");
      if (reader.TagExists("MaxHitDecalIndex"))
        this.MaxHitDecalIndex = reader.Read<int>("MaxHitDecalIndex");
      ManagerSingleton<GM>.Instance.RefreshGravity();
    }

    public void SaveToFile(ES2Writer writer)
    {
      writer.Write<SimulationOptions.GravityMode>(this.ObjectGravityMode, "ObjectGravityMode");
      writer.Write<SimulationOptions.GravityMode>(this.PlayerGravityMode, "PlayerGravityMode");
      writer.Write<SimulationOptions.GravityMode>(this.BallisticGravityMode, "BallisticGravityMode");
      writer.Write<SimulationOptions.SpentShellDespawnTime>(this.ShellTime, "ShellTime");
      writer.Write<bool>(this.SosigClownMode, "SosigClownMode");
      writer.Write<SimulationOptions.HitDecals>(this.HitDecalMode, "HitDecalMode");
      writer.Write<SimulationOptions.HitSounds>(this.HitSoundMode, "HitSoundMode");
      writer.Write<int>(this.MaxHitDecalIndex, "MaxHitDecalIndex");
      ManagerSingleton<GM>.Instance.RefreshGravity();
    }

    public enum HitDecals
    {
      Disabled,
      Enabled,
    }

    public enum HitSounds
    {
      Disabled,
      Enabled,
    }

    public enum GravityMode
    {
      Realistic,
      Playful,
      OnTheMoon,
      None,
    }

    public enum SpentShellDespawnTime
    {
      Seconds_5,
      Seconds_10,
      Seconds_30,
      Infinite,
    }
  }
}
