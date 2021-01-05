using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class HoloSight : FVRFireArmAttachmentInterface
	{
		private int m_zeroDistanceIndex = 1;

		private float[] m_zeroDistances = new float[7]
		{
			2f,
			5f,
			10f,
			15f,
			25f,
			50f,
			100f
		};

		public Transform TargetPoint;

		public Text ZeroingText;

		protected override void Awake()
		{
			base.Awake();
			Zero();
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			Vector2 touchpadAxes = hand.Input.TouchpadAxes;
			if (hand.Input.TouchpadDown && touchpadAxes.magnitude > 0.25f)
			{
				if (Vector2.Angle(touchpadAxes, Vector2.left) <= 45f)
				{
					DecreaseZeroDistance();
					Zero();
				}
				else if (Vector2.Angle(touchpadAxes, Vector2.right) <= 45f)
				{
					IncreaseZeroDistance();
					Zero();
				}
			}
			base.UpdateInteraction(hand);
		}

		private void IncreaseZeroDistance()
		{
			m_zeroDistanceIndex++;
			m_zeroDistanceIndex = Mathf.Clamp(m_zeroDistanceIndex, 0, m_zeroDistances.Length - 1);
		}

		private void DecreaseZeroDistance()
		{
			m_zeroDistanceIndex--;
			m_zeroDistanceIndex = Mathf.Clamp(m_zeroDistanceIndex, 0, m_zeroDistances.Length - 1);
		}

		private void Zero()
		{
			if (Attachment.curMount != null && Attachment.curMount.Parent != null && Attachment.curMount.Parent is FVRFireArm)
			{
				FVRFireArm fVRFireArm = Attachment.curMount.Parent as FVRFireArm;
				TargetPoint.position = fVRFireArm.MuzzlePos.position + fVRFireArm.MuzzlePos.forward * m_zeroDistances[m_zeroDistanceIndex];
			}
			else
			{
				TargetPoint.position = base.transform.position + base.transform.forward * m_zeroDistances[m_zeroDistanceIndex];
			}
			ZeroingText.text = "Zero Distance: " + m_zeroDistances[m_zeroDistanceIndex] + "m";
		}

		public override void OnAttach()
		{
			base.OnAttach();
			Zero();
		}

		public override void OnDetach()
		{
			base.OnDetach();
			Zero();
		}
	}
}
