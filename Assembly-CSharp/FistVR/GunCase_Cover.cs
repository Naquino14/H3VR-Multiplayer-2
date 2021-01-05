// Decompiled with JetBrains decompiler
// Type: FistVR.GunCase_Cover
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class GunCase_Cover : FVRInteractiveObject
  {
    public List<GunCase_Latch> Latches;
    public Transform Root;
    private float rotAngle;
    public float MinRot;
    public float MaxRot;
    private bool m_forceOpen;

    public void ForceOpen() => this.m_forceOpen = true;

    public void LockCase()
    {
      for (int index = 0; index < this.Latches.Count; ++index)
        this.Latches[index].gameObject.SetActive(false);
    }

    public void UnlockCase()
    {
      for (int index = 0; index < this.Latches.Count; ++index)
        this.Latches[index].gameObject.SetActive(true);
    }

    public override bool IsInteractable() => this.m_forceOpen || this.Latches[0].IsOpen() && this.Latches[1].IsOpen();

    public void Reset() => this.transform.localEulerAngles = Vector3.zero;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      Vector3 rhs = Vector3.ProjectOnPlane(hand.transform.position - this.transform.position, this.Root.right);
      rhs = rhs.normalized;
      Vector3 forward = this.Root.transform.forward;
      this.rotAngle = Mathf.Atan2(Vector3.Dot(this.Root.right, Vector3.Cross(forward, rhs)), Vector3.Dot(forward, rhs)) * 57.29578f;
      if ((double) this.rotAngle > 0.0)
        this.rotAngle -= 360f;
      if ((double) Mathf.Abs(this.rotAngle - this.MinRot) < 5.0)
        this.rotAngle = this.MinRot;
      if ((double) Mathf.Abs(this.rotAngle - this.MaxRot) < 5.0)
        this.rotAngle = this.MaxRot;
      if ((double) this.rotAngle < (double) this.MinRot || (double) this.rotAngle > (double) this.MaxRot)
        return;
      this.transform.localEulerAngles = new Vector3(this.rotAngle, 0.0f, 0.0f);
    }
  }
}
