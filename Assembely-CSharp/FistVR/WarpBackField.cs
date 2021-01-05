using UnityEngine;

namespace FistVR
{
	public class WarpBackField : MonoBehaviour
	{
		public Transform WarpPoint;

		public AudioEvent AudEvent_TP;

		private void OnTriggerEnter(Collider other)
		{
			GM.CurrentMovementManager.TeleportToPoint(WarpPoint.position, isAbsolute: true, WarpPoint.forward);
			SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_TP, WarpPoint.position);
		}
	}
}
