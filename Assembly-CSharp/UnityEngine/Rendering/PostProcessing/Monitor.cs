// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.Monitor
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace UnityEngine.Rendering.PostProcessing
{
  public abstract class Monitor
  {
    internal bool requested;

    public RenderTexture output { get; protected set; }

    public bool IsRequestedAndSupported() => this.requested && SystemInfo.supportsComputeShaders;

    internal virtual bool NeedsHalfRes() => false;

    protected void CheckOutput(int width, int height)
    {
      if (!((Object) this.output == (Object) null) && this.output.IsCreated() && (this.output.width == width && this.output.height == height))
        return;
      RuntimeUtilities.Destroy((Object) this.output);
      RenderTexture renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
      renderTexture.anisoLevel = 0;
      renderTexture.filterMode = FilterMode.Bilinear;
      renderTexture.wrapMode = TextureWrapMode.Clamp;
      renderTexture.useMipMap = false;
      this.output = renderTexture;
    }

    internal virtual void OnEnable()
    {
    }

    internal virtual void OnDisable() => RuntimeUtilities.Destroy((Object) this.output);

    internal abstract void Render(PostProcessRenderContext context);
  }
}
