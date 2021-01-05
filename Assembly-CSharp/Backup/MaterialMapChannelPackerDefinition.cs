// Decompiled with JetBrains decompiler
// Type: MaterialMapChannelPackerDefinition
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using Alloy;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MaterialMapChannelPackerDefinition : ScriptableObject
{
  public List<PackedMapDefinition> PackedMaps;
  [Header("Global settings")]
  public NormalMapChannelTextureChannelMapping NRMChannel = new NormalMapChannelTextureChannelMapping();
  [Space(15f)]
  public string VarianceText;
  public string AutoRegenerateText;

  public PackedMapDefinition PackedPack => this.PackedMaps[0];

  public PackedMapDefinition DetailPack => this.PackedMaps[1];

  public PackedMapDefinition TerrainPack => this.PackedMaps[2];

  public bool IsPackedMap(string path)
  {
    for (int index = 0; index < this.PackedMaps.Count; ++index)
    {
      PackedMapDefinition packedMap = this.PackedMaps[index];
      if (path.EndsWith(packedMap.Suffix, StringComparison.InvariantCultureIgnoreCase))
        return true;
    }
    return false;
  }
}
