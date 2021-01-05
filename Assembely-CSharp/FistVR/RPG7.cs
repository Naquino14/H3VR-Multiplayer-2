using UnityEngine;

namespace FistVR
{
	public class RPG7 : FVRFireArm
	{
		[Header("RPG-7 Config")]
		public RPG7Foregrip RPGForegrip;

		public FVRFireArmChamber Chamber;

		public Transform Trigger;

		public Vector2 TriggerRots;

		public Transform Hammer;

		public Vector2 HammerRots;

		private bool m_isHammerCocked;

		public override int GetTutorialState()
		{
			if (!Chamber.IsFull)
			{
				return 0;
			}
			if (base.AltGrip == null)
			{
				return 1;
			}
			if (!m_isHammerCocked)
			{
				return 2;
			}
			return 3;
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (IsAltHeld)
			{
				bool flag = false;
				if (hand.IsInStreamlinedMode && hand.Input.AXButtonDown)
				{
					flag = true;
				}
				else if (!hand.IsInStreamlinedMode && hand.Input.TouchpadDown)
				{
					flag = true;
				}
				if (flag)
				{
					CockHammer();
				}
				if (hand.Input.TriggerDown)
				{
					Fire();
				}
				float triggerFloat = hand.Input.TriggerFloat;
				UpdateTriggerRot(triggerFloat);
			}
		}

		public void UpdateTriggerRot(float f)
		{
			Trigger.localEulerAngles = new Vector3(Mathf.Lerp(TriggerRots.x, TriggerRots.y, f), 0f, 0f);
		}

		public void CockHammer()
		{
			if (!m_isHammerCocked)
			{
				PlayAudioEvent(FirearmAudioEventType.Prefire);
				m_isHammerCocked = true;
				Hammer.localEulerAngles = new Vector3(HammerRots.y, 0f, 0f);
			}
		}

		public void Fire()
		{
			if (m_isHammerCocked)
			{
				m_isHammerCocked = false;
				PlayAudioEvent(FirearmAudioEventType.HammerHit);
				Hammer.localEulerAngles = new Vector3(HammerRots.x, 0f, 0f);
				if (Chamber.IsFull && !Chamber.IsSpent)
				{
					base.Fire(Chamber, GetMuzzle(), doBuzz: true);
					FireMuzzleSmoke();
					bool twoHandStabilized = IsTwoHandStabilized();
					bool foregripStabilized = base.AltGrip != null;
					bool shoulderStabilized = IsShoulderStabilized();
					Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
					PlayAudioGunShot(Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
					PlayAudioEvent(FirearmAudioEventType.Shots_Suppressed);
					Chamber.SetRound(null);
				}
			}
		}
	}
}
