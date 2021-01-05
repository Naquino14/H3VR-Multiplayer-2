// Decompiled with JetBrains decompiler
// Type: DinoFracture.FractureOnCollision
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace DinoFracture
{
  [RequireComponent(typeof (FractureGeometry))]
  public class FractureOnCollision : MonoBehaviour
  {
    public float ForceThreshold;
    public float ForceFalloffRadius = 1f;
    public bool AdjustForKinematic = true;
    private Vector3 _impactVelocity;
    private float _impactMass;
    private Vector3 _impactPoint;
    private Rigidbody _impactBody;

    private void OnCollisionEnter(Collision col)
    {
      if (col.contacts.Length <= 0)
        return;
      this._impactBody = col.rigidbody;
      this._impactMass = !((Object) col.rigidbody != (Object) null) ? 1f : col.rigidbody.mass;
      this._impactVelocity = col.relativeVelocity;
      Rigidbody component = this.GetComponent<Rigidbody>();
      if ((Object) component != (Object) null)
        this._impactVelocity *= Mathf.Sign(Vector3.Dot(component.velocity, this._impactVelocity));
      if ((double) this.ForceThreshold * (double) this.ForceThreshold > (double) (0.5f * this._impactMass * this._impactVelocity * this._impactVelocity.magnitude).sqrMagnitude)
        return;
      this._impactPoint = Vector3.zero;
      for (int index = 0; index < col.contacts.Length; ++index)
        this._impactPoint += col.contacts[index].point;
      this._impactPoint *= 1f / (float) col.contacts.Length;
      Vector3 localPos = this.transform.worldToLocalMatrix.MultiplyPoint(this._impactPoint);
      this.GetComponent<FractureGeometry>().Fracture(localPos);
    }

    private void OnFracture(OnFractureEventArgs args)
    {
      if (!((Object) args.OriginalObject.gameObject == (Object) this.gameObject))
        return;
      float num = this.ForceFalloffRadius * this.ForceFalloffRadius;
      for (int index = 0; index < args.FracturePiecesRootObject.transform.childCount; ++index)
      {
        Transform child = args.FracturePiecesRootObject.transform.GetChild(index);
        Rigidbody component = child.GetComponent<Rigidbody>();
        if ((Object) component != (Object) null)
        {
          Vector3 vector3 = this._impactMass * this._impactVelocity / (component.mass + this._impactMass);
          if ((double) this.ForceFalloffRadius > 0.0)
          {
            float sqrMagnitude = (child.position - this._impactPoint).sqrMagnitude;
            vector3 *= Mathf.Clamp01((float) (1.0 - (double) sqrMagnitude / (double) num));
          }
          component.AddForceAtPosition(vector3 * component.mass, this._impactPoint, ForceMode.Impulse);
        }
      }
      if (!this.AdjustForKinematic)
        return;
      Rigidbody component1 = this.GetComponent<Rigidbody>();
      if (!((Object) component1 != (Object) null) || !component1.isKinematic || !((Object) this._impactBody != (Object) null))
        return;
      this._impactBody.AddForceAtPosition(component1.mass * this._impactVelocity / (component1.mass + this._impactMass) * this._impactMass, this._impactPoint, ForceMode.Impulse);
    }
  }
}
