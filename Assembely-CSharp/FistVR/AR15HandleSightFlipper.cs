using UnityEngine;

namespace FistVR
{
	public class AR15HandleSightFlipper : FVRInteractiveObject
	{
		public enum Axis
		{
			X,
			Y,
			Z
		}

		private bool m_isLargeAperture = true;

		public Transform Flipsight;

		public float m_flipsightStartRotX;

		public float m_flipsightEndRotX = -90f;

		private float m_flipsightCurRotX;

		public Axis RotAxis;

		private float m_curFlipLerp;

		private float m_tarFlipLerp;

		private float m_lastFlipLerp;

		protected override void Awake()
		{
			base.Awake();
		}

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			m_isLargeAperture = !m_isLargeAperture;
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (m_isLargeAperture)
			{
				m_tarFlipLerp = 0f;
			}
			else
			{
				m_tarFlipLerp = 1f;
			}
			m_curFlipLerp = Mathf.MoveTowards(m_curFlipLerp, m_tarFlipLerp, Time.deltaTime * 4f);
			if (Mathf.Abs(m_curFlipLerp - m_lastFlipLerp) > 0.01f)
			{
				m_flipsightCurRotX = Mathf.Lerp(m_flipsightStartRotX, m_flipsightEndRotX, m_curFlipLerp);
				switch (RotAxis)
				{
				case Axis.X:
					Flipsight.localEulerAngles = new Vector3(m_flipsightCurRotX, 0f, 0f);
					break;
				case Axis.Y:
					Flipsight.localEulerAngles = new Vector3(0f, m_flipsightCurRotX, 0f);
					break;
				case Axis.Z:
					Flipsight.localEulerAngles = new Vector3(0f, 0f, m_flipsightCurRotX);
					break;
				}
			}
			m_lastFlipLerp = m_curFlipLerp;
		}
	}
}
