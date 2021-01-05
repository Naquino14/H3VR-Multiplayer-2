using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class BreakActionWeapon : FVRFireArm
	{
		[Serializable]
		public class BreakActionBarrel
		{
			public FVRFireArmChamber Chamber;

			public Transform Hammer;

			public Transform Muzzle;

			public bool HasVisibleHammer;

			public InterpStyle HammerInterpStyle = InterpStyle.Rotation;

			public Axis HammerAxis;

			public float HammerUncocked;

			public float HammerCocked;

			public int MuzzleIndexBarrelFire;

			public int MuzzleIndexBarrelSmoke;

			public int GasOutIndexBarrel;

			public int GasOutIndexBreach;

			[HideInInspector]
			public bool IsBreachOpenForGasOut;

			[HideInInspector]
			public bool m_isHammerCocked;
		}

		[Header("Component Connections")]
		public BreakActionBarrel[] Barrels;

		public Transform[] Triggers;

		public InterpStyle TriggerInterpStyle = InterpStyle.Rotation;

		public Axis TriggerAxis;

		public float TriggerUnpulled;

		public float TriggerPulled;

		public bool FireAllBarrels;

		[Header("Hinge Params")]
		public HingeJoint Hinge;

		private Vector3 m_foreStartPos;

		[Header("Latch Params")]
		public bool m_hasLatch;

		public Transform Latch;

		public float MaxRotExtent;

		private float m_latchRot;

		private bool m_isLatched = true;

		[Header("Control Params")]
		public bool UsesManuallyCockedHammers;

		private int m_curBarrel;

		[HideInInspector]
		public bool IsLatchHeldOpen;

		[HideInInspector]
		public bool HasTriggerReset = true;

		private float m_triggerFloat;

		public bool IsLatched => m_isLatched;

		protected override void Awake()
		{
			base.Awake();
			m_foreStartPos = Hinge.transform.localPosition;
		}

		public override Transform GetMuzzle()
		{
			return Barrels[m_curBarrel].Muzzle;
		}

		public void Unlatch()
		{
		}

		public void PopOutEmpties()
		{
			for (int i = 0; i < Barrels.Length; i++)
			{
				if (Barrels[i].Chamber.IsFull && Barrels[i].Chamber.IsSpent)
				{
					PopOutRound(Barrels[i].Chamber);
				}
			}
		}

		public void PopOutRound(FVRFireArmChamber chamber)
		{
			PlayAudioEvent(FirearmAudioEventType.MagazineEjectRound);
			chamber.EjectRound(chamber.transform.position + chamber.transform.forward * -0.06f, chamber.transform.forward * -2.5f, Vector3.right);
		}

		public void CockHammer()
		{
			for (int i = 0; i < Barrels.Length; i++)
			{
				if (!Barrels[i].m_isHammerCocked)
				{
					Barrels[i].m_isHammerCocked = true;
					PlayAudioEvent(FirearmAudioEventType.Prefire);
					break;
				}
			}
			UpdateVisualHammers();
		}

		public void CockAllHammers()
		{
			bool flag = false;
			for (int i = 0; i < Barrels.Length; i++)
			{
				if (!Barrels[i].m_isHammerCocked)
				{
					Barrels[i].m_isHammerCocked = true;
					flag = true;
				}
			}
			if (flag)
			{
				PlayAudioEvent(FirearmAudioEventType.Prefire);
				UpdateVisualHammers();
			}
		}

		public void DropHammer()
		{
			if (!m_isLatched)
			{
				return;
			}
			for (int i = 0; i < Barrels.Length; i++)
			{
				if (Barrels[i].m_isHammerCocked)
				{
					PlayAudioEvent(FirearmAudioEventType.HammerHit);
					Barrels[i].m_isHammerCocked = false;
					UpdateVisualHammers();
					Fire(i);
					if (!FireAllBarrels)
					{
						break;
					}
				}
			}
		}

		public bool Fire(int b)
		{
			m_curBarrel = b;
			if (!Barrels[b].Chamber.Fire())
			{
				return false;
			}
			Fire(Barrels[b].Chamber, GetMuzzle(), doBuzz: true);
			FireMuzzleSmoke(Barrels[b].MuzzleIndexBarrelFire);
			FireMuzzleSmoke(Barrels[b].MuzzleIndexBarrelSmoke);
			AddGas(Barrels[b].GasOutIndexBarrel);
			AddGas(Barrels[b].GasOutIndexBreach);
			bool twoHandStabilized = IsTwoHandStabilized();
			bool foregripStabilized = IsForegripStabilized();
			bool shoulderStabilized = IsShoulderStabilized();
			Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
			PlayAudioGunShot(Barrels[b].Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
			if (GM.CurrentSceneSettings.IsAmmoInfinite || GM.CurrentPlayerBody.IsInfiniteAmmo)
			{
				Barrels[b].Chamber.IsSpent = false;
				Barrels[b].Chamber.UpdateProxyDisplay();
			}
			return true;
		}

		private void UpdateVisualHammers()
		{
			for (int i = 0; i < Barrels.Length; i++)
			{
				if (Barrels[i].HasVisibleHammer)
				{
					if (Barrels[i].m_isHammerCocked)
					{
						SetAnimatedComponent(Barrels[i].Hammer, Barrels[i].HammerCocked, Barrels[i].HammerInterpStyle, Barrels[i].HammerAxis);
					}
					else
					{
						SetAnimatedComponent(Barrels[i].Hammer, Barrels[i].HammerUncocked, Barrels[i].HammerInterpStyle, Barrels[i].HammerAxis);
					}
				}
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			UpdateInputAndAnimate(hand);
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			IsLatchHeldOpen = false;
		}

		private void UpdateInputAndAnimate(FVRViveHand hand)
		{
			IsLatchHeldOpen = false;
			if (IsAltHeld)
			{
				return;
			}
			if (m_hasTriggeredUpSinceBegin)
			{
				m_triggerFloat = hand.Input.TriggerFloat;
			}
			else
			{
				m_triggerFloat = 0f;
			}
			if (!HasTriggerReset && m_triggerFloat <= 0.45f)
			{
				HasTriggerReset = true;
				PlayAudioEvent(FirearmAudioEventType.TriggerReset);
			}
			Vector2 touchpadAxes = hand.Input.TouchpadAxes;
			if (hand.IsInStreamlinedMode)
			{
				if (hand.Input.BYButtonPressed)
				{
					IsLatchHeldOpen = true;
					m_latchRot = 1f * MaxRotExtent;
				}
				else
				{
					m_latchRot = Mathf.MoveTowards(m_latchRot, 0f, Time.deltaTime * MaxRotExtent * 3f);
				}
				if (hand.Input.AXButtonDown && UsesManuallyCockedHammers)
				{
					CockHammer();
				}
			}
			else
			{
				if (hand.Input.TouchpadPressed && touchpadAxes.y > 0.1f)
				{
					IsLatchHeldOpen = true;
					m_latchRot = touchpadAxes.y * MaxRotExtent;
				}
				else
				{
					m_latchRot = Mathf.MoveTowards(m_latchRot, 0f, Time.deltaTime * MaxRotExtent * 3f);
				}
				if (hand.Input.TouchpadDown && UsesManuallyCockedHammers && touchpadAxes.y < 0.1f)
				{
					CockHammer();
				}
			}
			if (UsesManuallyCockedHammers && base.IsHeld && m_hand.OtherHand != null)
			{
				Vector3 velLinearWorld = m_hand.OtherHand.Input.VelLinearWorld;
				float num = Vector3.Distance(m_hand.OtherHand.PalmTransform.position, Barrels[0].Hammer.position);
				if (num < 0.15f && Vector3.Angle(velLinearWorld, -base.transform.forward) < 60f && velLinearWorld.magnitude > 1f)
				{
					CockAllHammers();
				}
			}
			if (m_hasLatch)
			{
				Latch.localEulerAngles = new Vector3(0f, m_latchRot, 0f);
			}
			for (int i = 0; i < Triggers.Length; i++)
			{
				int num2 = Mathf.Clamp(m_curBarrel, 0, Triggers.Length - 1);
				if (num2 == i)
				{
					SetAnimatedComponent(Triggers[i], Mathf.Lerp(TriggerUnpulled, TriggerPulled, m_triggerFloat), TriggerInterpStyle, TriggerAxis);
				}
				else
				{
					SetAnimatedComponent(Triggers[i], TriggerUnpulled, TriggerInterpStyle, TriggerAxis);
				}
			}
			if (m_triggerFloat >= 0.9f && HasTriggerReset && m_isLatched)
			{
				HasTriggerReset = false;
				DropHammer();
			}
		}

		protected override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			for (int i = 0; i < Barrels.Length; i++)
			{
				GasOutEffects[Barrels[i].GasOutIndexBarrel].GasUpdate(BreachOpen: true);
				if (Barrels[i].Chamber.IsFull || !Barrels[i].Chamber.IsAccessible)
				{
					GasOutEffects[Barrels[i].GasOutIndexBreach].GasUpdate(BreachOpen: false);
				}
				else
				{
					GasOutEffects[Barrels[i].GasOutIndexBreach].GasUpdate(BreachOpen: true);
				}
			}
			if (m_isLatched && Mathf.Abs(m_latchRot) > 5f)
			{
				m_isLatched = false;
				PlayAudioEvent(FirearmAudioEventType.BreachOpen);
				JointLimits limits = Hinge.limits;
				limits.max = 45f;
				Hinge.limits = limits;
				for (int j = 0; j < Barrels.Length; j++)
				{
					Barrels[j].Chamber.IsAccessible = true;
				}
			}
			if (m_isLatched)
			{
				return;
			}
			if (!IsLatchHeldOpen && Hinge.transform.localEulerAngles.x <= 1f && Mathf.Abs(m_latchRot) < 5f)
			{
				m_isLatched = true;
				PlayAudioEvent(FirearmAudioEventType.BreachClose);
				JointLimits limits2 = Hinge.limits;
				limits2.max = 0f;
				Hinge.limits = limits2;
				for (int k = 0; k < Barrels.Length; k++)
				{
					Barrels[k].Chamber.IsAccessible = false;
				}
				Hinge.transform.localPosition = m_foreStartPos;
			}
			if (Mathf.Abs(Hinge.transform.localEulerAngles.x) >= 30f)
			{
				PopOutEmpties();
				if (!UsesManuallyCockedHammers)
				{
					CockAllHammers();
				}
			}
		}

		public override List<FireArmRoundClass> GetChamberRoundList()
		{
			bool flag = false;
			List<FireArmRoundClass> list = new List<FireArmRoundClass>();
			for (int i = 0; i < Barrels.Length; i++)
			{
				if (Barrels[i].Chamber.IsFull)
				{
					list.Add(Barrels[i].Chamber.GetRound().RoundClass);
					flag = true;
				}
			}
			if (flag)
			{
				return list;
			}
			return null;
		}

		public override void SetLoadedChambers(List<FireArmRoundClass> rounds)
		{
			if (rounds.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < Barrels.Length; i++)
			{
				if (i < rounds.Count)
				{
					Barrels[i].Chamber.Autochamber(rounds[i]);
				}
			}
		}

		public override List<string> GetFlagList()
		{
			return null;
		}

		public override void SetFromFlagList(List<string> flags)
		{
		}

		public override void ConfigureFromFlagDic(Dictionary<string, string> f)
		{
			for (int i = 0; i < Barrels.Length; i++)
			{
				string key = "HammerState_" + i;
				if (f.ContainsKey(key))
				{
					string text = f[key];
					if (text == "Cocked")
					{
						Barrels[i].m_isHammerCocked = true;
						UpdateVisualHammers();
					}
				}
			}
		}

		public override Dictionary<string, string> GetFlagDic()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			for (int i = 0; i < Barrels.Length; i++)
			{
				string key = "HammerState_" + i;
				string value = "Uncocked";
				if (Barrels[i].m_isHammerCocked)
				{
					value = "Cocked";
				}
				dictionary.Add(key, value);
			}
			return dictionary;
		}
	}
}
