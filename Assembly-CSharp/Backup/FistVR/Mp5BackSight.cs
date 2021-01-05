// Decompiled with JetBrains decompiler
// Type: FistVR.Mp5BackSight
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class Mp5BackSight : FVRInteractiveObject
  {
    public Transform BackSight;
    public Vector3[] SightPositions;
    private int m_sightPosition;

    public override void SimpleInteraction(FVRViveHand hand)
    {
      ++this.m_sightPosition;
      if (this.m_sightPosition >= this.SightPositions.Length)
        this.m_sightPosition = 0;
      this.BackSight.localEulerAngles = this.SightPositions[this.m_sightPosition];
      base.SimpleInteraction(hand);
    }
  }
}
