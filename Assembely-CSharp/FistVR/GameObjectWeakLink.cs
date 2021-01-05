using System;
using System.IO;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public struct GameObjectWeakLink
	{
		private GameObject m_goCache;

		[SerializeField]
		private string m_assetGUID;

		[SerializeField]
		private string m_filePath;

		public string FilePath => m_filePath;

		public GameObject GameObject
		{
			get
			{
				if (string.IsNullOrEmpty(m_assetGUID))
				{
					return null;
				}
				if (m_goCache == null)
				{
					m_goCache = Resources.Load<GameObject>(m_filePath);
					if (m_goCache == null)
					{
						Debug.LogError("Weak linked game object not found!");
					}
				}
				return m_goCache;
			}
		}

		public static string ToResourcesPath(string assetPath)
		{
			if (assetPath == string.Empty)
			{
				return string.Empty;
			}
			int num = assetPath.IndexOf("/Resources/", StringComparison.Ordinal);
			if (num == -1)
			{
				return string.Empty;
			}
			assetPath = assetPath.Substring(num + "/Resources/".Length);
			assetPath = Path.GetDirectoryName(assetPath) + "/" + Path.GetFileNameWithoutExtension(assetPath);
			return assetPath;
		}

		public void OnBuild()
		{
		}

		public void Preload()
		{
			if (m_goCache == null)
			{
				m_goCache = Resources.Load<GameObject>(m_filePath);
			}
		}
	}
}
