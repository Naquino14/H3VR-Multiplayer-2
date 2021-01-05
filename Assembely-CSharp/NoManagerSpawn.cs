using UnityEngine;

public class NoManagerSpawn : MonoBehaviour
{
	public FastPool fastPool;

	private GameObject[] hugeObjectsArray;

	private bool spawned;

	private void Start()
	{
		hugeObjectsArray = new GameObject[1000];
		fastPool.Init(base.transform);
	}

	public void Spawn()
	{
		for (int i = 0; i < 1000; i++)
		{
			hugeObjectsArray[i] = fastPool.FastInstantiate();
		}
	}

	public void DestroyObjects()
	{
		for (int i = 0; i < 1000; i++)
		{
			fastPool.FastDestroy(hugeObjectsArray[i]);
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
