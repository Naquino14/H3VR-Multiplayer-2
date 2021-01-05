using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class MM_LootCrate : MonoBehaviour, IFVRDamageable
	{
		public FVRObject m_storedObject1;

		public FVRObject m_storedObject2;

		public FVRObject Object3;

		public FVRObject Object4;

		public Transform[] SpawnPoints;

		public Rigidbody[] Shards;

		private bool m_isDestroyed;

		public GameObject ShatterFX_Prefab;

		public Transform ShatterFX_Point;

		private bool m_hasObjs;

		public List<PowerupType> PowerupTypes_Health;

		public List<PowerupType> PowerupTypes;

		public void Damage(Damage d)
		{
			if (!m_isDestroyed)
			{
				m_isDestroyed = true;
				Destroy();
			}
		}

		public void Init(FVRObject obj1, FVRObject obj2, FVRObject obj3, FVRObject obj4)
		{
			m_storedObject1 = obj1;
			m_storedObject2 = obj2;
			Object3 = obj3;
			Object4 = obj4;
			m_hasObjs = true;
		}

		private void Destroy()
		{
			Object.Instantiate(ShatterFX_Prefab, ShatterFX_Point.position, ShatterFX_Point.rotation);
			for (int i = 0; i < Shards.Length; i++)
			{
				Shards[i].transform.SetParent(null);
				Shards[i].gameObject.SetActive(value: true);
			}
			if (m_hasObjs)
			{
				if (m_storedObject1 != null)
				{
					GameObject gameObject = Object.Instantiate(m_storedObject1.GetGameObject(), SpawnPoints[0].position, SpawnPoints[0].rotation);
					if (m_storedObject1.CompatibleMagazines.Count > 0)
					{
						GameObject gameObject2 = Object.Instantiate(m_storedObject1.GetMagazineWithinCapacity(200).GetGameObject(), SpawnPoints[1].position, SpawnPoints[1].rotation);
						FVRFireArmMagazine component = gameObject2.GetComponent<FVRFireArmMagazine>();
						if (component != null && component.RoundType != FireArmRoundType.aFlameThrowerFuel && component.RoundType != FireArmRoundType.aFlameThrowerFuel)
						{
							component.ReloadMagWithType(AM.GetRandomValidRoundClass(component.RoundType));
						}
					}
					else if (m_storedObject1.CompatibleClips.Count > 0)
					{
						GameObject gameObject3 = Object.Instantiate(m_storedObject1.CompatibleClips[Random.Range(0, m_storedObject1.CompatibleClips.Count)].GetGameObject(), SpawnPoints[1].position, SpawnPoints[1].rotation);
						FVRFireArmClip component2 = gameObject3.GetComponent<FVRFireArmClip>();
						if (component2 != null)
						{
							component2.ReloadClipWithType(AM.GetRandomValidRoundClass(component2.RoundType));
						}
					}
					else if (m_storedObject1.CompatibleSpeedLoaders.Count > 0)
					{
						GameObject gameObject4 = Object.Instantiate(m_storedObject1.CompatibleSpeedLoaders[Random.Range(0, m_storedObject1.CompatibleSpeedLoaders.Count)].GetGameObject(), SpawnPoints[1].position, SpawnPoints[1].rotation);
						Speedloader component3 = gameObject4.GetComponent<Speedloader>();
						if (component3 != null)
						{
							component3.ReloadClipWithType(AM.GetRandomValidRoundClass(component3.Chambers[0].Type));
						}
					}
					else if (m_storedObject1.CompatibleSingleRounds.Count > 0)
					{
						GameObject gameObject5 = Object.Instantiate(m_storedObject1.CompatibleSingleRounds[Random.Range(0, m_storedObject1.CompatibleSingleRounds.Count)].GetGameObject(), SpawnPoints[1].position, SpawnPoints[1].rotation);
					}
				}
				if (m_storedObject2 != null)
				{
					GameObject gameObject6 = Object.Instantiate(m_storedObject2.GetGameObject(), SpawnPoints[2].position, SpawnPoints[2].rotation);
					if (m_storedObject2.RequiredSecondaryPieces.Count > 0)
					{
						GameObject gameObject7 = Object.Instantiate(m_storedObject2.RequiredSecondaryPieces[0].GetGameObject(), SpawnPoints[3].position, SpawnPoints[3].rotation);
					}
				}
				if (Object3 != null)
				{
					GameObject gameObject8 = Object.Instantiate(Object3.GetGameObject(), SpawnPoints[4].position, SpawnPoints[4].rotation);
					RW_Powerup component4 = gameObject8.GetComponent<RW_Powerup>();
					if (component4 != null)
					{
						component4.PowerupType = PowerupTypes_Health[Random.Range(0, PowerupTypes_Health.Count)];
					}
				}
				if (Object4 != null)
				{
					GameObject gameObject9 = Object.Instantiate(Object4.GetGameObject(), SpawnPoints[5].position, SpawnPoints[5].rotation);
					RW_Powerup component5 = gameObject9.GetComponent<RW_Powerup>();
					if (component5 != null)
					{
						component5.PowerupType = PowerupTypes[Random.Range(0, PowerupTypes.Count)];
					}
				}
			}
			Object.Destroy(base.gameObject);
		}
	}
}
