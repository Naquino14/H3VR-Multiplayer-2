// Decompiled with JetBrains decompiler
// Type: DinoFracture.GlueEdgeOnFracture
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace DinoFracture
{
  public class GlueEdgeOnFracture : MonoBehaviour
  {
    public string CollisionTag = string.Empty;
    private int _collisionCount;
    private int _fractureFrame = int.MaxValue;
    private RigidbodyConstraints _rigidBodyConstraints;
    private Vector3 _rigidBodyVelocity;
    private Vector3 _rigidBodyAngularVelocity;
    private Vector3 _impactPoint;
    private Vector3 _impactVelocity;
    private float _impactMass;

    private void OnCollisionEnter(Collision col)
    {
      if (string.IsNullOrEmpty(this.CollisionTag) || col.collider.CompareTag(this.CollisionTag))
      {
        ++this._collisionCount;
        this.enabled = true;
      }
      else
      {
        this._impactMass += !((Object) col.rigidbody != (Object) null) ? 1f : col.rigidbody.mass;
        this._impactVelocity += col.relativeVelocity;
        Vector3 zero = Vector3.zero;
        for (int index = 0; index < col.contacts.Length; ++index)
          zero += col.contacts[index].point;
        this._impactPoint += zero * 1f / (float) col.contacts.Length;
      }
    }

    private void OnTriggerEnter(Collider col)
    {
      if (!string.IsNullOrEmpty(this.CollisionTag) && !col.CompareTag(this.CollisionTag))
        return;
      ++this._collisionCount;
      this.enabled = true;
    }

    private void Update()
    {
      if (this._fractureFrame < int.MaxValue)
      {
        if (Time.frameCount >= this._fractureFrame + 2)
        {
          this.enabled = false;
          this._collisionCount = 0;
          this._fractureFrame = int.MaxValue;
          Rigidbody component = this.GetComponent<Rigidbody>();
          if (!((Object) component != (Object) null))
            return;
          component.constraints = this._rigidBodyConstraints;
          component.angularVelocity = this._rigidBodyAngularVelocity;
          component.velocity = this._rigidBodyVelocity;
          component.WakeUp();
          Vector3 vector3 = this._impactMass * this._impactVelocity / (component.mass + this._impactMass);
          component.AddForceAtPosition(vector3 * component.mass, this._impactPoint, ForceMode.Impulse);
        }
        else
        {
          if (Time.frameCount < this._fractureFrame)
            return;
          this.SetGlued(this._collisionCount > 0);
        }
      }
      else
        this.enabled = false;
    }

    private void OnFracture(OnFractureEventArgs fractureRoot)
    {
      Rigidbody component = this.GetComponent<Rigidbody>();
      if (!((Object) component != (Object) null))
        return;
      component.isKinematic = false;
      this._rigidBodyVelocity = component.velocity;
      this._rigidBodyAngularVelocity = component.angularVelocity;
      this._rigidBodyConstraints = component.constraints;
      component.constraints = RigidbodyConstraints.FreezeAll;
      this._impactPoint = Vector3.zero;
      this._impactVelocity = Vector3.zero;
      this._impactMass = 0.0f;
      this._fractureFrame = Time.frameCount;
      this.enabled = true;
    }

    private void SetGlued(bool glued)
    {
      Rigidbody component = this.GetComponent<Rigidbody>();
      if (!((Object) component != (Object) null))
        return;
      if (glued)
      {
        Object.Destroy((Object) component);
      }
      else
      {
        component.isKinematic = false;
        component.useGravity = true;
      }
    }
  }
}
