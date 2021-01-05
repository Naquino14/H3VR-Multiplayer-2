using UnityEngine;

namespace FistVR
{
	public class RessemblerCoreTrigger : MonoBehaviour
	{
		public int SlotIndex;

		public Translocator T;

		private bool m_hasInserted;

		private void FixedUpdate()
		{
			m_hasInserted = false;
		}

		private void OnTriggerEnter(Collider col)
		{
			if (col.attachedRigidbody != null && col.attachedRigidbody.gameObject.GetComponent<RessemblerCore>() != null && !m_hasInserted && T.InsertCoreToSlot(SlotIndex))
			{
				m_hasInserted = true;
				Object.Destroy(col.attachedRigidbody.gameObject);
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
