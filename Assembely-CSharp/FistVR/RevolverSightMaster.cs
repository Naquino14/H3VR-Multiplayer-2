using UnityEngine;

namespace FistVR
{
	public class RevolverSightMaster : MonoBehaviour
	{
		public FVRFireArm FA;

		public Transform Sight;

		[ContextMenu("Zero")]
		public void Zero()
		{
			Vector3 vector = Sight.position - base.transform.position;
			vector.Normalize();
			vector *= 25f;
			Vector3 worldPosition = base.transform.position + vector;
			FA.MuzzlePos.transform.LookAt(worldPosition, Vector3.up);
		}
	}
}
