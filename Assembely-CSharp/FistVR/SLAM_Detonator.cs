using UnityEngine;

namespace FistVR
{
	public class SLAM_Detonator : FVRPhysicalObject
	{
		public Transform Trigger;

		public Vector2 TriggerRange;

		private float m_triggerfloat;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (m_hasTriggeredUpSinceBegin)
			{
				m_triggerfloat = hand.Input.TriggerFloat;
				float val = Mathf.Lerp(TriggerRange.x, TriggerRange.y, m_triggerfloat);
				if (hand.Input.TriggerDown)
				{
					FXM.DetonateSPAAMS();
				}
				SetAnimatedComponent(Trigger, val, InterpStyle.Rotation, Axis.X);
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			m_triggerfloat = 0f;
			SetAnimatedComponent(Trigger, TriggerRange.x, InterpStyle.Rotation, Axis.X);
		}
	}
}
