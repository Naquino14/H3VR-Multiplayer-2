using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class TubeFedShotgunMagazineSwitcher : FVRInteractiveObject
	{
		public enum MagState
		{
			LeftMag,
			NoMag,
			RightMag
		}

		[Serializable]
		public class FauxMag
		{
			public FVRFireArmMagazine Mag;

			public Transform LowerPathForward;

			public Transform LowerPathRearward;

			public Transform TrigPoint;
		}

		public TubeFedShotgun Shotgun;

		public FVRFireArmMagazineReloadTrigger Trig;

		public Transform Switch;

		public List<FauxMag> FauxMags;

		public MagState CurState;

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			if (Shotgun.CanCycleMagState())
			{
				CycleSwitch();
				Shotgun.PlayAudioAsHandling(Shotgun.AudioClipSet.Safety, base.transform.position);
			}
		}

		private void CycleSwitch()
		{
			if (CurState == MagState.LeftMag)
			{
				CurState = MagState.NoMag;
			}
			else if (CurState == MagState.NoMag)
			{
				CurState = MagState.RightMag;
			}
			else if (CurState == MagState.RightMag)
			{
				CurState = MagState.LeftMag;
			}
			UpdateState();
		}

		private void UpdateState()
		{
			if (CurState == MagState.LeftMag)
			{
				Shotgun.Magazine = FauxMags[0].Mag;
				Trig.Magazine = FauxMags[0].Mag;
				Trig.gameObject.SetActive(value: true);
				Shotgun.RoundPos_LowerPath_Forward.position = FauxMags[0].LowerPathForward.position;
				Shotgun.RoundPos_LowerPath_Rearward.position = FauxMags[0].LowerPathRearward.position;
				Trig.transform.position = FauxMags[0].TrigPoint.position;
				Switch.transform.localEulerAngles = new Vector3(0f, 0f, -16f);
			}
			else if (CurState == MagState.NoMag)
			{
				Shotgun.Magazine = null;
				Trig.gameObject.SetActive(value: false);
				Switch.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			}
			else if (CurState == MagState.RightMag)
			{
				Shotgun.Magazine = FauxMags[1].Mag;
				Trig.Magazine = FauxMags[1].Mag;
				Trig.gameObject.SetActive(value: true);
				Shotgun.RoundPos_LowerPath_Forward.position = FauxMags[1].LowerPathForward.position;
				Shotgun.RoundPos_LowerPath_Rearward.position = FauxMags[1].LowerPathRearward.position;
				Trig.transform.position = FauxMags[1].TrigPoint.position;
				Switch.transform.localEulerAngles = new Vector3(0f, 0f, 16f);
			}
		}
	}
}
