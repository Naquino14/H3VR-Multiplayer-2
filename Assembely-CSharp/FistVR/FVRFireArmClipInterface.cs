using UnityEngine;

namespace FistVR
{
	public class FVRFireArmClipInterface : FVRInteractiveObject
	{
		public FVRFireArmClip Clip;

		private float m_clipLoadTick;

		public override void BeginInteraction(FVRViveHand hand)
		{
			if ((Clip.FireArm != null && Clip.FireArm.Magazine != null && Clip.FireArm.Magazine.IsFull()) || Clip.m_numRounds <= 0)
			{
				base.EndInteraction(hand);
				Clip.FireArm.EjectClip();
				hand.ForceSetInteractable(Clip);
				Clip.BeginInteraction(hand);
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (Clip.State == FVRFireArmClip.ClipState.Free)
			{
				ForceBreakInteraction();
			}
			else if (m_clipLoadTick > 0f)
			{
				m_clipLoadTick -= Time.deltaTime;
			}
			else
			{
				Vector3 velLinearWorld = hand.Input.VelLinearWorld;
				if (velLinearWorld.magnitude > 0.1f && Vector3.Angle(velLinearWorld, -base.transform.up) < 45f)
				{
					Clip.LoadOneRoundFromClipToMag();
					m_clipLoadTick = 0.03f;
				}
			}
			if (hand.IsInStreamlinedMode)
			{
				if (hand.Input.AXButtonDown && Clip != null)
				{
					base.EndInteraction(hand);
					Clip.FireArm.EjectClip();
					hand.ForceSetInteractable(Clip);
					Clip.BeginInteraction(hand);
				}
			}
			else if (hand.Input.TouchpadDown && hand.Input.TouchpadAxes.magnitude > 0.25f && Vector2.Angle(hand.Input.TouchpadAxes, Vector2.down) <= 45f && Clip != null)
			{
				base.EndInteraction(hand);
				Clip.FireArm.EjectClip();
				hand.ForceSetInteractable(Clip);
				Clip.BeginInteraction(hand);
			}
		}
	}
}
