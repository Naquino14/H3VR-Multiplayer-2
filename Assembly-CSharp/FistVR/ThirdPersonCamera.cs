// Decompiled with JetBrains decompiler
// Type: FistVR.ThirdPersonCamera
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class ThirdPersonCamera : MonoBehaviour
  {
    private Transform rig;
    private Transform head;
    private Transform torso;
    private List<Transform> hands;
    public float DistanceBack;
    public Transform Root;
    public Transform VerticalHinge;
    public Transform HorizontalHinge;
    public Transform Dolly;
    public Transform Cam;
    public Rigidbody SosigTorso;
    private float elevationMag;
    private float elevationVel;
    public LayerMask CamMask;
    private RaycastHit m_hit;
    public bool UpdateCam = true;
    private float AttachedRotationMultiplier = 30f;
    private float AttachedPositionMultiplier = 4500f;
    private float AttachedRotationFudge = 500f;
    private float AttachedPositionFudge = 500f;

    private void Start()
    {
      this.rig = GM.CurrentMovementManager.transform;
      this.head = GM.CurrentPlayerBody.Head;
      this.torso = GM.CurrentPlayerBody.Torso;
    }

    private void Update()
    {
      if (this.UpdateCam)
      {
        if ((double) Vector3.Distance(this.rig.position, this.Root.position) > 20.0)
          this.Root.position = this.rig.position;
        this.Root.position = Vector3.Lerp(this.Root.position, new Vector3(this.torso.position.x, this.rig.position.y, this.torso.position.z), Time.deltaTime * 8f);
        this.VerticalHinge.rotation = Quaternion.Slerp(this.VerticalHinge.rotation, Quaternion.LookRotation(this.torso.forward, Vector3.up), Time.deltaTime * 2f);
        this.elevationMag = Mathf.SmoothDampAngle(this.elevationMag, (Vector3.Angle(this.head.forward, Vector3.up) - 90f) * 0.3f, ref this.elevationVel, 1f);
        this.HorizontalHinge.localEulerAngles = new Vector3(this.elevationMag, 0.0f, 0.0f);
        float num = this.DistanceBack;
        if (Physics.Raycast(this.HorizontalHinge.position, -this.HorizontalHinge.forward, out this.m_hit, this.DistanceBack, (int) this.CamMask))
          num = this.m_hit.distance - 0.1f;
        this.Dolly.localPosition = Vector3.Lerp(this.Dolly.localPosition, new Vector3(0.0f, 0.0f, -Mathf.Clamp(num, 0.0f, this.DistanceBack)), Time.deltaTime * 8f);
        this.Cam.position = Vector3.Lerp(this.Cam.position, this.Dolly.position, Time.deltaTime * 4f);
        this.Cam.rotation = this.HorizontalHinge.rotation;
      }
      if ((double) this.DistanceFromCoreTarget() <= 1.0)
        return;
      this.SosigTorso.position = this.torso.position - this.torso.up * 0.25f;
    }

    private void FixedUpdate() => this.SosigPhys();

    private void SosigPhys()
    {
      Vector3 position = this.SosigTorso.position;
      Quaternion rotation1 = this.SosigTorso.rotation;
      Vector3 vector3_1 = this.torso.position - this.torso.up * 0.25f;
      Quaternion rotation2 = this.torso.rotation;
      Vector3 vector3_2 = vector3_1 - position;
      Quaternion quaternion = rotation2 * Quaternion.Inverse(rotation1);
      float deltaTime = Time.deltaTime;
      float angle;
      Vector3 axis;
      quaternion.ToAngleAxis(out angle, out axis);
      if ((double) angle > 180.0)
        angle -= 360f;
      if ((double) angle != 0.0)
        this.SosigTorso.angularVelocity = Vector3.MoveTowards(this.SosigTorso.angularVelocity, deltaTime * angle * axis * this.AttachedRotationMultiplier, this.AttachedRotationFudge * Time.fixedDeltaTime);
      this.SosigTorso.velocity = Vector3.MoveTowards(this.SosigTorso.velocity, vector3_2 * this.AttachedPositionMultiplier * deltaTime, this.AttachedPositionFudge * deltaTime);
    }

    public float DistanceFromCoreTarget() => Vector3.Distance(this.SosigTorso.position, this.torso.position);
  }
}
