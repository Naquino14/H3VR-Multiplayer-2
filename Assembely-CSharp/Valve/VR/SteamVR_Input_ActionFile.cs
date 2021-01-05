using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Valve.Newtonsoft.Json;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Input_ActionFile
	{
		public List<SteamVR_Input_ActionFile_Action> actions = new List<SteamVR_Input_ActionFile_Action>();

		public List<SteamVR_Input_ActionFile_ActionSet> action_sets = new List<SteamVR_Input_ActionFile_ActionSet>();

		public List<SteamVR_Input_ActionFile_DefaultBinding> default_bindings = new List<SteamVR_Input_ActionFile_DefaultBinding>();

		public List<Dictionary<string, string>> localization = new List<Dictionary<string, string>>();

		[JsonIgnore]
		public List<SteamVR_Input_ActionFile_LocalizationItem> localizationHelperList = new List<SteamVR_Input_ActionFile_LocalizationItem>();

		private const string findString_appKeyStart = "\"app_key\"";

		private const string findString_appKeyEnd = "\",";

		public void InitializeHelperLists()
		{
			foreach (SteamVR_Input_ActionFile_ActionSet actionset in action_sets)
			{
				actionset.actionsInList = new List<SteamVR_Input_ActionFile_Action>(actions.Where((SteamVR_Input_ActionFile_Action action) => action.path.StartsWith(actionset.name) && SteamVR_Input_ActionFile_ActionTypes.listIn.Contains(action.type)));
				actionset.actionsOutList = new List<SteamVR_Input_ActionFile_Action>(actions.Where((SteamVR_Input_ActionFile_Action action) => action.path.StartsWith(actionset.name) && SteamVR_Input_ActionFile_ActionTypes.listOut.Contains(action.type)));
				actionset.actionsList = new List<SteamVR_Input_ActionFile_Action>(actions.Where((SteamVR_Input_ActionFile_Action action) => action.path.StartsWith(actionset.name)));
			}
			foreach (Dictionary<string, string> item in localization)
			{
				localizationHelperList.Add(new SteamVR_Input_ActionFile_LocalizationItem(item));
			}
		}

		public void SaveHelperLists()
		{
			foreach (SteamVR_Input_ActionFile_ActionSet action_set in action_sets)
			{
				action_set.actionsList.Clear();
				action_set.actionsList.AddRange(action_set.actionsInList);
				action_set.actionsList.AddRange(action_set.actionsOutList);
			}
			actions.Clear();
			foreach (SteamVR_Input_ActionFile_ActionSet action_set2 in action_sets)
			{
				actions.AddRange(action_set2.actionsInList);
				actions.AddRange(action_set2.actionsOutList);
			}
			localization.Clear();
			foreach (SteamVR_Input_ActionFile_LocalizationItem localizationHelper in localizationHelperList)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("language_tag", localizationHelper.language);
				foreach (KeyValuePair<string, string> item in localizationHelper.items)
				{
					dictionary.Add(item.Key, item.Value);
				}
				localization.Add(dictionary);
			}
		}

		public static string GetShortName(string name)
		{
			string text = name;
			int num = text.LastIndexOf('/');
			if (num != -1)
			{
				if (num == text.Length - 1)
				{
					text = text.Remove(num);
					num = text.LastIndexOf('/');
					if (num == -1)
					{
						return GetCodeFriendlyName(text);
					}
				}
				return GetCodeFriendlyName(text.Substring(num + 1));
			}
			return GetCodeFriendlyName(text);
		}

		public static string GetCodeFriendlyName(string name)
		{
			name = name.Replace('/', '_').Replace(' ', '_');
			if (!char.IsLetter(name[0]))
			{
				name = "_" + name;
			}
			for (int i = 0; i < name.Length; i++)
			{
				if (!char.IsLetterOrDigit(name[i]) && name[i] != '_')
				{
					name = name.Remove(i, 1);
					name = name.Insert(i, "_");
				}
			}
			return name;
		}

		public string[] GetFilesToCopy(bool throwErrors = false)
		{
			List<string> list = new List<string>();
			FileInfo fileInfo = new FileInfo(SteamVR_Input.actionsFilePath);
			string fullName = fileInfo.Directory.FullName;
			list.Add(SteamVR_Input.actionsFilePath);
			foreach (SteamVR_Input_ActionFile_DefaultBinding default_binding in default_bindings)
			{
				string text = Path.Combine(fullName, default_binding.binding_url);
				if (File.Exists(text))
				{
					list.Add(text);
				}
				else if (throwErrors)
				{
					Debug.LogError("<b>[SteamVR]</b> Could not bind binding file specified by the actions.json manifest: " + text);
				}
			}
			return list.ToArray();
		}

		public void CopyFilesToPath(string toPath, bool overwrite)
		{
			string[] filesToCopy = SteamVR_Input.actionFile.GetFilesToCopy();
			string[] array = filesToCopy;
			foreach (string text in array)
			{
				FileInfo fileInfo = new FileInfo(text);
				string text2 = Path.Combine(toPath, fileInfo.Name);
				bool flag = false;
				if (File.Exists(text2))
				{
					flag = true;
				}
				if (flag)
				{
					if (overwrite)
					{
						FileInfo fileInfo2 = new FileInfo(text2);
						fileInfo2.IsReadOnly = false;
						fileInfo2.Delete();
						File.Copy(text, text2);
						RemoveAppKey(text2);
						Debug.Log("<b>[SteamVR]</b> Copied (overwrote) SteamVR Input file at path: " + text2);
					}
					else
					{
						Debug.Log("<b>[SteamVR]</b> Skipped writing existing file at path: " + text2);
					}
				}
				else
				{
					File.Copy(text, text2);
					RemoveAppKey(text2);
					Debug.Log("<b>[SteamVR]</b> Copied SteamVR Input file to folder: " + text2);
				}
			}
		}

		public bool IsInStreamingAssets()
		{
			return SteamVR_Input.actionsFilePath.Contains("StreamingAssets");
		}

		private static void RemoveAppKey(string newFilePath)
		{
			if (!File.Exists(newFilePath))
			{
				return;
			}
			string text = File.ReadAllText(newFilePath);
			string value = "\"app_key\"";
			int num = text.IndexOf(value);
			if (num != -1)
			{
				int num2 = text.IndexOf("\",", num);
				if (num2 != -1)
				{
					num2 += "\",".Length;
					int count = num2 - num;
					string contents = text.Remove(num, count);
					FileInfo fileInfo = new FileInfo(newFilePath);
					fileInfo.IsReadOnly = false;
					File.WriteAllText(newFilePath, contents);
				}
			}
		}

		public void Save(string path)
		{
			FileInfo fileInfo = new FileInfo(path);
			if (fileInfo.Exists)
			{
				fileInfo.IsReadOnly = false;
			}
			string contents = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			});
			File.WriteAllText(path, contents);
		}
	}
}
