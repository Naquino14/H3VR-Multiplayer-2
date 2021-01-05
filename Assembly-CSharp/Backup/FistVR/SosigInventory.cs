// Decompiled with JetBrains decompiler
// Type: FistVR.SosigInventory
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [Serializable]
  public class SosigInventory
  {
    public Sosig S;
    public List<SosigInventory.Slot> Slots = new List<SosigInventory.Slot>();
    private int[] m_ammoStores = new int[12];
    private int[] m_objectsByType = new int[6];

    public void Init()
    {
      for (int index = 0; index < this.Slots.Count; ++index)
        this.Slots[index].SetInventory(this);
    }

    public void FillAllAmmo()
    {
      for (int index = 0; index < this.m_ammoStores.Length; ++index)
        this.m_ammoStores[index] = 200;
    }

    public void FillAmmoWithType(SosigWeapon.SosiggunAmmoType t) => this.m_ammoStores[(int) t] = 200;

    public void FillAmmoWithType(SosigWeapon.SosiggunAmmoType t, int i) => this.m_ammoStores[(int) t] = i;

    public void PhysHold()
    {
      for (int index = 0; index < this.Slots.Count; ++index)
        this.Slots[index].PhysHold();
    }

    public bool DoINeed(SosigWeapon w)
    {
      int num = 2;
      switch (w.Type)
      {
        case SosigWeapon.SosigWeaponType.Gun:
          num = 4;
          break;
        case SosigWeapon.SosigWeaponType.Melee:
          num = 1;
          break;
        case SosigWeapon.SosigWeaponType.Grenade:
          num = 1;
          break;
        case SosigWeapon.SosigWeaponType.Shield:
          num = 0;
          break;
      }
      return this.m_objectsByType[(int) w.Type] < num;
    }

    public bool HasAmmoFor(SosigWeapon w) => w.AmmoType == SosigWeapon.SosiggunAmmoType.None || this.HasAmmo((int) w.AmmoType);

    public bool HasAmmo(int i) => this.m_ammoStores[i] > 0;

    public int ReloadFromType(int i, int amount)
    {
      if (this.m_ammoStores[i] >= amount)
      {
        this.m_ammoStores[i] -= amount;
        return amount;
      }
      int ammoStore = this.m_ammoStores[i];
      this.m_ammoStores[i] = 0;
      return ammoStore;
    }

    public bool PutObjectInMe(SosigWeapon o)
    {
      if (!this.IsThereAFreeSlot())
        return false;
      if (this.DoINeed(o))
        this.GetFreeSlot().PlaceObjectIn(o);
      return true;
    }

    public void SwapObjectFromHandToObjectInInventory(
      SosigWeapon fromHand,
      SosigWeapon fromInventory)
    {
      SosigInventory.Slot inventorySlotWithThis = fromInventory.InventorySlotWithThis;
      inventorySlotWithThis.I.DropObjectInSlot(inventorySlotWithThis);
      SosigHand handHoldingThis = fromHand.HandHoldingThis;
      handHoldingThis.DropHeldObject();
      this.PutObjectInMe(fromHand);
      handHoldingThis.PickUp(fromInventory);
    }

    public void DropObjectInSlot(SosigInventory.Slot s)
    {
      for (int index = 0; index < this.Slots.Count; ++index)
      {
        if (this.Slots[index] == s)
          this.Slots[index].DetachHeldObject();
      }
    }

    public void DropAllObjects()
    {
      for (int index = 0; index < this.Slots.Count; ++index)
        this.Slots[index].DetachHeldObject();
    }

    public bool IsThereAFreeSlot()
    {
      if (this.Slots.Count == 0)
        return false;
      bool flag = false;
      for (int index = 0; index < this.Slots.Count; ++index)
      {
        if (!this.Slots[index].IsHoldingObject)
          flag = true;
      }
      return flag;
    }

    public SosigInventory.Slot GetFreeSlot()
    {
      for (int index = 0; index < this.Slots.Count; ++index)
      {
        if (!this.Slots[index].IsHoldingObject)
          return this.Slots[index];
      }
      return (SosigInventory.Slot) null;
    }

    public bool DoIHaveAnyEquipment()
    {
      bool flag = false;
      for (int index = 0; index < this.Slots.Count; ++index)
      {
        if (this.Slots[index].IsHoldingObject)
          flag = true;
      }
      return flag;
    }

    public bool DoIHaveAnyWeaponry()
    {
      bool flag = false;
      for (int index = 0; index < this.Slots.Count; ++index)
      {
        if (this.Slots[index].IsHoldingObject && (this.Slots[index].HeldObject.Type == SosigWeapon.SosigWeaponType.Gun || this.Slots[index].HeldObject.Type == SosigWeapon.SosigWeaponType.Grenade || this.Slots[index].HeldObject.Type == SosigWeapon.SosigWeaponType.Melee))
          flag = true;
      }
      return flag;
    }

    public bool DoIHaveAnObjectOfType(SosigWeapon.SosigWeaponType type)
    {
      for (int index = 0; index < this.Slots.Count; ++index)
      {
        if (this.Slots[index].IsHoldingObject && this.Slots[index].HeldObject.Type == type)
          return true;
      }
      return false;
    }

    public int GetBestItemQuality()
    {
      int a = -1;
      for (int index = 0; index < this.Slots.Count; ++index)
      {
        if (this.Slots[index].IsHoldingObject)
          a = Mathf.Max(a, this.Slots[index].HeldObject.Quality);
      }
      return a;
    }

    public int GetBestItemQuality(SosigWeapon.SosigWeaponType type)
    {
      int a = -1;
      for (int index = 0; index < this.Slots.Count; ++index)
      {
        if (this.Slots[index].IsHoldingObject && this.Slots[index].HeldObject.Type == type)
          a = Mathf.Max(a, this.Slots[index].HeldObject.Quality);
      }
      return a;
    }

    public SosigWeapon GetBestWeaponOut(SosigWeapon.SosigWeaponType type)
    {
      int a = -1;
      SosigWeapon sosigWeapon = (SosigWeapon) null;
      for (int index = 0; index < this.Slots.Count; ++index)
      {
        if (this.Slots[index].IsHoldingObject && this.Slots[index].HeldObject.Type == type)
        {
          a = Mathf.Max(a, this.Slots[index].HeldObject.Quality);
          sosigWeapon = this.Slots[index].HeldObject;
        }
      }
      return sosigWeapon;
    }

    public SosigWeapon GetBestGunOut() => this.GetBestWeaponOut(SosigWeapon.SosigWeaponType.Gun);

    public SosigWeapon GetBestMeleeWeaponOut() => this.GetBestWeaponOut(SosigWeapon.SosigWeaponType.Melee);

    public SosigWeapon GetBestShieldWeaponOut() => this.GetBestWeaponOut(SosigWeapon.SosigWeaponType.Shield);

    public SosigWeapon GetBestThrownWeaponOut() => this.GetBestWeaponOut(SosigWeapon.SosigWeaponType.Grenade);

    [Serializable]
    public class Slot
    {
      private SosigInventory m_inventory;
      public Transform Target;
      public SosigLink LinkAttachedTo;
      public SosigWeapon HeldObject;
      public bool IsHoldingObject;
      private float m_timeAwayFromTarget;

      public SosigInventory I => this.m_inventory;

      public void SetInventory(SosigInventory i) => this.m_inventory = i;

      public void PlaceObjectIn(SosigWeapon o)
      {
        this.HeldObject = o;
        this.IsHoldingObject = true;
        this.HeldObject.IsInBotInventory = true;
        this.HeldObject.SosigWithInventory = this.I.S;
        this.HeldObject.InventorySlotWithThis = this;
        ++this.I.m_objectsByType[(int) o.Type];
        if (this.I.S.IgnoreRBs.Contains(o.O.RootRigidbody))
          return;
        this.I.S.IgnoreRBs.Add(o.O.RootRigidbody);
      }

      public SosigWeapon GetObjectFromSlot() => this.HeldObject;

      public void DetachHeldObject()
      {
        if (!this.IsHoldingObject)
          return;
        if ((UnityEngine.Object) this.HeldObject != (UnityEngine.Object) null && this.I.S.IgnoreRBs.Contains(this.HeldObject.O.RootRigidbody))
          this.I.S.IgnoreRBs.Remove(this.HeldObject.O.RootRigidbody);
        --this.I.m_objectsByType[(int) this.HeldObject.Type];
        this.HeldObject.SosigWithInventory = (Sosig) null;
        this.HeldObject.InventorySlotWithThis = (SosigInventory.Slot) null;
        this.HeldObject.IsInBotInventory = false;
        this.HeldObject = (SosigWeapon) null;
        this.IsHoldingObject = false;
      }

      public void PhysHold()
      {
        if (!this.IsHoldingObject || (UnityEngine.Object) this.HeldObject == (UnityEngine.Object) null)
          return;
        Vector3 position1 = this.Target.position;
        Quaternion rotation1 = this.Target.rotation;
        Vector3 position2 = this.HeldObject.RecoilHolder.position;
        Quaternion rotation2 = this.HeldObject.RecoilHolder.rotation;
        if (this.HeldObject.O.IsHeld)
        {
          if ((double) Vector3.Distance(position1, position2) > 0.699999988079071)
          {
            this.DetachHeldObject();
            return;
          }
        }
        else if ((double) Vector3.Distance(position1, position2) < 0.200000002980232)
        {
          this.m_timeAwayFromTarget = 0.0f;
        }
        else
        {
          this.m_timeAwayFromTarget += Time.deltaTime;
          if ((double) this.m_timeAwayFromTarget > 1.0)
          {
            this.HeldObject.O.RootRigidbody.position = position1;
            this.HeldObject.O.RootRigidbody.rotation = rotation1;
          }
        }
        Vector3 vector3_1 = position2;
        Quaternion rotation3 = rotation2;
        Vector3 vector3_2 = position1;
        Quaternion quaternion1 = rotation1;
        Vector3 vector3_3 = vector3_2 - vector3_1;
        Quaternion quaternion2 = quaternion1 * Quaternion.Inverse(rotation3);
        float deltaTime = Time.deltaTime;
        float angle;
        Vector3 axis;
        quaternion2.ToAngleAxis(out angle, out axis);
        float num = 1f;
        if ((double) angle > 180.0)
          angle -= 360f;
        if ((double) angle != 0.0)
          this.HeldObject.O.RootRigidbody.angularVelocity = Vector3.MoveTowards(this.HeldObject.O.RootRigidbody.angularVelocity, deltaTime * angle * axis * this.I.S.AttachedRotationMultiplier * num, this.I.S.AttachedRotationFudge * 0.5f * Time.fixedDeltaTime);
        this.HeldObject.O.RootRigidbody.velocity = Vector3.MoveTowards(this.HeldObject.O.RootRigidbody.velocity, vector3_3 * this.I.S.AttachedPositionMultiplier * 0.5f * deltaTime, this.I.S.AttachedPositionFudge * 0.5f * deltaTime);
      }
    }
  }
}
