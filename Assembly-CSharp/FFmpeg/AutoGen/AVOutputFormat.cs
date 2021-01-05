﻿// Decompiled with JetBrains decompiler
// Type: FFmpeg.AutoGen.AVOutputFormat
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace FFmpeg.AutoGen
{
  public struct AVOutputFormat
  {
    public unsafe sbyte* name;
    public unsafe sbyte* long_name;
    public unsafe sbyte* mime_type;
    public unsafe sbyte* extensions;
    public AVCodecID audio_codec;
    public AVCodecID video_codec;
    public AVCodecID subtitle_codec;
    public int flags;
    public unsafe AVCodecTag** codec_tag;
    public unsafe AVClass* priv_class;
    public unsafe AVOutputFormat* next;
    public int priv_data_size;
    public IntPtr write_header;
    public IntPtr write_packet;
    public IntPtr write_trailer;
    public IntPtr interleave_packet;
    public IntPtr query_codec;
    public IntPtr get_output_timestamp;
    public IntPtr control_message;
    public IntPtr write_uncoded_frame;
    public IntPtr get_device_list;
    public IntPtr create_device_capabilities;
    public IntPtr free_device_capabilities;
    public AVCodecID data_codec;
    public IntPtr init;
    public IntPtr deinit;
    public IntPtr check_bitstream;
  }
}
