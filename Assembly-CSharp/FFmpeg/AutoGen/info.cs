// Decompiled with JetBrains decompiler
// Type: FFmpeg.AutoGen.info
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace FFmpeg.AutoGen
{
  public struct info
  {
    public long last_dts;
    public long duration_gcd;
    public int duration_count;
    public long rfps_duration_sum;
    public IntPtr duration_error;
    public long codec_info_duration;
    public long codec_info_duration_fields;
    public int found_decoder;
    public long last_duration;
    public long fps_first_dts;
    public int fps_first_dts_idx;
    public long fps_last_dts;
    public int fps_last_dts_idx;
  }
}
