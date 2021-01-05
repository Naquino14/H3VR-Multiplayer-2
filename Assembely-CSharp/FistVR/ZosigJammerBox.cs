using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ZosigJammerBox : MonoBehaviour, IFVRDamageable
	{
		public enum JammerBoxState
		{
			Functioning,
			Destroyed
		}

		public List<GameObject> Prefab_OnDestroy;

		public Transform DestroySpawnPoint;

		public GameObject Obj_Undestroyed;

		public GameObject Obj_Destroyed;

		public JammerBoxState BState;

		private float m_lifeLeft = 3000f;

		private void Start()
		{
		}

		private void Update()
		{
		}

		public void Damage(Damage d)
		{
			if (BState != JammerBoxState.Destroyed)
			{
				m_lifeLeft -= d.Dam_TotalEnergetic + d.Dam_TotalKinetic;
				if (m_lifeLeft <= 0f)
				{
					Splode();
				}
			}
		}

		private void Splode()
		{
			SetDestroyed();
			for (int i = 0; i < Prefab_OnDestroy.Count; i++)
			{
				Object.Instantiate(Prefab_OnDestroy[i], DestroySpawnPoint.position, DestroySpawnPoint.rotation);
			}
		}

		public void SetDestroyed()
		{
			BState = JammerBoxState.Destroyed;
			Obj_Destroyed.SetActive(value: true);
			Obj_Undestroyed.SetActive(value: false);
		}
	}
}
