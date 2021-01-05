using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class trackCam : MonoBehaviour
	{
		public float speed;

		public bool negative;

		private void Update()
		{
			Vector3 vector = Camera.main.transform.position - base.transform.position;
			if (negative)
			{
				vector = -vector;
			}
			if (speed == 0f)
			{
				base.transform.rotation = Quaternion.LookRotation(vector);
			}
			else
			{
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.LookRotation(vector), speed * Time.deltaTime);
			}
		}
	}
}
