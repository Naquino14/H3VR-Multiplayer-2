// Decompiled with JetBrains decompiler
// Type: MedigunBeam
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

[RequireComponent(typeof (LineRenderer))]
public class MedigunBeam : MonoBehaviour
{
  private bool wasBeamEnabled;
  public bool beamEnabled;
  private bool wasElectricityEnabled;
  public bool electricityEnabled;
  public Transform target;
  public LineRenderer lineRenderer;
  public GameObject Electricity;
  public int segments = 256;
  public float startTangentApproachRate = 0.9f;
  public float endTangentApproachRate = 0.1f;
  private Vector3 startTangent;
  private Vector3 endTangent;
  private Vector3[] positions;
  private Material lineMaterial;
  [ColorUsage(true, true, 0.0f, 8f, 0.0f, 8f)]
  public Color[] LineColors;
  public GameObject[] ElectricityByTeam;
  private float lineMaterialCullTime = 2f;
  private bool wasVisible;
  private float endTime = -100000f;
  private bool initialized;

  private static Vector3 Lerp(Vector3 a, Vector3 b, float t) => Vector3.Lerp(a, b, t);

  private static Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
  {
    float num1 = (b.x - a.x) * t + a.x;
    float num2 = (b.y - a.y) * t + a.y;
    float num3 = (b.z - a.z) * t + a.z;
    float num4 = (c.x - b.x) * t + b.x;
    float num5 = (c.y - b.y) * t + b.y;
    float num6 = (c.z - b.z) * t + b.z;
    float num7 = (d.x - c.x) * t + c.x;
    float num8 = (d.y - c.y) * t + c.y;
    float num9 = (d.z - c.z) * t + c.z;
    float num10 = (num4 - num1) * t + num1;
    float num11 = (num5 - num2) * t + num2;
    float num12 = (num6 - num3) * t + num3;
    float num13 = (num7 - num4) * t + num4;
    float num14 = (num8 - num5) * t + num5;
    float num15 = (num9 - num6) * t + num6;
    return new Vector3((num13 - num10) * t + num10, (num14 - num11) * t + num11, (num15 - num12) * t + num12);
  }

  private static float Approach(float rate, float time) => 1f - Mathf.Pow(1f - rate, time);

  private static float Approach(float rate, float time, float referenceFramerate) => 1f - Mathf.Pow(1f - rate, time * referenceFramerate);

  private static void ComputeTangents(
    Vector3 start,
    Vector3 end,
    Vector3 startDirection,
    out Vector3 startTangentPosition,
    out Vector3 endTangentPosition)
  {
    Vector3 vector3 = end - start;
    float magnitude = vector3.magnitude;
    float t = (float) (1.0 - ((double) Vector3.Dot(Vector3.Normalize(vector3), startDirection) * 0.5 + 0.5));
    startTangentPosition = start + startDirection * Mathf.Lerp(magnitude * 0.3333333f, magnitude, t);
    endTangentPosition = (startTangentPosition + end) * 0.5f;
  }

  public void StartBeam(Transform target)
  {
    MedigunBeam.ComputeTangents(this.transform.position, target.position, this.transform.forward, out this.startTangent, out this.endTangent);
    this.beamEnabled = true;
    this.target = target;
  }

  public void StopBeam() => this.beamEnabled = false;

  public void SetLineColor(int i) => this.lineMaterial.SetColor("_Color", this.LineColors[i]);

  public void SetElectricityColor(int i) => this.Electricity = this.ElectricityByTeam[i];

  public void Initialize()
  {
    MedigunBeam.ComputeTangents(this.transform.position, this.target.position, this.transform.forward, out this.startTangent, out this.endTangent);
    if ((Object) this.lineRenderer == (Object) null)
      this.lineRenderer = this.GetComponentInChildren<LineRenderer>();
    if ((Object) this.lineRenderer == (Object) null)
    {
      Debug.LogError((object) "No LineRenderer attached to MedicBeam object", (Object) this);
    }
    else
    {
      this.positions = new Vector3[this.segments];
      this.lineRenderer.positionCount = this.segments;
      this.lineMaterial = this.lineRenderer.material;
      this.lineRenderer.sharedMaterial = this.lineMaterial;
      float num1 = this.lineMaterial.GetFloat("_BlendRange");
      float num2 = this.lineMaterial.GetFloat("_BlendTime");
      this.lineMaterialCullTime = num2 + num2 * num1;
      this.initialized = true;
    }
  }

