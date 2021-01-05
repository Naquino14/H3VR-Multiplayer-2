using UnityEngine;

namespace FistVR
{
	public class TwoStageRotationOnHeld : MonoBehaviour
	{
		public FVRInteractiveObject IO;

		private bool m_isHeld;

		public Transform RotPiece;

		public Vector3 Rots_Held;

		public Vector3 Rots_NotHeld;

		private void Start()
		{
		}

		private void Update()
		{
			if (!m_isHeld)
			{
				if (IO.IsHeld)
				{
					m_isHeld = true;
					SetRot(isHeld: true);
				}
			}
			else if (!IO.IsHeld)
			{
				m_isHeld = false;
				SetRot(isHeld: false);
			}
		}

		private void SetRot(bool isHeld)
		{
			if (isHeld)
			{
				RotPiece.localEulerAngles = Rots_Held;
			}
			else
			{
				RotPiece.localEulerAngles = Rots_NotHeld;
			}
		}
	}
}
