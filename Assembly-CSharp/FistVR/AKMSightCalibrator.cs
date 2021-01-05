// Decompiled with JetBrains decompiler
// Type: FistVR.AKMSightCalibrator
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class AKMSightCalibrator : MonoBehaviour
  {
    public List<float> Drops;
    public List<float> DropDistances;
    public Transform Muzzle;
    public Transform FrontSightPoint;
    public int selected;
    public Transform Chamber;
    public FVRFireArmRoundDisplayData Data;
    public int ZeroWhich;
    public float ZeroingVel;
    public float ZeroingMass;
    public float ZeroingXDim;
    public AnimationCurve BDCC;
    public AnimationCurve DropCurve;

    [ContextMenu("PopulateBasics")]
    public void PopulateBasics()
    {
      BallisticProjectile component = this.Data.Classes[this.ZeroWhich].ObjectID.GetGameObject().GetComponent<FVRFireArmRound>().BallisticProjectilePrefab.GetComponent<BallisticProjectile>();
      this.ZeroingVel = component.MuzzleVelocityBase;
      this.ZeroingMass = component.Mass;
      this.ZeroingXDim = component.Dimensions.x;
    }

    private void Start()
    {
    }

    private void Update()
    {
    }

    private void OnDrawGizmos()
    {
      if (!this.isActiveAndEnabled)
        return;
      for (int index = 0; index < this.Drops.Count; ++index)
      {
        Vector3 from = this.Muzzle.position + this.Muzzle.forward * this.DropDistances[index] + Vector3.up * this.Drops[index];
        Vector3 vector3_1 = this.FrontSightPoint.position - from;
        Vector3 vector3_2 = vector3_1 + vector3_1.normalized;
        Gizmos.color = index != this.selected ? Color.Lerp(Color.red, Color.yellow, (float) index / 11f) : Color.green;
        Gizmos.DrawLine(from, from + vector3_2);
      }
    }

    [ContextMenu("TestVel")]
    public void GetTestVel()
    {
      Keyframe[] keyframeArray = new Keyframe[21];
      for (int index = 0; index < keyframeArray.Length; ++index)
        keyframeArray[index] = new Keyframe((float) index * 0.1f, 0.0f);
      Vector3 b = Vector3.zero;
      float num = 0.0f;
      float time = 0.012f;
      Vector3 vector3 = new Vector3(0.0f, 0.0f, this.ZeroingVel * this.Data.VelMultByBarrelLengthCurve.Evaluate(Vector3.Distance(this.Chamber.position, this.Muzzle.position) * 39.3701f));
      float zeroingMass = this.ZeroingMass;
      float zeroingXdim = this.ZeroingXDim;
      bool flag = false;
      int index1 = 0;
      for (int index2 = 0; index2 < 2000; ++index2)
      {
        if ((double) b.z >= (double) index1 * 100.0 && index1 < keyframeArray.Length)
        {
          keyframeArray[index1].value = b.y;
          ++index1;
        }
        if ((double) b.z >= 2000.0)
        {
          Debug.Log((object) ("Height at 1000m is " + (object) b.y + " at " + (object) index2 + "step, with " + (object) vector3.magnitude + " vel"));
          break;
        }
        if (!flag && (double) vector3.magnitude < 430.0)
        {
          flag = true;
          Debug.Log((object) ("Dropped subsonic at distance of" + (object) b.z + " and height of" + (object) b.y + " at " + (object) index2 + "step, with " + (object) vector3.magnitude + " vel"));
        }
        float materialDensity = 1.225f;
        vector3 = this.ApplyDrag(vector3 + Vector3.down * 9.81f * time, materialDensity, time, zeroingXdim, zeroingMass);
        Vector3 a = b + vector3 * time;
        num += Vector3.Distance(a, b);
        b = a;
      }
      this.DropCurve = new AnimationCurve(keyframeArray);
      for (int index2 = 0; index2 < keyframeArray.Length; ++index2)
        this.DropCurve.SmoothTangents(index2, 1f);
    }

    private Vector3 ApplyDrag(
      Vector3 velocity,
      float materialDensity,
      float time,
      float XDim,
      float Mass)
    {
      float num = 3.141593f * Mathf.Pow(XDim * 0.5f, 2f);
      float magnitude = velocity.magnitude;
      Vector3 normalized = velocity.normalized;
      float currentDragCoefficient = this.GetCurrentDragCoefficient(velocity.magnitude);
      Vector3 vector3 = -velocity * (materialDensity * 0.5f * currentDragCoefficient * num / Mass) * magnitude;
      return normalized * Mathf.Clamp(magnitude - vector3.magnitude * time, 0.0f, magnitude);
    }

    private float GetCurrentDragCoefficient(float velocityMS) => this.BDCC.Evaluate(velocityMS * 0.00291545f);

    [ContextMenu("PopDrops")]
    public void PopDrops()
    {
      for (int index = 0; index < this.Drops.Count; ++index)
        this.Drops[index] = this.DropCurve.Evaluate(this.DropDistances[index] * (1f / 1000f));
    }
  }
}
