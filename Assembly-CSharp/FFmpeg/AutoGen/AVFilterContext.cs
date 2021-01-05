// Decompiled with JetBrains decompiler
// Type: FFmpeg.AutoGen.AVFilterContext
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FFmpeg.AutoGen
{
  public struct AVFilterContext
  {
    public unsafe AVClass* av_class;
    public unsafe AVFilter* filter;
    public unsafe sbyte* name;
    public unsafe AVFilterPad* input_pads;
    public unsafe AVFilterLink** inputs;
    public uint nb_inputs;
    public unsafe AVFilterPad* output_pads;
    public unsafe AVFilterLink** outputs;
    public uint nb_outputs;
    public unsafe void* priv;
    public unsafe AVFilterGraph* graph;
    public int thread_type;
    public unsafe AVFilterInternal* @internal;
    public unsafe AVFilterCommand* command_queue;
    public unsafe sbyte* enable_str;
    public unsafe void* enable;
    public unsafe double* var_values;
    public int is_disabled;
  }
}
