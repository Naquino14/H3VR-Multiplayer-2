// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.AmbientOcclusionRenderer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace UnityEngine.Rendering.PostProcessing
{
  public sealed class AmbientOcclusionRenderer : PostProcessEffectRenderer<AmbientOcclusion>
  {
    private IAmbientOcclusionMethod[] m_Methods;

    public override void Init()
    {
      if (this.m_Methods != null)
        return;
      this.m_Methods = new IAmbientOcclusionMethod[2]
      {
        (IAmbientOcclusionMethod) new ScalableAO(this.settings),
        (IAmbientOcclusionMethod) new MultiScaleVO(this.settings)
      };
    }

    public bool IsAmbientOnly(PostProcessRenderContext context)
    {
      Camera camera = context.camera;
      return this.settings.ambientOnly.value && camera.actualRenderingPath == RenderingPath.DeferredShading && camera.allowHDR;
    }

    public IAmbientOcclusionMethod Get() => this.m_Methods[(int) this.settings.mode.value];

    public override DepthTextureMode GetCameraFlags() => this.Get().GetCameraFlags();

    public override void Release()
    {
      foreach (IAmbientOcclusionMethod method in this.m_Methods)
        method.Release();
    }

    public override void Render(PostProcessRenderContext context)
    {
    }
  }
}
