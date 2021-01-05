using UnityEngine;

namespace FistVR
{
	public class GP25 : AttachableFirearm
	{
		[Header("GP25")]
		public FVRFireArmChamber Chamber;

		public Transform Trigger;

		public Vector2 TriggerRange;

		public Transform Safety;

		public Vector2 SafetyRange;

		public Transform Ejector;

		public Vector2 EjectorRange = new Vector2(0f, 0.005f);

		private bool m_isEjectorForward = true;

		public bool m_safetyEngaged = true;

		public Transform EjectPos;

		public bool UsesChargeUp;

		private float m_ChargeupAmount;

		private FVRPooledAudioSource m_chargeAudio;

		private int DestabilizedShots;

		public override void ProcessInput(FVRViveHand hand, bool fromInterface, FVRInteractiveObject o)
		{
			if (Attachment != null && o.m_hasTriggeredUpSinceBegin && !m_safetyEngaged)
			{
				Attachment.SetAnimatedComponent(Trigger, Mathf.Lerp(TriggerRange.x, TriggerRange.y, hand.Input.TriggerFloat), FVRPhysicalObject.InterpStyle.Translate, FVRPhysicalObject.Axis.Z);
			}
			if (UsesChargeUp && o.m_hasTriggeredUpSinceBegin)
			{
				if (hand.Input.TriggerPressed && Chamber.IsFull)
				{
					if (m_chargeAudio == null)
					{
						m_chargeAudio = OverrideFA.PlayAudioAsHandling(AudioClipSet.Prefire, MuzzlePos.position);
					}
					m_ChargeupAmount += Time.deltaTime;
					if (m_ChargeupAmount >= 1f)
					{
						m_chargeAudio.Source.Stop();
						m_chargeAudio = null;
						Fire(fromInterface);
					}
				}
				else
				{
					if (m_ChargeupAmount > 0f && m_chargeAudio != null)
					{
						m_chargeAudio.Source.Stop();
						m_chargeAudio = null;
					}
					m_ChargeupAmount = 0f;
				}
			}
			else if (hand.Input.TriggerDown && o.m_hasTriggeredUpSinceBegin && !m_safetyEngaged)
			{
				Fire(fromInterface);
			}
		}

		public void Fire(bool firedFromInterface)
		{
			if (!Chamber.Fire())
			{
				return;
			}
			FireMuzzleSmoke();
			if (firedFromInterface)
			{
				FVRFireArm fVRFireArm = ((!(Attachment != null)) ? OverrideFA : (Attachment.curMount.MyObject as FVRFireArm));
				if (fVRFireArm != null)
				{
					fVRFireArm.Recoil(fVRFireArm.IsTwoHandStabilized(), fVRFireArm.IsForegripStabilized(), fVRFireArm.IsShoulderStabilized(), RecoilProfile);
					Fire(Chamber, MuzzlePos, doBuzz: true, fVRFireArm);
				}
				else
				{
					Fire(Chamber, MuzzlePos, doBuzz: true, null);
				}
			}
			else
			{
				Recoil(twoHandStabilized: false, foregripStabilized: false, shoulderStabilized: false);
				Fire(Chamber, MuzzlePos, doBuzz: true, null);
			}
			PlayAudioGunShot(Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
			if (Chamber.GetRound().IsCaseless)
			{
				Chamber.SetRound(null);
			}
		}

		public void SafeEject()
		{
			Chamber.EjectRound(EjectPos.position, EjectPos.forward, Vector3.zero, ForceCaseLessEject: true);
			PlayAudioEvent(FirearmAudioEventType.MagazineEjectRound);
		}

		public void ToggleSafety()
		{
			m_safetyEngaged = !m_safetyEngaged;
			PlayAudioEvent(FirearmAudioEventType.Safety);
			if (Attachment != null)
			{
				if (m_safetyEngaged)
				{
					Attachment.SetAnimatedComponent(Safety, SafetyRange.y, FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
				}
				else
				{
					Attachment.SetAnimatedComponent(Safety, SafetyRange.x, FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
				}
			}
		}

		private void FixedUpdate()
		{
			if (m_ChargeupAmount > 0f)
			{
				OverrideFA.RootRigidbody.velocity += Random.onUnitSphere * 0.2f * m_ChargeupAmount * m_ChargeupAmount;
				OverrideFA.RootRigidbody.angularVelocity += Random.onUnitSphere * 0.7f * m_ChargeupAmount;
			}
		}

		private void Update()
		{
			if (m_isEjectorForward)
			{
				if (Chamber.IsFull)
				{
					FVRFireArm fVRFireArm = ((!(Attachment != null)) ? OverrideFA : (Attachment.curMount.MyObject as FVRFireArm));
					m_isEjectorForward = false;
					fVRFireArm.SetAnimatedComponent(Ejector, EjectorRange.x, FVRPhysicalObject.InterpStyle.Translate, FVRPhysicalObject.Axis.Z);
				}
			}
			else if (!Chamber.IsFull)
			{
				FVRFireArm fVRFireArm2 = ((!(Attachment != null)) ? OverrideFA : (Attachment.curMount.MyObject as FVRFireArm));
				m_isEjectorForward = true;
				fVRFireArm2.SetAnimatedComponent(Ejector, EjectorRange.y, FVRPhysicalObject.InterpStyle.Translate, FVRPhysicalObject.Axis.Z);
			}
		}
	}
}
