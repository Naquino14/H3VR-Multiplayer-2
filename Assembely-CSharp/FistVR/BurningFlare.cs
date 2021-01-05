using UnityEngine;

namespace FistVR
{
	public class BurningFlare : MonoBehaviour
	{
		public Color FlareColor;

		public float Life;

		private float m_lifeLeft;

		public AnimationCurve IntensityOverTimeCurve;

		public AnimationCurve RadiusOverTimeCurve;

		private float m_timeTilFlicker;

		private void Awake()
		{
			m_lifeLeft = Life;
		}

		private void Update()
		{
			m_lifeLeft -= Time.deltaTime;
			if (!(m_lifeLeft > 0f))
			{
				return;
			}
			m_timeTilFlicker -= Time.deltaTime;
			if (m_timeTilFlicker <= 0f)
			{
				m_timeTilFlicker = Random.Range(0.03f, 0.1f);
				if (GM.CurrentSceneSettings.IsSceneLowLight)
				{
					FXM.InitiateMuzzleFlash(base.transform.position + Vector3.up * 0.1f, base.transform.forward, IntensityOverTimeCurve.Evaluate(Life - m_lifeLeft) * 12f, FlareColor, RadiusOverTimeCurve.Evaluate(Life - m_lifeLeft) * 80f);
				}
				else
				{
					FXM.InitiateMuzzleFlashLowPriority(base.transform.position + Vector3.up * 0.1f, base.transform.forward, IntensityOverTimeCurve.Evaluate(Life - m_lifeLeft) * 0.3f, FlareColor, RadiusOverTimeCurve.Evaluate(Life - m_lifeLeft) * 0.3f);
				}
			}
		}
	}
}
