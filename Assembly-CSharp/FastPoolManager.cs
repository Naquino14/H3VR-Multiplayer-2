// Decompiled with JetBrains decompiler
// Type: FastPoolManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class FastPoolManager : MonoBehaviour
{
  [SerializeField]
  private List<FastPool> predefinedPools;
  private Dictionary<int, FastPool> pools;
  private Transform curTransform;

  public static FastPoolManager Instance { get; private set; }

  public Dictionary<int, FastPool> Pools => this.pools;

  private void Awake()
  {
    if ((Object) FastPoolManager.Instance == (Object) null)
    {
      FastPoolManager.Instance = this;
      this.curTransform = this.GetComponent<Transform>();
      this.pools = new Dictionary<int, FastPool>();
    }
    else
      Debug.LogError((object) "You cannot have more than one instance of FastPoolManager in the scene!");
    for (int index = 0; index < this.predefinedPools.Count; ++index)
    {
      if (this.predefinedPools[index].Init(this.curTransform))
      {
        if (!this.pools.ContainsKey(this.predefinedPools[index].ID))
          this.pools.Add(this.predefinedPools[index].ID, this.predefinedPools[index]);
        else
          Debug.LogError((object) "FastPoolManager has a several pools with the same ID. Please make sure that all your pools have unique IDs");
      }
    }
    this.predefinedPools.Clear();
  }

  private void Start()
  {
  }

  public static FastPool CreatePoolC<T>(
    T component,
    bool warmOnLoad = true,
    int preloadCount = 0,
    int capacity = 0)
    where T : Component
  {
    return (Object) component != (Object) null ? FastPoolManager.CreatePool(component.gameObject, warmOnLoad, preloadCount, capacity) : (FastPool) null;
  }

  public static FastPool CreatePool(
    GameObject prefab,
    bool warmOnLoad = true,
    int preloadCount = 0,
    int capacity = 0)
  {
    if ((Object) prefab != (Object) null)
    {
      if (FastPoolManager.Instance.pools.ContainsKey(prefab.GetInstanceID()))
        return FastPoolManager.Instance.pools[prefab.GetInstanceID()];
      FastPool fastPool = new FastPool(prefab, FastPoolManager.Instance.curTransform, warmOnLoad, preloadCount, capacity);
      FastPoolManager.Instance.pools.Add(prefab.GetInstanceID(), fastPool);
      return fastPool;
    }
    Debug.LogError((object) "Creating pool with null object");
    return (FastPool) null;
  }

  public static FastPool CreatePool(
    int id,
    GameObject prefab,
    bool warmOnLoad = true,
    int preloadCount = 0,
    int capacity = 0)
  {
    if ((Object) prefab != (Object) null)
    {
      if (FastPoolManager.Instance.pools.ContainsKey(id))
        return FastPoolManager.Instance.pools[id];
      FastPool fastPool = new FastPool(id, prefab, FastPoolManager.Instance.curTransform, warmOnLoad, preloadCount, capacity);
      FastPoolManager.Instance.pools.Add(id, fastPool);
      return fastPool;
    }
    Debug.LogError((object) "Creating pool with null object");
    return (FastPool) null;
  }

  public static FastPool GetPool(GameObject prefab, bool createIfNotExists = true)
  {
    if ((Object) prefab != (Object) null)
      return FastPoolManager.Instance.pools.ContainsKey(prefab.GetInstanceID()) ? FastPoolManager.Instance.pools[prefab.GetInstanceID()] : FastPoolManager.CreatePool(prefab);
    Debug.LogError((object) "Trying to get pool for null object");
    return (FastPool) null;
  }

  public static FastPool GetPool(int id, GameObject prefab, bool createIfNotExists = true) => FastPoolManager.Instance.pools.ContainsKey(id) ? FastPoolManager.Instance.pools[id] : FastPoolManager.CreatePool(id, prefab);

  public static FastPool GetPool(Component component, bool createIfNotExists = true)
  {
    if ((Object) component != (Object) null)
    {
      GameObject gameObject = component.gameObject;
      return FastPoolManager.Instance.pools.ContainsKey(gameObject.GetInstanceID()) ? FastPoolManager.Instance.pools[gameObject.GetInstanceID()] : FastPoolManager.CreatePool(gameObject);
    }
    Debug.LogError((object) "Trying to get pool for null object");
    return (FastPool) null;
  }

  public static void DestroyPool(FastPool pool)
  {
    pool.ClearCache();
    FastPoolManager.Instance.pools.Remove(pool.ID);
  }
}
