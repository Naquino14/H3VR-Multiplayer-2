// Decompiled with JetBrains decompiler
// Type: FistVR.FVRPhysicalObject
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FVRPhysicalObject : FVRInteractiveObject
  {
    [Header("Physical Object Config")]
    public FVRObject ObjectWrapper;
    public bool SpawnLockable;
    public bool Harnessable;
    public HandlingReleaseIntoSlotType HandlingReleaseIntoSlotSound = HandlingReleaseIntoSlotType.Generic;
    [HideInInspector]
    public bool m_isSpawnLock;
    [HideInInspector]
    public bool m_isHardnessed;
    public FVRPhysicalObject.FVRPhysicalObjectSize Size;
    public FVRQuickBeltSlot.QuickbeltSlotType QBSlotType;
    public float ThrowVelMultiplier = 1f;
    public float ThrowAngMultiplier = 1f;
    protected float AttachedRotationMultiplier = 60f;
    protected float AttachedPositionMultiplier = 9000f;
    protected float AttachedRotationFudge = 10000f;
    protected float AttachedPositionFudge = 10000f;
    public bool UsesGravity = true;
    public Rigidbody[] DependantRBs;
    protected FVRPhysicalObject.RigidbodyStoredParams StoredRBParams = new FVRPhysicalObject.RigidbodyStoredParams();
    public bool DistantGrabbable = true;
    public bool IsDebug;
    public bool IsAltHeld;
    public bool IsKinematicLocked;
    public bool DoesQuickbeltSlotFollowHead;
    public bool IsInWater;
    private Vector3 m_storedCOMLocal = Vector3.zero;
    public List<FVRFireArmAttachmentMount> AttachmentMounts;
    private List<FVRFireArmAttachment> AttachmentsList = new List<FVRFireArmAttachment>();
    private HashSet<FVRFireArmAttachment> AttachmentsHash = new HashSet<FVRFireArmAttachment>();
    private FVRQuickBeltSlot m_quickbeltSlot;
    private float m_timeSinceInQuickbelt = 10f;
    private FVRAlternateGrip m_altGrip;
    private FVRAlternateGrip savedGrip;
    public bool IsAltToAltTransfer;
    private FVRFireArmBipod m_bipod;
    private bool m_isPivotLocked;
    private Vector3 m_pivotLockedPos = Vector3.zero;
    private Quaternion m_pivotLockedRot = Quaternion.identity;
    public FVRPhysicalObject.FVRPhysicalSoundParams CollisionSound;
    private ItemSpawnerID m_IDSpawnedFrom;
    public bool IsPickUpLocked;
    private bool m_doesDirectParent = true;
    public FVRPhysicalObject.ObjectToHandOverrideMode OverridesObjectToHand;
    protected AudioImpactController audioImpactController;
    private bool m_hasImpactController;
    [Header("Melee Stuff")]
    public FVRPhysicalObject.MeleeParams MP;
    private float CheckDestroyTick = 1f;
    private Vector3 fakeHandPosWorld = Vector3.zero;
    private Vector3 fakeHandVelWorld = Vector3.zero;
    private float m_timeSinceFakeVelWorldTransfered = 1f;

    [HideInInspector]
    public Rigidbody RootRigidbody { get; set; }

    public List<FVRFireArmAttachment> Attachments => this.AttachmentsList;

    public float TimeSinceInQuickbelt => this.m_timeSinceInQuickbelt;

    [HideInInspector]
    public FVRQuickBeltSlot QuickbeltSlot => this.m_quickbeltSlot;

    public virtual void SetQuickBeltSlot(FVRQuickBeltSlot slot)
    {
      if ((UnityEngine.Object) slot != (UnityEngine.Object) null && !this.IsHeld)
      {
        if (this.AttachmentsList.Count > 0)
        {
          for (int index = 0; index < this.AttachmentsList.Count; ++index)
          {
            if ((UnityEngine.Object) this.AttachmentsList[index] != (UnityEngine.Object) null)
              this.AttachmentsList[index].SetAllCollidersToLayer(false, "NoCol");
          }
        }
      }
      else if (this.AttachmentsList.Count > 0)
      {
        for (int index = 0; index < this.AttachmentsList.Count; ++index)
        {
          if ((UnityEngine.Object) this.AttachmentsList[index] != (UnityEngine.Object) null)
            this.AttachmentsList[index].SetAllCollidersToLayer(false, "Default");
        }
      }
      if ((UnityEngine.Object) this.m_quickbeltSlot != (UnityEngine.Object) null && (UnityEngine.Object) slot != (UnityEngine.Object) this.m_quickbeltSlot)
      {
        this.m_quickbeltSlot.HeldObject = (FVRInteractiveObject) null;
        this.m_quickbeltSlot.CurObject = (FVRPhysicalObject) null;
        this.m_quickbeltSlot.IsKeepingTrackWithHead = false;
      }
      if ((UnityEngine.Object) slot != (UnityEngine.Object) null && !this.IsHeld)
      {
        this.SetAllCollidersToLayer(false, "NoCol");
        slot.HeldObject = (FVRInteractiveObject) this;
        slot.CurObject = this;
        slot.IsKeepingTrackWithHead = this.DoesQuickbeltSlotFollowHead;
      }
      else
        this.SetAllCollidersToLayer(false, "Default");
      this.m_quickbeltSlot = slot;
    }

    [HideInInspector]
    public FVRAlternateGrip AltGrip
    {
      get => this.m_altGrip;
      set
      {
        if (!((UnityEngine.Object) this.m_altGrip != (UnityEngine.Object) null) || !((UnityEngine.Object) value != (UnityEngine.Object) this.m_altGrip))
          ;
        if (!((UnityEngine.Object) value != (UnityEngine.Object) null))
          ;
        this.m_altGrip = value;
      }
    }

    [HideInInspector]
    public FVRFireArmBipod Bipod
    {
      get => this.m_bipod;
      set
      {
        if (!((UnityEngine.Object) this.m_bipod != (UnityEngine.Object) null) || !((UnityEngine.Object) value != (UnityEngine.Object) this.m_bipod))
          ;
        if (!((UnityEngine.Object) value != (UnityEngine.Object) null))
          ;
        this.m_bipod = value;
      }
    }

    public bool IsPivotLocked
    {
      get => this.m_isPivotLocked;
      set => this.m_isPivotLocked = value;
    }

    public Vector3 PivotLockPos
    {
      set => this.m_pivotLockedPos = value;
    }

    public Quaternion PivotLockRot
    {
      set => this.m_pivotLockedRot = value;
    }

    public ItemSpawnerID IDSpawnedFrom
    {
      get => this.m_IDSpawnedFrom;
      set => this.m_IDSpawnedFrom = value;
    }

    public bool DoesDirectParent => this.m_doesDirectParent;

    public AudioImpactController AudioImpactController => this.audioImpactController;

    public bool HasImpactController => this.m_hasImpactController;

    public void SetIFF(int iff)
    {
      if (!this.m_hasImpactController)
        return;
      this.audioImpactController.SetIFF(iff);
    }

    protected override void Awake()
    {
      if (GM.Options.QuickbeltOptions.ObjectToHandMode == QuickbeltOptions.ObjectToHandConnectionMode.Floating)
        this.m_doesDirectParent = false;
      if (this.OverridesObjectToHand == FVRPhysicalObject.ObjectToHandOverrideMode.Floating)
        this.m_doesDirectParent = false;
      else if (this.OverridesObjectToHand == FVRPhysicalObject.ObjectToHandOverrideMode.Direct)
        this.m_doesDirectParent = true;
      if ((UnityEngine.Object) this.ObjectWrapper == (UnityEngine.Object) null)
        this.SpawnLockable = false;
      base.Awake();
      if ((UnityEngine.Object) this.GetComponent<AudioSource>() != (UnityEngine.Object) null & this.CollisionSound.Clips.Length > 0)
      {
        this.CollisionSound.m_colSoundTick = 1f;
        this.CollisionSound.m_hasCollisionSound = true;
        this.CollisionSound.m_audioCollision = this.GetComponent<AudioSource>();
      }
      this.RootRigidbody = this.GetComponent<Rigidbody>();
      if ((UnityEngine.Object) this.RootRigidbody != (UnityEngine.Object) null)
      {
        this.m_storedCOMLocal = this.RootRigidbody.centerOfMass;
        this.RootRigidbody.interpolation = RigidbodyInterpolation.None;
        this.RootRigidbody.maxAngularVelocity = 100f;
        this.StoredRBParams.Mass = this.RootRigidbody.mass;
        this.StoredRBParams.Drag = this.RootRigidbody.drag;
        this.StoredRBParams.AngularDrag = this.RootRigidbody.angularDrag;
        this.StoredRBParams.Interpolation = this.RootRigidbody.interpolation;
        this.StoredRBParams.ColDetectMode = this.RootRigidbody.collisionDetectionMode;
      }
      this.audioImpactController = this.GetComponent<AudioImpactController>();
      if ((UnityEngine.Object) this.audioImpactController != (UnityEngine.Object) null)
      {
        this.m_hasImpactController = true;
        this.audioImpactController.SetIFF(-3);
      }
      if (!this.MP.IsMeleeWeapon)
        return;
      this.MP.InitMeleeParams(this);
    }

    private void UpdatePosesBasedOnCMode(FVRViveHand hand)
    {
      if (hand.CMode != ControlMode.Oculus && hand.CMode != ControlMode.Index || !((UnityEngine.Object) this.PoseOverride_Touch != (UnityEngine.Object) null))
        return;
      this.PoseOverride.localPosition = this.PoseOverride_Touch.localPosition;
      this.PoseOverride.localRotation = this.PoseOverride_Touch.localRotation;
    }

    public virtual int GetTutorialState() => 0;

    public override bool IsInteractable() => !this.IsPickUpLocked && base.IsInteractable();

    public override bool IsSelectionRestricted() => (UnityEngine.Object) this.QuickbeltSlot != (UnityEngine.Object) null && !this.QuickbeltSlot.IsSelectable || base.IsSelectionRestricted();

    public override bool IsDistantGrabbable() => !this.IsPivotLocked && (!((UnityEngine.Object) this.Bipod != (UnityEngine.Object) null) || !this.Bipod.IsBipodActive) && !this.IsPickUpLocked && this.DistantGrabbable;

    public void ToggleKinematicLocked()
    {
      if (!((UnityEngine.Object) this.QuickbeltSlot == (UnityEngine.Object) null))
        return;
      this.SetIsKinematicLocked(!this.IsKinematicLocked);
    }

    public void SetIsKinematicLocked(bool b)
    {
      if (!((UnityEngine.Object) this.QuickbeltSlot == (UnityEngine.Object) null))
        return;
      this.IsKinematicLocked = b;
      if (!this.IsKinematicLocked && this.RootRigidbody.isKinematic)
      {
        this.RootRigidbody.isKinematic = false;
      }
      else
      {
        if (!this.IsKinematicLocked || this.IsHeld)
          return;
        this.RootRigidbody.isKinematic = true;
      }
    }

    public virtual void BipodActivated()
    {
      this.UseFilteredHandPosition = true;
      this.UseSecondStepRotationFiltering = true;
      if (this.IsAltHeld && (UnityEngine.Object) this.m_hand != (UnityEngine.Object) null)
      {
        this.m_hand.EndInteractionIfHeld((FVRInteractiveObject) this);
        this.EndInteraction(this.m_hand);
      }
      if ((UnityEngine.Object) this.AltGrip != (UnityEngine.Object) null && (UnityEngine.Object) this.AltGrip.m_hand != (UnityEngine.Object) null)
      {
        Debug.Log((object) "ending");
        this.AltGrip.m_hand.ForceSetInteractable((FVRInteractiveObject) null);
        this.AltGrip = (FVRAlternateGrip) null;
      }
      this.SetParentage((Transform) null);
    }

    public virtual void BipodDeactivated()
    {
      if (this.RootRigidbody.isKinematic)
        this.RootRigidbody.isKinematic = false;
      this.UseFilteredHandPosition = false;
      this.UseSecondStepRotationFiltering = false;
      if (!this.IsHeld || !((UnityEngine.Object) this.transform.parent != (UnityEngine.Object) this.m_hand.WholeRig))
        return;
      this.SetParentage(this.m_hand.WholeRig);
    }

    private void ResetClampCOM()
    {
      this.RootRigidbody.ResetCenterOfMass();
      this.RootRigidbody.centerOfMass = new Vector3(Mathf.Clamp(this.RootRigidbody.centerOfMass.x, this.m_storedCOMLocal.x - 0.15f, this.m_storedCOMLocal.x + 0.15f), Mathf.Clamp(this.RootRigidbody.centerOfMass.y, this.m_storedCOMLocal.y - 0.15f, this.m_storedCOMLocal.y + 0.15f), Mathf.Clamp(this.RootRigidbody.centerOfMass.z, this.m_storedCOMLocal.z - 0.15f, this.m_storedCOMLocal.z + 0.15f));
    }

    public void RegisterAttachment(FVRFireArmAttachment attachment)
    {
      if (!this.AttachmentsHash.Add(attachment))
        return;
      this.AttachmentsList.Add(attachment);
      this.ResetClampCOM();
    }

    public void DeRegisterAttachment(FVRFireArmAttachment attachment)
    {
      if (!this.AttachmentsHash.Remove(attachment))
        return;
      this.AttachmentsList.Remove(attachment);
      this.ResetClampCOM();
    }

    protected virtual Vector3 GetGrabPos()
    {
      Vector3 position = this.Transform.position;
      if ((UnityEngine.Object) this.QBPoseOverride != (UnityEngine.Object) null && (UnityEngine.Object) this.QuickbeltSlot != (UnityEngine.Object) null && !this.IsHeld)
        return this.QBPoseOverride.position;
      if ((UnityEngine.Object) this.PoseOverride != (UnityEngine.Object) null)
        position = this.PoseOverride.position;
      if (this.UseGrabPointChild && this.UseGripRotInterp && ((UnityEngine.Object) this.m_grabPointTransform != (UnityEngine.Object) null && !this.IsAltHeld))
      {
        float posInterpTick = this.m_pos_interp_tick;
        return Vector3.Lerp(this.m_grabPointTransform.position, position, posInterpTick);
      }
      return (this.UseGrabPointChild || this.IsAltHeld) && ((UnityEngine.Object) this.m_grabPointTransform != (UnityEngine.Object) null && this.IsHeld) ? this.m_grabPointTransform.position : position;
    }

    protected virtual Quaternion GetGrabRot()
    {
      Quaternion b = this.Transform.rotation;
      if (!this.IsHeld && (UnityEngine.Object) this.QuickbeltSlot != (UnityEngine.Object) null && (UnityEngine.Object) this.QBPoseOverride != (UnityEngine.Object) null)
        b = !this.QuickbeltSlot.UseStraightAxisAlignment ? this.QBPoseOverride.rotation : this.transform.rotation;
      else if ((UnityEngine.Object) this.PoseOverride != (UnityEngine.Object) null)
        b = this.PoseOverride.rotation;
      if (this.UseGrabPointChild && this.UseGripRotInterp && ((UnityEngine.Object) this.m_grabPointTransform != (UnityEngine.Object) null && !this.IsAltHeld) && (UnityEngine.Object) this.AltGrip == (UnityEngine.Object) null)
      {
        float rotInterpTick = this.m_rot_interp_tick;
        return Quaternion.Slerp(this.m_grabPointTransform.rotation, b, rotInterpTick);
      }
      return (this.UseGrabPointChild || this.IsAltHeld) && ((UnityEngine.Object) this.m_grabPointTransform != (UnityEngine.Object) null && (UnityEngine.Object) this.AltGrip == (UnityEngine.Object) null) && this.IsHeld ? this.m_grabPointTransform.rotation : b;
    }

    protected virtual Vector3 GetPosTarget() => (UnityEngine.Object) this.QuickbeltSlot != (UnityEngine.Object) null && !this.IsHeld ? this.QuickbeltSlot.PoseOverride.position : this.m_handPos;

    protected virtual Quaternion GetRotTarget() => (UnityEngine.Object) this.QuickbeltSlot != (UnityEngine.Object) null && !this.IsHeld ? this.QuickbeltSlot.PoseOverride.rotation : this.m_handRot;

    protected override void FVRUpdate()
    {
      this.MP.UpdateTick(Time.deltaTime);
      if ((double) this.m_timeSinceFakeVelWorldTransfered >= 1.0)
        return;
      this.m_timeSinceFakeVelWorldTransfered += Time.deltaTime;
    }

    protected override void FVRFixedUpdate()
    {
      if ((UnityEngine.Object) this.Bipod != (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.AltGrip != (UnityEngine.Object) null && this.Bipod.IsBipodActive)
          this.Bipod.Deactivate();
        else
          this.Bipod.UpdateBipod();
      }
      this.MP.FixedUpdate(Time.deltaTime);
      base.FVRFixedUpdate();
      this.FU();
    }

    private void FU()
    {
      float fixedDeltaTime = Time.fixedDeltaTime;
      if ((double) this.m_timeSinceInQuickbelt < 10.0)
        this.m_timeSinceInQuickbelt += fixedDeltaTime;
      if ((UnityEngine.Object) this.m_quickbeltSlot != (UnityEngine.Object) null)
        this.m_timeSinceInQuickbelt = 0.0f;
      if ((double) this.CheckDestroyTick > 0.0)
      {
        this.CheckDestroyTick -= fixedDeltaTime;
      }
      else
      {
        this.CheckDestroyTick = UnityEngine.Random.Range(1f, 1.5f);
        if ((double) this.Transform.position.y < -1000.0)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }
      if (this.CollisionSound.m_hasCollisionSound && (double) this.CollisionSound.m_colSoundTick > 0.0)
        this.CollisionSound.m_colSoundTick -= fixedDeltaTime;
      if (!this.IsHeld && !((UnityEngine.Object) this.QuickbeltSlot != (UnityEngine.Object) null) && !this.IsPivotLocked)
        return;
      if ((UnityEngine.Object) this.RootRigidbody == (UnityEngine.Object) null)
        this.RecoverRigidbody();
      if (this.UseGrabPointChild && this.UseGripRotInterp && !this.IsAltHeld)
      {
        if ((UnityEngine.Object) this.Bipod != (UnityEngine.Object) null && this.Bipod.IsBipodActive)
          this.m_pos_interp_tick = 1f;
        else if ((double) this.m_pos_interp_tick < 1.0)
          this.m_pos_interp_tick += fixedDeltaTime * this.PositionInterpSpeed;
        else
          this.m_pos_interp_tick = 1f;
        if ((UnityEngine.Object) this.Bipod != (UnityEngine.Object) null && this.Bipod.IsBipodActive)
          this.m_rot_interp_tick = 1f;
        else if ((double) this.m_rot_interp_tick < 1.0)
          this.m_rot_interp_tick += fixedDeltaTime * this.RotationInterpSpeed;
        else
          this.m_rot_interp_tick = 1f;
      }
      Vector3 vector3_1;
      Quaternion quaternion;
      Vector3 position1;
      Quaternion rotation;
      if (this.IsPivotLocked)
      {
        vector3_1 = this.m_pivotLockedPos;
        quaternion = this.m_pivotLockedRot;
        position1 = this.transform.position;
        rotation = this.transform.rotation;
      }
      else
      {
        vector3_1 = this.GetPosTarget();
        quaternion = this.GetRotTarget();
        position1 = this.GetGrabPos();
        rotation = this.GetGrabRot();
      }
      Vector3 b1 = vector3_1 - position1;
      Quaternion b2 = quaternion * Quaternion.Inverse(rotation);
      bool flag1 = false;
      if ((UnityEngine.Object) this.Bipod != (UnityEngine.Object) null && this.Bipod.IsBipodActive)
        flag1 = true;
      bool flag2 = false;
      if (this is BreakActionWeapon && (UnityEngine.Object) (this as BreakActionWeapon).AltGrip != (UnityEngine.Object) null && !(this as BreakActionWeapon).IsLatched)
        flag2 = true;
      if (this.IsPivotLocked)
      {
        b1 = vector3_1 - position1;
        b2 = quaternion * Quaternion.Inverse(rotation);
      }
      else if (((UnityEngine.Object) this.AltGrip != (UnityEngine.Object) null && this.AltGrip.FunctionalityEnabled && !flag2 || flag1) && !GM.Options.ControlOptions.UseGunRigMode2)
      {
        Vector3 position2;
        Vector3 vector3_2;
        if ((UnityEngine.Object) this.AltGrip != (UnityEngine.Object) null)
        {
          position2 = this.AltGrip.GetPalmPos(this.m_doesDirectParent);
          vector3_2 = this.transform.InverseTransformPoint(this.AltGrip.PoseOverride.position);
        }
        else
        {
          position2 = this.Bipod.GetOffsetSavedWorldPoint();
          vector3_2 = this.transform.InverseTransformPoint(this.Bipod.GetBipodRootWorld());
        }
        Vector3 vector3_3 = this.transform.InverseTransformPoint(position2);
        this.transform.InverseTransformPoint(this.PoseOverride.position);
        float z = Mathf.Max(this.PoseOverride.localPosition.z + 0.05f, vector3_3.z);
        Vector3 position3 = new Vector3(vector3_3.x - vector3_2.x, vector3_3.y - vector3_2.y, z);
        Vector3 position4 = this.transform.TransformPoint(position3);
        Vector3 vector3_4 = Vector3.Cross(position4 - this.transform.position, this.m_hand.transform.right);
        if (flag1)
        {
          Vector3 from = Vector3.ProjectOnPlane(vector3_4, this.transform.forward);
          Vector3 vector3_5 = Vector3.ProjectOnPlane(Vector3.up, this.transform.forward);
          float t = Mathf.Clamp(Vector3.Angle(from, vector3_5) - 20f, 0.0f, 30f) * 0.1f;
          vector3_4 = Vector3.Slerp(vector3_5, vector3_4, t);
        }
        b2 = Quaternion.LookRotation((position4 - this.transform.position).normalized, vector3_4) * this.PoseOverride.localRotation * Quaternion.Inverse(rotation);
        if (!flag1 && GM.Options.ControlOptions.UseVirtualStock && (this is FVRFireArm && (this as FVRFireArm).HasActiveShoulderStock) && (UnityEngine.Object) (this as FVRFireArm).StockPos != (UnityEngine.Object) null)
        {
          FVRFireArm fvrFireArm = this as FVRFireArm;
          Vector3 vector3_5 = fvrFireArm.transform.InverseTransformPoint(fvrFireArm.StockPos.position);
          float num1 = Mathf.Abs(vector3_5.z - position3.z) - (this as FVRFireArm).GetRecoilZ();
          Vector3 vector3_6 = fvrFireArm.transform.InverseTransformPoint(position1);
          float num2 = Mathf.Abs(vector3_5.z - vector3_6.z) - (this as FVRFireArm).GetRecoilZ();
          Transform head = GM.CurrentPlayerBody.Head;
          head.transform.InverseTransformPoint(position4);
          head.transform.InverseTransformPoint(vector3_1);
          Vector3 position5 = GM.CurrentPlayerBody.Head.position - GM.CurrentPlayerBody.Head.forward * 0.1f - GM.CurrentPlayerBody.Head.up * 0.05f;
          Vector3 position6 = head.transform.InverseTransformPoint(position5);
          Vector3 vector3_7 = GM.CurrentPlayerBody.Head.transform.InverseTransformPoint(vector3_1);
          position6.x += vector3_7.x;
          position6.y += vector3_7.y + 0.05f;
          Vector3 vector3_8 = head.TransformPoint(position6);
          Vector3 normalized = (position4 - vector3_8).normalized;
          Vector3 a1 = vector3_8 + normalized * num2 - position1;
          Quaternion a2 = Quaternion.LookRotation((position4 - vector3_8).normalized, vector3_4) * this.PoseOverride.localRotation * Quaternion.Inverse(rotation);
          float t = Mathf.Clamp(Vector3.Distance(head.position, vector3_1) - 0.1f, 0.0f, 1f) * 5f;
          b1 = Vector3.Lerp(a1, b1, t);
          b2 = Quaternion.Slerp(a2, b2, t);
        }
      }
      else if (this.IsHeld && (UnityEngine.Object) this.AltGrip == (UnityEngine.Object) null && (!this.IsAltHeld && !GM.Options.ControlOptions.UseGunRigMode2) && (GM.Options.ControlOptions.UseVirtualStock && this is FVRFireArm && ((this as FVRFireArm).HasActiveShoulderStock && (UnityEngine.Object) (this as FVRFireArm).StockPos != (UnityEngine.Object) null)))
      {
        FVRFireArm fvrFireArm = this as FVRFireArm;
        float num = Mathf.Abs(fvrFireArm.transform.InverseTransformPoint(fvrFireArm.StockPos.position).z);
        Transform head = GM.CurrentPlayerBody.Head;
        Vector3 position2 = GM.CurrentPlayerBody.Head.position - head.forward * 0.1f - head.up * 0.05f;
        Vector3 position3 = head.transform.InverseTransformPoint(position2);
        Vector3 vector3_2 = head.TransformPoint(position3);
        Vector3 normalized = (fvrFireArm.PoseOverride.position - vector3_2).normalized;
        Vector3 a1 = vector3_2 + normalized * num - position1;
        Quaternion a2 = Quaternion.LookRotation((fvrFireArm.PoseOverride.position - vector3_2).normalized, this.m_hand.PointingTransform.up) * this.PoseOverride.localRotation * Quaternion.Inverse(rotation);
        float t = Mathf.Lerp(Mathf.Clamp(Vector3.Distance(head.position, vector3_1) - 0.1f, 0.0f, 1f) * 5f, 1f, (float) ((double) Vector3.Angle(head.forward, this.m_hand.PointingTransform.forward) / 40.0 - 0.200000002980232));
        b1 = Vector3.Lerp(a1, b1, t);
        b2 = Quaternion.Slerp(a2, b2, t);
      }
      float num3 = 1f;
      float angle;
      Vector3 axis;
      b2.ToAngleAxis(out angle, out axis);
      if ((double) angle > 180.0)
        angle -= 360f;
      if ((double) angle != 0.0)
      {
        Vector3 target = fixedDeltaTime * angle * axis * this.AttachedRotationMultiplier;
        this.RootRigidbody.angularVelocity = num3 * Vector3.MoveTowards(this.RootRigidbody.angularVelocity, target, this.AttachedRotationFudge * Time.fixedDeltaTime);
        if (this.UseSecondStepRotationFiltering)
        {
          float num1 = Mathf.Clamp(this.RootRigidbody.angularVelocity.magnitude * 0.35f, 0.0f, 1f);
          this.RootRigidbody.angularVelocity *= num1 * num1;
        }
      }
      this.RootRigidbody.velocity = Vector3.MoveTowards(this.RootRigidbody.velocity, b1 * this.AttachedPositionMultiplier * fixedDeltaTime, this.AttachedPositionFudge * fixedDeltaTime);
    }

    public void SetFakeHand(Vector3 v, Vector3 p)
    {
      this.fakeHandPosWorld = p;
      this.fakeHandVelWorld = v;
      this.m_timeSinceFakeVelWorldTransfered = 0.0f;
    }

    public float GettimeSinceFakeVelWorldTransfered() => this.m_timeSinceFakeVelWorldTransfered;

    public Vector3 GetHandPosWorld()
    {
      if ((double) this.m_timeSinceFakeVelWorldTransfered < 0.25)
        return this.fakeHandPosWorld;
      return this.IsHeld ? this.m_hand.Input.Pos : Vector3.zero;
    }

    public Vector3 GetHandVelWorld()
    {
      if ((double) this.m_timeSinceFakeVelWorldTransfered < 0.25)
        return this.fakeHandVelWorld;
      return this.IsHeld ? this.m_hand.Input.VelLinearWorld : Vector3.zero;
    }

    public virtual GameObject DuplicateFromSpawnLock(FVRViveHand hand)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.ObjectWrapper.GetGameObject(), this.Transform.position, this.Transform.rotation);
      FVRPhysicalObject component = gameObject.GetComponent<FVRPhysicalObject>();
      if (component is FVREntityProxy)
        (component as FVREntityProxy).Data.PrimeDataLists((component as FVREntityProxy).Flags);
      hand.ForceSetInteractable((FVRInteractiveObject) component);
      component.SetQuickBeltSlot((FVRQuickBeltSlot) null);
      component.BeginInteraction(hand);
      if (this.MP.IsMeleeWeapon && component.MP.IsThrownDisposable)
      {
        component.MP.IsCountingDownToDispose = true;
        if (component.MP.m_isThrownAutoAim)
        {
          component.MP.SetReadyToAim(true);
          component.MP.SetPose(this.MP.PoseIndex);
        }
      }
      return gameObject;
    }

    public virtual void BeginInteractionThroughAltGrip(FVRViveHand hand, FVRAlternateGrip grip)
    {
      if (this.m_hasImpactController)
        this.audioImpactController.SetIFF(GM.CurrentPlayerBody.GetPlayerIFF());
      this.IsAltToAltTransfer = this.IsAltHeld;
      this.IsAltHeld = true;
      this.savedGrip = grip;
      this.BeginInteraction(hand);
      this.m_hand.ForceSetInteractable((FVRInteractiveObject) this);
    }

    public override void BeginInteraction(FVRViveHand hand)
    {
      if ((UnityEngine.Object) this.Bipod != (UnityEngine.Object) null && this.Bipod.IsBipodActive)
        this.RootRigidbody.isKinematic = false;
      this.UpdatePosesBasedOnCMode(hand);
      this.RecoverDrag();
      if (this.m_hasImpactController)
        this.audioImpactController.SetIFF(GM.CurrentPlayerBody.GetPlayerIFF());
      if (this.m_isSpawnLock)
      {
        hand.EndInteractionIfHeld((FVRInteractiveObject) this);
        this.DuplicateFromSpawnLock(hand);
      }
      else
      {
        if (this.IsHeld && this.IsAltHeld && ((UnityEngine.Object) this.savedGrip != (UnityEngine.Object) null && (UnityEngine.Object) this.m_hand != (UnityEngine.Object) null))
        {
          if (!this.IsAltToAltTransfer)
          {
            this.IsAltHeld = false;
            this.AltGrip = this.savedGrip;
            if (this.AltGrip.HasLastGrabbedGrip())
              this.AltGrip.BeginInteractionFromAttachedGrip(this.AltGrip.GetLastGrabbedGrip(), this.m_hand);
            else
              this.AltGrip.BeginInteractionFromAttachedGrip((AttachableForegrip) null, this.m_hand);
            this.m_hand.CurrentInteractable = (FVRInteractiveObject) this.AltGrip;
            this.savedGrip = (FVRAlternateGrip) null;
          }
          else
          {
            this.IsAltHeld = true;
            this.AltGrip = this.savedGrip;
            this.m_hand.CurrentInteractable = (FVRInteractiveObject) null;
          }
        }
        if ((UnityEngine.Object) this.RootRigidbody != (UnityEngine.Object) null)
        {
          if (this.IsKinematicLocked && this.RootRigidbody.isKinematic)
            this.RootRigidbody.isKinematic = false;
          if (!this.IsKinematicLocked)
            this.RootRigidbody.isKinematic = false;
          this.RecoverDrag();
        }
        if (this.m_doesDirectParent && ((UnityEngine.Object) this.Transform.parent == (UnityEngine.Object) null || (UnityEngine.Object) this.Transform.parent != (UnityEngine.Object) hand.WholeRig))
          this.SetParentage(hand.WholeRig);
        if ((UnityEngine.Object) this.QuickbeltSlot != (UnityEngine.Object) null && !this.m_isHardnessed)
          this.SetQuickBeltSlot((FVRQuickBeltSlot) null);
        this.IsAltToAltTransfer = false;
        base.BeginInteraction(hand);
      }
      this.SetQuickBeltSlot(this.m_quickbeltSlot);
      if (!((UnityEngine.Object) this.Bipod != (UnityEngine.Object) null) || !this.Bipod.IsBipodActive)
        return;
      this.SetParentage((Transform) null);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (this.MP.IsMeleeWeapon)
        this.MP.MeleeUpdateInteraction(hand);
      if (!this.IsAltHeld || !((UnityEngine.Object) this.savedGrip != (UnityEngine.Object) null))
        return;
      this.savedGrip.PassHandInput(hand, (FVRInteractiveObject) this);
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      if ((UnityEngine.Object) this.RootRigidbody != (UnityEngine.Object) null)
        this.RootRigidbody.isKinematic = this.IsKinematicLocked;
      if (this.MP.IsMeleeWeapon && (this.MP.IsJointedToObject || this.MP.IsLodgedToObject))
        this.ClearQuickbeltState();
      this.RootRigidbody.useGravity = this.UsesGravity;
      if ((UnityEngine.Object) this.m_hand != (UnityEngine.Object) null)
      {
        float num = 1f;
        if (GM.CurrentPlayerBody.IsMuscleMeat || GM.CurrentPlayerBody.IsWeakMeat)
          num = GM.CurrentPlayerBody.GetMuscleMeatPower();
        this.RootRigidbody.angularVelocity = this.m_hand.Input.VelAngularWorld * this.ThrowAngMultiplier;
        this.RootRigidbody.velocity = this.m_hand.GetThrowLinearVelWorld() * this.ThrowVelMultiplier * num;
      }
      this.SetParentage((Transform) null);
      foreach (Rigidbody dependantRb in this.DependantRBs)
      {
        if ((UnityEngine.Object) dependantRb != (UnityEngine.Object) null)
          dependantRb.velocity = this.m_hand.Input.VelLinearWorld * this.ThrowVelMultiplier;
      }
      base.EndInteraction(hand);
      if (this.IsAltHeld)
        this.IsAltHeld = false;
      if ((UnityEngine.Object) this.AltGrip != (UnityEngine.Object) null)
      {
        this.IsAltHeld = true;
        this.savedGrip = this.AltGrip;
        this.AltGrip.m_hand.HandMadeGrabReleaseSound();
        this.BeginInteraction(this.AltGrip.m_hand);
        this.AltGrip.m_hand.ForceSetInteractable((FVRInteractiveObject) this);
      }
      else
        this.IsAltHeld = false;
      this.SetQuickBeltSlot(this.m_quickbeltSlot);
      this.MP.MeleeEndInteraction(hand);
      if (!((UnityEngine.Object) this.Bipod != (UnityEngine.Object) null) || !this.Bipod.IsBipodActive)
        return;
      this.RootRigidbody.isKinematic = true;
      this.SetParentage((Transform) null);
    }

    public virtual void ForceObjectIntoInventorySlot(FVRQuickBeltSlot slot)
    {
      this.SetQuickBeltSlot(slot);
      this.SetParentage(this.QuickbeltSlot.QuickbeltRoot);
      if ((UnityEngine.Object) this.m_grabPointTransform != (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.QBPoseOverride != (UnityEngine.Object) null)
        {
          this.m_grabPointTransform.position = this.QBPoseOverride.position;
          this.m_grabPointTransform.rotation = this.QBPoseOverride.rotation;
        }
        else if ((UnityEngine.Object) this.PoseOverride != (UnityEngine.Object) null)
        {
          this.m_grabPointTransform.position = this.PoseOverride.position;
          this.m_grabPointTransform.rotation = this.PoseOverride.rotation;
        }
      }
      if (this.IsAltHeld)
        this.IsAltHeld = false;
      this.SetQuickBeltSlot(this.m_quickbeltSlot);
    }

    public virtual void EndInteractionIntoInventorySlot(FVRViveHand hand, FVRQuickBeltSlot slot)
    {
      if ((UnityEngine.Object) this.Bipod != (UnityEngine.Object) null)
      {
        this.Bipod.Contract(true);
        this.RootRigidbody.isKinematic = false;
      }
      this.SetQuickBeltSlot(hand.CurrentHoveredQuickbeltSlot);
      this.SetParentage(this.QuickbeltSlot.QuickbeltRoot);
      if (this.HandlingReleaseIntoSlotSound != HandlingReleaseIntoSlotType.None)
        SM.PlayHandlingReleaseIntoSlotSound(this.HandlingReleaseIntoSlotSound, hand.Input.Pos);
      if ((UnityEngine.Object) this.m_grabPointTransform != (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.QBPoseOverride != (UnityEngine.Object) null)
        {
          this.m_grabPointTransform.position = this.QBPoseOverride.position;
          this.m_grabPointTransform.rotation = this.QBPoseOverride.rotation;
        }
        else if ((UnityEngine.Object) this.PoseOverride != (UnityEngine.Object) null)
        {
          this.m_grabPointTransform.position = this.PoseOverride.position;
          this.m_grabPointTransform.rotation = this.PoseOverride.rotation;
        }
      }
      if (this.IsAltHeld)
        this.IsAltHeld = false;
      if ((UnityEngine.Object) this.AltGrip != (UnityEngine.Object) null)
      {
        this.AltGrip.m_hand.EndInteractionIfHeld((FVRInteractiveObject) this.AltGrip);
        this.AltGrip.EndInteraction(this.AltGrip.m_hand);
      }
      base.EndInteraction(hand);
      this.SetQuickBeltSlot(this.m_quickbeltSlot);
    }

    public override void ForceBreakInteraction()
    {
      base.ForceBreakInteraction();
      if (!((UnityEngine.Object) this.RootRigidbody != (UnityEngine.Object) null))
        return;
      this.RootRigidbody.useGravity = this.UsesGravity;
    }

    public virtual void ClearQuickbeltState()
    {
      this.m_isSpawnLock = false;
      this.m_isHardnessed = false;
      this.SetQuickBeltSlot((FVRQuickBeltSlot) null);
    }

    public virtual void ToggleQuickbeltState()
    {
      if (this.SpawnLockable)
      {
        this.m_isSpawnLock = !this.m_isSpawnLock;
      }
      else
      {
        if (!this.Harnessable)
          return;
        this.m_isHardnessed = !this.m_isHardnessed;
        if (!this.m_isHardnessed && this.IsHeld)
          this.SetQuickBeltSlot((FVRQuickBeltSlot) null);
        else
          this.SetQuickBeltSlot(this.m_quickbeltSlot);
      }
    }

    public virtual void OnCollisionEnter(Collision col)
    {
      if (this.gameObject.activeInHierarchy && this.CollisionSound.m_hasCollisionSound && (double) this.CollisionSound.m_colSoundTick <= 0.0 && ((double) col.relativeVelocity.magnitude >= 0.1 && (UnityEngine.Object) col.collider.attachedRigidbody == (UnityEngine.Object) null))
      {
        this.CollisionSound.m_colSoundTick = this.CollisionSound.ColSoundCooldown;
        this.CollisionSound.m_audioCollision.PlayOneShot(this.CollisionSound.Clips[UnityEngine.Random.Range(0, this.CollisionSound.Clips.Length)], this.CollisionSound.ColSoundVolume * Mathf.Clamp(col.relativeVelocity.magnitude * 0.5f, 0.0f, 1f));
      }
      if (!this.MP.IsMeleeWeapon)
        return;
      this.MP.OnCollisionEnter(col);
    }

    public void StoreAndDestroyRigidbody()
    {
      if (!((UnityEngine.Object) this.RootRigidbody != (UnityEngine.Object) null))
        return;
      this.StoredRBParams.Mass = this.RootRigidbody.mass;
      this.StoredRBParams.Drag = this.RootRigidbody.drag;
      this.StoredRBParams.AngularDrag = this.RootRigidbody.angularDrag;
      this.StoredRBParams.Interpolation = this.RootRigidbody.interpolation;
      this.StoredRBParams.ColDetectMode = this.RootRigidbody.collisionDetectionMode;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.RootRigidbody);
    }

    public void RecoverRigidbody()
    {
      if (!((UnityEngine.Object) this.RootRigidbody == (UnityEngine.Object) null))
        return;
      this.RootRigidbody = this.gameObject.AddComponent<Rigidbody>();
      this.RootRigidbody.mass = this.StoredRBParams.Mass;
      this.RootRigidbody.drag = this.StoredRBParams.Drag;
      this.RootRigidbody.angularDrag = this.StoredRBParams.AngularDrag;
      this.RootRigidbody.interpolation = this.StoredRBParams.Interpolation;
      this.RootRigidbody.collisionDetectionMode = this.StoredRBParams.ColDetectMode;
      this.RootRigidbody.maxAngularVelocity = 100f;
    }

    public void RecoverDrag()
    {
      if (!((UnityEngine.Object) this.RootRigidbody != (UnityEngine.Object) null))
        return;
      this.RootRigidbody.drag = this.StoredRBParams.Drag;
      this.RootRigidbody.angularDrag = this.StoredRBParams.AngularDrag;
    }

    public void SetParentage(Transform t) => this.Transform.SetParent(t);

    public virtual void ConfigureFromFlagDic(Dictionary<string, string> f)
    {
    }

    public virtual Dictionary<string, string> GetFlagDic() => (Dictionary<string, string>) null;

    public override void TestHandDistance()
    {
    }

    [ContextMenu("TestCollidersNegative")]
    public void TestCollidersNegative()
    {
    }

    public void SetAnimatedComponent(
      Transform t,
      float val,
      FVRPhysicalObject.InterpStyle interp,
      FVRPhysicalObject.Axis axis)
    {
      switch (interp)
      {
        case FVRPhysicalObject.InterpStyle.Translate:
          Vector3 localPosition = t.localPosition;
          switch (axis)
          {
            case FVRPhysicalObject.Axis.X:
              localPosition.x = val;
              break;
            case FVRPhysicalObject.Axis.Y:
              localPosition.y = val;
              break;
            case FVRPhysicalObject.Axis.Z:
              localPosition.z = val;
              break;
          }
          t.localPosition = localPosition;
          break;
        case FVRPhysicalObject.InterpStyle.Rotation:
          Vector3 zero = Vector3.zero;
          switch (axis)
          {
            case FVRPhysicalObject.Axis.X:
              zero.x = val;
              break;
            case FVRPhysicalObject.Axis.Y:
              zero.y = val;
              break;
            case FVRPhysicalObject.Axis.Z:
              zero.z = val;
              break;
          }
          t.localEulerAngles = zero;
          break;
      }
    }

    public struct RigidbodyStoredParams
    {
      public float Mass;
      public float Drag;
      public float AngularDrag;
      public RigidbodyInterpolation Interpolation;
      public CollisionDetectionMode ColDetectMode;
    }

    public enum FVRPhysicalObjectSize
    {
      Small = 0,
      Medium = 1,
      Large = 2,
      Massive = 3,
      CantCarryBig = 5,
    }

    [Serializable]
    public class FVRPhysicalSoundParams
    {
      public AudioClip[] Clips;
      public float ColSoundCooldown = 0.65f;
      public float ColSoundVolume = 0.1f;
      [HideInInspector]
      public bool m_hasCollisionSound;
      [HideInInspector]
      public AudioSource m_audioCollision;
      [HideInInspector]
      public float m_colSoundTick = 1f;
    }

    public enum ObjectToHandOverrideMode
    {
      None,
      Direct,
      Floating,
    }

    public enum InterpStyle
    {
      Translate,
      Rotation,
    }

    public enum Axis
    {
      X,
      Y,
      Z,
    }

    [Serializable]
    public class MeleeParams
    {
      private FVRPhysicalObject m_obj;
      public bool IsMeleeWeapon;
      public List<Rigidbody> IgnoreRBs = new List<Rigidbody>();
      [Header("Transforms")]
      public Transform HandPoint;
      public Transform EndPoint;
      [Header("Damage Params")]
      public Vector3 BaseDamageBCP = new Vector3(0.0f, 0.0f, 0.0f);
      public Vector3 HighDamageBCP = new Vector3(0.0f, 0.0f, 0.0f);
      public Vector3 StabDamageBCP = new Vector3(0.0f, 0.0f, 0.0f);
      public Vector3 TearDamageBCP = new Vector3(0.0f, 0.0f, 0.0f);
      public List<Collider> HighDamageColliders;
      public List<Transform> HighDamageVectors;
      [Header("Pose Params")]
      public bool DoesCyclePosePoints;
      public Transform[] PosePoints;
      protected int m_poseIndex;
      [Header("Thrown Params")]
      public bool IsThrownDisposable;
      private bool m_isCountingDownToDispose;
      public bool m_isThrownAutoAim;
      private float m_countDownToDestroy = 10f;
      public LayerMask ThrownDetectMask;
      public bool StartThrownDisposalTickdownOnSpawn;
      public bool IsLongThrowable;
      public bool IsThrowableDirInverted = true;
      [Header("Stabbing Params")]
      public bool CanNewStab;
      public bool ForceStab;
      public float BladeLength = 1f;
      public float MassWhileStabbed = 10f;
      public Transform StabDirection;
      public float StabAngularThreshold = 20f;
      public List<Collider> StabColliders;
      public float StabVelocityRequirement = 1f;
      public bool CanTearOut;
      public float TearOutVelThreshold = 3f;
      protected bool m_isJointedToObject;
      protected FixedJoint m_stabJoint;
      protected Rigidbody m_stabTargetRB;
      protected Vector3 m_initialStabPointWorld;
      protected Vector3 m_initialStabPointLocal;
      protected Vector3 m_relativeStabDir;
      protected SosigLink m_stabbedLink;
      protected Vector3 m_initialPosOfStabbedThingLocal;
      [Header("Lodging Params")]
      public bool CanNewLodge;
      public float LodgeDepth = 0.04f;
      public float MassWhileLodged = 10f;
      public Transform[] LodgeDirections;
      public List<Collider> LodgeColliders;
      public float LodgeVelocityRequirement = 4f;
      public float DeLodgeVelocityRequirement = 1f;
      private bool m_isLodgedToObject;
      private FixedJoint m_lodgeJoint;
      private Rigidbody m_lodgeTargetRB;
      private Vector3 m_initialLodgeNormal;
      [Header("Sweep Damage")]
      public bool UsesSweepTesting;
      public bool UsesSweepDebug;
      public List<Transform> TestCols = new List<Transform>();
      public Transform SweepTransformStart;
      public Transform SweepTransformEnd;
      public LayerMask LM_DamageTest;
      protected bool m_isReadyToAim;
      private Vector3 m_lastHandPoint = Vector3.zero;
      private Vector3 m_lastEndPoint = Vector3.zero;
      private Vector3 handPointVelocity = Vector3.zero;
      private Vector3 endPointVelocity = Vector3.zero;
      private float m_pointDistance;
      private float m_lastangFlick;
      private float m_lastangFlickLinear;
      private Vector3 m_SweepPointStart;
      private Vector3 m_SweepPointEnd;
      private Vector3 m_lastSweepPointStart;
      private Vector3 m_lastSweepPointEnd;
      private RaycastHit m_dhit;
      private float m_timeSinceLastDamageDone;
      protected float timeSinceStateChange = 10f;
      protected Vector3 stabDistantPoint = Vector3.one;
      protected Vector3 stabInsidePoint = Vector3.one;
      protected float m_initialMass;
      protected bool m_initRot;

      public int PoseIndex => this.m_poseIndex;

      public bool IsCountingDownToDispose
      {
        get => this.m_isCountingDownToDispose;
        set => this.m_isCountingDownToDispose = value;
      }

      public bool IsJointedToObject => this.m_isJointedToObject;

      public bool IsLodgedToObject => this.m_isLodgedToObject;

      public void SetReadyToAim(bool b) => this.m_isReadyToAim = b;

      public bool GetReadyToAim() => this.m_isReadyToAim;

      public void InitMeleeParams(FVRPhysicalObject o)
      {
        this.m_obj = o;
        this.m_lastHandPoint = this.HandPoint.position;
        this.m_lastEndPoint = this.EndPoint.position;
        this.m_SweepPointStart = this.HandPoint.position;
        this.m_SweepPointEnd = this.HandPoint.position;
        this.m_lastSweepPointStart = this.HandPoint.position;
        this.m_lastSweepPointEnd = this.HandPoint.position;
        this.m_pointDistance = Vector3.Distance(this.HandPoint.position, this.EndPoint.position);
        if ((UnityEngine.Object) this.m_obj.PoseOverride_Touch != (UnityEngine.Object) null && GM.HMDMode == ControlMode.Oculus && this.PosePoints.Length > 0)
        {
          this.PosePoints[0].localPosition = this.m_obj.PoseOverride_Touch.localPosition;
          this.PosePoints[0].localRotation = this.m_obj.PoseOverride_Touch.localRotation;
        }
        if (!this.StartThrownDisposalTickdownOnSpawn)
          return;
        this.IsThrownDisposable = true;
        this.m_isCountingDownToDispose = true;
      }

      public void SetPose(int i) => this.m_poseIndex = i;

      public void UpdateTick(float t)
      {
        if (!this.IsMeleeWeapon)
          return;
        if (this.m_obj.IsHeld || (UnityEngine.Object) this.m_obj.QuickbeltSlot != (UnityEngine.Object) null)
          this.m_countDownToDestroy = 10f;
        if (this.m_obj.IsHeld || !this.m_isCountingDownToDispose)
          return;
        this.m_countDownToDestroy -= Time.deltaTime;
        if ((double) this.m_countDownToDestroy > 0.0)
          return;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_obj.gameObject);
      }

      public void MeleeUpdateInteraction(FVRViveHand hand)
      {
        if (!this.DoesCyclePosePoints)
          return;
        if (hand.IsInStreamlinedMode)
        {
          if (hand.Input.BYButtonDown)
          {
            --this.m_poseIndex;
            if (this.m_poseIndex < 0)
              this.m_poseIndex = this.PosePoints.Length - 1;
          }
          if (!hand.Input.AXButtonDown)
            return;
          ++this.m_poseIndex;
          if (this.m_poseIndex < this.PosePoints.Length)
            return;
          this.m_poseIndex = 0;
        }
        else
        {
          if (!hand.Input.TouchpadDown)
            return;
          if ((double) Vector2.Angle(hand.Input.TouchpadAxes, Vector2.left) < 45.0)
          {
            --this.m_poseIndex;
            if (this.m_poseIndex >= 0)
              return;
            this.m_poseIndex = this.PosePoints.Length - 1;
          }
          else
          {
            if ((double) Vector2.Angle(hand.Input.TouchpadAxes, Vector2.right) >= 45.0)
              return;
            ++this.m_poseIndex;
            if (this.m_poseIndex < this.PosePoints.Length)
              return;
            this.m_poseIndex = 0;
          }
        }
      }

      public void MeleeEndInteraction(FVRViveHand hand)
      {
        if (!this.IsMeleeWeapon || !this.GetReadyToAim())
          return;
        this.SetReadyToAim(false);
        Vector3 velocity = this.m_obj.RootRigidbody.velocity;
        if ((double) this.m_obj.RootRigidbody.velocity.magnitude <= 0.300000011920929)
          return;
        Collider[] colliderArray = Physics.OverlapCapsule(this.m_obj.transform.position, this.m_obj.transform.position + GM.CurrentPlayerBody.Head.forward * 40f, 4f, (int) this.ThrownDetectMask, QueryTriggerInteraction.Collide);
        if (colliderArray.Length <= 0)
          return;
        float num1 = 40f;
        Collider collider = (Collider) null;
        bool flag = false;
        Vector3 vector3 = Vector3.one;
        for (int index = 0; index < colliderArray.Length; ++index)
        {
          if (colliderArray[index].transform.gameObject.GetComponent<AIEntity>().IFFCode >= 0)
          {
            Vector3 to = colliderArray[index].transform.position - this.m_obj.transform.position;
            float num2 = Vector3.Angle(velocity, to);
            if ((double) num2 <= 25.0 && (double) num2 >= 1.0)
            {
              float magnitude = to.magnitude;
              if ((double) magnitude < (double) num1)
              {
                num1 = magnitude;
                collider = colliderArray[index];
                vector3 = to;
                flag = true;
              }
            }
          }
        }
        if (!flag)
          return;
        Debug.Log((object) "Found one");
        vector3.y = GM.CurrentPlayerBody.Head.forward.y + 0.1f;
        this.m_obj.RootRigidbody.velocity = vector3.normalized * velocity.magnitude * 2f;
      }

      public void FixedUpdate(float t)
      {
        if (!this.IsMeleeWeapon)
          return;
        if (this.IsLongThrowable && !this.m_obj.IsHeld && !this.m_obj.RootRigidbody.isKinematic && (double) this.m_obj.RootRigidbody.velocity.magnitude > 5.0)
        {
          Vector3 forward = this.m_obj.RootRigidbody.velocity.normalized;
          if (this.IsThrowableDirInverted)
            forward = -forward;
          this.m_obj.RootRigidbody.MoveRotation(Quaternion.Slerp(this.m_obj.RootRigidbody.rotation, Quaternion.LookRotation(forward, Vector3.up), Time.deltaTime * 18.5f));
        }
        if ((double) this.timeSinceStateChange < 2.0)
          this.timeSinceStateChange += Time.deltaTime * 1f;
        if ((UnityEngine.Object) this.m_obj.RootRigidbody != (UnityEngine.Object) null && (UnityEngine.Object) this.HandPoint != (UnityEngine.Object) null && (UnityEngine.Object) this.EndPoint != (UnityEngine.Object) null)
        {
          if (this.DoesCyclePosePoints)
          {
            this.m_obj.PoseOverride.position = Vector3.Slerp(this.m_obj.PoseOverride.position, this.PosePoints[this.m_poseIndex].position, Time.deltaTime * 6f);
            this.m_obj.PoseOverride.rotation = Quaternion.Slerp(this.m_obj.PoseOverride.rotation, this.PosePoints[this.m_poseIndex].rotation, Time.deltaTime * 6f);
          }
          if ((double) this.m_timeSinceLastDamageDone < 1.0)
            this.m_timeSinceLastDamageDone += Time.deltaTime;
          this.handPointVelocity = Vector3.ClampMagnitude(this.handPointVelocity, Mathf.Lerp(this.handPointVelocity.magnitude, 0.0f, Time.deltaTime * 3f));
          this.endPointVelocity = Vector3.ClampMagnitude(this.endPointVelocity, Mathf.Lerp(this.endPointVelocity.magnitude, 0.0f, Time.deltaTime * 3f));
          this.handPointVelocity += this.m_obj.RootRigidbody.GetPointVelocity(this.HandPoint.position) * Time.deltaTime;
          this.endPointVelocity += this.m_obj.RootRigidbody.GetPointVelocity(this.EndPoint.position) * Time.deltaTime;
        }
        if (this.UsesSweepTesting)
        {
          this.m_SweepPointStart = this.m_obj.GetClosestValidPoint(this.HandPoint.position, this.EndPoint.position, this.SweepTransformStart.position);
          this.m_SweepPointEnd = this.m_obj.GetClosestValidPoint(this.HandPoint.position, this.EndPoint.position, this.SweepTransformEnd.position);
        }
        HashSet<IFVRDamageable> fvrDamageableSet = new HashSet<IFVRDamageable>();
        if (this.UsesSweepTesting && ((double) this.m_obj.RootRigidbody.velocity.magnitude > 3.5 || (double) this.m_obj.RootRigidbody.angularVelocity.magnitude > 3.5) && this.m_obj.IsHeld)
        {
          bool flag = false;
          for (int index = 0; index < 10; ++index)
          {
            float t1 = (float) index / 10f;
            Vector3 vector3_1 = Vector3.Lerp(this.m_lastSweepPointStart, this.m_lastSweepPointEnd, t1);
            Vector3 a = Vector3.Lerp(this.m_SweepPointStart, this.m_SweepPointEnd, t1);
            Vector3 vector3_2 = a - vector3_1;
            Vector3 vector3_3 = a + vector3_2;
            if (this.UsesSweepDebug)
            {
              this.TestCols[index].transform.position = a;
              this.TestCols[index].transform.rotation = Quaternion.LookRotation(a - vector3_1);
              this.TestCols[index].transform.localScale = new Vector3(0.01f, 0.01f, Vector3.Distance(a, vector3_1) * 3f);
            }
            Vector3 normalized = vector3_2.normalized;
            if (Physics.Raycast(vector3_1, normalized, out this.m_dhit, vector3_2.magnitude, (int) this.LM_DamageTest, QueryTriggerInteraction.Ignore))
            {
              IFVRDamageable component = this.m_dhit.collider.transform.gameObject.GetComponent<IFVRDamageable>();
              if (component == null && (UnityEngine.Object) this.m_dhit.collider.attachedRigidbody != (UnityEngine.Object) null)
                component = this.m_dhit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
              if (component != null && !fvrDamageableSet.Contains(component))
              {
                Damage dam = new Damage();
                dam.Class = Damage.DamageClass.Melee;
                dam.point = this.m_dhit.point;
                dam.hitNormal = this.m_dhit.normal;
                dam.Source_IFF = GM.CurrentPlayerBody.GetPlayerIFF();
                dam.strikeDir = this.m_obj.RootRigidbody.GetPointVelocity(this.m_dhit.point).normalized;
                dam.damageSize = 0.02f;
                dam.edgeNormal = this.m_obj.transform.forward;
                float num = Mathf.Clamp(this.GetIntertiaFromPoint(this.m_dhit.point).magnitude, 0.0f, 1f);
                if ((double) num > 0.25)
                {
                  if ((double) this.GetMultiplierForStrikeDir(normalized, this.HighDamageVectors) > 0.5)
                  {
                    dam.Dam_Blunt = this.HighDamageBCP.x * num;
                    dam.Dam_Cutting = this.HighDamageBCP.y * num;
                    dam.Dam_Piercing = this.HighDamageBCP.z * num;
                  }
                  else
                  {
                    dam.Dam_Blunt = this.BaseDamageBCP.x * num;
                    dam.Dam_Cutting = this.BaseDamageBCP.y * num;
                    dam.Dam_Piercing = this.BaseDamageBCP.z * num;
                  }
                  dam.Dam_TotalKinetic = dam.Dam_Blunt + dam.Dam_Cutting + dam.Dam_Piercing;
                  if (GM.CurrentPlayerBody.IsMuscleMeat || GM.CurrentPlayerBody.IsWeakMeat)
                  {
                    dam.Dam_Blunt *= GM.CurrentPlayerBody.GetMuscleMeatPower();
                    dam.Dam_Piercing *= GM.CurrentPlayerBody.GetMuscleMeatPower();
                    dam.Dam_Piercing *= GM.CurrentPlayerBody.GetMuscleMeatPower();
                    dam.Dam_TotalKinetic *= GM.CurrentPlayerBody.GetMuscleMeatPower();
                  }
                  this.m_timeSinceLastDamageDone = 0.0f;
                  fvrDamageableSet.Add(component);
                  component.Damage(dam);
                  flag = true;
                }
              }
            }
          }
          if (flag)
          {
            this.handPointVelocity = Vector3.zero;
            this.endPointVelocity = Vector3.zero;
          }
        }
        if ((double) this.m_timeSinceLastDamageDone > 0.200000002980232)
          fvrDamageableSet.Clear();
        if (this.m_isJointedToObject && ((UnityEngine.Object) this.m_stabJoint == (UnityEngine.Object) null || (UnityEngine.Object) this.m_stabTargetRB == (UnityEngine.Object) null || (UnityEngine.Object) this.m_stabbedLink == (UnityEngine.Object) null))
        {
          this.m_isJointedToObject = false;
          this.m_obj.SetCollidersToLayer(this.StabColliders, false, "Default");
          this.m_obj.RootRigidbody.mass = this.m_initialMass;
          this.timeSinceStateChange = 0.0f;
        }
        if (this.m_isLodgedToObject && ((UnityEngine.Object) this.m_lodgeJoint == (UnityEngine.Object) null || (UnityEngine.Object) this.m_lodgeTargetRB == (UnityEngine.Object) null))
        {
          this.m_isLodgedToObject = false;
          this.m_obj.SetCollidersToLayer(this.LodgeColliders, false, "Default");
          this.m_obj.RootRigidbody.mass = this.m_initialMass;
          this.timeSinceStateChange = 0.0f;
        }
        if (this.m_isJointedToObject && (this.m_obj.IsHeld || (double) this.m_obj.GettimeSinceFakeVelWorldTransfered() < 0.100000001490116))
        {
          Vector3 vector3 = this.m_stabTargetRB.transform.InverseTransformPoint(this.m_obj.transform.position + (this.m_obj.GetHandPosWorld() - this.m_obj.PoseOverride.position));
          this.m_stabJoint.connectedAnchor = this.m_obj.GetClosestValidPoint(this.stabDistantPoint, this.stabInsidePoint, vector3);
          if (this.CanStateChange())
          {
            bool flag = false;
            if (this.CanTearOut)
            {
              Vector3 handVelWorld = this.m_obj.GetHandVelWorld();
              if ((double) handVelWorld.magnitude > (double) this.TearOutVelThreshold && (double) this.GetMultiplierForStrikeDir(handVelWorld, this.HighDamageVectors) >= 0.5)
              {
                Vector3 point = Vector3.Lerp(this.EndPoint.position, this.EndPoint.position - this.StabDirection.forward * this.BladeLength * 0.5f, 0.5f);
                this.m_stabbedLink.S.SpawnLargeMustardBurst(point, handVelWorld);
                this.DoTearOutDamage(handVelWorld.magnitude, point, handVelWorld.normalized);
                flag = true;
                UnityEngine.Object.Destroy((UnityEngine.Object) this.m_stabJoint);
                this.m_isJointedToObject = false;
                this.m_stabTargetRB = (Rigidbody) null;
                this.m_stabbedLink = (SosigLink) null;
                this.m_obj.SetCollidersToLayer(this.StabColliders, false, "Default");
                this.m_obj.RootRigidbody.mass = this.m_initialMass;
                this.timeSinceStateChange = 0.0f;
              }
            }
            if (!flag && (double) Vector3.Distance(vector3, this.stabInsidePoint) > (double) this.BladeLength)
            {
              UnityEngine.Object.Destroy((UnityEngine.Object) this.m_stabJoint);
              this.m_isJointedToObject = false;
              this.m_stabTargetRB = (Rigidbody) null;
              this.m_stabbedLink = (SosigLink) null;
              this.m_obj.SetCollidersToLayer(this.StabColliders, false, "Default");
              this.m_obj.RootRigidbody.mass = this.m_initialMass;
              this.timeSinceStateChange = 0.0f;
            }
          }
        }
        if (this.CanStateChange() && this.m_isLodgedToObject && this.m_obj.IsHeld)
        {
          Vector3 handVelWorld = this.m_obj.GetHandVelWorld();
          if ((double) handVelWorld.magnitude > (double) this.DeLodgeVelocityRequirement && (double) Vector3.Angle(handVelWorld, this.m_initialLodgeNormal) < 80.0)
          {
            UnityEngine.Object.Destroy((UnityEngine.Object) this.m_lodgeJoint);
            this.m_isLodgedToObject = false;
            this.m_obj.SetCollidersToLayer(this.LodgeColliders, false, "Default");
            this.m_obj.RootRigidbody.mass = this.m_initialMass;
            this.timeSinceStateChange = 0.0f;
          }
        }
        this.m_lastSweepPointStart = this.m_SweepPointStart;
        this.m_lastSweepPointEnd = this.m_SweepPointEnd;
        this.m_lastHandPoint = this.HandPoint.position;
        this.m_lastEndPoint = this.EndPoint.position;
      }

      public void DeJoint()
      {
        if (this.m_isJointedToObject)
        {
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_stabJoint);
          this.m_isJointedToObject = false;
          this.m_stabTargetRB = (Rigidbody) null;
          this.m_stabbedLink = (SosigLink) null;
          this.m_obj.SetCollidersToLayer(this.StabColliders, false, "Default");
          this.m_obj.RootRigidbody.mass = this.m_initialMass;
          this.timeSinceStateChange = 0.0f;
        }
        if (!this.m_isLodgedToObject)
          return;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_lodgeJoint);
        this.m_isLodgedToObject = false;
        this.m_lodgeTargetRB = (Rigidbody) null;
        this.m_stabbedLink = (SosigLink) null;
        this.m_obj.SetCollidersToLayer(this.StabColliders, false, "Default");
        this.m_obj.RootRigidbody.mass = this.m_initialMass;
        this.timeSinceStateChange = 0.0f;
      }

      private bool GetInside(Collider col) => this.HighDamageColliders.Contains(col);

      private bool GetIsLodgeCollider(Collider col) => this.LodgeColliders.Contains(col);

      private Vector3 GetIntertiaFromPoint(Vector3 point)
      {
        float t = Vector3.Distance(this.HandPoint.position, this.m_obj.GetClosestValidPoint(this.HandPoint.position, this.EndPoint.position, point)) / this.m_pointDistance;
        float num = 1f;
        return Vector3.Lerp(this.handPointVelocity, this.endPointVelocity, t) * num;
      }

      private float GetMultiplierForStrikeDir(Vector3 dir, Transform[] dirsToUse)
      {
        float a = 0.0f;
        for (int index = 0; index < dirsToUse.Length; ++index)
          a = Mathf.Max(a, Vector3.Dot(dir.normalized, dirsToUse[index].forward));
        return a;
      }

      private float GetMultiplierForStrikeDir(Vector3 dir, List<Transform> dirsToUse)
      {
        float a = 0.0f;
        for (int index = 0; index < dirsToUse.Count; ++index)
          a = Mathf.Max(a, Vector3.Dot(dir.normalized, dirsToUse[index].forward));
        return a;
      }

      private float GetMultiplierForStrikeDir(Vector3 dir, Transform dirToUse) => Mathf.Max(0.0f, Vector3.Dot(dir.normalized, dirToUse.forward));

      private void DoTearOutDamage(float mag, Vector3 point, Vector3 dir)
      {
        if (this.m_obj.IsHeld)
          this.m_obj.m_hand.Buzz(this.m_obj.m_hand.Buzzer.Buzz_GunShot);
        Damage dam = new Damage();
        dam.Class = Damage.DamageClass.Melee;
        dam.point = point;
        dam.hitNormal = -dir;
        dam.strikeDir = dir;
        dam.damageSize = 0.02f;
        dam.edgeNormal = this.m_obj.transform.forward;
        Vector3 zero = Vector3.zero;
        Vector3 vector3_1 = Vector3.zero;
        Vector3 vector3_2 = Vector3.zero;
        IFVRDamageable component = this.m_stabbedLink.gameObject.GetComponent<IFVRDamageable>();
        if (component == null)
          return;
        IFVRDamageable fvrDamageable = component;
        bool flag = true;
        Vector3 intertiaFromPoint = this.GetIntertiaFromPoint(point);
        vector3_1 = point;
        vector3_2 = -dir;
        float num1 = Mathf.Clamp(intertiaFromPoint.magnitude, 0.0f, 1f);
        if ((UnityEngine.Object) this.m_obj.AltGrip != (UnityEngine.Object) null && !this.m_obj.IsAltHeld)
          num1 *= 3f;
        float num2 = Mathf.Clamp(num1, 1f, 2f);
        if (flag && (double) num2 > 0.25)
        {
          dam.Dam_Blunt = this.TearDamageBCP.x * num2;
          dam.Dam_Cutting = this.TearDamageBCP.y * num2;
          dam.Dam_Piercing = this.TearDamageBCP.z * num2;
          dam.Dam_TotalKinetic = dam.Dam_Blunt + dam.Dam_Cutting + dam.Dam_Piercing;
          fvrDamageable.Damage(dam);
        }
        this.handPointVelocity = Vector3.zero;
        this.endPointVelocity = Vector3.zero;
      }

      public virtual bool CanStateChange() => (double) this.timeSinceStateChange > 0.25;

      public bool GetIsStabCollider(Collider col) => this.StabColliders.Contains(col);

      protected void DoStabDamage(float mag, Collision col)
      {
        if (this.m_obj.IsHeld)
          this.m_obj.m_hand.Buzz(this.m_obj.m_hand.Buzzer.Buzz_GunShot);
        bool flag1 = false;
        bool flag2 = false;
        IFVRDamageable fvrDamageable = (IFVRDamageable) null;
        Damage dam = new Damage();
        dam.Class = Damage.DamageClass.Melee;
        dam.point = col.contacts[0].point;
        dam.hitNormal = col.contacts[0].normal;
        dam.strikeDir = this.m_obj.RootRigidbody.GetPointVelocity(col.contacts[0].point).normalized;
        dam.damageSize = 0.02f;
        dam.edgeNormal = this.m_obj.transform.forward;
        Vector3 vector3_1 = Vector3.zero;
        Vector3 vector3_2 = Vector3.zero;
        Vector3 vector3_3 = Vector3.zero;
        for (int index = 0; index < col.contacts.Length; ++index)
        {
          IFVRDamageable component = col.contacts[index].otherCollider.transform.gameObject.GetComponent<IFVRDamageable>();
          if (component == null && (UnityEngine.Object) col.contacts[index].otherCollider.attachedRigidbody != (UnityEngine.Object) null)
            component = col.contacts[index].otherCollider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
          if (component != null)
          {
            fvrDamageable = component;
            flag2 = true;
            Vector3 intertiaFromPoint = this.GetIntertiaFromPoint(col.contacts[index].point);
            if ((double) intertiaFromPoint.magnitude > (double) vector3_1.magnitude)
            {
              vector3_1 = intertiaFromPoint;
              vector3_2 = col.contacts[index].point;
              vector3_3 = col.contacts[index].normal;
            }
            if (this.GetIsStabCollider(col.contacts[index].thisCollider))
              flag1 = true;
          }
        }
        float num1 = Mathf.Clamp(vector3_1.magnitude, 0.0f, 1f);
        if ((UnityEngine.Object) this.m_obj.AltGrip != (UnityEngine.Object) null && !this.m_obj.IsAltHeld)
          num1 *= 2f;
        float num2 = Mathf.Clamp(num1, 1f, 2f);
        if (flag2 && (double) num2 > 0.25)
        {
          float multiplierForStrikeDir = this.GetMultiplierForStrikeDir(vector3_1.normalized, this.StabDirection);
          if (flag1 && (double) multiplierForStrikeDir > 0.25)
          {
            dam.Dam_Blunt = this.StabDamageBCP.x * num2;
            dam.Dam_Cutting = this.StabDamageBCP.y * num2;
            dam.Dam_Piercing = this.StabDamageBCP.z * num2;
          }
          else
          {
            dam.Dam_Blunt = this.BaseDamageBCP.x * num2;
            dam.Dam_Cutting = this.BaseDamageBCP.y * num2;
            dam.Dam_Piercing = this.BaseDamageBCP.z * num2;
          }
          dam.Dam_TotalKinetic = dam.Dam_Blunt + dam.Dam_Cutting + dam.Dam_Piercing;
          fvrDamageable.Damage(dam);
        }
        this.handPointVelocity = Vector3.zero;
        this.endPointVelocity = Vector3.zero;
      }

      protected void JointMeObjectForStab(Rigidbody targetRB)
      {
        if (this.m_isJointedToObject)
          return;
        this.m_isJointedToObject = true;
        this.m_initRot = false;
        this.timeSinceStateChange = 0.0f;
        this.m_stabTargetRB = targetRB;
        this.m_stabJoint = this.m_obj.gameObject.AddComponent<FixedJoint>();
        this.m_stabJoint.autoConfigureConnectedAnchor = true;
        this.m_stabJoint.connectedBody = targetRB;
        this.m_stabJoint.connectedBody.transform.TransformPoint(this.m_stabJoint.connectedAnchor);
        this.stabDistantPoint = this.m_stabJoint.connectedAnchor;
        this.stabInsidePoint = this.stabDistantPoint + this.m_stabJoint.connectedBody.transform.InverseTransformDirection(this.StabDirection.forward) * this.BladeLength;
        this.m_stabJoint.autoConfigureConnectedAnchor = false;
        this.m_stabJoint.connectedAnchor = Vector3.Lerp(this.stabDistantPoint, this.stabInsidePoint, 0.5f);
        this.m_obj.SetCollidersToLayer(this.StabColliders, false, "NoCol");
        this.m_initialMass = this.m_obj.RootRigidbody.mass;
        this.m_obj.RootRigidbody.mass = this.MassWhileStabbed;
      }

      private void JointToObjectForLodge(Rigidbody targetRB, Vector3 anchorPoint)
      {
        if (this.m_isLodgedToObject)
          return;
        this.m_isLodgedToObject = true;
        this.timeSinceStateChange = 0.0f;
        this.m_lodgeTargetRB = targetRB;
        this.m_lodgeJoint = targetRB.gameObject.AddComponent<FixedJoint>();
        this.m_lodgeJoint.enableCollision = false;
        this.m_lodgeJoint.connectedBody = this.m_obj.RootRigidbody;
        this.m_obj.SetCollidersToLayer(this.LodgeColliders, false, "NoCol");
        this.m_initialMass = this.m_obj.RootRigidbody.mass;
        this.m_obj.RootRigidbody.mass = this.MassWhileStabbed;
      }

      public void OnCollisionEnter(Collision col)
      {
        bool flag1 = false;
        if ((UnityEngine.Object) col.collider.attachedRigidbody != (UnityEngine.Object) null)
          flag1 = true;
        if (flag1 && this.IgnoreRBs.Contains(col.collider.attachedRigidbody))
          return;
        float magnitude = col.relativeVelocity.magnitude;
        bool flag2 = false;
        bool flag3 = false;
        if (this.CanNewStab && !this.m_isLodgedToObject && (!this.m_isJointedToObject && this.CanStateChange()) && (this.GetIsStabCollider(col.contacts[0].thisCollider) && (double) magnitude > (double) this.StabVelocityRequirement && ((UnityEngine.Object) col.collider.attachedRigidbody != (UnityEngine.Object) null && (double) Vector3.Angle(col.contacts[0].normal, this.StabDirection.forward) > 120.0)))
        {
          Vector3 to = -col.relativeVelocity;
          if (this.m_obj.IsHeld)
            to = this.m_obj.m_hand.Input.VelLinearWorld;
          if ((double) Vector3.Angle(this.StabDirection.forward, to) < (double) this.StabAngularThreshold)
          {
            flag2 = true;
            if ((UnityEngine.Object) col.collider.transform.parent != (UnityEngine.Object) null)
            {
              SosigLink component = col.collider.transform.parent.gameObject.GetComponent<SosigLink>();
              if ((UnityEngine.Object) component != (UnityEngine.Object) null)
              {
                this.DoStabDamage(magnitude, col);
                bool flag4 = false;
                SosigWearable outwear = (SosigWearable) null;
                if (component.HitsWearable(col.contacts[0].point + col.contacts[0].normal, -col.contacts[0].normal, 1.1f, out outwear) && !outwear.IsStabbable && !this.ForceStab)
                  flag4 = true;
                if (!component.S.CanBeStabbed && !this.ForceStab)
                  flag4 = true;
                if (!flag4)
                {
                  this.m_stabbedLink = component;
                  this.JointMeObjectForStab(col.collider.attachedRigidbody);
                }
              }
            }
          }
        }
        if (flag2 || (double) magnitude <= 1.0 || (double) this.m_timeSinceLastDamageDone <= 0.200000002980232)
          return;
        if (this.m_obj.IsHeld)
          this.m_obj.m_hand.Buzz(this.m_obj.m_hand.Buzzer.Buzz_GunShot);
        bool flag5 = false;
        IFVRDamageable fvrDamageable = (IFVRDamageable) null;
        Damage dam = new Damage();
        dam.Class = Damage.DamageClass.Melee;
        dam.point = col.contacts[0].point;
        dam.hitNormal = col.contacts[0].normal;
        dam.strikeDir = this.m_obj.RootRigidbody.GetPointVelocity(col.contacts[0].point).normalized;
        dam.damageSize = 0.02f;
        dam.Source_IFF = GM.CurrentPlayerBody.GetPlayerIFF();
        dam.edgeNormal = this.m_obj.transform.forward;
        Vector3 vector3_1 = Vector3.zero;
        Vector3 vector3_2 = Vector3.zero;
        Vector3 vector3_3 = Vector3.zero;
        for (int index = 0; index < col.contacts.Length; ++index)
        {
          IFVRDamageable component = col.contacts[index].otherCollider.transform.gameObject.GetComponent<IFVRDamageable>();
          if (component == null && (UnityEngine.Object) col.contacts[index].otherCollider.attachedRigidbody != (UnityEngine.Object) null)
            component = col.contacts[index].otherCollider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
          if (component != null)
          {
            fvrDamageable = component;
            flag5 = true;
            Vector3 intertiaFromPoint = this.GetIntertiaFromPoint(col.contacts[index].point);
            if ((double) intertiaFromPoint.magnitude > (double) vector3_1.magnitude)
            {
              vector3_1 = intertiaFromPoint;
              vector3_2 = col.contacts[index].point;
              vector3_3 = col.contacts[index].normal;
            }
            if (this.GetInside(col.contacts[index].thisCollider))
              flag3 = true;
          }
        }
        float num = Mathf.Clamp(vector3_1.magnitude, 0.0f, 1f);
        if ((UnityEngine.Object) this.m_obj.AltGrip != (UnityEngine.Object) null && !this.m_obj.IsAltHeld)
          num *= 2f;
        if (flag5 && (double) num > 0.25)
        {
          float multiplierForStrikeDir = this.GetMultiplierForStrikeDir(vector3_1.normalized, this.HighDamageVectors);
          if (flag3 && (double) multiplierForStrikeDir > 0.25)
          {
            dam.Dam_Blunt = this.HighDamageBCP.x * num;
            dam.Dam_Cutting = this.HighDamageBCP.y * num;
            dam.Dam_Piercing = this.HighDamageBCP.z * num;
          }
          else
          {
            dam.Dam_Blunt = this.BaseDamageBCP.x * num;
            dam.Dam_Cutting = this.BaseDamageBCP.y * num;
            dam.Dam_Piercing = this.BaseDamageBCP.z * num;
          }
          dam.Dam_TotalKinetic = dam.Dam_Blunt + dam.Dam_Cutting + dam.Dam_Piercing;
          fvrDamageable.Damage(dam);
        }
        if (this.CanNewLodge && !this.m_isLodgedToObject && (!this.m_isLodgedToObject && this.CanStateChange()) && ((double) this.m_obj.RootRigidbody.GetPointVelocity(col.contacts[0].point).magnitude > (double) this.LodgeVelocityRequirement && this.GetIsLodgeCollider(col.contacts[0].thisCollider) && (UnityEngine.Object) col.collider.attachedRigidbody != (UnityEngine.Object) null))
        {
          Vector3 dir = -col.relativeVelocity;
          if (this.m_obj.IsHeld)
            dir = this.m_obj.m_hand.Input.VelLinearWorld;
          if ((double) this.GetMultiplierForStrikeDir(dir, this.LodgeDirections) > 0.600000023841858)
          {
            SosigLink component = col.collider.attachedRigidbody.gameObject.GetComponent<SosigLink>();
            if ((UnityEngine.Object) component != (UnityEngine.Object) null && !component.GetHasJustBeenSevered())
            {
              bool flag4 = false;
              SosigWearable outwear = (SosigWearable) null;
              if (component.HitsWearable(col.contacts[0].point + col.contacts[0].normal, -col.contacts[0].normal, 1.1f, out outwear) && !outwear.IsLodgeable)
                flag4 = true;
              if (!flag4)
              {
                this.m_initialLodgeNormal = col.contacts[0].normal;
                this.JointToObjectForLodge(col.collider.attachedRigidbody, col.contacts[0].point);
              }
            }
          }
        }
        this.handPointVelocity = Vector3.zero;
        this.endPointVelocity = Vector3.zero;
      }
    }
  }
}
