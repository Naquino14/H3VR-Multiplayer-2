using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class WarehouseRangeMainPanel : MonoBehaviour
	{
		public GameObject Targets;

		private GameObject m_curTargets;

		public Transform[] SlicerSpawns;

		public GameObject SlicerPrefab;

		public List<GameObject> CurSlicers = new List<GameObject>();

		private void Awake()
		{
			Targets.SetActive(value: false);
			m_curTargets = Object.Instantiate(Targets, Targets.transform.position, Targets.transform.rotation);
			m_curTargets.SetActive(value: true);
		}

		public void ResetTargets()
		{
			Object.Destroy(m_curTargets);
			m_curTargets = Object.Instantiate(Targets, Targets.transform.position, Targets.transform.rotation);
			m_curTargets.SetActive(value: true);
		}

		public void SpawnSlicerAttack()
		{
			if (CurSlicers.Count > 0)
			{
				for (int num = CurSlicers.Count - 1; num >= 0; num--)
				{
					if (CurSlicers[num] == null)
					{
						CurSlicers.RemoveAt(num);
					}
				}
			}
			if (CurSlicers.Count < 4)
			{
				for (int i = 0; i < 3; i++)
				{
					int num2 = Random.Range(0, SlicerSpawns.Length);
					GameObject item = Object.Instantiate(SlicerPrefab, SlicerSpawns[num2].position + Random.onUnitSphere * 2f, Random.rotation);
					CurSlicers.Add(item);
				}
			}
		}

		public void SpawnSingleSlicer()
		{
			if (CurSlicers.Count < 4)
			{
				int num = Random.Range(0, SlicerSpawns.Length);
				GameObject item = Object.Instantiate(SlicerPrefab, SlicerSpawns[num].position + Random.onUnitSphere * 2f, Random.rotation);
				CurSlicers.Add(item);
			}
		}

		public void KillAllSlicers()
		{
			if (CurSlicers.Count > 0)
			{
				for (int num = CurSlicers.Count - 1; num >= 0; num--)
				{
					if (CurSlicers[num] != null)
					{
						Object.Destroy(CurSlicers[num]);
						CurSlicers.RemoveAt(num);
					}
				}
			}
			CurSlicers.Clear();
		}
	}
}
