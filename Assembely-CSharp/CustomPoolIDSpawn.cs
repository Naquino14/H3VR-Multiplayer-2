using UnityEngine;

public class CustomPoolIDSpawn : MonoBehaviour
{
	private GameObject[] hugeObjectsArray;

	private bool spawned;

	private void Start()
	{
		hugeObjectsArray = new GameObject[1000];
	}

	public void Spawn()
	{
		for (int i = 0; i < 1000; i++)
		{
			FastPool pool = FastPoolManager.GetPool(1, null, createIfNotExists: false);
			hugeObjectsArray[i] = pool.FastInstantiate();
		}
	}

	public void DestroyObjects()
	{
		for (int i = 0; i < 1000; i++)
		{
			FastPool pool = FastPoolManager.GetPool(1, null, createIfNotExists: false);
			pool.FastDestroy(hugeObjectsArray[i]);
		}
	}

	private void OnGUI()
	{
		if (!spawned)
		{
			if (GUI.Button(new Rect((float)Screen.width * 0.5f - 75f, (float)Screen.height * 0.8f, 150f, 50f), "Spawn 1000 objects"))
			{
				Spawn();
				spawned = true;
			}
		}
		else if (GUI.Button(new Rect((float)Screen.width * 0.5f - 75f, (float)Screen.height * 0.8f, 150f, 50f), "Destroy 1000 objects"))
		{
			DestroyObjects();
			spawned = false;
		}
	}
}
