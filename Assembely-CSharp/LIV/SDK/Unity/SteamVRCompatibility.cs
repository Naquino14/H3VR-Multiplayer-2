using System;

namespace LIV.SDK.Unity
{
	internal static class SteamVRCompatibility
	{
		public static bool IsAvailable;

		public static Type SteamVRCamera;

		public static Type SteamVRExternalCamera;

		public static Type SteamVRFade;

		static SteamVRCompatibility()
		{
			IsAvailable = FindSteamVRAsset();
		}

		private static bool FindSteamVRAsset()
		{
			if (SteamVRCamera == null)
			{
				SteamVRCamera = Type.GetType("SteamVR_Camera", throwOnError: false);
			}
			if (SteamVRCamera == null)
			{
				SteamVRCamera = Type.GetType("Valve.VR.SteamVR_Camera", throwOnError: false);
			}
			if (SteamVRExternalCamera == null)
			{
				SteamVRExternalCamera = Type.GetType("SteamVR_ExternalCamera", throwOnError: false);
			}
			if (SteamVRExternalCamera == null)
			{
				SteamVRExternalCamera = Type.GetType("Valve.VR.SteamVR_ExternalCamera", throwOnError: false);
			}
			if (SteamVRFade == null)
			{
				SteamVRFade = Type.GetType("SteamVR_Fade", throwOnError: false);
			}
			if (SteamVRFade == null)
			{
				SteamVRFade = Type.GetType("Valve.VR.SteamVR_Fade", throwOnError: false);
			}
			return SteamVRCamera != null && SteamVRExternalCamera != null && SteamVRFade != null;
		}
	}
}
