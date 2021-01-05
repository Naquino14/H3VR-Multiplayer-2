using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ZosigContainer_Destoyable : ZosigContainer, IFVRDamageable
	{
		public List<Rigidbody> Shards;

		private bool m_hasShattered;

		public List<Transform> SpawnPoints;

		public float ChanceEmpty = 0.4f;

		public float ChanceDynamicAmmoSpawn = 0.05f;

		public AudioEvent AudEvent_Shatter;

		private float m_sinceSpawn;

		public void Update()
		{
			if (m_sinceSpawn < 5f)
			{
				m_sinceSpawn += Time.deltaTime;
			}
		}

		public override void PlaceObjectsInContainer(FVRObject obj1, int minAmmo = -1, int maxAmmo = 30)
		{
			m_storedObject1 = obj1;
			base.PlaceObjectsInContainer(obj1);
		}

		public void Damage(Damage d)
		{
			if (d.Dam_TotalKinetic > 20f)
			{
				SpawnItemsAndShatter();
			}
		}

		private void OnCollisionEnter(Collision col)
		{
			if (col.relativeVelocity.magnitude > 5f && m_sinceSpawn > 3f)
			{
				SpawnItemsAndShatter();
			}
		}

		private void SpawnItemsAndShatter()
		{
			if (!m_containsItems || m_hasShattered)
			{
				return;
			}
			m_hasShattered = true;
			if (GM.ZMaster != null)
			{
				GM.ZMaster.FlagM.AddToFlag("s_l", 1);
			}
			FlagOpen();
			for (int i = 0; i < Shards.Count; i++)
			{
				Shards[i].transform.SetParent(null);
			}
			SM.PlayGenericSound(AudEvent_Shatter, base.transform.position);
			float num = Random.Range(0f, 1f);
			if (num > ChanceEmpty)
			{
				float num2 = Random.Range(0f, 1f);
				bool flag = false;
				if (num2 <= ChanceDynamicAmmoSpawn)
				{
					FVRObject randomEquippedFirearm = GM.ZMaster.GetRandomEquippedFirearm();
					if (randomEquippedFirearm != null)
					{
						if (randomEquippedFirearm.CompatibleMagazines.Count > 0)
						{
							GameObject gameObject = Object.Instantiate(randomEquippedFirearm.CompatibleMagazines[Random.Range(0, randomEquippedFirearm.CompatibleMagazines.Count)].GetGameObject(), SpawnPoints[0].position, SpawnPoints[0].rotation);
							flag = true;
						}
						else if (randomEquippedFirearm.CompatibleClips.Count > 0)
						{
							GameObject gameObject2 = Object.Instantiate(randomEquippedFirearm.CompatibleClips[Random.Range(0, randomEquippedFirearm.CompatibleMagazines.Count)].GetGameObject(), SpawnPoints[0].position, SpawnPoints[0].rotation);
							flag = true;
						}
						else if (randomEquippedFirearm.CompatibleClips.Count > 0)
						{
							GameObject gameObject3 = Object.Instantiate(randomEquippedFirearm.CompatibleClips[Random.Range(0, randomEquippedFirearm.CompatibleMagazines.Count)].GetGameObject(), SpawnPoints[0].position, SpawnPoints[0].rotation);
							flag = true;
						}
					}
				}
				if (!flag && m_storedObject1 != null)
				{
					GameObject gameObject4 = Object.Instantiate(m_storedObject1.GetGameObject(), SpawnPoints[0].position, SpawnPoints[0].rotation);
					if (m_storedObject1.RequiredSecondaryPieces.Count > 0)
					{
						GameObject gameObject5 = Object.Instantiate(m_storedObject1.RequiredSecondaryPieces[0].GetGameObject(), SpawnPoints[1].position, SpawnPoints[1].rotation);
					}
				}
			}
			for (int j = 0; j < Shards.Count; j++)
			{
				Shards[j].gameObject.SetActive(value: true);
			}
			Object.Destroy(base.gameObject);
		}
	}
}
