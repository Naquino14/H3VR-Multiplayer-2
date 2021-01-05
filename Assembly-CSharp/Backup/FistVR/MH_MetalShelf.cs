// Decompiled with JetBrains decompiler
// Type: FistVR.MH_MetalShelf
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MH_MetalShelf : MonoBehaviour, IMGSpawnIntoAble
  {
    private bool m_canBeenSpawnedInto = true;
    public Transform[] SpawnPositions;

    public bool CanBeSpawnedInto() => this.m_canBeenSpawnedInto;

    public void PlaceObjectInto(GameObject obj)
    {
      int index = Random.Range(0, this.SpawnPositions.Length);
      obj.transform.position = this.SpawnPositions[index].position;
      obj.transform.rotation = this.SpawnPositions[index].rotation;
      this.m_canBeenSpawnedInto = false;
    }

    public void PlaceObjectInto(GameObject obj, GameObject obj2)
    {
      int index = Random.Range(0, this.SpawnPositions.Length);
      obj.transform.position = this.SpawnPositions[index].position;
      obj.transform.rotation = this.SpawnPositions[index].rotation;
      obj2.transform.position = this.SpawnPositions[index].position + Vector3.up * 0.3f;
      obj2.transform.rotation = this.SpawnPositions[index].rotation;
      this.m_canBeenSpawnedInto = false;
    }

    public void PlaceObjectInto(GameObject obj, GameObject[] objs)
    {
      int index1 = Random.Range(0, this.SpawnPositions.Length);
      obj.transform.position = this.SpawnPositions[index1].position;
      obj.transform.rotation = this.SpawnPositions[index1].rotation;
      for (int index2 = 0; index2 < objs.Length; ++index2)
      {
        objs[index2].transform.position = this.SpawnPositions[index1].position + Vector3.up * 0.1f * (float) (index2 + 1);
        objs[index2].transform.rotation = this.SpawnPositions[index1].rotation;
      }
      this.m_canBeenSpawnedInto = false;
    }
  }
}
