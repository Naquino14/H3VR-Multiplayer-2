﻿// Decompiled with JetBrains decompiler
// Type: Alloy.PackedMapDefinition
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace Alloy
{
  [CreateAssetMenu(fileName = "PackedMapDefinition.asset", menuName = "Alloy Packed Map Definition")]
  public class PackedMapDefinition : ScriptableObject
  {
    public string Title;
    public string Suffix;
    public bool VarianceBias = true;
    public TextureImportConfig ImportSettings = new TextureImportConfig();
    public List<MapTextureChannelMapping> Channels = new List<MapTextureChannelMapping>();
  }
}
