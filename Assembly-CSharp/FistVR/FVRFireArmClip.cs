// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFireArmClip
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FVRFireArmClip : FVRPhysicalObject
  {
    [Header("Clip Params")]
    public FireArmClipType ClipType;
    public FireArmRoundType RoundType;
    public FVRFireArm FireArm;
    public FVRFireArmClipInterface Interface;
    public bool CanManuallyEjectRounds = true;
    public Transform RoundEjectionPos;
    public float EjectionSpeed = 1f;
    public FVRFireArmClip.ClipState State;
    public GameObject[] DisplayBullets;
    public MeshFilter[] DisplayMeshFilters;
    public Renderer[] DisplayRenderers;
    [HideInInspector]
    public FVRFireArmClip.FVRLoadedRound[] LoadedRounds;
    public FVRFireArmClip.FVRClipLoadingPattern DefaultLoadingPattern;
    public int m_numRounds = 10;
    public int m_capacity = 10;
    public bool IsDropInLoadable;
    private float m_timeSinceRoundInserted;
    [Header("Audio Config")]
    public AudioEvent LoadFromClipToMag;
    public AudioEvent InsertOntoClip;
    public AudioEvent EjectFromClip;
    public AudioEvent AffixClip;
    public AudioEvent RemoveClip;
    public bool IsInfinite;
    public bool IsExtractable = true;

    public float TimeSinceRoundInserted => this.m_timeSinceRoundInserted;

    protected override void Awake()
    {
      base.Awake();
      this.Interface.gameObject.SetActive(false);
      this.LoadedRounds = new FVRFireArmClip.FVRLoadedRound[this.m_capacity];
      this.m_numRounds = 0;
      for (int index = 0; index < Mathf.Min(this.DefaultLoadingPattern.Classes.Length, this.m_capacity); ++index)
        this.AddRound(this.DefaultLoadingPattern.Classes[index], false, false);
      this.UpdateBulletDisplay();
    }

    public override GameObject DuplicateFromSpawnLock(FVRViveHand hand)
    {
      GameObject gameObject = base.DuplicateFromSpawnLock(hand);
      FVRFireArmClip component = gameObject.GetComponent<FVRFireArmClip>();
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

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (this.State != FVRFireArmClip.ClipState.Free || !this.CanManuallyEjectRounds || (!((UnityEngine.Object) this.RoundEjectionPos != (UnityEngine.Object) null) || !this.HasARound()))
        return;
      bool flag = false;
      if (hand.IsInStreamlinedMode && hand.Input.BYButtonDown)
        flag = true;
      else if (!hand.IsInStreamlinedMode && hand.Input.TouchpadDown && (double) Vector2.Angle(hand.Input.TouchpadAxes, Vector2.up) < 45.0)
        flag = true;
      if (!flag)
        return;
      SM.PlayGenericSound(this.EjectFromClip, this.transform.position);
      if ((UnityEngine.Object) hand.OtherHand.CurrentInteractable == (UnityEngine.Object) null && hand.OtherHand.Input.IsGrabbing && (double) Vector3.Distance(this.RoundEjectionPos.position, hand.OtherHand.Input.Pos) < 0.150000005960464)
      {
        FVRFireArmRound component = UnityEngine.Object.Instantiate<GameObject>(this.RemoveRound(false), this.RoundEjectionPos.position, this.RoundEjectionPos.rotation).GetComponent<FVRFireArmRound>();
        component.GetComponent<FVRFireArmRound>().SetIFF(GM.CurrentPlayerBody.GetPlayerIFF());
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
        component.GetComponent<FVRFireArmRound>().SetIFF(GM.CurrentPlayerBody.GetPlayerIFF());
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

    public void LoadOneRoundFromClipToMag()
    {
      if ((UnityEngine.Object) this.FireArm == (UnityEngine.Object) null || (UnityEngine.Object) this.FireArm.Magazine == (UnityEngine.Object) null || (this.FireArm.Magazine.IsFull() || !this.HasARound()))
        return;
      FireArmRoundClass rClass = this.RemoveRoundReturnClass();
      SM.PlayGenericSound(this.LoadFromClipToMag, this.transform.position);
      this.FireArm.Magazine.AddRound(rClass, false, true);
    }

    public bool HasARound() => this.m_numRounds > 0 && this.IsExtractable;

    public bool IsFull() => this.m_numRounds >= this.m_capacity;

    public void AddRound(FireArmRoundClass rClass, bool makeSound, bool updateDisplay)
    {
      if (this.m_numRounds < this.m_capacity)
      {
        this.m_timeSinceRoundInserted = 0.0f;
        this.LoadedRounds[this.m_numRounds] = new FVRFireArmClip.FVRLoadedRound()
        {
          LR_Class = rClass,
          LR_Mesh = AM.GetRoundMesh(this.RoundType, rClass),
          LR_Material = AM.GetRoundMaterial(this.RoundType, rClass),
          LR_ObjectWrapper = AM.GetRoundSelfPrefab(this.RoundType, rClass)
        };
        ++this.m_numRounds;
        if (makeSound)
          SM.PlayGenericSound(this.InsertOntoClip, this.transform.position);
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
        this.LoadedRounds[this.m_numRounds] = new FVRFireArmClip.FVRLoadedRound()
        {
          LR_Class = round.RoundClass,
          LR_Mesh = AM.GetRoundMesh(round.RoundType, round.RoundClass),
          LR_Material = AM.GetRoundMaterial(round.RoundType, round.RoundClass),
          LR_ObjectWrapper = AM.GetRoundSelfPrefab(round.RoundType, round.RoundClass)
        };
        ++this.m_numRounds;
        if (makeSound)
          SM.PlayGenericSound(this.InsertOntoClip, this.transform.position);
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
        this.LoadedRounds[this.m_numRounds - 1] = (FVRFireArmClip.FVRLoadedRound) null;
        --this.m_numRounds;
      }
      this.UpdateBulletDisplay();
    }

    public FireArmRoundClass RemoveRoundReturnClass()
    {
      FireArmRoundClass lrClass = this.LoadedRounds[this.m_numRounds - 1].LR_Class;
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
            this.LoadedRounds[this.m_numRounds - 1] = (FVRFireArmClip.FVRLoadedRound) null;
            --this.m_numRounds;
          }
          this.UpdateBulletDisplay();
        }
      }
      return lrClass;
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
            this.LoadedRounds[this.m_numRounds - 1] = (FVRFireArmClip.FVRLoadedRound) null;
            --this.m_numRounds;
          }
          this.UpdateBulletDisplay();
        }
      }
      return gameObject;
    }

    public void UpdateBulletDisplay()
    {
      int index1 = this.m_numRounds - 1;
      for (int index2 = 0; index2 < this.DisplayBullets.Length; ++index2)
      {
        if ((UnityEngine.Object) this.DisplayBullets[index2] != (UnityEngine.Object) null)
        {
          if (index2 >= this.m_numRounds || index1 < 0)
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
    }

    public void ReloadClipWithType(FireArmRoundClass rClass)
    {
      this.m_numRounds = 0;
      for (int index = 0; index < this.m_capacity; ++index)
        this.AddRound(rClass, false, false);
      this.UpdateBulletDisplay();
    }

    public void ReloadClipWithList(List<FireArmRoundClass> list)
    {
      this.m_numRounds = 0;
      List<FireArmRoundClass> fireArmRoundClassList = new List<FireArmRoundClass>();
      for (int index = list.Count - this.m_capacity; index < list.Count; ++index)
        fireArmRoundClassList.Add(list[index]);
      int num = Mathf.Min(fireArmRoundClassList.Count, this.m_capacity);
      for (int index = 0; index < num; ++index)
        this.AddRound(fireArmRoundClassList[index], false, false);
      this.UpdateBulletDisplay();
    }

    public void Release()
    {
      this.State = FVRFireArmClip.ClipState.Free;
      this.SetParentage((Transform) null);
      this.transform.position = this.FireArm.ClipEjectPos.position;
      this.transform.rotation = this.FireArm.ClipEjectPos.rotation;
      this.RecoverRigidbody();
      if ((UnityEngine.Object) this.FireArm != (UnityEngine.Object) null)
        SM.PlayGenericSound(this.RemoveClip, this.transform.position);
      this.RootRigidbody.isKinematic = false;
      this.RootRigidbody.velocity = this.FireArm.RootRigidbody.velocity + this.transform.up * this.EjectionSpeed;
      this.RootRigidbody.angularVelocity = this.FireArm.RootRigidbody.angularVelocity;
      this.FireArm = (FVRFireArm) null;
      this.SetAllCollidersToLayer(false, "Default");
      this.Interface.gameObject.SetActive(false);
    }

    public void Load(FVRFireArm fireArm)
    {
      this.State = FVRFireArmClip.ClipState.Locked;
      this.FireArm = fireArm;
      this.FireArm.LoadClip(this);
      this.IsHeld = false;
      this.ForceBreakInteraction();
      this.SetParentage(this.FireArm.transform);
      this.transform.rotation = this.FireArm.ClipMountPos.rotation;
      this.transform.position = this.FireArm.ClipMountPos.position;
      this.StoreAndDestroyRigidbody();
      SM.PlayGenericSound(this.AffixClip, this.transform.position);
      if ((UnityEngine.Object) this.FireArm.QuickbeltSlot != (UnityEngine.Object) null)
        this.SetAllCollidersToLayer(false, "NoCol");
      else
        this.SetAllCollidersToLayer(false, "Default");
      this.Interface.gameObject.SetActive(true);
    }

    public override bool IsInteractable() => this.State != FVRFireArmClip.ClipState.Locked && base.IsInteractable();

    protected override void FVRFixedUpdate()
    {
      if ((double) this.m_timeSinceRoundInserted < 5.0)
        this.m_timeSinceRoundInserted += Time.deltaTime;
      base.FVRFixedUpdate();
    }

    public enum ClipState
    {
      Free,
      Locked,
    }

    [Serializable]
    public class FVRLoadedRound
    {
      public FireArmRoundClass LR_Class;
      public Mesh LR_Mesh;
      public Material LR_Material;
      public FVRObject LR_ObjectWrapper;
      public GameObject LR_ProjectilePrefab;
    }

    [Serializable]
    public class FVRClipLoadingPattern
    {
      public FireArmRoundClass[] Classes;
    }
  }
}
