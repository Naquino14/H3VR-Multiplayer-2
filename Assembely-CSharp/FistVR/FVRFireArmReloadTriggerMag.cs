using UnityEngine;

namespace FistVR
{
	public class FVRFireArmReloadTriggerMag : MonoBehaviour
	{
		public FVRFireArmMagazine Magazine;

		private void OnTriggerEnter(Collider collider)
		{
			if (!(Magazine != null) || !(Magazine.FireArm == null) || !(Magazine.QuickbeltSlot == null) || !(collider.gameObject.tag == "FVRFireArmReloadTriggerWell"))
			{
				return;
			}
			FVRFireArmReloadTriggerWell component = collider.gameObject.GetComponent<FVRFireArmReloadTriggerWell>();
			bool flag = false;
			if (component != null && !Magazine.IsBeltBox && component.FireArm.HasBelt)
			{
				flag = true;
			}
			if (component != null && component.IsBeltBox == Magazine.IsBeltBox && component.FireArm != null && component.FireArm.Magazine == null && !flag)
			{
				FireArmMagazineType fireArmMagazineType = component.FireArm.MagazineType;
				if (component.UsesTypeOverride)
				{
					fireArmMagazineType = component.TypeOverride;
				}
				if (fireArmMagazineType == Magazine.MagazineType && (component.FireArm.EjectDelay <= 0f || Magazine != component.FireArm.LastEjectedMag) && component.FireArm.Magazine == null)
				{
					Magazine.Load(component.FireArm);
				}
			}
		}
	}
}
