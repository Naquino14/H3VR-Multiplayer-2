using UnityEngine;

namespace FistVR
{
	public class M203 : AttachableFirearm
	{
		[Header("M203")]
		public FVRFireArmChamber Chamber;

		public M203_Fore Fore;

		public Transform Trigger;

		public Vector2 TriggerRange;

		public override void ProcessInput(FVRViveHand hand, bool fromInterface, FVRInteractiveObject o)
		{
			if (o.m_hasTriggeredUpSinceBegin)
			{
				Attachment.SetAnimatedComponent(Trigger, Mathf.Lerp(TriggerRange.x, TriggerRange.y, hand.Input.TriggerFloat), FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
			}
			if (hand.Input.TriggerDown && Fore.CurPos == M203_Fore.ForePos.Rearward && o.m_hasTriggeredUpSinceBegin)
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
				Recoil(twoHandStabilized: false, foregripStabilized: false, shoulderStabilized: false);
				Fire(Chamber, MuzzlePos, doBuzz: true, null);
			}
			PlayAudioGunShot(Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
		}

		private void Update()
		{
			Fore.UpdateSlide();
		}
	}
}
