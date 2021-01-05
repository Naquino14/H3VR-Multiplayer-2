using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
	public class NavMeshLinkExtension : MonoBehaviour
	{
		public enum NavMeshLinkType
		{
			LateralJump,
			Climb,
			Drop
		}

		public NavMeshLinkType Type;

		public float TimeToClear = 1f;

		private float m_xySpeed;

		public OffMeshLink Link;

		public float GetXYSpeed()
		{
			return m_xySpeed;
		}

		private void Start()
		{
			Link = base.gameObject.GetComponent<OffMeshLink>();
			float magnitude = (Link.startTransform.position - Link.endTransform.position).magnitude;
			m_xySpeed = magnitude / TimeToClear;
		}
	}
}
