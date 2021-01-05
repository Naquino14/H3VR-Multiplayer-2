// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.TextureFormatUtilities
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
  public static class TextureFormatUtilities
  {
    private static Dictionary<TextureFormat, RenderTextureFormat> m_FormatMap = new Dictionary<TextureFormat, RenderTextureFormat>()
    {
      {
        TextureFormat.Alpha8,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.ARGB4444,
        RenderTextureFormat.ARGB4444
      },
      {
        TextureFormat.RGB24,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.RGBA32,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.ARGB32,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.RGB565,
        RenderTextureFormat.RGB565
      },
      {
        TextureFormat.R16,
        RenderTextureFormat.RHalf
      },
      {
        TextureFormat.DXT1,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.DXT5,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.RGBA4444,
        RenderTextureFormat.ARGB4444
      },
      {
        TextureFormat.BGRA32,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.RHalf,
        RenderTextureFormat.RHalf
      },
      {
        TextureFormat.RGHalf,
        RenderTextureFormat.RGHalf
      },
      {
        TextureFormat.RGBAHalf,
        RenderTextureFormat.ARGBHalf
      },
      {
        TextureFormat.RFloat,
        RenderTextureFormat.RFloat
      },
      {
        TextureFormat.RGFloat,
        RenderTextureFormat.RGFloat
      },
      {
        TextureFormat.RGBAFloat,
        RenderTextureFormat.ARGBFloat
      },
      {
        TextureFormat.RGB9e5Float,
        RenderTextureFormat.ARGBHalf
      },
      {
        TextureFormat.BC4,
        RenderTextureFormat.R8
      },
      {
        TextureFormat.BC5,
        RenderTextureFormat.RGHalf
      },
      {
        TextureFormat.BC6H,
        RenderTextureFormat.ARGBHalf
      },
      {
        TextureFormat.BC7,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.DXT1Crunched,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.DXT5Crunched,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.PVRTC_RGB2,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.PVRTC_RGBA2,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.PVRTC_RGB4,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.PVRTC_RGBA4,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.ETC_RGB4,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.ATC_RGB4,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.ATC_RGBA8,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.ETC2_RGB,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.ETC2_RGBA1,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.ETC2_RGBA8,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.ASTC_RGB_4x4,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.ASTC_RGB_5x5,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.ASTC_RGB_6x6,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.ASTC_RGB_8x8,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.ASTC_RGB_10x10,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.ASTC_RGB_12x12,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.ASTC_RGBA_4x4,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.ASTC_RGBA_5x5,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.ASTC_RGBA_6x6,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.ASTC_RGBA_8x8,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.ASTC_RGBA_10x10,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.ASTC_RGBA_12x12,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.ETC_RGB4_3DS,
        RenderTextureFormat.ARGB32
      },
      {
        TextureFormat.ETC_RGBA8_3DS,
        RenderTextureFormat.ARGB32
      }
    };

    public static RenderTextureFormat GetUncompressedRenderTextureFormat(
      Texture texture)
    {
      switch (texture)
      {
        case RenderTexture _:
          return (texture as RenderTexture).format;
        case Texture2D _:
          TextureFormat format = ((Texture2D) texture).format;
          RenderTextureFormat renderTextureFormat;
          if (!TextureFormatUtilities.m_FormatMap.TryGetValue(format, out renderTextureFormat))
            throw new NotSupportedException("Texture format not supported");
          return renderTextureFormat;
        default:
          return RenderTextureFormat.Default;
      }
    }
  }
}
