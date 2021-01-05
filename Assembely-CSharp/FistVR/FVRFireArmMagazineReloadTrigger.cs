using UnityEngine;

namespace FistVR
{
	public class FVRFireArmMagazineReloadTrigger : MonoBehaviour
	{
		public FVRFireArmMagazine Magazine;

		public FVRFireArmClip Clip;

		public SpeedloaderChamber SpeedloaderChamber;

		public bool IsClipTrigger;

		public bool IsSpeedloaderTrigger;
	}
}
