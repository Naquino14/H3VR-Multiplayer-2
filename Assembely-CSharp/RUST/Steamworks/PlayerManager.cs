using Steamworks;
using UnityEngine;

namespace RUST.Steamworks
{
	public class PlayerManager : MonoBehaviour
	{
		public static string SteamName;

		public static string PreferredLanguage;

		private void Start()
		{
			if (SteamManager.Initialized)
			{
				SteamName = SteamFriends.GetPersonaName();
				PreferredLanguage = SteamApps.GetCurrentGameLanguage();
			}
		}
	}
}
