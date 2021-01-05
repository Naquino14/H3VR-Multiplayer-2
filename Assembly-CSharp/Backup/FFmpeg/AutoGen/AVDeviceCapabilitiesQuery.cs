// Decompiled with JetBrains decompiler
// Type: FFmpeg.AutoGen.AVDeviceCapabilitiesQuery
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FFmpeg.AutoGen
{
  public struct AVDeviceCapabilitiesQuery
  {
    public unsafe AVClass* av_class;
    public unsafe AVFormatContext* device_context;
    public AVCodecID codec;
    public AVSampleFormat sample_format;
    public AVPixelFormat pixel_format;
    public int sample_rate;
    public int channels;
    public long channel_layout;
    public int window_width;
    public int window_height;
    public int frame_width;
    public int frame_height;
    public AVRational fps;
  }
}
