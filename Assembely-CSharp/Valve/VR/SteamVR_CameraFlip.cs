using UnityEngine;

namespace Valve.VR
{
	[ExecuteInEditMode]
	public class SteamVR_CameraFlip : MonoBehaviour
	{
		private void Awake()
		{
			Debug.Log("<b>[SteamVR]</b> SteamVR_CameraFlip is deprecated in Unity 5.4 - REMOVING");
			Object.DestroyImmediate(this);
		}
	}
}
