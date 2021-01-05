// Decompiled with JetBrains decompiler
// Type: AmplifyColor.VersionInfo
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace AmplifyColor
{
  [Serializable]
  public class VersionInfo
  {
    public const byte Major = 1;
    public const byte Minor = 6;
    public const byte Release = 6;
    private static string StageSuffix = "_dev002";
    private static string TrialSuffix = string.Empty;
    [SerializeField]
    private int m_major;
    [SerializeField]
    private int m_minor;
    [SerializeField]
    private int m_release;

    private VersionInfo()
    {
      this.m_major = 1;
      this.m_minor = 6;
      this.m_release = 6;
    }

    private VersionInfo(byte major, byte minor, byte release)
    {
      this.m_major = (int) major;
      this.m_minor = (int) minor;
      this.m_release = (int) release;
    }

    public static string StaticToString() => string.Format("{0}.{1}.{2}", (object) (byte) 1, (object) (byte) 6, (object) (byte) 6) + VersionInfo.StageSuffix + VersionInfo.TrialSuffix;

    public override string ToString() => string.Format("{0}.{1}.{2}", (object) this.m_major, (object) this.m_minor, (object) this.m_release) + VersionInfo.StageSuffix + VersionInfo.TrialSuffix;

    public int Number => this.m_major * 100 + this.m_minor * 10 + this.m_release;

    public static VersionInfo Current() => new VersionInfo((byte) 1, (byte) 6, (byte) 6);

    public static bool Matches(VersionInfo version) => version.m_major == 1 && version.m_minor == 6 && 6 == version.m_release;
  }
}
