// Decompiled with JetBrains decompiler
// Type: FistVR.AITurretController
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AITurretController : MonoBehaviour
  {
    public Transform root;
    public float XRotSpeed = 1f;
    public float YRotSpeed = 1f;
    public Transform YAxisTransform;
    public bool RotateY;
    public bool ClampY;
    public float RotateYMaxAngle;
    public Transform XAxisTransform;
    public bool RotateX;
    public bool ClampX;
    public float RotateXMaxAngle;
    private Vector3 m_targetDirection = Vector3.forward;
    private Vector3 m_lookPoint = Vector3.zero;
    public AIWeaponSystem WeaponSystem;
    public bool FireAtWill;
    public float AngularCutoff = 5f;

    public void SetFireAtWill(bool b) => this.FireAtWill = b;

    public void UpdateTurretController()
    {
      this.RotateTowardsTarget();
      if (this.FireAtWill)
      {
        Vector3 directionToPoint = this.GetDirectionToPoint(this.m_lookPoint, this.WeaponSystem.Muzzle.position);
        Debug.DrawLine(this.m_lookPoint, this.m_lookPoint + directionToPoint, Color.blue);
        Debug.DrawLine(this.m_lookPoint, this.m_lookPoint + this.WeaponSystem.Muzzle.forward, Color.green);
        if ((double) Vector3.Angle(directionToPoint, this.WeaponSystem.Muzzle.forward) <= (double) this.AngularCutoff)
          this.WeaponSystem.SetShouldFire(true);
        else
          this.WeaponSystem.SetShouldFire(false);
      }
      else
        this.WeaponSystem.SetShouldFire(false);
      this.WeaponSystem.UpdateWeaponSystem();
    }

    private void RotateTowardsTarget()
    {
      if (this.RotateX)
      {
        Quaternion to1 = Quaternion.LookRotation(Vector3.ProjectOnPlane(this.GetDirectionToPoint(this.m_lookPoint, this.XAxisTransform.position), this.XAxisTransform.right), Vector3.up);
        Quaternion to2 = Quaternion.RotateTowards(this.YAxisTransform.rotation, to1, this.RotateXMaxAngle);
        if (this.ClampX)
        {
          this.XAxisTransform.rotation = Quaternion.RotateTowards(this.XAxisTransform.rotation, to2, Time.deltaTime * this.XRotSpeed);
          this.XAxisTransform.localEulerAngles = new Vector3(this.XAxisTransform.localEulerAngles.x, 0.0f, 0.0f);
        }
        else
        {
          this.XAxisTransform.rotation = Quaternion.RotateTowards(this.XAxisTransform.rotation, to1, Time.deltaTime * this.XRotSpeed);
          this.XAxisTransform.localEulerAngles = new Vector3(this.XAxisTransform.localEulerAngles.x, 0.0f, 0.0f);
        }
      }
      if (!this.RotateY)
        return;
      Quaternion to3 = Quaternion.LookRotation(Vector3.ProjectOnPlane(this.GetDirectionToPoint(this.m_lookPoint, this.XAxisTransform.position), this.YAxisTransform.up), Vector3.up);
      Quaternion to4 = Quaternion.RotateTowards(this.root.rotation, to3, this.RotateYMaxAngle);
      if (this.ClampY)
      {
        this.YAxisTransform.rotation = Quaternion.RotateTowards(this.YAxisTransform.rotation, to4, Time.deltaTime * this.YRotSpeed);
        this.YAxisTransform.localEulerAngles = new Vector3(0.0f, this.YAxisTransform.localEulerAngles.y, 0.0f);
      }
      else
      {
        this.YAxisTransform.rotation = Quaternion.RotateTowards(this.YAxisTransform.rotation, to3, Time.deltaTime * this.YRotSpeed);
        this.YAxisTransform.localEulerAngles = new Vector3(0.0f, this.YAxisTransform.localEulerAngles.y, 0.0f);
      }
    }

    public void SetTargetPoint(Vector3 v)
    {
      this.m_lookPoint = v;
      this.m_targetDirection = this.GetDirectionToPoint(v);
    }

    public Vector3 GetDirectionToPoint(Vector3 point) => (point - this.transform.position).normalized;

    public Vector3 GetDirectionToPoint(Vector3 point, Vector3 origin) => (point - origin).normalized;
  }
}
