using UnityEngine;

namespace FistVR
{
	public class BreakActionModeSwitchButton : FVRInteractiveObject
	{
		public BreakActionWeapon Weapon;

		public Transform Switch;

		public Vector2 RotSet;

		private bool m_isSwitched;

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			Weapon.FireAllBarrels = !Weapon.FireAllBarrels;
			Weapon.PlayAudioEvent(FirearmAudioEventType.FireSelector);
			m_isSwitched = !m_isSwitched;
			if (m_isSwitched)
			{
				Weapon.SetAnimatedComponent(Switch, RotSet.y, FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
			}
			else
			{
				Weapon.SetAnimatedComponent(Switch, RotSet.x, FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
			}
		}
	}
}
