using UnityEngine;

namespace FistVR
{
	public class SamplerPlatter_Teleporter : MonoBehaviour
	{
		public int FromIndex;

		public int NextIndex;

		public Transform[] TeleportPointArray;

		public AudioEvent ClickEvent;

		public void TeleportToFromPoint()
		{
			TeleportToIndex(FromIndex);
		}

		public void TeleportToNextPoint()
		{
			TeleportToIndex(NextIndex);
		}

		public void TeleportToIndex(int i)
		{
			GM.CurrentMovementManager.TeleportToPoint(TeleportPointArray[i].position, isAbsolute: true, TeleportPointArray[i].forward);
			SM.PlayGenericSound(ClickEvent, base.transform.position);
		}
	}
}
