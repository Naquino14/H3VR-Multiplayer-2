// Decompiled with JetBrains decompiler
// Type: FistVR.LawnDart
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class LawnDart : FVRPhysicalObject
  {
    public ParticleSystem[] TrailSystems;
    public LawnDartGame Game;
    public Transform DartPoint;
    public LayerMask CastingMask;
    private RaycastHit m_hit;

    public void SetGame(LawnDartGame g) => this.Game = g;

    protected override void FVRFixedUpdate()
    {
      base.FVRFixedUpdate();
      if (this.IsHeld || this.RootRigidbody.isKinematic || (double) this.RootRigidbody.velocity.magnitude <= 2.0)
        return;
      foreach (ParticleSystem trailSystem in this.TrailSystems)
        trailSystem.Emit(1);
      this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(this.RootRigidbody.velocity.normalized, Vector3.up), Time.deltaTime * 4.5f);
    }

    public override void OnCollisionEnter(Collision col)
    {
      base.OnCollisionEnter(col);
      if (this.IsHeld || !((Object) this.QuickbeltSlot == (Object) null) || (double) this.transform.position.y >= 20.0)
        return;
      this.RootRigidbody.isKinematic = true;
      this.Scorecast();
    }

    private void Scorecast()
    {
      if ((double) this.transform.position.y >= 20.0 || !Physics.Raycast(this.DartPoint.position + Vector3.up * 5f, -Vector3.up, out this.m_hit, 100f, (int) this.CastingMask))
        return;
      if ((bool) (Object) this.m_hit.collider.gameObject.GetComponent<LawnDartPointCollider>())
      {
        LawnDartPointCollider component = this.m_hit.collider.gameObject.GetComponent<LawnDartPointCollider>();
        this.Game.ScoreEvent(this.DartPoint.position, component.SpecialDisplay, component.Points, component.Multiplier, this);
      }
      else
        this.Game.ScoreEvent(this.DartPoint.position, "Miss!", 0, 0, this);
    }
  }
}
