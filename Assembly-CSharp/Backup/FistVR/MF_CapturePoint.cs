// Decompiled with JetBrains decompiler
// Type: FistVR.MF_CapturePoint
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MF_CapturePoint : MonoBehaviour
  {
    private MF_Zone m_zone;
    private bool m_isCapturePointActive;
    public MF_CapturePoint.MF_CapturePointState State_RedTeam;
    public MF_CapturePoint.MF_CapturePointState State_BlueTeam;
    private int m_iffRed;
    private int m_iffBlue = 1;
    private float m_captureScaleRed;
    private float m_captureScaleBlue;

    public void SetZone(MF_Zone z) => this.m_zone = z;

    public MF_Zone GetZone() => this.m_zone;

    public void InitState(MF_TeamColor c, MF_CapturePoint.MF_CapturePointState s)
    {
      if (c == MF_TeamColor.Red)
        this.State_RedTeam = s;
      else
        this.State_BlueTeam = s;
      if (c == MF_TeamColor.Red)
        this.m_captureScaleRed = s == MF_CapturePoint.MF_CapturePointState.Unlocked || s == MF_CapturePoint.MF_CapturePointState.Locked ? 0.0f : 1f;
      if (c != MF_TeamColor.Blue)
        return;
      if (s == MF_CapturePoint.MF_CapturePointState.Unlocked || s == MF_CapturePoint.MF_CapturePointState.Locked)
        this.m_captureScaleBlue = 0.0f;
      else
        this.m_captureScaleBlue = 1f;
    }

    public MF_CapturePoint.MF_CapturePointState GetState(MF_TeamColor c) => c == MF_TeamColor.Red ? this.State_RedTeam : this.State_BlueTeam;

    public void Tick(float t)
    {
    }

    public enum MF_CapturePointState
    {
      Unlocked,
      Locked,
      Owned,
    }
  }
}
