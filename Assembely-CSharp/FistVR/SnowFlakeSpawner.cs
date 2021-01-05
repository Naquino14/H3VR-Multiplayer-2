using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class SnowFlakeSpawner : MonoBehaviour
	{
		public GameObject SnowflakePrefab;

		public GameObject SnowflakeLootPrefab;

		public GameObject SnowflakeLootPrefab2;

		private List<GameObject> m_snowFlakes = new List<GameObject>();

		private float m_TickDownToSpawn = 5f;

		public int maxFlakes = 10;

		public float Radius = 75f;

		public Vector2 HeightRange = new Vector2(45f, 95f);

		public Vector2 SpawnDelay = new Vector2(3f, 5f);

		public bool Moving;

		public Vector2 MoveSPeed = new Vector2(10f, 30f);

		private void Start()
		{
		}

		private void Update()
		{
			if (m_TickDownToSpawn <= 0f)
			{
				m_TickDownToSpawn = Random.Range(SpawnDelay.x, SpawnDelay.y);
				ClearDeadFlakesAndSpawn();
			}
			else
			{
				m_TickDownToSpawn -= Time.deltaTime;
			}
		}

		private void ClearDeadFlakesAndSpawn()
		{
			for (int num = m_snowFlakes.Count - 1; num >= 0; num--)
			{
				if (m_snowFlakes[num] == null)
				{
					m_snowFlakes.RemoveAt(num);
				}
			}
			if (m_snowFlakes.Count < maxFlakes)
			{
				SpawnSnowflake();
			}
		}

		private void SpawnSnowflake()
		{
			Vector2 vector = Random.insideUnitCircle * Radius;
			GameObject gameObject = Object.Instantiate(position: new Vector3(vector.x, Random.Range(HeightRange.x, HeightRange.y), vector.y), original: SnowflakePrefab, rotation: Random.rotation);
			Rigidbody component = gameObject.GetComponent<Rigidbody>();
			FVRDestroyableObject component2 = gameObject.GetComponent<FVRDestroyableObject>();
			float num = Random.Range(0f, 1f);
			if (num > 0.95f || num > 0.9f)
			{
			}
			component.angularVelocity = Random.onUnitSphere * Random.Range(0.25f, 3f);
			m_snowFlakes.Add(gameObject);
			if (Moving)
			{
				Vector3 onUnitSphere = Random.onUnitSphere;
				onUnitSphere.y = 0f;
				component.velocity = onUnitSphere.normalized * Random.Range(MoveSPeed.x, MoveSPeed.y);
			}
		}
	}
}
