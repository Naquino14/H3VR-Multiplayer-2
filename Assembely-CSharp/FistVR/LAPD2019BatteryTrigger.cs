using UnityEngine;

namespace FistVR
{
	public class LAPD2019BatteryTrigger : MonoBehaviour
	{
		public LAPD2019Battery Battery;

		public LAPD2019BatteryTriggerWell WellTrigger;

		private void OnTriggerEnter(Collider collider)
		{
			if (Battery != null && Battery.QuickbeltSlot == null && collider.gameObject.tag == "FVRFireArmMagazineReloadTrigger")
			{
				LAPD2019BatteryTriggerWell component = collider.gameObject.GetComponent<LAPD2019BatteryTriggerWell>();
				if (component != null)
				{
					WellTrigger = component;
				}
			}
		}

		private void OnTriggerExit(Collider collider)
		{
			if (WellTrigger != null && collider.gameObject.tag == "FVRFireArmMagazineReloadTrigger")
			{
				LAPD2019BatteryTriggerWell component = collider.gameObject.GetComponent<LAPD2019BatteryTriggerWell>();
				if (component == WellTrigger)
				{
					WellTrigger = null;
				}
			}
		}

		private void Update()
		{
			if (WellTrigger != null)
			{
				LoadMag();
			}
		}

		private void LoadMag()
		{
			if (WellTrigger.Gun.LoadBattery(Battery))
			{
				Object.Destroy(Battery.gameObject);
			}
		}
	}
}
