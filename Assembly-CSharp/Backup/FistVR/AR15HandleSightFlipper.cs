// Decompiled with JetBrains decompiler
// Type: FistVR.AR15HandleSightFlipper
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AR15HandleSightFlipper : FVRInteractiveObject
  {
    private bool m_isLargeAperture = true;
    public Transform Flipsight;
    public float m_flipsightStartRotX;
    public float m_flipsightEndRotX = -90f;
    private float m_flipsightCurRotX;
    public AR15HandleSightFlipper.Axis RotAxis;
    private float m_curFlipLerp;
    private float m_tarFlipLerp;
    private float m_lastFlipLerp;

    protected override void Awake() => base.Awake();

    public override void SimpleInteraction(FVRViveHand hand)
    {
      base.SimpleInteraction(hand);
      this.m_isLargeAperture = !this.m_isLargeAperture;
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      this.m_tarFlipLerp = !this.m_isLargeAperture ? 1f : 0.0f;
      this.m_curFlipLerp = Mathf.MoveTowards(this.m_curFlipLerp, this.m_tarFlipLerp, Time.deltaTime * 4f);
      if ((double) Mathf.Abs(this.m_curFlipLerp - this.m_lastFlipLerp) > 0.00999999977648258)
      {
        this.m_flipsightCurRotX = Mathf.Lerp(this.m_flipsightStartRotX, this.m_flipsightEndRotX, this.m_curFlipLerp);
        switch (this.RotAxis)
        {
          case AR15HandleSightFlipper.Axis.X:
            this.Flipsight.localEulerAngles = new Vector3(this.m_flipsightCurRotX, 0.0f, 0.0f);
            break;
          case AR15HandleSightFlipper.Axis.Y:
            this.Flipsight.localEulerAngles = new Vector3(0.0f, this.m_flipsightCurRotX, 0.0f);
            break;
          case AR15HandleSightFlipper.Axis.Z:
            this.Flipsight.localEulerAngles = new Vector3(0.0f, 0.0f, this.m_flipsightCurRotX);
            break;
        }
      }
      this.m_lastFlipLerp = this.m_curFlipLerp;
    }

    public enum Axis
    {
      X,
      Y,
      Z,
    }
  }
}
