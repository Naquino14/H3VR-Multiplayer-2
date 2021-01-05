using UnityEngine;

namespace FistVR
{
	public class FVRBodyPiece : MonoBehaviour
	{
		public Transform TrackedPos;

		private Rigidbody m_trackedPosRB;

		public Transform AlternateYPos;

		private Rigidbody m_alternateYPosRB;

		public bool TracksPosition;

		private Rigidbody m_rb;

		public void Awake()
		{
			if (GetComponent<Rigidbody>() != null)
			{
				m_rb = GetComponent<Rigidbody>();
			}
			if (!(TrackedPos != null) || (bool)TrackedPos.gameObject.GetComponent<Rigidbody>())
			{
			}
			if (AlternateYPos != null && AlternateYPos.gameObject.GetComponent<Rigidbody>() != null)
			{
				m_alternateYPosRB = AlternateYPos.gameObject.GetComponent<Rigidbody>();
			}
		}

		public void SetTrackedPos(Transform t)
		{
			TrackedPos = t;
			if (!t.gameObject.GetComponent<Rigidbody>())
			{
			}
		}

		private void LateUpdate()
		{
			if (m_rb == null)
			{
				if (TracksPosition)
				{
					if (AlternateYPos == null)
					{
						base.transform.position = TrackedPos.position;
					}
					else
					{
						base.transform.position = new Vector3(TrackedPos.position.x, AlternateYPos.position.y, TrackedPos.position.z);
					}
				}
				Vector3 forward = TrackedPos.forward;
				Vector3 vector = forward;
				vector.y = 0f;
				vector.Normalize();
				Vector3 a = Vector3.zero;
				a = ((!(forward.y > 0f)) ? TrackedPos.up : (-TrackedPos.up));
				a.y = 0f;
				a.Normalize();
				float num = Mathf.Clamp(Vector3.Dot(vector, forward), 0f, 1f);
				Vector3 forward2 = Vector3.Lerp(a, vector, num * num);
				base.transform.rotation = Quaternion.LookRotation(forward2, Vector3.up);
				return;
			}
			if (m_trackedPosRB != null)
			{
				if (TracksPosition)
				{
					if (AlternateYPos == null)
					{
						m_rb.MovePosition(m_trackedPosRB.position);
					}
					else
					{
						m_rb.MovePosition(new Vector3(m_trackedPosRB.position.x, AlternateYPos.position.y, m_trackedPosRB.position.z));
					}
				}
				Vector3 forward3 = TrackedPos.forward;
				Vector3 vector2 = forward3;
				vector2.y = 0f;
				vector2.Normalize();
				Vector3 a2 = Vector3.zero;
				a2 = ((!(forward3.y > 0f)) ? TrackedPos.up : (-TrackedPos.up));
				a2.y = 0f;
				a2.Normalize();
				float num2 = Mathf.Clamp(Vector3.Dot(vector2, forward3), 0f, 1f);
				Vector3 forward4 = Vector3.Lerp(a2, vector2, num2 * num2);
				m_rb.rotation = Quaternion.LookRotation(forward4, Vector3.up);
				return;
			}
			if (TracksPosition)
			{
				if (AlternateYPos == null)
				{
					m_rb.MovePosition(TrackedPos.position);
				}
				else
				{
					m_rb.MovePosition(new Vector3(TrackedPos.position.x, AlternateYPos.position.y, TrackedPos.position.z));
				}
			}
			Vector3 forward5 = TrackedPos.forward;
			Vector3 vector3 = forward5;
			vector3.y = 0f;
			vector3.Normalize();
			Vector3 a3 = Vector3.zero;
			a3 = ((!(forward5.y > 0f)) ? TrackedPos.up : (-TrackedPos.up));
			a3.y = 0f;
			a3.Normalize();
			float num3 = Mathf.Clamp(Vector3.Dot(vector3, forward5), 0f, 1f);
			Vector3 forward6 = Vector3.Lerp(a3, vector3, num3 * num3);
			m_rb.MoveRotation(Quaternion.LookRotation(forward6, Vector3.up));
		}
	}
}
