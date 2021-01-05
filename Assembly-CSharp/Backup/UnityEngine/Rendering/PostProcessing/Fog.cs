// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.Fog
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [Serializable]
  public sealed class Fog
  {
    [Tooltip("Enables the internal deferred fog pass. Actual fog settings should be set in the Lighting panel.")]
    public bool enabled = true;
    [Tooltip("Should the fog affect the skybox?")]
    public bool excludeSkybox = true;

    internal DepthTextureMode GetCameraFlags() => DepthTextureMode.Depth;

    internal bool IsEnabledAndSupported(PostProcessRenderContext context) => this.enabled && RenderSettings.fog && !RuntimeUtilities.scriptableRenderPipelineActive && context.camera.actualRenderingPath == RenderingPath.DeferredShading;

    internal void Render(PostProcessRenderContext context)
    {
      PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.deferredFog);
      propertySheet.ClearKeywords();
      Color color = !RuntimeUtilities.isLinearColorSpace ? RenderSettings.fogColor : RenderSettings.fogColor.linear;
      propertySheet.properties.SetVector(ShaderIDs.FogColor, (Vector4) color);
      propertySheet.properties.SetVector(ShaderIDs.FogParams, (Vector4) new Vector3(RenderSettings.fogDensity, RenderSettings.fogStartDistance, RenderSettings.fogEndDistance));
      context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, !this.excludeSkybox ? 0 : 1);
    }
  }
}
