// Decompiled with JetBrains decompiler
// Type: FistVR.RaygunRonnie
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class RaygunRonnie : MonoBehaviour
  {
    public Transform[] SpawnPoints;
    public GameObject Ronnie_Normal;
    public GameObject Ronnie_Boss;
    private List<GameObject> SpawnedEnemies = new List<GameObject>();
    private bool m_HotDogExists;
    public GameObject HotDog;
    public Transform HotDogSpawnPoint;

    private void Awake() => this.SpawnEdibleHotDog();

    public void BeginSequence()
    {
      this.m_HotDogExists = false;
      for (int index1 = 0; index1 < this.SpawnPoints.Length; ++index1)
      {
        this.SpawnedEnemies.Add(Object.Instantiate<GameObject>(this.Ronnie_Boss, this.SpawnPoints[index1].position, this.SpawnPoints[index1].rotation));
        for (int index2 = 0; index2 < 5; ++index2)
        {
          Vector3 vector3 = Random.onUnitSphere * 2.5f;
          vector3.y = 0.0f;
          this.SpawnedEnemies.Add(Object.Instantiate<GameObject>(this.Ronnie_Normal, this.SpawnPoints[index1].position + vector3, this.SpawnPoints[index1].rotation));
        }
      }
      this.SpawnEdibleHotDog();
    }

    public void PlayerDied()
    {
      if (this.SpawnedEnemies.Count <= 0)
        return;
      for (int index = this.SpawnedEnemies.Count - 1; index >= 0; --index)
      {
        if ((Object) this.SpawnedEnemies[index] != (Object) null)
          Object.Destroy((Object) this.SpawnedEnemies[index]);
      }
      this.SpawnedEnemies.Clear();
    }

    public void SpawnEdibleHotDog()
    {
      if (this.m_HotDogExists)
        return;
      this.m_HotDogExists = true;
      Object.Instantiate<GameObject>(this.HotDog, this.HotDogSpawnPoint.position, this.HotDogSpawnPoint.rotation);
    }
  }
}
