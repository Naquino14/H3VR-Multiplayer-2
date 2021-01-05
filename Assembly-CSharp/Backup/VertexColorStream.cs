// Decompiled with JetBrains decompiler
// Type: VertexColorStream
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
public class VertexColorStream : MonoBehaviour
{
  [HideInInspector]
  public Mesh originalMesh;
  [HideInInspector]
  public Mesh paintedMesh;
  [HideInInspector]
  public MeshHolder meshHold;
  [HideInInspector]
  public Vector3[] _vertices;
  [HideInInspector]
  public Vector3[] _normals;
  [HideInInspector]
  public int[] _triangles;
  [HideInInspector]
  public int[][] _Subtriangles;
  [HideInInspector]
  public Matrix4x4[] _bindPoses;
  [HideInInspector]
  public BoneWeight[] _boneWeights;
  [HideInInspector]
  public Bounds _bounds;
  [HideInInspector]
  public int _subMeshCount;
  [HideInInspector]
  public Vector4[] _tangents;
  [HideInInspector]
  public Vector2[] _uv;
  [HideInInspector]
  public Vector2[] _uv2;
  [HideInInspector]
  public Vector2[] _uv3;
  [HideInInspector]
  public Color[] _colors;
  [HideInInspector]
  public Vector2[] _uv4;

  private void OnDidApplyAnimationProperties()
  {
  }

  public void init(Mesh origMesh, bool destroyOld)
  {
    this.originalMesh = origMesh;
    this.paintedMesh = Object.Instantiate<Mesh>(origMesh);
    if (destroyOld)
      Object.DestroyImmediate((Object) origMesh);
    this.paintedMesh.hideFlags = HideFlags.None;
    this.paintedMesh.name = "vpp_" + this.gameObject.name;
    this.meshHold = new MeshHolder();
    this.meshHold._vertices = this.paintedMesh.vertices;
    this.meshHold._normals = this.paintedMesh.normals;
    this.meshHold._triangles = this.paintedMesh.triangles;
    this.meshHold._TrianglesOfSubs = new trisPerSubmesh[this.paintedMesh.subMeshCount];
    for (int submesh = 0; submesh < this.paintedMesh.subMeshCount; ++submesh)
    {
      this.meshHold._TrianglesOfSubs[submesh] = new trisPerSubmesh();
      this.meshHold._TrianglesOfSubs[submesh].triangles = this.paintedMesh.GetTriangles(submesh);
    }
    this.meshHold._bindPoses = this.paintedMesh.bindposes;
    this.meshHold._boneWeights = this.paintedMesh.boneWeights;
    this.meshHold._bounds = this.paintedMesh.bounds;
    this.meshHold._subMeshCount = this.paintedMesh.subMeshCount;
    this.meshHold._tangents = this.paintedMesh.tangents;
    this.meshHold._uv = this.paintedMesh.uv;
    this.meshHold._uv2 = this.paintedMesh.uv2;
    this.meshHold._uv3 = this.paintedMesh.uv3;
    this.meshHold._colors = this.paintedMesh.colors;
    this.meshHold._uv4 = this.paintedMesh.uv4;
    this.GetComponent<MeshFilter>().sharedMesh = this.paintedMesh;
    if (!(bool) (Object) this.GetComponent<MeshCollider>())
      return;
    this.GetComponent<MeshCollider>().sharedMesh = this.paintedMesh;
  }

  public void setWholeMesh(Mesh tmpMesh)
  {
    this.paintedMesh.vertices = tmpMesh.vertices;
    this.paintedMesh.triangles = tmpMesh.triangles;
    this.paintedMesh.normals = tmpMesh.normals;
    this.paintedMesh.colors = tmpMesh.colors;
    this.paintedMesh.uv = tmpMesh.uv;
    this.paintedMesh.uv2 = tmpMesh.uv2;
    this.paintedMesh.uv3 = tmpMesh.uv3;
    this.meshHold._vertices = tmpMesh.vertices;
    this.meshHold._triangles = tmpMesh.triangles;
    this.meshHold._normals = tmpMesh.normals;
    this.meshHold._colors = tmpMesh.colors;
    this.meshHold._uv = tmpMesh.uv;
    this.meshHold._uv2 = tmpMesh.uv2;
    this.meshHold._uv3 = tmpMesh.uv3;
  }

