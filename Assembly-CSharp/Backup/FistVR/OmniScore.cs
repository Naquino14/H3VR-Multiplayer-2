// Decompiled with JetBrains decompiler
// Type: FistVR.OmniScore
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace FistVR
{
  [Serializable]
  public class OmniScore : IComparable<OmniScore>
  {
    public int Score;
    public string Name = string.Empty;

    public int CompareTo(OmniScore that)
    {
      if (that == null)
        return 1;
      if (this.Score > that.Score)
        return -1;
      return this.Score < that.Score ? 1 : 0;
    }
  }
}
