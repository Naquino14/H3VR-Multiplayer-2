using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class Derringer : FVRFireArm
	{
		[Serializable]
		public class DBarrel
		{
			public Transform MuzzlePoint;

			public FVRFireArmChamber Chamber;
		}

		[Header("Derringer Params")]
		public List<DBarrel> Barrels;

		private int m_curBarrel;

		public Transform Hinge;

		public Axis Hinge_Axis;

		public InterpStyle Hinge_InterpStyle;

		public Vector2 HingeValues;

		private bool m_isHingeLatched = true;

		public bool DoesAutoEjectRounds;

		public bool HasLatchPiece;

		public Transform LatchPiece;

		public Axis Latch_Axis;

		public InterpStyle Latch_InterpStyle;

		public Vector2 LatchValues;

		public bool HasExternalHammer;

		private bool m_isExternalHammerCocked;

		public Transform ExternalHammer;

		public Axis ExternalHammer_Axis;

		public InterpStyle ExternalHammer_InterpStyle;

		public Vector2 ExternalHammer_Values;

		public Transform Trigger;

		public Axis Trigger_Axis;

		public InterpStyle Trigger_InterpStyle;

		public Vector2 Trigger_Values;

		public bool IsTriggerDoubleAction;

		private bool m_hasTriggerReset;

		public bool DeletesCartridgeOnFire;

		private float triggerFloat;

		public override Transform GetMuzzle()
		{
			return Barrels[m_curBarrel].MuzzlePoint;
		}

		private void CockHammer()
		{
			if (HasExternalHammer && !m_isExternalHammerCocked)
			{
				m_isExternalHammerCocked = true;
				SetAnimatedComponent(ExternalHammer, ExternalHammer_Values.y, ExternalHammer_InterpStyle, ExternalHammer_Axis);
				PlayAudioEvent(FirearmAudioEventType.Prefire);
				m_curBarrel++;
				if (m_curBarrel >= Barrels.Count)
				{
					m_curBarrel = 0;
				}
			}
		}

		private void DropHammer()
		{
			bool flag = false;
			if (HasExternalHammer)
			{
				if (m_isExternalHammerCocked)
				{
					m_isExternalHammerCocked = false;
					SetAnimatedComponent(ExternalHammer, ExternalHammer_Values.x, ExternalHammer_InterpStyle, ExternalHammer_Axis);
					PlayAudioEvent(FirearmAudioEventType.HammerHit);
					flag = true;
				}
			}
			else
			{
				PlayAudioEvent(FirearmAudioEventType.HammerHit);
				flag = true;
			}
			if (flag)
			{
				FireBarrel(m_curBarrel);
			}
		}

		private void Unlatch()
		{
			if (!m_isHingeLatched)
			{
				return;
			}
			m_isHingeLatched = false;
			if (HasLatchPiece)
			{
				SetAnimatedComponent(LatchPiece, LatchValues.y, Latch_InterpStyle, Latch_Axis);
			}
			SetAnimatedComponent(Hinge, HingeValues.y, Hinge_InterpStyle, Hinge_Axis);
			PlayAudioEvent(FirearmAudioEventType.BreachOpen);
			for (int i = 0; i < Barrels.Count; i++)
			{
				Barrels[i].Chamber.IsAccessible = true;
			}
			if (DoesAutoEjectRounds)
			{
				for (int j = 0; j < Barrels.Count; j++)
				{
					FVRFireArmChamber chamber = Barrels[j].Chamber;
					chamber.EjectRound(chamber.transform.position + chamber.transform.forward * -0.06f, chamber.transform.forward * -0.3f, Vector3.right);
				}
			}
		}

		private void Latch()
		{
			if (!m_isHingeLatched)
			{
				m_isHingeLatched = true;
				if (HasLatchPiece)
				{
					SetAnimatedComponent(LatchPiece, LatchValues.x, Latch_InterpStyle, Latch_Axis);
				}
				SetAnimatedComponent(Hinge, HingeValues.x, Hinge_InterpStyle, Hinge_Axis);
				PlayAudioEvent(FirearmAudioEventType.BreachClose);
				for (int i = 0; i < Barrels.Count; i++)
				{
					Barrels[i].Chamber.IsAccessible = false;
				}
			}
		}

		private void ToggleLatchState()
		{
			if (m_isHingeLatched)
			{
				Unlatch();
			}
			else
			{
				Latch();
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			float num = 0f;
			switch (Hinge_Axis)
			{
			case Axis.X:
				num = base.transform.InverseTransformDirection(hand.Input.VelAngularWorld).x;
				break;
			case Axis.Y:
				num = base.transform.InverseTransformDirection(hand.Input.VelAngularWorld).y;
				break;
			case Axis.Z:
				num = base.transform.InverseTransformDirection(hand.Input.VelAngularWorld).z;
				break;
			}
			if (num < -15f)
			{
				Latch();
			}
			if (!IsAltHeld)
			{
				if (hand.IsInStreamlinedMode)
				{
					if (hand.Input.BYButtonDown)
					{
						ToggleLatchState();
					}
					if (hand.Input.AXButtonDown)
					{
						CockHammer();
					}
				}
				else if (hand.Input.TouchpadDown)
				{
					if (hand.Input.TouchpadSouthPressed)
					{
						CockHammer();
					}
					if (hand.Input.TouchpadWestPressed)
					{
						ToggleLatchState();
					}
				}
			}
			if (m_hasTriggeredUpSinceBegin && !IsAltHeld)
			{
				triggerFloat = hand.Input.TriggerFloat;
			}
			else
			{
				triggerFloat = 0f;
			}
			SetAnimatedComponent(Trigger, Mathf.Lerp(Trigger_Values.x, Trigger_Values.y, triggerFloat), Trigger_InterpStyle, Trigger_Axis);
			if (triggerFloat > 0.7f && m_hasTriggerReset)
			{
				m_hasTriggerReset = false;
				DropHammer();
			}
			else
			{
				if (!(triggerFloat < 0.2f) || !m_hasTriggeredUpSinceBegin || m_hasTriggerReset)
				{
					return;
				}
				m_hasTriggerReset = true;
				if (IsTriggerDoubleAction)
				{
					m_curBarrel++;
					if (m_curBarrel >= Barrels.Count)
					{
						m_curBarrel = 0;
					}
				}
				PlayAudioEvent(FirearmAudioEventType.TriggerReset);
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			triggerFloat = 0f;
			SetAnimatedComponent(Trigger, 0f, Trigger_InterpStyle, Trigger_Axis);
		}

		private void FireBarrel(int i)
		{
			if (!m_isHingeLatched)
			{
				return;
			}
			FVRFireArmChamber chamber = Barrels[m_curBarrel].Chamber;
			if (chamber.Fire())
			{
				base.Fire(chamber, GetMuzzle(), doBuzz: true);
				FireMuzzleSmoke();
				bool twoHandStabilized = IsTwoHandStabilized();
				bool foregripStabilized = base.AltGrip != null;
				bool shoulderStabilized = IsShoulderStabilized();
				Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
				PlayAudioGunShot(chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
				if (GM.CurrentSceneSettings.IsAmmoInfinite || GM.CurrentPlayerBody.IsInfiniteAmmo)
				{
					chamber.IsSpent = false;
					chamber.UpdateProxyDisplay();
				}
				else if (chamber.GetRound().IsCaseless)
				{
					chamber.SetRound(null);
				}
				if (DeletesCartridgeOnFire)
				{
					chamber.SetRound(null);
				}
			}
		}
	}
}
