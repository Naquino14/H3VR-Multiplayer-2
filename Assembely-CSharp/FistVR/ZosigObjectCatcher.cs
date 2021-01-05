using UnityEngine;

namespace FistVR
{
	public class ZosigObjectCatcher : MonoBehaviour
	{
		public LayerMask CastLM;

		private RaycastHit m_hit;

		private void OnTriggerEnter(Collider col)
		{
			if (!(col.attachedRigidbody != null))
			{
				return;
			}
			if (col.attachedRigidbody.gameObject.GetComponent<FVRFireArmRound>() != null)
			{
				Object.Destroy(col.attachedRigidbody.gameObject);
				return;
			}
			Vector3 origin = col.attachedRigidbody.transform.position + Vector3.up * 200f;
			if (Physics.Raycast(origin, -Vector3.up, out m_hit, 200f, CastLM, QueryTriggerInteraction.Ignore))
			{
				col.attachedRigidbody.velocity = Vector3.zero;
				col.attachedRigidbody.angularVelocity = Vector3.zero;
				col.attachedRigidbody.transform.position = m_hit.point + Vector3.up * 0.6f;
				col.attachedRigidbody.velocity = Vector3.zero;
				col.attachedRigidbody.angularVelocity = Vector3.zero;
			}
		}
	}
}
