using UnityEngine;

namespace FistVR
{
	public class FVRBullshotCamera : MonoBehaviour
	{
		public Transform CameraTarget;

		private Vector3 filteredPos;

		private Quaternion filteredRot;

		private void Update()
		{
			if (CameraTarget != null)
			{
				float num = Mathf.Min(Vector3.Distance(filteredPos, CameraTarget.position) / 0.015f, 1f);
				filteredPos = (1f - num) * filteredPos + num * CameraTarget.position;
				float t = Mathf.Min(Quaternion.Angle(filteredRot, CameraTarget.rotation) / 5f, 1f);
				filteredRot = Quaternion.Slerp(filteredRot, CameraTarget.rotation, t);
				base.transform.position = Vector3.Lerp(base.transform.position, filteredPos, Time.deltaTime * 3f);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, filteredRot, Time.deltaTime * 3f);
			}
		}
	}
}
