// Decompiled with JetBrains decompiler
// Type: FistVR.SosigMeleeAnimationFrame
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class SosigMeleeAnimationFrame : MonoBehaviour
  {
    public SosigMeleeAnimationWriter w;
    public int Index;

    public void OnDrawGizmos()
    {
      if (!this.w.ShowGizmos || this.w.Frames.Count <= 0)
        return;
      Color interpolatedColor = this.w.GetInterpolatedColor(this.Index);
      Gizmos.color = new Color(interpolatedColor.r, interpolatedColor.g, interpolatedColor.b, 0.5f);
      if (this.w.IsShieldGizmo)
      {
        Matrix4x4 matrix4x4 = Matrix4x4.TRS(this.transform.position, this.transform.rotation, this.transform.localScale);
        Matrix4x4 matrix = Gizmos.matrix;
        Gizmos.matrix *= matrix4x4;
        Gizmos.DrawWireCube(Vector3.zero, this.w.ShieldDimensions);
        Gizmos.matrix = matrix;
      }
      else
      {
        Gizmos.DrawSphere(this.transform.position, 0.025f);
        Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward * this.w.WeaponLength_Forward);
        Gizmos.DrawLine(this.transform.position, this.transform.position - this.transform.forward * this.w.WeaponLength_Behind);
        if (!this.w.usesForwardBit)
          return;
        Gizmos.DrawLine(this.transform.position + this.transform.forward * this.w.WeaponLength_Forward, this.transform.position + this.transform.forward * this.w.WeaponLength_Forward - this.transform.up * this.w.WeaponLength_ForwardBit);
      }
    }

    public void OnDrawGizmosSelected()
    {
      if (!this.w.ShowGizmos || this.w.Frames.Count <= 0)
        return;
      Color interpolatedColor = this.w.GetInterpolatedColor(this.Index);
      Gizmos.color = new Color(interpolatedColor.r, interpolatedColor.g, interpolatedColor.b, 0.1f);
      if (this.w.IsShieldGizmo)
      {
        Matrix4x4 matrix4x4 = Matrix4x4.TRS(this.transform.position, this.transform.rotation, this.transform.localScale);
        Matrix4x4 matrix = Gizmos.matrix;
        Gizmos.matrix *= matrix4x4;
        Gizmos.DrawCube(Vector3.zero, this.w.ShieldDimensions * 0.95f);
        Gizmos.matrix = matrix;
      }
      else
      {
        Gizmos.DrawSphere(this.transform.position, 0.015f);
        Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward * this.w.WeaponLength_Forward);
        Gizmos.DrawLine(this.transform.position, this.transform.position - this.transform.forward * this.w.WeaponLength_Behind);
        if (!this.w.usesForwardBit)
          return;
        Gizmos.DrawLine(this.transform.position + this.transform.forward * this.w.WeaponLength_Forward, this.transform.position + this.transform.forward * this.w.WeaponLength_Forward - this.transform.up * this.w.WeaponLength_ForwardBit);
      }
    }
  }
}
