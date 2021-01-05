// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigLemonManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class ZosigLemonManager : MonoBehaviour
  {
    public List<FVRObject> ObjectWrappers;
    public List<ZosigSpawnFromTable> InitialTTSpawns;
    public List<ZosigSpawnFromTable> FinishedTTSpawns;
    public List<ZosigFlagOnItemDetect> BringTTHere;
    public List<ZosigFlagOnItemDetect> DONTBringTTHere;
    public List<Renderer> ScreamInstructions;
    public List<Material> ScreamMats;
    private List<int> indicies = new List<int>()
    {
      0,
      1,
      2,
      3,
      4,
      5,
      6,
      7,
      8,
      9,
      10,
      11,
      12
    };

    public void InitLemons()
    {
      this.indicies.Shuffle<int>();
      this.indicies.Shuffle<int>();
      this.indicies.Shuffle<int>();
      this.indicies.Shuffle<int>();
      for (int index = 0; index < this.InitialTTSpawns.Count; ++index)
      {
        int indicy = this.indicies[index];
        this.InitialTTSpawns[index].Item = this.ObjectWrappers[indicy];
      }
      for (int index1 = 0; index1 < 6; ++index1)
      {
        int indicy = this.indicies[index1];
        this.FinishedTTSpawns[index1].Item = this.ObjectWrappers[indicy];
        this.BringTTHere[index1].ObjectsToBeDetected[0] = this.ObjectWrappers[indicy];
        this.DONTBringTTHere[index1].ObjectsToBeDetected.Clear();
        for (int index2 = 0; index2 < this.ObjectWrappers.Count; ++index2)
          this.DONTBringTTHere[index1].ObjectsToBeDetected.Add(this.ObjectWrappers[index2]);
        this.DONTBringTTHere[index1].ObjectsToBeDetected.Remove(this.BringTTHere[index1].ObjectsToBeDetected[0]);
        this.DONTBringTTHere[index1].ObjectsToBeDetected.TrimExcess();
        this.BringTTHere[index1].RefreshFlagCache();
        this.DONTBringTTHere[index1].RefreshFlagCache();
        this.ScreamInstructions[index1].material = this.ScreamMats[indicy];
      }
    }
  }
}
