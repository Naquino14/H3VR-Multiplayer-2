using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class MG_Cabinet : MonoBehaviour, IMGSpawnIntoAble
	{
		public List<MG_CabinetDrawer> Drawers;

		private List<MG_CabinetDrawer> SpawnedDrawers;

		private bool m_canBeenSpawnedInto = true;

		public bool CanBeSpawnedInto()
		{
			return m_canBeenSpawnedInto;
		}

		public void PlaceObjectInto(GameObject obj)
		{
			int index = Random.Range(0, Drawers.Count);
			Drawers[index].SpawnIntoCabinet(obj);
			Drawers.RemoveAt(index);
			if (Drawers.Count <= 0)
			{
				m_canBeenSpawnedInto = false;
			}
		}

		public void PlaceObjectInto(GameObject[] objs)
		{
			int index = Random.Range(0, Drawers.Count);
			Drawers[index].SpawnIntoCabinet(objs);
			Drawers.RemoveAt(index);
			if (Drawers.Count <= 0)
			{
				m_canBeenSpawnedInto = false;
			}
		}

		public void Init()
		{
			for (int i = 0; i < Drawers.Count; i++)
			{
				Drawers[i].Init();
			}
		}

		public Transform GetRandomSpawnTransform()
		{
			return Drawers[Random.Range(0, Drawers.Count)].ItemPoint;
		}
	}
}
