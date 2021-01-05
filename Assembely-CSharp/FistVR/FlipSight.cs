using UnityEngine;

namespace FistVR
{
	public class FlipSight : FVRFireArmAttachmentInterface
	{
		private float curXRot;

		public float XRotUp;

		public float XRotDown;

		public bool IsUp = true;

		private float m_curFlipLerp;

		private float m_tarFlipLerp;

		private float m_lastFlipLerp;

		public Transform FlipPart;

		protected override void Awake()
		{
			base.Awake();
			if (IsUp)
			{
				curXRot = XRotUp;
				m_curFlipLerp = 1f;
				m_tarFlipLerp = 1f;
				m_lastFlipLerp = 1f;
			}
			else
			{
				curXRot = XRotDown;
				m_curFlipLerp = 0f;
				m_tarFlipLerp = 0f;
				m_lastFlipLerp = 0f;
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			Vector2 touchpadAxes = hand.Input.TouchpadAxes;
			if (hand.IsInStreamlinedMode)
			{
				if (hand.Input.BYButtonDown)
				{
					Flip();
				}
			}
			else if (hand.Input.TouchpadDown && touchpadAxes.magnitude > 0.25f && Vector2.Angle(touchpadAxes, Vector2.up) <= 45f)
			{
				Flip();
			}
			base.UpdateInteraction(hand);
		}

		protected override void FVRFixedUpdate()
		{
			if (Attachment.curMount != null)
			{
				if (IsUp)
				{
					m_tarFlipLerp = 1f;
				}
				else
				{
					m_tarFlipLerp = 0f;
				}
				m_curFlipLerp = Mathf.MoveTowards(m_curFlipLerp, m_tarFlipLerp, Time.deltaTime * 4f);
				if (Mathf.Abs(m_curFlipLerp - m_lastFlipLerp) > 0.01f)
				{
					FlipPart.localEulerAngles = new Vector3(Mathf.Lerp(XRotDown, XRotUp, m_curFlipLerp), 0f, 0f);
				}
				m_lastFlipLerp = m_curFlipLerp;
			}
			base.FVRFixedUpdate();
		}

		private void Flip()
		{
			IsUp = !IsUp;
		}
	}
}
