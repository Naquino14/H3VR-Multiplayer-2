using UnityEngine;

namespace FistVR
{
	public class SmartTrigger : FVRFireArmAttachmentInterface
	{
		public LayerMask SensingLayer;

		public string SensingString = "Geo_Head";

		public InputOverrider Overrider;

		private RaycastHit m_hit;

		private bool m_hasTriggeredUpSinceDischarge = true;

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			bool flag = false;
			if (Attachment.curMount != null && Attachment.curMount.MyObject != null && Attachment.curMount.MyObject.IsHeld)
			{
				flag = true;
			}
			if (flag)
			{
				Overrider.ConnectToHand(Attachment.curMount.MyObject.m_hand);
				RunSensingProgram(Attachment.curMount.MyObject.m_hand);
			}
			else
			{
				Overrider.FlushHandConnection();
			}
		}

		private void RunSensingProgram(FVRViveHand h)
		{
			if (Overrider.Real_triggerUp)
			{
				m_hasTriggeredUpSinceDischarge = true;
			}
			bool pressed = false;
			if (Overrider.Real_triggerPressed && m_hasTriggeredUpSinceDischarge)
			{
				FVRFireArm fVRFireArm = Attachment.curMount.MyObject as FVRFireArm;
				Vector3 position = fVRFireArm.GetMuzzle().position;
				Vector3 forward = fVRFireArm.GetMuzzle().forward;
				if (Physics.Raycast(position, forward, out m_hit, 100f, SensingLayer) && m_hit.collider.attachedRigidbody != null)
				{
					SosigLink component = m_hit.collider.attachedRigidbody.gameObject.GetComponent<SosigLink>();
					if (component != null && component.BodyPart == SosigLink.SosigBodyPart.Head)
					{
						pressed = true;
					}
				}
			}
			Overrider.UpdateTrigger(pressed);
		}
	}
}
