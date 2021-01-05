// Decompiled with JetBrains decompiler
// Type: FistVR.AINavigator
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
  public class AINavigator : AIBodyPiece
  {
    [Header("AI Navigator BaseClass Params")]
    public NavMeshAgent Agent;
    public Rigidbody BaseRB;
    public float MaxLinearSpeed = 1f;
    public float MaxTurningSpeed = 120f;
    private float currentTurningSpeed = 1f;
    private float currentLinearSpeed = 1f;
    private float currentMovementIntensity = 0.1f;
    public float DestinationThreshold = 1f;
    public bool IsAtDestination;
    protected Vector3 m_currentDestination;
    protected Vector3 m_currentNavTarget;
    protected NavMeshHit m_nHit;
    public LayerMask LMGround;
    private RaycastHit m_gHit;
    public Transform[] GroundCastPoints;
    private Vector3[] GroundContactPoints = new Vector3[3];
    private AINavigator.TopoPointSet m_tPCloseRandom;
    private AINavigator.TopoPointSet m_tPPastTravelled;
    public float TimeTilPlatesReset = 10f;
    public bool IsPermanentlyDisabled;
    private float m_damageReset;
    private Vector3 localForward = Vector3.zero;
    private Vector3 localUp = Vector3.zero;
    private Vector3 localRight = Vector3.zero;

    public Vector3 GetDestination() => this.m_currentDestination;

    public override void Awake()
    {
      base.Awake();
      this.IsAtDestination = true;
      this.m_currentDestination = this.BaseRB.position;
      this.m_currentNavTarget = this.BaseRB.position;
      this.Agent.updatePosition = false;
      this.Agent.updateRotation = false;
      for (int index = 0; index < this.GroundContactPoints.Length; ++index)
        this.GroundContactPoints[index] = this.GroundCastPoints[index].position;
      this.m_tPCloseRandom = new AINavigator.TopoPointSet(20, 1f, this.Agent.transform, AINavigator.TopoPointSampleMode.RandomRay);
      this.m_tPPastTravelled = new AINavigator.TopoPointSet(20, 10f, this.Agent.transform, AINavigator.TopoPointSampleMode.CurrentPos);
    }

    private void UpdateTPPoints()
    {
      this.m_tPCloseRandom.Update();
      this.m_tPPastTravelled.Update();
    }

    public override void DestroyEvent()
    {
      this.IsPermanentlyDisabled = true;
      base.DestroyEvent();
    }

    public void UpdateNavigationSystem()
    {
      this.UpdateTPPoints();
      this.CheckIfAtDestination();
      this.CheckIfNeedNewNavAgentPath();
    }

    public override void Update()
    {
      base.Update();
      if (this.IsPlateDisabled || this.IsPermanentlyDisabled)
      {
        this.currentTurningSpeed = 0.0f;
        this.currentLinearSpeed = 0.0f;
        this.Agent.Stop();
      }
      else if (this.IsPlateDamaged)
      {
        this.currentTurningSpeed = UnityEngine.Random.Range(0.0f, this.MaxTurningSpeed * 0.15f);
        this.currentLinearSpeed = Mathf.Lerp(0.1f, UnityEngine.Random.Range(0.0f, this.MaxLinearSpeed * 0.08f), this.currentMovementIntensity);
        this.m_damageReset -= Time.deltaTime;
        if ((double) this.m_damageReset > 0.0)
          return;
        this.ResetAllPlates();
      }
      else
      {
        this.currentTurningSpeed = this.MaxTurningSpeed;
        this.currentLinearSpeed = Mathf.Lerp(0.1f, this.MaxLinearSpeed, this.currentMovementIntensity);
      }
    }

    public override bool SetPlateDamaged(bool b)
    {
      if (!base.SetPlateDamaged(b))
        return false;
      this.m_damageReset = this.TimeTilPlatesReset;
      return true;
    }

    public override bool SetPlateDisabled(bool b)
    {
      if (!base.SetPlateDisabled(b))
        return false;
      Debug.Log((object) "DEAD NAV");
      return true;
    }

    public override void FixedUpdate()
    {
      base.FixedUpdate();
      this.MoveAgent();
    }

    private void MoveAgent()
    {
      for (int index = 0; index < this.GroundCastPoints.Length; ++index)
      {
        if (Physics.Raycast(this.GroundCastPoints[index].position + Vector3.up, Vector3.down, out this.m_gHit, 1.25f, (int) this.LMGround))
        {
          this.GroundContactPoints[index] = this.m_gHit.point;
          Debug.DrawLine(this.GroundContactPoints[index], this.GroundContactPoints[index] + Vector3.up, Color.yellow);
        }
        else
        {
          this.GroundContactPoints[index] = this.GroundCastPoints[index].position;
          Debug.DrawLine(this.GroundContactPoints[index], this.GroundContactPoints[index] + Vector3.up, Color.magenta);
        }
      }
      Plane plane = new Plane(this.GroundContactPoints[0], this.GroundContactPoints[1], this.GroundContactPoints[2]);
      this.localForward = ((this.GroundContactPoints[0] + this.GroundContactPoints[2]) * 0.5f - this.GroundContactPoints[1]).normalized;
      this.localUp = plane.normal;
      this.localUp = Vector3.up;
      this.localRight = Vector3.Cross(this.localUp, this.localForward);
      Debug.DrawLine(this.BaseRB.position, this.BaseRB.position + this.localRight, Color.red);
      Debug.DrawLine(this.BaseRB.position, this.BaseRB.position + this.localUp, Color.green);
      Debug.DrawLine(this.BaseRB.position, this.BaseRB.position + this.localForward, Color.blue);
      if ((double) this.Agent.desiredVelocity.magnitude < 0.00999999977648258)
        return;
      Vector3 normalized = this.Agent.desiredVelocity.normalized;
      if ((double) Vector3.Angle(normalized, this.localUp) <= 1.0 / 1000.0)
        this.localUp += UnityEngine.Random.onUnitSphere * (1f / 1000f);
      this.BaseRB.MoveRotation(Quaternion.RotateTowards(this.BaseRB.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(normalized, this.localUp), this.localUp), this.currentTurningSpeed * Time.deltaTime));
      Vector3 to = Vector3.ProjectOnPlane(this.transform.forward, Vector3.up);
      this.Agent.speed = Mathf.Lerp(this.currentLinearSpeed, 0.1f, (float) ((double) Vector3.Angle(normalized, to) * 0.100000001490116 - 2.0));
      this.BaseRB.MovePosition(this.Agent.nextPosition);
    }

    public Vector3 GetRandomNearDestination() => this.m_tPCloseRandom.GetRandomPoint();

    public Vector3 GetFurthestNearPointFrom(Vector3 targ) => this.m_tPCloseRandom.GetFurthestPointFrom(targ);

    public Vector3 GetNearestNearPointFrom(Vector3 targ) => this.m_tPCloseRandom.GetNearestPointTo(targ);

    public void SetMovementIntensity(float f) => this.currentMovementIntensity = f;

    public void SetNewNavDestination(Vector3 dest)
    {
      this.m_currentDestination = dest;
      this.IsAtDestination = false;
    }

    public void RotateTowards(Vector3 point)
    {
      Vector3 normalized = (point - this.transform.position).normalized;
      normalized.y = 0.0f;
      this.Agent.transform.rotation = Quaternion.Slerp(this.Agent.transform.rotation, Quaternion.LookRotation(normalized, Vector3.up), (float) ((double) Time.deltaTime * (double) this.MaxTurningSpeed * 0.100000001490116));
    }

    public void TryToSetDestinationTo(Vector3 point)
    {
      Vector3 dest = point;
      Vector3 vector3 = Vector3.down * 0.5f;
      if (NavMesh.SamplePosition(dest + vector3, out this.m_nHit, 1.9f, -1))
        dest = this.m_nHit.position;
      this.SetNewNavDestination(dest);
    }

    private void CheckIfAtDestination()
    {
      if ((double) Vector3.Distance(this.m_currentDestination, this.BaseRB.position) < (double) this.DestinationThreshold)
        this.IsAtDestination = true;
      else
        this.IsAtDestination = false;
    }

    private void CheckIfNeedNewNavAgentPath()
    {
      if (this.IsAtDestination)
        return;
      if (!this.Agent.hasPath && !this.Agent.pathPending)
      {
        if ((double) Vector3.Distance(this.m_currentDestination, this.Agent.transform.position) <= (double) this.DestinationThreshold)
          return;
        this.Agent.SetDestination(this.m_currentDestination);
      }
      else
      {
        if ((double) Vector3.Distance(this.m_currentDestination, this.Agent.destination) <= (double) this.DestinationThreshold)
          return;
        this.Agent.SetDestination(this.m_currentDestination);
      }
    }

    private void CheckIfNeedNewPath(Vector3 target)
    {
      Vector3 vector3_1 = this.m_currentNavTarget;
      Vector3 vector3_2 = UnityEngine.Random.onUnitSphere * 0.5f;
      vector3_2.y = 0.0f;
      Vector3 vector3_3 = Vector3.down * 0.5f;
      if (NavMesh.SamplePosition(target + vector3_2 + vector3_3, out this.m_nHit, 1.9f, -1))
        vector3_1 = this.m_nHit.position;
      if ((double) Vector3.Distance(vector3_1, this.m_currentNavTarget) <= 2.0)
        return;
      this.m_currentNavTarget = vector3_1;
      this.Agent.SetDestination(vector3_1);
    }

    public enum TopoPointSampleMode
    {
      CurrentPos,
      RandomRay,
    }

    public class TopoPoint : IComparable<AINavigator.TopoPoint>
    {
      public Vector3 point;
      public Transform origin;

      public int CompareTo(AINavigator.TopoPoint other)
      {
        if (other == null)
          return 1;
        if ((double) Vector3.Distance(this.point, this.origin.position) > (double) Vector3.Distance(other.point, this.origin.position))
          return -1;
        return (double) Vector3.Distance(this.point, this.origin.position) < (double) Vector3.Distance(other.point, this.origin.position) ? 1 : 0;
      }
    }

    public class TopoPointSet
    {
      public List<AINavigator.TopoPoint> m_points;
      public List<AINavigator.TopoPoint> m_sortedPoints;
      private int m_arraySize;
      private int m_arrayIndex;
      private float m_tick;
      private float m_maxTick;
      private AINavigator.TopoPointSampleMode m_sampleMode;
      private NavMeshHit m_hit;
      private Transform m_origin;

      public TopoPointSet(
        int size,
        float interval,
        Transform origin,
        AINavigator.TopoPointSampleMode mode)
      {
        this.m_points = new List<AINavigator.TopoPoint>();
        this.m_sortedPoints = new List<AINavigator.TopoPoint>();
        this.m_tick = UnityEngine.Random.Range(0.0f, interval);
        this.m_maxTick = interval;
        this.m_sampleMode = mode;
        this.m_arraySize = size;
        this.m_arrayIndex = 0;
        this.m_origin = origin;
        for (int index = 0; index < size; ++index)
          this.m_points.Add(new AINavigator.TopoPoint()
          {
            origin = origin,
            point = origin.position
          });
        for (int index = 0; index < size; ++index)
          this.m_sortedPoints.Add(new AINavigator.TopoPoint()
          {
            origin = origin,
            point = origin.position
          });
      }

      public void Update()
      {
        if ((double) this.m_tick <= 0.0)
        {
          this.m_tick = this.m_maxTick;
          this.SampleNewPoint();
        }
        else
          this.m_tick -= Time.deltaTime;
      }

      private void SampleNewPoint()
      {
        switch (this.m_sampleMode)
        {
          case AINavigator.TopoPointSampleMode.CurrentPos:
            this.m_points[this.m_arrayIndex].point = this.m_points[this.m_arrayIndex].origin.position;
            break;
          case AINavigator.TopoPointSampleMode.RandomRay:
            NavMesh.Raycast(this.m_origin.position, new Vector3(this.m_origin.position.x + UnityEngine.Random.Range(-20f, 20f), this.m_origin.position.y, this.m_origin.position.z + UnityEngine.Random.Range(-20f, 20f)), out this.m_hit, -1);
            this.m_points[this.m_arrayIndex].point = this.m_hit.position;
            break;
        }
        ++this.m_arrayIndex;
        if (this.m_arrayIndex < this.m_arraySize)
          return;
        this.m_arrayIndex = 0;
      }

      private void CopyAndSort()
      {
        for (int index = 0; index < this.m_arraySize; ++index)
          this.m_sortedPoints[index].point = this.m_points[index].point;
        this.m_sortedPoints.Sort();
      }

      public Vector3 GetRandomPoint() => this.m_points[UnityEngine.Random.Range(0, this.m_arraySize)].point;

      public Vector3 GetFurthestPointFrom(Vector3 point)
      {
        Vector3 vector3 = Vector3.zero;
        float num1 = 0.0f;
        for (int index = 0; index < this.m_points.Count; ++index)
        {
          float num2 = Vector3.Distance(this.m_points[index].point, point);
          if ((double) num2 >= (double) num1)
          {
            num1 = num2;
            vector3 = this.m_points[index].point;
          }
        }
        return vector3;
      }

      public Vector3 GetNearestPointTo(Vector3 point)
      {
        Vector3 vector3 = Vector3.zero;
        float num1 = 9000f;
        for (int index = 0; index < this.m_points.Count; ++index)
        {
          float num2 = Vector3.Distance(this.m_points[index].point, point);
          if ((double) num2 <= (double) num1)
          {
            num1 = num2;
            vector3 = this.m_points[index].point;
          }
        }
        return vector3;
      }
    }
  }
}
