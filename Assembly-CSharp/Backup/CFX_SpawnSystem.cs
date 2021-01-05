// Decompiled with JetBrains decompiler
// Type: CFX_SpawnSystem
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class CFX_SpawnSystem : MonoBehaviour
{
  private static CFX_SpawnSystem instance;
  public GameObject[] objectsToPreload = new GameObject[0];
  public int[] objectsToPreloadTimes = new int[0];
  public bool hideObjectsInHierarchy;
  private bool allObjectsLoaded;
  private Dictionary<int, List<GameObject>> instantiatedObjects = new Dictionary<int, List<GameObject>>();
  private Dictionary<int, int> poolCursors = new Dictionary<int, int>();

  public static GameObject GetNextObject(GameObject sourceObj, bool activateObject = true)
  {
    int instanceId = sourceObj.GetInstanceID();
    if (!CFX_SpawnSystem.instance.poolCursors.ContainsKey(instanceId))
    {
      Debug.LogError((object) ("[CFX_SpawnSystem.GetNextPoolObject()] Object hasn't been preloaded: " + sourceObj.name + " (ID:" + (object) instanceId + ")"));
      return (GameObject) null;
    }
    int poolCursor = CFX_SpawnSystem.instance.poolCursors[instanceId];
    Dictionary<int, int> poolCursors;
    int key;
    (poolCursors = CFX_SpawnSystem.instance.poolCursors)[key = instanceId] = poolCursors[key] + 1;
    if (CFX_SpawnSystem.instance.poolCursors[instanceId] >= CFX_SpawnSystem.instance.instantiatedObjects[instanceId].Count)
      CFX_SpawnSystem.instance.poolCursors[instanceId] = 0;
    GameObject gameObject = CFX_SpawnSystem.instance.instantiatedObjects[instanceId][poolCursor];
    if (activateObject)
      gameObject.SetActive(true);
    return gameObject;
  }

  public static void PreloadObject(GameObject sourceObj, int poolSize = 1) => CFX_SpawnSystem.instance.addObjectToPool(sourceObj, poolSize);

  public static void UnloadObjects(GameObject sourceObj) => CFX_SpawnSystem.instance.removeObjectsFromPool(sourceObj);

  public static bool AllObjectsLoaded => CFX_SpawnSystem.instance.allObjectsLoaded;

  private void addObjectToPool(GameObject sourceObject, int number)
  {
    int instanceId = sourceObject.GetInstanceID();
    if (!this.instantiatedObjects.ContainsKey(instanceId))
    {
      this.instantiatedObjects.Add(instanceId, new List<GameObject>());
      this.poolCursors.Add(instanceId, 0);
    }
    for (int index = 0; index < number; ++index)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(sourceObject);
      gameObject.SetActive(false);
      foreach (CFX_AutoDestructShuriken componentsInChild in gameObject.GetComponentsInChildren<CFX_AutoDestructShuriken>(true))
        componentsInChild.OnlyDeactivate = true;
      foreach (CFX_LightIntensityFade componentsInChild in gameObject.GetComponentsInChildren<CFX_LightIntensityFade>(true))
        componentsInChild.autodestruct = false;
      this.instantiatedObjects[instanceId].Add(gameObject);
      if (this.hideObjectsInHierarchy)
        gameObject.hideFlags = HideFlags.HideInHierarchy;
    }
  }

  private void removeObjectsFromPool(GameObject sourceObject)
  {
    int instanceId = sourceObject.GetInstanceID();
    if (!this.instantiatedObjects.ContainsKey(instanceId))
    {
      Debug.LogWarning((object) ("[CFX_SpawnSystem.removeObjectsFromPool()] There aren't any preloaded object for: " + sourceObject.name + " (ID:" + (object) instanceId + ")"));
    }
    else
    {
      for (int index = this.instantiatedObjects[instanceId].Count - 1; index >= 0; --index)
      {
        GameObject gameObject = this.instantiatedObjects[instanceId][index];
        this.instantiatedObjects[instanceId].RemoveAt(index);
        Object.Destroy((Object) gameObject);
      }
      this.instantiatedObjects.Remove(instanceId);
      this.poolCursors.Remove(instanceId);
    }
  }

  private void Awake()
  {
    if ((Object) CFX_SpawnSystem.instance != (Object) null)
      Debug.LogWarning((object) "CFX_SpawnSystem: There should only be one instance of CFX_SpawnSystem per Scene!");
    CFX_SpawnSystem.instance = this;
  }

  private void Start()
  {
    this.allObjectsLoaded = false;
    for (int index = 0; index < this.objectsToPreload.Length; ++index)
      CFX_SpawnSystem.PreloadObject(this.objectsToPreload[index], this.objectsToPreloadTimes[index]);
    this.allObjectsLoaded = true;
  }
}
