// Decompiled with JetBrains decompiler
// Type: FistVR.FVRMovementRig
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRMovementRig : MonoBehaviour
  {
    public Transform HeadProxy;
    public Transform ControllerProxy_Left;
    public Transform ControllerProxy_Right;
    private Transform m_head;
    private Transform m_lefthand;
    private Transform m_righthand;
    private bool m_hasFoundCorners;
    public Transform CornerHolder;
    public Transform[] CornerGeos;
    private Vector3 c1;
    private Vector3 c2;
    private Vector3 c3;
    private Vector3 c4;

    private void Update()
    {
      if (this.m_hasFoundCorners)
        return;
      this.m_hasFoundCorners = FVRPlayArea.TryGetPlayArea(out this.c1, out this.c2, out this.c3, out this.c4);
      if (!this.m_hasFoundCorners)
        return;
      this.CornerGeos[0].transform.localPosition = this.c1;
      this.CornerGeos[1].transform.localPosition = this.c2;
      this.CornerGeos[2].transform.localPosition = this.c3;
      this.CornerGeos[3].transform.localPosition = this.c4;
    }
  }
}
