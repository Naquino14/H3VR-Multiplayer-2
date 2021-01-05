using UnityEngine;

namespace FistVR
{
	public class ClosedBoltSecondarySwitch : FVRInteractiveObject
	{
		public ClosedBoltWeapon Weapon;

		public int CurModeIndex;

		public int ModeIndexToSub = 1;

		public Transform SelctorSwitch;

		public FVRPhysicalObject.Axis Axis;

		public FVRPhysicalObject.InterpStyle InterpStyle;

		public ClosedBoltWeapon.FireSelectorMode[] Modes;

		public new void Awake()
		{
			UpdateBaseGunSelector(CurModeIndex);
		}

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			CycleMode();
		}

		private void CycleMode()
		{
			CurModeIndex++;
			if (CurModeIndex >= Modes.Length)
			{
				CurModeIndex = 0;
			}
			UpdateBaseGunSelector(CurModeIndex);
			Weapon.PlayAudioEvent(FirearmAudioEventType.FireSelector);
		}

		private void UpdateBaseGunSelector(int i)
		{
			ClosedBoltWeapon.FireSelectorMode fireSelectorMode = Modes[i];
			Weapon.SetAnimatedComponent(SelctorSwitch, fireSelectorMode.SelectorPosition, InterpStyle, Axis);
			ClosedBoltWeapon.FireSelectorMode fireSelectorMode2 = Weapon.FireSelector_Modes[ModeIndexToSub];
			fireSelectorMode2.ModeType = fireSelectorMode.ModeType;
			fireSelectorMode2.BurstAmount = fireSelectorMode.BurstAmount;
			Weapon.ResetCamBurst();
		}
	}
}
