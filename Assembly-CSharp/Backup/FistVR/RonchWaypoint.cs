// Decompiled with JetBrains decompiler
// Type: FistVR.RonchWaypoint
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class RonchWaypoint : MonoBehaviour
  {
    public List<RonchWaypoint> Neighbors;
    public bool Debug = true;

    [ContextMenu("Neigh")]
    public void Neigh()
    {
      for (int index = 0; index < this.Neighbors.Count; ++index)
      {
        if (!this.Neighbors[index].Neighbors.Contains(this))
          this.Neighbors[index].Neighbors.Add(this);
      }
    }

    private void OnDrawGizmos()
    {
      if (!this.Debug || this.Neighbors.Count <= 0)
        return;
      for (int index = 0; index < this.Neighbors.Count; ++index)
      {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, this.Neighbors[index].transform.position);
      }
    }
  }
}
