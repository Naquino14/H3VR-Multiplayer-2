// Decompiled with JetBrains decompiler
// Type: MeshBuilder
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class MeshBuilder
{
  public int label;
  public List<Vector3> vertices = new List<Vector3>(1024);
  public List<Vector3> normals = new List<Vector3>(1024);
  public List<Vector2> uvs = new List<Vector2>(1024);
  public List<int> triangles = new List<int>(1024);

  public void Clear()
  {
    this.label = 0;
    this.vertices.Clear();
    this.normals.Clear();
    this.uvs.Clear();
    this.triangles.Clear();
  }

  public void Apply(Mesh mesh)
  {
    mesh.Clear();
    mesh.SetVertices(this.vertices);
    mesh.SetNormals(this.normals);
    mesh.SetUVs(0, this.uvs);
    mesh.SetTriangles(this.triangles, 0);
  }
}
