// Decompiled with JetBrains decompiler
// Type: FistVR.M79SightingSystemBase
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class M79SightingSystemBase : FVRInteractiveObject
  {
    public float m_xRotMin;
    public float m_xRotMax = 90f;
    public float m_xRotCur;
    public bool IsFlippedUp;

    public override void SimpleInteraction(FVRViveHand hand)
    {
      this.IsFlippedUp = !this.IsFlippedUp;
      if (this.IsFlippedUp)
      {
        this.m_xRotCur = this.m_xRotMax;
        this.transform.localEulerAngles = new Vector3(this.m_xRotCur, 0.0f, 0.0f);
      }
      else
      {
        this.m_xRotCur = this.m_xRotMin;
        this.transform.localEulerAngles = new Vector3(this.m_xRotCur, 0.0f, 0.0f);
      }
    }
  }
}
