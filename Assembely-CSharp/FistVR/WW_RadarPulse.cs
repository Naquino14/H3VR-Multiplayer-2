using UnityEngine;

namespace FistVR
{
	public class WW_RadarPulse : MonoBehaviour
	{
		public AnimationCurve XZSize;

		public AnimationCurve YSize;

		private float m_pulseTick;

		private float m_pulseSPeed = 0.4f;

		private void Start()
		{
		}

		private void Update()
		{
			m_pulseTick += Time.deltaTime * m_pulseSPeed;
			float num = XZSize.Evaluate(m_pulseTick);
			float y = YSize.Evaluate(m_pulseTick);
			base.transform.localScale = new Vector3(num, y, num);
			if (m_pulseTick >= 1f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
