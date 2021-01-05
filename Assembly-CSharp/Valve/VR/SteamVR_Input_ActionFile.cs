// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Input_ActionFile
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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
      foreach (SteamVR_Input_ActionFile_ActionSet actionSet in this.action_sets)
      {
        SteamVR_Input_ActionFile_ActionSet actionset = actionSet;
        actionset.actionsInList = new List<SteamVR_Input_ActionFile_Action>(this.actions.Where<SteamVR_Input_ActionFile_Action>((Func<SteamVR_Input_ActionFile_Action, bool>) (action => action.path.StartsWith(actionset.name) && ((IEnumerable<string>) SteamVR_Input_ActionFile_ActionTypes.listIn).Contains<string>(action.type))));
        actionset.actionsOutList = new List<SteamVR_Input_ActionFile_Action>(this.actions.Where<SteamVR_Input_ActionFile_Action>((Func<SteamVR_Input_ActionFile_Action, bool>) (action => action.path.StartsWith(actionset.name) && ((IEnumerable<string>) SteamVR_Input_ActionFile_ActionTypes.listOut).Contains<string>(action.type))));
        actionset.actionsList = new List<SteamVR_Input_ActionFile_Action>(this.actions.Where<SteamVR_Input_ActionFile_Action>((Func<SteamVR_Input_ActionFile_Action, bool>) (action => action.path.StartsWith(actionset.name))));
      }
      foreach (Dictionary<string, string> dictionary in this.localization)
        this.localizationHelperList.Add(new SteamVR_Input_ActionFile_LocalizationItem(dictionary));
    }

    public void SaveHelperLists()
    {
      foreach (SteamVR_Input_ActionFile_ActionSet actionSet in this.action_sets)
      {
        actionSet.actionsList.Clear();
        actionSet.actionsList.AddRange((IEnumerable<SteamVR_Input_ActionFile_Action>) actionSet.actionsInList);
        actionSet.actionsList.AddRange((IEnumerable<SteamVR_Input_ActionFile_Action>) actionSet.actionsOutList);
      }
      this.actions.Clear();
      foreach (SteamVR_Input_ActionFile_ActionSet actionSet in this.action_sets)
      {
        this.actions.AddRange((IEnumerable<SteamVR_Input_ActionFile_Action>) actionSet.actionsInList);
        this.actions.AddRange((IEnumerable<SteamVR_Input_ActionFile_Action>) actionSet.actionsOutList);
      }
      this.localization.Clear();
      foreach (SteamVR_Input_ActionFile_LocalizationItem localizationHelper in this.localizationHelperList)
      {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        dictionary.Add("language_tag", localizationHelper.language);
        foreach (KeyValuePair<string, string> keyValuePair in localizationHelper.items)
          dictionary.Add(keyValuePair.Key, keyValuePair.Value);
        this.localization.Add(dictionary);
      }
    }

    public static string GetShortName(string name)
    {
      string name1 = name;
      int startIndex = name1.LastIndexOf('/');
      if (startIndex == -1)
        return SteamVR_Input_ActionFile.GetCodeFriendlyName(name1);
      if (startIndex == name1.Length - 1)
      {
        name1 = name1.Remove(startIndex);
        startIndex = name1.LastIndexOf('/');
        if (startIndex == -1)
          return SteamVR_Input_ActionFile.GetCodeFriendlyName(name1);
      }
      return SteamVR_Input_ActionFile.GetCodeFriendlyName(name1.Substring(startIndex + 1));
    }

    public static string GetCodeFriendlyName(string name)
    {
      name = name.Replace('/', '_').Replace(' ', '_');
      if (!char.IsLetter(name[0]))
        name = "_" + name;
      for (int index = 0; index < name.Length; ++index)
      {
        if (!char.IsLetterOrDigit(name[index]) && name[index] != '_')
        {
          name = name.Remove(index, 1);
          name = name.Insert(index, "_");
        }
      }
      return name;
    }

    public string[] GetFilesToCopy(bool throwErrors = false)
    {
      List<string> stringList = new List<string>();
      string fullName = new FileInfo(SteamVR_Input.actionsFilePath).Directory.FullName;
      stringList.Add(SteamVR_Input.actionsFilePath);
      foreach (SteamVR_Input_ActionFile_DefaultBinding defaultBinding in this.default_bindings)
      {
        string path = Path.Combine(fullName, defaultBinding.binding_url);
        if (File.Exists(path))
          stringList.Add(path);
        else if (throwErrors)
          Debug.LogError((object) ("<b>[SteamVR]</b> Could not bind binding file specified by the actions.json manifest: " + path));
      }
      return stringList.ToArray();
    }

    public void CopyFilesToPath(string toPath, bool overwrite)
    {
      foreach (string str1 in SteamVR_Input.actionFile.GetFilesToCopy())
      {
        FileInfo fileInfo = new FileInfo(str1);
        string str2 = Path.Combine(toPath, fileInfo.Name);
        bool flag = false;
        if (File.Exists(str2))
          flag = true;
        if (flag)
        {
          if (overwrite)
          {
            new FileInfo(str2) { IsReadOnly = false }.Delete();
            File.Copy(str1, str2);
            SteamVR_Input_ActionFile.RemoveAppKey(str2);
            Debug.Log((object) ("<b>[SteamVR]</b> Copied (overwrote) SteamVR Input file at path: " + str2));
          }
          else
            Debug.Log((object) ("<b>[SteamVR]</b> Skipped writing existing file at path: " + str2));
        }
        else
        {
          File.Copy(str1, str2);
          SteamVR_Input_ActionFile.RemoveAppKey(str2);
          Debug.Log((object) ("<b>[SteamVR]</b> Copied SteamVR Input file to folder: " + str2));
        }
      }
    }

    public bool IsInStreamingAssets() => SteamVR_Input.actionsFilePath.Contains("StreamingAssets");

    private static void RemoveAppKey(string newFilePath)
    {
      if (!File.Exists(newFilePath))
        return;
      string str1 = File.ReadAllText(newFilePath);
      string str2 = "\"app_key\"";
      int startIndex = str1.IndexOf(str2);
      if (startIndex == -1)
        return;
      int num = str1.IndexOf("\",", startIndex);
      if (num == -1)
        return;
      int count = num + "\",".Length - startIndex;
      string contents = str1.Remove(startIndex, count);
      new FileInfo(newFilePath).IsReadOnly = false;
      File.WriteAllText(newFilePath, contents);
    }

    public void Save(string path)
    {
      FileInfo fileInfo = new FileInfo(path);
      if (fileInfo.Exists)
        fileInfo.IsReadOnly = false;
      string contents = JsonConvert.SerializeObject((object) this, Formatting.Indented, new JsonSerializerSettings()
      {
        NullValueHandling = NullValueHandling.Ignore
      });
      File.WriteAllText(path, contents);
    }
  }
}
