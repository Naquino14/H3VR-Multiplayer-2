using UnityEngine;

namespace FistVR
{
	public class AINavCriticalDestroyable : FVRDestroyableObject
	{
		[Header("NavCriticalDestroyable Params")]
		public AINavigator Navigator;

		public override void DestroyEvent()
		{
			Debug.Log("Disabling Nav");
			Navigator.IsPermanentlyDisabled = true;
			base.DestroyEvent();
		}
	}
}
