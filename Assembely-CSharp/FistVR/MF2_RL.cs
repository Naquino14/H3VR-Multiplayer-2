using UnityEngine;

namespace FistVR
{
	public class MF2_RL : FVRFireArm
	{
		[Header("Rocket Launcher Config")]
		public FVRFireArmChamber Chamber;

		private float m_refireLimit;

		public Transform Trigger;

		public Vector2 TriggerRange;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (!IsAltHeld && hand.Input.TriggerDown && m_hasTriggeredUpSinceBegin && m_refireLimit <= 0f)
			{
				Fire();
			}
			SetAnimatedComponent(Trigger, Mathf.Lerp(TriggerRange.x, TriggerRange.y, hand.Input.TriggerFloat), InterpStyle.Rotation, Axis.X);
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			SetAnimatedComponent(Trigger, TriggerRange.x, InterpStyle.Rotation, Axis.X);
			base.EndInteraction(hand);
		}

		protected override void FVRUpdate()
		{
			if (m_refireLimit > 0f)
			{
				m_refireLimit -= Time.deltaTime;
			}
			if (!Chamber.IsFull && Magazine.HasARound())
			{
				GameObject gameObject = Magazine.RemoveRound(b: false);
				Chamber.SetRound(gameObject.GetComponent<FVRFireArmRound>());
			}
			base.FVRUpdate();
		}

		public void Fire()
		{
			m_refireLimit = 0.8f;
			if (Chamber.IsFull && !Chamber.IsSpent)
			{
				Chamber.Fire();
				base.Fire(Chamber, GetMuzzle(), doBuzz: true);
				if (Chamber.GetRound().IsHighPressure)
				{
					FireMuzzleSmoke();
				}
				else
				{
					m_refireLimit = 0.25f;
				}
				bool twoHandStabilized = IsTwoHandStabilized();
				bool foregripStabilized = base.AltGrip != null;
				bool shoulderStabilized = IsShoulderStabilized();
				if (Chamber.GetRound().IsHighPressure)
				{
					Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
				}
				else
				{
					Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized, RecoilProfile, 0.2f);
				}
				PlayAudioGunShot(Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
				Chamber.SetRound(null);
			}
		}
	}
}
