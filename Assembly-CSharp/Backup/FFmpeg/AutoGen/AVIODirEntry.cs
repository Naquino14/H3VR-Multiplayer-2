// Decompiled with JetBrains decompiler
// Type: FFmpeg.AutoGen.AVIODirEntry
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FFmpeg.AutoGen
{
  public struct AVIODirEntry
  {
    public unsafe sbyte* name;
    public int type;
    public int utf8;
    public long size;
    public long modification_timestamp;
    public long access_timestamp;
    public long status_change_timestamp;
    public long user_id;
    public long group_id;
    public long filemode;
  }
}
