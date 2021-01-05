// Decompiled with JetBrains decompiler
// Type: FistVR.Speedloader
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class Speedloader : FVRPhysicalObject
  {
    public Speedloader.SpeedLoaderType SLType;
    public List<SpeedloaderChamber> Chambers;
    public FVRFirearmMagazineAudioSet ProfileOverride;
    private RevolverCylinder HoveredCylinder;
    private RevolvingShotgunTrigger HoveredRSTrigger;
    public float forwardEjectionDisplacement;
    private float TimeTilLoadAttempt;

    public override GameObject DuplicateFromSpawnLock(FVRViveHand hand)
    {
      GameObject gameObject = base.DuplicateFromSpawnLock(hand);
      Speedloader component = gameObject.GetComponent<Speedloader>();
      for (int index = 0; index < this.Chambers.Count; ++index)
      {
        if (this.Chambers[index].IsLoaded)
        {
          component.Chambers[index].Load(this.Chambers[index].LoadedClass);
        }
        else
        {
          int num = (int) component.Chambers[index].Unload();
        }
      }
      return gameObject;
    }

    protected override void FVRFixedUpdate()
    {
      base.FVRFixedUpdate();
      if ((Object) this.HoveredCylinder != (Object) null && (double) this.TimeTilLoadAttempt <= 0.0 && ((Object) this.QuickbeltSlot == (Object) null && this.HoveredCylinder.IsInteractable()) && (double) Vector3.Angle(this.transform.forward, this.HoveredCylinder.transform.forward) < 20.0)
      {
        this.TimeTilLoadAttempt = 0.25f;
        this.HoveredCylinder.LoadFromSpeedLoader(this);
      }
      if ((double) this.TimeTilLoadAttempt <= 0.0)
        return;
      this.TimeTilLoadAttempt -= Time.deltaTime;
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (!((Object) this.HoveredRSTrigger != (Object) null) || (double) this.TimeTilLoadAttempt > 0.0 || (!((Object) this.QuickbeltSlot == (Object) null) || this.HoveredRSTrigger.Shotgun.CylinderLoaded))
        return;
      this.HoveredRSTrigger.Shotgun.LoadCylinder(this);
      Object.Destroy((Object) this.gameObject);
    }

    public void ReloadClipWithType(FireArmRoundClass rClass)
    {
      for (int index = 0; index < this.Chambers.Count; ++index)
        this.Chambers[index].Load(rClass);
    }

    public void ReloadSpeedLoaderWithList(List<FireArmRoundClass> list)
    {
      for (int index1 = this.Chambers.Count - 1; index1 >= 0; --index1)
      {
        int index2 = list.Count - 1 - index1;
        if (index2 >= 0)
          this.Chambers[index1].Load(list[index2]);
      }
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      bool flag1 = false;
      if (hand.IsInStreamlinedMode && hand.Input.BYButtonDown)
        flag1 = true;
      else if (!hand.IsInStreamlinedMode && hand.Input.TouchpadDown && (double) Vector2.Angle(hand.Input.TouchpadAxes, Vector2.up) < 45.0)
        flag1 = true;
      if (!flag1)
        return;
      SpeedloaderChamber speedloaderChamber = (SpeedloaderChamber) null;
      bool flag2 = false;
      for (int index = 0; index < this.Chambers.Count; ++index)
      {
        if (this.Chambers[index].IsLoaded)
        {
          speedloaderChamber = this.Chambers[index];
          flag2 = true;
          break;
        }
      }
      if (!flag2)
        return;
      if ((Object) this.ProfileOverride != (Object) null)
        SM.PlayGenericSound(this.ProfileOverride.MagazineEjectRound, this.transform.position);
      Vector3 vector3 = speedloaderChamber.transform.position + speedloaderChamber.transform.forward * (0.02f + this.forwardEjectionDisplacement);
      Quaternion rotation = speedloaderChamber.transform.rotation;
      if ((Object) hand.OtherHand.CurrentInteractable == (Object) null && hand.OtherHand.Input.IsGrabbing && (double) Vector3.Distance(vector3, hand.OtherHand.Input.Pos) < 0.150000005960464)
      {
        FVRFireArmRound component = Object.Instantiate<GameObject>(AM.GetRoundSelfPrefab(speedloaderChamber.Type, speedloaderChamber.LoadedClass).GetGameObject(), vector3, rotation).GetComponent<FVRFireArmRound>();
        component.SetIFF(GM.CurrentPlayerBody.GetPlayerIFF());
        hand.OtherHand.ForceSetInteractable((FVRInteractiveObject) component);
        component.BeginInteraction(hand.OtherHand);
        if (speedloaderChamber.IsSpent)
        {
          component.Fire();
          component.SetKillCounting(true);
        }
        int num = (int) speedloaderChamber.Unload();
      }
      else if (hand.OtherHand.CurrentInteractable is FVRFireArmRound && ((FVRFireArmRound) hand.OtherHand.CurrentInteractable).RoundType == speedloaderChamber.Type && (((FVRFireArmRound) hand.OtherHand.CurrentInteractable).ProxyRounds.Count < ((FVRFireArmRound) hand.OtherHand.CurrentInteractable).MaxPalmedAmount && (double) Vector3.Distance(hand.Input.Pos, hand.OtherHand.Input.Pos) < 0.150000005960464))
      {
        FireArmRoundClass loadedClass = speedloaderChamber.LoadedClass;
        FVRObject roundSelfPrefab = AM.GetRoundSelfPrefab(speedloaderChamber.Type, speedloaderChamber.LoadedClass);
        ((FVRFireArmRound) hand.OtherHand.CurrentInteractable).AddProxy(loadedClass, roundSelfPrefab);
        ((FVRFireArmRound) hand.OtherHand.CurrentInteractable).UpdateProxyDisplay();
        int num = (int) speedloaderChamber.Unload();
      }
      else if ((Object) hand.CurrentHoveredQuickbeltSlotDirty != (Object) null && (Object) hand.CurrentHoveredQuickbeltSlotDirty.HeldObject == (Object) null)
      {
        FVRFireArmRound component = Object.Instantiate<GameObject>(AM.GetRoundSelfPrefab(speedloaderChamber.Type, speedloaderChamber.LoadedClass).GetGameObject(), vector3, rotation).GetComponent<FVRFireArmRound>();
        component.SetIFF(GM.CurrentPlayerBody.GetPlayerIFF());
        component.SetQuickBeltSlot(hand.CurrentHoveredQuickbeltSlotDirty);
        if (speedloaderChamber.IsSpent)
        {
          component.Fire();
          component.SetKillCounting(true);
        }
        int num = (int) speedloaderChamber.Unload();
      }
      else if ((Object) hand.CurrentHoveredQuickbeltSlotDirty != (Object) null && hand.CurrentHoveredQuickbeltSlotDirty.HeldObject is FVRFireArmRound && (((FVRFireArmRound) hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).RoundType == speedloaderChamber.Type && ((FVRFireArmRound) hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).ProxyRounds.Count < ((FVRFireArmRound) hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).MaxPalmedAmount))
      {
        FireArmRoundClass loadedClass = speedloaderChamber.LoadedClass;
        FVRObject roundSelfPrefab = AM.GetRoundSelfPrefab(speedloaderChamber.Type, speedloaderChamber.LoadedClass);
        ((FVRFireArmRound) hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).AddProxy(loadedClass, roundSelfPrefab);
        ((FVRFireArmRound) hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).UpdateProxyDisplay();
        int num = (int) speedloaderChamber.Unload();
      }
      else
      {
        GameObject gameObject = Object.Instantiate<GameObject>(AM.GetRoundSelfPrefab(speedloaderChamber.Type, speedloaderChamber.LoadedClass).GetGameObject(), vector3, rotation);
        FVRFireArmRound component = gameObject.GetComponent<FVRFireArmRound>();
        gameObject.GetComponent<FVRFireArmRound>().SetIFF(GM.CurrentPlayerBody.GetPlayerIFF());
        gameObject.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * 0.5f);
        if (speedloaderChamber.IsSpent)
        {
          component.Fire();
          component.SetKillCounting(true);
        }
        int num = (int) speedloaderChamber.Unload();
      }
    }

    public void OnTriggerEnter(Collider c)
    {
      if (!((Object) this.QuickbeltSlot == (Object) null))
        return;
      RevolverCylinder component1 = c.GetComponent<RevolverCylinder>();
      if ((Object) component1 != (Object) null && component1.Revolver.RoundType == this.Chambers[0].Type)
        this.HoveredCylinder = component1;
      RevolvingShotgunTrigger component2 = c.GetComponent<RevolvingShotgunTrigger>();
      if (!((Object) component2 != (Object) null) || (double) component2.Shotgun.EjectDelay > 0.0 || (!c.gameObject.CompareTag("FVRFireArmReloadTriggerWell") || component2.Shotgun.RoundType != this.Chambers[0].Type) || this.SLType != component2.Shotgun.SLType)
        return;
      this.HoveredRSTrigger = component2;
    }

    public void OnTriggerExit(Collider c)
    {
      RevolverCylinder component = c.GetComponent<RevolverCylinder>();
      if ((Object) component != (Object) null && (Object) this.HoveredCylinder == (Object) component)
        this.HoveredCylinder = (RevolverCylinder) null;
      if (!((Object) c.GetComponent<RevolvingShotgunTrigger>() != (Object) null))
        return;
      this.HoveredRSTrigger = (RevolvingShotgunTrigger) null;
    }

    public enum SpeedLoaderType
    {
      Standard,
      P6Twelve,
      Jackhammer,
    }
  }
}
