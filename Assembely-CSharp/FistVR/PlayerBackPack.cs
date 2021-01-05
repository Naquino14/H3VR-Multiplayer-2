using System.Collections;
using UnityEngine;

namespace FistVR
{
	public class PlayerBackPack : FVRPhysicalObject
	{
		[Header("Backpack Stuff")]
		public FVRQuickBeltSlot[] Slots;

		private bool m_isUsable = true;

		private new void Start()
		{
			for (int i = 0; i < Slots.Length; i++)
			{
				Slots[i].IsPlayer = false;
			}
			StartCoroutine(DelayedRegister());
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (base.QuickbeltSlot != null)
			{
				if (m_isHardnessed && base.IsHeld)
				{
					SetUsable(b: false);
				}
				else
				{
					SetUsable(b: true);
				}
				return;
			}
			Vector3 from = base.transform.position - GM.CurrentPlayerBody.Head.position;
			float num = Vector3.Angle(from, GM.CurrentPlayerBody.Head.forward);
			if (num > 80f)
			{
				SetUsable(b: false);
			}
			else
			{
				SetUsable(b: true);
			}
		}

		public IEnumerator DelayedRegister()
		{
			yield return new WaitForSeconds(0.5f);
			RegisterQuickbeltSlots();
		}

		public void RegisterQuickbeltSlots()
		{
			for (int i = 0; i < Slots.Length; i++)
			{
				if (!GM.CurrentPlayerBody.QuickbeltSlots.Contains(Slots[i]))
				{
					GM.CurrentPlayerBody.QuickbeltSlots.Add(Slots[i]);
				}
			}
		}

		public void DeRegisterQuickbeltSlots()
		{
			for (int i = 0; i < Slots.Length; i++)
			{
				if (GM.CurrentPlayerBody.QuickbeltSlots.Contains(Slots[i]))
				{
					GM.CurrentPlayerBody.QuickbeltSlots.Remove(Slots[i]);
				}
			}
		}

		private new void OnDestroy()
		{
			if (GM.CurrentPlayerBody != null)
			{
				DeRegisterQuickbeltSlots();
			}
		}

		private void SetUsable(bool b)
		{
			if (b == m_isUsable)
			{
				return;
			}
			m_isUsable = b;
			if (m_isUsable)
			{
				for (int i = 0; i < Slots.Length; i++)
				{
					Slots[i].IsSelectable = true;
				}
			}
			else
			{
				for (int j = 0; j < Slots.Length; j++)
				{
					Slots[j].IsSelectable = false;
				}
			}
		}
	}
}
