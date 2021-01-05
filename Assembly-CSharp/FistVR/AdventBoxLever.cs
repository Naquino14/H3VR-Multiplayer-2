// Decompiled with JetBrains decompiler
// Type: FistVR.AdventBoxLever
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  public class AdventBoxLever : FVRInteractiveObject
  {
    public Transform LeverRoot;
    public Transform RotPosUp;
    public Transform RotPosDown;
    private bool m_hasSwitched;
    public AdventBox Box;
    public WW_Bunker Bunker;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      Vector3 vector3_1 = Vector3.ProjectOnPlane(hand.transform.position - this.LeverRoot.position, this.LeverRoot.right);
      float num = Mathf.Min(Vector3.Angle(vector3_1, this.LeverRoot.forward), 45f);
      Vector3 vector3_2 = Vector3.RotateTowards(this.LeverRoot.forward, vector3_1, num * ((float) Math.PI / 180f), 1f);
      this.transform.rotation = Quaternion.LookRotation(vector3_2, Vector3.up);
      if (this.m_hasSwitched || (double) Vector3.Angle(vector3_2, this.RotPosDown.forward) >= 10.0)
        return;
      this.m_hasSwitched = true;
      if ((UnityEngine.Object) this.Bunker != (UnityEngine.Object) null)
        this.Bunker.OpenField();
      else
        this.Box.OpenBox();
    }

    public void SetPulled()
    {
      this.m_hasSwitched = true;
      this.transform.rotation = this.RotPosDown.rotation;
    }

    private void Update()
    {
      if (this.IsHeld)
        return;
      if (this.m_hasSwitched)
      {
        if ((double) Vector3.Angle(this.RotPosDown.forward, this.transform.forward) <= 1.0)
          return;
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, this.RotPosDown.rotation, Time.deltaTime * 2f);
      }
      else
      {
        if ((double) Vector3.Angle(this.RotPosUp.forward, this.transform.forward) <= 1.0)
          return;
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, this.RotPosUp.rotation, Time.deltaTime * 2f);
      }
    }
  }
}
