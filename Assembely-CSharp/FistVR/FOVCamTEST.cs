using UnityEngine;

namespace FistVR
{
	public class FOVCamTEST : MonoBehaviour
	{
		public Camera cam;

		private void Start()
		{
		}

		private void Update()
		{
			Vector3 vector = cam.transform.InverseTransformPoint(GM.CurrentPlayerBody.Head.position);
		}
	}
}
