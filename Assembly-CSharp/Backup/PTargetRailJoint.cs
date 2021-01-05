// Decompiled with JetBrains decompiler
// Type: PTargetRailJoint
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class PTargetRailJoint : MonoBehaviour
{
  public bool travelToEnd = true;
  public float travelSpeed = 1f;
  public float particleDistance = 1.5f;
  public float angleLimitBack = 45f;
  public float angleLimitFront = 45f;
  public float gravityScale = 1f;
  public float damping = 0.05f;
  public Transform targetTransform;
  private Vector3 particlePosition;
  private Vector3 particleVelocity;
  private Vector3 worldSpaceStartPosition;
  private Vector3 worldSpaceRailDirection;
  private Quaternion targetInitialRoation;
  private float currentMoveDistance;
  private float targetOffsetDistance;

  private static float Approach(float rate, float time) => 1f - Mathf.Pow(1f - rate, time);

  private static Vector3 Approach(Vector3 point, Vector3 target, float rate, float time) => Vector3.Lerp(point, target, PTargetRailJoint.Approach(rate, time));

  private static Vector3 ProjectOnHinge(
    Vector3 point,
    Vector3 hingePivot,
    Vector3 hingeRotationAxis,
    Vector3 hingeDirection,
    float distanceLimit,
    float angleLimitLeft,
    float angleLimitRight)
  {
    Vector3 rhs = Vector3.Cross(hingeRotationAxis, hingeDirection);
    Vector3 vector3 = Vector3.ProjectOnPlane(point - hingePivot, hingeRotationAxis);
    vector3 = vector3.normalized * distanceLimit;
    Vector3 normalized = vector3.normalized;
    float num1 = Vector3.Angle(normalized, hingeDirection);
    bool flag = (double) Vector3.Dot(normalized, rhs) > 0.0;
    float num2 = Mathf.Min(angleLimitLeft - num1, 0.0f);
    float num3 = -Mathf.Min(angleLimitRight - num1, 0.0f);
    return Quaternion.AngleAxis(!flag ? num3 : num2, hingeRotationAxis) * vector3 + hingePivot;
  }

  public void GoToDistance(float distance) => this.currentMoveDistance = distance;

  public void SetHeight(float height)
  {
    Vector3 position = this.transform.position;
    position.y = height;
    this.transform.position = position;
    this.worldSpaceStartPosition.y = height;
    this.particleVelocity = Vector3.zero;
    this.ResetSimulation();
  }

  public void ResetSimulation()
  {
    Transform transform = this.transform;
    this.particlePosition = transform.position - transform.up * this.particleDistance;
    this.particleVelocity = Vector3.zero;
  }

  private void Start()
  {
    Transform transform = this.transform;
    this.ResetSimulation();
    this.worldSpaceStartPosition = transform.position;
    this.currentMoveDistance = 0.0f;
    this.worldSpaceRailDirection = -transform.forward;
    this.targetOffsetDistance = -this.targetTransform.localPosition.y;
    this.targetInitialRoation = this.targetTransform.localRotation;
    Vector3 right = transform.right;
    Vector3 normalized = (this.particlePosition - transform.position).normalized;
    this.targetTransform.position = this.transform.position + normalized * this.targetOffsetDistance;
    this.targetTransform.rotation = Quaternion.LookRotation(Vector3.Cross(right, -normalized), -normalized) * this.targetInitialRoation;
  }

  private void Update()
  {
    float deltaTime = Time.deltaTime;
    Transform transform = this.transform;
    transform.position = Vector3.MoveTowards(transform.position, this.worldSpaceStartPosition + this.worldSpaceRailDirection * this.currentMoveDistance, this.travelSpeed * deltaTime);
    Vector3 position = transform.position;
    Vector3 up = transform.up;
    Vector3 right = transform.right;
    Vector3 forward = transform.forward;
    Vector3 vector3_1 = Physics.gravity * this.gravityScale;
    Vector3 particlePosition = this.particlePosition;
    Vector3 vector3_2 = PTargetRailJoint.Approach(this.particleVelocity, Vector3.zero, this.damping, deltaTime * 90f) + vector3_1 * deltaTime;
    Vector3 vector3_3 = PTargetRailJoint.ProjectOnHinge(particlePosition + vector3_2 * deltaTime, position, right, -up, this.particleDistance, this.angleLimitBack, this.angleLimitFront);
    this.particlePosition = vector3_3;
    this.particleVelocity = (vector3_3 - particlePosition) / deltaTime;
    Vector3 normalized = (this.particlePosition - position).normalized;
    this.targetTransform.position = position + normalized * this.targetOffsetDistance;
    this.targetTransform.rotation = Quaternion.LookRotation(Vector3.Cross(right, -normalized), -normalized) * this.targetInitialRoation;
  }

  private void OnDrawGizmosSelected()
  {
    if (Application.isPlaying)
    {
      Gizmos.color = new Color(0.1f, 0.3f, 0.9f);
      Gizmos.DrawWireCube(this.particlePosition, Vector3.one * 0.025f);
      Gizmos.color = new Color(0.1f, 0.3f, 0.9f, 0.5f);
      Gizmos.DrawCube(this.particlePosition, Vector3.one * 0.025f);
    }
    Vector3 center = this.transform.position - this.transform.up * this.particleDistance;
    Gizmos.color = new Color(0.7f, 0.9f, 0.1f);
    Gizmos.DrawWireCube(center, Vector3.one * 0.02f);
    Gizmos.color = new Color(0.7f, 0.9f, 0.1f, 0.5f);
    Gizmos.DrawCube(center, Vector3.one * 0.02f);
    Gizmos.color = new Color(0.9f, 0.7f, 0.1f);
    Gizmos.DrawRay(this.transform.position, Quaternion.AngleAxis(this.angleLimitBack, this.transform.right) * -this.transform.up * this.particleDistance);
    Gizmos.DrawRay(this.transform.position, Quaternion.AngleAxis(-this.angleLimitFront, this.transform.right) * -this.transform.up * this.particleDistance);
  }
}
