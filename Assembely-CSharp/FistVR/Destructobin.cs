using UnityEngine;

namespace FistVR
{
	public class Destructobin : MonoBehaviour
	{
		private void testCol(Collider col)
		{
			if (!(col.attachedRigidbody != null) || !(col.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>() != null))
			{
				return;
			}
			FVRPhysicalObject component = col.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
			if (!(component.QuickbeltSlot != null) && !component.m_isHardnessed)
			{
				if (!component.IsHeld)
				{
					Object.Destroy(component.gameObject);
					return;
				}
				component.m_hand.ForceSetInteractable(null);
				component.EndInteraction(component.m_hand);
				Object.Destroy(component.gameObject);
			}
		}

		private void OnTriggerStay(Collider other)
		{
			testCol(other);
		}
	}
}
