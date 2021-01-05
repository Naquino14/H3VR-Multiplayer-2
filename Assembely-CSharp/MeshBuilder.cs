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
		label = 0;
		vertices.Clear();
		normals.Clear();
		uvs.Clear();
		triangles.Clear();
	}

	public void Apply(Mesh mesh)
	{
		mesh.Clear();
		mesh.SetVertices(vertices);
		mesh.SetNormals(normals);
		mesh.SetUVs(0, uvs);
		mesh.SetTriangles(triangles, 0);
	}
}
