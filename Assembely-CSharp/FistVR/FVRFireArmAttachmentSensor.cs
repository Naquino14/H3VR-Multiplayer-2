using UnityEngine;

namespace FistVR
{
	public class FVRFireArmAttachmentSensor : MonoBehaviour
	{
		public FVRFireArmAttachment Attachment;

		[HideInInspector]
		public FVRFireArmAttachmentMount CurHoveredMount;

		private float m_storedScaleMagnified = 1f;

		private Collider m_col;

		private bool m_hasCollider;

		private void Awake()
		{
			m_col = GetComponent<Collider>();
			if (m_col != null)
			{
				m_hasCollider = true;
			}
		}

		public void SetTriggerState(bool b)
		{
			if (m_hasCollider)
			{
				m_col.enabled = b;
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (Attachment.IsHeld && CurHoveredMount == null && Attachment.CanAttach() && collider.gameObject.tag == "FVRFireArmAttachmentMount")
			{
				FVRFireArmAttachmentMount component = collider.gameObject.GetComponent<FVRFireArmAttachmentMount>();
				if (component.Type == Attachment.Type && component.isMountableOn(Attachment))
				{
					SetHoveredMount(component);
					component.BeginHover();
				}
			}
		}

		private void OnTriggerExit(Collider collider)
		{
			if (CurHoveredMount != null && collider.gameObject.tag == "FVRFireArmAttachmentMount")
			{
				FVRFireArmAttachmentMount component = collider.gameObject.GetComponent<FVRFireArmAttachmentMount>();
				if (component == CurHoveredMount)
				{
					component.EndHover();
					SetHoveredMount(null);
				}
			}
		}

		private void Update()
		{
			if (CurHoveredMount != null && !CurHoveredMount.isMountableOn(Attachment))
			{
				CurHoveredMount = null;
			}
		}

		private void SetHoveredMount(FVRFireArmAttachmentMount newMount)
		{
			if (newMount == CurHoveredMount)
			{
				return;
			}
			CurHoveredMount = newMount;
			if (Attachment.CanScaleToMount && CurHoveredMount != null && CurHoveredMount.CanThisRescale())
			{
				FVRFireArmAttachmentMount rootMount = CurHoveredMount.GetRootMount();
				if (m_storedScaleMagnified != rootMount.ScaleModifier)
				{
					m_storedScaleMagnified = rootMount.ScaleModifier;
					Attachment.transform.localScale = new Vector3(m_storedScaleMagnified, m_storedScaleMagnified, m_storedScaleMagnified);
				}
			}
			else if (m_storedScaleMagnified != 1f)
			{
				m_storedScaleMagnified = 1f;
				Attachment.transform.localScale = new Vector3(m_storedScaleMagnified, m_storedScaleMagnified, m_storedScaleMagnified);
			}
		}
	}
}
