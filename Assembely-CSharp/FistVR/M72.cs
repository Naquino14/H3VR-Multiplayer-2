using UnityEngine;

namespace FistVR
{
	public class M72 : FVRFireArm
	{
		public enum TubeState
		{
			Forward,
			Mid,
			Rear
		}

		[Header("M72 Params")]
		public FVRFireArmChamber Chamber;

		public Transform Point_BackBlast;

		public bool AlsoPlaysSuppressedSound = true;

		public bool DeletesCartridgeOnFire = true;

		[Header("Trigger Params")]
		public Transform Trigger;

		public Axis Trigger_Axis;

		public InterpStyle Trigger_Interp;

		public Vector2 Trigger_ValRange;

		[Header("Cap Params")]
		public Transform Cap;

		public Axis Cap_Axis;

		public InterpStyle Cap_Interp;

		public Vector2 Cap_ValRange;

		[Header("Tube Params")]
		public Transform Tube;

		public Transform Tube_Front;

		public Transform Tube_Rear;

		[Header("Safety Params")]
		public Transform Safety;

		public Axis Safety_Axis;

		public InterpStyle Safety_Interp;

		public Vector2 Safety_ValRange;

		[Header("RearSight Params")]
		public Transform RearSight;

		public Axis RearSight_Axis;

		public InterpStyle RearSight_Interp;

		public Vector2 RearSight_ValRange;

		private bool m_isSafetyEngaged = true;

		private bool m_isCapOpen;

		public TubeState TState;

		private float m_triggerVal;

		public bool CanCapBeToggled()
		{
			if (TState == TubeState.Forward)
			{
				return true;
			}
			return false;
		}

		public bool CanTubeBeGrabbed()
		{
			return m_isCapOpen;
		}

		public void ToggleSafety()
		{
			m_isSafetyEngaged = !m_isSafetyEngaged;
			if (m_isSafetyEngaged)
			{
				SetAnimatedComponent(Safety, Safety_ValRange.x, Safety_Interp, Safety_Axis);
			}
			else
			{
				SetAnimatedComponent(Safety, Safety_ValRange.y, Safety_Interp, Safety_Axis);
			}
			PlayAudioEvent(FirearmAudioEventType.FireSelector);
		}

		public void PopUpRearSight()
		{
			SetAnimatedComponent(RearSight, RearSight_ValRange.y, RearSight_Interp, RearSight_Axis);
		}

		public void ToggleCap()
		{
			if (CanCapBeToggled())
			{
				m_isCapOpen = !m_isCapOpen;
			}
			if (m_isCapOpen)
			{
				SetAnimatedComponent(Cap, Cap_ValRange.y, Cap_Interp, Cap_Axis);
				PlayAudioEvent(FirearmAudioEventType.BreachOpen);
			}
			else
			{
				SetAnimatedComponent(Cap, Cap_ValRange.x, Cap_Interp, Cap_Axis);
				PlayAudioEvent(FirearmAudioEventType.BreachClose);
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (!IsAltHeld && hand.Input.TriggerDown && m_hasTriggeredUpSinceBegin && !m_isSafetyEngaged && TState == TubeState.Rear)
			{
				Fire();
			}
			if (m_triggerVal != hand.Input.TriggerFloat)
			{
				m_triggerVal = hand.Input.TriggerFloat;
				SetAnimatedComponent(Trigger, Mathf.Lerp(Trigger_ValRange.x, Trigger_ValRange.y, m_triggerVal), Trigger_Interp, Trigger_Axis);
			}
		}

		public void Fire()
		{
			if (Chamber.IsFull && !Chamber.IsSpent)
			{
				Chamber.Fire();
				base.Fire(Chamber, GetMuzzle(), doBuzz: true);
				FireMuzzleSmoke();
				bool twoHandStabilized = IsTwoHandStabilized();
				bool foregripStabilized = base.AltGrip != null;
				bool shoulderStabilized = IsShoulderStabilized();
				Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
				PlayAudioGunShot(Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
				if (AlsoPlaysSuppressedSound)
				{
					PlayAudioEvent(FirearmAudioEventType.Shots_Suppressed);
				}
				if (GM.CurrentSceneSettings.IsAmmoInfinite || GM.CurrentPlayerBody.IsInfiniteAmmo)
				{
					Chamber.IsSpent = false;
					Chamber.UpdateProxyDisplay();
				}
				else if (DeletesCartridgeOnFire)
				{
					Chamber.SetRound(null);
				}
			}
		}
	}
}
