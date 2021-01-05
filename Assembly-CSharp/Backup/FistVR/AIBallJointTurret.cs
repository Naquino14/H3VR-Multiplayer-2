// Decompiled with JetBrains decompiler
// Type: FistVR.AIBallJointTurret
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AIBallJointTurret : AIBodyPiece
  {
    public AIClunk Clunk;
    public Transform Root;
    public Transform AimingTransform;
    public Rigidbody RB;
    public AISensorSystem SensorSystem;
    public float RotSpeed;
    private float m_currentRotSpeed;
    public float MaxRotAngle;
    private Vector3 m_targetDirection = Vector3.forward;
    private Vector3 m_lookPoint = Vector3.zero;
    public float TimeTilPlatesReset = 10f;
    public AIFireArmMount[] FireArms;
    private bool IsPermanentlyDisabled;
    public bool DoesRotate = true;
    public bool FireAtWill;
    public float FiringAngularCutoff = 5f;
    private float m_damageReset;

    public void SetTargetPoint(Vector3 v)
    {
      if (this.IsPlateDamaged)
        v += Random.onUnitSphere;
      this.m_lookPoint = v;
      this.m_targetDirection = this.GetDirectionToPoint(v);
      for (int index = 0; index < this.FireArms.Length; ++index)
      {
        if ((Object) this.FireArms[index] != (Object) null)
          this.FireArms[index].SetTargetPoint(this.m_lookPoint);
      }
    }

    public void SetFireAtWill(bool b)
    {
      this.FireAtWill = b;
      for (int index = 0; index < this.FireArms.Length; ++index)
      {
        if ((Object) this.FireArms[index] != (Object) null)
          this.FireArms[index].SetShouldFire(b);
      }
    }

    public void SetFiringAngleCutoff(float f) => this.FiringAngularCutoff = f;

    public override void Awake()
    {
      base.Awake();
      this.SetTargetPoint(this.AimingTransform.position + this.AimingTransform.forward * 2f);
    }

    public override void Update()
    {
      base.Update();
      if (this.IsPlateDisabled || this.IsPermanentlyDisabled)
      {
        this.m_currentRotSpeed = 0.0f;
        this.SetFireAtWill(false);
        for (int index = 0; index < this.FireArms.Length; ++index)
          this.FireArms[index] = (AIFireArmMount) null;
        this.Clunk.Die();
      }
      else if (this.IsPlateDamaged)
      {
        this.m_currentRotSpeed = Random.Range(0.0f, this.RotSpeed * 0.5f);
        this.m_damageReset -= Time.deltaTime;
        if ((double) this.m_damageReset <= 0.0)
          this.ResetAllPlates();
      }
      else
        this.m_currentRotSpeed = this.RotSpeed;
      bool flag = true;
      for (int index = 0; index < this.SensorSystem.Sensors.Length; ++index)
      {
        if ((Object) this.SensorSystem.Sensors[index] != (Object) null)
          flag = false;
      }
      if (!flag)
        return;
      this.IsPermanentlyDisabled = true;
    }

    public override void DestroyEvent()
    {
      this.IsPermanentlyDisabled = true;
      Debug.Log((object) "Destroyed");
      base.DestroyEvent();
    }

    public override bool SetPlateDamaged(bool b)
    {
      if (!base.SetPlateDamaged(b))
        return false;
      this.m_damageReset = this.TimeTilPlatesReset;
      return true;
    }

    public override bool SetPlateDisabled(bool b) => base.SetPlateDisabled(b);

    public override void FixedUpdate()
    {
      base.FixedUpdate();
      if (!this.DoesRotate)
        return;
      this.UpdateTurretRot();
    }

    private void UpdateTurretRot() => this.transform.rotation = Quaternion.RotateTowards(this.RB.rotation, Quaternion.LookRotation(Vector3.RotateTowards(Vector3.ProjectOnPlane(this.m_targetDirection, this.Root.up).normalized, this.m_targetDirection, (float) ((double) this.MaxRotAngle * 3.14159274101257 / 180.0), 1f), Vector3.up), this.m_currentRotSpeed * Time.deltaTime);

    public Vector3 GetDirectionToPoint(Vector3 point) => (point - this.AimingTransform.position).normalized;

    public Vector3 GetDirectionToPoint(Vector3 point, Vector3 origin) => (point - origin).normalized;
  }
}
