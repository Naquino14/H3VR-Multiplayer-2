﻿// Decompiled with JetBrains decompiler
// Type: AdvancedSpawn
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class AdvancedSpawn : MonoBehaviour
{
  public GameObject sampleGameObject;
  private GameObject[] hugeObjectsArray;
  private FastPool fastPool;
  private bool spawned;

  private void Start()
  {
    this.hugeObjectsArray = new GameObject[1000];
    this.fastPool = FastPoolManager.CreatePool(this.sampleGameObject, preloadCount: 1000);
  }

  public void Spawn()
  {
    for (int index = 0; index < 1000; ++index)
      this.hugeObjectsArray[index] = this.fastPool.FastInstantiate();
  }

  public void DestroyObjects()
  {
    for (int index = 0; index < 1000; ++index)
      this.fastPool.FastDestroy(this.hugeObjectsArray[index]);
  }

  private void OnGUI()
  {
    if (!this.spawned)
    {
      if (!GUI.Button(new Rect((float) ((double) Screen.width * 0.5 - 75.0), (float) Screen.height * 0.8f, 150f, 50f), "Spawn 1000 objects"))
        return;
      this.Spawn();
      this.spawned = true;
    }
    else
    {
      if (!GUI.Button(new Rect((float) ((double) Screen.width * 0.5 - 75.0), (float) Screen.height * 0.8f, 150f, 50f), "Destroy 1000 objects"))
        return;
      this.DestroyObjects();
      this.spawned = false;
    }
  }
}
