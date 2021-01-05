using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FVRBeamerLocus : MonoBehaviour
	{
		public List<Rigidbody> IgnoredRBs;

		public FVRBeamer Beamer;

		private HashSet<Rigidbody> m_rbHash = new HashSet<Rigidbody>();

		private List<Rigidbody> m_rbList = new List<Rigidbody>();

		private bool m_isExists;

		private bool m_isGrav;

		private Vector3 lastPos = Vector3.zero;

		private Vector3 lastlastPos = Vector3.zero;

		private float m_size = 0.01f;

		private Vector3 m_targetPoint = Vector3.zero;

		public GameObject Lightning;

		public ParticleSystem Lightning2;

		public void Awake()
		{
			lastPos = base.transform.position;
			lastlastPos = base.transform.position;
			m_targetPoint = base.transform.position;
			Lightning.SetActive(value: false);
			m_size = 0.01f;
		}

		public void SetExistence(bool b)
		{
			m_isExists = b;
		}

		public void SetGrav(bool b)
		{
			m_isGrav = b;
			Lightning.SetActive(b);
		}

		public void SetTargetPoint(Vector3 p)
		{
			m_targetPoint = p;
		}

		public void Shunt()
		{
			if (m_rbList.Count > 0)
			{
				foreach (Rigidbody item in m_rbHash)
				{
					if (item != null)
					{
						item.AddForceAtPosition(Beamer.transform.forward * 5000f, base.transform.position, ForceMode.Acceleration);
					}
				}
			}
			Lightning2.Emit(30);
			Beamer.Shunt();
			SetGrav(b: false);
		}

		private void FixedUpdate()
		{
			base.transform.position = Vector3.Lerp(base.transform.position, m_targetPoint, Time.deltaTime * 4f);
			if (m_isExists)
			{
				m_size += Time.deltaTime * 0.5f;
				m_size = Mathf.Clamp(m_size, 0.01f, 1f);
				base.transform.localScale = new Vector3(m_size, m_size, m_size);
			}
			else
			{
				m_size -= Time.deltaTime * 3f;
				base.transform.localScale = new Vector3(m_size, m_size, m_size);
				if (m_size <= 0.01f)
				{
					m_size = 0.01f;
					base.transform.localScale = new Vector3(m_size, m_size, m_size);
					base.gameObject.SetActive(value: false);
				}
			}
			if (m_isGrav)
			{
				foreach (Rigidbody item in m_rbHash)
				{
					if (item != null)
					{
						item.velocity = (base.transform.position - item.transform.position).normalized;
						item.transform.position += base.transform.position - lastPos;
					}
				}
			}
			else
			{
				foreach (Rigidbody rb in m_rbList)
				{
					if (rb != null)
					{
						rb.AddForce((lastPos - lastlastPos) * (1f / Time.deltaTime) * 100f, ForceMode.Acceleration);
					}
				}
				m_rbHash.Clear();
				m_rbList.Clear();
			}
			lastlastPos = lastPos;
			lastPos = base.transform.position;
		}

		private void OnTriggerStay(Collider col)
		{
			if (m_isGrav && m_rbList.Count <= 5 && col.attachedRigidbody != null && !col.attachedRigidbody.isKinematic && !IgnoredRBs.Contains(col.attachedRigidbody))
			{
				Rigidbody attachedRigidbody = col.attachedRigidbody;
				if (m_rbHash.Add(attachedRigidbody))
				{
					m_rbList.Add(attachedRigidbody);
				}
			}
		}

		private void OnTriggerExit(Collider col)
		{
			if (col.attachedRigidbody != null && !col.attachedRigidbody.isKinematic)
			{
				Rigidbody attachedRigidbody = col.attachedRigidbody;
				if (m_rbHash.Remove(attachedRigidbody))
				{
					m_rbList.Remove(attachedRigidbody);
				}
			}
		}
	}
}
