// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.TextureLerper
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
  internal class TextureLerper
  {
    private static TextureLerper m_Instance;
    private CommandBuffer m_Command;
    private PropertySheetFactory m_PropertySheets;
    private PostProcessResources m_Resources;
    private List<RenderTexture> m_Recycled;
    private List<RenderTexture> m_Actives;

    private TextureLerper()
    {
      this.m_Recycled = new List<RenderTexture>();
      this.m_Actives = new List<RenderTexture>();
    }

    internal static TextureLerper instance
    {
      get
      {
        if (TextureLerper.m_Instance == null)
          TextureLerper.m_Instance = new TextureLerper();
        return TextureLerper.m_Instance;
      }
    }

    internal void BeginFrame(PostProcessRenderContext context)
    {
      this.m_Command = context.command;
      this.m_PropertySheets = context.propertySheets;
      this.m_Resources = context.resources;
    }

    internal void EndFrame()
    {
      if (this.m_Recycled.Count > 0)
      {
        foreach (Object @object in this.m_Recycled)
          RuntimeUtilities.Destroy(@object);
        this.m_Recycled.Clear();
      }
      if (this.m_Actives.Count <= 0)
        return;
      foreach (RenderTexture active in this.m_Actives)
        this.m_Recycled.Add(active);
      this.m_Actives.Clear();
    }

    private RenderTexture Get(
      RenderTextureFormat format,
      int w,
      int h,
      int d = 1,
      bool enableRandomWrite = false)
    {
      RenderTexture renderTexture1 = (RenderTexture) null;
      int count = this.m_Recycled.Count;
      int index;
      for (index = 0; index < count; ++index)
      {
        RenderTexture renderTexture2 = this.m_Recycled[index];
        if (renderTexture2.width == w && renderTexture2.height == h && (renderTexture2.volumeDepth == d && renderTexture2.format == format) && renderTexture2.enableRandomWrite == enableRandomWrite)
        {
          renderTexture1 = renderTexture2;
          break;
        }
      }
      if ((Object) renderTexture1 == (Object) null)
      {
        TextureDimension textureDimension = d <= 1 ? TextureDimension.Tex2D : TextureDimension.Tex3D;
        RenderTexture renderTexture2 = new RenderTexture(w, h, d, format);
        renderTexture2.filterMode = FilterMode.Bilinear;
        renderTexture2.wrapMode = TextureWrapMode.Clamp;
        renderTexture2.anisoLevel = 0;
        renderTexture2.volumeDepth = d;
        renderTexture2.enableRandomWrite = enableRandomWrite;
        renderTexture2.dimension = textureDimension;
        renderTexture1 = renderTexture2;
        renderTexture1.Create();
      }
      else
        this.m_Recycled.RemoveAt(index);
      this.m_Actives.Add(renderTexture1);
      return renderTexture1;
    }

    internal Texture Lerp(Texture from, Texture to, float t)
    {
      int num1;
      switch (to)
      {
        case Texture3D _:
          num1 = 1;
          break;
        case RenderTexture _:
          num1 = ((RenderTexture) to).volumeDepth > 1 ? 1 : 0;
          break;
        default:
          num1 = 0;
          break;
      }
      RenderTexture renderTexture;
      if (num1 != 0)
      {
        int width = to.width;
        renderTexture = this.Get(RenderTextureFormat.ARGBHalf, width, width, width, true);
        ComputeShader texture3dLerp = this.m_Resources.computeShaders.texture3dLerp;
        int kernel = texture3dLerp.FindKernel("KTexture3DLerp");
        this.m_Command.SetComputeVectorParam(texture3dLerp, "_Params", new Vector4(t, (float) width, 0.0f, 0.0f));
        this.m_Command.SetComputeTextureParam(texture3dLerp, kernel, "_Output", (RenderTargetIdentifier) (Texture) renderTexture);
        this.m_Command.SetComputeTextureParam(texture3dLerp, kernel, "_From", (RenderTargetIdentifier) from);
        this.m_Command.SetComputeTextureParam(texture3dLerp, kernel, "_To", (RenderTargetIdentifier) to);
        int num2 = Mathf.CeilToInt((float) width / 8f);
        int threadGroupsZ = Mathf.CeilToInt((float) width / (!RuntimeUtilities.isAndroidOpenGL ? 8f : 2f));
        this.m_Command.DispatchCompute(texture3dLerp, kernel, num2, num2, threadGroupsZ);
      }
      else
      {
        renderTexture = this.Get(TextureFormatUtilities.GetUncompressedRenderTextureFormat(to), to.width, to.height);
        PropertySheet propertySheet = this.m_PropertySheets.Get(this.m_Resources.shaders.texture2dLerp);
        propertySheet.properties.SetTexture(ShaderIDs.To, to);
        propertySheet.properties.SetFloat(ShaderIDs.Interp, t);
        this.m_Command.BlitFullscreenTriangle((RenderTargetIdentifier) from, (RenderTargetIdentifier) (Texture) renderTexture, propertySheet, 0);
      }
      return (Texture) renderTexture;
    }

    internal void Clear()
    {
      foreach (Object active in this.m_Actives)
        RuntimeUtilities.Destroy(active);
      foreach (Object @object in this.m_Recycled)
        RuntimeUtilities.Destroy(@object);
      this.m_Actives.Clear();
      this.m_Recycled.Clear();
    }
  }
}
