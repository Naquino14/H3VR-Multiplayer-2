using UnityEngine;

namespace FistVR
{
	public class ScalpelMagArm : MonoBehaviour
	{
		public FVRFireArm FireArm;

		public Transform Latch;

		private bool m_hasMag;

		public Vector3 Rot_NoMag;

		public Vector3 Rot_HasMag_Low;

		public Vector3 Rot_HasMag_High;

		private void Start()
		{
		}

		private void Update()
		{
			if (FireArm.Magazine == null)
			{
				if (m_hasMag)
				{
					m_hasMag = false;
					SetMag(h: false);
				}
			}
			else if (!m_hasMag)
			{
				m_hasMag = true;
				SetMag(h: true);
			}
		}

		private void SetMag(bool h)
		{
			if (!h)
			{
				SetRot(Rot_NoMag);
			}
			else
			{
				SetRot(Rot_HasMag_Low);
			}
		}

		private void SetRot(Vector3 v)
		{
			Latch.localEulerAngles = v;
		}
	}
}
