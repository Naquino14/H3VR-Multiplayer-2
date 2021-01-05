// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.Dithering
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [Serializable]
  public sealed class Dithering
  {
    private int m_NoiseTextureIndex;

    internal void Render(PostProcessRenderContext context)
    {
      Texture2D[] blueNoise64 = context.resources.blueNoise64;
      if (++this.m_NoiseTextureIndex >= blueNoise64.Length)
        this.m_NoiseTextureIndex = 0;
      float z = UnityEngine.Random.value;
      float w = UnityEngine.Random.value;
      Texture2D texture2D = blueNoise64[this.m_NoiseTextureIndex];
      PropertySheet uberSheet = context.uberSheet;
      uberSheet.properties.SetTexture(ShaderIDs.DitheringTex, (Texture) texture2D);
      uberSheet.properties.SetVector(ShaderIDs.Dithering_Coords, new Vector4((float) context.width / (float) texture2D.width, (float) context.height / (float) texture2D.height, z, w));
    }
  }
}
