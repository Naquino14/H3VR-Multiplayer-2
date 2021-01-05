// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.TeleportArc
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Rendering;

namespace Valve.VR.InteractionSystem
{
  public class TeleportArc : MonoBehaviour
  {
    public int segmentCount = 60;
    public float thickness = 0.01f;
    [Tooltip("The amount of time in seconds to predict the motion of the projectile.")]
    public float arcDuration = 3f;
    [Tooltip("The amount of time in seconds between each segment of the projectile.")]
    public float segmentBreak = 0.025f;
    [Tooltip("The speed at which the line segments of the arc move.")]
    public float arcSpeed = 0.2f;
    public Material material;
    [HideInInspector]
    public int traceLayerMask;
    private LineRenderer[] lineRenderers;
    private float arcTimeOffset;
    private float prevThickness;
    private int prevSegmentCount;
    private bool showArc = true;
    private Vector3 startPos;
    private Vector3 projectileVelocity;
    private bool useGravity = true;
    private Transform arcObjectsTransfrom;
    private bool arcInvalid;
    private float scale = 1f;

    private void Start() => this.arcTimeOffset = Time.time;

    private void Update()
    {
      this.scale = Player.instance.transform.lossyScale.x;
      if ((double) this.thickness == (double) this.prevThickness && this.segmentCount == this.prevSegmentCount)
        return;
      this.CreateLineRendererObjects();
      this.prevThickness = this.thickness;
      this.prevSegmentCount = this.segmentCount;
    }

    private void CreateLineRendererObjects()
    {
      if ((Object) this.arcObjectsTransfrom != (Object) null)
        Object.Destroy((Object) this.arcObjectsTransfrom.gameObject);
      this.arcObjectsTransfrom = new GameObject("ArcObjects").transform;
      this.arcObjectsTransfrom.SetParent(this.transform);
      this.lineRenderers = new LineRenderer[this.segmentCount];
      for (int index = 0; index < this.segmentCount; ++index)
      {
        GameObject gameObject = new GameObject("LineRenderer_" + (object) index);
        gameObject.transform.SetParent(this.arcObjectsTransfrom);
        this.lineRenderers[index] = gameObject.AddComponent<LineRenderer>();
        this.lineRenderers[index].receiveShadows = false;
        this.lineRenderers[index].reflectionProbeUsage = ReflectionProbeUsage.Off;
        this.lineRenderers[index].lightProbeUsage = LightProbeUsage.Off;
        this.lineRenderers[index].shadowCastingMode = ShadowCastingMode.Off;
        this.lineRenderers[index].material = this.material;
        this.lineRenderers[index].startWidth = this.thickness * this.scale;
        this.lineRenderers[index].endWidth = this.thickness * this.scale;
        this.lineRenderers[index].enabled = false;
      }
    }

    public void SetArcData(
      Vector3 position,
      Vector3 velocity,
      bool gravity,
      bool pointerAtBadAngle)
    {
      this.startPos = position;
      this.projectileVelocity = velocity;
      this.useGravity = gravity;
      if (this.arcInvalid && !pointerAtBadAngle)
        this.arcTimeOffset = Time.time;
      this.arcInvalid = pointerAtBadAngle;
    }

    public void Show()
    {
      this.showArc = true;
      if (this.lineRenderers != null)
        return;
      this.CreateLineRendererObjects();
    }

    public void Hide()
    {
      if (this.showArc)
        this.HideLineSegments(0, this.segmentCount);
      this.showArc = false;
    }

    public bool DrawArc(out RaycastHit hitInfo)
    {
      float num1 = this.arcDuration / (float) this.segmentCount;
      float num2 = (Time.time - this.arcTimeOffset) * this.arcSpeed;
      if ((double) num2 > (double) num1 + (double) this.segmentBreak)
      {
        this.arcTimeOffset = Time.time;
        num2 = 0.0f;
      }
      float startTime = num2;
      float projectileCollision = this.FindProjectileCollision(out hitInfo);
      if (this.arcInvalid)
      {
        this.lineRenderers[0].enabled = true;
        this.lineRenderers[0].SetPosition(0, this.GetArcPositionAtTime(0.0f));
        this.lineRenderers[0].SetPosition(1, this.GetArcPositionAtTime((double) projectileCollision >= (double) num1 ? num1 : projectileCollision));
        this.HideLineSegments(1, this.segmentCount);
      }
      else
      {
        int num3 = 0;
        if ((double) startTime > (double) this.segmentBreak)
        {
          float endTime = num2 - this.segmentBreak;
          if ((double) projectileCollision < (double) endTime)
            endTime = projectileCollision;
          this.DrawArcSegment(0, 0.0f, endTime);
          num3 = 1;
        }
        bool flag = false;
        int num4 = 0;
        int index;
        if ((double) startTime < (double) projectileCollision)
        {
          for (index = num3; index < this.segmentCount; ++index)
          {
            float endTime = startTime + num1;
            if ((double) endTime >= (double) this.arcDuration)
            {
              endTime = this.arcDuration;
              flag = true;
            }
            if ((double) endTime >= (double) projectileCollision)
            {
              endTime = projectileCollision;
              flag = true;
            }
            this.DrawArcSegment(index, startTime, endTime);
            startTime += num1 + this.segmentBreak;
            if (flag || (double) startTime >= (double) this.arcDuration || (double) startTime >= (double) projectileCollision)
              break;
          }
        }
        else
          index = num4 - 1;
        this.HideLineSegments(index + 1, this.segmentCount);
      }
      return (double) projectileCollision != 3.40282346638529E+38;
    }

    private void DrawArcSegment(int index, float startTime, float endTime)
    {
      this.lineRenderers[index].enabled = true;
      this.lineRenderers[index].SetPosition(0, this.GetArcPositionAtTime(startTime));
      this.lineRenderers[index].SetPosition(1, this.GetArcPositionAtTime(endTime));
    }

    public void SetColor(Color color)
    {
      for (int index = 0; index < this.segmentCount; ++index)
      {
        this.lineRenderers[index].startColor = color;
        this.lineRenderers[index].endColor = color;
      }
    }

    private float FindProjectileCollision(out RaycastHit hitInfo)
    {
      float num1 = this.arcDuration / (float) this.segmentCount;
      float time1 = 0.0f;
      hitInfo = new RaycastHit();
      Vector3 vector3 = this.GetArcPositionAtTime(time1);
      for (int index = 0; index < this.segmentCount; ++index)
      {
        float time2 = time1 + num1;
        Vector3 arcPositionAtTime = this.GetArcPositionAtTime(time2);
        if (Physics.Linecast(vector3, arcPositionAtTime, out hitInfo, this.traceLayerMask) && (Object) hitInfo.collider.GetComponent<IgnoreTeleportTrace>() == (Object) null)
        {
          Util.DrawCross(hitInfo.point, Color.red, 0.5f);
          float num2 = Vector3.Distance(vector3, arcPositionAtTime);
          return time1 + num1 * (hitInfo.distance / num2);
        }
        time1 = time2;
        vector3 = arcPositionAtTime;
      }
      return float.MaxValue;
    }

    public Vector3 GetArcPositionAtTime(float time)
    {
      Vector3 vector3 = !this.useGravity ? Vector3.zero : Physics.gravity;
      return this.startPos + (this.projectileVelocity * time + 0.5f * time * time * vector3) * this.scale;
    }

    private void HideLineSegments(int startSegment, int endSegment)
    {
      if (this.lineRenderers == null)
        return;
      for (int index = startSegment; index < endSegment; ++index)
        this.lineRenderers[index].enabled = false;
    }
  }
}
