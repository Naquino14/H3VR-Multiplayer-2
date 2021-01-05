using UnityEngine;

internal class LowPassFilter
{
	private float y;

	private float a;

	private float s;

	private bool initialized;

	public LowPassFilter(float _alpha, float _initval = 0f)
	{
		y = (s = _initval);
		setAlpha(_alpha);
		initialized = false;
	}

	public void setAlpha(float _alpha)
	{
		if (_alpha <= 0f || _alpha > 1f)
		{
			Debug.LogError("alpha should be in (0.0., 1.0]");
		}
		else
		{
			a = _alpha;
		}
	}

	public float Filter(float _value)
	{
		float result;
		if (initialized)
		{
			result = a * _value + (1f - a) * s;
		}
		else
		{
			result = _value;
			initialized = true;
		}
		y = _value;
		s = result;
		return result;
	}

	public float filterWithAlpha(float _value, float _alpha)
	{
		setAlpha(_alpha);
		return Filter(_value);
	}

	public bool hasLastRawValue()
	{
		return initialized;
	}

	public float lastRawValue()
	{
		return y;
	}
}
