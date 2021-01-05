using UnityEngine;

namespace FistVR
{
	public class RailCam : FVRFireArmAttachmentInterface
	{
		public AudioEvent AudEvent_Trigger;

		public Transform CamPoint;

		private bool m_isEngaged;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (hand.IsInStreamlinedMode)
			{
				if (hand.Input.BYButtonDown)
				{
					GM.CurrentSceneSettings.SetCamObjectPoint(CamPoint);
					m_isEngaged = true;
					SM.PlayGenericSound(AudEvent_Trigger, base.transform.position);
				}
			}
			else if (hand.Input.TouchpadDown && Vector2.Angle(Vector2.up, hand.Input.TouchpadAxes) <= 90f)
			{
				GM.CurrentSceneSettings.SetCamObjectPoint(CamPoint);
				m_isEngaged = true;
				SM.PlayGenericSound(AudEvent_Trigger, base.transform.position);
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (m_isEngaged && GM.CurrentSceneSettings.GetCamObjectPoint() != CamPoint)
			{
				m_isEngaged = false;
				SM.PlayGenericSound(AudEvent_Trigger, base.transform.position);
			}
		}
	}
}
