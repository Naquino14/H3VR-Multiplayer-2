using System.Collections;
using UnityEngine;

namespace Valve.VR.Extras
{
	public class SteamVR_ForceSteamVRMode : MonoBehaviour
	{
		public GameObject vrCameraPrefab;

		public GameObject[] disableObjectsOnLoad;

		private IEnumerator Start()
		{
			yield return new WaitForSeconds(1f);
			SteamVR.Initialize(forceUnityVRMode: true);
			while (SteamVR.initializedState != SteamVR.InitializedStates.InitializeSuccess)
			{
				yield return null;
			}
			for (int i = 0; i < disableObjectsOnLoad.Length; i++)
			{
				GameObject gameObject = disableObjectsOnLoad[i];
				if (gameObject != null)
				{
					gameObject.SetActive(value: false);
				}
			}
			Object.Instantiate(vrCameraPrefab);
		}
	}
}
