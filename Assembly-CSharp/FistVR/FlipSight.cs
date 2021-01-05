// Decompiled with JetBrains decompiler
// Type: FistVR.FlipSight
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FlipSight : FVRFireArmAttachmentInterface
  {
    private float curXRot;
    public float XRotUp;
    public float XRotDown;
    public bool IsUp = true;
    private float m_curFlipLerp;
    private float m_tarFlipLerp;
    private float m_lastFlipLerp;
    public Transform FlipPart;

    protected override void Awake()
    {
      base.Awake();
      if (this.IsUp)
      {
        this.curXRot = this.XRotUp;
        this.m_curFlipLerp = 1f;
        this.m_tarFlipLerp = 1f;
        this.m_lastFlipLerp = 1f;
      }
      else
      {
        this.curXRot = this.XRotDown;
        this.m_curFlipLerp = 0.0f;
        this.m_tarFlipLerp = 0.0f;
        this.m_lastFlipLerp = 0.0f;
      }
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      Vector2 touchpadAxes = hand.Input.TouchpadAxes;
      if (hand.IsInStreamlinedMode)
      {
        if (hand.Input.BYButtonDown)
          this.Flip();
      }
      else if (hand.Input.TouchpadDown && (double) touchpadAxes.magnitude > 0.25 && (double) Vector2.Angle(touchpadAxes, Vector2.up) <= 45.0)
        this.Flip();
      base.UpdateInteraction(hand);
    }

    protected override void FVRFixedUpdate()
    {
      if ((Object) this.Attachment.curMount != (Object) null)
      {
        this.m_tarFlipLerp = !this.IsUp ? 0.0f : 1f;
        this.m_curFlipLerp = Mathf.MoveTowards(this.m_curFlipLerp, this.m_tarFlipLerp, Time.deltaTime * 4f);
        if ((double) Mathf.Abs(this.m_curFlipLerp - this.m_lastFlipLerp) > 0.00999999977648258)
          this.FlipPart.localEulerAngles = new Vector3(Mathf.Lerp(this.XRotDown, this.XRotUp, this.m_curFlipLerp), 0.0f, 0.0f);
        this.m_lastFlipLerp = this.m_curFlipLerp;
      }
      base.FVRFixedUpdate();
    }

    private void Flip() => this.IsUp = !this.IsUp;
  }
}
