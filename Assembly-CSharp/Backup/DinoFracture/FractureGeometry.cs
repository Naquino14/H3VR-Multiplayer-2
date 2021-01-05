// Decompiled with JetBrains decompiler
// Type: DinoFracture.FractureGeometry
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace DinoFracture
{
  public abstract class FractureGeometry : MonoBehaviour
  {
    public Material InsideMaterial;
    public GameObject FractureTemplate;
    public Transform PiecesParent;
    public int NumFracturePieces = 5;
    public int NumIterations = 2;
    public int NumGenerations = 1;
    public float FractureRadius;
    public FractureUVScale UVScale = FractureUVScale.Piece;
    public bool DistributeMass = true;
    private bool _processingFracture;

    public AsyncFractureResult Fracture() => this.NumGenerations == 0 || this._processingFracture ? (AsyncFractureResult) null : this.FractureInternal(Vector3.zero);

    public AsyncFractureResult Fracture(Vector3 localPos) => this.NumGenerations == 0 || this._processingFracture ? (AsyncFractureResult) null : this.FractureInternal(localPos);

    protected AsyncFractureResult Fracture(
      FractureDetails details,
      bool hideAfterFracture)
    {
      if (this.NumGenerations == 0 || this._processingFracture)
        return (AsyncFractureResult) null;
      if ((Object) this.FractureTemplate == (Object) null || (Object) this.FractureTemplate.GetComponent<MeshFilter>() == (Object) null)
        Debug.LogError((object) "DinoFracture: A fracture template with a MeshFilter component is required.");
      this._processingFracture = true;
      if ((Object) details.Mesh == (Object) null)
      {
        MeshFilter component1 = this.GetComponent<MeshFilter>();
        SkinnedMeshRenderer component2 = this.GetComponent<SkinnedMeshRenderer>();
        if ((Object) component1 == (Object) null && (Object) component2 == (Object) null)
        {
          Debug.LogError((object) "DinoFracture: A mesh filter required if a mesh is not supplied.");
          return (AsyncFractureResult) null;
        }
        Mesh mesh;
        if ((Object) component1 != (Object) null)
        {
          mesh = component1.sharedMesh;
        }
        else
        {
          mesh = new Mesh();
          component2.BakeMesh(mesh);
        }
        details.Mesh = mesh;
      }
      if (details.MeshScale == Vector3.zero)
        details.MeshScale = this.transform.localScale;
      Transform piecesParent = !((Object) this.PiecesParent == (Object) null) ? this.PiecesParent : (Transform) null;
      return FractureEngine.StartFracture(details, this, piecesParent, this.DistributeMass, hideAfterFracture);
    }

    protected void StopFracture() => this._processingFracture = false;

    protected abstract AsyncFractureResult FractureInternal(Vector3 localPos);

    internal virtual void OnFracture(OnFractureEventArgs args)
    {
      if (!((Object) args.OriginalObject == (Object) this))
        return;
      this._processingFracture = false;
    }
  }
}
