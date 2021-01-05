using UnityEngine;

namespace FistVR
{
	public class FVRMovementRig : MonoBehaviour
	{
		public Transform HeadProxy;

		public Transform ControllerProxy_Left;

		public Transform ControllerProxy_Right;

		private Transform m_head;

		private Transform m_lefthand;

		private Transform m_righthand;

		private bool m_hasFoundCorners;

		public Transform CornerHolder;

		public Transform[] CornerGeos;

		private Vector3 c1;

		private Vector3 c2;

		private Vector3 c3;

		private Vector3 c4;

		private void Update()
		{
			if (!m_hasFoundCorners)
			{
				m_hasFoundCorners = FVRPlayArea.TryGetPlayArea(out c1, out c2, out c3, out c4);
				if (m_hasFoundCorners)
				{
					CornerGeos[0].transform.localPosition = c1;
					CornerGeos[1].transform.localPosition = c2;
					CornerGeos[2].transform.localPosition = c3;
					CornerGeos[3].transform.localPosition = c4;
				}
			}
		}
	}
}
