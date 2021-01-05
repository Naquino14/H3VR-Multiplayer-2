// Decompiled with JetBrains decompiler
// Type: FistVR.GP25_RangingSight
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class GP25_RangingSight : FVRInteractiveObject
  {
    public GP25 GP25;
    public Transform RangingPiece;
    private int m_rangingIndex;
    private float[] m_rangingAmounts = new float[7]
    {
      3.5f,
      6f,
      13f,
      21f,
      43f,
      66f,
      73f
    };

    public override void UpdateInteraction(FVRViveHand hand)
    {
      bool flag1 = false;
      bool flag2 = false;
      if (hand.IsInStreamlinedMode)
      {
        if (hand.Input.BYButtonDown)
          flag1 = true;
        if (hand.Input.AXButtonDown)
          flag2 = true;
      }
      else if (hand.Input.TouchpadDown)
      {
        if (hand.Input.TouchpadWestPressed)
          flag1 = true;
        if (hand.Input.TouchpadEastPressed)
          flag2 = true;
      }
      if (flag1 && this.m_rangingIndex > 0)
      {
        --this.m_rangingIndex;
        this.UpdateRangingPiece();
      }
      if (flag2 && this.m_rangingIndex < this.m_rangingAmounts.Length - 1)
      {
        ++this.m_rangingIndex;
        this.UpdateRangingPiece();
      }
      base.UpdateInteraction(hand);
    }

    private void UpdateRangingPiece()
    {
      this.GP25.PlayAudioEvent(FirearmAudioEventType.Safety);
      this.RangingPiece.localEulerAngles = new Vector3(this.m_rangingAmounts[this.m_rangingIndex], 0.0f, 0.0f);
    }
  }
}
