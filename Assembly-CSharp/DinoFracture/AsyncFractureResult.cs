// Decompiled with JetBrains decompiler
// Type: DinoFracture.AsyncFractureResult
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace DinoFracture
{
  public sealed class AsyncFractureResult
  {
    public bool IsComplete { get; private set; }

    public GameObject PiecesRoot { get; private set; }

    public Bounds EntireMeshBounds { get; private set; }

    internal bool StopRequested { get; private set; }

    internal void SetResult(GameObject rootGO, Bounds bounds)
    {
      if (this.IsComplete)
      {
        Debug.LogWarning((object) "DinoFracture: Setting AsyncFractureResult's results twice.");
      }
      else
      {
        this.PiecesRoot = rootGO;
        this.EntireMeshBounds = bounds;
        this.IsComplete = true;
      }
    }

    public void StopFracture() => this.StopRequested = true;
  }
}
