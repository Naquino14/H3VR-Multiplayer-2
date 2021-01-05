using UnityEngine;

namespace FistVR
{
	public class FlagPoseSwitcher : FVRFireArmAttachmentInterface
	{
		public Transform Flag;

		public Transform[] Poses;

		private int m_index;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			Vector2 touchpadAxes = hand.Input.TouchpadAxes;
			if (hand.IsInStreamlinedMode)
			{
				if (hand.Input.BYButtonDown)
				{
					NextPose();
				}
			}
			else if (hand.Input.TouchpadDown)
			{
				if (touchpadAxes.magnitude > 0.25f && Vector2.Angle(touchpadAxes, Vector2.left) <= 45f)
				{
					NextPose();
				}
				if (touchpadAxes.magnitude > 0.25f && Vector2.Angle(touchpadAxes, Vector2.right) <= 45f)
				{
					PrevPose();
				}
			}
			base.UpdateInteraction(hand);
		}

		private void NextPose()
		{
			m_index++;
			if (m_index >= Poses.Length)
			{
				m_index = 0;
			}
			Flag.localPosition = Poses[m_index].localPosition;
			Flag.localRotation = Poses[m_index].localRotation;
		}

		private void PrevPose()
		{
			m_index--;
			if (m_index < 0)
			{
				m_index = Poses.Length - 1;
			}
			Flag.localPosition = Poses[m_index].localPosition;
			Flag.localRotation = Poses[m_index].localRotation;
		}
	}
}
