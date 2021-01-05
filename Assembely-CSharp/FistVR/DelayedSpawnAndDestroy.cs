using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class DelayedSpawnAndDestroy : MonoBehaviour
	{
		[Serializable]
		public class DestroyAbleGroup
		{
			public bool HasBeenDestroyed;

			public float DestroyAtThisPoint = 1f;

			public List<GameObject> SpawnThese;
		}

		private float m_timeTilDestroy;

		private bool m_isDestroyed;

		public List<DestroyAbleGroup> Groups;

		private void Start()
		{
			m_timeTilDestroy = 0f;
		}

		private void Update()
		{
			m_timeTilDestroy += Time.deltaTime;
			for (int i = 0; i < Groups.Count; i++)
			{
				if (!Groups[i].HasBeenDestroyed && m_timeTilDestroy >= Groups[i].DestroyAtThisPoint)
				{
					Groups[i].HasBeenDestroyed = true;
					for (int j = 0; j < Groups[i].SpawnThese.Count; j++)
					{
						UnityEngine.Object.Instantiate(Groups[i].SpawnThese[j], base.transform.position, base.transform.rotation);
					}
				}
				if (Groups[i].HasBeenDestroyed && i >= Groups.Count - 1)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}
	}
}
