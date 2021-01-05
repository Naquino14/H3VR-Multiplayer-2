// Decompiled with JetBrains decompiler
// Type: FistVR.ChainsawHandle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class ChainsawHandle : FVRInteractiveObject
  {
    [Header("ChainsawHandle Params")]
    public Transform BasePoint;
    public Chainsaw Chainsaw;
    public Transform Cable;
    private Vector3 dirToHand = Vector3.zero;
    private float m_curCableLength;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      this.dirToHand = hand.transform.position - this.BasePoint.position;
      this.dirToHand = Vector3.ClampMagnitude(this.dirToHand, 1.2f);
      this.m_curCableLength = this.dirToHand.magnitude;
      this.Chainsaw.SetCableLength(this.m_curCableLength);
      this.transform.position = this.BasePoint.position + this.dirToHand.normalized * Mathf.Clamp(this.m_curCableLength - 0.05f, 0.01f, this.dirToHand.magnitude);
      this.Cable.LookAt(this.transform.position, Vector3.up);
      this.Cable.localScale = new Vector3(1f / 500f, 1f / 500f, Vector3.Distance(this.Cable.position, this.transform.position) + 0.02f);
      this.transform.rotation = Quaternion.LookRotation(this.dirToHand, -hand.transform.forward);
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (this.IsHeld)
        return;
      this.Cable.LookAt(this.transform.position, Vector3.up);
      this.Cable.localScale = new Vector3(1f / 500f, 1f / 500f, Vector3.Distance(this.Cable.position, this.transform.position) + 0.02f);
      this.m_curCableLength -= Time.deltaTime * 10f;
      this.m_curCableLength = Mathf.Clamp(this.m_curCableLength, 1f / 500f, this.m_curCableLength);
      this.dirToHand = Vector3.ClampMagnitude(this.dirToHand, this.m_curCableLength);
      if ((double) this.m_curCableLength > 1.0 / 500.0)
      {
        this.transform.position = this.BasePoint.position + this.dirToHand;
        this.transform.rotation = this.BasePoint.rotation;
      }
      else
      {
        if ((double) this.m_curCableLength != 1.0 / 500.0)
          return;
        this.transform.position = this.BasePoint.position + this.dirToHand;
        this.transform.rotation = this.BasePoint.rotation;
        this.m_curCableLength = 1f / 1000f;
      }
    }
  }
}
