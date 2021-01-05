// Decompiled with JetBrains decompiler
// Type: FFmpeg.AutoGen.AVInputFormat
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace FFmpeg.AutoGen
{
  public struct AVInputFormat
  {
    public unsafe sbyte* name;
    public unsafe sbyte* long_name;
    public int flags;
    public unsafe sbyte* extensions;
    public unsafe AVCodecTag** codec_tag;
    public unsafe AVClass* priv_class;
    public unsafe sbyte* mime_type;
    public unsafe AVInputFormat* next;
    public int raw_codec_id;
    public int priv_data_size;
    public IntPtr read_probe;
    public IntPtr read_header;
    public IntPtr read_packet;
    public IntPtr read_close;
    public IntPtr read_seek;
    public IntPtr read_timestamp;
    public IntPtr read_play;
    public IntPtr read_pause;
    public IntPtr read_seek2;
    public IntPtr get_device_list;
    public IntPtr create_device_capabilities;
    public IntPtr free_device_capabilities;
  }
}
