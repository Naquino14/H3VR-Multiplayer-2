using UnityEngine;

namespace FistVR
{
	public class SingletonBootstrapper : MonoBehaviour
	{
		private void Awake()
		{
			ManagerBootStrap.BootStrap();
		}
	}
}
