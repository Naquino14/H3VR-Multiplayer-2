using UnityEngine;

namespace FistVR
{
	public class RomanCandlePayload : MonoBehaviour
	{
		public Color FlashColor;

		private void Awake()
		{
			if (GM.CurrentSceneSettings.IsSceneLowLight)
			{
				FXM.InitiateMuzzleFlash(base.transform.position, -Vector3.up, 30f, FlashColor, 150f);
			}
			else
			{
				FXM.InitiateMuzzleFlash(base.transform.position, -Vector3.up, 2f, FlashColor, 3f);
			}
		}
	}
}
