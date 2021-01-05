namespace Valve.VR
{
	public class SteamVR_Windows_Editor_Helper
	{
		public enum BrowserApplication
		{
			Unknown,
			InternetExplorer,
			Firefox,
			Chrome,
			Opera,
			Safari,
			Edge
		}

		public static BrowserApplication GetDefaultBrowser()
		{
			return BrowserApplication.Firefox;
		}
	}
}
