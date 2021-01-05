using System;
using System.Collections.Generic;
using UnityEngine;

public class AnvilCallback<TCall> : AnvilCallbackBase where TCall : UnityEngine.Object
{
	private AnvilCallback<AssetBundle> m_dependancy;

	private List<Action<TCall>> m_completed = new List<Action<TCall>>();

	private TCall m_result;

	public AsyncOperation Request
	{
		get;
		set;
	}

	public override float Progress
	{
		get
		{
			if (base.IsCompleted)
			{
				return 1f;
			}
			if (Request == null)
			{
				return 0f;
			}
			return Request.progress;
		}
	}

	public TCall Result
	{
		get
		{
			CompleteNow();
			return m_result;
		}
	}

	public override bool keepWaiting => !Pump();

	public AnvilCallback(AsyncOperation request, AnvilCallback<AssetBundle> bundle)
	{
		Request = request;
		m_dependancy = bundle;
	}

	public override bool Pump()
	{
		if (base.IsCompleted)
		{
			return true;
		}
		if (m_dependancy != null && !m_dependancy.IsCompleted)
		{
			return false;
		}
		if (Request == null || (!(Request is AnvilDummyOperation) && !Request.isDone))
		{
			return false;
		}
		CompleteNow();
		return true;
	}

	public void AddCallback(Action<TCall> completed)
	{
		if (completed != null)
		{
			if (!base.IsCompleted)
			{
				m_completed.Add(completed);
			}
			else
			{
				completed(m_result);
			}
		}
	}

	public override void CompleteNow()
	{
		if (base.IsCompleted)
		{
			return;
		}
		base.IsCompleted = true;
		if (m_dependancy != null)
		{
			m_dependancy.CompleteNow();
		}
		if (Request is AssetBundleRequest)
		{
			m_result = ((AssetBundleRequest)Request).asset as TCall;
		}
		else if (Request is AssetBundleCreateRequest)
		{
			m_result = ((AssetBundleCreateRequest)Request).assetBundle as TCall;
		}
		else if (Request is AnvilDummyOperation)
		{
			m_result = ((AnvilDummyOperation)Request).Result as TCall;
		}
		else
		{
			Debug.LogError("Anvil: Can't find load type! " + Request.GetType());
		}
		foreach (Action<TCall> item in m_completed)
		{
			item(m_result);
		}
		m_completed.Clear();
	}
}
