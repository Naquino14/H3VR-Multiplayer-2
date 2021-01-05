// Decompiled with JetBrains decompiler
// Type: FastPoolExtensions
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public static class FastPoolExtensions
{
  public static GameObject FastInstantiate(
    this GameObject sourcePrefab,
    Transform parentTransform = null)
  {
    return FastPoolManager.GetPool(sourcePrefab).FastInstantiate(parentTransform);
  }

  public static GameObject FastInstantiate(
    this GameObject sourcePrefab,
    Vector3 position,
    Quaternion rotation,
    Transform parentTransform = null)
  {
    return FastPoolManager.GetPool(sourcePrefab).FastInstantiate(position, rotation, parentTransform);
  }

  public static GameObject FastInstantiate(
    this GameObject sourcePrefab,
    int poolID,
    Transform parentTransform = null)
  {
    return FastPoolManager.GetPool(poolID, sourcePrefab).FastInstantiate(parentTransform);
  }

  public static GameObject FastInstantiate(
    this GameObject sourcePrefab,
    int poolID,
    Vector3 position,
    Quaternion rotation,
    Transform parentTransform = null)
  {
    return FastPoolManager.GetPool(poolID, sourcePrefab).FastInstantiate(position, rotation, parentTransform);
  }

  public static void FastDestroy(this GameObject objectToDestroy, GameObject sourcePrefab) => FastPoolManager.GetPool(sourcePrefab).FastDestroy(objectToDestroy);

  public static void FastDestroy(this GameObject objectToDestroy, Component sourcePrefab) => FastPoolManager.GetPool(sourcePrefab).FastDestroy(objectToDestroy);

  public static void FastDestroy(this GameObject objectToDestroy, FastPool pool) => pool.FastDestroy(objectToDestroy);

  public static void FastDestroy(this GameObject objectToDestroy, int poolID) => FastPoolManager.GetPool(poolID, (GameObject) null, false).FastDestroy(objectToDestroy);
}
