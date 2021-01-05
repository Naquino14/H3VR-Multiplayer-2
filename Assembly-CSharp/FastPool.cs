// Decompiled with JetBrains decompiler
// Type: FastPool
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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

  public FastPool(
    GameObject prefab,
    Transform rootTransform = null,
    bool warmOnLoad = true,
    int preloadCount = 0,
    int capacity = 0)
  {
    this.sourcePrefab = prefab;
    this.PreloadCount = preloadCount;
    this.Capacity = capacity;
    this.WarmOnLoad = warmOnLoad;
    this.Init(rootTransform);
  }

  public FastPool(
    int id,
    GameObject prefab,
    Transform rootTransform = null,
    bool warmOnLoad = true,
    int preloadCount = 0,
    int capacity = 0)
  {
    this.useCustomID = true;
    this.customID = id;
    this.sourcePrefab = prefab;
    this.PreloadCount = preloadCount;
    this.Capacity = capacity;
    this.WarmOnLoad = warmOnLoad;
    this.Init(rootTransform);
  }

  public int ID { get; private set; }

  public string Name => this.sourcePrefab.name;

  public int Cached => this.cache.Count;

  public bool IsValid { get; private set; }

  public bool Init(Transform rootTransform)
  {
    if ((UnityEngine.Object) this.sourcePrefab != (UnityEngine.Object) null)
    {
      this.cached_internal = 0;
      this.cache = new Stack<GameObject>();
      this.parentTransform = rootTransform;
      this.ID = !this.useCustomID ? this.sourcePrefab.GetInstanceID() : this.customID;
      if (this.WarmOnLoad)
      {
        string str = this.sourcePrefab.name + "_SceneSource";
        this.sourcePrefab = this.MakeClone(Vector3.zero, Quaternion.identity, this.parentTransform);
        this.sourcePrefab.name = str;
        this.sourcePrefab.SetActive(false);
      }
      for (int count = this.cache.Count; count < this.PreloadCount; ++count)
        this.FastDestroy(this.MakeClone(Vector3.zero, Quaternion.identity, (Transform) null));
      this.IsValid = true;
    }
    else
    {
      Debug.LogError((object) "Creating pool with null prefab!");
      this.IsValid = false;
    }
    return this.IsValid;
  }

  public void ClearCache()
  {
    while (this.cache.Count > 0)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.cache.Pop());
    this.cached_internal = 0;
  }

  public T FastInstantiate<T>(Transform parent = null) where T : Component => this.FastInstantiate<T>(Vector3.zero, Quaternion.identity, parent);

  public T FastInstantiate<T>(Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
  {
    GameObject gameObject = this.FastInstantiate(position, rotation, parent);
    return (UnityEngine.Object) gameObject != (UnityEngine.Object) null ? gameObject.GetComponent<T>() : (T) null;
  }

  public GameObject FastInstantiate(Transform parent = null) => this.FastInstantiate(Vector3.zero, Quaternion.identity, parent);

  public GameObject FastInstantiate(
    Vector3 position,
    Quaternion rotation,
    Transform parent = null)
  {
    while (this.cache.Count > 0)
    {
      GameObject gameObject = this.cache.Pop();
      --this.cached_internal;
      if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
      {
        Transform transform = gameObject.transform;
        transform.localPosition = position;
        transform.localRotation = rotation;
        if ((UnityEngine.Object) parent != (UnityEngine.Object) null)
          transform.SetParent(parent, false);
        gameObject.SetActive(true);
        switch (this.NotificationType)
        {
          case PoolItemNotificationType.Interface:
            foreach (IFastPoolItem component in gameObject.GetComponents<IFastPoolItem>())
              component.OnFastInstantiate();
            break;
          case PoolItemNotificationType.SendMessage:
            gameObject.SendMessage("OnFastInstantiate");
            break;
          case PoolItemNotificationType.BroadcastMessage:
            gameObject.BroadcastMessage("OnFastInstantiate");
            break;
        }
        return gameObject;
      }
      Debug.LogWarning((object) ("The pool with the " + this.sourcePrefab.name + " prefab contains null entry. Don't destroy cached items manually!"));
    }
    GameObject gameObject1 = this.MakeClone(position, rotation, parent);
    if (this.WarmOnLoad)
      gameObject1.SetActive(true);
    return gameObject1;
  }

  public void FastDestroy<T>(T sceneObject) where T : Component
  {
    if ((UnityEngine.Object) sceneObject != (UnityEngine.Object) null)
      this.FastDestroy(sceneObject.gameObject);
    else
      Debug.LogWarning((object) "Attempt to destroy a null object");
  }

  public void FastDestroy(GameObject sceneObject)
  {
    if ((UnityEngine.Object) sceneObject != (UnityEngine.Object) null)
    {
      if (this.cache.Count < this.Capacity || this.Capacity <= 0)
      {
        this.cache.Push(sceneObject);
        ++this.cached_internal;
        if (this.ParentOnCache)
          sceneObject.transform.SetParent(this.parentTransform, false);
        switch (this.NotificationType)
        {
          case PoolItemNotificationType.Interface:
            foreach (IFastPoolItem component in sceneObject.GetComponents<IFastPoolItem>())
              component.OnFastDestroy();
            break;
          case PoolItemNotificationType.SendMessage:
            sceneObject.SendMessage("OnFastDestroy");
            break;
        }
        sceneObject.SetActive(false);
      }
      else
        UnityEngine.Object.Destroy((UnityEngine.Object) sceneObject);
    }
    else
      Debug.LogWarning((object) "Attempt to destroy a null object");
  }

  private GameObject MakeClone(Vector3 position, Quaternion rotation, Transform parent)
  {
    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.sourcePrefab, position, rotation);
    gameObject.transform.SetParent(parent, false);
    return gameObject;
  }
}
