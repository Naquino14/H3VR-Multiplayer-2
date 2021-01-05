using UnityEngine;

namespace FistVR
{
	public class wwBotWurstLaserDot : MonoBehaviour
	{
		public Renderer Beam;

		public float MaxCastDist;

		public LayerMask LM_Collide;

		private RaycastHit m_hit;

		public float width = 0.005f;

		private void Start()
		{
		}

		private void Update()
		{
			float z = MaxCastDist;
			if (Physics.Raycast(base.transform.position, base.transform.forward, out m_hit, MaxCastDist, LM_Collide, QueryTriggerInteraction.Ignore))
			{
				z = m_hit.distance;
			}
			base.transform.localScale = new Vector3(width, width, z);
		}
	}
}
