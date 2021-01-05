// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.IAmbientOcclusionMethod
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace UnityEngine.Rendering.PostProcessing
{
  public interface IAmbientOcclusionMethod
  {
    DepthTextureMode GetCameraFlags();

    void RenderAfterOpaque(PostProcessRenderContext context);

    void RenderAmbientOnly(PostProcessRenderContext context);

    void CompositeAmbientOnly(PostProcessRenderContext context);

    void Release();
  }
}
