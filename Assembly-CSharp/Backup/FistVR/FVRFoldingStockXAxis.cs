// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFoldingStockXAxis
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRFoldingStockXAxis : FVRInteractiveObject
  {
    public Transform Root;
    public Transform Stock;
    public Transform EndPiece;
    private float rotAngle;
    public float MinRot;
    public float MaxRot;
    public float EndPieceMinRot;
    public float EndPieceMaxRot;
    public FVRFoldingStockXAxis.StockPos m_curPos;
    public FVRFoldingStockXAxis.StockPos m_lastPos;
    public bool isMinClosed = true;
    public FVRFireArm FireArm;
    public bool DoesToggleStockPoint = true;
    public bool InvertZRoot;

    protected override void Start()
    {
      base.Start();
      if (this.isMinClosed)
        this.rotAngle = this.MinRot;
      else
        this.rotAngle = this.MaxRot;
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      Vector3 rhs = Vector3.ProjectOnPlane(hand.transform.position - this.Root.position, this.Root.right);
      rhs = rhs.normalized;
      Vector3 lhs = this.Stock.forward;
      if (this.InvertZRoot)
        lhs = -this.Stock.forward;
      this.rotAngle += Mathf.Clamp(Mathf.Atan2(Vector3.Dot(this.Root.right, Vector3.Cross(lhs, rhs)), Vector3.Dot(lhs, rhs)) * 57.29578f, -10f, 10f);
      this.rotAngle = Mathf.Clamp(this.rotAngle, this.MinRot, this.MaxRot);
      if ((double) Mathf.Abs(this.rotAngle - this.MinRot) < 5.0)
        this.rotAngle = this.MinRot;
      if ((double) Mathf.Abs(this.rotAngle - this.MaxRot) < 5.0)
        this.rotAngle = this.MaxRot;
      if ((double) this.rotAngle >= (double) this.MinRot && (double) this.rotAngle <= (double) this.MaxRot)
      {
        this.Stock.localEulerAngles = new Vector3(this.rotAngle, 0.0f, 0.0f);
        float t = Mathf.InverseLerp(this.MinRot, this.MaxRot, this.rotAngle);
        if ((Object) this.EndPiece != (Object) null)
          this.EndPiece.localEulerAngles = new Vector3(Mathf.Lerp(this.EndPieceMinRot, this.EndPieceMaxRot, t), 0.0f, 0.0f);
        if (this.isMinClosed)
        {
          if ((double) t < 0.0199999995529652)
          {
            this.m_curPos = FVRFoldingStockXAxis.StockPos.Closed;
            if (this.DoesToggleStockPoint)
              this.FireArm.HasActiveShoulderStock = false;
          }
          else if ((double) t > 0.899999976158142)
          {
            this.m_curPos = FVRFoldingStockXAxis.StockPos.Open;
            if (this.DoesToggleStockPoint)
              this.FireArm.HasActiveShoulderStock = true;
          }
          else
          {
            this.m_curPos = FVRFoldingStockXAxis.StockPos.Mid;
            if (this.DoesToggleStockPoint)
              this.FireArm.HasActiveShoulderStock = false;
          }
        }
        else if ((double) t < 0.100000001490116)
        {
          this.m_curPos = FVRFoldingStockXAxis.StockPos.Open;
          if (this.DoesToggleStockPoint)
            this.FireArm.HasActiveShoulderStock = true;
        }
        else if ((double) t > 0.980000019073486)
        {
          this.m_curPos = FVRFoldingStockXAxis.StockPos.Closed;
          if (this.DoesToggleStockPoint)
            this.FireArm.HasActiveShoulderStock = false;
        }
        else
        {
          this.m_curPos = FVRFoldingStockXAxis.StockPos.Mid;
          if (this.DoesToggleStockPoint)
            this.FireArm.HasActiveShoulderStock = false;
        }
      }
      if (this.m_curPos == FVRFoldingStockXAxis.StockPos.Open && this.m_lastPos != FVRFoldingStockXAxis.StockPos.Open)
        this.FireArm.PlayAudioEvent(FirearmAudioEventType.StockOpen);
      if (this.m_curPos == FVRFoldingStockXAxis.StockPos.Closed && this.m_lastPos != FVRFoldingStockXAxis.StockPos.Closed)
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
