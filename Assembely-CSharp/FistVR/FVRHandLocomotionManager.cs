using UnityEngine;

namespace FistVR
{
	public class FVRHandLocomotionManager : MonoBehaviour
	{
		public bool DoesFall;

		public float FallSpeed = 1f;

		public FVRHandGrabPoint ActiveGrabPoint;

		private Vector2 clampX = Vector2.zero;

		private Vector2 clampY = Vector2.zero;

		private Vector2 clampZ = Vector2.zero;

		private Rigidbody rb;

		public void Awake()
		{
			rb = GetComponent<Rigidbody>();
		}

		public void NewGrab(FVRHandGrabPoint grabpoint)
		{
			ActiveGrabPoint = grabpoint;
		}

		public void EndGrab(FVRHandGrabPoint grabpoint)
		{
			if (ActiveGrabPoint == grabpoint)
			{
				ActiveGrabPoint = null;
			}
		}

		public void Move(FVRHandGrabPoint grabpoint, Vector3 dir)
		{
			if (ActiveGrabPoint == null)
			{
				ActiveGrabPoint = grabpoint;
			}
			if (ActiveGrabPoint == grabpoint)
			{
				Vector3 position = base.transform.position;
				if (position.x + dir.x >= clampX.x && position.x + dir.x <= clampX.y)
				{
					position.x += dir.x;
				}
				else if (position.x + dir.x < clampX.x)
				{
					position.x += dir.magnitude;
					position.x = Mathf.Min(position.x, clampX.x);
				}
				else if (position.x + dir.x > clampX.y)
				{
					position.x -= dir.magnitude;
					position.x = Mathf.Max(position.x, clampX.y);
				}
				if (position.y + dir.y >= clampY.x && position.y + dir.y <= clampY.y)
				{
					position.y += dir.y;
				}
				else if (position.y + dir.y < clampY.x)
				{
					position.y += Mathf.Abs(dir.y);
				}
				else if (position.y + dir.y > clampY.y)
				{
					position.y -= Mathf.Abs(dir.y);
				}
				if (position.z + dir.z >= clampZ.x && position.z + dir.z <= clampZ.y)
				{
					position.z += dir.z;
				}
				else if (position.z + dir.z < clampZ.x)
				{
					position.z += Mathf.Abs(dir.z);
				}
				else if (position.z + dir.z > clampZ.y)
				{
					position.z -= Mathf.Abs(dir.z);
				}
				rb.position = position;
			}
		}
	}
}
