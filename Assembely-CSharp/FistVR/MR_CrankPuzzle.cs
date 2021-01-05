using UnityEngine;

namespace FistVR
{
	public class MR_CrankPuzzle : MonoBehaviour, IRoomTriggerable, IMeatRoomAble
	{
		public MG_MeatChunk MeatChunk;

		private bool m_HasMeatBeenFreed;

		private RedRoom m_room;

		private float checkLightDistanceTick;

		private int m_MeatID;

		private bool m_hasBeenTriggered;

		private void Awake()
		{
			MeatChunk.transform.localRotation = Random.rotation;
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
