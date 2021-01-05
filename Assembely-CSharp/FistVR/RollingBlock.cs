using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class RollingBlock : FVRFireArm
	{
		public enum RollingBlockState
		{
			HammerForward,
			HammerBackBreachClosed,
			HammerBackBreachOpen
		}

		[Header("RollingBlock Params")]
		public FVRFireArmChamber Chamber;

		public Transform Trigger;

		public Vector2 TriggerRots;

		public Transform Hammer;

		public Vector2 HammerRots;

		private float m_curHammerRot;

		private float m_tarHammerRot;

		public Transform BreachBlock;

		public Vector2 BreachBlockRots;

		private float m_curBreachRot;

		private float m_tarBreachRot;

		public Transform EjectPos;

		private RollingBlockState m_state;

		private Vector2 m_pressDownPoint = Vector2.zero;

		private bool m_isPressedDown;

		protected override void Awake()
		{
			base.Awake();
			IsBreachOpenForGasOut = false;
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			m_curHammerRot = Mathf.Lerp(m_curHammerRot, m_tarHammerRot, Time.deltaTime * 8f);
			m_curBreachRot = Mathf.Lerp(m_curBreachRot, m_tarBreachRot, Time.deltaTime * 12f);
			Hammer.localEulerAngles = new Vector3(m_curHammerRot, 0f, 0f);
			BreachBlock.localEulerAngles = new Vector3(m_curBreachRot, 0f, 0f);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector2 touchpadAxes = hand.Input.TouchpadAxes;
			if (hand.IsInStreamlinedMode)
			{
				if (hand.Input.BYButtonDown)
				{
					ClickUpward();
				}
				if (hand.Input.AXButtonDown)
				{
					ClickDownward();
				}
			}
			else if (hand.Input.TouchpadDown)
			{
				if (touchpadAxes.y > 0f)
				{
					ClickUpward();
				}
				else
				{
					ClickDownward();
				}
			}
			float triggerFloat = hand.Input.TriggerFloat;
			if (m_state == RollingBlockState.HammerBackBreachClosed && hand.Input.TriggerDown && m_hasTriggeredUpSinceBegin && m_state != 0)
			{
				m_isPressedDown = false;
				DropHammer();
				m_state = RollingBlockState.HammerForward;
			}
		}

		private void ClickUpward()
		{
			switch (m_state)
			{
			case RollingBlockState.HammerBackBreachClosed:
				m_state = RollingBlockState.HammerForward;
				DecockHammer();
				break;
			case RollingBlockState.HammerBackBreachOpen:
				m_state = RollingBlockState.HammerBackBreachClosed;
				CloseBreach();
				break;
			}
		}

		private void ClickDownward()
		{
			switch (m_state)
			{
			case RollingBlockState.HammerForward:
				CockHammer();
				m_state = RollingBlockState.HammerBackBreachClosed;
				break;
			case RollingBlockState.HammerBackBreachClosed:
				OpenBreach();
				m_state = RollingBlockState.HammerBackBreachOpen;
				break;
			}
		}

		private void CockHammer()
		{
			m_tarHammerRot = HammerRots.y;
			PlayAudioEvent(FirearmAudioEventType.Prefire);
		}

		private void DecockHammer()
		{
			m_tarHammerRot = HammerRots.x;
			PlayAudioEvent(FirearmAudioEventType.BreachClose);
		}

		private void OpenBreach()
		{
			m_tarBreachRot = BreachBlockRots.y;
			PlayAudioEvent(FirearmAudioEventType.BreachOpen);
			if (Chamber.IsFull)
			{
				Chamber.EjectRound(EjectPos.position, base.transform.forward * -1f + base.transform.up * 0.5f, Vector3.right * 270f);
			}
			IsBreachOpenForGasOut = true;
			Chamber.IsAccessible = true;
		}

		private void CloseBreach()
		{
			m_tarBreachRot = BreachBlockRots.x;
			PlayAudioEvent(FirearmAudioEventType.BreachClose);
			IsBreachOpenForGasOut = false;
			Chamber.IsAccessible = false;
		}

		private void DropHammer()
		{
			m_tarHammerRot = HammerRots.x;
			m_curHammerRot = HammerRots.x;
			PlayAudioEvent(FirearmAudioEventType.HammerHit);
			if (Chamber.IsFull)
			{
				Fire();
			}
		}

		private void Fire()
		{
			if (Chamber.Fire())
			{
				base.Fire(Chamber, GetMuzzle(), doBuzz: true);
				FireMuzzleSmoke();
				bool twoHandStabilized = IsTwoHandStabilized();
				bool foregripStabilized = base.AltGrip != null;
				bool shoulderStabilized = IsShoulderStabilized();
				Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
				PlayAudioGunShot(Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
			}
		}

		public override List<FireArmRoundClass> GetChamberRoundList()
		{
			if (Chamber.IsFull && !Chamber.IsSpent)
			{
				List<FireArmRoundClass> list = new List<FireArmRoundClass>();
				list.Add(Chamber.GetRound().RoundClass);
				return list;
			}
			return null;
		}

		public override void SetLoadedChambers(List<FireArmRoundClass> rounds)
		{
			if (rounds.Count > 0)
			{
				Chamber.Autochamber(rounds[0]);
			}
		}

		public override List<string> GetFlagList()
		{
			return null;
		}

		public override void SetFromFlagList(List<string> flags)
		{
		}
	}
}
