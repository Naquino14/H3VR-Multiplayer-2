using System;
using UnityEngine;

internal class OneEuroFilter
{
	private float freq;

	private float mincutoff;

	private float beta;

	private float dcutoff;

	private LowPassFilter x;

	private LowPassFilter dx;

	private float lasttime;

	public float currValue
	{
		get;
		protected set;
	}

	public float prevValue
	{
		get;
		protected set;
	}

	public OneEuroFilter(float _freq, float _mincutoff = 1f, float _beta = 0f, float _dcutoff = 1f)
	{
		setFrequency(_freq);
		setMinCutoff(_mincutoff);
		setBeta(_beta);
		setDerivateCutoff(_dcutoff);
		x = new LowPassFilter(alpha(mincutoff));
		dx = new LowPassFilter(alpha(dcutoff));
		lasttime = -1f;
		currValue = 0f;
		prevValue = currValue;
	}

	private float alpha(float _cutoff)
	{
		float num = 1f / freq;
		float num2 = 1f / ((float)Math.PI * 2f * _cutoff);
		return 1f / (1f + num2 / num);
	}

	private void setFrequency(float _f)
	{
		if (_f <= 0f)
		{
			Debug.LogError("freq should be > 0");
		}
		else
		{
			freq = _f;
		}
	}

	private void setMinCutoff(float _mc)
	{
		if (_mc <= 0f)
		{
			Debug.LogError("mincutoff should be > 0");
		}
		else
		{
			mincutoff = _mc;
		}
	}

	private void setBeta(float _b)
	{
		beta = _b;
	}

	private void setDerivateCutoff(float _dc)
	{
		if (_dc <= 0f)
		{
			Debug.LogError("dcutoff should be > 0");
		}
		else
		{
			dcutoff = _dc;
		}
	}

	public void UpdateParams(float _freq, float _mincutoff, float _beta, float _dcutoff)
	{
		setFrequency(_freq);
		setMinCutoff(_mincutoff);
		setBeta(_beta);
		setDerivateCutoff(_dcutoff);
		x.setAlpha(alpha(mincutoff));
		dx.setAlpha(alpha(dcutoff));
	}

	public float Filter(float value, float timestamp = -1f)
	{
		prevValue = currValue;
		if (lasttime != -1f && timestamp != -1f)
		{
			freq = 1f / (timestamp - lasttime);
		}
		lasttime = timestamp;
		float value2 = ((!x.hasLastRawValue()) ? 0f : ((value - x.lastRawValue()) * freq));
		float f = dx.filterWithAlpha(value2, alpha(dcutoff));
		float cutoff = mincutoff + beta * Mathf.Abs(f);
		currValue = x.filterWithAlpha(value, alpha(cutoff));
		return currValue;
	}
}
public class OneEuroFilter<T> where T : struct
{
	private Type type;

	private OneEuroFilter[] oneEuroFilters;

	public float freq
	{
		get;
		protected set;
	}

	public float mincutoff
	{
		get;
		protected set;
	}

	public float beta
	{
		get;
		protected set;
	}

	public float dcutoff
	{
		get;
		protected set;
	}

	public T currValue
	{
		get;
		protected set;
	}

	public T prevValue
	{
		get;
		protected set;
	}

	public OneEuroFilter(float _freq, float _mincutoff = 1f, float _beta = 0f, float _dcutoff = 1f)
	{
		type = typeof(T);
		currValue = new T();
		prevValue = new T();
		freq = _freq;
		mincutoff = _mincutoff;
		beta = _beta;
		dcutoff = _dcutoff;
		if (type == typeof(Vector2))
		{
			oneEuroFilters = new OneEuroFilter[2];
		}
		else if (type == typeof(Vector3))
		{
			oneEuroFilters = new OneEuroFilter[3];
		}
		else
		{
			if (type != typeof(Vector4) && type != typeof(Quaternion))
			{
				Debug.LogError(string.Concat(type, " is not a supported type"));
				return;
			}
			oneEuroFilters = new OneEuroFilter[4];
		}
		for (int i = 0; i < oneEuroFilters.Length; i++)
		{
			oneEuroFilters[i] = new OneEuroFilter(freq, mincutoff, beta, dcutoff);
		}
	}

	public void UpdateParams(float _freq, float _mincutoff = 1f, float _beta = 0f, float _dcutoff = 1f)
	{
		freq = _freq;
		mincutoff = _mincutoff;
		beta = _beta;
		dcutoff = _dcutoff;
		for (int i = 0; i < oneEuroFilters.Length; i++)
		{
			oneEuroFilters[i].UpdateParams(freq, mincutoff, beta, dcutoff);
		}
	}

	public T Filter<U>(U _value, float timestamp = -1f) where U : struct
	{
		prevValue = currValue;
		if (typeof(U) != type)
		{
			Debug.LogError(string.Concat("WARNING! ", typeof(U), " when ", type, " is expected!\nReturning previous filtered value"));
			currValue = prevValue;
			return (T)Convert.ChangeType(currValue, typeof(T));
		}
		if (type == typeof(Vector2))
		{
			Vector2 zero = Vector2.zero;
			Vector2 vector = (Vector2)Convert.ChangeType(_value, typeof(Vector2));
			for (int i = 0; i < oneEuroFilters.Length; i++)
			{
				zero[i] = oneEuroFilters[i].Filter(vector[i], timestamp);
			}
			currValue = (T)Convert.ChangeType(zero, typeof(T));
		}
		else if (type == typeof(Vector3))
		{
			Vector3 zero2 = Vector3.zero;
			Vector3 vector2 = (Vector3)Convert.ChangeType(_value, typeof(Vector3));
			for (int j = 0; j < oneEuroFilters.Length; j++)
			{
				zero2[j] = oneEuroFilters[j].Filter(vector2[j], timestamp);
			}
			currValue = (T)Convert.ChangeType(zero2, typeof(T));
		}
		else if (type == typeof(Vector4))
		{
			Vector4 zero3 = Vector4.zero;
			Vector4 vector3 = (Vector4)Convert.ChangeType(_value, typeof(Vector4));
			for (int k = 0; k < oneEuroFilters.Length; k++)
			{
				zero3[k] = oneEuroFilters[k].Filter(vector3[k], timestamp);
			}
			currValue = (T)Convert.ChangeType(zero3, typeof(T));
		}
		else
		{
			Quaternion identity = Quaternion.identity;
			Quaternion quaternion = (Quaternion)Convert.ChangeType(_value, typeof(Quaternion));
			for (int l = 0; l < oneEuroFilters.Length; l++)
			{
				identity[l] = oneEuroFilters[l].Filter(quaternion[l], timestamp);
			}
			currValue = (T)Convert.ChangeType(identity, typeof(T));
		}
		return (T)Convert.ChangeType(currValue, typeof(T));
	}
}
