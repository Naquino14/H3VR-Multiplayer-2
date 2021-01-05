// Decompiled with JetBrains decompiler
// Type: FFmpeg.AutoGen.AVIOContext
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace FFmpeg.AutoGen
{
  public struct AVIOContext
  {
    public unsafe AVClass* av_class;
    public unsafe sbyte* buffer;
    public int buffer_size;
    public unsafe sbyte* buf_ptr;
    public unsafe sbyte* buf_end;
    public unsafe void* opaque;
    public IntPtr read_packet;
    public IntPtr write_packet;
    public IntPtr seek;
    public long pos;
    public int must_flush;
    public int eof_reached;
    public int write_flag;
    public int max_packet_size;
    public int checksum;
    public unsafe sbyte* checksum_ptr;
    public IntPtr update_checksum;
    public int error;
    public IntPtr read_pause;
    public IntPtr read_seek;
    public int seekable;
    public long maxsize;
    public int direct;
    public long bytes_read;
    public int seek_count;
    public int writeout_count;
    public int orig_buffer_size;
    public int short_seek_threshold;
    public unsafe sbyte* protocol_whitelist;
  }
}
