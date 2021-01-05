using System.Threading;
using UnityEngine;

namespace DinoFracture
{
	public class TriggerExplosionOnCollision : MonoBehaviour
	{
		public FractureGeometry[] Explosives;

		public float Force;

		public float Radius;

		private void OnCollisionEnter(Collision col)
		{
			AsyncFractureResult[] array = new AsyncFractureResult[Explosives.Length];
			for (int i = 0; i < Explosives.Length; i++)
			{
				if (Explosives[i] != null && Explosives[i].gameObject.activeSelf)
				{
					array[i] = Explosives[i].Fracture();
				}
			}
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j] != null)
				{
					while (!array[j].IsComplete)
					{
						Thread.Sleep(1);
					}
					Explode(array[j].PiecesRoot, array[j].EntireMeshBounds);
				}
			}
		}

		private void Explode(GameObject root, Bounds bounds)
		{
			Vector3 explosionPosition = root.transform.localToWorldMatrix.MultiplyPoint(bounds.center);
			Transform transform = root.transform;
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				Rigidbody component = child.GetComponent<Rigidbody>();
				if (component != null)
				{
					component.AddExplosionForce(Force, explosionPosition, Radius);
				}
			}
		}
	}
}
