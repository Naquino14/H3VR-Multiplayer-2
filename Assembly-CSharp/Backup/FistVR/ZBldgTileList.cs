// Decompiled with JetBrains decompiler
// Type: FistVR.ZBldgTileList
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New zBldgTable", menuName = "Zosig/zBldgTable", order = 0)]
  public class ZBldgTileList : ScriptableObject
  {
    public List<ZBldgTileList.ZTileSet> Tilesets = new List<ZBldgTileList.ZTileSet>();

    [Serializable]
    public class ZTileSet
    {
      public List<UnityEngine.Object> Tiles = new List<UnityEngine.Object>();
    }
  }
}
