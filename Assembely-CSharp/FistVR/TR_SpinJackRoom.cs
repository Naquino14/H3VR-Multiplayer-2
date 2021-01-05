using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class TR_SpinJackRoom : MonoBehaviour, IRoomTriggerable
	{
		private RedRoom m_room;

		public GameObject[] SpinJackPrefabs;

		public AlloyAreaLight MyLight;

		private float curIntensity;

		private float tarIntensity;

		private bool m_isTriggered;

		private bool m_isDeactivated;

		public Transform[] JackSpawns_Size2;

		public Transform[] JackSpawns_Size3;

		public Transform[] JackSpawns_Size4;

		private List<TR_SpinJack> m_spawnedJacks = new List<TR_SpinJack>();

		public void Start()
		{
		}

		public void SetRoom(RedRoom room)
		{
			m_room = room;
		}

		public void Init(int roomTileSize, RedRoom room)
		{
			if (room != null)
			{
				m_room = room;
			}
			GM.MGMaster.Narrator.PlayTrapRoomInit();
			GetComponent<AudioSource>().Play();
			if (m_room != null)
			{
				m_room.CloseDoors(playSound: true);
			}
			m_isTriggered = true;
			MyLight.gameObject.SetActive(value: true);
			tarIntensity = 0.75f;
			Transform[] array = null;
			switch (roomTileSize)
			{
			case 2:
				array = JackSpawns_Size2;
				break;
			case 3:
				array = JackSpawns_Size3;
				break;
			case 4:
				array = JackSpawns_Size4;
				break;
			}
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = Object.Instantiate(SpinJackPrefabs[i], array[i].position, array[i].rotation);
				TR_SpinJack component = gameObject.GetComponent<TR_SpinJack>();
				m_spawnedJacks.Add(component);
			}
		}

		public void Update()
		{
			if (m_isTriggered)
			{
				if (curIntensity < tarIntensity)
				{
					curIntensity += Time.deltaTime * 0.3f;
					MyLight.Intensity = curIntensity;
				}
			}
			else if (m_isDeactivated)
			{
				if (curIntensity > tarIntensity)
				{
					curIntensity -= Time.deltaTime * 0.2f;
					MyLight.Intensity = curIntensity;
				}
				else
				{
					MyLight.gameObject.SetActive(value: false);
				}
			}
			for (int num = m_spawnedJacks.Count - 1; num >= 0; num--)
			{
				if (m_spawnedJacks[num] == null)
				{
					m_spawnedJacks.RemoveAt(num);
				}
			}
			if (m_isTriggered && m_spawnedJacks.Count <= 0 && !m_isDeactivated)
			{
				TrapOver();
			}
		}

		private void TrapOver()
		{
			Debug.Log("trap over");
			m_isDeactivated = true;
			m_room.OpenDoors(playSound: true);
		}
	}
}
