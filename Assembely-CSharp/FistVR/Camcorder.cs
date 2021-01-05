using UnityEngine;

namespace FistVR
{
	public class Camcorder : FVRPhysicalObject
	{
		public Transform CamPoint;

		public Transform Trigger;

		private bool m_isEngaged;

		public Vector2 trig;

		public AudioEvent AudEvent_Trigger;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (m_hasTriggeredUpSinceBegin && hand.Input.TriggerDown)
			{
				GM.CurrentSceneSettings.SetCamObjectPoint(CamPoint);
				m_isEngaged = true;
				Trigger.localEulerAngles = new Vector3(trig.y, 0f, 0f);
				SM.PlayGenericSound(AudEvent_Trigger, base.transform.position);
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (m_isEngaged && GM.CurrentSceneSettings.GetCamObjectPoint() != CamPoint)
			{
				m_isEngaged = false;
				Trigger.localEulerAngles = new Vector3(trig.x, 0f, 0f);
				SM.PlayGenericSound(AudEvent_Trigger, base.transform.position);
			}
		}
	}
}
