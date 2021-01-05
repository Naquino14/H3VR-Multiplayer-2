// Decompiled with JetBrains decompiler
// Type: FFmpeg.AutoGen.AVFrameSideData
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FFmpeg.AutoGen
{
  public struct AVFrameSideData
  {
    public AVFrameSideDataType type;
    public unsafe sbyte* data;
    public int size;
    public unsafe AVDictionary* metadata;
    public unsafe AVBufferRef* buf;
  }
}
