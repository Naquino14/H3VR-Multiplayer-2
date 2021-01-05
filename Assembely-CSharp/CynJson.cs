using System;
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
		string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
		string text = Path.Combine(folderPath, "My Games");
		Directory.CreateDirectory(text);
		string text2 = Path.Combine(text, "H3VR");
		Directory.CreateDirectory(text2);
		return text2;
	}

	public static bool FileExists(string rootFolderName, string catFolderName, string subFolderName, string fileNameWithExtension)
	{
		return FileExists(GetOrCreateH3VRDataPath(), rootFolderName, catFolderName, subFolderName);
	}

	public static bool FileExists(string dataPath, string rootFolderName, string catFolderName, string subFolderName, string fileNameWithExtension)
	{
		string path = Path.Combine(dataPath, rootFolderName);
		string path2 = Path.Combine(path, catFolderName);
		string path3 = Path.Combine(path2, subFolderName);
		string path4 = Path.Combine(path3, fileNameWithExtension);
		return File.Exists(path4);
	}

	public static string[] GetFiles(string rootFolderName, string catFolderName, string subFolderName, string suffixToTrim)
	{
		return GetFiles(GetOrCreateH3VRDataPath(), rootFolderName, catFolderName, subFolderName, suffixToTrim);
	}

	public static string[] GetFiles(string dataPath, string rootFolderName, string catFolderName, string subFolderName, string suffixToTrim)
	{
		string text2 = Path.Combine(dataPath, rootFolderName);
		if (!Directory.Exists(text2))
		{
			Debug.LogError("Root folder path: " + text2 + " does not exist.");
			return _emptyStringArray;
		}
		string text3 = Path.Combine(text2, catFolderName);
		if (!Directory.Exists(text3))
		{
			Debug.LogError("Cat folder path: " + text3 + " does not exist.");
			return _emptyStringArray;
		}
		string text4 = Path.Combine(text3, subFolderName);
		if (!Directory.Exists(text4))
		{
			Debug.LogError("Sub folder path: " + text4 + " does not exist.");
			return _emptyStringArray;
		}
		try
		{
			string[] files = Directory.GetFiles(text4);
			return (from text in files.Select(Path.GetFileName)
				where text.EndsWith(suffixToTrim)
				select text.Remove(text.LastIndexOf(suffixToTrim))).ToArray();
		}
		catch (UnauthorizedAccessException ex)
		{
			Debug.LogError("CynJson error: '" + ex.Message + "' in CynJson.GetFiles()");
			return _emptyStringArray;
		}
		catch (ArgumentException ex2)
		{
			Debug.LogError("CynJson error: '" + ex2.Message + "' in CynJson.GetFiles()");
			return _emptyStringArray;
		}
		catch (IOException ex3)
		{
			Debug.LogError("CynJson error: '" + ex3.Message + "' in CynJson.GetFiles()");
			return _emptyStringArray;
		}
	}

	public static bool Save(string rootFolderName, string catFolderName, string subFolderName, string fileNameWithExtension, object objectToSave, out string errorMessage)
	{
		return Save(GetOrCreateH3VRDataPath(), rootFolderName, catFolderName, subFolderName, fileNameWithExtension, objectToSave, out errorMessage);
	}

	public static bool Save(string rootFolderName, string catFolderName, string subFolderName, string fileNameWithExtension, string jsonToSave, out string errorMessage)
	{
		return Save(GetOrCreateH3VRDataPath(), rootFolderName, catFolderName, subFolderName, fileNameWithExtension, jsonToSave, out errorMessage);
	}

	public static bool Save(string dataPath, string rootFolderName, string catFolderName, string subFolderName, string fileNameWithExtension, object objectToSave, out string errorMessage)
	{
		return Save(dataPath, rootFolderName, catFolderName, subFolderName, fileNameWithExtension, JsonUtility.ToJson(objectToSave, prettyPrint: true), out errorMessage);
	}

	public static bool Save(string dataPath, string rootFolderName, string catFolderName, string subFolderName, string fileNameWithExtension, string jsonToSave, out string errorMessage)
	{
		string text = Path.Combine(dataPath, rootFolderName);
		Directory.CreateDirectory(text);
		string text2 = Path.Combine(text, catFolderName);
		Directory.CreateDirectory(text2);
		string text3 = Path.Combine(text2, subFolderName);
		Directory.CreateDirectory(text3);
		string path = Path.Combine(text3, fileNameWithExtension);
		try
		{
			File.WriteAllText(path, jsonToSave);
		}
		catch (ArgumentException ex)
		{
			errorMessage = "Save failed with error: " + ex.Message;
			Debug.LogError("CynJson error: " + errorMessage);
			return false;
		}
		catch (PathTooLongException)
		{
			errorMessage = "Path exceeds the maximum supported path length.";
			Debug.LogError("CynJson error: " + errorMessage);
			return false;
		}
		catch (DirectoryNotFoundException)
		{
			errorMessage = "The directory specified cannot be found.";
			Debug.LogError("CynJson error: " + errorMessage);
			return false;
		}
		catch (IOException ex4)
		{
			errorMessage = "Save failed with error: " + ex4.Message;
			Debug.LogError("CynJson error: " + errorMessage);
			return false;
		}
		catch (UnauthorizedAccessException)
		{
			errorMessage = "You do not have permission to write this file.";
			Debug.LogError("CynJson error: " + errorMessage);
			return false;
		}
		errorMessage = "Success!";
		return true;
	}

	public static bool Delete(string rootFolderName, string catFolderName, string subFolderName, string fileNameWithExtension, out string errorMessage)
	{
		return Delete(GetOrCreateH3VRDataPath(), rootFolderName, catFolderName, subFolderName, fileNameWithExtension, out errorMessage);
	}

	public static bool Delete(string dataPath, string rootFolderName, string catFolderName, string subFolderName, string fileNameWithExtension, out string errorMessage)
	{
		string path = Path.Combine(dataPath, rootFolderName);
		string path2 = Path.Combine(path, catFolderName);
		string path3 = Path.Combine(path2, subFolderName);
		string path4 = Path.Combine(path3, fileNameWithExtension);
		if (FileExists(dataPath, rootFolderName, catFolderName, subFolderName, fileNameWithExtension))
		{
			try
			{
				File.Delete(path4);
			}
			catch (ArgumentException ex)
			{
				errorMessage = "Save failed with error: " + ex.Message;
				Debug.LogError("CynJson error: " + errorMessage);
				return false;
			}
			catch (PathTooLongException)
			{
				errorMessage = "Path exceeds the maximum supported path length.";
				Debug.LogError("CynJson error: " + errorMessage);
				return false;
			}
			catch (DirectoryNotFoundException)
			{
				errorMessage = "The directory specified cannot be found.";
				Debug.LogError("CynJson error: " + errorMessage);
				return false;
			}
			catch (IOException ex4)
			{
				errorMessage = "Save failed with error: " + ex4.Message;
				Debug.LogError("CynJson error: " + errorMessage);
				return false;
			}
			catch (UnauthorizedAccessException)
			{
				errorMessage = "You do not have permission to write this file.";
				Debug.LogError("CynJson error: " + errorMessage);
				return false;
			}
		}
		errorMessage = "Success!";
		return true;
	}

	public static bool Load<T>(string rootFolderName, string catFolderName, string subFolderName, string fileNameWithExtension, T objectToOverwrite, out string errorMessage)
	{
		return Load(GetOrCreateH3VRDataPath(), rootFolderName, catFolderName, subFolderName, fileNameWithExtension, objectToOverwrite, out errorMessage);
	}

	public static bool Load<T>(string dataPath, string rootFolderName, string catFolderName, string subFolderName, string fileNameWithExtension, T objectToOverwrite, out string errorMessage)
	{
		string text = Path.Combine(dataPath, rootFolderName);
		if (!Directory.Exists(text))
		{
			errorMessage = "Root folder path: " + text + " does not exist.";
			Debug.LogError(errorMessage);
			objectToOverwrite = default(T);
			return false;
		}
		string text2 = Path.Combine(text, catFolderName);
		if (!Directory.Exists(text2))
		{
			errorMessage = "Cat folder path: " + text2 + " does not exist";
			Debug.LogError(errorMessage);
			objectToOverwrite = default(T);
			return false;
		}
		string text3 = Path.Combine(text2, subFolderName);
		if (!Directory.Exists(text3))
		{
			errorMessage = "Sub folder path: " + text3 + " does not exist";
			Debug.LogError(errorMessage);
			objectToOverwrite = default(T);
			return false;
		}
		string path = Path.Combine(text3, fileNameWithExtension);
		string empty = string.Empty;
		try
		{
			empty = File.ReadAllText(path);
		}
		catch (ArgumentException ex)
		{
			errorMessage = ex.Message;
			Debug.LogError("Load failed with error: " + errorMessage);
			objectToOverwrite = default(T);
			return false;
		}
		catch (PathTooLongException)
		{
			errorMessage = "Path exceeds the maximum supported path length.";
			Debug.LogError("CynJson error: " + errorMessage);
			objectToOverwrite = default(T);
			return false;
		}
		catch (DirectoryNotFoundException)
		{
			errorMessage = "The directory specified cannot be found.";
			Debug.LogError("CynJson error: " + errorMessage);
			objectToOverwrite = default(T);
			return false;
		}
		catch (IOException ex4)
		{
			errorMessage = "Load failed with error: " + ex4.Message;
			Debug.LogError("CynJson error: " + errorMessage);
			objectToOverwrite = default(T);
			return false;
		}
		catch (UnauthorizedAccessException)
		{
			errorMessage = "You do not have permission to read this file.";
			Debug.LogError("CynJson error: " + errorMessage);
			objectToOverwrite = default(T);
			return false;
		}
		errorMessage = "Success!";
		try
		{
			JsonUtility.FromJsonOverwrite(empty, objectToOverwrite);
		}
		catch (ArgumentException ex6)
		{
			if (ex6.Message == "JSON parse error: Invalid value.")
			{
				errorMessage = "File contains invalid JSON, did you modify the file incorrectly?";
				Debug.LogError("CynJson error: " + errorMessage);
			}
			else if (ex6.Message == "JSON parse error: Missing a name for object member.")
			{
				errorMessage = "File contains invalid JSON, did you modify the file incorrectly?";
				Debug.LogError("CynJson error: " + errorMessage);
			}
			else
			{
				errorMessage = ex6.Message;
				Debug.LogError("CynJson error: " + ex6.Message);
			}
			objectToOverwrite = default(T);
			return false;
		}
		return true;
	}
}
