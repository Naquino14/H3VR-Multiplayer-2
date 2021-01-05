// Decompiled with JetBrains decompiler
// Type: Unity.Labs.SuperScience.PhysicsTracker
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Unity.Labs.SuperScience
{
  [Serializable]
  public class PhysicsTracker
  {
    private const float k_Period = 0.125f;
    private const float k_HalfPeriod = 0.0625f;
    private const int k_Steps = 4;
    private const float k_SamplePeriod = 0.03125f;
    private const float k_NewSampleWeight = 2f;
    private const float k_AdditiveWeight = 1f;
    private const float k_PredictedPeriod = 0.15625f;
    private const int k_SampleLength = 5;
    private const float k_MinOffset = 0.001f;
    private const float k_MinAngle = 0.5f;
    private const float k_MinLength = 1E-05f;
    private int m_CurrentSampleIndex = -1;
    private PhysicsTracker.Sample[] m_Samples = new PhysicsTracker.Sample[5];
    private Vector3 m_LastOffsetPosition = Vector3.zero;
    private Vector3 m_LastDirectionPosition = Vector3.zero;
    private Quaternion m_LastRotation = Quaternion.identity;

    public float Speed { get; private set; }

    public float AccelerationStrength { get; private set; }

    public Vector3 Direction { get; private set; }

    public Vector3 Velocity { get; private set; }

    public Vector3 Acceleration { get; private set; }

    public float AngularSpeed { get; private set; }

    public Vector3 AngularAxis { get; private set; }

    public Vector3 AngularVelocity { get; private set; }

    public float AngularAccelerationStrength { get; private set; }

    public Vector3 AngularAcceleration { get; private set; }

    public void Reset(
      Vector3 currentPosition,
      Quaternion currentRotation,
      Vector3 currentVelocity,
      Vector3 currentAngularVelocity)
    {
      this.m_LastOffsetPosition = currentPosition;
      this.m_LastDirectionPosition = currentPosition;
      this.m_LastRotation = currentRotation;
      this.Speed = currentVelocity.magnitude;
      this.Direction = currentVelocity.normalized;
      this.Velocity = currentVelocity;
      this.AccelerationStrength = 0.0f;
      this.Acceleration = Vector3.zero;
      this.AngularSpeed = currentAngularVelocity.magnitude * 57.29578f;
      this.AngularAxis = currentAngularVelocity.normalized;
      this.AngularVelocity = currentAngularVelocity;
      this.m_CurrentSampleIndex = 0;
      this.m_Samples[0] = new PhysicsTracker.Sample()
      {
        distance = this.Speed * 0.125f,
        offset = this.Velocity * 0.125f,
        angle = this.AngularSpeed * 0.125f,
        axisOffset = this.AngularAxis * 0.125f,
        speed = this.Speed,
        angularSpeed = this.AngularSpeed,
        time = 0.125f
      };
    }

    public void Update(Vector3 newPosition, Quaternion newRotation, float timeSlice)
    {
      if (this.m_CurrentSampleIndex == -1)
      {
        this.Reset(newPosition, newRotation, Vector3.zero, Vector3.zero);
      }
      else
      {
        if ((double) timeSlice <= 0.0)
          return;
        Vector3 vector3_1 = newPosition - this.m_LastOffsetPosition;
        float magnitude = vector3_1.magnitude;
        this.m_LastOffsetPosition = newPosition;
        Vector3 vector3_2 = newPosition - this.m_LastDirectionPosition;
        if ((double) vector3_2.magnitude < 1.0 / 1000.0)
        {
          vector3_2 = this.Direction;
        }
        else
        {
          vector3_2.Normalize();
          this.m_LastDirectionPosition = newPosition;
        }
        Quaternion quaternion = newRotation * Quaternion.Inverse(this.m_LastRotation);
        Vector3 axis = Vector3.zero;
        float angle;
        quaternion.ToAngleAxis(out angle, out axis);
        if ((double) angle < 0.5)
        {
          angle = 0.0f;
          axis = this.AngularAxis;
        }
        else
          this.m_LastRotation = newRotation;
        float num1 = (float) (1.0 + (double) angle / 90.0);
        this.m_Samples[this.m_CurrentSampleIndex].distance += magnitude;
        this.m_Samples[this.m_CurrentSampleIndex].offset += vector3_1;
        this.m_Samples[this.m_CurrentSampleIndex].angle += angle;
        this.m_Samples[this.m_CurrentSampleIndex].time += timeSlice;
        if ((double) Vector3.Dot(axis, this.m_Samples[this.m_CurrentSampleIndex].axisOffset) < 0.0)
          this.m_Samples[this.m_CurrentSampleIndex].axisOffset += -axis * num1;
        else
          this.m_Samples[this.m_CurrentSampleIndex].axisOffset += axis * num1;
        PhysicsTracker.Sample sample = new PhysicsTracker.Sample();
        int index1 = this.m_CurrentSampleIndex;
        while ((double) sample.time < 0.125)
        {
          float scalar = Mathf.Clamp01((0.125f - sample.time) / this.m_Samples[index1].time);
          sample.Accumulate(ref this.m_Samples[index1], scalar, vector3_2, axis);
          index1 = (index1 + 1) % 5;
        }
        float speed = sample.speed;
        float angularSpeed = sample.angularSpeed;
        int index2 = this.m_CurrentSampleIndex;
        while ((double) sample.time < 5.0 / 32.0)
        {
          float scalar = Mathf.Clamp01((5f / 32f - sample.time) / this.m_Samples[index2].time);
          sample.Accumulate(ref this.m_Samples[index2], scalar, vector3_2, axis);
          index2 = (index2 + 1) % 5;
        }
        this.Speed = sample.distance / (5f / 32f);
        if ((double) sample.offset.magnitude > 9.99999974737875E-06)
        {
          this.Direction = sample.offset.normalized;
        }
        else
        {
          float t = Vector3.Dot(this.Direction, vector3_2);
          if ((double) t < 0.0)
          {
            t = -t;
            this.Direction = -this.Direction;
          }
          this.Direction = Vector3.Lerp(vector3_2, this.Direction, t).normalized;
        }
        this.Velocity = this.Direction * this.Speed;
        this.AngularSpeed = sample.angle / (5f / 32f);
        if ((double) sample.axisOffset.magnitude > 9.99999974737875E-06)
          axis = sample.axisOffset.normalized;
        float t1 = Vector3.Dot(this.AngularAxis, axis);
        if ((double) t1 < 0.0)
        {
          t1 = -t1;
          this.AngularAxis = -this.AngularAxis;
        }
        this.AngularAxis = Vector3.Lerp(axis, this.AngularAxis, t1).normalized;
        this.AngularVelocity = this.AngularAxis * this.AngularSpeed * ((float) Math.PI / 180f);
        float num2 = this.Speed - speed;
        float num3 = this.AngularSpeed - angularSpeed;
        this.AccelerationStrength = num2 / 0.125f;
        this.Acceleration = this.AccelerationStrength * this.Direction;
        this.AngularAccelerationStrength = num3 / 0.125f;
        this.AngularAcceleration = this.AngularAxis * this.AngularAccelerationStrength * ((float) Math.PI / 180f);
        if ((double) this.m_Samples[this.m_CurrentSampleIndex].time < 1.0 / 32.0)
          return;
        this.m_Samples[this.m_CurrentSampleIndex].speed = this.Speed;
        this.m_Samples[this.m_CurrentSampleIndex].angularSpeed = this.AngularSpeed;
        this.m_CurrentSampleIndex = (this.m_CurrentSampleIndex - 1 + 5) % 5;
        this.m_Samples[this.m_CurrentSampleIndex] = new PhysicsTracker.Sample();
      }
    }

    private struct Sample
    {
      public float distance;
      public float angle;
      public Vector3 offset;
      public Vector3 axisOffset;
      public float speed;
      public float angularSpeed;
      public float time;

      public void Accumulate(
        ref PhysicsTracker.Sample other,
        float scalar,
        Vector3 directionAnchor,
        Vector3 axisAnchor)
      {
        this.distance += other.distance * scalar;
        this.angle += other.angle * scalar;
        this.offset += other.offset * Vector3.Dot(directionAnchor, other.offset) * scalar;
        this.axisOffset += other.axisOffset * Vector3.Dot(axisAnchor, other.axisOffset) * scalar;
        this.time += other.time * scalar;
        this.speed = Mathf.Lerp(this.speed, other.speed, scalar);
        this.angularSpeed = Mathf.Lerp(this.angularSpeed, other.angularSpeed, scalar);
      }
    }
  }
}
