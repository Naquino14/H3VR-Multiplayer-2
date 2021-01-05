// Decompiled with JetBrains decompiler
// Type: FistVR.WWFlags
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace FistVR
{
  [Serializable]
  public class WWFlags
  {
    public int[] State_TargetPuzzles = new int[9];
    public int[] State_TargetPuzzleSafes = new int[9];
    public int[] State_Bandits = new int[10];
    public int State_NextBanditToSpawn;
    public int[] State_Horseshoes = new int[14];
    public int[] State_EventPuzzles = new int[4];
    public int[] State_Chests = new int[7];
    public int[] State_Keys = new int[7];
    public int[] State_EndDoors = new int[7];

    public void InitializeFromSaveFile(ES2Reader reader)
    {
      if (reader.TagExists("State_TargetPuzzles"))
        this.State_TargetPuzzles = reader.ReadArray<int>("State_TargetPuzzles");
      if (reader.TagExists("State_TargetPuzzleSafes"))
        this.State_TargetPuzzleSafes = reader.ReadArray<int>("State_TargetPuzzleSafes");
      if (reader.TagExists("State_Bandits"))
        this.State_Bandits = reader.ReadArray<int>("State_Bandits");
      if (reader.TagExists("State_NextBanditToSpawn"))
        this.State_NextBanditToSpawn = reader.Read<int>("State_NextBanditToSpawn");
      if (reader.TagExists("State_Horseshoes"))
        this.State_Horseshoes = reader.ReadArray<int>("State_Horseshoes");
      if (reader.TagExists("State_EventPuzzles"))
        this.State_EventPuzzles = reader.ReadArray<int>("State_EventPuzzles");
      if (reader.TagExists("State_Chests"))
        this.State_Chests = reader.ReadArray<int>("State_Chests");
      if (reader.TagExists("State_Keys"))
        this.State_Keys = reader.ReadArray<int>("State_Keys");
      if (!reader.TagExists("State_EndDoors"))
        return;
      this.State_EndDoors = reader.ReadArray<int>("State_EndDoors");
    }

    public void SaveToFile(ES2Writer writer)
    {
      writer.Write<int>(this.State_TargetPuzzles, "State_TargetPuzzles");
      writer.Write<int>(this.State_TargetPuzzleSafes, "State_TargetPuzzleSafes");
      writer.Write<int>(this.State_Bandits, "State_Bandits");
      writer.Write<int>(this.State_NextBanditToSpawn, "State_NextBanditToSpawn");
      writer.Write<int>(this.State_Horseshoes, "State_Horseshoes");
      writer.Write<int>(this.State_EventPuzzles, "State_EventPuzzles");
      writer.Write<int>(this.State_Chests, "State_Chests");
      writer.Write<int>(this.State_Keys, "State_Keys");
      writer.Write<int>(this.State_EndDoors, "State_EndDoors");
    }
  }
}
