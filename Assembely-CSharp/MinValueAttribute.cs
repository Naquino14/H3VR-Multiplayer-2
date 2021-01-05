using UnityEngine;

public class MinValueAttribute : PropertyAttribute
{
	public float Min;

	public MinValueAttribute(float min)
	{
		Min = min;
	}
}
