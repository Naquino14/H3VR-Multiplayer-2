// Decompiled with JetBrains decompiler
// Type: FistVR.PhysicalMagazineReleaseLatch
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class PhysicalMagazineReleaseLatch : MonoBehaviour
  {
    public FVRFireArm FireArm;
    public HingeJoint Joint;
    private float timeSinceLastCollision = 6f;

    private void FixedUpdate()
    {
      if ((double) this.timeSinceLastCollision < 5.0)
        this.timeSinceLastCollision += Time.deltaTime;
      if (!((Object) this.FireArm.Magazine != (Object) null) || (double) this.timeSinceLastCollision >= 0.0299999993294477 || (double) this.Joint.angle >= -35.0)
        return;
      this.FireArm.EjectMag();
    }

    private void OnCollisionEnter(Collision col)
    {
      if (!((Object) col.collider.attachedRigidbody != (Object) null) || !((Object) col.collider.attachedRigidbody != (Object) this.FireArm.RootRigidbody) || (!((Object) col.collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>() != (Object) null) || !col.collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>().IsHeld))
        return;
      this.timeSinceLastCollision = 0.0f;
    }
  }
}
