using UnityEngine;

namespace FistVR
{
	public class BreakActionForeFollower : MonoBehaviour
	{
		public Transform FollowThis;

		private void Update()
		{
			base.transform.localRotation = FollowThis.localRotation;
		}
	}
}
