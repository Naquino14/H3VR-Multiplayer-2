using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class AISensor : FVRDestroyableObject
	{
		[Header("Sensor Params")]
		public bool IsOmni;

		public AISensorSystem m_sensorSystem;

		public Transform vision_CastPoint;

		public LayerMask vision_LayerMask;

		public LayerMask vision_LayerMask_Omni;

		public LayerMask vision_Targets;

		public float vision_Distance;

		public float vision_Angle = 25f;

		private RaycastHit m_hit;

		private Queue<Transform> SensorContactQueue = new Queue<Transform>();

		private HashSet<Transform> SensorContactHashes = new HashSet<Transform>();

		public bool SensorLoop()
		{
			Transform transform = null;
			if (SensorContactQueue.Count == 0)
			{
				SensorContactHashes.Clear();
			}
			else
			{
				transform = SensorContactQueue.Dequeue();
			}
			Collider[] array = Physics.OverlapSphere(base.transform.position, vision_Distance, vision_Targets, QueryTriggerInteraction.Collide);
			for (int i = 0; i < array.Length; i++)
			{
				if (IsOmni)
				{
					TriggerProxy(array[i]);
					continue;
				}
				Vector3 normalized = (array[i].transform.position - base.transform.position).normalized;
				if (Vector3.Angle(normalized, base.transform.forward) < vision_Angle)
				{
					TriggerProxy(array[i]);
				}
			}
			if (transform != null)
			{
				if (IsOmni)
				{
					if (Physics.Raycast(vision_CastPoint.position, transform.position - vision_CastPoint.position, out m_hit, vision_Distance, vision_LayerMask_Omni, QueryTriggerInteraction.Collide) && m_hit.collider.transform.root == transform.root)
					{
						m_sensorSystem.Regard(transform);
					}
				}
				else if (Physics.Raycast(vision_CastPoint.position, transform.position - vision_CastPoint.position, out m_hit, vision_Distance, vision_LayerMask, QueryTriggerInteraction.Collide) && m_hit.collider.transform.root == transform.root)
				{
					m_sensorSystem.Regard(transform);
				}
			}
			return true;
		}

		public void TriggerProxy(Collider other)
		{
			if (!(other.gameObject.transform == null) && !(other.attachedRigidbody == null) && other.attachedRigidbody.gameObject.activeSelf && !(other.attachedRigidbody.gameObject.tag == m_sensorSystem.IFFvalue) && SensorContactHashes.Add(other.attachedRigidbody.gameObject.transform))
			{
				SensorContactQueue.Enqueue(other.attachedRigidbody.gameObject.transform);
			}
		}

		public new virtual void DestroyEvent()
		{
		}
	}
}
