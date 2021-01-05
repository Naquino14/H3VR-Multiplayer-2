using UnityEngine;

namespace DinoFracture
{
	public class NotifyOnFracture : MonoBehaviour
	{
		public GameObject[] GameObjects = new GameObject[1];

		private void OnFracture(OnFractureEventArgs args)
		{
			if (!(args.OriginalObject.gameObject == base.gameObject))
			{
				return;
			}
			for (int i = 0; i < GameObjects.Length; i++)
			{
				if (GameObjects[i] != null)
				{
					GameObjects[i].SendMessage("OnFracture", args, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}
}
