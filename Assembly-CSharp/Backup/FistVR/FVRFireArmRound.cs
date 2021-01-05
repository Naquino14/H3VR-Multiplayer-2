// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFireArmRound
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FVRFireArmRound : FVRPhysicalObject, IFVRDamageable
  {
    [Header("FireArm Round Config")]
    [SearchableEnum]
    public FireArmRoundType RoundType;
    [SearchableEnum]
    public FireArmRoundClass RoundClass;
    public bool IsHighPressure = true;
    public GameObject BallisticProjectilePrefab;
    public GameObject ProjectilePrefab;
    public int NumProjectiles = 1;
    public float ProjectileSpread;
    public Renderer UnfiredRenderer;
    public Renderer FiredRenderer;
    private bool m_isChambered;
    private bool m_isSpent;
    public bool IsDestroyedAfterCounter = true;
    private float m_killAfter = 10f;
    private bool m_isKillCounting;
    public bool isManuallyChamberable;
    [HideInInspector]
    public FVRFireArmChamber HoveredOverChamber;
    public bool isMagazineLoadable;
    private FVRFireArmMagazineReloadTrigger m_hoverOverReloadTrigger;
    private float m_phys_mass;
    private float m_phys_drag;
    private float m_phys_angularDrag;
    private float m_pickUpCooldown = 1f;
    public bool isPalmable = true;
    public Vector3 PalmingDimensions = new Vector3(0.01f, 0.01f, 0.03f);
    public int MaxPalmedAmount = 5;
    [HideInInspector]
    public FVRFireArmRound HoveredOverRound;
    public List<FVRFireArmRound.ProxyRound> ProxyRounds = new List<FVRFireArmRound.ProxyRound>();
    private bool m_proxyDumpFlag = true;
    public bool IsCaseless;
    private bool isCookingOff;
    private float TickTilCookOff = 1f;
    public PMaterial PopType = PMaterial.BulletPopNormal;
    [Header("Audio Configuration")]
    public FVRTailSoundClass TailClass;
    public FVRTailSoundClass TailClassSuppressed = FVRTailSoundClass.SuppressedSmall;
    public ImpactType ImpactSound_Unfired;
    public ImpactType ImpactSound_Fired;

    public bool IsChambered => this.m_isChambered;

    public bool IsSpent => this.m_isSpent;

    protected override void Awake()
    {
      base.Awake();
      if ((UnityEngine.Object) this.UnfiredRenderer != (UnityEngine.Object) null)
        this.UnfiredRenderer.enabled = true;
      if ((UnityEngine.Object) this.FiredRenderer != (UnityEngine.Object) null)
        this.FiredRenderer.enabled = false;
      this.m_phys_mass = this.RootRigidbody.mass;
      this.m_phys_drag = this.RootRigidbody.drag;
      this.m_phys_angularDrag = this.RootRigidbody.angularDrag;
      switch (GM.Options.SimulationOptions.ShellTime)
      {
        case SimulationOptions.SpentShellDespawnTime.Seconds_5:
          this.m_killAfter = 5f;
          break;
        case SimulationOptions.SpentShellDespawnTime.Seconds_10:
          this.m_killAfter = 10f;
          break;
        case SimulationOptions.SpentShellDespawnTime.Seconds_30:
          this.m_killAfter = 30f;
          break;
        case SimulationOptions.SpentShellDespawnTime.Infinite:
          this.m_killAfter = 999999f;
          break;
      }
      if (GM.CurrentSceneSettings.ForcesCasingDespawn)
        this.m_killAfter = 5f;
      if (this.IsCaseless)
      {
        this.IsDestroyedAfterCounter = true;
        this.m_killAfter = 0.1f;
      }
      if (!this.HasImpactController)
        return;
      this.AudioImpactController.ImpactType = this.ImpactSound_Unfired;
    }

    public override bool IsInteractable() => !this.m_isChambered;

    public void Fire()
    {
      if ((UnityEngine.Object) this.UnfiredRenderer != (UnityEngine.Object) null)
        this.UnfiredRenderer.enabled = false;
      if ((UnityEngine.Object) this.FiredRenderer != (UnityEngine.Object) null)
        this.FiredRenderer.enabled = true;
      this.m_isSpent = true;
      if (!this.HasImpactController)
        return;
      this.AudioImpactController.ImpactType = this.ImpactSound_Fired;
    }

    public void Damage(FistVR.Damage d)
    {
      if (this.m_isSpent || (UnityEngine.Object) this.QuickbeltSlot != (UnityEngine.Object) null || this.m_isChambered)
        return;
      if ((double) d.Dam_TotalKinetic > 100.0)
      {
        this.Splode(UnityEngine.Random.Range(0.3f, 0.8f), false, false);
      }
      else
      {
        if ((double) d.Dam_Thermal <= 0.0)
          return;
        this.isCookingOff = true;
        this.TickTilCookOff = UnityEngine.Random.Range(0.5f, 1.5f);
      }
    }

    private void Splode(float velMultiplier, bool isRandomDir, bool DoesDestroyBrass)
    {
      this.Fire();
      if (this.IsHeld)
      {
        this.EndInteraction(this.m_hand);
        this.ForceBreakInteraction();
      }
      this.isCookingOff = false;
      for (int index = 0; index < this.NumProjectiles; ++index)
      {
        float max = this.ProjectileSpread * 5f;
        if (isRandomDir)
          max *= 5f;
        if ((UnityEngine.Object) this.BallisticProjectilePrefab != (UnityEngine.Object) null)
        {
          GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.BallisticProjectilePrefab, this.transform.position + this.PalmingDimensions.z * this.transform.forward * 2f, this.transform.rotation);
          gameObject.transform.Rotate(new Vector3(UnityEngine.Random.Range(-max, max), UnityEngine.Random.Range(-max, max), 0.0f));
          BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
          component.Fire(component.MuzzleVelocityBase * velMultiplier, this.transform.forward, (FVRFireArm) null);
        }
      }
      AudioEvent ClipSet = new AudioEvent();
      ClipSet.Clips.Add(PM.GetRandomImpactClip(this.PopType));
      ClipSet.VolumeRange = new Vector2(0.3f, 0.4f);
      ClipSet.PitchRange = new Vector2(0.8f, 1.1f);
      float delay = Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) / 343f;
      SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, ClipSet, this.transform.position, delay);
      if (this.IsSpent && this.IsDestroyedAfterCounter)
        this.m_isKillCounting = true;
      this.RootRigidbody.velocity = -this.transform.forward * UnityEngine.Random.Range(2f, 15f);
      if (!DoesDestroyBrass)
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }

    public void Recharge()
    {
      if ((UnityEngine.Object) this.UnfiredRenderer != (UnityEngine.Object) null)
        this.UnfiredRenderer.enabled = true;
      if ((UnityEngine.Object) this.FiredRenderer != (UnityEngine.Object) null)
        this.FiredRenderer.enabled = false;
      this.m_isSpent = false;
      if (!this.HasImpactController)
        return;
      this.AudioImpactController.ImpactType = this.ImpactSound_Unfired;
    }

    public void Chamber(FVRFireArmChamber c, bool makeChamberingSound)
    {
      c.SetRound(this);
      if (!makeChamberingSound)
        return;
      c.PlayChamberingAudio();
    }

    public void Eject(
      Vector3 EjectionPosition,
      Vector3 EjectionVelocity,
      Vector3 EjectionAngularVelocity)
    {
      this.GetComponent<Collider>().enabled = true;
      this.m_pickUpCooldown = 1f;
      if ((UnityEngine.Object) this.RootRigidbody == (UnityEngine.Object) null)
        this.RootRigidbody = this.gameObject.AddComponent<Rigidbody>();
      this.RootRigidbody.mass = this.m_phys_mass;
      this.RootRigidbody.drag = this.m_phys_drag;
      this.RootRigidbody.angularDrag = this.m_phys_angularDrag;
      this.RootRigidbody.isKinematic = false;
      this.RootRigidbody.useGravity = true;
      this.SetParentage((Transform) null);
      this.SetAllCollidersToLayer(false, "Default");
      this.HoveredOverChamber = (FVRFireArmChamber) null;
      this.m_isChambered = false;
      this.RootRigidbody.transform.position = EjectionPosition;
      this.RootRigidbody.velocity = Vector3.Lerp(EjectionVelocity * 0.7f, EjectionVelocity, UnityEngine.Random.value);
      this.RootRigidbody.maxAngularVelocity = 200f;
      this.RootRigidbody.angularVelocity = Vector3.Lerp(EjectionAngularVelocity * 0.3f, EjectionAngularVelocity, UnityEngine.Random.value);
      if (!this.IsSpent || !this.IsDestroyedAfterCounter)
        return;
      this.m_isKillCounting = true;
    }

    public void SetKillCounting(bool b) => this.m_isKillCounting = b;

    public override GameObject DuplicateFromSpawnLock(FVRViveHand hand)
    {
      GameObject gameObject = base.DuplicateFromSpawnLock(hand);
      FVRFireArmRound component = gameObject.GetComponent<FVRFireArmRound>();
      for (int index = 0; index < this.ProxyRounds.Count; ++index)
        component.AddProxy(this.ProxyRounds[index].Class, this.ProxyRounds[index].ObjectWrapper);
      component.UpdateProxyDisplay();
      return gameObject;
    }

    public void AddProxy(FireArmRoundClass roundClass, FVRObject prefabWrapper)
    {
      FVRFireArmRound.ProxyRound proxyRound = new FVRFireArmRound.ProxyRound();
      GameObject gameObject = new GameObject("Proxy");
      proxyRound.GO = gameObject;
      gameObject.transform.SetParent(this.transform);
      proxyRound.Filter = gameObject.AddComponent<MeshFilter>();
      proxyRound.Renderer = (Renderer) gameObject.AddComponent<MeshRenderer>();
      proxyRound.Class = roundClass;
      proxyRound.ObjectWrapper = prefabWrapper;
      this.ProxyRounds.Add(proxyRound);
    }

    public void PalmRound(
      FVRFireArmRound round,
      bool insertAtFront,
      bool updateDisplay,
      int addAtIndex = 0)
    {
      SM.PlayHandlingGrabSound(this.HandlingGrabSound, this.transform.position, false);
      FVRFireArmRound.ProxyRound proxyRound = new FVRFireArmRound.ProxyRound();
      GameObject gameObject = new GameObject("Proxy");
      proxyRound.GO = gameObject;
      gameObject.transform.SetParent(this.transform);
      proxyRound.Filter = gameObject.AddComponent<MeshFilter>();
      proxyRound.Renderer = (Renderer) gameObject.AddComponent<MeshRenderer>();
      proxyRound.Class = round.RoundClass;
      proxyRound.ObjectWrapper = round.ObjectWrapper;
      if (insertAtFront)
      {
        for (int index = this.ProxyRounds.Count - 1; index >= 1; --index)
          this.ProxyRounds[index] = this.ProxyRounds[index - 1];
        this.ProxyRounds[0] = proxyRound;
      }
      else
        this.ProxyRounds.Add(proxyRound);
      this.HoveredOverRound = (FVRFireArmRound) null;
      UnityEngine.Object.Destroy((UnityEngine.Object) round.gameObject);
      if (!updateDisplay)
        return;
      this.UpdateProxyDisplay();
    }

    public void CycleToProxy(bool forward, bool destroyThisRound)
    {
      int num = 0;
      if (!forward)
        num = this.ProxyRounds.Count - 1;
      this.m_proxyDumpFlag = false;
      FVRFireArmRound.ProxyRound proxyRound = this.ProxyRounds[num];
      if (forward)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) this.ProxyRounds[num].GO);
        this.ProxyRounds.RemoveAt(num);
      }
      if (destroyThisRound)
        this.PalmRound(this, !forward, true, num);
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(proxyRound.ObjectWrapper.GetGameObject(), this.transform.position, this.transform.rotation);
      FVRFireArmRound component = gameObject.GetComponent<FVRFireArmRound>();
      for (int index = 0; index < this.ProxyRounds.Count; ++index)
        component.ProxyRounds.Add(this.ProxyRounds[index]);
      for (int index = 0; index < this.ProxyRounds.Count; ++index)
        this.ProxyRounds[index].GO.transform.SetParent(gameObject.transform);
      if (!destroyThisRound)
        this.ProxyRounds.Clear();
      component.BeginInteraction(this.m_hand);
      this.m_hand.ForceSetInteractable((FVRInteractiveObject) component);
      if (!destroyThisRound)
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }

    public void UpdateProxyDisplay()
    {
      if (this.ProxyRounds.Count <= 0)
        return;
      if (this.IsHeld)
      {
        Vector3 position = this.transform.position;
        Vector3 vector3 = -this.transform.forward * this.PalmingDimensions.z + -this.transform.up * this.PalmingDimensions.y;
        for (int index = 0; index < this.ProxyRounds.Count; ++index)
        {
          this.ProxyRounds[index].GO.transform.position = position + vector3 * (float) (index + 2);
          this.ProxyRounds[index].GO.transform.localRotation = Quaternion.identity;
        }
      }
      else
      {
        Vector3 position = this.transform.position;
        Vector3 vector3 = -this.transform.up * this.PalmingDimensions.y;
        for (int index = 0; index < this.ProxyRounds.Count; ++index)
        {
          this.ProxyRounds[index].GO.transform.position = position + vector3 * (float) (index + 2);
          this.ProxyRounds[index].GO.transform.localRotation = Quaternion.identity;
        }
      }
      for (int index = 0; index < this.ProxyRounds.Count; ++index)
      {
        this.ProxyRounds[index].Filter.mesh = AM.GetRoundMesh(this.RoundType, this.ProxyRounds[index].Class);
        this.ProxyRounds[index].Renderer.material = AM.GetRoundMaterial(this.RoundType, this.ProxyRounds[index].Class);
      }
    }

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      this.m_pickUpCooldown = 0.3f;
      this.m_proxyDumpFlag = true;
      this.UpdateProxyDisplay();
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      bool flag4 = false;
      if (hand.IsInStreamlinedMode)
      {
        if (hand.Input.BYButtonDown)
          flag3 = true;
        if (hand.Input.AXButtonPressed)
          flag4 = true;
      }
      else
      {
        if (hand.Input.TouchpadDown && (double) hand.Input.TouchpadAxes.magnitude > 0.100000001490116)
        {
          if ((double) Vector2.Angle(hand.Input.TouchpadAxes, Vector2.right) < 45.0)
            flag1 = true;
          else if ((double) Vector2.Angle(hand.Input.TouchpadAxes, Vector2.left) < 45.0)
            flag2 = true;
          else if ((double) Vector2.Angle(hand.Input.TouchpadAxes, Vector2.up) < 45.0)
            flag3 = true;
        }
        if (hand.Input.TouchpadPressed && (double) hand.Input.TouchpadAxes.magnitude > 0.100000001490116 && (double) Vector2.Angle(hand.Input.TouchpadAxes, Vector2.down) < 45.0)
          flag4 = true;
      }
      if (!this.isPalmable || this.MaxPalmedAmount <= 1)
        return;
      if (flag1)
      {
        if (this.ProxyRounds.Count <= 0)
          return;
        this.CycleToProxy(true, true);
        SM.PlayHandlingGrabSound(this.HandlingGrabSound, this.transform.position, false);
      }
      else if (flag2)
      {
        if (this.ProxyRounds.Count <= 0)
          return;
        this.CycleToProxy(false, true);
        SM.PlayHandlingGrabSound(this.HandlingGrabSound, this.transform.position, false);
      }
      else if (flag3 && !this.IsSpent)
      {
        if ((UnityEngine.Object) hand.OtherHand.CurrentInteractable == (UnityEngine.Object) null && hand.OtherHand.Input.IsGrabbing && (double) Vector3.Distance(hand.Input.Pos, hand.OtherHand.Input.Pos) < 0.150000005960464)
        {
          if (this.ProxyRounds.Count > 0)
          {
            this.CycleToProxy(true, false);
            this.m_proxyDumpFlag = false;
          }
          hand.OtherHand.ForceSetInteractable((FVRInteractiveObject) this);
          this.BeginInteraction(hand.OtherHand);
          this.UpdateProxyDisplay();
          this.m_proxyDumpFlag = true;
        }
        else if (hand.OtherHand.CurrentInteractable is FVRFireArmRound && ((FVRFireArmRound) hand.OtherHand.CurrentInteractable).RoundType == this.RoundType && (((FVRFireArmRound) hand.OtherHand.CurrentInteractable).ProxyRounds.Count < ((FVRFireArmRound) hand.OtherHand.CurrentInteractable).MaxPalmedAmount && (double) Vector3.Distance(hand.Input.Pos, hand.OtherHand.Input.Pos) < 0.150000005960464))
        {
          ((FVRFireArmRound) hand.OtherHand.CurrentInteractable).AddProxy(this.RoundClass, this.ObjectWrapper);
          ((FVRFireArmRound) hand.OtherHand.CurrentInteractable).UpdateProxyDisplay();
          if (this.ProxyRounds.Count > 0)
          {
            this.CycleToProxy(true, false);
            this.m_proxyDumpFlag = false;
          }
          SM.PlayHandlingGrabSound(this.HandlingGrabSound, this.transform.position, false);
          this.ForceBreakInteraction();
          UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
        }
        else if ((UnityEngine.Object) hand.CurrentHoveredQuickbeltSlotDirty != (UnityEngine.Object) null && (UnityEngine.Object) hand.CurrentHoveredQuickbeltSlotDirty.HeldObject == (UnityEngine.Object) null)
        {
          if (this.ProxyRounds.Count > 0)
          {
            this.CycleToProxy(true, false);
            this.m_proxyDumpFlag = false;
          }
          else
            this.ForceBreakInteraction();
          SM.PlayHandlingReleaseIntoSlotSound(this.HandlingReleaseIntoSlotSound, this.transform.position);
          this.EndInteractionIntoInventorySlot(hand, hand.CurrentHoveredQuickbeltSlot);
          this.UpdateProxyDisplay();
          this.m_proxyDumpFlag = true;
        }
        else if ((UnityEngine.Object) hand.CurrentHoveredQuickbeltSlotDirty != (UnityEngine.Object) null && hand.CurrentHoveredQuickbeltSlotDirty.HeldObject is FVRFireArmRound && (((FVRFireArmRound) hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).RoundType == this.RoundType && ((FVRFireArmRound) hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).ProxyRounds.Count < ((FVRFireArmRound) hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).MaxPalmedAmount))
        {
          ((FVRFireArmRound) hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).AddProxy(this.RoundClass, this.ObjectWrapper);
          ((FVRFireArmRound) hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).UpdateProxyDisplay();
          if (this.ProxyRounds.Count > 0)
          {
            this.CycleToProxy(true, false);
            this.m_proxyDumpFlag = false;
          }
          SM.PlayHandlingReleaseIntoSlotSound(this.HandlingReleaseIntoSlotSound, this.transform.position);
          this.ForceBreakInteraction();
          UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
        }
        else
        {
          if (this.ProxyRounds.Count <= 0)
            return;
          SM.PlayHandlingGrabSound(this.HandlingGrabSound, this.transform.position, false);
          this.CycleToProxy(true, false);
          this.m_proxyDumpFlag = false;
          this.transform.position += this.PalmingDimensions.z * this.transform.forward - this.PalmingDimensions.y * this.transform.up;
        }
      }
      else
      {
        if (!flag4 || !((UnityEngine.Object) this.HoveredOverRound != (UnityEngine.Object) null))
          return;
        this.PalmRound(this.HoveredOverRound, false, true);
      }
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      base.EndInteraction(hand);
      if (this.ProxyRounds.Count <= 0 || !this.m_proxyDumpFlag)
        return;
      for (int index = 0; index < this.ProxyRounds.Count; ++index)
      {
        UnityEngine.Object.Instantiate<GameObject>(this.ProxyRounds[index].ObjectWrapper.GetGameObject(), this.ProxyRounds[index].GO.transform.position, this.ProxyRounds[index].GO.transform.rotation).GetComponent<Rigidbody>().velocity = this.RootRigidbody.velocity;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.ProxyRounds[index].GO);
        this.ProxyRounds[index].GO = (GameObject) null;
        this.ProxyRounds[index].Filter = (MeshFilter) null;
        this.ProxyRounds[index].Renderer = (Renderer) null;
        this.ProxyRounds[index].ObjectWrapper = (FVRObject) null;
      }
      this.ProxyRounds.Clear();
    }

    public override void EndInteractionIntoInventorySlot(FVRViveHand hand, FVRQuickBeltSlot slot)
    {
      base.EndInteractionIntoInventorySlot(hand, slot);
      this.UpdateProxyDisplay();
    }

    protected override void FVRFixedUpdate()
    {
      base.FVRFixedUpdate();
      if (this.m_isSpent || this.IsHeld || (this.m_isChambered || (UnityEngine.Object) this.QuickbeltSlot != (UnityEngine.Object) null))
        this.isCookingOff = false;
      if (this.isCookingOff)
      {
        this.TickTilCookOff -= Time.deltaTime;
        if ((double) this.TickTilCookOff <= 0.0)
          this.Splode(UnityEngine.Random.Range(0.2f, 0.5f), false, false);
      }
      float num = 5f;
      switch (GM.Options.SimulationOptions.ShellTime)
      {
        case SimulationOptions.SpentShellDespawnTime.Seconds_5:
          num = 5f;
          break;
        case SimulationOptions.SpentShellDespawnTime.Seconds_10:
          num = 10f;
          break;
        case SimulationOptions.SpentShellDespawnTime.Seconds_30:
          num = 30f;
          break;
        case SimulationOptions.SpentShellDespawnTime.Infinite:
          num = 999999f;
          break;
      }
      if ((double) this.m_pickUpCooldown > 0.0)
        this.m_pickUpCooldown -= Time.deltaTime;
      if (this.m_isKillCounting && (UnityEngine.Object) this.QuickbeltSlot == (UnityEngine.Object) null)
      {
        if ((double) this.m_killAfter > (double) num)
          this.m_killAfter = num;
        this.m_killAfter -= Time.deltaTime;
        if ((double) this.m_killAfter <= 0.0)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }
      bool flag = true;
      if (this.isManuallyChamberable && (double) this.m_pickUpCooldown <= 0.0 && (UnityEngine.Object) this.QuickbeltSlot == (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.HoveredOverChamber != (UnityEngine.Object) null && !this.HoveredOverChamber.IsFull && this.HoveredOverChamber.IsAccessible)
        {
          this.Chamber(this.HoveredOverChamber, true);
          flag = false;
          if (this.ProxyRounds.Count > 0)
          {
            this.CycleToProxy(true, false);
            this.m_proxyDumpFlag = false;
          }
          this.ForceBreakInteraction();
          UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
        }
        else
          this.HoveredOverChamber = (FVRFireArmChamber) null;
      }
      if (!((UnityEngine.Object) this.m_hoverOverReloadTrigger != (UnityEngine.Object) null) || !flag)
        return;
      if (this.m_hoverOverReloadTrigger.IsClipTrigger)
      {
        if (!this.IsHeld && !this.m_hoverOverReloadTrigger.Clip.IsDropInLoadable || (this.IsSpent || !((UnityEngine.Object) this.QuickbeltSlot == (UnityEngine.Object) null)) || (this.m_hoverOverReloadTrigger.Clip.RoundType != this.RoundType || (double) this.m_hoverOverReloadTrigger.Clip.TimeSinceRoundInserted <= 0.5))
          return;
        if (this.ProxyRounds.Count > 0)
        {
          this.CycleToProxy(true, false);
          this.m_proxyDumpFlag = false;
        }
        this.m_hoverOverReloadTrigger.Clip.AddRound(this, true, true);
        this.ForceBreakInteraction();
        UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }
      else if (this.m_hoverOverReloadTrigger.IsSpeedloaderTrigger)
      {
        if (!this.IsHeld || this.IsSpent || (!((UnityEngine.Object) this.QuickbeltSlot == (UnityEngine.Object) null) || this.m_hoverOverReloadTrigger.SpeedloaderChamber.Type != this.RoundType))
          return;
        if (this.ProxyRounds.Count > 0)
        {
          this.CycleToProxy(true, false);
          this.m_proxyDumpFlag = false;
        }
        this.m_hoverOverReloadTrigger.SpeedloaderChamber.Load(this.RoundClass, true);
        this.ForceBreakInteraction();
        UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }
      else
      {
        if (!this.IsHeld && !this.m_hoverOverReloadTrigger.Magazine.IsDropInLoadable || (this.IsSpent || !((UnityEngine.Object) this.QuickbeltSlot == (UnityEngine.Object) null)) || (this.m_hoverOverReloadTrigger.Magazine.RoundType != this.RoundType || (double) this.m_hoverOverReloadTrigger.Magazine.TimeSinceRoundInserted <= 0.300000011920929))
          return;
        if (this.ProxyRounds.Count > 0)
        {
          this.CycleToProxy(true, false);
          this.m_proxyDumpFlag = false;
        }
        this.m_hoverOverReloadTrigger.Magazine.AddRound(this, true, true);
        this.ForceBreakInteraction();
        UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }
    }

    public void OnParticleCollision(GameObject other)
    {
      if (!other.CompareTag("IgnitorSystem"))
        return;
      this.isCookingOff = true;
      this.TickTilCookOff = Mathf.Min(this.TickTilCookOff, UnityEngine.Random.Range(0.5f, 1.5f));
    }

    public void OnTriggerEnter(Collider collider)
    {
      if (this.IsSpent)
        return;
      if (this.isManuallyChamberable && !this.IsSpent && ((UnityEngine.Object) this.HoveredOverChamber == (UnityEngine.Object) null && (UnityEngine.Object) this.m_hoverOverReloadTrigger == (UnityEngine.Object) null) && (!this.IsSpent && collider.gameObject.CompareTag("FVRFireArmChamber")))
      {
        FVRFireArmChamber component = collider.gameObject.GetComponent<FVRFireArmChamber>();
        if (component.RoundType == this.RoundType && component.IsManuallyChamberable && (component.IsAccessible && !component.IsFull))
          this.HoveredOverChamber = component;
      }
      if (this.isMagazineLoadable && (UnityEngine.Object) this.HoveredOverChamber == (UnityEngine.Object) null && (!this.IsSpent && collider.gameObject.CompareTag("FVRFireArmMagazineReloadTrigger")))
      {
        FVRFireArmMagazineReloadTrigger component = collider.gameObject.GetComponent<FVRFireArmMagazineReloadTrigger>();
        if (component.IsClipTrigger)
        {
          if ((UnityEngine.Object) component != (UnityEngine.Object) null && (UnityEngine.Object) component.Clip != (UnityEngine.Object) null && (component.Clip.RoundType == this.RoundType && !component.Clip.IsFull()) && ((UnityEngine.Object) component.Clip.FireArm == (UnityEngine.Object) null || component.Clip.IsDropInLoadable))
            this.m_hoverOverReloadTrigger = component;
        }
        else if (component.IsSpeedloaderTrigger)
        {
          if (!component.SpeedloaderChamber.IsLoaded)
            this.m_hoverOverReloadTrigger = component;
        }
        else if ((UnityEngine.Object) component != (UnityEngine.Object) null && (UnityEngine.Object) component.Magazine != (UnityEngine.Object) null && (component.Magazine.RoundType == this.RoundType && !component.Magazine.IsFull()) && ((UnityEngine.Object) component.Magazine.FireArm == (UnityEngine.Object) null || component.Magazine.IsDropInLoadable))
          this.m_hoverOverReloadTrigger = component;
      }
      if (!this.isPalmable || this.ProxyRounds.Count >= this.MaxPalmedAmount || (this.IsSpent || !collider.gameObject.CompareTag(nameof (FVRFireArmRound))))
        return;
      FVRFireArmRound component1 = collider.gameObject.GetComponent<FVRFireArmRound>();
      if (component1.RoundType != this.RoundType || component1.IsSpent || !((UnityEngine.Object) component1.QuickbeltSlot == (UnityEngine.Object) null))
        return;
      this.HoveredOverRound = component1;
    }

    public void OnTriggerExit(Collider collider)
    {
      if (this.isManuallyChamberable && collider.gameObject.CompareTag("FVRFireArmChamber") && ((UnityEngine.Object) this.HoveredOverChamber != (UnityEngine.Object) null && (UnityEngine.Object) this.HoveredOverChamber.gameObject == (UnityEngine.Object) collider.gameObject))
        this.HoveredOverChamber = (FVRFireArmChamber) null;
      if (this.isMagazineLoadable && collider.gameObject.CompareTag("FVRFireArmMagazineReloadTrigger") && ((UnityEngine.Object) this.m_hoverOverReloadTrigger != (UnityEngine.Object) null && (UnityEngine.Object) this.m_hoverOverReloadTrigger.gameObject == (UnityEngine.Object) collider.gameObject))
        this.m_hoverOverReloadTrigger = (FVRFireArmMagazineReloadTrigger) null;
      if (!this.isPalmable || !((UnityEngine.Object) collider != (UnityEngine.Object) null) || (!((UnityEngine.Object) this.HoveredOverRound != (UnityEngine.Object) null) || !collider.gameObject.CompareTag(nameof (FVRFireArmRound))) || !((UnityEngine.Object) collider.gameObject == (UnityEngine.Object) this.HoveredOverRound.gameObject))
        return;
      this.HoveredOverRound = (FVRFireArmRound) null;
    }

    [ContextMenu("MigrateImpactSetting")]
    public void MigrateImpactSetting()
    {
      AudioImpactController component = this.gameObject.GetComponent<AudioImpactController>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      {
        this.ImpactSound_Fired = component.ImpactType;
        bool flag = false;
        switch (component.ImpactType)
        {
          case ImpactType.CasePistolSmall:
            flag = true;
            this.ImpactSound_Unfired = ImpactType.BulletSmall;
            break;
          case ImpactType.CasePistolLarge:
            flag = true;
            this.ImpactSound_Unfired = ImpactType.BulletSmall;
            break;
          case ImpactType.CaseRifleSmall:
            flag = true;
            this.ImpactSound_Unfired = ImpactType.BulletLarge;
            break;
          case ImpactType.CaseRifleLarge:
            flag = true;
            this.ImpactSound_Unfired = ImpactType.BulletLarge;
            break;
          case ImpactType.CaseAntiMateriel:
            flag = true;
            this.ImpactSound_Unfired = ImpactType.BulletVeryLarge;
            break;
          case ImpactType.CaseShotgun:
            flag = true;
            this.ImpactSound_Unfired = ImpactType.BulletShotgun;
            break;
          case ImpactType.CaseLauncher:
            flag = true;
            this.ImpactSound_Unfired = ImpactType.BulletVeryLarge;
            break;
          case ImpactType.CaseRifleTiny:
            flag = true;
            this.ImpactSound_Unfired = ImpactType.BulletTiny;
            break;
          case ImpactType.CasePistolTiny:
            flag = true;
            this.ImpactSound_Unfired = ImpactType.BulletTiny;
            break;
        }
        if (flag)
          return;
        Debug.Log((object) ("Cartidge " + this.gameObject.name + " used unusual impact sound"));
      }
      else
        Debug.Log((object) ("Cartidge " + this.gameObject.name + " used NO impact sound"));
    }

    [Serializable]
    public class ProxyRound
    {
      public GameObject GO;
      public MeshFilter Filter;
      public Renderer Renderer;
      public FireArmRoundClass Class;
      public FVRObject ObjectWrapper;
    }
  }
}
