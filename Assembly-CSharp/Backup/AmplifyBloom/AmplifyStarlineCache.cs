// Decompiled with JetBrains decompiler
// Type: AmplifyBloom.AmplifyStarlineCache
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace AmplifyBloom
{
  [Serializable]
  public class AmplifyStarlineCache
  {
    [SerializeField]
    internal AmplifyPassCache[] Passes;

    public AmplifyStarlineCache()
    {
      this.Passes = new AmplifyPassCache[4];
      for (int index = 0; index < 4; ++index)
        this.Passes[index] = new AmplifyPassCache();
    }

    public void Destroy()
    {
      for (int index = 0; index < 4; ++index)
        this.Passes[index].Destroy();
      this.Passes = (AmplifyPassCache[]) null;
    }
  }
}
