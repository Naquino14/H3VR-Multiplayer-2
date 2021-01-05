// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_IK
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR
{
  public class SteamVR_IK : MonoBehaviour
  {
    public Transform target;
    public Transform start;
    public Transform joint;
    public Transform end;
    public Transform poleVector;
    public Transform upVector;
    public float blendPct = 1f;
    [HideInInspector]
    public Transform startXform;
    [HideInInspector]
    public Transform jointXform;
    [HideInInspector]
    public Transform endXform;

    private void LateUpdate()
    {
      if ((double) this.blendPct < 1.0 / 1000.0)
        return;
      Vector3 worldUp = !(bool) (Object) this.upVector ? Vector3.Cross(this.end.position - this.start.position, this.joint.position - this.start.position).normalized : this.upVector.up;
      Vector3 position1 = this.target.position;
      Quaternion rotation = this.target.rotation;
      Vector3 position2 = this.joint.position;
      Vector3 up;
      SteamVR_IK.Solve(this.start.position, position1, this.poleVector.position, (this.joint.position - this.start.position).magnitude, (this.end.position - this.joint.position).magnitude, ref position2, out Vector3 _, out up);
      if (up == Vector3.zero)
        return;
      Vector3 position3 = this.start.position;
      Vector3 position4 = this.joint.position;
      Vector3 position5 = this.end.position;
      Quaternion localRotation1 = this.start.localRotation;
      Quaternion localRotation2 = this.joint.localRotation;
      Quaternion localRotation3 = this.end.localRotation;
      Transform parent1 = this.start.parent;
      Transform parent2 = this.joint.parent;
      Transform parent3 = this.end.parent;
      Vector3 localScale1 = this.start.localScale;
      Vector3 localScale2 = this.joint.localScale;
      Vector3 localScale3 = this.end.localScale;
      if ((Object) this.startXform == (Object) null)
      {
        this.startXform = new GameObject("startXform").transform;
        this.startXform.parent = this.transform;
      }
      this.startXform.position = position3;
      this.startXform.LookAt(this.joint, worldUp);
      this.start.parent = this.startXform;
      if ((Object) this.jointXform == (Object) null)
      {
        this.jointXform = new GameObject("jointXform").transform;
        this.jointXform.parent = this.startXform;
      }
      this.jointXform.position = position4;
      this.jointXform.LookAt(this.end, worldUp);
      this.joint.parent = this.jointXform;
      if ((Object) this.endXform == (Object) null)
      {
        this.endXform = new GameObject("endXform").transform;
        this.endXform.parent = this.jointXform;
      }
      this.endXform.position = position5;
      this.end.parent = this.endXform;
      this.startXform.LookAt(position2, up);
      this.jointXform.LookAt(position1, up);
      this.endXform.rotation = rotation;
      this.start.parent = parent1;
      this.joint.parent = parent2;
      this.end.parent = parent3;
      this.end.rotation = rotation;
      if ((double) this.blendPct < 1.0)
      {
        this.start.localRotation = Quaternion.Slerp(localRotation1, this.start.localRotation, this.blendPct);
        this.joint.localRotation = Quaternion.Slerp(localRotation2, this.joint.localRotation, this.blendPct);
        this.end.localRotation = Quaternion.Slerp(localRotation3, this.end.localRotation, this.blendPct);
      }
      this.start.localScale = localScale1;
      this.joint.localScale = localScale2;
      this.end.localScale = localScale3;
    }

    public static bool Solve(
      Vector3 start,
      Vector3 end,
      Vector3 poleVector,
      float jointDist,
      float targetDist,
      ref Vector3 result,
      out Vector3 forward,
      out Vector3 up)
    {
      float num1 = jointDist + targetDist;
      Vector3 vector3_1 = end - start;
      Vector3 normalized = (poleVector - start).normalized;
      float magnitude = vector3_1.magnitude;
      result = start;
      if ((double) magnitude < 1.0 / 1000.0)
      {
        result += normalized * jointDist;
        forward = Vector3.Cross(normalized, Vector3.up);
        up = Vector3.Cross(forward, normalized).normalized;
      }
      else
      {
        forward = vector3_1 * (1f / magnitude);
        up = Vector3.Cross(forward, normalized).normalized;
        if ((double) magnitude + 1.0 / 1000.0 < (double) num1)
        {
          float num2 = (float) (((double) num1 + (double) magnitude) * 0.5);
          if ((double) num2 > (double) jointDist + 1.0 / 1000.0 && (double) num2 > (double) targetDist + 1.0 / 1000.0)
          {
            float num3 = 2f * Mathf.Sqrt((float) ((double) num2 * ((double) num2 - (double) jointDist) * ((double) num2 - (double) targetDist) * ((double) num2 - (double) magnitude))) / magnitude;
            float num4 = Mathf.Sqrt((float) ((double) jointDist * (double) jointDist - (double) num3 * (double) num3));
            Vector3 vector3_2 = Vector3.Cross(up, forward);
            result += forward * num4 + vector3_2 * num3;
            return true;
          }
          result += normalized * jointDist;
        }
        else
          result += forward * jointDist;
      }
      return false;
    }
  }
}
