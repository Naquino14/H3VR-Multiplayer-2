// Decompiled with JetBrains decompiler
// Type: FistVR.AIFireArmMount
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AIFireArmMount : MonoBehaviour
  {
    public AIFireArm FireArm;
    public bool DoesXRot;
    public Transform XRotMount;
    public Transform XMountedTo;
    public float XMinRot;
    public float XMaxRot;
    public float XRotSpeed;
    public bool DoesYRot;
    public Transform YRotMount;
    public Transform YMountedTo;
    public float YMinRot;
    public float YMaxRot;
    public float YRotSpeed;
    private Vector3 m_targetPoint;
    private bool shouldFire;

    public void Awake() => this.m_targetPoint = this.transform.position + this.transform.forward;

    public void SetTargetPoint(Vector3 v) => this.m_targetPoint = v;

    public void SetShouldFire(bool b) => this.shouldFire = b;

    public void Update()
    {
      float num1 = 0.0f;
      if (this.DoesYRot)
      {
        Vector3 to = Vector3.ProjectOnPlane(this.GetDirectionToPoint(this.m_targetPoint, this.YRotMount.position), this.YRotMount.up);
        Vector3 vector3 = Vector3.RotateTowards(this.YMountedTo.forward, -this.YMountedTo.right, (float) ((double) Mathf.Abs(this.YMinRot) * 3.14159274101257 / 180.0), 1f);
        float maxRadiansDelta = (float) ((double) Mathf.Clamp(Vector3.Angle(vector3, to), 0.0f, Mathf.Abs(this.YMinRot - this.YMaxRot)) * 3.14159274101257 / 180.0);
        this.YRotMount.rotation = Quaternion.RotateTowards(this.YRotMount.rotation, Quaternion.LookRotation(Vector3.RotateTowards(vector3, this.YMountedTo.right, maxRadiansDelta, 1f), this.YMountedTo.up), this.YRotSpeed * Time.deltaTime);
      }
      if (this.DoesXRot)
      {
        float num2 = 40f;
        Vector2 angles = Vector2.zero;
        float vel = 30f;
        float grav = 9.81f;
        if ((Object) this.FireArm != (Object) null)
        {
          vel = this.FireArm.TrajectoryMuzzleVelocity;
          grav = this.FireArm.TrajectoryGravityMultiplier * 9.81f;
        }
        if (this.CalculateInclinationsToTarget(this.XRotMount.position, this.m_targetPoint, vel, out angles, grav))
        {
          num2 = angles.x;
        }
        else
        {
          float num3 = num1 + 90f;
        }
        Vector3.ProjectOnPlane(this.GetDirectionToPoint(this.m_targetPoint, this.XRotMount.position), this.XRotMount.right);
        Vector3 vector3 = Vector3.RotateTowards(this.XMountedTo.forward, -this.XMountedTo.up, (float) ((double) Mathf.Abs(this.XMinRot) * 3.14159274101257 / 180.0), 1f);
        float num4 = Vector3.Angle(Vector3.down, this.XMountedTo.forward) - 90f;
        Vector3 to = Vector3.RotateTowards(this.XMountedTo.forward, Vector3.up, (float) (((double) num2 - (double) num4) * 3.14159274101257 / 180.0), 1f);
        float maxRadiansDelta = (float) ((double) Mathf.Clamp(Vector3.Angle(vector3, to), 0.0f, Mathf.Abs(this.XMinRot - this.XMaxRot)) * 3.14159274101257 / 180.0);
        this.XRotMount.rotation = Quaternion.RotateTowards(this.XRotMount.rotation, Quaternion.LookRotation(Vector3.RotateTowards(vector3, this.XMountedTo.up, maxRadiansDelta, 1f), this.XMountedTo.up), this.XRotSpeed * Time.deltaTime);
      }
      if (!((Object) this.FireArm != (Object) null))
        return;
      if (this.shouldFire)
      {
        Vector3 to = Vector3.ProjectOnPlane(this.GetDirectionToPoint(this.m_targetPoint, this.FireArm.Muzzle.position), Vector3.up);
        if ((double) Vector3.Angle(Vector3.ProjectOnPlane(this.FireArm.Muzzle.forward, Vector3.up), to) <= (double) this.FireArm.FiringAngleThreshold)
          this.FireArm.SetShouldFire(true);
        else
          this.FireArm.SetShouldFire(false);
      }
      else
        this.FireArm.SetShouldFire(false);
      this.FireArm.UpdateWeaponSystem();
    }

    public Vector3 GetDirectionToPoint(Vector3 point, Vector3 origin) => (point - origin).normalized;

    public bool CalculateInclinationsToTarget(
      Vector3 start,
      Vector3 end,
      float vel,
      out Vector2 angles,
      float grav)
    {
      angles = Vector2.zero;
      float num1 = Vector2.Distance(new Vector2(start.x, start.z), new Vector2(end.x, end.z));
      float num2 = (float) -((double) start.y - (double) end.y);
      float num3 = grav;
      float num4 = num1 * num1;
      float num5 = vel * vel;
      float f = vel * vel * vel * vel - num3 * (float) ((double) num3 * (double) num4 + 2.0 * (double) num2 * (double) num5);
      if ((double) f < 0.0)
        return false;
      float num6 = num5 - Mathf.Sqrt(f);
      float num7 = num5 + Mathf.Sqrt(f);
      float num8 = num3 * num1;
      float x = Mathf.Atan(num6 / num8) * 57.29578f;
      float y = Mathf.Atan(num7 / num8) * 57.29578f;
      angles = new Vector2(x, y);
      return true;
    }
  }
}
