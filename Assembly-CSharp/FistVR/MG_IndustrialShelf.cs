// Decompiled with JetBrains decompiler
// Type: FistVR.MG_IndustrialShelf
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MG_IndustrialShelf : MonoBehaviour
  {
    public Transform[] BoxSpawnPoints;
    public float Incidence = 0.5f;
    public GameObject[] BoxPrefabs;

    public void Init()
    {
      for (int index = 0; index < this.BoxSpawnPoints.Length; ++index)
      {
        if ((double) Random.Range(0.0f, 1f) <= (double) this.Incidence)
          Object.Instantiate<GameObject>(this.BoxPrefabs[Random.Range(0, this.BoxPrefabs.Length)], this.BoxSpawnPoints[index].position, this.BoxSpawnPoints[index].rotation);
      }
    }
  }
}
