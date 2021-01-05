using UnityEngine;

namespace FistVR
{
	public class D6Room : MonoBehaviour, IRoomTriggerable
	{
		public RedRoom m_room;

		private bool m_isTriggered;

		public GameObject Plinth;

		public MG_D6Lotto D6;

		public void Init(int size, RedRoom room)
		{
			m_room = room;
			D6.m_room = room;
			if (!m_isTriggered)
			{
				m_isTriggered = true;
			}
			m_room.CloseDoors(playSound: true);
			Plinth.SetActive(value: true);
			D6.gameObject.SetActive(value: true);
		}

		public void SetRoom(RedRoom room)
		{
			m_room = room;
		}
	}
}
