// Decompiled with JetBrains decompiler
// Type: FistVR.HoloSight
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class HoloSight : FVRFireArmAttachmentInterface
  {
    private int m_zeroDistanceIndex = 1;
    private float[] m_zeroDistances = new float[7]
    {
      2f,
      5f,
      10f,
      15f,
      25f,
      50f,
      100f
    };
    public Transform TargetPoint;
    public Text ZeroingText;

    protected override void Awake()
    {
      base.Awake();
      this.Zero();
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      Vector2 touchpadAxes = hand.Input.TouchpadAxes;
      if (hand.Input.TouchpadDown && (double) touchpadAxes.magnitude > 0.25)
      {
        if ((double) Vector2.Angle(touchpadAxes, Vector2.left) <= 45.0)
        {
          this.DecreaseZeroDistance();
          this.Zero();
        }
        else if ((double) Vector2.Angle(touchpadAxes, Vector2.right) <= 45.0)
        {
          this.IncreaseZeroDistance();
          this.Zero();
        }
      }
      base.UpdateInteraction(hand);
    }

    private void IncreaseZeroDistance()
    {
      ++this.m_zeroDistanceIndex;
      this.m_zeroDistanceIndex = Mathf.Clamp(this.m_zeroDistanceIndex, 0, this.m_zeroDistances.Length - 1);
    }

    private void DecreaseZeroDistance()
    {
      --this.m_zeroDistanceIndex;
      this.m_zeroDistanceIndex = Mathf.Clamp(this.m_zeroDistanceIndex, 0, this.m_zeroDistances.Length - 1);
    }

    private void Zero()
    {
      if ((Object) this.Attachment.curMount != (Object) null && (Object) this.Attachment.curMount.Parent != (Object) null && this.Attachment.curMount.Parent is FVRFireArm)
      {
        FVRFireArm parent = this.Attachment.curMount.Parent as FVRFireArm;
        this.TargetPoint.position = parent.MuzzlePos.position + parent.MuzzlePos.forward * this.m_zeroDistances[this.m_zeroDistanceIndex];
      }
      else
        this.TargetPoint.position = this.transform.position + this.transform.forward * this.m_zeroDistances[this.m_zeroDistanceIndex];
      this.ZeroingText.text = "Zero Distance: " + this.m_zeroDistances[this.m_zeroDistanceIndex].ToString() + "m";
    }

    public override void OnAttach()
    {
      base.OnAttach();
      this.Zero();
    }

    public override void OnDetach()
    {
      base.OnDetach();
      this.Zero();
    }
  }
}
