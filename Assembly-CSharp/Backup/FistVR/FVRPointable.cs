// Decompiled with JetBrains decompiler
// Type: FistVR.FVRPointable
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FVRPointable : MonoBehaviour
  {
    public float MaxPointingRange = 1f;
    protected List<FVRViveHand> PointingHands = new List<FVRViveHand>();
    protected bool m_isBeingPointedAt;

    public virtual void OnPoint(FVRViveHand hand)
    {
      if (this.PointingHands.Contains(hand))
        return;
      this.PointingHands.Add(hand);
      if (this.m_isBeingPointedAt)
        return;
      this.m_isBeingPointedAt = true;
      this.BeginHoverDisplay();
    }

    public virtual void EndPoint(FVRViveHand hand)
    {
      if (this.PointingHands.Contains(hand))
        this.PointingHands.Remove(hand);
      if (this.PointingHands.Count > 0)
        return;
      this.m_isBeingPointedAt = false;
      this.EndHoverDisplay();
    }

    public virtual void Update()
    {
      if (!this.m_isBeingPointedAt)
        return;
      this.OnHoverDisplay();
    }

    public virtual void BeginHoverDisplay()
    {
    }

    public virtual void EndHoverDisplay()
    {
    }

    public virtual void OnHoverDisplay()
    {
    }
  }
}
