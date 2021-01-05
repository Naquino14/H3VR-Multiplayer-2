using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class SamplerPlatter_HelpScreenPlacer : MonoBehaviour
	{
		private int m_closestIndex;

		public Transform Screen;

		public List<Transform> ScreenPoints;

		private void Update()
		{
			float num = Vector3.Distance(GM.CurrentPlayerBody.transform.position, ScreenPoints[0].position);
			int num2 = 0;
			for (int i = 1; i < ScreenPoints.Count; i++)
			{
				float num3 = Vector3.Distance(GM.CurrentPlayerBody.transform.position, ScreenPoints[i].position);
				if (num3 < num)
				{
					num = num3;
					num2 = i;
				}
			}
			if (num2 != m_closestIndex)
			{
				m_closestIndex = num2;
				Screen.position = ScreenPoints[m_closestIndex].position;
				Screen.rotation = ScreenPoints[m_closestIndex].rotation;
			}
		}
	}
}
