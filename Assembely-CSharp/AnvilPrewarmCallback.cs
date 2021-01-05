using System;
using System.Collections.Generic;
using Anvil;
using UnityEngine;

public class AnvilPrewarmCallback : AnvilCallbackBase
{
	private List<Action> m_completed = new List<Action>();

	private AnvilCallback<AssetBundle> m_bundleCallback;

	private AnvilCallback<GameObject>[] m_gameCallbacks;

	public override bool keepWaiting => !base.IsCompleted;

	public override float Progress
	{
		get
		{
			if (base.IsCompleted)
			{
				return 1f;
			}
			if (m_gameCallbacks == null)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < m_gameCallbacks.Length; i++)
			{
				num += m_gameCallbacks[i].Progress;
			}
			return num / (float)m_gameCallbacks.Length;
		}
	}

	public AnvilPrewarmCallback(string bundleName)
	{
		AnvilPrewarmCallback anvilPrewarmCallback = this;
		m_bundleCallback = AnvilManager.GetBundleAsync(bundleName);
		if (m_bundleCallback == null || m_bundleCallback.Request is AnvilDummyOperation)
		{
			base.IsCompleted = true;
			return;
		}
		m_bundleCallback.AddCallback(delegate(AssetBundle bundle)
		{
			string[] allAssetNames = bundle.GetAllAssetNames();
			anvilPrewarmCallback.m_gameCallbacks = new AnvilCallback<GameObject>[allAssetNames.Length];
			for (int i = 0; i < allAssetNames.Length; i++)
			{
				anvilPrewarmCallback.m_gameCallbacks[i] = AnvilManager.LoadAsync(new AssetID
				{
					AssetName = allAssetNames[i],
					Bundle = bundleName
				});
			}
		});
	}

	public override void CompleteNow()
	{
		if (base.IsCompleted)
		{
			return;
		}
		base.IsCompleted = true;
		m_bundleCallback.CompleteNow();
		for (int i = 0; i < m_gameCallbacks.Length; i++)
		{
			m_gameCallbacks[i].CompleteNow();
		}
		foreach (Action item in m_completed)
		{
			item();
		}
		m_completed.Clear();
	}

	public override bool Pump()
	{
		if (base.IsCompleted)
		{
			return true;
		}
		if (m_bundleCallback.Pump())
		{
			for (int i = 0; i < m_gameCallbacks.Length; i++)
			{
				if (!m_gameCallbacks[i].Pump())
				{
					return false;
				}
			}
		}
		CompleteNow();
		return true;
	}

	public void AddCallback(Action completed)
	{
		if (completed != null)
		{
			if (base.IsCompleted)
			{
				completed();
			}
			else
			{
				m_completed.Add(completed);
			}
		}
	}
}
