// Decompiled with JetBrains decompiler
// Type: FistVR.wwGatlingGunHandle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwGatlingGunHandle : FVRInteractiveObject
  {
    public wwGatlingGun Gun;
    private Vector3 m_curCrankDir;
    private float m_CrankDelta;
    private float m_crankedAmount;
    public bool m_isCrankHeld;
    public Transform YUpTarget;

    protected override void Awake()
    {
      base.Awake();
      this.m_curCrankDir = this.transform.forward;
    }

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      this.m_isCrankHeld = true;
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      Vector3 normalized = Vector3.ProjectOnPlane((hand.transform.position - this.transform.position).normalized, this.YUpTarget.up).normalized;
      Vector3 forward = this.transform.forward;
      float crank = Mathf.Clamp(Mathf.Atan2(Vector3.Dot(this.YUpTarget.up, Vector3.Cross(forward, normalized)), Vector3.Dot(forward, normalized)) * 57.29578f, 0.0f, 10f);
      if ((double) crank <= 0.0)
        return;
      this.Gun.CrankGun(crank);
      this.m_curCrankDir = Vector3.RotateTowards(this.m_curCrankDir, normalized, crank * 0.0174533f, 0.0f);
      this.m_curCrankDir = Vector3.ProjectOnPlane(this.m_curCrankDir, this.YUpTarget.up);
      this.transform.rotation = Quaternion.LookRotation(this.m_curCrankDir, this.YUpTarget.up);
    }
  }
}
