// Decompiled with JetBrains decompiler
// Type: FistVR.ForceExplosion
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class ForceExplosion : MonoBehaviour
  {
    public int MaxChecksPerFrame = 10;
    public float ExplosionRadius;
    public float ExplosiveForce;
    private Collider[] hitColliders;
    private int currentIndex;
    private HashSet<Rigidbody> hitRBs = new HashSet<Rigidbody>();

    private void Start() => this.hitColliders = Physics.OverlapSphere(this.transform.position, this.ExplosionRadius);

    private void Update()
    {
      int currentIndex = this.currentIndex;
      if (this.hitColliders == null || this.currentIndex >= this.hitColliders.Length)
        return;
      for (int index = currentIndex; index < Mathf.Min(currentIndex + this.MaxChecksPerFrame, this.hitColliders.Length); ++index)
      {
        if ((Object) this.hitColliders[index] != (Object) null && (Object) this.hitColliders[index].attachedRigidbody != (Object) null && this.hitRBs.Add(this.hitColliders[index].attachedRigidbody))
          this.hitColliders[index].attachedRigidbody.AddExplosionForce(this.ExplosiveForce, this.transform.position, this.ExplosionRadius, 0.2f, ForceMode.Force);
        ++this.currentIndex;
      }
    }
  }
}
