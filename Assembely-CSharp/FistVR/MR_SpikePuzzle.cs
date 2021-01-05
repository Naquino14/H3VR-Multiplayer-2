using UnityEngine;

namespace FistVR
{
	public class MR_SpikePuzzle : MonoBehaviour, IRoomTriggerable, IMeatRoomAble
	{
		public MG_MeatChunk MeatChunk;

		public Transform[] Spikes;

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

		public void UpdatePuzzle()
		{
			if (m_HasMeatBeenFreed)
			{
				return;
			}
			bool flag = true;
			for (int i = 0; i < Spikes.Length; i++)
			{
				if (Spikes[i].transform.localPosition.z > -0.2f)
				{
					flag = false;
				}
			}
			if (flag)
			{
				m_HasMeatBeenFreed = true;
				MeatChunk.CanMeatBePickedUp = true;
				GM.MGMaster.Narrator.PlayMeatAcquire(m_MeatID);
			}
		}
	}
}
