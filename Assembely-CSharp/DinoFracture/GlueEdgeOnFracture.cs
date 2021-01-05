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
			if (string.IsNullOrEmpty(CollisionTag) || col.collider.CompareTag(CollisionTag))
			{
				_collisionCount++;
				base.enabled = true;
				return;
			}
			_impactMass += ((!(col.rigidbody != null)) ? 1f : col.rigidbody.mass);
			_impactVelocity += col.relativeVelocity;
			Vector3 zero = Vector3.zero;
			for (int i = 0; i < col.contacts.Length; i++)
			{
				zero += col.contacts[i].point;
			}
			_impactPoint += zero * 1f / col.contacts.Length;
		}

		private void OnTriggerEnter(Collider col)
		{
			if (string.IsNullOrEmpty(CollisionTag) || col.CompareTag(CollisionTag))
			{
				_collisionCount++;
				base.enabled = true;
			}
		}

		private void Update()
		{
			if (_fractureFrame < int.MaxValue)
			{
				if (Time.frameCount >= _fractureFrame + 2)
				{
					base.enabled = false;
					_collisionCount = 0;
					_fractureFrame = int.MaxValue;
					Rigidbody component = GetComponent<Rigidbody>();
					if (component != null)
					{
						component.constraints = _rigidBodyConstraints;
						component.angularVelocity = _rigidBodyAngularVelocity;
						component.velocity = _rigidBodyVelocity;
						component.WakeUp();
						Vector3 vector = _impactMass * _impactVelocity / (component.mass + _impactMass);
						component.AddForceAtPosition(vector * component.mass, _impactPoint, ForceMode.Impulse);
					}
				}
				else if (Time.frameCount >= _fractureFrame)
				{
					SetGlued(_collisionCount > 0);
				}
			}
			else
			{
				base.enabled = false;
			}
		}

		private void OnFracture(OnFractureEventArgs fractureRoot)
		{
			Rigidbody component = GetComponent<Rigidbody>();
			if (component != null)
			{
				component.isKinematic = false;
				_rigidBodyVelocity = component.velocity;
				_rigidBodyAngularVelocity = component.angularVelocity;
				_rigidBodyConstraints = component.constraints;
				component.constraints = RigidbodyConstraints.FreezeAll;
				_impactPoint = Vector3.zero;
				_impactVelocity = Vector3.zero;
				_impactMass = 0f;
				_fractureFrame = Time.frameCount;
				base.enabled = true;
			}
		}

		private void SetGlued(bool glued)
		{
			Rigidbody component = GetComponent<Rigidbody>();
			if (component != null)
			{
				if (glued)
				{
					Object.Destroy(component);
					return;
				}
				component.isKinematic = false;
				component.useGravity = true;
			}
		}
	}
}
