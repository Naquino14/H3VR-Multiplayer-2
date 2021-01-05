using UnityEngine;

namespace FistVR
{
	public class OrnamentShatter : MonoBehaviour
	{
		private void OnDestroy()
		{
			GameObject gameObject = GameObject.Find("Mom");
			if (gameObject != null)
			{
				gameObject.GetComponent<MomStuff>().InitiateMom();
			}
		}
	}
}
