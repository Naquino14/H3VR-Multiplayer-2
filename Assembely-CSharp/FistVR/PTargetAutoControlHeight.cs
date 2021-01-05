using UnityEngine;

namespace FistVR
{
	public class PTargetAutoControlHeight : MonoBehaviour
	{
		public Vector3 StartPos = new Vector3(0f, -1f, 0.677f);

		private float[] HeightThresholds = new float[4]
		{
			0.9f,
			3f,
			5.7f,
			10f
		};

		private float[] SetToHeights = new float[4]
		{
			-1f,
			0f,
			4f,
			5f
		};

		private int m_currentHeightIndex = -1;

		private void Start()
		{
		}

		private void Update()
		{
			int num = -1;
			for (int i = 0; i < 4; i++)
			{
				if (GM.CurrentPlayerBody.Head.position.y < HeightThresholds[i])
				{
					num = i;
					break;
				}
			}
			if (num >= 0 && num != m_currentHeightIndex)
			{
				m_currentHeightIndex = num;
				base.transform.position = new Vector3(StartPos.x, SetToHeights[m_currentHeightIndex], StartPos.z);
			}
		}
	}
}
