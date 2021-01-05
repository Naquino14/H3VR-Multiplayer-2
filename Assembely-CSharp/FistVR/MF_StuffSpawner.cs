using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class MF_StuffSpawner : MonoBehaviour
	{
		public List<FVRObject> Equipment;

		public List<FVRObject> Equipment_Secondaries;

		public List<Transform> SpawnPoints;

		public List<Transform> SpawnPoints_Secondaries;

		public void SpawnEquipmentPiece(int i)
		{
			if (Equipment.Count > i)
			{
				Object.Instantiate(Equipment[i].GetGameObject(), SpawnPoints[i].position, SpawnPoints[i].rotation);
			}
			if (Equipment_Secondaries.Count > i)
			{
				Object.Instantiate(Equipment_Secondaries[i].GetGameObject(), SpawnPoints_Secondaries[i].position, SpawnPoints_Secondaries[i].rotation);
			}
		}
	}
}
