using UnityEngine;

namespace FistVR
{
	public class FVRFirearmClipFeedPort : MonoBehaviour
	{
		public FVRFireArm Firearm;

		public Transform PortPiece;

		public FVRPhysicalObject.Axis Axis = FVRPhysicalObject.Axis.Z;

		public float RotOpen;

		public float RotClosed;

		private bool m_isClosed = true;

		private void Awake()
		{
			UpdateRot();
		}

		private void Update()
		{
			if (m_isClosed && Firearm.Clip != null)
			{
				m_isClosed = false;
				UpdateRot();
			}
			else if (!m_isClosed && Firearm.Clip == null)
			{
				m_isClosed = true;
				UpdateRot();
			}
		}

		private void UpdateRot()
		{
			float num = RotOpen;
			if (m_isClosed)
			{
				num = RotClosed;
			}
			Vector3 localEulerAngles = new Vector3(0f, 0f, 0f);
			switch (Axis)
			{
			case FVRPhysicalObject.Axis.X:
				localEulerAngles.x = num;
				break;
			case FVRPhysicalObject.Axis.Y:
				localEulerAngles.y = num;
				break;
			case FVRPhysicalObject.Axis.Z:
				localEulerAngles.z = num;
				break;
			}
			PortPiece.localEulerAngles = localEulerAngles;
		}
	}
}
