using UnityEngine;

namespace FistVR
{
	public class wwKey : FVRPhysicalObject
	{
		public wwParkManager Manager;

		public int KeyIndex;

		public int State;

		public void OnTriggerEnter(Collider col)
		{
			if (col.gameObject.CompareTag("KeyDetectTrigger"))
			{
				wwFinaleDoorTrigger component = col.gameObject.GetComponent<wwFinaleDoorTrigger>();
				Manager.FinaleManager.OpenDoor(component.Door.Index);
				Manager.RegisterDoorStateChange(component.Door.Index, 1);
				if (base.IsHeld)
				{
					FVRViveHand hand = m_hand;
					EndInteraction(hand);
					hand.ForceSetInteractable(null);
				}
				Manager.RegisterKeyStateChange(KeyIndex, 2);
				Object.Destroy(base.gameObject);
			}
		}
	}
}
