// Decompiled with JetBrains decompiler
// Type: FistVR.AR15HandleSightRaiser
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AR15HandleSightRaiser : FVRInteractiveObject
  {
    public Transform SightPiece;
    public Transform SightBottomPoint;
    public Transform SightTopPoint;
    private float m_sightHeight;
    private Vector2 TotalAmountMoved;
    private AR15HandleSightRaiser.SightHeights height;

    public override void SimpleInteraction(FVRViveHand hand)
    {
      base.SimpleInteraction(hand);
      switch (this.height)
      {
        case AR15HandleSightRaiser.SightHeights.Lowest:
          this.height = AR15HandleSightRaiser.SightHeights.Low;
          this.m_sightHeight = 0.25f;
          break;
        case AR15HandleSightRaiser.SightHeights.Low:
          this.height = AR15HandleSightRaiser.SightHeights.Mid;
          this.m_sightHeight = 0.5f;
          break;
        case AR15HandleSightRaiser.SightHeights.Mid:
          this.height = AR15HandleSightRaiser.SightHeights.High;
          this.m_sightHeight = 0.75f;
          break;
        case AR15HandleSightRaiser.SightHeights.High:
          this.height = AR15HandleSightRaiser.SightHeights.Highest;
          this.m_sightHeight = 1f;
          break;
        case AR15HandleSightRaiser.SightHeights.Highest:
          this.height = AR15HandleSightRaiser.SightHeights.Lowest;
          this.m_sightHeight = 0.0f;
          break;
      }
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      this.SightPiece.transform.position = Vector3.Lerp(this.SightBottomPoint.position, this.SightTopPoint.position, this.m_sightHeight);
    }

    public enum SightHeights
    {
      Lowest,
      Low,
      Mid,
      High,
      Highest,
    }
  }
}
