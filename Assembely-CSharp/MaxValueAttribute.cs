using UnityEngine;

public class MaxValueAttribute : PropertyAttribute
{
	public float Max;

	public MaxValueAttribute(float min)
	{
		Max = min;
	}
}
