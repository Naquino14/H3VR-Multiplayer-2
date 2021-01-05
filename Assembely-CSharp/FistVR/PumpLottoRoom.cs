using UnityEngine;

namespace FistVR
{
	public class PumpLottoRoom : MonoBehaviour, IRoomTriggerable
	{
		public RedRoom m_room;

		private bool m_isTriggered;

		public MR_PumpLottoBase[] Bases;

		public void Init(int size, RedRoom room)
		{
			m_room = room;
			if (!m_isTriggered)
			{
				m_isTriggered = true;
			}
			m_room.CloseDoors(playSound: true);
			for (int i = 0; i < Bases.Length; i++)
			{
				Bases[i].gameObject.SetActive(value: true);
			}
		}

		public void SetRoom(RedRoom room)
		{
			m_room = room;
			SetUpPumps();
		}

		public void SetUpPumps()
		{
			ShuffleBases();
			Bases[0].SetPLType(MR_PumpLottoBase.PumpLottoType.OpenDoor);
			float num = Random.Range(0f, 1f);
			if (num > 0.95f)
			{
				Bases[1].SetPLType(MR_PumpLottoBase.PumpLottoType.WeinerBot);
			}
			else if (num > 0.5f)
			{
				Bases[1].SetPLType(MR_PumpLottoBase.PumpLottoType.Loot);
			}
			else
			{
				Bases[1].SetPLType(MR_PumpLottoBase.PumpLottoType.CloseDoor);
			}
			num = Random.Range(0f, 1f);
			if (num > 0.8f)
			{
				Bases[2].SetPLType(MR_PumpLottoBase.PumpLottoType.WeinerBot);
			}
			else if (num > 0.3f)
			{
				Bases[2].SetPLType(MR_PumpLottoBase.PumpLottoType.Loot);
			}
			else
			{
				Bases[2].SetPLType(MR_PumpLottoBase.PumpLottoType.Goof);
			}
			num = Random.Range(0f, 1f);
			if (num > 0.8f)
			{
				Bases[3].SetPLType(MR_PumpLottoBase.PumpLottoType.Slicer);
			}
			else if (num > 0.3f)
			{
				Bases[3].SetPLType(MR_PumpLottoBase.PumpLottoType.Loot);
			}
			else
			{
				Bases[3].SetPLType(MR_PumpLottoBase.PumpLottoType.Goof);
			}
			for (int i = 0; i < Bases.Length; i++)
			{
				Bases[i].gameObject.SetActive(value: false);
			}
		}

		private void ShuffleBases()
		{
			for (int i = 0; i < Bases.Length; i++)
			{
				MR_PumpLottoBase mR_PumpLottoBase = Bases[i];
				int num = Random.Range(i, Bases.Length);
				Bases[i] = Bases[num];
				Bases[num] = mR_PumpLottoBase;
			}
		}
	}
}
