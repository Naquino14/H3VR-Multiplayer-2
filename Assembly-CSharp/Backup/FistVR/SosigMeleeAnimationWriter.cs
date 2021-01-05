// Decompiled with JetBrains decompiler
// Type: FistVR.SosigMeleeAnimationWriter
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class SosigMeleeAnimationWriter : MonoBehaviour
  {
    public Transform Root;
    public bool ShowGizmos = true;
    public bool IsShieldGizmo;
    public float WeaponLength_Forward = 0.1f;
    public float WeaponLength_Behind = 0.1f;
    public Vector3 ShieldDimensions;
    public bool usesForwardBit = true;
    public float WeaponLength_ForwardBit = 0.1f;
    public Color AnimColor_Start;
    public Color AnimColor_End;
    public List<SosigMeleeAnimationFrame> Frames;
    public SosigMeleeAnimationSet SetToWriteTo;
    public bool IsAnimTesting;
    private float m_animTestFloat;
    private float m_testanimSpeed = 1f;

    public Color GetInterpolatedColor(int index) => Color.Lerp(this.AnimColor_Start, this.AnimColor_End, (float) index / (float) this.Frames.Count);

    public Color GetInterpolatedColor(float f) => Color.Lerp(this.AnimColor_Start, this.AnimColor_End, f);

    public void Update()
    {
      this.m_animTestFloat += Time.deltaTime * this.m_testanimSpeed;
      this.m_animTestFloat = Mathf.Repeat(this.m_animTestFloat, 1f);
    }

    public void OnDrawGizmos()
    {
      if (this.Frames.Count <= 0 || !this.IsAnimTesting)
        return;
      Color interpolatedColor = this.GetInterpolatedColor(this.m_animTestFloat);
      Vector3 vector3_1 = this.Root.TransformPoint(this.SetToWriteTo.GetPos(this.m_animTestFloat, true, true));
      Vector3 vector3_2 = this.Root.TransformDirection(this.SetToWriteTo.GetForward(this.m_animTestFloat, true, true));
      Vector3 vector3_3 = this.Root.TransformDirection(this.SetToWriteTo.GetUp(this.m_animTestFloat, true, true));
      Gizmos.color = new Color(interpolatedColor.r, interpolatedColor.g, interpolatedColor.b, 1f);
      Gizmos.DrawSphere(vector3_1, 0.025f);
      Gizmos.DrawLine(vector3_1, vector3_1 + vector3_2 * this.WeaponLength_Forward);
      Gizmos.DrawLine(vector3_1, vector3_1 - vector3_2 * this.WeaponLength_Behind);
      if (!this.usesForwardBit)
        return;
      Gizmos.DrawLine(vector3_1 + vector3_2 * this.WeaponLength_Forward, vector3_1 + vector3_2 * this.WeaponLength_Forward - vector3_3 * this.WeaponLength_ForwardBit);
    }

    [ContextMenu("SetIndicies")]
    public void SetIndicies()
    {
      for (int index = 0; index < this.Frames.Count; ++index)
        this.Frames[index].Index = index;
    }

    [ContextMenu("Write")]
    public void Write()
    {
      this.SetToWriteTo.Frames_LocalPos.Clear();
      this.SetToWriteTo.Frames_LocalForward.Clear();
      this.SetToWriteTo.Frames_LocalUp.Clear();
      for (int index = 0; index < this.Frames.Count; ++index)
      {
        Transform transform = this.Frames[index].transform;
        this.SetToWriteTo.Frames_LocalPos.Add(transform.localPosition);
        this.SetToWriteTo.Frames_LocalForward.Add(transform.forward);
        this.SetToWriteTo.Frames_LocalUp.Add(transform.up);
      }
    }
  }
}
