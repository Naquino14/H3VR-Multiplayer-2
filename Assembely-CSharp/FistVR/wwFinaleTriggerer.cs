using UnityEngine;

namespace FistVR
{
	public class wwFinaleTriggerer : MonoBehaviour
	{
		public wwFinaleManager FManager;

		private void OnTriggerEnter(Collider col)
		{
			FManager.BeginEnding();
		}
	}
}
