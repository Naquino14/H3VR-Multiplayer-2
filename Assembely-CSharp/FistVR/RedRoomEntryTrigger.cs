using UnityEngine;

namespace FistVR
{
	public class RedRoomEntryTrigger : MonoBehaviour
	{
		private bool m_hasTriggered;

		public RedRoom Room;

		private void OnTriggerEnter(Collider col)
		{
			if (!m_hasTriggered)
			{
				m_hasTriggered = true;
				Room.TriggerRoom();
			}
		}
	}
}
