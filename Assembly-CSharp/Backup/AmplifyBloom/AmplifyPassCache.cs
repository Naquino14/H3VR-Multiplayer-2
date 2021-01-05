// Decompiled with JetBrains decompiler
// Type: AmplifyBloom.AmplifyPassCache
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace AmplifyBloom
{
  [Serializable]
  public class AmplifyPassCache
  {
    [SerializeField]
    internal Vector4[] Offsets;
    [SerializeField]
    internal Vector4[] Weights;

    public AmplifyPassCache()
    {
      this.Offsets = new Vector4[16];
      this.Weights = new Vector4[16];
    }

    public void Destroy()
    {
      this.Offsets = (Vector4[]) null;
      this.Weights = (Vector4[]) null;
    }
  }
}
