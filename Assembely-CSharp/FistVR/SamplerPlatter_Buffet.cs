using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class SamplerPlatter_Buffet : MonoBehaviour
	{
		public List<FVRPhysicalObject> objs;

		public List<FVRObject> Prefabs;

		public List<Transform> SpawnPoints;

		private List<FVRPhysicalObject> m_spawnedObjects = new List<FVRPhysicalObject>();

		private void Start()
		{
			InitialSpawn();
		}

		private void InitialSpawn()
		{
			for (int i = 0; i < Prefabs.Count; i++)
			{
				GameObject gameObject = Object.Instantiate(Prefabs[i].GetGameObject(), SpawnPoints[i].position, SpawnPoints[i].rotation);
				FVRPhysicalObject component = gameObject.GetComponent<FVRPhysicalObject>();
				m_spawnedObjects.Add(component);
			}
		}

		public void ResetBuffet()
		{
			for (int i = 0; i < m_spawnedObjects.Count; i++)
			{
				if (m_spawnedObjects[i] == null)
				{
					RespawnIndex(i);
					continue;
				}
				FVRPhysicalObject fVRPhysicalObject = m_spawnedObjects[i];
				if (fVRPhysicalObject.IsHeld || fVRPhysicalObject.QuickbeltSlot != null)
				{
					continue;
				}
				if (fVRPhysicalObject is FVRFireArmMagazine)
				{
					if (!(fVRPhysicalObject as FVRFireArmMagazine).HasARound())
					{
						FVRPhysicalObject fVRPhysicalObject2 = fVRPhysicalObject;
						fVRPhysicalObject = RespawnIndex(i);
						Object.Destroy(fVRPhysicalObject2.gameObject);
					}
				}
				else if (fVRPhysicalObject is FVRFireArmAttachment)
				{
					if ((fVRPhysicalObject as FVRFireArmAttachment).curMount == null)
					{
						float num = Vector3.Distance(m_spawnedObjects[i].transform.position, GM.CurrentPlayerBody.transform.position);
						if (num > 6f)
						{
							MoveToIndex(i);
						}
					}
				}
				else if (fVRPhysicalObject is FVRStrikeAnyWhereMatch || fVRPhysicalObject is Speedloader || fVRPhysicalObject is FVRFireArmClip)
				{
					FVRPhysicalObject fVRPhysicalObject3 = fVRPhysicalObject;
					fVRPhysicalObject = RespawnIndex(i);
					Object.Destroy(fVRPhysicalObject3.gameObject);
				}
				else
				{
					float num2 = Vector3.Distance(m_spawnedObjects[i].transform.position, GM.CurrentPlayerBody.transform.position);
					if (num2 > 6f)
					{
						MoveToIndex(i);
					}
				}
			}
		}

		private FVRPhysicalObject RespawnIndex(int i)
		{
			GameObject gameObject = Object.Instantiate(Prefabs[i].GetGameObject(), SpawnPoints[i].position, SpawnPoints[i].rotation);
			FVRPhysicalObject component = gameObject.GetComponent<FVRPhysicalObject>();
			m_spawnedObjects[i] = component;
			return component;
		}

		private void MoveToIndex(int i)
		{
			m_spawnedObjects[i].transform.position = SpawnPoints[i].position;
			m_spawnedObjects[i].transform.rotation = SpawnPoints[i].rotation;
			m_spawnedObjects[i].RootRigidbody.velocity = Vector3.zero;
			m_spawnedObjects[i].RootRigidbody.angularVelocity = Vector3.zero;
		}

		[ContextMenu("Migrate")]
		public void Migrate()
		{
			for (int i = 0; i < objs.Count; i++)
			{
				Prefabs[i] = objs[i].ObjectWrapper;
				Transform transform = new GameObject().transform;
				transform.gameObject.name = "_SpawnPoint" + objs[i].gameObject.name;
				transform.transform.parent = base.transform;
				transform.position = objs[i].transform.position;
				transform.rotation = objs[i].transform.rotation;
				SpawnPoints[i] = transform;
			}
		}
	}
}
