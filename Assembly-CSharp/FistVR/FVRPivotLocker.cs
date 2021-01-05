// Decompiled with JetBrains decompiler
// Type: FistVR.FVRPivotLocker
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FVRPivotLocker : FVRPhysicalObject
  {
    [Header("PivotLockerStuff")]
    public Transform TestingBox;
    public Transform SavedPose;
    private FVRPhysicalObject m_obj;
    public List<Transform> AxisTools;
    private string m_axis = "X";
    private float m_axisSensitivity = 1f;

    public override bool IsDistantGrabbable() => !((Object) this.m_obj != (Object) null) && base.IsDistantGrabbable();

    public override bool IsInteractable() => !((Object) this.m_obj != (Object) null) && base.IsInteractable();

    public void ToggleLock()
    {
      if ((Object) this.m_obj != (Object) null)
        this.UnlockObject();
      else
        this.TryToLockObject();
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (!((Object) this.m_obj != (Object) null))
        return;
      if (!this.m_obj.RootRigidbody.isKinematic)
        this.m_obj.RootRigidbody.isKinematic = true;
      if (!((Object) this.m_obj.transform.parent != (Object) null))
        return;
      this.m_obj.transform.SetParent((Transform) null);
    }

    private void UnlockObject()
    {
      this.m_obj.IsPivotLocked = false;
      this.m_obj.RootRigidbody.isKinematic = false;
      this.m_obj = (FVRPhysicalObject) null;
    }

    public void BTN_SetAxis(string a) => this.m_axis = a;

    public void BTN_NudgeAxis(float a)
    {
      switch (this.m_axis)
      {
        case "X":
          this.SavedPose.localEulerAngles += new Vector3(a, 0.0f, 0.0f);
          break;
        case "Y":
          this.SavedPose.localEulerAngles += new Vector3(0.0f, a, 0.0f);
          break;
        case "Z":
          this.SavedPose.localEulerAngles += new Vector3(0.0f, 0.0f, a);
          break;
      }
      this.UpdatePose();
    }

    public void BTN_Set0()
    {
      switch (this.m_axis)
      {
        case "X":
          this.SavedPose.localEulerAngles = new Vector3(0.0f, this.SavedPose.localEulerAngles.y, this.SavedPose.localEulerAngles.z);
          break;
        case "Y":
          this.SavedPose.localEulerAngles = new Vector3(this.SavedPose.localEulerAngles.x, 0.0f, this.SavedPose.localEulerAngles.z);
          break;
        case "Z":
          this.SavedPose.localEulerAngles = new Vector3(this.SavedPose.localEulerAngles.x, this.SavedPose.localEulerAngles.y, 0.0f);
          break;
      }
      this.UpdatePose();
    }

    public void BTN_SetAxisToolsSide(int s)
    {
      switch (s)
      {
        case 0:
          for (int index = 0; index < this.AxisTools.Count; ++index)
          {
            this.AxisTools[index].gameObject.SetActive(true);
            this.AxisTools[index].localPosition = new Vector3(-Mathf.Abs(this.AxisTools[index].localPosition.x), this.AxisTools[index].localPosition.y, this.AxisTools[index].localPosition.z);
          }
          break;
        case 1:
          for (int index = 0; index < this.AxisTools.Count; ++index)
          {
            this.AxisTools[index].gameObject.SetActive(true);
            this.AxisTools[index].localPosition = new Vector3(Mathf.Abs(this.AxisTools[index].localPosition.x), this.AxisTools[index].localPosition.y, this.AxisTools[index].localPosition.z);
          }
          break;
        case 2:
          for (int index = 0; index < this.AxisTools.Count; ++index)
            this.AxisTools[index].gameObject.SetActive(false);
          break;
      }
    }

    public void BTN_SetAxisSensitivity(float s) => this.m_axisSensitivity = s;

    public void SetXYZZero()
    {
      if ((Object) this.m_obj == (Object) null)
        return;
      this.m_obj.transform.rotation = this.transform.rotation;
      this.m_obj.RootRigidbody.rotation = this.transform.rotation;
      this.m_obj.RootRigidbody.isKinematic = true;
      this.m_obj.PivotLockPos = this.m_obj.transform.position;
      this.m_obj.PivotLockRot = this.m_obj.transform.rotation;
      this.SavedPose.position = this.m_obj.transform.position;
      this.SavedPose.rotation = this.m_obj.transform.rotation;
      this.UpdatePose();
    }

    public void SlideOnAxis(Vector3 amount)
    {
      amount *= this.m_axisSensitivity;
      this.SavedPose.localPosition += amount;
      this.UpdatePose();
    }

    public void RotateOnAxis(string Axis, float amount)
    {
      amount *= this.m_axisSensitivity;
      switch (Axis)
      {
        case "X":
          this.m_obj.transform.rotation = this.m_obj.transform.rotation * Quaternion.AngleAxis(amount, Vector3.right);
          break;
        case "Y":
          this.m_obj.transform.rotation = this.m_obj.transform.rotation * Quaternion.AngleAxis(amount, Vector3.up);
          break;
        case "Z":
          this.m_obj.transform.rotation = this.m_obj.transform.rotation * Quaternion.AngleAxis(amount, Vector3.forward);
          break;
      }
      this.m_obj.PivotLockPos = this.m_obj.transform.position;
      this.m_obj.PivotLockRot = this.m_obj.transform.rotation;
      this.SavedPose.position = this.m_obj.transform.position;
      this.SavedPose.rotation = this.m_obj.transform.rotation;
      this.UpdatePose();
    }

    private void TryToLockObject()
    {
      if ((Object) GM.CurrentMovementManager.Hands[0].CurrentInteractable != (Object) null && GM.CurrentMovementManager.Hands[0].CurrentInteractable is FVRPhysicalObject && this.IsInsideMyBox(GM.CurrentMovementManager.Hands[0].transform.position))
        this.LockObject(GM.CurrentMovementManager.Hands[0].CurrentInteractable as FVRPhysicalObject);
      if ((Object) this.m_obj != (Object) null || !((Object) GM.CurrentMovementManager.Hands[1].CurrentInteractable != (Object) null) || (!(GM.CurrentMovementManager.Hands[1].CurrentInteractable is FVRPhysicalObject) || !this.IsInsideMyBox(GM.CurrentMovementManager.Hands[1].transform.position)))
        return;
      this.LockObject(GM.CurrentMovementManager.Hands[1].CurrentInteractable as FVRPhysicalObject);
    }

    private void LockObject(FVRPhysicalObject o)
    {
      this.RootRigidbody.isKinematic = true;
      this.m_obj = o;
      this.m_obj.IsPivotLocked = true;
      this.m_obj.PivotLockPos = this.m_obj.transform.position;
      this.m_obj.PivotLockRot = this.m_obj.transform.rotation;
      this.m_obj.RootRigidbody.isKinematic = true;
      this.SavedPose.position = this.m_obj.transform.position;
      this.SavedPose.rotation = this.m_obj.transform.rotation;
    }

    private void UpdatePose()
    {
      if ((Object) this.m_obj == (Object) null)
        return;
      this.m_obj.PivotLockPos = this.SavedPose.position;
      this.m_obj.PivotLockRot = this.SavedPose.rotation;
      this.m_obj.transform.position = this.SavedPose.position;
      this.m_obj.transform.rotation = this.SavedPose.rotation;
    }

    public bool IsInsideMyBox(Vector3 pos)
    {
      bool flag = true;
      Vector3 vector3 = this.TestingBox.InverseTransformPoint(pos);
      if ((double) Mathf.Abs(vector3.x) > 0.5 || (double) Mathf.Abs(vector3.y) > 0.5 || (double) Mathf.Abs(vector3.z) > 0.5)
        flag = false;
      return flag;
    }

    public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
    {
      Quaternion quaternion = new Quaternion();
      quaternion.w = Mathf.Sqrt(Mathf.Max(0.0f, 1f + m[0, 0] + m[1, 1] + m[2, 2])) / 2f;
      quaternion.x = Mathf.Sqrt(Mathf.Max(0.0f, 1f + m[0, 0] - m[1, 1] - m[2, 2])) / 2f;
      quaternion.y = Mathf.Sqrt(Mathf.Max(0.0f, 1f - m[0, 0] + m[1, 1] - m[2, 2])) / 2f;
      quaternion.z = Mathf.Sqrt(Mathf.Max(0.0f, 1f - m[0, 0] - m[1, 1] + m[2, 2])) / 2f;
      quaternion.x *= Mathf.Sign(quaternion.x * (m[2, 1] - m[1, 2]));
      quaternion.y *= Mathf.Sign(quaternion.y * (m[0, 2] - m[2, 0]));
      quaternion.z *= Mathf.Sign(quaternion.z * (m[1, 0] - m[0, 1]));
      return quaternion;
    }

    public static void AlignChild(Transform main, Transform child, Transform alignTo)
    {
      Matrix4x4 matrix4x4 = Matrix4x4.TRS(child.position, child.rotation, Vector3.one);
      Matrix4x4 m = Matrix4x4.TRS(alignTo.position, alignTo.rotation, Vector3.one) * matrix4x4.inverse * main.localToWorldMatrix;
      main.position = (Vector3) m.GetColumn(3);
      main.rotation = FVRPivotLocker.QuaternionFromMatrix(m);
    }
  }
}
