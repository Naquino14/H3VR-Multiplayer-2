// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Frustum
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (MeshRenderer), typeof (MeshFilter))]
  public class SteamVR_Frustum : MonoBehaviour
  {
    public SteamVR_TrackedObject.EIndex index;
    public float fovLeft = 45f;
    public float fovRight = 45f;
    public float fovTop = 45f;
    public float fovBottom = 45f;
    public float nearZ = 0.5f;
    public float farZ = 2.5f;

    public void UpdateModel()
    {
      this.fovLeft = Mathf.Clamp(this.fovLeft, 1f, 89f);
      this.fovRight = Mathf.Clamp(this.fovRight, 1f, 89f);
      this.fovTop = Mathf.Clamp(this.fovTop, 1f, 89f);
      this.fovBottom = Mathf.Clamp(this.fovBottom, 1f, 89f);
      this.farZ = Mathf.Max(this.farZ, this.nearZ + 0.01f);
      this.nearZ = Mathf.Clamp(this.nearZ, 0.01f, this.farZ - 0.01f);
      float num1 = Mathf.Sin((float) (-(double) this.fovLeft * (Math.PI / 180.0)));
      float num2 = Mathf.Sin(this.fovRight * ((float) Math.PI / 180f));
      float num3 = Mathf.Sin(this.fovTop * ((float) Math.PI / 180f));
      float num4 = Mathf.Sin((float) (-(double) this.fovBottom * (Math.PI / 180.0)));
      float num5 = Mathf.Cos((float) (-(double) this.fovLeft * (Math.PI / 180.0)));
      float num6 = Mathf.Cos(this.fovRight * ((float) Math.PI / 180f));
      float num7 = Mathf.Cos(this.fovTop * ((float) Math.PI / 180f));
      float num8 = Mathf.Cos((float) (-(double) this.fovBottom * (Math.PI / 180.0)));
      Vector3[] vector3Array1 = new Vector3[8]
      {
        new Vector3(num1 * this.nearZ / num5, num3 * this.nearZ / num7, this.nearZ),
        new Vector3(num2 * this.nearZ / num6, num3 * this.nearZ / num7, this.nearZ),
        new Vector3(num2 * this.nearZ / num6, num4 * this.nearZ / num8, this.nearZ),
        new Vector3(num1 * this.nearZ / num5, num4 * this.nearZ / num8, this.nearZ),
        new Vector3(num1 * this.farZ / num5, num3 * this.farZ / num7, this.farZ),
        new Vector3(num2 * this.farZ / num6, num3 * this.farZ / num7, this.farZ),
        new Vector3(num2 * this.farZ / num6, num4 * this.farZ / num8, this.farZ),
        new Vector3(num1 * this.farZ / num5, num4 * this.farZ / num8, this.farZ)
      };
      int[] numArray1 = new int[48]
      {
        0,
        4,
        7,
        0,
        7,
        3,
        0,
        7,
        4,
        0,
        3,
        7,
        1,
        5,
        6,
        1,
        6,
        2,
        1,
        6,
        5,
        1,
        2,
        6,
        0,
        4,
        5,
        0,
        5,
        1,
        0,
        5,
        4,
        0,
        1,
        5,
        2,
        3,
        7,
        2,
        7,
        6,
        2,
        7,
        3,
        2,
        6,
        7
      };
      int num9 = 0;
      Vector3[] vector3Array2 = new Vector3[numArray1.Length];
      Vector3[] vector3Array3 = new Vector3[numArray1.Length];
      for (int index1 = 0; index1 < numArray1.Length / 3; ++index1)
      {
        Vector3 vector3_1 = vector3Array1[numArray1[index1 * 3]];
        Vector3 vector3_2 = vector3Array1[numArray1[index1 * 3 + 1]];
        Vector3 vector3_3 = vector3Array1[numArray1[index1 * 3 + 2]];
        Vector3 normalized = Vector3.Cross(vector3_2 - vector3_1, vector3_3 - vector3_1).normalized;
        vector3Array3[index1 * 3] = normalized;
        vector3Array3[index1 * 3 + 1] = normalized;
        vector3Array3[index1 * 3 + 2] = normalized;
        vector3Array2[index1 * 3] = vector3_1;
        vector3Array2[index1 * 3 + 1] = vector3_2;
        vector3Array2[index1 * 3 + 2] = vector3_3;
        int[] numArray2 = numArray1;
        int index2 = index1 * 3;
        int num10 = num9;
        int num11 = num10 + 1;
        numArray2[index2] = num10;
        int[] numArray3 = numArray1;
        int index3 = index1 * 3 + 1;
        int num12 = num11;
        int num13 = num12 + 1;
        numArray3[index3] = num12;
        int[] numArray4 = numArray1;
        int index4 = index1 * 3 + 2;
        int num14 = num13;
        num9 = num14 + 1;
        numArray4[index4] = num14;
      }
      this.GetComponent<MeshFilter>().mesh = new Mesh()
      {
        vertices = vector3Array2,
        normals = vector3Array3,
        triangles = numArray1
      };
    }

    private void OnDeviceConnected(int i, bool connected)
    {
      if ((SteamVR_TrackedObject.EIndex) i != this.index)
        return;
      this.GetComponent<MeshFilter>().mesh = (Mesh) null;
      if (!connected)
        return;
      CVRSystem system = OpenVR.System;
      if (system == null || system.GetTrackedDeviceClass((uint) i) != ETrackedDeviceClass.TrackingReference)
        return;
      ETrackedPropertyError pError = ETrackedPropertyError.TrackedProp_Success;
      float trackedDeviceProperty1 = system.GetFloatTrackedDeviceProperty((uint) i, ETrackedDeviceProperty.Prop_FieldOfViewLeftDegrees_Float, ref pError);
      if (pError == ETrackedPropertyError.TrackedProp_Success)
        this.fovLeft = trackedDeviceProperty1;
      float trackedDeviceProperty2 = system.GetFloatTrackedDeviceProperty((uint) i, ETrackedDeviceProperty.Prop_FieldOfViewRightDegrees_Float, ref pError);
      if (pError == ETrackedPropertyError.TrackedProp_Success)
        this.fovRight = trackedDeviceProperty2;
      float trackedDeviceProperty3 = system.GetFloatTrackedDeviceProperty((uint) i, ETrackedDeviceProperty.Prop_FieldOfViewTopDegrees_Float, ref pError);
      if (pError == ETrackedPropertyError.TrackedProp_Success)
        this.fovTop = trackedDeviceProperty3;
      float trackedDeviceProperty4 = system.GetFloatTrackedDeviceProperty((uint) i, ETrackedDeviceProperty.Prop_FieldOfViewBottomDegrees_Float, ref pError);
      if (pError == ETrackedPropertyError.TrackedProp_Success)
        this.fovBottom = trackedDeviceProperty4;
      float trackedDeviceProperty5 = system.GetFloatTrackedDeviceProperty((uint) i, ETrackedDeviceProperty.Prop_TrackingRangeMinimumMeters_Float, ref pError);
      if (pError == ETrackedPropertyError.TrackedProp_Success)
        this.nearZ = trackedDeviceProperty5;
      float trackedDeviceProperty6 = system.GetFloatTrackedDeviceProperty((uint) i, ETrackedDeviceProperty.Prop_TrackingRangeMaximumMeters_Float, ref pError);
      if (pError == ETrackedPropertyError.TrackedProp_Success)
        this.farZ = trackedDeviceProperty6;
      this.UpdateModel();
    }

    private void OnEnable()
    {
      this.GetComponent<MeshFilter>().mesh = (Mesh) null;
      SteamVR_Events.DeviceConnected.Listen(new UnityAction<int, bool>(this.OnDeviceConnected));
    }

    private void OnDisable()
    {
      SteamVR_Events.DeviceConnected.Remove(new UnityAction<int, bool>(this.OnDeviceConnected));
      this.GetComponent<MeshFilter>().mesh = (Mesh) null;
    }
  }
}
