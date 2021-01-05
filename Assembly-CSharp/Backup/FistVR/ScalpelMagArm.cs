// Decompiled with JetBrains decompiler
// Type: FistVR.ScalpelMagArm
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class ScalpelMagArm : MonoBehaviour
  {
    public FVRFireArm FireArm;
    public Transform Latch;
    private bool m_hasMag;
    public Vector3 Rot_NoMag;
    public Vector3 Rot_HasMag_Low;
    public Vector3 Rot_HasMag_High;

    private void Start()
    {
    }

    private void Update()
    {
      if ((Object) this.FireArm.Magazine == (Object) null)
      {
        if (!this.m_hasMag)
          return;
        this.m_hasMag = false;
        this.SetMag(false);
      }
      else
      {
        if (this.m_hasMag)
          return;
        this.m_hasMag = true;
        this.SetMag(true);
      }
    }

    private void SetMag(bool h)
    {
      if (!h)
        this.SetRot(this.Rot_NoMag);
      else
        this.SetRot(this.Rot_HasMag_Low);
    }

    private void SetRot(Vector3 v) => this.Latch.localEulerAngles = v;
  }
}
