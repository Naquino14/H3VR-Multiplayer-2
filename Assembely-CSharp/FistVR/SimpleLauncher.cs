using UnityEngine;

namespace FistVR
{
	public class SimpleLauncher : FVRFireArm
	{
		[Header("Simple Launcher Config")]
		public FVRFireArmChamber Chamber;

		public bool HasTrigger = true;

		public bool AlsoPlaysSuppressedSound = true;

		public bool DeletesCartridgeOnFire = true;

		public bool FireOnCol;

		public float ColThresh = 1f;

		public Collider HammerCol;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (!IsAltHeld && hand.Input.TriggerDown && m_hasTriggeredUpSinceBegin && HasTrigger)
			{
				Fire();
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

		public override void OnCollisionEnter(Collision col)
		{
			base.OnCollisionEnter(col);
			if (FireOnCol && col.relativeVelocity.magnitude > ColThresh && col.contacts[0].thisCollider == HammerCol)
			{
				Fire();
			}
		}
	}
}
