using UnityEngine;

public class UsingExtensions : MonoBehaviour
{
	public GameObject sampleGameObject;

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
			hugeObjectsArray[i] = sampleGameObject.FastInstantiate();
		}
	}

	public void DestroyObjects()
	{
		for (int i = 0; i < 1000; i++)
		{
			hugeObjectsArray[i].FastDestroy(sampleGameObject);
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
