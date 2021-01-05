using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class GronchGunSpawner : MonoBehaviour
	{
		private GronchJobManager m_M;

		private bool m_isSpawning;

		private float m_timeTilSpawn = 1f;

		private Vector2 TickRange = new Vector2(2f, 5f);

		public List<Transform> Positions;

		private List<GameObject> m_spawned = new List<GameObject>();

		private HashSet<FVRFireArm> m_spawnedFA = new HashSet<FVRFireArm>();

		public ObjectTableDef ObjectTableDef;

		private ObjectTable table;

		public Collider GronchTarget;

		public List<Transform> GronchTargetPlaces;

		private RaycastHit r;

		private bool m_isLoading;

		public void Awake()
		{
			table = new ObjectTable();
			table.Initialize(ObjectTableDef);
		}

		private void Start()
		{
			GM.CurrentSceneSettings.ShotFiredEvent += ShotFired;
		}

		private void OnDestroy()
		{
			GM.CurrentSceneSettings.ShotFiredEvent -= ShotFired;
		}

		private void ShotFired(FVRFireArm f)
		{
			if (m_isSpawning && m_spawnedFA.Contains(f))
			{
				m_spawnedFA.Remove(f);
				m_M.DidJobStuff();
				if (GronchTarget.Raycast(new Ray(f.MuzzlePos.position, f.MuzzlePos.forward), out r, 100f))
				{
					m_M.Promotion();
					GronchTargetPlaces.Shuffle();
					GronchTarget.transform.position = GronchTargetPlaces[0].position;
				}
			}
		}

		public void BeginJob(GronchJobManager m)
		{
			m_M = m;
			m_isSpawning = true;
			m_spawned.Clear();
			GronchTargetPlaces.Shuffle();
			GronchTargetPlaces.Shuffle();
			GronchTarget.transform.position = GronchTargetPlaces[0].position;
			GronchTarget.gameObject.SetActive(value: true);
		}

		public void EndJob(GronchJobManager m)
		{
			m_M = null;
			m_isSpawning = false;
			GronchTarget.gameObject.SetActive(value: false);
			ClearSpawned();
		}

		public void PlayerDied(GronchJobManager m)
		{
			m_M = m;
		}

		private void ClearSpawned()
		{
			for (int num = m_spawned.Count - 1; num >= 0; num--)
			{
				if (m_spawned[num] != null)
				{
					Object.Destroy(m_spawned[num]);
				}
			}
			m_spawned.Clear();
			m_spawnedFA.Clear();
		}

		private void Update()
		{
			if (m_isSpawning)
			{
				if (!m_isLoading)
				{
					m_timeTilSpawn -= Time.deltaTime;
				}
				if (m_timeTilSpawn <= 0f)
				{
					m_timeTilSpawn = Random.Range(TickRange.x, TickRange.y);
					AnvilManager.Run(SpawnObject(table));
				}
			}
		}

		private IEnumerator SpawnObject(ObjectTable table)
		{
			m_isLoading = true;
			FVRObject obj = table.GetRandomObject();
			FVRObject ammoObject = obj.GetRandomAmmoObject(obj, null, table.MinCapacity, table.MaxCapacity);
			int numAmmo4 = 0;
			if (ammoObject != null && ammoObject.Category == FVRObject.ObjectCategory.Cartridge)
			{
				numAmmo4 = 8;
			}
			else
			{
				numAmmo4 = 3;
			}
			numAmmo4 = 1;
			Positions.Shuffle();
			Positions.Shuffle();
			Positions.Shuffle();
			Positions.Shuffle();
			Transform sp_gun = Positions[0];
			Transform sp_ammo = Positions[1];
			yield return obj.GetGameObjectAsync();
			GameObject Ggun = Object.Instantiate(obj.GetGameObject(), sp_gun.position, sp_gun.rotation);
			m_spawned.Add(Ggun);
			FVRFireArm fa = Ggun.GetComponent<FVRFireArm>();
			m_spawnedFA.Add(fa);
			if (ammoObject != null)
			{
				Vector3 point = sp_ammo.position;
				yield return ammoObject.GetGameObjectAsync();
				for (int i = 0; i < numAmmo4; i++)
				{
					switch (ammoObject.Category)
					{
					case FVRObject.ObjectCategory.Cartridge:
					{
						GameObject item = Object.Instantiate(ammoObject.GetGameObject(), point, sp_ammo.rotation);
						m_spawned.Add(item);
						break;
					}
					case FVRObject.ObjectCategory.Clip:
					{
						GameObject gameObject2 = Object.Instantiate(ammoObject.GetGameObject(), point, sp_ammo.rotation);
						FVRFireArmClip component2 = gameObject2.GetComponent<FVRFireArmClip>();
						for (int k = 0; k < component2.m_capacity; k++)
						{
							if (component2.m_numRounds > 1)
							{
								component2.RemoveRound();
							}
						}
						m_spawned.Add(gameObject2);
						break;
					}
					case FVRObject.ObjectCategory.Magazine:
					{
						GameObject gameObject3 = Object.Instantiate(ammoObject.GetGameObject(), point, sp_ammo.rotation);
						FVRFireArmMagazine component3 = gameObject3.GetComponent<FVRFireArmMagazine>();
						m_spawned.Add(gameObject3);
						for (int l = 0; l < component3.m_capacity; l++)
						{
							if (component3.m_numRounds > 1)
							{
								component3.RemoveRound();
							}
						}
						break;
					}
					case FVRObject.ObjectCategory.SpeedLoader:
					{
						GameObject gameObject = Object.Instantiate(ammoObject.GetGameObject(), point, sp_ammo.rotation);
						Speedloader component = gameObject.GetComponent<Speedloader>();
						m_spawned.Add(gameObject);
						for (int j = 1; j < component.Chambers.Count; j++)
						{
							component.Chambers[j].Unload();
						}
						break;
					}
					}
					point += Vector3.up * 0.15f;
				}
			}
			m_isLoading = false;
		}
	}
}
