// Decompiled with JetBrains decompiler
// Type: AmplifyBloom.AmplifyGlareCache
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace AmplifyBloom
{
  [Serializable]
  public class AmplifyGlareCache
  {
    [SerializeField]
    internal AmplifyStarlineCache[] Starlines;
    [SerializeField]
    internal Vector4 AverageWeight;
    [SerializeField]
    internal Vector4[,] CromaticAberrationMat;
    [SerializeField]
    internal int TotalRT;
    [SerializeField]
    internal GlareDefData GlareDef;
    [SerializeField]
    internal StarDefData StarDef;
    [SerializeField]
    internal int CurrentPassCount;

    public AmplifyGlareCache()
    {
      this.Starlines = new AmplifyStarlineCache[4];
      this.CromaticAberrationMat = new Vector4[4, 8];
      for (int index = 0; index < 4; ++index)
        this.Starlines[index] = new AmplifyStarlineCache();
    }

    public void Destroy()
    {
      for (int index = 0; index < 4; ++index)
        this.Starlines[index].Destroy();
      this.Starlines = (AmplifyStarlineCache[]) null;
      this.CromaticAberrationMat = (Vector4[,]) null;
    }
  }
}
