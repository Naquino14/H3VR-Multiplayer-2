using UnityEngine;

namespace FistVR
{
	public class MH_MetalShelf : MonoBehaviour, IMGSpawnIntoAble
	{
		private bool m_canBeenSpawnedInto = true;

		public Transform[] SpawnPositions;

		public bool CanBeSpawnedInto()
		{
			return m_canBeenSpawnedInto;
		}

		public void PlaceObjectInto(GameObject obj)
		{
			int num = Random.Range(0, SpawnPositions.Length);
			obj.transform.position = SpawnPositions[num].position;
			obj.transform.rotation = SpawnPositions[num].rotation;
			m_canBeenSpawnedInto = false;
		}

		public void PlaceObjectInto(GameObject obj, GameObject obj2)
		{
			int num = Random.Range(0, SpawnPositions.Length);
			obj.transform.position = SpawnPositions[num].position;
			obj.transform.rotation = SpawnPositions[num].rotation;
			obj2.transform.position = SpawnPositions[num].position + Vector3.up * 0.3f;
			obj2.transform.rotation = SpawnPositions[num].rotation;
			m_canBeenSpawnedInto = false;
		}

		public void PlaceObjectInto(GameObject obj, GameObject[] objs)
		{
			int num = Random.Range(0, SpawnPositions.Length);
			obj.transform.position = SpawnPositions[num].position;
			obj.transform.rotation = SpawnPositions[num].rotation;
			for (int i = 0; i < objs.Length; i++)
			{
				objs[i].transform.position = SpawnPositions[num].position + Vector3.up * 0.1f * (i + 1);
				objs[i].transform.rotation = SpawnPositions[num].rotation;
			}
			m_canBeenSpawnedInto = false;
		}
	}
}
