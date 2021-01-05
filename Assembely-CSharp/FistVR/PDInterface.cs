using UnityEngine;

namespace FistVR
{
	public class PDInterface : MonoBehaviour
	{
		public void PrintEventMessage(PDSys Sys, string S)
		{
			Debug.Log(Sys.GetIdentifier() + ":" + S);
		}
	}
}
