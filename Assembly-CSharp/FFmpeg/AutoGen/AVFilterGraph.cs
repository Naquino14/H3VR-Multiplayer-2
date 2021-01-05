// Decompiled with JetBrains decompiler
// Type: FFmpeg.AutoGen.AVFilterGraph
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace FFmpeg.AutoGen
{
  public struct AVFilterGraph
  {
    public unsafe AVClass* av_class;
    public unsafe AVFilterContext** filters;
    public uint nb_filters;
    public unsafe sbyte* scale_sws_opts;
    public unsafe sbyte* resample_lavr_opts;
    public int thread_type;
    public int nb_threads;
    public unsafe AVFilterGraphInternal* @internal;
    public unsafe void* opaque;
    public IntPtr execute;
    public unsafe sbyte* aresample_swr_opts;
    public unsafe AVFilterLink** sink_links;
    public int sink_links_count;
    public uint disable_auto_convert;
  }
}
