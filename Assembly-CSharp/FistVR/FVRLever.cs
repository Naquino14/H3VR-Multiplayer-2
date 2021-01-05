// Decompiled with JetBrains decompiler
// Type: FistVR.FVRLever
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRLever : FVRInteractiveObject
  {
    public Transform LeverTransform;
    public Transform Base;
    private bool m_isForward;
    private float m_curRot = -22.5f;
    public float minValue = -22.5f;
    public float maxValue = -22.5f;
    private FVRLever.LeverState curState;
    private FVRLever.LeverState lastState;
    private AudioSource aud;

    protected override void Awake()
    {
      base.Awake();
      this.aud = this.GetComponent<AudioSource>();
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      Vector3 to = Vector3.ProjectOnPlane(hand.transform.position - this.LeverTransform.position, this.LeverTransform.right);
      if ((double) Vector3.Dot(to.normalized, this.Base.up) > 0.0)
        this.m_curRot = -Vector3.Angle(this.Base.forward, to);
      else
        this.m_curRot = Vector3.Angle(this.Base.forward, to);
    }

    public float GetLeverValue() => Mathf.InverseLerp(this.minValue, this.maxValue, this.m_curRot);

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      this.m_curRot = Mathf.Clamp(this.m_curRot, -22.5f, 22.5f);
      this.LeverTransform.localEulerAngles = new Vector3(this.m_curRot, 0.0f, 0.0f);
      this.curState = (double) this.m_curRot <= 22.0 ? ((double) this.m_curRot >= -22.0 ? FVRLever.LeverState.Mid : FVRLever.LeverState.Off) : FVRLever.LeverState.On;
      if (this.curState == FVRLever.LeverState.On && this.lastState != FVRLever.LeverState.On)
        this.aud.PlayOneShot(this.aud.clip);
      if (this.curState == FVRLever.LeverState.Off && this.lastState != FVRLever.LeverState.Off)
        this.aud.PlayOneShot(this.aud.clip);
      this.lastState = this.curState;
    }

    public enum LeverState
    {
      Off,
      Mid,
      On,
    }
  }
}
