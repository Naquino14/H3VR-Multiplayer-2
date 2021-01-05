using UnityEngine;

public class TestSpawner : MonoBehaviour
{
	public Transform SpawnPoint;

	public GameObject Prefab;

	private bool m_hasSpawned;

	private void OnTriggerEnter(Collider col)
	{
		if (!m_hasSpawned)
		{
			m_hasSpawned = true;
			Object.Instantiate(Prefab, SpawnPoint.position, SpawnPoint.rotation);
		}
	}
}
