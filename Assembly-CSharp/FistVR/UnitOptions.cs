// Decompiled with JetBrains decompiler
// Type: FistVR.UnitOptions
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace FistVR
{
  [Serializable]
  public class UnitOptions
  {
    public UnitOptions.UnitType Units;
    public float FloorHeightOffset;

    public void InitializeFromSaveFile(ES2Reader reader)
    {
      if (reader.TagExists("Units"))
        this.Units = reader.Read<UnitOptions.UnitType>("Units");
      if (!reader.TagExists("FloorHeightOffset"))
        return;
      this.FloorHeightOffset = reader.Read<float>("FloorHeightOffset");
    }

    public void SaveToFile(ES2Writer writer)
    {
      writer.Write<UnitOptions.UnitType>(this.Units, "Units");
      writer.Write<float>(this.FloorHeightOffset, "FloorHeightOffset");
    }

    public enum UnitType
    {
      Imperial,
      Metric,
      Zorgborgs,
    }
  }
}
