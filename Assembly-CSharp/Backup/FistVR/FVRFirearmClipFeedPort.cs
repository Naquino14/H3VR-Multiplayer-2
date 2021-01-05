// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFirearmClipFeedPort
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRFirearmClipFeedPort : MonoBehaviour
  {
    public FVRFireArm Firearm;
    public Transform PortPiece;
    public FVRPhysicalObject.Axis Axis = FVRPhysicalObject.Axis.Z;
    public float RotOpen;
    public float RotClosed;
    private bool m_isClosed = true;

    private void Awake() => this.UpdateRot();

    private void Update()
    {
      if (this.m_isClosed && (Object) this.Firearm.Clip != (Object) null)
      {
        this.m_isClosed = false;
        this.UpdateRot();
      }
      else
      {
        if (this.m_isClosed || !((Object) this.Firearm.Clip == (Object) null))
          return;
        this.m_isClosed = true;
        this.UpdateRot();
      }
    }

    private void UpdateRot()
    {
      float num = this.RotOpen;
      if (this.m_isClosed)
        num = this.RotClosed;
      Vector3 vector3 = new Vector3(0.0f, 0.0f, 0.0f);
      switch (this.Axis)
      {
        case FVRPhysicalObject.Axis.X:
          vector3.x = num;
          break;
        case FVRPhysicalObject.Axis.Y:
          vector3.y = num;
          break;
        case FVRPhysicalObject.Axis.Z:
          vector3.z = num;
          break;
      }
      this.PortPiece.localEulerAngles = vector3;
    }
  }
}
