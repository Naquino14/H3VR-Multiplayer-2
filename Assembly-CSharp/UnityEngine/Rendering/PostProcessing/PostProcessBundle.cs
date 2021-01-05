// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.PostProcessBundle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  public sealed class PostProcessBundle
  {
    private PostProcessEffectRenderer m_Renderer;

    internal PostProcessBundle(PostProcessEffectSettings settings)
    {
      this.settings = settings;
      this.attribute = settings.GetType().GetAttribute<PostProcessAttribute>();
    }

    public PostProcessAttribute attribute { get; private set; }

    public PostProcessEffectSettings settings { get; private set; }

    internal PostProcessEffectRenderer renderer
    {
      get
      {
        if (this.m_Renderer == null)
        {
          this.m_Renderer = (PostProcessEffectRenderer) Activator.CreateInstance(this.attribute.renderer);
          this.m_Renderer.SetSettings(this.settings);
          this.m_Renderer.Init();
        }
        return this.m_Renderer;
      }
    }

    internal void Release()
    {
      if (this.m_Renderer != null)
        this.m_Renderer.Release();
      RuntimeUtilities.Destroy((UnityEngine.Object) this.settings);
    }

    internal void ResetHistory()
    {
      if (this.m_Renderer == null)
        return;
      this.m_Renderer.ResetHistory();
    }

    internal T CastSettings<T>() where T : PostProcessEffectSettings => (T) this.settings;

    internal T CastRenderer<T>() where T : PostProcessEffectRenderer => (T) this.renderer;
  }
}
