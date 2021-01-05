// Decompiled with JetBrains decompiler
// Type: TestSpawner
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class TestSpawner : MonoBehaviour
{
  public Transform SpawnPoint;
  public GameObject Prefab;
  private bool m_hasSpawned;

  private void OnTriggerEnter(Collider col)
  {
    if (this.m_hasSpawned)
      return;
    this.m_hasSpawned = true;
    Object.Instantiate<GameObject>(this.Prefab, this.SpawnPoint.position, this.SpawnPoint.rotation);
  }
}
