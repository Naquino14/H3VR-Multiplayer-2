// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFireArmBipod
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FVRFireArmBipod : MonoBehaviour
  {
    [Header("Bipod Params")]
    public FVRPhysicalObject FireArm;
    public FVRFireArmBipod.BipodStyle Style;
    public Transform[] BipodLegs;
    public float FoldedXRot;
    public float UnFoldedXRot = 90f;
    public List<Vector3> FoldedRPK;
    public List<Vector3> UnFoldedRPK;
    public List<Vector3> FoldedPosStroner;
    public List<Vector3> UnFoldedPosStroner;
    public Transform OverrideTranslationFrame;
    public Transform[] BipodFeet;
    public Transform GroundFollower;
    public Transform GroundContactReference;
    public Transform PointToOverride;
    private bool m_isBipodExpanded;
    private bool m_isBipodActive;
    public LayerMask LM_BipodTouch;
    private RaycastHit m_hit;
    private Vector3 m_savedGroundPoint;
    public Vector3 GroundFollowerTargetPoint = Vector3.zero;
    public float RecoilDamping = 0.4f;
    public float RecoilFactor;
    [Header("GalilStyleParams")]
    public Transform BaseClip;
    public float[] ExpandedLegYAngle;
    [Header("ExtendyBits")]
    public bool UsesExtendyBits;
    public Transform Bit1;
    public Transform Bit2;
    public Vector3 LocalPosContracted1;
    public Vector3 LocalPosContracted2;
    public Vector3 LocalPosExpanded1;
    public Vector3 LocalPosExpanded2;
    public AudioEvent AudEvent_BOpen;
    public AudioEvent AudEvent_BClose;
    [Header("MultiLength")]
    public bool UsesMultiLength;
    private int m_mlIndex;
    public Transform MLBit1;
    public Transform MLBit2;
    public List<Vector3> MLDistances = new List<Vector3>();
    public List<float> MLHeights = new List<float>();

    public bool IsBipodActive => this.m_isBipodActive;

    protected void Awake()
    {
      if ((Object) this.FireArm != (Object) null)
        this.FireArm.Bipod = this;
      this.Contract(false);
    }

    public void NextML()
    {
      if (!this.UsesMultiLength)
        return;
      ++this.m_mlIndex;
      if (this.m_mlIndex >= this.MLDistances.Count)
        this.m_mlIndex = 0;
      this.UpdateML();
    }

    public void PrevML()
    {
      if (!this.UsesMultiLength)
        return;
      if (this.m_mlIndex < 0)
        this.m_mlIndex = this.MLDistances.Count - 1;
      this.UpdateML();
    }

    private void UpdateML()
    {
      this.MLBit1.localPosition = this.MLDistances[this.m_mlIndex];
      this.MLBit2.localPosition = this.MLDistances[this.m_mlIndex];
      this.GroundContactReference.localPosition = new Vector3(0.0f, this.MLHeights[this.m_mlIndex], 0.0f);
    }

    private Vector3 GetUp() => (Object) this.OverrideTranslationFrame != (Object) null ? this.OverrideTranslationFrame.up : this.transform.up;

    private Vector3 GetForward() => (Object) this.OverrideTranslationFrame != (Object) null ? this.OverrideTranslationFrame.forward : this.transform.forward;

    private Vector3 GetRight() => (Object) this.OverrideTranslationFrame != (Object) null ? this.OverrideTranslationFrame.right : this.transform.right;

    public void Toggle()
    {
      if (this.m_isBipodExpanded)
        this.Contract(true);
      else
        this.Expand();
    }

    public Transform GetPointTo() => (Object) this.PointToOverride != (Object) null ? this.PointToOverride : this.GroundFollower;

    public void Expand()
    {
      if ((Object) this.FireArm == (Object) null)
        return;
      this.FireArm.ClearQuickbeltState();
      this.m_isBipodExpanded = true;
      if (this.FireArm is FVRFireArm)
        (this.FireArm as FVRFireArm).PlayAudioAsHandling(this.AudEvent_BOpen, this.transform.position);
      switch (this.Style)
      {
        case FVRFireArmBipod.BipodStyle.Independent:
          for (int index = 0; index < this.BipodFeet.Length; ++index)
          {
            this.BipodLegs[index].transform.localEulerAngles = new Vector3(this.UnFoldedXRot, 0.0f, 0.0f);
            this.BipodFeet[index].gameObject.SetActive(true);
          }
          break;
        case FVRFireArmBipod.BipodStyle.Galil:
          this.BaseClip.transform.localEulerAngles = new Vector3(this.UnFoldedXRot, 0.0f, 0.0f);
          for (int index = 0; index < this.BipodFeet.Length; ++index)
            this.BipodFeet[index].gameObject.SetActive(true);
          for (int index = 0; index < this.BipodLegs.Length; ++index)
            this.BipodLegs[index].transform.localEulerAngles = new Vector3(0.0f, this.ExpandedLegYAngle[index], 0.0f);
          break;
        case FVRFireArmBipod.BipodStyle.Vepr:
          for (int index = 0; index < this.BipodFeet.Length; ++index)
          {
            this.BipodLegs[index].transform.localEulerAngles = new Vector3(this.UnFoldedXRot, 0.0f, 0.0f);
            this.BipodFeet[index].gameObject.SetActive(true);
          }
          break;
        case FVRFireArmBipod.BipodStyle.RPK:
          for (int index = 0; index < this.BipodFeet.Length; ++index)
          {
            this.BipodLegs[index].transform.localEulerAngles = this.UnFoldedRPK[index];
            this.BipodFeet[index].gameObject.SetActive(true);
          }
          break;
        case FVRFireArmBipod.BipodStyle.Stoner63:
          for (int index = 0; index < this.BipodFeet.Length; ++index)
          {
            this.BipodLegs[index].transform.localEulerAngles = this.UnFoldedRPK[index];
            this.BipodLegs[index].transform.localPosition = this.UnFoldedPosStroner[index];
            this.BipodFeet[index].gameObject.SetActive(true);
          }
          break;
      }
      if (!this.UsesExtendyBits)
        return;
      this.Bit1.localPosition = this.LocalPosExpanded1;
      this.Bit2.localPosition = this.LocalPosExpanded2;
    }

    public void Contract(bool playSound)
    {
      if (this.m_isBipodActive)
        this.Deactivate();
      this.m_isBipodExpanded = false;
      if (playSound && (Object) this.FireArm != (Object) null && this.FireArm is FVRFireArm)
        (this.FireArm as FVRFireArm).PlayAudioAsHandling(this.AudEvent_BClose, this.transform.position);
      switch (this.Style)
      {
        case FVRFireArmBipod.BipodStyle.Independent:
          for (int index = 0; index < this.BipodFeet.Length; ++index)
          {
            this.BipodFeet[index].gameObject.SetActive(false);
            this.BipodLegs[index].transform.localEulerAngles = new Vector3(this.FoldedXRot, 0.0f, 0.0f);
          }
          break;
        case FVRFireArmBipod.BipodStyle.Galil:
          this.BaseClip.transform.localEulerAngles = new Vector3(this.FoldedXRot, 0.0f, 0.0f);
          for (int index = 0; index < this.BipodFeet.Length; ++index)
            this.BipodFeet[index].gameObject.SetActive(false);
          for (int index = 0; index < this.BipodLegs.Length; ++index)
            this.BipodLegs[index].transform.localEulerAngles = Vector3.zero;
          break;
        case FVRFireArmBipod.BipodStyle.Vepr:
          for (int index = 0; index < this.BipodFeet.Length; ++index)
          {
            this.BipodFeet[index].gameObject.SetActive(false);
            this.BipodLegs[index].transform.localEulerAngles = new Vector3(this.FoldedXRot, 0.0f, 0.0f);
          }
          break;
        case FVRFireArmBipod.BipodStyle.RPK:
          for (int index = 0; index < this.BipodFeet.Length; ++index)
          {
            this.BipodLegs[index].transform.localEulerAngles = this.FoldedRPK[index];
            this.BipodFeet[index].gameObject.SetActive(true);
          }
          break;
        case FVRFireArmBipod.BipodStyle.Stoner63:
          for (int index = 0; index < this.BipodFeet.Length; ++index)
          {
            this.BipodLegs[index].transform.localEulerAngles = this.FoldedRPK[index];
            this.BipodLegs[index].transform.localPosition = this.FoldedPosStroner[index];
            this.BipodFeet[index].gameObject.SetActive(true);
          }
          break;
      }
      if (!this.UsesExtendyBits)
        return;
      this.Bit1.localPosition = this.LocalPosContracted1;
      this.Bit2.localPosition = this.LocalPosContracted2;
    }

    public void Activate()
    {
      this.m_isBipodActive = true;
      if (!((Object) this.FireArm != (Object) null))
        return;
      this.FireArm.BipodActivated();
    }

    public void Deactivate()
    {
      this.m_isBipodActive = false;
      if (!((Object) this.FireArm != (Object) null))
        return;
      this.FireArm.BipodDeactivated();
    }

    public void UpdateBipod()
    {
      if (!this.m_isBipodExpanded)
        return;
      this.GroundFollower.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(this.FireArm.transform.forward, Vector3.up), Vector3.up);
      switch (this.Style)
      {
        case FVRFireArmBipod.BipodStyle.Independent:
          for (int index = 0; index < this.BipodLegs.Length; ++index)
          {
            Vector3 forward = Vector3.ProjectOnPlane(-Vector3.up, this.BipodLegs[index].transform.right);
            if ((double) Vector3.Dot(forward.normalized, -this.GetUp()) > 0.0)
              this.BipodLegs[index].rotation = Quaternion.LookRotation(forward, this.FireArm.transform.forward);
          }
          break;
        case FVRFireArmBipod.BipodStyle.Galil:
          Vector3 forward1 = Vector3.ProjectOnPlane(-Vector3.up, this.BaseClip.transform.right);
          if ((double) Vector3.Dot(forward1.normalized, -this.GetUp()) > 0.0)
          {
            this.BaseClip.rotation = Quaternion.LookRotation(forward1, this.FireArm.transform.forward);
            break;
          }
          break;
      }
      if (!this.m_isBipodActive && (Object) this.FireArm.AltGrip == (Object) null)
      {
        float f = Vector3.Dot(-this.GetUp(), Vector3.up);
        float num = Vector3.Dot(this.GetRight(), Vector3.up);
        if ((double) Mathf.Abs(f) <= 0.100000001490116 || (double) num <= 0.200000002980232)
          ;
        bool flag = false;
        float y = 0.0f;
        for (int index = 0; index < this.BipodFeet.Length; ++index)
        {
          Vector3 vector3 = this.BipodFeet[index].transform.position - this.BipodFeet[index].transform.parent.position;
          float magnitude = vector3.magnitude;
          Vector3 normalized = vector3.normalized;
          if (Physics.Raycast(this.BipodFeet[index].transform.parent.position, normalized, out this.m_hit, magnitude + 0.05f, (int) this.LM_BipodTouch, QueryTriggerInteraction.Ignore) && ((Object) this.m_hit.collider.attachedRigidbody == (Object) null || (Object) this.m_hit.collider.attachedRigidbody != (Object) null && this.m_hit.collider.attachedRigidbody.isKinematic))
          {
            Vector3 point = this.m_hit.point;
            if (flag)
            {
              if ((double) point.y < (double) y)
                y = point.y;
            }
            else
            {
              y = point.y;
              flag = true;
            }
          }
        }
        if (flag)
        {
          this.m_savedGroundPoint = new Vector3(this.GroundFollower.position.x, y, this.GroundFollower.position.z);
          this.Activate();
        }
      }
      if (this.m_isBipodActive)
        this.GroundFollowerTargetPoint = this.m_savedGroundPoint + Vector3.up * Mathf.Abs(Vector3.Distance(this.GroundFollower.position, this.GroundContactReference.position)) - this.GroundFollower.forward * this.RecoilFactor * this.RecoilDamping;
      if (!this.m_isBipodActive)
        return;
      bool flag1 = false;
      float y1 = 0.0f;
      for (int index = 0; index < this.BipodFeet.Length; ++index)
      {
        Vector3 vector3 = this.BipodFeet[index].transform.position - this.BipodFeet[index].transform.parent.position;
        float magnitude = vector3.magnitude;
        Vector3 normalized = vector3.normalized;
        if (Physics.Raycast(this.BipodFeet[index].transform.parent.position, normalized, out this.m_hit, magnitude + 0.15f, (int) this.LM_BipodTouch, QueryTriggerInteraction.Ignore) && (double) Vector3.Angle(this.m_hit.normal, Vector3.up) < 75.0)
        {
          Vector3 point = this.m_hit.point;
          if (flag1)
          {
            if ((double) point.y < (double) y1)
              y1 = point.y;
          }
          else
          {
            y1 = point.y;
            flag1 = true;
          }
        }
      }
      if (flag1)
      {
        if ((double) Vector3.Distance(new Vector3(this.m_savedGroundPoint.x, 0.0f, this.m_savedGroundPoint.z), new Vector3(this.GroundFollower.position.x, 0.0f, this.GroundFollower.position.z)) > 0.0500000007450581)
          this.m_savedGroundPoint = new Vector3(this.GroundContactReference.position.x, y1, this.GroundContactReference.position.z);
        else
          this.m_savedGroundPoint.y = y1;
      }
      else
        this.Deactivate();
      if ((double) Vector3.Angle(this.GetRight(), Vector3.up) >= 60.0 && (double) Vector3.Angle(this.GetRight(), Vector3.up) <= 120.0)
        return;
      this.Deactivate();
    }

    public Vector3 GetBipodRootWorld() => this.transform.position;

    public Vector3 GetOffsetSavedWorldPoint() => this.m_savedGroundPoint + Vector3.up * Mathf.Abs(Vector3.Distance(this.GroundFollower.position, this.GroundContactReference.position));

    [ContextMenu("Config")]
    public void Config() => this.FireArm = this.transform.root.GetComponent<FVRPhysicalObject>();

    public enum BipodStyle
    {
      Independent,
      Galil,
      Vepr,
      RPK,
      Stoner63,
    }
  }
}
