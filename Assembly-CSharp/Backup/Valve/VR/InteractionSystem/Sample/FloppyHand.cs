// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Sample.FloppyHand
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
  public class FloppyHand : MonoBehaviour
  {
    protected float fingerFlexAngle = 140f;
    public SteamVR_Action_Single squeezyAction = SteamVR_Input.GetAction<SteamVR_Action_Single>("Squeeze");
    public SteamVR_Input_Sources inputSource;
    public FloppyHand.Finger[] fingers;
    public Vector3 constforce;

    private void Start()
    {
      for (int index = 0; index < this.fingers.Length; ++index)
      {
        this.fingers[index].Init();
        this.fingers[index].flexAngle = this.fingerFlexAngle;
        this.fingers[index].squeezyAction = this.squeezyAction;
        this.fingers[index].inputSource = this.inputSource;
      }
    }

    private void Update()
    {
      for (int index = 0; index < this.fingers.Length; ++index)
      {
        this.fingers[index].ApplyForce(this.constforce);
        this.fingers[index].UpdateFinger(Time.deltaTime);
        this.fingers[index].ApplyTransforms();
      }
    }

    [Serializable]
    public class Finger
    {
      public float mass;
      [Range(0.0f, 1f)]
      public float pos;
      public Vector3 forwardAxis;
      public SkinnedMeshRenderer renderer;
      [HideInInspector]
      public SteamVR_Action_Single squeezyAction;
      public SteamVR_Input_Sources inputSource;
      public Transform[] bones;
      public Transform referenceBone;
      public Vector2 referenceAngles;
      public FloppyHand.Finger.eulerAxis referenceAxis;
      [HideInInspector]
      public float flexAngle;
      private Vector3[] rotation;
      private Vector3[] velocity;
      private Transform[] boneTips;
      private Vector3[] oldTipPosition;
      private Vector3[] oldTipDelta;
      private Vector3[,] inertiaSmoothing;
      private float squeezySmooth;
      private int inertiaSteps = 10;
      private float k = 400f;
      private float damping = 8f;
      private Quaternion[] startRot;

      public void ApplyForce(Vector3 worldForce)
      {
        for (int index = 0; index < this.startRot.Length; ++index)
          this.velocity[index] += worldForce / 50f;
      }

      public void Init()
      {
        this.startRot = new Quaternion[this.bones.Length];
        this.rotation = new Vector3[this.bones.Length];
        this.velocity = new Vector3[this.bones.Length];
        this.oldTipPosition = new Vector3[this.bones.Length];
        this.oldTipDelta = new Vector3[this.bones.Length];
        this.boneTips = new Transform[this.bones.Length];
        this.inertiaSmoothing = new Vector3[this.bones.Length, this.inertiaSteps];
        for (int index = 0; index < this.bones.Length; ++index)
        {
          this.startRot[index] = this.bones[index].localRotation;
          if (index < this.bones.Length - 1)
            this.boneTips[index] = this.bones[index + 1];
        }
      }

      public void UpdateFinger(float deltaTime)
      {
        if ((double) deltaTime == 0.0)
          return;
        float f = 0.0f;
        if ((SteamVR_Action) this.squeezyAction != (SteamVR_Action) null && this.squeezyAction.GetActive(this.inputSource))
          f = this.squeezyAction.GetAxis(this.inputSource);
        this.squeezySmooth = Mathf.Lerp(this.squeezySmooth, Mathf.Sqrt(f), deltaTime * 10f);
        if (this.renderer.sharedMesh.blendShapeCount > 0)
          this.renderer.SetBlendShapeWeight(0, this.squeezySmooth * 100f);
        float ang = 0.0f;
        if (this.referenceAxis == FloppyHand.Finger.eulerAxis.X)
          ang = this.referenceBone.localEulerAngles.x;
        if (this.referenceAxis == FloppyHand.Finger.eulerAxis.Y)
          ang = this.referenceBone.localEulerAngles.y;
        if (this.referenceAxis == FloppyHand.Finger.eulerAxis.Z)
          ang = this.referenceBone.localEulerAngles.z;
        this.pos = Mathf.InverseLerp(this.referenceAngles.x, this.referenceAngles.y, this.FixAngle(ang));
        if ((double) this.mass > 0.0)
        {
          for (int index1 = 0; index1 < this.bones.Length; ++index1)
          {
            bool flag = (UnityEngine.Object) this.boneTips[index1] != (UnityEngine.Object) null;
            if (flag)
            {
              Vector3 vector3_1 = (this.boneTips[index1].localPosition - this.bones[index1].InverseTransformPoint(this.oldTipPosition[index1])) / deltaTime;
              Vector3 vector3_2 = (vector3_1 - this.oldTipDelta[index1]) / deltaTime;
              this.oldTipDelta[index1] = vector3_1;
              Vector3 vector3_3 = vector3_1 * -2f;
              Vector3 vector3_4 = vector3_2 * -2f;
              for (int index2 = this.inertiaSteps - 1; index2 > 0; --index2)
                this.inertiaSmoothing[index1, index2] = this.inertiaSmoothing[index1, index2 - 1];
              this.inertiaSmoothing[index1, 0] = vector3_4;
              Vector3 zero = Vector3.zero;
              for (int index2 = 0; index2 < this.inertiaSteps; ++index2)
                zero += this.inertiaSmoothing[index1, index2];
              Vector3 vector3_5 = this.PowVector(zero / (float) this.inertiaSteps / 20f, 3f) * 20f;
              Vector3 forwardAxis = this.forwardAxis;
              Vector3 toDirection1 = this.forwardAxis + vector3_3;
              Vector3 toDirection2 = this.forwardAxis + vector3_5;
              Quaternion rotation1 = Quaternion.FromToRotation(forwardAxis, toDirection1);
              Quaternion rotation2 = Quaternion.FromToRotation(forwardAxis, toDirection2);
              this.velocity[index1] += this.FixVector(rotation1.eulerAngles) * 2f * deltaTime;
              this.velocity[index1] += this.FixVector(rotation2.eulerAngles) * 50f * deltaTime;
              this.velocity[index1] = Vector3.ClampMagnitude(this.velocity[index1], 1000f);
            }
            Vector3 vector3_6 = this.pos * Vector3.right * (this.flexAngle / (float) this.bones.Length);
            Vector3 vector3_7 = (-this.k * (this.rotation[index1] - vector3_6) - this.damping * this.velocity[index1]) / this.mass;
            this.velocity[index1] += vector3_7 * deltaTime;
            this.rotation[index1] += this.velocity[index1] * Time.deltaTime;
            this.rotation[index1] = Vector3.ClampMagnitude(this.rotation[index1], 180f);
            if (flag)
              this.oldTipPosition[index1] = this.boneTips[index1].position;
          }
        }
        else
          Debug.LogError((object) "<b>[SteamVR Interaction]</b> finger mass is zero");
      }

      public void ApplyTransforms()
      {
        for (int index = 0; index < this.bones.Length; ++index)
        {
          this.bones[index].localRotation = this.startRot[index];
          this.bones[index].Rotate(this.rotation[index], Space.Self);
        }
      }

      private Vector3 FixVector(Vector3 ang) => new Vector3(this.FixAngle(ang.x), this.FixAngle(ang.y), this.FixAngle(ang.z));

      private float FixAngle(float ang)
      {
        if ((double) ang > 180.0)
          ang -= 360f;
        return ang;
      }

      private Vector3 PowVector(Vector3 vector, float power)
      {
        Vector3 vector3 = new Vector3(Mathf.Sign(vector.x), Mathf.Sign(vector.y), Mathf.Sign(vector.z));
        vector.x = Mathf.Pow(Mathf.Abs(vector.x), power) * vector3.x;
        vector.y = Mathf.Pow(Mathf.Abs(vector.y), power) * vector3.y;
        vector.z = Mathf.Pow(Mathf.Abs(vector.z), power) * vector3.z;
        return vector;
      }

      public enum eulerAxis
      {
        X,
        Y,
        Z,
      }
    }
  }
}
