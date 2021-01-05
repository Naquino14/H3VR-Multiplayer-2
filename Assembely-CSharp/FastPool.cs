using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FastPool
{
	[SerializeField]
	[Tooltip("Prefab that would be used as source")]
	private GameObject sourcePrefab;

	[Tooltip("Cache size (maximum amount of the cached items).\r\n[0 - unlimited]")]
	public int Capacity;

	[Tooltip("How much items must be cached at the start")]
	public int PreloadCount;

	[Tooltip("How to call events OnFastInstantiate and OnFastDestroy. Note, that if use choose notification via Interface, you must implement IFastPoolItem in any script on your sourcePrefab")]
	public PoolItemNotificationType NotificationType;

	[Tooltip("Load source prefab in the memory while scene is loading. A bit slower scene loading, but faster instantiating of new objects in pool")]
	public bool WarmOnLoad = true;

	[Tooltip("Parent cached objects to FastPoolManager game object.\r\n[WARNING] Turning this option on will make objects cached a bit slower.")]
	public bool ParentOnCache;

	[Tooltip("Use custom pool ID. By default it equals to the InstanceID of the source prefab.\r\n[WARNING] Be careful with this option.")]
	[SerializeField]
	private bool useCustomID;

	[SerializeField]
	[HideInInspector]
	private int customID = -1;

	[SerializeField]
	[HideInInspector]
	private int cached_internal;

	private Stack<GameObject> cache;

	private Transform parentTransform;

	public int ID
	{
		get;
		private set;
	}

	public string Name => sourcePrefab.name;

	public int Cached => cache.Count;

	public bool IsValid
	{
		get;
		private set;
	}

	public FastPool(GameObject prefab, Transform rootTransform = null, bool warmOnLoad = true, int preloadCount = 0, int capacity = 0)
	{
		sourcePrefab = prefab;
		PreloadCount = preloadCount;
		Capacity = capacity;
		WarmOnLoad = warmOnLoad;
		Init(rootTransform);
	}

	public FastPool(int id, GameObject prefab, Transform rootTransform = null, bool warmOnLoad = true, int preloadCount = 0, int capacity = 0)
	{
		useCustomID = true;
		customID = id;
		sourcePrefab = prefab;
		PreloadCount = preloadCount;
		Capacity = capacity;
		WarmOnLoad = warmOnLoad;
		Init(rootTransform);
	}

	public bool Init(Transform rootTransform)
	{
		if (sourcePrefab != null)
		{
			cached_internal = 0;
			cache = new Stack<GameObject>();
			parentTransform = rootTransform;
			ID = ((!useCustomID) ? sourcePrefab.GetInstanceID() : customID);
			if (WarmOnLoad)
			{
				string name = sourcePrefab.name + "_SceneSource";
				sourcePrefab = MakeClone(Vector3.zero, Quaternion.identity, parentTransform);
				sourcePrefab.name = name;
				sourcePrefab.SetActive(value: false);
			}
			for (int i = cache.Count; i < PreloadCount; i++)
			{
				FastDestroy(MakeClone(Vector3.zero, Quaternion.identity, null));
			}
			IsValid = true;
		}
		else
		{
			Debug.LogError("Creating pool with null prefab!");
			IsValid = false;
		}
		return IsValid;
	}

	public void ClearCache()
	{
		while (cache.Count > 0)
		{
			UnityEngine.Object.Destroy(cache.Pop());
		}
		cached_internal = 0;
	}

	public T FastInstantiate<T>(Transform parent = null) where T : Component
	{
		return FastInstantiate<T>(Vector3.zero, Quaternion.identity, parent);
	}

	public T FastInstantiate<T>(Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
	{
		GameObject gameObject = FastInstantiate(position, rotation, parent);
		return (!(gameObject != null)) ? ((T)null) : gameObject.GetComponent<T>();
	}

	public GameObject FastInstantiate(Transform parent = null)
	{
		return FastInstantiate(Vector3.zero, Quaternion.identity, parent);
	}

	public GameObject FastInstantiate(Vector3 position, Quaternion rotation, Transform parent = null)
	{
		GameObject gameObject;
		while (cache.Count > 0)
		{
			gameObject = cache.Pop();
			cached_internal--;
			if (gameObject != null)
			{
				Transform transform = gameObject.transform;
				transform.localPosition = position;
				transform.localRotation = rotation;
				if (parent != null)
				{
					transform.SetParent(parent, worldPositionStays: false);
				}
				gameObject.SetActive(value: true);
				switch (NotificationType)
				{
				case PoolItemNotificationType.Interface:
				{
					IFastPoolItem[] components = gameObject.GetComponents<IFastPoolItem>();
					for (int i = 0; i < components.Length; i++)
					{
						components[i].OnFastInstantiate();
					}
					break;
				}
				case PoolItemNotificationType.SendMessage:
					gameObject.SendMessage("OnFastInstantiate");
					break;
				case PoolItemNotificationType.BroadcastMessage:
					gameObject.BroadcastMessage("OnFastInstantiate");
					break;
				}
				return gameObject;
			}
			Debug.LogWarning("The pool with the " + sourcePrefab.name + " prefab contains null entry. Don't destroy cached items manually!");
		}
		gameObject = MakeClone(position, rotation, parent);
		if (WarmOnLoad)
		{
			gameObject.SetActive(value: true);
		}
		return gameObject;
	}

	public void FastDestroy<T>(T sceneObject) where T : Component
	{
		if ((UnityEngine.Object)sceneObject != (UnityEngine.Object)null)
		{
			FastDestroy(sceneObject.gameObject);
		}
		else
		{
			Debug.LogWarning("Attempt to destroy a null object");
		}
	}

	public void FastDestroy(GameObject sceneObject)
	{
		if (sceneObject != null)
		{
			if (cache.Count < Capacity || Capacity <= 0)
			{
				cache.Push(sceneObject);
				cached_internal++;
				if (ParentOnCache)
				{
					sceneObject.transform.SetParent(parentTransform, worldPositionStays: false);
				}
				switch (NotificationType)
				{
				case PoolItemNotificationType.Interface:
				{
					IFastPoolItem[] components = sceneObject.GetComponents<IFastPoolItem>();
					for (int i = 0; i < components.Length; i++)
					{
						components[i].OnFastDestroy();
					}
					break;
				}
				case PoolItemNotificationType.SendMessage:
					sceneObject.SendMessage("OnFastDestroy");
					break;
				}
				sceneObject.SetActive(value: false);
			}
			else
			{
				UnityEngine.Object.Destroy(sceneObject);
			}
		}
		else
		{
			Debug.LogWarning("Attempt to destroy a null object");
		}
	}

	private GameObject MakeClone(Vector3 position, Quaternion rotation, Transform parent)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(sourcePrefab, position, rotation);
		gameObject.transform.SetParent(parent, worldPositionStays: false);
		return gameObject;
	}
}
