// Decompiled with JetBrains decompiler
// Type: FistVR.DestructibleChunkProfile
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New DestructibleChunkProfile", menuName = "Destruction/ChunkProfile", order = 0)]
  public class DestructibleChunkProfile : ScriptableObject
  {
    [Header("Chunk Vars")]
    public string Name;
    public int MaxRandomIndex;
    public bool ScalesSpawns;
    public float TotalLife;
    public float DamageCutoff;
    public bool IsDestroyedOnZeroLife;
    public bool SpawnsOnDestruction;
    public GameObject SpawnOnDestruction;
    public bool UsesFinalMesh;
    public Mesh FinalMesh;
    public List<DestructibleChunkProfile.DGeoStage> DGeoStages;

    [Serializable]
    public class DGeoStage
    {
      public bool SpawnsOnEnterIndex;
      public GameObject SpawnOnEnterIndex;
      public List<Mesh> Meshes;

      public Mesh GetMesh(int index) => this.Meshes[Mathf.Clamp(index, 0, this.Meshes.Count - 1)];
    }
  }
}
