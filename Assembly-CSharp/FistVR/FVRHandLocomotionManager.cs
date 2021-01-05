// Decompiled with JetBrains decompiler
// Type: FistVR.FVRHandLocomotionManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRHandLocomotionManager : MonoBehaviour
  {
    public bool DoesFall;
    public float FallSpeed = 1f;
    public FVRHandGrabPoint ActiveGrabPoint;
    private Vector2 clampX = Vector2.zero;
    private Vector2 clampY = Vector2.zero;
    private Vector2 clampZ = Vector2.zero;
    private Rigidbody rb;

    public void Awake() => this.rb = this.GetComponent<Rigidbody>();

    public void NewGrab(FVRHandGrabPoint grabpoint) => this.ActiveGrabPoint = grabpoint;

    public void EndGrab(FVRHandGrabPoint grabpoint)
    {
      if (!((Object) this.ActiveGrabPoint == (Object) grabpoint))
        return;
      this.ActiveGrabPoint = (FVRHandGrabPoint) null;
    }

    public void Move(FVRHandGrabPoint grabpoint, Vector3 dir)
    {
      if ((Object) this.ActiveGrabPoint == (Object) null)
        this.ActiveGrabPoint = grabpoint;
      if (!((Object) this.ActiveGrabPoint == (Object) grabpoint))
        return;
      Vector3 position = this.transform.position;
      if ((double) position.x + (double) dir.x >= (double) this.clampX.x && (double) position.x + (double) dir.x <= (double) this.clampX.y)
        position.x += dir.x;
      else if ((double) position.x + (double) dir.x < (double) this.clampX.x)
      {
        position.x += dir.magnitude;
        position.x = Mathf.Min(position.x, this.clampX.x);
      }
      else if ((double) position.x + (double) dir.x > (double) this.clampX.y)
      {
        position.x -= dir.magnitude;
        position.x = Mathf.Max(position.x, this.clampX.y);
      }
      if ((double) position.y + (double) dir.y >= (double) this.clampY.x && (double) position.y + (double) dir.y <= (double) this.clampY.y)
        position.y += dir.y;
      else if ((double) position.y + (double) dir.y < (double) this.clampY.x)
        position.y += Mathf.Abs(dir.y);
      else if ((double) position.y + (double) dir.y > (double) this.clampY.y)
        position.y -= Mathf.Abs(dir.y);
      if ((double) position.z + (double) dir.z >= (double) this.clampZ.x && (double) position.z + (double) dir.z <= (double) this.clampZ.y)
        position.z += dir.z;
      else if ((double) position.z + (double) dir.z < (double) this.clampZ.x)
        position.z += Mathf.Abs(dir.z);
      else if ((double) position.z + (double) dir.z > (double) this.clampZ.y)
        position.z -= Mathf.Abs(dir.z);
      this.rb.position = position;
    }
  }
}
