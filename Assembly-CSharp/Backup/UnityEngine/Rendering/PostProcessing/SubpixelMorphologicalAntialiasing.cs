// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.SubpixelMorphologicalAntialiasing
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [Serializable]
  public sealed class SubpixelMorphologicalAntialiasing
  {
    public bool IsSupported() => !RuntimeUtilities.isSinglePassStereoEnabled;

    internal void Render(PostProcessRenderContext context)
    {
      PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.subpixelMorphologicalAntialiasing);
      propertySheet.properties.SetTexture("_AreaTex", (Texture) context.resources.smaaLuts.area);
      propertySheet.properties.SetTexture("_SearchTex", (Texture) context.resources.smaaLuts.search);
      CommandBuffer command = context.command;
      command.BeginSample(nameof (SubpixelMorphologicalAntialiasing));
      command.GetTemporaryRT(ShaderIDs.SMAA_Flip, context.width, context.height, 0, FilterMode.Bilinear, context.sourceFormat, RenderTextureReadWrite.Linear);
      command.GetTemporaryRT(ShaderIDs.SMAA_Flop, context.width, context.height, 0, FilterMode.Bilinear, context.sourceFormat, RenderTextureReadWrite.Linear);
      command.BlitFullscreenTriangle(context.source, (RenderTargetIdentifier) ShaderIDs.SMAA_Flip, propertySheet, 0, true);
      command.BlitFullscreenTriangle((RenderTargetIdentifier) ShaderIDs.SMAA_Flip, (RenderTargetIdentifier) ShaderIDs.SMAA_Flop, propertySheet, 1);
      command.SetGlobalTexture("_BlendTex", (RenderTargetIdentifier) ShaderIDs.SMAA_Flop);
      command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 2);
      command.ReleaseTemporaryRT(ShaderIDs.SMAA_Flip);
      command.ReleaseTemporaryRT(ShaderIDs.SMAA_Flop);
      command.EndSample(nameof (SubpixelMorphologicalAntialiasing));
    }

    private enum Pass
    {
      EdgeDetection,
      BlendWeights,
      NeighborhoodBlending,
    }
  }
}
