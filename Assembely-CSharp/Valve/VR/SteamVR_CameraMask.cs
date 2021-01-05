using UnityEngine;

namespace Valve.VR
{
	[ExecuteInEditMode]
	public class SteamVR_CameraMask : MonoBehaviour
	{
		private void Awake()
		{
			Debug.Log("<b>[SteamVR]</b> SteamVR_CameraMask is deprecated in Unity 5.4 - REMOVING");
			Object.DestroyImmediate(this);
		}
	}
}
