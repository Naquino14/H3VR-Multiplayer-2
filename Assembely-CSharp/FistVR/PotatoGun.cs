using UnityEngine;

namespace FistVR
{
	public class PotatoGun : FVRFireArm
	{
		[Header("PatootGun Config")]
		public FVRFireArmChamber Chamber;

		public Transform Trigger;

		public Vector2 TriggerRange;

		private float m_chamberGas;

		private bool m_hasTriggerReset;

		private float m_triggerfloat;

		public ParticleSystem PSystem_Sparks;

		public ParticleSystem PSystem_Backblast;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (!IsAltHeld && hand.Input.TriggerDown && m_hasTriggeredUpSinceBegin && m_hasTriggerReset)
			{
				PlayAudioEvent(FirearmAudioEventType.HammerHit);
				Spark();
			}
			float num = 0f;
			num = ((!m_hasTriggeredUpSinceBegin) ? 0f : hand.Input.TriggerFloat);
			if (num != m_triggerfloat)
			{
				m_triggerfloat = num;
				SetAnimatedComponent(Trigger, Mathf.Lerp(TriggerRange.x, TriggerRange.y, m_triggerfloat), InterpStyle.Translate, Axis.Z);
			}
			if (num < 0.4f && !m_hasTriggerReset)
			{
				m_hasTriggerReset = true;
				PlayAudioEvent(FirearmAudioEventType.TriggerReset);
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			m_triggerfloat = 0f;
			SetAnimatedComponent(Trigger, Mathf.Lerp(TriggerRange.x, TriggerRange.y, 0f), InterpStyle.Translate, Axis.Z);
		}

		public void InsertGas(float f)
		{
			if (!(Magazine != null))
			{
				m_chamberGas += f;
				m_chamberGas = Mathf.Clamp(m_chamberGas, 0f, 1.5f);
			}
		}

		private void Spark()
		{
			PSystem_Sparks.Emit(5);
			m_hasTriggerReset = false;
			if (m_chamberGas > 0f)
			{
				if (Magazine == null)
				{
					PSystem_Backblast.Emit(Mathf.RoundToInt(m_chamberGas * 10f));
					m_chamberGas = 0f;
					PlayAudioGunShot(IsHighPressure: false, FVRTailSoundClass.Launcher, FVRTailSoundClass.SuppressedSmall, GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
					FireMuzzleSmoke();
				}
				else
				{
					Fire();
				}
			}
		}

		public void Fire()
		{
			if (Chamber.IsFull && !Chamber.IsSpent)
			{
				Chamber.Fire();
				base.Fire(Chamber, GetMuzzle(), doBuzz: true, m_chamberGas);
				FireMuzzleSmoke();
				bool twoHandStabilized = IsTwoHandStabilized();
				bool foregripStabilized = base.AltGrip != null;
				bool shoulderStabilized = IsShoulderStabilized();
				Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
				PlayAudioGunShot(IsHighPressure: true, Chamber.GetRound().TailClass, Chamber.GetRound().TailClassSuppressed, GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
				if (GM.CurrentSceneSettings.IsAmmoInfinite || GM.CurrentPlayerBody.IsInfiniteAmmo)
				{
					Chamber.IsSpent = false;
					Chamber.UpdateProxyDisplay();
				}
				else if (Chamber.GetRound().IsCaseless)
				{
					Chamber.SetRound(null);
				}
			}
			else
			{
				PlayAudioGunShot(IsHighPressure: false, FVRTailSoundClass.Launcher, FVRTailSoundClass.SuppressedSmall, GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
				FireMuzzleSmoke();
			}
			m_chamberGas = 0f;
		}
	}
}