  private void Update()
  {
    if ((Object) this.target == (Object) null)
      return;
    if (!this.initialized)
      this.Initialize();
    if (!this.wasBeamEnabled && this.beamEnabled)
    {
      this.lineMaterial.SetFloat("_StartTime", Time.timeSinceLevelLoad);
      this.endTime = float.MaxValue;
      this.lineMaterial.SetFloat("_EndTime", this.endTime);
    }
    else if (this.wasBeamEnabled && !this.beamEnabled)
    {
      this.endTime = Time.timeSinceLevelLoad;
      this.lineMaterial.SetFloat("_EndTime", this.endTime);
    }
    if ((Object) this.target == (Object) null)
      this.lineMaterial.SetFloat("_EndTime", float.MinValue);
    this.wasBeamEnabled = this.beamEnabled;
    this.wasElectricityEnabled = this.electricityEnabled;
    Vector3 position1 = this.transform.position;
    Vector3 position2 = this.target.position;
    Vector3 startTangentPosition;
    Vector3 endTangentPosition;
    MedigunBeam.ComputeTangents(position1, position2, this.transform.forward, out startTangentPosition, out endTangentPosition);
    this.startTangent = Vector3.Lerp(this.startTangent, startTangentPosition, MedigunBeam.Approach(this.startTangentApproachRate, Time.deltaTime));
    this.endTangent = Vector3.Lerp(this.endTangent, endTangentPosition, MedigunBeam.Approach(this.endTangentApproachRate, Time.deltaTime));
    for (int index = 0; index < this.segments; ++index)
      this.positions[index] = MedigunBeam.Bezier(position1, this.startTangent, this.endTangent, position2, (float) index / (float) (this.segments - 1));
    this.lineRenderer.SetPositions(this.positions);
    bool flag = (double) Time.timeSinceLevelLoad < (double) this.endTime + (double) this.lineMaterialCullTime;
    if (flag && !this.wasVisible)
      this.lineRenderer.enabled = true;
    else if (!flag && this.wasVisible)
      this.lineRenderer.enabled = false;
    this.wasVisible = flag;
  }

  private void OnDrawGizmos()
  {
    if ((Object) this.target == (Object) null || Application.isPlaying)
      return;
    Vector3 position1 = this.transform.position;
    Vector3 position2 = this.target.position;
    float num = 0.05f;
    Gizmos.color = new Color(0.1f, 0.3f, 0.9f);
    Gizmos.DrawWireCube(this.startTangent, Vector3.one * num);
    Gizmos.DrawWireCube(this.endTangent, Vector3.one * num);
    Gizmos.DrawWireCube(position1, Vector3.one * num);
    Gizmos.DrawWireCube(position2, Vector3.one * num);
    Gizmos.color = new Color(0.1f, 0.3f, 0.9f, 0.25f);
    Gizmos.DrawLine(position1, this.startTangent);
    Gizmos.DrawLine(this.startTangent, this.endTangent);
    Gizmos.DrawLine(this.endTangent, position2);
    Gizmos.DrawCube(position1, Vector3.one * num);
    Gizmos.DrawCube(position2, Vector3.one * num);
    Gizmos.DrawCube(this.startTangent, Vector3.one * num);
    Gizmos.DrawCube(this.endTangent, Vector3.one * num);
    Gizmos.color = new Color(0.7f, 0.9f, 0.1f);
    for (int index = 0; index < this.segments; ++index)
    {
      float t1 = (float) index / (float) this.segments;
      float t2 = t1 + 1f / (float) this.segments;
      Gizmos.DrawLine(MedigunBeam.Bezier(position1, this.startTangent, this.endTangent, position2, t1), MedigunBeam.Bezier(position1, this.startTangent, this.endTangent, position2, t2));
    }
  }
}
