using UnityEngine;

namespace FistVR
{
	public class OpenBoltSecondarySelectorSwitch : FVRInteractiveObject
	{
		public enum InterpStyle
		{
			Translate,
			Rotation
		}

		public enum Axis
		{
			X,
			Y,
			Z
		}

		public OpenBoltReceiver Gun;

		public InterpStyle FireSelector_InterpStyle = InterpStyle.Rotation;

		public Axis FireSelector_Axis;

		public int ModeIndexToSub;

		public OpenBoltReceiver.FireSelectorMode[] Modes;

		public int CurModeIndex;

		public Transform Selector;

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
			Gun.SecondaryFireSelectorClicked();
		}

		private void UpdateBaseGunSelector(int i)
		{
			CurModeIndex = i;
			Gun.FireSelector_Modes[ModeIndexToSub].ModeType = Modes[CurModeIndex].ModeType;
			switch (FireSelector_InterpStyle)
			{
			case InterpStyle.Rotation:
			{
				Vector3 zero2 = Vector3.zero;
				switch (FireSelector_Axis)
				{
				case Axis.X:
					zero2.x = Modes[CurModeIndex].SelectorPosition;
					break;
				case Axis.Y:
					zero2.y = Modes[CurModeIndex].SelectorPosition;
					break;
				case Axis.Z:
					zero2.z = Modes[CurModeIndex].SelectorPosition;
					break;
				}
				Selector.localEulerAngles = zero2;
				break;
			}
			case InterpStyle.Translate:
			{
				Vector3 zero = Vector3.zero;
				switch (FireSelector_Axis)
				{
				case Axis.X:
					zero.x = Modes[CurModeIndex].SelectorPosition;
					break;
				case Axis.Y:
					zero.y = Modes[CurModeIndex].SelectorPosition;
					break;
				case Axis.Z:
					zero.z = Modes[CurModeIndex].SelectorPosition;
					break;
				}
				Selector.localPosition = zero;
				break;
			}
			}
		}
	}
}
