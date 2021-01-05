using UnityEngine;

namespace FistVR
{
	public class RGM40 : FVRFireArm
	{
		[Header("RGM Stuff")]
		public FVRFireArmChamber Chamber;

		public Transform Trigger;

		public Vector2 TriggerRange;

		public Transform Ejector;

		public Vector2 EjectorRange = new Vector2(0f, 0.005f);

		private bool m_isEjectorForward = true;

		public Transform EjectPos;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			if (m_hasTriggeredUpSinceBegin)
			{
				SetAnimatedComponent(Trigger, Mathf.Lerp(TriggerRange.x, TriggerRange.y, hand.Input.TriggerFloat), InterpStyle.Translate, Axis.Z);
				if (hand.Input.TriggerDown)
				{
					Fire();
				}
			}
			base.UpdateInteraction(hand);
		}

		public void Fire()
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
				if (Chamber.GetRound().IsCaseless)
				{
					Chamber.SetRound(null);
				}
			}
		}

		public void SafeEject()
		{
			Chamber.EjectRound(EjectPos.position, EjectPos.forward, Vector3.zero, ForceCaseLessEject: true);
			PlayAudioEvent(FirearmAudioEventType.MagazineEjectRound);
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (m_isEjectorForward)
			{
				if (Chamber.IsFull)
				{
					m_isEjectorForward = false;
					SetAnimatedComponent(Ejector, EjectorRange.x, InterpStyle.Translate, Axis.Z);
				}
			}
			else if (!Chamber.IsFull)
			{
				m_isEjectorForward = true;
				SetAnimatedComponent(Ejector, EjectorRange.y, InterpStyle.Translate, Axis.Z);
			}
		}
	}
}
