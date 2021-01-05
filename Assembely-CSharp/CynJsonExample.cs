using UnityEngine;

public class CynJsonExample : MonoBehaviour
{
	public string rootFolderName = "Gameplanner";

	public string catFolderName = "Scenarios";

	public string subFolderName = "TestLevel";

	public string baseFileName = "MyFile";

	public SBTeamDef teamDef;

	public SBSavedRange savedRange;

	[ContextMenu("Save")]
	private void Save()
	{
		CynJson.Save(rootFolderName, catFolderName, subFolderName, baseFileName + "_TeamDef.json", teamDef, out var errorMessage);
		MonoBehaviour.print(errorMessage);
		CynJson.Save(rootFolderName, catFolderName, subFolderName, baseFileName + "_SavedRange.json", savedRange, out errorMessage);
		MonoBehaviour.print(errorMessage);
	}

	[ContextMenu("Load")]
	private void Load()
	{
		CynJson.Load(rootFolderName, catFolderName, subFolderName, baseFileName + "_TeamDef.json", teamDef, out var errorMessage);
		MonoBehaviour.print(errorMessage);
		CynJson.Load(rootFolderName, catFolderName, subFolderName, baseFileName + "_SavedRange.json", savedRange, out errorMessage);
		MonoBehaviour.print(errorMessage);
	}

	[ContextMenu("Print")]
	private void PrintPaths()
	{
		string[] files = CynJson.GetFiles(rootFolderName, catFolderName, subFolderName, "_TeamDef.json");
		string[] array = files;
		foreach (string message in array)
		{
			MonoBehaviour.print(message);
		}
	}
}
