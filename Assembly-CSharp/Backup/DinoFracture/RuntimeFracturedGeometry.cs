// Decompiled with JetBrains decompiler
// Type: DinoFracture.RuntimeFracturedGeometry
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace DinoFracture
{
  public class RuntimeFracturedGeometry : FractureGeometry
  {
    public bool Asynchronous = true;

    protected override AsyncFractureResult FractureInternal(Vector3 localPos) => this.Fracture(new FractureDetails()
    {
      NumPieces = this.NumFracturePieces,
      NumIterations = this.NumIterations,
      UVScale = FractureUVScale.Piece,
      FractureCenter = localPos,
      FractureRadius = this.FractureRadius,
      Asynchronous = this.Asynchronous
    }, true);
  }
}
