// Decompiled with JetBrains decompiler
// Type: CynJson
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class CynJson
{
  private static readonly string[] _emptyStringArray = new string[0];
  public const string JSON_ERROR_PREFIX = "CynJson error: ";
  public const string JSON_SAVE_ERROR_PREFIX = "Save failed with error: ";
  public const string JSON_LOAD_ERROR_PREFIX = "Load failed with error: ";
  public const string ERROR_MESSAGE_DIRECTORYNOTFOUND = "The directory specified cannot be found.";
  public const string ERROR_MESSAGE_FILENOTFOUND = "The file specified cannot be found.";
  public const string ERROR_MESSAGE_PATHTOOLONG = "Path exceeds the maximum supported path length.";
  public const string ERROR_MESSAGE_UNAUTHORIZEDACCESS_READ = "You do not have permission to read this file.";
  public const string ERROR_MESSAGE_UNAUTHORIZEDACCESS_WRITE = "You do not have permission to write this file.";
  public const string ERROR_MESSAGE_SUCCESS = "Success!";

  public static string GetOrCreateH3VRDataPath()
  {
    string str = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "My Games");
    Directory.CreateDirectory(str);
    string path = Path.Combine(str, "H3VR");
    Directory.CreateDirectory(path);
    return path;
  }

  public static bool FileExists(
    string rootFolderName,
    string catFolderName,
    string subFolderName,
    string fileNameWithExtension)
  {
    return CynJson.FileExists(CynJson.GetOrCreateH3VRDataPath(), rootFolderName, catFolderName, subFolderName);
  }

  public static bool FileExists(
    string dataPath,
    string rootFolderName,
    string catFolderName,
    string subFolderName,
    string fileNameWithExtension)
  {
    return File.Exists(Path.Combine(Path.Combine(Path.Combine(Path.Combine(dataPath, rootFolderName), catFolderName), subFolderName), fileNameWithExtension));
  }

  public static string[] GetFiles(
    string rootFolderName,
    string catFolderName,
    string subFolderName,
    string suffixToTrim)
  {
    return CynJson.GetFiles(CynJson.GetOrCreateH3VRDataPath(), rootFolderName, catFolderName, subFolderName, suffixToTrim);
  }

  public static string[] GetFiles(
    string dataPath,
    string rootFolderName,
    string catFolderName,
    string subFolderName,
    string suffixToTrim)
  {
    string str1 = Path.Combine(dataPath, rootFolderName);
    if (!Directory.Exists(str1))
    {
      Debug.LogError((object) ("Root folder path: " + str1 + " does not exist."));
      return CynJson._emptyStringArray;
    }
    string str2 = Path.Combine(str1, catFolderName);
    if (!Directory.Exists(str2))
    {
      Debug.LogError((object) ("Cat folder path: " + str2 + " does not exist."));
      return CynJson._emptyStringArray;
    }
    string path = Path.Combine(str2, subFolderName);
    if (!Directory.Exists(path))
    {
      Debug.LogError((object) ("Sub folder path: " + path + " does not exist."));
      return CynJson._emptyStringArray;
    }
    try
    {
      string[] files = Directory.GetFiles(path);
      // ISSUE: reference to a compiler-generated field
      if (CynJson.\u003C\u003Ef__mg\u0024cache0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        CynJson.\u003C\u003Ef__mg\u0024cache0 = new Func<string, string>(Path.GetFileName);
      }
      // ISSUE: reference to a compiler-generated field
      Func<string, string> fMgCache0 = CynJson.\u003C\u003Ef__mg\u0024cache0;
      return ((IEnumerable<string>) files).Select<string, string>(fMgCache0).Where<string>((Func<string, bool>) (text => text.EndsWith(suffixToTrim))).Select<string, string>((Func<string, string>) (text => text.Remove(text.LastIndexOf(suffixToTrim)))).ToArray<string>();
    }
    catch (UnauthorizedAccessException ex)
    {
      Debug.LogError((object) ("CynJson error: '" + ex.Message + "' in CynJson.GetFiles()"));
      return CynJson._emptyStringArray;
    }
    catch (ArgumentException ex)
    {
      Debug.LogError((object) ("CynJson error: '" + ex.Message + "' in CynJson.GetFiles()"));
      return CynJson._emptyStringArray;
    }
    catch (IOException ex)
    {
      Debug.LogError((object) ("CynJson error: '" + ex.Message + "' in CynJson.GetFiles()"));
      return CynJson._emptyStringArray;
    }
  }

  public static bool Save(
    string rootFolderName,
    string catFolderName,
    string subFolderName,
    string fileNameWithExtension,
    object objectToSave,
    out string errorMessage)
  {
    return CynJson.Save(CynJson.GetOrCreateH3VRDataPath(), rootFolderName, catFolderName, subFolderName, fileNameWithExtension, objectToSave, out errorMessage);
  }

  public static bool Save(
    string rootFolderName,
    string catFolderName,
    string subFolderName,
    string fileNameWithExtension,
    string jsonToSave,
    out string errorMessage)
  {
    return CynJson.Save(CynJson.GetOrCreateH3VRDataPath(), rootFolderName, catFolderName, subFolderName, fileNameWithExtension, jsonToSave, out errorMessage);
  }

  public static bool Save(
    string dataPath,
    string rootFolderName,
    string catFolderName,
    string subFolderName,
    string fileNameWithExtension,
    object objectToSave,
    out string errorMessage)
  {
    return CynJson.Save(dataPath, rootFolderName, catFolderName, subFolderName, fileNameWithExtension, JsonUtility.ToJson(objectToSave, true), out errorMessage);
  }

  public static bool Save(
    string dataPath,
    string rootFolderName,
    string catFolderName,
    string subFolderName,
    string fileNameWithExtension,
    string jsonToSave,
    out string errorMessage)
  {
    string str1 = Path.Combine(dataPath, rootFolderName);
    Directory.CreateDirectory(str1);
    string str2 = Path.Combine(str1, catFolderName);
    Directory.CreateDirectory(str2);
    string str3 = Path.Combine(str2, subFolderName);
    Directory.CreateDirectory(str3);
    string path = Path.Combine(str3, fileNameWithExtension);
    try
    {
      File.WriteAllText(path, jsonToSave);
    }
    catch (ArgumentException ex)
    {
      errorMessage = "Save failed with error: " + ex.Message;
      Debug.LogError((object) ("CynJson error: " + errorMessage));
      return false;
    }
    catch (PathTooLongException ex)
    {
      errorMessage = "Path exceeds the maximum supported path length.";
      Debug.LogError((object) ("CynJson error: " + errorMessage));
      return false;
    }
    catch (DirectoryNotFoundException ex)
    {
      errorMessage = "The directory specified cannot be found.";
      Debug.LogError((object) ("CynJson error: " + errorMessage));
      return false;
    }
    catch (IOException ex)
    {
      errorMessage = "Save failed with error: " + ex.Message;
      Debug.LogError((object) ("CynJson error: " + errorMessage));
      return false;
    }
    catch (UnauthorizedAccessException ex)
    {
      errorMessage = "You do not have permission to write this file.";
      Debug.LogError((object) ("CynJson error: " + errorMessage));
      return false;
    }
    errorMessage = "Success!";
    return true;
  }

  public static bool Delete(
    string rootFolderName,
    string catFolderName,
    string subFolderName,
    string fileNameWithExtension,
    out string errorMessage)
  {
    return CynJson.Delete(CynJson.GetOrCreateH3VRDataPath(), rootFolderName, catFolderName, subFolderName, fileNameWithExtension, out errorMessage);
  }

  public static bool Delete(
    string dataPath,
    string rootFolderName,
    string catFolderName,
    string subFolderName,
    string fileNameWithExtension,
    out string errorMessage)
  {
    string path = Path.Combine(Path.Combine(Path.Combine(Path.Combine(dataPath, rootFolderName), catFolderName), subFolderName), fileNameWithExtension);
    if (CynJson.FileExists(dataPath, rootFolderName, catFolderName, subFolderName, fileNameWithExtension))
    {
      try
      {
        File.Delete(path);
      }
      catch (ArgumentException ex)
      {
        errorMessage = "Save failed with error: " + ex.Message;
        Debug.LogError((object) ("CynJson error: " + errorMessage));
        return false;
      }
      catch (PathTooLongException ex)
      {
        errorMessage = "Path exceeds the maximum supported path length.";
        Debug.LogError((object) ("CynJson error: " + errorMessage));
        return false;
      }
      catch (DirectoryNotFoundException ex)
      {
        errorMessage = "The directory specified cannot be found.";
        Debug.LogError((object) ("CynJson error: " + errorMessage));
        return false;
      }
      catch (IOException ex)
      {
        errorMessage = "Save failed with error: " + ex.Message;
        Debug.LogError((object) ("CynJson error: " + errorMessage));
        return false;
      }
      catch (UnauthorizedAccessException ex)
      {
        errorMessage = "You do not have permission to write this file.";
        Debug.LogError((object) ("CynJson error: " + errorMessage));
        return false;
      }
    }
    errorMessage = "Success!";
    return true;
  }

  public static bool Load<T>(
    string rootFolderName,
    string catFolderName,
    string subFolderName,
    string fileNameWithExtension,
    T objectToOverwrite,
    out string errorMessage)
  {
    return CynJson.Load<T>(CynJson.GetOrCreateH3VRDataPath(), rootFolderName, catFolderName, subFolderName, fileNameWithExtension, objectToOverwrite, out errorMessage);
  }

  public static bool Load<T>(
    string dataPath,
    string rootFolderName,
    string catFolderName,
    string subFolderName,
    string fileNameWithExtension,
    T objectToOverwrite,
    out string errorMessage)
  {
    string str1 = Path.Combine(dataPath, rootFolderName);
    if (!Directory.Exists(str1))
    {
      errorMessage = "Root folder path: " + str1 + " does not exist.";
      Debug.LogError((object) errorMessage);
      objectToOverwrite = default (T);
      return false;
    }
    string str2 = Path.Combine(str1, catFolderName);
    if (!Directory.Exists(str2))
    {
      errorMessage = "Cat folder path: " + str2 + " does not exist";
      Debug.LogError((object) errorMessage);
      objectToOverwrite = default (T);
      return false;
    }
    string str3 = Path.Combine(str2, subFolderName);
    if (!Directory.Exists(str3))
    {
      errorMessage = "Sub folder path: " + str3 + " does not exist";
      Debug.LogError((object) errorMessage);
      objectToOverwrite = default (T);
      return false;
    }
    string path = Path.Combine(str3, fileNameWithExtension);
    string empty = string.Empty;
    string json;
    try
    {
      json = File.ReadAllText(path);
    }
    catch (ArgumentException ex)
    {
      errorMessage = ex.Message;
      Debug.LogError((object) ("Load failed with error: " + errorMessage));
      objectToOverwrite = default (T);
      return false;
    }
    catch (PathTooLongException ex)
    {
      errorMessage = "Path exceeds the maximum supported path length.";
      Debug.LogError((object) ("CynJson error: " + errorMessage));
      objectToOverwrite = default (T);
      return false;
    }
    catch (DirectoryNotFoundException ex)
    {
      errorMessage = "The directory specified cannot be found.";
      Debug.LogError((object) ("CynJson error: " + errorMessage));
      objectToOverwrite = default (T);
      return false;
    }
    catch (IOException ex)
    {
      errorMessage = "Load failed with error: " + ex.Message;
      Debug.LogError((object) ("CynJson error: " + errorMessage));
      objectToOverwrite = default (T);
      return false;
    }
    catch (UnauthorizedAccessException ex)
    {
      errorMessage = "You do not have permission to read this file.";
      Debug.LogError((object) ("CynJson error: " + errorMessage));
      objectToOverwrite = default (T);
      return false;
    }
    errorMessage = "Success!";
    try
    {
      JsonUtility.FromJsonOverwrite(json, (object) objectToOverwrite);
    }
    catch (ArgumentException ex)
    {
      if (ex.Message == "JSON parse error: Invalid value.")
      {
        errorMessage = "File contains invalid JSON, did you modify the file incorrectly?";
        Debug.LogError((object) ("CynJson error: " + errorMessage));
      }
      else if (ex.Message == "JSON parse error: Missing a name for object member.")
      {
        errorMessage = "File contains invalid JSON, did you modify the file incorrectly?";
        Debug.LogError((object) ("CynJson error: " + errorMessage));
      }
      else
      {
        errorMessage = ex.Message;
        Debug.LogError((object) ("CynJson error: " + ex.Message));
      }
      objectToOverwrite = default (T);
      return false;
    }
    return true;
  }
}
