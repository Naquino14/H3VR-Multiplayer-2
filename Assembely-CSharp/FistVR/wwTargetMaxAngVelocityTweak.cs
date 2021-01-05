using UnityEngine;

namespace FistVR
{
	public class wwTargetMaxAngVelocityTweak : MonoBehaviour
	{
		private void Awake()
		{
			GetComponent<Rigidbody>().maxAngularVelocity = 25f;
		}
	}
}
