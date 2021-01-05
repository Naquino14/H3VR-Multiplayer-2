using System.Diagnostics;
using UnityEngine;

public class FastPoolBenchmark : MonoBehaviour
{
	public GUIText Results;

	public GameObject sourcePrefab;

	private GameObject[] spawnedObjects;

	private Stopwatch sw;

	private int times = 1000;

	private void RunFPBenchmark()
	{
		if (spawnedObjects == null)
		{
			spawnedObjects = new GameObject[times];
		}
		sw = new Stopwatch();
		sw.Reset();
		sw.Start();
		for (int i = 0; i < times; i++)
		{
			spawnedObjects[i] = FastPoolManager.GetPool(sourcePrefab).FastInstantiate();
		}
		sw.Stop();
		long elapsedMilliseconds = sw.ElapsedMilliseconds;
		sw.Reset();
		sw.Start();
		for (int j = 0; j < times; j++)
		{
			FastPoolManager.GetPool(sourcePrefab, createIfNotExists: false).FastDestroy(spawnedObjects[j]);
		}
		sw.Stop();
		long elapsedMilliseconds2 = sw.ElapsedMilliseconds;
		Results.text = $"FastInstantiating 1000 cubes: {elapsedMilliseconds}ms\r\nFastDestroying 1000 cubes: {elapsedMilliseconds2}ms";
	}

	private void RunGenericBenchmark()
	{
		if (spawnedObjects == null)
		{
			spawnedObjects = new GameObject[times];
		}
		sw = new Stopwatch();
		sw.Reset();
		sw.Start();
		for (int i = 0; i < times; i++)
		{
			spawnedObjects[i] = Object.Instantiate(sourcePrefab);
		}
		sw.Stop();
		long elapsedMilliseconds = sw.ElapsedMilliseconds;
		sw.Reset();
		sw.Start();
		for (int j = 0; j < times; j++)
		{
			Object.Destroy(spawnedObjects[j]);
		}
		sw.Stop();
		long elapsedMilliseconds2 = sw.ElapsedMilliseconds;
		Results.text = $"Unity Instantiating 1000 cubes: {elapsedMilliseconds}ms\r\nUnity Destroying 1000 cubes: {elapsedMilliseconds2}ms";
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 50, 100f, 30f), "Unity Test"))
		{
			RunGenericBenchmark();
		}
		if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 85, 100f, 30f), "FastPool Test"))
		{
			RunFPBenchmark();
		}
	}
}
