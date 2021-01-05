// Decompiled with JetBrains decompiler
// Type: FistVR.TAH_LootTableEntry
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  [Serializable]
  public class TAH_LootTableEntry
  {
    public FVRObject MainObj;
    public FVRObject SecondaryObj;
    public Vector2 Nums;
    public int AttachmentSpawn;
    public bool UsesLargeCase = true;
  }
}
