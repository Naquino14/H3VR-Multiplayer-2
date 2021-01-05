// Decompiled with JetBrains decompiler
// Type: AmplifyBloom.AmplifyBokehData
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace AmplifyBloom
{
  [Serializable]
  public class AmplifyBokehData
  {
    internal RenderTexture BokehRenderTexture;
    internal Vector4[] Offsets;

    public AmplifyBokehData(Vector4[] offsets) => this.Offsets = offsets;

    public void Destroy()
    {
      if ((UnityEngine.Object) this.BokehRenderTexture != (UnityEngine.Object) null)
      {
        AmplifyUtils.ReleaseTempRenderTarget(this.BokehRenderTexture);
        this.BokehRenderTexture = (RenderTexture) null;
      }
      this.Offsets = (Vector4[]) null;
    }
  }
}
