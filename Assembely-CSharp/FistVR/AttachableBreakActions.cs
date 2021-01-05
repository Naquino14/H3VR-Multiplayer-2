using UnityEngine;

namespace FistVR
{
	public class AttachableBreakActions : AttachableFirearm
	{
		[Header("Attachable Break Action")]
		public FVRFireArmChamber Chamber;

		public Transform Breach;

		public Vector2 BreachRots = new Vector2(0f, 30f);

		public Transform Trigger;

		public Vector2 TriggerRange;

		public Transform Ejector;

		public Vector2 EjectorRange = new Vector2(0f, 0.005f);

		private bool m_isEjectorForward = true;

		public Transform EjectPos;

		private bool m_isBreachOpen;

		public Transform InertialCloseDir;

		public override void ProcessInput(FVRViveHand hand, bool fromInterface, FVRInteractiveObject o)
		{
			if (o.m_hasTriggeredUpSinceBegin)
			{
				Attachment.SetAnimatedComponent(Trigger, Mathf.Lerp(TriggerRange.x, TriggerRange.y, hand.Input.TriggerFloat), FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
			}
			if (hand.Input.TriggerDown && o.m_hasTriggeredUpSinceBegin && !m_isBreachOpen)
			{
				Fire(fromInterface);
			}
			if (hand.IsInStreamlinedMode)
			{
				if (hand.Input.BYButtonDown)
				{
					ToggleBreach();
				}
			}
			else if (hand.Input.TouchpadDown && hand.Input.TouchpadWestPressed)
			{
				ToggleBreach();
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
				FVRFireArm fVRFireArm = Attachment.curMount.MyObject as FVRFireArm;
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
				Debug.Log("Should fire");
				Recoil(twoHandStabilized: false, foregripStabilized: false, shoulderStabilized: false);
				Fire(Chamber, MuzzlePos, doBuzz: true, null);
			}
			PlayAudioGunShot(Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
		}

		public void Eject()
		{
			Chamber.EjectRound(EjectPos.position, -EjectPos.forward * 1f, Vector3.zero, ForceCaseLessEject: true);
			PlayAudioEvent(FirearmAudioEventType.MagazineEjectRound);
		}

		public void ToggleBreach()
		{
			m_isBreachOpen = !m_isBreachOpen;
			if (m_isBreachOpen)
			{
				Breach.localEulerAngles = new Vector3(BreachRots.y, 0f, 0f);
				Chamber.IsAccessible = true;
				PlayAudioEvent(FirearmAudioEventType.BreachOpen);
				if (Chamber.IsFull && Chamber.IsSpent)
				{
					Eject();
				}
			}
			else
			{
				Breach.localEulerAngles = new Vector3(BreachRots.x, 0f, 0f);
				Chamber.IsAccessible = false;
				PlayAudioEvent(FirearmAudioEventType.BreachClose);
			}
		}

		private void Update()
		{
			if (m_isEjectorForward)
			{
				if (m_isBreachOpen)
				{
					m_isEjectorForward = false;
					Attachment.SetAnimatedComponent(Ejector, EjectorRange.x, FVRPhysicalObject.InterpStyle.Translate, FVRPhysicalObject.Axis.Z);
				}
			}
			else if (!m_isBreachOpen)
			{
				m_isEjectorForward = true;
				Attachment.SetAnimatedComponent(Ejector, EjectorRange.y, FVRPhysicalObject.InterpStyle.Translate, FVRPhysicalObject.Axis.Z);
			}
		}

		public void FixedUpdate()
		{
			bool flag = false;
			Vector3 to = Vector3.zero;
			if (Attachment.IsHeld)
			{
				flag = true;
				to = Attachment.m_hand.Input.VelLinearWorld;
			}
			else if (Attachment.curMount != null && Attachment.curMount.GetRootMount().MyObject != null && Attachment.curMount.GetRootMount().MyObject.IsHeld)
			{
				flag = true;
				to = Attachment.curMount.GetRootMount().MyObject.m_hand.Input.VelLinearWorld;
			}
			if (flag && m_isBreachOpen && Vector3.Angle(InertialCloseDir.forward, to) < 80f && to.magnitude > 2.5f)
			{
				ToggleBreach();
			}
		}
	}
}
