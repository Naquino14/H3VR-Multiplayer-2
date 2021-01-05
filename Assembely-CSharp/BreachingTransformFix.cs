using System.Collections.Generic;
using UnityEngine;

public class BreachingTransformFix : MonoBehaviour
{
	public List<Transform> refs;

	public List<Transform> points;

	[ContextMenu("DoIt")]
	public void DoIt()
	{
		for (int i = 0; i < points.Count; i++)
		{
			points[i].position = refs[i].position;
			points[i].rotation = refs[i].rotation;
		}
	}
}
