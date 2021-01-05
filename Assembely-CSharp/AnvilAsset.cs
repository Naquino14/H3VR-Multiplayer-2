using Anvil;
using UnityEngine;

public abstract class AnvilAsset : ScriptableObject
{
	[SerializeField]
	private AssetID m_anvilPrefab;

	private AnvilCallback<GameObject> m_loadingState;

	public string Guid => m_anvilPrefab.Guid;

	public GameObject GetGameObject()
	{
		if (m_loadingState == null)
		{
			m_loadingState = AnvilManager.LoadAsync(m_anvilPrefab);
		}
		return m_loadingState.Result;
	}

	public void RefreshCache()
	{
		m_loadingState = AnvilManager.LoadAsync(m_anvilPrefab);
	}

	public AnvilCallback<GameObject> GetGameObjectAsync()
	{
		if (m_loadingState == null)
		{
			m_loadingState = AnvilManager.LoadAsync(m_anvilPrefab);
		}
		return m_loadingState;
	}

	public void UpgradeFrom(GameObject go)
	{
	}
}
