// Decompiled with JetBrains decompiler
// Type: FastPoolBenchmark
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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
    if (this.spawnedObjects == null)
      this.spawnedObjects = new GameObject[this.times];
    this.sw = new Stopwatch();
    this.sw.Reset();
    this.sw.Start();
    for (int index = 0; index < this.times; ++index)
      this.spawnedObjects[index] = FastPoolManager.GetPool(this.sourcePrefab).FastInstantiate();
    this.sw.Stop();
    long elapsedMilliseconds1 = this.sw.ElapsedMilliseconds;
    this.sw.Reset();
    this.sw.Start();
    for (int index = 0; index < this.times; ++index)
      FastPoolManager.GetPool(this.sourcePrefab, false).FastDestroy(this.spawnedObjects[index]);
    this.sw.Stop();
    long elapsedMilliseconds2 = this.sw.ElapsedMilliseconds;
    this.Results.text = string.Format("FastInstantiating 1000 cubes: {0}ms\r\nFastDestroying 1000 cubes: {1}ms", (object) elapsedMilliseconds1, (object) elapsedMilliseconds2);
  }

  private void RunGenericBenchmark()
  {
    if (this.spawnedObjects == null)
      this.spawnedObjects = new GameObject[this.times];
    this.sw = new Stopwatch();
    this.sw.Reset();
    this.sw.Start();
    for (int index = 0; index < this.times; ++index)
      this.spawnedObjects[index] = Object.Instantiate<GameObject>(this.sourcePrefab);
    this.sw.Stop();
    long elapsedMilliseconds1 = this.sw.ElapsedMilliseconds;
    this.sw.Reset();
    this.sw.Start();
    for (int index = 0; index < this.times; ++index)
      Object.Destroy((Object) this.spawnedObjects[index]);
    this.sw.Stop();
    long elapsedMilliseconds2 = this.sw.ElapsedMilliseconds;
    this.Results.text = string.Format("Unity Instantiating 1000 cubes: {0}ms\r\nUnity Destroying 1000 cubes: {1}ms", (object) elapsedMilliseconds1, (object) elapsedMilliseconds2);
  }

  private void OnGUI()
  {
    if (GUI.Button(new Rect((float) (Screen.width / 2 - 50), (float) (Screen.height / 2 + 50), 100f, 30f), "Unity Test"))
      this.RunGenericBenchmark();
    if (!GUI.Button(new Rect((float) (Screen.width / 2 - 50), (float) (Screen.height / 2 + 85), 100f, 30f), "FastPool Test"))
      return;
    this.RunFPBenchmark();
  }
}
