using UnityEngine;

namespace DinoFracture
{
	[RequireComponent(typeof(FractureGeometry))]
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
			{
				return;
			}
			_impactBody = col.rigidbody;
			_impactMass = ((!(col.rigidbody != null)) ? 1f : col.rigidbody.mass);
			_impactVelocity = col.relativeVelocity;
			Rigidbody component = GetComponent<Rigidbody>();
			if (component != null)
			{
				_impactVelocity *= Mathf.Sign(Vector3.Dot(component.velocity, _impactVelocity));
			}
			float magnitude = _impactVelocity.magnitude;
			Vector3 vector = 0.5f * _impactMass * _impactVelocity * magnitude;
			if (ForceThreshold * ForceThreshold <= vector.sqrMagnitude)
			{
				_impactPoint = Vector3.zero;
				for (int i = 0; i < col.contacts.Length; i++)
				{
					_impactPoint += col.contacts[i].point;
				}
				_impactPoint *= 1f / (float)col.contacts.Length;
				Vector3 localPos = base.transform.worldToLocalMatrix.MultiplyPoint(_impactPoint);
				GetComponent<FractureGeometry>().Fracture(localPos);
			}
		}

		private void OnFracture(OnFractureEventArgs args)
		{
			if (!(args.OriginalObject.gameObject == base.gameObject))
			{
				return;
			}
			float num = ForceFalloffRadius * ForceFalloffRadius;
			for (int i = 0; i < args.FracturePiecesRootObject.transform.childCount; i++)
			{
				Transform child = args.FracturePiecesRootObject.transform.GetChild(i);
				Rigidbody component = child.GetComponent<Rigidbody>();
				if (component != null)
				{
					Vector3 vector = _impactMass * _impactVelocity / (component.mass + _impactMass);
					if (ForceFalloffRadius > 0f)
					{
						float sqrMagnitude = (child.position - _impactPoint).sqrMagnitude;
						vector *= Mathf.Clamp01(1f - sqrMagnitude / num);
					}
					component.AddForceAtPosition(vector * component.mass, _impactPoint, ForceMode.Impulse);
				}
			}
			if (AdjustForKinematic)
			{
				Rigidbody component2 = GetComponent<Rigidbody>();
				if (component2 != null && component2.isKinematic && _impactBody != null)
				{
					Vector3 vector2 = component2.mass * _impactVelocity / (component2.mass + _impactMass);
					_impactBody.AddForceAtPosition(vector2 * _impactMass, _impactPoint, ForceMode.Impulse);
				}
			}
		}
	}
}
