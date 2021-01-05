// Decompiled with JetBrains decompiler
// Type: FistVR.MF_StuffSpawner
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class MF_StuffSpawner : MonoBehaviour
  {
    public List<FVRObject> Equipment;
    public List<FVRObject> Equipment_Secondaries;
    public List<Transform> SpawnPoints;
    public List<Transform> SpawnPoints_Secondaries;

    public void SpawnEquipmentPiece(int i)
    {
      if (this.Equipment.Count > i)
        Object.Instantiate<GameObject>(this.Equipment[i].GetGameObject(), this.SpawnPoints[i].position, this.SpawnPoints[i].rotation);
      if (this.Equipment_Secondaries.Count <= i)
        return;
      Object.Instantiate<GameObject>(this.Equipment_Secondaries[i].GetGameObject(), this.SpawnPoints_Secondaries[i].position, this.SpawnPoints_Secondaries[i].rotation);
    }
  }
}
