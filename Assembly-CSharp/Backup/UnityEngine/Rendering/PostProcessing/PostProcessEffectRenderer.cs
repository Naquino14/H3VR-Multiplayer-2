// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.PostProcessEffectRenderer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace UnityEngine.Rendering.PostProcessing
{
  public abstract class PostProcessEffectRenderer
  {
    protected bool m_ResetHistory = true;

    public virtual void Init()
    {
    }

    public virtual DepthTextureMode GetCameraFlags() => DepthTextureMode.None;

    public virtual void ResetHistory() => this.m_ResetHistory = true;

    public virtual void Release() => this.ResetHistory();

    public abstract void Render(PostProcessRenderContext context);

    internal abstract void SetSettings(PostProcessEffectSettings settings);
  }
}
