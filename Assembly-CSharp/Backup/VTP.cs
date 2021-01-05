// Decompiled with JetBrains decompiler
// Type: VTP
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class VTP : MonoBehaviour
{
  public static Color getSingleVertexColorAtHit(Transform transform, RaycastHit hit)
  {
    Vector3[] vertices = transform.GetComponent<MeshFilter>().sharedMesh.vertices;
    int[] triangles = transform.GetComponent<MeshFilter>().sharedMesh.triangles;
    Color[] colors = transform.GetComponent<MeshFilter>().sharedMesh.colors;
    int triangleIndex = hit.triangleIndex;
    float num1 = float.PositiveInfinity;
    int index1 = 0;
    for (int index2 = 0; index2 < 3; ++index2)
    {
      float num2 = Vector3.Distance(transform.TransformPoint(vertices[triangles[triangleIndex * 3 + index2]]), hit.point);
      if ((double) num2 < (double) num1)
      {
        index1 = triangles[triangleIndex * 3 + index2];
        num1 = num2;
      }
    }
    return colors[index1];
  }

  public static Color getFaceVerticesColorAtHit(Transform transform, RaycastHit hit)
  {
    int[] triangles = transform.GetComponent<MeshFilter>().sharedMesh.triangles;
    Color[] colors = transform.GetComponent<MeshFilter>().sharedMesh.colors;
    int triangleIndex = hit.triangleIndex;
    int index = triangles[triangleIndex * 3];
    return (colors[index] + colors[index + 1] + colors[index + 2]) / 3f;
  }

  public static void paintSingleVertexOnHit(
    Transform transform,
    RaycastHit hit,
    Color color,
    float strength)
  {
    Vector3[] vertices = transform.GetComponent<MeshFilter>().sharedMesh.vertices;
    int[] triangles = transform.GetComponent<MeshFilter>().sharedMesh.triangles;
    Color[] colors = transform.GetComponent<MeshFilter>().sharedMesh.colors;
    int triangleIndex = hit.triangleIndex;
    float num1 = float.PositiveInfinity;
    int index1 = 0;
    for (int index2 = 0; index2 < 3; index2 += 3)
    {
      float num2 = Vector3.Distance(transform.TransformPoint(vertices[triangles[triangleIndex * 3 + index2]]), hit.point);
      if ((double) num2 < (double) num1)
      {
        index1 = triangles[triangleIndex * 3 + index2];
        num1 = num2;
      }
    }
    Color color1 = VTP.VertexColorLerp(colors[index1], color, strength);
    colors[index1] = color1;
    transform.GetComponent<MeshFilter>().sharedMesh.colors = colors;
  }

  public static void paintFaceVerticesOnHit(
    Transform transform,
    RaycastHit hit,
    Color color,
    float strength)
  {
    int[] triangles = transform.GetComponent<MeshFilter>().sharedMesh.triangles;
    Color[] colors = transform.GetComponent<MeshFilter>().sharedMesh.colors;
    int triangleIndex = hit.triangleIndex;
    for (int index1 = 0; index1 < 3; ++index1)
    {
      int index2 = triangles[triangleIndex * 3 + index1];
      Color color1 = VTP.VertexColorLerp(colors[index2], color, strength);
      colors[index2] = color1;
    }
    transform.GetComponent<MeshFilter>().sharedMesh.colors = colors;
  }

  public static void deformSingleVertexOnHit(
    Transform transform,
    RaycastHit hit,
    bool up,
    float strength,
    bool recalculateNormals,
    bool recalculateCollider,
    bool recalculateFlow)
  {
    Vector3[] vertices = transform.GetComponent<MeshFilter>().sharedMesh.vertices;
    int[] triangles = transform.GetComponent<MeshFilter>().sharedMesh.triangles;
    Vector3[] normals = transform.GetComponent<MeshFilter>().sharedMesh.normals;
    int triangleIndex = hit.triangleIndex;
    float num1 = float.PositiveInfinity;
    int index1 = 0;
    for (int index2 = 0; index2 < 3; ++index2)
    {
      float num2 = Vector3.Distance(transform.TransformPoint(vertices[triangles[triangleIndex * 3 + index2]]), hit.point);
      if ((double) num2 < (double) num1)
      {
        index1 = triangles[triangleIndex * 3 + index2];
        num1 = num2;
      }
    }
    int num3 = 1;
    if (!up)
      num3 = -1;
    vertices[index1] += (float) num3 * 0.1f * strength * normals[index1];
    transform.GetComponent<MeshFilter>().sharedMesh.vertices = vertices;
    if (recalculateNormals)
      transform.GetComponent<MeshFilter>().sharedMesh.RecalculateNormals();
    if (recalculateCollider)
      transform.GetComponent<MeshCollider>().sharedMesh = transform.GetComponent<MeshFilter>().sharedMesh;
    if (!recalculateFlow)
      return;
    Vector4[] meshTangents = VTP.calculateMeshTangents(triangles, vertices, transform.GetComponent<MeshCollider>().sharedMesh.uv, normals);
    transform.GetComponent<MeshCollider>().sharedMesh.tangents = meshTangents;
    VTP.recalculateMeshForFlow(transform, vertices, normals, meshTangents);
  }

  public static void deformFaceVerticesOnHit(
    Transform transform,
    RaycastHit hit,
    bool up,
    float strength,
    bool recalculateNormals,
    bool recalculateCollider,
    bool recalculateFlow)
  {
    Vector3[] vertices = transform.GetComponent<MeshFilter>().sharedMesh.vertices;
    int[] triangles = transform.GetComponent<MeshFilter>().sharedMesh.triangles;
    Vector3[] normals = transform.GetComponent<MeshFilter>().sharedMesh.normals;
    int triangleIndex = hit.triangleIndex;
    int num = 1;
    if (!up)
      num = -1;
    for (int index1 = 0; index1 < 3; ++index1)
    {
      int index2 = triangles[triangleIndex * 3 + index1];
      vertices[index2] += (float) num * 0.1f * strength * normals[index2];
    }
    transform.GetComponent<MeshFilter>().sharedMesh.vertices = vertices;
    if (recalculateNormals)
      transform.GetComponent<MeshFilter>().sharedMesh.RecalculateNormals();
    if (recalculateCollider)
      transform.GetComponent<MeshCollider>().sharedMesh = transform.GetComponent<MeshFilter>().sharedMesh;
    if (!recalculateFlow)
      return;
    Vector4[] meshTangents = VTP.calculateMeshTangents(triangles, vertices, transform.GetComponent<MeshCollider>().sharedMesh.uv, normals);
    transform.GetComponent<MeshCollider>().sharedMesh.tangents = meshTangents;
    VTP.recalculateMeshForFlow(transform, vertices, normals, meshTangents);
  }

  private static void recalculateMeshForFlow(
    Transform transform,
    Vector3[] currentVertices,
    Vector3[] currentNormals,
    Vector4[] currentTangents)
  {
    Vector2[] uv4 = transform.GetComponent<MeshFilter>().sharedMesh.uv4;
    for (int index = 0; index < currentVertices.Length; ++index)
    {
      Vector3 vector3 = transform.TransformDirection(Vector3.Cross(currentNormals[index], new Vector3(currentTangents[index].x, currentTangents[index].y, currentTangents[index].z)).normalized * currentTangents[index].w);
      float x = (float) (0.5 + 0.5 * (double) transform.TransformDirection((Vector3) currentTangents[index].normalized).y);
      float y = (float) (0.5 + 0.5 * (double) vector3.y);
      uv4[index] = new Vector2(x, y);
    }
    transform.GetComponent<MeshFilter>().sharedMesh.uv4 = uv4;
  }

  private static Vector4[] calculateMeshTangents(
    int[] triangles,
    Vector3[] vertices,
    Vector2[] uv,
    Vector3[] normals)
  {
    int length1 = triangles.Length;
    int length2 = vertices.Length;
    Vector3[] vector3Array1 = new Vector3[length2];
    Vector3[] vector3Array2 = new Vector3[length2];
    Vector4[] vector4Array = new Vector4[length2];
    for (long index = 0; index < (long) length1; index += 3L)
    {
      long triangle1 = (long) triangles[index];
      long triangle2 = (long) triangles[index + 1L];
      long triangle3 = (long) triangles[index + 2L];
      Vector3 vertex1 = vertices[triangle1];
      Vector3 vertex2 = vertices[triangle2];
      Vector3 vertex3 = vertices[triangle3];
      Vector2 vector2_1 = uv[triangle1];
      Vector2 vector2_2 = uv[triangle2];
      Vector2 vector2_3 = uv[triangle3];
      float num1 = vertex2.x - vertex1.x;
      float num2 = vertex3.x - vertex1.x;
      float num3 = vertex2.y - vertex1.y;
      float num4 = vertex3.y - vertex1.y;
      float num5 = vertex2.z - vertex1.z;
      float num6 = vertex3.z - vertex1.z;
      float num7 = vector2_2.x - vector2_1.x;
      float num8 = vector2_3.x - vector2_1.x;
      float num9 = vector2_2.y - vector2_1.y;
      float num10 = vector2_3.y - vector2_1.y;
      float num11 = (float) ((double) num7 * (double) num10 - (double) num8 * (double) num9);
      float num12 = (double) num11 != 0.0 ? 1f / num11 : 0.0f;
      Vector3 vector3_1 = new Vector3((float) ((double) num10 * (double) num1 - (double) num9 * (double) num2) * num12, (float) ((double) num10 * (double) num3 - (double) num9 * (double) num4) * num12, (float) ((double) num10 * (double) num5 - (double) num9 * (double) num6) * num12);
      Vector3 vector3_2 = new Vector3((float) ((double) num7 * (double) num2 - (double) num8 * (double) num1) * num12, (float) ((double) num7 * (double) num4 - (double) num8 * (double) num3) * num12, (float) ((double) num7 * (double) num6 - (double) num8 * (double) num5) * num12);
      vector3Array1[triangle1] += vector3_1;
      vector3Array1[triangle2] += vector3_1;
      vector3Array1[triangle3] += vector3_1;
      vector3Array2[triangle1] += vector3_2;
      vector3Array2[triangle2] += vector3_2;
      vector3Array2[triangle3] += vector3_2;
    }
    for (long index = 0; index < (long) length2; ++index)
    {
      Vector3 normal = normals[index];
      Vector3 tangent = vector3Array1[index];
      Vector3.OrthoNormalize(ref normal, ref tangent);
      vector4Array[index].x = tangent.x;
      vector4Array[index].y = tangent.y;
      vector4Array[index].z = tangent.z;
      vector4Array[index].w = (double) Vector3.Dot(Vector3.Cross(normal, tangent), vector3Array2[index]) >= 0.0 ? 1f : -1f;
    }
    return vector4Array;
  }

  public static Color VertexColorLerp(Color colorA, Color colorB, float value)
  {
    if ((double) value >= 1.0)
      return colorB;
    return (double) value <= 0.0 ? colorA : new Color(colorA.r + (colorB.r - colorA.r) * value, colorA.g + (colorB.g - colorA.g) * value, colorA.b + (colorB.b - colorA.b) * value, colorA.a + (colorB.a - colorA.a) * value);
  }
}
