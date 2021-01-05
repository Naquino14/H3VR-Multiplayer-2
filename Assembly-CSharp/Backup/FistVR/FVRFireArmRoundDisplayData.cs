// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFireArmRoundDisplayData
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu]
  public class FVRFireArmRoundDisplayData : ScriptableObject
  {
    public string DisplayName = string.Empty;
    public FireArmRoundType Type;
    public FVRObject.OTagFirearmRoundPower RoundPower;
    public FVRFireArmRoundDisplayData.DisplayDataClass[] Classes;
    public bool IsMeatFortress;
    public AnimationCurve BDCC;
    public int ZeroWhichAmmo;
    public float ZeroingVel;
    public float ZeroingMass;
    public float ZeroingXDim;
    public AnimationCurve BulletDropCurve;
    public AnimationCurve BulletWindCurve;
    public AnimationCurve VelMultByBarrelLengthCurve;

    public FVRFireArmRoundDisplayData.DisplayDataClass GetDisplayClass(
      FireArmRoundClass c)
    {
      for (int index = 0; index < this.Classes.Length; ++index)
      {
        if (this.Classes[index].Class == c)
          return this.Classes[index];
      }
      return (FVRFireArmRoundDisplayData.DisplayDataClass) null;
    }

    [ContextMenu("PopulateBasics")]
    public void PopulateBasics()
    {
      BallisticProjectile component = this.Classes[this.ZeroWhichAmmo].ObjectID.GetGameObject().GetComponent<FVRFireArmRound>().BallisticProjectilePrefab.GetComponent<BallisticProjectile>();
      this.ZeroingVel = component.MuzzleVelocityBase;
      this.ZeroingMass = component.Mass;
      this.ZeroingXDim = component.Dimensions.x;
    }

    [ContextMenu("TestVel")]
    public void GetTestVel()
    {
      Keyframe[] keyframeArray1 = new Keyframe[11];
      for (int index = 0; index < keyframeArray1.Length; ++index)
        keyframeArray1[index] = new Keyframe((float) index * 0.1f, 0.0f);
      Keyframe[] keyframeArray2 = new Keyframe[11];
      for (int index = 0; index < keyframeArray2.Length; ++index)
        keyframeArray2[index] = new Keyframe((float) index * 0.1f, 0.0f);
      Vector3 b = Vector3.zero;
      float num = 0.0f;
      float time = 0.012f;
      Vector3 vector3 = new Vector3(0.0f, 0.0f, this.ZeroingVel);
      float zeroingMass = this.ZeroingMass;
      float zeroingXdim = this.ZeroingXDim;
      bool flag = false;
      int index1 = 0;
      for (int index2 = 0; index2 < 1000; ++index2)
      {
        if ((double) b.z >= (double) index1 * 100.0 && index1 < keyframeArray1.Length)
        {
          keyframeArray1[index1].value = b.y;
          keyframeArray2[index1].value = b.x;
          ++index1;
        }
        if ((double) b.z >= 1000.0)
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
      this.BulletDropCurve = new AnimationCurve(keyframeArray1);
      this.BulletWindCurve = new AnimationCurve(keyframeArray2);
      for (int index2 = 0; index2 < keyframeArray1.Length; ++index2)
        this.BulletDropCurve.SmoothTangents(index2, 1f);
      for (int index2 = 0; index2 < keyframeArray2.Length; ++index2)
        this.BulletDropCurve.SmoothTangents(index2, 1f);
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

    [Serializable]
    public class DisplayDataClass
    {
      public string Name;
      public FireArmRoundClass Class;
      private Mesh m_mesh;
      private Material m_material;
      public FVRObject ObjectID;

      public Mesh Mesh
      {
        get
        {
          if ((UnityEngine.Object) this.m_mesh == (UnityEngine.Object) null)
            this.m_mesh = this.ObjectID.GetGameObject().GetComponent<FVRFireArmRound>().UnfiredRenderer.GetComponent<MeshFilter>().sharedMesh;
          return this.m_mesh;
        }
      }

      public Material Material
      {
        get
        {
          if ((UnityEngine.Object) this.m_material == (UnityEngine.Object) null)
            this.m_material = this.ObjectID.GetGameObject().GetComponent<FVRFireArmRound>().UnfiredRenderer.sharedMaterial;
          return this.m_material;
        }
      }
    }
  }
}
