using UnityEngine;

namespace FistVR
{
	public class FVRFireArmClipTriggerClip : MonoBehaviour
	{
		public FVRFireArmClip Clip;

		private void OnTriggerEnter(Collider collider)
		{
			if (Clip != null && Clip.FireArm == null && Clip.QuickbeltSlot == null && collider.gameObject.CompareTag("FVRFireArmClipReloadTriggerWell"))
			{
				FVRFireArmClipTriggerWell component = collider.gameObject.GetComponent<FVRFireArmClipTriggerWell>();
				if (component != null && component.FireArm != null && component.FireArm.ClipType == Clip.ClipType && component.FireArm.ClipEjectDelay <= 0f && component.FireArm.Clip == null)
				{
					Clip.Load(component.FireArm);
				}
			}
		}
	}
}
