using System.Collections.Generic;
using UnityEngine;

public class ftLocalStorage : ScriptableObject
{
	[SerializeField]
	public List<string> modifiedAssetPathList = new List<string>();

	[SerializeField]
	public List<int> modifiedAssetPaddingHash = new List<int>();
}
