// Decompiled with JetBrains decompiler
// Type: WhunkCameraSpring
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class WhunkCameraSpring : MonoBehaviour
{
  public Vector3 sphereCenter;
  public float sphereRadius = 0.5f;
  [Range(0.0f, 1f)]
  public float springAppoachSpeed = 0.1f;
  [Range(0.0f, 1f)]
  public float localDamping;
  [Range(0.0f, 1f)]
  public float worldDamping;
  [HideInInspector]
  public Vector3 particlePosition;
  [HideInInspector]
  public Vector3 particleVelocity;
  private Vector3 lastWorldSphereCenter;

  private static float Approach(float rate, float time) => 1f - Mathf.Pow(1f - rate, time);

  private static Vector3 Approach(Vector3 point, Vector3 target, float rate, float time) => Vector3.Lerp(point, target, WhunkCameraSpring.Approach(rate, time));

  private static Vector3 ClampSphere(Vector3 x, Vector3 center, float radius)
  {
    Vector3 vector3 = Vector3.ClampMagnitude(x - center, radius);
    return center + vector3;
  }

  private static Vector3 FromToVelocity(Vector3 from, Vector3 to, float deltaTime) => (to - from) / deltaTime;

  public static void SimulateSpringBall(
    ref Vector3 particlePosition,
    ref Vector3 particleVelocity,
    Vector3 sphereCenter,
    Vector3 sphereVelocity,
    float sphereRadius,
    float deltaTime,
    float springAppoachSpeed = 0.95f,
    float localDamping = 0.1f,
    float worldDamping = 0.0f)
  {
    Vector3 from = particlePosition;
    Vector3 point = particleVelocity;
    WhunkCameraSpring.Approach(point, new Vector3(0.0f, 0.0f, 0.0f), worldDamping, deltaTime * 90f);
    Vector3 vector3 = WhunkCameraSpring.Approach(WhunkCameraSpring.Approach(point, sphereVelocity, localDamping, deltaTime * 90f), WhunkCameraSpring.FromToVelocity(from, sphereCenter, deltaTime), springAppoachSpeed, deltaTime * 90f) * deltaTime;
    Vector3 to = WhunkCameraSpring.ClampSphere(from + vector3, sphereCenter, sphereRadius);
    particlePosition = to;
    particleVelocity = WhunkCameraSpring.FromToVelocity(from, to, deltaTime);
  }

  public Vector3 GetSphereCenterWS()
  {
    Vector3 position = this.sphereCenter;
    Transform parent = this.transform.parent;
    if ((Object) parent != (Object) null)
      position = parent.TransformPoint(position);
    return position;
  }

  public void Tick(float deltaTime)
  {
    Vector3 sphereCenterWs = this.GetSphereCenterWS();
    WhunkCameraSpring.SimulateSpringBall(ref this.particlePosition, ref this.particleVelocity, sphereCenterWs, WhunkCameraSpring.FromToVelocity(this.lastWorldSphereCenter, sphereCenterWs, deltaTime), this.sphereRadius, deltaTime, this.springAppoachSpeed, this.localDamping, this.worldDamping);
    this.lastWorldSphereCenter = sphereCenterWs;
    this.transform.position = this.particlePosition;
  }

  public void Reset()
  {
    Vector3 sphereCenterWs = this.GetSphereCenterWS();
    this.lastWorldSphereCenter = sphereCenterWs;
    this.particlePosition = sphereCenterWs;
    this.transform.position = sphereCenterWs;
    this.particleVelocity = new Vector3(0.0f, 0.0f, 0.0f);
  }

  public void ResetPosition()
  {
    this.particlePosition = this.GetSphereCenterWS();
    this.transform.position = this.particlePosition;
  }

  public void ResetVelocity()
  {
    this.lastWorldSphereCenter = this.GetSphereCenterWS();
    this.particleVelocity = new Vector3(0.0f, 0.0f, 0.0f);
  }

  private void OnEnable() => this.lastWorldSphereCenter = this.GetSphereCenterWS();

  private void FixedUpdate() => this.Tick(Time.deltaTime);

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = new Color(0.3f, 0.9f, 0.1f);
    Gizmos.DrawWireSphere(this.GetSphereCenterWS(), this.sphereRadius);
  }
}
