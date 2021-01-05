using UnityEngine;

namespace FistVR
{
	public class RPG7Foregrip : FVRAlternateGrip
	{
		[Header("RPG-7 Config")]
		public RPG7 RPG;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			bool flag = false;
			if (hand.IsInStreamlinedMode && hand.Input.AXButtonDown)
			{
				flag = true;
			}
			else if (!hand.IsInStreamlinedMode && hand.Input.TouchpadDown)
			{
				flag = true;
			}
			if (flag)
			{
				RPG.CockHammer();
			}
			if (hand.Input.TriggerDown)
			{
				RPG.Fire();
			}
			float triggerFloat = hand.Input.TriggerFloat;
			RPG.UpdateTriggerRot(triggerFloat);
		}
	}
}
