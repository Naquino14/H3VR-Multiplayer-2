using UnityEngine;

namespace FistVR
{
	public static class ManagerBootStrap
	{
		private static GameObject ManagerGO;

		public static void BootStrap()
		{
			if (ManagerGO == null)
			{
				GameObject original = Resources.Load<GameObject>("Prefabs/_Managers/_GameManager");
				ManagerGO = Object.Instantiate(original);
				Object.DontDestroyOnLoad(ManagerGO);
			}
		}
	}
}
