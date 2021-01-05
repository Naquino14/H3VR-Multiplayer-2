// Decompiled with JetBrains decompiler
// Type: AICoverPoint
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class AICoverPoint : MonoBehaviour
{
  [InspectorButton("Calc")]
  public bool DoCalc;
  [InspectorButton("TestDist")]
  public bool DoTestDist;
  public int GroupIndex;
  public bool IsActive = true;
  private const int width = 8;
  private const int height = 3;
  public float[] MaxVis = new float[24];
  public LayerMask VisMask;
  public bool[] DoDebug = new bool[3];
  public bool[] DoDebugNew = new bool[3];
  public float[] Heights = new float[3];
  public Vector3[] HitPoints = new Vector3[24];
  public Vector3 Pos;
  public Transform TestCube;
  public float Scratch;
  public bool IsClaimed;
  public float[] MaxDist_Standing = new float[42];
  public float[] MaxDist_Crouching = new float[42];
  public float[] MaxDist_Prone = new float[42];

  public void Start() => this.Pos = this.transform.position;

  public void TestDist()
  {
  }

  public float GetMaxVisToPoint(Vector3 point, int whichHeight)
  {
    Vector3 v = point - this.Pos;
    v.y = 0.0f;
    int indexFromDir = this.GetIndexFromDir(v);
    int num = 8 * whichHeight + indexFromDir;
    int length = this.MaxVis.Length;
    if (num >= length)
      Debug.Log((object) "HOLY SHIT INDEX WRONG");
    return this.MaxVis[Mathf.Clamp(num, 0, length - 1)];
  }

  public float GetMaxVisToPoint(Vector3 point, AICoverPoint.SampleHeight height)
  {
    Vector3 dir = point - this.Pos;
    if (height == AICoverPoint.SampleHeight.Standing)
      return this.GetMaxVisFromDir(dir, this.MaxDist_Standing);
    return height == AICoverPoint.SampleHeight.Crouching ? this.GetMaxVisFromDir(dir, this.MaxDist_Crouching) : this.GetMaxVisFromDir(dir, this.MaxDist_Prone);
  }

  public float GetMaxVisFromDir(Vector3 dir, int whichHeight)
  {
    dir.y = 0.0f;
    int indexFromDir = this.GetIndexFromDir(dir);
    return this.MaxVis[8 * whichHeight + indexFromDir];
  }

  public float GetMaxVisFromDir(Vector3 dir, AICoverPoint.SampleHeight height)
  {
    if (height == AICoverPoint.SampleHeight.Standing)
      return this.GetMaxVisFromDir(dir, this.MaxDist_Standing);
    return height == AICoverPoint.SampleHeight.Crouching ? this.GetMaxVisFromDir(dir, this.MaxDist_Crouching) : this.GetMaxVisFromDir(dir, this.MaxDist_Prone);
  }

  public float GetMaxVisFromDir(Vector3 dir, float[] dists)
  {
    float num = Vector3.Angle(dir, Vector3.up);
    int index = (double) num > 45.0 ? ((double) num > 65.0 ? ((double) num > 80.0 ? ((double) num < 100.0 ? ((double) num < 115.0 ? ((double) num < 135.0 ? this.GetIndexFromDir(dir) : 51) : this.GetIndexFromDir(dir) + 32) : this.GetIndexFromDir(dir) + 16) : this.GetIndexFromDir(dir) + 8) : this.GetIndexFromDir(dir) + 24) : 40;
    return dists[index];
  }

  private int GetIndexFromDir(Vector3 v)
  {
    float num = AICoverPoint.AngleSigned(Vector3.forward, v, Vector3.up);
    if ((double) num < 0.0)
      num += 360f;
    return Mathf.FloorToInt(num / 45f);
  }

  public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n) => Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * 57.29578f;

  private void OnDrawGizmos()
  {
    Gizmos.color = this.IsActive ? (!this.IsClaimed ? new Color(0.2f, 1f, 0.2f) : new Color(1f, 0.2f, 0.2f)) : new Color(0.2f, 0.2f, 0.2f);
    Gizmos.DrawSphere(this.transform.position, 0.1f);
    Gizmos.DrawWireSphere(this.transform.position, 0.1f);
    for (int index1 = 0; index1 < 3; ++index1)
    {
      if (this.DoDebug[index1])
      {
        Vector3 from = this.transform.position + Vector3.up * this.Heights[index1];
        for (int index2 = 0; index2 < 8; ++index2)
        {
          Vector3 forward = Vector3.forward;
          Vector3 vector3 = Quaternion.AngleAxis(45f * (float) index2, Vector3.up) * forward * this.MaxVis[8 * index1 + index2];
          Gizmos.color = Color.red;
          Gizmos.DrawLine(from, from + vector3);
        }
      }
    }
    if (!this.DoDebugNew[0])
      return;
    Vector3 p1 = this.transform.position + Vector3.up * this.Heights[0];
    this.DrawLineSet(p1, this.MaxDist_Standing, 0.0f, 0);
    this.DrawLineSet(p1, this.MaxDist_Standing, -10f, 8);
    this.DrawLineSet(p1, this.MaxDist_Standing, 10f, 16);
    this.DrawLineSet(p1, this.MaxDist_Standing, -25f, 24);
    this.DrawLineSet(p1, this.MaxDist_Standing, 25f, 32);
  }

  private void DrawLineSet(Vector3 p1, float[] values, float angle, int offset)
  {
    for (int index = 0; index < 8; ++index)
    {
      Vector3 vector3_1 = Vector3.forward * values[index + offset];
      Vector3 vector3_2 = Quaternion.AngleAxis(angle, Vector3.right) * vector3_1;
      Vector3 vector3_3 = Quaternion.AngleAxis((float) index * 45f, Vector3.up) * vector3_2;
      Debug.DrawLine(p1, p1 + vector3_3, Color.red);
    }
  }

  [ContextMenu("Calc")]
  public void Calc()
  {
    for (int index1 = 0; index1 < 3; ++index1)
    {
      Vector3 vector3_1 = this.transform.position + Vector3.up * this.Heights[index1];
      for (int index2 = 0; index2 < 8; ++index2)
      {
        Vector3 forward = Vector3.forward;
        float num1 = -22.5f;
        int num2 = 60;
        float num3 = 0.0f;
        float a = 0.0f;
        for (int index3 = 0; index3 <= num2; ++index3)
        {
          float num4 = Mathf.Lerp(0.0f, 45f, (float) index3 / (float) num2);
          Vector3 direction = Quaternion.AngleAxis(num1 + 45f * (float) index2 + num4, Vector3.up) * forward;
          RaycastHit hitInfo;
          if (Physics.Raycast(vector3_1 + Random.onUnitSphere * 0.05f, direction, out hitInfo, 500f, (int) this.VisMask, QueryTriggerInteraction.Ignore))
          {
            num3 += hitInfo.distance;
            a = Mathf.Max(a, hitInfo.distance);
          }
          else
            num3 += 500f;
        }
        this.MaxVis[8 * index1 + index2] = a;
        Vector3 vector3_2 = Quaternion.AngleAxis(45f * (float) index2, Vector3.up) * Vector3.forward;
        this.HitPoints[8 * index1 + index2] = vector3_1 + vector3_2 * this.MaxVis[8 * index1 + index2];
      }
    }
  }

  [ContextMenu("CalcNew")]
  public void CalcNew()
  {
    this.CalcDistsForHeight(this.Heights[0], this.MaxDist_Standing);
    this.CalcDistsForHeight(this.Heights[1], this.MaxDist_Crouching);
    this.CalcDistsForHeight(this.Heights[2], this.MaxDist_Prone);
  }

  private void CalcDistsForHeight(float height, float[] dists)
  {
    Vector3 p1 = this.transform.position + Vector3.up * height;
    this.CalcDistForRingWithOffset(p1, dists, 0.0f, 0);
    this.CalcDistForRingWithOffset(p1, dists, -10f, 8);
    this.CalcDistForRingWithOffset(p1, dists, 10f, 16);
    this.CalcDistForRingWithOffset(p1, dists, -25f, 24);
    this.CalcDistForRingWithOffset(p1, dists, 25f, 32);
    dists[40] = this.GetDistSampleSet(p1, -89f, 0);
    dists[41] = this.GetDistSampleSet(p1, 89f, 0);
  }

  private void CalcDistForRingWithOffset(Vector3 p1, float[] dists, float xRotAmount, int offSet)
  {
    for (int y = 0; y < 8; ++y)
    {
      float distSampleSet = this.GetDistSampleSet(p1, xRotAmount, y);
      dists[offSet + y] = distSampleSet;
    }
  }

  private float GetDistSampleSet(Vector3 p1, float xRotAmount, int y)
  {
    Vector3 forward = Vector3.forward;
    Vector3 vector3 = Quaternion.AngleAxis(xRotAmount, Vector3.right) * forward;
    float num1 = -22.5f;
    int num2 = 60;
    float num3 = 0.0f;
    float a = 0.0f;
    for (int index = 0; index <= num2; ++index)
    {
      float num4 = Mathf.Lerp(0.0f, 45f, (float) index / (float) num2);
      Vector3 direction = Quaternion.AngleAxis(num1 + 45f * (float) y + num4, Vector3.up) * vector3;
      RaycastHit hitInfo;
      if (Physics.Raycast(p1 + Random.onUnitSphere * 0.05f, direction, out hitInfo, 500f, (int) this.VisMask, QueryTriggerInteraction.Ignore))
      {
        a = Mathf.Max(a, hitInfo.distance);
        num3 += hitInfo.distance;
      }
      else
        num3 += 500f;
    }
    return a;
  }

  public enum SampleHeight
  {
    Standing,
    Crouching,
    Prone,
  }
}
