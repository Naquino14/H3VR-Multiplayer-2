using UnityEngine;

namespace FistVR
{
	public class MR_BoardBreaker : MonoBehaviour, IRoomTriggerable, IMeatRoomAble
	{
		public MG_MeatChunk MeatChunk;

		private RedRoom m_room;

		private int m_MeatID;

		private bool m_hasBeenTriggered;

		private void Awake()
		{
		}

		public void SetRoom(RedRoom room)
		{
			m_room = room;
		}

		public void Init(int roomTileSize, RedRoom room)
		{
			m_room = room;
			m_hasBeenTriggered = true;
			GM.MGMaster.Narrator.PlayMeatDiscover(m_MeatID);
		}

		public void SetMeatID(int i)
		{
			m_MeatID = i;
			MeatChunk.MeatID = i;
		}
	}
}
