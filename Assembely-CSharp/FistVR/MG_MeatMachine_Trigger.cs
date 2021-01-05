using UnityEngine;

namespace FistVR
{
	public class MG_MeatMachine_Trigger : MonoBehaviour
	{
		public MG_MeatMachine Machine;

		private void OnTriggerEnter(Collider col)
		{
			CheckCol(col);
		}

		private void CheckCol(Collider col)
		{
			if (col.attachedRigidbody == null)
			{
				return;
			}
			MG_MeatChunk component = col.attachedRigidbody.gameObject.GetComponent<MG_MeatChunk>();
			if (component != null)
			{
				int meatID = component.MeatID;
				if (component.IsHeld)
				{
					component.m_hand.ForceSetInteractable(null);
					component.EndInteraction(component.m_hand);
				}
				Object.Destroy(component.gameObject);
				Machine.FedMeatIn(meatID);
			}
			FVRPhysicalObject component2 = col.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
			if (component2 != null)
			{
				if (component2.IsHeld)
				{
					component2.m_hand.ForceSetInteractable(null);
					component2.EndInteraction(component2.m_hand);
				}
				Object.Destroy(component2.gameObject);
				Machine.FedObjIn();
			}
		}
	}
}
