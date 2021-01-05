// Decompiled with JetBrains decompiler
// Type: WaggleJoint
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class WaggleJoint : MonoBehaviour
{
  private const float startTouchingAngle = 0.5f;
  private const float stopTouchingAngle = 10f;
  [Tooltip("The distance the particle is clamped to, scale this up to make it less sensitive to changes.")]
  public float distanceLimit = 0.25f;
  public float angleLimitLeft = 45f;
  public float angleLimitRight = 45f;
  [Tooltip("Multiplier for the physics gravity effecting the particle. Good for dealing with stiff joints or low gravity values.")]
  public float gravityScale = 1f;
  [Tooltip("Pulls the particle back to the hinge direction")]
  public bool useSpring;
  [Tooltip("Rate at which the particle approaches the hinge direction, eg: 0.95 mean's 95% closer each second.")]
  public float springApproachRate = 0.95f;
  [Tooltip("Rate at which the particle loses velocity, eg: 0.5 mean's 50% slower each second.")]
  public float damping;
  [Tooltip("This transform will LookAt() the particle being simulated.")]
  public Transform hingeGraphic;
  public bool ManualExecution;
  [Tooltip("Puts a cooldown on how often the OnHitLimit event can be fired.")]
  public float onHitLimitRefireLimit = 0.1f;
  private Vector3 particlePos;
  private Vector3 particleVel;
  private bool wasTouchingLimit;
  private bool wasTouchingRight;
  private float lastOnHitLimitTime = float.MinValue;

  private static float Approach(float rate, float time) => 1f - Mathf.Pow(1f - rate, time);

  private static Vector3 Approach(Vector3 point, Vector3 target, float rate, float time) => Vector3.Lerp(point, target, WaggleJoint.Approach(rate, time));

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

  private static bool DetectOnHitLimit(
    ref bool wasTouching,
    ref bool wasTouchingMax,
    float min,
    float max,
    float currentValue,
    float limit,
    float releaseLimit)
  {
    float a = currentValue - min;
    float b = max - a;
    float num1 = Mathf.Min(a, b);
    bool flag1 = (double) a > (double) b;
    bool flag2 = (double) num1 < (double) limit;
    bool flag3 = wasTouching;
    bool flag4 = wasTouchingMax;
    float num2 = !flag4 ? a : b;
    bool flag5 = false;
    if (flag2 && flag1 != flag4)
    {
      flag3 = true;
      flag4 = flag1;
      flag5 = true;
    }
    if ((double) num2 > (double) releaseLimit && wasTouching)
      flag3 = false;
    if (flag2 && !wasTouching)
    {
      flag3 = true;
      flag4 = flag1;
      flag5 = true;
    }
    wasTouching = flag3;
    wasTouchingMax = flag4;
    return flag5;
  }

  private void Start()
  {
    this.particlePos = this.transform.position + this.hingeGraphic.forward * this.distanceLimit;
    this.particleVel = Vector3.zero;
  }

  public void ResetParticlePos()
  {
    this.particlePos = this.transform.position + this.hingeGraphic.forward * this.distanceLimit;
    this.particleVel = Vector3.zero;
  }

  private void Update()
  {
    if (this.ManualExecution)
      return;
    this.Execute();
  }

  public void Execute()
  {
    Transform transform = this.transform;
    Vector3 position = transform.position;
    Vector3 up = transform.up;
    Vector3 right = transform.right;
    Vector3 forward = transform.forward;
    float deltaTime = Time.deltaTime;
    Vector3 vector3_1 = Physics.gravity * this.gravityScale;
    Vector3 particlePos = this.particlePos;
    Vector3 vector3_2 = WaggleJoint.Approach(this.particleVel, Vector3.zero, this.damping, deltaTime) + vector3_1 * deltaTime;
    Vector3 point = particlePos + vector3_2 * deltaTime;
    Vector3 vector3_3 = point;
    if (this.useSpring)
      point = WaggleJoint.Approach(point, position + up * this.distanceLimit, this.springApproachRate, deltaTime);
    Vector3 vector3_4 = WaggleJoint.ProjectOnHinge(point, position, right, up, this.distanceLimit, this.angleLimitLeft, this.angleLimitRight);
    this.particlePos = vector3_4;
    this.particleVel = (vector3_4 - particlePos) / deltaTime;
    Vector3 normalized = (vector3_4 - position).normalized;
    Vector3 to1 = Quaternion.AngleAxis(this.angleLimitLeft, right) * up;
    Vector3 to2 = Quaternion.AngleAxis(-this.angleLimitRight, right) * up;
    float num1 = Vector3.Angle(normalized, to1);
    float num2 = Vector3.Angle(normalized, to2);
    bool flag = WaggleJoint.DetectOnHitLimit(ref this.wasTouchingLimit, ref this.wasTouchingRight, -this.angleLimitLeft, this.angleLimitRight, (double) num2 >= (double) num1 ? -num1 : num2, 0.5f, 10f);
    float angularVelocity = Vector3.Angle((particlePos - position).normalized, (vector3_3 - position).normalized) / deltaTime;
    if (flag && (double) Time.timeSinceLevelLoad >= (double) this.lastOnHitLimitTime + (double) this.onHitLimitRefireLimit)
    {
      this.OnHitLimit(angularVelocity);
      this.lastOnHitLimitTime = Time.timeSinceLevelLoad;
    }
    if (!((Object) this.hingeGraphic != (Object) null))
      return;
    this.hingeGraphic.LookAt(this.particlePos, Vector3.Cross(this.hingeGraphic.forward, right));
  }

  private void OnHitLimit(float angularVelocity)
  {
  }

  private void OnDrawGizmosSelected()
  {
    if (Application.isPlaying)
    {
      Gizmos.color = new Color(0.1f, 0.3f, 0.9f);
      Gizmos.DrawWireCube(this.particlePos, Vector3.one * 0.025f);
      Gizmos.color = new Color(0.1f, 0.3f, 0.9f, 0.5f);
      Gizmos.DrawCube(this.particlePos, Vector3.one * 0.025f);
      float num = Time.timeSinceLevelLoad - this.lastOnHitLimitTime;
      Gizmos.color = new Color(0.1f, 0.7f, 0.9f, Mathf.Clamp01((float) ((0.5 - (double) num) * 2.0)));
      Gizmos.DrawWireCube(this.particlePos, Vector3.one * (float) (0.100000001490116 * ((double) num + 0.5)));
    }
    Vector3 center = this.transform.position + this.transform.up * this.distanceLimit;
    Gizmos.color = new Color(0.7f, 0.9f, 0.1f);
    Gizmos.DrawWireCube(center, Vector3.one * 0.02f);
    Gizmos.color = new Color(0.7f, 0.9f, 0.1f, 0.5f);
    Gizmos.DrawCube(center, Vector3.one * 0.02f);
    Gizmos.color = new Color(0.9f, 0.7f, 0.1f);
    Gizmos.DrawRay(this.transform.position, Quaternion.AngleAxis(this.angleLimitLeft, this.transform.right) * this.transform.up * this.distanceLimit);
    Gizmos.DrawRay(this.transform.position, Quaternion.AngleAxis(-this.angleLimitRight, this.transform.right) * this.transform.up * this.distanceLimit);
  }
}
