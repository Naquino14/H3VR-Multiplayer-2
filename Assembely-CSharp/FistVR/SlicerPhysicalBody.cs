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
			if (base.IsHeld)
			{
				tarThrusterForce = Computer.CurrentThrusterForce * (1.5f + Mathf.PerlinNoise(base.transform.position.x, Time.time * 5f) * 0.5f);
				tarThrusterForce += Random.onUnitSphere * 0.5f;
				curThrusterForce = Vector3.Lerp(curThrusterForce, tarThrusterForce, Time.deltaTime * 4f);
				base.RootRigidbody.velocity += curThrusterForce;
				base.RootRigidbody.angularVelocity += curThrusterForce * 3f;
			}
		}
	}
}
