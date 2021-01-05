// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Sample.BuggyBuddy
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
  public class BuggyBuddy : MonoBehaviour
  {
    public Transform turret;
    private float turretRot;
    [Tooltip("Maximum steering angle of the wheels")]
    public float maxAngle = 30f;
    [Tooltip("Maximum Turning torque")]
    public float maxTurnTorque = 30f;
    [Tooltip("Maximum torque applied to the driving wheels")]
    public float maxTorque = 300f;
    [Tooltip("Maximum brake torque applied to the driving wheels")]
    public float brakeTorque = 30000f;
    [Tooltip("If you need the visual wheels to be attached automatically, drag the wheel shape here.")]
    public GameObject[] wheelRenders;
    [Tooltip("The vehicle's speed when the physics engine can use different amount of sub-steps (in m/s).")]
    public float criticalSpeed = 5f;
    [Tooltip("Simulation sub-steps when the speed is above critical.")]
    public int stepsBelow = 5;
    [Tooltip("Simulation sub-steps when the speed is below critical.")]
    public int stepsAbove = 1;
    private WheelCollider[] m_Wheels;
    public AudioSource au_motor;
    [HideInInspector]
    public float mvol;
    public AudioSource au_skid;
    private float svol;
    public WheelDust skidsample;
    private float skidSpeed = 3f;
    public Vector3 localGravity;
    [HideInInspector]
    public Rigidbody body;
    public float rapidfireTime;
    private float shootTimer;
    [HideInInspector]
    public Vector2 steer;
    [HideInInspector]
    public float throttle;
    [HideInInspector]
    public float handBrake;
    [HideInInspector]
    public Transform controllerReference;
    [HideInInspector]
    public float speed;
    public Transform centerOfMass;

    private void Start()
    {
      this.body = this.GetComponent<Rigidbody>();
      this.m_Wheels = this.GetComponentsInChildren<WheelCollider>();
      this.body.centerOfMass = this.body.transform.InverseTransformPoint(this.centerOfMass.position) * this.body.transform.lossyScale.x;
    }

    private void Update()
    {
      this.m_Wheels[0].ConfigureVehicleSubsteps(this.criticalSpeed, this.stepsBelow, this.stepsAbove);
      float f = this.maxTorque * this.throttle;
      if ((double) this.steer.y < -0.5)
        f *= -1f;
      float num1 = this.maxAngle * this.steer.x;
      this.speed = this.transform.InverseTransformVector(this.body.velocity).z;
      float num2 = Mathf.Abs(this.speed);
      float num3 = num1 / (float) (1.0 + (double) num2 / 20.0);
      this.mvol = Mathf.Lerp(this.mvol, Mathf.Pow(Mathf.Abs(f) / this.maxTorque, 0.8f) * Mathf.Lerp(0.4f, 1f, Mathf.Abs(this.m_Wheels[2].rpm) / 200f) * Mathf.Lerp(1f, 0.5f, this.handBrake), Time.deltaTime * 9f);
      this.au_motor.volume = Mathf.Clamp01(this.mvol);
      this.au_motor.pitch = Mathf.Clamp01(Mathf.Lerp(0.8f, 1f, this.mvol));
      this.svol = Mathf.Lerp(this.svol, this.skidsample.amt / this.skidSpeed, Time.deltaTime * 9f);
      this.au_skid.volume = Mathf.Clamp01(this.svol);
      this.au_skid.pitch = Mathf.Clamp01(Mathf.Lerp(0.9f, 1f, this.svol));
      for (int index = 0; index < this.wheelRenders.Length; ++index)
      {
        WheelCollider wheel = this.m_Wheels[index];
        if ((double) wheel.transform.localPosition.z > 0.0)
        {
          wheel.steerAngle = num3;
          wheel.motorTorque = f;
        }
        if ((double) wheel.transform.localPosition.z >= 0.0)
          ;
        wheel.motorTorque = f;
        if ((double) wheel.transform.localPosition.x >= 0.0)
          ;
        if ((double) wheel.transform.localPosition.x < 0.0)
          ;
        if ((UnityEngine.Object) this.wheelRenders[index] != (UnityEngine.Object) null && this.m_Wheels[0].enabled)
        {
          Vector3 pos;
          Quaternion quat;
          wheel.GetWorldPose(out pos, out quat);
          Transform transform = this.wheelRenders[index].transform;
          transform.position = pos;
          transform.rotation = quat;
        }
      }
      this.steer = Vector2.Lerp(this.steer, Vector2.zero, Time.deltaTime * 4f);
    }

    private void FixedUpdate() => this.body.AddForce(this.localGravity, ForceMode.Acceleration);

    public static float FindAngle(Vector3 fromVector, Vector3 toVector, Vector3 upVector) => toVector == Vector3.zero ? 0.0f : Vector3.Angle(fromVector, toVector) * Mathf.Sign(Vector3.Dot(Vector3.Cross(fromVector, toVector), upVector)) * ((float) Math.PI / 180f);
  }
}
