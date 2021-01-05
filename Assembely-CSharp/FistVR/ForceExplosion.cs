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

		private void Start()
		{
			hitColliders = Physics.OverlapSphere(base.transform.position, ExplosionRadius);
		}

		private void Update()
		{
			int num = currentIndex;
			if (hitColliders == null || currentIndex >= hitColliders.Length)
			{
				return;
			}
			for (int i = num; i < Mathf.Min(num + MaxChecksPerFrame, hitColliders.Length); i++)
			{
				if (hitColliders[i] != null && hitColliders[i].attachedRigidbody != null && hitRBs.Add(hitColliders[i].attachedRigidbody))
				{
					hitColliders[i].attachedRigidbody.AddExplosionForce(ExplosiveForce, base.transform.position, ExplosionRadius, 0.2f, ForceMode.Force);
				}
				currentIndex++;
			}
		}
	}
}
