using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class SosigInventory
	{
		[Serializable]
		public class Slot
		{
			private SosigInventory m_inventory;

			public Transform Target;

			public SosigLink LinkAttachedTo;

			public SosigWeapon HeldObject;

			public bool IsHoldingObject;

			private float m_timeAwayFromTarget;

			public SosigInventory I => m_inventory;

			public void SetInventory(SosigInventory i)
			{
				m_inventory = i;
			}

			public void PlaceObjectIn(SosigWeapon o)
			{
				HeldObject = o;
				IsHoldingObject = true;
				HeldObject.IsInBotInventory = true;
				HeldObject.SosigWithInventory = I.S;
				HeldObject.InventorySlotWithThis = this;
				I.m_objectsByType[(int)o.Type]++;
				if (!I.S.IgnoreRBs.Contains(o.O.RootRigidbody))
				{
					I.S.IgnoreRBs.Add(o.O.RootRigidbody);
				}
			}

			public SosigWeapon GetObjectFromSlot()
			{
				return HeldObject;
			}

			public void DetachHeldObject()
			{
				if (IsHoldingObject)
				{
					if (HeldObject != null && I.S.IgnoreRBs.Contains(HeldObject.O.RootRigidbody))
					{
						I.S.IgnoreRBs.Remove(HeldObject.O.RootRigidbody);
					}
					I.m_objectsByType[(int)HeldObject.Type]--;
					HeldObject.SosigWithInventory = null;
					HeldObject.InventorySlotWithThis = null;
					HeldObject.IsInBotInventory = false;
					HeldObject = null;
					IsHoldingObject = false;
				}
			}

			public void PhysHold()
			{
				if (!IsHoldingObject || HeldObject == null)
				{
					return;
				}
				Vector3 position = Target.position;
				Quaternion rotation = Target.rotation;
				Vector3 position2 = HeldObject.RecoilHolder.position;
				Quaternion rotation2 = HeldObject.RecoilHolder.rotation;
				if (HeldObject.O.IsHeld)
				{
					float num = Vector3.Distance(position, position2);
					if (num > 0.7f)
					{
						DetachHeldObject();
						return;
					}
				}
				else
				{
					float num2 = Vector3.Distance(position, position2);
					if (num2 < 0.2f)
					{
						m_timeAwayFromTarget = 0f;
					}
					else
					{
						m_timeAwayFromTarget += Time.deltaTime;
						if (m_timeAwayFromTarget > 1f)
						{
							HeldObject.O.RootRigidbody.position = position;
							HeldObject.O.RootRigidbody.rotation = rotation;
						}
					}
				}
				Vector3 vector = position2;
				Quaternion rotation3 = rotation2;
				Vector3 vector2 = position;
				Quaternion quaternion = rotation;
				Vector3 vector3 = vector2 - vector;
				Quaternion quaternion2 = quaternion * Quaternion.Inverse(rotation3);
				float deltaTime = Time.deltaTime;
				quaternion2.ToAngleAxis(out var angle, out var axis);
				float num3 = 1f;
				if (angle > 180f)
				{
					angle -= 360f;
				}
				if (angle != 0f)
				{
					Vector3 target = deltaTime * angle * axis * I.S.AttachedRotationMultiplier * num3;
					HeldObject.O.RootRigidbody.angularVelocity = Vector3.MoveTowards(HeldObject.O.RootRigidbody.angularVelocity, target, I.S.AttachedRotationFudge * 0.5f * Time.fixedDeltaTime);
				}
				Vector3 target2 = vector3 * I.S.AttachedPositionMultiplier * 0.5f * deltaTime;
				HeldObject.O.RootRigidbody.velocity = Vector3.MoveTowards(HeldObject.O.RootRigidbody.velocity, target2, I.S.AttachedPositionFudge * 0.5f * deltaTime);
			}
		}

		public Sosig S;

		public List<Slot> Slots = new List<Slot>();

		private int[] m_ammoStores = new int[12];

		private int[] m_objectsByType = new int[6];

		public void Init()
		{
			for (int i = 0; i < Slots.Count; i++)
			{
				Slots[i].SetInventory(this);
			}
		}

		public void FillAllAmmo()
		{
			for (int i = 0; i < m_ammoStores.Length; i++)
			{
				m_ammoStores[i] = 200;
			}
		}

		public void FillAmmoWithType(SosigWeapon.SosiggunAmmoType t)
		{
			m_ammoStores[(int)t] = 200;
		}

		public void FillAmmoWithType(SosigWeapon.SosiggunAmmoType t, int i)
		{
			m_ammoStores[(int)t] = i;
		}

		public void PhysHold()
		{
			for (int i = 0; i < Slots.Count; i++)
			{
				Slots[i].PhysHold();
			}
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
			if (m_objectsByType[(int)w.Type] < num)
			{
				return true;
			}
			return false;
		}

		public bool HasAmmoFor(SosigWeapon w)
		{
			if (w.AmmoType == SosigWeapon.SosiggunAmmoType.None)
			{
				return true;
			}
			return HasAmmo((int)w.AmmoType);
		}

		public bool HasAmmo(int i)
		{
			if (m_ammoStores[i] > 0)
			{
				return true;
			}
			return false;
		}

		public int ReloadFromType(int i, int amount)
		{
			if (m_ammoStores[i] >= amount)
			{
				m_ammoStores[i] -= amount;
				return amount;
			}
			int result = m_ammoStores[i];
			m_ammoStores[i] = 0;
			return result;
		}

		public bool PutObjectInMe(SosigWeapon o)
		{
			if (IsThereAFreeSlot())
			{
				if (DoINeed(o))
				{
					GetFreeSlot().PlaceObjectIn(o);
				}
				return true;
			}
			return false;
		}

		public void SwapObjectFromHandToObjectInInventory(SosigWeapon fromHand, SosigWeapon fromInventory)
		{
			Slot inventorySlotWithThis = fromInventory.InventorySlotWithThis;
			inventorySlotWithThis.I.DropObjectInSlot(inventorySlotWithThis);
			SosigHand handHoldingThis = fromHand.HandHoldingThis;
			handHoldingThis.DropHeldObject();
			PutObjectInMe(fromHand);
			handHoldingThis.PickUp(fromInventory);
		}

		public void DropObjectInSlot(Slot s)
		{
			for (int i = 0; i < Slots.Count; i++)
			{
				if (Slots[i] == s)
				{
					Slots[i].DetachHeldObject();
				}
			}
		}

		public void DropAllObjects()
		{
			for (int i = 0; i < Slots.Count; i++)
			{
				Slots[i].DetachHeldObject();
			}
		}

		public bool IsThereAFreeSlot()
		{
			if (Slots.Count == 0)
			{
				return false;
			}
			bool result = false;
			for (int i = 0; i < Slots.Count; i++)
			{
				if (!Slots[i].IsHoldingObject)
				{
					result = true;
				}
			}
			return result;
		}

		public Slot GetFreeSlot()
		{
			for (int i = 0; i < Slots.Count; i++)
			{
				if (!Slots[i].IsHoldingObject)
				{
					return Slots[i];
				}
			}
			return null;
		}

		public bool DoIHaveAnyEquipment()
		{
			bool result = false;
			for (int i = 0; i < Slots.Count; i++)
			{
				if (Slots[i].IsHoldingObject)
				{
					result = true;
				}
			}
			return result;
		}

		public bool DoIHaveAnyWeaponry()
		{
			bool result = false;
			for (int i = 0; i < Slots.Count; i++)
			{
				if (Slots[i].IsHoldingObject && (Slots[i].HeldObject.Type == SosigWeapon.SosigWeaponType.Gun || Slots[i].HeldObject.Type == SosigWeapon.SosigWeaponType.Grenade || Slots[i].HeldObject.Type == SosigWeapon.SosigWeaponType.Melee))
				{
					result = true;
				}
			}
			return result;
		}

		public bool DoIHaveAnObjectOfType(SosigWeapon.SosigWeaponType type)
		{
			for (int i = 0; i < Slots.Count; i++)
			{
				if (Slots[i].IsHoldingObject && Slots[i].HeldObject.Type == type)
				{
					return true;
				}
			}
			return false;
		}

		public int GetBestItemQuality()
		{
			int num = -1;
			for (int i = 0; i < Slots.Count; i++)
			{
				if (Slots[i].IsHoldingObject)
				{
					num = Mathf.Max(num, Slots[i].HeldObject.Quality);
				}
			}
			return num;
		}

		public int GetBestItemQuality(SosigWeapon.SosigWeaponType type)
		{
			int num = -1;
			for (int i = 0; i < Slots.Count; i++)
			{
				if (Slots[i].IsHoldingObject && Slots[i].HeldObject.Type == type)
				{
					num = Mathf.Max(num, Slots[i].HeldObject.Quality);
				}
			}
			return num;
		}

		public SosigWeapon GetBestWeaponOut(SosigWeapon.SosigWeaponType type)
		{
			int a = -1;
			SosigWeapon result = null;
			for (int i = 0; i < Slots.Count; i++)
			{
				if (Slots[i].IsHoldingObject && Slots[i].HeldObject.Type == type)
				{
					a = Mathf.Max(a, Slots[i].HeldObject.Quality);
					result = Slots[i].HeldObject;
				}
			}
			return result;
		}

		public SosigWeapon GetBestGunOut()
		{
			return GetBestWeaponOut(SosigWeapon.SosigWeaponType.Gun);
		}

		public SosigWeapon GetBestMeleeWeaponOut()
		{
			return GetBestWeaponOut(SosigWeapon.SosigWeaponType.Melee);
		}

		public SosigWeapon GetBestShieldWeaponOut()
		{
			return GetBestWeaponOut(SosigWeapon.SosigWeaponType.Shield);
		}

		public SosigWeapon GetBestThrownWeaponOut()
		{
			return GetBestWeaponOut(SosigWeapon.SosigWeaponType.Grenade);
		}
	}
}
