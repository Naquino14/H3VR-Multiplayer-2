// Decompiled with JetBrains decompiler
// Type: FFmpeg.AutoGen.SwrDitherType
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FFmpeg.AutoGen
{
  public enum SwrDitherType
  {
    SWR_DITHER_NONE = 0,
    SWR_DITHER_RECTANGULAR = 1,
    SWR_DITHER_TRIANGULAR = 2,
    SWR_DITHER_TRIANGULAR_HIGHPASS = 3,
    SWR_DITHER_NS = 64, // 0x00000040
    SWR_DITHER_NS_LIPSHITZ = 65, // 0x00000041
    SWR_DITHER_NS_F_WEIGHTED = 66, // 0x00000042
    SWR_DITHER_NS_MODIFIED_E_WEIGHTED = 67, // 0x00000043
    SWR_DITHER_NS_IMPROVED_E_WEIGHTED = 68, // 0x00000044
    SWR_DITHER_NS_SHIBATA = 69, // 0x00000045
    SWR_DITHER_NS_LOW_SHIBATA = 70, // 0x00000046
    SWR_DITHER_NS_HIGH_SHIBATA = 71, // 0x00000047
    SWR_DITHER_NB = 72, // 0x00000048
  }
}
