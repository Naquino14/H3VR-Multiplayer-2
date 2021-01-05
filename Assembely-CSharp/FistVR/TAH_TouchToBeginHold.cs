using UnityEngine;

namespace FistVR
{
	public class TAH_TouchToBeginHold : MonoBehaviour
	{
		private void OnTriggerEnter()
		{
			GM.TAHMaster.TouchToEndTake();
		}
	}
}
