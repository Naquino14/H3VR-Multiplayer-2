// Decompiled with JetBrains decompiler
// Type: FFmpeg.AutoGen.AVSubtitleRect
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FFmpeg.AutoGen
{
  public struct AVSubtitleRect
  {
    public int x;
    public int y;
    public int w;
    public int h;
    public int nb_colors;
    public AVPicture pict;
    public unsafe sbyte* data0;
    public unsafe sbyte* data1;
    public unsafe sbyte* data2;
    public unsafe sbyte* data3;
    public unsafe fixed int linesize[4];
    public AVSubtitleType type;
    public unsafe sbyte* text;
    public unsafe sbyte* ass;
    public int flags;
  }
}