  public Vector3[] setVertices(Vector3[] _deformedVertices)
  {
    this.paintedMesh.vertices = _deformedVertices;
    this.meshHold._vertices = _deformedVertices;
    this.paintedMesh.RecalculateNormals();
    this.paintedMesh.RecalculateBounds();
    this.meshHold._normals = this.paintedMesh.normals;
    this.meshHold._bounds = this.paintedMesh.bounds;
    this.GetComponent<MeshCollider>().sharedMesh = (Mesh) null;
    if ((bool) (Object) this.GetComponent<MeshCollider>())
      this.GetComponent<MeshCollider>().sharedMesh = this.paintedMesh;
    return this.meshHold._normals;
  }

  public Vector3[] getVertices() => this.paintedMesh.vertices;

  public Vector3[] getNormals() => this.paintedMesh.normals;

  public int[] getTriangles() => this.paintedMesh.triangles;

  public void setTangents(Vector4[] _meshTangents)
  {
    this.paintedMesh.tangents = _meshTangents;
    this.meshHold._tangents = _meshTangents;
  }

  public Vector4[] getTangents() => this.paintedMesh.tangents;

  public void setColors(Color[] _vertexColors)
  {
    this.paintedMesh.colors = _vertexColors;
    this.meshHold._colors = _vertexColors;
  }

  public Color[] getColors() => this.paintedMesh.colors;

  public Vector2[] getUVs() => this.paintedMesh.uv;

  public void setUV4s(Vector2[] _uv4s)
  {
    this.paintedMesh.uv4 = _uv4s;
    this.meshHold._uv4 = _uv4s;
  }

  public Vector2[] getUV4s() => this.paintedMesh.uv4;

  public void unlink() => this.init(this.paintedMesh, false);

  public void rebuild()
  {
    if (!(bool) (Object) this.GetComponent<MeshFilter>())
      return;
    this.paintedMesh = new Mesh();
    this.paintedMesh.hideFlags = HideFlags.HideAndDontSave;
    this.paintedMesh.name = "vpp_" + this.gameObject.name;
    if (this.meshHold == null || this.meshHold._vertices.Length == 0 || this.meshHold._TrianglesOfSubs.Length == 0)
    {
      this.paintedMesh.subMeshCount = this._subMeshCount;
      this.paintedMesh.vertices = this._vertices;
      this.paintedMesh.normals = this._normals;
      this.paintedMesh.triangles = this._triangles;
      this.meshHold._TrianglesOfSubs = new trisPerSubmesh[this.paintedMesh.subMeshCount];
      for (int submesh = 0; submesh < this.paintedMesh.subMeshCount; ++submesh)
      {
        this.meshHold._TrianglesOfSubs[submesh] = new trisPerSubmesh();
        this.meshHold._TrianglesOfSubs[submesh].triangles = this.paintedMesh.GetTriangles(submesh);
      }
      this.paintedMesh.bindposes = this._bindPoses;
      this.paintedMesh.boneWeights = this._boneWeights;
      this.paintedMesh.bounds = this._bounds;
      this.paintedMesh.tangents = this._tangents;
      this.paintedMesh.uv = this._uv;
      this.paintedMesh.uv2 = this._uv2;
      this.paintedMesh.uv3 = this._uv3;
      this.paintedMesh.colors = this._colors;
      this.paintedMesh.uv4 = this._uv4;
      this.init(this.paintedMesh, true);
    }
    else
    {
      this.paintedMesh.subMeshCount = this.meshHold._subMeshCount;
      this.paintedMesh.vertices = this.meshHold._vertices;
      this.paintedMesh.normals = this.meshHold._normals;
      for (int submesh = 0; submesh < this.meshHold._subMeshCount; ++submesh)
        this.paintedMesh.SetTriangles(this.meshHold._TrianglesOfSubs[submesh].triangles, submesh);
      this.paintedMesh.bindposes = this.meshHold._bindPoses;
      this.paintedMesh.boneWeights = this.meshHold._boneWeights;
      this.paintedMesh.bounds = this.meshHold._bounds;
      this.paintedMesh.tangents = this.meshHold._tangents;
      this.paintedMesh.uv = this.meshHold._uv;
      this.paintedMesh.uv2 = this.meshHold._uv2;
      this.paintedMesh.uv3 = this.meshHold._uv3;
      this.paintedMesh.colors = this.meshHold._colors;
      this.paintedMesh.uv4 = this.meshHold._uv4;
      this.init(this.paintedMesh, true);
    }
  }

  private void Start()
  {
    if ((bool) (Object) this.paintedMesh && this.meshHold != null)
      return;
    this.rebuild();
  }
}
