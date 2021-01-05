using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Input_ActionFile_LocalizationItem
	{
		public const string languageTagKeyName = "language_tag";

		public string language;

		public Dictionary<string, string> items = new Dictionary<string, string>();

		public SteamVR_Input_ActionFile_LocalizationItem(string newLanguage)
		{
			language = newLanguage;
		}

		public SteamVR_Input_ActionFile_LocalizationItem(Dictionary<string, string> dictionary)
		{
			if (dictionary == null)
			{
				return;
			}
			if (dictionary.ContainsKey("language_tag"))
			{
				language = dictionary["language_tag"];
			}
			else
			{
				Debug.Log("<b>[SteamVR]</b> Input: Error in actions file, no language_tag in localization array item.");
			}
			foreach (KeyValuePair<string, string> item in dictionary)
			{
				if (item.Key != "language_tag")
				{
					items.Add(item.Key, item.Value);
				}
			}
		}
	}
}
