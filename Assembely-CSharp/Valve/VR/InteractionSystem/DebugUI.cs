using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class DebugUI : MonoBehaviour
	{
		private Player player;

		private static DebugUI _instance;

		public static DebugUI instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Object.FindObjectOfType<DebugUI>();
				}
				return _instance;
			}
		}

		private void Start()
		{
			player = Player.instance;
		}

		private void OnGUI()
		{
			if (Debug.isDebugBuild)
			{
				player.Draw2DDebug();
			}
		}
	}
}
