// Decompiled with JetBrains decompiler
// Type: FistVR.SlicerPhysicalBody
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class SlicerPhysicalBody : FVRPhysicalObject
  {
    public SlicerComputer Computer;
    private Vector3 curThrusterForce = Vector3.zero;
    private Vector3 tarThrusterForce = Vector3.zero;

    protected override void FVRFixedUpdate()
    {
      base.FVRFixedUpdate();
      if (!this.IsHeld)
        return;
      this.tarThrusterForce = this.Computer.CurrentThrusterForce * (float) (1.5 + (double) Mathf.PerlinNoise(this.transform.position.x, Time.time * 5f) * 0.5);
      this.tarThrusterForce += Random.onUnitSphere * 0.5f;
      this.curThrusterForce = Vector3.Lerp(this.curThrusterForce, this.tarThrusterForce, Time.deltaTime * 4f);
      this.RootRigidbody.velocity += this.curThrusterForce;
      this.RootRigidbody.angularVelocity += this.curThrusterForce * 3f;
    }
  }
}
