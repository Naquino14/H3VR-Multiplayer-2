using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnvilImportPaths : ScriptableObject
{
	[SerializeField]
	private List<string> m_paths = new List<string>();

	[SerializeField]
	private List<string> m_bundleNames = new List<string>();

	private static AnvilImportPaths s_instance;

	public ShaderVariantCollection ShaderCollection;

	private void SetPathStates()
	{
		m_paths.Sort((string a, string b) => b.Length.CompareTo(a.Length));
		m_bundleNames = m_paths.Select((string p) => p.Replace('/', '_').ToLower()).ToList();
	}

	public void AddPath(string path)
	{
		m_paths.Add(path);
		SetPathStates();
	}

	public void RemovePath(string path)
	{
		m_paths.Remove(path);
		SetPathStates();
	}

	public string GetBundle(string path)
	{
		string result = "assets";
		for (int i = 0; i < m_paths.Count; i++)
		{
			string value = m_paths[i];
			if (path.StartsWith(value))
			{
				return m_bundleNames[i];
			}
		}
		return result;
	}

	public bool Contains(string assetPath)
	{
		return m_paths.Contains(assetPath);
	}
}
