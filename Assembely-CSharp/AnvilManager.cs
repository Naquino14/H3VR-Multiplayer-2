using System.Collections;
using System.Collections.Generic;
using System.IO;
using Anvil;
using UnityEngine;

public class AnvilManager : MonoBehaviour
{
	private class LoadingHolder<TKey>
	{
		private Dictionary<TKey, AnvilCallbackBase> m_lookup = new Dictionary<TKey, AnvilCallbackBase>();

		private List<AnvilCallbackBase> m_loading = new List<AnvilCallbackBase>();

		public void Add(TKey key, AnvilCallbackBase cb)
		{
			m_lookup.Add(key, cb);
			m_loading.Add(cb);
		}

		public void PumpLoading()
		{
			for (int num = m_loading.Count - 1; num >= 0; num--)
			{
				if (m_loading[num].Pump())
				{
					m_loading.RemoveAt(num);
				}
			}
		}

		public bool TryGetValue(TKey key, out AnvilCallbackBase cb)
		{
			return m_lookup.TryGetValue(key, out cb);
		}
	}

	private static LoadingHolder<string> m_bundles = new LoadingHolder<string>();

	private static LoadingHolder<string> m_prewarms = new LoadingHolder<string>();

	private static LoadingHolder<AssetID> m_assets = new LoadingHolder<AssetID>();

	public static AnvilManager Instance;

	private const bool UseBundles = true;

	private void Awake()
	{
		Instance = this;
	}

	public static AnvilPrewarmCallback PreloadBundleAsync(string bundle)
	{
		return PreloadBundleAsyncInternal(bundle);
	}

	private static AnvilPrewarmCallback PreloadBundleAsyncInternal(string bundle)
	{
		if (m_prewarms.TryGetValue(bundle, out var cb))
		{
			return cb as AnvilPrewarmCallback;
		}
		cb = new AnvilPrewarmCallback(bundle);
		m_prewarms.Add(bundle, cb);
		return (AnvilPrewarmCallback)cb;
	}

	public static AnvilCallback<AssetBundle> GetBundleAsync(string bundle)
	{
		return GetAssetBundleAsyncInternal(bundle);
	}

	private static AnvilCallback<AssetBundle> GetAssetBundleAsyncInternal(string bundle)
	{
		if (m_bundles.TryGetValue(bundle, out var cb))
		{
			return cb as AnvilCallback<AssetBundle>;
		}
		string text = Path.Combine(Application.streamingAssetsPath, bundle);
		AsyncOperation request;
		if (!File.Exists(text))
		{
			Debug.LogError("Anvil: Couldn't find bundle " + text);
			request = new AnvilDummyOperation(null);
		}
		else
		{
			request = AssetBundle.LoadFromFileAsync(text);
		}
		cb = new AnvilCallback<AssetBundle>(request, null);
		m_bundles.Add(bundle, cb);
		return (AnvilCallback<AssetBundle>)cb;
	}

	public static AnvilCallback<GameObject> LoadAsync(AssetID assetID)
	{
		if (m_assets.TryGetValue(assetID, out var cb))
		{
			return cb as AnvilCallback<GameObject>;
		}
		AnvilCallback<AssetBundle> assetBundleAsyncInternal = GetAssetBundleAsyncInternal(assetID.Bundle);
		if (assetBundleAsyncInternal.IsCompleted)
		{
			cb = new AnvilCallback<GameObject>(null, null);
			((AnvilCallback<GameObject>)cb).Request = GetCallbackRequest(assetID, assetBundleAsyncInternal.Result);
		}
		else
		{
			cb = new AnvilCallback<GameObject>(null, assetBundleAsyncInternal);
			AnvilCallback<GameObject> tempCB = (AnvilCallback<GameObject>)cb;
			assetBundleAsyncInternal.AddCallback(delegate(AssetBundle bundle)
			{
				tempCB.Request = GetCallbackRequest(assetID, bundle);
			});
		}
		m_assets.Add(assetID, cb);
		return (AnvilCallback<GameObject>)cb;
	}

	private static AsyncOperation GetCallbackRequest(AssetID assetID, AssetBundle bundle)
	{
		if (bundle == null)
		{
			return new AnvilDummyOperation(null);
		}
		return bundle.LoadAssetAsync(assetID.AssetName, typeof(Object));
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			AnvilUpdate();
		}
	}

	public static void AnvilUpdate()
	{
		m_bundles.PumpLoading();
		m_assets.PumpLoading();
		m_prewarms.PumpLoading();
	}

	private IEnumerator RunDriven(IEnumerator routine)
	{
		while (routine.MoveNext())
		{
			if (routine.Current == null)
			{
				yield return null;
				continue;
			}
			CustomYieldInstruction cy = null;
			if (routine.Current is CustomYieldInstruction)
			{
				cy = (CustomYieldInstruction)routine.Current;
			}
			else if (routine.Current is AnvilAsset)
			{
				cy = ((AnvilAsset)routine.Current).GetGameObjectAsync();
			}
			if (cy != null && cy.keepWaiting)
			{
				yield return cy;
				continue;
			}
			IList asArray = routine.Current as IList;
			if (asArray == null || asArray.Count == 0)
			{
				continue;
			}
			if (asArray[0] is AnvilAsset)
			{
				for (int k = 0; k < asArray.Count; k++)
				{
					((AnvilAsset)asArray[k]).GetGameObjectAsync();
				}
				for (int j = 0; j < asArray.Count; j++)
				{
					AnvilCallback<GameObject> cb2 = ((AnvilAsset)asArray[j]).GetGameObjectAsync();
					if (cb2.keepWaiting)
					{
						yield return cb2;
					}
				}
			}
			else
			{
				if (!(asArray[0] is CustomYieldInstruction))
				{
					continue;
				}
				for (int i = 0; i < asArray.Count; i++)
				{
					CustomYieldInstruction cb = asArray[i] as CustomYieldInstruction;
					if (cb.keepWaiting)
					{
						yield return cb;
					}
				}
			}
		}
	}

	public static void Run(IEnumerator routine)
	{
		if (!Application.isPlaying)
		{
			Debug.LogError("Can't run Anvil routines out of playmode!");
		}
		else
		{
			Instance.StartCoroutine(Instance.RunDriven(routine));
		}
	}
}
