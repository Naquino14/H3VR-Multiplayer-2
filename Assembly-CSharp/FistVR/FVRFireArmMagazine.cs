// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFireArmMagazine
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FVRFireArmMagazine : FVRPhysicalObject
  {
    [Header("Magazine Config")]
    public bool IsIntegrated;
    public bool DoesFollowerStopBolt = true;
    public bool IsBeltBox;
    public bool UsesVizInterp;
    public Transform Viz;
    private Vector3 m_vizLerpStartPos;
    private Quaternion m_vizLerpStartRot;
    private float m_vizLerp;
    private bool m_isVizLerping;
    private bool m_isVizLerpInward;
    private float m_vizLerpSpeedMultiplier_Eject = 18f;
    private float m_vizLerpSpeedMultiplier_Insert = 18f;
    private Transform m_vizLerpReferenceTransform;
    [SearchableEnum]
    public FireArmMagazineType MagazineType;
    [SearchableEnum]
    public FireArmRoundType RoundType;
    public float EjectionSpeed = 1f;
    public Transform RoundEjectionPos;
    public bool IsNonPhysForLoad;
    public FVRFireArmMagazine.MagazineState State;
    public bool DoesDisplayXOscillate;
    public GameObject[] DisplayBullets;
    public MeshFilter[] DisplayMeshFilters;
    public Renderer[] DisplayRenderers;
    private Vector3[] m_DisplayStartPositions;
    [HideInInspector]
    public FVRLoadedRound[] LoadedRounds;
    public FVRFireArmMagazine.FVRMagazineLoadingPattern DefaultLoadingPattern;
    public int m_numRounds = 30;
    public int m_capacity = 30;
    public FVRFireArm FireArm;
    public bool IsDropInLoadable;
    private float m_timeSinceRoundInserted;
    public Transform AlternatePoseOverride;
    private Vector3 m_originalPoseOverrideLocalPosition = Vector3.zero;
    private Quaternion m_originalPoseOverrideLocalRotation = Quaternion.identity;
    public bool IsInfinite;
    public bool CanManuallyEjectRounds = true;
    public bool IsExtractable = true;
    public bool AutoEjectsOnEmpty;
    public FVRFireArmBeltGrabTrigger BeltGrabTrigger;
    [Header("Audio Config")]
    public FVRFirearmAudioSet Profile;
    public bool UsesOverrideInOut;
    public FVRFirearmMagazineAudioSet ProfileOverride;
    public bool SwapsImpactTypeOnEmpty;
    public ImpactType ImpactEmptyType = ImpactType.Generic;
    private bool m_setToFullSounds = true;
    private ImpactType m_originalImpactType;
    [Header("Mag Rotation Bits")]
    public bool UsesRotatingBit;
    public Transform RotatingBit;
    public FVRPhysicalObject.Axis RotAxis = FVRPhysicalObject.Axis.Y;
    public float Rot_Full;
    public float Rot_Empty;
    [Header("FuelOverride")]
    public float FuelAmountLeft;
    [Header("BeltBoxOverride")]
    public bool HidesAllDisplayBulletsWhenGrabbed = true;
    public int IndexBeforeWhichIsHiddenWhenGrabbed;
    public bool CanBeTornOut;

    public float TimeSinceRoundInserted => this.m_timeSinceRoundInserted;

    public override bool IsInteractable() => this.State == FVRFireArmMagazine.MagazineState.Free;

    public void ReloadMagWithType(FireArmRoundClass rClass)
    {
      this.m_numRounds = 0;
      for (int index = 0; index < this.m_capacity; ++index)
        this.AddRound(rClass, false, false);
      this.UpdateBulletDisplay();
    }

    public void ReloadMagWithList(List<FireArmRoundClass> list)
    {
      this.m_numRounds = 0;
      int num = Mathf.Min(list.Count, this.m_capacity);
      for (int index = 0; index < num; ++index)
        this.AddRound(list[index], false, false);
      this.UpdateBulletDisplay();
    }

    public void ReloadMagWithTypeUpToPercentage(FireArmRoundClass rClass, float percentage)
    {
      int num = Mathf.Clamp((int) ((double) this.m_capacity * (double) percentage), 1, this.m_capacity);
      this.m_numRounds = 0;
      for (int index = 0; index < num; ++index)
        this.AddRound(rClass, false, false);
      this.UpdateBulletDisplay();
    }

    public override GameObject DuplicateFromSpawnLock(FVRViveHand hand)
    {
      GameObject gameObject = base.DuplicateFromSpawnLock(hand);
      FVRFireArmMagazine component = gameObject.GetComponent<FVRFireArmMagazine>();
      for (int index = 0; index < Mathf.Min(this.LoadedRounds.Length, component.LoadedRounds.Length); ++index)
      {
        if (this.LoadedRounds[index] != null && (UnityEngine.Object) this.LoadedRounds[index].LR_Mesh != (UnityEngine.Object) null)
        {
          component.LoadedRounds[index].LR_Class = this.LoadedRounds[index].LR_Class;
          component.LoadedRounds[index].LR_Mesh = this.LoadedRounds[index].LR_Mesh;
          component.LoadedRounds[index].LR_Material = this.LoadedRounds[index].LR_Material;
          component.LoadedRounds[index].LR_ObjectWrapper = this.LoadedRounds[index].LR_ObjectWrapper;
        }
      }
      component.m_numRounds = this.m_numRounds;
      component.UpdateBulletDisplay();
      return gameObject;
    }

    protected override void Awake()
    {
      base.Awake();
      this.m_DisplayStartPositions = new Vector3[this.DisplayBullets.Length];
      for (int index = 0; index < this.DisplayBullets.Length; ++index)
      {
        if ((UnityEngine.Object) this.DisplayBullets[index] != (UnityEngine.Object) null)
          this.m_DisplayStartPositions[index] = this.DisplayBullets[index].transform.localPosition;
      }
      this.LoadedRounds = new FVRLoadedRound[this.m_capacity];
      this.m_numRounds = 0;
      for (int index = 0; index < Mathf.Min(this.DefaultLoadingPattern.Classes.Length, this.m_capacity); ++index)
        this.AddRound(this.DefaultLoadingPattern.Classes[index], false, false);
      this.UpdateBulletDisplay();
      if (!this.SwapsImpactTypeOnEmpty || !this.HasImpactController)
        return;
      this.m_originalImpactType = this.AudioImpactController.ImpactType;
      this.m_setToFullSounds = true;
    }

    public Vector3 GetAmmoLocalEulers()
    {
      float num = Mathf.Lerp(this.Rot_Empty, this.Rot_Full, (float) this.m_numRounds / (float) this.m_capacity);
      switch (this.RotAxis)
      {
        case FVRPhysicalObject.Axis.X:
          return new Vector3(num, 0.0f, 0.0f);
        case FVRPhysicalObject.Axis.Y:
          return new Vector3(0.0f, num, 0.0f);
        case FVRPhysicalObject.Axis.Z:
          return new Vector3(0.0f, 0.0f, num);
        default:
          return new Vector3(0.0f, 0.0f, 0.0f);
      }
    }

    public bool HasARound() => this.m_numRounds > 0 && this.IsExtractable;

    public bool IsFull() => this.m_numRounds >= this.m_capacity;

    public void ForceEmpty() => this.m_numRounds = 0;

    public void ForceFull() => this.m_numRounds = this.m_capacity;

    public bool HasFuel() => (double) this.FuelAmountLeft > 0.0;

    public void DrainFuel(float f) => this.FuelAmountLeft -= f;

    public override void EndInteraction(FVRViveHand hand)
    {
      if (this.IsNonPhysForLoad)
        this.SetAllCollidersToLayer(false, "Default");
      base.EndInteraction(hand);
    }

    public void AddRound(FireArmRoundClass rClass, bool makeSound, bool updateDisplay)
    {
      if (this.m_numRounds < this.m_capacity)
      {
        this.m_timeSinceRoundInserted = 0.0f;
        this.LoadedRounds[this.m_numRounds] = new FVRLoadedRound()
        {
          LR_Class = rClass,
          LR_Mesh = AM.GetRoundMesh(this.RoundType, rClass),
          LR_Material = AM.GetRoundMaterial(this.RoundType, rClass),
          LR_ObjectWrapper = AM.GetRoundSelfPrefab(this.RoundType, rClass)
        };
        ++this.m_numRounds;
        if (makeSound)
        {
          if ((UnityEngine.Object) this.FireArm != (UnityEngine.Object) null)
            this.FireArm.PlayAudioEvent(FirearmAudioEventType.MagazineInsertRound);
          else if (this.UsesOverrideInOut)
            SM.PlayGenericSound(this.ProfileOverride.MagazineInsertRound, this.transform.position);
          else
            SM.PlayGenericSound(this.Profile.MagazineInsertRound, this.transform.position);
        }
      }
      if (!updateDisplay)
        return;
      this.UpdateBulletDisplay();
    }

    public void AddRound(FVRFireArmRound round, bool makeSound, bool updateDisplay)
    {
      if (this.m_numRounds < this.m_capacity)
      {
        this.m_timeSinceRoundInserted = 0.0f;
        this.LoadedRounds[this.m_numRounds] = new FVRLoadedRound()
        {
          LR_Class = round.RoundClass,
          LR_Mesh = AM.GetRoundMesh(round.RoundType, round.RoundClass),
          LR_Material = AM.GetRoundMaterial(round.RoundType, round.RoundClass),
          LR_ObjectWrapper = AM.GetRoundSelfPrefab(round.RoundType, round.RoundClass)
        };
        ++this.m_numRounds;
        if (makeSound)
        {
          if ((UnityEngine.Object) this.FireArm != (UnityEngine.Object) null)
            this.FireArm.PlayAudioEvent(FirearmAudioEventType.MagazineInsertRound);
          else
            SM.PlayGenericSound(this.Profile.MagazineInsertRound, this.transform.position);
        }
      }
      if (!updateDisplay)
        return;
      this.UpdateBulletDisplay();
    }

    public void RemoveRound()
    {
      if (this.IsInfinite && GM.CurrentSceneSettings.AllowsInfiniteAmmoMags || (GM.CurrentSceneSettings.IsAmmoInfinite || GM.CurrentPlayerBody.IsInfiniteAmmo))
        return;
      if (GM.CurrentPlayerBody.IsAmmoDrain)
        this.m_numRounds = 0;
      if (this.m_numRounds > 0)
      {
        this.LoadedRounds[this.m_numRounds - 1] = (FVRLoadedRound) null;
        --this.m_numRounds;
      }
      this.UpdateBulletDisplay();
    }

    public GameObject RemoveRound(bool b)
    {
      GameObject gameObject = this.LoadedRounds[this.m_numRounds - 1].LR_ObjectWrapper.GetGameObject();
      if ((!this.IsInfinite || !GM.CurrentSceneSettings.AllowsInfiniteAmmoMags) && (!GM.CurrentSceneSettings.IsAmmoInfinite && !GM.CurrentPlayerBody.IsInfiniteAmmo))
      {
        if (GM.CurrentPlayerBody.IsAmmoDrain)
        {
          this.m_numRounds = 0;
        }
        else
        {
          if (this.m_numRounds > 0)
          {
            this.LoadedRounds[this.m_numRounds - 1] = (FVRLoadedRound) null;
            --this.m_numRounds;
          }
          this.UpdateBulletDisplay();
        }
      }
      return gameObject;
    }

    public FVRLoadedRound RemoveRound(int i)
    {
      FVRLoadedRound loadedRound = this.LoadedRounds[this.m_numRounds - 1];
      if ((!this.IsInfinite || !GM.CurrentSceneSettings.AllowsInfiniteAmmoMags) && (!GM.CurrentSceneSettings.IsAmmoInfinite && !GM.CurrentPlayerBody.IsInfiniteAmmo))
      {
        if (GM.CurrentPlayerBody.IsAmmoDrain)
        {
          this.m_numRounds = 0;
        }
        else
        {
          if (this.m_numRounds > 0)
          {
            this.LoadedRounds[this.m_numRounds - 1] = (FVRLoadedRound) null;
            --this.m_numRounds;
          }
          this.UpdateBulletDisplay();
        }
      }
      return loadedRound;
    }

    public void UpdateBulletDisplay()
    {
      if ((UnityEngine.Object) this.FireArm != (UnityEngine.Object) null && this.FireArm.UsesBelts)
      {
        bool flag = false;
        if (this.FireArm.BeltDD.isBeltGrabbed())
          flag = true;
        if (this.HidesAllDisplayBulletsWhenGrabbed && (this.FireArm.ConnectedToBox || flag))
        {
          for (int index = 0; index < this.DisplayBullets.Length; ++index)
            this.DisplayBullets[index].SetActive(false);
          return;
        }
      }
      int index1 = this.m_numRounds - 1;
      for (int index2 = 0; index2 < this.DisplayBullets.Length; ++index2)
      {
        if ((UnityEngine.Object) this.DisplayBullets[index2] != (UnityEngine.Object) null)
        {
          bool flag = false;
          if (this.HidesAllDisplayBulletsWhenGrabbed && (UnityEngine.Object) this.FireArm != (UnityEngine.Object) null && (this.FireArm.UsesBelts && this.FireArm.BeltDD.isBeltGrabbed()))
            flag = true;
          if (index2 >= this.m_numRounds || index1 < 0 || flag && index2 < this.IndexBeforeWhichIsHiddenWhenGrabbed)
          {
            this.DisplayBullets[index2].SetActive(false);
          }
          else
          {
            this.DisplayBullets[index2].SetActive(true);
            this.DisplayMeshFilters[index2].mesh = this.LoadedRounds[index1].LR_Mesh;
            this.DisplayRenderers[index2].material = this.LoadedRounds[index1].LR_Material;
          }
        }
        --index1;
      }
      if (this.UsesRotatingBit)
        this.RotatingBit.localEulerAngles = this.GetAmmoLocalEulers();
      if (this.DoesDisplayXOscillate)
      {
        if (this.m_numRounds % 2 == 1)
        {
          for (int index2 = 0; index2 < this.DisplayBullets.Length; ++index2)
          {
            if ((UnityEngine.Object) this.DisplayBullets[index2] != (UnityEngine.Object) null)
              this.DisplayBullets[index2].transform.localPosition = new Vector3(this.m_DisplayStartPositions[index2].x * -1f, this.m_DisplayStartPositions[index2].y, this.m_DisplayStartPositions[index2].z);
          }
        }
        else
        {
          for (int index2 = 0; index2 < this.DisplayBullets.Length; ++index2)
          {
            if ((UnityEngine.Object) this.DisplayBullets[index2] != (UnityEngine.Object) null)
              this.DisplayBullets[index2].transform.localPosition = this.m_DisplayStartPositions[index2];
          }
        }
      }
      if (!this.SwapsImpactTypeOnEmpty)
        return;
      if (this.m_setToFullSounds)
      {
        if (this.m_numRounds > 0)
          return;
        this.m_setToFullSounds = false;
        this.AudioImpactController.ImpactType = this.ImpactEmptyType;
      }
      else
      {
        if (this.m_numRounds <= 0)
          return;
        this.m_setToFullSounds = true;
        this.AudioImpactController.ImpactType = this.m_originalImpactType;
      }
    }

    public void Release()
    {
      this.State = FVRFireArmMagazine.MagazineState.Free;
      this.SetParentage((Transform) null);
      if (this.UsesVizInterp)
      {
        this.m_vizLerpStartPos = this.Viz.transform.position;
        this.m_vizLerp = 0.0f;
        this.m_isVizLerpInward = false;
        this.m_isVizLerping = true;
      }
      if ((UnityEngine.Object) this.FireArm.MagazineEjectPos != (UnityEngine.Object) null)
        this.transform.position = this.FireArm.GetMagEjectPos(this.IsBeltBox).position;
      else
        this.transform.position = this.FireArm.GetMagMountPos(this.IsBeltBox).position;
      if (this.UsesVizInterp)
      {
        this.Viz.position = this.m_vizLerpStartPos;
        this.m_vizLerpReferenceTransform = this.FireArm.GetMagMountPos(this.IsBeltBox);
      }
      this.RecoverRigidbody();
      this.RootRigidbody.isKinematic = false;
      this.RootRigidbody.velocity = this.FireArm.RootRigidbody.velocity - this.transform.up * this.EjectionSpeed;
      this.RootRigidbody.angularVelocity = this.FireArm.RootRigidbody.angularVelocity;
      if ((UnityEngine.Object) this.FireArm.m_hand != (UnityEngine.Object) null)
      {
        bool flag = false;
        FVRViveHand otherHand = this.FireArm.m_hand.OtherHand;
        if ((UnityEngine.Object) otherHand.CurrentInteractable == (UnityEngine.Object) null && otherHand.Input.IsGrabbing)
          flag = true;
        if (flag)
        {
          Vector3 position = otherHand.transform.position;
          if (GM.Options.ControlOptions.UseInvertedHandgunMagPose)
            position = otherHand.GetMagPose().position;
          Vector3 to = position - this.FireArm.GetMagMountPos(this.IsBeltBox).position;
          if ((double) Vector3.Distance(this.transform.position, position) < 0.200000002980232 && (double) Vector3.Angle(this.transform.up, to) > 90.0)
          {
            otherHand.ForceSetInteractable((FVRInteractiveObject) this);
            this.BeginInteraction(otherHand);
          }
        }
      }
      this.FireArm = (FVRFireArm) null;
      this.SetAllCollidersToLayer(false, "Default");
    }

    public void Load(FVRFireArm fireArm)
    {
      this.State = FVRFireArmMagazine.MagazineState.Locked;
      this.FireArm = fireArm;
      this.FireArm.LoadMag(this);
      this.IsHeld = false;
      this.ForceBreakInteraction();
      if (this.UsesVizInterp)
      {
        this.m_vizLerpStartPos = this.Viz.transform.position;
        this.m_vizLerpStartRot = this.Viz.transform.rotation;
        this.m_vizLerp = 0.0f;
        this.m_isVizLerpInward = true;
        this.m_isVizLerping = true;
      }
      this.SetParentage(this.FireArm.GetMagMountingTransform());
      this.transform.rotation = this.FireArm.GetMagMountPos(this.IsBeltBox).rotation;
      this.transform.position = this.FireArm.GetMagMountPos(this.IsBeltBox).position;
      if (this.UsesVizInterp)
      {
        this.Viz.position = this.m_vizLerpStartPos;
        this.Viz.rotation = this.m_vizLerpStartRot;
      }
      this.StoreAndDestroyRigidbody();
      if ((UnityEngine.Object) this.FireArm.QuickbeltSlot != (UnityEngine.Object) null)
        this.SetAllCollidersToLayer(false, "NoCol");
      else
        this.SetAllCollidersToLayer(false, "Default");
      if (!((UnityEngine.Object) fireArm.ObjectWrapper != (UnityEngine.Object) null))
        return;
      GM.CurrentSceneSettings.OnFireArmReloaded(fireArm.ObjectWrapper);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (!this.CanManuallyEjectRounds || !((UnityEngine.Object) this.RoundEjectionPos != (UnityEngine.Object) null) || !this.HasARound())
        return;
      bool flag = false;
      if (hand.IsInStreamlinedMode && hand.Input.BYButtonDown)
        flag = true;
      else if (!hand.IsInStreamlinedMode && hand.Input.TouchpadDown && (double) Vector2.Angle(hand.Input.TouchpadAxes, Vector2.up) < 45.0)
        flag = true;
      if (!flag)
        return;
      if ((UnityEngine.Object) this.FireArm != (UnityEngine.Object) null)
        this.FireArm.PlayAudioEvent(FirearmAudioEventType.MagazineEjectRound);
      else
        SM.PlayGenericSound(this.Profile.MagazineEjectRound, this.transform.position);
      if ((UnityEngine.Object) hand.OtherHand.CurrentInteractable == (UnityEngine.Object) null && hand.OtherHand.Input.IsGrabbing && (double) Vector3.Distance(this.RoundEjectionPos.position, hand.OtherHand.Input.Pos) < 0.150000005960464)
      {
        FVRFireArmRound component = UnityEngine.Object.Instantiate<GameObject>(this.RemoveRound(false), this.RoundEjectionPos.position, this.RoundEjectionPos.rotation).GetComponent<FVRFireArmRound>();
        component.SetIFF(GM.CurrentPlayerBody.GetPlayerIFF());
        hand.OtherHand.ForceSetInteractable((FVRInteractiveObject) component);
        component.BeginInteraction(hand.OtherHand);
      }
      else if (hand.OtherHand.CurrentInteractable is FVRFireArmRound && ((FVRFireArmRound) hand.OtherHand.CurrentInteractable).RoundType == this.RoundType && (((FVRFireArmRound) hand.OtherHand.CurrentInteractable).ProxyRounds.Count < ((FVRFireArmRound) hand.OtherHand.CurrentInteractable).MaxPalmedAmount && (double) Vector3.Distance(hand.Input.Pos, hand.OtherHand.Input.Pos) < 0.150000005960464))
      {
        FireArmRoundClass lrClass = this.LoadedRounds[this.m_numRounds - 1].LR_Class;
        FVRObject lrObjectWrapper = this.LoadedRounds[this.m_numRounds - 1].LR_ObjectWrapper;
        ((FVRFireArmRound) hand.OtherHand.CurrentInteractable).AddProxy(lrClass, lrObjectWrapper);
        ((FVRFireArmRound) hand.OtherHand.CurrentInteractable).UpdateProxyDisplay();
        this.RemoveRound();
      }
      else if ((UnityEngine.Object) hand.CurrentHoveredQuickbeltSlotDirty != (UnityEngine.Object) null && (UnityEngine.Object) hand.CurrentHoveredQuickbeltSlotDirty.HeldObject == (UnityEngine.Object) null)
      {
        FVRFireArmRound component = UnityEngine.Object.Instantiate<GameObject>(this.RemoveRound(false), this.RoundEjectionPos.position, this.RoundEjectionPos.rotation).GetComponent<FVRFireArmRound>();
        component.SetIFF(GM.CurrentPlayerBody.GetPlayerIFF());
        component.SetQuickBeltSlot(hand.CurrentHoveredQuickbeltSlotDirty);
      }
      else if ((UnityEngine.Object) hand.CurrentHoveredQuickbeltSlotDirty != (UnityEngine.Object) null && hand.CurrentHoveredQuickbeltSlotDirty.HeldObject is FVRFireArmRound && (((FVRFireArmRound) hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).RoundType == this.RoundType && ((FVRFireArmRound) hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).ProxyRounds.Count < ((FVRFireArmRound) hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).MaxPalmedAmount))
      {
        FireArmRoundClass lrClass = this.LoadedRounds[this.m_numRounds - 1].LR_Class;
        FVRObject lrObjectWrapper = this.LoadedRounds[this.m_numRounds - 1].LR_ObjectWrapper;
        ((FVRFireArmRound) hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).AddProxy(lrClass, lrObjectWrapper);
        ((FVRFireArmRound) hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).UpdateProxyDisplay();
        this.RemoveRound();
      }
      else
      {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.RemoveRound(false), this.RoundEjectionPos.position, this.RoundEjectionPos.rotation);
        gameObject.GetComponent<FVRFireArmRound>().SetIFF(GM.CurrentPlayerBody.GetPlayerIFF());
        gameObject.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * 0.5f);
      }
    }

    protected override Vector3 GetGrabPos() => base.GetGrabPos();

    protected override Quaternion GetGrabRot() => base.GetGrabRot();

    protected override Vector3 GetPosTarget() => GM.Options.ControlOptions.UseInvertedHandgunMagPose && (UnityEngine.Object) this.QuickbeltSlot == (UnityEngine.Object) null && this.IsHeld ? this.m_hand.GetMagPose().position : base.GetPosTarget();

    protected override Quaternion GetRotTarget() => GM.Options.ControlOptions.UseInvertedHandgunMagPose && (UnityEngine.Object) this.QuickbeltSlot == (UnityEngine.Object) null && this.IsHeld ? this.m_hand.GetMagPose().rotation : base.GetRotTarget();

    protected override void FVRFixedUpdate()
    {
      if ((double) this.m_timeSinceRoundInserted < 5.0)
        this.m_timeSinceRoundInserted += Time.deltaTime;
      base.FVRFixedUpdate();
      if (this.IsHeld && GM.Options.ControlOptions.UseEasyMagLoading && ((UnityEngine.Object) this.m_hand.OtherHand.CurrentInteractable != (UnityEngine.Object) null && this.m_hand.OtherHand.CurrentInteractable is FVRFireArm))
      {
        FVRFireArm currentInteractable = this.m_hand.OtherHand.CurrentInteractable as FVRFireArm;
        if (currentInteractable.MagazineType == this.MagazineType && (UnityEngine.Object) currentInteractable.GetMagMountPos(this.IsBeltBox) != (UnityEngine.Object) null)
        {
          if ((double) Vector3.Distance(this.RoundEjectionPos.position, currentInteractable.GetMagMountPos(this.IsBeltBox).position) <= 0.150000005960464)
          {
            this.SetAllCollidersToLayer(false, "NoCol");
            this.IsNonPhysForLoad = true;
          }
          else
          {
            this.SetAllCollidersToLayer(false, "Default");
            this.IsNonPhysForLoad = false;
          }
        }
      }
      if (!this.UsesVizInterp || !this.m_isVizLerping)
        return;
      if (!this.m_isVizLerpInward)
      {
        this.m_vizLerp += Time.deltaTime * this.m_vizLerpSpeedMultiplier_Eject;
        if ((double) this.m_vizLerp >= 1.0)
        {
          this.m_vizLerp = 1f;
          this.m_isVizLerping = false;
        }
        this.Viz.position = Vector3.Lerp(this.m_vizLerpStartPos, this.transform.position, this.m_vizLerp);
        if ((UnityEngine.Object) this.m_vizLerpReferenceTransform != (UnityEngine.Object) null)
          this.Viz.rotation = Quaternion.Slerp(this.m_vizLerpReferenceTransform.rotation, this.transform.rotation, this.m_vizLerp * this.m_vizLerp);
        else
          this.Viz.rotation = Quaternion.Slerp(this.Viz.rotation, this.transform.rotation, this.m_vizLerp);
      }
      else
      {
        this.m_vizLerp += Time.deltaTime * this.m_vizLerpSpeedMultiplier_Insert;
        if ((double) this.m_vizLerp >= 1.0)
        {
          this.m_vizLerp = 1f;
          this.m_isVizLerping = false;
        }
        this.Viz.position = Vector3.Lerp(this.m_vizLerpStartPos, this.transform.position, this.m_vizLerp);
        this.Viz.rotation = Quaternion.Slerp(this.m_vizLerpStartRot, this.transform.rotation, Mathf.Sqrt(this.m_vizLerp));
      }
    }

    public enum MagazineState
    {
      Free,
      Locked,
    }

    [Serializable]
    public class FVRMagazineLoadingPattern
    {
      public FireArmRoundClass[] Classes;
    }
  }
}
