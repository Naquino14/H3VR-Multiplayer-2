using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class MG_4PlinthRoom : MonoBehaviour, IRoomTriggerable
	{
		public RedRoom m_room;

		private bool m_isTriggered;

		private bool m_hasFinished;

		private bool m_hasPickupAGun;

		private bool m_isGreedy;

		private float greedyTick = 60f;

		private int m_numTaken;

		public GameObject[] Plinths;

		public Transform[] SpawnPoints;

		public GameObject Warning1;

		public GameObject Warning2;

		private List<FVRPhysicalObject> SpawnedPOs = new List<FVRPhysicalObject>();

		public Transform[] EnemySpawnPoints;

		private bool m_hasSpawnedBaddies;

		public void Init(int size, RedRoom room)
		{
			m_room = room;
			m_room.CloseDoors(playSound: true);
			SpawnItemsOntoPlinths();
			if (!m_isTriggered)
			{
				m_isTriggered = true;
			}
		}

		public void SetRoom(RedRoom room)
		{
			m_room = room;
		}

		private void Update()
		{
			WeaponCheck();
		}

		private void WeaponCheck()
		{
			if (!m_isTriggered || m_hasFinished)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < SpawnedPOs.Count; i++)
			{
				if (SpawnedPOs[i] != null && !SpawnedPOs[i].RootRigidbody.isKinematic)
				{
					num++;
				}
			}
			if (num > 0 && !m_hasPickupAGun)
			{
				m_hasPickupAGun = true;
			}
			if (m_hasPickupAGun && Warning1.activeSelf)
			{
				Warning1.SetActive(value: false);
				m_room.OpenDoors(playSound: true);
				greedyTick = 10f;
			}
			if (num > 1 && !m_hasSpawnedBaddies)
			{
				m_hasSpawnedBaddies = true;
				m_room.CloseDoors(playSound: true);
				Warning2.SetActive(value: true);
				m_isGreedy = true;
				greedyTick = 30f;
				SpawnSlicer();
				ClearOtherGuns();
			}
			if (m_hasPickupAGun)
			{
				greedyTick -= Time.deltaTime;
				if (greedyTick <= 0f)
				{
					m_room.OpenDoors(playSound: true);
					ClearOtherGuns();
					m_hasFinished = true;
				}
			}
		}

		private void SpawnSlicer()
		{
			Object.Instantiate(GM.MGMaster.SlicerPrefab, SpawnPoints[Random.Range(0, SpawnPoints.Length - 1)].position, Random.rotation);
			GM.MGMaster.Narrator.PlayMonsterRebuilt();
		}

		private void ClearOtherGuns()
		{
			for (int num = SpawnedPOs.Count - 1; num >= 0; num--)
			{
				if (SpawnedPOs[num] != null && SpawnedPOs[num].RootRigidbody.isKinematic)
				{
					Object.Destroy(SpawnedPOs[num].gameObject);
				}
			}
			SpawnedPOs.Clear();
		}

		private void SpawnEnemies()
		{
		}

		private void SpawnItemsOntoPlinths()
		{
			Warning1.SetActive(value: true);
			for (int i = 0; i < Plinths.Length; i++)
			{
				Plinths[i].SetActive(value: true);
			}
			GameObject gameObject = Object.Instantiate(GM.MGMaster.LTEntry_Shotgun2.GetGameObject(), SpawnPoints[0].position, SpawnPoints[0].rotation);
			SpawnedPOs.Add(gameObject.GetComponent<FVRPhysicalObject>());
			SpawnedPOs[0].RootRigidbody.isKinematic = true;
			GameObject gameObject2 = Object.Instantiate(GM.MGMaster.LTEntry_RareGun1.GetGameObject(), SpawnPoints[1].position, SpawnPoints[1].rotation);
			SpawnedPOs.Add(gameObject2.GetComponent<FVRPhysicalObject>());
			SpawnedPOs[1].RootRigidbody.isKinematic = true;
			GameObject gameObject3 = Object.Instantiate(GM.MGMaster.LTEntry_Handgun2.GetGameObject(), SpawnPoints[2].position, SpawnPoints[2].rotation);
			SpawnedPOs.Add(gameObject3.GetComponent<FVRPhysicalObject>());
			SpawnedPOs[2].RootRigidbody.isKinematic = true;
			GameObject gameObject4 = Object.Instantiate(GM.MGMaster.LTEntry_RareGun2.GetGameObject(), SpawnPoints[3].position, SpawnPoints[3].rotation);
			SpawnedPOs.Add(gameObject4.GetComponent<FVRPhysicalObject>());
			SpawnedPOs[3].RootRigidbody.isKinematic = true;
		}
	}
}
