// Decompiled with JetBrains decompiler
// Type: FistVR.TwoStageRotationOnHeld
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class TwoStageRotationOnHeld : MonoBehaviour
  {
    public FVRInteractiveObject IO;
    private bool m_isHeld;
    public Transform RotPiece;
    public Vector3 Rots_Held;
    public Vector3 Rots_NotHeld;

    private void Start()
    {
    }

    private void Update()
    {
      if (!this.m_isHeld)
      {
        if (!this.IO.IsHeld)
          return;
        this.m_isHeld = true;
        this.SetRot(true);
      }
      else
      {
        if (this.IO.IsHeld)
          return;
        this.m_isHeld = false;
        this.SetRot(false);
      }
    }

    private void SetRot(bool isHeld)
    {
      if (isHeld)
        this.RotPiece.localEulerAngles = this.Rots_Held;
      else
        this.RotPiece.localEulerAngles = this.Rots_NotHeld;
    }
  }
}
