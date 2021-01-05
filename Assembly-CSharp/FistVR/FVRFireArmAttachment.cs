// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFireArmAttachment
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FVRFireArmAttachment : FVRPhysicalObject
  {
    [Header("Attachment Params")]
    public FVRFireArmAttachementMountType Type;
    public FVRFireArmAttachmentMount curMount;
    public FVRFireArmAttachmentSensor Sensor;
    public FVRFireArmAttachmentInterface AttachmentInterface;
    public AudioEvent AudClipAttach;
    public AudioEvent AudClipDettach;
    public bool IsBiDirectional = true;
    public bool CanScaleToMount = true;
    private Collider m_col;
    private bool m_hasCollider;
    protected bool m_isInSnappingMode;

    protected override void Awake()
    {
      base.Awake();
      this.m_col = this.GetComponent<Collider>();
      if (!((UnityEngine.Object) this.m_col != (UnityEngine.Object) null))
        return;
      this.m_hasCollider = true;
    }

    public void SetTriggerState(bool b)
    {
      if (!this.m_hasCollider)
        return;
      this.m_col.enabled = b;
    }

    public override bool IsInteractable() => !((UnityEngine.Object) this.curMount != (UnityEngine.Object) null);

    public virtual bool CanAttach() => true;

    public virtual bool CanDetach() => true;

    public override void BeginInteraction(FVRViveHand hand)
    {
      if ((UnityEngine.Object) this.curMount != (UnityEngine.Object) null)
        this.DetachFromMount();
      base.BeginInteraction(hand);
    }

    public FVRPhysicalObject GetRootObject()
    {
      if (!((UnityEngine.Object) this.curMount != (UnityEngine.Object) null))
        return (FVRPhysicalObject) this;
      return this.curMount.MyObject is FVRFireArmAttachment ? (this.curMount.MyObject as FVRFireArmAttachment).GetRootObject() : this.curMount.MyObject;
    }

    public override void UpdateInteraction(FVRViveHand hand) => base.UpdateInteraction(hand);

    public override void EndInteraction(FVRViveHand hand)
    {
      this.SetSnapping(false);
      base.EndInteraction(hand);
      if (!((UnityEngine.Object) this.Sensor.CurHoveredMount != (UnityEngine.Object) null))
        return;
      this.AttachToMount(this.Sensor.CurHoveredMount, true);
    }

    protected override Vector3 GetGrabPos() => (UnityEngine.Object) this.Sensor.CurHoveredMount != (UnityEngine.Object) null ? this.transform.position : base.GetGrabPos();

    protected override Quaternion GetGrabRot() => (UnityEngine.Object) this.Sensor.CurHoveredMount != (UnityEngine.Object) null ? this.transform.rotation : base.GetGrabRot();

    public void ScaleToMount(FVRFireArmAttachmentMount mount)
    {
      float scaleModifier = mount.GetRootMount().ScaleModifier;
      this.transform.localScale = new Vector3(scaleModifier, scaleModifier, scaleModifier);
    }

    protected override Vector3 GetPosTarget()
    {
      if (!((UnityEngine.Object) this.Sensor.CurHoveredMount != (UnityEngine.Object) null))
        return base.GetPosTarget();
      Vector3 closestValidPoint = this.GetClosestValidPoint(this.Sensor.CurHoveredMount.Point_Front.position, this.Sensor.CurHoveredMount.Point_Rear.position, this.m_handPos);
      return (double) Vector3.Distance(closestValidPoint, this.m_handPos) < 0.150000005960464 ? closestValidPoint : base.GetPosTarget();
    }

    protected override Quaternion GetRotTarget()
    {
      if (!((UnityEngine.Object) this.Sensor.CurHoveredMount != (UnityEngine.Object) null))
        return base.GetRotTarget();
      if (!this.IsBiDirectional)
        return this.Sensor.CurHoveredMount.transform.rotation;
      return (double) Vector3.Dot(this.transform.forward, this.Sensor.CurHoveredMount.transform.forward) >= 0.0 ? this.Sensor.CurHoveredMount.transform.rotation : Quaternion.LookRotation(-this.Sensor.CurHoveredMount.transform.forward, this.Sensor.CurHoveredMount.transform.up);
    }

    protected virtual void UpdateSnappingBasedOnDistance()
    {
      if ((UnityEngine.Object) this.Sensor.CurHoveredMount != (UnityEngine.Object) null)
      {
        Vector3 zero = Vector3.zero;
        if ((double) Vector3.Distance(this.Type != FVRFireArmAttachementMountType.Suppressor ? this.GetClosestValidPoint(this.Sensor.CurHoveredMount.Point_Front.position, this.Sensor.CurHoveredMount.Point_Rear.position, this.transform.position) : this.GetClosestValidPoint(this.Sensor.CurHoveredMount.Point_Front.position, (this.Sensor.CurHoveredMount.GetRootMount().MyObject as FVRFireArm).MuzzlePos.position, this.transform.position), this.transform.position) < 0.0799999982118607)
          this.SetSnapping(true);
        else
          this.SetSnapping(false);
      }
      else
        this.SetSnapping(false);
    }

    protected override void FVRFixedUpdate()
    {
      if (this.IsHeld)
        this.UpdateSnappingBasedOnDistance();
      base.FVRFixedUpdate();
    }

    protected virtual void SetSnapping(bool b)
    {
      if (this.m_isInSnappingMode == b)
        return;
      this.m_isInSnappingMode = b;
      if (this.m_isInSnappingMode)
        this.SetAllCollidersToLayer(false, "NoCol");
      else
        this.SetAllCollidersToLayer(false, "Default");
    }

    public virtual void AttachToMount(FVRFireArmAttachmentMount m, bool playSound)
    {
      if (!playSound)
        ;
      this.curMount = m;
      this.StoreAndDestroyRigidbody();
      if (this.curMount.GetRootMount().ParentToThis)
        this.SetParentage(this.curMount.GetRootMount().transform);
      else
        this.SetParentage(this.curMount.MyObject.transform);
      if (this.IsBiDirectional)
      {
        if ((double) Vector3.Dot(this.transform.forward, this.curMount.transform.forward) >= 0.0)
          this.transform.rotation = this.curMount.transform.rotation;
        else
          this.transform.rotation = Quaternion.LookRotation(-this.curMount.transform.forward, this.curMount.transform.up);
      }
      else
        this.transform.rotation = this.curMount.transform.rotation;
      this.transform.position = this.GetClosestValidPoint(this.curMount.Point_Front.position, this.curMount.Point_Rear.position, this.transform.position);
      if ((UnityEngine.Object) this.curMount.Parent != (UnityEngine.Object) null)
        this.curMount.Parent.RegisterAttachment(this);
      this.curMount.RegisterAttachment(this);
      if ((UnityEngine.Object) this.curMount.Parent != (UnityEngine.Object) null && (UnityEngine.Object) this.curMount.Parent.QuickbeltSlot != (UnityEngine.Object) null)
        this.SetAllCollidersToLayer(false, "NoCol");
      else
        this.SetAllCollidersToLayer(false, "Default");
      if ((UnityEngine.Object) this.AttachmentInterface != (UnityEngine.Object) null)
      {
        this.AttachmentInterface.OnAttach();
        this.AttachmentInterface.gameObject.SetActive(true);
      }
      this.SetTriggerState(false);
    }

    public void DetachFromMount()
    {
      if ((UnityEngine.Object) this.AttachmentInterface != (UnityEngine.Object) null)
      {
        this.AttachmentInterface.OnDetach();
        this.AttachmentInterface.gameObject.SetActive(false);
      }
      this.SetTriggerState(true);
      this.SetParentage((Transform) null);
      this.curMount.DeRegisterAttachment(this);
      if ((UnityEngine.Object) this.curMount.Parent != (UnityEngine.Object) null)
        this.curMount.Parent.DeRegisterAttachment(this);
      this.curMount = (FVRFireArmAttachmentMount) null;
      this.RecoverRigidbody();
    }

    public override void ConfigureFromFlagDic(Dictionary<string, string> f)
    {
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      if (!((UnityEngine.Object) this.AttachmentInterface != (UnityEngine.Object) null) || !(this.AttachmentInterface is Amplifier))
        return;
      string key1 = "m_zoomSettingIndex";
      if (f.ContainsKey(key1))
        (this.AttachmentInterface as Amplifier).m_zoomSettingIndex = Convert.ToInt32(f[key1]);
      string key2 = "ZeroDistanceIndex";
      if (f.ContainsKey(key2))
        (this.AttachmentInterface as Amplifier).ZeroDistanceIndex = Convert.ToInt32(f[key2]);
      string key3 = "ElevationStep";
      if (f.ContainsKey(key3))
        (this.AttachmentInterface as Amplifier).ElevationStep = Convert.ToInt32(f[key3]);
      string key4 = "WindageStep";
      if (!f.ContainsKey(key4))
        return;
      (this.AttachmentInterface as Amplifier).WindageStep = Convert.ToInt32(f[key4]);
    }

    public override Dictionary<string, string> GetFlagDic()
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      if ((UnityEngine.Object) this.AttachmentInterface != (UnityEngine.Object) null && this.AttachmentInterface is Amplifier)
      {
        dictionary.Add("m_zoomSettingIndex", (this.AttachmentInterface as Amplifier).m_zoomSettingIndex.ToString());
        dictionary.Add("ZeroDistanceIndex", (this.AttachmentInterface as Amplifier).ZeroDistanceIndex.ToString());
        dictionary.Add("ElevationStep", (this.AttachmentInterface as Amplifier).ElevationStep.ToString());
        dictionary.Add("WindageStep", (this.AttachmentInterface as Amplifier).WindageStep.ToString());
      }
      return dictionary;
    }
  }
}
