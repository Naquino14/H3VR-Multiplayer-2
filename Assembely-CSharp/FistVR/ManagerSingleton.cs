using UnityEngine;

namespace FistVR
{
	public class ManagerSingleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		public static T Instance
		{
			get;
			private set;
		}

		protected virtual void Awake()
		{
			if ((Object)Instance == (Object)null)
			{
				Instance = this as T;
			}
			else
			{
				Debug.LogError("wtf Anton MANAGER IN SCENE BEEEP BEEEEP BEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEPPPP;");
			}
		}
	}
}
