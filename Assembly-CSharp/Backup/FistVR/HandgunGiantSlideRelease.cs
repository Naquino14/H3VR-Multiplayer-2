// Decompiled with JetBrains decompiler
// Type: FistVR.HandgunGiantSlideRelease
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class HandgunGiantSlideRelease : FVRInteractiveObject
  {
    public Transform UpPoint;
    public Transform DownPoint;
    public Handgun Handgun;

    protected override void FVRFixedUpdate()
    {
      base.FVRFixedUpdate();
      if (this.IsHeld)
      {
        Vector3 closestValidPoint = this.GetClosestValidPoint(this.UpPoint.position, this.DownPoint.position, this.m_handPos);
        this.Handgun.IsSlideLockExternalPushedUp = (double) this.m_handPos.y > (double) closestValidPoint.y;
        if ((double) this.m_handPos.y < (double) closestValidPoint.y)
          this.Handgun.IsSlideLockExternalHeldDown = true;
        else
          this.Handgun.IsSlideLockExternalHeldDown = false;
      }
      else
      {
        this.Handgun.IsSlideLockExternalPushedUp = false;
        this.Handgun.IsSlideLockExternalHeldDown = false;
      }
    }
  }
}
