// Decompiled with JetBrains decompiler
// Type: FistVR.ClosedBoltReceiverDustCoverTrigger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class ClosedBoltReceiverDustCoverTrigger : FVRInteractiveObject
  {
    public ClosedBolt Bolt;
    public Transform DustCoverGeo;
    private bool m_isOpen;
    public float OpenRot;
    public float ClosedRot;
    public float RotSpeed = 360f;
    private float m_curRot;
    private float m_tarRot;

    protected override void Awake()
    {
      base.Awake();
      this.m_curRot = this.ClosedRot;
      this.m_tarRot = this.ClosedRot;
    }

    public override void SimpleInteraction(FVRViveHand hand)
    {
      base.SimpleInteraction(hand);
      this.ToggleDustCoverState();
    }

    private void ToggleDustCoverState()
    {
      if (this.m_isOpen)
        this.Close();
      else
        this.Open();
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (!this.m_isOpen && this.Bolt.CurPos != ClosedBolt.BoltPos.Forward)
        this.Open();
      if ((double) Mathf.Abs(this.m_tarRot - this.m_curRot) <= 0.00999999977648258)
        return;
      this.m_curRot = Mathf.MoveTowards(this.m_curRot, this.m_tarRot, Time.deltaTime * this.RotSpeed);
      this.DustCoverGeo.localEulerAngles = new Vector3(0.0f, 0.0f, this.m_curRot);
    }

    private void Open()
    {
      this.m_isOpen = true;
      this.m_tarRot = this.OpenRot;
      this.RotSpeed = 1900f;
    }

    private void Close()
    {
      this.m_isOpen = false;
      this.m_tarRot = this.ClosedRot;
      this.RotSpeed = 500f;
    }
  }
}
