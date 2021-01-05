// Decompiled with JetBrains decompiler
// Type: FistVR.PDFCS
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class PDFCS : MonoBehaviour
  {
    public PD PD;
    public PDFCSConfig FCSConfig;
    public Transform Root;
    public Rigidbody RB;
    public Transform Cam;
    public List<PDFCS.Thruster> LinearThrusters;
    public List<PDFCS.Thruster> AngularThrusters;
    public List<PDFCS.Thruster> Afterburners;
    private bool m_isStabilizationActive = true;
    private bool m_fireAfterBurners;
    private Vector3 localLinearControlMag = Vector3.zero;
    private Vector3 m_desiredLinearVelocityWorld = Vector3.zero;
    private Vector3 localAngularControlMag = Vector3.zero;
    private Vector3 m_desiredAngularVelocity = Vector3.zero;
    public LayerMask LM_GroundCheck;
    private RaycastHit m_hit;
    public float DistFromGround = 2f;
    private float cx;
    private float cy;
    private float cz;

    private void Start()
    {
      this.RB.maxAngularVelocity = 12f;
      for (int index = 0; index < this.LinearThrusters.Count; ++index)
        this.LinearThrusters[index].Init();
      for (int index = 0; index < this.AngularThrusters.Count; ++index)
        this.AngularThrusters[index].Init();
      for (int index = 0; index < this.Afterburners.Count; ++index)
        this.Afterburners[index].Init();
    }

    private void Update() => this.FlyByWire();

    private void FlyByWire()
    {
      if (!Input.GetButtonDown("Left Stick Click"))
        ;
      this.m_fireAfterBurners = Input.GetButton("Left Stick Click");
      this.localLinearControlMag = Vector3.zero;
      this.localLinearControlMag.x = this.GetSignedSquare(Input.GetAxis("LeftStickXAxis")) * 20f;
      this.localLinearControlMag.y = this.GetSignedSquare(Input.GetAxis("D-Pad Y Axis")) * 20f;
      this.localLinearControlMag.z = this.GetSignedSquare(-Input.GetAxis("LeftStickYAxis")) * 40f;
      if (this.m_fireAfterBurners)
        this.localLinearControlMag.z = 100f;
      this.m_desiredLinearVelocityWorld = this.transform.TransformVector(this.localLinearControlMag);
      this.localAngularControlMag = Vector3.zero;
      this.localAngularControlMag.x = this.GetSignedSquare(Input.GetAxis("RightStickYAxis"));
      this.localAngularControlMag.y = this.GetSignedSquare(Input.GetAxis("RightStickXAxis"));
      if (Input.GetButton("Left Bumper"))
        ++this.localAngularControlMag.z;
      if (Input.GetButton("Right Bumper"))
        --this.localAngularControlMag.z;
      this.localAngularControlMag *= 5f;
      this.cx = Mathf.Lerp(this.cx, this.localAngularControlMag.x * 3f, Time.deltaTime * 1f);
      this.cy = Mathf.Lerp(this.cy, this.localAngularControlMag.y * 3f, Time.deltaTime * 1f);
      this.cz = Mathf.Lerp(this.cz, this.localAngularControlMag.z * 6f, Time.deltaTime * 1f);
      this.Cam.localEulerAngles = new Vector3(this.cx, this.cy, this.cz);
      this.m_desiredAngularVelocity = this.localAngularControlMag;
    }

    private void FixedUpdate() => this.ThrustControl();

    private void ThrustControl()
    {
      float deltaTime = Time.deltaTime;
      this.DistFromGround = !Physics.Raycast(this.transform.position, Vector3.down, out this.m_hit, 100f, (int) this.LM_GroundCheck, QueryTriggerInteraction.Ignore) ? 100f : this.m_hit.distance;
      float num = 1f + (float) ((1.0 - (double) Mathf.Clamp(this.DistFromGround, 0.0f, 1f)) * 50.0);
      Vector3 vector3_1 = this.transform.InverseTransformVector(this.m_desiredLinearVelocityWorld);
      Vector3 velocity = this.RB.velocity;
      Vector3 vector3_2 = !this.m_isStabilizationActive ? vector3_1 - this.transform.InverseTransformDirection(velocity) : vector3_1 - this.transform.InverseTransformVector(Physics.gravity * num) - this.transform.InverseTransformDirection(velocity);
      if ((double) vector3_2.x > 0.0)
      {
        this.SetLinearThrustLoadTargets(PDFCS.Thruster.Facing.PX, vector3_2.x);
        this.KillLoadTargetLinear(PDFCS.Thruster.Facing.NX);
      }
      else if ((double) vector3_2.x < 0.0)
      {
        this.SetLinearThrustLoadTargets(PDFCS.Thruster.Facing.NX, -vector3_2.x);
        this.KillLoadTargetLinear(PDFCS.Thruster.Facing.PX);
      }
      else
      {
        this.KillLoadTargetLinear(PDFCS.Thruster.Facing.NX);
        this.KillLoadTargetLinear(PDFCS.Thruster.Facing.PX);
      }
      if ((double) vector3_2.y > 0.0)
      {
        this.SetLinearThrustLoadTargets(PDFCS.Thruster.Facing.PY, vector3_2.y);
        this.KillLoadTargetLinear(PDFCS.Thruster.Facing.NY);
      }
      else if ((double) vector3_2.y < 0.0)
      {
        this.SetLinearThrustLoadTargets(PDFCS.Thruster.Facing.NY, -vector3_2.y);
        this.KillLoadTargetLinear(PDFCS.Thruster.Facing.PY);
      }
      else
      {
        this.KillLoadTargetLinear(PDFCS.Thruster.Facing.NY);
        this.KillLoadTargetLinear(PDFCS.Thruster.Facing.PY);
      }
      if ((double) vector3_2.z > 0.0)
      {
        this.SetLinearThrustLoadTargets(PDFCS.Thruster.Facing.PZ, vector3_2.z);
        this.KillLoadTargetLinear(PDFCS.Thruster.Facing.NZ);
      }
      else if ((double) vector3_2.z < 0.0)
      {
        this.SetLinearThrustLoadTargets(PDFCS.Thruster.Facing.NZ, -vector3_2.z);
        this.KillLoadTargetLinear(PDFCS.Thruster.Facing.PZ);
      }
      else
      {
        this.KillLoadTargetLinear(PDFCS.Thruster.Facing.NZ);
        this.KillLoadTargetLinear(PDFCS.Thruster.Facing.PZ);
      }
      Vector3 angularVelocity = this.RB.angularVelocity;
      Vector3 vector3_3 = this.m_desiredAngularVelocity - this.transform.InverseTransformDirection(this.RB.angularVelocity);
      if ((double) vector3_3.x > 0.0)
      {
        this.SetAngularThrustLoadTargets(PDFCS.Thruster.Facing.PX, vector3_3.x);
        this.KillLoadTargetAngular(PDFCS.Thruster.Facing.NX);
      }
      else if ((double) vector3_3.x < 0.0)
      {
        this.SetAngularThrustLoadTargets(PDFCS.Thruster.Facing.NX, -vector3_3.x);
        this.KillLoadTargetAngular(PDFCS.Thruster.Facing.PX);
      }
      else
      {
        this.KillLoadTargetAngular(PDFCS.Thruster.Facing.NX);
        this.KillLoadTargetAngular(PDFCS.Thruster.Facing.PX);
      }
      if ((double) vector3_3.y > 0.0)
      {
        this.SetAngularThrustLoadTargets(PDFCS.Thruster.Facing.PY, vector3_3.y);
        this.KillLoadTargetAngular(PDFCS.Thruster.Facing.NY);
      }
      else if ((double) vector3_3.y < 0.0)
      {
        this.SetAngularThrustLoadTargets(PDFCS.Thruster.Facing.NY, -vector3_3.y);
        this.KillLoadTargetAngular(PDFCS.Thruster.Facing.PY);
      }
      else
      {
        this.KillLoadTargetAngular(PDFCS.Thruster.Facing.NY);
        this.KillLoadTargetAngular(PDFCS.Thruster.Facing.PY);
      }
      if ((double) vector3_3.z > 0.0)
      {
        this.SetAngularThrustLoadTargets(PDFCS.Thruster.Facing.PZ, vector3_3.z);
        this.KillLoadTargetAngular(PDFCS.Thruster.Facing.NZ);
      }
      else if ((double) vector3_3.z < 0.0)
      {
        this.SetAngularThrustLoadTargets(PDFCS.Thruster.Facing.NZ, -vector3_3.z);
        this.KillLoadTargetAngular(PDFCS.Thruster.Facing.PZ);
      }
      else
      {
        this.KillLoadTargetAngular(PDFCS.Thruster.Facing.NZ);
        this.KillLoadTargetAngular(PDFCS.Thruster.Facing.PZ);
      }
      if (this.m_fireAfterBurners)
      {
        this.SetAfterburnerThrustLoadTargets(PDFCS.Thruster.Facing.PZ, vector3_2.z);
        this.KillLoadTargetLinear(PDFCS.Thruster.Facing.NZ);
        this.KillLoadTargetLinear(PDFCS.Thruster.Facing.PZ);
      }
      else
        this.KillLoadTargetAfterburned(PDFCS.Thruster.Facing.PZ);
      for (int index = 0; index < this.LinearThrusters.Count; ++index)
        this.LinearThrusters[index].Tick(deltaTime);
      for (int index = 0; index < this.AngularThrusters.Count; ++index)
        this.AngularThrusters[index].Tick(deltaTime);
      for (int index = 0; index < this.Afterburners.Count; ++index)
        this.Afterburners[index].Tick(deltaTime);
    }

    private void KillLoadTargetLinear(PDFCS.Thruster.Facing faceing)
    {
      for (int index = 0; index < this.LinearThrusters.Count; ++index)
      {
        if (this.LinearThrusters[index].ThrustDirLocal == faceing)
          this.LinearThrusters[index].SetTargetLoad(0.0f);
      }
    }

    private void KillLoadTargetAngular(PDFCS.Thruster.Facing faceing)
    {
      for (int index = 0; index < this.AngularThrusters.Count; ++index)
      {
        if (this.AngularThrusters[index].ThrustDirLocal == faceing)
          this.AngularThrusters[index].SetTargetLoad(0.0f);
      }
    }

    private void KillLoadTargetAfterburned(PDFCS.Thruster.Facing faceing)
    {
      for (int index = 0; index < this.Afterburners.Count; ++index)
      {
        if (this.Afterburners[index].ThrustDirLocal == faceing)
          this.Afterburners[index].SetTargetLoad(0.0f);
      }
    }

    private void SetLinearThrustLoadTargets(PDFCS.Thruster.Facing faceing, float amountNeededInDir)
    {
      float num = 0.0f;
      for (int index = 0; index < this.LinearThrusters.Count; ++index)
      {
        if (this.LinearThrusters[index].ThrustDirLocal == faceing)
        {
          num += this.LinearThrusters[index].GetMaxLinearOutput();
          if (this.LinearThrusters[index].isDebug)
            Debug.Log((object) ("output " + (object) this.LinearThrusters[index].GetMaxLinearOutput()));
        }
      }
      float l = 0.0f;
      if ((double) num > 0.0)
        l = Mathf.Clamp(amountNeededInDir / num, 0.0f, 1f);
      for (int index = 0; index < this.LinearThrusters.Count; ++index)
      {
        if (this.LinearThrusters[index].ThrustDirLocal == faceing)
          this.LinearThrusters[index].SetTargetLoad(l);
      }
    }

    private void SetAngularThrustLoadTargets(PDFCS.Thruster.Facing faceing, float amountNeededInDir)
    {
      float num = 0.0f;
      for (int index = 0; index < this.AngularThrusters.Count; ++index)
      {
        if (this.AngularThrusters[index].ThrustDirLocal == faceing)
        {
          num += this.AngularThrusters[index].GetMaxLinearOutput();
          if (this.AngularThrusters[index].isDebug)
            Debug.Log((object) ("output " + (object) this.AngularThrusters[index].GetMaxLinearOutput()));
        }
      }
      float l = 0.0f;
      if ((double) num > 0.0)
        l = Mathf.Clamp(amountNeededInDir / num, 0.0f, 1f);
      for (int index = 0; index < this.AngularThrusters.Count; ++index)
      {
        if (this.AngularThrusters[index].ThrustDirLocal == faceing)
          this.AngularThrusters[index].SetTargetLoad(l);
      }
    }

    private void SetAfterburnerThrustLoadTargets(
      PDFCS.Thruster.Facing faceing,
      float amountNeededInDir)
    {
      float num = 0.0f;
      for (int index = 0; index < this.Afterburners.Count; ++index)
      {
        if (this.Afterburners[index].ThrustDirLocal == faceing)
        {
          num += this.Afterburners[index].GetMaxLinearOutput();
          if (this.Afterburners[index].isDebug)
            Debug.Log((object) ("output " + (object) this.Afterburners[index].GetMaxLinearOutput()));
        }
      }
      float l = 0.0f;
      if ((double) num > 0.0)
        l = Mathf.Clamp(amountNeededInDir / num, 0.0f, 1f);
      for (int index = 0; index < this.Afterburners.Count; ++index)
      {
        if (this.Afterburners[index].ThrustDirLocal == faceing)
          this.Afterburners[index].SetTargetLoad(l);
      }
    }

    [ContextMenu("ConfigThrusters")]
    public void ConfigThrusters()
    {
      this.LinearThrusters.Clear();
      this.AngularThrusters.Clear();
      for (int index = 0; index < this.FCSConfig.Configs.Count; ++index)
      {
        PDFCSConfig.ThrusterConfig config = this.FCSConfig.Configs[index];
        PDFCS.Thruster thruster = new PDFCS.Thruster();
        thruster.FCS = this;
        thruster.ThrustDirLocal = config.TDir;
        thruster.isAngular = config.IsAng;
        thruster.ParentSys = config.Sys;
        thruster.TConfig = config.TConfig;
        thruster.IsAfterburner = config.IsGAB;
        if (thruster.IsAfterburner)
          this.Afterburners.Add(thruster);
        else if (thruster.isAngular)
          this.AngularThrusters.Add(thruster);
        else
          this.LinearThrusters.Add(thruster);
      }
    }

    private float GetSignedSquare(float i) => Mathf.Pow(i, 2f) * Mathf.Sign(i);

    [Serializable]
    public class Thruster
    {
      public PDFCS FCS;
      public bool isDebug;
      private Vector3 thrustDir = Vector3.zero;
      public PDFCS.Thruster.Facing ThrustDirLocal;
      public bool isAngular;
      public Vector3 PositionLocal;
      public PDComponentID ParentSys;
      public bool IsAfterburner;
      public PDThrusterConfig TConfig;
      private float m_targetLoad;
      private float m_currentLoad;

      public void SetTargetLoad(float l) => this.m_targetLoad = l;

      public void Init()
      {
        switch (this.ThrustDirLocal)
        {
          case PDFCS.Thruster.Facing.PX:
            this.thrustDir = Vector3.right;
            break;
          case PDFCS.Thruster.Facing.NX:
            this.thrustDir = Vector3.left;
            break;
          case PDFCS.Thruster.Facing.PY:
            this.thrustDir = Vector3.up;
            break;
          case PDFCS.Thruster.Facing.NY:
            this.thrustDir = Vector3.down;
            break;
          case PDFCS.Thruster.Facing.PZ:
            this.thrustDir = Vector3.forward;
            break;
          case PDFCS.Thruster.Facing.NZ:
            this.thrustDir = Vector3.back;
            break;
        }
      }

      public float GetMaxLinearOutput() => this.TConfig.MaxThrustForce * this.TConfig.IntegrityToMaxLoadCurve.Evaluate(this.FCS.PD.GetSys((int) this.ParentSys).GetIntegrity());

      public void Tick(float t)
      {
        float num1 = (double) this.m_targetLoad >= (double) this.m_currentLoad ? this.TConfig.ThrustEngagementResponseSpeedUp * this.TConfig.ThrustResponseCurve.Evaluate(this.m_currentLoad) : this.TConfig.ThrustEngagementResponseSpeedDown;
        this.m_currentLoad = Mathf.MoveTowards(this.m_currentLoad, this.m_targetLoad, t * num1);
        float num2 = Mathf.Clamp(this.m_currentLoad * this.TConfig.MaxThrustForce, 0.0f, this.TConfig.MaxThrustForce * this.TConfig.IntegrityToMaxLoadCurve.Evaluate(this.FCS.PD.GetSys((int) this.ParentSys).GetIntegrity()) * this.TConfig.HeatToMaxLoadCurve.Evaluate(this.FCS.PD.GetSys((int) this.ParentSys).GetHeat()));
        float h = this.TConfig.LoadToHeatCurve.Evaluate(this.m_currentLoad) * this.TConfig.HeatGenerationBase * t;
        this.FCS.PD.GetSys((int) this.ParentSys).AddHeat(h);
        if (!this.isAngular)
        {
          Vector3 force = this.FCS.transform.TransformVector(this.thrustDir * num2);
          if (!this.IsAfterburner)
          {
            float t1 = 0.0f;
            if ((double) this.FCS.DistFromGround > 3.0)
              t1 = (float) (((double) this.FCS.DistFromGround - 3.0) * 0.200000002980232);
            if ((double) force.y > 0.0)
              force.y = Mathf.Lerp(force.y, 0.0f, t1);
          }
          this.FCS.RB.AddForce(force, ForceMode.Acceleration);
        }
        else
          this.FCS.RB.AddRelativeTorque(this.thrustDir * num2, ForceMode.Acceleration);
      }

      public enum Facing
      {
        PX,
        NX,
        PY,
        NY,
        PZ,
        NZ,
      }
    }
  }
}
