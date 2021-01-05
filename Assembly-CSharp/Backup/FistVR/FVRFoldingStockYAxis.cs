// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFoldingStockYAxis
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRFoldingStockYAxis : FVRInteractiveObject
  {
    public Transform Root;
    public Transform Stock;
    private float rotAngle;
    public float MinRot;
    public float MaxRot;
    public FVRFoldingStockYAxis.StockPos m_curPos;
    public FVRFoldingStockYAxis.StockPos m_lastPos;
    public bool isMinClosed = true;
    public FVRFireArm FireArm;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      Vector3 rhs = Vector3.ProjectOnPlane(hand.transform.position - this.Root.position, this.Root.up);
      rhs = rhs.normalized;
      Vector3 lhs = -this.Root.transform.forward;
      this.rotAngle = Mathf.Atan2(Vector3.Dot(this.Root.up, Vector3.Cross(lhs, rhs)), Vector3.Dot(lhs, rhs)) * 57.29578f;
      if ((double) Mathf.Abs(this.rotAngle - this.MinRot) < 5.0)
        this.rotAngle = this.MinRot;
      if ((double) Mathf.Abs(this.rotAngle - this.MaxRot) < 5.0)
        this.rotAngle = this.MaxRot;
      if ((double) this.rotAngle < (double) this.MinRot || (double) this.rotAngle > (double) this.MaxRot)
        return;
      this.Stock.localEulerAngles = new Vector3(0.0f, this.rotAngle, 0.0f);
      float num = Mathf.InverseLerp(this.MinRot, this.MaxRot, this.rotAngle);
      if (this.isMinClosed)
      {
        if ((double) num < 0.0199999995529652)
        {
          this.m_curPos = FVRFoldingStockYAxis.StockPos.Closed;
          this.FireArm.HasActiveShoulderStock = false;
        }
        else if ((double) num > 0.899999976158142)
        {
          this.m_curPos = FVRFoldingStockYAxis.StockPos.Open;
          this.FireArm.HasActiveShoulderStock = true;
        }
        else
        {
          this.m_curPos = FVRFoldingStockYAxis.StockPos.Mid;
          this.FireArm.HasActiveShoulderStock = false;
        }
      }
      else if ((double) num < 0.100000001490116)
      {
        this.m_curPos = FVRFoldingStockYAxis.StockPos.Open;
        this.FireArm.HasActiveShoulderStock = true;
      }
      else if ((double) num > 0.980000019073486)
      {
        this.m_curPos = FVRFoldingStockYAxis.StockPos.Closed;
        this.FireArm.HasActiveShoulderStock = false;
      }
      else
      {
        this.m_curPos = FVRFoldingStockYAxis.StockPos.Mid;
        this.FireArm.HasActiveShoulderStock = false;
      }
      if (this.m_curPos == FVRFoldingStockYAxis.StockPos.Open && this.m_lastPos != FVRFoldingStockYAxis.StockPos.Open)
        this.FireArm.PlayAudioEvent(FirearmAudioEventType.StockOpen);
      if (this.m_curPos == FVRFoldingStockYAxis.StockPos.Closed && this.m_lastPos != FVRFoldingStockYAxis.StockPos.Closed)
        this.FireArm.PlayAudioEvent(FirearmAudioEventType.StockClosed);
      this.m_lastPos = this.m_curPos;
    }

    public enum StockPos
    {
      Closed,
      Mid,
      Open,
    }
  }
}
